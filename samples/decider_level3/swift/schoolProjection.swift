import Foundation
private struct IndexKey:Hashable,Sendable{let kind:String;let a:String;let b:String;let node:Int;init(_ kind:String,_ a:String="",_ b:String="",node:Int=0){self.kind=kind;self.a=a;self.b=b;self.node=node}}
private struct Room:Sendable{var phase:ClassState;var first=0;var last=0;var count=0}
private struct Member:Sendable{let student:String;var previous:Int;var next:Int}
private enum IndexRow:Sendable{case student(Student);case room(Room);case enrollment(Enrollment);case node(Member);case member(Int)}
private func nodeKey(_ n:Int)->IndexKey{IndexKey("node",node:n)}
private func getNode(_ tx:IndexBatch<IndexKey,IndexRow>,_ n:Int)throws->Member{guard case .node(let m)=try tx.get(nodeKey(n))else{throw IndexFailure.invalid};return m}
private func applyIndex(_ tx:IndexBatch<IndexKey,IndexRow>,_ recorded:Recorded<Event>)throws{
    let e=recorded.payload
    switch e{
    case .studentRegistered(let id,let grade),.studentPromoted(let id,let grade):tx.put(IndexKey("student",id),.student(Student(studentId:id,grade:grade)));return
    case .classOpened(let id,_,_):tx.put(IndexKey("class",id),.room(Room(phase:reduce(Registers(),e).classes[id]!)));return
    default:break
    }
    let id:String
    switch e{case .studentEnrolled(let c,_,_),.studentDropped(let c,_),.classStarted(let c,_),.classCompleted(let c,_),.studyStarted(let c,_),.attendanceRecorded(let c,_),.resultRecorded(let c,_,_):id=c;default:throw IndexFailure.invalid}
    let ck=IndexKey("class",id)
    if case .room(var c)=tx.find(ck){let plan=planOf(c.phase)
        switch e{
        case .studentEnrolled(_,let student,_):
            let n=recorded.position;if c.last != 0{var m=try getNode(tx,c.last);m.next=n;tx.put(nodeKey(c.last),.node(m))}
            tx.put(nodeKey(n),.node(Member(student:student,previous:c.last,next:0)));tx.put(IndexKey("member",id,student),.member(n))
            if c.first==0{c.first=n};c.last=n;c.count+=1;c.phase=c.count==plan.capacity ? .full(plan:plan,students:[]):.available(plan:plan,students:[]);tx.put(ck,.room(c))
        case .studentDropped(_,let student):
            if case .member(let n)=tx.find(IndexKey("member",id,student)){let m=try getNode(tx,n)
                if m.previous != 0{var p=try getNode(tx,m.previous);p.next=m.next;tx.put(nodeKey(m.previous),.node(p))}
                if m.next != 0{var p=try getNode(tx,m.next);p.previous=m.previous;tx.put(nodeKey(m.next),.node(p))}
                tx.delete(nodeKey(n));tx.delete(IndexKey("member",id,student));if c.first==n{c.first=m.next};if c.last==n{c.last=m.previous};c.count-=1
            };c.phase = .available(plan:plan,students:[]);tx.put(ck,.room(c))
        case .classStarted(_,let date):c.phase = .teaching(plan:plan,students:[],startedOn:SchoolDate.fromEvent(date).value);tx.put(ck,.room(c))
        case .classCompleted(_,let results):c.phase = .completed(plan:plan,results:results);tx.put(ck,.room(c))
        default:break
        }
    }
    let student:String
    switch e{case .studentEnrolled(_,let s,_),.studentDropped(_,let s),.studyStarted(_,let s),.attendanceRecorded(_,let s),.resultRecorded(_,let s,_):student=s;default:return}
    let ek=IndexKey("enrollment",id,student)
    if case .studentEnrolled(_,_,let course)=e{tx.put(ek,.enrollment(Enrollment(classId:id,studentId:student,course:course,progress:.reserved)));return}
    guard case .enrollment(let value)=tx.find(ek)else{return}
    let progress:Progress
    switch e{
    case .studentDropped:progress = .withdrawn(reason:.studentRequest)
    case .studyStarted:progress = .studying(attendance:0)
    case .attendanceRecorded:guard case .studying(let n)=value.progress else{throw IndexFailure.invalid};progress = .studying(attendance:n+1)
    case .resultRecorded(_,_,let score):progress=score>=60 ? .passed(score:AssessmentScore.fromEvent(score).value,credits:value.course == .foundation ? 1:2):.failed(score:score,reason:.belowPassMark)
    default:progress=value.progress
    }
    tx.put(ek,.enrollment(Enrollment(classId:id,studentId:student,course:value.course,progress:progress)))
}
final class SchoolProjection:@unchecked Sendable{
    private let index=IndexedProjection(applyIndex)
    var checkpoint:Int{index.checkpoint};var rowCount:Int{index.rowCount};var work:Int64{index.work}
    func accept(_ events:[Recorded<Event>])->Bool{index.accept(events)}
    func snapshot()->(Int,SchoolList){index.read{rows in
        var students:[Student]=[],classes:[ClassState]=[],enrollments:[Enrollment]=[]
        for row in rows.values{switch row{case .student(let value):students.append(value);case .enrollment(let value):enrollments.append(value)
        case .room(let c):
            var ids:[String]=[]
            if case .completed=c.phase{}else{var n=c.first;while n != 0{guard case .node(let m)=rows[nodeKey(n)]else{preconditionFailure("missing node")};ids.append(m.student);n=m.next}}
            switch c.phase{case .available(let p,_):classes.append(.available(plan:p,students:ids));case .full(let p,_):classes.append(.full(plan:p,students:ids));case .teaching(let p,_,let d):classes.append(.teaching(plan:p,students:ids,startedOn:d));case .completed:classes.append(c.phase)}
        default:break}}
        students.sort{idBefore($0.studentId,$1.studentId)};classes.sort{idBefore(planOf($0).classId,planOf($1).classId)};enrollments.sort{$0.classId == $1.classId ? idBefore($0.studentId,$1.studentId):idBefore($0.classId,$1.classId)}
        return SchoolList(students:students,classes:classes,enrollments:enrollments,summaries:enrollments.map{describe($0.progress)})
    }}
}
