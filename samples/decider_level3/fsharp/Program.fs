namespace SchoolDomain
open System
open System.Text.Json
open Playground.Dcb

type Input = {History:Recorded<Event> list;Operations:Operation list}
module Program =
    let options=JsonSerializerOptions(PropertyNamingPolicy=JsonNamingPolicy.CamelCase,AllowOutOfOrderMetadataProperties=true,RespectRequiredConstructorParameters=true)
    let rec containsNull (e:JsonElement) =
        match e.ValueKind with
        | JsonValueKind.Null -> true
        | JsonValueKind.Array -> e.EnumerateArray() |> Seq.exists containsNull
        | JsonValueKind.Object -> e.EnumerateObject() |> Seq.exists(fun p -> containsNull p.Value)
        | _ -> false
    [<EntryPoint>]
    let main args =
        try
            if args.Length=2 && args[0]="--projection-scale" then
                ProjectionTests.run (int args[1])
                0
            elif Array.contains "--domain-test" args then
                DomainTests.run()
                0
            elif Array.contains "--self-test" args then
                LibraryTests.run()
                Console.WriteLine("{\"library\":\"PASS\"}")
                0
            else
                let text=Console.In.ReadToEnd()
                use doc=JsonDocument.Parse text
                if containsNull doc.RootElement then failwith "null is outside the input contract"
                let input=JsonSerializer.Deserialize<Input>(text,options) |> nonNull
                let output=Application.run input.History input.Operations
                Console.WriteLine(if Environment.GetEnvironmentVariable("DECIDER_BENCHMARK_SUMMARY")="1" then BenchmarkOutput.serialize output options else JsonSerializer.Serialize(output,options))
                0
        with error -> Console.Error.WriteLine(error.Message);1
