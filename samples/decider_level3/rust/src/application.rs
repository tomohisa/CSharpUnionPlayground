use crate::{dcb::*,models::*,projection::{self,Registers},routing};
use std::{collections::BTreeMap,sync::Arc};
use serde::Serialize;
#[derive(Serialize)]
pub struct ReadAudit{query:Query,after:usize,returned:usize}
#[derive(Serialize)]
#[serde(rename_all="camelCase")]
pub struct RunOutput{events:Vec<Recorded<Event>>,steps:Vec<Trace>,lists:Vec<SchoolList>,list:SchoolList,rebuilt:SchoolList,checkpoint:usize,reads:Vec<ReadAudit>,listener_faults:usize}
fn prepared(processor:&CommandProcessor<Command,Event,Rejection>,reads:&mut Vec<ReadAudit>,id:&str,command:Command)->Ticket<Command,Event,Rejection>{
    let t=processor.prepare(id,command);reads.push(ReadAudit{query:t.query.clone(),after:t.after,returned:t.returned});t
}
fn committed(processor:&CommandProcessor<Command,Event,Rejection>,steps:&mut Vec<Trace>,op:&str,t:Ticket<Command,Event,Rejection>){
    let r=processor.commit(t);steps.push(Trace{operation:op.into(),status:r.status,conflicts:r.conflicts as i32,appended:r.appended as i32,position:r.position as i32,errors:r.errors});
}
pub fn run(history:Vec<Recorded<Event>>,operations:Vec<Operation>)->RunOutput{
    let store=EventStore::new(routing::describe,Clone::clone);
    store.load(history);
    let projection=Arc::new(LiveProjection::new(Registers::default(),projection::reduce));let listener=projection.clone();store.subscribe(Box::new(move|batch|listener.accept(batch)));
    let processor=CommandProcessor{store:&store,query_for:&routing::query_for,decide:&routing::decide,max_conflicts:3};
    let mut tickets=BTreeMap::new();let mut steps=vec![];let mut lists=vec![];let mut reads=vec![];
    for op in operations{match op{
        Operation::Execute{command_id,command}=>{let t=prepared(&processor,&mut reads,&command_id,command);committed(&processor,&mut steps,"Execute",t);},
        Operation::Prepare{key,command_id,command}=>{let t=prepared(&processor,&mut reads,&command_id,command);steps.push(Trace{operation:"Prepare".into(),status:"Prepared".into(),conflicts:0,appended:0,position:t.after as i32,errors:vec![]});assert!(tickets.insert(key,t).is_none(),"duplicate ticket");},
        Operation::Commit{key}=>{let t=tickets.remove(&key).expect("unknown ticket");committed(&processor,&mut steps,"Commit",t);},
        Operation::ListAll=>{assert_eq!(store.listener_faults(),0,"projection unavailable; rebuild required");let (position,s)=projection.snapshot();lists.push(projection::list(&s));steps.push(Trace{operation:"ListAll".into(),status:"Listed".into(),conflicts:0,appended:0,position:position as i32,errors:vec![]});},
        Operation::Rebuild=>{let fresh=LiveProjection::new(Registers::default(),projection::reduce);assert!(fresh.accept(&store.read(&Query{items:vec![]}).events));let(position,s)=fresh.snapshot();lists.push(projection::list(&s));steps.push(Trace{operation:"Rebuild".into(),status:"Rebuilt".into(),conflicts:0,appended:0,position:position as i32,errors:vec![]});},
        Operation::Redeliver=>{assert!(projection.accept(&store.read(&Query{items:vec![]}).events));steps.push(Trace{operation:"Redeliver".into(),status:"Redelivered".into(),conflicts:0,appended:0,position:projection.snapshot().0 as i32,errors:vec![]});}
    }}
    let events=store.read(&Query{items:vec![]}).events;let fresh=LiveProjection::new(Registers::default(),projection::reduce);assert!(fresh.accept(&events));let(checkpoint,s)=projection.snapshot();
    assert_eq!(store.listener_faults(),0,"projection unavailable; rebuild required");
    RunOutput{events,steps,lists,list:projection::list(&s),rebuilt:projection::list(&fresh.snapshot().1),checkpoint,reads,listener_faults:store.listener_faults()}
}
