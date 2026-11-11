use crate::models::{Command, Event, Roster, State};

pub fn phase(state: &State) -> &'static str {
    match state {
        State::NotCreated => "NotCreated",
        State::Available { .. } => "Available",
        State::Full { .. } => "Full",
    }
}
fn data(state: &State) -> Option<&Roster> {
    match state {
        State::NotCreated => None,
        State::Available { roster } => Some(roster),
        State::Full { roster } => Some(roster),
    }
}
pub fn decide(state: &State, command: &Command) -> Result<Vec<Event>, &'static str> {
    match command {
        Command::Create { name, capacity } => {
            if !matches!(state, State::NotCreated) {
                return Err("already_created");
            }
            if name.is_empty() {
                return Err("invalid_name");
            }
            if !(1..=10).contains(capacity) {
                return Err("invalid_capacity");
            }
            Ok(vec![Event::RoomCreated {
                name: name.clone(),
                capacity: *capacity,
            }])
        }
        Command::Enroll { student_id } => change(state, student_id, true),
        Command::Drop { student_id } => change(state, student_id, false),
    }
}
fn change(state: &State, student: &str, enrolling: bool) -> Result<Vec<Event>, &'static str> {
    let roster = data(state).ok_or("not_created")?;
    if student.is_empty() {
        return Err("invalid_student");
    }
    let member = roster.students.iter().any(|s| s == student);
    if enrolling {
        if member {
            return Err("already_enrolled");
        }
        if matches!(state, State::Full { .. }) {
            return Err("full");
        }
        Ok(vec![Event::StudentEnrolled {
            student_id: student.to_owned(),
        }])
    } else if member {
        Ok(vec![Event::StudentDropped {
            student_id: student.to_owned(),
        }])
    } else {
        Err("not_enrolled")
    }
}
fn classify(roster: Roster) -> State {
    if roster.students.len() == roster.capacity as usize {
        State::Full { roster }
    } else {
        State::Available { roster }
    }
}
pub fn evolve(state: &State, event: &Event) -> Result<State, &'static str> {
    match event {
        Event::RoomCreated { name, capacity } => {
            if !matches!(state, State::NotCreated)
                || name.is_empty()
                || !(1..=10).contains(capacity)
            {
                return Err("invalid_history");
            }
            Ok(State::Available {
                roster: Roster {
                    name: name.clone(),
                    capacity: *capacity,
                    students: vec![],
                },
            })
        }
        Event::StudentEnrolled { student_id } => match state {
            State::Available { roster }
                if !student_id.is_empty() && !roster.students.contains(student_id) =>
            {
                let mut next = roster.clone();
                next.students.push(student_id.clone());
                Ok(classify(next))
            }
            State::NotCreated | State::Available { .. } | State::Full { .. } => {
                Err("invalid_history")
            }
        },
        Event::StudentDropped { student_id } => {
            let roster = data(state).ok_or("invalid_history")?;
            if !roster.students.contains(student_id) {
                return Err("invalid_history");
            }
            let mut next = roster.clone();
            next.students.retain(|s| s != student_id);
            Ok(classify(next))
        }
    }
}
pub fn replay(events: &[Event]) -> Result<State, &'static str> {
    events
        .iter()
        .try_fold(State::NotCreated, |state, event| evolve(&state, event))
}
