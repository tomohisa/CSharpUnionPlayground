import playground.dcb.Dcb;
import playground.dcb.JsonStore;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.*;
final class Application{
    record ReadAudit(Dcb.Query query,int after,int returned){}
    record RunOutput(List<Dcb.Recorded<Event>> events,List<Trace> steps,List<SchoolList> lists,SchoolList list,SchoolList rebuilt,int checkpoint,List<ReadAudit> reads,int listenerFaults){}
    static RunOutput run(List<Dcb.Recorded<Event>> history,List<Operation> operations){
        var path=System.getenv("DECIDER_PERSISTENT");var mapper=new ObjectMapper();
        var store=new JsonStore<Event>(path==null?new JsonStore.Memory():new JsonStore.Sqlite(path,"FULL"),Routing::describe,
            e->{try{return mapper.writerFor(Event.class).writeValueAsString(e);}catch(Exception error){throw new IllegalStateException(error);}},
            json->{try{return mapper.readValue(json,Event.class);}catch(Exception error){throw new IllegalStateException(error);}});
        if(!history.isEmpty())store.load(history);
        if("1".equals(System.getenv("DECIDER_BENCHMARK")))System.err.println("STORE "+store.version());var projection=new SchoolProjection();store.subscribe(projection::accept);
        Dcb.Executor<Event,Rejection> executor=new Dcb.DcbExecutor<>(store);var handler=new SchoolHandler();
        var tickets=new HashMap<String,Dcb.PreparedCommand<Rejection>>();var steps=new ArrayList<Trace>();var lists=new ArrayList<SchoolList>();var reads=new ArrayList<ReadAudit>();
        boolean bench="1".equals(System.getenv("DECIDER_BENCHMARK"));long start=System.nanoTime();if(bench)System.err.println("READY");
        for(var operation:operations){switch(operation){
            case Execute x->{var t=prepare(executor,handler,reads,x.commandId(),x.command());commit(steps,"Execute",t);}
            case Prepare x->{if(tickets.containsKey(x.key()))throw new IllegalArgumentException("duplicate ticket");var t=prepare(executor,handler,reads,x.commandId(),x.command());tickets.put(x.key(),t);steps.add(new Trace("Prepare","Prepared",0,0,t.after(),List.of()));}
            case Commit x->{var t=Optional.ofNullable(tickets.remove(x.key())).orElseThrow();commit(steps,"Commit",t);}
            case ListAll x->{store.refresh();if(store.listenerFaults()!=0)throw new IllegalStateException("projection unavailable; rebuild required");var snapshot=projection.snapshot();lists.add(snapshot.state());steps.add(new Trace("ListAll","Listed",0,0,snapshot.position(),List.of()));}
            case Rebuild x->{var fresh=new SchoolProjection();if(!fresh.accept(store.read(new Dcb.Query(List.of())).events()))throw new IllegalStateException();var snapshot=fresh.snapshot();lists.add(snapshot.state());steps.add(new Trace("Rebuild","Rebuilt",0,0,snapshot.position(),List.of()));}
            case Redeliver x->{if(!projection.accept(store.read(new Dcb.Query(List.of())).events()))throw new IllegalStateException();steps.add(new Trace("Redeliver","Redelivered",0,0,projection.checkpoint(),List.of()));}
        }}
        if(bench)System.err.println("PHASE operations "+(System.nanoTime()-start)/1e9);start=System.nanoTime();
        store.refresh();
        QueryBenchmark.run(store,projection);start=System.nanoTime();
        var events=store.read(new Dcb.Query(List.of())).events();var fresh=new SchoolProjection();if(!fresh.accept(events))throw new IllegalStateException("rebuild failed");
        if(store.listenerFaults()!=0)throw new IllegalStateException("projection unavailable; rebuild required");
        var snapshot=projection.snapshot();
        if(bench){System.err.println("PHASE read_rebuild_snapshot "+(System.nanoTime()-start)/1e9);System.err.println("CODEC "+store.encoded+" "+store.decoded+" "+store.encodedBytes+" "+store.decodedBytes);}
        store.close();
        StorageBenchmark.run();
        return new RunOutput(events,steps,lists,snapshot.state(),fresh.snapshot().state(),snapshot.position(),reads,store.listenerFaults());
    }
    static Dcb.PreparedCommand<Rejection> prepare(Dcb.Executor<Event,Rejection> executor,SchoolHandler handler,List<ReadAudit>reads,String id,Command c){var t=executor.prepare(id,c,handler);if(t.wasRead())reads.add(new ReadAudit(t.query(),t.after(),t.returned()));return t;}
    static void commit(List<Trace>steps,String op,Dcb.PreparedCommand<Rejection>t){var r=t.commit();steps.add(new Trace(op,r.status(),r.conflicts(),r.appended(),r.position(),r.errors()));}
}
