using System.Collections.Immutable;

namespace Classroom;

public static class Decider
{
    public static string Phase(State state) => state switch
    {
        NotCreated => nameof(NotCreated),
        Available => nameof(Available),
        Full => nameof(Full)
    };

    private static Roster? Data(State state) => state switch
    {
        NotCreated => null,
        Available available => available.Roster,
        Full full => full.Roster
    };

    public static Decision Decide(State state, Command command) => command switch
    {
        Create create => state is not NotCreated ? new Rejected("already_created")
            : create.Name.Length == 0 ? new Rejected("invalid_name")
            : create.Capacity is < 1 or > 10 ? new Rejected("invalid_capacity")
            : Accept(new RoomCreated(create.Name, create.Capacity)),
        Enroll enroll => Change(state, enroll.StudentId, enrolling: true),
        Drop drop => Change(state, drop.StudentId, enrolling: false)
    };

    private static Decision Change(State state, string student, bool enrolling)
    {
        var roster = Data(state);
        if (roster is null) return new Rejected("not_created");
        if (student.Length == 0) return new Rejected("invalid_student");
        bool member = roster.Students.Contains(student);
        if (enrolling)
        {
            if (member) return new Rejected("already_enrolled");
            if (state is Full) return new Rejected("full");
            return Accept(new StudentEnrolled(student));
        }
        return member ? Accept(new StudentDropped(student)) : new Rejected("not_enrolled");
    }

    public static State Evolve(State state, Event @event) => @event switch
    {
        RoomCreated created => state is NotCreated && created.Name.Length > 0 &&
            created.Capacity is >= 1 and <= 10
            ? new Available(new Roster(created.Name, created.Capacity, []))
            : throw InvalidHistory(),
        StudentEnrolled enrolled => Enrolled(state, enrolled.StudentId),
        StudentDropped dropped => Dropped(state, dropped.StudentId)
    };

    private static State Enrolled(State state, string student)
    {
        if (state is not Available available || student.Length == 0 ||
            available.Roster.Students.Contains(student)) throw InvalidHistory();
        return Classify(available.Roster with { Students = [.. available.Roster.Students, student] });
    }

    private static State Dropped(State state, string student)
    {
        var roster = Data(state);
        if (roster is null || !roster.Students.Contains(student)) throw InvalidHistory();
        return Classify(roster with { Students = [.. roster.Students.Where(s => s != student)] });
    }

    private static State Classify(Roster roster) => roster.Students.Length == roster.Capacity
        ? new Full(roster) : new Available(roster);

    public static State Replay(IEnumerable<Event> events) =>
        events.Aggregate((State)new NotCreated(), Evolve);

    private static Accepted Accept(Event @event) => new([@event]);
    private static InvalidOperationException InvalidHistory() => new("invalid_history");
}
