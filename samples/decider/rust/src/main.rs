mod decider;
mod models;

use models::{Command, Event, State};
use serde::{Deserialize, Serialize};
use std::io::{self, Read};

#[derive(Deserialize)]
struct Input {
    commands: Vec<Command>,
    #[serde(default)]
    history: Vec<Event>,
}
#[derive(Serialize)]
struct Step {
    phase: &'static str,
    emitted: Vec<&'static str>,
    error: Option<&'static str>,
}
#[derive(Serialize)]
struct Output {
    initial: State,
    state: State,
    events: Vec<Event>,
    steps: Vec<Step>,
    restored: State,
    replayed: State,
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let mut json = String::new();
    io::stdin().read_to_string(&mut json)?;
    let input: Input = serde_json::from_str(&json)?;
    let mut history = input.history;
    let mut state = decider::replay(&history)?;
    let initial = state.clone();
    let mut steps = vec![];
    for command in input.commands {
        match decider::decide(&state, &command) {
            Ok(events) => {
                let emitted = events.iter().map(Event::tag).collect();
                for event in &events {
                    state = decider::evolve(&state, event)?;
                }
                history.extend(events);
                steps.push(Step {
                    phase: decider::phase(&state),
                    emitted,
                    error: None,
                });
            }
            Err(error) => steps.push(Step {
                phase: decider::phase(&state),
                emitted: vec![],
                error: Some(error),
            }),
        }
    }
    let events: Vec<Event> = serde_json::from_str(&serde_json::to_string(&history)?)?;
    let restored: State = serde_json::from_str(&serde_json::to_string(&state)?)?;
    let replayed = decider::replay(&events)?;
    println!(
        "{}",
        serde_json::to_string(&Output {
            initial,
            state,
            events,
            steps,
            restored,
            replayed
        })?
    );
    Ok(())
}
