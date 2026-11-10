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
struct D00Created:EventPayload{let data:EventData}
struct D00Renamed:EventPayload{let data:EventData}
struct D00BalanceAdded:EventPayload{let data:EventData}
struct D00ItemAppended:EventPayload{let data:EventData}
struct D00FlagToggled:EventPayload{let data:EventData}
struct D00CountAdded:EventPayload{let data:EventData}
struct D00TagChanged:EventPayload{let data:EventData}
struct D00Seen:EventPayload{let data:EventData}
struct D00Revised:EventPayload{let data:EventData}
struct D00Archived:EventPayload{let data:EventData}
struct D01Created:EventPayload{let data:EventData}
struct D01Renamed:EventPayload{let data:EventData}
struct D01BalanceAdded:EventPayload{let data:EventData}
struct D01ItemAppended:EventPayload{let data:EventData}
struct D01FlagToggled:EventPayload{let data:EventData}
struct D01CountAdded:EventPayload{let data:EventData}
struct D01TagChanged:EventPayload{let data:EventData}
struct D01Seen:EventPayload{let data:EventData}
struct D01Revised:EventPayload{let data:EventData}
struct D01Archived:EventPayload{let data:EventData}
struct D02Created:EventPayload{let data:EventData}
struct D02Renamed:EventPayload{let data:EventData}
struct D02BalanceAdded:EventPayload{let data:EventData}
struct D02ItemAppended:EventPayload{let data:EventData}
struct D02FlagToggled:EventPayload{let data:EventData}
struct D02CountAdded:EventPayload{let data:EventData}
struct D02TagChanged:EventPayload{let data:EventData}
struct D02Seen:EventPayload{let data:EventData}
struct D02Revised:EventPayload{let data:EventData}
struct D02Archived:EventPayload{let data:EventData}
struct D03Created:EventPayload{let data:EventData}
struct D03Renamed:EventPayload{let data:EventData}
struct D03BalanceAdded:EventPayload{let data:EventData}
struct D03ItemAppended:EventPayload{let data:EventData}
struct D03FlagToggled:EventPayload{let data:EventData}
struct D03CountAdded:EventPayload{let data:EventData}
struct D03TagChanged:EventPayload{let data:EventData}
struct D03Seen:EventPayload{let data:EventData}
struct D03Revised:EventPayload{let data:EventData}
struct D03Archived:EventPayload{let data:EventData}
struct D04Created:EventPayload{let data:EventData}
struct D04Renamed:EventPayload{let data:EventData}
struct D04BalanceAdded:EventPayload{let data:EventData}
struct D04ItemAppended:EventPayload{let data:EventData}
struct D04FlagToggled:EventPayload{let data:EventData}
struct D04CountAdded:EventPayload{let data:EventData}
struct D04TagChanged:EventPayload{let data:EventData}
struct D04Seen:EventPayload{let data:EventData}
struct D04Revised:EventPayload{let data:EventData}
struct D04Archived:EventPayload{let data:EventData}
struct D05Created:EventPayload{let data:EventData}
struct D05Renamed:EventPayload{let data:EventData}
struct D05BalanceAdded:EventPayload{let data:EventData}
struct D05ItemAppended:EventPayload{let data:EventData}
struct D05FlagToggled:EventPayload{let data:EventData}
struct D05CountAdded:EventPayload{let data:EventData}
struct D05TagChanged:EventPayload{let data:EventData}
struct D05Seen:EventPayload{let data:EventData}
struct D05Revised:EventPayload{let data:EventData}
struct D05Archived:EventPayload{let data:EventData}
struct D06Created:EventPayload{let data:EventData}
struct D06Renamed:EventPayload{let data:EventData}
struct D06BalanceAdded:EventPayload{let data:EventData}
struct D06ItemAppended:EventPayload{let data:EventData}
struct D06FlagToggled:EventPayload{let data:EventData}
struct D06CountAdded:EventPayload{let data:EventData}
struct D06TagChanged:EventPayload{let data:EventData}
struct D06Seen:EventPayload{let data:EventData}
struct D06Revised:EventPayload{let data:EventData}
struct D06Archived:EventPayload{let data:EventData}
struct D07Created:EventPayload{let data:EventData}
struct D07Renamed:EventPayload{let data:EventData}
struct D07BalanceAdded:EventPayload{let data:EventData}
struct D07ItemAppended:EventPayload{let data:EventData}
struct D07FlagToggled:EventPayload{let data:EventData}
struct D07CountAdded:EventPayload{let data:EventData}
struct D07TagChanged:EventPayload{let data:EventData}
struct D07Seen:EventPayload{let data:EventData}
struct D07Revised:EventPayload{let data:EventData}
struct D07Archived:EventPayload{let data:EventData}
struct D08Created:EventPayload{let data:EventData}
struct D08Renamed:EventPayload{let data:EventData}
struct D08BalanceAdded:EventPayload{let data:EventData}
struct D08ItemAppended:EventPayload{let data:EventData}
struct D08FlagToggled:EventPayload{let data:EventData}
struct D08CountAdded:EventPayload{let data:EventData}
struct D08TagChanged:EventPayload{let data:EventData}
struct D08Seen:EventPayload{let data:EventData}
struct D08Revised:EventPayload{let data:EventData}
struct D08Archived:EventPayload{let data:EventData}
struct D09Created:EventPayload{let data:EventData}
struct D09Renamed:EventPayload{let data:EventData}
struct D09BalanceAdded:EventPayload{let data:EventData}
struct D09ItemAppended:EventPayload{let data:EventData}
struct D09FlagToggled:EventPayload{let data:EventData}
struct D09CountAdded:EventPayload{let data:EventData}
struct D09TagChanged:EventPayload{let data:EventData}
struct D09Seen:EventPayload{let data:EventData}
struct D09Revised:EventPayload{let data:EventData}
struct D09Archived:EventPayload{let data:EventData}
final class State{let id:Int;var balance:Int64=0,counter:Int64=0,seen:Int64=0,revision:Int64=0;var name="";var exists=false,flag=false,archived=false;var items:[Int64]=[];var tags:[Int64:Int64]=[:];init(_ id:Int){self.id=id}}
final class Store{var map:[Int:State]=[:];var list:[State]=[];var ignored:Int64=0}
func create(_ i:Int,_ data:EventData)->any EventPayload{switch i{
case 0:return D00Created(data:data)
case 1:return D00Renamed(data:data)
case 2:return D00BalanceAdded(data:data)
case 3:return D00ItemAppended(data:data)
case 4:return D00FlagToggled(data:data)
case 5:return D00CountAdded(data:data)
case 6:return D00TagChanged(data:data)
case 7:return D00Seen(data:data)
case 8:return D00Revised(data:data)
case 9:return D00Archived(data:data)
case 10:return D01Created(data:data)
case 11:return D01Renamed(data:data)
case 12:return D01BalanceAdded(data:data)
case 13:return D01ItemAppended(data:data)
case 14:return D01FlagToggled(data:data)
case 15:return D01CountAdded(data:data)
case 16:return D01TagChanged(data:data)
case 17:return D01Seen(data:data)
case 18:return D01Revised(data:data)
case 19:return D01Archived(data:data)
case 20:return D02Created(data:data)
case 21:return D02Renamed(data:data)
case 22:return D02BalanceAdded(data:data)
case 23:return D02ItemAppended(data:data)
case 24:return D02FlagToggled(data:data)
case 25:return D02CountAdded(data:data)
case 26:return D02TagChanged(data:data)
case 27:return D02Seen(data:data)
case 28:return D02Revised(data:data)
case 29:return D02Archived(data:data)
case 30:return D03Created(data:data)
case 31:return D03Renamed(data:data)
case 32:return D03BalanceAdded(data:data)
case 33:return D03ItemAppended(data:data)
case 34:return D03FlagToggled(data:data)
case 35:return D03CountAdded(data:data)
case 36:return D03TagChanged(data:data)
case 37:return D03Seen(data:data)
case 38:return D03Revised(data:data)
case 39:return D03Archived(data:data)
case 40:return D04Created(data:data)
case 41:return D04Renamed(data:data)
case 42:return D04BalanceAdded(data:data)
case 43:return D04ItemAppended(data:data)
case 44:return D04FlagToggled(data:data)
case 45:return D04CountAdded(data:data)
case 46:return D04TagChanged(data:data)
case 47:return D04Seen(data:data)
case 48:return D04Revised(data:data)
case 49:return D04Archived(data:data)
case 50:return D05Created(data:data)
case 51:return D05Renamed(data:data)
case 52:return D05BalanceAdded(data:data)
case 53:return D05ItemAppended(data:data)
case 54:return D05FlagToggled(data:data)
case 55:return D05CountAdded(data:data)
case 56:return D05TagChanged(data:data)
case 57:return D05Seen(data:data)
case 58:return D05Revised(data:data)
case 59:return D05Archived(data:data)
case 60:return D06Created(data:data)
case 61:return D06Renamed(data:data)
case 62:return D06BalanceAdded(data:data)
case 63:return D06ItemAppended(data:data)
case 64:return D06FlagToggled(data:data)
case 65:return D06CountAdded(data:data)
case 66:return D06TagChanged(data:data)
case 67:return D06Seen(data:data)
case 68:return D06Revised(data:data)
case 69:return D06Archived(data:data)
case 70:return D07Created(data:data)
case 71:return D07Renamed(data:data)
case 72:return D07BalanceAdded(data:data)
case 73:return D07ItemAppended(data:data)
case 74:return D07FlagToggled(data:data)
case 75:return D07CountAdded(data:data)
case 76:return D07TagChanged(data:data)
case 77:return D07Seen(data:data)
case 78:return D07Revised(data:data)
case 79:return D07Archived(data:data)
case 80:return D08Created(data:data)
case 81:return D08Renamed(data:data)
case 82:return D08BalanceAdded(data:data)
case 83:return D08ItemAppended(data:data)
case 84:return D08FlagToggled(data:data)
case 85:return D08CountAdded(data:data)
case 86:return D08TagChanged(data:data)
case 87:return D08Seen(data:data)
case 88:return D08Revised(data:data)
case 89:return D08Archived(data:data)
case 90:return D09Created(data:data)
case 91:return D09Renamed(data:data)
case 92:return D09BalanceAdded(data:data)
case 93:return D09ItemAppended(data:data)
case 94:return D09FlagToggled(data:data)
case 95:return D09CountAdded(data:data)
case 96:return D09TagChanged(data:data)
case 97:return D09Seen(data:data)
case 98:return D09Revised(data:data)
case 99:return D09Archived(data:data)
default:fatalError("Unknown")}}
func route(_ domain:Int,_ p:any EventPayload,_ stores:[Store],_ mode:String){switch domain{
case 0:apply0(p,stores[0],mode)
case 1:apply1(p,stores[1],mode)
case 2:apply2(p,stores[2],mode)
case 3:apply3(p,stores[3],mode)
case 4:apply4(p,stores[4],mode)
case 5:apply5(p,stores[5],mode)
case 6:apply6(p,stores[6],mode)
case 7:apply7(p,stores[7],mode)
case 8:apply8(p,stores[8],mode)
case 9:apply9(p,stores[9],mode)
default:fatalError("Unknown")}}
func apply0(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D00Created: update(s,e.data,0,mode)
case let e as D00Renamed: update(s,e.data,1,mode)
case let e as D00BalanceAdded: update(s,e.data,2,mode)
case let e as D00ItemAppended: update(s,e.data,3,mode)
case let e as D00FlagToggled: update(s,e.data,4,mode)
case let e as D00CountAdded: update(s,e.data,5,mode)
case let e as D00TagChanged: update(s,e.data,6,mode)
case let e as D00Seen: update(s,e.data,7,mode)
case let e as D00Revised: update(s,e.data,8,mode)
case let e as D00Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply1(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D01Created: update(s,e.data,0,mode)
case let e as D01Renamed: update(s,e.data,1,mode)
case let e as D01BalanceAdded: update(s,e.data,2,mode)
case let e as D01ItemAppended: update(s,e.data,3,mode)
case let e as D01FlagToggled: update(s,e.data,4,mode)
case let e as D01CountAdded: update(s,e.data,5,mode)
case let e as D01TagChanged: update(s,e.data,6,mode)
case let e as D01Seen: update(s,e.data,7,mode)
case let e as D01Revised: update(s,e.data,8,mode)
case let e as D01Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply2(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D02Created: update(s,e.data,0,mode)
case let e as D02Renamed: update(s,e.data,1,mode)
case let e as D02BalanceAdded: update(s,e.data,2,mode)
case let e as D02ItemAppended: update(s,e.data,3,mode)
case let e as D02FlagToggled: update(s,e.data,4,mode)
case let e as D02CountAdded: update(s,e.data,5,mode)
case let e as D02TagChanged: update(s,e.data,6,mode)
case let e as D02Seen: update(s,e.data,7,mode)
case let e as D02Revised: update(s,e.data,8,mode)
case let e as D02Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply3(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D03Created: update(s,e.data,0,mode)
case let e as D03Renamed: update(s,e.data,1,mode)
case let e as D03BalanceAdded: update(s,e.data,2,mode)
case let e as D03ItemAppended: update(s,e.data,3,mode)
case let e as D03FlagToggled: update(s,e.data,4,mode)
case let e as D03CountAdded: update(s,e.data,5,mode)
case let e as D03TagChanged: update(s,e.data,6,mode)
case let e as D03Seen: update(s,e.data,7,mode)
case let e as D03Revised: update(s,e.data,8,mode)
case let e as D03Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply4(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D04Created: update(s,e.data,0,mode)
case let e as D04Renamed: update(s,e.data,1,mode)
case let e as D04BalanceAdded: update(s,e.data,2,mode)
case let e as D04ItemAppended: update(s,e.data,3,mode)
case let e as D04FlagToggled: update(s,e.data,4,mode)
case let e as D04CountAdded: update(s,e.data,5,mode)
case let e as D04TagChanged: update(s,e.data,6,mode)
case let e as D04Seen: update(s,e.data,7,mode)
case let e as D04Revised: update(s,e.data,8,mode)
case let e as D04Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply5(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D05Created: update(s,e.data,0,mode)
case let e as D05Renamed: update(s,e.data,1,mode)
case let e as D05BalanceAdded: update(s,e.data,2,mode)
case let e as D05ItemAppended: update(s,e.data,3,mode)
case let e as D05FlagToggled: update(s,e.data,4,mode)
case let e as D05CountAdded: update(s,e.data,5,mode)
case let e as D05TagChanged: update(s,e.data,6,mode)
case let e as D05Seen: update(s,e.data,7,mode)
case let e as D05Revised: update(s,e.data,8,mode)
case let e as D05Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply6(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D06Created: update(s,e.data,0,mode)
case let e as D06Renamed: update(s,e.data,1,mode)
case let e as D06BalanceAdded: update(s,e.data,2,mode)
case let e as D06ItemAppended: update(s,e.data,3,mode)
case let e as D06FlagToggled: update(s,e.data,4,mode)
case let e as D06CountAdded: update(s,e.data,5,mode)
case let e as D06TagChanged: update(s,e.data,6,mode)
case let e as D06Seen: update(s,e.data,7,mode)
case let e as D06Revised: update(s,e.data,8,mode)
case let e as D06Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply7(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D07Created: update(s,e.data,0,mode)
case let e as D07Renamed: update(s,e.data,1,mode)
case let e as D07BalanceAdded: update(s,e.data,2,mode)
case let e as D07ItemAppended: update(s,e.data,3,mode)
case let e as D07FlagToggled: update(s,e.data,4,mode)
case let e as D07CountAdded: update(s,e.data,5,mode)
case let e as D07TagChanged: update(s,e.data,6,mode)
case let e as D07Seen: update(s,e.data,7,mode)
case let e as D07Revised: update(s,e.data,8,mode)
case let e as D07Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply8(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D08Created: update(s,e.data,0,mode)
case let e as D08Renamed: update(s,e.data,1,mode)
case let e as D08BalanceAdded: update(s,e.data,2,mode)
case let e as D08ItemAppended: update(s,e.data,3,mode)
case let e as D08FlagToggled: update(s,e.data,4,mode)
case let e as D08CountAdded: update(s,e.data,5,mode)
case let e as D08TagChanged: update(s,e.data,6,mode)
case let e as D08Seen: update(s,e.data,7,mode)
case let e as D08Revised: update(s,e.data,8,mode)
case let e as D08Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
func apply9(_ p:any EventPayload,_ s:Store,_ mode:String){switch p{
case let e as D09Created: update(s,e.data,0,mode)
case let e as D09Renamed: update(s,e.data,1,mode)
case let e as D09BalanceAdded: update(s,e.data,2,mode)
case let e as D09ItemAppended: update(s,e.data,3,mode)
case let e as D09FlagToggled: update(s,e.data,4,mode)
case let e as D09CountAdded: update(s,e.data,5,mode)
case let e as D09TagChanged: update(s,e.data,6,mode)
case let e as D09Seen: update(s,e.data,7,mode)
case let e as D09Revised: update(s,e.data,8,mode)
case let e as D09Archived: update(s,e.data,9,mode)
default:s.ignored+=1}}
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
