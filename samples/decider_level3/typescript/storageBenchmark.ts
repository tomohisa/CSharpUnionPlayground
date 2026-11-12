import {JsonEventStore,MemoryJsonJournal,SqliteJournal} from "./jsonStore.ts";
import type {JsonJournal} from "./jsonStore.ts";
import {z} from "zod";
const Revision=z.object({documentId:z.string(),revision:z.int(),body:z.string()});type Revision=z.infer<typeof Revision>;
export function storageBenchmark():void{
 const count=Number(process.env.DECIDER_DOCUMENTS??0);if(count<=0)return;
 const path=process.env.DECIDER_PERSISTENT,open=():JsonJournal=>path?new SqliteJournal(path+".documents"):new MemoryJsonJournal();
 const store=(j:JsonJournal)=>new JsonEventStore<Revision>(j,e=>({eventType:"DocumentRevision",tags:[{category:"document",keys:[e.documentId]}]}),JSON.stringify,json=>Revision.parse(JSON.parse(json)));
 const start=performance.now(),s=store(open()),latest=new Map<string,number>();
 s.subscribe(events=>{for(const e of events)latest.set(e.payload.documentId,e.payload.revision);return true;});const body="x".repeat(4096);
 for(let revision=1;revision<=16;revision++)for(let i=0;i<count;i++){
  const documentId=`document-${String(i).padStart(8,"0")}`,q={items:[{tags:[{category:"document",keys:[documentId]}],types:[]}]},r=s.read(q);
  if(r.events.length!==revision-1||s.append(`doc-${i}-${revision}`,q,r.after,[{documentId,revision,body}]).status!=="Committed")throw new Error("document revision mismatch");
 }
 const all=s.read({items:[]});if(all.after!==16*count||latest.size!==count||[...latest.values()].some(r=>r!==16)||s.listenerFaults||all.events.some(e=>e.payload.body.length!==4096))throw new Error("document projection mismatch");
 s.close();if(path){const reopened=store(open()),r=reopened.read({items:[]});if(r.events.length!==16*count||r.events.some(e=>e.payload.body.length!==4096))throw new Error("document restart mismatch");reopened.close();}
 console.error(`DOCUMENT ${(performance.now()-start)/1000} ${count} ${all.after} ${s.encoded} ${s.decoded}`);
}
