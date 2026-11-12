namespace SchoolDomain
open Playground.Dcb

module DomainTests =
    type CountingStore(inner:EventStore<Event>) =
        let mutable reads=0
        member _.Reads=reads
        interface ICommandStore<Event> with
            member _.Read query=reads<-reads+1;inner.Read query
            member _.Append(id,query,after,events)=inner.Append(id,query,after,events)
    type Given(events:Event list) =
        let store=EventStore<Event>(Routing.describe,id)
        let projection=SchoolProjection()
        do
            if not(List.isEmpty events) then store.Append("given",{Items=[]},0,events) |> ignore
            store.Subscribe projection.Accept
        let port=CountingStore(store)
        let executor:IDcbExecutor<Event,Rejection>=DcbExecutor(port,3)
        member _.Store=store
        member _.Projection=projection
        member _.Port=port
        member _.Executor=executor
    let require condition name = if not condition then failwith name
    let checkProjection (g:Given) =
        let events=(g.Store.Read {Items=[]}).Events
        let position,state=g.Projection.Snapshot()
        require (position=events.Length && state=Registers.list(Registers.fold events)) "live / replay state"
    let check name given command thenEvents thenErrors invalid =
        let g=Given(given) // Given: event history independent of the command under test.
        let prepared=g.Executor.Prepare("when",command,Handlers.school) // When
        require (prepared.WasRead<>invalid && g.Port.Reads=(if invalid then 0 else 1)) (name+": validation before reads")
        let result=prepared.Commit()
        let appended=(g.Store.Read {Items=[]}).Events |> List.skip given.Length |> List.map(fun e->e.Payload)
        require (result.Errors=thenErrors && result.Status=(if List.isEmpty thenErrors then "Committed" else "Rejected")) (name+": result")
        require (appended=thenEvents && result.Appended=thenEvents.Length && result.Conflicts=0) (name+": exact events") // Then
        checkProjection g
    let run () =
        let empty:Event list=[]
        let registered:Event list=[Event.StudentRegistered("s",Year3)]
        let opened:Event list=[Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1)]
        let enrolled:Event list=[Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation)]
        let teaching:Event list=[Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s")]
        let attended:Event list=[Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s");Event.AttendanceRecorded("a","s")]
        let passed:Event list=[Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s");Event.AttendanceRecorded("a","s");Event.ResultRecorded("a","s",60)]
        let failed:Event list=[Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s");Event.AttendanceRecorded("a","s");Event.ResultRecorded("a","s",59)]
        let advanced:Event list=[Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s");Event.AttendanceRecorded("a","s");Event.ResultRecorded("a","s",60);Event.ClassOpened("b",Advanced,1)]
        check ("register student") (empty) (Command.RegisterStudent("s",Year3)) ([Event.StudentRegistered("s",Year3)]) ([]) (false)
        check ("duplicate student") (registered) (Command.RegisterStudent("s",Year3)) ([]) ([Rejection.DuplicateStudent]) (false)
        check ("promote one grade") ([Event.StudentRegistered("s",Year1)]) (Command.PromoteStudent("s",Year2)) ([Event.StudentPromoted("s",Year2)]) ([]) (false)
        check ("promotion cannot skip") ([Event.StudentRegistered("s",Year1)]) (Command.PromoteStudent("s",Year3)) ([]) ([Rejection.InvalidPromotion]) (false)
        check ("promotion cannot go backwards") (registered) (Command.PromoteStudent("s",Year2)) ([]) ([Rejection.InvalidPromotion]) (false)
        check ("unknown student promotion") (empty) (Command.PromoteStudent("s",Year2)) ([]) ([Rejection.UnknownStudent]) (false)
        check ("open foundation") (registered) (Command.OpenClass("a",Foundation,1)) ([Event.ClassOpened("a",Foundation,1)]) ([]) (false)
        check ("open advanced") (registered) (Command.OpenClass("b",Advanced,10)) ([Event.ClassOpened("b",Advanced,10)]) ([]) (false)
        check ("capacity upper bound") (empty) (Command.OpenClass("a",Foundation,10)) ([Event.ClassOpened("a",Foundation,10)]) ([]) (false)
        check ("duplicate class") (opened) (Command.OpenClass("a",Foundation,1)) ([]) ([Rejection.DuplicateClass]) (false)
        check ("enroll") (opened) (Command.Enroll("a","s")) ([Event.StudentEnrolled("a","s",Foundation)]) ([]) (false)
        check ("unknown class") (registered) (Command.Enroll("a","s")) ([]) ([Rejection.UnknownClass]) (false)
        check ("unknown student enrollment") ([Event.ClassOpened("a",Foundation,1)]) (Command.Enroll("a","s")) ([]) ([Rejection.UnknownStudent]) (false)
        check ("grade below minimum") ([Event.StudentRegistered("s",Year1);Event.ClassOpened("a",Foundation,1)]) (Command.Enroll("a","s")) ([]) ([Rejection.GradeTooLow]) (false)
        check ("grade exactly minimum") ([Event.StudentRegistered("s",Year2);Event.ClassOpened("a",Foundation,1)]) (Command.Enroll("a","s")) ([Event.StudentEnrolled("a","s",Foundation)]) ([]) (false)
        check ("missing prerequisite") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("b",Advanced,1)]) (Command.Enroll("b","s")) ([]) ([Rejection.PrerequisiteMissing]) (false)
        check ("failed prerequisite") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s");Event.AttendanceRecorded("a","s");Event.ResultRecorded("a","s",59);Event.ClassOpened("b",Advanced,1)]) (Command.Enroll("b","s")) ([]) ([Rejection.PrerequisiteMissing]) (false)
        check ("passed prerequisite") (advanced) (Command.Enroll("b","s")) ([Event.StudentEnrolled("b","s",Advanced)]) ([]) (false)
        check ("duplicate enrollment") (enrolled) (Command.Enroll("a","s")) ([]) ([Rejection.AlreadyEnrolled]) (false)
        check ("last seat occupied") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.StudentRegistered("t",Year3)]) (Command.Enroll("a","t")) ([]) ([Rejection.ClassFull]) (false)
        check ("enrollment after start") (teaching) (Command.Enroll("a","s")) ([]) ([Rejection.RegistrationClosed]) (false)
        check ("drop reservation") (enrolled) (Command.Drop("a","s")) ([Event.StudentDropped("a","s")]) ([]) (false)
        check ("drop nonmember") (opened) (Command.Drop("a","s")) ([]) ([Rejection.NotEnrolled]) (false)
        check ("drop after start") (teaching) (Command.Drop("a","s")) ([]) ([Rejection.RegistrationClosed]) (false)
        check ("reenroll after withdrawal") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.StudentDropped("a","s")]) (Command.Enroll("a","s")) ([Event.StudentEnrolled("a","s",Foundation)]) ([]) (false)
        check ("start empty class") (opened) (Command.StartClass("a","2026-04-01")) ([Event.ClassStarted("a","2026-04-01")]) ([]) (false)
        check ("start emits study event atomically") (enrolled) (Command.StartClass("a","2026-04-01")) ([Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s")]) ([]) (false)
        check ("start twice") (teaching) (Command.StartClass("a","2026-04-01")) ([]) ([Rejection.RegistrationClosed]) (false)
        check ("valid leap date") (opened) (Command.StartClass("a","2028-02-29")) ([Event.ClassStarted("a","2028-02-29")]) ([]) (false)
        check ("attendance before start") (enrolled) (Command.Attend("a","s")) ([]) ([Rejection.NotTeaching]) (false)
        check ("attendance") (teaching) (Command.Attend("a","s")) ([Event.AttendanceRecorded("a","s")]) ([]) (false)
        check ("attendance nonmember") (teaching) (Command.Attend("a","t")) ([]) ([Rejection.NotEnrolled]) (false)
        check ("assessment without attendance") (teaching) (Command.Assess("a","s",60)) ([]) ([Rejection.NoAttendance]) (false)
        check ("assessment boundary 0") (attended) (Command.Assess("a","s",0)) ([Event.ResultRecorded("a","s",0)]) ([]) (false)
        check ("assessment boundary 59") (attended) (Command.Assess("a","s",59)) ([Event.ResultRecorded("a","s",59)]) ([]) (false)
        check ("assessment boundary 60") (attended) (Command.Assess("a","s",60)) ([Event.ResultRecorded("a","s",60)]) ([]) (false)
        check ("assessment boundary 100") (attended) (Command.Assess("a","s",100)) ([Event.ResultRecorded("a","s",100)]) ([]) (false)
        check ("assessment before start") (enrolled) (Command.Assess("a","s",60)) ([]) ([Rejection.NotTeaching]) (false)
        check ("assessment nonmember") (attended) (Command.Assess("a","t",60)) ([]) ([Rejection.NotEnrolled]) (false)
        check ("assessment twice") (passed) (Command.Assess("a","s",60)) ([]) ([Rejection.AlreadyAssessed]) (false)
        check ("attendance after assessment") (passed) (Command.Attend("a","s")) ([]) ([Rejection.AlreadyAssessed]) (false)
        check ("completion before start") (opened) (Command.CompleteClass("a")) ([]) ([Rejection.NotTeaching]) (false)
        check ("completion with unassessed student") (teaching) (Command.CompleteClass("a")) ([]) ([Rejection.UnassessedStudents]) (false)
        check ("complete passed") (passed) (Command.CompleteClass("a")) ([Event.ClassCompleted("a",[{StudentId="s";Progress=Progress.Passed(60,1)}])]) ([]) (false)
        check ("complete failed") (failed) (Command.CompleteClass("a")) ([Event.ClassCompleted("a",[{StudentId="s";Progress=Progress.Failed(59,BelowPassMark)}])]) ([]) (false)
        check ("complete empty class") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.ClassStarted("a","2026-04-01")]) (Command.CompleteClass("a")) ([Event.ClassCompleted("a",[])]) ([]) (false)
        check ("unknown class Drop") (registered) (Command.Drop("missing","s")) ([]) ([Rejection.UnknownClass]) (false)
        check ("unknown class StartClass") (registered) (Command.StartClass("missing","2026-04-01")) ([]) ([Rejection.UnknownClass]) (false)
        check ("unknown class Attend") (registered) (Command.Attend("missing","s")) ([]) ([Rejection.UnknownClass]) (false)
        check ("unknown class Assess") (registered) (Command.Assess("missing","s",60)) ([]) ([Rejection.UnknownClass]) (false)
        check ("unknown class CompleteClass") (registered) (Command.CompleteClass("missing")) ([]) ([Rejection.UnknownClass]) (false)
        check ("invalid id RegisterStudent") (empty) (Command.RegisterStudent(" ",Year2)) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id PromoteStudent") (empty) (Command.PromoteStudent("",Year2)) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id OpenClass") (empty) (Command.OpenClass("",Foundation,1)) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id Enroll") (empty) (Command.Enroll("a","")) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id Drop") (empty) (Command.Drop("","s")) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id StartClass") (empty) (Command.StartClass("","2026-04-01")) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id Attend") (empty) (Command.Attend("a","\t")) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id Assess") (empty) (Command.Assess("","s",60)) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid id CompleteClass") (empty) (Command.CompleteClass("")) ([]) ([Rejection.InvalidId]) (true)
        check ("invalid capacity 0") (empty) (Command.OpenClass("a",Foundation,0)) ([]) ([Rejection.InvalidCapacity]) (true)
        check ("invalid capacity 11") (empty) (Command.OpenClass("a",Foundation,11)) ([]) ([Rejection.InvalidCapacity]) (true)
        check ("invalid score -1") (empty) (Command.Assess("a","s",-1)) ([]) ([Rejection.InvalidScore]) (true)
        check ("invalid score 101") (empty) (Command.Assess("a","s",101)) ([]) ([Rejection.InvalidScore]) (true)
        check ("invalid date ") (empty) (Command.StartClass("a","")) ([]) ([Rejection.InvalidDate]) (true)
        check ("invalid date 2026-02-29") (empty) (Command.StartClass("a","2026-02-29")) ([]) ([Rejection.InvalidDate]) (true)
        check ("invalid date 2026-04-31") (empty) (Command.StartClass("a","2026-04-31")) ([]) ([Rejection.InvalidDate]) (true)
        check ("invalid date 2026-1-01") (empty) (Command.StartClass("a","2026-1-01")) ([]) ([Rejection.InvalidDate]) (true)
        check ("invalid date 0000-01-01") (empty) (Command.StartClass("a","0000-01-01")) ([]) ([Rejection.InvalidDate]) (true)
        check ("invalid date 2026-13-01") (empty) (Command.StartClass("a","2026-13-01")) ([]) ([Rejection.InvalidDate]) (true)
        check ("historical capacity remains replayable") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,25)]) (Command.Enroll("a","s")) ([Event.StudentEnrolled("a","s",Foundation)]) ([]) (false)
        check ("historical date remains replayable") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","Spring 2020");Event.StudyStarted("a","s")]) (Command.Attend("a","s")) ([Event.AttendanceRecorded("a","s")]) ([]) (false)
        check ("historical score remains replayable") ([Event.StudentRegistered("s",Year3);Event.ClassOpened("a",Foundation,1);Event.StudentEnrolled("a","s",Foundation);Event.ClassStarted("a","2026-04-01");Event.StudyStarted("a","s");Event.AttendanceRecorded("a","s");Event.ResultRecorded("a","s",120);Event.ClassOpened("b",Advanced,1)]) (Command.Enroll("b","s")) ([Event.StudentEnrolled("b","s",Advanced)]) ([]) (false)
        check ("new command cannot use historical capacity") (empty) (Command.OpenClass("a",Foundation,25)) ([]) ([Rejection.InvalidCapacity]) (true)
        check ("new command cannot use historical date") (empty) (Command.StartClass("a","Spring 2020")) ([]) ([Rejection.InvalidDate]) (true)
        check ("new command cannot use historical score") (empty) (Command.Assess("a","s",120)) ([]) ([Rejection.InvalidScore]) (true)
        let g=Given([Event.StudentRegistered("s",Year3);Event.StudentRegistered("t",Year3);Event.ClassOpened("a",Foundation,1)])
        let first=g.Executor.Prepare("first",Command.Enroll("a","s"),Handlers.school)
        let second=g.Executor.Prepare("second",Command.Enroll("a","t"),Handlers.school)
        require (first.Commit().Status="Committed") "first last-seat command"
        let result=second.Commit()
        require (result.Conflicts=1 && result.Errors=[Rejection.ClassFull] && (g.Store.Read {Items=[]}).Events.Length=4) "retry must redecide"
        checkProjection g
        let valid command=ValidatedCommand.create command |> function Ok value->value | Error error->failwithf "%A" error
        let pure=List.fold Registers.reduce Registers.empty opened |> Registers.decisionModel
        require (Decider.decide pure (valid(Command.Enroll("a","s")))=Decision.Accepted [Event.StudentEnrolled("a","s",Foundation)]) "pure Decider enrollment"
        require (Decider.decide pure (valid(Command.Enroll("a","missing")))=Decision.Rejected Rejection.UnknownStudent) "pure Decider rejection"
        require (Decider.decide pure (valid(Command.StartClass("a","2026-04-01")))=Decision.Accepted [Event.ClassStarted("a","2026-04-01")]) "pure empty-class start"
        require (StudentId.create "s" |> Result.isOk) "student ID smart constructor"
        require (ClassId.create " " |> Result.isError) "class ID smart constructor"
        require (SeatCapacity.create 25=Error Rejection.InvalidCapacity) "capacity smart constructor"
        require (AssessmentScore.create 120=Error Rejection.InvalidScore) "score smart constructor"
        require (SchoolDate.create "Spring 2020"=Error Rejection.InvalidDate) "date smart constructor"
        require (SeatCapacity.value(SeatCapacity.fromEvent 25)=25 && AssessmentScore.value(AssessmentScore.fromEvent 120)=120 && SchoolDate.value(SchoolDate.fromEvent "Spring 2020")="Spring 2020") "event restoration preserves historical values"
        printfn "domain: 76 GWT cases + conflict PASS"
