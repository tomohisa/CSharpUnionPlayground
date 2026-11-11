mod models;
mod decider;
use models::*;
use serde::{Deserialize, Serialize};
use std::io::{self, Read};
#[derive(Deserialize)]
struct Input { commands: Vec<Command>, history: Vec<Event> }
#[derive(Serialize)]
struct Output { initial: State, state: State, events: Vec<Event>, steps: Vec<Decision>, restored: State, replayed: State, summaries: Vec<String> }
fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut text = String::new(); io::stdin().read_to_string(&mut text)?;
    let input: Input = serde_json::from_str(&text)?;
    let mut history = input.history;
    let mut state = decider::replay(&history).map_err(|e| format!("{e:?}"))?;
    let initial = state.clone(); let mut steps = vec![];
    for command in input.commands {
        let decision = decider::decide(&state, &command);
        if let Decision::Accepted {events} = &decision {
            for event in events { state = decider::evolve(&state,event).map_err(|e| format!("{e:?}"))?; history.push(event.clone()); }
        }
        steps.push(decision);
    }
    let events: Vec<Event> = serde_json::from_str(&serde_json::to_string(&history)?)?;
    let restored = serde_json::from_str(&serde_json::to_string(&state)?)?;
    let replayed = decider::replay(&events).map_err(|e| format!("{e:?}"))?;
    let summaries = state.enrollments.iter().map(|e| decider::describe(&e.progress)).collect();
    println!("{}", serde_json::to_string(&Output {initial,state,events,steps,restored,replayed,summaries})?);
    Ok(())
}
