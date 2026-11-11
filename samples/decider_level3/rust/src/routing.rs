use crate::{dcb::*,models::*,projection,decider};
fn student(id:&str)->Tag{Tag{category:"student".into(),keys:vec![id.into()]}}
fn classroom(id:&str)->Tag{Tag{category:"class".into(),keys:vec![id.into()]}}
fn enrollment(c:&str,s:&str)->Tag{Tag{category:"enrollment".into(),keys:vec![c.into(),s.into()]}}
pub fn query_for(command:&Command)->Query{
    let tags=match command{
        Command::RegisterStudent{student_id,..}=>vec![student(student_id)],
        Command::PromoteStudent{student_id,..}=>vec![student(student_id)],
        Command::OpenClass{class_id,..}=>vec![classroom(class_id)],
        Command::Enroll{class_id,student_id,..}=>vec![student(student_id),classroom(class_id)],
        Command::Drop{class_id,student_id,..}=>vec![classroom(class_id)],
        Command::StartClass{class_id,..}=>vec![classroom(class_id)],
        Command::Attend{class_id,student_id,..}=>vec![classroom(class_id)],
        Command::Assess{class_id,student_id,..}=>vec![classroom(class_id)],
        Command::CompleteClass{class_id,..}=>vec![classroom(class_id)],
    };Query{items:tags.into_iter().map(|t|QueryItem{tags:vec![t],types:vec![]}).collect()}
}
pub fn describe(event:&Event)->EventDescriptor{
    let (event_type,tags)=match event{
        Event::StudentRegistered{student_id,..}=>("StudentRegistered",vec![student(student_id)]),
        Event::StudentPromoted{student_id,..}=>("StudentPromoted",vec![student(student_id)]),
        Event::ClassOpened{class_id,..}=>("ClassOpened",vec![classroom(class_id)]),
        Event::StudentEnrolled{class_id,student_id,..}=>("StudentEnrolled",vec![student(student_id),classroom(class_id),enrollment(class_id,student_id)]),
        Event::StudentDropped{class_id,student_id,..}=>("StudentDropped",vec![student(student_id),classroom(class_id),enrollment(class_id,student_id)]),
        Event::ClassStarted{class_id,..}=>("ClassStarted",vec![classroom(class_id)]),
        Event::AttendanceRecorded{class_id,student_id,..}=>("AttendanceRecorded",vec![student(student_id),classroom(class_id),enrollment(class_id,student_id)]),
        Event::ResultRecorded{class_id,student_id,..}=>("ResultRecorded",vec![student(student_id),classroom(class_id),enrollment(class_id,student_id)]),
        Event::ClassCompleted{class_id,..}=>("ClassCompleted",vec![classroom(class_id)]),
        Event::StudyStarted{class_id,student_id,..}=>("StudyStarted",vec![student(student_id),classroom(class_id),enrollment(class_id,student_id)]),
    };EventDescriptor{event_type:event_type.into(),tags}
}
pub fn decide(events:&[Recorded<Event>],command:&Command)->Preparation<Event,Rejection>{
    match decider::decide(&projection::decision_model(&projection::fold(events)),command){
        Decision::Accepted{events}=>Preparation::Emit(events),Decision::Rejected{code}=>Preparation::Reject(code)
    }
}
