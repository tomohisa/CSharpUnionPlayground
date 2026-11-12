using Playground.Dcb;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace SchoolDomain;
// A second payload family proves the storage library is independent of School.Event.
internal sealed record DocumentRevision(string DocumentId,int Revision,string Body);
internal static class StorageBenchmark
{
    public static void Run()
    {
        if(!int.TryParse(Environment.GetEnvironmentVariable("DECIDER_DOCUMENTS"),out var count)||count<=0)return;
        var path=Environment.GetEnvironmentVariable("DECIDER_PERSISTENT");
        IJsonJournal Open()=>path is null?new MemoryJsonJournal():new SqliteJournal(path+".documents");
        var journal=Open();
        JsonEventStore<DocumentRevision> Store(IJsonJournal j)=>new(j,e=>new("DocumentRevision",[new("document",[e.DocumentId])]),
            e=>JsonSerializer.Serialize(e,DocumentJson.Default.DocumentRevision),json=>JsonSerializer.Deserialize(json,DocumentJson.Default.DocumentRevision)??throw new JsonException());
        var latest=new Dictionary<string,int>();var clock=Stopwatch.StartNew();long encoded,decoded;int position;
        using(var store=Store(journal))
        {
            store.Subscribe(events=>{foreach(var e in events){latest[e.Payload.DocumentId]=e.Payload.Revision;}return true;});
            var body=new string('x',4096);
            for(var revision=1;revision<=16;revision++)for(var i=0;i<count;i++)
            {
                var id=$"document-{i:D8}";var query=new Query([new([new("document",[id])],[])]);var read=store.Read(query);
                if(read.Events.Length!=revision-1)throw new Exception("document revision mismatch");
                if(store.Append($"doc-{i}-{revision}",query,read.After,[new(id,revision,body)]).Status!=AppendStatus.Committed)throw new Exception("document conflict");
            }
            var all=store.Read(new([]));position=all.After;encoded=store.Encoded;decoded=store.Decoded;
            if(latest.Count!=count||latest.Values.Any(r=>r!=16)||store.ListenerFaults!=0||all.Events.Any(e=>e.Payload.Body.Length!=4096))throw new Exception("document projection mismatch");
        }
        if(path is not null){using var reopened=Store(Open());var events=reopened.Read(new([])).Events;if(events.Length!=16*count||events.Any(e=>e.Payload.Body.Length!=4096))throw new Exception("document restart mismatch");}
        Console.Error.WriteLine($"DOCUMENT {clock.Elapsed.TotalSeconds:R} {count} {position} {encoded} {decoded}");
    }
}
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DocumentRevision))]
internal partial class DocumentJson:JsonSerializerContext;
