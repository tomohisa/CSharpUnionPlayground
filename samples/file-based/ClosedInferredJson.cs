#:property TargetFramework=net11.0
#:property Nullable=enable
#:property JsonSerializerIsReflectionEnabledByDefault=false
#:property WarningsAsErrors=CS8509;CS8655

using System.Text.Json;
using System.Text.Json.Serialization;

Payment original = new Cash(100m, new Card("4111", 250m));
string json = JsonSerializer.Serialize(original, PaymentContext.Default.Payment);
Payment restored = JsonSerializer.Deserialize(json, PaymentContext.Default.Payment)
    ?? throw new JsonException("Payment must not be null.");
Console.WriteLine(json);
Console.WriteLine(Describe(restored));

static string Describe(Payment payment) => payment switch
{
    Cash cash => $"cash:{cash.Amount}",
    Card card => $"card:{card.CardNumber}"
};

// No JsonDerivedType list: the standard generator infers cases and their tags.
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
// closedは暗黙にabstract。abstractを併記するとCS9384になる。
public closed record Payment;
public sealed record Cash(decimal Amount, Payment? Next = null) : Payment;
public sealed record Card(string CardNumber, decimal Amount, Payment? Next = null) : Payment;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment))]
internal partial class PaymentContext : JsonSerializerContext;
