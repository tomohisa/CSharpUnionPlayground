public sealed class PaymentConverter : JsonConverter<Payment>
{
    public override Payment Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var data = root.GetProperty("data");
        return root.GetProperty("$type").GetString() switch
        {
@@READ@@
            _ => throw new JsonException("Unknown case")
        };
    }
    public override void Write(Utf8JsonWriter writer, Payment value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        switch (value.Value)
        {
@@WRITE@@
            default: throw new JsonException("Unknown case");
        }
        writer.WriteEndObject();
    }
}
