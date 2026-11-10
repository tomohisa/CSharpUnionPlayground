package main
import("encoding/json";"fmt";"os";"strconv";"time")
type EventPayload interface{isPayload()}
type Unknown struct{};func(Unknown)isPayload(){}
type EventData struct{Id int;Value int64;Label string}
type D00Created struct{Data EventData}
func(D00Created)isPayload(){}
type D00Renamed struct{Data EventData}
func(D00Renamed)isPayload(){}
type D00BalanceAdded struct{Data EventData}
func(D00BalanceAdded)isPayload(){}
type D00ItemAppended struct{Data EventData}
func(D00ItemAppended)isPayload(){}
type D00FlagToggled struct{Data EventData}
func(D00FlagToggled)isPayload(){}
type D00CountAdded struct{Data EventData}
func(D00CountAdded)isPayload(){}
type D00TagChanged struct{Data EventData}
func(D00TagChanged)isPayload(){}
type D00Seen struct{Data EventData}
func(D00Seen)isPayload(){}
type D00Revised struct{Data EventData}
func(D00Revised)isPayload(){}
type D00Archived struct{Data EventData}
func(D00Archived)isPayload(){}
type D01Created struct{Data EventData}
func(D01Created)isPayload(){}
type D01Renamed struct{Data EventData}
func(D01Renamed)isPayload(){}
type D01BalanceAdded struct{Data EventData}
func(D01BalanceAdded)isPayload(){}
type D01ItemAppended struct{Data EventData}
func(D01ItemAppended)isPayload(){}
type D01FlagToggled struct{Data EventData}
func(D01FlagToggled)isPayload(){}
type D01CountAdded struct{Data EventData}
func(D01CountAdded)isPayload(){}
type D01TagChanged struct{Data EventData}
func(D01TagChanged)isPayload(){}
type D01Seen struct{Data EventData}
func(D01Seen)isPayload(){}
type D01Revised struct{Data EventData}
func(D01Revised)isPayload(){}
type D01Archived struct{Data EventData}
func(D01Archived)isPayload(){}
type D02Created struct{Data EventData}
func(D02Created)isPayload(){}
type D02Renamed struct{Data EventData}
func(D02Renamed)isPayload(){}
type D02BalanceAdded struct{Data EventData}
func(D02BalanceAdded)isPayload(){}
type D02ItemAppended struct{Data EventData}
func(D02ItemAppended)isPayload(){}
type D02FlagToggled struct{Data EventData}
func(D02FlagToggled)isPayload(){}
type D02CountAdded struct{Data EventData}
func(D02CountAdded)isPayload(){}
type D02TagChanged struct{Data EventData}
func(D02TagChanged)isPayload(){}
type D02Seen struct{Data EventData}
func(D02Seen)isPayload(){}
type D02Revised struct{Data EventData}
func(D02Revised)isPayload(){}
type D02Archived struct{Data EventData}
func(D02Archived)isPayload(){}
type D03Created struct{Data EventData}
func(D03Created)isPayload(){}
type D03Renamed struct{Data EventData}
func(D03Renamed)isPayload(){}
type D03BalanceAdded struct{Data EventData}
func(D03BalanceAdded)isPayload(){}
type D03ItemAppended struct{Data EventData}
func(D03ItemAppended)isPayload(){}
type D03FlagToggled struct{Data EventData}
func(D03FlagToggled)isPayload(){}
type D03CountAdded struct{Data EventData}
func(D03CountAdded)isPayload(){}
type D03TagChanged struct{Data EventData}
func(D03TagChanged)isPayload(){}
type D03Seen struct{Data EventData}
func(D03Seen)isPayload(){}
type D03Revised struct{Data EventData}
func(D03Revised)isPayload(){}
type D03Archived struct{Data EventData}
func(D03Archived)isPayload(){}
type D04Created struct{Data EventData}
func(D04Created)isPayload(){}
type D04Renamed struct{Data EventData}
func(D04Renamed)isPayload(){}
type D04BalanceAdded struct{Data EventData}
func(D04BalanceAdded)isPayload(){}
type D04ItemAppended struct{Data EventData}
func(D04ItemAppended)isPayload(){}
type D04FlagToggled struct{Data EventData}
func(D04FlagToggled)isPayload(){}
type D04CountAdded struct{Data EventData}
func(D04CountAdded)isPayload(){}
type D04TagChanged struct{Data EventData}
func(D04TagChanged)isPayload(){}
type D04Seen struct{Data EventData}
func(D04Seen)isPayload(){}
type D04Revised struct{Data EventData}
func(D04Revised)isPayload(){}
type D04Archived struct{Data EventData}
func(D04Archived)isPayload(){}
type D05Created struct{Data EventData}
func(D05Created)isPayload(){}
type D05Renamed struct{Data EventData}
func(D05Renamed)isPayload(){}
type D05BalanceAdded struct{Data EventData}
func(D05BalanceAdded)isPayload(){}
type D05ItemAppended struct{Data EventData}
func(D05ItemAppended)isPayload(){}
type D05FlagToggled struct{Data EventData}
func(D05FlagToggled)isPayload(){}
type D05CountAdded struct{Data EventData}
func(D05CountAdded)isPayload(){}
type D05TagChanged struct{Data EventData}
func(D05TagChanged)isPayload(){}
type D05Seen struct{Data EventData}
func(D05Seen)isPayload(){}
type D05Revised struct{Data EventData}
func(D05Revised)isPayload(){}
type D05Archived struct{Data EventData}
func(D05Archived)isPayload(){}
type D06Created struct{Data EventData}
func(D06Created)isPayload(){}
type D06Renamed struct{Data EventData}
func(D06Renamed)isPayload(){}
type D06BalanceAdded struct{Data EventData}
func(D06BalanceAdded)isPayload(){}
type D06ItemAppended struct{Data EventData}
func(D06ItemAppended)isPayload(){}
type D06FlagToggled struct{Data EventData}
func(D06FlagToggled)isPayload(){}
type D06CountAdded struct{Data EventData}
func(D06CountAdded)isPayload(){}
type D06TagChanged struct{Data EventData}
func(D06TagChanged)isPayload(){}
type D06Seen struct{Data EventData}
func(D06Seen)isPayload(){}
type D06Revised struct{Data EventData}
func(D06Revised)isPayload(){}
type D06Archived struct{Data EventData}
func(D06Archived)isPayload(){}
type D07Created struct{Data EventData}
func(D07Created)isPayload(){}
type D07Renamed struct{Data EventData}
func(D07Renamed)isPayload(){}
type D07BalanceAdded struct{Data EventData}
func(D07BalanceAdded)isPayload(){}
type D07ItemAppended struct{Data EventData}
func(D07ItemAppended)isPayload(){}
type D07FlagToggled struct{Data EventData}
func(D07FlagToggled)isPayload(){}
type D07CountAdded struct{Data EventData}
func(D07CountAdded)isPayload(){}
type D07TagChanged struct{Data EventData}
func(D07TagChanged)isPayload(){}
type D07Seen struct{Data EventData}
func(D07Seen)isPayload(){}
type D07Revised struct{Data EventData}
func(D07Revised)isPayload(){}
type D07Archived struct{Data EventData}
func(D07Archived)isPayload(){}
type D08Created struct{Data EventData}
func(D08Created)isPayload(){}
type D08Renamed struct{Data EventData}
func(D08Renamed)isPayload(){}
type D08BalanceAdded struct{Data EventData}
func(D08BalanceAdded)isPayload(){}
type D08ItemAppended struct{Data EventData}
func(D08ItemAppended)isPayload(){}
type D08FlagToggled struct{Data EventData}
func(D08FlagToggled)isPayload(){}
type D08CountAdded struct{Data EventData}
func(D08CountAdded)isPayload(){}
type D08TagChanged struct{Data EventData}
func(D08TagChanged)isPayload(){}
type D08Seen struct{Data EventData}
func(D08Seen)isPayload(){}
type D08Revised struct{Data EventData}
func(D08Revised)isPayload(){}
type D08Archived struct{Data EventData}
func(D08Archived)isPayload(){}
type D09Created struct{Data EventData}
func(D09Created)isPayload(){}
type D09Renamed struct{Data EventData}
func(D09Renamed)isPayload(){}
type D09BalanceAdded struct{Data EventData}
func(D09BalanceAdded)isPayload(){}
type D09ItemAppended struct{Data EventData}
func(D09ItemAppended)isPayload(){}
type D09FlagToggled struct{Data EventData}
func(D09FlagToggled)isPayload(){}
type D09CountAdded struct{Data EventData}
func(D09CountAdded)isPayload(){}
type D09TagChanged struct{Data EventData}
func(D09TagChanged)isPayload(){}
type D09Seen struct{Data EventData}
func(D09Seen)isPayload(){}
type D09Revised struct{Data EventData}
func(D09Revised)isPayload(){}
type D09Archived struct{Data EventData}
func(D09Archived)isPayload(){}
type State struct{Id int;Balance,Counter,Seen,Revision int64;Name string;Exists,Flag,Archived bool;Items []int64;Tags map[int64]int64}
type Store struct{Map map[int]*State;List []*State;Ignored int64}
func create(i int,data EventData)EventPayload{switch i{
case 0:return D00Created{data}
case 1:return D00Renamed{data}
case 2:return D00BalanceAdded{data}
case 3:return D00ItemAppended{data}
case 4:return D00FlagToggled{data}
case 5:return D00CountAdded{data}
case 6:return D00TagChanged{data}
case 7:return D00Seen{data}
case 8:return D00Revised{data}
case 9:return D00Archived{data}
case 10:return D01Created{data}
case 11:return D01Renamed{data}
case 12:return D01BalanceAdded{data}
case 13:return D01ItemAppended{data}
case 14:return D01FlagToggled{data}
case 15:return D01CountAdded{data}
case 16:return D01TagChanged{data}
case 17:return D01Seen{data}
case 18:return D01Revised{data}
case 19:return D01Archived{data}
case 20:return D02Created{data}
case 21:return D02Renamed{data}
case 22:return D02BalanceAdded{data}
case 23:return D02ItemAppended{data}
case 24:return D02FlagToggled{data}
case 25:return D02CountAdded{data}
case 26:return D02TagChanged{data}
case 27:return D02Seen{data}
case 28:return D02Revised{data}
case 29:return D02Archived{data}
case 30:return D03Created{data}
case 31:return D03Renamed{data}
case 32:return D03BalanceAdded{data}
case 33:return D03ItemAppended{data}
case 34:return D03FlagToggled{data}
case 35:return D03CountAdded{data}
case 36:return D03TagChanged{data}
case 37:return D03Seen{data}
case 38:return D03Revised{data}
case 39:return D03Archived{data}
case 40:return D04Created{data}
case 41:return D04Renamed{data}
case 42:return D04BalanceAdded{data}
case 43:return D04ItemAppended{data}
case 44:return D04FlagToggled{data}
case 45:return D04CountAdded{data}
case 46:return D04TagChanged{data}
case 47:return D04Seen{data}
case 48:return D04Revised{data}
case 49:return D04Archived{data}
case 50:return D05Created{data}
case 51:return D05Renamed{data}
case 52:return D05BalanceAdded{data}
case 53:return D05ItemAppended{data}
case 54:return D05FlagToggled{data}
case 55:return D05CountAdded{data}
case 56:return D05TagChanged{data}
case 57:return D05Seen{data}
case 58:return D05Revised{data}
case 59:return D05Archived{data}
case 60:return D06Created{data}
case 61:return D06Renamed{data}
case 62:return D06BalanceAdded{data}
case 63:return D06ItemAppended{data}
case 64:return D06FlagToggled{data}
case 65:return D06CountAdded{data}
case 66:return D06TagChanged{data}
case 67:return D06Seen{data}
case 68:return D06Revised{data}
case 69:return D06Archived{data}
case 70:return D07Created{data}
case 71:return D07Renamed{data}
case 72:return D07BalanceAdded{data}
case 73:return D07ItemAppended{data}
case 74:return D07FlagToggled{data}
case 75:return D07CountAdded{data}
case 76:return D07TagChanged{data}
case 77:return D07Seen{data}
case 78:return D07Revised{data}
case 79:return D07Archived{data}
case 80:return D08Created{data}
case 81:return D08Renamed{data}
case 82:return D08BalanceAdded{data}
case 83:return D08ItemAppended{data}
case 84:return D08FlagToggled{data}
case 85:return D08CountAdded{data}
case 86:return D08TagChanged{data}
case 87:return D08Seen{data}
case 88:return D08Revised{data}
case 89:return D08Archived{data}
case 90:return D09Created{data}
case 91:return D09Renamed{data}
case 92:return D09BalanceAdded{data}
case 93:return D09ItemAppended{data}
case 94:return D09FlagToggled{data}
case 95:return D09CountAdded{data}
case 96:return D09TagChanged{data}
case 97:return D09Seen{data}
case 98:return D09Revised{data}
case 99:return D09Archived{data}
};panic("Unknown")}
func route(domain int,p EventPayload,stores []*Store,mode string){switch domain{
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
}}
func apply0(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D00Created:update(s,e.Data,0,mode)
case D00Renamed:update(s,e.Data,1,mode)
case D00BalanceAdded:update(s,e.Data,2,mode)
case D00ItemAppended:update(s,e.Data,3,mode)
case D00FlagToggled:update(s,e.Data,4,mode)
case D00CountAdded:update(s,e.Data,5,mode)
case D00TagChanged:update(s,e.Data,6,mode)
case D00Seen:update(s,e.Data,7,mode)
case D00Revised:update(s,e.Data,8,mode)
case D00Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply1(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D01Created:update(s,e.Data,0,mode)
case D01Renamed:update(s,e.Data,1,mode)
case D01BalanceAdded:update(s,e.Data,2,mode)
case D01ItemAppended:update(s,e.Data,3,mode)
case D01FlagToggled:update(s,e.Data,4,mode)
case D01CountAdded:update(s,e.Data,5,mode)
case D01TagChanged:update(s,e.Data,6,mode)
case D01Seen:update(s,e.Data,7,mode)
case D01Revised:update(s,e.Data,8,mode)
case D01Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply2(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D02Created:update(s,e.Data,0,mode)
case D02Renamed:update(s,e.Data,1,mode)
case D02BalanceAdded:update(s,e.Data,2,mode)
case D02ItemAppended:update(s,e.Data,3,mode)
case D02FlagToggled:update(s,e.Data,4,mode)
case D02CountAdded:update(s,e.Data,5,mode)
case D02TagChanged:update(s,e.Data,6,mode)
case D02Seen:update(s,e.Data,7,mode)
case D02Revised:update(s,e.Data,8,mode)
case D02Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply3(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D03Created:update(s,e.Data,0,mode)
case D03Renamed:update(s,e.Data,1,mode)
case D03BalanceAdded:update(s,e.Data,2,mode)
case D03ItemAppended:update(s,e.Data,3,mode)
case D03FlagToggled:update(s,e.Data,4,mode)
case D03CountAdded:update(s,e.Data,5,mode)
case D03TagChanged:update(s,e.Data,6,mode)
case D03Seen:update(s,e.Data,7,mode)
case D03Revised:update(s,e.Data,8,mode)
case D03Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply4(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D04Created:update(s,e.Data,0,mode)
case D04Renamed:update(s,e.Data,1,mode)
case D04BalanceAdded:update(s,e.Data,2,mode)
case D04ItemAppended:update(s,e.Data,3,mode)
case D04FlagToggled:update(s,e.Data,4,mode)
case D04CountAdded:update(s,e.Data,5,mode)
case D04TagChanged:update(s,e.Data,6,mode)
case D04Seen:update(s,e.Data,7,mode)
case D04Revised:update(s,e.Data,8,mode)
case D04Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply5(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D05Created:update(s,e.Data,0,mode)
case D05Renamed:update(s,e.Data,1,mode)
case D05BalanceAdded:update(s,e.Data,2,mode)
case D05ItemAppended:update(s,e.Data,3,mode)
case D05FlagToggled:update(s,e.Data,4,mode)
case D05CountAdded:update(s,e.Data,5,mode)
case D05TagChanged:update(s,e.Data,6,mode)
case D05Seen:update(s,e.Data,7,mode)
case D05Revised:update(s,e.Data,8,mode)
case D05Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply6(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D06Created:update(s,e.Data,0,mode)
case D06Renamed:update(s,e.Data,1,mode)
case D06BalanceAdded:update(s,e.Data,2,mode)
case D06ItemAppended:update(s,e.Data,3,mode)
case D06FlagToggled:update(s,e.Data,4,mode)
case D06CountAdded:update(s,e.Data,5,mode)
case D06TagChanged:update(s,e.Data,6,mode)
case D06Seen:update(s,e.Data,7,mode)
case D06Revised:update(s,e.Data,8,mode)
case D06Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply7(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D07Created:update(s,e.Data,0,mode)
case D07Renamed:update(s,e.Data,1,mode)
case D07BalanceAdded:update(s,e.Data,2,mode)
case D07ItemAppended:update(s,e.Data,3,mode)
case D07FlagToggled:update(s,e.Data,4,mode)
case D07CountAdded:update(s,e.Data,5,mode)
case D07TagChanged:update(s,e.Data,6,mode)
case D07Seen:update(s,e.Data,7,mode)
case D07Revised:update(s,e.Data,8,mode)
case D07Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply8(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D08Created:update(s,e.Data,0,mode)
case D08Renamed:update(s,e.Data,1,mode)
case D08BalanceAdded:update(s,e.Data,2,mode)
case D08ItemAppended:update(s,e.Data,3,mode)
case D08FlagToggled:update(s,e.Data,4,mode)
case D08CountAdded:update(s,e.Data,5,mode)
case D08TagChanged:update(s,e.Data,6,mode)
case D08Seen:update(s,e.Data,7,mode)
case D08Revised:update(s,e.Data,8,mode)
case D08Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func apply9(p EventPayload,s *Store,mode string){switch e:=p.(type){
case D09Created:update(s,e.Data,0,mode)
case D09Renamed:update(s,e.Data,1,mode)
case D09BalanceAdded:update(s,e.Data,2,mode)
case D09ItemAppended:update(s,e.Data,3,mode)
case D09FlagToggled:update(s,e.Data,4,mode)
case D09CountAdded:update(s,e.Data,5,mode)
case D09TagChanged:update(s,e.Data,6,mode)
case D09Seen:update(s,e.Data,7,mode)
case D09Revised:update(s,e.Data,8,mode)
case D09Archived:update(s,e.Data,9,mode)
default:s.Ignored++}}
func update(store *Store,e EventData,kind int,mode string){var s *State
 if mode=="linear-append"{for _,v:=range store.List{if v.Id==e.Id{s=v;break}}}else{s=store.Map[e.Id]}
 if s==nil{s=&State{Id:e.Id,Tags:make(map[int64]int64)};if mode=="linear-append"{store.List=append(store.List,s)}else{store.Map[e.Id]=s}}
 switch kind{case 0:s.Exists=true;case 1:s.Name=e.Label;case 2:s.Balance+=e.Value;case 3:if mode=="indexed-copy"{items:=make([]int64,len(s.Items)+1);copy(items,s.Items);items[len(s.Items)]=e.Value;s.Items=items}else{s.Items=append(s.Items,e.Value)};case 4:s.Flag=!s.Flag;case 5:s.Counter+=e.Value;case 6:s.Tags[e.Value%16]=e.Value;case 7:s.Seen=e.Value;case 8:s.Revision=e.Value;case 9:s.Archived=true}
}
func main(){fmt.Println(`{"event":"ready"}`);events,_:=strconv.Atoi(os.Args[1]);aggregates,_:=strconv.Atoi(os.Args[2]);mode,routing:=os.Args[3],os.Args[4];stores:=make([]*Store,10);for i:=range stores{stores[i]=&Store{Map:make(map[int]*State)}};fmt.Println(`{"event":"prepared"}`);start:=time.Now()
 for i:=0;i<events;i++{label:="";if i%10==1{label=fmt.Sprintf("name-%d",i+1)};p:=create(i%100,EventData{(i/100)%aggregates,int64(i+1),label});if routing=="fanout"{for d:=0;d<10;d++{route(d,p,stores,mode)}}else{route((i%100)/10,p,stores,mode)}};for d:=0;d<10;d++{route(d,Unknown{},stores,mode)};elapsed:=time.Since(start).Seconds()
 var sum,items,states,maxHistory,ignored int64;for d,store:=range stores{ignored+=store.Ignored;all:=store.List;if mode!="linear-append"{all=make([]*State,0,len(store.Map));for _,s:=range store.Map{all=append(all,s)}};for _,s:=range all{states++;items+=int64(len(s.Items));maxHistory=max(maxHistory,int64(len(s.Items)));sum+=int64(s.Id+d)+s.Balance+s.Counter+s.Seen+s.Revision+int64(len(s.Name)+len(s.Items));if s.Exists{sum++};if s.Flag{sum++};if s.Archived{sum++};for _,v:=range s.Items{sum+=v};for k,v:=range s.Tags{sum+=k+v}}}
 result,_:=json.Marshal(map[string]any{"roots":events,"samples":1,"sink":sum,"stateCount":states,"retainedItems":items,"maxHistoryLength":maxHistory,"ignored":ignored,"measurements":[]map[string]any{{"encodeSeconds":0,"decodeSeconds":0,"switchSeconds":elapsed,"totalSeconds":elapsed,"bytes":max(int64(1),items*8)}}});fmt.Println(string(result))
}
