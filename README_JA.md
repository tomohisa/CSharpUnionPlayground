# CSharpUnionPlayground

[English](README.md) | **日本語**

**C# 15 Union 型** (.NET 11 Preview 3+) の機能と制限を検証するプレイグラウンドです。

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

## JSON シリアライズの検証結果

| アプローチ | シリアライズ | デシリアライズ |
|-----------|------------|--------------|
| union 型を直接 | OK | NG (Value が null — 型判別子なし) |
| `abstract record` 基底型 + `[JsonDerivedType]` 経由 | OK | OK |
| `interface` + `[JsonDerivedType]` 経由 | OK | OK |
| 具象子型を直接 | OK | OK |

**結論:** union 型は struct なので `[JsonDerivedType]` を直接付与できません。基底型や interface に `[JsonPolymorphic]`/`[JsonDerivedType]` を付与し、デシリアライズ後に union に再代入する方法で対応可能です。

## プロジェクト構成

```
src/
  UnionTest/          # テスト用コンソールアプリ
    Union100.cs       # 100ケース union ストレステスト
    Union500.cs       # 500ケース union ストレステスト
    UnionJsonTest.cs  # 基本 JSON シリアライズテスト
    UnionJsonDerivedTest.cs  # JsonDerivedType テスト
    Program.cs        # テストランナー
```

## 要件

- .NET 11 Preview 3+
- C# 15 (`LangVersion preview`)

## 実行方法

```bash
cd src/UnionTest
dotnet run
```

## ライセンス

[MIT](LICENSE)
