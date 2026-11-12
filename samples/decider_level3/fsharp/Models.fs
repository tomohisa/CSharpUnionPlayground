namespace SchoolDomain

open System.Text.Json.Serialization

type Grade =
    | [<JsonPropertyName("Year1")>] Year1
    | [<JsonPropertyName("Year2")>] Year2
    | [<JsonPropertyName("Year3")>] Year3

type Course =
    | [<JsonPropertyName("Foundation")>] Foundation
    | [<JsonPropertyName("Advanced")>] Advanced

type FailureReason =
    | [<JsonPropertyName("BelowPassMark")>] BelowPassMark

type WithdrawalReason =
    | [<JsonPropertyName("StudentRequest")>] StudentRequest

type Rejection =
    | [<JsonPropertyName("InvalidCommand")>] InvalidCommand
    | [<JsonPropertyName("InvalidId")>] InvalidId
    | [<JsonPropertyName("InvalidCapacity")>] InvalidCapacity
    | [<JsonPropertyName("InvalidScore")>] InvalidScore
    | [<JsonPropertyName("InvalidDate")>] InvalidDate
    | [<JsonPropertyName("DuplicateStudent")>] DuplicateStudent
    | [<JsonPropertyName("UnknownStudent")>] UnknownStudent
    | [<JsonPropertyName("InvalidPromotion")>] InvalidPromotion
    | [<JsonPropertyName("DuplicateClass")>] DuplicateClass
    | [<JsonPropertyName("UnknownClass")>] UnknownClass
    | [<JsonPropertyName("GradeTooLow")>] GradeTooLow
    | [<JsonPropertyName("PrerequisiteMissing")>] PrerequisiteMissing
    | [<JsonPropertyName("AlreadyEnrolled")>] AlreadyEnrolled
    | [<JsonPropertyName("ClassFull")>] ClassFull
    | [<JsonPropertyName("RegistrationClosed")>] RegistrationClosed
    | [<JsonPropertyName("NotEnrolled")>] NotEnrolled
    | [<JsonPropertyName("NotTeaching")>] NotTeaching
    | [<JsonPropertyName("NoAttendance")>] NoAttendance
    | [<JsonPropertyName("AlreadyAssessed")>] AlreadyAssessed
    | [<JsonPropertyName("UnassessedStudents")>] UnassessedStudents
    | [<JsonPropertyName("InvalidHistory")>] InvalidHistory

[<RequireQualifiedAccess>]
type Prerequisite =
    | [<JsonPropertyName("NoPrerequisite")>] NoPrerequisite
    | [<JsonPropertyName("PassedCourse")>] PassedCourse of course: Course

type Student = { StudentId: string; Grade: Grade }

type Plan = { ClassId: string; Course: Course; MinimumGrade: Grade; Prerequisite: Prerequisite; Capacity: int }

[<RequireQualifiedAccess>]
type Progress =
    | [<JsonPropertyName("Reserved")>] Reserved
    | [<JsonPropertyName("Studying")>] Studying of attendance: int
    | [<JsonPropertyName("Passed")>] Passed of score: int * credits: int
    | [<JsonPropertyName("Failed")>] Failed of score: int * reason: FailureReason
    | [<JsonPropertyName("Withdrawn")>] Withdrawn of reason: WithdrawalReason

type Enrollment = { ClassId: string; StudentId: string; Course: Course; Progress: Progress }

type ClassResult = { StudentId: string; Progress: Progress }

[<RequireQualifiedAccess>]
type ClassState =
    | [<JsonPropertyName("Available")>] Available of plan: Plan * students: string list
    | [<JsonPropertyName("Full")>] Full of plan: Plan * students: string list
    | [<JsonPropertyName("Teaching")>] Teaching of plan: Plan * students: string list * startedOn: string
    | [<JsonPropertyName("Completed")>] Completed of plan: Plan * results: ClassResult list

type DecisionModel = { Students: Student list; Classes: ClassState list; Enrollments: Enrollment list }

type SchoolList = { Students: Student list; Classes: ClassState list; Enrollments: Enrollment list; Summaries: string list }

type Trace = { Operation: string; Status: string; Conflicts: int; Appended: int; Position: int; Errors: Rejection list }

[<RequireQualifiedAccess>]
type Command =
    | [<JsonPropertyName("RegisterStudent")>] RegisterStudent of studentId: string * grade: Grade
    | [<JsonPropertyName("PromoteStudent")>] PromoteStudent of studentId: string * grade: Grade
    | [<JsonPropertyName("OpenClass")>] OpenClass of classId: string * course: Course * capacity: int
    | [<JsonPropertyName("Enroll")>] Enroll of classId: string * studentId: string
    | [<JsonPropertyName("Drop")>] Drop of classId: string * studentId: string
    | [<JsonPropertyName("StartClass")>] StartClass of classId: string * startedOn: string
    | [<JsonPropertyName("Attend")>] Attend of classId: string * studentId: string
    | [<JsonPropertyName("Assess")>] Assess of classId: string * studentId: string * score: int
    | [<JsonPropertyName("CompleteClass")>] CompleteClass of classId: string

[<RequireQualifiedAccess>]
type Event =
    | [<JsonPropertyName("StudentRegistered")>] StudentRegistered of studentId: string * grade: Grade
    | [<JsonPropertyName("StudentPromoted")>] StudentPromoted of studentId: string * grade: Grade
    | [<JsonPropertyName("ClassOpened")>] ClassOpened of classId: string * course: Course * capacity: int
    | [<JsonPropertyName("StudentEnrolled")>] StudentEnrolled of classId: string * studentId: string * course: Course
    | [<JsonPropertyName("StudentDropped")>] StudentDropped of classId: string * studentId: string
    | [<JsonPropertyName("ClassStarted")>] ClassStarted of classId: string * startedOn: string
    | [<JsonPropertyName("AttendanceRecorded")>] AttendanceRecorded of classId: string * studentId: string
    | [<JsonPropertyName("ResultRecorded")>] ResultRecorded of classId: string * studentId: string * score: int
    | [<JsonPropertyName("ClassCompleted")>] ClassCompleted of classId: string * results: ClassResult list
    | [<JsonPropertyName("StudyStarted")>] StudyStarted of classId: string * studentId: string

[<RequireQualifiedAccess>]
type Decision =
    | [<JsonPropertyName("Accepted")>] Accepted of events: Event list
    | [<JsonPropertyName("Rejected")>] Rejected of code: Rejection

[<RequireQualifiedAccess>]
type Operation =
    | [<JsonPropertyName("Execute")>] Execute of commandId: string * command: Command
    | [<JsonPropertyName("Prepare")>] Prepare of key: string * commandId: string * command: Command
    | [<JsonPropertyName("Commit")>] Commit of key: string
    | [<JsonPropertyName("ListAll")>] ListAll
    | [<JsonPropertyName("Rebuild")>] Rebuild
    | [<JsonPropertyName("Redeliver")>] Redeliver
