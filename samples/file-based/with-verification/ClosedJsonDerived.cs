#:property TargetFramework=net11.0
#:property LangVersion=preview
#:property Nullable=enable
#:property JsonSerializerIsReflectionEnabledByDefault=false
#:property WarningsAsErrors=CS8509;CS8655;IL2026;IL3050

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

Payment original = new Cash(100m, new Card("4111", 250m));
string json = JsonSerializer.Serialize(original, PaymentContext.Default.Payment);
Payment restored = JsonSerializer.Deserialize(json, PaymentContext.Default.Payment)
    ?? throw new JsonException("Payment must not be null.");
Console.WriteLine(json);
if (restored is not Cash { Amount: 100m, Next: Card { CardNumber: "4111", Amount: 250m, Next: null } })
    throw new Exception("Round trip failed.");
if (Describe(restored) != "cash:100" || Describe(((Cash)restored).Next!) != "card:4111")
    throw new Exception("Case matching failed.");
if (args.Contains("--require-native") && RuntimeFeature.IsDynamicCodeSupported)
    throw new Exception("Native AOT execution is required.");
if (JsonSerializer.IsReflectionEnabledByDefault)
    throw new Exception("JSON reflection must be disabled.");
Console.WriteLine($"switch: {Describe(restored)} -> {Describe(((Cash)restored).Next!)}");
Console.WriteLine("PASS");

// closedによって全ケースを認識する。_ や null の分岐は不要（入力は非nullable）。
static string Describe(Payment payment) => payment switch
{
    Cash cash => $"cash:{cash.Amount}",
    Card card => $"card:{card.CardNumber}"
};

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Cash), "cash")]
[JsonDerivedType(typeof(Card), "card")]
// closedは暗黙にabstract。abstractを併記するとCS9384になる。
public closed record Payment;
public sealed record Cash(decimal Amount, Payment? Next = null) : Payment;
public sealed record Card(string CardNumber, decimal Amount, Payment? Next = null) : Payment;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment))]
internal partial class PaymentContext : JsonSerializerContext;
