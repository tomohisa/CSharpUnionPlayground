#:property TargetFramework=net11.0
#:property Nullable=enable
#:property WarningsAsErrors=CS8509;CS8655
#:property JsonSerializerIsReflectionEnabledByDefault=false

using System.Text.Json;
using System.Text.Json.Serialization;

// 共通基底型 + 標準属性。内側のPaymentにも$typeが付く。
Payment original = new Cash(100m, new Card("4111", 250m));
string json = JsonSerializer.Serialize(original, PaymentContext.Default.Payment);
Payment restored = JsonSerializer.Deserialize(json, PaymentContext.Default.Payment)
    ?? throw new JsonException("Payment must not be null.");
Console.WriteLine(json);
Console.WriteLine(Describe(restored));

static string Describe(Payment payment) => payment switch
{
    Cash cash => $"cash:{cash.Amount}",
    Card card => $"card:{card.CardNumber}",
    _ => throw new InvalidOperationException("Unknown payment case.")
};



[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Cash), "cash")]
[JsonDerivedType(typeof(Card), "card")]
public abstract record Payment;
public record Cash(decimal Amount, Payment? Next = null) : Payment;
public record Card(string CardNumber, decimal Amount, Payment? Next = null) : Payment;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment))]
internal partial class PaymentContext : JsonSerializerContext;
