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

## C#とF#を詳しく比較する

今回の条件では、新しく履修ドメインを書くならF#を第一候補にします。理由は行数よりも、状態別データ・欠損・不変更新を一貫した記法で扱いやすいことです。ただし、JSONを含む全体ではC#もかなり近く、既存のC#／Sekibanへ組み込むならclosed recordを選ぶ判断も合理的です。これは今回の実装を比較した評価で、言語一般の順位ではありません。

| 観点 | 今回のC# | 今回のF# |
|---|---|---|
| 状態別データ | closed record＋ケース別record | 関連データ付きDUでまとめて宣言 |
| 網羅性 | switch式の未対応警告をエラーにする | matchの未対応警告をエラーにする |
| 学年・科目・拒否の集合 | enum。未定義の数値を構築できる余地がある | DU。通常の構築では宣言したケースを使う |
| 検索の欠損 | 今回は検索・存在確認と拒否を明示 | OptionのNone／Someをmatchして拒否へ変換 |
| 不変更新 | recordのwith＋ImmutableArray | record更新＋不変list |
| ケースだけを受け取るAPI | 派生recordを引数型として指定できる | DUケース自体は独立した引数型にならない |
| JSON | 標準closed推論＋ルートだけのSource Generation | .NET 11標準DU対応＋入力境界のnull検査 |
| 実行条件 | Release JITに加えNative AOTも検証 | .NET 11 Release JIT＋Dynamic PGO。WASM・AOTは今回の対象外 |

### 状態を宣言する記法と、型で守れる範囲

C#のProgress定義は上に引用したとおりです。対応するF#は次の形で、ケースを一つの宣言として読めます。

出典：[fsharp/Models.fs](../fsharp/Models.fs#L52-L57)

```fsharp
type Progress =
    | [<JsonPropertyName("Reserved")>] Reserved
    | [<JsonPropertyName("Studying")>] Studying of attendance: int
    | [<JsonPropertyName("Passed")>] Passed of score: int * credits: int
    | [<JsonPropertyName("Failed")>] Failed of score: int * reason: FailureReason
    | [<JsonPropertyName("Withdrawn")>] Withdrawn of reason: WithdrawalReason
```

Studyingに未確定の点数を持たせず、Passedには単位、Failedには不合格理由を持たせられる点は両方同じです。ケース追加実験でも、対象の網羅的な表示分岐は両方で未対応を検出しました。defaultやワイルドカードで残りを受ける分岐まで、自動的に対応漏れを防ぐという意味ではありません。

F#の利点は、学年・科目・拒否理由も同じDUの仕組みで表せることです。今回C#ではそれらをenumにしており、たとえば(Grade)999という値も構築できます。C#でも学年をclosed recordにすれば近づけられますが、モデルの記述を追加することになります。

一方、両方とも「業務上の不正値をすべて型で排除した」実装ではありません。

- 0〜100という点数の範囲、60点という合格条件は実行時の検証です。
- Bの履修前にAへ合格していること、定員と名簿の整合性はStateを使って判断します。
- ClassResult.Progressは型だけではPassed／Failedに限定されません。
- 公開されたモデルから、負の点数を持つPassedや不整合なFullを直接構築できる余地があります。

F#のDUにしただけで、これらの条件が証明されるわけではありません。さらに型で守るなら、ScoreやPassingResultの検証済みの値を作る関数、非公開の構築方法などを加える設計になります。今回のサンプルではそこまで実装していません。

### C#ではケース自体を引数型にできる

C#のPassedは独立したrecord型なので、合格結果だけを受け取る関数を次のように宣言できます。これは現在のソースにある関数ではなく、設計上の例です。

```csharp
static int AwardCredits(Passed result) => result.Credits;
```

F#のPassedはProgressを構築するケースで、独立した型ではありません。同じ目的ならPassingResultというrecordを作り、Passed of PassingResultのように関連データへ切り出せます。ケース単位の処理を別のサービスやAPIへ渡すときには、C#のケース別recordにも利点があります。

### 検索結果の欠損と、複数コレクションの更新

F#では検索の欠損をOptionで受け、その場でUnknownStudentへ変換しています。

出典：[fsharp/Decider.fs](../fsharp/Decider.fs#L30-L33)

```fsharp
                match state.Students |> List.tryFind (fun s -> s.StudentId = studentId) with
                | None -> reject UnknownStudent
                | Some student ->
                    let eligible = match plan.Prerequisite with
```

Noneは一時的な検索結果です。StudentやProgressの各フィールドを欠損可能にしたものではありません。[C#版の検索](Decider.cs#L64)も同じ目的ですが、今回のforeachとreturnの組合せよりF#の方が意図を直接表せています。C#でこのforeachが必須という意味ではなく、別の検索APIや結果型を使う設計もできます。

不変更新では差は小さく、C#のwithもF#のrecord更新も未変更フィールドの再指定を減らせました。どちらも、生徒・クラス・履修のコレクションを検索して更新する仕事は残ります。F#のlistへの末尾追加も走査を伴い、短いコードだからコピーや検索のコストが小さいとは言えません。

### JSONは今回のC#が扱いやすかった

C#はInput／OutputをContextに登録すれば、ネストしたclosed型を標準Generatorがたどります。履修ケースを追加するたびに個別のJsonDerivedType一覧やコンバーターを更新する必要はありません。JSONリフレクションを無効にしたNative AOTでも同じ機能検証が通っています。

F#も.NET 11標準機能でJSONを往復できましたが、共通形式に揃えるために次の調整が入りました。

- ケース名を維持するJsonPropertyName属性。CamelCase設定はケース名にも作用します。
- フィールドなしDUが文字列になる表現差。比較時はprogress／prerequisiteだけを正規化します。
- 標準デシリアライザーが受理したnullを、入力のJSON木で拒否する処理。

出典：[fsharp/Program.fs](../fsharp/Program.fs#L18-L21)

```fsharp
    let readInput text =
        use document = JsonDocument.Parse(text: string)
        if containsNull document.RootElement then raise (JsonException("null is outside the input contract"))
        JsonSerializer.Deserialize<Input>(text, options) |> nonNull
```

[.NET 11の公式説明](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-11/libraries#systemtextjson-improvements)でも、標準F# DU対応はリフレクション方式とされています。今回のJIT＋PGO前提では使えますが、C#のSource Generationと同じ方式ではありません。

F#だけで完結するシステムなら、他言語とのタグ形式の統一に必要だった属性や比較用の正規化は減らせます。それでも外部JSONをドメインへ入れる境界の検証は必要です。通常のF#のDU構築と、.NET APIやデシリアライズ経由の入力は区別して扱います。

### コード量と今回の選択

[共通の集計表](../README.md#コード量の比較)では、F#のDeciderは行数が短い一方、空白以外の文字数の差は小さくなっています。全体の文字数ではC#の方が少なく、F#のJSON属性・境界処理も集計に含めています。「F#は短いからよい」という結論にはしません。

今回のJIT前提でドメインを新しく組み立てるなら、DUとOptionを一貫して使えるF#を選びたいです。既存のC#／Sekibanへ組み込む場合は、closed recordで重要な状態表現と網羅性を得られるC#が有力です。C#を選ぶと今回のドメイン表現を大きく諦めなければならない、という比較結果ではありません。速度を測って選んだ結論でもありません。

## このStateと、イベントストアの集約・DCBの違い

出典：[csharp/Models.cs](Models.cs#L25-L25)

```csharp
public sealed record State(ImmutableArray<Student> Students, ImmutableArray<ClassState> Classes, ImmutableArray<Enrollment> Enrollments);
```

このStateには、生徒全員・全クラス・全履修が入ります。Input.Historyの1本の履歴をReplayして復元し、コマンドを順番に処理します。Enrollの判断時は、同じStateのStudentsから学年、EnrollmentsからAの合格実績、ClassesからBの空席を参照します。

その意味では「学校全体を1つの集約にしたような、メモリ上の整合性境界」です。ただし永続ストアはなく、集約IDでストリームを分割する実装や、ストリームの期待バージョン付き追記もありません。StudentId／ClassIdはイベントのデータ項目で、DCBタグではありません。タグクエリ、読み取り位置、条件付きの原子的追記、競合時の再読込・再判断も含みません。

イベントからStateを復元するESの中核処理はありますが、集約の永続化・同時実行制御まで備えたES実装ではありません。具体例と、DCBへ拡張する場合の違いは[共通README](../README.md#学校全体を1つのstateに入れるとは)に記載しています。
