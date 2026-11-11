use serde::{Deserialize, Serialize};

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
pub struct Roster {
    pub name: String,
    pub capacity: i32,
    pub students: Vec<String>,
}

#[derive(Clone, Debug, PartialEq, Serialize, Deserialize)]
#[serde(tag = "$type")]
pub enum State {
    NotCreated,
    Available { roster: Roster },
    Full { roster: Roster },
}

#[derive(Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Command {
    Create { name: String, capacity: i32 },
    Enroll { student_id: String },
    Drop { student_id: String },
}

#[derive(Clone, Serialize, Deserialize)]
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Event {
    RoomCreated { name: String, capacity: i32 },
    StudentEnrolled { student_id: String },
    StudentDropped { student_id: String },
}
impl Event {
    pub fn tag(&self) -> &'static str {
        match self {
            Self::RoomCreated { .. } => "RoomCreated",
            Self::StudentEnrolled { .. } => "StudentEnrolled",
            Self::StudentDropped { .. } => "StudentDropped",
        }
    }
}
