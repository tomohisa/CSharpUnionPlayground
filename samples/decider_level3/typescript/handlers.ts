import type {CommandHandler} from "./dcb.ts";
import {CommandSchema} from "./models.ts";
import type {Command,Event,Rejection} from "./models.ts";
import {queryFor} from "./routing.ts";
import {decide} from "./decider.ts";
import {fold,decisionModel} from "./projection.ts";
import {ValidatedCommand} from "./valueObjects.ts";
export const schoolHandler:CommandHandler<Command,Event,Rejection> = {
  validate:command=>ValidatedCommand.validate(command),
  snapshot:command=>CommandSchema.parse(command),
  queryFor,
  handle(events,command){
    const decision=decide(decisionModel(fold(events)),ValidatedCommand.create(command));
    return decision.$type==="Accepted"?{kind:"Emit",events:decision.events}:{kind:"Reject",error:decision.code};
  }
};
