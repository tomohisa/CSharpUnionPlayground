import { DcbExecutor, EventStore, LiveProjection, sameTag } from "./dcb.ts";
import type { Query, Recorded, PreparedCommand } from "./dcb.ts";
import type { Command, Event, Operation, Rejection, SchoolList, Trace } from "./models.ts";
import { EventSchema } from "./models.ts";
import { describeEvent } from "./routing.ts";
import { schoolHandler } from "./handlers.ts";
import { empty, copy, reduce, list } from "./projection.ts";
export function run(history: readonly Recorded<Event>[], operations: readonly Operation[]) {
  const store=new EventStore<Event>(describeEvent,e=>EventSchema.parse(e));
  store.load(history);
  const projection=new LiveProjection(empty(),reduce,copy);store.subscribe(projection.accept);
  const executor=new DcbExecutor<Event,Rejection>(store);
  const tickets=new Map<string,PreparedCommand<Rejection>>(), steps:Trace[]=[], lists:SchoolList[]=[];
  const reads:{query:Query;after:number;returned:number}[]=[];
  const prepare=(id:string,command:Command)=>{const t=executor.prepare(id,command,schoolHandler);if(t.wasRead)reads.push({query:t.query,after:t.after,returned:t.returned});return t;};
  const commit=(operation:string,t:PreparedCommand<Rejection>)=>steps.push({operation,...t.commit()});
  for(const operation of operations){
    switch(operation.$type){
      case "Execute":commit("Execute",prepare(operation.commandId,operation.command));break;
      case "Prepare":{if(tickets.has(operation.key))throw new Error("duplicate ticket");const t=prepare(operation.commandId,operation.command);tickets.set(operation.key,t);steps.push({operation:"Prepare",status:"Prepared",conflicts:0,appended:0,position:t.after,errors:[]});break;}
      case "Commit":{const t=tickets.get(operation.key);if(!t)throw new Error("unknown ticket");tickets.delete(operation.key);commit("Commit",t);break;}
      case "ListAll":{if(store.listenerFaults)throw new Error("projection unavailable; rebuild required");const s=projection.snapshot();lists.push(list(s.state));steps.push({operation:"ListAll",status:"Listed",conflicts:0,appended:0,position:s.position,errors:[]});break;}
      case "Rebuild":{const fresh=new LiveProjection(empty(),reduce,copy);if(!fresh.accept(store.read({items:[]}).events))throw new Error("rebuild failed");const s=fresh.snapshot();lists.push(list(s.state));steps.push({operation:"Rebuild",status:"Rebuilt",conflicts:0,appended:0,position:s.position,errors:[]});break;}
      case "Redeliver":{if(!projection.accept(store.read({items:[]}).events))throw new Error("duplicate delivery failed");steps.push({operation:"Redeliver",status:"Redelivered",conflicts:0,appended:0,position:projection.snapshot().position,errors:[]});break;}
    }
  }
  const events=store.read({items:[]}).events,fresh=new LiveProjection(empty(),reduce,copy);if(!fresh.accept(events))throw new Error("rebuild failed");
  if(store.listenerFaults)throw new Error("projection unavailable; rebuild required");
  const final=projection.snapshot();
  return {events,steps,lists,list:list(final.state),rebuilt:list(fresh.snapshot().state),checkpoint:final.position,reads,listenerFaults:store.listenerFaults};
}
