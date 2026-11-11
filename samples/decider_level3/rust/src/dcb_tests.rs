use crate::dcb::*;
use std::sync::{Arc,Barrier};
#[derive(Clone)]struct Counter{key:String,delta:i32}
fn tag(key:&str)->Tag{Tag{category:"counter".into(),keys:vec![key.into()]}}
fn query(key:&str)->Query{Query{items:vec![QueryItem{tags:vec![tag(key)],types:vec![]}]}}
fn describe(e:&Counter)->EventDescriptor{EventDescriptor{event_type:"CounterDelta".into(),tags:vec![tag(&e.key),Tag{category:"scope".into(),keys:vec!["test".into()]}]}}
fn event(key:&str,delta:i32)->Counter{Counter{key:key.into(),delta}}
pub fn run(){
    let store=EventStore::new(describe,Clone::clone);let p=Arc::new(LiveProjection::new(0,|s:&mut i32,e:&Counter|*s+=e.delta));let listener=p.clone();store.subscribe(Box::new(move|e|listener.accept(e)));
    store.subscribe(Box::new(|e|if e.is_empty(){true}else{panic!("expected listener failure")}));
    assert!(matches!(store.append("first",&query("a"),0,vec![event("a",1),event("a",2)]),AppendResult::Committed(_)));
    assert_eq!(store.listener_faults(),1);assert_eq!(p.snapshot(),(2,3));
    assert!(matches!(store.append("other",&query("b"),0,vec![event("b",9)]),AppendResult::Committed(_)));
    assert!(matches!(store.append("stale",&query("a"),0,vec![event("a",99)]),AppendResult::Conflict));
    let all=store.read(&Query{items:vec![]});assert_eq!(all.after,3);assert_eq!(p.snapshot(),(3,12));
    let scope=Tag{category:"scope".into(),keys:vec!["test".into()]};
    assert_eq!(store.read(&Query{items:vec![QueryItem{tags:vec![tag("a"),scope],types:vec![]}]}).events.len(),2);
    assert_eq!(store.read(&Query{items:vec![query("a").items[0].clone(),query("b").items[0].clone()]}).events.len(),3);
    assert_eq!(store.read(&Query{items:vec![QueryItem{tags:vec![tag("a")],types:vec!["OtherType".into()]}]}).events.len(),0);
    assert!(p.accept(&all.events));assert_eq!(p.snapshot(),(3,12));
    let gap=LiveProjection::new(0,|s:&mut i32,e:&Counter|*s+=e.delta);assert!(!gap.accept(&[all.events[0].clone(),all.events[2].clone()]));assert_eq!(gap.snapshot(),(0,0));
    let late=Arc::new(LiveProjection::new(0,|s:&mut i32,e:&Counter|*s+=e.delta));let listener=late.clone();store.subscribe(Box::new(move|e|listener.accept(e)));assert_eq!(late.snapshot(),(3,12));
    let threshold=0;let decide=|_:&[Recorded<Counter>],c:&Counter|if c.delta>threshold{Preparation::Emit(vec![c.clone()])}else{Preparation::Reject("NegativeDelta")};
    let processor=CommandProcessor{store:&store,query_for:&|c:&Counter|query(&c.key),decide:&decide,max_conflicts:3};
    let ticket=processor.prepare("retry",event("a",1));store.append("interfere",&query("a"),3,vec![event("a",1)]);
    let result=processor.commit(ticket);assert_eq!(result.status,"Committed");assert_eq!(result.conflicts,1);assert_eq!(p.snapshot(),(5,14));assert_eq!(late.snapshot(),(5,14));
    assert_eq!(processor.commit(processor.prepare("reject",event("a",-1))).status,"Rejected");
    let restored=EventStore::new(describe,Clone::clone);restored.load(store.read(&Query{items:vec![]}).events);assert_eq!(restored.read(&Query{items:vec![]}).after,5);
    let mut invalid=all.events.clone();invalid[0].tags.clear();let bad=EventStore::new(describe,Clone::clone);assert!(std::panic::catch_unwind(std::panic::AssertUnwindSafe(||bad.load(invalid))).is_err());assert_eq!(bad.read(&Query{items:vec![]}).after,0);
    let racing=Arc::new(EventStore::new(describe,Clone::clone));let barrier=Arc::new(Barrier::new(2));let mut handles=vec![];
    for i in 0..2{let store=racing.clone();let barrier=barrier.clone();handles.push(std::thread::spawn(move||{let read=store.read(&query("a"));barrier.wait();matches!(store.append(&format!("race-{i}"),&query("a"),read.after,vec![event("a",1)]),AppendResult::Committed(_))}));}
    let winners=handles.into_iter().map(|h|h.join().unwrap() as usize).sum::<usize>();assert_eq!(winners,1);assert_eq!(racing.read(&Query{items:vec![]}).after,1);
}
