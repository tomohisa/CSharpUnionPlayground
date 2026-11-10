using System.Diagnostics;
using System.Globalization;
Console.WriteLine("{\"event\":\"ready\"}");
int events=int.Parse(args[0]),aggregates=int.Parse(args[1]);string mode=args[2],routing=args[3];
var stores=Enumerable.Range(0,10).Select(_=>new Store()).ToArray();
Console.WriteLine("{\"event\":\"prepared\"}");
var timer=Stopwatch.StartNew();
for(int i=0;i<events;i++){
 var data=new EventData((i/100)%aggregates,i+1,(i%10)==1?"name-"+(i+1):"");
 IEventPayload p=Create(i%100,data);
 if(routing=="fanout")for(int d=0;d<10;d++)Route(d,p,stores,mode);
 else Route((i%100)/10,p,stores,mode);
}
for(int d=0;d<10;d++)Route(d,new Unknown(),stores,mode);
timer.Stop();long sum=0,items=0,states=0,maxHistory=0,ignored=stores.Sum(s=>s.Ignored);
for(int d=0;d<10;d++)foreach(var s in stores[d].All(mode)){
 states++;items+=s.Items.Count;maxHistory=Math.Max(maxHistory,s.Items.Count);
 sum+=s.Id+d+s.Balance+s.Counter+s.Seen+s.Revision+s.Name.Length+(s.Exists?1:0)+(s.Flag?1:0)+(s.Archived?1:0)+s.Items.Sum()+s.Items.Count+s.Tags.Sum(kv=>kv.Key+kv.Value);
}
string elapsed=timer.Elapsed.TotalSeconds.ToString("R",CultureInfo.InvariantCulture);
Console.WriteLine($$"""{"roots":{{events}},"samples":1,"sink":{{sum}},"stateCount":{{states}},"retainedItems":{{items}},"maxHistoryLength":{{maxHistory}},"ignored":{{ignored}},"measurements":[{"encodeSeconds":0,"decodeSeconds":0,"switchSeconds":{{elapsed}},"totalSeconds":{{elapsed}},"bytes":{{Math.Max(1,items*8)}}}]}""");

static IEventPayload Create(int i,EventData data)=>i switch{
0=>new D00Created(data),
1=>new D00Renamed(data),
2=>new D00BalanceAdded(data),
3=>new D00ItemAppended(data),
4=>new D00FlagToggled(data),
5=>new D00CountAdded(data),
6=>new D00TagChanged(data),
7=>new D00Seen(data),
8=>new D00Revised(data),
9=>new D00Archived(data),
10=>new D01Created(data),
11=>new D01Renamed(data),
12=>new D01BalanceAdded(data),
13=>new D01ItemAppended(data),
14=>new D01FlagToggled(data),
15=>new D01CountAdded(data),
16=>new D01TagChanged(data),
17=>new D01Seen(data),
18=>new D01Revised(data),
19=>new D01Archived(data),
20=>new D02Created(data),
21=>new D02Renamed(data),
22=>new D02BalanceAdded(data),
23=>new D02ItemAppended(data),
24=>new D02FlagToggled(data),
25=>new D02CountAdded(data),
26=>new D02TagChanged(data),
27=>new D02Seen(data),
28=>new D02Revised(data),
29=>new D02Archived(data),
30=>new D03Created(data),
31=>new D03Renamed(data),
32=>new D03BalanceAdded(data),
33=>new D03ItemAppended(data),
34=>new D03FlagToggled(data),
35=>new D03CountAdded(data),
36=>new D03TagChanged(data),
37=>new D03Seen(data),
38=>new D03Revised(data),
39=>new D03Archived(data),
40=>new D04Created(data),
41=>new D04Renamed(data),
42=>new D04BalanceAdded(data),
43=>new D04ItemAppended(data),
44=>new D04FlagToggled(data),
45=>new D04CountAdded(data),
46=>new D04TagChanged(data),
47=>new D04Seen(data),
48=>new D04Revised(data),
49=>new D04Archived(data),
50=>new D05Created(data),
51=>new D05Renamed(data),
52=>new D05BalanceAdded(data),
53=>new D05ItemAppended(data),
54=>new D05FlagToggled(data),
55=>new D05CountAdded(data),
56=>new D05TagChanged(data),
57=>new D05Seen(data),
58=>new D05Revised(data),
59=>new D05Archived(data),
60=>new D06Created(data),
61=>new D06Renamed(data),
62=>new D06BalanceAdded(data),
63=>new D06ItemAppended(data),
64=>new D06FlagToggled(data),
65=>new D06CountAdded(data),
66=>new D06TagChanged(data),
67=>new D06Seen(data),
68=>new D06Revised(data),
69=>new D06Archived(data),
70=>new D07Created(data),
71=>new D07Renamed(data),
72=>new D07BalanceAdded(data),
73=>new D07ItemAppended(data),
74=>new D07FlagToggled(data),
75=>new D07CountAdded(data),
76=>new D07TagChanged(data),
77=>new D07Seen(data),
78=>new D07Revised(data),
79=>new D07Archived(data),
80=>new D08Created(data),
81=>new D08Renamed(data),
82=>new D08BalanceAdded(data),
83=>new D08ItemAppended(data),
84=>new D08FlagToggled(data),
85=>new D08CountAdded(data),
86=>new D08TagChanged(data),
87=>new D08Seen(data),
88=>new D08Revised(data),
89=>new D08Archived(data),
90=>new D09Created(data),
91=>new D09Renamed(data),
92=>new D09BalanceAdded(data),
93=>new D09ItemAppended(data),
94=>new D09FlagToggled(data),
95=>new D09CountAdded(data),
96=>new D09TagChanged(data),
97=>new D09Seen(data),
98=>new D09Revised(data),
99=>new D09Archived(data),
_=>throw new Exception()};
static void Route(int domain,IEventPayload p,Store[] stores,string mode){switch(domain){
case 0:Apply0(p,stores[0],mode);break;
case 1:Apply1(p,stores[1],mode);break;
case 2:Apply2(p,stores[2],mode);break;
case 3:Apply3(p,stores[3],mode);break;
case 4:Apply4(p,stores[4],mode);break;
case 5:Apply5(p,stores[5],mode);break;
case 6:Apply6(p,stores[6],mode);break;
case 7:Apply7(p,stores[7],mode);break;
case 8:Apply8(p,stores[8],mode);break;
case 9:Apply9(p,stores[9],mode);break;
}}
static void Apply0(IEventPayload payload, Store store, string mode) { switch(payload) {
case D00Created e: Update(store, e.Data, 0, mode); break;
case D00Renamed e: Update(store, e.Data, 1, mode); break;
case D00BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D00ItemAppended e: Update(store, e.Data, 3, mode); break;
case D00FlagToggled e: Update(store, e.Data, 4, mode); break;
case D00CountAdded e: Update(store, e.Data, 5, mode); break;
case D00TagChanged e: Update(store, e.Data, 6, mode); break;
case D00Seen e: Update(store, e.Data, 7, mode); break;
case D00Revised e: Update(store, e.Data, 8, mode); break;
case D00Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply1(IEventPayload payload, Store store, string mode) { switch(payload) {
case D01Created e: Update(store, e.Data, 0, mode); break;
case D01Renamed e: Update(store, e.Data, 1, mode); break;
case D01BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D01ItemAppended e: Update(store, e.Data, 3, mode); break;
case D01FlagToggled e: Update(store, e.Data, 4, mode); break;
case D01CountAdded e: Update(store, e.Data, 5, mode); break;
case D01TagChanged e: Update(store, e.Data, 6, mode); break;
case D01Seen e: Update(store, e.Data, 7, mode); break;
case D01Revised e: Update(store, e.Data, 8, mode); break;
case D01Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply2(IEventPayload payload, Store store, string mode) { switch(payload) {
case D02Created e: Update(store, e.Data, 0, mode); break;
case D02Renamed e: Update(store, e.Data, 1, mode); break;
case D02BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D02ItemAppended e: Update(store, e.Data, 3, mode); break;
case D02FlagToggled e: Update(store, e.Data, 4, mode); break;
case D02CountAdded e: Update(store, e.Data, 5, mode); break;
case D02TagChanged e: Update(store, e.Data, 6, mode); break;
case D02Seen e: Update(store, e.Data, 7, mode); break;
case D02Revised e: Update(store, e.Data, 8, mode); break;
case D02Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply3(IEventPayload payload, Store store, string mode) { switch(payload) {
case D03Created e: Update(store, e.Data, 0, mode); break;
case D03Renamed e: Update(store, e.Data, 1, mode); break;
case D03BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D03ItemAppended e: Update(store, e.Data, 3, mode); break;
case D03FlagToggled e: Update(store, e.Data, 4, mode); break;
case D03CountAdded e: Update(store, e.Data, 5, mode); break;
case D03TagChanged e: Update(store, e.Data, 6, mode); break;
case D03Seen e: Update(store, e.Data, 7, mode); break;
case D03Revised e: Update(store, e.Data, 8, mode); break;
case D03Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply4(IEventPayload payload, Store store, string mode) { switch(payload) {
case D04Created e: Update(store, e.Data, 0, mode); break;
case D04Renamed e: Update(store, e.Data, 1, mode); break;
case D04BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D04ItemAppended e: Update(store, e.Data, 3, mode); break;
case D04FlagToggled e: Update(store, e.Data, 4, mode); break;
case D04CountAdded e: Update(store, e.Data, 5, mode); break;
case D04TagChanged e: Update(store, e.Data, 6, mode); break;
case D04Seen e: Update(store, e.Data, 7, mode); break;
case D04Revised e: Update(store, e.Data, 8, mode); break;
case D04Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply5(IEventPayload payload, Store store, string mode) { switch(payload) {
case D05Created e: Update(store, e.Data, 0, mode); break;
case D05Renamed e: Update(store, e.Data, 1, mode); break;
case D05BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D05ItemAppended e: Update(store, e.Data, 3, mode); break;
case D05FlagToggled e: Update(store, e.Data, 4, mode); break;
case D05CountAdded e: Update(store, e.Data, 5, mode); break;
case D05TagChanged e: Update(store, e.Data, 6, mode); break;
case D05Seen e: Update(store, e.Data, 7, mode); break;
case D05Revised e: Update(store, e.Data, 8, mode); break;
case D05Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply6(IEventPayload payload, Store store, string mode) { switch(payload) {
case D06Created e: Update(store, e.Data, 0, mode); break;
case D06Renamed e: Update(store, e.Data, 1, mode); break;
case D06BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D06ItemAppended e: Update(store, e.Data, 3, mode); break;
case D06FlagToggled e: Update(store, e.Data, 4, mode); break;
case D06CountAdded e: Update(store, e.Data, 5, mode); break;
case D06TagChanged e: Update(store, e.Data, 6, mode); break;
case D06Seen e: Update(store, e.Data, 7, mode); break;
case D06Revised e: Update(store, e.Data, 8, mode); break;
case D06Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply7(IEventPayload payload, Store store, string mode) { switch(payload) {
case D07Created e: Update(store, e.Data, 0, mode); break;
case D07Renamed e: Update(store, e.Data, 1, mode); break;
case D07BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D07ItemAppended e: Update(store, e.Data, 3, mode); break;
case D07FlagToggled e: Update(store, e.Data, 4, mode); break;
case D07CountAdded e: Update(store, e.Data, 5, mode); break;
case D07TagChanged e: Update(store, e.Data, 6, mode); break;
case D07Seen e: Update(store, e.Data, 7, mode); break;
case D07Revised e: Update(store, e.Data, 8, mode); break;
case D07Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply8(IEventPayload payload, Store store, string mode) { switch(payload) {
case D08Created e: Update(store, e.Data, 0, mode); break;
case D08Renamed e: Update(store, e.Data, 1, mode); break;
case D08BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D08ItemAppended e: Update(store, e.Data, 3, mode); break;
case D08FlagToggled e: Update(store, e.Data, 4, mode); break;
case D08CountAdded e: Update(store, e.Data, 5, mode); break;
case D08TagChanged e: Update(store, e.Data, 6, mode); break;
case D08Seen e: Update(store, e.Data, 7, mode); break;
case D08Revised e: Update(store, e.Data, 8, mode); break;
case D08Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Apply9(IEventPayload payload, Store store, string mode) { switch(payload) {
case D09Created e: Update(store, e.Data, 0, mode); break;
case D09Renamed e: Update(store, e.Data, 1, mode); break;
case D09BalanceAdded e: Update(store, e.Data, 2, mode); break;
case D09ItemAppended e: Update(store, e.Data, 3, mode); break;
case D09FlagToggled e: Update(store, e.Data, 4, mode); break;
case D09CountAdded e: Update(store, e.Data, 5, mode); break;
case D09TagChanged e: Update(store, e.Data, 6, mode); break;
case D09Seen e: Update(store, e.Data, 7, mode); break;
case D09Revised e: Update(store, e.Data, 8, mode); break;
case D09Archived e: Update(store, e.Data, 9, mode); break;
default: store.Ignored++; break; } }
static void Update(Store store,EventData e,int kind,string mode){
 State? s=null;
 if(mode=="linear-append"){foreach(var candidate in store.List)if(candidate.Id==e.Id){s=candidate;break;}if(s is null){s=new State(e.Id);store.List.Add(s);}}
 else if(!store.Map.TryGetValue(e.Id,out s)){s=new State(e.Id);store.Map.Add(e.Id,s);}
 switch(kind){
 case 0:s.Exists=true;break;case 1:s.Name=e.Label;break;case 2:s.Balance+=e.Value;break;
 case 3:if(mode=="indexed-copy")s.Items=s.Items.Append(e.Value).ToList();else s.Items.Add(e.Value);break;
 case 4:s.Flag=!s.Flag;break;case 5:s.Counter+=e.Value;break;case 6:s.Tags[e.Value%16]=e.Value;break;
 case 7:s.Seen=e.Value;break;case 8:s.Revision=e.Value;break;case 9:s.Archived=true;break;
 }
}
public interface IEventPayload{}
public record Unknown:IEventPayload;
public record EventData(int Id,long Value,string Label);
public record D00Created(EventData Data):IEventPayload;
public record D00Renamed(EventData Data):IEventPayload;
public record D00BalanceAdded(EventData Data):IEventPayload;
public record D00ItemAppended(EventData Data):IEventPayload;
public record D00FlagToggled(EventData Data):IEventPayload;
public record D00CountAdded(EventData Data):IEventPayload;
public record D00TagChanged(EventData Data):IEventPayload;
public record D00Seen(EventData Data):IEventPayload;
public record D00Revised(EventData Data):IEventPayload;
public record D00Archived(EventData Data):IEventPayload;
public record D01Created(EventData Data):IEventPayload;
public record D01Renamed(EventData Data):IEventPayload;
public record D01BalanceAdded(EventData Data):IEventPayload;
public record D01ItemAppended(EventData Data):IEventPayload;
public record D01FlagToggled(EventData Data):IEventPayload;
public record D01CountAdded(EventData Data):IEventPayload;
public record D01TagChanged(EventData Data):IEventPayload;
public record D01Seen(EventData Data):IEventPayload;
public record D01Revised(EventData Data):IEventPayload;
public record D01Archived(EventData Data):IEventPayload;
public record D02Created(EventData Data):IEventPayload;
public record D02Renamed(EventData Data):IEventPayload;
public record D02BalanceAdded(EventData Data):IEventPayload;
public record D02ItemAppended(EventData Data):IEventPayload;
public record D02FlagToggled(EventData Data):IEventPayload;
public record D02CountAdded(EventData Data):IEventPayload;
public record D02TagChanged(EventData Data):IEventPayload;
public record D02Seen(EventData Data):IEventPayload;
public record D02Revised(EventData Data):IEventPayload;
public record D02Archived(EventData Data):IEventPayload;
public record D03Created(EventData Data):IEventPayload;
public record D03Renamed(EventData Data):IEventPayload;
public record D03BalanceAdded(EventData Data):IEventPayload;
public record D03ItemAppended(EventData Data):IEventPayload;
public record D03FlagToggled(EventData Data):IEventPayload;
public record D03CountAdded(EventData Data):IEventPayload;
public record D03TagChanged(EventData Data):IEventPayload;
public record D03Seen(EventData Data):IEventPayload;
public record D03Revised(EventData Data):IEventPayload;
public record D03Archived(EventData Data):IEventPayload;
public record D04Created(EventData Data):IEventPayload;
public record D04Renamed(EventData Data):IEventPayload;
public record D04BalanceAdded(EventData Data):IEventPayload;
public record D04ItemAppended(EventData Data):IEventPayload;
public record D04FlagToggled(EventData Data):IEventPayload;
public record D04CountAdded(EventData Data):IEventPayload;
public record D04TagChanged(EventData Data):IEventPayload;
public record D04Seen(EventData Data):IEventPayload;
public record D04Revised(EventData Data):IEventPayload;
public record D04Archived(EventData Data):IEventPayload;
public record D05Created(EventData Data):IEventPayload;
public record D05Renamed(EventData Data):IEventPayload;
public record D05BalanceAdded(EventData Data):IEventPayload;
public record D05ItemAppended(EventData Data):IEventPayload;
public record D05FlagToggled(EventData Data):IEventPayload;
public record D05CountAdded(EventData Data):IEventPayload;
public record D05TagChanged(EventData Data):IEventPayload;
public record D05Seen(EventData Data):IEventPayload;
public record D05Revised(EventData Data):IEventPayload;
public record D05Archived(EventData Data):IEventPayload;
public record D06Created(EventData Data):IEventPayload;
public record D06Renamed(EventData Data):IEventPayload;
public record D06BalanceAdded(EventData Data):IEventPayload;
public record D06ItemAppended(EventData Data):IEventPayload;
public record D06FlagToggled(EventData Data):IEventPayload;
public record D06CountAdded(EventData Data):IEventPayload;
public record D06TagChanged(EventData Data):IEventPayload;
public record D06Seen(EventData Data):IEventPayload;
public record D06Revised(EventData Data):IEventPayload;
public record D06Archived(EventData Data):IEventPayload;
public record D07Created(EventData Data):IEventPayload;
public record D07Renamed(EventData Data):IEventPayload;
public record D07BalanceAdded(EventData Data):IEventPayload;
public record D07ItemAppended(EventData Data):IEventPayload;
public record D07FlagToggled(EventData Data):IEventPayload;
public record D07CountAdded(EventData Data):IEventPayload;
public record D07TagChanged(EventData Data):IEventPayload;
public record D07Seen(EventData Data):IEventPayload;
public record D07Revised(EventData Data):IEventPayload;
public record D07Archived(EventData Data):IEventPayload;
public record D08Created(EventData Data):IEventPayload;
public record D08Renamed(EventData Data):IEventPayload;
public record D08BalanceAdded(EventData Data):IEventPayload;
public record D08ItemAppended(EventData Data):IEventPayload;
public record D08FlagToggled(EventData Data):IEventPayload;
public record D08CountAdded(EventData Data):IEventPayload;
public record D08TagChanged(EventData Data):IEventPayload;
public record D08Seen(EventData Data):IEventPayload;
public record D08Revised(EventData Data):IEventPayload;
public record D08Archived(EventData Data):IEventPayload;
public record D09Created(EventData Data):IEventPayload;
public record D09Renamed(EventData Data):IEventPayload;
public record D09BalanceAdded(EventData Data):IEventPayload;
public record D09ItemAppended(EventData Data):IEventPayload;
public record D09FlagToggled(EventData Data):IEventPayload;
public record D09CountAdded(EventData Data):IEventPayload;
public record D09TagChanged(EventData Data):IEventPayload;
public record D09Seen(EventData Data):IEventPayload;
public record D09Revised(EventData Data):IEventPayload;
public record D09Archived(EventData Data):IEventPayload;
public class State(int id){public int Id=id;public long Balance,Counter,Seen,Revision;public string Name="";public bool Exists,Flag,Archived;public List<long> Items=[];public Dictionary<long,long> Tags=[];}
public class Store{public long Ignored;public Dictionary<int,State> Map=[];public List<State> List=[];public IEnumerable<State> All(string mode)=>mode=="linear-append"?List:Map.Values;}
