import {z} from "zod";
import {CommandSchema} from "./models.ts";
import type {Command,Rejection} from "./models.ts";
function validDate(value:string):boolean {
  if(!/^\d{4}-\d{2}-\d{2}$/.test(value))return false;
  const [y,m,d]=value.split("-").map(Number), leap=y%4===0 && (y%100!==0 || y%400===0);
  return y>=1 && m>=1 && m<=12 && d>=1 && d<=[31,leap?29:28,31,30,31,30,31,31,30,31,30,31][m-1];
}

export const StudentIdSchema=z.string().refine(v=>v.trim().length>0).brand<"StudentId">();
export const ClassIdSchema=z.string().refine(v=>v.trim().length>0).brand<"ClassId">();
export const SeatCapacitySchema=z.int().min(1).max(10).brand<"SeatCapacity">();
export const AssessmentScoreSchema=z.int().min(0).max(100).brand<"AssessmentScore">();
export const SchoolDateSchema=z.string().refine(validDate).brand<"SchoolDate">();
export type StudentId=z.infer<typeof StudentIdSchema>;
export type ClassId=z.infer<typeof ClassIdSchema>;
export type SeatCapacity=z.infer<typeof SeatCapacitySchema>;
export type AssessmentScore=z.infer<typeof AssessmentScoreSchema>;
export type SchoolDate=z.infer<typeof SchoolDateSchema>;
export class ValidatedCommand {
  private readonly brand="ValidatedCommand";
  readonly value:Command;
  private constructor(value:Command){this.value=value;}
  static validate(command:Command):readonly Rejection[]{
    const parsed=CommandSchema.safeParse(command);if(!parsed.success)return ["InvalidCommand"];
    const c=parsed.data;
    if(("classId" in c && !ClassIdSchema.safeParse(c.classId).success) || ("studentId" in c && !StudentIdSchema.safeParse(c.studentId).success))return ["InvalidId"];
    switch(c.$type){
      case "OpenClass":return SeatCapacitySchema.safeParse(c.capacity).success?[]:["InvalidCapacity"];
      case "Assess":return AssessmentScoreSchema.safeParse(c.score).success?[]:["InvalidScore"];
      case "StartClass":return SchoolDateSchema.safeParse(c.startedOn).success?[]:["InvalidDate"];
      default:return [];
    }
  }
  static create(command:Command):ValidatedCommand{
    const errors=this.validate(command);if(errors.length)throw new Error(errors.join(","));
    return new ValidatedCommand(CommandSchema.parse(command));
  }
}

// Storage boundaries check representation, not today's business policy.
export const restoreStudentId=(value:string)=>z.string().parse(value) as StudentId;
export const restoreClassId=(value:string)=>z.string().parse(value) as ClassId;
export const restoreSeatCapacity=(value:number)=>z.int().parse(value) as SeatCapacity;
export const restoreAssessmentScore=(value:number)=>z.int().parse(value) as AssessmentScore;
export const restoreSchoolDate=(value:string)=>z.string().parse(value) as SchoolDate;
