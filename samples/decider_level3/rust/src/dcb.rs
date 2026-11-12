use serde::{Serialize, Deserialize};
use std::sync::Mutex;
#[derive(Clone, Debug, PartialEq, Eq, Serialize, Deserialize)]
pub struct Tag { pub category: String, pub keys: Vec<String> }
#[derive(Clone, Serialize, Deserialize)]
pub struct QueryItem { pub tags: Vec<Tag>, pub types: Vec<String> }
#[derive(Clone, Serialize, Deserialize)]
pub struct Query { pub items: Vec<QueryItem> }
impl Query {
    pub fn matches(&self, event_type: &str, tags: &[Tag]) -> bool {
        self.items.is_empty() || self.items.iter().any(|q| (q.types.is_empty() || q.types.iter().any(|t| t == event_type)) && q.tags.iter().all(|t| tags.contains(t)))
    }
}
#[derive(Clone)]
pub struct EventDescriptor { pub event_type: String, pub tags: Vec<Tag> }
#[derive(Clone, Serialize, Deserialize)]
#[serde(rename_all="camelCase")]
pub struct Recorded<E> { pub position: usize, pub command_id: String, pub event_type: String, pub tags: Vec<Tag>, pub payload: E }
pub struct ReadResult<E> { pub after: usize, pub events: Vec<Recorded<E>> }
pub enum AppendResult<E> { Committed(Vec<Recorded<E>>), Conflict }
pub trait CommandStore<E:Clone+Send>:Sync {
    fn read(&self,query:&Query)->ReadResult<E>;
    fn append(&self,command_id:&str,condition:&Query,after:usize,batch:Vec<E>)->AppendResult<E>;
}
type Listener<E> = Box<dyn Fn(&[Recorded<E>]) -> bool + Send>;
struct Inner<E> { log: Vec<Recorded<E>>, listeners: Vec<Listener<E>>, faults: usize }
// Listener callbacks are synchronous, non-reentrant and must report failure via bool.
pub struct EventStore<E: Clone + Send> { inner: Mutex<Inner<E>>, describe:Box<dyn Fn(&E)->EventDescriptor+Send+Sync>, copy_payload:Box<dyn Fn(&E)->E+Send+Sync> }
impl<E: Clone + Send> EventStore<E> {
    pub fn new(describe:impl Fn(&E)->EventDescriptor+Send+Sync+'static,copy_payload:impl Fn(&E)->E+Send+Sync+'static) -> Self { Self { describe:Box::new(describe),copy_payload:Box::new(copy_payload), inner: Mutex::new(Inner {log: vec![], listeners: vec![], faults: 0}) } }
    fn copy(&self,e:&Recorded<E>)->Recorded<E>{Recorded{position:e.position,command_id:e.command_id.clone(),event_type:e.event_type.clone(),tags:e.tags.clone(),payload:(self.copy_payload)(&e.payload)}}
    pub fn read(&self, query: &Query) -> ReadResult<E> {
        let s = self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);
        ReadResult {after: s.log.len(), events: s.log.iter().filter(|e| query.matches(&e.event_type,&e.tags)).map(|e|self.copy(e)).collect()}
    }
    pub fn append(&self, command_id: &str, condition: &Query, after: usize, batch: Vec<E>) -> AppendResult<E> {
        let mut s = self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);
        assert!(!batch.is_empty() && !command_id.is_empty() && after <= s.log.len(), "invalid append");
        if s.log.iter().any(|e| e.position > after && condition.matches(&e.event_type,&e.tags)) { return AppendResult::Conflict; }
        let committed: Vec<_> = batch.into_iter().enumerate().map(|(i,payload)| {
            let payload=(self.copy_payload)(&payload);let e=(self.describe)(&payload);
            assert!(!e.event_type.is_empty(), "event type required");
            Recorded {position:s.log.len()+i+1, command_id:command_id.into(), event_type:e.event_type, tags:e.tags, payload}
        }).collect();
        let copies:Vec<_>=committed.iter().map(|e|self.copy(e)).collect();
        s.log.extend(copies);
        let before = s.listeners.len();
        s.listeners.retain(|listener| std::panic::catch_unwind(std::panic::AssertUnwindSafe(||listener(&committed.iter().map(|e|self.copy(e)).collect::<Vec<_>>()))).unwrap_or(false));
        s.faults += before - s.listeners.len();
        AppendResult::Committed(committed.iter().map(|e|self.copy(e)).collect())
    }
    pub fn subscribe(&self, listener: Listener<E>) {
        let mut s = self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);
        assert!(listener(&s.log.iter().map(|e|self.copy(e)).collect::<Vec<_>>()), "subscription replay failed"); s.listeners.push(listener);
    }
    pub fn load(&self, events: Vec<Recorded<E>>) {
        let mut s = self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);
        assert!(s.log.is_empty() && s.listeners.is_empty(), "load only into an empty store");
        assert!(events.iter().enumerate().all(|(i,e)| e.position == i+1 && !e.command_id.is_empty() && !e.event_type.is_empty() && {let d=(self.describe)(&e.payload);d.event_type==e.event_type && d.tags.len()==e.tags.len() && d.tags.iter().all(|t|e.tags.contains(t))}), "invalid imported log");
        s.log = events.iter().map(|e|self.copy(e)).collect();
    }
    pub fn listener_faults(&self) -> usize { self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner).faults }
}
impl<E:Clone+Send> CommandStore<E> for EventStore<E> {
    fn read(&self,q:&Query)->ReadResult<E>{EventStore::read(self,q)}
    fn append(&self,id:&str,q:&Query,after:usize,batch:Vec<E>)->AppendResult<E>{EventStore::append(self,id,q,after,batch)}
}
pub struct LiveProjection<E,S: Clone> { inner: Mutex<(usize,S)>, reduce: Box<dyn Fn(&mut S,&E)+Send+Sync> }
impl<E,S: Clone> LiveProjection<E,S> {
    pub fn new(initial: S, reduce: impl Fn(&mut S,&E)+Send+Sync+'static) -> Self { Self {inner:Mutex::new((0,initial)),reduce:Box::new(reduce)} }
    pub fn accept(&self, batch: &[Recorded<E>]) -> bool {
        let mut s = self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner); let (mut position, mut next) = s.clone();
        for e in batch {
            if e.position <= position { continue; }
            if e.position != position+1 { return false; }
            (self.reduce)(&mut next,&e.payload); position=e.position;
        }
        *s = (position,next); true
    }
    pub fn snapshot(&self) -> (usize,S) { self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner).clone() }
}
pub enum Preparation<E,R> { Emit(Vec<E>), Reject(R) }
pub struct Ticket<C,E,R> { pub command_id:String, pub command:C, pub query:Query, pub after:usize, pub returned:usize, pub decision:Preparation<E,R> }
pub struct HandlingResult<R> { pub status:String, pub conflicts:usize, pub appended:usize, pub position:usize, pub errors:Vec<R> }
pub struct CommandProcessor<'a,C,E:Clone+Send,R> {
    pub store:&'a dyn CommandStore<E>, pub query_for:&'a dyn Fn(&C)->Query,
    pub decide:&'a dyn Fn(&[Recorded<E>],&C)->Preparation<E,R>, pub max_conflicts:usize
}
impl<'a,C,E:Clone+Send,R> CommandProcessor<'a,C,E,R> {
    pub fn prepare(&self, command_id:&str, command:C) -> Ticket<C,E,R> {
        let query=(self.query_for)(&command); let read=self.store.read(&query); let decision=(self.decide)(&read.events,&command);
        Ticket{command_id:command_id.into(),command,query,after:read.after,returned:read.events.len(),decision}
    }
    pub fn commit(&self, mut ticket:Ticket<C,E,R>) -> HandlingResult<R> {
        let mut conflicts=0;
        loop {
            let events=match ticket.decision {
                Preparation::Reject(error) => return HandlingResult{status:"Rejected".into(),conflicts,appended:0,position:ticket.after,errors:vec![error]},
                Preparation::Emit(events)=>events
            };
            match self.store.append(&ticket.command_id,&ticket.query,ticket.after,events) {
                AppendResult::Committed(events)=>return HandlingResult{status:"Committed".into(),conflicts,appended:events.len(),position:events.last().unwrap().position,errors:vec![]},
                AppendResult::Conflict=>{}
            }
            conflicts+=1;
            if conflicts>=self.max_conflicts {return HandlingResult{status:"RetryExhausted".into(),conflicts,appended:0,position:ticket.after,errors:vec![]};}
            ticket=self.prepare(&ticket.command_id,ticket.command);
        }
    }
}

pub trait CommandHandler<C,E,R> {
    fn validate(&self,command:&C)->Vec<R>;
    fn query_for(&self,command:&C)->Query;
    fn handle(&self,events:&[Recorded<E>],command:&C)->Preparation<E,R>;
}
pub struct PreparedCommand<'a,R> {
    pub query:Query, pub after:usize, pub returned:usize, pub was_read:bool,
    commit:Box<dyn FnOnce()->HandlingResult<R>+'a>
}
impl<R> PreparedCommand<'_,R> { pub fn commit(self)->HandlingResult<R>{(self.commit)()} }
pub struct DcbExecutor<'a,E:Clone+Send> { pub store:&'a dyn CommandStore<E> }
impl<'a,E:Clone+Send+'a> DcbExecutor<'a,E> {
    pub fn prepare<C:'a,R:'a,H:CommandHandler<C,E,R>+'a>(&self,id:&str,command:C,handler:&'a H)->PreparedCommand<'a,R> {
        assert!(!id.trim().is_empty(),"command id required");
        let errors=handler.validate(&command);
        if !errors.is_empty(){return PreparedCommand{query:Query{items:vec![]},after:0,returned:0,was_read:false,
            commit:Box::new(move||HandlingResult{status:"Rejected".into(),conflicts:0,appended:0,position:0,errors})};}
        let query=handler.query_for(&command);let read=self.store.read(&query);
        let decision=handler.handle(&read.events,&command);
        let ticket=Ticket{command_id:id.into(),command,query:query.clone(),after:read.after,returned:read.events.len(),decision};
        let store=self.store;
        PreparedCommand{query,after:read.after,returned:read.events.len(),was_read:true,commit:Box::new(move||{
            CommandProcessor{store,query_for:&|c|handler.query_for(c),decide:&|events,c|handler.handle(events,c),max_conflicts:3}.commit(ticket)
        })}
    }
    pub fn execute<C:'a,R:'a,H:CommandHandler<C,E,R>+'a>(&self,id:&str,command:C,handler:&'a H)->HandlingResult<R>{self.prepare(id,command,handler).commit()}
}

// Clone is per touched row, never per entire projection.
pub struct IndexBatch<'a,K:std::hash::Hash+Eq+Clone,V:Clone>{rows:&'a std::collections::HashMap<K,V>,changes:std::collections::HashMap<K,Option<V>>,work:u64}
impl<K:std::hash::Hash+Eq+Clone,V:Clone> IndexBatch<'_,K,V>{
    pub fn find(&mut self,key:&K)->Option<V>{self.work+=1;match self.changes.get(key){Some(v)=>v.clone(),None=>self.rows.get(key).cloned()}}
    pub fn get(&mut self,key:&K)->V{self.find(key).expect("missing index row")}
    pub fn put(&mut self,key:K,value:V){self.work+=1;self.changes.insert(key,Some(value));}
    pub fn delete(&mut self,key:K){self.work+=1;self.changes.insert(key,None);}
}
struct IndexState<K,V>{rows:std::collections::HashMap<K,V>,checkpoint:usize,work:u64}
pub struct IndexedProjection<K:std::hash::Hash+Eq+Clone,V:Clone,E>{inner:Mutex<IndexState<K,V>>,reduce:Box<dyn Fn(&mut IndexBatch<K,V>,&Recorded<E>)->Result<(),String>+Send+Sync>}
impl<K:std::hash::Hash+Eq+Clone,V:Clone,E> IndexedProjection<K,V,E>{
    pub fn new(reduce:impl Fn(&mut IndexBatch<K,V>,&Recorded<E>)->Result<(),String>+Send+Sync+'static)->Self{Self{inner:Mutex::new(IndexState{rows:std::collections::HashMap::new(),checkpoint:0,work:0}),reduce:Box::new(reduce)}}
    pub fn accept(&self,events:&[Recorded<E>])->bool{
        let mut s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);
        let mut batch=IndexBatch{rows:&s.rows,changes:std::collections::HashMap::new(),work:0};let mut position=s.checkpoint;
        for e in events{if e.position<=position{continue;}if e.position!=position+1{return false;}if (self.reduce)(&mut batch,e).is_err(){return false;}position=e.position;}
        let work=batch.work;let changes=batch.changes;
        for(key,value)in changes{match value{Some(v)=>{s.rows.insert(key,v);},None=>{s.rows.remove(&key);}}}
        s.checkpoint=position;s.work+=work;true
    }
    pub fn checkpoint(&self)->usize{self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner).checkpoint}
    pub fn row_count(&self)->usize{self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner).rows.len()}
    pub fn work(&self)->u64{self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner).work}
    pub fn read<T>(&self,query:impl FnOnce(&std::collections::HashMap<K,V>)->T)->(usize,T){let s=self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);(s.checkpoint,query(&s.rows))}
}

// Owned preparation can cross request/thread boundaries without leaking a store.
// The generic handler and store stay alive until the one-shot ticket is dropped.
pub struct OwnedPrepared<R> {
    pub after:usize, pub was_read:bool,
    action:Box<dyn FnOnce()->HandlingResult<R>+Send>
}
impl<R> OwnedPrepared<R>{pub fn commit(self)->HandlingResult<R>{(self.action)()}}
pub fn prepare_owned<C:Send+'static,E:Clone+Send+'static,R:Send+'static,H:CommandHandler<C,E,R>+Send+Sync+'static>(
    store:std::sync::Arc<dyn CommandStore<E>+Send+Sync>,id:String,command:C,handler:std::sync::Arc<H>
)->OwnedPrepared<R>{
    assert!(!id.trim().is_empty(),"command id required");
    let errors=handler.validate(&command);
    if !errors.is_empty(){return OwnedPrepared{after:0,was_read:false,action:Box::new(move||HandlingResult{status:"Rejected".into(),conflicts:0,appended:0,position:0,errors})};}
    let query=handler.query_for(&command);let read=store.read(&query);
    let decision=handler.handle(&read.events,&command);
    let ticket=Ticket{command_id:id,command,query,after:read.after,returned:read.events.len(),decision};
    OwnedPrepared{after:read.after,was_read:true,action:Box::new(move||CommandProcessor{
        store:&*store,query_for:&|c|handler.query_for(c),decide:&|events,c|handler.handle(events,c),max_conflicts:3
    }.commit(ticket))}
}
