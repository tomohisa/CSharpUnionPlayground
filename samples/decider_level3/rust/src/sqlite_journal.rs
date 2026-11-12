use crate::{dcb::Query,json_store::*};
use rusqlite::{Connection,TransactionBehavior,types::Value,params_from_iter,params};
use std::sync::{Mutex,Arc};
pub struct SqliteJournal{connection:Mutex<Connection>,version:String}
impl SqliteJournal{
    pub fn new(path:&str,synchronous:&str)->Self{
        assert!(matches!(synchronous,"FULL"|"NORMAL"),"durability must be FULL or NORMAL");
        if path!=":memory:"{if let Some(parent)=std::path::Path::new(path).parent(){std::fs::create_dir_all(parent).unwrap();}}
        let c=Connection::open(path).expect("open SQLite");c.busy_timeout(std::time::Duration::from_millis(5000)).unwrap();
        c.execute_batch(&format!("PRAGMA journal_mode=WAL;PRAGMA synchronous={synchronous};PRAGMA foreign_keys=ON;PRAGMA cache_size=-2048;PRAGMA wal_autocheckpoint=1000;
            CREATE TABLE IF NOT EXISTS events(position INTEGER PRIMARY KEY,command_id TEXT NOT NULL,event_type TEXT NOT NULL,payload_json TEXT NOT NULL,tags_json TEXT NOT NULL);
            CREATE TABLE IF NOT EXISTS event_tags(position INTEGER NOT NULL REFERENCES events(position),category TEXT NOT NULL,keys_key BLOB NOT NULL,PRIMARY KEY(position,category,keys_key));
            CREATE INDEX IF NOT EXISTS tags_match ON event_tags(category,keys_key,position);CREATE INDEX IF NOT EXISTS events_type_position ON events(event_type,position);" )).unwrap();
        c.set_prepared_statement_cache_capacity(64);
        let version:i32=c.query_row("PRAGMA user_version",[],|r|r.get(0)).unwrap();assert!(matches!(version,0|1),"unsupported schema version");c.execute_batch("PRAGMA user_version=1").unwrap();
        let version=c.query_row("SELECT sqlite_version()",[],|r|r.get(0)).unwrap();Self{connection:Mutex::new(c),version}
    }
    fn high(c:&Connection)->usize{usize::try_from(c.query_row::<i64,_,_>("SELECT COALESCE(MAX(position),0) FROM events",[],|r|r.get(0)).unwrap()).expect("invalid high watermark")}
    fn selection(q:&Query,low:usize,high:usize,exists:bool)->(String,Vec<Value>){
        fn range(args:&mut Vec<Value>,low:usize,high:usize)->String{args.push(Value::Integer(low as i64));args.push(Value::Integer(high as i64));"position>? AND position<=?".into()}
        let mut args=vec![];
        let condition=if q.items.is_empty()||q.items.iter().any(|i|i.tags.is_empty()&&i.types.is_empty()){range(&mut args,low,high)}else{
            let mut alternatives=vec![];for item in &q.items{let mut terms=vec![];for tag in &item.tags{args.push(Value::Text(tag.category.clone()));args.push(Value::Blob(keys(&tag.keys)));terms.push(format!("SELECT position FROM event_tags WHERE category=? AND keys_key=? AND {}",range(&mut args,low,high)));}
                if !item.types.is_empty(){let placeholders=item.types.iter().map(|t|{args.push(Value::Text(t.clone()));"?"}).collect::<Vec<_>>().join(",");terms.push(format!("SELECT position FROM events WHERE event_type IN ({placeholders}) AND {}",range(&mut args,low,high)));}
                alternatives.push(format!("SELECT position FROM ({})",terms.join(" INTERSECT ")));}
            format!("position IN ({})",alternatives.join(" UNION "))
        };
        (format!("SELECT {} FROM events WHERE {}{}",if exists{"1"}else{"position,command_id,event_type,payload_json,tags_json"},condition,if exists{" LIMIT 1"}else{" ORDER BY position"}),args)
    }
    fn insert(c:&Connection,e:&JsonEvent){
        c.prepare_cached("INSERT INTO events(position,command_id,event_type,payload_json,tags_json) VALUES(?,?,?,?,?)").unwrap().execute(params![e.position as i64,e.command_id,e.event_type,e.payload_json.as_str(),e.tags_json.as_str()]).unwrap();
        for tag in decode_tags(&e.tags_json){c.prepare_cached("INSERT INTO event_tags(position,category,keys_key) VALUES(?,?,?)").unwrap().execute(params![e.position as i64,tag.category,keys(&tag.keys)]).unwrap();}
    }
}
impl JsonJournal for SqliteJournal{
    fn engine_version(&self)->&str{&self.version}
    fn read(&self,q:&Query,after:usize)->JsonRead{
        let mut c=self.connection.lock().unwrap_or_else(std::sync::PoisonError::into_inner);let tx=c.transaction_with_behavior(TransactionBehavior::Deferred).unwrap();let high=Self::high(&tx);assert!(after<=high,"invalid position");let(sql,args)=Self::selection(q,after,high,false);
        let events={let mut s=tx.prepare_cached(&sql).unwrap();s.query_map(params_from_iter(args),|r|Ok(JsonEvent{position:usize::try_from(r.get::<_,i64>(0)?).expect("invalid position"),command_id:r.get(1)?,event_type:r.get(2)?,payload_json:Arc::new(r.get(3)?),tags_json:Arc::new(r.get(4)?)})).unwrap().collect::<Result<Vec<_>,_>>().unwrap()};
        tx.commit().unwrap();JsonRead{after:high,events}
    }
    fn append(&self,id:&str,q:&Query,after:usize,events:Vec<JsonEvent>)->JsonAppend{
        let mut c=self.connection.lock().unwrap_or_else(std::sync::PoisonError::into_inner);let tx=c.transaction_with_behavior(TransactionBehavior::Immediate).unwrap();let high=Self::high(&tx);assert!(!events.is_empty()&&!id.is_empty()&&after<=high,"invalid append");let(sql,args)=Self::selection(q,after,high,true);
        let conflict={let mut statement=tx.prepare_cached(&sql).unwrap();let mut rows=statement.query(params_from_iter(args)).unwrap();rows.next().unwrap().is_some()};
        if conflict{tx.rollback().unwrap();return JsonAppend::Conflict;}
        let rows:Vec<_>=events.into_iter().enumerate().map(|(i,mut e)|{e.position=high+i+1;e.command_id=id.into();e}).collect();for row in &rows{Self::insert(&tx,row);}tx.commit().unwrap();JsonAppend::Committed(rows)
    }
    fn load(&self,events:Vec<JsonEvent>){let mut c=self.connection.lock().unwrap_or_else(std::sync::PoisonError::into_inner);let tx=c.transaction_with_behavior(TransactionBehavior::Immediate).unwrap();assert_eq!(Self::high(&tx),0,"load requires empty journal");assert!(events.iter().enumerate().all(|(i,e)|e.position==i+1),"invalid imported position");for row in &events{Self::insert(&tx,row);}tx.commit().unwrap();}
}
