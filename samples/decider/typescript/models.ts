import { z } from "zod";

export const RosterSchema = z.object({
  name: z.string(), capacity: z.int(), students: z.array(z.string()).readonly()
}).readonly();
export const StateSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("NotCreated") }),
  z.object({ $type: z.literal("Available"), roster: RosterSchema }),
  z.object({ $type: z.literal("Full"), roster: RosterSchema })
]).readonly();
export const CommandSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Create"), name: z.string(), capacity: z.int() }),
  z.object({ $type: z.literal("Enroll"), studentId: z.string() }),
  z.object({ $type: z.literal("Drop"), studentId: z.string() })
]).readonly();
export const EventSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("RoomCreated"), name: z.string(), capacity: z.int() }),
  z.object({ $type: z.literal("StudentEnrolled"), studentId: z.string() }),
  z.object({ $type: z.literal("StudentDropped"), studentId: z.string() })
]).readonly();
export type Roster = z.infer<typeof RosterSchema>;
export type State = z.infer<typeof StateSchema>;
export type Command = z.infer<typeof CommandSchema>;
export type Event = z.infer<typeof EventSchema>;
export type Decision =
  | { ok: true; events: readonly Event[] }
  | { ok: false; error: string };
export function unreachable(value: never): never {
  throw new Error("Unknown case: " + JSON.stringify(value));
}
