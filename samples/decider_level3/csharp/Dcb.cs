using System.Collections.Immutable;
namespace Playground.Dcb;

public sealed record Tag(string Category, ImmutableArray<string> Keys)
{
    public bool Equals(Tag? other) => other is not null && Category == other.Category && Keys.SequenceEqual(other.Keys);
    public override int GetHashCode() { var hash=new HashCode();hash.Add(Category);foreach(var key in Keys)hash.Add(key);return hash.ToHashCode(); }
    public bool Same(Tag other) => Equals(other);
}
public sealed record QueryItem(ImmutableArray<Tag> Tags, ImmutableArray<string> Types);
public sealed record Query(ImmutableArray<QueryItem> Items)
{
    public bool Matches(string type, ImmutableArray<Tag> tags) => Items.IsEmpty || Items.Any(item =>
        (item.Types.IsEmpty || item.Types.Contains(type)) && item.Tags.All(want => tags.Any(want.Same)));
}
public sealed record EventDescriptor(string EventType, ImmutableArray<Tag> Tags);
public sealed record Recorded<T>(int Position, string CommandId, string EventType, ImmutableArray<Tag> Tags, T Payload);
public sealed record ReadResult<T>(int After, ImmutableArray<Recorded<T>> Events);
public enum AppendStatus { Committed, Conflict }
public sealed record AppendResult<T>(AppendStatus Status, ImmutableArray<Recorded<T>> Events);

public interface ICommandStore<E>
{
    ReadResult<E> Read(Query query);
    AppendResult<E> Append(string commandId,Query condition,int after,ImmutableArray<E> batch);
}
// Domain-free in-process store. Listener callbacks are synchronous, pure consumers:
// they must return false on failure and must not call back into the store.
public sealed class EventStore<T>(Func<T,EventDescriptor> describe, Func<T,T> copyPayload) : ICommandStore<T>
{
    private readonly object gate = new();
    private readonly List<Recorded<T>> log = [];
    private readonly List<Func<ImmutableArray<Recorded<T>>,bool>> listeners = [];
    private int faults;
    public int ListenerFaults {get{lock(gate)return faults;}}
    private Recorded<T> Copy(Recorded<T> e) => e with { Payload = copyPayload(e.Payload) };
    public ReadResult<T> Read(Query query)
    {
        lock (gate) return new(log.Count, [.. log.Where(e => query.Matches(e.EventType, e.Tags)).Select(Copy)]);
    }
    public AppendResult<T> Append(string commandId, Query condition, int after, ImmutableArray<T> batch)
    {
        lock (gate)
        {
            if (batch.IsEmpty || commandId.Length == 0 || after < 0 || after > log.Count) throw new ArgumentException("invalid append");
            if (log.Any(e => e.Position > after && condition.Matches(e.EventType, e.Tags))) return new(AppendStatus.Conflict, []);
            // Validate/copy the entire batch before changing the log.
            var committed = batch.Select((payload,i) => {
                var copy=copyPayload(payload);var meta=describe(copy);
                if(meta.EventType.Length==0)throw new ArgumentException("event type required");
                return new Recorded<T>(log.Count+i+1,commandId,meta.EventType,meta.Tags,copy);
            }).ToImmutableArray();
            log.AddRange(committed);
            foreach (var listener in listeners.ToArray())
                if (!Notify(listener,[.. committed.Select(Copy)])) { listeners.Remove(listener); faults++; }
            return new(AppendStatus.Committed, [.. committed.Select(Copy)]);
        }
    }
    private static bool Notify(Func<ImmutableArray<Recorded<T>>,bool> listener,ImmutableArray<Recorded<T>> batch)
    { try { return listener(batch); } catch(Exception) { return false; } }
    private bool ValidMetadata(Recorded<T> e) { var d=describe(e.Payload);return d.EventType==e.EventType && d.Tags.Length==e.Tags.Length && d.Tags.All(e.Tags.Contains); }
    public void Subscribe(Func<ImmutableArray<Recorded<T>>,bool> listener)
    {
        lock (gate)
        {
            if (!Notify(listener,[.. log.Select(Copy)])) throw new InvalidOperationException("subscription replay failed");
            listeners.Add(listener); // Replay and attachment share the append lock: no gap.
        }
    }
    public void Load(ImmutableArray<Recorded<T>> events)
    {
        lock (gate)
        {
            if (log.Count != 0 || listeners.Count != 0) throw new InvalidOperationException("load only into an empty store");
            if (events.Where((e,i) => e.Position != i+1 || e.CommandId.Length == 0 || e.EventType.Length == 0 || !ValidMetadata(e)).Any()) throw new ArgumentException("invalid imported log");
            var copy = events.Select(Copy).ToArray(); log.AddRange(copy);
        }
    }
}

public sealed class LiveProjection<E,S>(S initial, Func<S,E,S> reduce, Func<S,S> copyState)
{
    private readonly object gate = new();
    private S state = initial;
    private int checkpoint;
    public bool Accept(ImmutableArray<Recorded<E>> batch)
    {
        lock (gate)
        {
            var next = copyState(state); int position = checkpoint;
            foreach (var e in batch)
            {
                if (e.Position <= position) continue;
                if (e.Position != position + 1) return false;
                next = reduce(next, e.Payload); position = e.Position;
            }
            state = next; checkpoint = position; return true;
        }
    }
    public (int Position, S State) Snapshot() { lock (gate) return (checkpoint, copyState(state)); }
}

public abstract record Preparation<E,R>;
public sealed record Emit<E,R>(ImmutableArray<E> Events) : Preparation<E,R>;
public sealed record Reject<E,R>(R Error) : Preparation<E,R>;
public sealed record Ticket<C,E,R>(string CommandId, C Command, Query Query, int After, int Returned, Preparation<E,R> Decision);
public sealed record HandlingResult<R>(string Status, int Conflicts, int Appended, int Position, ImmutableArray<R> Errors);
public sealed class CommandProcessor<C,E,R>(ICommandStore<E> store, Func<C,Query> queryFor,
    Func<ImmutableArray<Recorded<E>>,C,Preparation<E,R>> decide, int maxConflicts = 3)
{
    public Ticket<C,E,R> Prepare(string commandId, C command)
    {
        var query = queryFor(command); var read = store.Read(query);
        return new(commandId, command, query, read.After, read.Events.Length, decide(read.Events, command));
    }
    public HandlingResult<R> Commit(Ticket<C,E,R> ticket)
    {
        int conflicts = 0;
        while (true)
        {
            if (ticket.Decision is Reject<E,R> rejected) return new("Rejected", conflicts, 0, ticket.After, [rejected.Error]);
            if (ticket.Decision is not Emit<E,R> accepted) throw new InvalidOperationException("invalid preparation");
            var result = store.Append(ticket.CommandId, ticket.Query, ticket.After, accepted.Events);
            if (result.Status == AppendStatus.Committed) return new("Committed", conflicts, result.Events.Length, result.Events[^1].Position, []);
            conflicts++;
            if (conflicts >= maxConflicts) return new("RetryExhausted", conflicts, 0, ticket.After, []);
            ticket = Prepare(ticket.CommandId, ticket.Command); // Read + reduce + decide again, never reuse old events.
        }
    }
}

// The handler belongs to the domain; the executor owns persistence and retries.
public interface ICommandHandler<C,E,R>
{
    ImmutableArray<R> Validate(C command);
    Query QueryFor(C command);
    Preparation<E,R> Handle(ImmutableArray<Recorded<E>> events,C command);
}
public sealed class PreparedCommand<R>(Query query,int after,int returned,bool wasRead,Func<HandlingResult<R>> commit)
{
    public Query Query {get;}=query;
    public int After {get;}=after;
    public int Returned {get;}=returned;
    public bool WasRead {get;}=wasRead;
    public HandlingResult<R> Commit() => commit();
}
public interface IDcbExecutor<E,R>
{
    PreparedCommand<R> Prepare<C>(string id,C command,ICommandHandler<C,E,R> handler);
    HandlingResult<R> Execute<C>(string id,C command,ICommandHandler<C,E,R> handler);
}
public sealed class DcbExecutor<E,R>(ICommandStore<E> store,int maxConflicts=3):IDcbExecutor<E,R>
{
    public PreparedCommand<R> Prepare<C>(string id,C command,ICommandHandler<C,E,R> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var errors=handler.Validate(command);
        if(!errors.IsEmpty) return new(new([]),0,0,false,()=>new("Rejected",0,0,0,errors));
        var processor=new CommandProcessor<C,E,R>(store,handler.QueryFor,handler.Handle,maxConflicts);
        var ticket=processor.Prepare(id,command);
        return new(ticket.Query,ticket.After,ticket.Returned,true,()=>processor.Commit(ticket));
    }
    public HandlingResult<R> Execute<C>(string id,C command,ICommandHandler<C,E,R> handler) => Prepare(id,command,handler).Commit();
}

// Transaction rows must be immutable. Only touched keys are staged; no table clone.
public sealed class IndexBatch<K,V>(IReadOnlyDictionary<K,V> rows) where K:notnull
{
    internal readonly Dictionary<K,(bool Remove,V Value)> Changes=[];
    public long Work {get;private set;}
    public bool TryGet(K key,out V value)
    {
        Work++;
        if(Changes.TryGetValue(key,out var edit)){value=edit.Value;return !edit.Remove;}
        return rows.TryGetValue(key,out value!);
    }
    public V Get(K key)=>TryGet(key,out var value)?value:throw new InvalidOperationException("missing index row");
    public void Put(K key,V value){Work++;Changes[key]=(false,value);}
    public void Delete(K key){Work++;Changes[key]=(true,default!);}
}
public sealed class IndexedProjection<K,V,E>(Action<IndexBatch<K,V>,Recorded<E>> reduce) where K:notnull
{
    private readonly object gate=new();
    private readonly Dictionary<K,V> rows=[];
    private int checkpoint;
    private long work;
    public int Checkpoint {get{lock(gate)return checkpoint;}}
    public int RowCount {get{lock(gate)return rows.Count;}}
    public long Work {get{lock(gate)return work;}}
    public bool Accept(ImmutableArray<Recorded<E>> events)
    {
        lock(gate)
        {
            var batch=new IndexBatch<K,V>(rows);var position=checkpoint;
            try
            {
                foreach(var e in events){if(e.Position<=position)continue;if(e.Position!=position+1)return false;reduce(batch,e);position=e.Position;}
            }
            catch(Exception){return false;}
            foreach(var (key,edit) in batch.Changes){if(edit.Remove)rows.Remove(key);else rows[key]=edit.Value;}
            checkpoint=position;work+=batch.Work;return true;
        }
    }
    // Query must return a detached value, never the row table or its enumerator.
    public (int Position,T State) Read<T>(Func<IReadOnlyDictionary<K,V>,T> query){lock(gate)return(checkpoint,query(rows));}
}
