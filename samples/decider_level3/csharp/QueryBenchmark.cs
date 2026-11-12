using Playground.Dcb;
using System.Diagnostics;
using System.Text.Json;
namespace SchoolDomain;
internal static class QueryBenchmark
{
 public static void Run(JsonEventStore<Event> store,SchoolProjection projection)
 {
  if(!int.TryParse(Environment.GetEnvironmentVariable("DECIDER_STUDENTS"),out var n)||n<=0)return;
  var clock=Stopwatch.StartNew();
  for(var i=0;i<1000;i++){var id=$"student-{(long)i*7919%n:D8}";if(store.Read(new([new([Routing.Student(id)],[])])).Events.IsEmpty)throw new Exception("point read missing");}
  Console.Error.WriteLine($"PHASE tagged_point {clock.Elapsed.TotalSeconds:R}");clock.Restart();
  for(var i=0;i<1000;i++){var id=$"student-{(long)i*7919%n:D8}";var json=JsonSerializer.Serialize(projection.Student(id),Wire.Default.Student);if(JsonSerializer.Deserialize(json,Wire.Default.Student)?.StudentId!=id)throw new Exception("point query mismatch");}
  Console.Error.WriteLine($"PHASE projection_point_json {clock.Elapsed.TotalSeconds:R}");
 }
}
