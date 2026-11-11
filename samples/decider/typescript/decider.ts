import { unreachable } from "./models.ts";
import type { Command, Decision, Event, Roster, State } from "./models.ts";

export function phase(state: State): string {
  switch (state.$type) {
    case "NotCreated": return "NotCreated";
    case "Available": return "Available";
    case "Full": return "Full";
    default: return unreachable(state);
  }
}
function data(state: State): Roster | null {
  switch (state.$type) {
    case "NotCreated": return null;
    case "Available": return state.roster;
    case "Full": return state.roster;
    default: return unreachable(state);
  }
}
const accept = (event: Event): Decision => ({ ok: true, events: [event] });
const reject = (error: string): Decision => ({ ok: false, error });

export function decide(state: State, command: Command): Decision {
  switch (command.$type) {
    case "Create":
      if (state.$type !== "NotCreated") return reject("already_created");
      if (command.name.length === 0) return reject("invalid_name");
      if (command.capacity < 1 || command.capacity > 10) return reject("invalid_capacity");
      return accept({ $type: "RoomCreated", name: command.name, capacity: command.capacity });
    case "Enroll": return change(state, command.studentId, true);
    case "Drop": return change(state, command.studentId, false);
    default: return unreachable(command);
  }
}
function change(state: State, student: string, enrolling: boolean): Decision {
  const roster = data(state);
  if (roster === null) return reject("not_created");
  if (student.length === 0) return reject("invalid_student");
  const member = roster.students.includes(student);
  if (enrolling) {
    if (member) return reject("already_enrolled");
    if (state.$type === "Full") return reject("full");
    return accept({ $type: "StudentEnrolled", studentId: student });
  }
  return member ? accept({ $type: "StudentDropped", studentId: student }) : reject("not_enrolled");
}
function classify(roster: Roster): State {
  return { $type: roster.students.length === roster.capacity ? "Full" : "Available", roster };
}
function invalidHistory(): never { throw new Error("invalid_history"); }

export function evolve(state: State, event: Event): State {
  switch (event.$type) {
    case "RoomCreated":
      if (state.$type !== "NotCreated" || event.name.length === 0 ||
          event.capacity < 1 || event.capacity > 10) return invalidHistory();
      return { $type: "Available", roster: { name: event.name, capacity: event.capacity, students: [] } };
    case "StudentEnrolled":
      if (state.$type !== "Available" || event.studentId.length === 0 ||
          state.roster.students.includes(event.studentId)) return invalidHistory();
      return classify({ ...state.roster, students: [...state.roster.students, event.studentId] });
    case "StudentDropped": {
      const roster = data(state);
      if (roster === null || !roster.students.includes(event.studentId)) return invalidHistory();
      return classify({ ...roster, students: roster.students.filter(s => s !== event.studentId) });
    }
    default: return unreachable(event);
  }
}
export function replay(events: readonly Event[]): State {
  return events.reduce<State>(evolve, { $type: "NotCreated" });
}
