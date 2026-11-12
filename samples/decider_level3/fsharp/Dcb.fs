namespace Playground.Dcb

type Tag = { Category: string; Keys: string list }
type QueryItem = { Tags: Tag list; Types: string list }
type Query = { Items: QueryItem list }
type EventDescriptor = { EventType: string; Tags: Tag list }
type Recorded<'E> = { Position: int; CommandId: string; EventType: string; Tags: Tag list; Payload: 'E }
type ReadResult<'E> = { After: int; Events: Recorded<'E> list }
type AppendResult<'E> = Committed of Recorded<'E> list | Conflict

module Queries =
    let matches query eventType tags =
        List.isEmpty query.Items || query.Items |> List.exists (fun item ->
            (List.isEmpty item.Types || List.contains eventType item.Types) && item.Tags |> List.forall (fun t -> List.contains t tags))

type ICommandStore<'E> =
    abstract Read: Query -> ReadResult<'E>
    abstract Append: string * Query * int * 'E list -> AppendResult<'E>

// Consumers are synchronous, non-reentrant and report failure by returning false.
type EventStore<'E>(describe: 'E -> EventDescriptor, copyPayload: 'E -> 'E) =
    let gate = obj()
    let log = ResizeArray<Recorded<'E>>()
    let listeners = ResizeArray<Recorded<'E> list -> bool>()
    let mutable faults = 0
    let copy (e: Recorded<'E>) = { e with Payload = copyPayload e.Payload }
    let notify listener events = try listener events with _ -> false
    let valid (e:Recorded<'E>) = let d=describe e.Payload in d.EventType=e.EventType && d.Tags.Length=e.Tags.Length && d.Tags |> List.forall(fun t->List.contains t e.Tags)
    member _.ListenerFaults = lock gate (fun () -> faults)
    member _.Read(query) = lock gate (fun () ->
        { After = log.Count; Events = log |> Seq.filter (fun e -> Queries.matches query e.EventType e.Tags) |> Seq.map copy |> Seq.toList })
    member _.Append(commandId: string, condition, after, batch: 'E list) = lock gate (fun () ->
        if List.isEmpty batch || commandId.Length = 0 || after < 0 || after > log.Count then invalidArg "batch" "invalid append"
        if log |> Seq.exists (fun e -> e.Position > after && Queries.matches condition e.EventType e.Tags) then Conflict
        else
            let committed = batch |> List.mapi (fun i payload ->
                let copy=copyPayload payload
                let e=describe copy
                if e.EventType.Length = 0 then invalidArg "batch" "event type required"
                { Position = log.Count+i+1; CommandId = commandId; EventType = e.EventType; Tags = e.Tags; Payload = copy })
            log.AddRange committed
            for listener in listeners.ToArray() do
                if not (notify listener (committed |> List.map copy)) then
                    listeners.Remove listener |> ignore
                    faults <- faults + 1
            Committed(committed |> List.map copy))
    member _.Subscribe(listener) = lock gate (fun () ->
        if not (notify listener (log |> Seq.map copy |> Seq.toList)) then failwith "subscription replay failed"
        listeners.Add listener)
    member _.Load(events: Recorded<'E> list) = lock gate (fun () ->
        if log.Count <> 0 || listeners.Count <> 0 then failwith "load only into an empty store"
        if events |> List.mapi (fun i e -> e.Position <> i+1 || e.CommandId.Length = 0 || e.EventType.Length = 0 || not(valid e)) |> List.exists id then invalidArg "events" "invalid imported log"
        let copied = events |> List.map copy
        log.AddRange copied)

    interface ICommandStore<'E> with
        member this.Read query = this.Read query
        member this.Append(id,query,after,batch) = this.Append(id,query,after,batch)

type LiveProjection<'E,'S>(initial: 'S, reduce: 'S -> 'E -> 'S, copyState: 'S -> 'S) =
    let gate = obj()
    let mutable state, checkpoint = initial, 0
    member _.Accept(batch: Recorded<'E> list) = lock gate (fun () ->
        let mutable next, position, valid = copyState state, checkpoint, true
        for e in batch do
            if valid && e.Position > position then
                if e.Position <> position+1 then valid <- false
                else next <- reduce next e.Payload; position <- e.Position
        if valid then state <- next; checkpoint <- position
        valid)
    member _.Snapshot() = lock gate (fun () -> checkpoint, copyState state)

type Preparation<'E,'R> = Emit of 'E list | Reject of 'R
type Ticket<'C,'E,'R> = { CommandId: string; Command: 'C; Query: Query; After: int; Returned:int; Decision: Preparation<'E,'R> }
type HandlingResult<'R> = { Status: string; Conflicts: int; Appended: int; Position: int; Errors: 'R list }
type CommandProcessor<'C,'E,'R>(store: ICommandStore<'E>, queryFor: 'C -> Query, decide: Recorded<'E> list -> 'C -> Preparation<'E,'R>, maxConflicts: int) =
    member _.Prepare(commandId, command) =
        let query = queryFor command
        let read = store.Read query
        { CommandId=commandId; Command=command; Query=query; After=read.After; Returned=read.Events.Length; Decision=decide read.Events command }
    member this.Commit(ticket) =
        let rec attempt ticket conflicts =
            match ticket.Decision with
            | Reject error -> { Status="Rejected"; Conflicts=conflicts; Appended=0; Position=ticket.After; Errors=[error] }
            | Emit events ->
                match store.Append(ticket.CommandId,ticket.Query,ticket.After,events) with
                | Committed committed -> { Status="Committed"; Conflicts=conflicts; Appended=committed.Length; Position=(List.last committed).Position; Errors=[] }
                | Conflict when conflicts+1 >= maxConflicts -> { Status="RetryExhausted"; Conflicts=conflicts+1; Appended=0; Position=ticket.After; Errors=[] }
                | Conflict -> attempt (this.Prepare(ticket.CommandId,ticket.Command)) (conflicts+1)
        attempt ticket 0

// Function records keep the domain handler separate from the generic executor.
type CommandHandler<'C,'E,'R> = {
    Validate: 'C -> 'R list
    QueryFor: 'C -> Query
    Handle: Recorded<'E> list -> 'C -> Preparation<'E,'R> }
type PreparedCommand<'R> = {
    Query: Query; After: int; Returned: int; WasRead: bool
    Commit: unit -> HandlingResult<'R> }
type IDcbExecutor<'E,'R> =
    abstract Prepare<'C>: string * 'C * CommandHandler<'C,'E,'R> -> PreparedCommand<'R>
    abstract Execute<'C>: string * 'C * CommandHandler<'C,'E,'R> -> HandlingResult<'R>
type DcbExecutor<'E,'R>(store:ICommandStore<'E>,maxConflicts:int) =
    member _.Prepare(id,command,handler:CommandHandler<'C,'E,'R>) =
        if System.String.IsNullOrWhiteSpace id then invalidArg "id" "command id required"
        let errors=handler.Validate command
        if not(List.isEmpty errors) then
            {Query={Items=[]};After=0;Returned=0;WasRead=false
             Commit=fun () -> {Status="Rejected";Conflicts=0;Appended=0;Position=0;Errors=errors}}
        else
            let processor=CommandProcessor(store,handler.QueryFor,handler.Handle,maxConflicts)
            let ticket=processor.Prepare(id,command)
            {Query=ticket.Query;After=ticket.After;Returned=ticket.Returned;WasRead=true;Commit=fun () -> processor.Commit ticket}
    member this.Execute(id,command,handler) = (this.Prepare(id,command,handler)).Commit()
    interface IDcbExecutor<'E,'R> with
        member this.Prepare(id,command,handler) = this.Prepare(id,command,handler)
        member this.Execute(id,command,handler) = this.Execute(id,command,handler)

// Rows are immutable; a failed batch discards only its staged changes.
type IndexBatch<'K,'V when 'K:equality and 'K : not null>(rows:System.Collections.Generic.Dictionary<'K,'V>) =
    let changes=System.Collections.Generic.Dictionary<'K,'V option>()
    let mutable work=0L
    member _.Changes=changes
    member _.Work=work
    member _.TryGet key =
        work<-work+1L
        match changes.TryGetValue key with
        | true,value -> value
        | _ -> match rows.TryGetValue key with true,value->Some value | _->None
    member this.Get key=this.TryGet key |> Option.defaultWith(fun ()->failwith "missing index row")
    member _.Put(key,value)=work<-work+1L;changes[key]<-Some value
    member _.Delete key=work<-work+1L;changes[key]<-None
type IndexedProjection<'K,'V,'E when 'K:equality and 'K : not null>(reduce:IndexBatch<'K,'V> -> Recorded<'E> -> unit) =
    let gate=obj()
    let rows=System.Collections.Generic.Dictionary<'K,'V>()
    let mutable checkpoint,work=0,0L
    member _.Checkpoint=lock gate (fun ()->checkpoint)
    member _.RowCount=lock gate (fun ()->rows.Count)
    member _.Work=lock gate (fun ()->work)
    member _.Accept(events:Recorded<'E> list)=lock gate (fun ()->
        let batch=IndexBatch(rows)
        let mutable position=checkpoint
        let valid=
            try
                for e in events do
                    if e.Position>position then
                        if e.Position<>position+1 then failwith "position gap"
                        reduce batch e;position<-e.Position
                true
            with _->false
        if valid then
            for pair in batch.Changes do
                match pair.Value with Some value->rows[pair.Key]<-value | None->rows.Remove pair.Key |> ignore
            checkpoint<-position;work<-work+batch.Work
        valid)
    member _.Read(query:System.Collections.Generic.IReadOnlyDictionary<'K,'V> -> 'T)=lock gate (fun ()->checkpoint,query rows)
