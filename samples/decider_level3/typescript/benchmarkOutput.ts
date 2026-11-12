import type {run} from "./application.ts";
export function benchmarkOutput(o:ReturnType<typeof run>):string{
 const stepCounts:Record<string,number>={},errors:Record<string,number>={};let conflicts=0;
 for(const s of o.steps){stepCounts[s.status]=(stepCounts[s.status]??0)+1;for(const e of s.errors)errors[e]=(errors[e]??0)+1;conflicts+=s.conflicts;}
 return JSON.stringify({list:o.list,rebuildMatches:JSON.stringify(o.list)===JSON.stringify(o.rebuilt),eventCount:o.events.length,checkpoint:o.checkpoint,listenerFaults:o.listenerFaults,stepCounts,errors,conflicts});
}
