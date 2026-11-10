using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using CSharpUnionPlayground.Experimental.Json;

Console.WriteLine("{\"event\":\"ready\"}");
int roots = int.Parse(args[0]), samples = int.Parse(args[1]);
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, TypeInfoResolver = BenchContext.Default };
@@CONVERTER_REGISTRATION@@
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
@@CREATE@@
        _ => throw new Exception()
    };
}
static long Score(Payment payment)
{
    (Payload data, int index) = payment switch
    {
@@DISPATCH@@
    };
    return index + data.Id + data.Label.Length + data.Values.Sum(v => (long)v) + data.Tags.Sum(p => p.Key.Length + p.Value.Length) + (data.Next is { } next ? Score(next) : 0);
}
public record Payload(int Id, string Label, int[] Values, Dictionary<string,string> Tags, Payment? Next);
@@ATTRS@@
@@DECLARATION@@
@@CASES@@
@@CONVERTER@@
public record Measurement(double EncodeSeconds, double DecodeSeconds, double SwitchSeconds, double TotalSeconds, int Bytes);
public record Result(int Roots, int Samples, long Sink, List<Measurement> Measurements);
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Payment[]))]
[JsonSerializable(typeof(Result))]
@@REGISTRATIONS@@
internal partial class BenchContext : JsonSerializerContext;
