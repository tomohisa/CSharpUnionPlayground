using EnrollmentDomain;
using System.Text.Json;
using System.Text.Json.Serialization;

try
{
    if (JsonSerializer.IsReflectionEnabledByDefault) throw new Exception("Reflection must be disabled");
    if (Environment.GetEnvironmentVariable("DECIDER_REQUIRE_NATIVE") == "1" && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        throw new Exception("Native AOT required");
    var input = JsonSerializer.Deserialize(Console.In.ReadToEnd(), Wire.Default.Input) ?? throw new JsonException("Input required");
    var history = input.History.ToList();
    var state = Decider.Replay(history);
    var initial = state;
    var steps = new List<Decision>();
    foreach (var command in input.Commands)
    {
        var decision = Decider.Decide(state, command);
        steps.Add(decision);
        if (decision is Accepted accepted)
            foreach (var e in accepted.Events) { state = Decider.Evolve(state, e); history.Add(e); }
    }
    var events = JsonSerializer.Deserialize(JsonSerializer.Serialize(history.ToArray(), Wire.Default.EventArray), Wire.Default.EventArray)!;
    var restored = JsonSerializer.Deserialize(JsonSerializer.Serialize(state, Wire.Default.State), Wire.Default.State)!;
    Console.WriteLine(JsonSerializer.Serialize(new Output(initial, state, events, steps.ToArray(), restored, Decider.Replay(events), state.Enrollments.Select(e => Decider.Describe(e.Progress)).ToArray()), Wire.Default.Output));
}
catch (Exception e) { Console.Error.WriteLine(e.Message); Environment.ExitCode = 1; }

public sealed record Input(Command[] Commands, Event[] History);
public sealed record Output(State Initial, State State, Event[] Events, Decision[] Steps, State Restored, State Replayed, string[] Summaries);
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    AllowOutOfOrderMetadataProperties = true, RespectNullableAnnotations = true, RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(Input))]
[JsonSerializable(typeof(Output))]
internal partial class Wire : JsonSerializerContext;
