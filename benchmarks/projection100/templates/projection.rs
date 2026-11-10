use std::{any::Any,collections::HashMap,time::Instant,io::Write};
struct EventData{id:usize,value:i64,label:String}
trait EventPayload{fn as_any(&self)->&dyn Any;}
struct Unknown;impl EventPayload for Unknown{fn as_any(&self)->&dyn Any{self}}
@@TYPES@@
#[derive(Default)]struct State{id:usize,balance:i64,counter:i64,seen:i64,revision:i64,name:String,exists:bool,flag:bool,archived:bool,items:Vec<i64>,tags:HashMap<i64,i64>}
#[derive(Default)]struct Store{map:HashMap<usize,State>,list:Vec<State>,ignored:i64}
fn create(i:usize,data:EventData)->Box<dyn EventPayload>{match i{
@@CREATE@@
_=>unreachable!()}}
fn route(domain:usize,p:&dyn EventPayload,stores:&mut[Store],mode:&str){match domain{
@@ROUTE@@
_=>unreachable!()}}
@@HANDLERS@@
fn update(store:&mut Store,e:&EventData,kind:usize,mode:&str){
 let s=if mode=="linear-append"{let pos=store.list.iter().position(|s|s.id==e.id).unwrap_or_else(||{store.list.push(State{id:e.id,..Default::default()});store.list.len()-1});&mut store.list[pos]}else{store.map.entry(e.id).or_insert_with(||State{id:e.id,..Default::default()})};
 match kind{0=>s.exists=true,1=>s.name=e.label.clone(),2=>s.balance+=e.value,3=>{if mode=="indexed-copy"{let mut copy=s.items.clone();copy.push(e.value);s.items=copy;}else{s.items.push(e.value);}},4=>s.flag=!s.flag,5=>s.counter+=e.value,6=>{s.tags.insert(e.value%16,e.value);},7=>s.seen=e.value,8=>s.revision=e.value,9=>s.archived=true,_=>unreachable!()}
}
fn main(){
 println!("{{\"event\":\"ready\"}}");std::io::stdout().flush().unwrap();
 let args:Vec<_>=std::env::args().collect();let events:usize=args[1].parse().unwrap();let aggregates:usize=args[2].parse().unwrap();let mode=&args[3];let routing=&args[4];
 let mut stores:Vec<Store>=(0..10).map(|_|Store::default()).collect();
 println!("{{\"event\":\"prepared\"}}");std::io::stdout().flush().unwrap();let start=Instant::now();
 for i in 0..events{let data=EventData{id:(i/100)%aggregates,value:(i+1)as i64,label:if i%10==1{format!("name-{}",i+1)}else{String::new()}};let p=create(i%100,data);
 if routing=="fanout"{for d in 0..10{route(d,p.as_ref(),&mut stores,mode);}}else{route((i%100)/10,p.as_ref(),&mut stores,mode);}}
 for d in 0..10{route(d,&Unknown,&mut stores,mode);}let elapsed=start.elapsed().as_secs_f64();
 let(mut sum,mut items,mut states,mut max_history,mut ignored)=(0i64,0usize,0usize,0usize,0i64);
 for(d,store)in stores.iter().enumerate(){ignored+=store.ignored;let all:Box<dyn Iterator<Item=&State>>=if mode=="linear-append"{Box::new(store.list.iter())}else{Box::new(store.map.values())};for s in all{states+=1;items+=s.items.len();max_history=max_history.max(s.items.len());sum+=s.id as i64+d as i64+s.balance+s.counter+s.seen+s.revision+s.name.len()as i64+s.exists as i64+s.flag as i64+s.archived as i64+s.items.iter().sum::<i64>()+s.items.len()as i64+s.tags.iter().map(|(k,v)|k+v).sum::<i64>();}}
 println!("{{\"roots\":{events},\"samples\":1,\"sink\":{sum},\"stateCount\":{states},\"retainedItems\":{items},\"maxHistoryLength\":{max_history},\"ignored\":{ignored},\"measurements\":[{{\"encodeSeconds\":0,\"decodeSeconds\":0,\"switchSeconds\":{elapsed},\"totalSeconds\":{elapsed},\"bytes\":{}}}]}}",(items*8).max(1));
}
