# CSharpUnionPlayground

[English](README.md) | **日本語**

**C# 15 Union 型** (.NET 11 RC3) の機能と制限を検証するプレイグラウンドです。

## 概要

C# 15 で導入された `union` キーワードの動作を実験・検証するリポジトリです:

- 大量ケースのストレステスト (100ケース、500ケース)
- `System.Text.Json` によるシリアライズ/デシリアライズの動作検証
- `[JsonDerivedType]` を使ったポリモーフィックJSONラウンドトリップパターン

## Union 型の基本

```csharp
// union の宣言
public union Shape(Circle, Rectangle, Triangle);

public record Circle(double Radius);
public record Rectangle(double Width, double Height);
public record Triangle(double A, double B, double C);

// 網羅的パターンマッチング
string describe = shape switch
{
    Circle c    => $"円 r={c.Radius}",
    Rectangle r => $"四角 {r.Width}x{r.Height}",
    Triangle t  => $"三角形",
};
```

## JSON シリアライズ: RC3 検証

`global.json` で SDK `11.0.100-rc.3.26522.148` に固定しました。`UnionAttribute` と `IUnion` はランタイムに含まれるため、Preview 3 用 polyfill を削除しました。

- `int`・`string`・`bool` のように JSON トークンの種類が異なるケースは、標準設定でラウンドトリップできます。
- 有効なケースが直接出力され、`Value` ラッパーや `$type` は付きません。default union は `null` として往復できます。
- 複数の object ケースは型判別器が必要です。標準設定のデシリアライズは、空の union を返す代わりに `JsonException` を送出します。
- `JsonUnionTypeStructuralClassifier` を設定すると、プロパティ名によって object ケースを復元できます。親オブジェクトのリスト内の union と camelCase 設定でも確認しました（`UnionRc3Tests`）。
- union の JSON から抽象基底型への復元には、引き続き型判別子が必要です。`[JsonDerivedType]` を使う場合は基底型としてシリアライズします。

以下の初回検証は標準設定です。現在の設定での結果は後続の節を参照してください。skip や期待値の緩和はしていません。今回 Preview 3 の再実行による比較は行っていません。

検証結果: **81件中54成功・27失敗・skip 0件**。追加した RC3 テスト7件は全成功。失敗27件の内訳は object union の直接往復19件、親オブジェクトの往復5件、union から抽象基底型への変換3件です。

## 構造判別器を既存テストに適用した結果

既存テストの期待値を変更せず、JSON テスト3クラスのオプションを更新しました。

| 設定 | 元の失敗27件のうち成功 | 元の成功54件で新たに失敗 | 元の81件の成功 / 失敗 |
|---|---:|---:|---:|
| RC3 標準設定 | 0 | 0 | 54 / 27 |
| 構造判別器のみ | 14 | 11 | 57 / 24 |
| 構造判別器 + 下記の厳密な設定（現在） | 21 | 3 | 72 / 9 |

```csharp
PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
RespectRequiredConstructorParameters = true,
UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
TypeClassifiers = { new JsonUnionTypeStructuralClassifier() }
```

最終結果は、対応方法の追加テスト5件を含め **86件中77成功・9失敗・skip 0件**。追加5件は全成功です。

構造判別器だけで PersonInfo・ComplexData・MixedUnion の往復10件と、Payment 以外の親オブジェクトの往復4件が成功しました。Animal・Varied・Payment はプロパティ集合が包含関係にあり、既定ではコンストラクター引数が必須ではなく、未知プロパティも無視されるため、候補が絞れません。構造判別器の契約初期化はシリアライズ時にも実行されるため、書き込みテストも失敗しました。必須引数を尊重し未知プロパティを拒否すると、Animal 3件・Varied 3件・Payment を持つ親1件も往復できます。厳密な設定は欠落した必須引数や余分なプロパティのある既存 JSON を拒否するため、互換性の変更に注意が必要です。

| 残る失敗 | 原因 | 対応方法 |
|---|---|---|
| ResultUnion 6件（往復3件 + 新規のシリアライズ失敗3件） | StringResult と IntResult がともに `value` を持ち、構造判別器は値のトークン種類を見ない。ListResult も union 全体の契約初期化で失敗 | 値の種類を読む独自 `JsonTypeClassifierFactory`、専用コンバーター、または `textValue` / `intValue` のように JSON プロパティ名を分ける。書き込みだけなら構造判別器なしの別オプションを使えるが、読み込みは解決しない |
| union JSON → Payment 3件 | union は `$type` を出力しない。構造判別器は抽象 Payment のポリモーフィズム判別子を追加しない | 既存の成功例のように `JsonSerializer.Serialize<Payment>(caseValue, options)` で書き出す。または同じ PaymentUnion に読み戻す（上記設定で検証済み）。union と基底型の両方で共通 JSON が必要なら専用コンバーターで契約を定義する |

対応方法の検証は `UnionStructuralRemedyTests`。失敗を隠す skip や期待値の緩和は行っていません。独自判別器・コンバーター・プロパティ名変更は今回実装していません。

仕様: [Microsoft の union serialization ドキュメント](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/libraries#union-serialization)。

## 実験版: 属性からJSONコンバーターを生成

標準と区別するため `CSharpUnionPlayground.Experimental.Json` 名前空間の `PlaygroundTaggedUnion` / `PlaygroundUnionCase` 属性を使う Source Generator を追加しました。共通の親型・手書きコンバーターなしで、型判別子付きJSONを往復できます。

macOS arm64 の Native AOT 実行で40チェック成功（`--require-native`指定）（動的コード・JSONリフレクション無効）、コンパイル診断6件も成功しました。正式版ではありません。利用者側で生成コンバーターの登録と `JsonSerializerContext` の定義が必要です。[使用例・再現手順・制限](docs/experimental-tagged-json.md)。

## 実行方法

通常サンプルは利用コードだけに絞り、assertや実行環境の検証は [with-verification](samples/file-based/with-verification/README.md) に分離しています。


closed派生型を `InferClosedTypePolymorphism` で自動登録する [標準機能の1ファイル例](samples/file-based/ClosedInferredJson.cs) も追加済みです。JsonDerivedTypeや派生型のcontext登録を列挙せずに、ネストしたPaymentのNative AOT往復を確認しました。

ネストしたPaymentを比較する、C# 5方式・Rust・TypeScript・Java・Go・Swiftの各1ファイルサンプルは [こちら](samples/file-based/README.md)。[switch/match網羅性の分析](samples/file-based/EXHAUSTIVENESS.md)では各言語の構文と、ケース追加・分岐削除の診断を比較しています。Rust/TypeScript/Java/Goの16パターンと、Swiftの6パターンを実測済みです。closed継承モデル＋JsonDerivedType版ではNative AOTのJSON往復と、switch網羅性など7パターンも検証しています。なおclosedは暗黙にabstractで、`abstract closed` の併記はエラーになります。

```bash
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj --filter FullyQualifiedName~UnionRc3Tests
```

## ライセンス

[MIT](LICENSE)

## CLI benchmarks

[100-case JSON and event projection benchmarks](benchmarks/README.md) compare C#, Rust, TypeScript, Java, Go and Swift, including memory, process lifecycle timings, scaling and WASI.
