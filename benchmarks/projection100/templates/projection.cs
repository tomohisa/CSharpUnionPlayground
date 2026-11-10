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
@@CREATE@@
_=>throw new Exception()};
static void Route(int domain,IEventPayload p,Store[] stores,string mode){switch(domain){
@@ROUTE@@
}}
@@HANDLERS@@
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
@@TYPES@@
public class State(int id){public int Id=id;public long Balance,Counter,Seen,Revision;public string Name="";public bool Exists,Flag,Archived;public List<long> Items=[];public Dictionary<long,long> Tags=[];}
public class Store{public long Ignored;public Dictionary<int,State> Map=[];public List<State> List=[];public IEnumerable<State> All(string mode)=>mode=="linear-append"?List:Map.Values;}
