use crate::{dcb::*,models::*,decider,projection,value_objects::AssessmentScore};
use std::sync::Arc;
#[derive(Clone,Hash,PartialEq,Eq)]
enum Key{Student(String),Class(String),Enrollment(String,String),Member(String,String),Node(usize)}
#[derive(Clone)]
struct Room{phase:Arc<ClassState>,first:usize,last:usize,count:usize}
#[derive(Clone)]
struct Member{student:String,previous:usize,next:usize}
#[derive(Clone)]
enum Row{Student(Student),Class(Room),Enrollment(Enrollment),Node(Member),Member(usize)}
fn node(tx:&mut IndexBatch<Key,Row>,n:usize)->Member{match tx.get(&Key::Node(n)){Row::Node(v)=>v,_=>panic!("wrong row kind")}}
fn apply(tx:&mut IndexBatch<Key,Row>,recorded:&Recorded<Event>)->Result<(),String>{
    let e=&recorded.payload;
    match e{
        Event::StudentRegistered{student_id,grade}|Event::StudentPromoted{student_id,grade}=>{tx.put(Key::Student(student_id.clone()),Row::Student(Student{student_id:student_id.clone(),grade:*grade}));return Ok(());},
        Event::ClassOpened{class_id,..}=>{let mut state=projection::Registers::default();projection::reduce(&mut state,e);let phase=state.classes.remove(class_id).unwrap();tx.put(Key::Class(class_id.clone()),Row::Class(Room{phase:Arc::new(phase),first:0,last:0,count:0}));return Ok(());},_=>{}
    }
    let id=match e{Event::StudentEnrolled{class_id,..}|Event::StudentDropped{class_id,..}|Event::ClassStarted{class_id,..}|Event::ClassCompleted{class_id,..}|Event::StudyStarted{class_id,..}|Event::AttendanceRecorded{class_id,..}|Event::ResultRecorded{class_id,..}=>class_id,_=>return Err("invariant".into())};
    if let Some(Row::Class(mut c))=tx.find(&Key::Class(id.clone())){
        let plan=decider::plan_of(&c.phase).clone();
        match e{
            Event::StudentEnrolled{student_id,..}=>{let n=recorded.position;if c.last!=0{let mut p=node(tx,c.last);p.next=n;tx.put(Key::Node(c.last),Row::Node(p));}
                tx.put(Key::Node(n),Row::Node(Member{student:student_id.clone(),previous:c.last,next:0}));tx.put(Key::Member(id.clone(),student_id.clone()),Row::Member(n));
                if c.first==0{c.first=n;}c.last=n;c.count+=1;c.phase=Arc::new(if c.count==plan.capacity as usize{ClassState::Full{plan,students:vec![]}}else{ClassState::Available{plan,students:vec![]}});},
            Event::StudentDropped{student_id,..}=>{if let Some(Row::Member(n))=tx.find(&Key::Member(id.clone(),student_id.clone())){
                let m=node(tx,n);if m.previous!=0{let mut p=node(tx,m.previous);p.next=m.next;tx.put(Key::Node(m.previous),Row::Node(p));}
                if m.next!=0{let mut p=node(tx,m.next);p.previous=m.previous;tx.put(Key::Node(m.next),Row::Node(p));}
                tx.delete(Key::Node(n));tx.delete(Key::Member(id.clone(),student_id.clone()));if c.first==n{c.first=m.next;}if c.last==n{c.last=m.previous;}c.count-=1;}
                c.phase=Arc::new(ClassState::Available{plan,students:vec![]});},
            Event::ClassStarted{started_on,..}=>c.phase=Arc::new(ClassState::Teaching{plan,students:vec![],started_on:started_on.clone()}),
            Event::ClassCompleted{results,..}=>c.phase=Arc::new(ClassState::Completed{plan,results:results.clone()}),_=>{}
        }
        if matches!(e,Event::StudentEnrolled{..}|Event::StudentDropped{..}|Event::ClassStarted{..}|Event::ClassCompleted{..}){tx.put(Key::Class(id.clone()),Row::Class(c));}
    }
    let student=match e{Event::StudentEnrolled{student_id,..}|Event::StudentDropped{student_id,..}|Event::StudyStarted{student_id,..}|Event::AttendanceRecorded{student_id,..}|Event::ResultRecorded{student_id,..}=>student_id,_=>return Ok(())};
    let key=Key::Enrollment(id.clone(),student.clone());
    if let Event::StudentEnrolled{course,..}=e{tx.put(key,Row::Enrollment(Enrollment{class_id:id.clone(),student_id:student.clone(),course:*course,progress:Progress::Reserved}));return Ok(());}
    if let Some(Row::Enrollment(mut value))=tx.find(&key){
        value.progress=match e{Event::StudentDropped{..}=>Progress::Withdrawn{reason:WithdrawalReason::StudentRequest},Event::StudyStarted{..}=>Progress::Studying{attendance:0},
            Event::AttendanceRecorded{..}=>match value.progress{Progress::Studying{attendance}=>Progress::Studying{attendance:attendance+1},_=>return Err("invalid committed event".into())},
            Event::ResultRecorded{score,..}=>if *score>=60{Progress::Passed{score:*AssessmentScore::from_event(*score).value(),credits:if value.course==Course::Foundation{1}else{2}}}else{Progress::Failed{score:*score,reason:FailureReason::BelowPassMark}},_=>value.progress};
        tx.put(key,Row::Enrollment(value));
    }
    Ok(())
}
pub struct SchoolProjection{index:IndexedProjection<Key,Row,Event>}
impl SchoolProjection{
    pub fn new()->Self{Self{index:IndexedProjection::new(apply)}}
    pub fn accept(&self,events:&[Recorded<Event>])->bool{self.index.accept(events)}
    pub fn checkpoint(&self)->usize{self.index.checkpoint()}
    pub fn row_count(&self)->usize{self.index.row_count()}
    pub fn work(&self)->u64{self.index.work()}
    pub fn student(&self,id:&str)->Student{self.index.read(|rows|match &rows[&Key::Student(id.into())]{Row::Student(s)=>s.clone(),_=>panic!("not a student")}).1}
    pub fn find_student(&self,id:&str)->(usize,Option<Student>){self.index.read(|rows|match rows.get(&Key::Student(id.into())){Some(Row::Student(s))=>Some(s.clone()),_=>None})}
    pub fn snapshot(&self)->(usize,SchoolList){self.index.read(|rows|{
        let(mut students,mut classes,mut enrollments)=(vec![],vec![],vec![]);
        for row in rows.values(){match row{Row::Student(v)=>students.push(v.clone()),Row::Enrollment(v)=>enrollments.push(v.clone()),Row::Class(c)=>{
            let mut ids=vec![];if !matches!(&*c.phase,ClassState::Completed{..}){let mut n=c.first;while n!=0{let Row::Node(m)=&rows[&Key::Node(n)]else{panic!("wrong node")};ids.push(m.student.clone());n=m.next;}}
            classes.push(match &*c.phase{ClassState::Available{plan,..}=>ClassState::Available{plan:plan.clone(),students:ids},ClassState::Full{plan,..}=>ClassState::Full{plan:plan.clone(),students:ids},ClassState::Teaching{plan,started_on,..}=>ClassState::Teaching{plan:plan.clone(),students:ids,started_on:started_on.clone()},ClassState::Completed{..}=>(*c.phase).clone()});},_=>{}}}
        students.sort_by(|a,b|a.student_id.cmp(&b.student_id));classes.sort_by(|a,b|decider::plan_of(a).class_id.cmp(&decider::plan_of(b).class_id));enrollments.sort_by(|a,b|(&a.class_id,&a.student_id).cmp(&(&b.class_id,&b.student_id)));
        let summaries=enrollments.iter().map(|e|decider::describe(&e.progress)).collect();SchoolList{students,classes,enrollments,summaries}
    })}
}
