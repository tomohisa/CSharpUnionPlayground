# Payment JSON: 1ファイルずつの比較

すべて「Cash(100) の Next に Card(4111, 250)」を持つ例です。各ファイルに、型定義・オブジェクト生成・シリアライズ・デシリアライズ・復元結果の確認をまとめています。タグは内外とも `$type`。成功時はJSONと `PASS` を表示します。

| ファイル | 方法 | 共通基底型 | 手書きコンバーター |
|---|---|---|---|
| [JsonDerived.cs](JsonDerived.cs) | 標準の JsonPolymorphic / JsonDerivedType | 必要 | 不要 |
| [CustomConverter.cs](CustomConverter.cs) | C# union + 専用コンバーター | 不要 | 必要（同ファイル内） |
| [SourceGenerated.cs](SourceGenerated.cs) | C# union + 実験版Generator | 不要 | 不要 |
| [Payment.rs](Payment.rs) | Rust enum + Serde | 不要 | 不要 |
| [Payment.ts](Payment.ts) | TypeScript union + JSON + 読み込み検証 | 不要 | 不要（型検証関数は同ファイル内） |

## 実行

リポジトリのルートから:

```bash
dotnet run --file samples/file-based/JsonDerived.cs
dotnet run --file samples/file-based/CustomConverter.cs
dotnet run --file samples/file-based/SourceGenerated.cs
python3 scripts/run_payment_rust.py
node samples/file-based/Payment.ts
```

C#は固定済みの.NET 11 RC3を使います。各 `.cs` はfile-based appで、専用 `.csproj` は不要です。Generator版だけは `#:project` で既存のGeneratorをAnalyzer参照します（外部配布版ではありません）。JSONリフレクションは全3方式で無効にしています。

Rustは安定版CargoとSerdeを使用します。補助スクリプトが一時ディレクトリに依存用 `Cargo.toml` を作り、リポジトリの `Payment.rs` を直接ビルドします。ソース本体は1ファイルで、Cargoプロジェクトをリポジトリに追加する必要はありません。初回は依存のダウンロードが必要です。再帰enumのサイズを有限にするため `Box<Payment>` を使用します。

TypeScriptはNode.js 24.18.0で直接実行を確認しました。`JSON.parse(...) as Payment` だけにはせず、型検証関数が内側のPaymentまでチェックします。Nodeの実行自体は型チェックではないため、別途 `tsc --noEmit --strict --target ES2022 samples/file-based/Payment.ts` でも確認しました。

## 出力の違い

JsonDerived / Rust / TypeScript は、型判別子とケースのフィールドを同じオブジェクトに入れます:

```json
{"$type":"cash","amount":100,"next":{"$type":"card","cardNumber":"4111","amount":250,"next":null}}
```

手書きConverter / Generator は、ケースのフィールドを `data` に入れます:

```json
{"$type":"cash","data":{"amount":100,"next":{"$type":"card","data":{"cardNumber":"4111","amount":250,"next":null}}}}
```

**JsonDerived版は継承モデルであり、C# unionを使用しません。** これが基底型を採用する場合の最短例です。ほかのC# 2方式は、共通基底型なしでunionをそのまま読み書きします。Generator版の `NoWarn=SYSLIB1227` は、標準のunion判別の代わりに生成コンバーターを明示登録するための限定的な指定です。

## Native AOTでの確認

C# 3ファイルすべてでmacOS arm64へのNative AOT発行・ネイティブ実行を確認し、各 `PASS` を確認しました。file-based appはNative AOT発行が既定です。

```bash
dotnet publish samples/file-based/JsonDerived.cs -c Release -r osx-arm64 -o /tmp/payment-derived
/tmp/payment-derived/JsonDerived

dotnet publish samples/file-based/CustomConverter.cs -c Release -r osx-arm64 -o /tmp/payment-converter
/tmp/payment-converter/CustomConverter

dotnet publish samples/file-based/SourceGenerated.cs -c Release -r osx-arm64 -o /tmp/payment-generated
/tmp/payment-generated/SourceGenerated
```

検証時は `IL2026` / `IL3050` をエラーとして扱い、該当警告はありませんでした。SDKネイティブライブラリ由来の既知のdebug-mapリンカー警告は出ますが、発行・実行は成功します。

短い比較サンプルであり、全方式の入力検証ルールを同一にしたものではありません。手書きConverterは最小例で、Generator版の重複フィールド拒否などをすべて再実装していません。金額の数値表現も言語ごとに異なります（decimal / u64 / number）。
