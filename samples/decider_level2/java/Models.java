import com.fasterxml.jackson.annotation.*;
import java.util.List;
import java.util.Objects;

enum Grade { Year1, Year2, Year3 }
enum Course { Foundation, Advanced }
enum FailureReason { BelowPassMark }
enum WithdrawalReason { StudentRequest }
enum Rejection { InvalidId, InvalidCapacity, InvalidScore, InvalidDate, DuplicateStudent, UnknownStudent, InvalidPromotion, DuplicateClass, UnknownClass, GradeTooLow, PrerequisiteMissing, AlreadyEnrolled, ClassFull, RegistrationClosed, NotEnrolled, NotTeaching, NoAttendance, AlreadyAssessed, UnassessedStudents, InvalidHistory }
record Student(String studentId, Grade grade) {
    public Student {
        Objects.requireNonNull(studentId);
        Objects.requireNonNull(grade);
    }
}
record Plan(String classId, Course course, Grade minimumGrade, Prerequisite prerequisite, int capacity) {
    public Plan {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(course);
        Objects.requireNonNull(minimumGrade);
        Objects.requireNonNull(prerequisite);
    }
}
record Enrollment(String classId, String studentId, Course course, Progress progress) {
    public Enrollment {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
        Objects.requireNonNull(course);
        Objects.requireNonNull(progress);
    }
}
record ClassResult(String studentId, Progress progress) {
    public ClassResult {
        Objects.requireNonNull(studentId);
        Objects.requireNonNull(progress);
    }
}
record State(List<Student> students, List<ClassState> classes, List<Enrollment> enrollments) {
    public State {
        students = List.copyOf(students);
        classes = List.copyOf(classes);
        enrollments = List.copyOf(enrollments);
    }
}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = NoPrerequisite.class, name = "NoPrerequisite"),
    @JsonSubTypes.Type(value = PassedCourse.class, name = "PassedCourse")
})
sealed interface Prerequisite permits NoPrerequisite, PassedCourse {}
record NoPrerequisite() implements Prerequisite {
    public NoPrerequisite {
    }
}
record PassedCourse(Course course) implements Prerequisite {
    public PassedCourse {
        Objects.requireNonNull(course);
    }
}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = Reserved.class, name = "Reserved"),
    @JsonSubTypes.Type(value = Studying.class, name = "Studying"),
    @JsonSubTypes.Type(value = Passed.class, name = "Passed"),
    @JsonSubTypes.Type(value = Failed.class, name = "Failed"),
    @JsonSubTypes.Type(value = Withdrawn.class, name = "Withdrawn")
})
sealed interface Progress permits Reserved, Studying, Passed, Failed, Withdrawn {}
record Reserved() implements Progress {
    public Reserved {
    }
}
record Studying(int attendance) implements Progress {
    public Studying {
    }
}
record Passed(int score, int credits) implements Progress {
    public Passed {
    }
}
record Failed(int score, FailureReason reason) implements Progress {
    public Failed {
        Objects.requireNonNull(reason);
    }
}
record Withdrawn(WithdrawalReason reason) implements Progress {
    public Withdrawn {
        Objects.requireNonNull(reason);
    }
}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = Available.class, name = "Available"),
    @JsonSubTypes.Type(value = Full.class, name = "Full"),
    @JsonSubTypes.Type(value = Teaching.class, name = "Teaching"),
    @JsonSubTypes.Type(value = Completed.class, name = "Completed")
})
sealed interface ClassState permits Available, Full, Teaching, Completed {}
record Available(Plan plan, List<String> students) implements ClassState {
    public Available {
        Objects.requireNonNull(plan);
        students = List.copyOf(students);
    }
}
record Full(Plan plan, List<String> students) implements ClassState {
    public Full {
        Objects.requireNonNull(plan);
        students = List.copyOf(students);
    }
}
record Teaching(Plan plan, List<String> students, String startedOn) implements ClassState {
    public Teaching {
        Objects.requireNonNull(plan);
        students = List.copyOf(students);
        Objects.requireNonNull(startedOn);
    }
}
record Completed(Plan plan, List<ClassResult> results) implements ClassState {
    public Completed {
        Objects.requireNonNull(plan);
        results = List.copyOf(results);
    }
}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = RegisterStudent.class, name = "RegisterStudent"),
    @JsonSubTypes.Type(value = PromoteStudent.class, name = "PromoteStudent"),
    @JsonSubTypes.Type(value = OpenClass.class, name = "OpenClass"),
    @JsonSubTypes.Type(value = Enroll.class, name = "Enroll"),
    @JsonSubTypes.Type(value = Drop.class, name = "Drop"),
    @JsonSubTypes.Type(value = StartClass.class, name = "StartClass"),
    @JsonSubTypes.Type(value = Attend.class, name = "Attend"),
    @JsonSubTypes.Type(value = Assess.class, name = "Assess"),
    @JsonSubTypes.Type(value = CompleteClass.class, name = "CompleteClass")
})
sealed interface Command permits RegisterStudent, PromoteStudent, OpenClass, Enroll, Drop, StartClass, Attend, Assess, CompleteClass {}
record RegisterStudent(String studentId, Grade grade) implements Command {
    public RegisterStudent {
        Objects.requireNonNull(studentId);
        Objects.requireNonNull(grade);
    }
}
record PromoteStudent(String studentId, Grade grade) implements Command {
    public PromoteStudent {
        Objects.requireNonNull(studentId);
        Objects.requireNonNull(grade);
    }
}
record OpenClass(String classId, Course course, int capacity) implements Command {
    public OpenClass {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(course);
    }
}
record Enroll(String classId, String studentId) implements Command {
    public Enroll {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record Drop(String classId, String studentId) implements Command {
    public Drop {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record StartClass(String classId, String startedOn) implements Command {
    public StartClass {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(startedOn);
    }
}
record Attend(String classId, String studentId) implements Command {
    public Attend {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record Assess(String classId, String studentId, int score) implements Command {
    public Assess {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record CompleteClass(String classId) implements Command {
    public CompleteClass {
        Objects.requireNonNull(classId);
    }
}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = StudentRegistered.class, name = "StudentRegistered"),
    @JsonSubTypes.Type(value = StudentPromoted.class, name = "StudentPromoted"),
    @JsonSubTypes.Type(value = ClassOpened.class, name = "ClassOpened"),
    @JsonSubTypes.Type(value = StudentEnrolled.class, name = "StudentEnrolled"),
    @JsonSubTypes.Type(value = StudentDropped.class, name = "StudentDropped"),
    @JsonSubTypes.Type(value = ClassStarted.class, name = "ClassStarted"),
    @JsonSubTypes.Type(value = AttendanceRecorded.class, name = "AttendanceRecorded"),
    @JsonSubTypes.Type(value = ResultRecorded.class, name = "ResultRecorded"),
    @JsonSubTypes.Type(value = ClassCompleted.class, name = "ClassCompleted")
})
sealed interface Event permits StudentRegistered, StudentPromoted, ClassOpened, StudentEnrolled, StudentDropped, ClassStarted, AttendanceRecorded, ResultRecorded, ClassCompleted {}
record StudentRegistered(String studentId, Grade grade) implements Event {
    public StudentRegistered {
        Objects.requireNonNull(studentId);
        Objects.requireNonNull(grade);
    }
}
record StudentPromoted(String studentId, Grade grade) implements Event {
    public StudentPromoted {
        Objects.requireNonNull(studentId);
        Objects.requireNonNull(grade);
    }
}
record ClassOpened(String classId, Course course, int capacity) implements Event {
    public ClassOpened {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(course);
    }
}
record StudentEnrolled(String classId, String studentId) implements Event {
    public StudentEnrolled {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record StudentDropped(String classId, String studentId) implements Event {
    public StudentDropped {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record ClassStarted(String classId, String startedOn) implements Event {
    public ClassStarted {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(startedOn);
    }
}
record AttendanceRecorded(String classId, String studentId) implements Event {
    public AttendanceRecorded {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record ResultRecorded(String classId, String studentId, int score) implements Event {
    public ResultRecorded {
        Objects.requireNonNull(classId);
        Objects.requireNonNull(studentId);
    }
}
record ClassCompleted(String classId) implements Event {
    public ClassCompleted {
        Objects.requireNonNull(classId);
    }
}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = Accepted.class, name = "Accepted"),
    @JsonSubTypes.Type(value = Rejected.class, name = "Rejected")
})
sealed interface Decision permits Accepted, Rejected {}
record Accepted(List<Event> events) implements Decision {
    public Accepted {
        events = List.copyOf(events);
    }
}
record Rejected(Rejection code) implements Decision {
    public Rejected {
        Objects.requireNonNull(code);
    }
}
