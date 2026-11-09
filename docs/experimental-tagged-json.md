# Experimental tagged union JSON generator

This is a repository experiment, not a released package or a stable API. Names live under `CSharpUnionPlayground.Experimental` to avoid implying a framework feature. Requires the pinned .NET 11 RC3 SDK.

## Usage

Reference the generator project as an analyzer:

```xml
<ProjectReference Include="../PlaygroundTaggedJson.Generator/PlaygroundTaggedJson.Generator.csproj"
                  OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
```

Define unrelated case types and register each case explicitly:

```csharp
using CSharpUnionPlayground.Experimental.Json;

public record Cash(decimal Amount);
public record Card(string CardNumber, decimal Amount);

[PlaygroundTaggedUnion] // Optional: Discriminator = "kind"
[PlaygroundUnionCase(typeof(Cash), "cash")]
[PlaygroundUnionCase(typeof(Card), "card")]
public union CheckoutPayment(Cash, Card);
```

The generator emits `CheckoutPaymentPlaygroundJsonConverter` in the union's namespace. No common base, interface, or handwritten converter is needed. The generated attributes are internal to the consumer assembly; there is no runtime generator dependency.

Explicitly register the union, all case types, and parent models in an **authored** System.Text.Json context:

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(CheckoutPayment))]
[JsonSerializable(typeof(Cash))]
[JsonSerializable(typeof(Card))]
internal partial class AppJsonContext : JsonSerializerContext;

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    TypeInfoResolver = AppJsonContext.Default
};
options.Converters.Add(new CheckoutPaymentPlaygroundJsonConverter());
var context = new AppJsonContext(options);

CheckoutPayment payment = new Card("4111", 250m);
string json = JsonSerializer.Serialize(payment, context.CheckoutPayment);
CheckoutPayment copy = JsonSerializer.Deserialize(json, context.CheckoutPayment);
```

Output:

```json
{"$type":"card","data":{"cardNumber":"4111","amount":250}}
```

The envelope isolates metadata from case fields. Discriminator and `data` names do not follow the property naming policy; the policy applies inside `data`. Reads accept either envelope property order. Unknown tags, missing/duplicate envelope fields, null case data, and extra envelope fields throw `JsonException`. A default/empty union reads and writes JSON `null`; this format intentionally does not distinguish that state from a case holding null.

## Why explicit registration?

Generators cannot consume one another's generated source in the same compilation. In this experiment, putting `[JsonConverter(typeof(GeneratedConverter))]` on the union caused System.Text.Json's generator to reject the not-yet-visible converter type. Therefore converter registration and context declarations are authored input. The sample narrowly suppresses `SYSLIB1227`: the default union classifier would be ambiguous, but explicitly registered generated converters replace those contracts. AOT warnings `IL2026` and `IL3050` are errors, not suppressed.

The generated converter calls the `JsonTypeInfo<T>` serializer overloads and retrieves child metadata from the supplied options. No runtime attribute scanning, `MakeGenericType`, dynamic code generation, or default reflection resolver is used. Missing child context registrations are configuration errors. Callers must keep the generated converter registration and source-generated metadata together.

## Verification

```bash
# Real compiler diagnostic fixtures: 6 pass
python3 scripts/verify_tagged_json_diagnostics.py

# Smoke checks with JSON reflection disabled
# PublishAot is enabled on this project; some SDK run modes may also use AOT.
dotnet run --project src/PlaygroundTaggedJson.AotSmoke

# Explicit Native AOT publish and direct native execution on this host
dotnet publish src/PlaygroundTaggedJson.AotSmoke -c Release -r osx-arm64 \
  -o /tmp/csharp-union-tagged-aot
file /tmp/csharp-union-tagged-aot/PlaygroundTaggedJson.AotSmoke
/tmp/csharp-union-tagged-aot/PlaygroundTaggedJson.AotSmoke
```

Observed with SDK `11.0.100-rc.3.26522.148` on macOS arm64:

- Published file: `Mach-O 64-bit executable arm64`.
- 19 smoke checks pass, with `RuntimeFeature.IsDynamicCodeSupported == false` and `JsonSerializer.IsReflectionEnabledByDefault == false`.
- Cash/Card cases; nested parent record; list of cases sharing the `value` property (record class and record struct); custom discriminator; default/null union; reversed field order; nine invalid payloads.
- Six diagnostic fixtures pass: duplicate tag, missing case, unrelated type, non-union target, reserved discriminator, generic union. Each requires `CUPJ001` from the real RC3 compiler.
- No IL2026/IL3050 warnings. Apple's linker reports duplicate debug-map entries in the SDK Native AOT runtime libraries; publishing and execution succeed.
- Existing suite remains 86 tests: 77 passed and the same 9 previously documented failures. This experiment is separate from those models and does not rewrite their JSON contracts.

## Prototype limits

- Only top-level, non-generic unions; one explicit, unique type/tag registration for every case. The current compiler's union syntax node name and synthesized constructors are used for validation; SDK updates may require changes.
- Null cases, derived runtime subtypes, generic/nested unions, converter name collisions, version migration, and package distribution are not supported features. Runtime subtypes are rejected rather than silently serialized as a registered base case.
- Extra envelope fields are rejected. Child data validation follows the supplied System.Text.Json options; required fields are not automatically enforced by this generator.
- `JsonDocument` buffers each union payload. This prototype prioritizes correctness and AOT feasibility; it is not benchmarked or streaming optimized.
- Native execution verified only on macOS arm64. Other platforms require their own Native AOT publish/run checks.

References: [System.Text.Json source generation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation), [custom converters](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to), [Native AOT prerequisites](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/).
