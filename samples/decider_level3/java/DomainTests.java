import playground.dcb.Dcb;
import java.util.List;
import java.util.Objects;
final class DomainTests {
    static final class CountingStore implements Dcb.CommandStore<Event>{
        final Dcb.EventStore<Event> inner;int reads;
        CountingStore(Dcb.EventStore<Event> inner){this.inner=inner;}
        public Dcb.ReadResult<Event> read(Dcb.Query query){reads++;return inner.read(query);}
        public Dcb.AppendResult<Event> append(String id,Dcb.Query query,int after,List<Event> events){return inner.append(id,query,after,events);}
    }
    static final class Given {
        final Dcb.EventStore<Event> store=new Dcb.EventStore<>(Routing::describe,e->e);
        final SchoolProjection projection=new SchoolProjection();
        final CountingStore port=new CountingStore(store);
        final Dcb.Executor<Event,Rejection> executor=new Dcb.DcbExecutor<>(port);
        final SchoolHandler handler=new SchoolHandler();
        Given(List<Event> events){if(!events.isEmpty())store.append("given",new Dcb.Query(List.of()),0,events);store.subscribe(projection::accept);}
    }
    static void require(boolean value,String name){if(!value)throw new AssertionError(name);}
    static void checkProjection(Given g){
        var events=g.store.read(new Dcb.Query(List.of())).events();var snapshot=g.projection.snapshot();
        require(snapshot.position()==events.size() && snapshot.state().equals(Projection.fold(events).list()),"live / replay state");
    }
    static void check(String name,List<Event> given,Command when,List<Event> thenEvents,List<Rejection> thenErrors,boolean invalid){
        var g=new Given(given); // Given
        var prepared=g.executor.prepare("when",when,g.handler); // When
        require(prepared.wasRead()!=invalid && g.port.reads==(invalid?0:1),name+": validation before reads");
        var result=prepared.commit();
        var appended=g.store.read(new Dcb.Query(List.of())).events().stream().skip(given.size()).map(Dcb.Recorded::payload).toList();
        require(result.status().equals(thenErrors.isEmpty()?"Committed":"Rejected") && result.errors().equals(thenErrors),name+": result"); // Then
        require(appended.equals(thenEvents) && result.appended()==thenEvents.size() && result.conflicts()==0,name+": exact events");
        checkProjection(g);
    }
    static void run(){
        List<Event> empty=List.of();
        List<Event> registered=List.of(new StudentRegistered("s",Grade.Year3));
        List<Event> opened=List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1));
        List<Event> enrolled=List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation));
        List<Event> teaching=List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"));
        List<Event> attended=List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"));
        List<Event> passed=List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",60));
        List<Event> failed=List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",59));
        List<Event> advanced=List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",60),new ClassOpened("b",Course.Advanced,1));
        check("register student",empty,new RegisterStudent("s",Grade.Year3),List.of(new StudentRegistered("s",Grade.Year3)),List.of(),false);
        check("duplicate student",registered,new RegisterStudent("s",Grade.Year3),List.of(),List.of(Rejection.DuplicateStudent),false);
        check("promote one grade",List.of(new StudentRegistered("s",Grade.Year1)),new PromoteStudent("s",Grade.Year2),List.of(new StudentPromoted("s",Grade.Year2)),List.of(),false);
        check("promotion cannot skip",List.of(new StudentRegistered("s",Grade.Year1)),new PromoteStudent("s",Grade.Year3),List.of(),List.of(Rejection.InvalidPromotion),false);
        check("promotion cannot go backwards",registered,new PromoteStudent("s",Grade.Year2),List.of(),List.of(Rejection.InvalidPromotion),false);
        check("unknown student promotion",empty,new PromoteStudent("s",Grade.Year2),List.of(),List.of(Rejection.UnknownStudent),false);
        check("open foundation",registered,new OpenClass("a",Course.Foundation,1),List.of(new ClassOpened("a",Course.Foundation,1)),List.of(),false);
        check("open advanced",registered,new OpenClass("b",Course.Advanced,10),List.of(new ClassOpened("b",Course.Advanced,10)),List.of(),false);
        check("capacity upper bound",empty,new OpenClass("a",Course.Foundation,10),List.of(new ClassOpened("a",Course.Foundation,10)),List.of(),false);
        check("duplicate class",opened,new OpenClass("a",Course.Foundation,1),List.of(),List.of(Rejection.DuplicateClass),false);
        check("enroll",opened,new Enroll("a","s"),List.of(new StudentEnrolled("a","s",Course.Foundation)),List.of(),false);
        check("unknown class",registered,new Enroll("a","s"),List.of(),List.of(Rejection.UnknownClass),false);
        check("unknown student enrollment",List.of(new ClassOpened("a",Course.Foundation,1)),new Enroll("a","s"),List.of(),List.of(Rejection.UnknownStudent),false);
        check("grade below minimum",List.of(new StudentRegistered("s",Grade.Year1),new ClassOpened("a",Course.Foundation,1)),new Enroll("a","s"),List.of(),List.of(Rejection.GradeTooLow),false);
        check("grade exactly minimum",List.of(new StudentRegistered("s",Grade.Year2),new ClassOpened("a",Course.Foundation,1)),new Enroll("a","s"),List.of(new StudentEnrolled("a","s",Course.Foundation)),List.of(),false);
        check("missing prerequisite",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("b",Course.Advanced,1)),new Enroll("b","s"),List.of(),List.of(Rejection.PrerequisiteMissing),false);
        check("failed prerequisite",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",59),new ClassOpened("b",Course.Advanced,1)),new Enroll("b","s"),List.of(),List.of(Rejection.PrerequisiteMissing),false);
        check("passed prerequisite",advanced,new Enroll("b","s"),List.of(new StudentEnrolled("b","s",Course.Advanced)),List.of(),false);
        check("duplicate enrollment",enrolled,new Enroll("a","s"),List.of(),List.of(Rejection.AlreadyEnrolled),false);
        check("last seat occupied",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new StudentRegistered("t",Grade.Year3)),new Enroll("a","t"),List.of(),List.of(Rejection.ClassFull),false);
        check("enrollment after start",teaching,new Enroll("a","s"),List.of(),List.of(Rejection.RegistrationClosed),false);
        check("drop reservation",enrolled,new Drop("a","s"),List.of(new StudentDropped("a","s")),List.of(),false);
        check("drop nonmember",opened,new Drop("a","s"),List.of(),List.of(Rejection.NotEnrolled),false);
        check("drop after start",teaching,new Drop("a","s"),List.of(),List.of(Rejection.RegistrationClosed),false);
        check("reenroll after withdrawal",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new StudentDropped("a","s")),new Enroll("a","s"),List.of(new StudentEnrolled("a","s",Course.Foundation)),List.of(),false);
        check("start empty class",opened,new StartClass("a","2026-04-01"),List.of(new ClassStarted("a","2026-04-01")),List.of(),false);
        check("start emits study event atomically",enrolled,new StartClass("a","2026-04-01"),List.of(new ClassStarted("a","2026-04-01"),new StudyStarted("a","s")),List.of(),false);
        check("start twice",teaching,new StartClass("a","2026-04-01"),List.of(),List.of(Rejection.RegistrationClosed),false);
        check("valid leap date",opened,new StartClass("a","2028-02-29"),List.of(new ClassStarted("a","2028-02-29")),List.of(),false);
        check("attendance before start",enrolled,new Attend("a","s"),List.of(),List.of(Rejection.NotTeaching),false);
        check("attendance",teaching,new Attend("a","s"),List.of(new AttendanceRecorded("a","s")),List.of(),false);
        check("attendance nonmember",teaching,new Attend("a","t"),List.of(),List.of(Rejection.NotEnrolled),false);
        check("assessment without attendance",teaching,new Assess("a","s",60),List.of(),List.of(Rejection.NoAttendance),false);
        check("assessment boundary 0",attended,new Assess("a","s",0),List.of(new ResultRecorded("a","s",0)),List.of(),false);
        check("assessment boundary 59",attended,new Assess("a","s",59),List.of(new ResultRecorded("a","s",59)),List.of(),false);
        check("assessment boundary 60",attended,new Assess("a","s",60),List.of(new ResultRecorded("a","s",60)),List.of(),false);
        check("assessment boundary 100",attended,new Assess("a","s",100),List.of(new ResultRecorded("a","s",100)),List.of(),false);
        check("assessment before start",enrolled,new Assess("a","s",60),List.of(),List.of(Rejection.NotTeaching),false);
        check("assessment nonmember",attended,new Assess("a","t",60),List.of(),List.of(Rejection.NotEnrolled),false);
        check("assessment twice",passed,new Assess("a","s",60),List.of(),List.of(Rejection.AlreadyAssessed),false);
        check("attendance after assessment",passed,new Attend("a","s"),List.of(),List.of(Rejection.AlreadyAssessed),false);
        check("completion before start",opened,new CompleteClass("a"),List.of(),List.of(Rejection.NotTeaching),false);
        check("completion with unassessed student",teaching,new CompleteClass("a"),List.of(),List.of(Rejection.UnassessedStudents),false);
        check("complete passed",passed,new CompleteClass("a"),List.of(new ClassCompleted("a",List.of(new ClassResult("s",new Passed(60,1))))),List.of(),false);
        check("complete failed",failed,new CompleteClass("a"),List.of(new ClassCompleted("a",List.of(new ClassResult("s",new Failed(59,FailureReason.BelowPassMark))))),List.of(),false);
        check("complete empty class",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new ClassStarted("a","2026-04-01")),new CompleteClass("a"),List.of(new ClassCompleted("a",List.of())),List.of(),false);
        check("unknown class Drop",registered,new Drop("missing","s"),List.of(),List.of(Rejection.UnknownClass),false);
        check("unknown class StartClass",registered,new StartClass("missing","2026-04-01"),List.of(),List.of(Rejection.UnknownClass),false);
        check("unknown class Attend",registered,new Attend("missing","s"),List.of(),List.of(Rejection.UnknownClass),false);
        check("unknown class Assess",registered,new Assess("missing","s",60),List.of(),List.of(Rejection.UnknownClass),false);
        check("unknown class CompleteClass",registered,new CompleteClass("missing"),List.of(),List.of(Rejection.UnknownClass),false);
        check("invalid id RegisterStudent",empty,new RegisterStudent(" ",Grade.Year2),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id PromoteStudent",empty,new PromoteStudent("",Grade.Year2),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id OpenClass",empty,new OpenClass("",Course.Foundation,1),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id Enroll",empty,new Enroll("a",""),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id Drop",empty,new Drop("","s"),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id StartClass",empty,new StartClass("","2026-04-01"),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id Attend",empty,new Attend("a","\t"),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id Assess",empty,new Assess("","s",60),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid id CompleteClass",empty,new CompleteClass(""),List.of(),List.of(Rejection.InvalidId),true);
        check("invalid capacity 0",empty,new OpenClass("a",Course.Foundation,0),List.of(),List.of(Rejection.InvalidCapacity),true);
        check("invalid capacity 11",empty,new OpenClass("a",Course.Foundation,11),List.of(),List.of(Rejection.InvalidCapacity),true);
        check("invalid score -1",empty,new Assess("a","s",-1),List.of(),List.of(Rejection.InvalidScore),true);
        check("invalid score 101",empty,new Assess("a","s",101),List.of(),List.of(Rejection.InvalidScore),true);
        check("invalid date ",empty,new StartClass("a",""),List.of(),List.of(Rejection.InvalidDate),true);
        check("invalid date 2026-02-29",empty,new StartClass("a","2026-02-29"),List.of(),List.of(Rejection.InvalidDate),true);
        check("invalid date 2026-04-31",empty,new StartClass("a","2026-04-31"),List.of(),List.of(Rejection.InvalidDate),true);
        check("invalid date 2026-1-01",empty,new StartClass("a","2026-1-01"),List.of(),List.of(Rejection.InvalidDate),true);
        check("invalid date 0000-01-01",empty,new StartClass("a","0000-01-01"),List.of(),List.of(Rejection.InvalidDate),true);
        check("invalid date 2026-13-01",empty,new StartClass("a","2026-13-01"),List.of(),List.of(Rejection.InvalidDate),true);
        check("historical capacity remains replayable",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,25)),new Enroll("a","s"),List.of(new StudentEnrolled("a","s",Course.Foundation)),List.of(),false);
        check("historical date remains replayable",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","Spring 2020"),new StudyStarted("a","s")),new Attend("a","s"),List.of(new AttendanceRecorded("a","s")),List.of(),false);
        check("historical score remains replayable",List.of(new StudentRegistered("s",Grade.Year3),new ClassOpened("a",Course.Foundation,1),new StudentEnrolled("a","s",Course.Foundation),new ClassStarted("a","2026-04-01"),new StudyStarted("a","s"),new AttendanceRecorded("a","s"),new ResultRecorded("a","s",120),new ClassOpened("b",Course.Advanced,1)),new Enroll("b","s"),List.of(new StudentEnrolled("b","s",Course.Advanced)),List.of(),false);
        check("new command cannot use historical capacity",empty,new OpenClass("a",Course.Foundation,25),List.of(),List.of(Rejection.InvalidCapacity),true);
        check("new command cannot use historical date",empty,new StartClass("a","Spring 2020"),List.of(),List.of(Rejection.InvalidDate),true);
        check("new command cannot use historical score",empty,new Assess("a","s",120),List.of(),List.of(Rejection.InvalidScore),true);
        var g=new Given(List.of(new StudentRegistered("s",Grade.Year3),new StudentRegistered("t",Grade.Year3),new ClassOpened("a",Course.Foundation,1)));
        var first=g.executor.prepare("first",new Enroll("a","s"),g.handler);
        var second=g.executor.prepare("second",new Enroll("a","t"),g.handler);
        require(first.commit().status().equals("Committed"),"first last-seat command");var conflict=second.commit();
        require(conflict.conflicts()==1 && conflict.errors().equals(List.of(Rejection.ClassFull)) && g.store.read(new Dcb.Query(List.of())).events().size()==4,"retry must redecide");checkProjection(g);
        boolean nullRejected=false;try{new Enroll("a",null);}catch(NullPointerException error){nullRejected=true;}
        require(nullRejected,"native constructor rejects null before execution");
        var state=Projection.Registers.empty();for(var e:opened)state=Projection.reduce(state,e);var pure=state.decisionModel();
        require(Decider.decide(pure,ValidatedCommand.create(new Enroll("a","s"))).equals(new Accepted(List.of(new StudentEnrolled("a","s",Course.Foundation)))),"pure Decider enrollment");
        require(Decider.decide(pure,ValidatedCommand.create(new Enroll("a","missing"))).equals(new Rejected(Rejection.UnknownStudent)),"pure Decider rejection");
        require(Decider.decide(pure,ValidatedCommand.create(new StartClass("a","2026-04-01"))).equals(new Accepted(List.of(new ClassStarted("a","2026-04-01")))),"pure empty-class start");
        require(StudentId.create("s").value().equals("s") && SeatCapacity.create(10).value()==10,"valid smart constructors");
        for(Runnable invalidValue:List.<Runnable>of(()->ClassId.create(" "),()->SeatCapacity.create(25),()->AssessmentScore.create(120),()->SchoolDate.parse("Spring 2020"))){
            boolean rejected=false;try{invalidValue.run();}catch(InputViolation error){rejected=true;}require(rejected,"smart constructor rejects invalid value");
        }
        require(SeatCapacity.fromEvent(25).value()==25 && AssessmentScore.fromEvent(120).value()==120 && SchoolDate.fromEvent("Spring 2020").value().equals("Spring 2020"),"event restoration preserves historical values");
        System.out.println("domain: 76 GWT cases + conflict + null constructor case PASS");
    }
}
