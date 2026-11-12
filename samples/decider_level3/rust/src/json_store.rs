use crate::dcb::*;
use std::collections::{HashMap,HashSet};
use std::sync::{Arc,Mutex};
#[derive(Clone)]
pub struct JsonEvent{pub position:usize,pub command_id:String,pub event_type:String,pub payload_json:Arc<String>,pub tags_json:Arc<String>}
pub struct JsonRead{pub after:usize,pub events:Vec<JsonEvent>}
pub enum JsonAppend{Committed(Vec<JsonEvent>),Conflict}
pub trait JsonJournal:Send+Sync{
    fn engine_version(&self)->&str;
    fn read(&self,query:&Query,after:usize)->JsonRead;
    fn append(&self,id:&str,query:&Query,after:usize,events:Vec<JsonEvent>)->JsonAppend;
    fn load(&self,events:Vec<JsonEvent>);
}
pub fn keys(keys:&[String])->Vec<u8>{let mut bytes=vec![];for key in keys{bytes.extend_from_slice(&(key.len() as u32).to_be_bytes());bytes.extend_from_slice(key.as_bytes());}bytes}
fn index_key(tag:&Tag)->Vec<u8>{let mut parts=vec![tag.category.clone()];parts.extend_from_slice(&tag.keys);keys(&parts)}
pub fn decode_tags(json:&str)->Vec<Tag>{let tags:Vec<Tag>=serde_json::from_str(json).expect("invalid stored tags");let mut seen=HashSet::new();for tag in &tags{assert!(seen.insert(index_key(tag)),"duplicate stored tag");}tags}
struct MemoryState{log:Vec<JsonEvent>,tags:HashMap<Vec<u8>,Vec<usize>>,types:HashMap<String,Vec<usize>>}
impl MemoryState{
    fn candidates(&self,q:&Query,after:usize,high:usize)->Vec<usize>{
        if q.items.is_empty()||q.items.iter().any(|i|i.tags.is_empty()&&i.types.is_empty()){return (after+1..=high).collect();}
        let mut all=HashSet::new();for item in &q.items{
            let mut sets:Vec<Vec<usize>>=item.tags.iter().map(|t|self.tags.get(&index_key(t)).cloned().unwrap_or_default()).collect();
            if !item.types.is_empty(){sets.push(item.types.iter().flat_map(|t|self.types.get(t).into_iter().flatten().copied()).collect());}
            if let Some(first)=sets.first(){let mut selected:HashSet<usize>=first.iter().copied().collect();for next in sets.iter().skip(1){let next:HashSet<_>=next.iter().copied().collect();selected.retain(|p|next.contains(p));}all.extend(selected.into_iter().filter(|p|*p>after&&*p<=high));}
        }
        let mut rows:Vec<_>=all.into_iter().collect();rows.sort_unstable();rows
    }
    fn insert(&mut self,row:JsonEvent){self.types.entry(row.event_type.clone()).or_default().push(row.position);for tag in decode_tags(&row.tags_json){self.tags.entry(index_key(&tag)).or_default().push(row.position);}self.log.push(row);}
}
pub struct MemoryJsonJournal{inner:Mutex<MemoryState>}
impl MemoryJsonJournal{pub fn new()->Self{Self{inner:Mutex::new(MemoryState{log:vec![],tags:HashMap::new(),types:HashMap::new()})}}}
impl JsonJournal for MemoryJsonJournal{
    fn engine_version(&self)->&str{"memory-json-v1"}
    fn read(&self,q:&Query,after:usize)->JsonRead{let s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);assert!(after<=s.log.len(),"invalid position");JsonRead{after:s.log.len(),events:s.candidates(q,after,s.log.len()).into_iter().map(|p|s.log[p-1].clone()).collect()}}
    fn append(&self,id:&str,q:&Query,after:usize,events:Vec<JsonEvent>)->JsonAppend{
        let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);assert!(!events.is_empty()&&!id.is_empty()&&after<=s.log.len(),"invalid append");if !s.candidates(q,after,s.log.len()).is_empty(){return JsonAppend::Conflict;}
        let rows:Vec<_>=events.into_iter().enumerate().map(|(i,mut e)|{e.position=s.log.len()+i+1;e.command_id=id.into();e}).collect();for row in &rows{decode_tags(&row.tags_json);}for row in &rows{s.insert(row.clone());}JsonAppend::Committed(rows)
    }
    fn load(&self,events:Vec<JsonEvent>){let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);assert!(s.log.is_empty(),"load requires empty journal");assert!(events.iter().enumerate().all(|(i,e)|e.position==i+1),"invalid imported position");for e in &events{decode_tags(&e.tags_json);}for e in events{s.insert(e);}}
}
type Listener<E>=Box<dyn Fn(&[Recorded<E>])->bool+Send>;
struct StoreState<E>{listeners:Vec<Listener<E>>,published:usize,faults:usize,encoded:u64,decoded:u64,encoded_bytes:u64,decoded_bytes:u64}
pub struct JsonEventStore<E:Clone+Send>{journal:Box<dyn JsonJournal>,describe:Box<dyn Fn(&E)->EventDescriptor+Send+Sync>,encode:Box<dyn Fn(&E)->String+Send+Sync>,decode:Box<dyn Fn(&str)->E+Send+Sync>,inner:Mutex<StoreState<E>>}
impl<E:Clone+Send> JsonEventStore<E>{
    pub fn new(journal:Box<dyn JsonJournal>,describe:impl Fn(&E)->EventDescriptor+Send+Sync+'static,encode:impl Fn(&E)->String+Send+Sync+'static,decode:impl Fn(&str)->E+Send+Sync+'static)->Self{Self{journal,describe:Box::new(describe),encode:Box::new(encode),decode:Box::new(decode),inner:Mutex::new(StoreState{listeners:vec![],published:0,faults:0,encoded:0,decoded:0,encoded_bytes:0,decoded_bytes:0})}}
    pub fn engine_version(&self)->&str{self.journal.engine_version()}
    pub fn counters(&self)->(u64,u64,u64,u64){let s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);(s.encoded,s.decoded,s.encoded_bytes,s.decoded_bytes)}
    pub fn listener_faults(&self)->usize{self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner).faults}
    fn encode_event(&self,id:&str,e:&E,s:&mut StoreState<E>)->JsonEvent{let meta=(self.describe)(e);assert!(!meta.event_type.is_empty(),"event type required");let json=(self.encode)(e);s.encoded+=1;s.encoded_bytes+=json.len() as u64;JsonEvent{position:0,command_id:id.into(),event_type:meta.event_type,payload_json:Arc::new(json),tags_json:Arc::new(serde_json::to_string(&meta.tags).unwrap())}}
    fn decode_event(&self,e:&JsonEvent,s:&mut StoreState<E>)->Recorded<E>{let payload=(self.decode)(&e.payload_json);s.decoded+=1;s.decoded_bytes+=e.payload_json.len() as u64;let tags=decode_tags(&e.tags_json);let expected=(self.describe)(&payload);assert!(expected.event_type==e.event_type&&expected.tags.len()==tags.len()&&expected.tags.iter().all(|t|tags.contains(t)),"invalid stored event metadata");Recorded{position:e.position,command_id:e.command_id.clone(),event_type:e.event_type.clone(),tags,payload}}
    fn notify(listener:&Listener<E>,events:&[Recorded<E>])->bool{std::panic::catch_unwind(std::panic::AssertUnwindSafe(||listener(events))).unwrap_or(false)}
    fn refresh_locked(&self,s:&mut StoreState<E>){if s.listeners.is_empty(){return;}let read=self.journal.read(&Query{items:vec![]},s.published);let events:Vec<_>=read.events.iter().map(|e|self.decode_event(e,s)).collect();let before=s.listeners.len();s.listeners.retain(|listener|Self::notify(listener,&events));s.faults+=before-s.listeners.len();s.published=read.after;}
    pub fn refresh(&self){let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);self.refresh_locked(&mut s);}
    pub fn read(&self,q:&Query)->ReadResult<E>{let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);let read=self.journal.read(q,0);ReadResult{after:read.after,events:read.events.iter().map(|e|self.decode_event(e,&mut s)).collect()}}
    pub fn append(&self,id:&str,q:&Query,after:usize,events:Vec<E>)->AppendResult<E>{let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);let batch=events.iter().map(|e|self.encode_event(id,e,&mut s)).collect();match self.journal.append(id,q,after,batch){JsonAppend::Conflict=>AppendResult::Conflict,JsonAppend::Committed(rows)=>{if s.listeners.is_empty(){s.published=rows.last().unwrap().position;}else{self.refresh_locked(&mut s);}AppendResult::Committed(rows.iter().map(|e|self.decode_event(e,&mut s)).collect())}}}
    pub fn subscribe(&self,listener:Listener<E>){let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);if !s.listeners.is_empty(){self.refresh_locked(&mut s);}let read=self.journal.read(&Query{items:vec![]},0);let events:Vec<_>=read.events.iter().map(|e|self.decode_event(e,&mut s)).collect();assert!(Self::notify(&listener,&events),"subscription replay failed");s.listeners.push(listener);s.published=read.after;}
    pub fn load(&self,events:Vec<Recorded<E>>){let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);assert!(s.listeners.is_empty(),"load before subscribe");let rows=events.iter().enumerate().map(|(i,e)|{assert!(e.position==i+1&&!e.command_id.is_empty(),"invalid imported log");let mut row=self.encode_event(&e.command_id,&e.payload,&mut s);let meta=(self.describe)(&e.payload);assert!(meta.event_type==e.event_type&&meta.tags.len()==e.tags.len()&&meta.tags.iter().all(|t|e.tags.contains(t)),"invalid imported metadata");row.position=e.position;row}).collect();self.journal.load(rows);}
}
impl<E:Clone+Send> CommandStore<E> for JsonEventStore<E>{fn read(&self,q:&Query)->ReadResult<E>{self.read(q)}fn append(&self,id:&str,q:&Query,after:usize,events:Vec<E>)->AppendResult<E>{self.append(id,q,after,events)}}
