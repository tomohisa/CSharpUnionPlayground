namespace SchoolDomain
open Playground.Dcb
open System.Collections.Generic

type private IndexKey = StudentKey of string | ClassKey of string | EnrollmentKey of string*string | MemberKey of string*string | NodeKey of int
type private ClassIndex = {Phase:ClassState;First:int;Last:int;Count:int}
type private MemberNode = {Student:string;Previous:int;Next:int}
type private IndexRow = StudentRow of Student | ClassRow of ClassIndex | EnrollmentRow of Enrollment | NodeRow of MemberNode | MemberIndex of int
module private SchoolIndex =
    let node (tx:IndexBatch<IndexKey,IndexRow>) n = match tx.Get(NodeKey n) with NodeRow row->row | _->failwith "wrong node kind"
    let apply (tx:IndexBatch<IndexKey,IndexRow>) (recorded:Recorded<Event>) =
        let event=recorded.Payload
        match event with
        | Event.StudentRegistered(id,grade) | Event.StudentPromoted(id,grade) -> tx.Put(StudentKey id,StudentRow {StudentId=id;Grade=grade})
        | Event.ClassOpened(id,_,_) ->
            let phase=(Registers.reduce Registers.empty event).Classes[id]
            tx.Put(ClassKey id,ClassRow {Phase=phase;First=0;Last=0;Count=0})
        | _ ->
            let id=match event with
                   | Event.StudentEnrolled(id,_,_) | Event.StudentDropped(id,_) | Event.ClassStarted(id,_) | Event.ClassCompleted(id,_)
                   | Event.StudyStarted(id,_) | Event.AttendanceRecorded(id,_) | Event.ResultRecorded(id,_,_) -> id
                   | _ -> failwith "invariant"
            match tx.TryGet(ClassKey id) with
            | Some(ClassRow original) ->
                let mutable c=original
                let plan=Decider.planOf c.Phase
                match event with
                | Event.StudentEnrolled(_,student,_) ->
                    let n=recorded.Position
                    if c.Last<>0 then tx.Put(NodeKey c.Last,NodeRow {node tx c.Last with Next=n})
                    tx.Put(NodeKey n,NodeRow {Student=student;Previous=c.Last;Next=0})
                    tx.Put(MemberKey(id,student),MemberIndex n)
                    let count=c.Count+1
                    c<-{First=(if c.First=0 then n else c.First);Last=n;Count=count;Phase=(if count=plan.Capacity then ClassState.Full(plan,[]) else ClassState.Available(plan,[]))}
                    tx.Put(ClassKey id,ClassRow c)
                | Event.StudentDropped(_,student) ->
                    match tx.TryGet(MemberKey(id,student)) with
                    | Some(MemberIndex n) ->
                        let memberRow=node tx n
                        if memberRow.Previous<>0 then tx.Put(NodeKey memberRow.Previous,NodeRow {node tx memberRow.Previous with Next=memberRow.Next})
                        if memberRow.Next<>0 then tx.Put(NodeKey memberRow.Next,NodeRow {node tx memberRow.Next with Previous=memberRow.Previous})
                        tx.Delete(NodeKey n);tx.Delete(MemberKey(id,student))
                        c<-{c with First=(if c.First=n then memberRow.Next else c.First);Last=(if c.Last=n then memberRow.Previous else c.Last);Count=c.Count-1}
                    | _ -> ()
                    tx.Put(ClassKey id,ClassRow {c with Phase=ClassState.Available(plan,[])})
                | Event.ClassStarted(_,date) -> tx.Put(ClassKey id,ClassRow {c with Phase=ClassState.Teaching(plan,[],SchoolDate.value(SchoolDate.fromEvent date))})
                | Event.ClassCompleted(_,results) -> tx.Put(ClassKey id,ClassRow {c with Phase=ClassState.Completed(plan,results)})
                | _ -> ()
            | _ -> ()
            let student=match event with Event.StudentEnrolled(_,s,_) | Event.StudentDropped(_,s) | Event.StudyStarted(_,s) | Event.AttendanceRecorded(_,s) | Event.ResultRecorded(_,s,_) -> Some s | _->None
            match student with
            | None -> ()
            | Some student ->
                let key=EnrollmentKey(id,student)
                match event,tx.TryGet key with
                | Event.StudentEnrolled(_,_,course),_ -> tx.Put(key,EnrollmentRow {ClassId=id;StudentId=student;Course=course;Progress=Progress.Reserved})
                | _,Some(EnrollmentRow value) ->
                    let progress=match event with
                                 | Event.StudentDropped _ -> Progress.Withdrawn StudentRequest
                                 | Event.StudyStarted _ -> Progress.Studying 0
                                 | Event.AttendanceRecorded _ -> match value.Progress with Progress.Studying n->Progress.Studying(n+1) | _->failwith "invalid committed event"
                                 | Event.ResultRecorded(_,_,score) -> let score=AssessmentScore.value(AssessmentScore.fromEvent score) in if score>=60 then Progress.Passed(score,if value.Course=Foundation then 1 else 2) else Progress.Failed(score,BelowPassMark)
                                 | _ -> value.Progress
                    tx.Put(key,EnrollmentRow {value with Progress=progress})
                | _ -> ()
    let list (rows:IReadOnlyDictionary<IndexKey,IndexRow>):SchoolList =
        let students,classes,enrollments=ResizeArray<Student>(),ResizeArray<ClassState>(),ResizeArray<Enrollment>()
        for row in rows.Values do
            match row with
            | StudentRow value -> students.Add value
            | EnrollmentRow value -> enrollments.Add value
            | ClassRow c ->
                match c.Phase with
                | ClassState.Completed _ -> classes.Add c.Phase
                | _ ->
                    let ids=ResizeArray<string>()
                    let mutable n=c.First
                    while n<>0 do
                        match rows[NodeKey n] with NodeRow m->ids.Add m.Student;n<-m.Next | _->failwith "wrong node kind"
                    let ids=List.ofSeq ids
                    classes.Add(match c.Phase with ClassState.Available(p,_)->ClassState.Available(p,ids) | ClassState.Full(p,_)->ClassState.Full(p,ids) | ClassState.Teaching(p,_,d)->ClassState.Teaching(p,ids,d) | ClassState.Completed _->c.Phase)
            | _ -> ()
        let enrollments=enrollments |> Seq.sortBy(fun e->Registers.idOrder e.ClassId,Registers.idOrder e.StudentId) |> Seq.toList
        {Students=students |> Seq.sortBy(fun s->Registers.idOrder s.StudentId) |> Seq.toList
         Classes=classes |> Seq.sortBy(fun c->Registers.idOrder((Decider.planOf c).ClassId)) |> Seq.toList
         Enrollments=enrollments;Summaries=enrollments |> List.map(fun e->Decider.describe e.Progress)}
type SchoolProjection() =
    let index=IndexedProjection(SchoolIndex.apply)
    member _.Accept events=index.Accept events
    member _.Checkpoint=index.Checkpoint
    member _.RowCount=index.RowCount
    member _.Work=index.Work
    member _.Student id=index.Read(fun rows->match rows[StudentKey id] with StudentRow s->s | _->failwith "not a student") |> snd
    member _.FindStudent id=index.Read(fun rows->match rows.TryGetValue(StudentKey id) with true,StudentRow s->Some s | _->None)
    member _.Snapshot()=index.Read(SchoolIndex.list)
