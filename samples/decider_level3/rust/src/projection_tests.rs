use crate::{dcb::Recorded,models::*,school_projection::SchoolProjection,decider};
use std::time::Instant;
fn envelope(position:usize,payload:Event)->Recorded<Event>{Recorded{position,command_id:"scale".into(),event_type:"projection-test".into(),tags:vec![],payload}}
fn send(p:&SchoolProjection,position:&mut usize,e:Event){*position+=1;assert!(p.accept(&[envelope(*position,e)]));}
pub fn run(size:usize){
    let updates=1000;let p=SchoolProjection::new();let mut position=0;let start=Instant::now();
    send(&p,&mut position,Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:(size+updates+1)as i32});
    for i in 0..size{let id=format!("s{i}");send(&p,&mut position,Event::StudentRegistered{student_id:id.clone(),grade:Grade::Year3});send(&p,&mut position,Event::StudentEnrolled{class_id:"a".into(),student_id:id,course:Course::Foundation});}
    let seed=start.elapsed().as_secs_f64();let(_,old)=p.snapshot();let before=p.checkpoint();let work=p.work();
    assert!(!p.accept(&[envelope(before+1,Event::StudentPromoted{student_id:"s0".into(),grade:Grade::Year1}),envelope(before+2,Event::AttendanceRecorded{class_id:"a".into(),student_id:"s0".into()})]));
    assert_eq!(p.checkpoint(),before);assert_eq!(p.work(),work);assert_eq!(p.snapshot().1.students.iter().find(|s|s.student_id=="s0").unwrap().grade,Grade::Year3);
    assert!(!p.accept(&[envelope(before+1,Event::StudentPromoted{student_id:"s0".into(),grade:Grade::Year1}),envelope(before+3,Event::StudentPromoted{student_id:"s0".into(),grade:Grade::Year1})]));
    assert_eq!(p.checkpoint(),before);assert_eq!(p.work(),work);assert!(p.accept(&[envelope(1,Event::ClassOpened{class_id:"ignored".into(),course:Course::Foundation,capacity:1})]));assert_eq!(p.work(),work);
    let stable_rows=p.row_count();
    for _ in 0..100{send(&p,&mut position,Event::StudentDropped{class_id:"a".into(),student_id:"s1".into()});send(&p,&mut position,Event::StudentEnrolled{class_id:"a".into(),student_id:"s1".into(),course:Course::Foundation});}
    let order=decider::members(&p.snapshot().1.classes[0]);assert_eq!(p.row_count(),stable_rows);assert_eq!(order.len(),size);assert_eq!(order[0],"s0");assert_eq!(order[1],"s2");assert_eq!(order.last().unwrap(),"s1");
    let start=Instant::now();let work=p.work();
    for i in size..size+updates{let id=format!("s{i}");send(&p,&mut position,Event::StudentRegistered{student_id:id.clone(),grade:Grade::Year3});send(&p,&mut position,Event::StudentEnrolled{class_id:"a".into(),student_id:id,course:Course::Foundation});}
    let growth=start.elapsed().as_secs_f64();let growth_work=p.work()-work;assert_eq!(growth_work,8*updates as u64);assert_eq!(decider::members(&old.classes[0]).len(),size);assert_eq!(old.students.len(),size);
    send(&p,&mut position,Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()});send(&p,&mut position,Event::StudyStarted{class_id:"a".into(),student_id:"s0".into()});
    let start=Instant::now();let work=p.work();
    for _ in 0..updates{send(&p,&mut position,Event::AttendanceRecorded{class_id:"a".into(),student_id:"s0".into()});}
    let update=start.elapsed().as_secs_f64();let update_work=p.work()-work;assert_eq!(update_work,3*updates as u64);
    let start=Instant::now();let(_,snapshot)=p.snapshot();let query=start.elapsed().as_secs_f64();
    assert_eq!(snapshot.students.len(),size+updates);assert_eq!(decider::members(&snapshot.classes[0]).len(),size+updates);assert_eq!(snapshot.enrollments.iter().find(|e|e.student_id=="s0").unwrap().progress,Progress::Studying{attendance:updates as i32});
    println!("{}",serde_json::json!({"size":size,"updates":updates,"rows":p.row_count(),"seedSeconds":seed,"updateSeconds":update,"growthSeconds":growth,"snapshotSeconds":query,"updateWork":update_work,"growthWork":growth_work}));
}
