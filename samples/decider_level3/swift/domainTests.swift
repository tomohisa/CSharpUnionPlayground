import Foundation
final class CountingStore:CommandStore,@unchecked Sendable {
    typealias Payload=Event
    let inner:EventStore<Event>
    var reads=0 // This probe is only used sequentially; threaded races live in libraryTests.
    init(_ inner:EventStore<Event>){self.inner=inner}
    func read(_ query:Query)->ReadResult<Event>{reads+=1;return inner.read(query)}
    func append(_ id:String,_ query:Query,_ after:Int,_ events:[Event])->AppendResult<Event>{inner.append(id,query,after,events)}
}
struct GivenHistory {
    let store:EventStore<Event>
    let projection:SchoolProjection
    let port:CountingStore
    let executor:DcbExecutor<Event>
    init(_ events:[Event]){
        store=EventStore(describe:describeEvent,copyPayload:{$0})
        if !events.isEmpty{_ = store.append("given",Query(items:[]),0,events)}
        projection=SchoolProjection();store.subscribe(projection.accept)
        port=CountingStore(store);executor=DcbExecutor(store:port)
    }
}
func domainTests(){
    func encoded<T:Encodable>(_ value:T)->Data{let encoder=JSONEncoder();encoder.outputFormatting=[.sortedKeys];return try! encoder.encode(value)}
    func checkProjection(_ g:GivenHistory){
        let events=g.store.read(Query(items:[])).events, (position,state)=g.projection.snapshot()
        precondition(position==events.count && encoded(state)==encoded(fold(events).list()),"live / replay state")
    }
    func check(_ name:String,_ given:[Event],_ when:Command,_ thenEvents:[Event],_ thenErrors:[Rejection],_ invalid:Bool){
        let g=GivenHistory(given) // Given
        let prepared=g.executor.prepare("when",when,SchoolHandler()) // When
        precondition(prepared.wasRead != invalid && g.port.reads == (invalid ? 0:1),name+": validation before reads")
        let result=prepared.commit(),appended=g.store.read(Query(items:[])).events.dropFirst(given.count).map{$0.payload}
        precondition(result.status == (thenErrors.isEmpty ? "Committed":"Rejected") && result.errors==thenErrors,name+": result") // Then
        precondition(encoded(appended)==encoded(thenEvents) && result.appended==thenEvents.count && result.conflicts==0,name+": exact events")
        checkProjection(g)
    }
        let empty:[Event]=[]
        let registered:[Event]=[.studentRegistered(studentId:"s",grade:.year3)]
        let opened:[Event]=[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1)]
        let enrolled:[Event]=[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation)]
        let teaching:[Event]=[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s")]
        let attended:[Event]=[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s"),.attendanceRecorded(classId:"a",studentId:"s")]
        let passed:[Event]=[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s"),.attendanceRecorded(classId:"a",studentId:"s"),.resultRecorded(classId:"a",studentId:"s",score:60)]
        let failed:[Event]=[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s"),.attendanceRecorded(classId:"a",studentId:"s"),.resultRecorded(classId:"a",studentId:"s",score:59)]
        let advanced:[Event]=[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s"),.attendanceRecorded(classId:"a",studentId:"s"),.resultRecorded(classId:"a",studentId:"s",score:60),.classOpened(classId:"b",course:.advanced,capacity:1)]
        check("register student",empty,.registerStudent(studentId:"s",grade:.year3),[.studentRegistered(studentId:"s",grade:.year3)],[],false)
        check("duplicate student",registered,.registerStudent(studentId:"s",grade:.year3),[],[.duplicateStudent],false)
        check("promote one grade",[.studentRegistered(studentId:"s",grade:.year1)],.promoteStudent(studentId:"s",grade:.year2),[.studentPromoted(studentId:"s",grade:.year2)],[],false)
        check("promotion cannot skip",[.studentRegistered(studentId:"s",grade:.year1)],.promoteStudent(studentId:"s",grade:.year3),[],[.invalidPromotion],false)
        check("promotion cannot go backwards",registered,.promoteStudent(studentId:"s",grade:.year2),[],[.invalidPromotion],false)
        check("unknown student promotion",empty,.promoteStudent(studentId:"s",grade:.year2),[],[.unknownStudent],false)
        check("open foundation",registered,.openClass(classId:"a",course:.foundation,capacity:1),[.classOpened(classId:"a",course:.foundation,capacity:1)],[],false)
        check("open advanced",registered,.openClass(classId:"b",course:.advanced,capacity:10),[.classOpened(classId:"b",course:.advanced,capacity:10)],[],false)
        check("capacity upper bound",empty,.openClass(classId:"a",course:.foundation,capacity:10),[.classOpened(classId:"a",course:.foundation,capacity:10)],[],false)
        check("duplicate class",opened,.openClass(classId:"a",course:.foundation,capacity:1),[],[.duplicateClass],false)
        check("enroll",opened,.enroll(classId:"a",studentId:"s"),[.studentEnrolled(classId:"a",studentId:"s",course:.foundation)],[],false)
        check("unknown class",registered,.enroll(classId:"a",studentId:"s"),[],[.unknownClass],false)
        check("unknown student enrollment",[.classOpened(classId:"a",course:.foundation,capacity:1)],.enroll(classId:"a",studentId:"s"),[],[.unknownStudent],false)
        check("grade below minimum",[.studentRegistered(studentId:"s",grade:.year1),.classOpened(classId:"a",course:.foundation,capacity:1)],.enroll(classId:"a",studentId:"s"),[],[.gradeTooLow],false)
        check("grade exactly minimum",[.studentRegistered(studentId:"s",grade:.year2),.classOpened(classId:"a",course:.foundation,capacity:1)],.enroll(classId:"a",studentId:"s"),[.studentEnrolled(classId:"a",studentId:"s",course:.foundation)],[],false)
        check("missing prerequisite",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"b",course:.advanced,capacity:1)],.enroll(classId:"b",studentId:"s"),[],[.prerequisiteMissing],false)
        check("failed prerequisite",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s"),.attendanceRecorded(classId:"a",studentId:"s"),.resultRecorded(classId:"a",studentId:"s",score:59),.classOpened(classId:"b",course:.advanced,capacity:1)],.enroll(classId:"b",studentId:"s"),[],[.prerequisiteMissing],false)
        check("passed prerequisite",advanced,.enroll(classId:"b",studentId:"s"),[.studentEnrolled(classId:"b",studentId:"s",course:.advanced)],[],false)
        check("duplicate enrollment",enrolled,.enroll(classId:"a",studentId:"s"),[],[.alreadyEnrolled],false)
        check("last seat occupied",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.studentRegistered(studentId:"t",grade:.year3)],.enroll(classId:"a",studentId:"t"),[],[.classFull],false)
        check("enrollment after start",teaching,.enroll(classId:"a",studentId:"s"),[],[.registrationClosed],false)
        check("drop reservation",enrolled,.drop(classId:"a",studentId:"s"),[.studentDropped(classId:"a",studentId:"s")],[],false)
        check("drop nonmember",opened,.drop(classId:"a",studentId:"s"),[],[.notEnrolled],false)
        check("drop after start",teaching,.drop(classId:"a",studentId:"s"),[],[.registrationClosed],false)
        check("reenroll after withdrawal",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.studentDropped(classId:"a",studentId:"s")],.enroll(classId:"a",studentId:"s"),[.studentEnrolled(classId:"a",studentId:"s",course:.foundation)],[],false)
        check("start empty class",opened,.startClass(classId:"a",startedOn:"2026-04-01"),[.classStarted(classId:"a",startedOn:"2026-04-01")],[],false)
        check("start emits study event atomically",enrolled,.startClass(classId:"a",startedOn:"2026-04-01"),[.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s")],[],false)
        check("start twice",teaching,.startClass(classId:"a",startedOn:"2026-04-01"),[],[.registrationClosed],false)
        check("valid leap date",opened,.startClass(classId:"a",startedOn:"2028-02-29"),[.classStarted(classId:"a",startedOn:"2028-02-29")],[],false)
        check("attendance before start",enrolled,.attend(classId:"a",studentId:"s"),[],[.notTeaching],false)
        check("attendance",teaching,.attend(classId:"a",studentId:"s"),[.attendanceRecorded(classId:"a",studentId:"s")],[],false)
        check("attendance nonmember",teaching,.attend(classId:"a",studentId:"t"),[],[.notEnrolled],false)
        check("assessment without attendance",teaching,.assess(classId:"a",studentId:"s",score:60),[],[.noAttendance],false)
        check("assessment boundary 0",attended,.assess(classId:"a",studentId:"s",score:0),[.resultRecorded(classId:"a",studentId:"s",score:0)],[],false)
        check("assessment boundary 59",attended,.assess(classId:"a",studentId:"s",score:59),[.resultRecorded(classId:"a",studentId:"s",score:59)],[],false)
        check("assessment boundary 60",attended,.assess(classId:"a",studentId:"s",score:60),[.resultRecorded(classId:"a",studentId:"s",score:60)],[],false)
        check("assessment boundary 100",attended,.assess(classId:"a",studentId:"s",score:100),[.resultRecorded(classId:"a",studentId:"s",score:100)],[],false)
        check("assessment before start",enrolled,.assess(classId:"a",studentId:"s",score:60),[],[.notTeaching],false)
        check("assessment nonmember",attended,.assess(classId:"a",studentId:"t",score:60),[],[.notEnrolled],false)
        check("assessment twice",passed,.assess(classId:"a",studentId:"s",score:60),[],[.alreadyAssessed],false)
        check("attendance after assessment",passed,.attend(classId:"a",studentId:"s"),[],[.alreadyAssessed],false)
        check("completion before start",opened,.completeClass(classId:"a"),[],[.notTeaching],false)
        check("completion with unassessed student",teaching,.completeClass(classId:"a"),[],[.unassessedStudents],false)
        check("complete passed",passed,.completeClass(classId:"a"),[.classCompleted(classId:"a",results:[ClassResult(studentId:"s",progress:.passed(score:60,credits:1))])],[],false)
        check("complete failed",failed,.completeClass(classId:"a"),[.classCompleted(classId:"a",results:[ClassResult(studentId:"s",progress:.failed(score:59,reason:.belowPassMark))])],[],false)
        check("complete empty class",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.classStarted(classId:"a",startedOn:"2026-04-01")],.completeClass(classId:"a"),[.classCompleted(classId:"a",results:[])],[],false)
        check("unknown class Drop",registered,.drop(classId:"missing",studentId:"s"),[],[.unknownClass],false)
        check("unknown class StartClass",registered,.startClass(classId:"missing",startedOn:"2026-04-01"),[],[.unknownClass],false)
        check("unknown class Attend",registered,.attend(classId:"missing",studentId:"s"),[],[.unknownClass],false)
        check("unknown class Assess",registered,.assess(classId:"missing",studentId:"s",score:60),[],[.unknownClass],false)
        check("unknown class CompleteClass",registered,.completeClass(classId:"missing"),[],[.unknownClass],false)
        check("invalid id RegisterStudent",empty,.registerStudent(studentId:" ",grade:.year2),[],[.invalidId],true)
        check("invalid id PromoteStudent",empty,.promoteStudent(studentId:"",grade:.year2),[],[.invalidId],true)
        check("invalid id OpenClass",empty,.openClass(classId:"",course:.foundation,capacity:1),[],[.invalidId],true)
        check("invalid id Enroll",empty,.enroll(classId:"a",studentId:""),[],[.invalidId],true)
        check("invalid id Drop",empty,.drop(classId:"",studentId:"s"),[],[.invalidId],true)
        check("invalid id StartClass",empty,.startClass(classId:"",startedOn:"2026-04-01"),[],[.invalidId],true)
        check("invalid id Attend",empty,.attend(classId:"a",studentId:"\t"),[],[.invalidId],true)
        check("invalid id Assess",empty,.assess(classId:"",studentId:"s",score:60),[],[.invalidId],true)
        check("invalid id CompleteClass",empty,.completeClass(classId:""),[],[.invalidId],true)
        check("invalid capacity 0",empty,.openClass(classId:"a",course:.foundation,capacity:0),[],[.invalidCapacity],true)
        check("invalid capacity 11",empty,.openClass(classId:"a",course:.foundation,capacity:11),[],[.invalidCapacity],true)
        check("invalid score -1",empty,.assess(classId:"a",studentId:"s",score:-1),[],[.invalidScore],true)
        check("invalid score 101",empty,.assess(classId:"a",studentId:"s",score:101),[],[.invalidScore],true)
        check("invalid date ",empty,.startClass(classId:"a",startedOn:""),[],[.invalidDate],true)
        check("invalid date 2026-02-29",empty,.startClass(classId:"a",startedOn:"2026-02-29"),[],[.invalidDate],true)
        check("invalid date 2026-04-31",empty,.startClass(classId:"a",startedOn:"2026-04-31"),[],[.invalidDate],true)
        check("invalid date 2026-1-01",empty,.startClass(classId:"a",startedOn:"2026-1-01"),[],[.invalidDate],true)
        check("invalid date 0000-01-01",empty,.startClass(classId:"a",startedOn:"0000-01-01"),[],[.invalidDate],true)
        check("invalid date 2026-13-01",empty,.startClass(classId:"a",startedOn:"2026-13-01"),[],[.invalidDate],true)
        check("historical capacity remains replayable",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:25)],.enroll(classId:"a",studentId:"s"),[.studentEnrolled(classId:"a",studentId:"s",course:.foundation)],[],false)
        check("historical date remains replayable",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"Spring 2020"),.studyStarted(classId:"a",studentId:"s")],.attend(classId:"a",studentId:"s"),[.attendanceRecorded(classId:"a",studentId:"s")],[],false)
        check("historical score remains replayable",[.studentRegistered(studentId:"s",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1),.studentEnrolled(classId:"a",studentId:"s",course:.foundation),.classStarted(classId:"a",startedOn:"2026-04-01"),.studyStarted(classId:"a",studentId:"s"),.attendanceRecorded(classId:"a",studentId:"s"),.resultRecorded(classId:"a",studentId:"s",score:120),.classOpened(classId:"b",course:.advanced,capacity:1)],.enroll(classId:"b",studentId:"s"),[.studentEnrolled(classId:"b",studentId:"s",course:.advanced)],[],false)
        check("new command cannot use historical capacity",empty,.openClass(classId:"a",course:.foundation,capacity:25),[],[.invalidCapacity],true)
        check("new command cannot use historical date",empty,.startClass(classId:"a",startedOn:"Spring 2020"),[],[.invalidDate],true)
        check("new command cannot use historical score",empty,.assess(classId:"a",studentId:"s",score:120),[],[.invalidScore],true)
    let g=GivenHistory([.studentRegistered(studentId:"s",grade:.year3),.studentRegistered(studentId:"t",grade:.year3),.classOpened(classId:"a",course:.foundation,capacity:1)])
    let first=g.executor.prepare("first",Command.enroll(classId:"a",studentId:"s"),SchoolHandler())
    let second=g.executor.prepare("second",Command.enroll(classId:"a",studentId:"t"),SchoolHandler())
    precondition(first.commit().status=="Committed");let conflict=second.commit()
    precondition(conflict.conflicts==1 && conflict.errors==[.classFull] && g.store.read(Query(items:[])).events.count==4,"retry must redecide");checkProjection(g)
    let pure=opened.reduce(Registers(),reduce).decisionModel()
    precondition(encoded(decide(pure,try! ValidatedCommand.create(.enroll(classId:"a",studentId:"s"))))==encoded(Decision.accepted(events:[.studentEnrolled(classId:"a",studentId:"s",course:.foundation)])),"pure Decider enrollment")
    precondition(encoded(decide(pure,try! ValidatedCommand.create(.enroll(classId:"a",studentId:"missing"))))==encoded(Decision.rejected(code:.unknownStudent)),"pure Decider rejection")
    precondition(encoded(decide(pure,try! ValidatedCommand.create(.startClass(classId:"a",startedOn:"2026-04-01"))))==encoded(Decision.accepted(events:[.classStarted(classId:"a",startedOn:"2026-04-01")])),"pure empty-class start")
    precondition((try? StudentId(validating:"s")) != nil && (try? ClassId(validating:" ")) == nil,"ID smart constructors")
    precondition((try? SeatCapacity(validating:25)) == nil && (try? AssessmentScore(validating:120)) == nil && (try? SchoolDate(validating:"Spring 2020")) == nil,"invalid smart constructors")
    precondition(SeatCapacity.fromEvent(25).value==25 && AssessmentScore.fromEvent(120).value==120 && SchoolDate.fromEvent("Spring 2020").value=="Spring 2020","event restoration preserves historical values")
    print("domain: 76 GWT cases + conflict PASS")
}
