import assert from "node:assert/strict";
import {performance} from "node:perf_hooks";
import {SchoolProjection} from "./schoolProjection.ts";
import type {Event} from "./models.ts";
import type {Recorded as Envelope} from "./dcb.ts";
import {members} from "./decider.ts";
export function runProjectionTests(size:number):void{
  const updates=1000,p=new SchoolProjection();let position=0;
  const envelope=(position:number,payload:Event):Envelope<Event>=>({position,commandId:"scale",eventType:"projection-test",tags:[],payload});
  const send=(e:Event)=>assert.ok(p.accept([envelope(++position,e)]));
  let start=performance.now();send({$type:"ClassOpened",classId:"a",course:"Foundation",capacity:size+updates+1});
  for(let i=0;i<size;i++){const id="s"+i;send({$type:"StudentRegistered",studentId:id,grade:"Year3"});send({$type:"StudentEnrolled",classId:"a",studentId:id,course:"Foundation"});}
  const seedSeconds=(performance.now()-start)/1000,old=p.snapshot().state,before=p.checkpoint;let work=p.work;
  assert.equal(p.accept([envelope(before+1,{$type:"StudentPromoted",studentId:"s0",grade:"Year1"}),envelope(before+2,{$type:"AttendanceRecorded",classId:"a",studentId:"s0"})]),false);
  assert.equal(p.checkpoint,before);assert.equal(p.work,work);assert.equal(p.snapshot().state.students.find(s=>s.studentId==="s0")!.grade,"Year3");
  assert.equal(p.accept([envelope(before+1,{$type:"StudentPromoted",studentId:"s0",grade:"Year1"}),envelope(before+3,{$type:"StudentPromoted",studentId:"s0",grade:"Year1"})]),false);
  assert.equal(p.checkpoint,before);assert.equal(p.work,work);assert.ok(p.accept([envelope(1,{$type:"ClassOpened",classId:"ignored",course:"Foundation",capacity:1})]));assert.equal(p.work,work);
  const stableRows=p.rowCount;
  for(let i=0;i<100;i++){send({$type:"StudentDropped",classId:"a",studentId:"s1"});send({$type:"StudentEnrolled",classId:"a",studentId:"s1",course:"Foundation"});}
  const order=members(p.snapshot().state.classes[0]);assert.equal(p.rowCount,stableRows);assert.equal(order.length,size);assert.equal(order[0],"s0");assert.equal(order[1],"s2");assert.equal(order.at(-1),"s1");
  start=performance.now();work=p.work;
  for(let i=size;i<size+updates;i++){const id="s"+i;send({$type:"StudentRegistered",studentId:id,grade:"Year3"});send({$type:"StudentEnrolled",classId:"a",studentId:id,course:"Foundation"});}
  const growthSeconds=(performance.now()-start)/1000,growthWork=p.work-work;assert.equal(growthWork,8*updates);assert.equal(members(old.classes[0]).length,size);assert.equal(old.students.length,size);
  send({$type:"ClassStarted",classId:"a",startedOn:"2026-04-01"});send({$type:"StudyStarted",classId:"a",studentId:"s0"});
  start=performance.now();work=p.work;
  for(let i=0;i<updates;i++)send({$type:"AttendanceRecorded",classId:"a",studentId:"s0"});
  const updateSeconds=(performance.now()-start)/1000,updateWork=p.work-work;assert.equal(updateWork,3*updates);
  start=performance.now();const snapshot=p.snapshot().state,snapshotSeconds=(performance.now()-start)/1000;
  assert.equal(snapshot.students.length,size+updates);assert.equal(members(snapshot.classes[0]).length,size+updates);assert.deepEqual(snapshot.enrollments.find(e=>e.studentId==="s0")!.progress,{$type:"Studying",attendance:updates});
  // A caller's mutations to a detached DTO cannot mutate the live rows.
  (snapshot.students[0] as {grade:string}).grade="Year1";assert.equal(p.snapshot().state.students[0].grade,"Year3");
  console.log(JSON.stringify({size,updates,rows:p.rowCount,seedSeconds,updateSeconds,growthSeconds,snapshotSeconds,updateWork,growthWork}));
}
