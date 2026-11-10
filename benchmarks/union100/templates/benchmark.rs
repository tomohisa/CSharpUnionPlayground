use serde::{Serialize, Deserialize};
use std::{collections::BTreeMap, time::Instant, hint::black_box};
#[derive(Serialize, Deserialize)]
struct Payload { id: i32, label: String, values: Vec<i32>, tags: BTreeMap<String,String>, next: Option<Box<Payment>> }
@@TYPES@@
#[derive(Serialize, Deserialize)]
#[serde(tag="$type")]
enum Payment {
@@CASES@@
}
fn create(seed: i32, depth: i32) -> Payment {
    let data = Payload { id: seed, label: format!("item-{seed}"), values: vec![seed,seed+1,seed+2,seed+3], tags: BTreeMap::from([("group".into(),format!("g{}",seed%7)),("note".into(),"bench".into())]), next: if depth>1 { Some(Box::new(create(seed+37,depth-1))) } else {None} };
    match seed%100 {
@@CREATE@@
        _=>unreachable!()
    }
}
fn score(payment: &Payment) -> i64 {
    let (data,index) = match payment {
@@DISPATCH@@
    };
    index + data.id as i64 + data.label.len() as i64 + data.values.iter().map(|v|*v as i64).sum::<i64>() + data.tags.iter().map(|(k,v)|(k.len()+v.len()) as i64).sum::<i64>() + data.next.as_ref().map_or(0,|p|score(p))
}
fn main() {
    use std::io::Write;
    println!("{{\"event\":\"ready\"}}");std::io::stdout().flush().unwrap();
    let args: Vec<String> = std::env::args().collect();
    let roots: i32 = args[1].parse().unwrap(); let samples: usize = args[2].parse().unwrap();
    let warm: Vec<_> = (0..roots.min(1000)).map(|i|create(i,3)).collect();
    let _: Vec<Payment> = serde_json::from_slice(&serde_json::to_vec(&warm).unwrap()).unwrap();
    let original: Vec<_>=(0..roots).map(|i|create(i,3)).collect();
    let expected: i64=original.iter().map(score).sum();
    let mut measurements=Vec::new(); let mut sink=0i64;
    println!("{{\"event\":\"prepared\"}}");std::io::stdout().flush().unwrap();
    for trial in 0..samples {
        let start=Instant::now();
        let json=serde_json::to_vec(black_box(&original)).unwrap();
        let encoded=start.elapsed().as_secs_f64();
        let copy: Vec<Payment>=serde_json::from_slice(black_box(&json)).unwrap();
        let decoded=start.elapsed().as_secs_f64();
        let sum:i64=black_box(&copy).iter().map(score).sum();
        let total=start.elapsed().as_secs_f64();
        assert_eq!(sum,expected); assert_eq!(copy.len(),roots as usize);
        if trial==0 {assert_eq!(json,serde_json::to_vec(&copy).unwrap());}
        sink+=sum;
        measurements.push(serde_json::json!({"encodeSeconds":encoded,"decodeSeconds":decoded-encoded,"switchSeconds":total-decoded,"totalSeconds":total,"bytes":json.len()}));
    }
    println!("{}",serde_json::json!({"roots":roots,"samples":samples,"sink":sink,"measurements":measurements}));
}
