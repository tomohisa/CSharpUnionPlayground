using System.Collections.Immutable;
namespace EnrollmentDomain;

public static class Decider
{
    public static State Empty => new([], [], []);
    public static Plan PlanOf(ClassState state) => state switch
    {
        Available c => c.Plan, Full c => c.Plan, Teaching c => c.Plan, Completed c => c.Plan
    };
    public static ImmutableArray<string> Members(ClassState state) => state switch
    {
        Available c => c.Students, Full c => c.Students, Teaching c => c.Students,
        Completed c => [.. c.Results.Select(r => r.StudentId)]
    };
    public static string Describe(Progress progress) => progress switch
    {
        Reserved => "reserved", Studying p => $"studying:{p.Attendance}",
        Passed p => $"passed:{p.Score}:{p.Credits}", Failed p => $"failed:{p.Score}:{p.Reason}",
        Withdrawn p => $"withdrawn:{p.Reason}"
    };
    private static int Rank(Grade grade) => grade switch
    { Grade.Year1 => 1, Grade.Year2 => 2, Grade.Year3 => 3, _ => throw new ArgumentException("grade") };
    public static Decision Decide(State state, Command command)
    {
        var students = state.Students;
        var classes = state.Classes;
        Accepted Accept(Event e) => new([e]);
        Rejected Reject(Rejection code) => new(code);
        switch (command)
        {
            case RegisterStudent c:
                if (c.StudentId.Length == 0) return Reject(Rejection.InvalidId);
                return students.Any(s => s.StudentId == c.StudentId) ? Reject(Rejection.DuplicateStudent)
                    : Accept(new StudentRegistered(c.StudentId, c.Grade));
            case PromoteStudent c:
                foreach (var s in students.Where(s => s.StudentId == c.StudentId))
                    return Rank(c.Grade) == Rank(s.Grade) + 1 ? Accept(new StudentPromoted(c.StudentId, c.Grade)) : Reject(Rejection.InvalidPromotion);
                return Reject(Rejection.UnknownStudent);
            case OpenClass c:
                if (c.ClassId.Length == 0) return Reject(Rejection.InvalidId);
                if (c.Capacity is < 1 or > 10) return Reject(Rejection.InvalidCapacity);
                return classes.Any(x => PlanOf(x).ClassId == c.ClassId) ? Reject(Rejection.DuplicateClass)
                    : Accept(new ClassOpened(c.ClassId, c.Course, c.Capacity));
            default: return DecideClass(state, command);
        }
    }
    private static Decision DecideClass(State state, Command command)
    {
        string id = command switch
        {
            RegisterStudent => throw new InvalidOperationException(), PromoteStudent => throw new InvalidOperationException(),
            OpenClass => throw new InvalidOperationException(), Enroll c => c.ClassId, Drop c => c.ClassId,
            StartClass c => c.ClassId, Attend c => c.ClassId, Assess c => c.ClassId, CompleteClass c => c.ClassId
        };
        foreach (var room in state.Classes.Where(c => PlanOf(c).ClassId == id))
        {
            var plan = PlanOf(room);
            Rejected Reject(Rejection code) => new(code);
            Accepted Accept(Event e) => new([e]);
            switch (command)
            {
                case Enroll c:
                    foreach (var student in state.Students.Where(s => s.StudentId == c.StudentId))
                    {
                        if (room is not (Available or Full)) return Reject(Rejection.RegistrationClosed);
                        if (Members(room).Contains(c.StudentId)) return Reject(Rejection.AlreadyEnrolled);
                        if (Rank(student.Grade) < Rank(plan.MinimumGrade)) return Reject(Rejection.GradeTooLow);
                        bool eligible = plan.Prerequisite switch
                        {
                            NoPrerequisite => true,
                            PassedCourse p => state.Enrollments.Any(e => e.StudentId == c.StudentId && e.Course == p.Course && e.Progress is Passed)
                        };
                        if (!eligible) return Reject(Rejection.PrerequisiteMissing);
                        return room is Full ? Reject(Rejection.ClassFull) : Accept(new StudentEnrolled(id, c.StudentId));
                    }
                    return Reject(Rejection.UnknownStudent);
                case Drop c:
                    if (room is not (Available or Full)) return Reject(Rejection.RegistrationClosed);
                    return Members(room).Contains(c.StudentId) ? Accept(new StudentDropped(id, c.StudentId)) : Reject(Rejection.NotEnrolled);
                case StartClass c:
                    if (room is not (Available or Full)) return Reject(Rejection.RegistrationClosed);
                    return c.StartedOn.Length == 0 ? Reject(Rejection.InvalidDate) : Accept(new ClassStarted(id, c.StartedOn));
                case Attend c: return Study(state, room, id, c.StudentId, c);
                case Assess c: return Study(state, room, id, c.StudentId, c);
                case CompleteClass:
                    if (room is not Teaching) return Reject(Rejection.NotTeaching);
                    return state.Enrollments.Any(e => e.ClassId == id && e.Progress is Studying)
                        ? Reject(Rejection.UnassessedStudents) : Accept(new ClassCompleted(id));
                case RegisterStudent or PromoteStudent or OpenClass: throw new InvalidOperationException();
            }
        }
        return new Rejected(Rejection.UnknownClass);
    }
    private static Decision Study(State state, ClassState room, string id, string student, Command command)
    {
        if (room is not Teaching) return new Rejected(Rejection.NotTeaching);
        if (!Members(room).Contains(student)) return new Rejected(Rejection.NotEnrolled);
        foreach (var e in state.Enrollments.Where(e => e.ClassId == id && e.StudentId == student))
        {
            if (e.Progress is not Studying study) return new Rejected(Rejection.AlreadyAssessed);
            return command switch
            {
                Attend => new Accepted([new AttendanceRecorded(id, student)]),
                Assess a => a.Score is < 0 or > 100 ? new Rejected(Rejection.InvalidScore)
                    : study.Attendance == 0 ? new Rejected(Rejection.NoAttendance) : new Accepted([new ResultRecorded(id, student, a.Score)]),
                RegisterStudent or PromoteStudent or OpenClass or Enroll or Drop or StartClass or CompleteClass => throw new InvalidOperationException("invariant")
            };
        }
        throw new InvalidOperationException("invariant");
    }
    public static Command Cause(Event e) => e switch
    {
        StudentRegistered x => new RegisterStudent(x.StudentId, x.Grade), StudentPromoted x => new PromoteStudent(x.StudentId, x.Grade),
        ClassOpened x => new OpenClass(x.ClassId, x.Course, x.Capacity), StudentEnrolled x => new Enroll(x.ClassId, x.StudentId),
        StudentDropped x => new Drop(x.ClassId, x.StudentId), ClassStarted x => new StartClass(x.ClassId, x.StartedOn),
        AttendanceRecorded x => new Attend(x.ClassId, x.StudentId), ResultRecorded x => new Assess(x.ClassId, x.StudentId, x.Score),
        ClassCompleted x => new CompleteClass(x.ClassId)
    };
    public static State Evolve(State s, Event e)
    {
        if (Decide(s, Cause(e)) is Rejected) throw new InvalidOperationException("InvalidHistory");
        switch (e)
        {
            case StudentRegistered x: return s with { Students = [.. s.Students, new(x.StudentId, x.Grade)] };
            case StudentPromoted x: return s with { Students = [.. s.Students.Select(v => v.StudentId == x.StudentId ? v with { Grade = x.Grade } : v)] };
            case ClassOpened x:
                var plan = x.Course switch
                {
                    Course.Foundation => new Plan(x.ClassId, x.Course, Grade.Year2, new NoPrerequisite(), x.Capacity),
                    Course.Advanced => new Plan(x.ClassId, x.Course, Grade.Year3, new PassedCourse(Course.Foundation), x.Capacity),
                    _ => throw new ArgumentException("course")
                };
                return s with { Classes = [.. s.Classes, new Available(plan, [])] };
            default: return ApplyClass(s, e);
        }
    }
    private static State ApplyClass(State s, Event e)
    {
        var command = Cause(e);
        string id = command switch
        {
            Enroll x => x.ClassId, Drop x => x.ClassId, StartClass x => x.ClassId, Attend x => x.ClassId,
            Assess x => x.ClassId, CompleteClass x => x.ClassId,
            RegisterStudent or PromoteStudent or OpenClass => throw new InvalidOperationException()
        };
        var room = s.Classes.Single(c => PlanOf(c).ClassId == id);
        var plan = PlanOf(room);
        var records = s.Enrollments;
        ClassState updated = room;
        switch (e)
        {
            case StudentEnrolled x:
                var members = Members(room).Add(x.StudentId);
                updated = members.Length == plan.Capacity ? new Full(plan, members) : new Available(plan, members);
                records = [.. records.Where(v => v.ClassId != id || v.StudentId != x.StudentId), new Enrollment(id, x.StudentId, plan.Course, new Reserved())];
                break;
            case StudentDropped x:
                updated = new Available(plan, [.. Members(room).Where(v => v != x.StudentId)]);
                records = [.. records.Select(v => v.ClassId == id && v.StudentId == x.StudentId ? v with { Progress = new Withdrawn(WithdrawalReason.StudentRequest) } : v)];
                break;
            case ClassStarted x:
                updated = new Teaching(plan, Members(room), x.StartedOn);
                records = [.. records.Select(v => v.ClassId == id && v.Progress is Reserved ? v with { Progress = new Studying(0) } : v)];
                break;
            case AttendanceRecorded x:
                records = [.. records.Select(v => v.ClassId == id && v.StudentId == x.StudentId && v.Progress is Studying p ? v with { Progress = new Studying(p.Attendance + 1) } : v)];
                break;
            case ResultRecorded x:
                Progress result = x.Score >= 60 ? new Passed(x.Score, plan.Course == Course.Foundation ? 1 : 2) : new Failed(x.Score, FailureReason.BelowPassMark);
                records = [.. records.Select(v => v.ClassId == id && v.StudentId == x.StudentId ? v with { Progress = result } : v)];
                break;
            case ClassCompleted:
                updated = new Completed(plan, [.. records.Where(v => v.ClassId == id && v.Progress is Passed or Failed).Select(v => new ClassResult(v.StudentId, v.Progress))]);
                break;
            case StudentRegistered or StudentPromoted or ClassOpened: throw new InvalidOperationException();
        }
        return s with { Classes = [.. s.Classes.Select(v => PlanOf(v).ClassId == id ? updated : v)], Enrollments = records };
    }
    public static State Replay(IEnumerable<Event> events) => events.Aggregate(Empty, Evolve);
}
