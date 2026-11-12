using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Playground.Dcb;
namespace SchoolDomain;

public static class DomainTests
{
    private sealed class CountingStore(EventStore<Event> inner):ICommandStore<Event>
    {
        public int Reads;
        public ReadResult<Event> Read(Query q){Reads++;return inner.Read(q);}
        public AppendResult<Event> Append(string id,Query q,int after,ImmutableArray<Event> events)=>inner.Append(id,q,after,events);
    }
    private sealed class Given
    {
        public readonly EventStore<Event> Store=new(Routing.Describe,e=>e);
        public readonly SchoolProjection Projection=new();
        public readonly CountingStore Port;
        public readonly IDcbExecutor<Event,Rejection> Executor;
        public readonly SchoolHandler Handler=new();
        public Given(ImmutableArray<Event> events)
        {
            if(!events.IsEmpty)Store.Append("given",new([]),0,events);
            Store.Subscribe(Projection.Accept);
            Port=new(Store);Executor=new DcbExecutor<Event,Rejection>(Port);
        }
    }
    private static void Require(bool condition,string name){if(!condition)throw new Exception(name);}
    private static bool Same(Event a,Event b)=>a is ClassCompleted x && b is ClassCompleted y
        ? x.ClassId==y.ClassId && x.Results.SequenceEqual(y.Results):Equals(a,b);
    private static void CheckProjection(Given g)
    {
        var events=g.Store.Read(new([])).Events;
        var (position,state)=g.Projection.Snapshot();
        Require(position==events.Length,"live checkpoint");
        Require(JsonSerializer.Serialize(state,TestJson.Default.SchoolList)==JsonSerializer.Serialize(Registers.Fold(events).List(),TestJson.Default.SchoolList),"live / replay state");
    }
    private static void check(string name,ImmutableArray<Event> given,Command when,ImmutableArray<Event> thenEvents,ImmutableArray<Rejection> thenErrors,bool invalid)
    {
        var g=new Given(given); // Given: independent event history, not commands under test.
        var prepared=g.Executor.Prepare("when",when,g.Handler); // When: real public executor + handler.
        Require(prepared.WasRead!=invalid && g.Port.Reads==(invalid?0:1),name+": validation must precede reads");
        var result=prepared.Commit();
        var appended=g.Store.Read(new([])).Events.Skip(given.Length).ToArray(); // Then: exact payloads and no partial writes.
        Require(result.Errors.SequenceEqual(thenErrors) && result.Status==(thenErrors.IsEmpty?"Committed":"Rejected"),name+": result");
        Require(appended.Length==thenEvents.Length && appended.Zip(thenEvents).All(p=>Same(p.First.Payload,p.Second)),name+": events");
        Require(result.Appended==thenEvents.Length && result.Conflicts==0,name+": append count");
        CheckProjection(g);
    }
    public static void Run()
    {
        ImmutableArray<Event> empty=[];
        ImmutableArray<Event> registered=[new StudentRegistered("s",Grade.Year3)];
        ImmutableArray<Event> opened=[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1)];
        ImmutableArray<Event> enrolled=[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation)];
        ImmutableArray<Event> teaching=[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s")];
        ImmutableArray<Event> attended=[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s")];
        ImmutableArray<Event> passed=[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",60)];
        ImmutableArray<Event> failed=[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",59)];
        ImmutableArray<Event> advanced=[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",60),new ClassOpened("b",Course.Advanced,1)];
        check("register student",empty,new RegisterStudent("s",Grade.Year3),[new StudentRegistered("s",Grade.Year3)],[],false);
        check("duplicate student",registered,new RegisterStudent("s",Grade.Year3),[],[Rejection.DuplicateStudent],false);
        check("promote one grade",[new StudentRegistered("s",Grade.Year1)],new PromoteStudent("s",Grade.Year2),[new StudentPromoted("s",Grade.Year2)],[],false);
        check("promotion cannot skip",[new StudentRegistered("s",Grade.Year1)],new PromoteStudent("s",Grade.Year3),[],[Rejection.InvalidPromotion],false);
        check("promotion cannot go backwards",registered,new PromoteStudent("s",Grade.Year2),[],[Rejection.InvalidPromotion],false);
        check("unknown student promotion",empty,new PromoteStudent("s",Grade.Year2),[],[Rejection.UnknownStudent],false);
        check("open foundation",registered,new OpenClass("a",Course.Foundation,1),[new ClassOpened("a",Course.Foundation,1)],[],false);
        check("open advanced",registered,new OpenClass("b",Course.Advanced,10),[new ClassOpened("b",Course.Advanced,10)],[],false);
        check("capacity upper bound",empty,new OpenClass("a",Course.Foundation,10),[new ClassOpened("a",Course.Foundation,10)],[],false);
        check("duplicate class",opened,new OpenClass("a",Course.Foundation,1),[],[Rejection.DuplicateClass],false);
        check("enroll",opened,new Enroll("a","s"),[new StudentEnrolled("a","s",Course.Foundation)],[],false);
        check("unknown class",registered,new Enroll("a","s"),[],[Rejection.UnknownClass],false);
        check("unknown student enrollment",[new ClassOpened("a",Course.Foundation,1)],new Enroll("a","s"),[],[Rejection.UnknownStudent],false);
        check("grade below minimum",[new StudentRegistered("s",Grade.Year1),new ClassOpened("a",Course.Foundation,1)],new Enroll("a","s"),[],[Rejection.GradeTooLow],false);
        check("grade exactly minimum",[new StudentRegistered("s",Grade.Year2),new ClassOpened("a",Course.Foundation,1)],new Enroll("a","s"),[new StudentEnrolled("a","s",Course.Foundation)],[],false);
        check("missing prerequisite",[new StudentRegistered("s",Grade.Year3),new ClassOpened("b",Course.Advanced,1)],new Enroll("b","s"),[],[Rejection.PrerequisiteMissing],false);
        check("failed prerequisite",[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",59),new ClassOpened("b",Course.Advanced,1)],new Enroll("b","s"),[],[Rejection.PrerequisiteMissing],false);
        check("passed prerequisite",advanced,new Enroll("b","s"),[new StudentEnrolled("b","s",Course.Advanced)],[],false);
        check("duplicate enrollment",enrolled,new Enroll("a","s"),[],[Rejection.AlreadyEnrolled],false);
        check("last seat occupied",[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new StudentRegistered("t",Grade.Year3)],new Enroll("a","t"),[],[Rejection.ClassFull],false);
        check("enrollment after start",teaching,new Enroll("a","s"),[],[Rejection.RegistrationClosed],false);
        check("drop reservation",enrolled,new Drop("a","s"),[new StudentDropped("a","s")],[],false);
        check("drop nonmember",opened,new Drop("a","s"),[],[Rejection.NotEnrolled],false);
        check("drop after start",teaching,new Drop("a","s"),[],[Rejection.RegistrationClosed],false);
        check("reenroll after withdrawal",[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new StudentDropped("a","s")],new Enroll("a","s"),[new StudentEnrolled("a","s",Course.Foundation)],[],false);
        check("start empty class",opened,new StartClass("a","2026-04-01"),[new ClassStarted("a","2026-04-01")],[],false);
        check("start emits study event atomically",enrolled,new StartClass("a","2026-04-01"),[new ClassStarted("a","2026-04-01"),new StudyStarted("a","s")],[],false);
        check("start twice",teaching,new StartClass("a","2026-04-01"),[],[Rejection.RegistrationClosed],false);
        check("valid leap date",opened,new StartClass("a","2028-02-29"),[new ClassStarted("a","2028-02-29")],[],false);
        check("attendance before start",enrolled,new Attend("a","s"),[],[Rejection.NotTeaching],false);
        check("attendance",teaching,new Attend("a","s"),[new AttendanceRecorded("a","s")],[],false);
        check("attendance nonmember",teaching,new Attend("a","t"),[],[Rejection.NotEnrolled],false);
        check("assessment without attendance",teaching,new Assess("a","s",60),[],[Rejection.NoAttendance],false);
        check("assessment boundary 0",attended,new Assess("a","s",0),[new ResultRecorded("a","s",0)],[],false);
        check("assessment boundary 59",attended,new Assess("a","s",59),[new ResultRecorded("a","s",59)],[],false);
        check("assessment boundary 60",attended,new Assess("a","s",60),[new ResultRecorded("a","s",60)],[],false);
        check("assessment boundary 100",attended,new Assess("a","s",100),[new ResultRecorded("a","s",100)],[],false);
        check("assessment before start",enrolled,new Assess("a","s",60),[],[Rejection.NotTeaching],false);
        check("assessment nonmember",attended,new Assess("a","t",60),[],[Rejection.NotEnrolled],false);
        check("assessment twice",passed,new Assess("a","s",60),[],[Rejection.AlreadyAssessed],false);
        check("attendance after assessment",passed,new Attend("a","s"),[],[Rejection.AlreadyAssessed],false);
        check("completion before start",opened,new CompleteClass("a"),[],[Rejection.NotTeaching],false);
        check("completion with unassessed student",teaching,new CompleteClass("a"),[],[Rejection.UnassessedStudents],false);
        check("complete passed",passed,new CompleteClass("a"),[new ClassCompleted("a",[new ClassResult("s",new Passed(60,1))])],[],false);
        check("complete failed",failed,new CompleteClass("a"),[new ClassCompleted("a",[new ClassResult("s",new Failed(59,FailureReason.BelowPassMark))])],[],false);
        check("complete empty class",[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new ClassStarted("a","2026-04-01")],new CompleteClass("a"),[new ClassCompleted("a",[])],[],false);
        check("unknown class Drop",registered,new Drop("missing","s"),[],[Rejection.UnknownClass],false);
        check("unknown class StartClass",registered,new StartClass("missing","2026-04-01"),[],[Rejection.UnknownClass],false);
        check("unknown class Attend",registered,new Attend("missing","s"),[],[Rejection.UnknownClass],false);
        check("unknown class Assess",registered,new Assess("missing","s",60),[],[Rejection.UnknownClass],false);
        check("unknown class CompleteClass",registered,new CompleteClass("missing"),[],[Rejection.UnknownClass],false);
        check("invalid id RegisterStudent",empty,new RegisterStudent(" ",Grade.Year2),[],[Rejection.InvalidId],true);
        check("invalid id PromoteStudent",empty,new PromoteStudent("",Grade.Year2),[],[Rejection.InvalidId],true);
        check("invalid id OpenClass",empty,new OpenClass("",Course.Foundation,1),[],[Rejection.InvalidId],true);
        check("invalid id Enroll",empty,new Enroll("a",""),[],[Rejection.InvalidId],true);
        check("invalid id Drop",empty,new Drop("","s"),[],[Rejection.InvalidId],true);
        check("invalid id StartClass",empty,new StartClass("","2026-04-01"),[],[Rejection.InvalidId],true);
        check("invalid id Attend",empty,new Attend("a","\t"),[],[Rejection.InvalidId],true);
        check("invalid id Assess",empty,new Assess("","s",60),[],[Rejection.InvalidId],true);
        check("invalid id CompleteClass",empty,new CompleteClass(""),[],[Rejection.InvalidId],true);
        check("invalid capacity 0",empty,new OpenClass("a",Course.Foundation,0),[],[Rejection.InvalidCapacity],true);
        check("invalid capacity 11",empty,new OpenClass("a",Course.Foundation,11),[],[Rejection.InvalidCapacity],true);
        check("invalid score -1",empty,new Assess("a","s",-1),[],[Rejection.InvalidScore],true);
        check("invalid score 101",empty,new Assess("a","s",101),[],[Rejection.InvalidScore],true);
        check("invalid date ",empty,new StartClass("a",""),[],[Rejection.InvalidDate],true);
        check("invalid date 2026-02-29",empty,new StartClass("a","2026-02-29"),[],[Rejection.InvalidDate],true);
        check("invalid date 2026-04-31",empty,new StartClass("a","2026-04-31"),[],[Rejection.InvalidDate],true);
        check("invalid date 2026-1-01",empty,new StartClass("a","2026-1-01"),[],[Rejection.InvalidDate],true);
        check("invalid date 0000-01-01",empty,new StartClass("a","0000-01-01"),[],[Rejection.InvalidDate],true);
        check("invalid date 2026-13-01",empty,new StartClass("a","2026-13-01"),[],[Rejection.InvalidDate],true);
        check("historical capacity remains replayable",[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,25)],new Enroll("a","s"),[new StudentEnrolled("a","s",Course.Foundation)],[],false);
        check("historical date remains replayable",[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","Spring 2020"),new StudyStarted("a","s")],new Attend("a","s"),[new AttendanceRecorded("a","s")],[],false);
        check("historical score remains replayable",[new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",120),new ClassOpened("b",Course.Advanced,1)],new Enroll("b","s"),[new StudentEnrolled("b","s",Course.Advanced)],[],false);
        check("new command cannot use historical capacity",empty,new OpenClass("a",Course.Foundation,25),[],[Rejection.InvalidCapacity],true);
        check("new command cannot use historical date",empty,new StartClass("a","Spring 2020"),[],[Rejection.InvalidDate],true);
        check("new command cannot use historical score",empty,new Assess("a","s",120),[],[Rejection.InvalidScore],true);
        var g=new Given([new StudentRegistered("s",Grade.Year3),new StudentRegistered("t",Grade.Year3),new ClassOpened("a",Course.Foundation,1)]);
        var first=g.Executor.Prepare("first",new Enroll("a","s"),g.Handler);
        var second=g.Executor.Prepare("second",new Enroll("a","t"),g.Handler);
        Require(first.Commit().Status=="Committed","first last-seat command");
        var conflict=second.Commit();
        Require(conflict.Conflicts==1 && conflict.Errors.SequenceEqual([Rejection.ClassFull]) && g.Store.Read(new([])).Events.Length==4,"retry must revalidate current domain state");
        CheckProjection(g);
        check("invalid native enum",empty,new RegisterStudent("s",(Grade)999),[],[Rejection.InvalidCommand],true);
        check("null native ID",empty,new Enroll("a",null!),[],[Rejection.InvalidId],true);
        var pure=opened.Aggregate(Registers.Empty,Registers.Reduce).DecisionModel();
        Require(Decider.Decide(pure,ValidatedCommand.Create(new Enroll("a","s"))) is Accepted {Events:[StudentEnrolled {ClassId:"a",StudentId:"s"} ]},"pure Decider enrollment");
        Require(Decider.Decide(pure,ValidatedCommand.Create(new Enroll("a","missing"))) is Rejected {Code:Rejection.UnknownStudent},"pure Decider rejection");
        Require(Decider.Decide(Registers.Reduce(Registers.Empty,new ClassOpened("a",Course.Foundation,1)).DecisionModel(),ValidatedCommand.Create(new StartClass("a","2026-04-01"))) is Accepted {Events:[ClassStarted]},"pure empty-class start");
        Require(StudentId.Create("s").Value=="s" && ClassId.Create("a").Value=="a","nominal ID factories");
        Require(SeatCapacity.Create(10).Value==10 && AssessmentScore.Create(0).Value==0,"valid VO boundaries");
        Require(SchoolDate.Create("2028-02-29").Value=="2028-02-29","calendar factory");
        foreach(Action invalidValue in new Action[]{()=>StudentId.Create(" "),()=>ClassId.Create(""),()=>SeatCapacity.Create(25),()=>AssessmentScore.Create(120),()=>SchoolDate.Create("Spring 2020")})
        {
            bool rejected=false;try{invalidValue();}catch(ArgumentException){rejected=true;}Require(rejected,"smart constructor rejects invalid value");
        }
        Require(SeatCapacity.FromEvent(25).Value==25 && AssessmentScore.FromEvent(120).Value==120 && SchoolDate.FromEvent("Spring 2020").Value=="Spring 2020","event restoration preserves historical values");
        Console.WriteLine("domain: 76 GWT cases + conflict + 2 native boundary cases PASS");
    }
}
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(SchoolList))]
internal partial class TestJson:JsonSerializerContext;
