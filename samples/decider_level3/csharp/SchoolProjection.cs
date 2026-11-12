using System.Collections.Immutable;
using Playground.Dcb;
namespace SchoolDomain;

// Write-side index for the live list. Class rosters are normalized linked rows.
public sealed class SchoolProjection
{
    private readonly record struct Key(string Kind,string A="",string B="",int Node=0);
    private abstract record Row;
    private sealed record StudentRow(Student Value):Row;
    private sealed record ClassRow(ClassState Phase,int First=0,int Last=0,int Count=0):Row;
    private sealed record EnrollmentRow(Enrollment Value):Row;
    private sealed record MemberRow(string Student,int Previous,int Next):Row;
    private sealed record MemberIndex(int Node):Row;
    private readonly IndexedProjection<Key,Row,Event> index=new(Apply);
    public Student Student(string id)=>index.Read(rows=>((StudentRow)rows[new("student",id)]).Value).State;
    public (int Position,ImmutableArray<Student> State) FindStudent(string id)=>index.Read(rows=>rows.TryGetValue(new("student",id),out var row)&&row is StudentRow s?ImmutableArray.Create(s.Value):ImmutableArray<Student>.Empty);
    public int Checkpoint=>index.Checkpoint;
    public int RowCount=>index.RowCount;
    public long Work=>index.Work;
    public bool Accept(ImmutableArray<Recorded<Event>> events)=>index.Accept(events);
    private static Key ClassKey(string id)=>new("class",id);
    private static Key NodeKey(int id)=>new("node",Node:id);
    private static void Apply(IndexBatch<Key,Row> tx,Recorded<Event> recorded)
    {
        var e=recorded.Payload;
        switch(e)
        {
            case StudentRegistered x:tx.Put(new("student",x.StudentId),new StudentRow(new(x.StudentId,x.Grade)));return;
            case StudentPromoted x:tx.Put(new("student",x.StudentId),new StudentRow(new(x.StudentId,x.Grade)));return;
            case ClassOpened:
                var single=Registers.Reduce(Registers.Empty,e);
                var opened=(ClassOpened)e;tx.Put(ClassKey(opened.ClassId),new ClassRow(single.Classes[opened.ClassId]));return;
        }
        string id=e switch {StudentEnrolled x=>x.ClassId,StudentDropped x=>x.ClassId,ClassStarted x=>x.ClassId,
            ClassCompleted x=>x.ClassId,StudyStarted x=>x.ClassId,AttendanceRecorded x=>x.ClassId,ResultRecorded x=>x.ClassId,_=>throw new InvalidOperationException()};
        if(tx.TryGet(ClassKey(id),out var row))
        {
            var c=(ClassRow)row;var plan=Decider.PlanOf(c.Phase);
            switch(e)
            {
                case StudentEnrolled x:
                    var node=recorded.Position;
                    if(c.Last!=0){var last=(MemberRow)tx.Get(NodeKey(c.Last));tx.Put(NodeKey(c.Last),last with {Next=node});}
                    tx.Put(NodeKey(node),new MemberRow(x.StudentId,c.Last,0));tx.Put(new("member",id,x.StudentId),new MemberIndex(node));
                    var count=c.Count+1;c=c with {First=c.First==0?node:c.First,Last=node,Count=count,
                        Phase=count==plan.Capacity?new Full(plan,[]):new Available(plan,[])};break;
                case StudentDropped x:
                    if(tx.TryGet(new("member",id,x.StudentId),out var found))
                    {
                        int n=((MemberIndex)found).Node;var member=(MemberRow)tx.Get(NodeKey(n));
                        if(member.Previous!=0){var previous=(MemberRow)tx.Get(NodeKey(member.Previous));tx.Put(NodeKey(member.Previous),previous with {Next=member.Next});}
                        if(member.Next!=0){var next=(MemberRow)tx.Get(NodeKey(member.Next));tx.Put(NodeKey(member.Next),next with {Previous=member.Previous});}
                        tx.Delete(NodeKey(n));tx.Delete(new("member",id,x.StudentId));
                        c=c with {First=c.First==n?member.Next:c.First,Last=c.Last==n?member.Previous:c.Last,Count=c.Count-1};
                    }
                    c=c with {Phase=new Available(plan,[])};break;
                case ClassStarted x:c=c with {Phase=new Teaching(plan,[],SchoolDate.FromEvent(x.StartedOn).Value)};break;
                case ClassCompleted x:c=c with {Phase=new Completed(plan,x.Results)};break;
            }
            if(e is StudentEnrolled or StudentDropped or ClassStarted or ClassCompleted)tx.Put(ClassKey(id),c);
        }
        string student=e switch{StudentEnrolled x=>x.StudentId,StudentDropped x=>x.StudentId,StudyStarted x=>x.StudentId,
            AttendanceRecorded x=>x.StudentId,ResultRecorded x=>x.StudentId,_=>""};
        if(e is not (StudentEnrolled or StudentDropped or StudyStarted or AttendanceRecorded or ResultRecorded))return;
        var key=new Key("enrollment",id,student);
        if(e is StudentEnrolled enrollment){tx.Put(key,new EnrollmentRow(new(id,student,enrollment.Course,new Reserved())));return;}
        if(!tx.TryGet(key,out var existing))return;
        var value=((EnrollmentRow)existing).Value;
        Progress progress=e switch{
            StudentDropped=>new Withdrawn(WithdrawalReason.StudentRequest),StudyStarted=>new Studying(0),
            AttendanceRecorded when value.Progress is Studying p=>new Studying(p.Attendance+1),
            AttendanceRecorded=>throw new InvalidOperationException("invalid committed event"),
            ResultRecorded x=>x.Score>=60?new Passed(AssessmentScore.FromEvent(x.Score).Value,value.Course==Course.Foundation?1:2):new Failed(AssessmentScore.FromEvent(x.Score).Value,FailureReason.BelowPassMark),
            _=>value.Progress};
        tx.Put(key,new EnrollmentRow(value with {Progress=progress}));
    }
    public (int Position,SchoolList State) Snapshot()=>index.Read(rows=>
    {
        var students=new List<Student>();var classes=new List<ClassState>();var enrollments=new List<Enrollment>();
        foreach(var row in rows.Values)switch(row){case StudentRow x:students.Add(x.Value);break;case EnrollmentRow x:enrollments.Add(x.Value);break;
            case ClassRow x:
                var members=ImmutableArray.CreateBuilder<string>();
                if(x.Phase is not Completed)for(int n=x.First;n!=0;){var m=(MemberRow)rows[NodeKey(n)];members.Add(m.Student);n=m.Next;}
                var ids=members.ToImmutable();classes.Add(x.Phase switch{Available c=>c with {Students=ids},Full c=>c with {Students=ids},Teaching c=>c with {Students=ids},Completed c=>c});break;}
        students.Sort((a,b)=>Registers.CompareIds(a.StudentId,b.StudentId));classes.Sort((a,b)=>Registers.CompareIds(Decider.PlanOf(a).ClassId,Decider.PlanOf(b).ClassId));
        enrollments.Sort((a,b)=>{int c=Registers.CompareIds(a.ClassId,b.ClassId);return c!=0?c:Registers.CompareIds(a.StudentId,b.StudentId);});
        return new SchoolList([..students],[..classes],[..enrollments],[..enrollments.Select(e=>Decider.Describe(e.Progress))]);
    });
}
