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
    null => throw new Exception("Empty union"),
    };
    return index + data.Id + data.Label.Length + data.Values.Sum(v => (long)v) + data.Tags.Sum(p => p.Key.Length + p.Value.Length) + (data.Next is { } next ? Score(next) : 0);
}
public record Payload(int Id, string Label, int[] Values, Dictionary<string,string> Tags, Payment? Next);
[JsonConverter(typeof(PaymentConverter))]
public union Payment(Case000, Case001, Case002, Case003, Case004, Case005, Case006, Case007, Case008, Case009, Case010, Case011, Case012, Case013, Case014, Case015, Case016, Case017, Case018, Case019, Case020, Case021, Case022, Case023, Case024, Case025, Case026, Case027, Case028, Case029, Case030, Case031, Case032, Case033, Case034, Case035, Case036, Case037, Case038, Case039, Case040, Case041, Case042, Case043, Case044, Case045, Case046, Case047, Case048, Case049, Case050, Case051, Case052, Case053, Case054, Case055, Case056, Case057, Case058, Case059, Case060, Case061, Case062, Case063, Case064, Case065, Case066, Case067, Case068, Case069, Case070, Case071, Case072, Case073, Case074, Case075, Case076, Case077, Case078, Case079, Case080, Case081, Case082, Case083, Case084, Case085, Case086, Case087, Case088, Case089, Case090, Case091, Case092, Case093, Case094, Case095, Case096, Case097, Case098, Case099);
public sealed record Case000(Payload Data);
public sealed record Case001(Payload Data);
public sealed record Case002(Payload Data);
public sealed record Case003(Payload Data);
public sealed record Case004(Payload Data);
public sealed record Case005(Payload Data);
public sealed record Case006(Payload Data);
public sealed record Case007(Payload Data);
public sealed record Case008(Payload Data);
public sealed record Case009(Payload Data);
public sealed record Case010(Payload Data);
public sealed record Case011(Payload Data);
public sealed record Case012(Payload Data);
public sealed record Case013(Payload Data);
public sealed record Case014(Payload Data);
public sealed record Case015(Payload Data);
public sealed record Case016(Payload Data);
public sealed record Case017(Payload Data);
public sealed record Case018(Payload Data);
public sealed record Case019(Payload Data);
public sealed record Case020(Payload Data);
public sealed record Case021(Payload Data);
public sealed record Case022(Payload Data);
public sealed record Case023(Payload Data);
public sealed record Case024(Payload Data);
public sealed record Case025(Payload Data);
public sealed record Case026(Payload Data);
public sealed record Case027(Payload Data);
public sealed record Case028(Payload Data);
public sealed record Case029(Payload Data);
public sealed record Case030(Payload Data);
public sealed record Case031(Payload Data);
public sealed record Case032(Payload Data);
public sealed record Case033(Payload Data);
public sealed record Case034(Payload Data);
public sealed record Case035(Payload Data);
public sealed record Case036(Payload Data);
public sealed record Case037(Payload Data);
public sealed record Case038(Payload Data);
public sealed record Case039(Payload Data);
public sealed record Case040(Payload Data);
public sealed record Case041(Payload Data);
public sealed record Case042(Payload Data);
public sealed record Case043(Payload Data);
public sealed record Case044(Payload Data);
public sealed record Case045(Payload Data);
public sealed record Case046(Payload Data);
public sealed record Case047(Payload Data);
public sealed record Case048(Payload Data);
public sealed record Case049(Payload Data);
public sealed record Case050(Payload Data);
public sealed record Case051(Payload Data);
public sealed record Case052(Payload Data);
public sealed record Case053(Payload Data);
public sealed record Case054(Payload Data);
public sealed record Case055(Payload Data);
public sealed record Case056(Payload Data);
public sealed record Case057(Payload Data);
public sealed record Case058(Payload Data);
public sealed record Case059(Payload Data);
public sealed record Case060(Payload Data);
public sealed record Case061(Payload Data);
public sealed record Case062(Payload Data);
public sealed record Case063(Payload Data);
public sealed record Case064(Payload Data);
public sealed record Case065(Payload Data);
public sealed record Case066(Payload Data);
public sealed record Case067(Payload Data);
public sealed record Case068(Payload Data);
public sealed record Case069(Payload Data);
public sealed record Case070(Payload Data);
public sealed record Case071(Payload Data);
public sealed record Case072(Payload Data);
public sealed record Case073(Payload Data);
public sealed record Case074(Payload Data);
public sealed record Case075(Payload Data);
public sealed record Case076(Payload Data);
public sealed record Case077(Payload Data);
public sealed record Case078(Payload Data);
public sealed record Case079(Payload Data);
public sealed record Case080(Payload Data);
public sealed record Case081(Payload Data);
public sealed record Case082(Payload Data);
public sealed record Case083(Payload Data);
public sealed record Case084(Payload Data);
public sealed record Case085(Payload Data);
public sealed record Case086(Payload Data);
public sealed record Case087(Payload Data);
public sealed record Case088(Payload Data);
public sealed record Case089(Payload Data);
public sealed record Case090(Payload Data);
public sealed record Case091(Payload Data);
public sealed record Case092(Payload Data);
public sealed record Case093(Payload Data);
public sealed record Case094(Payload Data);
public sealed record Case095(Payload Data);
public sealed record Case096(Payload Data);
public sealed record Case097(Payload Data);
public sealed record Case098(Payload Data);
public sealed record Case099(Payload Data);
public sealed class PaymentConverter : JsonConverter<Payment>
{
    public override Payment Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var data = root.GetProperty("data");
        return root.GetProperty("$type").GetString() switch
        {
            "Case000" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case000>)options.GetTypeInfo(typeof(Case000)))!),
            "Case001" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case001>)options.GetTypeInfo(typeof(Case001)))!),
            "Case002" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case002>)options.GetTypeInfo(typeof(Case002)))!),
            "Case003" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case003>)options.GetTypeInfo(typeof(Case003)))!),
            "Case004" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case004>)options.GetTypeInfo(typeof(Case004)))!),
            "Case005" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case005>)options.GetTypeInfo(typeof(Case005)))!),
            "Case006" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case006>)options.GetTypeInfo(typeof(Case006)))!),
            "Case007" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case007>)options.GetTypeInfo(typeof(Case007)))!),
            "Case008" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case008>)options.GetTypeInfo(typeof(Case008)))!),
            "Case009" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case009>)options.GetTypeInfo(typeof(Case009)))!),
            "Case010" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case010>)options.GetTypeInfo(typeof(Case010)))!),
            "Case011" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case011>)options.GetTypeInfo(typeof(Case011)))!),
            "Case012" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case012>)options.GetTypeInfo(typeof(Case012)))!),
            "Case013" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case013>)options.GetTypeInfo(typeof(Case013)))!),
            "Case014" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case014>)options.GetTypeInfo(typeof(Case014)))!),
            "Case015" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case015>)options.GetTypeInfo(typeof(Case015)))!),
            "Case016" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case016>)options.GetTypeInfo(typeof(Case016)))!),
            "Case017" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case017>)options.GetTypeInfo(typeof(Case017)))!),
            "Case018" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case018>)options.GetTypeInfo(typeof(Case018)))!),
            "Case019" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case019>)options.GetTypeInfo(typeof(Case019)))!),
            "Case020" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case020>)options.GetTypeInfo(typeof(Case020)))!),
            "Case021" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case021>)options.GetTypeInfo(typeof(Case021)))!),
            "Case022" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case022>)options.GetTypeInfo(typeof(Case022)))!),
            "Case023" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case023>)options.GetTypeInfo(typeof(Case023)))!),
            "Case024" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case024>)options.GetTypeInfo(typeof(Case024)))!),
            "Case025" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case025>)options.GetTypeInfo(typeof(Case025)))!),
            "Case026" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case026>)options.GetTypeInfo(typeof(Case026)))!),
            "Case027" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case027>)options.GetTypeInfo(typeof(Case027)))!),
            "Case028" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case028>)options.GetTypeInfo(typeof(Case028)))!),
            "Case029" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case029>)options.GetTypeInfo(typeof(Case029)))!),
            "Case030" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case030>)options.GetTypeInfo(typeof(Case030)))!),
            "Case031" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case031>)options.GetTypeInfo(typeof(Case031)))!),
            "Case032" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case032>)options.GetTypeInfo(typeof(Case032)))!),
            "Case033" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case033>)options.GetTypeInfo(typeof(Case033)))!),
            "Case034" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case034>)options.GetTypeInfo(typeof(Case034)))!),
            "Case035" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case035>)options.GetTypeInfo(typeof(Case035)))!),
            "Case036" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case036>)options.GetTypeInfo(typeof(Case036)))!),
            "Case037" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case037>)options.GetTypeInfo(typeof(Case037)))!),
            "Case038" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case038>)options.GetTypeInfo(typeof(Case038)))!),
            "Case039" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case039>)options.GetTypeInfo(typeof(Case039)))!),
            "Case040" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case040>)options.GetTypeInfo(typeof(Case040)))!),
            "Case041" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case041>)options.GetTypeInfo(typeof(Case041)))!),
            "Case042" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case042>)options.GetTypeInfo(typeof(Case042)))!),
            "Case043" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case043>)options.GetTypeInfo(typeof(Case043)))!),
            "Case044" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case044>)options.GetTypeInfo(typeof(Case044)))!),
            "Case045" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case045>)options.GetTypeInfo(typeof(Case045)))!),
            "Case046" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case046>)options.GetTypeInfo(typeof(Case046)))!),
            "Case047" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case047>)options.GetTypeInfo(typeof(Case047)))!),
            "Case048" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case048>)options.GetTypeInfo(typeof(Case048)))!),
            "Case049" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case049>)options.GetTypeInfo(typeof(Case049)))!),
            "Case050" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case050>)options.GetTypeInfo(typeof(Case050)))!),
            "Case051" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case051>)options.GetTypeInfo(typeof(Case051)))!),
            "Case052" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case052>)options.GetTypeInfo(typeof(Case052)))!),
            "Case053" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case053>)options.GetTypeInfo(typeof(Case053)))!),
            "Case054" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case054>)options.GetTypeInfo(typeof(Case054)))!),
            "Case055" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case055>)options.GetTypeInfo(typeof(Case055)))!),
            "Case056" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case056>)options.GetTypeInfo(typeof(Case056)))!),
            "Case057" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case057>)options.GetTypeInfo(typeof(Case057)))!),
            "Case058" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case058>)options.GetTypeInfo(typeof(Case058)))!),
            "Case059" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case059>)options.GetTypeInfo(typeof(Case059)))!),
            "Case060" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case060>)options.GetTypeInfo(typeof(Case060)))!),
            "Case061" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case061>)options.GetTypeInfo(typeof(Case061)))!),
            "Case062" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case062>)options.GetTypeInfo(typeof(Case062)))!),
            "Case063" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case063>)options.GetTypeInfo(typeof(Case063)))!),
            "Case064" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case064>)options.GetTypeInfo(typeof(Case064)))!),
            "Case065" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case065>)options.GetTypeInfo(typeof(Case065)))!),
            "Case066" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case066>)options.GetTypeInfo(typeof(Case066)))!),
            "Case067" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case067>)options.GetTypeInfo(typeof(Case067)))!),
            "Case068" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case068>)options.GetTypeInfo(typeof(Case068)))!),
            "Case069" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case069>)options.GetTypeInfo(typeof(Case069)))!),
            "Case070" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case070>)options.GetTypeInfo(typeof(Case070)))!),
            "Case071" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case071>)options.GetTypeInfo(typeof(Case071)))!),
            "Case072" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case072>)options.GetTypeInfo(typeof(Case072)))!),
            "Case073" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case073>)options.GetTypeInfo(typeof(Case073)))!),
            "Case074" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case074>)options.GetTypeInfo(typeof(Case074)))!),
            "Case075" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case075>)options.GetTypeInfo(typeof(Case075)))!),
            "Case076" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case076>)options.GetTypeInfo(typeof(Case076)))!),
            "Case077" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case077>)options.GetTypeInfo(typeof(Case077)))!),
            "Case078" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case078>)options.GetTypeInfo(typeof(Case078)))!),
            "Case079" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case079>)options.GetTypeInfo(typeof(Case079)))!),
            "Case080" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case080>)options.GetTypeInfo(typeof(Case080)))!),
            "Case081" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case081>)options.GetTypeInfo(typeof(Case081)))!),
            "Case082" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case082>)options.GetTypeInfo(typeof(Case082)))!),
            "Case083" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case083>)options.GetTypeInfo(typeof(Case083)))!),
            "Case084" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case084>)options.GetTypeInfo(typeof(Case084)))!),
            "Case085" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case085>)options.GetTypeInfo(typeof(Case085)))!),
            "Case086" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case086>)options.GetTypeInfo(typeof(Case086)))!),
            "Case087" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case087>)options.GetTypeInfo(typeof(Case087)))!),
            "Case088" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case088>)options.GetTypeInfo(typeof(Case088)))!),
            "Case089" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case089>)options.GetTypeInfo(typeof(Case089)))!),
            "Case090" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case090>)options.GetTypeInfo(typeof(Case090)))!),
            "Case091" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case091>)options.GetTypeInfo(typeof(Case091)))!),
            "Case092" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case092>)options.GetTypeInfo(typeof(Case092)))!),
            "Case093" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case093>)options.GetTypeInfo(typeof(Case093)))!),
            "Case094" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case094>)options.GetTypeInfo(typeof(Case094)))!),
            "Case095" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case095>)options.GetTypeInfo(typeof(Case095)))!),
            "Case096" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case096>)options.GetTypeInfo(typeof(Case096)))!),
            "Case097" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case097>)options.GetTypeInfo(typeof(Case097)))!),
            "Case098" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case098>)options.GetTypeInfo(typeof(Case098)))!),
            "Case099" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<Case099>)options.GetTypeInfo(typeof(Case099)))!),
            _ => throw new JsonException("Unknown case")
        };
    }
    public override void Write(Utf8JsonWriter writer, Payment value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        switch (value.Value)
        {
            case Case000 item: writer.WriteString("$type", "Case000"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case000>)options.GetTypeInfo(typeof(Case000))); break;
            case Case001 item: writer.WriteString("$type", "Case001"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case001>)options.GetTypeInfo(typeof(Case001))); break;
            case Case002 item: writer.WriteString("$type", "Case002"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case002>)options.GetTypeInfo(typeof(Case002))); break;
            case Case003 item: writer.WriteString("$type", "Case003"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case003>)options.GetTypeInfo(typeof(Case003))); break;
            case Case004 item: writer.WriteString("$type", "Case004"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case004>)options.GetTypeInfo(typeof(Case004))); break;
            case Case005 item: writer.WriteString("$type", "Case005"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case005>)options.GetTypeInfo(typeof(Case005))); break;
            case Case006 item: writer.WriteString("$type", "Case006"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case006>)options.GetTypeInfo(typeof(Case006))); break;
            case Case007 item: writer.WriteString("$type", "Case007"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case007>)options.GetTypeInfo(typeof(Case007))); break;
            case Case008 item: writer.WriteString("$type", "Case008"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case008>)options.GetTypeInfo(typeof(Case008))); break;
            case Case009 item: writer.WriteString("$type", "Case009"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case009>)options.GetTypeInfo(typeof(Case009))); break;
            case Case010 item: writer.WriteString("$type", "Case010"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case010>)options.GetTypeInfo(typeof(Case010))); break;
            case Case011 item: writer.WriteString("$type", "Case011"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case011>)options.GetTypeInfo(typeof(Case011))); break;
            case Case012 item: writer.WriteString("$type", "Case012"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case012>)options.GetTypeInfo(typeof(Case012))); break;
            case Case013 item: writer.WriteString("$type", "Case013"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case013>)options.GetTypeInfo(typeof(Case013))); break;
            case Case014 item: writer.WriteString("$type", "Case014"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case014>)options.GetTypeInfo(typeof(Case014))); break;
            case Case015 item: writer.WriteString("$type", "Case015"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case015>)options.GetTypeInfo(typeof(Case015))); break;
            case Case016 item: writer.WriteString("$type", "Case016"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case016>)options.GetTypeInfo(typeof(Case016))); break;
            case Case017 item: writer.WriteString("$type", "Case017"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case017>)options.GetTypeInfo(typeof(Case017))); break;
            case Case018 item: writer.WriteString("$type", "Case018"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case018>)options.GetTypeInfo(typeof(Case018))); break;
            case Case019 item: writer.WriteString("$type", "Case019"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case019>)options.GetTypeInfo(typeof(Case019))); break;
            case Case020 item: writer.WriteString("$type", "Case020"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case020>)options.GetTypeInfo(typeof(Case020))); break;
            case Case021 item: writer.WriteString("$type", "Case021"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case021>)options.GetTypeInfo(typeof(Case021))); break;
            case Case022 item: writer.WriteString("$type", "Case022"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case022>)options.GetTypeInfo(typeof(Case022))); break;
            case Case023 item: writer.WriteString("$type", "Case023"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case023>)options.GetTypeInfo(typeof(Case023))); break;
            case Case024 item: writer.WriteString("$type", "Case024"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case024>)options.GetTypeInfo(typeof(Case024))); break;
            case Case025 item: writer.WriteString("$type", "Case025"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case025>)options.GetTypeInfo(typeof(Case025))); break;
            case Case026 item: writer.WriteString("$type", "Case026"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case026>)options.GetTypeInfo(typeof(Case026))); break;
            case Case027 item: writer.WriteString("$type", "Case027"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case027>)options.GetTypeInfo(typeof(Case027))); break;
            case Case028 item: writer.WriteString("$type", "Case028"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case028>)options.GetTypeInfo(typeof(Case028))); break;
            case Case029 item: writer.WriteString("$type", "Case029"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case029>)options.GetTypeInfo(typeof(Case029))); break;
            case Case030 item: writer.WriteString("$type", "Case030"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case030>)options.GetTypeInfo(typeof(Case030))); break;
            case Case031 item: writer.WriteString("$type", "Case031"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case031>)options.GetTypeInfo(typeof(Case031))); break;
            case Case032 item: writer.WriteString("$type", "Case032"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case032>)options.GetTypeInfo(typeof(Case032))); break;
            case Case033 item: writer.WriteString("$type", "Case033"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case033>)options.GetTypeInfo(typeof(Case033))); break;
            case Case034 item: writer.WriteString("$type", "Case034"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case034>)options.GetTypeInfo(typeof(Case034))); break;
            case Case035 item: writer.WriteString("$type", "Case035"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case035>)options.GetTypeInfo(typeof(Case035))); break;
            case Case036 item: writer.WriteString("$type", "Case036"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case036>)options.GetTypeInfo(typeof(Case036))); break;
            case Case037 item: writer.WriteString("$type", "Case037"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case037>)options.GetTypeInfo(typeof(Case037))); break;
            case Case038 item: writer.WriteString("$type", "Case038"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case038>)options.GetTypeInfo(typeof(Case038))); break;
            case Case039 item: writer.WriteString("$type", "Case039"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case039>)options.GetTypeInfo(typeof(Case039))); break;
            case Case040 item: writer.WriteString("$type", "Case040"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case040>)options.GetTypeInfo(typeof(Case040))); break;
            case Case041 item: writer.WriteString("$type", "Case041"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case041>)options.GetTypeInfo(typeof(Case041))); break;
            case Case042 item: writer.WriteString("$type", "Case042"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case042>)options.GetTypeInfo(typeof(Case042))); break;
            case Case043 item: writer.WriteString("$type", "Case043"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case043>)options.GetTypeInfo(typeof(Case043))); break;
            case Case044 item: writer.WriteString("$type", "Case044"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case044>)options.GetTypeInfo(typeof(Case044))); break;
            case Case045 item: writer.WriteString("$type", "Case045"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case045>)options.GetTypeInfo(typeof(Case045))); break;
            case Case046 item: writer.WriteString("$type", "Case046"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case046>)options.GetTypeInfo(typeof(Case046))); break;
            case Case047 item: writer.WriteString("$type", "Case047"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case047>)options.GetTypeInfo(typeof(Case047))); break;
            case Case048 item: writer.WriteString("$type", "Case048"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case048>)options.GetTypeInfo(typeof(Case048))); break;
            case Case049 item: writer.WriteString("$type", "Case049"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case049>)options.GetTypeInfo(typeof(Case049))); break;
            case Case050 item: writer.WriteString("$type", "Case050"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case050>)options.GetTypeInfo(typeof(Case050))); break;
            case Case051 item: writer.WriteString("$type", "Case051"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case051>)options.GetTypeInfo(typeof(Case051))); break;
            case Case052 item: writer.WriteString("$type", "Case052"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case052>)options.GetTypeInfo(typeof(Case052))); break;
            case Case053 item: writer.WriteString("$type", "Case053"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case053>)options.GetTypeInfo(typeof(Case053))); break;
            case Case054 item: writer.WriteString("$type", "Case054"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case054>)options.GetTypeInfo(typeof(Case054))); break;
            case Case055 item: writer.WriteString("$type", "Case055"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case055>)options.GetTypeInfo(typeof(Case055))); break;
            case Case056 item: writer.WriteString("$type", "Case056"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case056>)options.GetTypeInfo(typeof(Case056))); break;
            case Case057 item: writer.WriteString("$type", "Case057"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case057>)options.GetTypeInfo(typeof(Case057))); break;
            case Case058 item: writer.WriteString("$type", "Case058"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case058>)options.GetTypeInfo(typeof(Case058))); break;
            case Case059 item: writer.WriteString("$type", "Case059"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case059>)options.GetTypeInfo(typeof(Case059))); break;
            case Case060 item: writer.WriteString("$type", "Case060"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case060>)options.GetTypeInfo(typeof(Case060))); break;
            case Case061 item: writer.WriteString("$type", "Case061"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case061>)options.GetTypeInfo(typeof(Case061))); break;
            case Case062 item: writer.WriteString("$type", "Case062"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case062>)options.GetTypeInfo(typeof(Case062))); break;
            case Case063 item: writer.WriteString("$type", "Case063"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case063>)options.GetTypeInfo(typeof(Case063))); break;
            case Case064 item: writer.WriteString("$type", "Case064"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case064>)options.GetTypeInfo(typeof(Case064))); break;
            case Case065 item: writer.WriteString("$type", "Case065"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case065>)options.GetTypeInfo(typeof(Case065))); break;
            case Case066 item: writer.WriteString("$type", "Case066"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case066>)options.GetTypeInfo(typeof(Case066))); break;
            case Case067 item: writer.WriteString("$type", "Case067"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case067>)options.GetTypeInfo(typeof(Case067))); break;
            case Case068 item: writer.WriteString("$type", "Case068"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case068>)options.GetTypeInfo(typeof(Case068))); break;
            case Case069 item: writer.WriteString("$type", "Case069"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case069>)options.GetTypeInfo(typeof(Case069))); break;
            case Case070 item: writer.WriteString("$type", "Case070"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case070>)options.GetTypeInfo(typeof(Case070))); break;
            case Case071 item: writer.WriteString("$type", "Case071"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case071>)options.GetTypeInfo(typeof(Case071))); break;
            case Case072 item: writer.WriteString("$type", "Case072"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case072>)options.GetTypeInfo(typeof(Case072))); break;
            case Case073 item: writer.WriteString("$type", "Case073"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case073>)options.GetTypeInfo(typeof(Case073))); break;
            case Case074 item: writer.WriteString("$type", "Case074"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case074>)options.GetTypeInfo(typeof(Case074))); break;
            case Case075 item: writer.WriteString("$type", "Case075"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case075>)options.GetTypeInfo(typeof(Case075))); break;
            case Case076 item: writer.WriteString("$type", "Case076"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case076>)options.GetTypeInfo(typeof(Case076))); break;
            case Case077 item: writer.WriteString("$type", "Case077"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case077>)options.GetTypeInfo(typeof(Case077))); break;
            case Case078 item: writer.WriteString("$type", "Case078"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case078>)options.GetTypeInfo(typeof(Case078))); break;
            case Case079 item: writer.WriteString("$type", "Case079"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case079>)options.GetTypeInfo(typeof(Case079))); break;
            case Case080 item: writer.WriteString("$type", "Case080"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case080>)options.GetTypeInfo(typeof(Case080))); break;
            case Case081 item: writer.WriteString("$type", "Case081"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case081>)options.GetTypeInfo(typeof(Case081))); break;
            case Case082 item: writer.WriteString("$type", "Case082"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case082>)options.GetTypeInfo(typeof(Case082))); break;
            case Case083 item: writer.WriteString("$type", "Case083"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case083>)options.GetTypeInfo(typeof(Case083))); break;
            case Case084 item: writer.WriteString("$type", "Case084"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case084>)options.GetTypeInfo(typeof(Case084))); break;
            case Case085 item: writer.WriteString("$type", "Case085"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case085>)options.GetTypeInfo(typeof(Case085))); break;
            case Case086 item: writer.WriteString("$type", "Case086"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case086>)options.GetTypeInfo(typeof(Case086))); break;
            case Case087 item: writer.WriteString("$type", "Case087"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case087>)options.GetTypeInfo(typeof(Case087))); break;
            case Case088 item: writer.WriteString("$type", "Case088"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case088>)options.GetTypeInfo(typeof(Case088))); break;
            case Case089 item: writer.WriteString("$type", "Case089"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case089>)options.GetTypeInfo(typeof(Case089))); break;
            case Case090 item: writer.WriteString("$type", "Case090"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case090>)options.GetTypeInfo(typeof(Case090))); break;
            case Case091 item: writer.WriteString("$type", "Case091"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case091>)options.GetTypeInfo(typeof(Case091))); break;
            case Case092 item: writer.WriteString("$type", "Case092"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case092>)options.GetTypeInfo(typeof(Case092))); break;
            case Case093 item: writer.WriteString("$type", "Case093"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case093>)options.GetTypeInfo(typeof(Case093))); break;
            case Case094 item: writer.WriteString("$type", "Case094"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case094>)options.GetTypeInfo(typeof(Case094))); break;
            case Case095 item: writer.WriteString("$type", "Case095"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case095>)options.GetTypeInfo(typeof(Case095))); break;
            case Case096 item: writer.WriteString("$type", "Case096"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case096>)options.GetTypeInfo(typeof(Case096))); break;
            case Case097 item: writer.WriteString("$type", "Case097"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case097>)options.GetTypeInfo(typeof(Case097))); break;
            case Case098 item: writer.WriteString("$type", "Case098"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case098>)options.GetTypeInfo(typeof(Case098))); break;
            case Case099 item: writer.WriteString("$type", "Case099"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<Case099>)options.GetTypeInfo(typeof(Case099))); break;
            default: throw new JsonException("Unknown case");
        }
        writer.WriteEndObject();
    }
}

public record Measurement(double EncodeSeconds, double DecodeSeconds, double SwitchSeconds, double TotalSeconds, int Bytes);
public record Result(int Roots, int Samples, long Sink, List<Measurement> Measurements);
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment[]))]
[JsonSerializable(typeof(Result))]
[JsonSerializable(typeof(Case000))]
[JsonSerializable(typeof(Case001))]
[JsonSerializable(typeof(Case002))]
[JsonSerializable(typeof(Case003))]
[JsonSerializable(typeof(Case004))]
[JsonSerializable(typeof(Case005))]
[JsonSerializable(typeof(Case006))]
[JsonSerializable(typeof(Case007))]
[JsonSerializable(typeof(Case008))]
[JsonSerializable(typeof(Case009))]
[JsonSerializable(typeof(Case010))]
[JsonSerializable(typeof(Case011))]
[JsonSerializable(typeof(Case012))]
[JsonSerializable(typeof(Case013))]
[JsonSerializable(typeof(Case014))]
[JsonSerializable(typeof(Case015))]
[JsonSerializable(typeof(Case016))]
[JsonSerializable(typeof(Case017))]
[JsonSerializable(typeof(Case018))]
[JsonSerializable(typeof(Case019))]
[JsonSerializable(typeof(Case020))]
[JsonSerializable(typeof(Case021))]
[JsonSerializable(typeof(Case022))]
[JsonSerializable(typeof(Case023))]
[JsonSerializable(typeof(Case024))]
[JsonSerializable(typeof(Case025))]
[JsonSerializable(typeof(Case026))]
[JsonSerializable(typeof(Case027))]
[JsonSerializable(typeof(Case028))]
[JsonSerializable(typeof(Case029))]
[JsonSerializable(typeof(Case030))]
[JsonSerializable(typeof(Case031))]
[JsonSerializable(typeof(Case032))]
[JsonSerializable(typeof(Case033))]
[JsonSerializable(typeof(Case034))]
[JsonSerializable(typeof(Case035))]
[JsonSerializable(typeof(Case036))]
[JsonSerializable(typeof(Case037))]
[JsonSerializable(typeof(Case038))]
[JsonSerializable(typeof(Case039))]
[JsonSerializable(typeof(Case040))]
[JsonSerializable(typeof(Case041))]
[JsonSerializable(typeof(Case042))]
[JsonSerializable(typeof(Case043))]
[JsonSerializable(typeof(Case044))]
[JsonSerializable(typeof(Case045))]
[JsonSerializable(typeof(Case046))]
[JsonSerializable(typeof(Case047))]
[JsonSerializable(typeof(Case048))]
[JsonSerializable(typeof(Case049))]
[JsonSerializable(typeof(Case050))]
[JsonSerializable(typeof(Case051))]
[JsonSerializable(typeof(Case052))]
[JsonSerializable(typeof(Case053))]
[JsonSerializable(typeof(Case054))]
[JsonSerializable(typeof(Case055))]
[JsonSerializable(typeof(Case056))]
[JsonSerializable(typeof(Case057))]
[JsonSerializable(typeof(Case058))]
[JsonSerializable(typeof(Case059))]
[JsonSerializable(typeof(Case060))]
[JsonSerializable(typeof(Case061))]
[JsonSerializable(typeof(Case062))]
[JsonSerializable(typeof(Case063))]
[JsonSerializable(typeof(Case064))]
[JsonSerializable(typeof(Case065))]
[JsonSerializable(typeof(Case066))]
[JsonSerializable(typeof(Case067))]
[JsonSerializable(typeof(Case068))]
[JsonSerializable(typeof(Case069))]
[JsonSerializable(typeof(Case070))]
[JsonSerializable(typeof(Case071))]
[JsonSerializable(typeof(Case072))]
[JsonSerializable(typeof(Case073))]
[JsonSerializable(typeof(Case074))]
[JsonSerializable(typeof(Case075))]
[JsonSerializable(typeof(Case076))]
[JsonSerializable(typeof(Case077))]
[JsonSerializable(typeof(Case078))]
[JsonSerializable(typeof(Case079))]
[JsonSerializable(typeof(Case080))]
[JsonSerializable(typeof(Case081))]
[JsonSerializable(typeof(Case082))]
[JsonSerializable(typeof(Case083))]
[JsonSerializable(typeof(Case084))]
[JsonSerializable(typeof(Case085))]
[JsonSerializable(typeof(Case086))]
[JsonSerializable(typeof(Case087))]
[JsonSerializable(typeof(Case088))]
[JsonSerializable(typeof(Case089))]
[JsonSerializable(typeof(Case090))]
[JsonSerializable(typeof(Case091))]
[JsonSerializable(typeof(Case092))]
[JsonSerializable(typeof(Case093))]
[JsonSerializable(typeof(Case094))]
[JsonSerializable(typeof(Case095))]
[JsonSerializable(typeof(Case096))]
[JsonSerializable(typeof(Case097))]
[JsonSerializable(typeof(Case098))]
[JsonSerializable(typeof(Case099))]
internal partial class BenchContext : JsonSerializerContext;
