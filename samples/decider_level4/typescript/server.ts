import {createServer} from "node:http";
import {readFileSync} from "node:fs";
import {randomUUID} from "node:crypto";
import {JsonEventStore,MemoryJsonJournal,SqliteJournal} from "../../decider_level3/typescript/jsonStore.ts";
import {DcbExecutor} from "../../decider_level3/typescript/dcb.ts";
import type {PreparedCommand} from "../../decider_level3/typescript/dcb.ts";
import {CommandSchema,EventSchema} from "../../decider_level3/typescript/models.ts";
import type {Event,Rejection} from "../../decider_level3/typescript/models.ts";
import {schoolHandler} from "../../decider_level3/typescript/handlers.ts";
import {describeEvent} from "../../decider_level3/typescript/routing.ts";
import {SchoolProjection} from "../../decider_level3/typescript/schoolProjection.ts";
const schemas=JSON.parse(readFileSync(process.env.API_SPEC!,"utf8")).components.schemas;
class HttpError extends Error{readonly code:number;constructor(code:number,message:string){super(message);this.code=code;}}
function validate(body:any,schema:any):void{
  if(schema.$ref)return validate(body,schemas[schema.$ref.split('/').at(-1)]);
  if(schema.oneOf){if(!body||typeof body!=="object"||!schema.oneOf.some((x:any)=>x.$ref.endsWith('/'+body.$type)))throw new HttpError(400,"InvalidRequest");return validate(body,schemas[body.$type]);}
  if(schema.type==="object"){
    if(!body||typeof body!=="object"||Array.isArray(body)||schema.required.some((k:string)=>!(k in body)))throw new HttpError(400,"InvalidRequest");
    for(const [key,value] of Object.entries(body)){if(!Object.hasOwn(schema.properties,key))throw new HttpError(400,"InvalidRequest");validate(value,schema.properties[key]);}
  }else if(schema.type==="string"){
    if(typeof body!=="string"||schema.const&&body!==schema.const||schema.enum&&!schema.enum.includes(body)||schema.minLength&&(!body.trim()||[...body].length>128))throw new HttpError(400,"InvalidRequest");
  }else if(schema.type==="integer"&&(!Number.isInteger(body)||body<schema.minimum||body>schema.maximum))throw new HttpError(400,"InvalidRequest");
}
const path=process.env.DECIDER_PERSISTENT;
const store=new JsonEventStore<Event>(path?new SqliteJournal(path):new MemoryJsonJournal(),describeEvent,JSON.stringify,json=>EventSchema.parse(JSON.parse(json)));
const projection=new SchoolProjection();store.subscribe(projection.accept);
const executor=new DcbExecutor<Event,Rejection>(store);
const tickets=new Map<string,{created:number;command:PreparedCommand<Rejection>}>();
const ttl=Number(process.env.TICKET_TTL_SECONDS??60)*1000;
const expire=()=>{for(const [id,t]of tickets)if(performance.now()-t.created>=ttl)tickets.delete(id);};
function commit(t:PreparedCommand<Rejection>){const r=t.commit();return {code:r.status==="Committed"?200:r.status==="Rejected"?422:409,body:{...r,projectionPosition:projection.checkpoint}};}
const server=createServer(async(req,res)=>{
  let code=200,body:unknown;
  try{
    const route=new URL(req.url!,"http://localhost").pathname;
    const get=["/health","/school","/status"].includes(route)||route.startsWith("/students/");
    const post=["/commands","/tickets"].includes(route)||/^\/tickets\/[^/]+\/commit$/.test(route);
    if(!get&&!post)throw new HttpError(404,"NotFound");
    if(req.method!==(get?"GET":"POST")){res.setHeader("Allow",get?"GET":"POST");throw new HttpError(405,"MethodNotAllowed");}
    if(get){
      store.refresh();if(store.listenerFaults)throw new Error("projection unavailable");
      if(route==="/health")body={status:"ok"};
      else if(route==="/status")body={eventPosition:store.read({items:[]}).after,projectionPosition:projection.checkpoint,listenerFaults:store.listenerFaults};
      else if(route==="/school")body=projection.snapshot();
      else{const s=projection.findStudent(decodeURIComponent(route.slice(10)));if(!s.state.length)throw new HttpError(404,"NotFound");body={position:s.position,student:s.state[0]};}
    }else if(route==="/commands"||route==="/tickets"){
      if(req.headers['content-type']?.split(';')[0].trim()!=="application/json")throw new HttpError(415,"UnsupportedMediaType");
      let size=0;const parts:Buffer[]=[];for await(const part of req){size+=part.length;if(size<=1024*1024)parts.push(part);}
      if(size>1024*1024)throw new HttpError(413,"PayloadTooLarge");
      let input:any;try{input=JSON.parse(Buffer.concat(parts).toString('utf8'));}catch{throw new HttpError(400,"InvalidRequest");}
      validate(input,schemas.CommandRequest);const command=CommandSchema.parse(input.command);
      const t=executor.prepare(input.commandId,command,schoolHandler);
      if(route==="/commands"||!t.wasRead)({code,body}=commit(t));
      else{expire();if(tickets.size>=1024)throw new HttpError(429,"TooManyTickets");const id=randomUUID();tickets.set(id,{created:performance.now(),command:t});code=201;body={ticket:id,position:t.after};}
    }else{expire();const id=route.split('/')[2],t=tickets.get(id);if(!t)throw new HttpError(404,"NotFound");tickets.delete(id);({code,body}=commit(t.command));}
  }catch(e){code=e instanceof HttpError?e.code:500;body={error:e instanceof HttpError?e.message:"InternalError"};if(code===500)console.error(e);}
  res.writeHead(code,{'Content-Type':'application/json'});res.end(JSON.stringify(body));
});
server.listen(Number(process.env.PORT??8080),'127.0.0.1');
process.on('SIGTERM',()=>server.close(()=>{store.close();process.exit(0);}));
