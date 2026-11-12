mod query_benchmark;
mod storage_benchmark;
mod dcb;mod json_store;mod sqlite_journal;mod dcb_tests;mod models;mod decider;mod projection;mod routing;mod application;mod handlers;mod projection_tests;mod school_projection;mod value_objects;mod domain_tests;
use serde::Deserialize;
use std::io::{self,Read};
#[derive(Deserialize)]
struct Input{history:Vec<dcb::Recorded<models::Event>>,operations:Vec<models::Operation>}
fn main()->Result<(),Box<dyn std::error::Error>>{
    let args:Vec<String>=std::env::args().collect();if args.len()==3 && args[1]=="--projection-scale"{projection_tests::run(args[2].parse()?);return Ok(());}
    if std::env::args().any(|a|a=="--domain-test"){domain_tests::run();return Ok(());}
    if std::env::args().any(|a|a=="--self-test"){dcb_tests::run();println!("{{\"library\":\"PASS\"}}");return Ok(());}
    let mut text=String::new();io::stdin().read_to_string(&mut text)?;let input:Input=serde_json::from_str(&text)?;
    let output=application::run(input.history,input.operations);
    if std::env::var("DECIDER_BENCHMARK_SUMMARY").as_deref()==Ok("1"){println!("{}",serde_json::to_string(&application::benchmark_summary(output))?);}else{println!("{}",serde_json::to_string(&output)?);}Ok(())
}
