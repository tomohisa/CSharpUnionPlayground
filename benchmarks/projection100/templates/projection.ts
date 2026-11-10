import{performance}from"node:perf_hooks";
interface EventData{id:number;value:number;label:string}
interface IEventPayload{kind:string;data:EventData}
class Unknown implements IEventPayload{kind="Unknown";data={id:0,value:0,label:""}}
@@TYPES@@
class State{id:number;balance=0;counter=0;seen=0;revision=0;name="";exists=false;flag=false;archived=false;items:number[]=[];tags=new Map<number,number>();constructor(id:number){this.id=id;}}
class Store{map=new Map<number,State>();list:State[]=[];ignored=0;}
function create(i:number,data:EventData):IEventPayload{switch(i){
@@CREATE@@
default:throw Error("Unknown");}}
function route(domain:number,p:IEventPayload,stores:Store[],mode:string){switch(domain){
@@ROUTE@@
}}
@@HANDLERS@@
function update(store:Store,e:EventData,kind:number,mode:string){
 let s=mode==="linear-append"?store.list.find(v=>v.id===e.id):store.map.get(e.id);
 if(!s){s=new State(e.id);if(mode==="linear-append")store.list.push(s);else store.map.set(e.id,s);}
 switch(kind){case 0:s.exists=true;break;case 1:s.name=e.label;break;case 2:s.balance+=e.value;break;case 3:if(mode==="indexed-copy")s.items=s.items.concat(e.value);else s.items.push(e.value);break;case 4:s.flag=!s.flag;break;case 5:s.counter+=e.value;break;case 6:s.tags.set(e.value%16,e.value);break;case 7:s.seen=e.value;break;case 8:s.revision=e.value;break;case 9:s.archived=true;break;}
}
console.log(JSON.stringify({event:"ready"}));
const events=Number(process.argv[2]),aggregates=Number(process.argv[3]),mode=process.argv[4],routing=process.argv[5];
const stores=Array.from({length:10},()=>new Store());console.log(JSON.stringify({event:"prepared"}));const start=performance.now();
for(let i=0;i<events;i++){const p=create(i%100,{id:Math.floor(i/100)%aggregates,value:i+1,label:i%10===1?`name-${i+1}`:""});if(routing==="fanout")for(let d=0;d<10;d++)route(d,p,stores,mode);else route(Math.floor((i%100)/10),p,stores,mode);}
for(let d=0;d<10;d++)route(d,new Unknown(),stores,mode);const elapsed=(performance.now()-start)/1000;
let sum=0,items=0,states=0,maxHistory=0,ignored=0;
stores.forEach((store,d)=>{ignored+=store.ignored;for(const s of mode==="linear-append"?store.list:store.map.values()){states++;items+=s.items.length;maxHistory=Math.max(maxHistory,s.items.length);sum+=s.id+d+s.balance+s.counter+s.seen+s.revision+s.name.length+Number(s.exists)+Number(s.flag)+Number(s.archived)+s.items.reduce((a,b)=>a+b,0)+s.items.length+[...s.tags].reduce((a,[k,v])=>a+k+v,0);}});
console.log(JSON.stringify({roots:events,samples:1,sink:sum,stateCount:states,retainedItems:items,maxHistoryLength:maxHistory,ignored,measurements:[{encodeSeconds:0,decodeSeconds:0,switchSeconds:elapsed,totalSeconds:elapsed,bytes:Math.max(1,items*8)}]}));
