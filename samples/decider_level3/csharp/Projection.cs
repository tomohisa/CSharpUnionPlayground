using System.Collections.Immutable;
using Playground.Dcb;
using System.Text;
namespace SchoolDomain;

public sealed record Registers(ImmutableSortedDictionary<string,Student> Students,
    ImmutableSortedDictionary<string,ClassState> Classes, ImmutableSortedDictionary<(string,string),Enrollment> Enrollments)
{
    public static Registers Empty => new(ImmutableSortedDictionary.Create<string,Student>(StringComparer.Ordinal),
        ImmutableSortedDictionary.Create<string,ClassState>(StringComparer.Ordinal), ImmutableSortedDictionary.Create<(string,string),Enrollment>(Comparer<(string,string)>.Create((a,b)=>{int c=CompareIds(a.Item1,b.Item1);return c!=0?c:CompareIds(a.Item2,b.Item2);})));
    public static int CompareIds(string a,string b)
    {
        var x=a.EnumerateRunes();var y=b.EnumerateRunes();
        while(true){bool ax=x.MoveNext(),by=y.MoveNext();if(!ax||!by)return ax?1:by?-1:0;int c=x.Current.Value.CompareTo(y.Current.Value);if(c!=0)return c;}
    }
    private static readonly IComparer<string> IdOrder=Comparer<string>.Create(CompareIds);
    public DecisionModel DecisionModel() => new([.. Students.Values.OrderBy(s=>s.StudentId,IdOrder)],
        [.. Classes.Values.OrderBy(c=>Decider.PlanOf(c).ClassId,IdOrder)],
        [.. Enrollments.Values.OrderBy(e=>e.ClassId,IdOrder).ThenBy(e=>e.StudentId,IdOrder)]);
    public SchoolList List() {var m=DecisionModel();return new(m.Students,m.Classes,m.Enrollments,[.. m.Enrollments.Select(e=>Decider.Describe(e.Progress))]);}
    public static Registers Reduce(Registers state, Event e) => new(StudentRegister.Apply(state.Students,e),
        ClassRegister.Apply(state.Classes,e), EnrollmentRegister.Apply(state.Enrollments,e));
    public static Registers Fold(IEnumerable<Recorded<Event>> events) => events.Aggregate(Empty,(s,e) => Reduce(s,e.Payload));
}
public static class StudentRegister
{
    public static ImmutableSortedDictionary<string,Student> Apply(ImmutableSortedDictionary<string,Student> students, Event e) => e switch
    {
        StudentRegistered x => students.SetItem(x.StudentId,new(x.StudentId,x.Grade)),
        StudentPromoted x => students.SetItem(x.StudentId,new(x.StudentId,x.Grade)),
        _ => students // Events from other domains do not affect the student registry.
    };
}
public static class ClassRegister
{
    public static ImmutableSortedDictionary<string,ClassState> Apply(ImmutableSortedDictionary<string,ClassState> classes, Event e)
    {
        if (e is ClassOpened opened)
        {
            var plan = opened.Course switch
            {
                Course.Foundation => new Plan(opened.ClassId,opened.Course,Grade.Year2,new NoPrerequisite(),SeatCapacity.FromEvent(opened.Capacity).Value),
                Course.Advanced => new Plan(opened.ClassId,opened.Course,Grade.Year3,new PassedCourse(Course.Foundation),SeatCapacity.FromEvent(opened.Capacity).Value),
                _ => throw new ArgumentException("course")
            };
            return classes.SetItem(opened.ClassId,new Available(plan,[]));
        }
        string id = e switch { StudentEnrolled x => x.ClassId, StudentDropped x => x.ClassId,
            ClassStarted x => x.ClassId, ClassCompleted x => x.ClassId, _ => "" };
        // A student-tag query can contain enrollment facts without this class's opening.
        if (!classes.TryGetValue(id,out var room)) return classes;
        var p = Decider.PlanOf(room);
        ClassState next = e switch
        {
            StudentEnrolled x => Decider.Members(room).Add(x.StudentId) is var members && members.Length == p.Capacity ? new Full(p,members) : new Available(p,Decider.Members(room).Add(x.StudentId)),
            StudentDropped x => new Available(p,[.. Decider.Members(room).Where(s => s != x.StudentId)]),
            ClassStarted x => new Teaching(p,Decider.Members(room),SchoolDate.FromEvent(x.StartedOn).Value),
            ClassCompleted x => new Completed(p,x.Results),
            _ => room
        };
        return classes.SetItem(id,next);
    }
}
public static class EnrollmentRegister
{
    public static ImmutableSortedDictionary<(string,string),Enrollment> Apply(ImmutableSortedDictionary<(string,string),Enrollment> records, Event e)
    {
        if (e is StudentEnrolled enrolled) return records.SetItem((enrolled.ClassId,enrolled.StudentId),new(enrolled.ClassId,enrolled.StudentId,enrolled.Course,new Reserved()));
        (string,string) key = e switch { StudentDropped x => (x.ClassId,x.StudentId), StudyStarted x => (x.ClassId,x.StudentId),
            AttendanceRecorded x => (x.ClassId,x.StudentId), ResultRecorded x => (x.ClassId,x.StudentId), _ => ("","") };
        if (!records.TryGetValue(key,out var record)) return records;
        Progress progress = e switch
        {
            StudentDropped => new Withdrawn(WithdrawalReason.StudentRequest), StudyStarted => new Studying(0),
            AttendanceRecorded => record.Progress is Studying p ? new Studying(p.Attendance+1) : throw new InvalidOperationException("invalid committed event"),
            ResultRecorded x => x.Score >= 60 ? new Passed(AssessmentScore.FromEvent(x.Score).Value,record.Course == Course.Foundation ? 1 : 2) : new Failed(AssessmentScore.FromEvent(x.Score).Value,FailureReason.BelowPassMark),
            _ => record.Progress
        };
        return records.SetItem(key,record with {Progress=progress});
    }
}
