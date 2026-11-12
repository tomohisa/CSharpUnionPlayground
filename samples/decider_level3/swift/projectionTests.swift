import Foundation
func projectionTests(_ size:Int){
    let updates=1000,p=SchoolProjection();var position=0
    func envelope(_ n:Int,_ e:Event)->Recorded<Event>{Recorded(position:n,commandId:"scale",eventType:"projection-test",tags:[],payload:e)}
    func send(_ e:Event){position+=1;precondition(p.accept([envelope(position,e)]))}
    var start=Date();send(.classOpened(classId:"a",course:.foundation,capacity:size+updates+1))
    for i in 0..<size{let id="s\(i)";send(.studentRegistered(studentId:id,grade:.year3));send(.studentEnrolled(classId:"a",studentId:id,course:.foundation))}
    let seed=Date().timeIntervalSince(start),(_,old)=p.snapshot(),before=p.checkpoint;var work=p.work
    precondition(!p.accept([envelope(before+1,.studentPromoted(studentId:"s0",grade:.year1)),envelope(before+2,.attendanceRecorded(classId:"a",studentId:"s0"))]))
    precondition(p.checkpoint==before && p.work==work && p.snapshot().1.students.first{$0.studentId=="s0"}!.grade == .year3)
    precondition(!p.accept([envelope(before+1,.studentPromoted(studentId:"s0",grade:.year1)),envelope(before+3,.studentPromoted(studentId:"s0",grade:.year1))]))
    precondition(p.checkpoint==before && p.work==work && p.accept([envelope(1,.classOpened(classId:"ignored",course:.foundation,capacity:1))]) && p.work==work)
    let stableRows=p.rowCount
    for _ in 0..<100{send(.studentDropped(classId:"a",studentId:"s1"));send(.studentEnrolled(classId:"a",studentId:"s1",course:.foundation))}
    let order=members(p.snapshot().1.classes[0]);precondition(p.rowCount==stableRows && order.count==size && order[0]=="s0" && order[1]=="s2" && order.last=="s1")
    start=Date();work=p.work
    for i in size..<size+updates{let id="s\(i)";send(.studentRegistered(studentId:id,grade:.year3));send(.studentEnrolled(classId:"a",studentId:id,course:.foundation))}
    let growth=Date().timeIntervalSince(start),growthWork=p.work-work
    precondition(growthWork==Int64(8*updates) && members(old.classes[0]).count==size && old.students.count==size)
    send(.classStarted(classId:"a",startedOn:"2026-04-01"));send(.studyStarted(classId:"a",studentId:"s0"));start=Date();work=p.work
    for _ in 0..<updates{send(.attendanceRecorded(classId:"a",studentId:"s0"))}
    let update=Date().timeIntervalSince(start),updateWork=p.work-work;precondition(updateWork==Int64(3*updates))
    start=Date();let(_,snapshot)=p.snapshot(),query=Date().timeIntervalSince(start)
    precondition(snapshot.students.count==size+updates && members(snapshot.classes[0]).count==size+updates)
    guard case .studying(let count)=snapshot.enrollments.first(where:{$0.studentId=="s0"})!.progress else{preconditionFailure("not studying")};precondition(count==updates)
    let report:[String:Any] = ["size":size,"updates":updates,"rows":p.rowCount,"seedSeconds":seed,"updateSeconds":update,"growthSeconds":growth,"snapshotSeconds":query,"updateWork":updateWork,"growthWork":growthWork]
    print(String(decoding:try! JSONSerialization.data(withJSONObject:report,options:[.sortedKeys]),as:UTF8.self))
}
