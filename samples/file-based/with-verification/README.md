# 検証付きサンプル

通常の利用例は [親フォルダ](../README.md) にあります。このフォルダは従来のassert・不正入力チェック・Native AOT実行確認を保持したものです。以下は検証版の実行手順と検証時の結果です。コンパイラー診断スクリプトは通常版を対象とするものもあり、自動推論の実行検証スクリプトはこのフォルダの検証版を使います。

# Payment JSON: 1ファイルずつの比較

すべて「Cash(100) の Next に Card(4111, 250)」を持つ例です。各ファイルに、型定義・オブジェクト生成・シリアライズ・デシリアライズ・復元結果の確認をまとめています。Swift以外のタグは内外とも `$type`。Swiftは標準Codableのケース名ラッパーを使います。成功時はJSONと `PASS` を表示します。C#の各ファイルはswitch式による外側・内側のケース処理と、その結果も表示します。

| ファイル | 方法 | 共通基底型 | 手書きコンバーター |
|---|---|---|---|
| [JsonDerived.cs](JsonDerived.cs) | 標準の JsonPolymorphic / JsonDerivedType | 必要 | 不要 |
| [ClosedJsonDerived.cs](ClosedJsonDerived.cs) | closed 継承モデル + JsonDerivedType + switch網羅性 | 必要 | 不要 |
| [ClosedInferredJson.cs](ClosedInferredJson.cs) | closed派生型の自動推論（ケース列挙不要） | 必要 | 不要 |
| [CustomConverter.cs](CustomConverter.cs) | C# union + 専用コンバーター | 不要 | 必要（同ファイル内） |
| [SourceGenerated.cs](SourceGenerated.cs) | C# union + 実験版Generator | 不要 | 不要 |
| [Payment.rs](Payment.rs) | Rust enum + Serde | 不要 | 不要 |
| [Payment.ts](Payment.ts) | TypeScript union + JSON + 読み込み検証 | 不要 | 不要（型検証関数は同ファイル内） |
| [Payment.java](Payment.java) | Java sealed interface + Jackson | 共通interface | 不要 |
| [Payment.go](Payment.go) | Go marker interface + encoding/json | 共通interface | 型判別dispatchを同ファイル内に実装 |
| [Payment.swift](Payment.swift) | Swift indirect enum + Codable | 不要 | 不要 |

各言語のswitch / Rust matchと、ケース追加・分岐漏れによる網羅性の実測比較は **[EXHAUSTIVENESS.md](../EXHAUSTIVENESS.md)** を参照してください。Rust・TypeScriptにも分岐例を追加し、Java・Goを含む16パターンのコンパイル検証を用意しています。

## 実行

リポジトリのルートから:

```bash
dotnet run --file samples/file-based/with-verification/JsonDerived.cs
dotnet run --file samples/file-based/with-verification/ClosedJsonDerived.cs
dotnet run --file samples/file-based/with-verification/ClosedInferredJson.cs
dotnet run --file samples/file-based/with-verification/CustomConverter.cs
dotnet run --file samples/file-based/with-verification/SourceGenerated.cs
python3 scripts/run_payment_rust.py samples/file-based/with-verification/Payment.rs
node samples/file-based/with-verification/Payment.ts
python3 scripts/run_payment_java.py samples/file-based/with-verification/Payment.java
go run samples/file-based/with-verification/Payment.go
swift samples/file-based/with-verification/Payment.swift
```

C#は固定済みの.NET 11 RC3を使います。各 `.cs` はfile-based appで、専用 `.csproj` は不要です。Generator版だけは `#:project` で既存のGeneratorをAnalyzer参照します（外部配布版ではありません）。JSONリフレクションは全5方式で無効にしています。

Rustは安定版CargoとSerdeを使用します。補助スクリプトが一時ディレクトリに依存用 `Cargo.toml` を作り、リポジトリの `Payment.rs` を直接ビルドします。ソース本体は1ファイルで、Cargoプロジェクトをリポジトリに追加する必要はありません。初回は依存のダウンロードが必要です。再帰enumのサイズを有限にするため `Box<Payment>` を使用します。

TypeScriptはNode.js 24.18.0で直接実行を確認しました。`JSON.parse(...) as Payment` だけにはせず、型検証関数が内側のPaymentまでチェックします。Nodeの実行自体は型チェックではないため、別途 `tsc --noEmit --strict --target ES2022 samples/file-based/with-verification/Payment.ts` でも確認しました。

JavaはJava 21以降とJacksonを使用します。補助スクリプトが固定バージョンのjarを一時取得してsource-file modeで実行するため、サンプル本体は1ファイルで、Maven/Gradleの設定は不要です。Goは標準ライブラリだけを使い、`go run` で直接実行できます。全4言語で外側・内側の分岐処理と、不正な内側タグを拒否することも検証します。

Swift版はネストしたケースと値の等価性、両階層のswitch、ケース名によるJSON構造、不正なケースとJSON nullの拒否を検証します。コンパイラー診断6件は `python3 scripts/verify_swift_payment_switches.py` で再現できます。Apple Swift 6.4で確認済みです。ネイティブ実行も確認しています:

```bash
swiftc samples/file-based/with-verification/Payment.swift -o /tmp/payment-swift
/tmp/payment-swift
```

## 出力の違い

Swiftの標準Codableはケース名をキーにして包みます。Optionalのnextがnilなら、そのフィールドは出力されません:

```json
{"cash":{"amount":100,"next":{"card":{"amount":250,"cardNumber":"4111"}}}}
```

JsonDerived / ClosedJsonDerived / Rust / TypeScript / Java / Go は、型判別子とケースのフィールドを同じオブジェクトに入れます（プロパティの出力順は言語によって異なります）:

```json
{"$type":"cash","amount":100,"next":{"$type":"card","cardNumber":"4111","amount":250,"next":null}}
```

手書きConverter / Generator は、ケースのフィールドを `data` に入れます:

```json
{"$type":"cash","data":{"amount":100,"next":{"$type":"card","data":{"cardNumber":"4111","amount":250,"next":null}}}}
```

**JsonDerived版は継承モデルであり、C# unionを使用しません。** これが基底型を採用する場合の最短例です。ほかのC# 2方式は、共通基底型なしでunionをそのまま読み書きします。Generator版の `NoWarn=SYSLIB1227` は、標準のunion判別の代わりに生成コンバーターを明示登録するための限定的な指定です。

## Native AOTでの確認

C# 5ファイルすべてでmacOS arm64へのNative AOT発行・ネイティブ実行を確認し、各 `PASS` を確認しました。file-based appはNative AOT発行が既定です。

```bash
dotnet publish samples/file-based/with-verification/JsonDerived.cs -c Release -r osx-arm64 -o /tmp/payment-derived
/tmp/payment-derived/JsonDerived

dotnet publish samples/file-based/with-verification/ClosedJsonDerived.cs -c Release -r osx-arm64 -o /tmp/payment-closed
/tmp/payment-closed/ClosedJsonDerived --require-native

dotnet publish samples/file-based/with-verification/ClosedInferredJson.cs -c Release -r osx-arm64 -o /tmp/payment-inferred
/tmp/payment-inferred/ClosedInferredJson --require-native

dotnet publish samples/file-based/with-verification/CustomConverter.cs -c Release -r osx-arm64 -o /tmp/payment-converter
/tmp/payment-converter/CustomConverter

dotnet publish samples/file-based/with-verification/SourceGenerated.cs -c Release -r osx-arm64 -o /tmp/payment-generated
/tmp/payment-generated/SourceGenerated
```

検証時は `IL2026` / `IL3050` をエラーとして扱い、該当警告はありませんでした。SDKネイティブライブラリ由来の既知のdebug-mapリンカー警告は出ますが、発行・実行は成功します。

短い比較サンプルであり、全方式の入力検証ルールを同一にしたものではありません。手書きConverterは最小例で、Generator版の重複フィールド拒否などをすべて再実装していません。金額の数値表現も言語ごとに異なります（decimal / u64 / number / long / int64）。

## closed派生型を自動登録する標準機能

[ClosedInferredJson.cs](ClosedInferredJson.cs) は、既存のClosedJsonDerived版から `JsonDerivedType` のリストを削除し、次の属性だけで派生ケースを登録します。独自Generatorや専用Converterは不要です。

```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Payment;
public sealed record Cash(decimal Amount, Payment? Next = null) : Payment;
public sealed record Card(string CardNumber, decimal Amount, Payment? Next = null) : Payment;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment))]
internal partial class PaymentContext : JsonSerializerContext;
```

**Native AOTでも使用できました。** macOS arm64向けRelease発行・ネイティブ実行でPASSし、動的コード生成・JSONリフレクションともに無効であることを確認しています。標準のSystem.Text.Jsonソース生成が、基底型から派生型のメタデータを作ります。contextへのCash/Cardの個別登録も不要です。通常のJSON呼び出しは他のサンプル同様、生成済み `PaymentContext.Default.Payment` を渡します。

自動判別子は型の単純名で、camelCase設定でも変更されません:

```json
{"$type":"Cash","amount":100,"next":{"$type":"Card","cardNumber":"4111","amount":250,"next":null}}
```

明示登録版の `"cash"` / `"card"` と自動推論版の `"Cash"` / `"Card"` は異なる契約です。型名をDebitCardに変えると判別子も `"DebitCard"` になります。長期保存や外部APIでタグの安定性が必要なら、明示的な `JsonDerivedType` を選びます。同名の型がある場合も自動判別子の衝突を考慮します。

検証スクリプト:

```bash
python3 scripts/verify_closed_inferred_json.py
```

5パターンすべて成功しました:

1. 型レベルの `JsonPolymorphic(InferClosedTypePolymorphism = true)` で往復。
2. 型属性の代わりに `JsonSourceGenerationOptions(InferClosedTypePolymorphism = true)` でも往復。
3. Cryptoケースを追加してswitch未更新ならCS8509。
4. switchを更新すればCryptoもJSON登録・context登録の追加なしで往復。
5. 型名をCardからDebitCardへ変更すると自動判別子も変更。

型レベルの属性がない場合、実行時の `JsonSerializerOptions` だけで推論を有効にしても、AOT用の派生メタデータは後から追加されません。標準ソース生成に認識される型属性または `JsonSourceGenerationOptions` を使います。明示的な `JsonDerivedType` の登録は推論リストに追加されるのではなく、その階層の推論を置き換えます。

公式資料: [Infer polymorphism from a closed hierarchy](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism#infer-polymorphism-from-a-closed-hierarchy)。

## switch式の比較

5つのC#サンプルに同じ `Describe(Payment)` を置きました。JSONの変換方式とswitchの網羅性は別の仕組みです。

**通常の基底型（JsonDerived.cs）:** Cash/Card以外も継承できるため、`_` の受け皿が必要です。

```csharp
static string Describe(Payment payment) => payment switch
{
    Cash cash => $"cash:{cash.Amount}",
    Card card => $"card:{card.CardNumber}",
    _ => throw new InvalidOperationException("Unknown payment case.")
};
```

**closed階層（ClosedJsonDerived.cs）:** 非nullableなPaymentなら全ケースを列挙するだけです。

```csharp
static string Describe(Payment payment) => payment switch
{
    Cash cash => $"cash:{cash.Amount}",
    Card card => $"card:{card.CardNumber}"
};
```

**union（CustomConverter.cs / SourceGenerated.cs）:** `payment.Value` ではなくunion自体にswitchを書きます。`null` はunion内部の空のdefault値を扱います。`_` は置かないため、ケース追加時の警告を維持できます。

```csharp
static string Describe(Payment payment) => payment switch
{
    Cash cash => $"cash:{cash.Amount}",
    Card card => $"card:{card.CardNumber}",
    null => "empty"
};
```

union版は `Describe(default)` が `"empty"` になることも実行確認します。非nullableケースのみのunionではコンパイラーがnull分岐の省略を警告しない場合がありますが、実際の空のdefault状態を安全に処理するため、ここでは明示しています。

| 方式 | Card分岐を削除 | Cryptoケースを追加してswitch未更新 | 空の扱い |
|---|---|---|---|
| 通常の基底型＋`_` | ビルド成功（`_`に入る） | ビルド成功（`_`に入る） | 参照のnullも`_`に入る |
| closed＋ケース列挙 | CS8509 | CS8509 | 入力がPayment?ならnull分岐も必要 |
| union＋ケース列挙＋null | CS8509 | CS8509 | defaultはnull分岐で処理 |

実サンプルを一時コピーして、全4方式の通常ビルド・分岐削除・ケース追加、および通常基底型のdiscard削除を検証します。13パターンすべて期待どおりでした。各サンプルではCS8509 / CS8655をエラーとして扱います。

```bash
python3 scripts/verify_payment_switch_comparison.py
```

なお、手書きコンバーター内のタグ文字列のswitchや生成コード内のswitchは、ここで比較する利用側の `Describe(Payment)` とは別です。未知のJSONタグを拒否するための分岐は引き続き必要です。

## closed継承モデルの追加検証

RC3の正しい構文は `public closed record Payment;` です。**closedは暗黙にabstractであり、`abstract closed` を併記するとCS9384になります。** ケースは `sealed record Cash(...) : Payment` / `sealed record Card(...) : Payment` と定義します。

`ClosedJsonDerived.cs` は `Describe(Payment)` でCash/Cardだけを列挙し、`_` や `null` の分岐を置きません。ネストしたJSONの復元結果に対して、外側のCash・内側のCardの両方でこのswitchを実行します。JSON nullは読み込み境界で拒否し、内側の終端 `Next` だけは `Payment?` としてnullを許します。

実サンプルの一時コピーを変更して、実際のRC3コンパイラーの診断も検証できます:

```bash
python3 scripts/verify_closed_payment_exhaustiveness.py
```

| 変更 | 結果 |
|---|---|
| 全ケースを処理、discardなし | ビルド成功 |
| abstract closedを併記 | CS9384 |
| Cardの分岐を削除 | CS8509（非網羅） |
| Cryptoケースを追加してswitchを変更しない | CS8509（非網羅） |
| 入力をPayment?に変更しnull分岐なし | CS8655（null未処理） |
| Payment?でnull分岐も追加 | ビルド成功 |
| closedを通常のabstractに戻す | CS8509（非網羅） |

7パターンすべて期待どおりでした。サンプルはCS8509 / CS8655をエラーとして扱うため、ケース漏れを含むコードはビルドに失敗します。Native AOT実行は `--require-native` で動的コード生成が無効であることも検証します。

closedでも参照型のnullを実行時に禁止するわけではありません。非nullableな入力の網羅性チェックと、外部JSONのnull検証は別です。これは既存ケースを合成するC# `union` ではなく、閉じた継承階層のサンプルです。

Zod版は`node samples/file-based/with-verification/PaymentZod.ts`で実行します。事前に`npm ci --prefix samples/file-based`で固定版依存を用意してください。ネストの往復・switch・不正入力の拒否を検証します。
