namespace SchoolDomain
open Playground.Dcb
module Routing =
    let student id: Tag = {Category="student";Keys=[id]}
    let classroom id: Tag = {Category="class";Keys=[id]}
    let enrollment room student: Tag = {Category="enrollment";Keys=[room;student]}
    let one tags: Query = {Items=tags |> List.map(fun t -> {Tags=[t];Types=[]})}
    let queryFor = function
        | Command.RegisterStudent(studentId,grade) -> one [student studentId]
        | Command.PromoteStudent(studentId,grade) -> one [student studentId]
        | Command.OpenClass(classId,course,capacity) -> one [classroom classId]
        | Command.Enroll(classId,studentId) -> one [student studentId;classroom classId]
        | Command.Drop(classId,studentId) -> one [classroom classId]
        | Command.StartClass(classId,startedOn) -> one [classroom classId]
        | Command.Attend(classId,studentId) -> one [classroom classId]
        | Command.Assess(classId,studentId,score) -> one [classroom classId]
        | Command.CompleteClass(classId) -> one [classroom classId]
    let describe event: EventDescriptor =
        let eventType,tags=
            match event with
            | Event.StudentRegistered(studentId,grade) -> "StudentRegistered",[student studentId]
            | Event.StudentPromoted(studentId,grade) -> "StudentPromoted",[student studentId]
            | Event.ClassOpened(classId,course,capacity) -> "ClassOpened",[classroom classId]
            | Event.StudentEnrolled(classId,studentId,course) -> "StudentEnrolled",[student studentId;classroom classId;enrollment classId studentId]
            | Event.StudentDropped(classId,studentId) -> "StudentDropped",[student studentId;classroom classId;enrollment classId studentId]
            | Event.ClassStarted(classId,startedOn) -> "ClassStarted",[classroom classId]
            | Event.AttendanceRecorded(classId,studentId) -> "AttendanceRecorded",[student studentId;classroom classId;enrollment classId studentId]
            | Event.ResultRecorded(classId,studentId,score) -> "ResultRecorded",[student studentId;classroom classId;enrollment classId studentId]
            | Event.ClassCompleted(classId,results) -> "ClassCompleted",[classroom classId]
            | Event.StudyStarted(classId,studentId) -> "StudyStarted",[student studentId;classroom classId;enrollment classId studentId]
        {EventType=eventType;Tags=tags}
    let decide events command =
        match Decider.decide (Registers.decisionModel (Registers.fold events)) command with
        | Decision.Accepted events -> Emit events
        | Decision.Rejected code -> Reject code
