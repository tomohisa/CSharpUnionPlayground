#:property TargetFramework=net11.0
#:property LangVersion=preview
#:property Nullable=enable
#:property WarningsAsErrors=CS8509;CS8655
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
Console.WriteLine(Describe(restored));

static string Describe(Payment payment) => payment switch
{
    Cash cash => $"cash:{cash.Amount}",
    Card card => $"card:{card.CardNumber}",
    null => "empty"
};



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
