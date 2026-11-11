# F#：.NET 11のDUと不変リストで履修を表す

[共通仕様](../README.md)／[コード量](../README.md#コード量の比較)。対象はこの履修ドメインの実装です。言語一般の順位や、測っていない速度の比較ではありません。引用は現在のソースとCIで照合します。

この版は.NET 11のRelease／JIT＋Dynamic PGOで動かします。WASM・Native AOTは対象にしません。

## この複雑さでよかったこと

### 1. 状態別データと意味のある「条件なし」をDUで宣言できる

出典：[fsharp/Models.fs](Models.fs#L52-L57)

```fsharp
type Progress =
    | [<JsonPropertyName("Reserved")>] Reserved
    | [<JsonPropertyName("Studying")>] Studying of attendance: int
    | [<JsonPropertyName("Passed")>] Passed of score: int * credits: int
    | [<JsonPropertyName("Failed")>] Failed of score: int * reason: FailureReason
    | [<JsonPropertyName("Withdrawn")>] Withdrawn of reason: WithdrawalReason
```

出典：[fsharp/Models.fs](Models.fs#L43-L45)

```fsharp
type Prerequisite =
    | [<JsonPropertyName("NoPrerequisite")>] NoPrerequisite
    | [<JsonPropertyName("PassedCourse")>] PassedCourse of course: Course
```

空のケースはデータなし、必要なケースだけ関連データを持つので、score optionやcourse optionを各レコードにばらまく必要がありません。[csharp/Models.cs](../csharp/Models.cs#L33)のケース別recordより、同じ集合をひとつの宣言で追えます。

### 2. 検索で見つからない場合を業務上の拒否へ変換できる

出典：[fsharp/Decider.fs](Decider.fs#L30-L33)

```fsharp
                match state.Students |> List.tryFind (fun s -> s.StudentId = studentId) with
                | None -> reject UnknownStudent
                | Some student ->
                    let eligible = match plan.Prerequisite with
```

OptionのNoneは一時的な検索結果です。ドメインのStudentやProgressに欠損フィールドを持たせず、UnknownStudentへ変換しています。型名とフィールド名が重複するこのモデルでも、matchとrecord更新は読みやすく保てました。

### 3. レコードの差分更新とリストで過去のStateを保てる

出典：[fsharp/Decider.fs](Decider.fs#L133-L133)

```fsharp
            | Event.StudentPromoted(id, grade) -> { state with Students = state.Students |> List.map (fun s -> if s.StudentId = id then { s with Grade = grade } else s) }
```

State内のStudentだけを置き換え、元のレコードを変更しません。[java/Decider.java](../java/Decider.java#L100)でStateの全フィールドを指定する記述と比べ、変更箇所が短くなります。describeのケース追加漏れはFS0025をエラーにして確認します。

## 実装で増えた負担・残る制約

### 1. 標準JSONがDUのnullを受け入れ、境界の検査が必要だった

出典：[fsharp/Program.fs](Program.fs#L11-L22)

```fsharp
    // The standard DU reader can accept JSON null: reject it at the boundary.
    let rec containsNull (value: JsonElement) =
        match value.ValueKind with
        | JsonValueKind.Null -> true
        | JsonValueKind.Array -> value.EnumerateArray() |> Seq.exists containsNull
        | JsonValueKind.Object -> value.EnumerateObject() |> Seq.exists (fun p -> containsNull p.Value)
        | _ -> false
    let readInput text =
        use document = JsonDocument.Parse(text: string)
        if containsNull document.RootElement then raise (JsonException("null is outside the input contract"))
        JsonSerializer.Deserialize<Input>(text, options) |> nonNull
    [<EntryPoint>]
```

null-gradeの共通テストで実際に受理されることを確認したため、入力のJSON木を先に検査しています。nonNullは.NET APIの戻り値にも使います。F#の通常のDU設計と、外部JSONから来る値は分けて考える必要がありました。

### 2. 共通JSONのケース名を維持する属性と表現差が残る

出典：[fsharp/Models.fs](Models.fs#L64-L68)

```fsharp
type ClassState =
    | [<JsonPropertyName("Available")>] Available of plan: Plan * students: string list
    | [<JsonPropertyName("Full")>] Full of plan: Plan * students: string list
    | [<JsonPropertyName("Teaching")>] Teaching of plan: Plan * students: string list * startedOn: string
    | [<JsonPropertyName("Completed")>] Completed of plan: Plan * results: ClassResult list
```

CamelCase設定がDUケース名にも作用するため、他言語と同じタグを保つ属性を明示しています。フィールドなしケースの出力は文字列なので、共通テストはprogress／prerequisiteだけを意味的に正規化します。イベントはそのまま交換できます。[.NET 11の標準DU対応](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-11/libraries#systemtextjson-improvements)はリフレクション方式です。今回は許容する実行条件ですが、C# Source Generationと同じ条件ではありません。

### 3. リストの末尾追加と、似たレコードの型推論には注意が要った

出典：[fsharp/Decider.fs](Decider.fs#L122-L123)

```fsharp
                let results: ClassResult list = state.Enrollments |> List.filter (fun e -> e.ClassId = id && (match e.Progress with Progress.Passed _ | Progress.Failed _ -> true | _ -> false))
                                               |> List.map (fun e -> { StudentId = e.StudentId; Progress = e.Progress })
```

StudentIdなどが重複するレコードでは型注釈でClassResultを指定しました。またStudents、履歴、stepsへの@による追加はリストをたどります。大きいログなら逆順蓄積などに変えられますが、この機能比較では速度を測っていません。
