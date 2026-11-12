using SchoolDomain;
using Playground.Dcb;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
try
{
    if(JsonSerializer.IsReflectionEnabledByDefault)throw new Exception("reflection must be disabled");
    if(Environment.GetEnvironmentVariable("DECIDER_REQUIRE_NATIVE")=="1" && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)throw new Exception("Native AOT required");
    if(args.Length==2 && args[0]=="--persistence-test"){PersistenceTests.Run(args[1]);return;}
    if(args.Length==2 && args[0]=="--projection-scale"){ProjectionTests.Run(int.Parse(args[1]));return;}
    if(args.Contains("--domain-test")){DomainTests.Run();return;}
    if(args.Contains("--self-test")){LibraryTests.Run();Console.WriteLine("{\"library\":\"PASS\"}");return;}
    var input=JsonSerializer.Deserialize(Console.In.ReadToEnd(),Wire.Default.Input)??throw new JsonException("input required");
    var output=Application.Run(input.History,input.Operations);
    Console.WriteLine(Environment.GetEnvironmentVariable("DECIDER_BENCHMARK_SUMMARY")=="1"?BenchmarkOutput.Serialize(output):JsonSerializer.Serialize(output,Wire.Default.RunOutput));
}
catch(Exception e){Console.Error.WriteLine(e.Message);Environment.ExitCode=1;}
public sealed record Input(ImmutableArray<Recorded<Event>> History,ImmutableArray<Operation> Operations);
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase,AllowOutOfOrderMetadataProperties=true,
    RespectNullableAnnotations=true,RespectRequiredConstructorParameters=true)]
[JsonSerializable(typeof(Input))]
[JsonSerializable(typeof(RunOutput))]
internal partial class Wire:JsonSerializerContext;
