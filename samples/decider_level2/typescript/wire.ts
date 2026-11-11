import { z } from "zod";
import { CommandSchema, EventSchema, StateSchema } from "./models.ts";
export const InputSchema = z.object({ commands: z.array(CommandSchema), history: z.array(EventSchema) });
export const input = (value: unknown) => InputSchema.parse(value);
export const events = (value: unknown) => z.array(EventSchema).parse(value);
export const state = (value: unknown) => StateSchema.parse(value);
