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
var original = Enumerable.Range(0, roots).Select(i => Create(i, 3)).ToArray();
long expected = original.Sum(Score), sink = 0;
var measurements = new List<Measurement>();
Console.WriteLine("{\"event\":\"prepared\"}");
for (int trial = 0; trial < samples; trial++)
{
    long start = Stopwatch.GetTimestamp();
    byte[] json = JsonSerializer.SerializeToUtf8Bytes(original, context.PaymentArray);
    long encoded = Stopwatch.GetTimestamp();
    var copy = JsonSerializer.Deserialize(json, context.PaymentArray)!;
    long decoded = Stopwatch.GetTimestamp();
    long sum = copy.Sum(Score);
    long end = Stopwatch.GetTimestamp();
    if (sum != expected || copy.Length != roots) throw new Exception("Incorrect round trip");
    if (trial == 0 && !json.AsSpan().SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(copy, context.PaymentArray))) throw new Exception("Data changed");
    sink += sum;
    measurements.Add(new Measurement((encoded-start)/(double)Stopwatch.Frequency, (decoded-encoded)/(double)Stopwatch.Frequency, (end-decoded)/(double)Stopwatch.Frequency, (end-start)/(double)Stopwatch.Frequency, json.Length));
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
    _ => throw new Exception("Unknown case"),
    };
    return index + data.Id + data.Label.Length + data.Values.Sum(v => (long)v) + data.Tags.Sum(p => p.Key.Length + p.Value.Length) + (data.Next is { } next ? Score(next) : 0);
}
public record Payload(int Id, string Label, int[] Values, Dictionary<string,string> Tags, Payment? Next);
[JsonPolymorphic]
[JsonDerivedType(typeof(Case000), "Case000")]
[JsonDerivedType(typeof(Case001), "Case001")]
[JsonDerivedType(typeof(Case002), "Case002")]
[JsonDerivedType(typeof(Case003), "Case003")]
[JsonDerivedType(typeof(Case004), "Case004")]
[JsonDerivedType(typeof(Case005), "Case005")]
[JsonDerivedType(typeof(Case006), "Case006")]
[JsonDerivedType(typeof(Case007), "Case007")]
[JsonDerivedType(typeof(Case008), "Case008")]
[JsonDerivedType(typeof(Case009), "Case009")]
[JsonDerivedType(typeof(Case010), "Case010")]
[JsonDerivedType(typeof(Case011), "Case011")]
[JsonDerivedType(typeof(Case012), "Case012")]
[JsonDerivedType(typeof(Case013), "Case013")]
[JsonDerivedType(typeof(Case014), "Case014")]
[JsonDerivedType(typeof(Case015), "Case015")]
[JsonDerivedType(typeof(Case016), "Case016")]
[JsonDerivedType(typeof(Case017), "Case017")]
[JsonDerivedType(typeof(Case018), "Case018")]
[JsonDerivedType(typeof(Case019), "Case019")]
[JsonDerivedType(typeof(Case020), "Case020")]
[JsonDerivedType(typeof(Case021), "Case021")]
[JsonDerivedType(typeof(Case022), "Case022")]
[JsonDerivedType(typeof(Case023), "Case023")]
[JsonDerivedType(typeof(Case024), "Case024")]
[JsonDerivedType(typeof(Case025), "Case025")]
[JsonDerivedType(typeof(Case026), "Case026")]
[JsonDerivedType(typeof(Case027), "Case027")]
[JsonDerivedType(typeof(Case028), "Case028")]
[JsonDerivedType(typeof(Case029), "Case029")]
[JsonDerivedType(typeof(Case030), "Case030")]
[JsonDerivedType(typeof(Case031), "Case031")]
[JsonDerivedType(typeof(Case032), "Case032")]
[JsonDerivedType(typeof(Case033), "Case033")]
[JsonDerivedType(typeof(Case034), "Case034")]
[JsonDerivedType(typeof(Case035), "Case035")]
[JsonDerivedType(typeof(Case036), "Case036")]
[JsonDerivedType(typeof(Case037), "Case037")]
[JsonDerivedType(typeof(Case038), "Case038")]
[JsonDerivedType(typeof(Case039), "Case039")]
[JsonDerivedType(typeof(Case040), "Case040")]
[JsonDerivedType(typeof(Case041), "Case041")]
[JsonDerivedType(typeof(Case042), "Case042")]
[JsonDerivedType(typeof(Case043), "Case043")]
[JsonDerivedType(typeof(Case044), "Case044")]
[JsonDerivedType(typeof(Case045), "Case045")]
[JsonDerivedType(typeof(Case046), "Case046")]
[JsonDerivedType(typeof(Case047), "Case047")]
[JsonDerivedType(typeof(Case048), "Case048")]
[JsonDerivedType(typeof(Case049), "Case049")]
[JsonDerivedType(typeof(Case050), "Case050")]
[JsonDerivedType(typeof(Case051), "Case051")]
[JsonDerivedType(typeof(Case052), "Case052")]
[JsonDerivedType(typeof(Case053), "Case053")]
[JsonDerivedType(typeof(Case054), "Case054")]
[JsonDerivedType(typeof(Case055), "Case055")]
[JsonDerivedType(typeof(Case056), "Case056")]
[JsonDerivedType(typeof(Case057), "Case057")]
[JsonDerivedType(typeof(Case058), "Case058")]
[JsonDerivedType(typeof(Case059), "Case059")]
[JsonDerivedType(typeof(Case060), "Case060")]
[JsonDerivedType(typeof(Case061), "Case061")]
[JsonDerivedType(typeof(Case062), "Case062")]
[JsonDerivedType(typeof(Case063), "Case063")]
[JsonDerivedType(typeof(Case064), "Case064")]
[JsonDerivedType(typeof(Case065), "Case065")]
[JsonDerivedType(typeof(Case066), "Case066")]
[JsonDerivedType(typeof(Case067), "Case067")]
[JsonDerivedType(typeof(Case068), "Case068")]
[JsonDerivedType(typeof(Case069), "Case069")]
[JsonDerivedType(typeof(Case070), "Case070")]
[JsonDerivedType(typeof(Case071), "Case071")]
[JsonDerivedType(typeof(Case072), "Case072")]
[JsonDerivedType(typeof(Case073), "Case073")]
[JsonDerivedType(typeof(Case074), "Case074")]
[JsonDerivedType(typeof(Case075), "Case075")]
[JsonDerivedType(typeof(Case076), "Case076")]
[JsonDerivedType(typeof(Case077), "Case077")]
[JsonDerivedType(typeof(Case078), "Case078")]
[JsonDerivedType(typeof(Case079), "Case079")]
[JsonDerivedType(typeof(Case080), "Case080")]
[JsonDerivedType(typeof(Case081), "Case081")]
[JsonDerivedType(typeof(Case082), "Case082")]
[JsonDerivedType(typeof(Case083), "Case083")]
[JsonDerivedType(typeof(Case084), "Case084")]
[JsonDerivedType(typeof(Case085), "Case085")]
[JsonDerivedType(typeof(Case086), "Case086")]
[JsonDerivedType(typeof(Case087), "Case087")]
[JsonDerivedType(typeof(Case088), "Case088")]
[JsonDerivedType(typeof(Case089), "Case089")]
[JsonDerivedType(typeof(Case090), "Case090")]
[JsonDerivedType(typeof(Case091), "Case091")]
[JsonDerivedType(typeof(Case092), "Case092")]
[JsonDerivedType(typeof(Case093), "Case093")]
[JsonDerivedType(typeof(Case094), "Case094")]
[JsonDerivedType(typeof(Case095), "Case095")]
[JsonDerivedType(typeof(Case096), "Case096")]
[JsonDerivedType(typeof(Case097), "Case097")]
[JsonDerivedType(typeof(Case098), "Case098")]
[JsonDerivedType(typeof(Case099), "Case099")]
public abstract record Payment;
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

public record Measurement(double EncodeSeconds, double DecodeSeconds, double SwitchSeconds, double TotalSeconds, int Bytes);
public record Result(int Roots, int Samples, long Sink, List<Measurement> Measurements);
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment[]))]
[JsonSerializable(typeof(Result))]

internal partial class BenchContext : JsonSerializerContext;
