using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Classroom;

public sealed record Roster(string Name, int Capacity, ImmutableArray<string> Students);

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record State;
public sealed record NotCreated : State;
public sealed record Available(Roster Roster) : State;
public sealed record Full(Roster Roster) : State;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Command;
public sealed record Create(string Name, int Capacity) : Command;
public sealed record Enroll(string StudentId) : Command;
public sealed record Drop(string StudentId) : Command;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Event;
public sealed record RoomCreated(string Name, int Capacity) : Event;
public sealed record StudentEnrolled(string StudentId) : Event;
public sealed record StudentDropped(string StudentId) : Event;

public closed record Decision;
public sealed record Accepted(ImmutableArray<Event> Events) : Decision;
public sealed record Rejected(string Code) : Decision;
