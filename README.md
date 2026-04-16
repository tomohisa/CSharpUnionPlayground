# CSharpUnionPlayground

**English** | [日本語](README_JA.md)

A playground for testing and exploring **C# 15 Union Types** (.NET 11 Preview 3+).

## What's This?

This repository contains experiments to test the capabilities and limitations of the new `union` keyword in C# 15, including:

- Large union stress tests (100 and 500 case types)
- `System.Text.Json` serialization/deserialization behavior
- `[JsonDerivedType]` polymorphic JSON round-trip patterns

## Union Type Basics

```csharp
// Declare a union
public union Shape(Circle, Rectangle, Triangle);

public record Circle(double Radius);
public record Rectangle(double Width, double Height);
public record Triangle(double A, double B, double C);

// Exhaustive pattern matching
string describe = shape switch
{
    Circle c    => $"Circle r={c.Radius}",
    Rectangle r => $"Rect {r.Width}x{r.Height}",
    Triangle t  => $"Triangle",
};
```

## JSON Serialization Findings

| Approach | Serialize | Deserialize |
|----------|-----------|-------------|
| Union type directly | OK | FAIL (Value is null — no type discriminator) |
| Via `abstract record` base with `[JsonDerivedType]` | OK | OK |
| Via `interface` with `[JsonDerivedType]` | OK | OK |
| Via concrete child type | OK | OK |

**Key finding:** Union types are structs, so `[JsonDerivedType]` cannot be applied directly. Use a base type or interface with `[JsonPolymorphic]`/`[JsonDerivedType]`, then wrap back into the union after deserialization.

## Project Structure

```
src/
  UnionTest/          # Console app with all tests
    Union100.cs       # 100-case union stress test
    Union500.cs       # 500-case union stress test
    UnionJsonTest.cs  # Basic JSON serialization tests
    UnionJsonDerivedTest.cs  # JsonDerivedType tests
    Program.cs        # Test runner
```

## Requirements

- .NET 11 Preview 3+
- C# 15 (`LangVersion preview`)

## Run

```bash
cd src/UnionTest
dotnet run
```

## License

[MIT](LICENSE)
