use crate::models::*;

pub fn empty() -> DecisionModel { DecisionModel { students: vec![], classes: vec![], enrollments: vec![] } }
pub fn plan_of(room: &ClassState) -> &Plan {
    match room { ClassState::Available {plan,..} | ClassState::Full {plan,..} |
        ClassState::Teaching {plan,..} | ClassState::Completed {plan,..} => plan }
}
pub fn members(room: &ClassState) -> Vec<String> {
    match room {
        ClassState::Available {students,..} | ClassState::Full {students,..} | ClassState::Teaching {students,..} => students.clone(),
        ClassState::Completed {results,..} => results.iter().map(|r| r.student_id.clone()).collect()
    }
}
fn rank(g: Grade) -> i32 { match g { Grade::Year1 => 1, Grade::Year2 => 2, Grade::Year3 => 3 } }
fn recruiting(c: &ClassState) -> bool { matches!(c, ClassState::Available {..} | ClassState::Full {..}) }
fn validate(s: &DecisionModel, command: &Command) -> Result<Event, Rejection> {
    use Rejection::*;
    match command {
        Command::RegisterStudent {student_id,grade} => {
            if student_id.is_empty() { return Err(InvalidId); }
            if s.students.iter().any(|x| x.student_id == *student_id) { return Err(DuplicateStudent); }
            return Ok(Event::StudentRegistered {student_id: student_id.clone(), grade: *grade});
        }
        Command::PromoteStudent {student_id,grade} => {
            let student = s.students.iter().find(|x| x.student_id == *student_id).ok_or(UnknownStudent)?;
            if rank(*grade) != rank(student.grade) + 1 { return Err(InvalidPromotion); }
            return Ok(Event::StudentPromoted {student_id: student_id.clone(), grade: *grade});
        }
        Command::OpenClass {class_id,course,capacity} => {
            if class_id.is_empty() { return Err(InvalidId); }
            if !(1..=10).contains(capacity) { return Err(InvalidCapacity); }
            if s.classes.iter().any(|c| plan_of(c).class_id == *class_id) { return Err(DuplicateClass); }
            return Ok(Event::ClassOpened {class_id: class_id.clone(), course: *course, capacity: *capacity});
        }
        Command::Enroll {..} | Command::Drop {..} | Command::StartClass {..} | Command::Attend {..} | Command::Assess {..} | Command::CompleteClass {..} => {}
    }
    let id = match command {
        Command::Enroll {class_id,..} | Command::Drop {class_id,..} | Command::StartClass {class_id,..} |
        Command::Attend {class_id,..} | Command::Assess {class_id,..} | Command::CompleteClass {class_id} => class_id,
        Command::RegisterStudent {..} | Command::PromoteStudent {..} | Command::OpenClass {..} => unreachable!()
    };
    let room = s.classes.iter().find(|c| plan_of(c).class_id == *id).ok_or(UnknownClass)?;
    let plan = plan_of(room);
    match command {
        Command::Enroll {student_id,..} => {
            let student = s.students.iter().find(|x| x.student_id == *student_id).ok_or(UnknownStudent)?;
            if !recruiting(room) { return Err(RegistrationClosed); }
            if members(room).contains(student_id) { return Err(AlreadyEnrolled); }
            if rank(student.grade) < rank(plan.minimum_grade) { return Err(GradeTooLow); }
            let eligible = match plan.prerequisite {
                Prerequisite::NoPrerequisite => true,
                Prerequisite::PassedCourse {course} => s.enrollments.iter().any(|e| e.student_id == *student_id && e.course == course && matches!(e.progress, Progress::Passed {..}))
            };
            if !eligible { return Err(PrerequisiteMissing); }
            if matches!(room, ClassState::Full {..}) { return Err(ClassFull); }
            Ok(Event::StudentEnrolled {class_id: id.clone(), student_id: student_id.clone(), course: plan.course})
        }
        Command::Drop {student_id,..} => {
            if !recruiting(room) { return Err(RegistrationClosed); }
            if !members(room).contains(student_id) { return Err(NotEnrolled); }
            Ok(Event::StudentDropped {class_id: id.clone(), student_id: student_id.clone()})
        }
        Command::StartClass {started_on,..} => {
            if !recruiting(room) { return Err(RegistrationClosed); }
            if started_on.is_empty() { return Err(InvalidDate); }
            Ok(Event::ClassStarted {class_id: id.clone(), started_on: started_on.clone()})
        }
        Command::Attend {student_id,..} | Command::Assess {student_id,..} => {
            if !matches!(room, ClassState::Teaching {..}) { return Err(NotTeaching); }
            if !members(room).contains(student_id) { return Err(NotEnrolled); }
            let record = s.enrollments.iter().find(|e| e.class_id == *id && e.student_id == *student_id).expect("invariant");
            let Progress::Studying {attendance} = record.progress else { return Err(AlreadyAssessed); };
            match command {
                Command::Attend {..} => Ok(Event::AttendanceRecorded {class_id: id.clone(), student_id: student_id.clone()}),
                Command::Assess {score,..} => {
                    if !(0..=100).contains(score) { return Err(InvalidScore); }
                    if attendance == 0 { return Err(NoAttendance); }
                    Ok(Event::ResultRecorded {class_id: id.clone(), student_id: student_id.clone(), score: *score})
                }
                _ => unreachable!()
            }
        }
        Command::CompleteClass {..} => {
            if !matches!(room, ClassState::Teaching {..}) { return Err(NotTeaching); }
            if s.enrollments.iter().any(|e| e.class_id == *id && matches!(e.progress, Progress::Studying {..})) { return Err(UnassessedStudents); }
            Ok(Event::ClassCompleted {class_id: id.clone(), results: s.enrollments.iter().filter(|e| e.class_id == *id && matches!(e.progress, Progress::Passed {..} | Progress::Failed {..})).map(|e| ClassResult {student_id: e.student_id.clone(), progress: e.progress.clone()}).collect()})
        }
        Command::RegisterStudent {..} | Command::PromoteStudent {..} | Command::OpenClass {..} => unreachable!()
    }
}
pub fn decide(s: &DecisionModel, command: &Command) -> Decision {
    match validate(s, command) { Ok(e) => {
        let mut events = vec![e.clone()];
        if let Event::ClassStarted {class_id,..} = e { events.extend(s.enrollments.iter().filter(|r| r.class_id == class_id && matches!(r.progress, Progress::Reserved)).map(|r| Event::StudyStarted {class_id: class_id.clone(), student_id: r.student_id.clone()})); }
        Decision::Accepted {events}
    }, Err(code) => Decision::Rejected {code} }
}
pub fn describe(progress: &Progress) -> String {
    match progress {
        Progress::Reserved => "reserved".into(),
        Progress::Studying {attendance} => format!("studying:{attendance}"),
        Progress::Passed {score,credits} => format!("passed:{score}:{credits}"),
        Progress::Failed {score,reason} => format!("failed:{score}:{reason:?}"),
        Progress::Withdrawn {reason} => format!("withdrawn:{reason:?}")
    }
}
