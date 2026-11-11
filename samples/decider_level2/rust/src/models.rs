use serde::{Deserialize, Serialize};

#[derive(Clone, Copy, Debug, PartialEq, Eq, Serialize, Deserialize)]
pub enum Grade { Year1, Year2, Year3 }

#[derive(Clone, Copy, Debug, PartialEq, Eq, Serialize, Deserialize)]
pub enum Course { Foundation, Advanced }

#[derive(Clone, Copy, Debug, PartialEq, Eq, Serialize, Deserialize)]
pub enum FailureReason { BelowPassMark }

#[derive(Clone, Copy, Debug, PartialEq, Eq, Serialize, Deserialize)]
pub enum WithdrawalReason { StudentRequest }

#[derive(Clone, Copy, Debug, PartialEq, Eq, Serialize, Deserialize)]
pub enum Rejection { InvalidId, InvalidCapacity, InvalidScore, InvalidDate, DuplicateStudent, UnknownStudent, InvalidPromotion, DuplicateClass, UnknownClass, GradeTooLow, PrerequisiteMissing, AlreadyEnrolled, ClassFull, RegistrationClosed, NotEnrolled, NotTeaching, NoAttendance, AlreadyAssessed, UnassessedStudents, InvalidHistory }

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Student {
    pub student_id: String,
    pub grade: Grade,
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Plan {
    pub class_id: String,
    pub course: Course,
    pub minimum_grade: Grade,
    pub prerequisite: Prerequisite,
    pub capacity: i32,
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Enrollment {
    pub class_id: String,
    pub student_id: String,
    pub course: Course,
    pub progress: Progress,
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ClassResult {
    pub student_id: String,
    pub progress: Progress,
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct State {
    pub students: Vec<Student>,
    pub classes: Vec<ClassState>,
    pub enrollments: Vec<Enrollment>,
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Prerequisite {
    NoPrerequisite,
    PassedCourse { course: Course },
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Progress {
    Reserved,
    Studying { attendance: i32 },
    Passed { score: i32, credits: i32 },
    Failed { score: i32, reason: FailureReason },
    Withdrawn { reason: WithdrawalReason },
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum ClassState {
    Available { plan: Plan, students: Vec<String> },
    Full { plan: Plan, students: Vec<String> },
    Teaching { plan: Plan, students: Vec<String>, started_on: String },
    Completed { plan: Plan, results: Vec<ClassResult> },
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Command {
    RegisterStudent { student_id: String, grade: Grade },
    PromoteStudent { student_id: String, grade: Grade },
    OpenClass { class_id: String, course: Course, capacity: i32 },
    Enroll { class_id: String, student_id: String },
    Drop { class_id: String, student_id: String },
    StartClass { class_id: String, started_on: String },
    Attend { class_id: String, student_id: String },
    Assess { class_id: String, student_id: String, score: i32 },
    CompleteClass { class_id: String },
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Event {
    StudentRegistered { student_id: String, grade: Grade },
    StudentPromoted { student_id: String, grade: Grade },
    ClassOpened { class_id: String, course: Course, capacity: i32 },
    StudentEnrolled { class_id: String, student_id: String },
    StudentDropped { class_id: String, student_id: String },
    ClassStarted { class_id: String, started_on: String },
    AttendanceRecorded { class_id: String, student_id: String },
    ResultRecorded { class_id: String, student_id: String, score: i32 },
    ClassCompleted { class_id: String },
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Decision {
    Accepted { events: Vec<Event> },
    Rejected { code: Rejection },
}
