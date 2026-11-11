import Foundation

enum WireError: Error { case unknownTag(String), invalidHistory }

extension Prerequisite: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case course
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "NoPrerequisite": self = .noPrerequisite
        case "PassedCourse": self = .passedCourse(course: try c.decode(Course.self, forKey: .course))
        default: throw WireError.unknownTag(tag)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        switch self {
        case .noPrerequisite:
            try c.encode("NoPrerequisite", forKey: .tag)
        case .passedCourse(let course):
            try c.encode("PassedCourse", forKey: .tag)
            try c.encode(course, forKey: .course)
        }
    }
}

extension Progress: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case attendance
        case credits
        case reason
        case score
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "Reserved": self = .reserved
        case "Studying": self = .studying(attendance: try c.decode(Int.self, forKey: .attendance))
        case "Passed": self = .passed(score: try c.decode(Int.self, forKey: .score), credits: try c.decode(Int.self, forKey: .credits))
        case "Failed": self = .failed(score: try c.decode(Int.self, forKey: .score), reason: try c.decode(FailureReason.self, forKey: .reason))
        case "Withdrawn": self = .withdrawn(reason: try c.decode(WithdrawalReason.self, forKey: .reason))
        default: throw WireError.unknownTag(tag)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        switch self {
        case .reserved:
            try c.encode("Reserved", forKey: .tag)
        case .studying(let attendance):
            try c.encode("Studying", forKey: .tag)
            try c.encode(attendance, forKey: .attendance)
        case .passed(let score, let credits):
            try c.encode("Passed", forKey: .tag)
            try c.encode(score, forKey: .score)
            try c.encode(credits, forKey: .credits)
        case .failed(let score, let reason):
            try c.encode("Failed", forKey: .tag)
            try c.encode(score, forKey: .score)
            try c.encode(reason, forKey: .reason)
        case .withdrawn(let reason):
            try c.encode("Withdrawn", forKey: .tag)
            try c.encode(reason, forKey: .reason)
        }
    }
}

extension ClassState: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case plan
        case results
        case startedOn
        case students
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "Available": self = .available(plan: try c.decode(Plan.self, forKey: .plan), students: try c.decode([String].self, forKey: .students))
        case "Full": self = .full(plan: try c.decode(Plan.self, forKey: .plan), students: try c.decode([String].self, forKey: .students))
        case "Teaching": self = .teaching(plan: try c.decode(Plan.self, forKey: .plan), students: try c.decode([String].self, forKey: .students), startedOn: try c.decode(String.self, forKey: .startedOn))
        case "Completed": self = .completed(plan: try c.decode(Plan.self, forKey: .plan), results: try c.decode([ClassResult].self, forKey: .results))
        default: throw WireError.unknownTag(tag)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        switch self {
        case .available(let plan, let students):
            try c.encode("Available", forKey: .tag)
            try c.encode(plan, forKey: .plan)
            try c.encode(students, forKey: .students)
        case .full(let plan, let students):
            try c.encode("Full", forKey: .tag)
            try c.encode(plan, forKey: .plan)
            try c.encode(students, forKey: .students)
        case .teaching(let plan, let students, let startedOn):
            try c.encode("Teaching", forKey: .tag)
            try c.encode(plan, forKey: .plan)
            try c.encode(students, forKey: .students)
            try c.encode(startedOn, forKey: .startedOn)
        case .completed(let plan, let results):
            try c.encode("Completed", forKey: .tag)
            try c.encode(plan, forKey: .plan)
            try c.encode(results, forKey: .results)
        }
    }
}

extension Command: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case capacity
        case classId
        case course
        case grade
        case score
        case startedOn
        case studentId
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "RegisterStudent": self = .registerStudent(studentId: try c.decode(String.self, forKey: .studentId), grade: try c.decode(Grade.self, forKey: .grade))
        case "PromoteStudent": self = .promoteStudent(studentId: try c.decode(String.self, forKey: .studentId), grade: try c.decode(Grade.self, forKey: .grade))
        case "OpenClass": self = .openClass(classId: try c.decode(String.self, forKey: .classId), course: try c.decode(Course.self, forKey: .course), capacity: try c.decode(Int.self, forKey: .capacity))
        case "Enroll": self = .enroll(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId))
        case "Drop": self = .drop(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId))
        case "StartClass": self = .startClass(classId: try c.decode(String.self, forKey: .classId), startedOn: try c.decode(String.self, forKey: .startedOn))
        case "Attend": self = .attend(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId))
        case "Assess": self = .assess(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId), score: try c.decode(Int.self, forKey: .score))
        case "CompleteClass": self = .completeClass(classId: try c.decode(String.self, forKey: .classId))
        default: throw WireError.unknownTag(tag)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        switch self {
        case .registerStudent(let studentId, let grade):
            try c.encode("RegisterStudent", forKey: .tag)
            try c.encode(studentId, forKey: .studentId)
            try c.encode(grade, forKey: .grade)
        case .promoteStudent(let studentId, let grade):
            try c.encode("PromoteStudent", forKey: .tag)
            try c.encode(studentId, forKey: .studentId)
            try c.encode(grade, forKey: .grade)
        case .openClass(let classId, let course, let capacity):
            try c.encode("OpenClass", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(course, forKey: .course)
            try c.encode(capacity, forKey: .capacity)
        case .enroll(let classId, let studentId):
            try c.encode("Enroll", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
        case .drop(let classId, let studentId):
            try c.encode("Drop", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
        case .startClass(let classId, let startedOn):
            try c.encode("StartClass", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(startedOn, forKey: .startedOn)
        case .attend(let classId, let studentId):
            try c.encode("Attend", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
        case .assess(let classId, let studentId, let score):
            try c.encode("Assess", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
            try c.encode(score, forKey: .score)
        case .completeClass(let classId):
            try c.encode("CompleteClass", forKey: .tag)
            try c.encode(classId, forKey: .classId)
        }
    }
}

extension Event: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case capacity
        case classId
        case course
        case grade
        case results
        case score
        case startedOn
        case studentId
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "StudentRegistered": self = .studentRegistered(studentId: try c.decode(String.self, forKey: .studentId), grade: try c.decode(Grade.self, forKey: .grade))
        case "StudentPromoted": self = .studentPromoted(studentId: try c.decode(String.self, forKey: .studentId), grade: try c.decode(Grade.self, forKey: .grade))
        case "ClassOpened": self = .classOpened(classId: try c.decode(String.self, forKey: .classId), course: try c.decode(Course.self, forKey: .course), capacity: try c.decode(Int.self, forKey: .capacity))
        case "StudentEnrolled": self = .studentEnrolled(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId), course: try c.decode(Course.self, forKey: .course))
        case "StudentDropped": self = .studentDropped(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId))
        case "ClassStarted": self = .classStarted(classId: try c.decode(String.self, forKey: .classId), startedOn: try c.decode(String.self, forKey: .startedOn))
        case "AttendanceRecorded": self = .attendanceRecorded(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId))
        case "ResultRecorded": self = .resultRecorded(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId), score: try c.decode(Int.self, forKey: .score))
        case "ClassCompleted": self = .classCompleted(classId: try c.decode(String.self, forKey: .classId), results: try c.decode([ClassResult].self, forKey: .results))
        case "StudyStarted": self = .studyStarted(classId: try c.decode(String.self, forKey: .classId), studentId: try c.decode(String.self, forKey: .studentId))
        default: throw WireError.unknownTag(tag)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        switch self {
        case .studentRegistered(let studentId, let grade):
            try c.encode("StudentRegistered", forKey: .tag)
            try c.encode(studentId, forKey: .studentId)
            try c.encode(grade, forKey: .grade)
        case .studentPromoted(let studentId, let grade):
            try c.encode("StudentPromoted", forKey: .tag)
            try c.encode(studentId, forKey: .studentId)
            try c.encode(grade, forKey: .grade)
        case .classOpened(let classId, let course, let capacity):
            try c.encode("ClassOpened", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(course, forKey: .course)
            try c.encode(capacity, forKey: .capacity)
        case .studentEnrolled(let classId, let studentId, let course):
            try c.encode("StudentEnrolled", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
            try c.encode(course, forKey: .course)
        case .studentDropped(let classId, let studentId):
            try c.encode("StudentDropped", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
        case .classStarted(let classId, let startedOn):
            try c.encode("ClassStarted", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(startedOn, forKey: .startedOn)
        case .attendanceRecorded(let classId, let studentId):
            try c.encode("AttendanceRecorded", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
        case .resultRecorded(let classId, let studentId, let score):
            try c.encode("ResultRecorded", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
            try c.encode(score, forKey: .score)
        case .classCompleted(let classId, let results):
            try c.encode("ClassCompleted", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(results, forKey: .results)
        case .studyStarted(let classId, let studentId):
            try c.encode("StudyStarted", forKey: .tag)
            try c.encode(classId, forKey: .classId)
            try c.encode(studentId, forKey: .studentId)
        }
    }
}

extension Decision: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case code
        case events
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "Accepted": self = .accepted(events: try c.decode([Event].self, forKey: .events))
        case "Rejected": self = .rejected(code: try c.decode(Rejection.self, forKey: .code))
        default: throw WireError.unknownTag(tag)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        switch self {
        case .accepted(let events):
            try c.encode("Accepted", forKey: .tag)
            try c.encode(events, forKey: .events)
        case .rejected(let code):
            try c.encode("Rejected", forKey: .tag)
            try c.encode(code, forKey: .code)
        }
    }
}

extension Operation: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case command
        case commandId
        case key
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "Execute": self = .execute(commandId: try c.decode(String.self, forKey: .commandId), command: try c.decode(Command.self, forKey: .command))
        case "Prepare": self = .prepare(key: try c.decode(String.self, forKey: .key), commandId: try c.decode(String.self, forKey: .commandId), command: try c.decode(Command.self, forKey: .command))
        case "Commit": self = .commit(key: try c.decode(String.self, forKey: .key))
        case "ListAll": self = .listAll
        case "Rebuild": self = .rebuild
        case "Redeliver": self = .redeliver
        default: throw WireError.unknownTag(tag)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        switch self {
        case .execute(let commandId, let command):
            try c.encode("Execute", forKey: .tag)
            try c.encode(commandId, forKey: .commandId)
            try c.encode(command, forKey: .command)
        case .prepare(let key, let commandId, let command):
            try c.encode("Prepare", forKey: .tag)
            try c.encode(key, forKey: .key)
            try c.encode(commandId, forKey: .commandId)
            try c.encode(command, forKey: .command)
        case .commit(let key):
            try c.encode("Commit", forKey: .tag)
            try c.encode(key, forKey: .key)
        case .listAll:
            try c.encode("ListAll", forKey: .tag)
        case .rebuild:
            try c.encode("Rebuild", forKey: .tag)
        case .redeliver:
            try c.encode("Redeliver", forKey: .tag)
        }
    }
}
