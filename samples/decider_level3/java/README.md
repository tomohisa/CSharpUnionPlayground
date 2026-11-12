# Java：公開generic portとsealedな学校イベントを組み合わせる

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

## この設計でよかったこと

### 1. 公開ライブラリが学校のsealed interfaceを知らずに済む

出典：[java/playground/dcb/Dcb.java](playground/dcb/Dcb.java#L22-L25)

```java
    public interface CommandStore<E> {
        ReadResult<E> read(Query query);
        AppendResult<E> append(String commandId,Query condition,int after,List<E> batch);
    }
```

出典：[SchoolHandler.java](SchoolHandler.java#L1-L11)

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

出典：[java/Main.java](Main.java#L6-L6)

```java
    record Input(List<Dcb.Recorded<Event>> history,List<Operation> operations){public Input{history=List.copyOf(history);operations=List.copyOf(operations);}}
```

Recorded<Event>のpayloadを学校のポリモーフィズムとして読むことで、汎用Objectから手動キャストしていません。Jacksonは学校側の依存で、Dcb.java自体にはありません。

### 3. スナップショットの状態と位置をひとつの値にできる

出典：[java/playground/dcb/Dcb.java](playground/dcb/Dcb.java#L62-L63)

```java
    public record Snapshot<S>(int position,S state) {}
    public static final class LiveProjection<E,S> {
```

出典：[java/playground/dcb/Dcb.java](playground/dcb/Dcb.java#L77-L77)

```java
        public synchronized Snapshot<S> snapshot() { return new Snapshot<>(checkpoint,copyState.apply(state)); }
```

レビュー前は状態と位置を別々に取得していたため、間に更新が入る余地がありました。同じsynchronizedで返すSnapshotへ変更し、他言語と同じ契約に揃えました。

## 実装で大変だったこと・残る制約

### 1. ライブラリ型とドメイン型のJSON登録は別の仕事

出典：[java/Models.java](Models.java#L224-L225)

```java
sealed interface Event permits StudentRegistered, StudentPromoted, ClassOpened, StudentEnrolled, StudentDropped, ClassStarted, AttendanceRecorded, ResultRecorded, ClassCompleted, StudyStarted {}
record StudentRegistered(String studentId, Grade grade) implements Event {
```

Eを保持しても、学校のpermitsとJsonSubTypesのケース一覧は引き続き保守します。F#のDUやTypeScriptのZod宣言に比べ、増えたイベントとOperationに対する記述が多くなりました。

### 2. 不変Mapの更新でも再構築が多い

出典：[java/Projection.java](Projection.java#L51-L52)

```java
    static Registers reduce(Registers s,Event e){return new Registers(student(s.students(),e),classroom(s.classes(),e),enrollment(s.enrollments(),e));}
    static Registers fold(List<Dcb.Recorded<Event>> events){var s=Registers.empty();for(var e:events)s=reduce(s,e.payload());return s;}
```

mutableなローカルコピーから、Map.copyOfを持つ新しいRegistersを返します。C#／F#の不変Map更新より明示的な構築が増えました。このコストをコード量から実行速度の順位へ読み替えてはいません。

### 3. Javaの型消去はあるが、payloadをObjectへ崩す必要はない

出典：[java/playground/dcb/Dcb.java](playground/dcb/Dcb.java#L80-L82)

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
