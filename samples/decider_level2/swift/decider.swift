import Foundation

func empty() -> State { State(students: [], classes: [], enrollments: []) }
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
func decide(_ state: State, _ command: Command) -> Decision {
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
func decideClass(_ state: State, _ command: Command, _ id: String) -> Decision {
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
        return accept(.studentEnrolled(classId: id, studentId: studentId))
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
        return state.enrollments.contains { $0.classId == id && studying($0.progress) } ? reject(.unassessedStudents) : accept(.classCompleted(classId: id))
    case .registerStudent, .promoteStudent, .openClass: preconditionFailure("invariant")
    }
}
func study(_ state: State, _ room: ClassState, _ id: String, _ student: String, _ command: Command) -> Decision {
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
func cause(_ event: Event) -> Command {
    switch event {
    case .studentRegistered(let id, let g): return .registerStudent(studentId: id, grade: g)
    case .studentPromoted(let id, let g): return .promoteStudent(studentId: id, grade: g)
    case .classOpened(let id, let c, let n): return .openClass(classId: id, course: c, capacity: n)
    case .studentEnrolled(let id, let s): return .enroll(classId: id, studentId: s)
    case .studentDropped(let id, let s): return .drop(classId: id, studentId: s)
    case .classStarted(let id, let d): return .startClass(classId: id, startedOn: d)
    case .attendanceRecorded(let id, let s): return .attend(classId: id, studentId: s)
    case .resultRecorded(let id, let s, let n): return .assess(classId: id, studentId: s, score: n)
    case .classCompleted(let id): return .completeClass(classId: id)
    }
}
func evolve(_ state: State, _ event: Event) throws -> State {
    if case .rejected = decide(state, cause(event)) { throw WireError.invalidHistory }
    switch event {
    case .studentRegistered(let id, let grade):
        return State(students: state.students + [Student(studentId: id, grade: grade)], classes: state.classes, enrollments: state.enrollments)
    case .studentPromoted(let id, let grade):
        return State(students: state.students.map { $0.studentId == id ? Student(studentId: id, grade: grade) : $0 }, classes: state.classes, enrollments: state.enrollments)
    case .classOpened(let id, let course, let capacity):
        let plan = Plan(classId: id, course: course, minimumGrade: course == .foundation ? .year2 : .year3,
                        prerequisite: course == .foundation ? .noPrerequisite : .passedCourse(course: .foundation), capacity: capacity)
        return State(students: state.students, classes: state.classes + [.available(plan: plan, students: [])], enrollments: state.enrollments)
    case .studentEnrolled(let id, _), .studentDropped(let id, _), .classStarted(let id, _), .attendanceRecorded(let id, _), .resultRecorded(let id, _, _), .classCompleted(let id):
        return applyClass(state, event, id)
    }
}
func applyClass(_ state: State, _ event: Event, _ id: String) -> State {
    guard let room = state.classes.first(where: { planOf($0).classId == id }) else { preconditionFailure("invariant") }
    let plan = planOf(room)
    var updated = room, records = state.enrollments
    func change(_ student: String, _ progress: (Progress) -> Progress) -> [Enrollment] {
        records.map { e in e.classId == id && e.studentId == student ? Enrollment(classId: id, studentId: student, course: e.course, progress: progress(e.progress)) : e }
    }
    switch event {
    case .studentEnrolled(_, let student):
        let students = members(room) + [student]
        updated = students.count == plan.capacity ? .full(plan: plan, students: students) : .available(plan: plan, students: students)
        records = records.filter { $0.classId != id || $0.studentId != student } + [Enrollment(classId: id, studentId: student, course: plan.course, progress: .reserved)]
    case .studentDropped(_, let student):
        updated = .available(plan: plan, students: members(room).filter { $0 != student })
        records = change(student) { _ in .withdrawn(reason: .studentRequest) }
    case .classStarted(_, let date):
        updated = .teaching(plan: plan, students: members(room), startedOn: date)
        records = records.map { e in
            if e.classId == id, case .reserved = e.progress { return Enrollment(classId: id, studentId: e.studentId, course: e.course, progress: .studying(attendance: 0)) }
            return e
        }
    case .attendanceRecorded(_, let student):
        records = change(student) { progress in
            guard case .studying(let count) = progress else { preconditionFailure("invariant") }
            return .studying(attendance: count + 1)
        }
    case .resultRecorded(_, let student, let score):
        records = change(student) { _ in score >= 60 ? .passed(score: score, credits: plan.course == .foundation ? 1 : 2) : .failed(score: score, reason: .belowPassMark) }
    case .classCompleted:
        let results = records.filter { e in
            guard e.classId == id else { return false }
            switch e.progress { case .passed, .failed: return true; case .reserved, .studying, .withdrawn: return false }
        }.map { ClassResult(studentId: $0.studentId, progress: $0.progress) }
        updated = .completed(plan: plan, results: results)
    case .studentRegistered, .studentPromoted, .classOpened: preconditionFailure("invariant")
    }
    return State(students: state.students, classes: state.classes.map { planOf($0).classId == id ? updated : $0 }, enrollments: records)
}
func replay(_ events: [Event]) throws -> State { try events.reduce(empty()) { try evolve($0, $1) } }

func describe(_ progress: Progress) -> String {
    switch progress {
    case .reserved: return "reserved"
    case .studying(let attendance): return "studying:\(attendance)"
    case .passed(let score, let credits): return "passed:\(score):\(credits)"
    case .failed(let score, let reason): return "failed:\(score):\(reason.rawValue)"
    case .withdrawn(let reason): return "withdrawn:\(reason.rawValue)"
    }
}
