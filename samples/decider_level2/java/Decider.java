import java.util.ArrayList;
import java.util.List;

final class Decider {
    static State empty() { return new State(List.of(), List.of(), List.of()); }
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
    static Decision decide(State state, Command command) {
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
    static Decision decideClass(State state, Command command) {
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
                        return room instanceof Full ? reject(Rejection.ClassFull) : accept(new StudentEnrolled(id, c.studentId()));
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
                    return state.enrollments().stream().anyMatch(e -> e.classId().equals(id) && e.progress() instanceof Studying) ? reject(Rejection.UnassessedStudents) : accept(new ClassCompleted(id));
                }
                case RegisterStudent c -> throw new IllegalStateException(); case PromoteStudent c -> throw new IllegalStateException(); case OpenClass c -> throw new IllegalStateException();
            }
        }
        return reject(Rejection.UnknownClass);
    }
    static Decision study(State state, ClassState room, String id, String student, Command command) {
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
    static Command cause(Event e) {
        return switch (e) {
            case StudentRegistered x -> new RegisterStudent(x.studentId(), x.grade()); case StudentPromoted x -> new PromoteStudent(x.studentId(), x.grade());
            case ClassOpened x -> new OpenClass(x.classId(), x.course(), x.capacity()); case StudentEnrolled x -> new Enroll(x.classId(), x.studentId());
            case StudentDropped x -> new Drop(x.classId(), x.studentId()); case ClassStarted x -> new StartClass(x.classId(), x.startedOn());
            case AttendanceRecorded x -> new Attend(x.classId(), x.studentId()); case ResultRecorded x -> new Assess(x.classId(), x.studentId(), x.score()); case ClassCompleted x -> new CompleteClass(x.classId());
        };
    }
    static State evolve(State s, Event event) {
        if (decide(s, cause(event)) instanceof Rejected) throw new IllegalArgumentException("InvalidHistory");
        switch (event) {
            case StudentRegistered x -> { var students = new ArrayList<>(s.students()); students.add(new Student(x.studentId(), x.grade())); return new State(students, s.classes(), s.enrollments()); }
            case StudentPromoted x -> { return new State(s.students().stream().map(v -> v.studentId().equals(x.studentId()) ? new Student(v.studentId(), x.grade()) : v).toList(), s.classes(), s.enrollments()); }
            case ClassOpened x -> {
                Plan plan = switch (x.course()) {
                    case Foundation -> new Plan(x.classId(), x.course(), Grade.Year2, new NoPrerequisite(), x.capacity());
                    case Advanced -> new Plan(x.classId(), x.course(), Grade.Year3, new PassedCourse(Course.Foundation), x.capacity());
                };
                var classes = new ArrayList<>(s.classes()); classes.add(new Available(plan, List.of()));
                return new State(s.students(), classes, s.enrollments());
            }
            default -> { return applyClass(s, event); }
        }
    }
    static State applyClass(State s, Event event) {
        String id = switch (cause(event)) {
            case Enroll x -> x.classId(); case Drop x -> x.classId(); case StartClass x -> x.classId();
            case Attend x -> x.classId(); case Assess x -> x.classId(); case CompleteClass x -> x.classId();
            default -> throw new IllegalStateException();
        };
        var room = s.classes().stream().filter(c -> planOf(c).classId().equals(id)).findFirst().orElseThrow();
        var plan = planOf(room); var records = new ArrayList<>(s.enrollments());
        ClassState updated = room;
        switch (event) {
            case StudentEnrolled x -> {
                var students = new ArrayList<>(members(room)); students.add(x.studentId());
                updated = students.size() == plan.capacity() ? new Full(plan, students) : new Available(plan, students);
                records.removeIf(e -> e.classId().equals(id) && e.studentId().equals(x.studentId()));
                records.add(new Enrollment(id, x.studentId(), plan.course(), new Reserved()));
            }
            case StudentDropped x -> {
                updated = new Available(plan, members(room).stream().filter(v -> !v.equals(x.studentId())).toList());
                records.replaceAll(e -> e.classId().equals(id) && e.studentId().equals(x.studentId()) ? new Enrollment(id, e.studentId(), e.course(), new Withdrawn(WithdrawalReason.StudentRequest)) : e);
            }
            case ClassStarted x -> {
                updated = new Teaching(plan, members(room), x.startedOn());
                records.replaceAll(e -> e.classId().equals(id) && e.progress() instanceof Reserved ? new Enrollment(id, e.studentId(), e.course(), new Studying(0)) : e);
            }
            case AttendanceRecorded x -> records.replaceAll(e -> e.classId().equals(id) && e.studentId().equals(x.studentId()) && e.progress() instanceof Studying p ? new Enrollment(id, e.studentId(), e.course(), new Studying(p.attendance() + 1)) : e);
            case ResultRecorded x -> {
                Progress result = x.score() >= 60 ? new Passed(x.score(), plan.course() == Course.Foundation ? 1 : 2) : new Failed(x.score(), FailureReason.BelowPassMark);
                records.replaceAll(e -> e.classId().equals(id) && e.studentId().equals(x.studentId()) ? new Enrollment(id, e.studentId(), e.course(), result) : e);
            }
            case ClassCompleted x -> updated = new Completed(plan, records.stream().filter(e -> e.classId().equals(id) && (e.progress() instanceof Passed || e.progress() instanceof Failed)).map(e -> new ClassResult(e.studentId(), e.progress())).toList());
            default -> throw new IllegalStateException();
        }
        var classes = new ArrayList<>(s.classes());
        for (int i = 0; i < classes.size(); i++) if (planOf(classes.get(i)).classId().equals(id)) classes.set(i, updated);
        return new State(s.students(), classes, records);
    }
    static State replay(List<Event> events) { State state = empty(); for (var event : events) state = evolve(state, event); return state; }
}
