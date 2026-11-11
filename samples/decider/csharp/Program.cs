using Classroom;
using System.Text.Json;
using System.Text.Json.Serialization;

try { Run(); }
catch (Exception error)
{
    Console.Error.WriteLine(error.Message);
    Environment.ExitCode = 1;
}

static void Run()
{
    if (JsonSerializer.IsReflectionEnabledByDefault)
        throw new InvalidOperationException("JSON reflection must be disabled");
    if (Environment.GetEnvironmentVariable("DECIDER_REQUIRE_NATIVE") == "1" &&
        System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
        throw new InvalidOperationException("Native AOT is required");
    var input = JsonSerializer.Deserialize(Console.In.ReadToEnd(), Wire.Default.Input)
        ?? throw new JsonException("Input is required");
    var history = new List<Event>(input.History ?? []);
    State state = Decider.Replay(history);
    State initial = state;
    var steps = new List<Step>();
    foreach (var command in input.Commands)
    {
        var decision = Decider.Decide(state, command);
        switch (decision)
        {
            case Accepted accepted:
                foreach (var @event in accepted.Events)
                {
                    state = Decider.Evolve(state, @event);
                    history.Add(@event);
                }
                steps.Add(new(Decider.Phase(state), accepted.Events.Select(Tag).ToArray(), null));
                break;
            case Rejected rejected:
                steps.Add(new(Decider.Phase(state), [], rejected.Code));
                break;
        }
    }
    var json = JsonSerializer.Serialize(history.ToArray(), Wire.Default.EventArray);
    var stored = JsonSerializer.Deserialize(json, Wire.Default.EventArray)!;
    var snapshot = JsonSerializer.Serialize(state, Wire.Default.State);
    var restored = JsonSerializer.Deserialize(snapshot, Wire.Default.State)!;
    Console.WriteLine(JsonSerializer.Serialize(
        new Output(initial, state, stored, steps.ToArray(), restored, Decider.Replay(stored)), Wire.Default.Output));
}

static string Tag(Event @event) => @event switch
{
    RoomCreated => nameof(RoomCreated),
    StudentEnrolled => nameof(StudentEnrolled),
    StudentDropped => nameof(StudentDropped)
};

public sealed record Input(Command[] Commands, Event[]? History = null);
public sealed record Step(string Phase, string[] Emitted, string? Error);
public sealed record Output(State Initial, State State, Event[] Events, Step[] Steps, State Restored, State Replayed);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    AllowOutOfOrderMetadataProperties = true)]
[JsonSerializable(typeof(Input))]
[JsonSerializable(typeof(Output))]
internal partial class Wire : JsonSerializerContext;
