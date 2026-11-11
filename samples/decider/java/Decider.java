import java.util.ArrayList;
import java.util.List;

final class Decider {
    static String phase(State state) {
        return switch (state) {
            case NotCreated n -> "NotCreated";
            case Available a -> "Available";
            case Full f -> "Full";
        };
    }
    private static Roster data(State state) {
        return switch (state) {
            case NotCreated n -> null;
            case Available a -> a.roster();
            case Full f -> f.roster();
        };
    }
    static Decision decide(State state, Command command) {
        return switch (command) {
            case Create c -> !(state instanceof NotCreated) ? new Rejected("already_created")
                : c.name().isEmpty() ? new Rejected("invalid_name")
                : c.capacity() < 1 || c.capacity() > 10 ? new Rejected("invalid_capacity")
                : accept(new RoomCreated(c.name(), c.capacity()));
            case Enroll e -> change(state, e.studentId(), true);
            case Drop d -> change(state, d.studentId(), false);
        };
    }
    private static Decision change(State state, String student, boolean enrolling) {
        var roster = data(state);
        if (roster == null) return new Rejected("not_created");
        if (student.isEmpty()) return new Rejected("invalid_student");
        boolean member = roster.students().contains(student);
        if (enrolling) {
            if (member) return new Rejected("already_enrolled");
            if (state instanceof Full) return new Rejected("full");
            return accept(new StudentEnrolled(student));
        }
        return member ? accept(new StudentDropped(student)) : new Rejected("not_enrolled");
    }
    static State evolve(State state, Event event) {
        return switch (event) {
            case RoomCreated e -> {
                if (!(state instanceof NotCreated) || e.name().isEmpty() ||
                    e.capacity() < 1 || e.capacity() > 10) throw invalidHistory();
                yield new Available(new Roster(e.name(), e.capacity(), List.of()));
            }
            case StudentEnrolled e -> {
                if (!(state instanceof Available a) || e.studentId().isEmpty() ||
                    a.roster().students().contains(e.studentId())) throw invalidHistory();
                var r = a.roster();
                var students = new ArrayList<>(r.students());
                students.add(e.studentId());
                yield classify(new Roster(r.name(), r.capacity(), students));
            }
            case StudentDropped e -> {
                var r = data(state);
                if (r == null || !r.students().contains(e.studentId())) throw invalidHistory();
                yield classify(new Roster(r.name(), r.capacity(),
                    r.students().stream().filter(s -> !s.equals(e.studentId())).toList()));
            }
        };
    }
    private static State classify(Roster r) {
        return r.students().size() == r.capacity() ? new Full(r) : new Available(r);
    }
    static State replay(List<Event> events) {
        State state = new NotCreated();
        for (var event : events) state = evolve(state, event);
        return state;
    }
    private static Accepted accept(Event event) { return new Accepted(List.of(event)); }
    private static IllegalStateException invalidHistory() { return new IllegalStateException("invalid_history"); }
}
