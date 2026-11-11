import playground.dcb.Dcb;
import java.util.*;
final class Application{
    record ReadAudit(Dcb.Query query,int after,int returned){}
    record RunOutput(List<Dcb.Recorded<Event>> events,List<Trace> steps,List<SchoolList> lists,SchoolList list,SchoolList rebuilt,int checkpoint,List<ReadAudit> reads,int listenerFaults){}
    static RunOutput run(List<Dcb.Recorded<Event>> history,List<Operation> operations){
        var store=new Dcb.EventStore<Event>(Routing::describe,e->e);
        store.load(history);var projection=new Dcb.LiveProjection<Event,Projection.Registers>(Projection.Registers.empty(),Projection::reduce,s->s);store.subscribe(projection::accept);
        var processor=new Dcb.CommandProcessor<Command,Event,Rejection>(store,Routing::queryFor,Routing::decide,3);
        var tickets=new HashMap<String,Dcb.Ticket<Command,Event,Rejection>>();var steps=new ArrayList<Trace>();var lists=new ArrayList<SchoolList>();var reads=new ArrayList<ReadAudit>();
        for(var operation:operations){switch(operation){
            case Execute x->{var t=prepare(processor,store,reads,x.commandId(),x.command());commit(processor,steps,"Execute",t);}
            case Prepare x->{if(tickets.containsKey(x.key()))throw new IllegalArgumentException("duplicate ticket");var t=prepare(processor,store,reads,x.commandId(),x.command());tickets.put(x.key(),t);steps.add(new Trace("Prepare","Prepared",0,0,t.after(),List.of()));}
            case Commit x->{var t=Optional.ofNullable(tickets.remove(x.key())).orElseThrow();commit(processor,steps,"Commit",t);}
            case ListAll x->{if(store.listenerFaults()!=0)throw new IllegalStateException("projection unavailable; rebuild required");var snapshot=projection.snapshot();lists.add(snapshot.state().list());steps.add(new Trace("ListAll","Listed",0,0,snapshot.position(),List.of()));}
            case Rebuild x->{var fresh=new Dcb.LiveProjection<Event,Projection.Registers>(Projection.Registers.empty(),Projection::reduce,s->s);if(!fresh.accept(store.read(new Dcb.Query(List.of())).events()))throw new IllegalStateException();var snapshot=fresh.snapshot();lists.add(snapshot.state().list());steps.add(new Trace("Rebuild","Rebuilt",0,0,snapshot.position(),List.of()));}
            case Redeliver x->{if(!projection.accept(store.read(new Dcb.Query(List.of())).events()))throw new IllegalStateException();steps.add(new Trace("Redeliver","Redelivered",0,0,projection.snapshot().position(),List.of()));}
        }}
        var events=store.read(new Dcb.Query(List.of())).events();var fresh=new Dcb.LiveProjection<Event,Projection.Registers>(Projection.Registers.empty(),Projection::reduce,s->s);if(!fresh.accept(events))throw new IllegalStateException("rebuild failed");
        if(store.listenerFaults()!=0)throw new IllegalStateException("projection unavailable; rebuild required");
        var snapshot=projection.snapshot();
        return new RunOutput(events,steps,lists,snapshot.state().list(),fresh.snapshot().state().list(),snapshot.position(),reads,store.listenerFaults());
    }
    static Dcb.Ticket<Command,Event,Rejection> prepare(Dcb.CommandProcessor<Command,Event,Rejection> p,Dcb.EventStore<Event>s,List<ReadAudit>reads,String id,Command c){var t=p.prepare(id,c);reads.add(new ReadAudit(t.query(),t.after(),t.returned()));return t;}
    static void commit(Dcb.CommandProcessor<Command,Event,Rejection>p,List<Trace>steps,String op,Dcb.Ticket<Command,Event,Rejection>t){var r=p.commit(t);steps.add(new Trace(op,r.status(),r.conflicts(),r.appended(),r.position(),r.errors()));}
}
