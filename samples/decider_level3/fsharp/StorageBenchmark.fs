namespace SchoolDomain
open Playground.Dcb
open System
open System.Text.Json
open System.Diagnostics
open System.Collections.Generic

type DocumentRevision={DocumentId:string;Revision:int;Body:string}
module StorageBenchmark=
    let run()=
        let count=match Int32.TryParse(Environment.GetEnvironmentVariable("DECIDER_DOCUMENTS")) with | true,n->n | _->0
        if count>0 then
            let path=Environment.GetEnvironmentVariable("DECIDER_PERSISTENT")
            let openJournal():IJsonJournal=if isNull path then new MemoryJsonJournal() else new SqliteJournal((nonNull path)+".documents","FULL")
            let options=JsonSerializerOptions(PropertyNamingPolicy=JsonNamingPolicy.CamelCase)
            let store journal=new JsonEventStore<DocumentRevision>(journal,(fun e->{EventType="DocumentRevision";Tags=[{Category="document";Keys=[e.DocumentId]}]}),(fun e->JsonSerializer.Serialize(e,options)),(fun s->JsonSerializer.Deserialize<DocumentRevision>(s,options)|>nonNull))
            let latest=Dictionary<string,int>()
            let clock=Stopwatch.StartNew()
            let mutable encoded,decoded,position=0L,0L,0
            do
                use s=store(openJournal())
                s.Subscribe(fun events->for e in events do latest[e.Payload.DocumentId]<-e.Payload.Revision
                                        true)
                let body=String('x',4096)
                for revision in 1..16 do
                    for i in 0..count-1 do
                        let id=sprintf "document-%08d" i
                        let q={Items=[{Tags=[{Category="document";Keys=[id]}];Types=[]}]}
                        let read=s.Read q
                        if read.Events.Length<>revision-1 then failwith "document revision mismatch"
                        match s.Append($"doc-{i}-{revision}",q,read.After,[{DocumentId=id;Revision=revision;Body=body}]) with
                        | Committed _->()
                        | Conflict->failwith "document conflict"
                let all=s.Read {Items=[]}
                position<-all.After;encoded<-s.Encoded;decoded<-s.Decoded
                if position<>16*count || latest.Count<>count || latest.Values |> Seq.exists(fun r->r<>16) || s.ListenerFaults<>0 || all.Events |> List.exists(fun e->e.Payload.Body.Length<>4096) then failwith "document projection mismatch"
            if not(isNull path) then
                use reopened=store(openJournal())
                let events=(reopened.Read {Items=[]}).Events
                if events.Length<>16*count || events |> List.exists(fun e->e.Payload.Body.Length<>4096) then failwith "document restart mismatch"
            Console.Error.WriteLine($"DOCUMENT {clock.Elapsed.TotalSeconds:R} {count} {position} {encoded} {decoded}")
