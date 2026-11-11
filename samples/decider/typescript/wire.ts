import { z } from "zod";
import { CommandSchema, EventSchema, StateSchema } from "./models.ts";

export const InputSchema = z.object({
  commands: z.array(CommandSchema),
  history: z.array(EventSchema).nullish().transform(value => value ?? [])
});
export const EventsSchema = z.array(EventSchema);
export const input = (value: unknown) => InputSchema.parse(value);
export const events = (value: unknown) => EventsSchema.parse(value);
export const state = (value: unknown) => StateSchema.parse(value);
