import { unreachable } from "./models.ts";
import type { ClassState, Command, Decision, Enrollment, Event, Grade, Plan, Progress, Rejection, DecisionModel } from "./models.ts";

export const empty = (): DecisionModel => ({ students: [], classes: [], enrollments: [] });
export function planOf(room: ClassState): Plan {
  switch (room.$type) {
    case "Available": case "Full": case "Teaching": case "Completed": return room.plan;
    default: return unreachable(room);
  }
}
export function members(room: ClassState): readonly string[] {
  switch (room.$type) {
    case "Available": case "Full": case "Teaching": return room.students;
    case "Completed": return room.results.map(r => r.studentId);
    default: return unreachable(room);
  }
}
export function describe(progress: Progress): string {
  switch (progress.$type) {
    case "Reserved": return "reserved";
    case "Studying": return `studying:${progress.attendance}`;
    case "Passed": return `passed:${progress.score}:${progress.credits}`;
    case "Failed": return `failed:${progress.score}:${progress.reason}`;
    case "Withdrawn": return `withdrawn:${progress.reason}`;
    default: return unreachable(progress);
  }
}
const rank = (grade: Grade): number => ({ Year1: 1, Year2: 2, Year3: 3 })[grade];
const accept = (event: Event): Decision => ({ $type: "Accepted", events: [event] });
const reject = (code: Rejection): Decision => ({ $type: "Rejected", code });
function decideOne(state: DecisionModel, command: Command): Decision {
  switch (command.$type) {
    case "RegisterStudent":
      if (command.studentId.length === 0) return reject("InvalidId");
      return state.students.some(s => s.studentId === command.studentId) ? reject("DuplicateStudent")
        : accept({ ...command, $type: "StudentRegistered" });
    case "PromoteStudent":
      for (const student of state.students.filter(s => s.studentId === command.studentId))
        return rank(command.grade) === rank(student.grade) + 1 ? accept({ ...command, $type: "StudentPromoted" }) : reject("InvalidPromotion");
      return reject("UnknownStudent");
    case "OpenClass":
      if (command.classId.length === 0) return reject("InvalidId");
      if (command.capacity < 1 || command.capacity > 10) return reject("InvalidCapacity");
      return state.classes.some(c => planOf(c).classId === command.classId) ? reject("DuplicateClass")
        : accept({ ...command, $type: "ClassOpened" });
    case "Enroll": case "Drop": case "StartClass": case "Attend": case "Assess": case "CompleteClass": break;
    default: return unreachable(command);
  }
  for (const room of state.classes.filter(c => planOf(c).classId === command.classId)) {
    const plan = planOf(room);
    switch (command.$type) {
      case "Enroll":
        for (const student of state.students.filter(s => s.studentId === command.studentId)) {
          if (room.$type !== "Available" && room.$type !== "Full") return reject("RegistrationClosed");
          if (members(room).includes(command.studentId)) return reject("AlreadyEnrolled");
          if (rank(student.grade) < rank(plan.minimumGrade)) return reject("GradeTooLow");
          const prerequisite = plan.prerequisite;
          const eligible = prerequisite.$type === "NoPrerequisite" || state.enrollments.some(e =>
            e.studentId === command.studentId && e.course === prerequisite.course && e.progress.$type === "Passed");
          if (!eligible) return reject("PrerequisiteMissing");
          return room.$type === "Full" ? reject("ClassFull") : accept({ ...command, $type: "StudentEnrolled", course: plan.course });
        }
        return reject("UnknownStudent");
      case "Drop":
        if (room.$type !== "Available" && room.$type !== "Full") return reject("RegistrationClosed");
        return members(room).includes(command.studentId) ? accept({ ...command, $type: "StudentDropped" }) : reject("NotEnrolled");
      case "StartClass":
        if (room.$type !== "Available" && room.$type !== "Full") return reject("RegistrationClosed");
        return command.startedOn.length === 0 ? reject("InvalidDate") : accept({ ...command, $type: "ClassStarted" });
      case "Attend": case "Assess":
        if (room.$type !== "Teaching") return reject("NotTeaching");
        if (!members(room).includes(command.studentId)) return reject("NotEnrolled");
        for (const record of state.enrollments.filter(e => e.classId === command.classId && e.studentId === command.studentId)) {
          if (record.progress.$type !== "Studying") return reject("AlreadyAssessed");
          if (command.$type === "Attend") return accept({ ...command, $type: "AttendanceRecorded" });
          if (command.score < 0 || command.score > 100) return reject("InvalidScore");
          return record.progress.attendance === 0 ? reject("NoAttendance") : accept({ ...command, $type: "ResultRecorded" });
        }
        throw new Error("invariant");
      case "CompleteClass":
        if (room.$type !== "Teaching") return reject("NotTeaching");
        return state.enrollments.some(e => e.classId === command.classId && e.progress.$type === "Studying")
          ? reject("UnassessedStudents") : accept({ ...command, $type: "ClassCompleted", results: state.enrollments.filter(e => e.classId === command.classId && (e.progress.$type === "Passed" || e.progress.$type === "Failed")).map(e => ({ studentId: e.studentId, progress: e.progress })) });
      default: return unreachable(command);
    }
  }
  return reject("UnknownClass");
}
export function decide(state: DecisionModel, command: Command): Decision {
  const decision = decideOne(state, command);
  if (decision.$type === "Accepted") {
    const event = decision.events[0];
    if (event.$type === "ClassStarted") return { $type: "Accepted", events: [event, ...state.enrollments.filter(e => e.classId === event.classId && e.progress.$type === "Reserved").map(e => ({ $type: "StudyStarted" as const, classId: event.classId, studentId: e.studentId }))] };
  }
  return decision;
}
