mod dcb;mod dcb_tests;mod models;mod decider;mod projection;mod routing;mod application;
use serde::Deserialize;
use std::io::{self,Read};
#[derive(Deserialize)]
struct Input{history:Vec<dcb::Recorded<models::Event>>,operations:Vec<models::Operation>}
fn main()->Result<(),Box<dyn std::error::Error>>{
    if std::env::args().any(|a|a=="--self-test"){dcb_tests::run();println!("{{\"library\":\"PASS\"}}");return Ok(());}
    let mut text=String::new();io::stdin().read_to_string(&mut text)?;let input:Input=serde_json::from_str(&text)?;
    println!("{}",serde_json::to_string(&application::run(input.history,input.operations))?);Ok(())
}
