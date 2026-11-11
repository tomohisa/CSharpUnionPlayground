# C#：closed record＋標準STJ

Stateのフェーズとイベントのケースを、別々のrecord型として表しています。既存のクラス設計を保ちつつ、switchの網羅性と標準JSON推論を使いたい場合に読みやすい構成です。

対象はこのClassroom Deciderの実装です。言語一般の順位ではありません。[共通仕様・検証](../README.md)／[コード量の集計](../README.md#コード量の比較)も併せて参照してください。以下の引用は現行ソースの抜粋で、出典リンクから該当行へ移動できます。

## Pros

### 1. ケース別の型と、defaultのないswitchを両立できる

出典：[csharp/Models.cs](Models.cs#L8-L12)

```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record State;
public sealed record NotCreated : State;
public sealed record Available(Roster Roster) : State;
public sealed record Full(Roster Roster) : State;
```
AvailableとFullは別の型です。[csharp/Decider.cs](Decider.cs#L7)のswitch式は3ケースを列挙し、[csharp/Decider.csproj](Decider.csproj#L5)ではCS8509をエラーにしています。[go/domain/decider.go](../go/domain/decider.go#L25)の実行時default拒否と異なり、既知のケース追加をコンパイラーに検査させる構成です。新しいunionとJSON用基底型を行き来する変換もありません。

### 2. コレクションの不変性と更新を短く表せる

出典：[csharp/Models.cs](Models.cs#L6-L6)

```csharp
public sealed record Roster(string Name, int Capacity, ImmutableArray<string> Students);
```
出典：[csharp/Decider.cs](Decider.cs#L60-L60)

```csharp
        return Classify(available.Roster with { Students = [.. available.Roster.Students, student] });
```
学生リストはImmutableArrayで、更新時に新しい配列を作ります。[java/Decider.java](../java/Decider.java#L52)の一時ArrayListと新Rosterの構築や、[go/domain/decider.go](../go/domain/decider.go#L76)の明示的なsliceコピーより、変更箇所を追いやすい書き方です。ただし配列の再構築コストがなくなるわけではありません。

### 3. 独自コンバーターなしで、このJSON契約とNative AOTに対応できる

closed推論で派生型を登録し、[csharp/Program.cs](Program.cs#L45)ではStateのまま保存・復元します。[swift/wire.swift](../swift/wire.swift#L45)や[go/wire.go](../go/wire.go#L75)のようなタグ別の手書き復元処理はありません。Native AOTも共通シナリオで確認済みです。これは対応可否の検証で、他言語より速いという測定ではありません。

## Cons

### 1. このまま.NET 10へ持ち込める書き方ではない

[csharp/Decider.csproj](Decider.csproj#L3)はnet11.0を指定し、closedとInferClosedTypePolymorphismを使っています。Java版はJava 21のsealed／recordで書いており、同じ前提のランタイムではありません。既存.NET 10を対象にするなら、abstract＋明示的なJsonDerivedTypeなど別の設計が必要です。

### 2. JSONの宣言が完全に消えるわけではない

出典：[csharp/Program.cs](Program.cs#L62-L66)

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    AllowOutOfOrderMetadataProperties = true)]
[JsonSerializable(typeof(Input))]
[JsonSerializable(typeof(Output))]
internal partial class Wire : JsonSerializerContext;
```
[file-basedのClosedInferredJson.cs](../../file-based/ClosedInferredJson.cs#L29)と同じく、JsonSerializerContextには入出力のルートだけを登録します。Input／Outputから参照されるCommand・State・Eventや、closed型の各ケースは標準Generatorがたどるため、個別のJsonSerializableは不要です。ルートがPaymentだけの例では1つ、入出力が別型のこの例では2つの登録が残ります。$typeが先頭でない他言語のJSONを読むためのオプションもあります。[rust/src/models.rs](../rust/src/models.rs#L11)のSerde deriveだけで済むモデル宣言と比べると、アプリ側に持つ設定が増えます。

### 3. 推論タグ・参照型・エラーコードの管理は残る

推論タグは型名なので、Availableなどの改名が保存済みJSONへ影響します。[java/Models.java](../java/Models.java#L12)のようにタグ文字列を別に固定する方式とのトレードオフです。C#でも明示登録へ変更できます。

出典：[csharp/Models.cs](Models.cs#L28-L28)

```csharp
public sealed record Rejected(string Code) : Decision;
```
拒否の種類はこの実装ではstringです。[swift/models.swift](../swift/models.swift#L43)のenumのようにはコードの綴りをコンパイラーが限定しません。またclosed recordは参照型で、nullや不正なRosterを渡せない保証ではありません。Fullという型と「登録者数＝定員」の業務条件は別です。
