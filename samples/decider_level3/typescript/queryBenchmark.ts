import {z} from "zod";
import type {JsonEventStore} from "./jsonStore.ts";
import type {Event} from "./models.ts";
import type {SchoolProjection} from "./schoolProjection.ts";
const Student=z.object({studentId:z.string(),grade:z.enum(["Year1","Year2","Year3"])});
export function queryBenchmark(store:JsonEventStore<Event>,projection:SchoolProjection):void{
 const n=Number(process.env.DECIDER_STUDENTS??0);if(n<=0)return;let start=performance.now();
 for(let i=0;i<1000;i++){const id=`student-${String(i*7919%n).padStart(8,"0")}`;if(!store.read({items:[{tags:[{category:"student",keys:[id]}],types:[]}]}).events.length)throw new Error("point read missing");}
 console.error("PHASE tagged_point "+(performance.now()-start)/1000);start=performance.now();
 for(let i=0;i<1000;i++){const id=`student-${String(i*7919%n).padStart(8,"0")}`;if(Student.parse(JSON.parse(JSON.stringify(projection.student(id)))).studentId!==id)throw new Error("point query mismatch");}
 console.error("PHASE projection_point_json "+(performance.now()-start)/1000);
}
