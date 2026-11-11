namespace EnrollmentDomain
open System
open System.Text.Json

type Input = { Commands: Command list; History: Event list }
type Output = { Initial: State; State: State; Events: Event list; Steps: Decision list; Restored: State; Replayed: State; Summaries: string list }

module Program =
    let options = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase, AllowOutOfOrderMetadataProperties = true,
                                       RespectRequiredConstructorParameters = true)
    // The standard DU reader can accept JSON null: reject it at the boundary.
    let rec containsNull (value: JsonElement) =
        match value.ValueKind with
        | JsonValueKind.Null -> true
        | JsonValueKind.Array -> value.EnumerateArray() |> Seq.exists containsNull
        | JsonValueKind.Object -> value.EnumerateObject() |> Seq.exists (fun p -> containsNull p.Value)
        | _ -> false
    let readInput text =
        use document = JsonDocument.Parse(text: string)
        if containsNull document.RootElement then raise (JsonException("null is outside the input contract"))
        JsonSerializer.Deserialize<Input>(text, options) |> nonNull
    [<EntryPoint>]
    let main _ =
        try
            let input = readInput (Console.In.ReadToEnd())
            let initial = Decider.replay input.History
            let state, history, steps =
                input.Commands |> List.fold (fun (state, history, steps) command ->
                    let decision = Decider.decide state command
                    match decision with
                    | Decision.Accepted events -> List.fold Decider.evolve state events, history @ events, steps @ [decision]
                    | Decision.Rejected _ -> state, history, steps @ [decision]) (initial, input.History, [])
            let events = JsonSerializer.Deserialize<Event list>(JsonSerializer.Serialize(history, options), options) |> nonNull
            let restored = JsonSerializer.Deserialize<State>(JsonSerializer.Serialize(state, options), options) |> nonNull
            Console.WriteLine(JsonSerializer.Serialize({ Initial = initial; State = state; Events = events; Steps = steps;
                                                        Restored = restored; Replayed = Decider.replay events; Summaries = state.Enrollments |> List.map (fun e -> Decider.describe e.Progress) }, options))
            0
        with error -> Console.Error.WriteLine(error.Message); 1
