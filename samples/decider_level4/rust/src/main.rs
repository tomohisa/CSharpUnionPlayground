#[path = "../../../decider_level3/rust/src/dcb.rs"]
mod dcb;
#[path = "../../../decider_level3/rust/src/decider.rs"]
mod decider;
#[path = "../../../decider_level3/rust/src/handlers.rs"]
mod handlers;
#[path = "../../../decider_level3/rust/src/json_store.rs"]
mod json_store;
#[path = "../../../decider_level3/rust/src/models.rs"]
mod models;
#[path = "../../../decider_level3/rust/src/projection.rs"]
mod projection;
#[path = "../../../decider_level3/rust/src/routing.rs"]
mod routing;
#[path = "../../../decider_level3/rust/src/school_projection.rs"]
mod school_projection;
#[path = "../../../decider_level3/rust/src/sqlite_journal.rs"]
mod sqlite_journal;
#[path = "../../../decider_level3/rust/src/value_objects.rs"]
mod value_objects;
use dcb::*;
use json_store::*;
use models::*;
use school_projection::SchoolProjection;
use serde_json::{Value, json};
use std::{
    collections::HashMap,
    io::Read,
    sync::{Arc, Mutex},
    time::{Duration, Instant},
};
type HttpResult = Result<(u16, Value), (u16, &'static str)>;
fn validate(body: &Value, schema: &Value, schemas: &Value) -> Result<(), (u16, &'static str)> {
    let bad = (400, "InvalidRequest");
    if let Some(reference) = schema["$ref"].as_str() {
        return validate(
            body,
            &schemas[reference.rsplit('/').next().unwrap()],
            schemas,
        );
    }
    if let Some(choices) = schema["oneOf"].as_array() {
        let kind = body["$type"].as_str().ok_or(bad)?;
        if !choices
            .iter()
            .any(|c| c["$ref"] == format!("#/components/schemas/{kind}"))
        {
            return Err(bad);
        };
        return validate(body, &schemas[kind], schemas);
    }
    match schema["type"].as_str() {
        Some("object") => {
            let object = body.as_object().ok_or(bad)?;
            for k in schema["required"].as_array().unwrap() {
                if !object.contains_key(k.as_str().unwrap()) {
                    return Err(bad);
                }
            }
            for (k, v) in object {
                let child = schema["properties"].get(k).ok_or(bad)?;
                validate(v, child, schemas)?;
            }
        }
        Some("string") => {
            let s = body.as_str().ok_or(bad)?;
            if schema.get("const").is_some() && schema["const"] != s {
                return Err(bad);
            }
            if let Some(values) = schema["enum"].as_array() {
                if !values.contains(body) {
                    return Err(bad);
                }
            }
            if schema.get("minLength").is_some() && (s.trim().is_empty() || s.chars().count() > 128)
            {
                return Err(bad);
            }
        }
        Some("integer") => {
            let n = body.as_f64().ok_or(bad)?;
            if n != n.trunc() || n < i32::MIN as f64 || n > i32::MAX as f64 {
                return Err(bad);
            }
        }
        _ => return Err(bad),
    }
    Ok(())
}
struct Ticket {
    created: Instant,
    command: OwnedPrepared<Rejection>,
}
struct Engine {
    store: Arc<JsonEventStore<Event>>,
    projection: Arc<SchoolProjection>,
    schemas: Value,
    tickets: Mutex<HashMap<String, Ticket>>,
    ttl: Duration,
    next: std::sync::atomic::AtomicU64,
}
impl Engine {
    fn commit(&self, t: OwnedPrepared<Rejection>) -> HttpResult {
        let r = t.commit();
        Ok((
            if r.status == "Committed" {
                200
            } else if r.status == "Rejected" {
                422
            } else {
                409
            },
            json!({"status":r.status,"conflicts":r.conflicts,"appended":r.appended,"position":r.position,"errors":r.errors,"projectionPosition":self.projection.checkpoint()}),
        ))
    }
    fn handle(&self, request: &mut tiny_http::Request) -> HttpResult {
        let route = request.url().split('?').next().unwrap().to_owned();
        let get = ["/health", "/school", "/status"].contains(&route.as_str())
            || route.starts_with("/students/");
        let post = ["/commands", "/tickets"].contains(&route.as_str())
            || route.starts_with("/tickets/")
                && route.ends_with("/commit")
                && route.split('/').count() == 4;
        if !get && !post {
            return Err((404, "NotFound"));
        }
        if request.method().as_str() != if get { "GET" } else { "POST" } {
            return Err((405, "MethodNotAllowed"));
        }
        if get {
            self.store.refresh();
            if self.store.listener_faults() != 0 {
                return Err((500, "InternalError"));
            }
            return Ok((
                200,
                match route.as_str() {
                    "/health" => json!({"status":"ok"}),
                    "/status" => {
                        json!({"eventPosition":self.store.read(&Query{items:vec![]}).after,"projectionPosition":self.projection.checkpoint(),"listenerFaults":self.store.listener_faults()})
                    }
                    "/school" => {
                        let (p, s) = self.projection.snapshot();
                        json!({"position":p,"state":s})
                    }
                    _ => {
                        let id = percent_encoding::percent_decode_str(
                            route.strip_prefix("/students/").unwrap(),
                        )
                        .decode_utf8()
                        .map_err(|_| (400, "InvalidRequest"))?;
                        let (p, s) = self.projection.find_student(&id);
                        json!({"position":p,"student":s.ok_or((404,"NotFound"))?})
                    }
                },
            ));
        }
        if route == "/commands" || route == "/tickets" {
            let content = request
                .headers()
                .iter()
                .find(|h| h.field.equiv("Content-Type"))
                .map(|h| h.value.as_str())
                .unwrap_or("");
            if content.split(';').next().unwrap().trim() != "application/json" {
                return Err((415, "UnsupportedMediaType"));
            }
            let mut raw = vec![];
            request
                .as_reader()
                .take(1048577)
                .read_to_end(&mut raw)
                .map_err(|_| (400, "InvalidRequest"))?;
            if raw.len() > 1048576 {
                return Err((413, "PayloadTooLarge"));
            }
            let input: Value = serde_json::from_slice(&raw).map_err(|_| (400, "InvalidRequest"))?;
            validate(&input, &self.schemas["CommandRequest"], &self.schemas)?;
            let mut command_value = input["command"].clone();
            for value in command_value.as_object_mut().unwrap().values_mut() {
                if value.is_number() && value.as_i64().is_none() {
                    *value = json!(value.as_f64().unwrap() as i32);
                }
            }
            let command: Command =
                serde_json::from_value(command_value).map_err(|_| (400, "InvalidRequest"))?;
            let t = prepare_owned(
                self.store.clone(),
                input["commandId"].as_str().unwrap().into(),
                command,
                Arc::new(handlers::SchoolHandler),
            );
            if route == "/commands" || !t.was_read {
                return self.commit(t);
            }
            let mut tickets = self.tickets.lock().unwrap();
            tickets.retain(|_, v| v.created.elapsed() < self.ttl);
            if tickets.len() >= 1024 {
                return Err((429, "TooManyTickets"));
            }
            let id = self
                .next
                .fetch_add(1, std::sync::atomic::Ordering::Relaxed)
                .to_string();
            let position = t.after;
            tickets.insert(
                id.clone(),
                Ticket {
                    created: Instant::now(),
                    command: t,
                },
            );
            Ok((201, json!({"ticket":id,"position":position})))
        } else {
            let ticket = {
                let mut tickets = self.tickets.lock().unwrap();
                tickets.retain(|_, v| v.created.elapsed() < self.ttl);
                tickets
                    .remove(route.split('/').nth(2).unwrap())
                    .ok_or((404, "NotFound"))?
            };
            self.commit(ticket.command)
        }
    }
}
fn main() {
    let schemas: Value =
        serde_json::from_str(&std::fs::read_to_string(std::env::var("API_SPEC").unwrap()).unwrap())
            .unwrap();
    let journal: Box<dyn JsonJournal> = match std::env::var("DECIDER_PERSISTENT") {
        Ok(p) => Box::new(sqlite_journal::SqliteJournal::new(&p, "FULL")),
        Err(_) => Box::new(MemoryJsonJournal::new()),
    };
    let store = Arc::new(JsonEventStore::new(
        journal,
        routing::describe,
        |e| serde_json::to_string(e).unwrap(),
        |s| serde_json::from_str(s).unwrap(),
    ));
    let projection = Arc::new(SchoolProjection::new());
    let listener = projection.clone();
    store.subscribe(Box::new(move |events| listener.accept(events)));
    let engine = Arc::new(Engine {
        store,
        projection,
        schemas: schemas["components"]["schemas"].clone(),
        tickets: Mutex::new(HashMap::new()),
        ttl: Duration::from_secs_f64(
            std::env::var("TICKET_TTL_SECONDS")
                .unwrap_or("60".into())
                .parse()
                .unwrap(),
        ),
        next: std::sync::atomic::AtomicU64::new(1),
    });
    let server = Arc::new(
        tiny_http::Server::http(format!(
            "127.0.0.1:{}",
            std::env::var("PORT").unwrap_or("8080".into())
        ))
        .unwrap(),
    );
    let workers: Vec<_> = (0..8)
        .map(|_| {
            let server = server.clone();
            let engine = engine.clone();
            std::thread::spawn(move || {
                for mut request in server.incoming_requests() {
                    let (code, body) = engine
                        .handle(&mut request)
                        .unwrap_or_else(|(c, e)| (c, json!({"error":e})));
                    let response = tiny_http::Response::from_string(body.to_string())
                        .with_status_code(code)
                        .with_header(
                            tiny_http::Header::from_bytes("Content-Type", "application/json")
                                .unwrap(),
                        );
                    let _ = request.respond(response);
                }
            })
        })
        .collect();
    for worker in workers {
        worker.join().unwrap();
    }
}
