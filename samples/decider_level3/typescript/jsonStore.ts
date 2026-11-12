import {DatabaseSync} from "node:sqlite";
import type {SQLInputValue,StatementSync} from "node:sqlite";
import {mkdirSync} from "node:fs";
import {dirname,resolve} from "node:path";
import type {Tag,Query,EventDescriptor,Recorded,ReadResult,AppendResult,CommandStore} from "./dcb.ts";
export type JsonEvent=Readonly<{position:number;commandId:string;eventType:string;payloadJson:string;tagsJson:string}>;
export interface JsonJournal{engineVersion:string;read(query:Query,after?:number):{after:number;events:readonly JsonEvent[]};append(id:string,query:Query,after:number,events:readonly JsonEvent[]):{committed:boolean;events:readonly JsonEvent[]};load(events:readonly JsonEvent[]):void;close():void;}
export function tagKeys(keys:readonly string[]):Buffer{const parts:Buffer[]=[];for(const key of keys){const data=Buffer.from(key),size=Buffer.allocUnsafe(4);size.writeUInt32BE(data.length);parts.push(size,data);}return Buffer.concat(parts);}
const indexKey=(t:Tag)=>tagKeys([t.category,...t.keys]).toString("hex");
export function decodeTags(json:string):readonly Tag[]{
  const value:unknown=JSON.parse(json);if(!Array.isArray(value))throw new Error("tags must be an array");
  const seen=new Set<string>();const tags:Tag[]=[];
  for(const item of value){if(typeof item!=="object"||item===null||typeof item.category!=="string"||!Array.isArray(item.keys)||!item.keys.every((k:unknown)=>typeof k==="string"))throw new Error("invalid tag");const t:Tag={category:item.category,keys:item.keys};const key=indexKey(t);if(seen.has(key))throw new Error("duplicate stored tag");seen.add(key);tags.push(t);}
  return tags;
}
export class MemoryJsonJournal implements JsonJournal{
  readonly engineVersion="memory-json-v1";
  private log:JsonEvent[]=[];
  private tags=new Map<string,number[]>();private types=new Map<string,number[]>();
  private candidates(q:Query,after:number,high:number):number[]{
    if(!q.items.length||q.items.some(i=>!i.tags.length&&!i.types.length))return Array.from({length:high-after},(_,i)=>after+i+1);
    const all=new Set<number>();for(const item of q.items){const sets=item.tags.map(t=>this.tags.get(indexKey(t))??[]);if(item.types.length)sets.push(item.types.flatMap(t=>this.types.get(t)??[]));
      if(sets.length){let selected=new Set(sets[0]);for(const set of sets.slice(1)){const next=new Set(set);selected=new Set([...selected].filter(p=>next.has(p)));}for(const p of selected)if(p>after&&p<=high)all.add(p);}}
    return [...all].sort((a,b)=>a-b);
  }
  private insert(row:JsonEvent):void{this.log.push(row);const type=this.types.get(row.eventType)??[];type.push(row.position);this.types.set(row.eventType,type);for(const tag of decodeTags(row.tagsJson)){const k=indexKey(tag),list=this.tags.get(k)??[];list.push(row.position);this.tags.set(k,list);}}
  read(q:Query,after=0){if(after<0||after>this.log.length)throw new Error("invalid position");return {after:this.log.length,events:this.candidates(q,after,this.log.length).map(p=>this.log[p-1])};}
  append(id:string,q:Query,after:number,events:readonly JsonEvent[]){
    if(!events.length||!id.length||after<0||after>this.log.length)throw new Error("invalid append");if(this.candidates(q,after,this.log.length).length)return {committed:false,events:[]};
    const rows=events.map((e,i)=>({...e,position:this.log.length+i+1,commandId:id}));for(const row of rows)decodeTags(row.tagsJson);for(const row of rows)this.insert(row);return {committed:true,events:rows};
  }
  load(events:readonly JsonEvent[]):void{if(this.log.length)throw new Error("load requires empty journal");if(events.some((e,i)=>e.position!==i+1))throw new Error("invalid imported position");for(const e of events)decodeTags(e.tagsJson);for(const e of events)this.insert({...e});}
  close():void{}
}
export class SqliteJournal implements JsonJournal{
  readonly engineVersion:string;private readonly db:DatabaseSync;private statements=new Map<string,StatementSync>();
  constructor(path:string,synchronous="FULL"){
    if(!["FULL","NORMAL"].includes(synchronous))throw new Error("durability must be FULL or NORMAL");if(path!==":memory:")mkdirSync(dirname(resolve(path)),{recursive:true});this.db=new DatabaseSync(path);
    this.db.exec(`PRAGMA journal_mode=WAL;PRAGMA synchronous=${synchronous};PRAGMA foreign_keys=ON;PRAGMA busy_timeout=5000;PRAGMA cache_size=-2048;PRAGMA wal_autocheckpoint=1000;`);
    this.db.exec("CREATE TABLE IF NOT EXISTS events(position INTEGER PRIMARY KEY,command_id TEXT NOT NULL,event_type TEXT NOT NULL,payload_json TEXT NOT NULL,tags_json TEXT NOT NULL);CREATE TABLE IF NOT EXISTS event_tags(position INTEGER NOT NULL REFERENCES events(position),category TEXT NOT NULL,keys_key BLOB NOT NULL,PRIMARY KEY(position,category,keys_key));CREATE INDEX IF NOT EXISTS tags_match ON event_tags(category,keys_key,position);CREATE INDEX IF NOT EXISTS events_type_position ON events(event_type,position);");
    const version=Number(this.stmt("PRAGMA user_version").get()!.user_version);if(![0,1].includes(version))throw new Error("unsupported schema version");this.db.exec("PRAGMA user_version=1");this.engineVersion=String(this.stmt("SELECT sqlite_version() AS version").get()!.version);
  }
  private stmt(sql:string):StatementSync{let s=this.statements.get(sql);if(!s){s=this.db.prepare(sql);this.statements.set(sql,s);}return s;}
  private high():number{return Number(this.stmt("SELECT COALESCE(MAX(position),0) AS high FROM events").get()!.high);}
  private selection(q:Query,low:number,high:number,exists:boolean):[string,SQLInputValue[]]{
    const args:SQLInputValue[]=[];const param=(v:SQLInputValue)=>{args.push(v);return "?";},range=()=>`position>${param(low)} AND position<=${param(high)}`;
    let condition:string;if(!q.items.length||q.items.some(i=>!i.tags.length&&!i.types.length))condition=range();else{
      const alternatives=q.items.map(item=>{const terms=item.tags.map(t=>`SELECT position FROM event_tags WHERE category=${param(t.category)} AND keys_key=${param(tagKeys(t.keys))} AND ${range()}`);if(item.types.length)terms.push(`SELECT position FROM events WHERE event_type IN (${item.types.map(t=>param(t)).join(",")}) AND ${range()}`);return `SELECT position FROM (${terms.join(" INTERSECT ")})`;});condition=`position IN (${alternatives.join(" UNION ")})`;}
    return [`SELECT ${exists?"1":"position,command_id,event_type,payload_json,tags_json"} FROM events WHERE ${condition}${exists?" LIMIT 1":" ORDER BY position"}`,args];
  }
  read(q:Query,after=0){this.db.exec("BEGIN");try{const high=this.high();if(after<0||after>high)throw new Error("invalid position");const [sql,args]=this.selection(q,after,high,false);const events=this.stmt(sql).all(...args).map(r=>({position:Number(r.position),commandId:String(r.command_id),eventType:String(r.event_type),payloadJson:String(r.payload_json),tagsJson:String(r.tags_json)}));this.db.exec("COMMIT");return {after:high,events};}catch(error){this.db.exec("ROLLBACK");throw error;}}
  private insert(e:JsonEvent):void{this.stmt("INSERT INTO events(position,command_id,event_type,payload_json,tags_json) VALUES(?,?,?,?,?)").run(e.position,e.commandId,e.eventType,e.payloadJson,e.tagsJson);for(const tag of decodeTags(e.tagsJson))this.stmt("INSERT INTO event_tags(position,category,keys_key) VALUES(?,?,?)").run(e.position,tag.category,tagKeys(tag.keys));}
  append(id:string,q:Query,after:number,events:readonly JsonEvent[]){this.db.exec("BEGIN IMMEDIATE");try{const high=this.high();if(!events.length||!id.length||after<0||after>high)throw new Error("invalid append");const [sql,args]=this.selection(q,after,high,true);if(this.stmt(sql).get(...args)!==undefined){this.db.exec("ROLLBACK");return {committed:false,events:[]};}const rows=events.map((e,i)=>({...e,position:high+i+1,commandId:id}));for(const e of rows)this.insert(e);this.db.exec("COMMIT");return {committed:true,events:rows};}catch(error){this.db.exec("ROLLBACK");throw error;}}
  load(events:readonly JsonEvent[]):void{this.db.exec("BEGIN IMMEDIATE");try{if(this.high())throw new Error("load requires empty journal");if(events.some((e,i)=>e.position!==i+1))throw new Error("invalid imported position");for(const e of events)this.insert(e);this.db.exec("COMMIT");}catch(error){this.db.exec("ROLLBACK");throw error;}}
  close():void{this.db.close();}
}
export class JsonEventStore<E> implements CommandStore<E>{
  private readonly journal:JsonJournal;private readonly describe:(e:E)=>EventDescriptor;private readonly encode:(e:E)=>string;private readonly decode:(json:string)=>E;
  private listeners:((batch:readonly Recorded<E>[])=>boolean)[]=[];private published=0;listenerFaults=0;
  encoded=0;decoded=0;encodedBytes=0;decodedBytes=0;
  constructor(journal:JsonJournal,describe:(e:E)=>EventDescriptor,encode:(e:E)=>string,decode:(json:string)=>E){this.journal=journal;this.describe=describe;this.encode=encode;this.decode=decode;}
  get engineVersion(){return this.journal.engineVersion;}
  private encodeEvent(id:string,e:E):JsonEvent{const meta=this.describe(e);if(!meta.eventType.length)throw new Error("event type required");const json=this.encode(e);this.encoded++;this.encodedBytes+=Buffer.byteLength(json);return {position:0,commandId:id,eventType:meta.eventType,payloadJson:json,tagsJson:JSON.stringify(meta.tags)};}
  private decodeEvent(e:JsonEvent):Recorded<E>{const payload=this.decode(e.payloadJson),tags=decodeTags(e.tagsJson),meta=this.describe(payload);this.decoded++;this.decodedBytes+=Buffer.byteLength(e.payloadJson);if(meta.eventType!==e.eventType||meta.tags.length!==tags.length||!meta.tags.every(t=>tags.some(x=>indexKey(t)===indexKey(x))))throw new Error("invalid stored event metadata");return {position:e.position,commandId:e.commandId,eventType:e.eventType,tags,payload};}
  read(q:Query):ReadResult<E>{const r=this.journal.read(q);return {after:r.after,events:r.events.map(e=>this.decodeEvent(e))};}
  private notify(listener:(e:readonly Recorded<E>[])=>boolean,events:readonly Recorded<E>[]):boolean{try{return listener(events);}catch{return false;}}
  refresh():void{if(!this.listeners.length)return;const tail=this.journal.read({items:[]},this.published),events=tail.events.map(e=>this.decodeEvent(e));this.listeners=this.listeners.filter(listener=>{const ok=this.notify(listener,events);if(!ok)this.listenerFaults++;return ok;});this.published=tail.after;}
  append(id:string,q:Query,after:number,events:readonly E[]):AppendResult<E>{const result=this.journal.append(id,q,after,events.map(e=>this.encodeEvent(id,e)));if(!result.committed)return {status:"Conflict",events:[]};if(this.listeners.length)this.refresh();else this.published=result.events.at(-1)!.position;return {status:"Committed",events:result.events.map(e=>this.decodeEvent(e))};}
  subscribe(listener:(events:readonly Recorded<E>[])=>boolean):void{if(this.listeners.length)this.refresh();const r=this.journal.read({items:[]});if(!this.notify(listener,r.events.map(e=>this.decodeEvent(e))))throw new Error("subscription replay failed");this.listeners.push(listener);this.published=r.after;}
  load(events:readonly Recorded<E>[]):void{if(this.listeners.length)throw new Error("load before subscribe");const rows=events.map((e,i)=>{if(e.position!==i+1||!e.commandId.length)throw new Error("invalid imported log");const row={...this.encodeEvent(e.commandId,e.payload),position:e.position},meta=this.describe(e.payload);if(meta.eventType!==e.eventType||meta.tags.length!==e.tags.length||!meta.tags.every(t=>e.tags.some(x=>indexKey(t)===indexKey(x))))throw new Error("invalid imported metadata");return row;});this.journal.load(rows);}
  close():void{this.journal.close();}
}
