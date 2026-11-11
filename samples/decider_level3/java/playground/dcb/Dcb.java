package playground.dcb;
import java.util.ArrayList;
import java.util.List;
import java.util.function.BiFunction;
import java.util.function.Function;
import java.util.function.Predicate;

// No school types or routing registry in this reusable library.
public final class Dcb {
    public record Tag(String category, List<String> keys) { public Tag { keys = List.copyOf(keys); } }
    public record QueryItem(List<Tag> tags, List<String> types) { public QueryItem { tags=List.copyOf(tags); types=List.copyOf(types); } }
    public record Query(List<QueryItem> items) {
        public Query { items=List.copyOf(items); }
        public boolean matches(String type, List<Tag> tags) {
            return items.isEmpty() || items.stream().anyMatch(q -> (q.types().isEmpty() || q.types().contains(type)) && tags.containsAll(q.tags()));
        }
    }
    public record EventDescriptor(String eventType, List<Tag> tags) { public EventDescriptor { tags=List.copyOf(tags); } }
    public record Recorded<E>(int position, String commandId, String eventType, List<Tag> tags, E payload) { public Recorded { tags=List.copyOf(tags); } }
    public record ReadResult<E>(int after, List<Recorded<E>> events) { public ReadResult { events=List.copyOf(events); } }
    public record AppendResult<E>(boolean committed, List<Recorded<E>> events) { public AppendResult { events=List.copyOf(events); } }
    public interface CommandStore<E> {
        ReadResult<E> read(Query query);
        AppendResult<E> append(String commandId,Query condition,int after,List<E> batch);
    }
    // Listener callbacks are synchronous, non-reentrant, and return false on failure.
    public static final class EventStore<E> implements CommandStore<E> {
        private final List<Recorded<E>> log = new ArrayList<>();
        private final List<Predicate<List<Recorded<E>>>> listeners = new ArrayList<>();
        private final Function<E,E> copyPayload;
        private final Function<E,EventDescriptor> describe;
        private int faults;
        public EventStore(Function<E,EventDescriptor> describe,Function<E,E> copyPayload) { this.describe=describe;this.copyPayload=copyPayload; }
        private Recorded<E> copy(Recorded<E> e) { return new Recorded<>(e.position(),e.commandId(),e.eventType(),e.tags(),copyPayload.apply(e.payload())); }
        public synchronized int listenerFaults() { return faults; }
        public synchronized ReadResult<E> read(Query query) { return new ReadResult<>(log.size(),log.stream().filter(e -> query.matches(e.eventType(),e.tags())).map(this::copy).toList()); }
        public synchronized AppendResult<E> append(String commandId, Query condition, int after, List<E> batch) {
            if (batch.isEmpty() || commandId.isEmpty() || after<0 || after>log.size()) throw new IllegalArgumentException("invalid append");
            if (log.stream().anyMatch(e -> e.position()>after && condition.matches(e.eventType(),e.tags()))) return new AppendResult<>(false,List.of());
            var committed = new ArrayList<Recorded<E>>();
            for (var payload : batch) {
                var copied=copyPayload.apply(payload);var e=describe.apply(copied);
                if (e.eventType().isEmpty()) throw new IllegalArgumentException("event type required");
                committed.add(new Recorded<>(log.size()+committed.size()+1,commandId,e.eventType(),e.tags(),copied));
            }
            log.addAll(committed);
            for (var listener : List.copyOf(listeners)) if (!notify(listener,committed.stream().map(this::copy).toList())) { listeners.remove(listener); faults++; }
            return new AppendResult<>(true,committed.stream().map(this::copy).toList());
        }
        private boolean notify(Predicate<List<Recorded<E>>> listener,List<Recorded<E>> events){try{return listener.test(events);}catch(RuntimeException | Error failure){return false;}}
        private boolean valid(Recorded<E> e){var d=describe.apply(e.payload());return d.eventType().equals(e.eventType())&&d.tags().size()==e.tags().size()&&e.tags().containsAll(d.tags());}
        public synchronized void subscribe(Predicate<List<Recorded<E>>> listener) {
            if (!notify(listener,log.stream().map(this::copy).toList())) throw new IllegalStateException("subscription replay failed");
            listeners.add(listener);
        }
        public synchronized void load(List<Recorded<E>> events) {
            if (!log.isEmpty() || !listeners.isEmpty()) throw new IllegalStateException("load only into an empty store");
            for (int i=0;i<events.size();i++) { var e=events.get(i); if (e.position()!=i+1 || e.commandId().isEmpty() || e.eventType().isEmpty() || !valid(e)) throw new IllegalArgumentException("invalid imported log"); }
            var copy=events.stream().map(this::copy).toList(); log.addAll(copy);
        }
    }
    public record Snapshot<S>(int position,S state) {}
    public static final class LiveProjection<E,S> {
        private S state; private int checkpoint;
        private final BiFunction<S,E,S> reduce;
        private final Function<S,S> copyState;
        public LiveProjection(S initial, BiFunction<S,E,S> reduce, Function<S,S> copyState) { state=initial; this.reduce=reduce; this.copyState=copyState; }
        public synchronized boolean accept(List<Recorded<E>> batch) {
            S next=copyState.apply(state); int position=checkpoint;
            for (var e : batch) {
                if (e.position()<=position) continue;
                if (e.position()!=position+1) return false;
                next=reduce.apply(next,e.payload()); position=e.position();
            }
            state=next; checkpoint=position; return true;
        }
        public synchronized Snapshot<S> snapshot() { return new Snapshot<>(checkpoint,copyState.apply(state)); }
    }

    public sealed interface Preparation<E,R> permits Emit, Reject {}
    public record Emit<E,R>(List<E> events) implements Preparation<E,R> { public Emit { events=List.copyOf(events); } }
    public record Reject<E,R>(R error) implements Preparation<E,R> {}
    public record Ticket<C,E,R>(String commandId, C command, Query query, int after, int returned, Preparation<E,R> decision) {}
    public record HandlingResult<R>(String status, int conflicts, int appended, int position, List<R> errors) {}
    public static final class CommandProcessor<C,E,R> {
        private final CommandStore<E> store; private final Function<C,Query> queryFor;
        private final BiFunction<List<Recorded<E>>,C,Preparation<E,R>> decide; private final int maxConflicts;
        public CommandProcessor(CommandStore<E> store, Function<C,Query> queryFor, BiFunction<List<Recorded<E>>,C,Preparation<E,R>> decide, int maxConflicts) {
            this.store=store;this.queryFor=queryFor;this.decide=decide;this.maxConflicts=maxConflicts;
        }
        public Ticket<C,E,R> prepare(String id,C command) {
            var query=queryFor.apply(command); var read=store.read(query);
            return new Ticket<>(id,command,query,read.after(),read.events().size(),decide.apply(read.events(),command));
        }
        public HandlingResult<R> commit(Ticket<C,E,R> ticket) {
            int conflicts=0;
            while (true) {
                if (ticket.decision() instanceof Reject<E,R> rejected) return new HandlingResult<>("Rejected",conflicts,0,ticket.after(),List.of(rejected.error()));
                if (!(ticket.decision() instanceof Emit<E,R> accepted)) throw new IllegalStateException("invalid preparation");
                var result=store.append(ticket.commandId(),ticket.query(),ticket.after(),accepted.events());
                if (result.committed()) return new HandlingResult<>("Committed",conflicts,result.events().size(),result.events().getLast().position(),List.of());
                if (++conflicts>=maxConflicts) return new HandlingResult<>("RetryExhausted",conflicts,0,ticket.after(),List.of());
                ticket=prepare(ticket.commandId(),ticket.command());
            }
        }
    }
}
