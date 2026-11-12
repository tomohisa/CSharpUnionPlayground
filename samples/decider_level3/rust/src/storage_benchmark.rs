use crate::{dcb::*,json_store::*,sqlite_journal::SqliteJournal};
use serde::{Serialize,Deserialize};use std::{collections::HashMap,sync::{Arc,Mutex}};
#[derive(Clone,Serialize,Deserialize)]#[serde(rename_all="camelCase")]
struct DocumentRevision{document_id:String,revision:usize,body:String}
pub fn run(){
 let count=std::env::var("DECIDER_DOCUMENTS").ok().and_then(|s|s.parse::<usize>().ok()).unwrap_or(0);if count==0{return;}
 let path=std::env::var("DECIDER_PERSISTENT").ok();
 let open=||->Box<dyn JsonJournal>{match &path{Some(p)=>Box::new(SqliteJournal::new(&format!("{p}.documents"),"FULL")),None=>Box::new(MemoryJsonJournal::new())}};
 let store=|j|JsonEventStore::new(j,|e:&DocumentRevision|EventDescriptor{event_type:"DocumentRevision".into(),tags:vec![Tag{category:"document".into(),keys:vec![e.document_id.clone()]}]},|e|serde_json::to_string(e).unwrap(),|s|serde_json::from_str(s).unwrap());
 let start=std::time::Instant::now();let latest=Arc::new(Mutex::new(HashMap::new()));let(encoded,decoded,position);
 {let s=store(open());let l=latest.clone();s.subscribe(Box::new(move|events|{let mut latest=l.lock().unwrap();for e in events{latest.insert(e.payload.document_id.clone(),e.payload.revision);}true}));let body="x".repeat(4096);
 for revision in 1..=16{for i in 0..count{let id=format!("document-{i:08}");let q=Query{items:vec![QueryItem{tags:vec![Tag{category:"document".into(),keys:vec![id.clone()]}],types:vec![]}]};let r=s.read(&q);assert_eq!(r.events.len(),revision-1);assert!(matches!(s.append(&format!("doc-{i}-{revision}"),&q,r.after,vec![DocumentRevision{document_id:id,revision,body:body.clone()}]),AppendResult::Committed(_)));}}
 let all=s.read(&Query{items:vec![]});position=all.after;(encoded,decoded,_,_)=s.counters();let l=latest.lock().unwrap();assert_eq!(position,16*count);assert_eq!(l.len(),count);assert!(l.values().all(|r|*r==16));assert!(all.events.iter().all(|e|e.payload.body.len()==4096));assert_eq!(s.listener_faults(),0);}
 if path.is_some(){let s=store(open());let all=s.read(&Query{items:vec![]});assert_eq!(all.events.len(),16*count);assert!(all.events.iter().all(|e|e.payload.body.len()==4096));}
 eprintln!("DOCUMENT {} {count} {position} {encoded} {decoded}",start.elapsed().as_secs_f64());
}
