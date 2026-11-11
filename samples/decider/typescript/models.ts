export type Roster = Readonly<{ name: string; capacity: number; students: readonly string[] }>;
export type State =
  | Readonly<{ $type: "NotCreated" }>
  | Readonly<{ $type: "Available"; roster: Roster }>
  | Readonly<{ $type: "Full"; roster: Roster }>;
export type Command =
  | Readonly<{ $type: "Create"; name: string; capacity: number }>
  | Readonly<{ $type: "Enroll"; studentId: string }>
  | Readonly<{ $type: "Drop"; studentId: string }>;
export type Event =
  | Readonly<{ $type: "RoomCreated"; name: string; capacity: number }>
  | Readonly<{ $type: "StudentEnrolled"; studentId: string }>
  | Readonly<{ $type: "StudentDropped"; studentId: string }>;
export type Decision =
  | { ok: true; events: readonly Event[] }
  | { ok: false; error: string };
export function unreachable(value: never): never {
  throw new Error("Unknown case: " + JSON.stringify(value));
}
