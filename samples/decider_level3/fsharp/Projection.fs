namespace SchoolDomain
open Playground.Dcb

type Registers = { Students: Map<string,Student>; Classes: Map<string,ClassState>; Enrollments: Map<string*string,Enrollment> }
module StudentRegister =
    let apply students = function
        | Event.StudentRegistered(id,g) | Event.StudentPromoted(id,g) -> Map.add id { StudentId=id;Grade=g } students
        | _ -> students
module ClassRegister =
    let apply classes event =
        match event with
        | Event.ClassOpened(id,course,capacity) ->
            let plan={ ClassId=id;Course=course;Capacity=SeatCapacity.value(SeatCapacity.fromEvent capacity);MinimumGrade=(if course=Foundation then Year2 else Year3)
                       Prerequisite=(if course=Foundation then Prerequisite.NoPrerequisite else Prerequisite.PassedCourse Foundation) }
            Map.add id (ClassState.Available(plan,[])) classes
        | Event.StudentEnrolled(id,_,_) | Event.StudentDropped(id,_) | Event.ClassStarted(id,_) | Event.ClassCompleted(id,_) ->
            match Map.tryFind id classes with
            | None -> classes // A student query need not include the class opening.
            | Some room ->
                let plan=Decider.planOf room
                let next=match event with
                         | Event.StudentEnrolled(_,student,_) ->
                             let students=Decider.members room @ [student]
                             if students.Length=plan.Capacity then ClassState.Full(plan,students) else ClassState.Available(plan,students)
                         | Event.StudentDropped(_,student) -> ClassState.Available(plan,Decider.members room |> List.filter ((<>) student))
                         | Event.ClassStarted(_,date) -> ClassState.Teaching(plan,Decider.members room,SchoolDate.value(SchoolDate.fromEvent date))
                         | Event.ClassCompleted(_,results) -> ClassState.Completed(plan,results)
                         | _ -> failwith "invariant"
                Map.add id next classes
        | _ -> classes
module EnrollmentRegister =
    let apply records event =
        match event with
        | Event.StudentEnrolled(id,student,course) -> Map.add (id,student) {ClassId=id;StudentId=student;Course=course;Progress=Progress.Reserved} records
        | Event.StudentDropped(id,student) | Event.StudyStarted(id,student) | Event.AttendanceRecorded(id,student) | Event.ResultRecorded(id,student,_) ->
            match Map.tryFind (id,student) records with
            | None -> records
            | Some record ->
                let progress=match event with
                             | Event.StudentDropped _ -> Progress.Withdrawn StudentRequest
                             | Event.StudyStarted _ -> Progress.Studying 0
                             | Event.AttendanceRecorded _ -> match record.Progress with Progress.Studying n -> Progress.Studying(n+1) | _ -> failwith "invalid committed event"
                             | Event.ResultRecorded(_,_,score) -> if score>=60 then Progress.Passed(AssessmentScore.value(AssessmentScore.fromEvent score),if record.Course=Foundation then 1 else 2) else Progress.Failed(AssessmentScore.value(AssessmentScore.fromEvent score),BelowPassMark)
                             | _ -> failwith "invariant"
                Map.add (id,student) {record with Progress=progress} records
        | _ -> records
module Registers =
    let idOrder (s:string) = System.Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes s)
    let empty: Registers = {Students=Map.empty;Classes=Map.empty;Enrollments=Map.empty}
    let reduce state event = {Students=StudentRegister.apply state.Students event;Classes=ClassRegister.apply state.Classes event;Enrollments=EnrollmentRegister.apply state.Enrollments event}
    let fold (events: Recorded<Event> list) = List.fold (fun s e -> reduce s e.Payload) empty events
    let decisionModel state: DecisionModel =
        {Students=state.Students |> Map.values |> Seq.sortBy(fun s->idOrder s.StudentId) |> Seq.toList
         Classes=state.Classes |> Map.values |> Seq.sortBy(fun c->idOrder (Decider.planOf c).ClassId) |> Seq.toList
         Enrollments=state.Enrollments |> Map.values |> Seq.sortBy(fun e->idOrder e.ClassId,idOrder e.StudentId) |> Seq.toList}
    let list state: SchoolList =
        let model=decisionModel state
        {Students=model.Students;Classes=model.Classes;Enrollments=model.Enrollments;Summaries=model.Enrollments |> List.map(fun e -> Decider.describe e.Progress)}
