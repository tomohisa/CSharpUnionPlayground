import {Buffer} from "node:buffer";
import type { Recorded } from "./dcb.ts";
import type { Student, ClassState, Enrollment, Event, Plan, DecisionModel, SchoolList, Progress } from "./models.ts";
import { describe, members, planOf } from "./decider.ts";
import {restoreSeatCapacity,restoreSchoolDate,restoreAssessmentScore} from "./valueObjects.ts";
export type Registers = { students: Map<string,Student>; classes: Map<string,ClassState>; enrollments: Map<string,Enrollment> };
export const empty = (): Registers => ({students:new Map(),classes:new Map(),enrollments:new Map()});
export const copy = (s: Registers): Registers => structuredClone(s);
const key = (room:string,student:string) => JSON.stringify([room,student]);
export function applyStudent(s: Registers, e: Event): void {
  switch(e.$type) {case "StudentRegistered": case "StudentPromoted": s.students.set(e.studentId,{studentId:e.studentId,grade:e.grade});}
}
export function applyClass(s: Registers,e: Event): void {
  if (e.$type === "ClassOpened") {
    const plan: Plan={classId:e.classId,course:e.course,capacity:restoreSeatCapacity(e.capacity),minimumGrade:e.course === "Foundation" ? "Year2" : "Year3",
      prerequisite:e.course === "Foundation" ? {$type:"NoPrerequisite"} : {$type:"PassedCourse",course:"Foundation"}};
    s.classes.set(e.classId,{$type:"Available",plan,students:[]});return;
  }
  if (!("classId" in e)) return;
  const room=s.classes.get(e.classId);if (!room) return; // A student query need not contain this class's opening.
  const plan=planOf(room);
  switch(e.$type) {
    case "StudentEnrolled": {const students=[...members(room),e.studentId];s.classes.set(e.classId,{$type:students.length === plan.capacity ? "Full" : "Available",plan,students});break;}
    case "StudentDropped": s.classes.set(e.classId,{$type:"Available",plan,students:members(room).filter(x=>x!==e.studentId)});break;
    case "ClassStarted": s.classes.set(e.classId,{$type:"Teaching",plan,students:members(room),startedOn:restoreSchoolDate(e.startedOn)});break;
    case "ClassCompleted": s.classes.set(e.classId,{$type:"Completed",plan,results:e.results});break;
  }
}
export function applyEnrollment(s: Registers,e: Event): void {
  if (!("classId" in e && "studentId" in e)) return;
  const k=key(e.classId,e.studentId);
  if (e.$type === "StudentEnrolled") {s.enrollments.set(k,{classId:e.classId,studentId:e.studentId,course:e.course,progress:{$type:"Reserved"}});return;}
  const record=s.enrollments.get(k);if (!record) return;
  let progress: Progress=record.progress;
  switch(e.$type) {
    case "StudentDropped": progress={$type:"Withdrawn",reason:"StudentRequest"};break;
    case "StudyStarted": progress={$type:"Studying",attendance:0};break;
    case "AttendanceRecorded": if (progress.$type !== "Studying") throw new Error("invalid committed event");progress={$type:"Studying",attendance:progress.attendance+1};break;
    case "ResultRecorded": progress=e.score >= 60 ? {$type:"Passed",score:restoreAssessmentScore(e.score),credits:record.course === "Foundation" ? 1 : 2} : {$type:"Failed",score:restoreAssessmentScore(e.score),reason:"BelowPassMark"};break;
  }
  s.enrollments.set(k,{...record,progress});
}
export function reduce(s: Registers,e: Event): Registers {applyStudent(s,e);applyClass(s,e);applyEnrollment(s,e);return s;}
export const fold = (events: readonly Recorded<Event>[]): Registers => events.reduce((s,e)=>reduce(s,e.payload),empty());
const compareIds=(a:string,b:string)=>Buffer.compare(Buffer.from(a,"utf8"),Buffer.from(b,"utf8"));
export function decisionModel(s: Registers): DecisionModel {
  return {students:[...s.students.values()].sort((a,b)=>compareIds(a.studentId,b.studentId)),
    classes:[...s.classes.values()].sort((a,b)=>compareIds(planOf(a).classId,planOf(b).classId)),
    enrollments:[...s.enrollments.values()].sort((a,b)=>compareIds(a.classId,b.classId)||compareIds(a.studentId,b.studentId))};
}
export function list(s: Registers): SchoolList {const model=decisionModel(s);return {...model,summaries:model.enrollments.map(e=>describe(e.progress))};}
