# CSharpUnionPlayground

**English** | [日本語](README_JA.md)

A playground for testing and exploring **C# 15 Union Types** (.NET 11 RC3).

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

## JSON serialization: RC3 verification

SDK `11.0.100-rc.3.26522.148` is pinned in `global.json`. The runtime now supplies `UnionAttribute` and `IUnion`; the Preview 3 polyfill has been removed.

- Distinct JSON token kinds (`int`, `string`, `bool`) round-trip automatically.
- The active case is serialized directly, without a `Value` wrapper or `$type` discriminator. A default union round-trips as `null`.
- Multiple object cases still need a classifier: default deserialization now throws `JsonException` instead of silently returning an empty union.
- `JsonUnionTypeStructuralClassifier` restores object cases by property names, including unions inside a parent object's list with camelCase naming. See `UnionRc3Tests`.
- Union JSON still cannot be deserialized as an abstract polymorphic base without a discriminator. Serialize as the base type to use `[JsonDerivedType]`.

The original desired-behavior tests remain intentionally failing to track the default-setting limitations; they are not skipped or weakened. No Preview 3 rerun was performed for this verification.

Verified result: **81 tests, 54 passed / 27 failed / 0 skipped**. All 7 new RC3 tests pass. The 27 failures comprise 19 direct object-union round trips, 5 container round trips, and 3 union-to-abstract-base conversions.

## Run

```bash
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj --filter FullyQualifiedName~UnionRc3Tests
```

## License

[MIT](LICENSE)
