using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace SchoolDomain;
internal sealed record BenchmarkSummary(SchoolList List,bool RebuildMatches,int EventCount,int Checkpoint,int ListenerFaults,Dictionary<string,int> StepCounts,Dictionary<string,int> Errors,int Conflicts);
internal static class BenchmarkOutput
{
 public static string Serialize(RunOutput o)=>JsonSerializer.Serialize(new BenchmarkSummary(o.List,
  JsonSerializer.Serialize(o.List,Wire.Default.SchoolList)==JsonSerializer.Serialize(o.Rebuilt,Wire.Default.SchoolList),o.Events.Length,o.Checkpoint,o.ListenerFaults,
  o.Steps.GroupBy(s=>s.Status).ToDictionary(g=>g.Key,g=>g.Count()),o.Steps.SelectMany(s=>s.Errors).GroupBy(e=>e.ToString()).ToDictionary(g=>g.Key,g=>g.Count()),o.Steps.Sum(s=>s.Conflicts)),BenchmarkJson.Default.BenchmarkSummary);
}
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(BenchmarkSummary))]
internal partial class BenchmarkJson:JsonSerializerContext;
