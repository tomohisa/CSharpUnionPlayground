namespace SchoolDomain

module Decider =
    let empty: DecisionModel = { Students = []; Classes = []; Enrollments = [] }
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
    let decideClass (state: DecisionModel) command id =
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
                         | ClassState.Available _ -> accept (Event.StudentEnrolled(id, studentId, plan.Course))
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
                    else accept (Event.ClassCompleted(id, state.Enrollments |> List.filter (fun e -> e.ClassId = id && (match e.Progress with Progress.Passed _ | Progress.Failed _ -> true | _ -> false)) |> List.map (fun e -> { StudentId = e.StudentId; Progress = e.Progress })))
                | ClassState.Available _ | ClassState.Full _ | ClassState.Completed _ -> reject NotTeaching
            | Command.RegisterStudent _ | Command.PromoteStudent _ | Command.OpenClass _ -> failwith "invariant"
    let decideOne (state: DecisionModel) = function
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
    let decide state validated =
        let command=ValidatedCommand.value validated
        match decideOne state command with
        | Decision.Accepted [Event.ClassStarted(id, date)] ->
            Decision.Accepted(Event.ClassStarted(id, date) :: (state.Enrollments |> List.filter (fun e -> e.ClassId = id && e.Progress = Progress.Reserved) |> List.map (fun e -> Event.StudyStarted(id, e.StudentId))))
        | decision -> decision
