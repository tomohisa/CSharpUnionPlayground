# Payment JSON: 1ファイルずつの利用例

各ファイルは「型定義 → ネストしたPaymentを生成 → JSONへ変換 → 読み込み → switch / matchで利用」を示します。期待値比較・テスト用のassert・不正入力テスト・`PASS`表示は含めません。

| ファイル | 型とJSON変換 | 分岐 |
|---|---|---|
| [JsonDerived.cs](JsonDerived.cs) | abstract record + JsonDerivedType | switch + `_` |
| [ClosedJsonDerived.cs](ClosedJsonDerived.cs) | closed record + JsonDerivedType | 全ケースのswitch |
| [ClosedInferredJson.cs](ClosedInferredJson.cs) | closed record + 派生型自動登録 | 全ケースのswitch |
| [CustomConverter.cs](CustomConverter.cs) | union + 同ファイル内の手書きConverter | ケース + null |
| [SourceGenerated.cs](SourceGenerated.cs) | union + 実験版Generator | ケース + null |
| [Payment.rs](Payment.rs) | Rust enum + Serde | match |
| [Payment.ts](Payment.ts) | TypeScript union + JSON | switch + never |
| [Payment.java](Payment.java) | Java sealed interface + Jackson | switch式 |
| [Payment.go](Payment.go) | Go interface + encoding/json | type switch |
| [Payment.swift](Payment.swift) | Swift indirect enum + 標準Codable | 全ケースのswitch |

## 実行

リポジトリのルートから、見たいファイルを指定します。

```bash
dotnet run --file samples/file-based/JsonDerived.cs
dotnet run --file samples/file-based/ClosedJsonDerived.cs
dotnet run --file samples/file-based/ClosedInferredJson.cs
dotnet run --file samples/file-based/CustomConverter.cs
dotnet run --file samples/file-based/SourceGenerated.cs
python3 scripts/run_payment_rust.py
node samples/file-based/Payment.ts
python3 scripts/run_payment_java.py
go run samples/file-based/Payment.go
swift samples/file-based/Payment.swift
```

出力はJSONとswitch / matchの結果（`cash:100`）です。CashのNextにはCardが入っています。

C#は固定済み.NET 11 RC3のfile-based appで、個別のcsprojは不要です。Generator版だけは `#:project` で既存の実験版Generatorを参照します。すべてJSONリフレクションを無効にして、標準の `JsonSerializerContext` を使います。

Rustのrunnerは安定版Cargoの依存設定を、JavaのrunnerはJacksonのjarを一時ディレクトリへ用意します。TypeScriptはNode.js 24.18.0で直接実行できますが、型チェックは別途 `tsc --noEmit --strict --target ES2022 samples/file-based/Payment.ts` を使います。Goは標準ライブラリだけを使います。

Swiftは外部パッケージ不要で、`indirect enum Payment: Codable` とFoundationのJSONEncoder / JSONDecoderだけを使います。標準の自動生成に任せるため、JSONは `$type` ではなくケース名で包む形式です:

```json
{"cash":{"amount":100,"next":{"card":{"amount":250,"cardNumber":"4111"}}}}
```

Swift版をほかの例と同じ `$type` 形式に合わせる場合は、独自のCodable実装などが必要です。この例は標準機能で最短に書くことを優先しています。

## 通常のエラー処理とテストの区別

不正なJSONを拒否するための例外・Goのerror処理・TypeScriptの読み込み時の型検証・nullの扱いは、利用コードの一部として残しています。TypeScriptのnever代入も網羅性チェックに必要です。これらは期待値を比較するテストコードではありません。

## 検証コードと分析

元の検証コードは **[with-verification/](with-verification/README.md)** に同名ファイルで保存しています。こちらでは外側・内側のケースと値の比較、不正入力、空のunion、Native AOT実行などを確認します。

```bash
dotnet run --file samples/file-based/with-verification/ClosedInferredJson.cs
python3 scripts/run_payment_rust.py samples/file-based/with-verification/Payment.rs
python3 scripts/run_payment_java.py samples/file-based/with-verification/Payment.java
node samples/file-based/with-verification/Payment.ts
go run samples/file-based/with-verification/Payment.go
swift samples/file-based/with-verification/Payment.swift
```

- [switch / matchと網羅性の比較分析](EXHAUSTIVENESS.md)
- [検証付きサンプルの詳細・Native AOT手順](with-verification/README.md)

closedの自動推論では `$type` は型名（Cash / Card）、明示登録版ではcash / cardです。長期保存するJSONのタグを固定したい場合は明示登録を選びます。手書き／生成Converter版だけはケースのフィールドを `data` に包みます。
