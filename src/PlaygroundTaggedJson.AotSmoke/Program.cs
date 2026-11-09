using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpUnionPlayground.Experimental.Json;

namespace CSharpUnionPlayground.Experimental.Smoke;

public record Cash(decimal Amount);
public record Card(string CardNumber, decimal Amount);
public record TextValue(string Value);
public record struct NumberValue(int Value);

[PlaygroundTaggedUnion]
[PlaygroundUnionCase(typeof(Cash), "cash")]
[PlaygroundUnionCase(typeof(Card), "card")]
public union CheckoutPayment(Cash, Card);

[PlaygroundTaggedUnion(Discriminator = "kind")]
[PlaygroundUnionCase(typeof(TextValue), "text")]
[PlaygroundUnionCase(typeof(NumberValue), "number")]
public union AmbiguousValue(TextValue, NumberValue);

public record Checkout(string Id, CheckoutPayment Payment, List<AmbiguousValue> Values);

// Authored in the input compilation: do not rely on one generator reading
// another generator's output. Case metadata is explicitly registered for AOT.
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Checkout))]
[JsonSerializable(typeof(CheckoutPayment))]
[JsonSerializable(typeof(AmbiguousValue))]
[JsonSerializable(typeof(Cash))]
[JsonSerializable(typeof(Card))]
[JsonSerializable(typeof(TextValue))]
[JsonSerializable(typeof(NumberValue))]
internal partial class SmokeJsonContext : JsonSerializerContext;

internal static class Program
{
    private static int checks;
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        checks++;
    }

    public static void Main()
    {
        Check(!JsonSerializer.IsReflectionEnabledByDefault, "JSON reflection must be disabled.");
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        options.Converters.Add(new CheckoutPaymentPlaygroundJsonConverter());
        options.Converters.Add(new AmbiguousValuePlaygroundJsonConverter());
        options.TypeInfoResolver = SmokeJsonContext.Default;
        var context = new SmokeJsonContext(options);

        foreach (CheckoutPayment payment in new CheckoutPayment[] { new Cash(100m), new Card("4111", 250m) })
        {
            string json = JsonSerializer.Serialize(payment, context.CheckoutPayment);
            var restored = JsonSerializer.Deserialize(json, context.CheckoutPayment);
            Check(Equals(payment.Value, restored.Value), "Payment round trip failed.");
            Check(json.Contains("\"$type\""), "Missing discriminator.");
            Console.WriteLine(json);
        }
        var checkout = new Checkout("order-1", new Card("4111", 250m), [new TextValue("42"), new NumberValue(42)]);
        var nestedJson = JsonSerializer.Serialize(checkout, context.Checkout);
        var copy = JsonSerializer.Deserialize(nestedJson, context.Checkout)!;
        Check(copy.Id == checkout.Id && Equals(copy.Payment.Value, checkout.Payment.Value), "Nested payment failed.");
        Check(copy.Values.Select(v => v.Value).SequenceEqual(checkout.Values.Select(v => v.Value)), "Same-name property cases failed.");
        Console.WriteLine(nestedJson);
        Check(JsonSerializer.Serialize(default(CheckoutPayment), context.CheckoutPayment) == "null", "Default write failed.");
        Check(JsonSerializer.Deserialize("null", context.CheckoutPayment).Value is null, "Default read failed.");
        var reversed = JsonSerializer.Deserialize("""{"data":{"amount":12},"$type":"cash"}""", context.CheckoutPayment);
        Check(reversed.Value is Cash { Amount: 12m }, "Envelope order should not matter.");
        foreach (string invalid in new[]
        {
            "{}", "[]", "{\"$type\":12,\"data\":{}}",
            "{\"$type\":\"unknown\",\"data\":{}}",
            "{\"$type\":\"cash\",\"data\":null}",
            "{\"$type\":\"cash\",\"$type\":\"card\",\"data\":{}}",
            "{\"$type\":\"cash\",\"data\":{},\"data\":{}}",
            "{\"$type\":\"cash\",\"data\":{},\"extra\":1}",
            "{\"$type\":\"cash\",\"data\":{\"amount\":\"bad\"}}"
        })
        {
            bool rejected = false;
            try { JsonSerializer.Deserialize(invalid, context.CheckoutPayment); }
            catch (JsonException) { rejected = true; }
            Check(rejected, "Invalid payload accepted: " + invalid);
        }
        Console.WriteLine($"PASS: {checks} checks; dynamic code supported: {RuntimeFeature.IsDynamicCodeSupported}; JSON reflection: {JsonSerializer.IsReflectionEnabledByDefault}");
    }
}
