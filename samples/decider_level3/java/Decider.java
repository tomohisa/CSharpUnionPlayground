import playground.dcb.Dcb;
import java.util.ArrayList;
import java.util.List;

final class Decider {
    static DecisionModel empty() { return new DecisionModel(List.of(), List.of(), List.of()); }
    static Plan planOf(ClassState room) {
        return switch (room) { case Available c -> c.plan(); case Full c -> c.plan(); case Teaching c -> c.plan(); case Completed c -> c.plan(); };
    }
    static List<String> members(ClassState room) {
        return switch (room) { case Available c -> c.students(); case Full c -> c.students(); case Teaching c -> c.students(); case Completed c -> c.results().stream().map(ClassResult::studentId).toList(); };
    }
    static String describe(Progress progress) {
        return switch (progress) {
            case Reserved p -> "reserved"; case Studying p -> "studying:" + p.attendance();
            case Passed p -> "passed:" + p.score() + ":" + p.credits();
            case Failed p -> "failed:" + p.score() + ":" + p.reason(); case Withdrawn p -> "withdrawn:" + p.reason();
        };
    }
    static int rank(Grade grade) { return switch (grade) { case Year1 -> 1; case Year2 -> 2; case Year3 -> 3; }; }
    static boolean recruiting(ClassState c) { return c instanceof Available || c instanceof Full; }
    static Decision accept(Event e) { return new Accepted(List.of(e)); }
    static Decision reject(Rejection code) { return new Rejected(code); }
    static Decision decideOne(DecisionModel state, Command command) {
        switch (command) {
            case RegisterStudent c -> {
                if (c.studentId().isEmpty()) return reject(Rejection.InvalidId);
                return state.students().stream().anyMatch(s -> s.studentId().equals(c.studentId())) ? reject(Rejection.DuplicateStudent) : accept(new StudentRegistered(c.studentId(), c.grade()));
            }
            case PromoteStudent c -> {
                for (var student : state.students()) if (student.studentId().equals(c.studentId()))
                    return rank(c.grade()) == rank(student.grade()) + 1 ? accept(new StudentPromoted(c.studentId(), c.grade())) : reject(Rejection.InvalidPromotion);
                return reject(Rejection.UnknownStudent);
            }
            case OpenClass c -> {
                if (c.classId().isEmpty()) return reject(Rejection.InvalidId);
                if (c.capacity() < 1 || c.capacity() > 10) return reject(Rejection.InvalidCapacity);
                return state.classes().stream().anyMatch(x -> planOf(x).classId().equals(c.classId())) ? reject(Rejection.DuplicateClass) : accept(new ClassOpened(c.classId(), c.course(), c.capacity()));
            }
            default -> { return decideClass(state, command); }
        }
    }
    static Decision decideClass(DecisionModel state, Command command) {
        String id = switch (command) {
            case Enroll c -> c.classId(); case Drop c -> c.classId(); case StartClass c -> c.classId();
            case Attend c -> c.classId(); case Assess c -> c.classId(); case CompleteClass c -> c.classId();
            case RegisterStudent c -> throw new IllegalStateException(); case PromoteStudent c -> throw new IllegalStateException(); case OpenClass c -> throw new IllegalStateException();
        };
        for (var room : state.classes()) if (planOf(room).classId().equals(id)) {
            var plan = planOf(room);
            switch (command) {
                case Enroll c -> {
                    for (var student : state.students()) if (student.studentId().equals(c.studentId())) {
                        if (!recruiting(room)) return reject(Rejection.RegistrationClosed);
                        if (members(room).contains(c.studentId())) return reject(Rejection.AlreadyEnrolled);
                        if (rank(student.grade()) < rank(plan.minimumGrade())) return reject(Rejection.GradeTooLow);
                        boolean eligible = switch (plan.prerequisite()) {
                            case NoPrerequisite p -> true;
                            case PassedCourse p -> state.enrollments().stream().anyMatch(e -> e.studentId().equals(c.studentId()) && e.course() == p.course() && e.progress() instanceof Passed);
                        };
                        if (!eligible) return reject(Rejection.PrerequisiteMissing);
                        return room instanceof Full ? reject(Rejection.ClassFull) : accept(new StudentEnrolled(id, c.studentId(), plan.course()));
                    }
                    return reject(Rejection.UnknownStudent);
                }
                case Drop c -> {
                    if (!recruiting(room)) return reject(Rejection.RegistrationClosed);
                    return members(room).contains(c.studentId()) ? accept(new StudentDropped(id, c.studentId())) : reject(Rejection.NotEnrolled);
                }
                case StartClass c -> {
                    if (!recruiting(room)) return reject(Rejection.RegistrationClosed);
                    return c.startedOn().isEmpty() ? reject(Rejection.InvalidDate) : accept(new ClassStarted(id, c.startedOn()));
                }
                case Attend c -> { return study(state, room, id, c.studentId(), c); }
                case Assess c -> { return study(state, room, id, c.studentId(), c); }
                case CompleteClass c -> {
                    if (!(room instanceof Teaching)) return reject(Rejection.NotTeaching);
                    return state.enrollments().stream().anyMatch(e -> e.classId().equals(id) && e.progress() instanceof Studying) ? reject(Rejection.UnassessedStudents) : accept(new ClassCompleted(id, state.enrollments().stream().filter(e -> e.classId().equals(id) && (e.progress() instanceof Passed || e.progress() instanceof Failed)).map(e -> new ClassResult(e.studentId(), e.progress())).toList()));
                }
                case RegisterStudent c -> throw new IllegalStateException(); case PromoteStudent c -> throw new IllegalStateException(); case OpenClass c -> throw new IllegalStateException();
            }
        }
        return reject(Rejection.UnknownClass);
    }
    static Decision study(DecisionModel state, ClassState room, String id, String student, Command command) {
        if (!(room instanceof Teaching)) return reject(Rejection.NotTeaching);
        if (!members(room).contains(student)) return reject(Rejection.NotEnrolled);
        for (var e : state.enrollments()) if (e.classId().equals(id) && e.studentId().equals(student)) {
            if (!(e.progress() instanceof Studying p)) return reject(Rejection.AlreadyAssessed);
            return switch (command) {
                case Attend c -> accept(new AttendanceRecorded(id, student));
                case Assess c -> c.score() < 0 || c.score() > 100 ? reject(Rejection.InvalidScore)
                    : p.attendance() == 0 ? reject(Rejection.NoAttendance) : accept(new ResultRecorded(id, student, c.score()));
                default -> throw new IllegalStateException("invariant");
            };
        }
        throw new IllegalStateException("invariant");
    }
    static Decision decide(DecisionModel state, Command command) {
        var decision = decideOne(state, command);
        if (decision instanceof Accepted a && a.events().getFirst() instanceof ClassStarted e) {
            var events = new ArrayList<Event>(a.events());
            state.enrollments().stream().filter(r -> r.classId().equals(e.classId()) && r.progress() instanceof Reserved).forEach(r -> events.add(new StudyStarted(e.classId(), r.studentId())));
            return new Accepted(events);
        }
        return decision;
    }
}
