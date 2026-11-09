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

既存の期待動作テストは標準設定の制限を追跡するため、失敗するものも維持しています。skip や期待値の緩和はしていません。今回 Preview 3 の再実行による比較は行っていません。

検証結果: **81件中54成功・27失敗・skip 0件**。追加した RC3 テスト7件は全成功。失敗27件の内訳は object union の直接往復19件、親オブジェクトの往復5件、union から抽象基底型への変換3件です。

## 実行方法

```bash
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj --filter FullyQualifiedName~UnionRc3Tests
```

## ライセンス

[MIT](LICENSE)
