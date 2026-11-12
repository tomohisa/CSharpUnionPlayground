namespace Playground.Dcb
open System
open System.Threading
open System.Threading.Tasks
module LibraryTests =
    type Counter = { Key:string; Delta:int }
    let tag key:Tag={Category="counter";Keys=[key]}
    let query key:Query={Items=[{Tags=[tag key];Types=[]}]}
    let describe (e:Counter):EventDescriptor={EventType="CounterDelta";Tags=[tag e.Key;{Category="scope";Keys=["test"]}]}
    let check value=if not value then failwith "DCB library probe failed"
    let run()=
        let executorStore=EventStore(describe,id)
        let executor:IDcbExecutor<Counter,string>=DcbExecutor(executorStore,3)
        let handler:CommandHandler<int,Counter,string>={
            Validate=fun n->if n>0 then [] else ["positive delta required"]
            QueryFor=fun n->check(n>0);query "executor"
            Handle=fun _ n->Emit [{Key="executor";Delta=n}] }
        check(executor.Execute("valid",5,handler).Appended=1)
        check(executor.Execute("invalid",0,handler).Status="Rejected" && (executorStore.Read {Items=[]}).After=1)
        let store=EventStore(describe,id)
        let p=LiveProjection(0,(fun s e->s+e.Delta),id)
        store.Subscribe p.Accept
        store.Subscribe(fun events->if List.isEmpty events then true else failwith "expected listener failure")
        match store.Append("first",query "a",0,[{Key="a";Delta=1};{Key="a";Delta=2}]) with Committed es->check(es.Length=2) | Conflict->failwith "unexpected conflict"
        check(store.ListenerFaults=1 && p.Snapshot()=(2,3))
        match store.Append("other",query "b",0,[{Key="b";Delta=9}]) with Committed _->() | Conflict->failwith "unrelated conflict"
        match store.Append("stale",query "a",0,[{Key="a";Delta=99}]) with Conflict->() | Committed _->failwith "stale write accepted"
        let all=store.Read {Items=[]}
        check(all.After=3 && p.Snapshot()=(3,12))
        check((store.Read {Items=[{Tags=[tag "a";{Category="scope";Keys=["test"]}];Types=[]}]}).Events.Length=2)
        check((store.Read {Items=[{Tags=[tag "a"];Types=[]};{Tags=[tag "b"];Types=[]}]}).Events.Length=3)
        check((store.Read {Items=[{Tags=[tag "a"];Types=["OtherType"]}]}).Events.IsEmpty)
        check(p.Accept all.Events && p.Snapshot()=(3,12))
        let gap=LiveProjection(0,(fun s e->s+e.Delta),id)
        check(not(gap.Accept [all.Events[0];all.Events[2]]) && gap.Snapshot()=(0,0))
        let late=LiveProjection(0,(fun s e->s+e.Delta),id)
        store.Subscribe late.Accept
        check(late.Snapshot()=(3,12))
        let processor=CommandProcessor(store,(fun c->query c.Key),(fun _ c->if c.Delta>0 then Emit[c] else Reject "NegativeDelta"),3)
        let ticket=processor.Prepare("retry",{Key="a";Delta=1})
        store.Append("interfere",query "a",3,[{Key="a";Delta=1}]) |> ignore
        let result=processor.Commit ticket
        check(result.Status="Committed" && result.Conflicts=1 && p.Snapshot()=(5,14) && late.Snapshot()=(5,14))
        check((processor.Commit(processor.Prepare("reject",{Key="a";Delta= -1}))).Status="Rejected")
        let restored=EventStore(describe,id)
        restored.Load (store.Read {Items=[]}).Events
        check((restored.Read {Items=[]}).After=5)
        let bad=EventStore(describe,id)
        let mutable refused=false
        try bad.Load [{all.Events[0] with Tags=[]}] with :? ArgumentException->refused<-true
        check refused
        let racing=EventStore(describe,id)
        use barrier=new Barrier(2)
        let results=Array.zeroCreate<bool> 2
        [|0;1|] |> Array.map(fun i->Task.Run(fun()->
            let read=racing.Read(query "a")
            barrier.SignalAndWait() |> ignore
            results[i]<-(match racing.Append("race-"+string i,query "a",read.After,[{Key="a";Delta=1}]) with Committed _->true|Conflict->false))) |> Task.WaitAll
        check((results |> Array.filter id).Length=1 && (racing.Read {Items=[]}).After=1)
