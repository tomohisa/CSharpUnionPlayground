package main
import("encoding/json";"fmt";"os";"strconv";"time")
type EventPayload interface{isPayload()}
type Unknown struct{};func(Unknown)isPayload(){}
type EventData struct{Id int;Value int64;Label string}
@@TYPES@@
type State struct{Id int;Balance,Counter,Seen,Revision int64;Name string;Exists,Flag,Archived bool;Items []int64;Tags map[int64]int64}
type Store struct{Map map[int]*State;List []*State;Ignored int64}
func create(i int,data EventData)EventPayload{switch i{
@@CREATE@@
};panic("Unknown")}
func route(domain int,p EventPayload,stores []*Store,mode string){switch domain{
@@ROUTE@@
}}
@@HANDLERS@@
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
