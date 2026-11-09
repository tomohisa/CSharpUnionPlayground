# Paymentのswitch / matchと網羅性の比較

通常サンプルは利用コードだけを示し、assert・不正入力テストは [with-verification](with-verification/README.md) に分離しています。

通常版は、Cashの中にCardを持つPaymentをJSONから復元し、`describe` / `Describe` の分岐結果を表示します。検証版では外側・内側の型・値や分岐結果も確認します。JSONの型復元と、分岐の網羅性チェックは別の仕組みです。

## 実測した比較

| 型・サンプル | 分岐 | Cardの分岐削除 | Cryptoケース追加・分岐未更新 | catch-allの影響 |
|---|---|---|---|---|
| C# 通常のabstract＋JsonDerived | switch式＋`_` | ビルド成功 | ビルド成功 | `_`が漏れを隠す |
| C# closed＋JsonDerived／自動推論 | switch式、`_`なし | CS8509 | CS8509 | `_`を追加すれば漏れを隠す |
| C# union（手書き／生成Converter） | union自体のswitch式＋`null` | CS8509 | CS8509 | nullだけなら新ケースのチェックを維持 |
| Rust enum | `match`、ワイルドカードなし | E0004 | E0004 | `_`を追加すれば漏れを隠す |
| TypeScript union | switch＋`never`への代入 | TS2322 | TS2322 | 普通のdefaultでは隠れるが、never検証なら検出 |
| Java sealed interface | switch式、defaultなし | 非網羅エラー | 非網羅エラー | defaultを追加すれば漏れを隠す |
| Go marker interface | type switch＋default | ビルド成功 | ビルド成功 | コンパイラーは実装型の網羅性を検査しない |

C#のCS8509は本来警告ですが、サンプルではエラー扱いです。Rust/TypeScript/Javaは上記構成で型チェックのエラーになります。いずれも型定義を変更して利用側も再コンパイルした結果です。配布済みバイナリーへ新ケースが来る場合の保証とは異なります。

[ClosedInferredJson.cs](ClosedInferredJson.cs) の `InferClosedTypePolymorphism` はJSONの登録を自動化しますが、switchの網羅性は同じclosed階層の機能です。新ケース追加時はswitchの更新が必要です。自動JSON登録・ケース追加・型名変更については `python3 scripts/verify_closed_inferred_json.py` の5パターンで検証しています。

## Rust: enumとmatchが直接対応

[Payment.rs](Payment.rs) の主要部分:

```rust
fn describe(payment: &Payment) -> String {
    match payment {
        Payment::Cash { amount, .. } => format!("cash:{amount}"),
        Payment::Card { card_number, .. } => format!("card:{card_number}"),
    }
}
```

enumがケース集合を定義し、コンパイラーが全ケースの処理を要求します。`_ => panic!(...)` を置くと将来ケースがそこに入るため、追加漏れの検出は失われます。JSON変換はSerdeのderiveが担当し、matchの網羅性とは独立です。

## TypeScript: neverで残りのケースがないことを証明

[Payment.ts](Payment.ts) の主要部分:

```typescript
function describe(payment: Payment): string {
  switch (payment.$type) {
    case "cash": return `cash:${payment.amount}`;
    case "card": return `card:${payment.cardNumber}`;
    default: {
      const exhaustive: never = payment;
      throw new Error(`Unhandled Payment: ${exhaustive}`);
    }
  }
}
```

全ケースを処理すると、default内のpaymentはneverになります。ケースを増やすと、その型をneverへ代入できずTS2322になります。**defaultがあっても、never検証があれば追加漏れを検出できます。** `throw` だけのdefaultでは同じ保証になりません。

Nodeの型構文除去による実行は型チェックをしません。必ず `tsc --noEmit --strict` を別途実行します。外部JSONは `unknown` から再帰的に検証してPaymentに変換します。型アサーションだけでは実行時の正当性は保証されません。

## Java: sealed interfaceとswitch式

[Payment.java](Payment.java) の主要部分:

```java
sealed interface Payment permits Cash, Card {}
record Cash(long amount, Payment next) implements Payment {}
record Card(String cardNumber, long amount, Payment next) implements Payment {}

static String describe(Payment payment) {
    return switch (payment) {
        case Cash cash -> "cash:" + cash.amount();
        case Card card -> "card:" + card.cardNumber();
    };
}
```

Java 21のsealed interfaceは `permits` で直接の実装型を限定します。recordは暗黙にfinalです。Cryptoをpermitsに追加してswitchを変更しないと、javacは `compiler.err.not.exhaustive`（switch式が全入力を扱っていない）で失敗します。defaultを置くと追加漏れは検出されません。

JSONはJacksonの `@JsonTypeInfo` / `@JsonSubTypes` を使用します。sealedのケース定義とJSONのタグ登録は別なので、ケース追加時は両方を更新します。Javaにも参照のnullはあります。このswitchのnull入力はNPEになり、サンプルは読み込み境界でnullを拒否します。内側の終端nextだけはnullを許します。

## Go: type switchは使えるが、閉じたケース集合ではない

[Payment.go](Payment.go) の主要部分:

```go
type Payment interface{ isPayment() }

func describe(payment Payment) string {
    switch p := payment.(type) {
    case Cash:
        return fmt.Sprintf("cash:%d", p.Amount)
    case Card:
        return "card:" + p.CardNumber
    default:
        panic("unknown or nil payment")
    }
}
```

marker methodは型をまとめるための制約であり、Cash/Cardだけの閉じたunionにはなりません。Cryptoが同じメソッドを実装しても、コンパイラーはswitchの更新を要求しません。defaultのpanicは実行時の防御です。

defaultを外しても、switchの後に戻り値を用意すればビルドできます。戻り値がない場合のエラーは「return不足」であって、ケース集合の網羅性エラーではありません。interfaceのnilや型付きnilも別途考慮が必要です。

JSONのinterfaceを自動で具象型に復元できないため、標準encoding/jsonと型判別子の明示的なdispatchを使用します。Goにはこの用途の標準コンパイラーによる網羅性検査はなく、追加の静的解析ツールを採用するか、設計・テストで補います。今回、外部lintツールは導入していません。

## 再実行

```bash
# JSON往復・内側と外側の分岐・不正な内側タグの拒否
python3 scripts/run_payment_rust.py samples/file-based/with-verification/Payment.rs
node samples/file-based/with-verification/Payment.ts
python3 scripts/run_payment_java.py samples/file-based/with-verification/Payment.java
go run samples/file-based/with-verification/Payment.go

# 実ソースを一時コピーして分岐漏れ・ケース追加・catch-allの影響を検証
python3 scripts/verify_cross_language_payment_switches.py

# 既存のC#4方式の比較も再実行できる
python3 scripts/verify_payment_switch_comparison.py
```

Rust/TypeScript/Java/Goは、各4パターンの計16コンパイル検証がすべて期待どおりでした。JSON往復サンプルも全言語でPASSです。確認環境はrustc/Cargo 1.98.0、TypeScript 7.0.2（strict）、Node 24.18.0、OpenJDK 21.0.6、Jackson databind/core 2.23.0＋annotations 2.23、Go 1.23.4です。Javaのrunnerは依存jarを一時ディレクトリに取得し、サンプル本体は1つの.javaのまま実行します。診断検証スクリプトも一時ファイルを使い、登録済みソースは変更しません。

## 公式資料

- [Rust: matchと網羅性](https://doc.rust-lang.org/book/ch06-02-match.html)
- [TypeScript: neverによる網羅性チェック](https://www.typescriptlang.org/docs/handbook/2/narrowing.html#exhaustiveness-checking)
- [Java 21: pattern matching for switch](https://docs.oracle.com/en/java/javase/21/language/pattern-matching-switch.html)
- [Java: sealed classes / interfaces](https://docs.oracle.com/en/java/javase/24/language/sealed-classes-and-interfaces.html)
- [Jackson: polymorphic type handling](https://github.com/FasterXML/jackson-docs/wiki/JacksonPolymorphicDeserialization)
- [Go: type switches](https://go.dev/ref/spec#Type_switches)
- [Go: encoding/json](https://pkg.go.dev/encoding/json)
