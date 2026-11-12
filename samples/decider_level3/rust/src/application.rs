use crate::{dcb::*,models::*,projection::{self,Registers},routing,handlers::SchoolHandler};
use std::{collections::BTreeMap,sync::Arc};
use serde::Serialize;
use crate::school_projection::SchoolProjection;
#[derive(Serialize)]
pub struct ReadAudit{query:Query,after:usize,returned:usize}
#[derive(Serialize)]
#[serde(rename_all="camelCase")]
pub struct RunOutput{events:Vec<Recorded<Event>>,steps:Vec<Trace>,lists:Vec<SchoolList>,list:SchoolList,rebuilt:SchoolList,checkpoint:usize,reads:Vec<ReadAudit>,listener_faults:usize}
fn prepared<'a>(executor:&DcbExecutor<'a,Event>,reads:&mut Vec<ReadAudit>,id:&str,command:Command)->PreparedCommand<'a,Rejection>{
    let t=executor.prepare(id,command,&SchoolHandler);if t.was_read{reads.push(ReadAudit{query:t.query.clone(),after:t.after,returned:t.returned});}t
}
fn committed(steps:&mut Vec<Trace>,op:&str,t:PreparedCommand<Rejection>){
    let r=t.commit();steps.push(Trace{operation:op.into(),status:r.status,conflicts:r.conflicts as i32,appended:r.appended as i32,position:r.position as i32,errors:r.errors});
}
pub fn run(history:Vec<Recorded<Event>>,operations:Vec<Operation>)->RunOutput{
    let journal:Box<dyn crate::json_store::JsonJournal>=match std::env::var("DECIDER_PERSISTENT"){Ok(path)=>Box::new(crate::sqlite_journal::SqliteJournal::new(&path,"FULL")),Err(_)=>Box::new(crate::json_store::MemoryJsonJournal::new())};
    let store=crate::json_store::JsonEventStore::new(journal,routing::describe,|e|serde_json::to_string(e).unwrap(),|s|serde_json::from_str(s).unwrap());
    if !history.is_empty(){store.load(history);}
    if std::env::var("DECIDER_BENCHMARK").as_deref()==Ok("1"){eprintln!("STORE {}",store.engine_version());}
    let projection=Arc::new(SchoolProjection::new());let listener=projection.clone();store.subscribe(Box::new(move|batch|listener.accept(batch)));
    let executor=DcbExecutor{store:&store};
    let mut tickets=BTreeMap::new();let mut steps=vec![];let mut lists=vec![];let mut reads=vec![];
    let bench=std::env::var("DECIDER_BENCHMARK").as_deref()==Ok("1");let mut start=std::time::Instant::now();if bench{eprintln!("READY");}
    for op in operations{match op{
        Operation::Execute{command_id,command}=>{let t=prepared(&executor,&mut reads,&command_id,command);committed(&mut steps,"Execute",t);},
        Operation::Prepare{key,command_id,command}=>{let t=prepared(&executor,&mut reads,&command_id,command);steps.push(Trace{operation:"Prepare".into(),status:"Prepared".into(),conflicts:0,appended:0,position:t.after as i32,errors:vec![]});assert!(tickets.insert(key,t).is_none(),"duplicate ticket");},
        Operation::Commit{key}=>{let t=tickets.remove(&key).expect("unknown ticket");committed(&mut steps,"Commit",t);},
        Operation::ListAll=>{store.refresh();assert_eq!(store.listener_faults(),0,"projection unavailable; rebuild required");let (position,s)=projection.snapshot();lists.push(s);steps.push(Trace{operation:"ListAll".into(),status:"Listed".into(),conflicts:0,appended:0,position:position as i32,errors:vec![]});},
        Operation::Rebuild=>{let fresh=SchoolProjection::new();assert!(fresh.accept(&store.read(&Query{items:vec![]}).events));let(position,s)=fresh.snapshot();lists.push(s);steps.push(Trace{operation:"Rebuild".into(),status:"Rebuilt".into(),conflicts:0,appended:0,position:position as i32,errors:vec![]});},
        Operation::Redeliver=>{assert!(projection.accept(&store.read(&Query{items:vec![]}).events));steps.push(Trace{operation:"Redeliver".into(),status:"Redelivered".into(),conflicts:0,appended:0,position:projection.checkpoint() as i32,errors:vec![]});}
    }}
    if bench{eprintln!("PHASE operations {}",start.elapsed().as_secs_f64());}start=std::time::Instant::now();
    store.refresh();
    crate::query_benchmark::run(&store,&projection);start=std::time::Instant::now();
    let events=store.read(&Query{items:vec![]}).events;let fresh=SchoolProjection::new();assert!(fresh.accept(&events));let(checkpoint,s)=projection.snapshot();
    assert_eq!(store.listener_faults(),0,"projection unavailable; rebuild required");
    if bench{let(a,b,c,d)=store.counters();eprintln!("PHASE read_rebuild_snapshot {}",start.elapsed().as_secs_f64());eprintln!("CODEC {a} {b} {c} {d}");}
    crate::storage_benchmark::run();
    RunOutput{events,steps,lists,list:s,rebuilt:fresh.snapshot().1,checkpoint,reads,listener_faults:store.listener_faults()}
}

#[derive(Serialize)]#[serde(rename_all="camelCase")]
pub struct BenchmarkSummary{list:SchoolList,rebuild_matches:bool,event_count:usize,checkpoint:usize,listener_faults:usize,step_counts:BTreeMap<String,usize>,errors:BTreeMap<String,usize>,conflicts:i32}
pub fn benchmark_summary(o:RunOutput)->BenchmarkSummary{
 let mut steps=BTreeMap::new();let mut errors=BTreeMap::new();let mut conflicts=0;
 for s in o.steps{*steps.entry(s.status).or_insert(0)+=1;for e in s.errors{let key=serde_json::to_value(e).unwrap().as_str().unwrap().to_owned();*errors.entry(key).or_insert(0)+=1;}conflicts+=s.conflicts;}
 BenchmarkSummary{rebuild_matches:serde_json::to_vec(&o.list).unwrap()==serde_json::to_vec(&o.rebuilt).unwrap(),event_count:o.events.len(),checkpoint:o.checkpoint,listener_faults:o.listener_faults,list:o.list,step_counts:steps,errors,conflicts}
}
