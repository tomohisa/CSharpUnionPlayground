# F#：DU・不変Map・関数注入で共通DCBを使う

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

この版は.NET 11 Release／JIT＋Dynamic PGO前提です。WASM・Native AOTは対象にしません。

## この設計でよかったこと

### 1. 共通部品を挟んでも、ドメインDUをそのまま扱える

出典：[fsharp/Dcb.fs](Dcb.fs#L73-L75)

```fsharp
type Preparation<'E,'R> = Emit of 'E list | Reject of 'R
type Ticket<'C,'E,'R> = { CommandId: string; Command: 'C; Query: Query; After: int; Returned:int; Decision: Preparation<'E,'R> }
type HandlingResult<'R> = { Status: string; Conflicts: int; Appended: int; Position: int; Errors: 'R list }
```

出典：[Handlers.fs](Handlers.fs#L1-L13)

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

出典：[fsharp/Projection.fs](Projection.fs#L4-L8)

```fsharp
type Registers = { Students: Map<string,Student>; Classes: Map<string,ClassState>; Enrollments: Map<string*string,Enrollment> }
module StudentRegister =
    let apply students = function
        | Event.StudentRegistered(id,g) | Event.StudentPromoted(id,g) -> Map.add id { StudentId=id;Grade=g } students
        | _ -> students
```

学籍・クラス・履修の更新を別々に定義し、必要なタグ履歴だけで一時的なDecisionModelを組み立てます。投影のバッチ適用では古いMapを変更せず、状態と位置を一緒に差し替えられます。GoのsliceやTypeScriptのMapのコピー契約と比べ、今回のidentityコピーの根拠が明確です。

### 3. ストア差し替えとタグ規則を型付き関数で表せる

出典：[fsharp/Dcb.fs](Dcb.fs#L16-L18)

```fsharp
type ICommandStore<'E> =
    abstract Read: Query -> ReadResult<'E>
    abstract Append: string * Query * int * 'E list -> AppendResult<'E>
```

出典：[fsharp/Routing.fs](Routing.fs#L8-L11)

```fsharp
    let queryFor = function
        | Command.RegisterStudent(studentId,grade) -> one [student studentId]
        | Command.PromoteStudent(studentId,grade) -> one [student studentId]
        | Command.OpenClass(classId,course,capacity) -> one [classroom classId]
```

ライブラリは学校のケースを知りません。interfaceと関数の引数で変わる部分を表せます。別ドメインを追加するときに、ライブラリ側のEvent unionや型登録一覧を更新しません。

## 実装で大変だったこと・残る制約

### 1. JSONの表現差はライブラリ化しても消えない

出典：[fsharp/Program.fs](Program.fs#L9-L14)

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

出典：[fsharp/Dcb.fs](Dcb.fs#L26-L26)

```fsharp
    let copy (e: Recorded<'E>) = { e with Payload = copyPayload e.Payload }
```

出典：[fsharp/Projection.fs](Projection.fs#L52-L52)

```fsharp
    let decisionModel state: DecisionModel =
```

Recorded、ReadResult、Ticket、学校のレジストリで似たフィールド名が増えました。型を明示して共通部品とドメインの境界を分かるようにしています。短い記法だけを優先すると、このあたりの推論が読みにくくなります。

### 3. 競合再試行の関数が正しくても、投影配信の失敗契約は別に必要

出典：[fsharp/Dcb.fs](Dcb.fs#L27-L28)

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
