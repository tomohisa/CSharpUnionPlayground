namespace SchoolDomain
open System
open System.Globalization

type StudentId = private StudentId of string
type ClassId = private ClassId of string
type SeatCapacity = private SeatCapacity of int
type AssessmentScore = private AssessmentScore of int
type SchoolDate = private SchoolDate of string
module StudentId =
    let fromEvent value=StudentId value
    let value (StudentId value)=value
    let create value=if String.IsNullOrWhiteSpace value then Error Rejection.InvalidId else Ok(StudentId value)
module ClassId =
    let fromEvent value=ClassId value
    let value (ClassId value)=value
    let create value=if String.IsNullOrWhiteSpace value then Error Rejection.InvalidId else Ok(ClassId value)
module SeatCapacity =
    let fromEvent value=SeatCapacity value
    let value (SeatCapacity value)=value
    let create value=if value<1 || value>10 then Error Rejection.InvalidCapacity else Ok(SeatCapacity value)
module AssessmentScore =
    let fromEvent value=AssessmentScore value
    let value (AssessmentScore value)=value
    let create value=if value<0 || value>100 then Error Rejection.InvalidScore else Ok(AssessmentScore value)
module SchoolDate =
    let fromEvent value=SchoolDate value
    let value (SchoolDate value)=value
    let create value=
        match DateOnly.TryParseExact(value,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None) with
        | true,date -> Ok(SchoolDate value)
        | _ -> Error Rejection.InvalidDate

type ValidatedCommand = private ValidatedCommand of Command
module ValidatedCommand =
    let value (ValidatedCommand command)=command
    let create command =
        let checks =
            match command with
            | Command.RegisterStudent(id,_) | Command.PromoteStudent(id,_) -> [StudentId.create id |> Result.map ignore]
            | Command.OpenClass(id,_,capacity) -> [ClassId.create id |> Result.map ignore;SeatCapacity.create capacity |> Result.map ignore]
            | Command.StartClass(id,date) -> [ClassId.create id |> Result.map ignore;SchoolDate.create date |> Result.map ignore]
            | Command.Assess(c,s,score) -> [ClassId.create c |> Result.map ignore;StudentId.create s |> Result.map ignore;AssessmentScore.create score |> Result.map ignore]
            | Command.Enroll(c,s) | Command.Drop(c,s) | Command.Attend(c,s) -> [ClassId.create c |> Result.map ignore;StudentId.create s |> Result.map ignore]
            | Command.CompleteClass id -> [ClassId.create id |> Result.map ignore]
        match checks |> List.tryPick(function Error e->Some e | Ok _->None) with
        | Some error -> Error error
        | None -> Ok(ValidatedCommand command)
    let validate command = match create command with Ok _->[] | Error e->[e]
