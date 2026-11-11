# C#：closed階層を組み合わせる履修モデル

[共通仕様](../README.md)／[コード量](../README.md#コード量の比較)。対象はこの履修ドメインの実装です。言語一般の順位や、測っていない速度の比較ではありません。引用は現在のソースとCIで照合します。

## この複雑さでよかったこと

### 1. 未確定の点数を置かず、状態ごとのデータを列挙できる

出典：[csharp/Models.cs](Models.cs#L33-L38)

```csharp
public closed record Progress;
public sealed record Reserved() : Progress;
public sealed record Studying(int Attendance) : Progress;
public sealed record Passed(int Score, int Credits) : Progress;
public sealed record Failed(int Score, FailureReason Reason) : Progress;
public sealed record Withdrawn(WithdrawalReason Reason) : Progress;
```

Studyingには出席数、Passedには点数と単位、Failedには点数と理由を持たせました。未採点のscoreや不合格でない理由をnullで埋めません。[fsharp/Models.fs](../fsharp/Models.fs#L52)のDUと同じ状態の集合を、ケース別recordで表しています。

### 2. 新しい履修状態を表示処理の対応漏れとして検出できる

出典：[csharp/Decider.cs](Decider.cs#L16-L21)

```csharp
    public static string Describe(Progress progress) => progress switch
    {
        Reserved => "reserved", Studying p => $"studying:{p.Attendance}",
        Passed p => $"passed:{p.Score}:{p.Credits}", Failed p => $"failed:{p.Score}:{p.Reason}",
        Withdrawn p => $"withdrawn:{p.Reason}"
    };
```

履修データからそのケースでしか持たない項目を直接取り出せます。CS8509をエラーにしたビルドで、PendingReviewを追加する実験が未対応switchを拒否することも検証します。[go/domain/decider.go](../go/domain/decider.go#L386)の実行時defaultとは検出時点が異なります。

### 3. ネストが増えてもJSONケースの登録一覧は増えない

出典：[csharp/Program.cs](Program.cs#L30-L34)

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    AllowOutOfOrderMetadataProperties = true, RespectNullableAnnotations = true, RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(Input))]
[JsonSerializable(typeof(Output))]
internal partial class Wire : JsonSerializerContext;
```

Input／Outputだけの登録で、State→ClassState→Plan→Prerequisite、Enrollment→Progressまでたどれます。標準STJで独自コンバーターを作らず、Native AOTで全シナリオを往復します。F#の今回はJIT＋PGOという実行条件との違いもあります。

## 実装で増えた負担・残る制約

### 1. nullを返さない検索にした分、分岐の囲いが増えた

出典：[csharp/Decider.cs](Decider.cs#L64-L67)

```csharp
                    foreach (var student in state.Students.Where(s => s.StudentId == c.StudentId))
                    {
                        if (room is not (Available or Full)) return Reject(Rejection.RegistrationClosed);
                        if (Members(room).Contains(c.StudentId)) return Reject(Rejection.AlreadyEnrolled);
```

検索結果のnullableなStudentやRosterを返す代わりに、見つかった場合の判断とUnknownStudentを分けています。[rust/src/decider.rs](../rust/src/decider.rs#L25)のOption→Result変換や[swift/decider.swift](../swift/decider.swift#L27)のguardと比べて、今回のforeachによる記述は長くなります。C#の必須の書き方ではなく、この版の選択です。

### 2. 複数のコレクションの更新はwithだけでは片付かない

出典：[csharp/Decider.cs](Decider.cs#L153-L158)

```csharp
            case StudentEnrolled x:
                var members = Members(room).Add(x.StudentId);
                updated = members.Length == plan.Capacity ? new Full(plan, members) : new Available(plan, members);
                records = [.. records.Where(v => v.ClassId != id || v.StudentId != x.StudentId), new Enrollment(id, x.StudentId, plan.Course, new Reserved())];
                break;
            case StudentDropped x:
```

席数の更新と履修のReserved化を同時に行うため、ClassesとEnrollmentsを別々に再構築します。withは未変更フィールドの繰り返しを減らしますが、検索や配列のコピーは残ります。この学校全体の配列モデルを巨大なイベントストア向けの設計とはしていません。

### 3. closedと非nullable注釈だけで業務条件を証明できない

出典：[csharp/Models.cs](Models.cs#L24-L24)

```csharp
public sealed record ClassResult(string StudentId, Progress Progress);
```

ClassResult.Progressを型だけでPassed／Failedに絞ってはいません。外部から不整合なrecordを直接作れますし、参照型に対するnullも言語上消えません。JSON境界と検証したイベントの適用経路で守る設計です。Grade／Courseなどのenumにも未定義の数値を持てる余地があります。より厳しい構築制約は未実装です。
