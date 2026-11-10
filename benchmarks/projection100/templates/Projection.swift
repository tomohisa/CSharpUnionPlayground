#if canImport(Darwin)
import Darwin
#elseif canImport(WASILibc)
import WASILibc
#else
import Glibc
#endif
struct EventData{let id:Int;let value:Int64;let label:String}
protocol EventPayload{}
struct Unknown:EventPayload{}
@@TYPES@@
final class State{let id:Int;var balance:Int64=0,counter:Int64=0,seen:Int64=0,revision:Int64=0;var name="";var exists=false,flag=false,archived=false;var items:[Int64]=[];var tags:[Int64:Int64]=[:];init(_ id:Int){self.id=id}}
final class Store{var map:[Int:State]=[:];var list:[State]=[];var ignored:Int64=0}
func create(_ i:Int,_ data:EventData)->any EventPayload{switch i{
@@CREATE@@
default:fatalError("Unknown")}}
func route(_ domain:Int,_ p:any EventPayload,_ stores:[Store],_ mode:String){switch domain{
@@ROUTE@@
default:fatalError("Unknown")}}
@@HANDLERS@@
func update(_ store:Store,_ e:EventData,_ kind:Int,_ mode:String){
 let s:State
 if mode=="linear-append"{if let found=store.list.first(where:{$0.id==e.id}){s=found}else{s=State(e.id);store.list.append(s)}}else{if let found=store.map[e.id]{s=found}else{s=State(e.id);store.map[e.id]=s}}
 switch kind{case 0:s.exists=true;case 1:s.name=e.label;case 2:s.balance+=e.value;case 3:if mode=="indexed-copy"{s.items=s.items+[e.value]}else{s.items.append(e.value)};case 4:s.flag.toggle();case 5:s.counter+=e.value;case 6:s.tags[e.value%16]=e.value;case 7:s.seen=e.value;case 8:s.revision=e.value;case 9:s.archived=true;default:fatalError("Unknown")}
}
func emit(_ text:String){print(text);fflush(nil)}
let clock=ContinuousClock();let origin=clock.now
func now()->Double{let c=origin.duration(to:clock.now).components;return Double(c.seconds)+Double(c.attoseconds)/1e18}
emit("{\"event\":\"ready\"}")
let events=Int(CommandLine.arguments[1])!,aggregates=Int(CommandLine.arguments[2])!,mode=CommandLine.arguments[3],routing=CommandLine.arguments[4]
let stores=(0..<10).map{_ in Store()};emit("{\"event\":\"prepared\"}");let start=now()
for i in 0..<events{let p=create(i%100,EventData(id:(i/100)%aggregates,value:Int64(i+1),label:i%10==1 ? "name-\(i+1)":""));if routing=="fanout"{for d in 0..<10{route(d,p,stores,mode)}}else{route((i%100)/10,p,stores,mode)}}
for d in 0..<10{route(d,Unknown(),stores,mode)};let elapsed=now()-start
var sum:Int64=0,items=0,states=0,maxHistory=0,ignored:Int64=0
for(d,store)in stores.enumerated(){ignored+=store.ignored;let all=mode=="linear-append" ? store.list:Array(store.map.values);for s in all{states+=1;items+=s.items.count;maxHistory=max(maxHistory,s.items.count);sum+=Int64(s.id+d)+s.balance+s.counter+s.seen+s.revision+Int64(s.name.utf8.count+s.items.count);sum+=(s.exists ? 1:0)+(s.flag ? 1:0)+(s.archived ? 1:0);sum+=s.items.reduce(0,+);sum+=s.tags.reduce(0){$0+$1.key+$1.value}}}
emit("{\"roots\":\(events),\"samples\":1,\"sink\":\(sum),\"stateCount\":\(states),\"retainedItems\":\(items),\"maxHistoryLength\":\(maxHistory),\"ignored\":\(ignored),\"measurements\":[{\"encodeSeconds\":0,\"decodeSeconds\":0,\"switchSeconds\":\(elapsed),\"totalSeconds\":\(elapsed),\"bytes\":\(max(1,items*8))}]}")
