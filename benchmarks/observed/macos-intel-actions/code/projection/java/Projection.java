import java.util.*;
class Projection{
 interface EventPayload{}
 record Unknown()implements EventPayload{}
 record EventData(int id,long value,String label){}
record D00Created(EventData data) implements EventPayload {}
record D00Renamed(EventData data) implements EventPayload {}
record D00BalanceAdded(EventData data) implements EventPayload {}
record D00ItemAppended(EventData data) implements EventPayload {}
record D00FlagToggled(EventData data) implements EventPayload {}
record D00CountAdded(EventData data) implements EventPayload {}
record D00TagChanged(EventData data) implements EventPayload {}
record D00Seen(EventData data) implements EventPayload {}
record D00Revised(EventData data) implements EventPayload {}
record D00Archived(EventData data) implements EventPayload {}
record D01Created(EventData data) implements EventPayload {}
record D01Renamed(EventData data) implements EventPayload {}
record D01BalanceAdded(EventData data) implements EventPayload {}
record D01ItemAppended(EventData data) implements EventPayload {}
record D01FlagToggled(EventData data) implements EventPayload {}
record D01CountAdded(EventData data) implements EventPayload {}
record D01TagChanged(EventData data) implements EventPayload {}
record D01Seen(EventData data) implements EventPayload {}
record D01Revised(EventData data) implements EventPayload {}
record D01Archived(EventData data) implements EventPayload {}
record D02Created(EventData data) implements EventPayload {}
record D02Renamed(EventData data) implements EventPayload {}
record D02BalanceAdded(EventData data) implements EventPayload {}
record D02ItemAppended(EventData data) implements EventPayload {}
record D02FlagToggled(EventData data) implements EventPayload {}
record D02CountAdded(EventData data) implements EventPayload {}
record D02TagChanged(EventData data) implements EventPayload {}
record D02Seen(EventData data) implements EventPayload {}
record D02Revised(EventData data) implements EventPayload {}
record D02Archived(EventData data) implements EventPayload {}
record D03Created(EventData data) implements EventPayload {}
record D03Renamed(EventData data) implements EventPayload {}
record D03BalanceAdded(EventData data) implements EventPayload {}
record D03ItemAppended(EventData data) implements EventPayload {}
record D03FlagToggled(EventData data) implements EventPayload {}
record D03CountAdded(EventData data) implements EventPayload {}
record D03TagChanged(EventData data) implements EventPayload {}
record D03Seen(EventData data) implements EventPayload {}
record D03Revised(EventData data) implements EventPayload {}
record D03Archived(EventData data) implements EventPayload {}
record D04Created(EventData data) implements EventPayload {}
record D04Renamed(EventData data) implements EventPayload {}
record D04BalanceAdded(EventData data) implements EventPayload {}
record D04ItemAppended(EventData data) implements EventPayload {}
record D04FlagToggled(EventData data) implements EventPayload {}
record D04CountAdded(EventData data) implements EventPayload {}
record D04TagChanged(EventData data) implements EventPayload {}
record D04Seen(EventData data) implements EventPayload {}
record D04Revised(EventData data) implements EventPayload {}
record D04Archived(EventData data) implements EventPayload {}
record D05Created(EventData data) implements EventPayload {}
record D05Renamed(EventData data) implements EventPayload {}
record D05BalanceAdded(EventData data) implements EventPayload {}
record D05ItemAppended(EventData data) implements EventPayload {}
record D05FlagToggled(EventData data) implements EventPayload {}
record D05CountAdded(EventData data) implements EventPayload {}
record D05TagChanged(EventData data) implements EventPayload {}
record D05Seen(EventData data) implements EventPayload {}
record D05Revised(EventData data) implements EventPayload {}
record D05Archived(EventData data) implements EventPayload {}
record D06Created(EventData data) implements EventPayload {}
record D06Renamed(EventData data) implements EventPayload {}
record D06BalanceAdded(EventData data) implements EventPayload {}
record D06ItemAppended(EventData data) implements EventPayload {}
record D06FlagToggled(EventData data) implements EventPayload {}
record D06CountAdded(EventData data) implements EventPayload {}
record D06TagChanged(EventData data) implements EventPayload {}
record D06Seen(EventData data) implements EventPayload {}
record D06Revised(EventData data) implements EventPayload {}
record D06Archived(EventData data) implements EventPayload {}
record D07Created(EventData data) implements EventPayload {}
record D07Renamed(EventData data) implements EventPayload {}
record D07BalanceAdded(EventData data) implements EventPayload {}
record D07ItemAppended(EventData data) implements EventPayload {}
record D07FlagToggled(EventData data) implements EventPayload {}
record D07CountAdded(EventData data) implements EventPayload {}
record D07TagChanged(EventData data) implements EventPayload {}
record D07Seen(EventData data) implements EventPayload {}
record D07Revised(EventData data) implements EventPayload {}
record D07Archived(EventData data) implements EventPayload {}
record D08Created(EventData data) implements EventPayload {}
record D08Renamed(EventData data) implements EventPayload {}
record D08BalanceAdded(EventData data) implements EventPayload {}
record D08ItemAppended(EventData data) implements EventPayload {}
record D08FlagToggled(EventData data) implements EventPayload {}
record D08CountAdded(EventData data) implements EventPayload {}
record D08TagChanged(EventData data) implements EventPayload {}
record D08Seen(EventData data) implements EventPayload {}
record D08Revised(EventData data) implements EventPayload {}
record D08Archived(EventData data) implements EventPayload {}
record D09Created(EventData data) implements EventPayload {}
record D09Renamed(EventData data) implements EventPayload {}
record D09BalanceAdded(EventData data) implements EventPayload {}
record D09ItemAppended(EventData data) implements EventPayload {}
record D09FlagToggled(EventData data) implements EventPayload {}
record D09CountAdded(EventData data) implements EventPayload {}
record D09TagChanged(EventData data) implements EventPayload {}
record D09Seen(EventData data) implements EventPayload {}
record D09Revised(EventData data) implements EventPayload {}
record D09Archived(EventData data) implements EventPayload {}
 static class State{int id;long balance,counter,seen,revision;String name="";boolean exists,flag,archived;ArrayList<Long>items=new ArrayList<>();Map<Long,Long>tags=new HashMap<>();State(int id){this.id=id;}}
 static class Store{Map<Integer,State>map=new HashMap<>();ArrayList<State>list=new ArrayList<>();long ignored;}
 static EventPayload create(int i,EventData data){return switch(i){
case 0->new D00Created(data);
case 1->new D00Renamed(data);
case 2->new D00BalanceAdded(data);
case 3->new D00ItemAppended(data);
case 4->new D00FlagToggled(data);
case 5->new D00CountAdded(data);
case 6->new D00TagChanged(data);
case 7->new D00Seen(data);
case 8->new D00Revised(data);
case 9->new D00Archived(data);
case 10->new D01Created(data);
case 11->new D01Renamed(data);
case 12->new D01BalanceAdded(data);
case 13->new D01ItemAppended(data);
case 14->new D01FlagToggled(data);
case 15->new D01CountAdded(data);
case 16->new D01TagChanged(data);
case 17->new D01Seen(data);
case 18->new D01Revised(data);
case 19->new D01Archived(data);
case 20->new D02Created(data);
case 21->new D02Renamed(data);
case 22->new D02BalanceAdded(data);
case 23->new D02ItemAppended(data);
case 24->new D02FlagToggled(data);
case 25->new D02CountAdded(data);
case 26->new D02TagChanged(data);
case 27->new D02Seen(data);
case 28->new D02Revised(data);
case 29->new D02Archived(data);
case 30->new D03Created(data);
case 31->new D03Renamed(data);
case 32->new D03BalanceAdded(data);
case 33->new D03ItemAppended(data);
case 34->new D03FlagToggled(data);
case 35->new D03CountAdded(data);
case 36->new D03TagChanged(data);
case 37->new D03Seen(data);
case 38->new D03Revised(data);
case 39->new D03Archived(data);
case 40->new D04Created(data);
case 41->new D04Renamed(data);
case 42->new D04BalanceAdded(data);
case 43->new D04ItemAppended(data);
case 44->new D04FlagToggled(data);
case 45->new D04CountAdded(data);
case 46->new D04TagChanged(data);
case 47->new D04Seen(data);
case 48->new D04Revised(data);
case 49->new D04Archived(data);
case 50->new D05Created(data);
case 51->new D05Renamed(data);
case 52->new D05BalanceAdded(data);
case 53->new D05ItemAppended(data);
case 54->new D05FlagToggled(data);
case 55->new D05CountAdded(data);
case 56->new D05TagChanged(data);
case 57->new D05Seen(data);
case 58->new D05Revised(data);
case 59->new D05Archived(data);
case 60->new D06Created(data);
case 61->new D06Renamed(data);
case 62->new D06BalanceAdded(data);
case 63->new D06ItemAppended(data);
case 64->new D06FlagToggled(data);
case 65->new D06CountAdded(data);
case 66->new D06TagChanged(data);
case 67->new D06Seen(data);
case 68->new D06Revised(data);
case 69->new D06Archived(data);
case 70->new D07Created(data);
case 71->new D07Renamed(data);
case 72->new D07BalanceAdded(data);
case 73->new D07ItemAppended(data);
case 74->new D07FlagToggled(data);
case 75->new D07CountAdded(data);
case 76->new D07TagChanged(data);
case 77->new D07Seen(data);
case 78->new D07Revised(data);
case 79->new D07Archived(data);
case 80->new D08Created(data);
case 81->new D08Renamed(data);
case 82->new D08BalanceAdded(data);
case 83->new D08ItemAppended(data);
case 84->new D08FlagToggled(data);
case 85->new D08CountAdded(data);
case 86->new D08TagChanged(data);
case 87->new D08Seen(data);
case 88->new D08Revised(data);
case 89->new D08Archived(data);
case 90->new D09Created(data);
case 91->new D09Renamed(data);
case 92->new D09BalanceAdded(data);
case 93->new D09ItemAppended(data);
case 94->new D09FlagToggled(data);
case 95->new D09CountAdded(data);
case 96->new D09TagChanged(data);
case 97->new D09Seen(data);
case 98->new D09Revised(data);
case 99->new D09Archived(data);
default->throw new IllegalArgumentException();};}
 static void route(int domain,EventPayload p,Store[]stores,String mode){switch(domain){
case 0->apply0(p,stores[0],mode);
case 1->apply1(p,stores[1],mode);
case 2->apply2(p,stores[2],mode);
case 3->apply3(p,stores[3],mode);
case 4->apply4(p,stores[4],mode);
case 5->apply5(p,stores[5],mode);
case 6->apply6(p,stores[6],mode);
case 7->apply7(p,stores[7],mode);
case 8->apply8(p,stores[8],mode);
case 9->apply9(p,stores[9],mode);
}}
static void apply0(EventPayload p,Store s,String mode) {switch(p){
case D00Created e -> update(s,e.data(),0,mode);
case D00Renamed e -> update(s,e.data(),1,mode);
case D00BalanceAdded e -> update(s,e.data(),2,mode);
case D00ItemAppended e -> update(s,e.data(),3,mode);
case D00FlagToggled e -> update(s,e.data(),4,mode);
case D00CountAdded e -> update(s,e.data(),5,mode);
case D00TagChanged e -> update(s,e.data(),6,mode);
case D00Seen e -> update(s,e.data(),7,mode);
case D00Revised e -> update(s,e.data(),8,mode);
case D00Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply1(EventPayload p,Store s,String mode) {switch(p){
case D01Created e -> update(s,e.data(),0,mode);
case D01Renamed e -> update(s,e.data(),1,mode);
case D01BalanceAdded e -> update(s,e.data(),2,mode);
case D01ItemAppended e -> update(s,e.data(),3,mode);
case D01FlagToggled e -> update(s,e.data(),4,mode);
case D01CountAdded e -> update(s,e.data(),5,mode);
case D01TagChanged e -> update(s,e.data(),6,mode);
case D01Seen e -> update(s,e.data(),7,mode);
case D01Revised e -> update(s,e.data(),8,mode);
case D01Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply2(EventPayload p,Store s,String mode) {switch(p){
case D02Created e -> update(s,e.data(),0,mode);
case D02Renamed e -> update(s,e.data(),1,mode);
case D02BalanceAdded e -> update(s,e.data(),2,mode);
case D02ItemAppended e -> update(s,e.data(),3,mode);
case D02FlagToggled e -> update(s,e.data(),4,mode);
case D02CountAdded e -> update(s,e.data(),5,mode);
case D02TagChanged e -> update(s,e.data(),6,mode);
case D02Seen e -> update(s,e.data(),7,mode);
case D02Revised e -> update(s,e.data(),8,mode);
case D02Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply3(EventPayload p,Store s,String mode) {switch(p){
case D03Created e -> update(s,e.data(),0,mode);
case D03Renamed e -> update(s,e.data(),1,mode);
case D03BalanceAdded e -> update(s,e.data(),2,mode);
case D03ItemAppended e -> update(s,e.data(),3,mode);
case D03FlagToggled e -> update(s,e.data(),4,mode);
case D03CountAdded e -> update(s,e.data(),5,mode);
case D03TagChanged e -> update(s,e.data(),6,mode);
case D03Seen e -> update(s,e.data(),7,mode);
case D03Revised e -> update(s,e.data(),8,mode);
case D03Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply4(EventPayload p,Store s,String mode) {switch(p){
case D04Created e -> update(s,e.data(),0,mode);
case D04Renamed e -> update(s,e.data(),1,mode);
case D04BalanceAdded e -> update(s,e.data(),2,mode);
case D04ItemAppended e -> update(s,e.data(),3,mode);
case D04FlagToggled e -> update(s,e.data(),4,mode);
case D04CountAdded e -> update(s,e.data(),5,mode);
case D04TagChanged e -> update(s,e.data(),6,mode);
case D04Seen e -> update(s,e.data(),7,mode);
case D04Revised e -> update(s,e.data(),8,mode);
case D04Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply5(EventPayload p,Store s,String mode) {switch(p){
case D05Created e -> update(s,e.data(),0,mode);
case D05Renamed e -> update(s,e.data(),1,mode);
case D05BalanceAdded e -> update(s,e.data(),2,mode);
case D05ItemAppended e -> update(s,e.data(),3,mode);
case D05FlagToggled e -> update(s,e.data(),4,mode);
case D05CountAdded e -> update(s,e.data(),5,mode);
case D05TagChanged e -> update(s,e.data(),6,mode);
case D05Seen e -> update(s,e.data(),7,mode);
case D05Revised e -> update(s,e.data(),8,mode);
case D05Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply6(EventPayload p,Store s,String mode) {switch(p){
case D06Created e -> update(s,e.data(),0,mode);
case D06Renamed e -> update(s,e.data(),1,mode);
case D06BalanceAdded e -> update(s,e.data(),2,mode);
case D06ItemAppended e -> update(s,e.data(),3,mode);
case D06FlagToggled e -> update(s,e.data(),4,mode);
case D06CountAdded e -> update(s,e.data(),5,mode);
case D06TagChanged e -> update(s,e.data(),6,mode);
case D06Seen e -> update(s,e.data(),7,mode);
case D06Revised e -> update(s,e.data(),8,mode);
case D06Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply7(EventPayload p,Store s,String mode) {switch(p){
case D07Created e -> update(s,e.data(),0,mode);
case D07Renamed e -> update(s,e.data(),1,mode);
case D07BalanceAdded e -> update(s,e.data(),2,mode);
case D07ItemAppended e -> update(s,e.data(),3,mode);
case D07FlagToggled e -> update(s,e.data(),4,mode);
case D07CountAdded e -> update(s,e.data(),5,mode);
case D07TagChanged e -> update(s,e.data(),6,mode);
case D07Seen e -> update(s,e.data(),7,mode);
case D07Revised e -> update(s,e.data(),8,mode);
case D07Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply8(EventPayload p,Store s,String mode) {switch(p){
case D08Created e -> update(s,e.data(),0,mode);
case D08Renamed e -> update(s,e.data(),1,mode);
case D08BalanceAdded e -> update(s,e.data(),2,mode);
case D08ItemAppended e -> update(s,e.data(),3,mode);
case D08FlagToggled e -> update(s,e.data(),4,mode);
case D08CountAdded e -> update(s,e.data(),5,mode);
case D08TagChanged e -> update(s,e.data(),6,mode);
case D08Seen e -> update(s,e.data(),7,mode);
case D08Revised e -> update(s,e.data(),8,mode);
case D08Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
static void apply9(EventPayload p,Store s,String mode) {switch(p){
case D09Created e -> update(s,e.data(),0,mode);
case D09Renamed e -> update(s,e.data(),1,mode);
case D09BalanceAdded e -> update(s,e.data(),2,mode);
case D09ItemAppended e -> update(s,e.data(),3,mode);
case D09FlagToggled e -> update(s,e.data(),4,mode);
case D09CountAdded e -> update(s,e.data(),5,mode);
case D09TagChanged e -> update(s,e.data(),6,mode);
case D09Seen e -> update(s,e.data(),7,mode);
case D09Revised e -> update(s,e.data(),8,mode);
case D09Archived e -> update(s,e.data(),9,mode);
default -> s.ignored++;}}
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
