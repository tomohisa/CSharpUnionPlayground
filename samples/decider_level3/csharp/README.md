# C#：closedイベントをジェネリックDCBへ渡す

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

## この設計でよかったこと

### 1. ライブラリ化しても閉じたEventを保てる

出典：[Handlers.cs](Handlers.cs#L1-L16)

```csharp
using System.Collections.Immutable;
using System.Globalization;
using Playground.Dcb;
namespace SchoolDomain;

public sealed class SchoolHandler : ICommandHandler<Command,Event,Rejection>
{
    public ImmutableArray<Rejection> Validate(Command command)=>ValidatedCommand.Validate(command);
    public Query QueryFor(Command command)=>Routing.QueryFor(command);
    public Preparation<Event,Rejection> Handle(ImmutableArray<Recorded<Event>> events,Command command) =>
        Decider.Decide(Registers.Fold(events).DecisionModel(),ValidatedCommand.Create(command)) switch
        {
            Accepted a=>new Emit<Event,Rejection>(a.Events),
            Rejected r=>new Reject<Event,Rejection>(r.Code)
        };
}
```

共通のobjectやIEventPayloadへ変換しません。EmitがそのままEventの配列を持ち、学校のDeciderは元の派生recordをswitchできます。Counterという別の型でも同じDcb.csをビルドし、学校型への依存がないことを検証しています。

### 2. 具象ストアとメタ情報の登録を切り離せる

出典：[csharp/Dcb.cs](Dcb.cs#L22-L26)

```csharp
public interface ICommandStore<E>
{
    ReadResult<E> Read(Query query);
    AppendResult<E> Append(string commandId,Query condition,int after,ImmutableArray<E> batch);
}
```

出典：[csharp/Routing.cs](Routing.cs#L22-L25)

```csharp
    public static EventDescriptor Describe(Event e) => e switch
    {
        StudentRegistered x => new("StudentRegistered",[Student(x.StudentId)]),
        StudentPromoted x => new("StudentPromoted",[Student(x.StudentId)]),
```

CommandProcessorはinterfaceへ依存し、タグと型名は学校の網羅的な関数で決めます。新ドメインのケースを共通ライブラリへ追加する必要がありません。型の集合を閉じているのは学校側です。

### 3. メタ情報が増えてもSource GenerationでJSONを往復できる

出典：[csharp/Program.cs](Program.cs#L17-L21)

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase,AllowOutOfOrderMetadataProperties=true,
    RespectNullableAnnotations=true,RespectRequiredConstructorParameters=true)]
[JsonSerializable(typeof(Input))]
[JsonSerializable(typeof(RunOutput))]
internal partial class Wire:JsonSerializerContext;
```

Recorded<Event>、Operation、ネストした履修状態までルートから生成できます。独自コンバーターなしで、Native AOT・JSONリフレクション無効でも機能検証します。F#の今回は.NET 11の標準DU対応をJITで使う方式と異なります。

## 実装で大変だったこと・残る制約

### 1. Tagのrecordだけでは構造的な等価性にならなかった

出典：[csharp/Dcb.cs](Dcb.cs#L6-L8)

```csharp
    public bool Equals(Tag? other) => other is not null && Category == other.Category && Keys.SequenceEqual(other.Keys);
    public override int GetHashCode() { var hash=new HashCode();hash.Add(Category);foreach(var key in Keys)hash.Add(key);return hash.ToHashCode(); }
    public bool Same(Tag other) => Equals(other);
```

ImmutableArrayをそのままrecordの既定比較へ任せず、Keysの値比較とHashCodeを揃えました。F#のlistやJavaのListの値比較より明示する仕事が増えました。独立テストではHashSetでも同値になることを確認しています。

### 2. 分割すると、イベントに必要な事実とタグを設計し直す必要があった

出典：[csharp/Decider.cs](Decider.cs#L115-L122)

```csharp
    public static Decision Decide(DecisionModel state, ValidatedCommand validated)
    {
        var command=validated.Value;
        var decision = DecideOne(state, command);
        if (decision is Accepted a && a.Events[0] is ClassStarted e)
            return new Accepted([e, .. state.Enrollments.Where(r => r.ClassId == e.ClassId && r.Progress is Reserved).Select(r => (Event)new StudyStarted(e.ClassId, r.StudentId))]);
        return decision;
    }
```

授業開始でクラスと各生徒のStudyStartedを原子的に保存します。StudentEnrolledのcourseやClassCompletedのresultsも、他のモデルを参照せず復元するための情報です。学校全体のStateを渡していたLevel 2では隠れていた設計負担です。

### 3. 抽出可能な同期APIと、本番用の非同期基盤は別

出典：[csharp/Dcb.cs](Dcb.cs#L59-L60)

```csharp
    private static bool Notify(Func<ImmutableArray<Recorded<T>>,bool> listener,ImmutableArray<Recorded<T>> batch)
    { try { return listener(batch); } catch(Exception) { return false; } }
```

購読例外を追記失敗と混同しないよう分離しましたが、購読はロック内・非再入・短い消費処理という契約です。I/Oや再接続、永続的な冪等性はありません。公開されたモデルやenumの不正値を型だけですべて防ぐ設計でもありません。

## コマンド・検証・履歴復元の更新

プロパティのRequired・EnumDataType・独自の値制約属性をMicrosoft.Extensions.Optionsの標準Source Generatorへ渡します。Validator.TryValidateObjectのリフレクション走査は使いません。生成した検証とJSON Source Generationの両方をNative AOTで実行します。privateコンストラクターのrecord classにしたため、structのdefaultによる未検証値はありません。属性・生成validatorの宣言量はF#やZodより増えます。

[Handlers.cs](Handlers.cs)は入力検証とDecider接続、[ValueObjects.cs](ValueObjects.cs)は値の制約と検証済み入力、[DomainTests.cs](DomainTests.cs)はドメインテストです。Routingはタグ・読み取り条件・イベントのメタ情報を担当し、業務ハンドラーとは別分類です。

現在のポリシーで値を検証するcreate系factoryと、過去の事実を再検証しないfromEvent／restore系factoryを区別します。イベント・JSONの保存形と一覧DTOはプリミティブを保ち、投影で復元した値を取り出して使います。全てのStateの全フィールドを名目的なVOへ置き換えた実装ではありません。ValidatedCommandも永続化せず、新しいコマンドを受ける境界だけに使います。

76ケースのGWTではExecutor経由の結果と正確なイベントpayload、拒否時の無追記、投影と再構築の一致を確認します。これとは別に、ストアを使わないDeciderの3例、VOのfactoryと履歴復元、最後の1席の競合再判断もテストします。言語固有の不正入力ケースも追加しています。テスト全体をコード量の別枠へ計上しています。
