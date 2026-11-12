import playground.dcb.Dcb;
import java.util.*;
final class Application{
    record ReadAudit(Dcb.Query query,int after,int returned){}
    record RunOutput(List<Dcb.Recorded<Event>> events,List<Trace> steps,List<SchoolList> lists,SchoolList list,SchoolList rebuilt,int checkpoint,List<ReadAudit> reads,int listenerFaults){}
    static RunOutput run(List<Dcb.Recorded<Event>> history,List<Operation> operations){
        var store=new Dcb.EventStore<Event>(Routing::describe,e->e);
        store.load(history);var projection=new Dcb.LiveProjection<Event,Projection.Registers>(Projection.Registers.empty(),Projection::reduce,s->s);store.subscribe(projection::accept);
        Dcb.Executor<Event,Rejection> executor=new Dcb.DcbExecutor<>(store);var handler=new SchoolHandler();
        var tickets=new HashMap<String,Dcb.PreparedCommand<Rejection>>();var steps=new ArrayList<Trace>();var lists=new ArrayList<SchoolList>();var reads=new ArrayList<ReadAudit>();
        for(var operation:operations){switch(operation){
            case Execute x->{var t=prepare(executor,handler,reads,x.commandId(),x.command());commit(steps,"Execute",t);}
            case Prepare x->{if(tickets.containsKey(x.key()))throw new IllegalArgumentException("duplicate ticket");var t=prepare(executor,handler,reads,x.commandId(),x.command());tickets.put(x.key(),t);steps.add(new Trace("Prepare","Prepared",0,0,t.after(),List.of()));}
            case Commit x->{var t=Optional.ofNullable(tickets.remove(x.key())).orElseThrow();commit(steps,"Commit",t);}
            case ListAll x->{if(store.listenerFaults()!=0)throw new IllegalStateException("projection unavailable; rebuild required");var snapshot=projection.snapshot();lists.add(snapshot.state().list());steps.add(new Trace("ListAll","Listed",0,0,snapshot.position(),List.of()));}
            case Rebuild x->{var fresh=new Dcb.LiveProjection<Event,Projection.Registers>(Projection.Registers.empty(),Projection::reduce,s->s);if(!fresh.accept(store.read(new Dcb.Query(List.of())).events()))throw new IllegalStateException();var snapshot=fresh.snapshot();lists.add(snapshot.state().list());steps.add(new Trace("Rebuild","Rebuilt",0,0,snapshot.position(),List.of()));}
            case Redeliver x->{if(!projection.accept(store.read(new Dcb.Query(List.of())).events()))throw new IllegalStateException();steps.add(new Trace("Redeliver","Redelivered",0,0,projection.snapshot().position(),List.of()));}
        }}
        var events=store.read(new Dcb.Query(List.of())).events();var fresh=new Dcb.LiveProjection<Event,Projection.Registers>(Projection.Registers.empty(),Projection::reduce,s->s);if(!fresh.accept(events))throw new IllegalStateException("rebuild failed");
        if(store.listenerFaults()!=0)throw new IllegalStateException("projection unavailable; rebuild required");
        var snapshot=projection.snapshot();
        return new RunOutput(events,steps,lists,snapshot.state().list(),fresh.snapshot().state().list(),snapshot.position(),reads,store.listenerFaults());
    }
    static Dcb.PreparedCommand<Rejection> prepare(Dcb.Executor<Event,Rejection> executor,SchoolHandler handler,List<ReadAudit>reads,String id,Command c){var t=executor.prepare(id,c,handler);if(t.wasRead())reads.add(new ReadAudit(t.query(),t.after(),t.returned()));return t;}
    static void commit(List<Trace>steps,String op,Dcb.PreparedCommand<Rejection>t){var r=t.commit();steps.add(new Trace(op,r.status(),r.conflicts(),r.appended(),r.position(),r.errors()));}
}
