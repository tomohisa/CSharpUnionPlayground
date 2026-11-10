using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

Console.WriteLine("{\"event\":\"ready\"}");
int roots = int.Parse(args[0]), samples = int.Parse(args[1]);
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, TypeInfoResolver = BenchContext.Default };

var context = new BenchContext(options);
var warm = Enumerable.Range(0, Math.Min(roots, 1000)).Select(i => Create(i, 3)).ToArray();
_ = JsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(warm, context.PaymentArray), context.PaymentArray);
var probe = JsonSerializer.SerializeToUtf8Bytes(warm, context.PaymentArray);
if (!probe.AsSpan().SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(DirectReader.Decode(probe), context.PaymentArray))) throw new Exception("Direct reader probe failed");
var original = Enumerable.Range(0, roots).Select(i => Create(i, 3)).ToArray();
long expected = original.Sum(Score), sink = 0;
var measurements = new List<Measurement>();
Console.WriteLine("{\"event\":\"prepared\"}");
for (int trial = 0; trial < samples; trial++)
{
    long start = Stopwatch.GetTimestamp();
    byte[] json = JsonSerializer.SerializeToUtf8Bytes(original, context.PaymentArray);
    long encoded = Stopwatch.GetTimestamp();
    long allocated = GC.GetAllocatedBytesForCurrentThread();
    var copy = args[2] == "direct" ? DirectReader.Decode(json) : JsonSerializer.Deserialize(json, context.PaymentArray)!;
    long decodeAllocated = GC.GetAllocatedBytesForCurrentThread() - allocated;
    long decoded = Stopwatch.GetTimestamp();
    long sum = copy.Sum(Score);
    long end = Stopwatch.GetTimestamp();
    if (sum != expected || copy.Length != roots) throw new Exception("Incorrect round trip");
    if (trial == 0 && !json.AsSpan().SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(copy, context.PaymentArray))) throw new Exception("Data changed");
    sink += sum;
    measurements.Add(new Measurement((encoded-start)/(double)Stopwatch.Frequency, (decoded-encoded)/(double)Stopwatch.Frequency, (end-decoded)/(double)Stopwatch.Frequency, (end-start)/(double)Stopwatch.Frequency, json.Length, decodeAllocated));
}
Console.WriteLine(JsonSerializer.Serialize(new Result(roots, samples, sink, measurements), context.Result));

static Payment Create(int seed, int depth)
{
    Payment? next = null;
    if (depth > 1) next = Create(seed+37, depth-1);
    var data = new Payload(seed, "item-" + seed, [seed, seed+1, seed+2, seed+3], new() { ["group"] = "g"+(seed%7), ["note"] = "bench" }, next);
    return (seed % 100) switch
    {
    0 => new Case000(data),
    1 => new Case001(data),
    2 => new Case002(data),
    3 => new Case003(data),
    4 => new Case004(data),
    5 => new Case005(data),
    6 => new Case006(data),
    7 => new Case007(data),
    8 => new Case008(data),
    9 => new Case009(data),
    10 => new Case010(data),
    11 => new Case011(data),
    12 => new Case012(data),
    13 => new Case013(data),
    14 => new Case014(data),
    15 => new Case015(data),
    16 => new Case016(data),
    17 => new Case017(data),
    18 => new Case018(data),
    19 => new Case019(data),
    20 => new Case020(data),
    21 => new Case021(data),
    22 => new Case022(data),
    23 => new Case023(data),
    24 => new Case024(data),
    25 => new Case025(data),
    26 => new Case026(data),
    27 => new Case027(data),
    28 => new Case028(data),
    29 => new Case029(data),
    30 => new Case030(data),
    31 => new Case031(data),
    32 => new Case032(data),
    33 => new Case033(data),
    34 => new Case034(data),
    35 => new Case035(data),
    36 => new Case036(data),
    37 => new Case037(data),
    38 => new Case038(data),
    39 => new Case039(data),
    40 => new Case040(data),
    41 => new Case041(data),
    42 => new Case042(data),
    43 => new Case043(data),
    44 => new Case044(data),
    45 => new Case045(data),
    46 => new Case046(data),
    47 => new Case047(data),
    48 => new Case048(data),
    49 => new Case049(data),
    50 => new Case050(data),
    51 => new Case051(data),
    52 => new Case052(data),
    53 => new Case053(data),
    54 => new Case054(data),
    55 => new Case055(data),
    56 => new Case056(data),
    57 => new Case057(data),
    58 => new Case058(data),
    59 => new Case059(data),
    60 => new Case060(data),
    61 => new Case061(data),
    62 => new Case062(data),
    63 => new Case063(data),
    64 => new Case064(data),
    65 => new Case065(data),
    66 => new Case066(data),
    67 => new Case067(data),
    68 => new Case068(data),
    69 => new Case069(data),
    70 => new Case070(data),
    71 => new Case071(data),
    72 => new Case072(data),
    73 => new Case073(data),
    74 => new Case074(data),
    75 => new Case075(data),
    76 => new Case076(data),
    77 => new Case077(data),
    78 => new Case078(data),
    79 => new Case079(data),
    80 => new Case080(data),
    81 => new Case081(data),
    82 => new Case082(data),
    83 => new Case083(data),
    84 => new Case084(data),
    85 => new Case085(data),
    86 => new Case086(data),
    87 => new Case087(data),
    88 => new Case088(data),
    89 => new Case089(data),
    90 => new Case090(data),
    91 => new Case091(data),
    92 => new Case092(data),
    93 => new Case093(data),
    94 => new Case094(data),
    95 => new Case095(data),
    96 => new Case096(data),
    97 => new Case097(data),
    98 => new Case098(data),
    99 => new Case099(data),
        _ => throw new Exception()
    };
}
static long Score(Payment payment)
{
    (Payload data, int index) = payment switch
    {
    Case000 c => (c.Data, 0),
    Case001 c => (c.Data, 1),
    Case002 c => (c.Data, 2),
    Case003 c => (c.Data, 3),
    Case004 c => (c.Data, 4),
    Case005 c => (c.Data, 5),
    Case006 c => (c.Data, 6),
    Case007 c => (c.Data, 7),
    Case008 c => (c.Data, 8),
    Case009 c => (c.Data, 9),
    Case010 c => (c.Data, 10),
    Case011 c => (c.Data, 11),
    Case012 c => (c.Data, 12),
    Case013 c => (c.Data, 13),
    Case014 c => (c.Data, 14),
    Case015 c => (c.Data, 15),
    Case016 c => (c.Data, 16),
    Case017 c => (c.Data, 17),
    Case018 c => (c.Data, 18),
    Case019 c => (c.Data, 19),
    Case020 c => (c.Data, 20),
    Case021 c => (c.Data, 21),
    Case022 c => (c.Data, 22),
    Case023 c => (c.Data, 23),
    Case024 c => (c.Data, 24),
    Case025 c => (c.Data, 25),
    Case026 c => (c.Data, 26),
    Case027 c => (c.Data, 27),
    Case028 c => (c.Data, 28),
    Case029 c => (c.Data, 29),
    Case030 c => (c.Data, 30),
    Case031 c => (c.Data, 31),
    Case032 c => (c.Data, 32),
    Case033 c => (c.Data, 33),
    Case034 c => (c.Data, 34),
    Case035 c => (c.Data, 35),
    Case036 c => (c.Data, 36),
    Case037 c => (c.Data, 37),
    Case038 c => (c.Data, 38),
    Case039 c => (c.Data, 39),
    Case040 c => (c.Data, 40),
    Case041 c => (c.Data, 41),
    Case042 c => (c.Data, 42),
    Case043 c => (c.Data, 43),
    Case044 c => (c.Data, 44),
    Case045 c => (c.Data, 45),
    Case046 c => (c.Data, 46),
    Case047 c => (c.Data, 47),
    Case048 c => (c.Data, 48),
    Case049 c => (c.Data, 49),
    Case050 c => (c.Data, 50),
    Case051 c => (c.Data, 51),
    Case052 c => (c.Data, 52),
    Case053 c => (c.Data, 53),
    Case054 c => (c.Data, 54),
    Case055 c => (c.Data, 55),
    Case056 c => (c.Data, 56),
    Case057 c => (c.Data, 57),
    Case058 c => (c.Data, 58),
    Case059 c => (c.Data, 59),
    Case060 c => (c.Data, 60),
    Case061 c => (c.Data, 61),
    Case062 c => (c.Data, 62),
    Case063 c => (c.Data, 63),
    Case064 c => (c.Data, 64),
    Case065 c => (c.Data, 65),
    Case066 c => (c.Data, 66),
    Case067 c => (c.Data, 67),
    Case068 c => (c.Data, 68),
    Case069 c => (c.Data, 69),
    Case070 c => (c.Data, 70),
    Case071 c => (c.Data, 71),
    Case072 c => (c.Data, 72),
    Case073 c => (c.Data, 73),
    Case074 c => (c.Data, 74),
    Case075 c => (c.Data, 75),
    Case076 c => (c.Data, 76),
    Case077 c => (c.Data, 77),
    Case078 c => (c.Data, 78),
    Case079 c => (c.Data, 79),
    Case080 c => (c.Data, 80),
    Case081 c => (c.Data, 81),
    Case082 c => (c.Data, 82),
    Case083 c => (c.Data, 83),
    Case084 c => (c.Data, 84),
    Case085 c => (c.Data, 85),
    Case086 c => (c.Data, 86),
    Case087 c => (c.Data, 87),
    Case088 c => (c.Data, 88),
    Case089 c => (c.Data, 89),
    Case090 c => (c.Data, 90),
    Case091 c => (c.Data, 91),
    Case092 c => (c.Data, 92),
    Case093 c => (c.Data, 93),
    Case094 c => (c.Data, 94),
    Case095 c => (c.Data, 95),
    Case096 c => (c.Data, 96),
    Case097 c => (c.Data, 97),
    Case098 c => (c.Data, 98),
    Case099 c => (c.Data, 99),
    
    };
    return index + data.Id + data.Label.Length + data.Values.Sum(v => (long)v) + data.Tags.Sum(p => p.Key.Length + p.Value.Length) + (data.Next is { } next ? Score(next) : 0);
}
public record Payload(int Id, string Label, int[] Values, Dictionary<string,string> Tags, Payment? Next);
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Payment;
public sealed record Case000(Payload Data) : Payment;
public sealed record Case001(Payload Data) : Payment;
public sealed record Case002(Payload Data) : Payment;
public sealed record Case003(Payload Data) : Payment;
public sealed record Case004(Payload Data) : Payment;
public sealed record Case005(Payload Data) : Payment;
public sealed record Case006(Payload Data) : Payment;
public sealed record Case007(Payload Data) : Payment;
public sealed record Case008(Payload Data) : Payment;
public sealed record Case009(Payload Data) : Payment;
public sealed record Case010(Payload Data) : Payment;
public sealed record Case011(Payload Data) : Payment;
public sealed record Case012(Payload Data) : Payment;
public sealed record Case013(Payload Data) : Payment;
public sealed record Case014(Payload Data) : Payment;
public sealed record Case015(Payload Data) : Payment;
public sealed record Case016(Payload Data) : Payment;
public sealed record Case017(Payload Data) : Payment;
public sealed record Case018(Payload Data) : Payment;
public sealed record Case019(Payload Data) : Payment;
public sealed record Case020(Payload Data) : Payment;
public sealed record Case021(Payload Data) : Payment;
public sealed record Case022(Payload Data) : Payment;
public sealed record Case023(Payload Data) : Payment;
public sealed record Case024(Payload Data) : Payment;
public sealed record Case025(Payload Data) : Payment;
public sealed record Case026(Payload Data) : Payment;
public sealed record Case027(Payload Data) : Payment;
public sealed record Case028(Payload Data) : Payment;
public sealed record Case029(Payload Data) : Payment;
public sealed record Case030(Payload Data) : Payment;
public sealed record Case031(Payload Data) : Payment;
public sealed record Case032(Payload Data) : Payment;
public sealed record Case033(Payload Data) : Payment;
public sealed record Case034(Payload Data) : Payment;
public sealed record Case035(Payload Data) : Payment;
public sealed record Case036(Payload Data) : Payment;
public sealed record Case037(Payload Data) : Payment;
public sealed record Case038(Payload Data) : Payment;
public sealed record Case039(Payload Data) : Payment;
public sealed record Case040(Payload Data) : Payment;
public sealed record Case041(Payload Data) : Payment;
public sealed record Case042(Payload Data) : Payment;
public sealed record Case043(Payload Data) : Payment;
public sealed record Case044(Payload Data) : Payment;
public sealed record Case045(Payload Data) : Payment;
public sealed record Case046(Payload Data) : Payment;
public sealed record Case047(Payload Data) : Payment;
public sealed record Case048(Payload Data) : Payment;
public sealed record Case049(Payload Data) : Payment;
public sealed record Case050(Payload Data) : Payment;
public sealed record Case051(Payload Data) : Payment;
public sealed record Case052(Payload Data) : Payment;
public sealed record Case053(Payload Data) : Payment;
public sealed record Case054(Payload Data) : Payment;
public sealed record Case055(Payload Data) : Payment;
public sealed record Case056(Payload Data) : Payment;
public sealed record Case057(Payload Data) : Payment;
public sealed record Case058(Payload Data) : Payment;
public sealed record Case059(Payload Data) : Payment;
public sealed record Case060(Payload Data) : Payment;
public sealed record Case061(Payload Data) : Payment;
public sealed record Case062(Payload Data) : Payment;
public sealed record Case063(Payload Data) : Payment;
public sealed record Case064(Payload Data) : Payment;
public sealed record Case065(Payload Data) : Payment;
public sealed record Case066(Payload Data) : Payment;
public sealed record Case067(Payload Data) : Payment;
public sealed record Case068(Payload Data) : Payment;
public sealed record Case069(Payload Data) : Payment;
public sealed record Case070(Payload Data) : Payment;
public sealed record Case071(Payload Data) : Payment;
public sealed record Case072(Payload Data) : Payment;
public sealed record Case073(Payload Data) : Payment;
public sealed record Case074(Payload Data) : Payment;
public sealed record Case075(Payload Data) : Payment;
public sealed record Case076(Payload Data) : Payment;
public sealed record Case077(Payload Data) : Payment;
public sealed record Case078(Payload Data) : Payment;
public sealed record Case079(Payload Data) : Payment;
public sealed record Case080(Payload Data) : Payment;
public sealed record Case081(Payload Data) : Payment;
public sealed record Case082(Payload Data) : Payment;
public sealed record Case083(Payload Data) : Payment;
public sealed record Case084(Payload Data) : Payment;
public sealed record Case085(Payload Data) : Payment;
public sealed record Case086(Payload Data) : Payment;
public sealed record Case087(Payload Data) : Payment;
public sealed record Case088(Payload Data) : Payment;
public sealed record Case089(Payload Data) : Payment;
public sealed record Case090(Payload Data) : Payment;
public sealed record Case091(Payload Data) : Payment;
public sealed record Case092(Payload Data) : Payment;
public sealed record Case093(Payload Data) : Payment;
public sealed record Case094(Payload Data) : Payment;
public sealed record Case095(Payload Data) : Payment;
public sealed record Case096(Payload Data) : Payment;
public sealed record Case097(Payload Data) : Payment;
public sealed record Case098(Payload Data) : Payment;
public sealed record Case099(Payload Data) : Payment;

public record Measurement(double EncodeSeconds, double DecodeSeconds, double SwitchSeconds, double TotalSeconds, int Bytes, long DecodeAllocatedBytes);
public record Result(int Roots, int Samples, long Sink, List<Measurement> Measurements);
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment[]))]
[JsonSerializable(typeof(Result))]

internal partial class BenchContext : JsonSerializerContext;

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
            "Case000" => new Case000(data),
            "Case001" => new Case001(data),
            "Case002" => new Case002(data),
            "Case003" => new Case003(data),
            "Case004" => new Case004(data),
            "Case005" => new Case005(data),
            "Case006" => new Case006(data),
            "Case007" => new Case007(data),
            "Case008" => new Case008(data),
            "Case009" => new Case009(data),
            "Case010" => new Case010(data),
            "Case011" => new Case011(data),
            "Case012" => new Case012(data),
            "Case013" => new Case013(data),
            "Case014" => new Case014(data),
            "Case015" => new Case015(data),
            "Case016" => new Case016(data),
            "Case017" => new Case017(data),
            "Case018" => new Case018(data),
            "Case019" => new Case019(data),
            "Case020" => new Case020(data),
            "Case021" => new Case021(data),
            "Case022" => new Case022(data),
            "Case023" => new Case023(data),
            "Case024" => new Case024(data),
            "Case025" => new Case025(data),
            "Case026" => new Case026(data),
            "Case027" => new Case027(data),
            "Case028" => new Case028(data),
            "Case029" => new Case029(data),
            "Case030" => new Case030(data),
            "Case031" => new Case031(data),
            "Case032" => new Case032(data),
            "Case033" => new Case033(data),
            "Case034" => new Case034(data),
            "Case035" => new Case035(data),
            "Case036" => new Case036(data),
            "Case037" => new Case037(data),
            "Case038" => new Case038(data),
            "Case039" => new Case039(data),
            "Case040" => new Case040(data),
            "Case041" => new Case041(data),
            "Case042" => new Case042(data),
            "Case043" => new Case043(data),
            "Case044" => new Case044(data),
            "Case045" => new Case045(data),
            "Case046" => new Case046(data),
            "Case047" => new Case047(data),
            "Case048" => new Case048(data),
            "Case049" => new Case049(data),
            "Case050" => new Case050(data),
            "Case051" => new Case051(data),
            "Case052" => new Case052(data),
            "Case053" => new Case053(data),
            "Case054" => new Case054(data),
            "Case055" => new Case055(data),
            "Case056" => new Case056(data),
            "Case057" => new Case057(data),
            "Case058" => new Case058(data),
            "Case059" => new Case059(data),
            "Case060" => new Case060(data),
            "Case061" => new Case061(data),
            "Case062" => new Case062(data),
            "Case063" => new Case063(data),
            "Case064" => new Case064(data),
            "Case065" => new Case065(data),
            "Case066" => new Case066(data),
            "Case067" => new Case067(data),
            "Case068" => new Case068(data),
            "Case069" => new Case069(data),
            "Case070" => new Case070(data),
            "Case071" => new Case071(data),
            "Case072" => new Case072(data),
            "Case073" => new Case073(data),
            "Case074" => new Case074(data),
            "Case075" => new Case075(data),
            "Case076" => new Case076(data),
            "Case077" => new Case077(data),
            "Case078" => new Case078(data),
            "Case079" => new Case079(data),
            "Case080" => new Case080(data),
            "Case081" => new Case081(data),
            "Case082" => new Case082(data),
            "Case083" => new Case083(data),
            "Case084" => new Case084(data),
            "Case085" => new Case085(data),
            "Case086" => new Case086(data),
            "Case087" => new Case087(data),
            "Case088" => new Case088(data),
            "Case089" => new Case089(data),
            "Case090" => new Case090(data),
            "Case091" => new Case091(data),
            "Case092" => new Case092(data),
            "Case093" => new Case093(data),
            "Case094" => new Case094(data),
            "Case095" => new Case095(data),
            "Case096" => new Case096(data),
            "Case097" => new Case097(data),
            "Case098" => new Case098(data),
            "Case099" => new Case099(data),
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
