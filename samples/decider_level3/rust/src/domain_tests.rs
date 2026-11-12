use crate::{dcb::*,models::*,handlers::SchoolHandler,routing,projection::{self,Registers}};
use crate::school_projection::SchoolProjection;
use std::sync::{Arc,atomic::{AtomicUsize,Ordering}};
struct CountingStore<'a>{inner:&'a EventStore<Event>,reads:AtomicUsize}
impl CommandStore<Event> for CountingStore<'_>{
    fn read(&self,q:&Query)->ReadResult<Event>{self.reads.fetch_add(1,Ordering::SeqCst);self.inner.read(q)}
    fn append(&self,id:&str,q:&Query,after:usize,events:Vec<Event>)->AppendResult<Event>{self.inner.append(id,q,after,events)}
}
fn given_history(events:Vec<Event>)->(EventStore<Event>,Arc<SchoolProjection>){
    let store=EventStore::new(routing::describe,Clone::clone);
    if !events.is_empty(){store.append("given",&Query{items:vec![]},0,events);}
    let projection=Arc::new(SchoolProjection::new());let listener=projection.clone();
    store.subscribe(Box::new(move|batch|listener.accept(batch)));(store,projection)
}
fn check_projection(store:&EventStore<Event>,projection:&SchoolProjection){
    let events=store.read(&Query{items:vec![]}).events;let(position,state)=projection.snapshot();
    assert_eq!(position,events.len());assert_eq!(state,projection::list(&projection::fold(&events)));
}
fn check(name:&str,given:Vec<Event>,when:Command,then_events:Vec<Event>,then_errors:Vec<Rejection>,invalid:bool){
    let before=given.len();let(store,projection)=given_history(given); // Given
    let port=CountingStore{inner:&store,reads:AtomicUsize::new(0)};let executor=DcbExecutor{store:&port};
    let prepared=executor.prepare("when",when,&SchoolHandler); // When
    assert_eq!(prepared.was_read,!invalid,"{name}: validation before reads");assert_eq!(port.reads.load(Ordering::SeqCst),if invalid{0}else{1},"{name}");
    let result=prepared.commit();let appended:Vec<_>=store.read(&Query{items:vec![]}).events.into_iter().skip(before).map(|e|e.payload).collect();
    assert_eq!(result.status,if then_errors.is_empty(){"Committed"}else{"Rejected"},"{name}"); // Then
    assert_eq!(result.errors,then_errors,"{name}");assert_eq!(appended,then_events,"{name}");assert_eq!(result.appended,then_events.len());assert_eq!(result.conflicts,0);
    check_projection(&store,&projection);
}
pub fn run(){
        let empty=vec![];
        let registered=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3}];
        let opened=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1}];
        let enrolled=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation}];
        let teaching=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()}];
        let attended=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()},Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()}];
        let passed=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()},Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()},Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:60}];
        let failed=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()},Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()},Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:59}];
        let advanced=vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()},Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()},Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:60},Event::ClassOpened{class_id:"b".into(),course:Course::Advanced,capacity:1}];
        check("register student",empty.clone(),Command::RegisterStudent{student_id:"s".into(),grade:Grade::Year3},vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3}],vec![],false);
        check("duplicate student",registered.clone(),Command::RegisterStudent{student_id:"s".into(),grade:Grade::Year3},vec![],vec![Rejection::DuplicateStudent],false);
        check("promote one grade",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year1}],Command::PromoteStudent{student_id:"s".into(),grade:Grade::Year2},vec![Event::StudentPromoted{student_id:"s".into(),grade:Grade::Year2}],vec![],false);
        check("promotion cannot skip",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year1}],Command::PromoteStudent{student_id:"s".into(),grade:Grade::Year3},vec![],vec![Rejection::InvalidPromotion],false);
        check("promotion cannot go backwards",registered.clone(),Command::PromoteStudent{student_id:"s".into(),grade:Grade::Year2},vec![],vec![Rejection::InvalidPromotion],false);
        check("unknown student promotion",empty.clone(),Command::PromoteStudent{student_id:"s".into(),grade:Grade::Year2},vec![],vec![Rejection::UnknownStudent],false);
        check("open foundation",registered.clone(),Command::OpenClass{class_id:"a".into(),course:Course::Foundation,capacity:1},vec![Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1}],vec![],false);
        check("open advanced",registered.clone(),Command::OpenClass{class_id:"b".into(),course:Course::Advanced,capacity:10},vec![Event::ClassOpened{class_id:"b".into(),course:Course::Advanced,capacity:10}],vec![],false);
        check("capacity upper bound",empty.clone(),Command::OpenClass{class_id:"a".into(),course:Course::Foundation,capacity:10},vec![Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:10}],vec![],false);
        check("duplicate class",opened.clone(),Command::OpenClass{class_id:"a".into(),course:Course::Foundation,capacity:1},vec![],vec![Rejection::DuplicateClass],false);
        check("enroll",opened.clone(),Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation}],vec![],false);
        check("unknown class",registered.clone(),Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::UnknownClass],false);
        check("unknown student enrollment",vec![Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1}],Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::UnknownStudent],false);
        check("grade below minimum",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year1},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1}],Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::GradeTooLow],false);
        check("grade exactly minimum",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year2},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1}],Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation}],vec![],false);
        check("missing prerequisite",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"b".into(),course:Course::Advanced,capacity:1}],Command::Enroll{class_id:"b".into(),student_id:"s".into()},vec![],vec![Rejection::PrerequisiteMissing],false);
        check("failed prerequisite",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()},Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()},Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:59},Event::ClassOpened{class_id:"b".into(),course:Course::Advanced,capacity:1}],Command::Enroll{class_id:"b".into(),student_id:"s".into()},vec![],vec![Rejection::PrerequisiteMissing],false);
        check("passed prerequisite",advanced.clone(),Command::Enroll{class_id:"b".into(),student_id:"s".into()},vec![Event::StudentEnrolled{class_id:"b".into(),student_id:"s".into(),course:Course::Advanced}],vec![],false);
        check("duplicate enrollment",enrolled.clone(),Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::AlreadyEnrolled],false);
        check("last seat occupied",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::StudentRegistered{student_id:"t".into(),grade:Grade::Year3}],Command::Enroll{class_id:"a".into(),student_id:"t".into()},vec![],vec![Rejection::ClassFull],false);
        check("enrollment after start",teaching.clone(),Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::RegistrationClosed],false);
        check("drop reservation",enrolled.clone(),Command::Drop{class_id:"a".into(),student_id:"s".into()},vec![Event::StudentDropped{class_id:"a".into(),student_id:"s".into()}],vec![],false);
        check("drop nonmember",opened.clone(),Command::Drop{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::NotEnrolled],false);
        check("drop after start",teaching.clone(),Command::Drop{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::RegistrationClosed],false);
        check("reenroll after withdrawal",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::StudentDropped{class_id:"a".into(),student_id:"s".into()}],Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation}],vec![],false);
        check("start empty class",opened.clone(),Command::StartClass{class_id:"a".into(),started_on:"2026-04-01".into()},vec![Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()}],vec![],false);
        check("start emits study event atomically",enrolled.clone(),Command::StartClass{class_id:"a".into(),started_on:"2026-04-01".into()},vec![Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()}],vec![],false);
        check("start twice",teaching.clone(),Command::StartClass{class_id:"a".into(),started_on:"2026-04-01".into()},vec![],vec![Rejection::RegistrationClosed],false);
        check("valid leap date",opened.clone(),Command::StartClass{class_id:"a".into(),started_on:"2028-02-29".into()},vec![Event::ClassStarted{class_id:"a".into(),started_on:"2028-02-29".into()}],vec![],false);
        check("attendance before start",enrolled.clone(),Command::Attend{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::NotTeaching],false);
        check("attendance",teaching.clone(),Command::Attend{class_id:"a".into(),student_id:"s".into()},vec![Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()}],vec![],false);
        check("attendance nonmember",teaching.clone(),Command::Attend{class_id:"a".into(),student_id:"t".into()},vec![],vec![Rejection::NotEnrolled],false);
        check("assessment without attendance",teaching.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:60},vec![],vec![Rejection::NoAttendance],false);
        check("assessment boundary 0",attended.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:0},vec![Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:0}],vec![],false);
        check("assessment boundary 59",attended.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:59},vec![Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:59}],vec![],false);
        check("assessment boundary 60",attended.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:60},vec![Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:60}],vec![],false);
        check("assessment boundary 100",attended.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:100},vec![Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:100}],vec![],false);
        check("assessment before start",enrolled.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:60},vec![],vec![Rejection::NotTeaching],false);
        check("assessment nonmember",attended.clone(),Command::Assess{class_id:"a".into(),student_id:"t".into(),score:60},vec![],vec![Rejection::NotEnrolled],false);
        check("assessment twice",passed.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:60},vec![],vec![Rejection::AlreadyAssessed],false);
        check("attendance after assessment",passed.clone(),Command::Attend{class_id:"a".into(),student_id:"s".into()},vec![],vec![Rejection::AlreadyAssessed],false);
        check("completion before start",opened.clone(),Command::CompleteClass{class_id:"a".into()},vec![],vec![Rejection::NotTeaching],false);
        check("completion with unassessed student",teaching.clone(),Command::CompleteClass{class_id:"a".into()},vec![],vec![Rejection::UnassessedStudents],false);
        check("complete passed",passed.clone(),Command::CompleteClass{class_id:"a".into()},vec![Event::ClassCompleted{class_id:"a".into(),results:vec![ClassResult{student_id:"s".into(),progress:Progress::Passed{score:60,credits:1}}]}],vec![],false);
        check("complete failed",failed.clone(),Command::CompleteClass{class_id:"a".into()},vec![Event::ClassCompleted{class_id:"a".into(),results:vec![ClassResult{student_id:"s".into(),progress:Progress::Failed{score:59,reason:FailureReason::BelowPassMark}}]}],vec![],false);
        check("complete empty class",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()}],Command::CompleteClass{class_id:"a".into()},vec![Event::ClassCompleted{class_id:"a".into(),results:vec![]}],vec![],false);
        check("unknown class Drop",registered.clone(),Command::Drop{class_id:"missing".into(),student_id:"s".into()},vec![],vec![Rejection::UnknownClass],false);
        check("unknown class StartClass",registered.clone(),Command::StartClass{class_id:"missing".into(),started_on:"2026-04-01".into()},vec![],vec![Rejection::UnknownClass],false);
        check("unknown class Attend",registered.clone(),Command::Attend{class_id:"missing".into(),student_id:"s".into()},vec![],vec![Rejection::UnknownClass],false);
        check("unknown class Assess",registered.clone(),Command::Assess{class_id:"missing".into(),student_id:"s".into(),score:60},vec![],vec![Rejection::UnknownClass],false);
        check("unknown class CompleteClass",registered.clone(),Command::CompleteClass{class_id:"missing".into()},vec![],vec![Rejection::UnknownClass],false);
        check("invalid id RegisterStudent",empty.clone(),Command::RegisterStudent{student_id:" ".into(),grade:Grade::Year2},vec![],vec![Rejection::InvalidId],true);
        check("invalid id PromoteStudent",empty.clone(),Command::PromoteStudent{student_id:"".into(),grade:Grade::Year2},vec![],vec![Rejection::InvalidId],true);
        check("invalid id OpenClass",empty.clone(),Command::OpenClass{class_id:"".into(),course:Course::Foundation,capacity:1},vec![],vec![Rejection::InvalidId],true);
        check("invalid id Enroll",empty.clone(),Command::Enroll{class_id:"a".into(),student_id:"".into()},vec![],vec![Rejection::InvalidId],true);
        check("invalid id Drop",empty.clone(),Command::Drop{class_id:"".into(),student_id:"s".into()},vec![],vec![Rejection::InvalidId],true);
        check("invalid id StartClass",empty.clone(),Command::StartClass{class_id:"".into(),started_on:"2026-04-01".into()},vec![],vec![Rejection::InvalidId],true);
        check("invalid id Attend",empty.clone(),Command::Attend{class_id:"a".into(),student_id:"\t".into()},vec![],vec![Rejection::InvalidId],true);
        check("invalid id Assess",empty.clone(),Command::Assess{class_id:"".into(),student_id:"s".into(),score:60},vec![],vec![Rejection::InvalidId],true);
        check("invalid id CompleteClass",empty.clone(),Command::CompleteClass{class_id:"".into()},vec![],vec![Rejection::InvalidId],true);
        check("invalid capacity 0",empty.clone(),Command::OpenClass{class_id:"a".into(),course:Course::Foundation,capacity:0},vec![],vec![Rejection::InvalidCapacity],true);
        check("invalid capacity 11",empty.clone(),Command::OpenClass{class_id:"a".into(),course:Course::Foundation,capacity:11},vec![],vec![Rejection::InvalidCapacity],true);
        check("invalid score -1",empty.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:-1},vec![],vec![Rejection::InvalidScore],true);
        check("invalid score 101",empty.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:101},vec![],vec![Rejection::InvalidScore],true);
        check("invalid date ",empty.clone(),Command::StartClass{class_id:"a".into(),started_on:"".into()},vec![],vec![Rejection::InvalidDate],true);
        check("invalid date 2026-02-29",empty.clone(),Command::StartClass{class_id:"a".into(),started_on:"2026-02-29".into()},vec![],vec![Rejection::InvalidDate],true);
        check("invalid date 2026-04-31",empty.clone(),Command::StartClass{class_id:"a".into(),started_on:"2026-04-31".into()},vec![],vec![Rejection::InvalidDate],true);
        check("invalid date 2026-1-01",empty.clone(),Command::StartClass{class_id:"a".into(),started_on:"2026-1-01".into()},vec![],vec![Rejection::InvalidDate],true);
        check("invalid date 0000-01-01",empty.clone(),Command::StartClass{class_id:"a".into(),started_on:"0000-01-01".into()},vec![],vec![Rejection::InvalidDate],true);
        check("invalid date 2026-13-01",empty.clone(),Command::StartClass{class_id:"a".into(),started_on:"2026-13-01".into()},vec![],vec![Rejection::InvalidDate],true);
        check("historical capacity remains replayable",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:25}],Command::Enroll{class_id:"a".into(),student_id:"s".into()},vec![Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation}],vec![],false);
        check("historical date remains replayable",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"Spring 2020".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()}],Command::Attend{class_id:"a".into(),student_id:"s".into()},vec![Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()}],vec![],false);
        check("historical score remains replayable",vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1},Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation},Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()},Event::StudyStarted{class_id:"a".into(),student_id:"s".into()},Event::AttendanceRecorded{class_id:"a".into(),student_id:"s".into()},Event::ResultRecorded{class_id:"a".into(),student_id:"s".into(),score:120},Event::ClassOpened{class_id:"b".into(),course:Course::Advanced,capacity:1}],Command::Enroll{class_id:"b".into(),student_id:"s".into()},vec![Event::StudentEnrolled{class_id:"b".into(),student_id:"s".into(),course:Course::Advanced}],vec![],false);
        check("new command cannot use historical capacity",empty.clone(),Command::OpenClass{class_id:"a".into(),course:Course::Foundation,capacity:25},vec![],vec![Rejection::InvalidCapacity],true);
        check("new command cannot use historical date",empty.clone(),Command::StartClass{class_id:"a".into(),started_on:"Spring 2020".into()},vec![],vec![Rejection::InvalidDate],true);
        check("new command cannot use historical score",empty.clone(),Command::Assess{class_id:"a".into(),student_id:"s".into(),score:120},vec![],vec![Rejection::InvalidScore],true);
    let(store,projection)=given_history(vec![Event::StudentRegistered{student_id:"s".into(),grade:Grade::Year3},Event::StudentRegistered{student_id:"t".into(),grade:Grade::Year3},Event::ClassOpened{class_id:"a".into(),course:Course::Foundation,capacity:1}]);
    let executor=DcbExecutor{store:&store};
    let first=executor.prepare("first",Command::Enroll{class_id:"a".into(),student_id:"s".into()},&SchoolHandler);
    let second=executor.prepare("second",Command::Enroll{class_id:"a".into(),student_id:"t".into()},&SchoolHandler);
    assert_eq!(first.commit().status,"Committed");let conflict=second.commit();
    assert_eq!(conflict.conflicts,1);assert_eq!(conflict.errors,vec![Rejection::ClassFull]);assert_eq!(store.read(&Query{items:vec![]}).events.len(),4);check_projection(&store,&projection);
    let mut state=Registers::default();for e in &opened{projection::reduce(&mut state,e);}let pure=projection::decision_model(&state);
    let valid=|command|crate::value_objects::ValidatedCommand::create(command).unwrap_or_else(|e|panic!("{:?}",e));
    assert_eq!(crate::decider::decide(&pure,&valid(Command::Enroll{class_id:"a".into(),student_id:"s".into()})),Decision::Accepted{events:vec![Event::StudentEnrolled{class_id:"a".into(),student_id:"s".into(),course:Course::Foundation}]});
    assert_eq!(crate::decider::decide(&pure,&valid(Command::Enroll{class_id:"a".into(),student_id:"missing".into()})),Decision::Rejected{code:Rejection::UnknownStudent});
    assert_eq!(crate::decider::decide(&pure,&valid(Command::StartClass{class_id:"a".into(),started_on:"2026-04-01".into()})),Decision::Accepted{events:vec![Event::ClassStarted{class_id:"a".into(),started_on:"2026-04-01".into()}]});
    use crate::value_objects::{StudentId,ClassId,SeatCapacity,AssessmentScore,SchoolDate};
    assert!(StudentId::try_from("s".to_string()).is_ok());assert!(ClassId::try_from(" ".to_string()).is_err());
    assert_eq!(SeatCapacity::try_from(25),Err(Rejection::InvalidCapacity));assert_eq!(AssessmentScore::try_from(120),Err(Rejection::InvalidScore));assert_eq!(SchoolDate::try_from("Spring 2020".to_string()),Err(Rejection::InvalidDate));
    assert_eq!(*SeatCapacity::from_event(25).value(),25);assert_eq!(*AssessmentScore::from_event(120).value(),120);assert_eq!(SchoolDate::from_event("Spring 2020".into()).value(),"Spring 2020");
    println!("domain: 76 GWT cases + conflict PASS");
}
