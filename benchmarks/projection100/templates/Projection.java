import java.util.*;
class Projection{
 interface EventPayload{}
 record Unknown()implements EventPayload{}
 record EventData(int id,long value,String label){}
@@TYPES@@
 static class State{int id;long balance,counter,seen,revision;String name="";boolean exists,flag,archived;ArrayList<Long>items=new ArrayList<>();Map<Long,Long>tags=new HashMap<>();State(int id){this.id=id;}}
 static class Store{Map<Integer,State>map=new HashMap<>();ArrayList<State>list=new ArrayList<>();long ignored;}
 static EventPayload create(int i,EventData data){return switch(i){
@@CREATE@@
default->throw new IllegalArgumentException();};}
 static void route(int domain,EventPayload p,Store[]stores,String mode){switch(domain){
@@ROUTE@@
}}
@@HANDLERS@@
 static void update(Store store,EventData e,int kind,String mode){State s=null;
 if(mode.equals("linear-append")){for(var v:store.list)if(v.id==e.id()){s=v;break;}if(s==null){s=new State(e.id());store.list.add(s);}}else{s=store.map.computeIfAbsent(e.id(),State::new);}
 switch(kind){case 0->s.exists=true;case 1->s.name=e.label();case 2->s.balance+=e.value();case 3->{if(mode.equals("indexed-copy")){var copy=new ArrayList<>(s.items);copy.add(e.value());s.items=copy;}else s.items.add(e.value());}case 4->s.flag=!s.flag;case 5->s.counter+=e.value();case 6->s.tags.put(e.value()%16,e.value());case 7->s.seen=e.value();case 8->s.revision=e.value();case 9->s.archived=true;}
 }
 public static void main(String[]args){System.out.println("{\"event\":\"ready\"}");System.out.flush();int events=Integer.parseInt(args[0]),aggregates=Integer.parseInt(args[1]);String mode=args[2],routing=args[3];var stores=new Store[10];Arrays.setAll(stores,i->new Store());System.out.println("{\"event\":\"prepared\"}");System.out.flush();long start=System.nanoTime();
 for(int i=0;i<events;i++){var p=create(i%100,new EventData((i/100)%aggregates,i+1,i%10==1?"name-"+(i+1):""));if(routing.equals("fanout"))for(int d=0;d<10;d++)route(d,p,stores,mode);else route((i%100)/10,p,stores,mode);}for(int d=0;d<10;d++)route(d,new Unknown(),stores,mode);double elapsed=(System.nanoTime()-start)/1e9;
 long sum=0,items=0,states=0,maxHistory=0,ignored=0;for(int d=0;d<10;d++){var store=stores[d];ignored+=store.ignored;for(var s:mode.equals("linear-append")?store.list:store.map.values()){states++;items+=s.items.size();maxHistory=Math.max(maxHistory,s.items.size());sum+=s.id+d+s.balance+s.counter+s.seen+s.revision+s.name.length()+(s.exists?1:0)+(s.flag?1:0)+(s.archived?1:0)+s.items.size();for(long v:s.items)sum+=v;for(var e:s.tags.entrySet())sum+=e.getKey()+e.getValue();}}
 System.out.printf(Locale.ROOT,"{\"roots\":%d,\"samples\":1,\"sink\":%d,\"stateCount\":%d,\"retainedItems\":%d,\"maxHistoryLength\":%d,\"ignored\":%d,\"measurements\":[{\"encodeSeconds\":0,\"decodeSeconds\":0,\"switchSeconds\":%.9f,\"totalSeconds\":%.9f,\"bytes\":%d}]}%n",events,sum,states,items,maxHistory,ignored,elapsed,elapsed,Math.max(1,items*8));
 }
}
