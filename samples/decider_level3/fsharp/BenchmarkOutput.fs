namespace SchoolDomain
open System.Text.Json
module BenchmarkOutput=
    let serialize (o:RunOutput) (options:JsonSerializerOptions)=
        let steps=o.Steps |> Seq.countBy(fun s->s.Status) |> dict
        let errors=o.Steps |> Seq.collect(fun s->s.Errors) |> Seq.countBy string |> dict
        JsonSerializer.Serialize({|List=o.List;RebuildMatches=(JsonSerializer.Serialize(o.List,options)=JsonSerializer.Serialize(o.Rebuilt,options));EventCount=o.Events.Length;Checkpoint=o.Checkpoint;ListenerFaults=o.ListenerFaults;StepCounts=steps;Errors=errors;Conflicts=(o.Steps |> List.sumBy(fun s->s.Conflicts))|},options)
