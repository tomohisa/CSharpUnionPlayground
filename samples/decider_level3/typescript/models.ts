import { z } from "zod";

export const GradeSchema = z.enum(["Year1", "Year2", "Year3"]);
export type Grade = z.infer<typeof GradeSchema>;
export const CourseSchema = z.enum(["Foundation", "Advanced"]);
export type Course = z.infer<typeof CourseSchema>;
export const FailureReasonSchema = z.enum(["BelowPassMark"]);
export type FailureReason = z.infer<typeof FailureReasonSchema>;
export const WithdrawalReasonSchema = z.enum(["StudentRequest"]);
export type WithdrawalReason = z.infer<typeof WithdrawalReasonSchema>;
export const RejectionSchema = z.enum(["InvalidId", "InvalidCapacity", "InvalidScore", "InvalidDate", "DuplicateStudent", "UnknownStudent", "InvalidPromotion", "DuplicateClass", "UnknownClass", "GradeTooLow", "PrerequisiteMissing", "AlreadyEnrolled", "ClassFull", "RegistrationClosed", "NotEnrolled", "NotTeaching", "NoAttendance", "AlreadyAssessed", "UnassessedStudents", "InvalidHistory"]);
export type Rejection = z.infer<typeof RejectionSchema>;

export const PrerequisiteSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("NoPrerequisite") }),
  z.object({ $type: z.literal("PassedCourse") , course: CourseSchema }),
]).readonly();
export type Prerequisite = z.infer<typeof PrerequisiteSchema>;

export const StudentSchema = z.object({ studentId: z.string(), grade: GradeSchema }).readonly();
export type Student = z.infer<typeof StudentSchema>;

export const PlanSchema = z.object({ classId: z.string(), course: CourseSchema, minimumGrade: GradeSchema, prerequisite: PrerequisiteSchema, capacity: z.int() }).readonly();
export type Plan = z.infer<typeof PlanSchema>;

export const ProgressSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Reserved") }),
  z.object({ $type: z.literal("Studying") , attendance: z.int() }),
  z.object({ $type: z.literal("Passed") , score: z.int(), credits: z.int() }),
  z.object({ $type: z.literal("Failed") , score: z.int(), reason: FailureReasonSchema }),
  z.object({ $type: z.literal("Withdrawn") , reason: WithdrawalReasonSchema }),
]).readonly();
export type Progress = z.infer<typeof ProgressSchema>;

export const EnrollmentSchema = z.object({ classId: z.string(), studentId: z.string(), course: CourseSchema, progress: ProgressSchema }).readonly();
export type Enrollment = z.infer<typeof EnrollmentSchema>;

export const ClassResultSchema = z.object({ studentId: z.string(), progress: ProgressSchema }).readonly();
export type ClassResult = z.infer<typeof ClassResultSchema>;

export const ClassStateSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Available") , plan: PlanSchema, students: z.array(z.string()).readonly() }),
  z.object({ $type: z.literal("Full") , plan: PlanSchema, students: z.array(z.string()).readonly() }),
  z.object({ $type: z.literal("Teaching") , plan: PlanSchema, students: z.array(z.string()).readonly(), startedOn: z.string() }),
  z.object({ $type: z.literal("Completed") , plan: PlanSchema, results: z.array(ClassResultSchema).readonly() }),
]).readonly();
export type ClassState = z.infer<typeof ClassStateSchema>;

export const DecisionModelSchema = z.object({ students: z.array(StudentSchema).readonly(), classes: z.array(ClassStateSchema).readonly(), enrollments: z.array(EnrollmentSchema).readonly() }).readonly();
export type DecisionModel = z.infer<typeof DecisionModelSchema>;

export const SchoolListSchema = z.object({ students: z.array(StudentSchema).readonly(), classes: z.array(ClassStateSchema).readonly(), enrollments: z.array(EnrollmentSchema).readonly(), summaries: z.array(z.string()).readonly() }).readonly();
export type SchoolList = z.infer<typeof SchoolListSchema>;

export const TraceSchema = z.object({ operation: z.string(), status: z.string(), conflicts: z.int(), appended: z.int(), position: z.int(), errors: z.array(RejectionSchema).readonly() }).readonly();
export type Trace = z.infer<typeof TraceSchema>;

export const CommandSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("RegisterStudent") , studentId: z.string(), grade: GradeSchema }),
  z.object({ $type: z.literal("PromoteStudent") , studentId: z.string(), grade: GradeSchema }),
  z.object({ $type: z.literal("OpenClass") , classId: z.string(), course: CourseSchema, capacity: z.int() }),
  z.object({ $type: z.literal("Enroll") , classId: z.string(), studentId: z.string() }),
  z.object({ $type: z.literal("Drop") , classId: z.string(), studentId: z.string() }),
  z.object({ $type: z.literal("StartClass") , classId: z.string(), startedOn: z.string() }),
  z.object({ $type: z.literal("Attend") , classId: z.string(), studentId: z.string() }),
  z.object({ $type: z.literal("Assess") , classId: z.string(), studentId: z.string(), score: z.int() }),
  z.object({ $type: z.literal("CompleteClass") , classId: z.string() }),
]).readonly();
export type Command = z.infer<typeof CommandSchema>;

export const EventSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("StudentRegistered") , studentId: z.string(), grade: GradeSchema }),
  z.object({ $type: z.literal("StudentPromoted") , studentId: z.string(), grade: GradeSchema }),
  z.object({ $type: z.literal("ClassOpened") , classId: z.string(), course: CourseSchema, capacity: z.int() }),
  z.object({ $type: z.literal("StudentEnrolled") , classId: z.string(), studentId: z.string(), course: CourseSchema }),
  z.object({ $type: z.literal("StudentDropped") , classId: z.string(), studentId: z.string() }),
  z.object({ $type: z.literal("ClassStarted") , classId: z.string(), startedOn: z.string() }),
  z.object({ $type: z.literal("AttendanceRecorded") , classId: z.string(), studentId: z.string() }),
  z.object({ $type: z.literal("ResultRecorded") , classId: z.string(), studentId: z.string(), score: z.int() }),
  z.object({ $type: z.literal("ClassCompleted") , classId: z.string(), results: z.array(ClassResultSchema).readonly() }),
  z.object({ $type: z.literal("StudyStarted") , classId: z.string(), studentId: z.string() }),
]).readonly();
export type Event = z.infer<typeof EventSchema>;

export const DecisionSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Accepted") , events: z.array(EventSchema).readonly() }),
  z.object({ $type: z.literal("Rejected") , code: RejectionSchema }),
]).readonly();
export type Decision = z.infer<typeof DecisionSchema>;

export const OperationSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Execute") , commandId: z.string(), command: CommandSchema }),
  z.object({ $type: z.literal("Prepare") , key: z.string(), commandId: z.string(), command: CommandSchema }),
  z.object({ $type: z.literal("Commit") , key: z.string() }),
  z.object({ $type: z.literal("ListAll") }),
  z.object({ $type: z.literal("Rebuild") }),
  z.object({ $type: z.literal("Redeliver") }),
]).readonly();
export type Operation = z.infer<typeof OperationSchema>;

export function unreachable(value: never): never { throw new Error("Unknown case: " + JSON.stringify(value)); }
