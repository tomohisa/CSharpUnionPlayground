using System.Collections.Immutable;
namespace SchoolDomain;

public static class Decider
{
    public static DecisionModel Empty => new([], [], []);
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
    private static Decision DecideOne(DecisionModel state, Command command)
    {
        var students = state.Students;
        var classes = state.Classes;
        Accepted Accept(Event e) => new([e]);
        Rejected Reject(Rejection code) => new(code);
        switch (command)
        {
            case RegisterStudent c:
                if(!Enum.IsDefined(c.Grade))throw new ArgumentException("invalid grade");
                if (c.StudentId.Length == 0) return Reject(Rejection.InvalidId);
                return students.Any(s => s.StudentId == c.StudentId) ? Reject(Rejection.DuplicateStudent)
                    : Accept(new StudentRegistered(c.StudentId, c.Grade));
            case PromoteStudent c:
                if(!Enum.IsDefined(c.Grade))throw new ArgumentException("invalid grade");
                foreach (var s in students.Where(s => s.StudentId == c.StudentId))
                    return Rank(c.Grade) == Rank(s.Grade) + 1 ? Accept(new StudentPromoted(c.StudentId, c.Grade)) : Reject(Rejection.InvalidPromotion);
                return Reject(Rejection.UnknownStudent);
            case OpenClass c:
                if(!Enum.IsDefined(c.Course))throw new ArgumentException("invalid course");
                if (c.ClassId.Length == 0) return Reject(Rejection.InvalidId);
                if (c.Capacity is < 1 or > 10) return Reject(Rejection.InvalidCapacity);
                return classes.Any(x => PlanOf(x).ClassId == c.ClassId) ? Reject(Rejection.DuplicateClass)
                    : Accept(new ClassOpened(c.ClassId, c.Course, c.Capacity));
            default: return DecideClass(state, command);
        }
    }
    private static Decision DecideClass(DecisionModel state, Command command)
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
                        return room is Full ? Reject(Rejection.ClassFull) : Accept(new StudentEnrolled(id, c.StudentId, plan.Course));
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
                        ? Reject(Rejection.UnassessedStudents) : Accept(new ClassCompleted(id, [.. state.Enrollments.Where(e => e.ClassId == id && e.Progress is Passed or Failed).Select(e => new ClassResult(e.StudentId, e.Progress))]));
                case RegisterStudent or PromoteStudent or OpenClass: throw new InvalidOperationException();
            }
        }
        return new Rejected(Rejection.UnknownClass);
    }
    private static Decision Study(DecisionModel state, ClassState room, string id, string student, Command command)
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
    public static Decision Decide(DecisionModel state, Command command)
    {
        var decision = DecideOne(state, command);
        if (decision is Accepted a && a.Events[0] is ClassStarted e)
            return new Accepted([e, .. state.Enrollments.Where(r => r.ClassId == e.ClassId && r.Progress is Reserved).Select(r => (Event)new StudyStarted(e.ClassId, r.StudentId))]);
        return decision;
    }
}
