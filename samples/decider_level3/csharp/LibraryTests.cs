using Playground.Dcb;
using System.Collections.Immutable;
// This probe deliberately has no SchoolDomain reference.
public static class LibraryTests
{
    private sealed record Counter(string Key,int Delta);
    private static Tag Tag(string key)=>new("counter",[key]);
    private static EventDescriptor Describe(Counter e)=>new("CounterDelta",[Tag(e.Key),new("scope",["test"])]);
    private static Query Query(string key)=>new([new([Tag(key)],[])]);
    private static void Check(bool value){if(!value)throw new Exception("DCB library probe failed");}
    private sealed class CounterHandler:ICommandHandler<int,Counter,string>
    {
        public ImmutableArray<string> Validate(int n)=>n>0?[]:["positive delta required"];
        public Query QueryFor(int n){Check(n>0);return Query("executor");}
        public Preparation<Counter,string> Handle(ImmutableArray<Recorded<Counter>> _,int n)=>new Emit<Counter,string>([new("executor",n)]);
    }
    public static void Run()
    {
        var executorStore=new EventStore<Counter>(Describe,e=>e);
        IDcbExecutor<Counter,string> executor=new DcbExecutor<Counter,string>(executorStore);
        Check(executor.Execute("valid",5,new CounterHandler()).Appended==1);
        Check(executor.Execute("invalid",0,new CounterHandler()).Status=="Rejected" && executorStore.Read(new([])).After==1);
        Check(Tag("a")==Tag("a"));Check(new HashSet<Tag>{Tag("a"),Tag("a")}.Count==1);
        var store=new EventStore<Counter>(Describe,e=>e);
        var p=new LiveProjection<Counter,int>(0,(s,e)=>s+e.Delta,s=>s);store.Subscribe(p.Accept);
        store.Subscribe(batch=>batch.IsEmpty?true:throw new Exception("expected listener failure"));
        var first=store.Append("first",Query("a"),0,[new("a",1),new("a",2)]);
        Check(first.Status==AppendStatus.Committed && first.Events.Length==2 && store.ListenerFaults==1 && p.Snapshot()==(2,3));
        var unrelated=store.Append("other",Query("b"),0,[new("b",9)]);Check(unrelated.Status==AppendStatus.Committed);
        Check(store.Append("stale",Query("a"),0,[new("a",99)]).Status==AppendStatus.Conflict);
        var all=store.Read(new([]));Check(all.After==3 && p.Snapshot()==(3,12));
        Check(store.Read(new([new([Tag("a"),new("scope",["test"])],[])])).Events.Length==2);
        Check(store.Read(new([new([Tag("a")],[]),new([Tag("b")],[])])).Events.Length==3);
        Check(store.Read(new([new([Tag("a")],["OtherType"])])).Events.IsEmpty);
        Check(p.Accept(all.Events) && p.Snapshot()==(3,12));
        var gap=new LiveProjection<Counter,int>(0,(s,e)=>s+e.Delta,s=>s);
        Check(!gap.Accept([all.Events[0],all.Events[2]]) && gap.Snapshot()==(0,0));
        var late=new LiveProjection<Counter,int>(0,(s,e)=>s+e.Delta,s=>s);store.Subscribe(late.Accept);Check(late.Snapshot()==(3,12));
        var processor=new CommandProcessor<Counter,Counter,string>(store,c=>Query(c.Key),(events,c)=>c.Delta>0?new Emit<Counter,string>([c]):new Reject<Counter,string>("NegativeDelta"));
        var ticket=processor.Prepare("retry",new("a",1));
        store.Append("interfere",Query("a"),3,[new("a",1)]);
        var result=processor.Commit(ticket);Check(result.Status=="Committed" && result.Conflicts==1 && p.Snapshot()==(5,14) && late.Snapshot()==(5,14));
        Check(processor.Commit(processor.Prepare("reject",new("a",-1))).Status=="Rejected");
        var restored=new EventStore<Counter>(Describe,e=>e);restored.Load(store.Read(new([])).Events);Check(restored.Read(new([])).After==5);
        var invalid=store.Read(new([])).Events.SetItem(0,all.Events[0] with{Tags=[]});
        try{new EventStore<Counter>(Describe,e=>e).Load(invalid);throw new Exception("invalid metadata accepted");}catch(ArgumentException){}
        var racing=new EventStore<Counter>(Describe,e=>e);int winners=0;
        using var barrier=new Barrier(2);
        Parallel.For(0,2,i=>{var read=racing.Read(Query("a"));barrier.SignalAndWait();if(racing.Append("race-"+i,Query("a"),read.After,[new("a",1)]).Status==AppendStatus.Committed)Interlocked.Increment(ref winners);});
        Check(winners==1 && racing.Read(new([])).After==1);
    }
}
