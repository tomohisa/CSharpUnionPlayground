using System.Text.Json;

namespace CSharpUnionPlayground.Experimental.Smoke;

// Both unions use [PlaygroundTaggedUnion] without a discriminator override.
// The definitions, metadata and converter registrations are in Program.cs.
internal static class NestedUnionSample
{
    internal static void VerifyRoundTrips(SmokeJsonContext context, Action<bool, string> check)
    {
        // Cover every combination of the outer and inner case types.
        foreach (AmbiguousValue detail in new AmbiguousValue[] { new TextValue("42"), new NumberValue(42) })
        {
            foreach (CheckoutPayment original in new CheckoutPayment[]
            {
                new Cash(100m) { Detail = detail },
                new Card("4111", 250m) { Detail = detail }
            })
            {
                string json = JsonSerializer.Serialize(original, context.CheckoutPayment);
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                var inner = root.GetProperty("data").GetProperty("detail");

                check(root.GetProperty("$type").GetString() == (original.Value is Cash ? "cash" : "card"),
                    "Outer discriminator must be $type with the correct tag.");
                check(inner.GetProperty("$type").GetString() == (detail.Value is TextValue ? "text" : "number"),
                    "Inner discriminator must also be $type with the correct tag.");
                check(!inner.TryGetProperty("kind", out _), "The old kind discriminator must not be emitted.");

                CheckoutPayment restored = JsonSerializer.Deserialize(json, context.CheckoutPayment);
                check(Equals(original.Value, restored.Value), "Outer case and all fields must round-trip.");
                AmbiguousValue restoredDetail = restored.Value switch
                {
                    Cash cash => cash.Detail,
                    Card card => card.Detail,
                    _ => throw new InvalidOperationException("Unexpected outer case.")
                };
                check(Equals(detail.Value, restoredDetail.Value),
                    "Inner case and value must round-trip, including string versus number.");
                Console.WriteLine(json);
            }
        }
    }
}
