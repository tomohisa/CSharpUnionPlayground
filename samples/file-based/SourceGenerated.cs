#:property TargetFramework=net11.0
#:property LangVersion=preview
#:property JsonSerializerIsReflectionEnabledByDefault=false
// 標準のunion判別は使わず、生成コンバーターを明示登録する。
#:property NoWarn=SYSLIB1227
#:project ../../src/PlaygroundTaggedJson.Generator/PlaygroundTaggedJson.Generator.csproj OutputItemType=Analyzer ReferenceOutputAssembly=false

using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpUnionPlayground.Experimental.Json;

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    TypeInfoResolver = PaymentContext.Default
};
options.Converters.Add(new PaymentPlaygroundJsonConverter());
var context = new PaymentContext(options);

Payment inner = new Card("4111", 250m);
Payment original = new Cash(100m, inner);
string json = JsonSerializer.Serialize(original, context.Payment);
Payment restored = JsonSerializer.Deserialize(json, context.Payment);
Console.WriteLine(json);
if (restored.Value is not Cash { Amount: 100m, Next: { } next } ||
    next.Value is not Card { CardNumber: "4111", Amount: 250m })
    throw new Exception("Round trip failed.");
Console.WriteLine("PASS");

public record Cash(decimal Amount, Payment? Next = null);
public record Card(string CardNumber, decimal Amount, Payment? Next = null);
[PlaygroundTaggedUnion]
[PlaygroundUnionCase(typeof(Cash), "cash")]
[PlaygroundUnionCase(typeof(Card), "card")]
public union Payment(Cash, Card);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment))]
[JsonSerializable(typeof(Cash))]
[JsonSerializable(typeof(Card))]
internal partial class PaymentContext : JsonSerializerContext;
