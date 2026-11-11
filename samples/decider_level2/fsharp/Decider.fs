namespace EnrollmentDomain

module Decider =
    let empty: State = { Students = []; Classes = []; Enrollments = [] }
    let planOf = function
        | ClassState.Available(p, _) | ClassState.Full(p, _)
        | ClassState.Teaching(p, _, _) | ClassState.Completed(p, _) -> p
    let members = function
        | ClassState.Available(_, s) | ClassState.Full(_, s) | ClassState.Teaching(_, s, _) -> s
        | ClassState.Completed(_, results) -> results |> List.map (fun r -> r.StudentId)
    let describe = function
        | Progress.Reserved -> "reserved"
        | Progress.Studying attendance -> $"studying:{attendance}"
        | Progress.Passed(score, credits) -> $"passed:{score}:{credits}"
        | Progress.Failed(score, reason) -> $"failed:{score}:{reason}"
        | Progress.Withdrawn reason -> $"withdrawn:{reason}"
    let rank = function Year1 -> 1 | Year2 -> 2 | Year3 -> 3
    let accept event = Decision.Accepted [event]
    let reject code = Decision.Rejected code
    let isRecruiting = function ClassState.Available _ | ClassState.Full _ -> true | ClassState.Teaching _ | ClassState.Completed _ -> false
    let isPassed = function Progress.Passed _ -> true | Progress.Reserved | Progress.Studying _ | Progress.Failed _ | Progress.Withdrawn _ -> false
    let isStudying = function Progress.Studying _ -> true | Progress.Reserved | Progress.Passed _ | Progress.Failed _ | Progress.Withdrawn _ -> false
    let decideClass (state: State) command id =
        match state.Classes |> List.tryFind (fun c -> (planOf c).ClassId = id) with
        | None -> reject UnknownClass
        | Some room ->
            let plan = planOf room
            match command with
            | Command.Enroll(_, studentId) ->
                match state.Students |> List.tryFind (fun s -> s.StudentId = studentId) with
                | None -> reject UnknownStudent
                | Some student ->
                    let eligible = match plan.Prerequisite with
                                   | Prerequisite.NoPrerequisite -> true
                                   | Prerequisite.PassedCourse course -> state.Enrollments |> List.exists (fun e -> e.StudentId = studentId && e.Course = course && isPassed e.Progress)
                    if not (isRecruiting room) then reject RegistrationClosed
                    elif List.contains studentId (members room) then reject AlreadyEnrolled
                    elif rank student.Grade < rank plan.MinimumGrade then reject GradeTooLow
                    elif not eligible then reject PrerequisiteMissing
                    else match room with
                         | ClassState.Full _ -> reject ClassFull
                         | ClassState.Available _ -> accept (Event.StudentEnrolled(id, studentId))
                         | ClassState.Teaching _ | ClassState.Completed _ -> failwith "invariant"
            | Command.Drop(_, studentId) ->
                if not (isRecruiting room) then reject RegistrationClosed
                elif List.contains studentId (members room) then accept (Event.StudentDropped(id, studentId))
                else reject NotEnrolled
            | Command.StartClass(_, startedOn) ->
                if not (isRecruiting room) then reject RegistrationClosed
                elif startedOn.Length = 0 then reject InvalidDate
                else accept (Event.ClassStarted(id, startedOn))
            | Command.Attend(_, studentId) | Command.Assess(_, studentId, _) ->
                match room with
                | ClassState.Available _ | ClassState.Full _ | ClassState.Completed _ -> reject NotTeaching
                | ClassState.Teaching _ ->
                    if not (List.contains studentId (members room)) then reject NotEnrolled
                    else match state.Enrollments |> List.tryFind (fun e -> e.ClassId = id && e.StudentId = studentId) with
                         | None -> failwith "invariant"
                         | Some record ->
                             match record.Progress, command with
                             | Progress.Studying _, Command.Attend _ -> accept (Event.AttendanceRecorded(id, studentId))
                             | Progress.Studying attendance, Command.Assess(_, _, score) ->
                                 if score < 0 || score > 100 then reject InvalidScore
                                 elif attendance = 0 then reject NoAttendance
                                 else accept (Event.ResultRecorded(id, studentId, score))
                             | _ -> reject AlreadyAssessed
            | Command.CompleteClass _ ->
                match room with
                | ClassState.Teaching _ ->
                    if state.Enrollments |> List.exists (fun e -> e.ClassId = id && isStudying e.Progress) then reject UnassessedStudents
                    else accept (Event.ClassCompleted id)
                | ClassState.Available _ | ClassState.Full _ | ClassState.Completed _ -> reject NotTeaching
            | Command.RegisterStudent _ | Command.PromoteStudent _ | Command.OpenClass _ -> failwith "invariant"
    let decide (state: State) = function
        | Command.RegisterStudent(id, grade) ->
            if id.Length = 0 then reject InvalidId
            elif state.Students |> List.exists (fun s -> s.StudentId = id) then reject DuplicateStudent
            else accept (Event.StudentRegistered(id, grade))
        | Command.PromoteStudent(id, grade) ->
            match state.Students |> List.tryFind (fun s -> s.StudentId = id) with
            | None -> reject UnknownStudent
            | Some student -> if rank grade = rank student.Grade + 1 then accept (Event.StudentPromoted(id, grade)) else reject InvalidPromotion
        | Command.OpenClass(id, course, capacity) ->
            if id.Length = 0 then reject InvalidId
            elif capacity < 1 || capacity > 10 then reject InvalidCapacity
            elif state.Classes |> List.exists (fun c -> (planOf c).ClassId = id) then reject DuplicateClass
            else accept (Event.ClassOpened(id, course, capacity))
        | (Command.Enroll(id, _) | Command.Drop(id, _) | Command.StartClass(id, _)
          | Command.Attend(id, _) | Command.Assess(id, _, _) | Command.CompleteClass id) as command -> decideClass state command id
    let cause = function
        | Event.StudentRegistered(id, g) -> Command.RegisterStudent(id, g)
        | Event.StudentPromoted(id, g) -> Command.PromoteStudent(id, g)
        | Event.ClassOpened(id, c, n) -> Command.OpenClass(id, c, n)
        | Event.StudentEnrolled(id, s) -> Command.Enroll(id, s)
        | Event.StudentDropped(id, s) -> Command.Drop(id, s)
        | Event.ClassStarted(id, d) -> Command.StartClass(id, d)
        | Event.AttendanceRecorded(id, s) -> Command.Attend(id, s)
        | Event.ResultRecorded(id, s, n) -> Command.Assess(id, s, n)
        | Event.ClassCompleted id -> Command.CompleteClass id
    let applyClass (state: State) event id =
        let room = state.Classes |> List.find (fun c -> (planOf c).ClassId = id)
        let plan = planOf room
        let change student progress = state.Enrollments |> List.map (fun e -> if e.ClassId = id && e.StudentId = student then { e with Progress = progress e.Progress } else e)
        let updated, records =
            match event with
            | Event.StudentEnrolled(_, student) ->
                let students = members room @ [student]
                let updated = if students.Length = plan.Capacity then ClassState.Full(plan, students) else ClassState.Available(plan, students)
                let record = { ClassId = id; StudentId = student; Course = plan.Course; Progress = Progress.Reserved }
                updated, (state.Enrollments |> List.filter (fun e -> e.ClassId <> id || e.StudentId <> student)) @ [record]
            | Event.StudentDropped(_, student) ->
                ClassState.Available(plan, members room |> List.filter ((<>) student)), change student (fun _ -> Progress.Withdrawn StudentRequest)
            | Event.ClassStarted(_, date) ->
                ClassState.Teaching(plan, members room, date), (state.Enrollments |> List.map (fun e ->
                    if e.ClassId = id && e.Progress = Progress.Reserved then { e with Progress = Progress.Studying 0 } else e))
            | Event.AttendanceRecorded(_, student) ->
                room, change student (function Progress.Studying n -> Progress.Studying(n + 1) | _ -> failwith "invariant")
            | Event.ResultRecorded(_, student, score) ->
                let result = if score >= 60 then Progress.Passed(score, if plan.Course = Foundation then 1 else 2) else Progress.Failed(score, BelowPassMark)
                room, change student (fun _ -> result)
            | Event.ClassCompleted _ ->
                let results: ClassResult list = state.Enrollments |> List.filter (fun e -> e.ClassId = id && (match e.Progress with Progress.Passed _ | Progress.Failed _ -> true | _ -> false))
                                               |> List.map (fun e -> { StudentId = e.StudentId; Progress = e.Progress })
                ClassState.Completed(plan, results), state.Enrollments
            | Event.StudentRegistered _ | Event.StudentPromoted _ | Event.ClassOpened _ -> failwith "invariant"
        { state with Classes = state.Classes |> List.map (fun c -> if (planOf c).ClassId = id then updated else c); Enrollments = records }
    let evolve (state: State) event =
        match decide state (cause event) with
        | Decision.Rejected _ -> failwith "InvalidHistory"
        | Decision.Accepted _ ->
            match event with
            | Event.StudentRegistered(id, grade) -> { state with Students = state.Students @ [{ StudentId = id; Grade = grade }] }
            | Event.StudentPromoted(id, grade) -> { state with Students = state.Students |> List.map (fun s -> if s.StudentId = id then { s with Grade = grade } else s) }
            | Event.ClassOpened(id, course, capacity) ->
                let plan = { ClassId = id; Course = course; Capacity = capacity; MinimumGrade = (if course = Foundation then Year2 else Year3)
                             Prerequisite = (if course = Foundation then Prerequisite.NoPrerequisite else Prerequisite.PassedCourse Foundation) }
                { state with Classes = state.Classes @ [ClassState.Available(plan, [])] }
            | Event.StudentEnrolled(id, _) | Event.StudentDropped(id, _) | Event.ClassStarted(id, _)
            | Event.AttendanceRecorded(id, _) | Event.ResultRecorded(id, _, _) | Event.ClassCompleted id -> applyClass state event id
    let replay events = List.fold evolve empty events
