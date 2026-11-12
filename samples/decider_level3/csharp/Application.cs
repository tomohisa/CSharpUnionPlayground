using System.Collections.Immutable;
using Playground.Dcb;
namespace SchoolDomain;
public sealed record ReadAudit(Query Query,int After,int Returned);
public sealed record RunOutput(ImmutableArray<Recorded<Event>> Events,ImmutableArray<Trace> Steps,
    ImmutableArray<SchoolList> Lists,SchoolList List,SchoolList Rebuilt,int Checkpoint,ImmutableArray<ReadAudit> Reads,int ListenerFaults);
public static class Application
{
    public static RunOutput Run(ImmutableArray<Recorded<Event>> history,ImmutableArray<Operation> operations)
    {
        var store=new EventStore<Event>(Routing.Describe,e=>e);
        store.Load(history);
        var projection=new LiveProjection<Event,Registers>(Registers.Empty,Registers.Reduce,s=>s);
        store.Subscribe(projection.Accept);
        var reads=new List<ReadAudit>();
        IDcbExecutor<Event,Rejection> executor=new DcbExecutor<Event,Rejection>(store);
        var handler=new SchoolHandler();
        var tickets=new Dictionary<string,PreparedCommand<Rejection>>();
        var steps=new List<Trace>();var lists=new List<SchoolList>();
        PreparedCommand<Rejection> Prepare(string id,Command c) {var t=executor.Prepare(id,c,handler);if(t.WasRead)reads.Add(new(t.Query,t.After,t.Returned));return t;}
        void Commit(string op,PreparedCommand<Rejection> t) {var r=t.Commit();steps.Add(new(op,r.Status,r.Conflicts,r.Appended,r.Position,r.Errors));}
        foreach(var operation in operations)
        {
            switch(operation)
            {
                case Execute x: Commit("Execute",Prepare(x.CommandId,x.Command));break;
                case Prepare x:
                    if(tickets.ContainsKey(x.Key)) throw new ArgumentException("duplicate ticket");
                    var t=Prepare(x.CommandId,x.Command);tickets.Add(x.Key,t);steps.Add(new("Prepare","Prepared",0,0,t.After,[]));break;
                case Commit x: var ticket=tickets[x.Key];tickets.Remove(x.Key);Commit("Commit",ticket);break;
                case ListAll: if(store.ListenerFaults!=0)throw new Exception("projection unavailable; rebuild required");var snap=projection.Snapshot();lists.Add(snap.State.List());steps.Add(new("ListAll","Listed",0,0,snap.Position,[]));break;
                case Rebuild:
                    var rebuild=new LiveProjection<Event,Registers>(Registers.Empty,Registers.Reduce,s=>s);
                    if(!rebuild.Accept(store.Read(new([])).Events))throw new Exception("rebuild failed");
                    var rebuilt=rebuild.Snapshot();lists.Add(rebuilt.State.List());steps.Add(new("Rebuild","Rebuilt",0,0,rebuilt.Position,[]));break;
                case Redeliver:
                    if(!projection.Accept(store.Read(new([])).Events))throw new Exception("duplicate delivery failed");
                    steps.Add(new("Redeliver","Redelivered",0,0,projection.Snapshot().Position,[]));break;
            }
        }
        var all=store.Read(new([])).Events;
        var fresh=new LiveProjection<Event,Registers>(Registers.Empty,Registers.Reduce,s=>s);if(!fresh.Accept(all))throw new Exception("rebuild failed");
        if(store.ListenerFaults!=0)throw new Exception("projection unavailable; rebuild required");
        var final=projection.Snapshot();
        return new(all,[..steps],[..lists],final.State.List(),fresh.Snapshot().State.List(),final.Position,[..reads],store.ListenerFaults);
    }
}
