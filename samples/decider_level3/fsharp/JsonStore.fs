namespace Playground.Dcb
open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Text.Json

type JsonEvent = {Position:int;CommandId:string;EventType:string;PayloadJson:string;TagsJson:string}
type JsonRead = {After:int;Events:JsonEvent list}
type JsonAppend = RawCommitted of JsonEvent list | RawConflict
type IJsonJournal =
    inherit IDisposable
    abstract EngineVersion:string
    abstract Read:Query*int -> JsonRead
    abstract Append:string*Query*int*JsonEvent list -> JsonAppend
    abstract Load:JsonEvent list -> unit
module JournalMetadata =
    let options=JsonSerializerOptions(PropertyNamingPolicy=JsonNamingPolicy.CamelCase,RespectRequiredConstructorParameters=true)
    let encode (tags:Tag list)=JsonSerializer.Serialize(tags,options)
    let decode (json:string)=JsonSerializer.Deserialize<Tag list>(json,options) |> nonNull
    let keys (keys:string list)=
        use output=new MemoryStream()
        for key in keys do
            let bytes=Encoding.UTF8.GetBytes key
            let size=BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder bytes.Length)
            output.Write(size);output.Write(bytes)
        output.ToArray()
    let indexKey (tag:Tag)=keys(tag.Category::tag.Keys) |> Convert.ToHexString
    let validate json=
        let keys=HashSet<string>(StringComparer.Ordinal)
        for tag in decode json do if not(keys.Add(indexKey tag)) then invalidArg "tags" "duplicate stored tag"
type MemoryJsonJournal() =
    let gate=obj()
    let log=ResizeArray<JsonEvent>()
    let tags,types=Dictionary<string,ResizeArray<int>>(StringComparer.Ordinal),Dictionary<string,ResizeArray<int>>(StringComparer.Ordinal)
    let candidates (query:Query) after high =
        if query.Items.IsEmpty || query.Items |> List.exists(fun i->i.Tags.IsEmpty && i.Types.IsEmpty) then [after+1..high]
        else
            let all=HashSet<int>()
            let bucket (table:Dictionary<string,ResizeArray<int>>) key=match table.TryGetValue key with true,v->Seq.toList v | _->[]
            for item in query.Items do
                let sets=[for tag in item.Tags do yield bucket tags (JournalMetadata.indexKey tag)
                          if not item.Types.IsEmpty then yield item.Types |> List.collect(bucket types)]
                match sets with
                | first::rest ->
                    let selected=HashSet<int>(first)
                    for next in rest do selected.IntersectWith next
                    all.UnionWith selected
                | [] -> ()
            all |> Seq.filter(fun p->p>after && p<=high) |> Seq.sort |> Seq.toList
    let insert row=
        log.Add row
        let add (table:Dictionary<string,ResizeArray<int>>) key=
            let bucket=match table.TryGetValue key with true,v->v | _->let v=ResizeArray<int>() in table.Add(key,v);v
            bucket.Add row.Position
        add types row.EventType
        for tag in JournalMetadata.decode row.TagsJson do add tags (JournalMetadata.indexKey tag)
    interface IJsonJournal with
        member _.EngineVersion="memory-json-v1"
        member _.Read(query,after)=lock gate (fun ()->
            if after<0 || after>log.Count then invalidArg "after" "invalid position"
            {After=log.Count;Events=candidates query after log.Count |> List.map(fun p->log[p-1])})
        member _.Append(id,query,after,events)=lock gate (fun ()->
            if List.isEmpty events || String.IsNullOrEmpty id || after<0 || after>log.Count then invalidArg "events" "invalid append"
            if not(List.isEmpty(candidates query after log.Count)) then RawConflict
            else
                let rows=events |> List.mapi(fun i e->{e with Position=log.Count+i+1;CommandId=id})
                for row in rows do JournalMetadata.validate row.TagsJson
                for row in rows do insert row
                RawCommitted rows)
        member _.Load events=lock gate (fun ()->
            if log.Count<>0 then invalidOp "load requires empty journal"
            if events |> List.mapi(fun i e->e.Position<>i+1) |> List.exists id then invalidArg "events" "invalid imported position"
            for e in events do JournalMetadata.validate e.TagsJson
            for e in events do insert e)
        member _.Dispose()=()
type JsonEventStore<'E>(journal:IJsonJournal,describe:'E->EventDescriptor,encode:'E->string,decode:string->'E) =
    let gate=obj()
    let listeners=ResizeArray<Recorded<'E> list->bool>()
    let mutable published,faults=0,0
    let mutable encoded,decoded,encodedBytes,decodedBytes=0L,0L,0L,0L
    let encodeEvent id value=
        let descriptor=describe value
        if String.IsNullOrEmpty descriptor.EventType then invalidArg "event" "event type required"
        let json=encode value
        encoded<-encoded+1L;encodedBytes<-encodedBytes+int64(Encoding.UTF8.GetByteCount json)
        {Position=0;CommandId=id;EventType=descriptor.EventType;PayloadJson=json;TagsJson=JournalMetadata.encode descriptor.Tags}
    let decodeEvent e:Recorded<'E>=
        let payload=decode e.PayloadJson
        decoded<-decoded+1L;decodedBytes<-decodedBytes+int64(Encoding.UTF8.GetByteCount e.PayloadJson)
        let tags=JournalMetadata.decode e.TagsJson
        let expected=describe payload
        if expected.EventType<>e.EventType || expected.Tags.Length<>tags.Length || not(expected.Tags |> List.forall(fun t->List.contains t tags)) then invalidOp "invalid stored event metadata"
        {Position=e.Position;CommandId=e.CommandId;EventType=e.EventType;Tags=tags;Payload=payload}
    let notify listener events=try listener events with _->false
    member _.EngineVersion=journal.EngineVersion
    member _.Encoded=encoded
    member _.Decoded=decoded
    member _.EncodedBytes=encodedBytes
    member _.DecodedBytes=decodedBytes
    member _.ListenerFaults=lock gate (fun ()->faults)
    member _.Read query:ReadResult<'E>=lock gate (fun ()->let read=journal.Read(query,0) in {After=read.After;Events=List.map decodeEvent read.Events})
    member _.Refresh()=lock gate (fun ()->
        if listeners.Count<>0 then
            let tail=journal.Read({Items=[]},published)
            let events=List.map decodeEvent tail.Events
            for listener in listeners.ToArray() do if not(notify listener events) then listeners.Remove listener |> ignore;faults<-faults+1
            published<-tail.After)
    member this.Append(id,query,after,events)=lock gate (fun ()->
        let batch=events |> List.map(encodeEvent id)
        match journal.Append(id,query,after,batch) with
        | RawConflict -> Conflict
        | RawCommitted rows ->
            if listeners.Count=0 then published<-(List.last rows).Position else this.Refresh()
            Committed(List.map decodeEvent rows))
    member this.Subscribe listener=lock gate (fun ()->
        if listeners.Count<>0 then this.Refresh()
        let read=journal.Read({Items=[]},0)
        if not(notify listener (List.map decodeEvent read.Events)) then invalidOp "subscription replay failed"
        listeners.Add listener;published<-read.After)
    member _.Load(events:Recorded<'E> list)=lock gate (fun ()->
        if listeners.Count<>0 then invalidOp "load before subscribe"
        let rows=events |> List.mapi(fun i e->
            if e.Position<>i+1 || String.IsNullOrEmpty e.CommandId then invalidArg "events" "invalid imported position"
            let row=encodeEvent e.CommandId e.Payload
            let meta=describe e.Payload
            if meta.EventType<>e.EventType || meta.Tags.Length<>e.Tags.Length || not(meta.Tags |> List.forall(fun t->List.contains t e.Tags)) then invalidArg "events" "invalid imported metadata"
            {row with Position=e.Position})
        journal.Load rows)
    interface ICommandStore<'E> with
        member this.Read query=this.Read query
        member this.Append(id,query,after,events)=this.Append(id,query,after,events)
    interface IDisposable with member _.Dispose()=journal.Dispose()
