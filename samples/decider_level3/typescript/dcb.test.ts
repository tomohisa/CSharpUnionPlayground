import assert from "node:assert/strict";
import {test} from "node:test";
import {EventStore,LiveProjection,CommandProcessor,DcbExecutor,IndexedProjection} from "./dcb.ts";
import type {Tag,Query,EventDescriptor,CommandHandler} from "./dcb.ts";
type Counter={readonly key:string;readonly delta:number};
const tag=(key:string):Tag=>({category:"counter",keys:[key]});
const query=(key:string):Query=>({items:[{tags:[tag(key)],types:[]}]});
const describe=(e:Counter):EventDescriptor=>({eventType:"CounterDelta",tags:[tag(e.key),{category:"scope",keys:["test"]}]});
test("domain-free store, conditional batch, projection, replay, retries and listener failure",()=>{
  const store=new EventStore(describe,structuredClone),p=new LiveProjection(0,(s:number,e:Counter)=>s+e.delta,s=>s);store.subscribe(p.accept);
  store.subscribe(batch=>{if(batch.length)throw new Error("expected listener failure");return true;});
  assert.throws(()=>store.append("bad-position",query("a"),Number.NaN,[{key:"a",delta:1}]));
  const first=store.append("first",query("a"),0,[{key:"a",delta:1},{key:"a",delta:2}]);assert.equal(first.status,"Committed");assert.equal(first.events.length,2);
  assert.equal(store.listenerFaults,1);assert.deepEqual(p.snapshot(),{position:2,state:3});
  assert.equal(store.append("other",query("b"),0,[{key:"b",delta:9}]).status,"Committed");
  assert.equal(store.append("stale",query("a"),0,[{key:"a",delta:99}]).status,"Conflict");
  const all=store.read({items:[]});assert.equal(all.after,3);assert.deepEqual(p.snapshot(),{position:3,state:12});
  assert.equal(store.read({items:[{tags:[tag("a"),{category:"scope",keys:["test"]}],types:[]}]}).events.length,2);
  assert.equal(store.read({items:[{tags:[tag("a")],types:[]},{tags:[tag("b")],types:[]}]}).events.length,3);
  assert.equal(store.read({items:[{tags:[tag("a")],types:["OtherType"]}]}).events.length,0);
  assert.ok(p.accept(all.events));assert.deepEqual(p.snapshot(),{position:3,state:12});
  const gap=new LiveProjection(0,(s:number,e:Counter)=>s+e.delta,s=>s);assert.equal(gap.accept([all.events[0],all.events[2]]),false);assert.deepEqual(gap.snapshot(),{position:0,state:0});
  const late=new LiveProjection(0,(s:number,e:Counter)=>s+e.delta,s=>s);store.subscribe(late.accept);assert.equal(late.snapshot().state,12);
  const processor=new CommandProcessor<Counter,Counter,string>(store,c=>query(c.key),(_events,c)=>c.delta>0?{kind:"Emit",events:[c]}:{kind:"Reject",error:"NegativeDelta"});
  const ticket=processor.prepare("retry",{key:"a",delta:1});store.append("interfere",query("a"),3,[{key:"a",delta:1}]);
  const result=processor.commit(ticket);assert.equal(result.conflicts,1);assert.equal(result.status,"Committed");assert.deepEqual(p.snapshot(),{position:5,state:14});assert.equal(late.snapshot().state,14);
  assert.equal(processor.commit(processor.prepare("reject",{key:"a",delta:-1})).status,"Rejected");
  const restored=new EventStore(describe,structuredClone);restored.load(store.read({items:[]}).events);assert.equal(restored.read({items:[]}).after,5);
  assert.throws(()=>new EventStore(describe,structuredClone).load([{...all.events[0],tags:[]}]));
});
test("two prepared writers: exactly one conditional append wins on the JS event loop",async()=>{
  const store=new EventStore(describe,structuredClone),reads=[store.read(query("a")),store.read(query("a"))];
  const results=await Promise.all(reads.map((r,i)=>Promise.resolve().then(()=>store.append("race-"+i,query("a"),r.after,[{key:"a",delta:1}]))));
  assert.equal(results.filter(r=>r.status==="Committed").length,1);assert.equal(store.read({items:[]}).after,1);
});

test("executor accepts an unrelated typed command / handler pair",()=>{
  const store=new EventStore<Counter>(describe,structuredClone),executor=new DcbExecutor<Counter,string>(store);
  const handler:CommandHandler<number,Counter,string>={validate:n=>n>0?[]:["positive delta required"],snapshot:n=>n,
    queryFor:n=>{assert.ok(n>0);return query("executor");},handle:(_,n)=>({kind:"Emit",events:[{key:"executor",delta:n}]})};
  assert.equal(executor.execute("valid",5,handler).appended,1);
  assert.equal(executor.execute("invalid",0,handler).status,"Rejected");assert.equal(store.read({items:[]}).after,1);
});

test("indexed projection commits only staged keys; failures and gaps leave its checkpoint and rows unchanged",()=>{
  const p=new IndexedProjection<string,number,Counter>((tx,e)=>{if(e.payload.delta<0)throw new Error("invalid");tx.put(e.payload.key,(tx.find(e.payload.key)??0)+e.payload.delta);});
  const item=(position:number,delta:number)=>({position,commandId:"index",eventType:"CounterDelta",tags:[],payload:{key:"x",delta}});
  assert.ok(p.accept([item(1,3)]));const work=p.work;assert.equal(p.read(rows=>rows.get("x")).state,3);
  assert.equal(p.accept([item(2,4),item(3,-1)]),false);assert.equal(p.checkpoint,1);assert.equal(p.work,work);assert.equal(p.read(rows=>rows.get("x")).state,3);
  assert.equal(p.accept([item(2,4),item(4,5)]),false);assert.equal(p.checkpoint,1);assert.ok(p.accept([item(1,3)]));assert.equal(p.work,work);
});
