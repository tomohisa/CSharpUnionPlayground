using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Playground.Dcb;
namespace SchoolDomain;

internal sealed record JournalCounter(string Key,int Delta,string Body);
internal static class PersistenceTests
{
    private static EventDescriptor Describe(JournalCounter e)=>new("Delta",[new("counter",[e.Key]),new("all",[])]);
    private static Query Query(string key)=>new([new([new("counter",[key])],[])]);
    private static JsonEventStore<JournalCounter> Open(IJsonJournal journal)=>new(journal,Describe,e=>JsonSerializer.Serialize(e,CounterJson.Default.JournalCounter),s=>JsonSerializer.Deserialize(s,CounterJson.Default.JournalCounter)??throw new JsonException());
    private static void Require(bool condition){if(!condition)throw new Exception("persistence invariant failed");}
    private static void Basic(IJsonJournal journal)
    {
        using var store=Open(journal);var p=new IndexedProjection<string,int,JournalCounter>((tx,e)=>tx.Put(e.Payload.Key,(tx.TryGet(e.Payload.Key,out var old)?old:0)+e.Payload.Delta));store.Subscribe(p.Accept);
        var first=store.Append("one",Query("é"),0,[new("é",1,new string('x',4096))]);Require(first.Status==AppendStatus.Committed);
        Require(store.Append("stale",Query("é"),0,[new("é",9,"")]).Status==AppendStatus.Conflict);
        Require(store.Append("unrelated",Query("e\u0301"),0,[new("e\u0301",2,"")]).Status==AppendStatus.Committed);
        Require(store.Read(Query("é")).Events.Length==1 && store.Read(Query("e\u0301")).Events.Length==1);
        Require(store.Read(new([new([new("all",[])],[])])).Events.Length==2);
        Require(store.Read(new([new([new("all",[]),new("counter",["é"])],["Delta"])])).Events.Length==1);
        Require(store.Read(new([new([new("all",[])],["Unknown"])])).Events.IsEmpty);
        Require(p.Checkpoint==2 && p.Read(rows=>rows["é"]).State==1 && store.Encoded>0 && store.Decoded>0 && store.EncodedBytes>=4096);
    }
    public static void Run(string folder)
    {
        Directory.CreateDirectory(folder);Basic(new MemoryJsonJournal());Basic(new SqliteJournal(Path.Combine(folder,"basic.db")));
        string path=Path.Combine(folder,"concurrent.db");
        using(var a=Open(new SqliteJournal(path)))using(var b=Open(new SqliteJournal(path)))
        {
            var pa=new LiveProjection<JournalCounter,int>(0,(s,e)=>s+e.Delta,s=>s);var pb=new LiveProjection<JournalCounter,int>(0,(s,e)=>s+e.Delta,s=>s);a.Subscribe(pa.Accept);b.Subscribe(pb.Accept);
            int afterA=a.Read(Query("x")).After,afterB=b.Read(Query("x")).After;
            Require(a.Append("writer-a",Query("x"),afterA,[new("x",1,"")]).Status==AppendStatus.Committed);
            Require(b.Append("writer-b-stale",Query("x"),afterB,[new("x",9,"")]).Status==AppendStatus.Conflict);
            Require(b.Append("writer-b-other",Query("y"),afterB,[new("y",2,"")]).Status==AppendStatus.Committed);
            Require(pb.Snapshot()==(2,3));a.Refresh();Require(pa.Snapshot()==(2,3));
        }
        using(var reopened=Open(new SqliteJournal(path)))
        {
            var p=new LiveProjection<JournalCounter,int>(0,(s,e)=>s+e.Delta,s=>s);reopened.Subscribe(p.Accept);Require(p.Snapshot()==(2,3));Require(reopened.Read(new([])).Events.Length==2);
            Console.WriteLine("persistence: memory JSON / SQLite "+reopened.EngineVersion+" / indexed tags / stale writers / restart PASS");
        }
    }
}
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JournalCounter))]
internal partial class CounterJson:JsonSerializerContext;
