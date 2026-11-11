import Foundation

enum Grade: String, Codable, Sendable { case year1 = "Year1"; case year2 = "Year2"; case year3 = "Year3" }
enum Course: String, Codable, Sendable { case foundation = "Foundation"; case advanced = "Advanced" }
enum FailureReason: String, Codable, Sendable { case belowPassMark = "BelowPassMark" }
enum WithdrawalReason: String, Codable, Sendable { case studentRequest = "StudentRequest" }
enum Rejection: String, Codable, Sendable { case invalidId = "InvalidId"; case invalidCapacity = "InvalidCapacity"; case invalidScore = "InvalidScore"; case invalidDate = "InvalidDate"; case duplicateStudent = "DuplicateStudent"; case unknownStudent = "UnknownStudent"; case invalidPromotion = "InvalidPromotion"; case duplicateClass = "DuplicateClass"; case unknownClass = "UnknownClass"; case gradeTooLow = "GradeTooLow"; case prerequisiteMissing = "PrerequisiteMissing"; case alreadyEnrolled = "AlreadyEnrolled"; case classFull = "ClassFull"; case registrationClosed = "RegistrationClosed"; case notEnrolled = "NotEnrolled"; case notTeaching = "NotTeaching"; case noAttendance = "NoAttendance"; case alreadyAssessed = "AlreadyAssessed"; case unassessedStudents = "UnassessedStudents"; case invalidHistory = "InvalidHistory" }
struct Student: Codable, Sendable {
    let studentId: String
    let grade: Grade
}
struct Plan: Codable, Sendable {
    let classId: String
    let course: Course
    let minimumGrade: Grade
    let prerequisite: Prerequisite
    let capacity: Int
}
struct Enrollment: Codable, Sendable {
    let classId: String
    let studentId: String
    let course: Course
    let progress: Progress
}
struct ClassResult: Codable, Sendable {
    let studentId: String
    let progress: Progress
}
struct DecisionModel: Codable, Sendable {
    let students: [Student]
    let classes: [ClassState]
    let enrollments: [Enrollment]
}
struct SchoolList: Codable, Sendable {
    let students: [Student]
    let classes: [ClassState]
    let enrollments: [Enrollment]
    let summaries: [String]
}
struct Trace: Codable, Sendable {
    let operation: String
    let status: String
    let conflicts: Int
    let appended: Int
    let position: Int
    let errors: [Rejection]
}
enum Prerequisite: Sendable {
    case noPrerequisite
    case passedCourse(course: Course)
}
enum Progress: Sendable {
    case reserved
    case studying(attendance: Int)
    case passed(score: Int, credits: Int)
    case failed(score: Int, reason: FailureReason)
    case withdrawn(reason: WithdrawalReason)
}
enum ClassState: Sendable {
    case available(plan: Plan, students: [String])
    case full(plan: Plan, students: [String])
    case teaching(plan: Plan, students: [String], startedOn: String)
    case completed(plan: Plan, results: [ClassResult])
}
enum Command: Sendable {
    case registerStudent(studentId: String, grade: Grade)
    case promoteStudent(studentId: String, grade: Grade)
    case openClass(classId: String, course: Course, capacity: Int)
    case enroll(classId: String, studentId: String)
    case drop(classId: String, studentId: String)
    case startClass(classId: String, startedOn: String)
    case attend(classId: String, studentId: String)
    case assess(classId: String, studentId: String, score: Int)
    case completeClass(classId: String)
}
enum Event: Sendable {
    case studentRegistered(studentId: String, grade: Grade)
    case studentPromoted(studentId: String, grade: Grade)
    case classOpened(classId: String, course: Course, capacity: Int)
    case studentEnrolled(classId: String, studentId: String, course: Course)
    case studentDropped(classId: String, studentId: String)
    case classStarted(classId: String, startedOn: String)
    case attendanceRecorded(classId: String, studentId: String)
    case resultRecorded(classId: String, studentId: String, score: Int)
    case classCompleted(classId: String, results: [ClassResult])
    case studyStarted(classId: String, studentId: String)
}
enum Decision: Sendable {
    case accepted(events: [Event])
    case rejected(code: Rejection)
}
enum Operation: Sendable {
    case execute(commandId: String, command: Command)
    case prepare(key: String, commandId: String, command: Command)
    case commit(key: String)
    case listAll
    case rebuild
    case redeliver
}
