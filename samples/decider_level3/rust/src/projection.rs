use crate::{models::*,dcb::Recorded,decider};
use std::collections::BTreeMap;
#[derive(Clone,Default)]
pub struct Registers { pub students:BTreeMap<String,Student>, pub classes:BTreeMap<String,ClassState>, pub enrollments:BTreeMap<(String,String),Enrollment> }
pub fn apply_student(s:&mut Registers,e:&Event) {
    match e {Event::StudentRegistered {student_id,grade}|Event::StudentPromoted {student_id,grade}=>{s.students.insert(student_id.clone(),Student{student_id:student_id.clone(),grade:*grade});},_=>{}}
}
pub fn apply_class(s:&mut Registers,e:&Event) {
    if let Event::ClassOpened {class_id,course,capacity}=e {
        let (minimum_grade,prerequisite)=match course {Course::Foundation=>(Grade::Year2,Prerequisite::NoPrerequisite),Course::Advanced=>(Grade::Year3,Prerequisite::PassedCourse{course:Course::Foundation})};
        s.classes.insert(class_id.clone(),ClassState::Available {plan:Plan{class_id:class_id.clone(),course:*course,minimum_grade,prerequisite,capacity:*capacity},students:vec![]});return;
    }
    let id=match e {Event::StudentEnrolled{class_id,..}|Event::StudentDropped{class_id,..}|Event::ClassStarted{class_id,..}|Event::ClassCompleted{class_id,..}=>class_id,_=>return};
    let Some(room)=s.classes.get(id) else{return};
    let plan=decider::plan_of(room).clone();
    let next=match e {
        Event::StudentEnrolled{student_id,..}=>{let mut students=decider::members(room);students.push(student_id.clone());if students.len()==plan.capacity as usize {ClassState::Full{plan,students}}else{ClassState::Available{plan,students}}},
        Event::StudentDropped{student_id,..}=>ClassState::Available{plan,students:decider::members(room).into_iter().filter(|x|x!=student_id).collect()},
        Event::ClassStarted{started_on,..}=>ClassState::Teaching{plan,students:decider::members(room),started_on:started_on.clone()},
        Event::ClassCompleted{results,..}=>ClassState::Completed{plan,results:results.clone()},_=>unreachable!()
    };s.classes.insert(id.clone(),next);
}
pub fn apply_enrollment(s:&mut Registers,e:&Event) {
    if let Event::StudentEnrolled{class_id,student_id,course}=e {
        s.enrollments.insert((class_id.clone(),student_id.clone()),Enrollment{class_id:class_id.clone(),student_id:student_id.clone(),course:*course,progress:Progress::Reserved});return;
    }
    let (id,student)=match e {Event::StudentDropped{class_id,student_id}|Event::StudyStarted{class_id,student_id}|Event::AttendanceRecorded{class_id,student_id}|Event::ResultRecorded{class_id,student_id,..}=>(class_id,student_id),_=>return};
    let Some(record)=s.enrollments.get_mut(&(id.clone(),student.clone())) else{return};
    record.progress=match e {
        Event::StudentDropped{..}=>Progress::Withdrawn{reason:WithdrawalReason::StudentRequest},Event::StudyStarted{..}=>Progress::Studying{attendance:0},
        Event::AttendanceRecorded{..}=>match record.progress {Progress::Studying{attendance}=>Progress::Studying{attendance:attendance+1},_=>panic!("invalid committed event")},
        Event::ResultRecorded{score,..}=>if *score>=60 {Progress::Passed{score:*score,credits:if record.course==Course::Foundation{1}else{2}}}else{Progress::Failed{score:*score,reason:FailureReason::BelowPassMark}},_=>unreachable!()
    };
}
pub fn reduce(s:&mut Registers,e:&Event){apply_student(s,e);apply_class(s,e);apply_enrollment(s,e);}
pub fn fold(events:&[Recorded<Event>])->Registers{let mut s=Registers::default();for e in events{reduce(&mut s,&e.payload);}s}
pub fn decision_model(s:&Registers)->DecisionModel{DecisionModel{students:s.students.values().cloned().collect(),classes:s.classes.values().cloned().collect(),enrollments:s.enrollments.values().cloned().collect()}}
pub fn list(s:&Registers)->SchoolList{let m=decision_model(s);let summaries=m.enrollments.iter().map(|e|decider::describe(&e.progress)).collect();SchoolList{students:m.students,classes:m.classes,enrollments:m.enrollments,summaries}}
