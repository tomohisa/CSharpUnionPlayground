import { z } from "zod";
import { EventSchema,OperationSchema } from "./models.ts";
export const InputSchema=z.object({history:z.array(z.object({position:z.int().positive(),commandId:z.string().min(1),eventType:z.string().min(1),
  tags:z.array(z.object({category:z.string(),keys:z.array(z.string())})),payload:EventSchema})),operations:z.array(OperationSchema)});
