import Foundation

func empty() -> DecisionModel { DecisionModel(students: [], classes: [], enrollments: []) }
func planOf(_ room: ClassState) -> Plan {
    switch room {
    case .available(let plan, _), .full(let plan, _), .teaching(let plan, _, _), .completed(let plan, _): return plan
    }
}
func members(_ room: ClassState) -> [String] {
    switch room {
    case .available(_, let students), .full(_, let students), .teaching(_, let students, _): return students
    case .completed(_, let results): return results.map(\.studentId)
    }
}
func rank(_ grade: Grade) -> Int { switch grade { case .year1: return 1; case .year2: return 2; case .year3: return 3 } }
func recruiting(_ room: ClassState) -> Bool { switch room { case .available, .full: return true; case .teaching, .completed: return false } }
func passed(_ progress: Progress) -> Bool { switch progress { case .passed: return true; case .reserved, .studying, .failed, .withdrawn: return false } }
func studying(_ progress: Progress) -> Bool { switch progress { case .studying: return true; case .reserved, .passed, .failed, .withdrawn: return false } }
func accept(_ event: Event) -> Decision { .accepted(events: [event]) }
func reject(_ code: Rejection) -> Decision { .rejected(code: code) }
func decideOne(_ state: DecisionModel, _ command: Command) -> Decision {
    switch command {
    case .registerStudent(let id, let grade):
        if id.isEmpty { return reject(.invalidId) }
        return state.students.contains(where: { $0.studentId == id }) ? reject(.duplicateStudent) : accept(.studentRegistered(studentId: id, grade: grade))
    case .promoteStudent(let id, let grade):
        guard let student = state.students.first(where: { $0.studentId == id }) else { return reject(.unknownStudent) }
        return rank(grade) == rank(student.grade) + 1 ? accept(.studentPromoted(studentId: id, grade: grade)) : reject(.invalidPromotion)
    case .openClass(let id, let course, let capacity):
        if id.isEmpty { return reject(.invalidId) }
        if !(1...10).contains(capacity) { return reject(.invalidCapacity) }
        return state.classes.contains(where: { planOf($0).classId == id }) ? reject(.duplicateClass) : accept(.classOpened(classId: id, course: course, capacity: capacity))
    case .enroll(let id, _), .drop(let id, _), .startClass(let id, _), .attend(let id, _), .assess(let id, _, _), .completeClass(let id):
        return decideClass(state, command, id)
    }
}
func decideClass(_ state: DecisionModel, _ command: Command, _ id: String) -> Decision {
    guard let room = state.classes.first(where: { planOf($0).classId == id }) else { return reject(.unknownClass) }
    let plan = planOf(room)
    switch command {
    case .enroll(_, let studentId):
        guard let student = state.students.first(where: { $0.studentId == studentId }) else { return reject(.unknownStudent) }
        if !recruiting(room) { return reject(.registrationClosed) }
        if members(room).contains(studentId) { return reject(.alreadyEnrolled) }
        if rank(student.grade) < rank(plan.minimumGrade) { return reject(.gradeTooLow) }
        let eligible: Bool
        switch plan.prerequisite {
        case .noPrerequisite: eligible = true
        case .passedCourse(let course): eligible = state.enrollments.contains { $0.studentId == studentId && $0.course == course && passed($0.progress) }
        }
        if !eligible { return reject(.prerequisiteMissing) }
        if case .full = room { return reject(.classFull) }
        return accept(.studentEnrolled(classId: id, studentId: studentId, course: plan.course))
    case .drop(_, let studentId):
        if !recruiting(room) { return reject(.registrationClosed) }
        return members(room).contains(studentId) ? accept(.studentDropped(classId: id, studentId: studentId)) : reject(.notEnrolled)
    case .startClass(_, let startedOn):
        if !recruiting(room) { return reject(.registrationClosed) }
        return startedOn.isEmpty ? reject(.invalidDate) : accept(.classStarted(classId: id, startedOn: startedOn))
    case .attend(_, let studentId): return study(state, room, id, studentId, command)
    case .assess(_, let studentId, _): return study(state, room, id, studentId, command)
    case .completeClass:
        guard case .teaching = room else { return reject(.notTeaching) }
        return state.enrollments.contains { $0.classId == id && studying($0.progress) } ? reject(.unassessedStudents) : accept(.classCompleted(classId: id, results: state.enrollments.filter { $0.classId == id && (passed($0.progress) || failed($0.progress)) }.map { ClassResult(studentId: $0.studentId, progress: $0.progress) }))
    case .registerStudent, .promoteStudent, .openClass: preconditionFailure("invariant")
    }
}
func study(_ state: DecisionModel, _ room: ClassState, _ id: String, _ student: String, _ command: Command) -> Decision {
    guard case .teaching = room else { return reject(.notTeaching) }
    if !members(room).contains(student) { return reject(.notEnrolled) }
    guard let record = state.enrollments.first(where: { $0.classId == id && $0.studentId == student }) else { preconditionFailure("invariant") }
    guard case .studying(let attendance) = record.progress else { return reject(.alreadyAssessed) }
    switch command {
    case .attend: return accept(.attendanceRecorded(classId: id, studentId: student))
    case .assess(_, _, let score):
        if !(0...100).contains(score) { return reject(.invalidScore) }
        return attendance == 0 ? reject(.noAttendance) : accept(.resultRecorded(classId: id, studentId: student, score: score))
    case .registerStudent, .promoteStudent, .openClass, .enroll, .drop, .startClass, .completeClass: preconditionFailure("invariant")
    }
}
func describe(_ progress: Progress) -> String {
    switch progress {
    case .reserved: return "reserved"
    case .studying(let attendance): return "studying:\(attendance)"
    case .passed(let score, let credits): return "passed:\(score):\(credits)"
    case .failed(let score, let reason): return "failed:\(score):\(reason.rawValue)"
    case .withdrawn(let reason): return "withdrawn:\(reason.rawValue)"
    }
}
func failed(_ p: Progress) -> Bool { if case .failed = p { return true }; return false }
func decide(_ state: DecisionModel, _ validated: ValidatedCommand) -> Decision {
    let command=validated.value
    let decision = decideOne(state, command)
    if case .accepted(let events) = decision, case .classStarted(let id, _) = events[0] {
        let starts = state.enrollments.filter { e in if e.classId == id, case .reserved = e.progress { return true }; return false }.map { Event.studyStarted(classId: id, studentId: $0.studentId) }
        return .accepted(events: events + starts)
    }
    return decision
}
