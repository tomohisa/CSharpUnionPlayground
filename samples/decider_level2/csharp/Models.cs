using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace EnrollmentDomain;

[JsonConverter(typeof(JsonStringEnumConverter<Grade>))]
public enum Grade { Year1, Year2, Year3 }

[JsonConverter(typeof(JsonStringEnumConverter<Course>))]
public enum Course { Foundation, Advanced }

[JsonConverter(typeof(JsonStringEnumConverter<FailureReason>))]
public enum FailureReason { BelowPassMark }

[JsonConverter(typeof(JsonStringEnumConverter<WithdrawalReason>))]
public enum WithdrawalReason { StudentRequest }

[JsonConverter(typeof(JsonStringEnumConverter<Rejection>))]
public enum Rejection { InvalidId, InvalidCapacity, InvalidScore, InvalidDate, DuplicateStudent, UnknownStudent, InvalidPromotion, DuplicateClass, UnknownClass, GradeTooLow, PrerequisiteMissing, AlreadyEnrolled, ClassFull, RegistrationClosed, NotEnrolled, NotTeaching, NoAttendance, AlreadyAssessed, UnassessedStudents, InvalidHistory }

public sealed record Student(string StudentId, Grade Grade);
public sealed record Plan(string ClassId, Course Course, Grade MinimumGrade, Prerequisite Prerequisite, int Capacity);
public sealed record Enrollment(string ClassId, string StudentId, Course Course, Progress Progress);
public sealed record ClassResult(string StudentId, Progress Progress);
public sealed record State(ImmutableArray<Student> Students, ImmutableArray<ClassState> Classes, ImmutableArray<Enrollment> Enrollments);

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Prerequisite;
public sealed record NoPrerequisite() : Prerequisite;
public sealed record PassedCourse(Course Course) : Prerequisite;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Progress;
public sealed record Reserved() : Progress;
public sealed record Studying(int Attendance) : Progress;
public sealed record Passed(int Score, int Credits) : Progress;
public sealed record Failed(int Score, FailureReason Reason) : Progress;
public sealed record Withdrawn(WithdrawalReason Reason) : Progress;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record ClassState;
public sealed record Available(Plan Plan, ImmutableArray<string> Students) : ClassState;
public sealed record Full(Plan Plan, ImmutableArray<string> Students) : ClassState;
public sealed record Teaching(Plan Plan, ImmutableArray<string> Students, string StartedOn) : ClassState;
public sealed record Completed(Plan Plan, ImmutableArray<ClassResult> Results) : ClassState;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Command;
public sealed record RegisterStudent(string StudentId, Grade Grade) : Command;
public sealed record PromoteStudent(string StudentId, Grade Grade) : Command;
public sealed record OpenClass(string ClassId, Course Course, int Capacity) : Command;
public sealed record Enroll(string ClassId, string StudentId) : Command;
public sealed record Drop(string ClassId, string StudentId) : Command;
public sealed record StartClass(string ClassId, string StartedOn) : Command;
public sealed record Attend(string ClassId, string StudentId) : Command;
public sealed record Assess(string ClassId, string StudentId, int Score) : Command;
public sealed record CompleteClass(string ClassId) : Command;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Event;
public sealed record StudentRegistered(string StudentId, Grade Grade) : Event;
public sealed record StudentPromoted(string StudentId, Grade Grade) : Event;
public sealed record ClassOpened(string ClassId, Course Course, int Capacity) : Event;
public sealed record StudentEnrolled(string ClassId, string StudentId) : Event;
public sealed record StudentDropped(string ClassId, string StudentId) : Event;
public sealed record ClassStarted(string ClassId, string StartedOn) : Event;
public sealed record AttendanceRecorded(string ClassId, string StudentId) : Event;
public sealed record ResultRecorded(string ClassId, string StudentId, int Score) : Event;
public sealed record ClassCompleted(string ClassId) : Event;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Decision;
public sealed record Accepted(ImmutableArray<Event> Events) : Decision;
public sealed record Rejected(Rejection Code) : Decision;
