using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Playground.Dcb;

// Storage representation is independent of the domain's event type.
public sealed record JsonEvent(int Position,string CommandId,string EventType,string PayloadJson,string TagsJson);
public sealed record JsonRead(int After,ImmutableArray<JsonEvent> Events);
public sealed record JsonAppend(bool Committed,ImmutableArray<JsonEvent> Events);
public interface IJsonJournal:IDisposable
{
    string EngineVersion {get;}
    JsonRead Read(Query query,int after=0);
    JsonAppend Append(string commandId,Query condition,int after,ImmutableArray<JsonEvent> events);
    void Load(ImmutableArray<JsonEvent> events);
}
public static class JournalMetadata
{
    public static string Encode(ImmutableArray<Tag> tags)=>JsonSerializer.Serialize(tags,JournalJson.Default.ImmutableArrayTag);
    public static ImmutableArray<Tag> Decode(string json)=>JsonSerializer.Deserialize(json,JournalJson.Default.ImmutableArrayTag);
    public static byte[] Keys(ImmutableArray<string> keys)
    {
        using var output=new MemoryStream();Span<byte> size=stackalloc byte[4];
        foreach(var key in keys){var bytes=Encoding.UTF8.GetBytes(key);System.Buffers.Binary.BinaryPrimitives.WriteInt32BigEndian(size,bytes.Length);output.Write(size);output.Write(bytes);}
        return output.ToArray();
    }
    public static void Validate(string json)
    {
        var keys=new HashSet<string>(StringComparer.Ordinal);
        foreach(var tag in Decode(json))if(!keys.Add(IndexKey(tag)))throw new InvalidDataException("duplicate stored tag");
    }
    public static string IndexKey(Tag tag)=>Convert.ToHexString(Keys([tag.Category,..tag.Keys]));
}
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ImmutableArray<Tag>))]
internal partial class JournalJson:JsonSerializerContext;

// Indexed memory backend stores JSON, not live event object references.
public sealed class MemoryJsonJournal:IJsonJournal
{
    private readonly object gate=new();
    private readonly List<JsonEvent> log=[];
    private readonly Dictionary<string,List<int>> tags=new(StringComparer.Ordinal),types=new(StringComparer.Ordinal);
    public string EngineVersion=>"memory-json-v1";
    private List<int> Candidates(Query query,int after,int high)
    {
        if(query.Items.IsEmpty || query.Items.Any(i=>i.Tags.IsEmpty && i.Types.IsEmpty))return Enumerable.Range(after+1,high-after).ToList();
        var all=new HashSet<int>();
        foreach(var item in query.Items)
        {
            HashSet<int>? selected=null;
            foreach(var tag in item.Tags){var found=tags.GetValueOrDefault(JournalMetadata.IndexKey(tag),[]);if(selected is null)selected=[..found];else selected.IntersectWith(found);}
            if(!item.Types.IsEmpty){var found=item.Types.SelectMany(t=>types.GetValueOrDefault(t,[]));if(selected is null)selected=[..found];else selected.IntersectWith(found);}
            if(selected is not null)all.UnionWith(selected);
        }
        return all.Where(p=>p>after && p<=high).Order().ToList();
    }
    public JsonRead Read(Query query,int after=0){lock(gate){if(after<0 || after>log.Count)throw new ArgumentException("invalid position");return new(log.Count,[..Candidates(query,after,log.Count).Select(p=>log[p-1])]);}}
    private void Insert(JsonEvent e)
    {
        log.Add(e);
        if(!types.TryGetValue(e.EventType,out var byType))types[e.EventType]=byType=[];byType.Add(e.Position);
        foreach(var tag in JournalMetadata.Decode(e.TagsJson)){var key=JournalMetadata.IndexKey(tag);if(!tags.TryGetValue(key,out var byTag))tags[key]=byTag=[];byTag.Add(e.Position);}
    }
    public JsonAppend Append(string id,Query query,int after,ImmutableArray<JsonEvent> events)
    {
        lock(gate)
        {
            if(events.IsEmpty || id.Length==0 || after<0 || after>log.Count)throw new ArgumentException("invalid append");
            if(Candidates(query,after,log.Count).Count!=0)return new(false,[]);
            var rows=events.Select((e,i)=>e with{Position=log.Count+i+1,CommandId=id}).ToImmutableArray();
            foreach(var row in rows)JournalMetadata.Validate(row.TagsJson);
            foreach(var row in rows)Insert(row);
            return new(true,rows);
        }
    }
    public void Load(ImmutableArray<JsonEvent> events){lock(gate){if(log.Count!=0)throw new InvalidOperationException("load requires empty journal");if(events.Where((e,i)=>e.Position!=i+1).Any())throw new ArgumentException("invalid imported position");foreach(var e in events)JournalMetadata.Validate(e.TagsJson);foreach(var e in events)Insert(e);}}
    public void Dispose(){}
}

// Codec delegates keep domain JSON registration outside the reusable library.
public sealed class JsonEventStore<E>(IJsonJournal journal,Func<E,EventDescriptor> describe,Func<E,string> encode,Func<string,E> decode):ICommandStore<E>,IDisposable
{
    private readonly object gate=new();
    private readonly List<Func<ImmutableArray<Recorded<E>>,bool>> listeners=[];
    private int published,faults;
    public long Encoded {get;private set;}
    public long Decoded {get;private set;}
    public long EncodedBytes {get;private set;}
    public long DecodedBytes {get;private set;}
    public string EngineVersion=>journal.EngineVersion;
    public int ListenerFaults {get{lock(gate)return faults;}}
    private JsonEvent Encode(string id,E value)
    {
        var descriptor=describe(value);if(descriptor.EventType.Length==0)throw new ArgumentException("event type required");
        var json=encode(value);Encoded++;EncodedBytes+=Encoding.UTF8.GetByteCount(json);
        return new(0,id,descriptor.EventType,json,JournalMetadata.Encode(descriptor.Tags));
    }
    private Recorded<E> Decode(JsonEvent e)
    {
        var payload=decode(e.PayloadJson);Decoded++;DecodedBytes+=Encoding.UTF8.GetByteCount(e.PayloadJson);
        var tags=JournalMetadata.Decode(e.TagsJson);var expected=describe(payload);
        if(expected.EventType!=e.EventType || expected.Tags.Length!=tags.Length || !expected.Tags.All(tags.Contains))throw new InvalidDataException("invalid stored event metadata");
        return new(e.Position,e.CommandId,e.EventType,tags,payload);
    }
    public ReadResult<E> Read(Query query){lock(gate){var read=journal.Read(query);return new(read.After,[..read.Events.Select(Decode)]);}}
    private static bool Notify(Func<ImmutableArray<Recorded<E>>,bool> listener,ImmutableArray<Recorded<E>> events){try{return listener(events);}catch(Exception){return false;}}
    public void Refresh()
    {
        lock(gate)
        {
            if(listeners.Count==0)return;
            var tail=journal.Read(new([]),published);var events=tail.Events.Select(Decode).ToImmutableArray();
            foreach(var listener in listeners.ToArray())if(!Notify(listener,events)){listeners.Remove(listener);faults++;}
            published=tail.After;
        }
    }
    public AppendResult<E> Append(string id,Query query,int after,ImmutableArray<E> events)
    {
        lock(gate)
        {
            var batch=events.Select(e=>Encode(id,e)).ToImmutableArray();var result=journal.Append(id,query,after,batch);
            if(!result.Committed)return new(AppendStatus.Conflict,[]);
            if(listeners.Count==0)published=result.Events[^1].Position;else Refresh();return new(AppendStatus.Committed,[..result.Events.Select(Decode)]);
        }
    }
    public void Subscribe(Func<ImmutableArray<Recorded<E>>,bool> listener)
    {
        lock(gate)
        {
            if(listeners.Count!=0)Refresh();var read=journal.Read(new([]));var events=read.Events.Select(Decode).ToImmutableArray();
            if(!Notify(listener,events))throw new InvalidOperationException("subscription replay failed");
            listeners.Add(listener);published=read.After;
        }
    }
    public void Load(ImmutableArray<Recorded<E>> events)
    {
        lock(gate)
        {
            if(listeners.Count!=0)throw new InvalidOperationException("load before subscribe");
            if(events.Where((e,i)=>e.Position!=i+1 || e.CommandId.Length==0).Any())throw new ArgumentException("invalid imported log");
            var rows=events.Select(e=>{var row=Encode(e.CommandId,e.Payload) with{Position=e.Position};var meta=describe(e.Payload);if(meta.EventType!=e.EventType || meta.Tags.Length!=e.Tags.Length || !meta.Tags.All(e.Tags.Contains))throw new ArgumentException("invalid imported metadata");return row;}).ToImmutableArray();
            journal.Load(rows);
        }
    }
    public void Dispose()=>journal.Dispose();
}
