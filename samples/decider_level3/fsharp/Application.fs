namespace SchoolDomain
open Playground.Dcb
open System
open System.Text.Json

type ReadAudit = {Query:Query;After:int;Returned:int}
type RunOutput = {Events:Recorded<Event> list;Steps:Trace list;Lists:SchoolList list;List:SchoolList;Rebuilt:SchoolList;Checkpoint:int;Reads:ReadAudit list;ListenerFaults:int}
module Application =
    let run history operations =
        let options=JsonSerializerOptions(PropertyNamingPolicy=JsonNamingPolicy.CamelCase,AllowOutOfOrderMetadataProperties=true,RespectRequiredConstructorParameters=true)
        let journal:IJsonJournal=
            match Environment.GetEnvironmentVariable("DECIDER_PERSISTENT") with
            | null -> new MemoryJsonJournal()
            | path -> new SqliteJournal(path,"FULL")
        use store=new JsonEventStore<Event>(journal,Routing.describe,(fun e->JsonSerializer.Serialize(e,options)),(fun json->JsonSerializer.Deserialize<Event>(json,options) |> nonNull))
        if not (List.isEmpty history) then store.Load history
        if Environment.GetEnvironmentVariable("DECIDER_BENCHMARK")="1" then Console.Error.WriteLine("STORE "+store.EngineVersion)
        let projection=SchoolProjection()
        store.Subscribe projection.Accept
        let executor:IDcbExecutor<Event,Rejection>=DcbExecutor<Event,Rejection>(store,3)
        let mutable tickets=Map.empty<string,PreparedCommand<Rejection>>
        let steps,lists,reads=ResizeArray<Trace>(),ResizeArray<SchoolList>(),ResizeArray<ReadAudit>()
        let prepare id command =
            let ticket=executor.Prepare(id,command,Handlers.school)
            if ticket.WasRead then reads.Add {Query=ticket.Query;After=ticket.After;Returned=ticket.Returned}
            ticket
        let commit op (ticket:PreparedCommand<Rejection>) =
            let r=ticket.Commit()
            steps.Add {Operation=op;Status=r.Status;Conflicts=r.Conflicts;Appended=r.Appended;Position=r.Position;Errors=r.Errors}
        let bench=Environment.GetEnvironmentVariable("DECIDER_BENCHMARK")="1"
        let clock=System.Diagnostics.Stopwatch.StartNew()
        if bench then Console.Error.WriteLine("READY")
        for operation in operations do
            match operation with
            | Operation.Execute(id,c) -> commit "Execute" (prepare id c)
            | Operation.Prepare(key,id,c) ->
                if Map.containsKey key tickets then invalidArg "key" "duplicate ticket"
                let t=prepare id c
                tickets<-Map.add key t tickets
                steps.Add {Operation="Prepare";Status="Prepared";Conflicts=0;Appended=0;Position=t.After;Errors=[]}
            | Operation.Commit key ->
                let t=Map.find key tickets
                tickets<-Map.remove key tickets
                commit "Commit" t
            | Operation.ListAll ->
                store.Refresh()
                if store.ListenerFaults<>0 then failwith "projection unavailable; rebuild required"
                let position,state=projection.Snapshot()
                lists.Add(state)
                steps.Add {Operation="ListAll";Status="Listed";Conflicts=0;Appended=0;Position=position;Errors=[]}
            | Operation.Rebuild ->
                let fresh=SchoolProjection()
                if not (fresh.Accept (store.Read {Items=[]}).Events) then failwith "rebuild failed"
                let position,state=fresh.Snapshot()
                lists.Add(state)
                steps.Add {Operation="Rebuild";Status="Rebuilt";Conflicts=0;Appended=0;Position=position;Errors=[]}
            | Operation.Redeliver ->
                if not (projection.Accept (store.Read {Items=[]}).Events) then failwith "duplicate delivery failed"
                let position=projection.Checkpoint
                steps.Add {Operation="Redeliver";Status="Redelivered";Conflicts=0;Appended=0;Position=position;Errors=[]}
        if bench then Console.Error.WriteLine($"PHASE operations {clock.Elapsed.TotalSeconds:R}")
        clock.Restart()
        store.Refresh()
        QueryBenchmark.run store projection
        clock.Restart()
        let events=(store.Read {Items=[]}).Events
        let fresh=SchoolProjection()
        if not(fresh.Accept events) then failwith "rebuild failed"
        if store.ListenerFaults<>0 then failwith "projection unavailable; rebuild required"
        let checkpoint,state=projection.Snapshot()
        let _,rebuilt=fresh.Snapshot()
        if bench then
            Console.Error.WriteLine($"PHASE read_rebuild_snapshot {clock.Elapsed.TotalSeconds:R}")
            Console.Error.WriteLine($"CODEC {store.Encoded} {store.Decoded} {store.EncodedBytes} {store.DecodedBytes}")
        StorageBenchmark.run()
        {Events=events;Steps=List.ofSeq steps;Lists=List.ofSeq lists;List=state;Rebuilt=rebuilt;Checkpoint=checkpoint;Reads=List.ofSeq reads;ListenerFaults=store.ListenerFaults}
