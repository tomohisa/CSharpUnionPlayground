# Java：公開generic portとsealedな学校イベントを組み合わせる

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。このページでは記述を比較し、速度・メモリは[ベンチマーク](../benchmark/README.md)で比較します。

## この設計でよかったこと

### 1. 公開ライブラリが学校のsealed interfaceを知らずに済む

出典：[playground/dcb/Dcb.java:23–26](playground/dcb/Dcb.java#L23-L26)

```java
    public interface CommandStore<E> {
        ReadResult<E> read(Query query);
        AppendResult<E> append(String commandId,Query condition,int after,List<E> batch);
    }
```

出典：[SchoolHandler.java:1–11](SchoolHandler.java#L1-L11)

```java
import playground.dcb.Dcb;
import java.util.List;
final class SchoolHandler implements Dcb.CommandHandler<Command,Event,Rejection> {
    public List<Rejection> validate(Command command){return ValidatedCommand.validate(command);}
    public Dcb.Query queryFor(Command command){return Routing.queryFor(command);}
    public Dcb.Preparation<Event,Rejection> handle(List<Dcb.Recorded<Event>> events,Command command){
        return switch(Decider.decide(Projection.fold(events).decisionModel(),ValidatedCommand.create(command))){
            case Accepted a->new Dcb.Emit<>(a.events()); case Rejected r->new Dcb.Reject<>(r.code());
        };
    }
}
```

ライブラリはplayground.dcbパッケージです。学校側のEventをEへ渡すため、学校のコードではケース別recordとdefaultなしのswitchを使えます。カウンターの独立ビルドは別パッケージから同じAPIを使います。

### 2. JSONのgenericエンベロープにも元のEvent型を指定できる

出典：[Main.java:6–6](Main.java#L6-L6)

```java
    record Input(List<Dcb.Recorded<Event>> history,List<Operation> operations){public Input{history=List.copyOf(history);operations=List.copyOf(operations);}}
```

Recorded<Event>のpayloadを学校のポリモーフィズムとして読むことで、汎用Objectから手動キャストしていません。Jacksonは学校側の依存で、Dcb.java自体にはありません。

### 3. スナップショットの状態と位置をひとつの値にできる

出典：[playground/dcb/Dcb.java:63–64](playground/dcb/Dcb.java#L63-L64)

```java
    public record Snapshot<S>(int position,S state) {}
    public static final class LiveProjection<E,S> {
```

出典：[playground/dcb/Dcb.java:78–78](playground/dcb/Dcb.java#L78-L78)

```java
        public synchronized Snapshot<S> snapshot() { return new Snapshot<>(checkpoint,copyState.apply(state)); }
```

レビュー前は状態と位置を別々に取得していたため、間に更新が入る余地がありました。同じsynchronizedで返すSnapshotへ変更し、他言語と同じ契約に揃えました。

## 実装で大変だったこと・残る制約

### 1. ライブラリ型とドメイン型のJSON登録は別の仕事

出典：[Models.java:224–225](Models.java#L224-L225)

```java
sealed interface Event permits StudentRegistered, StudentPromoted, ClassOpened, StudentEnrolled, StudentDropped, ClassStarted, AttendanceRecorded, ResultRecorded, ClassCompleted, StudyStarted {}
record StudentRegistered(String studentId, Grade grade) implements Event {
```

Eを保持しても、学校のpermitsとJsonSubTypesのケース一覧は引き続き保守します。F#のDUやTypeScriptのZod宣言に比べ、増えたイベントとOperationに対する記述が多くなりました。

### 2. 不変Mapの更新でも再構築が多い

出典：[Projection.java:51–52](Projection.java#L51-L52)

```java
    static Registers reduce(Registers s,Event e){return new Registers(student(s.students(),e),classroom(s.classes(),e),enrollment(s.enrollments(),e));}
    static Registers fold(List<Dcb.Recorded<Event>> events){var s=Registers.empty();for(var e:events)s=reduce(s,e.payload());return s;}
```

mutableなローカルコピーから、Map.copyOfを持つ新しいRegistersを返します。C#／F#の不変Map更新より明示的な構築が増えました。このコストをコード量から実行速度の順位へ読み替えてはいません。

### 3. Javaの型消去はあるが、payloadをObjectへ崩す必要はない

出典：[playground/dcb/Dcb.java:81–83](playground/dcb/Dcb.java#L81-L83)

```java
    public sealed interface Preparation<E,R> permits Emit, Reject {}
    public record Emit<E,R>(List<E> events) implements Preparation<E,R> { public Emit { events=List.copyOf(events); } }
    public record Reject<E,R>(R error) implements Preparation<E,R> {}
```

共通処理はEとRを保ち、型消去のある言語でも学校のイベント集合はドメイン側で閉じています。購読のRuntimeExceptionは追記結果から分離しますが、同期・非再入のAPIという制約は残ります。

## コマンド・検証・履歴復元の更新

ID・定員・点数・日付のfactoryがInputViolationを投げ、Handlerが型付きのRejectionへ変換します。Commandのrecordコンストラクターはnullを拒否します。検証済みコマンドはprivateコンストラクターで作成経路を限定します。外部のBean Validationは追加していません。値の不変性は保てますが、値型ごとのfactory・getterの宣言量は多くなります。

[SchoolHandler.java](SchoolHandler.java)は入力検証とDecider接続、[ValueObjects.java](ValueObjects.java)は値の制約と検証済み入力、[DomainTests.java](DomainTests.java)はドメインテストです。Routingはタグ・読み取り条件・イベントのメタ情報を担当し、業務ハンドラーとは別分類です。

現在のポリシーで値を検証するcreate系factoryと、過去の事実を再検証しないfromEvent／restore系factoryを区別します。イベント・JSONの保存形と一覧DTOはプリミティブを保ち、投影で復元した値を取り出して使います。全てのStateの全フィールドを名目的なVOへ置き換えた実装ではありません。ValidatedCommandも永続化せず、新しいコマンドを受ける境界だけに使います。

76ケースのGWTではExecutor経由の結果と正確なイベントpayload、拒否時の無追記、投影と再構築の一致を確認します。これとは別に、ストアを使わないDeciderの3例、VOのfactoryと履歴復元、最後の1席の競合再判断もテストします。言語固有の不正入力ケースも追加しています。テスト全体をコード量の別枠へ計上しています。

## 大きな一覧Projectionの更新

学校全体のライブ一覧は[SchoolProjection.java](SchoolProjection.java)へ分離しました。既存のProjectionにある不変MapやコピーするReducerは、タグで絞ったコマンド判断用モデルと参照再生に残しています。ライブ一覧の更新には使いません。

ライブ一覧は汎用IndexedProjectionのハッシュ索引を使い、バッチ中に触った行だけを差分へ積み、全イベントを処理できてから反映します。クラスの名簿はイベント位置を内部ノードIDにした連結索引です。追加・取消で名簿全体をコピーせず、前後のノードだけを更新します。削除したノードは実際に索引から削除するため、同じ生徒の取消と再登録を繰り返してもノードを蓄積しません。

一覧要求時だけ名簿の配列／listと並び順を持つDTOを組み立てます。過去のDTOはその後のイベントで変わりません。全件Snapshotは出力サイズに比例する仕事があり、ロック中の読み取りもあるため、大規模な並行クエリまで一定時間という保証ではありません。頻繁な大量一覧の提供には、ページングや公開用Snapshotの別管理が必要です。

[ProjectionTests.java](ProjectionTests.java)では1千・1万・10万件を作り、更新と名簿拡大の時間を全件Snapshotと分けて測ります。途中失敗・位置の欠落で部分更新しないこと、再配送、過去Snapshotの不変性、取消と再登録後の順序と行数も検証します。[ローカルの測定値](../observed/projection-scaling.md)は言語の総合性能順位ではなく、この更新経路のサイズ依存を見るためのものです。

## JSON／SQLiteと計測

sqlite-jdbc。ConnectionとPreparedStatementを再利用し、payloadのJSONはJacksonで復元します。JDBCドライバーのSQLiteを使用するため、他言語とエンジン版が一致するとは限りません。

[共通ベンチマーク](../benchmark/README.md)では、履修コマンド・拒否・競合後の再判断、索引Projectionのクエリ、JSON応答、再起動を同じ件数で実行します。SQLite版とメモリJSON版の両方でシリアライズ・デシリアライズを通します。インフラ障害は業務のRejectedと混ぜず、プロセスを失敗させます。外部接続のイベントは明示的なRefreshまたは次の自分のcommitで取り込みます。
