namespace SchoolDomain
open Playground.Dcb
open System
open System.Text.Json
open System.Diagnostics
module QueryBenchmark=
    let run (store:JsonEventStore<Event>) (projection:SchoolProjection)=
        match Int32.TryParse(Environment.GetEnvironmentVariable("DECIDER_STUDENTS")) with
        | true,n when n>0 ->
            let clock=Stopwatch.StartNew()
            for i in 0..999 do
                let id=sprintf "student-%08d" ((int64 i*7919L) % int64 n |> int)
                if (store.Read {Items=[{Tags=[Routing.student id];Types=[]}]}).Events.IsEmpty then failwith "point read missing"
            Console.Error.WriteLine($"PHASE tagged_point {clock.Elapsed.TotalSeconds:R}")
            clock.Restart()
            let options=JsonSerializerOptions(PropertyNamingPolicy=JsonNamingPolicy.CamelCase)
            for i in 0..999 do
                let id=sprintf "student-%08d" ((int64 i*7919L) % int64 n |> int)
                let json=JsonSerializer.Serialize(projection.Student id,options)
                if (JsonSerializer.Deserialize<Student>(json,options)|>nonNull).StudentId<>id then failwith "point query mismatch"
            Console.Error.WriteLine($"PHASE projection_point_json {clock.Elapsed.TotalSeconds:R}")
        | _->()
