import Foundation
func idBefore(_ a:String,_ b:String)->Bool{a.utf8.lexicographicallyPrecedes(b.utf8)}
struct EnrollmentKey: Hashable, Sendable { let room:String;let student:String }
struct Registers: Sendable {
    var students:[String:Student]=[:]
    var classes:[String:ClassState]=[:]
    var enrollments:[EnrollmentKey:Enrollment]=[:]
    func decisionModel()->DecisionModel {
        DecisionModel(students:students.values.sorted{idBefore($0.studentId,$1.studentId)},classes:classes.values.sorted{idBefore(planOf($0).classId,planOf($1).classId)},enrollments:enrollments.values.sorted{$0.classId == $1.classId ? idBefore($0.studentId,$1.studentId) : idBefore($0.classId,$1.classId)})
    }
    func list()->SchoolList {let m=decisionModel();return SchoolList(students:m.students,classes:m.classes,enrollments:m.enrollments,summaries:m.enrollments.map{describe($0.progress)})}
}
func applyStudent(_ s:inout Registers,_ e:Event){
    switch e {case .studentRegistered(let id,let grade),.studentPromoted(let id,let grade):s.students[id]=Student(studentId:id,grade:grade);default:break}
}
func applyClass(_ s:inout Registers,_ e:Event){
    if case .classOpened(let id,let course,let capacity)=e {
        let plan=Plan(classId:id,course:course,minimumGrade:course == .foundation ? .year2 : .year3,prerequisite:course == .foundation ? .noPrerequisite : .passedCourse(course:.foundation),capacity:SeatCapacity.fromEvent(capacity).value)
        s.classes[id] = .available(plan:plan,students:[]);return
    }
    let id:String
    switch e {case .studentEnrolled(let c,_,_),.studentDropped(let c,_),.classStarted(let c,_),.classCompleted(let c,_):id=c;default:return}
    guard let room=s.classes[id] else{return}
    let p=planOf(room)
    switch e {
    case .studentEnrolled(_,let student,_):let students=members(room)+[student];s.classes[id]=students.count == p.capacity ? .full(plan:p,students:students) : .available(plan:p,students:students)
    case .studentDropped(_,let student):s.classes[id] = .available(plan:p,students:members(room).filter{$0 != student})
    case .classStarted(_,let date):s.classes[id] = .teaching(plan:p,students:members(room),startedOn:SchoolDate.fromEvent(date).value)
    case .classCompleted(_,let results):s.classes[id] = .completed(plan:p,results:results)
    default:preconditionFailure("invariant")
    }
}
func applyEnrollment(_ s:inout Registers,_ e:Event){
    if case .studentEnrolled(let id,let student,let course)=e{s.enrollments[EnrollmentKey(room:id,student:student)]=Enrollment(classId:id,studentId:student,course:course,progress:.reserved);return}
    let key:EnrollmentKey
    switch e {case .studentDropped(let c,let s),.studyStarted(let c,let s),.attendanceRecorded(let c,let s),.resultRecorded(let c,let s,_):key=EnrollmentKey(room:c,student:s);default:return}
    guard let r=s.enrollments[key] else{return}
    let progress:Progress
    switch e {
    case .studentDropped:progress = .withdrawn(reason:.studentRequest)
    case .studyStarted:progress = .studying(attendance:0)
    case .attendanceRecorded:guard case .studying(let n)=r.progress else{preconditionFailure("invalid committed event")};progress = .studying(attendance:n+1)
    case .resultRecorded(_,_,let score):progress=score>=60 ? .passed(score:AssessmentScore.fromEvent(score).value,credits:r.course == .foundation ? 1 : 2) : .failed(score:AssessmentScore.fromEvent(score).value,reason:.belowPassMark)
    default:preconditionFailure("invariant")
    }
    s.enrollments[key]=Enrollment(classId:r.classId,studentId:r.studentId,course:r.course,progress:progress)
}
func reduce(_ state:Registers,_ event:Event)->Registers{var s=state;applyStudent(&s,event);applyClass(&s,event);applyEnrollment(&s,event);return s}
func fold(_ events:[Recorded<Event>])->Registers{events.reduce(Registers()){reduce($0,$1.payload)}}
