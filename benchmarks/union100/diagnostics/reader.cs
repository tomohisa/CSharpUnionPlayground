// Specialized, order-independent decoder for the benchmark schema.
// Unknown ordinary properties are skipped, as with the baseline STJ options.
static class DirectReader
{
    public static Payment[] Decode(byte[] json)
    {
        var reader = new Utf8JsonReader(json);
        Require(ref reader, JsonTokenType.StartArray);
        var result = new List<Payment>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            result.Add(Payment(ref reader));
        if (reader.TokenType != JsonTokenType.EndArray || reader.Read()) throw new JsonException();
        return result.ToArray();
    }
    static void Require(ref Utf8JsonReader r, JsonTokenType token)
    {
        if (!r.Read() || r.TokenType != token) throw new JsonException();
    }
    static Payment Payment(ref Utf8JsonReader r)
    {
        if (r.TokenType != JsonTokenType.StartObject) throw new JsonException();
        string? tag = null;
        Payload? data = null;
        while (r.Read() && r.TokenType != JsonTokenType.EndObject)
        {
            if (r.TokenType != JsonTokenType.PropertyName) throw new JsonException();
            if (r.ValueTextEquals("$type"u8))
            {
                if (tag is not null) throw new JsonException("Duplicate discriminator");
                Require(ref r, JsonTokenType.String); tag = r.GetString();
            }
            else if (r.ValueTextEquals("data"u8))
            {
                Require(ref r, JsonTokenType.StartObject); data = Payload(ref r);
            }
            else { if (!r.Read()) throw new JsonException(); r.Skip(); }
        }
        if (r.TokenType != JsonTokenType.EndObject || data is null) throw new JsonException();
        return tag switch
        {
@@CASES@@
            _ => throw new JsonException("Unknown/missing discriminator")
        };
    }
    static Payload Payload(ref Utf8JsonReader r)
    {
        int id = 0; string? label = null; int[]? values = null;
        Dictionary<string,string>? tags = null; Payment? next = null;
        while (r.Read() && r.TokenType != JsonTokenType.EndObject)
        {
            if (r.TokenType != JsonTokenType.PropertyName) throw new JsonException();
            if (r.ValueTextEquals("id"u8)) { if (!r.Read()) throw new JsonException(); id = r.GetInt32(); }
            else if (r.ValueTextEquals("label"u8)) { Require(ref r, JsonTokenType.String); label = r.GetString(); }
            else if (r.ValueTextEquals("values"u8))
            {
                Require(ref r, JsonTokenType.StartArray);
                var items = new List<int>();
                while (r.Read() && r.TokenType != JsonTokenType.EndArray) items.Add(r.GetInt32());
                if (r.TokenType != JsonTokenType.EndArray) throw new JsonException();
                values = items.ToArray();
            }
            else if (r.ValueTextEquals("tags"u8))
            {
                Require(ref r, JsonTokenType.StartObject); tags = new();
                while (r.Read() && r.TokenType != JsonTokenType.EndObject)
                {
                    if (r.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                    string key = r.GetString()!; Require(ref r, JsonTokenType.String); tags[key] = r.GetString()!;
                }
                if (r.TokenType != JsonTokenType.EndObject) throw new JsonException();
            }
            else if (r.ValueTextEquals("next"u8))
            {
                if (!r.Read()) throw new JsonException();
                next = r.TokenType == JsonTokenType.Null ? null : Payment(ref r);
            }
            else { if (!r.Read()) throw new JsonException(); r.Skip(); }
        }
        if (r.TokenType != JsonTokenType.EndObject || label is null || values is null || tags is null) throw new JsonException();
        return new(id, label, values, tags, next);
    }
}
