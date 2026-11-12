using System.Collections.Immutable;
using Playground.Dcb;
using System.Text.Json;
namespace SchoolDomain;
public sealed record ReadAudit(Query Query,int After,int Returned);
public sealed record RunOutput(ImmutableArray<Recorded<Event>> Events,ImmutableArray<Trace> Steps,
    ImmutableArray<SchoolList> Lists,SchoolList List,SchoolList Rebuilt,int Checkpoint,ImmutableArray<ReadAudit> Reads,int ListenerFaults);
public static class Application
{
    public static RunOutput Run(ImmutableArray<Recorded<Event>> history,ImmutableArray<Operation> operations)
    {
        var path=Environment.GetEnvironmentVariable("DECIDER_PERSISTENT");
        using var store=new JsonEventStore<Event>(path is null ? new MemoryJsonJournal() : new SqliteJournal(path),Routing.Describe,
            e=>JsonSerializer.Serialize(e,Wire.Default.Event),json=>JsonSerializer.Deserialize(json,Wire.Default.Event)??throw new JsonException());
        if(!history.IsEmpty)store.Load(history);
        if(Environment.GetEnvironmentVariable("DECIDER_BENCHMARK")=="1")Console.Error.WriteLine("STORE "+store.EngineVersion);
        var projection=new SchoolProjection();
        store.Subscribe(projection.Accept);
        var reads=new List<ReadAudit>();
        IDcbExecutor<Event,Rejection> executor=new DcbExecutor<Event,Rejection>(store);
        var handler=new SchoolHandler();
        var tickets=new Dictionary<string,PreparedCommand<Rejection>>();
        var steps=new List<Trace>();var lists=new List<SchoolList>();
        PreparedCommand<Rejection> Prepare(string id,Command c) {var t=executor.Prepare(id,c,handler);if(t.WasRead)reads.Add(new(t.Query,t.After,t.Returned));return t;}
        void Commit(string op,PreparedCommand<Rejection> t) {var r=t.Commit();steps.Add(new(op,r.Status,r.Conflicts,r.Appended,r.Position,r.Errors));}
        var bench=Environment.GetEnvironmentVariable("DECIDER_BENCHMARK")=="1";
        var clock=System.Diagnostics.Stopwatch.StartNew();if(bench)Console.Error.WriteLine("READY");
        foreach(var operation in operations)
        {
            switch(operation)
            {
                case Execute x: Commit("Execute",Prepare(x.CommandId,x.Command));break;
                case Prepare x:
                    if(tickets.ContainsKey(x.Key)) throw new ArgumentException("duplicate ticket");
                    var t=Prepare(x.CommandId,x.Command);tickets.Add(x.Key,t);steps.Add(new("Prepare","Prepared",0,0,t.After,[]));break;
                case Commit x: var ticket=tickets[x.Key];tickets.Remove(x.Key);Commit("Commit",ticket);break;
                case ListAll: store.Refresh();if(store.ListenerFaults!=0)throw new Exception("projection unavailable; rebuild required");var snap=projection.Snapshot();lists.Add(snap.State);steps.Add(new("ListAll","Listed",0,0,snap.Position,[]));break;
                case Rebuild:
                    var rebuild=new SchoolProjection();
                    if(!rebuild.Accept(store.Read(new([])).Events))throw new Exception("rebuild failed");
                    var rebuilt=rebuild.Snapshot();lists.Add(rebuilt.State);steps.Add(new("Rebuild","Rebuilt",0,0,rebuilt.Position,[]));break;
                case Redeliver:
                    if(!projection.Accept(store.Read(new([])).Events))throw new Exception("duplicate delivery failed");
                    steps.Add(new("Redeliver","Redelivered",0,0,projection.Checkpoint,[]));break;
            }
        }
        if(bench)Console.Error.WriteLine($"PHASE operations {clock.Elapsed.TotalSeconds:R}");clock.Restart();
        store.Refresh();
        QueryBenchmark.Run(store,projection);clock.Restart();
        var all=store.Read(new([])).Events;
        var fresh=new SchoolProjection();if(!fresh.Accept(all))throw new Exception("rebuild failed");
        if(store.ListenerFaults!=0)throw new Exception("projection unavailable; rebuild required");
        var final=projection.Snapshot();
        if(bench){Console.Error.WriteLine($"PHASE read_rebuild_snapshot {clock.Elapsed.TotalSeconds:R}");Console.Error.WriteLine($"CODEC {store.Encoded} {store.Decoded} {store.EncodedBytes} {store.DecodedBytes}");}
        StorageBenchmark.Run();
        return new(all,[..steps],[..lists],final.State,fresh.Snapshot().State,final.Position,[..reads],store.ListenerFaults);
    }
}
