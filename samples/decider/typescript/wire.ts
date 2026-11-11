import type { Command, Event, State } from "./models.ts";

// JSON.parse is not a runtime type check. Keep boundary checks outside the decider.
function object(value: unknown): Record<string, unknown> {
  if (value === null || typeof value !== "object" || Array.isArray(value)) throw new Error("invalid_json");
  return value as Record<string, unknown>;
}
function text(value: unknown): string {
  if (typeof value !== "string") throw new Error("invalid_json");
  return value;
}
function integer(value: unknown): number {
  if (typeof value !== "number" || !Number.isSafeInteger(value)) throw new Error("invalid_json");
  return value;
}
export function array<T>(value: unknown, decode: (x: unknown) => T): T[] {
  if (!Array.isArray(value)) throw new Error("invalid_json");
  return value.map(decode);
}
export function command(value: unknown): Command {
  const v = object(value);
  switch (v.$type) {
    case "Create": return { $type: "Create", name: text(v.name), capacity: integer(v.capacity) };
    case "Enroll": return { $type: "Enroll", studentId: text(v.studentId) };
    case "Drop": return { $type: "Drop", studentId: text(v.studentId) };
    default: throw new Error("unknown_command");
  }
}
export function event(value: unknown): Event {
  const v = object(value);
  switch (v.$type) {
    case "RoomCreated": return { $type: "RoomCreated", name: text(v.name), capacity: integer(v.capacity) };
    case "StudentEnrolled": return { $type: "StudentEnrolled", studentId: text(v.studentId) };
    case "StudentDropped": return { $type: "StudentDropped", studentId: text(v.studentId) };
    default: throw new Error("unknown_event");
  }
}
export function state(value: unknown): State {
  const v = object(value);
  switch (v.$type) {
    case "NotCreated": return { $type: "NotCreated" };
    case "Available":
    case "Full": {
      const r = object(v.roster);
      return { $type: v.$type, roster: {
        name: text(r.name), capacity: integer(r.capacity), students: array(r.students, text)
      }};
    }
    default: throw new Error("unknown_state");
  }
}
export function input(value: unknown): { commands: Command[]; history: Event[] } {
  const v = object(value);
  return { commands: array(v.commands, command), history: v.history == null ? [] : array(v.history, event) };
}
