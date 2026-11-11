// Reusable library: no school types, Zod, routing rules or command handlers.
export type Tag = Readonly<{ category: string; keys: readonly string[] }>;
export type QueryItem = Readonly<{ tags: readonly Tag[]; types: readonly string[] }>;
export type Query = Readonly<{ items: readonly QueryItem[] }>;
export type EventDescriptor = Readonly<{ eventType: string; tags: readonly Tag[] }>;
export type Recorded<E> = EventDescriptor & Readonly<{ position: number; commandId: string; payload:E }>;
export type ReadResult<E> = Readonly<{ after: number; events: readonly Recorded<E>[] }>;
export const sameTag = (a: Tag, b: Tag) => a.category === b.category && JSON.stringify(a.keys) === JSON.stringify(b.keys);
export const matches = (q: Query, e: EventDescriptor): boolean => q.items.length === 0 || q.items.some(item =>
  (item.types.length === 0 || item.types.includes(e.eventType)) && item.tags.every(t => e.tags.some(x => sameTag(t,x))));
export type AppendResult<E> = { status: "Committed"; events: readonly Recorded<E>[] } | { status: "Conflict"; events: readonly Recorded<E>[] };
export interface CommandStore<E> {
  read(query:Query):ReadResult<E>;
  append(commandId:string,condition:Query,after:number,batch:readonly E[]):AppendResult<E>;
}
// Operations have no await boundary: atomic within one JavaScript event loop.
// This store is not shared across Workers/processes. Consumers must not reenter it.
export class EventStore<E> implements CommandStore<E> {
  private log: Recorded<E>[] = [];
  private listeners: ((batch: readonly Recorded<E>[]) => boolean)[] = [];
  listenerFaults = 0;
  private readonly describe:(event:E)=>EventDescriptor;
  private readonly copyPayload:(event:E)=>E;
  constructor(describe:(event:E)=>EventDescriptor,copyPayload:(event:NoInfer<E>)=>NoInfer<E>){this.describe=describe;this.copyPayload=copyPayload;}
  private copy(e: Recorded<E>): Recorded<E> { return { ...e, tags: e.tags.map(t => ({ ...t, keys: [...t.keys] })), payload: this.copyPayload(e.payload) }; }
  read(query: Query): ReadResult<E> { return { after: this.log.length, events: this.log.filter(e => matches(query,e)).map(e => this.copy(e)) }; }
  append(commandId: string, condition: Query, after: number, batch: readonly E[]): AppendResult<E> {
    if (!batch.length || !commandId.length || !Number.isSafeInteger(after) || after < 0 || after > this.log.length) throw new Error("invalid append");
    if (this.log.some(e => e.position > after && matches(condition,e))) return { status: "Conflict", events: [] };
    const committed = batch.map((payload,i) => {
      const copy=this.copyPayload(payload), e=this.describe(copy);
      if (!e.eventType.length) throw new Error("event type required");
      return this.copy({ ...e, payload:copy, position: this.log.length+i+1, commandId });
    });
    this.log.push(...committed);
    this.listeners = this.listeners.filter(listener => {
      const ok = this.notify(listener,committed.map(e => this.copy(e))); if (!ok) this.listenerFaults++; return ok;
    });
    return { status: "Committed", events: committed.map(e => this.copy(e)) };
  }
  private notify(listener:(events:readonly Recorded<E>[])=>boolean,events:readonly Recorded<E>[]):boolean{try{return listener(events);}catch{return false;}}
  private valid(e:Recorded<E>):boolean{const d=this.describe(e.payload);return d.eventType===e.eventType && d.tags.length===e.tags.length && d.tags.every(t=>e.tags.some(x=>sameTag(t,x)));}
  subscribe(listener: (batch: readonly Recorded<E>[]) => boolean): void {
    if (!this.notify(listener,this.log.map(e => this.copy(e)))) throw new Error("subscription replay failed");
    this.listeners.push(listener);
  }
  load(events: readonly Recorded<E>[]): void {
    if (this.log.length || this.listeners.length) throw new Error("load only into an empty store");
    if (events.some((e,i) => e.position !== i+1 || !e.commandId.length || !e.eventType.length || !this.valid(e))) throw new Error("invalid imported log");
    this.log = events.map(e => this.copy(e));
  }
}
export class LiveProjection<E,S> {
  private state: S;
  private checkpoint = 0;
  private readonly reduce:(state:S,event:E)=>S;
  private readonly copyState:(state:S)=>S;
  constructor(initial:S,reduce:(state:S,event:E)=>S,copyState:(state:S)=>S){this.state=initial;this.reduce=reduce;this.copyState=copyState;}
  accept = (batch: readonly Recorded<E>[]): boolean => {
    let state = this.copyState(this.state), position = this.checkpoint;
    for (const e of batch) {
      if (e.position <= position) continue;
      if (e.position !== position+1) return false;
      state = this.reduce(state,e.payload); position = e.position;
    }
    this.state = state; this.checkpoint = position; return true;
  };
  snapshot(): { position: number; state: S } { return { position: this.checkpoint, state: this.copyState(this.state) }; }
}
export type Preparation<E,R> = { kind: "Emit"; events: readonly E[] } | { kind: "Reject"; error: R };
export type Ticket<C,E,R> = Readonly<{ commandId: string; command: C; query: Query; after: number; returned:number; decision: Preparation<E,R> }>;
export type HandlingResult<R> = { status: string; conflicts: number; appended: number; position: number; errors: readonly R[] };
export class CommandProcessor<C,E,R> {
  private readonly store:CommandStore<E>;
  private readonly queryFor:(c:C)=>Query;
  private readonly decide:(events:readonly Recorded<E>[],c:C)=>Preparation<E,R>;
  private readonly maxConflicts:number;
  constructor(store:CommandStore<E>,queryFor:(c:C)=>Query,decide:(events:readonly Recorded<E>[],c:C)=>Preparation<E,R>,maxConflicts=3){this.store=store;this.queryFor=queryFor;this.decide=decide;this.maxConflicts=maxConflicts;}
  prepare(commandId: string, command: C): Ticket<C,E,R> {
    const query=this.queryFor(command), read=this.store.read(query);
    return {commandId,command,query,after:read.after,returned:read.events.length,decision:this.decide(read.events,command)};
  }
  commit(ticket: Ticket<C,E,R>): HandlingResult<R> {
    let conflicts=0;
    while (true) {
      if (ticket.decision.kind === "Reject") return {status:"Rejected",conflicts,appended:0,position:ticket.after,errors:[ticket.decision.error]};
      const result=this.store.append(ticket.commandId,ticket.query,ticket.after,ticket.decision.events);
      if (result.status === "Committed") return {status:"Committed",conflicts,appended:result.events.length,position:result.events[result.events.length-1].position,errors:[]};
      if (++conflicts >= this.maxConflicts) return {status:"RetryExhausted",conflicts,appended:0,position:ticket.after,errors:[]};
      ticket=this.prepare(ticket.commandId,ticket.command);
    }
  }
}
