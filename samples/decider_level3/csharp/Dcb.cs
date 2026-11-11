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
