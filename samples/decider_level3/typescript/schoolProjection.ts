import {IndexBatch,IndexedProjection} from "./dcb.ts";
import type {Recorded} from "./dcb.ts";
import type {Student,ClassState,Enrollment,Event,Progress,SchoolList} from "./models.ts";
import {planOf,describe} from "./decider.ts";
import {empty,reduce} from "./projection.ts";
import {restoreSchoolDate,restoreAssessmentScore} from "./valueObjects.ts";

type Row={kind:"student";value:Student}|{kind:"class";phase:ClassState;first:number;last:number;count:number}|
  {kind:"enrollment";value:Enrollment}|{kind:"node";student:string;previous:number;next:number}|{kind:"member";node:number};
const key=(kind:string,a="",b="")=>JSON.stringify([kind,a,b]);
const nodeKey=(n:number)=>key("node",String(n));
const typed=<K extends Row["kind"]>(row:Row,kind:K):Extract<Row,{kind:K}>=>{if(row.kind!==kind)throw new Error("wrong row kind");return row as Extract<Row,{kind:K}>;};
function apply(tx:IndexBatch<string,Row>,recorded:Recorded<Event>):void{
  const e=recorded.payload;
  if(e.$type==="StudentRegistered"||e.$type==="StudentPromoted"){tx.put(key("student",e.studentId),{kind:"student",value:{studentId:e.studentId,grade:e.grade}});return;}
  if(e.$type==="ClassOpened"){tx.put(key("class",e.classId),{kind:"class",phase:reduce(empty(),e).classes.get(e.classId)!,first:0,last:0,count:0});return;}
  const id=e.classId,ck=key("class",id),existing=tx.find(ck);
  if(existing){let c=typed(existing,"class");const plan=planOf(c.phase);
    switch(e.$type){
      case "StudentEnrolled":{
        const n=recorded.position;if(c.last){const previous=typed(tx.get(nodeKey(c.last)),"node");tx.put(nodeKey(c.last),{...previous,next:n});}
        tx.put(nodeKey(n),{kind:"node",student:e.studentId,previous:c.last,next:0});tx.put(key("member",id,e.studentId),{kind:"member",node:n});
        const count=c.count+1;c={...c,first:c.first||n,last:n,count,phase:{$type:count===plan.capacity?"Full":"Available",plan,students:[]}};break;
      }
      case "StudentDropped":{
        const found=tx.find(key("member",id,e.studentId));if(found){const n=typed(found,"member").node,m=typed(tx.get(nodeKey(n)),"node");
          if(m.previous){const p=typed(tx.get(nodeKey(m.previous)),"node");tx.put(nodeKey(m.previous),{...p,next:m.next});}
          if(m.next){const p=typed(tx.get(nodeKey(m.next)),"node");tx.put(nodeKey(m.next),{...p,previous:m.previous});}
          tx.delete(nodeKey(n));tx.delete(key("member",id,e.studentId));c={...c,first:c.first===n?m.next:c.first,last:c.last===n?m.previous:c.last,count:c.count-1};}
        c={...c,phase:{$type:"Available",plan,students:[]}};break;
      }
      case "ClassStarted":c={...c,phase:{$type:"Teaching",plan,students:[],startedOn:restoreSchoolDate(e.startedOn)}};break;
      case "ClassCompleted":c={...c,phase:{$type:"Completed",plan,results:e.results}};break;
    }
    if(["StudentEnrolled","StudentDropped","ClassStarted","ClassCompleted"].includes(e.$type))tx.put(ck,c);
  }
  if(!("studentId" in e))return;
  const ek=key("enrollment",id,e.studentId);
  if(e.$type==="StudentEnrolled"){tx.put(ek,{kind:"enrollment",value:{classId:id,studentId:e.studentId,course:e.course,progress:{$type:"Reserved"}}});return;}
  const found=tx.find(ek);if(!found)return;const value=typed(found,"enrollment").value;
  let progress:Progress=value.progress;
  switch(e.$type){
    case "StudentDropped":progress={$type:"Withdrawn",reason:"StudentRequest"};break;
    case "StudyStarted":progress={$type:"Studying",attendance:0};break;
    case "AttendanceRecorded":if(progress.$type!=="Studying")throw new Error("invalid committed event");progress={$type:"Studying",attendance:progress.attendance+1};break;
    case "ResultRecorded":progress=e.score>=60?{$type:"Passed",score:restoreAssessmentScore(e.score),credits:value.course==="Foundation"?1:2}:{$type:"Failed",score:restoreAssessmentScore(e.score),reason:"BelowPassMark"};break;
  }
  tx.put(ek,{kind:"enrollment",value:{...value,progress}});
}
export class SchoolProjection{
  private readonly index=new IndexedProjection(apply);
  get checkpoint(){return this.index.checkpoint;}get rowCount(){return this.index.rowCount;}get work(){return this.index.work;}
  accept=this.index.accept;
  student(id:string):Student{return this.index.read(rows=>structuredClone(typed(rows.get(key("student",id))!,"student").value)).state;}
  findStudent(id:string):{position:number;state:Student[]}{return this.index.read(rows=>{const row=rows.get(key("student",id));return row?.kind==="student"?[structuredClone(row.value)]:[];});}
  snapshot():{position:number;state:SchoolList}{return this.index.read(rows=>{
    const students:Student[]=[],classes:ClassState[]=[],enrollments:Enrollment[]=[];
    for(const row of rows.values())switch(row.kind){case "student":students.push(row.value);break;case "enrollment":enrollments.push(row.value);break;
      case "class":{if(row.phase.$type==="Completed"){classes.push(row.phase);break;}const ids:string[]=[];
        for(let n=row.first;n;){const m=typed(rows.get(nodeKey(n))!,"node");ids.push(m.student);n=m.next;}
        classes.push({...row.phase,students:ids});break;}}
    const cmp=(a:string,b:string)=>Buffer.compare(Buffer.from(a),Buffer.from(b));
    students.sort((a,b)=>cmp(a.studentId,b.studentId));classes.sort((a,b)=>cmp(planOf(a).classId,planOf(b).classId));enrollments.sort((a,b)=>cmp(a.classId,b.classId)||cmp(a.studentId,b.studentId));
    return structuredClone({students,classes,enrollments,summaries:enrollments.map(e=>describe(e.progress))});
  });}
}
