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

The initial results below used default settings; see the next section for current classifier settings and results. No Preview 3 rerun was performed for this verification.

Verified result: **81 tests, 54 passed / 27 failed / 0 skipped**. All 7 new RC3 tests pass. The 27 failures comprise 19 direct object-union round trips, 5 container round trips, and 3 union-to-abstract-base conversions.

## Applying structural classification to existing tests

Existing assertions are unchanged. All three JSON test classes now use the structural classifier, `RespectRequiredConstructorParameters = true`, and `UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow`, with camelCase naming.

| Configuration | Original 27 failures fixed | Previously passing tests now failing | Original 81 tests: pass / fail |
|---|---:|---:|---:|
| RC3 defaults | 0 | 0 | 54 / 27 |
| Structural classifier only | 14 | 11 | 57 / 24 |
| Classifier + strict settings (current) | 21 | 3 | 72 / 9 |

With five additional remedy checks: **86 tests, 77 passed / 9 failed / 0 skipped**. All five remedy checks pass.

The classifier alone restores PersonInfo, ComplexData, MixedUnion (10 round trips), and four non-Payment containers. Animal, Varied, and Payment cases have overlapping property sets. Requiring constructor arguments and rejecting unknown properties makes another seven original round trips pass. These stricter settings reject missing required arguments and extra JSON properties, changing input compatibility. Contract validation runs during serialization too, so an invalid classifier configuration can break writes.

Remaining failures:

- **ResultUnion: 6 tests** (3 original read failures and 3 new write failures). StringResult and IntResult both recognize `value`; the structural classifier does not examine property values. Even ListResult fails because the whole union contract is rejected. Use a custom `JsonTypeClassifierFactory` that inspects value tokens, a converter, or distinct JSON property names. Separate write-only options without the classifier avoid the write failures but do not solve reads.
- **Union JSON to abstract Payment: 3 tests**. Union serialization emits no `$type`; the classifier does not add polymorphic base metadata. Serialize explicitly as `Payment` (existing passing tests), deserialize as `PaymentUnion` (verified with strict settings), or define a shared JSON contract in a converter.

`UnionStructuralRemedyTests` verifies the strict settings and remaining limitations. No tests were skipped or assertions weakened. Custom classifiers, converters, and renamed properties were not implemented.

Reference: [Microsoft union serialization documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/libraries#union-serialization).

## Experimental attribute-driven converter generation

An unreleased Source Generator prototype supports tagged union JSON without a common base class. Native AOT publish/run verified on macOS arm64: 40 smoke checks pass (`--require-native`), including all four combinations of nested unions using `$type` at both levels with JSON reflection and dynamic code disabled; 6 compiler diagnostic fixtures pass. See [usage, verification and limitations](docs/experimental-tagged-json.md).

## Run

Default examples contain usage code only; assertions and runtime verification live in [with-verification](samples/file-based/with-verification/README.md).


For nine independent single-source Payment examples (five C# approaches, Rust, TypeScript, Java, and Go), see [file-based samples](samples/file-based/README.md). See [switch/match exhaustiveness analysis](samples/file-based/EXHAUSTIVENESS.md) for compiler-verified comparisons, including sixteen Rust/TypeScript/Java/Go mutation fixtures. The C# closed hierarchy examples also verify Native AOT round trips, including standard InferClosedTypePolymorphism without JsonDerivedType case lists.

```bash
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj --filter FullyQualifiedName~UnionRc3Tests
```

## License

[MIT](LICENSE)
