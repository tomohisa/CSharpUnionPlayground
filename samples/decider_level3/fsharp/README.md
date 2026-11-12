# F#：DU・不変Map・関数注入で共通DCBを使う

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。このページでは記述を比較し、速度・メモリは[ベンチマーク](../benchmark/README.md)で比較します。

この版は.NET 11 Release／JIT＋Dynamic PGO前提です。WASM・Native AOTは対象にしません。

## この設計でよかったこと

### 1. 共通部品を挟んでも、ドメインDUをそのまま扱える

出典：[Dcb.fs:73–75](Dcb.fs#L73-L75)

```fsharp
type Preparation<'E,'R> = Emit of 'E list | Reject of 'R
type Ticket<'C,'E,'R> = { CommandId: string; Command: 'C; Query: Query; After: int; Returned:int; Decision: Preparation<'E,'R> }
type HandlingResult<'R> = { Status: string; Conflicts: int; Appended: int; Position: int; Errors: 'R list }
```

出典：[Handlers.fs:1–13](Handlers.fs#L1-L13)

```fsharp
namespace SchoolDomain
open Playground.Dcb
module Handlers =
    let school:CommandHandler<Command,Event,Rejection> = {
        Validate=ValidatedCommand.validate
        QueryFor=Routing.queryFor
        Handle=fun events command ->
            match ValidatedCommand.create command with
            | Error error -> Reject error
            | Ok validated ->
                match Decider.decide (Registers.decisionModel (Registers.fold events)) validated with
                | Decision.Accepted events -> Emit events
                | Decision.Rejected code -> Reject code }
```

共通部品のEmitに入るのは学校のEvent listです。汎用イベントを受けてダウンキャストする処理はなく、ProgressやEventのmatchはドメイン側に残ります。カウンターのDUではない独立したrecordでも同じライブラリを利用できます。

### 2. 分かれた状態を不変Mapで更新できる

出典：[Projection.fs:4–8](Projection.fs#L4-L8)

```fsharp
type Registers = { Students: Map<string,Student>; Classes: Map<string,ClassState>; Enrollments: Map<string*string,Enrollment> }
module StudentRegister =
    let apply students = function
        | Event.StudentRegistered(id,g) | Event.StudentPromoted(id,g) -> Map.add id { StudentId=id;Grade=g } students
        | _ -> students
```

学籍・クラス・履修の更新を別々に定義し、必要なタグ履歴だけで一時的なDecisionModelを組み立てます。投影のバッチ適用では古いMapを変更せず、状態と位置を一緒に差し替えられます。GoのsliceやTypeScriptのMapのコピー契約と比べ、今回のidentityコピーの根拠が明確です。

### 3. ストア差し替えとタグ規則を型付き関数で表せる

出典：[Dcb.fs:16–18](Dcb.fs#L16-L18)

```fsharp
type ICommandStore<'E> =
    abstract Read: Query -> ReadResult<'E>
    abstract Append: string * Query * int * 'E list -> AppendResult<'E>
```

出典：[Routing.fs:8–11](Routing.fs#L8-L11)

```fsharp
    let queryFor = function
        | Command.RegisterStudent(studentId,grade) -> one [student studentId]
        | Command.PromoteStudent(studentId,grade) -> one [student studentId]
        | Command.OpenClass(classId,course,capacity) -> one [classroom classId]
```

ライブラリは学校のケースを知りません。interfaceと関数の引数で変わる部分を表せます。別ドメインを追加するときに、ライブラリ側のEvent unionや型登録一覧を更新しません。

## 実装で大変だったこと・残る制約

### 1. JSONの表現差はライブラリ化しても消えない

出典：[Program.fs:9–14](Program.fs#L9-L14)

```fsharp
    let rec containsNull (e:JsonElement) =
        match e.ValueKind with
        | JsonValueKind.Null -> true
        | JsonValueKind.Array -> e.EnumerateArray() |> Seq.exists containsNull
        | JsonValueKind.Object -> e.EnumerateObject() |> Seq.exists(fun p -> containsNull p.Value)
        | _ -> false
```

外部JSONのnullを拒否し、型付きイベントへ入れます。.NET 11標準DU対応のフィールドなしケースは文字列なので、共通テストはその意味を正規化します。C#のSource Generationと同じ方式ではなく、共通形式のためのケース名属性も必要でした。

### 2. 同じ名前のrecordが増え、型注釈が重要になった

出典：[Dcb.fs:26–26](Dcb.fs#L26-L26)

```fsharp
    let copy (e: Recorded<'E>) = { e with Payload = copyPayload e.Payload }
```

出典：[Projection.fs:52–52](Projection.fs#L52-L52)

```fsharp
    let decisionModel state: DecisionModel =
```

Recorded、ReadResult、Ticket、学校のレジストリで似たフィールド名が増えました。型を明示して共通部品とドメインの境界を分かるようにしています。短い記法だけを優先すると、このあたりの推論が読みにくくなります。

### 3. 競合再試行の関数が正しくても、投影配信の失敗契約は別に必要

出典：[Dcb.fs:27–28](Dcb.fs#L27-L28)

```fsharp
    let notify listener events = try listener events with _ -> false
    let valid (e:Recorded<'E>) = let d=describe e.Payload in d.EventType=e.EventType && d.Tags.Length=e.Tags.Length && d.Tags |> List.forall(fun t->List.contains t e.Tags)
```

購読失敗でコミットの事実を失敗へ戻さず、障害数を持って配信を止める設計を加えました。不変データだけでこの運用上の問題が解消するわけではありません。購読は同期・非再入で、イベントバスのようなI/O処理は対象外です。

## コマンド・検証・履歴復元の更新

privateな単一ケースDUとResultで、値の生成成功／拒否を表現します。Handlerは関数のrecordなのでクラス階層を増やさずExecutorへ渡せます。ValidatedCommandのprivateケースを通した入力だけをDeciderへ渡します。CLR経由のnull注入まで型だけでなくせるわけではありません。今回はJIT＋PGOで検証します。

[Handlers.fs](Handlers.fs)は入力検証とDecider接続、[ValueObjects.fs](ValueObjects.fs)は値の制約と検証済み入力、[DomainTests.fs](DomainTests.fs)はドメインテストです。Routingはタグ・読み取り条件・イベントのメタ情報を担当し、業務ハンドラーとは別分類です。

現在のポリシーで値を検証するcreate系factoryと、過去の事実を再検証しないfromEvent／restore系factoryを区別します。イベント・JSONの保存形と一覧DTOはプリミティブを保ち、投影で復元した値を取り出して使います。全てのStateの全フィールドを名目的なVOへ置き換えた実装ではありません。ValidatedCommandも永続化せず、新しいコマンドを受ける境界だけに使います。

76ケースのGWTではExecutor経由の結果と正確なイベントpayload、拒否時の無追記、投影と再構築の一致を確認します。これとは別に、ストアを使わないDeciderの3例、VOのfactoryと履歴復元、最後の1席の競合再判断もテストします。言語固有の不正入力ケースも追加しています。テスト全体をコード量の別枠へ計上しています。

## 大きな一覧Projectionの更新

学校全体のライブ一覧は[SchoolProjection.fs](SchoolProjection.fs)へ分離しました。既存のProjectionにある不変MapやコピーするReducerは、タグで絞ったコマンド判断用モデルと参照再生に残しています。ライブ一覧の更新には使いません。

ライブ一覧は汎用IndexedProjectionのハッシュ索引を使い、バッチ中に触った行だけを差分へ積み、全イベントを処理できてから反映します。クラスの名簿はイベント位置を内部ノードIDにした連結索引です。追加・取消で名簿全体をコピーせず、前後のノードだけを更新します。削除したノードは実際に索引から削除するため、同じ生徒の取消と再登録を繰り返してもノードを蓄積しません。

一覧要求時だけ名簿の配列／listと並び順を持つDTOを組み立てます。過去のDTOはその後のイベントで変わりません。全件Snapshotは出力サイズに比例する仕事があり、ロック中の読み取りもあるため、大規模な並行クエリまで一定時間という保証ではありません。頻繁な大量一覧の提供には、ページングや公開用Snapshotの別管理が必要です。

[ProjectionTests.fs](ProjectionTests.fs)では1千・1万・10万件を作り、更新と名簿拡大の時間を全件Snapshotと分けて測ります。途中失敗・位置の欠落で部分更新しないこと、再配送、過去Snapshotの不変性、取消と再登録後の順序と行数も検証します。[ローカルの測定値](../observed/projection-scaling.md)は言語の総合性能順位ではなく、この更新経路のサイズ依存を見るためのものです。

## JSON／SQLiteと計測

Microsoft.Data.Sqlite。codecを関数で注入し、学校のDUと別の文書レコードも同じライブラリで扱います。実行は.NET 11のJIT＋PGOを前提にしています。

[共通ベンチマーク](../benchmark/README.md)では、履修コマンド・拒否・競合後の再判断、索引Projectionのクエリ、JSON応答、再起動を同じ件数で実行します。SQLite版とメモリJSON版の両方でシリアライズ・デシリアライズを通します。インフラ障害は業務のRejectedと混ぜず、プロセスを失敗させます。外部接続のイベントは明示的なRefreshまたは次の自分のcommitで取り込みます。
