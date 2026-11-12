import type { EventDescriptor, Query, Tag, Recorded, Preparation } from "./dcb.ts";
import { unreachable } from "./models.ts";
import type { Command, Event, Rejection } from "./models.ts";
import { decide } from "./decider.ts";
import { fold, decisionModel } from "./projection.ts";
export const studentTag = (id: string): Tag => ({category:"student",keys:[id]});
export const classTag = (id: string): Tag => ({category:"class",keys:[id]});
export const enrollmentTag = (room:string,student:string): Tag => ({category:"enrollment",keys:[room,student]});
const one = (...tags: Tag[]): Query => ({items:tags.map(t => ({tags:[t],types:[]}))});
export function queryFor(command: Command): Query {
  switch(command.$type) {
    case "RegisterStudent": return one(studentTag(command.studentId));
    case "PromoteStudent": return one(studentTag(command.studentId));
    case "OpenClass": return one(classTag(command.classId));
    case "Enroll": return one(studentTag(command.studentId),classTag(command.classId));
    case "Drop": return one(classTag(command.classId));
    case "StartClass": return one(classTag(command.classId));
    case "Attend": return one(classTag(command.classId));
    case "Assess": return one(classTag(command.classId));
    case "CompleteClass": return one(classTag(command.classId));
    default: return unreachable(command);
  }
}
export function describeEvent(event: Event): EventDescriptor {
  switch(event.$type) {
    case "StudentRegistered": return {eventType:"StudentRegistered",tags:[studentTag(event.studentId)]};
    case "StudentPromoted": return {eventType:"StudentPromoted",tags:[studentTag(event.studentId)]};
    case "ClassOpened": return {eventType:"ClassOpened",tags:[classTag(event.classId)]};
    case "StudentEnrolled": return {eventType:"StudentEnrolled",tags:[studentTag(event.studentId),classTag(event.classId),enrollmentTag(event.classId,event.studentId)]};
    case "StudentDropped": return {eventType:"StudentDropped",tags:[studentTag(event.studentId),classTag(event.classId),enrollmentTag(event.classId,event.studentId)]};
    case "ClassStarted": return {eventType:"ClassStarted",tags:[classTag(event.classId)]};
    case "AttendanceRecorded": return {eventType:"AttendanceRecorded",tags:[studentTag(event.studentId),classTag(event.classId),enrollmentTag(event.classId,event.studentId)]};
    case "ResultRecorded": return {eventType:"ResultRecorded",tags:[studentTag(event.studentId),classTag(event.classId),enrollmentTag(event.classId,event.studentId)]};
    case "ClassCompleted": return {eventType:"ClassCompleted",tags:[classTag(event.classId)]};
    case "StudyStarted": return {eventType:"StudyStarted",tags:[studentTag(event.studentId),classTag(event.classId),enrollmentTag(event.classId,event.studentId)]};
    default: return unreachable(event);
  }
}
