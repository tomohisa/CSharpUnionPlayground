import playground.dcb.Dcb;
import java.util.*;
import java.util.concurrent.*;
final class DcbTests{
    record Counter(String key,int delta){}
    static Dcb.Tag tag(String key){return new Dcb.Tag("counter",List.of(key));}
    static Dcb.Query query(String key){return new Dcb.Query(List.of(new Dcb.QueryItem(List.of(tag(key)),List.of())));}
    static Dcb.EventDescriptor describe(Counter e){return new Dcb.EventDescriptor("CounterDelta",List.of(tag(e.key()),new Dcb.Tag("scope",List.of("test"))));}
    static void check(boolean value){if(!value)throw new IllegalStateException("DCB library probe failed");}
    static void run()throws Exception{
        var store=new Dcb.EventStore<Counter>(DcbTests::describe,e->e);var p=new Dcb.LiveProjection<Counter,Integer>(0,(s,e)->s+e.delta(),s->s);store.subscribe(p::accept);
        store.subscribe(events->{if(!events.isEmpty())throw new AssertionError("expected listener failure");return true;});
        var first=store.append("first",query("a"),0,List.of(new Counter("a",1),new Counter("a",2)));check(first.committed()&&first.events().size()==2&&store.listenerFaults()==1&&p.snapshot().state()==3&&p.snapshot().position()==2);
        check(store.append("other",query("b"),0,List.of(new Counter("b",9))).committed());check(!store.append("stale",query("a"),0,List.of(new Counter("a",99))).committed());
        var all=store.read(new Dcb.Query(List.of()));check(all.after()==3&&p.snapshot().state()==12);
        check(store.read(new Dcb.Query(List.of(new Dcb.QueryItem(List.of(tag("a"),new Dcb.Tag("scope",List.of("test"))),List.of())))).events().size()==2);
        check(store.read(new Dcb.Query(List.of(query("a").items().getFirst(),query("b").items().getFirst()))).events().size()==3);
        check(store.read(new Dcb.Query(List.of(new Dcb.QueryItem(List.of(tag("a")),List.of("OtherType"))))).events().isEmpty());
        check(p.accept(all.events())&&p.snapshot().state()==12);var gap=new Dcb.LiveProjection<Counter,Integer>(0,(s,e)->s+e.delta(),s->s);check(!gap.accept(List.of(all.events().get(0),all.events().get(2)))&&gap.snapshot().position()==0&&gap.snapshot().state()==0);
        var late=new Dcb.LiveProjection<Counter,Integer>(0,(s,e)->s+e.delta(),s->s);store.subscribe(late::accept);check(late.snapshot().state()==12);
        var processor=new Dcb.CommandProcessor<Counter,Counter,String>(store,c->query(c.key()),(events,c)->c.delta()>0?new Dcb.Emit<>(List.of(c)):new Dcb.Reject<>("NegativeDelta"),3);
        var ticket=processor.prepare("retry",new Counter("a",1));store.append("interfere",query("a"),3,List.of(new Counter("a",1)));var result=processor.commit(ticket);check(result.status().equals("Committed")&&result.conflicts()==1&&p.snapshot().state()==14&&late.snapshot().state()==14&&p.snapshot().position()==5);
        check(processor.commit(processor.prepare("reject",new Counter("a",-1))).status().equals("Rejected"));
        var restored=new Dcb.EventStore<Counter>(DcbTests::describe,e->e);restored.load(store.read(new Dcb.Query(List.of())).events());check(restored.read(new Dcb.Query(List.of())).after()==5);
        var e=all.events().getFirst();boolean refused=false;try{new Dcb.EventStore<Counter>(DcbTests::describe,x->x).load(List.of(new Dcb.Recorded<>(e.position(),e.commandId(),e.eventType(),List.of(),e.payload())));}catch(IllegalArgumentException expected){refused=true;}check(refused);
        var racing=new Dcb.EventStore<Counter>(DcbTests::describe,x->x);var barrier=new CyclicBarrier(2);
        try(var pool=Executors.newFixedThreadPool(2)){
            var tasks=new ArrayList<Future<Boolean>>();for(int i=0;i<2;i++){final int writer=i;tasks.add(pool.submit(()->{var read=racing.read(query("a"));barrier.await();return racing.append("race-"+writer,query("a"),read.after(),List.of(new Counter("a",1))).committed();}));}
            int wins=0;for(var task:tasks)if(task.get())wins++;check(wins==1&&racing.read(new Dcb.Query(List.of())).after()==1);
        }
    }
}
