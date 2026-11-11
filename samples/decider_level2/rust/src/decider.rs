use crate::models::*;

pub fn empty() -> State { State { students: vec![], classes: vec![], enrollments: vec![] } }
pub fn plan_of(room: &ClassState) -> &Plan {
    match room { ClassState::Available {plan,..} | ClassState::Full {plan,..} |
        ClassState::Teaching {plan,..} | ClassState::Completed {plan,..} => plan }
}
fn members(room: &ClassState) -> Vec<String> {
    match room {
        ClassState::Available {students,..} | ClassState::Full {students,..} | ClassState::Teaching {students,..} => students.clone(),
        ClassState::Completed {results,..} => results.iter().map(|r| r.student_id.clone()).collect()
    }
}
fn rank(g: Grade) -> i32 { match g { Grade::Year1 => 1, Grade::Year2 => 2, Grade::Year3 => 3 } }
fn recruiting(c: &ClassState) -> bool { matches!(c, ClassState::Available {..} | ClassState::Full {..}) }
fn validate(s: &State, command: &Command) -> Result<Event, Rejection> {
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
            Ok(Event::StudentEnrolled {class_id: id.clone(), student_id: student_id.clone()})
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
            Ok(Event::ClassCompleted {class_id: id.clone()})
        }
        Command::RegisterStudent {..} | Command::PromoteStudent {..} | Command::OpenClass {..} => unreachable!()
    }
}
pub fn decide(s: &State, command: &Command) -> Decision {
    match validate(s, command) { Ok(e) => Decision::Accepted {events: vec![e]}, Err(code) => Decision::Rejected {code} }
}
pub fn cause(event: &Event) -> Command {
    match event {
        Event::StudentRegistered { student_id, grade } => Command::RegisterStudent { student_id: student_id.clone(), grade: grade.to_owned() },
        Event::StudentPromoted { student_id, grade } => Command::PromoteStudent { student_id: student_id.clone(), grade: grade.to_owned() },
        Event::ClassOpened { class_id, course, capacity } => Command::OpenClass { class_id: class_id.clone(), course: course.to_owned(), capacity: capacity.to_owned() },
        Event::StudentEnrolled { class_id, student_id } => Command::Enroll { class_id: class_id.clone(), student_id: student_id.clone() },
        Event::StudentDropped { class_id, student_id } => Command::Drop { class_id: class_id.clone(), student_id: student_id.clone() },
        Event::ClassStarted { class_id, started_on } => Command::StartClass { class_id: class_id.clone(), started_on: started_on.clone() },
        Event::AttendanceRecorded { class_id, student_id } => Command::Attend { class_id: class_id.clone(), student_id: student_id.clone() },
        Event::ResultRecorded { class_id, student_id, score } => Command::Assess { class_id: class_id.clone(), student_id: student_id.clone(), score: score.to_owned() },
        Event::ClassCompleted { class_id } => Command::CompleteClass { class_id: class_id.clone() },
    }
}
pub fn evolve(state: &State, event: &Event) -> Result<State, Rejection> {
    validate(state, &cause(event)).map_err(|_| Rejection::InvalidHistory)?;
    let mut s = state.clone();
    match event {
        Event::StudentRegistered {student_id,grade} => { s.students.push(Student {student_id: student_id.clone(), grade: *grade}); return Ok(s); }
        Event::StudentPromoted {student_id,grade} => { s.students.iter_mut().find(|x| x.student_id == *student_id).expect("invariant").grade = *grade; return Ok(s); }
        Event::ClassOpened {class_id,course,capacity} => {
            let (minimum_grade, prerequisite) = match course {
                Course::Foundation => (Grade::Year2, Prerequisite::NoPrerequisite),
                Course::Advanced => (Grade::Year3, Prerequisite::PassedCourse {course: Course::Foundation})
            };
            s.classes.push(ClassState::Available {plan: Plan {class_id: class_id.clone(), course: *course, capacity: *capacity, minimum_grade, prerequisite}, students: vec![]});
            return Ok(s);
        }
        _ => {}
    }
    let id = match event {
        Event::StudentEnrolled {class_id,..} | Event::StudentDropped {class_id,..} | Event::ClassStarted {class_id,..} |
        Event::AttendanceRecorded {class_id,..} | Event::ResultRecorded {class_id,..} | Event::ClassCompleted {class_id} => class_id,
        Event::StudentRegistered {..} | Event::StudentPromoted {..} | Event::ClassOpened {..} => unreachable!()
    };
    let index = s.classes.iter().position(|c| plan_of(c).class_id == *id).expect("invariant");
    let room = s.classes[index].clone();
    let plan = plan_of(&room).clone();
    match event {
        Event::StudentEnrolled {student_id,..} => {
            let mut students = members(&room); students.push(student_id.clone());
            s.classes[index] = if students.len() == plan.capacity as usize { ClassState::Full {plan: plan.clone(), students} } else { ClassState::Available {plan: plan.clone(), students} };
            s.enrollments.retain(|e| e.class_id != *id || e.student_id != *student_id);
            s.enrollments.push(Enrollment {class_id: id.clone(), student_id: student_id.clone(), course: plan.course, progress: Progress::Reserved});
        }
        Event::StudentDropped {student_id,..} => {
            s.classes[index] = ClassState::Available {plan, students: members(&room).into_iter().filter(|s| s != student_id).collect()};
            s.enrollments.iter_mut().find(|e| e.class_id == *id && e.student_id == *student_id).expect("invariant").progress = Progress::Withdrawn {reason: WithdrawalReason::StudentRequest};
        }
        Event::ClassStarted {started_on,..} => {
            s.classes[index] = ClassState::Teaching {plan, students: members(&room), started_on: started_on.clone()};
            for e in &mut s.enrollments { if e.class_id == *id && matches!(e.progress, Progress::Reserved) { e.progress = Progress::Studying {attendance: 0}; } }
        }
        Event::AttendanceRecorded {student_id,..} => {
            let e = s.enrollments.iter_mut().find(|e| e.class_id == *id && e.student_id == *student_id).expect("invariant");
            if let Progress::Studying {attendance} = &mut e.progress { *attendance += 1; }
        }
        Event::ResultRecorded {student_id,score,..} => {
            let e = s.enrollments.iter_mut().find(|e| e.class_id == *id && e.student_id == *student_id).expect("invariant");
            e.progress = if *score >= 60 { Progress::Passed {score: *score, credits: if plan.course == Course::Foundation {1} else {2}} }
                else { Progress::Failed {score: *score, reason: FailureReason::BelowPassMark} };
        }
        Event::ClassCompleted {..} => {
            s.classes[index] = ClassState::Completed {plan, results: s.enrollments.iter().filter(|e| e.class_id == *id && matches!(e.progress, Progress::Passed {..} | Progress::Failed {..}))
                .map(|e| ClassResult {student_id: e.student_id.clone(), progress: e.progress.clone()}).collect()};
        }
        Event::StudentRegistered {..} | Event::StudentPromoted {..} | Event::ClassOpened {..} => unreachable!()
    }
    Ok(s)
}
pub fn replay(events: &[Event]) -> Result<State, Rejection> { events.iter().try_fold(empty(), |s,e| evolve(&s,e)) }

pub fn describe(progress: &Progress) -> String {
    match progress {
        Progress::Reserved => "reserved".into(),
        Progress::Studying {attendance} => format!("studying:{attendance}"),
        Progress::Passed {score,credits} => format!("passed:{score}:{credits}"),
        Progress::Failed {score,reason} => format!("failed:{score}:{reason:?}"),
        Progress::Withdrawn {reason} => format!("withdrawn:{reason:?}")
    }
}
