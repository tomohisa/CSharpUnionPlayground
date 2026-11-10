#:property TargetFramework=net11.0
#:property Nullable=enable
#:property WarningsAsErrors=CS8509;CS8655
#:property JsonSerializerIsReflectionEnabledByDefault=false

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

// 共通基底型なし。コンバーターをこのファイル内で手書きする。
Payment inner = new Card("4111", 250m);
Payment original = new Cash(100m, inner);
string json = JsonSerializer.Serialize(original, PaymentContext.Default.Payment);
Payment restored = JsonSerializer.Deserialize(json, PaymentContext.Default.Payment);
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
[JsonConverter(typeof(PaymentConverter))]
public union Payment(Cash, Card);

public sealed class PaymentConverter : JsonConverter<Payment>
{
    public override Payment Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("$type", out var tag) || tag.ValueKind != JsonValueKind.String ||
            !root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
            throw new JsonException("Expected $type and object data.");
        return tag.GetString() switch
        {
            "cash" => new Payment(JsonSerializer.Deserialize(data, Info<Cash>(options))!),
            "card" => new Payment(JsonSerializer.Deserialize(data, Info<Card>(options))!),
            _ => throw new JsonException("Unknown payment tag.")
        };
    }

    public override void Write(Utf8JsonWriter writer, Payment value, JsonSerializerOptions options)
    {
        if (value.Value is null) { writer.WriteNullValue(); return; }
        writer.WriteStartObject();
        switch (value.Value)
        {
            case Cash cash:
                writer.WriteString("$type", "cash");
                writer.WritePropertyName("data");
                JsonSerializer.Serialize(writer, cash, Info<Cash>(options));
                break;
            case Card card:
                writer.WriteString("$type", "card");
                writer.WritePropertyName("data");
                JsonSerializer.Serialize(writer, card, Info<Card>(options));
                break;
            default: throw new JsonException("Unknown payment case.");
        }
        writer.WriteEndObject();
    }

    private static JsonTypeInfo<T> Info<T>(JsonSerializerOptions options) =>
        (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment))]
[JsonSerializable(typeof(Cash))]
[JsonSerializable(typeof(Card))]
internal partial class PaymentContext : JsonSerializerContext;
