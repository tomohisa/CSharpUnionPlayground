import { unreachable } from "./models.ts";
import type { ClassState, Command, Decision, Enrollment, Event, Grade, Plan, Progress, Rejection, State } from "./models.ts";

export const empty = (): State => ({ students: [], classes: [], enrollments: [] });
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
export function decide(state: State, command: Command): Decision {
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
          return room.$type === "Full" ? reject("ClassFull") : accept({ ...command, $type: "StudentEnrolled" });
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
          ? reject("UnassessedStudents") : accept({ ...command, $type: "ClassCompleted" });
      default: return unreachable(command);
    }
  }
  return reject("UnknownClass");
}
export function cause(event: Event): Command {
  switch (event.$type) {
    case "StudentRegistered": return { ...event, $type: "RegisterStudent" };
    case "StudentPromoted": return { ...event, $type: "PromoteStudent" };
    case "ClassOpened": return { ...event, $type: "OpenClass" };
    case "StudentEnrolled": return { ...event, $type: "Enroll" };
    case "StudentDropped": return { ...event, $type: "Drop" };
    case "ClassStarted": return { ...event, $type: "StartClass" };
    case "AttendanceRecorded": return { ...event, $type: "Attend" };
    case "ResultRecorded": return { ...event, $type: "Assess" };
    case "ClassCompleted": return { ...event, $type: "CompleteClass" };
    default: return unreachable(event);
  }
}
export function evolve(state: State, event: Event): State {
  if (decide(state, cause(event)).$type === "Rejected") throw new Error("InvalidHistory");
  switch (event.$type) {
    case "StudentRegistered": return { ...state, students: [...state.students, { studentId: event.studentId, grade: event.grade }] };
    case "StudentPromoted": return { ...state, students: state.students.map(s => s.studentId === event.studentId ? { ...s, grade: event.grade } : s) };
    case "ClassOpened": {
      const plan: Plan = { classId: event.classId, course: event.course, capacity: event.capacity,
        minimumGrade: event.course === "Foundation" ? "Year2" : "Year3",
        prerequisite: event.course === "Foundation" ? { $type: "NoPrerequisite" } : { $type: "PassedCourse", course: "Foundation" } };
      return { ...state, classes: [...state.classes, { $type: "Available", plan, students: [] }] };
    }
    case "StudentEnrolled": case "StudentDropped": case "ClassStarted": case "AttendanceRecorded": case "ResultRecorded": case "ClassCompleted": break;
    default: return unreachable(event);
  }
  const rooms = state.classes.filter(c => planOf(c).classId === event.classId);
  if (rooms.length !== 1) throw new Error("invariant");
  const room = rooms[0], plan = planOf(room);
  let updated: ClassState = room, records = state.enrollments;
  const change = (studentId: string, progress: (e: Enrollment) => Progress) =>
    records.map(e => e.classId === event.classId && e.studentId === studentId ? { ...e, progress: progress(e) } : e);
  switch (event.$type) {
    case "StudentEnrolled": {
      const students = [...members(room), event.studentId];
      updated = { $type: students.length === plan.capacity ? "Full" : "Available", plan, students };
      records = [...records.filter(e => e.classId !== event.classId || e.studentId !== event.studentId),
        { classId: event.classId, studentId: event.studentId, course: plan.course, progress: { $type: "Reserved" } }];
      break;
    }
    case "StudentDropped":
      updated = { $type: "Available", plan, students: members(room).filter(s => s !== event.studentId) };
      records = change(event.studentId, () => ({ $type: "Withdrawn", reason: "StudentRequest" })); break;
    case "ClassStarted":
      updated = { $type: "Teaching", plan, students: members(room), startedOn: event.startedOn };
      records = records.map(e => e.classId === event.classId && e.progress.$type === "Reserved" ? { ...e, progress: { $type: "Studying", attendance: 0 } } : e); break;
    case "AttendanceRecorded":
      records = change(event.studentId, e => { if (e.progress.$type !== "Studying") throw new Error("invariant");
        return { $type: "Studying", attendance: e.progress.attendance + 1 }; }); break;
    case "ResultRecorded":
      records = change(event.studentId, () => event.score >= 60 ? { $type: "Passed", score: event.score, credits: plan.course === "Foundation" ? 1 : 2 }
        : { $type: "Failed", score: event.score, reason: "BelowPassMark" }); break;
    case "ClassCompleted":
      updated = { $type: "Completed", plan, results: records.filter(e => e.classId === event.classId && (e.progress.$type === "Passed" || e.progress.$type === "Failed"))
        .map(e => ({ studentId: e.studentId, progress: e.progress })) }; break;
    default: return unreachable(event);
  }
  return { ...state, classes: state.classes.map(c => planOf(c).classId === event.classId ? updated : c), enrollments: records };
}
export const replay = (events: readonly Event[]): State => events.reduce(evolve, empty());
