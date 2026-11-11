package domain

import "fmt"

func Empty() State { return State{[]Student{}, []ClassState{}, []Enrollment{}} }
func PlanOf(room ClassState) Plan {
	switch c := room.(type) {
	case Available:
		return c.Plan
	case Full:
		return c.Plan
	case Teaching:
		return c.Plan
	case Completed:
		return c.Plan
	default:
		panic("unknown class")
	}
}
func members(room ClassState) []string {
	switch c := room.(type) {
	case Available:
		return append([]string{}, c.Students...)
	case Full:
		return append([]string{}, c.Students...)
	case Teaching:
		return append([]string{}, c.Students...)
	case Completed:
		result := []string{}
		for _, r := range c.Results {
			result = append(result, r.StudentId)
		}
		return result
	default:
		panic("unknown class")
	}
}
func contains(values []string, id string) bool {
	for _, v := range values {
		if v == id {
			return true
		}
	}
	return false
}
func rank(g Grade) int {
	switch g {
	case Year1:
		return 1
	case Year2:
		return 2
	case Year3:
		return 3
	default:
		panic("unknown grade")
	}
}
func recruiting(room ClassState) bool {
	switch room.(type) {
	case Available, Full:
		return true
	default:
		return false
	}
}
func accept(e Event) Decision        { return Accepted{[]Event{e}} }
func reject(code Rejection) Decision { return Rejected{code} }
func Decide(state State, command Command) Decision {
	switch c := command.(type) {
	case RegisterStudent:
		if c.StudentId == "" {
			return reject(InvalidId)
		}
		for _, s := range state.Students {
			if s.StudentId == c.StudentId {
				return reject(DuplicateStudent)
			}
		}
		return accept(StudentRegistered{c.StudentId, c.Grade})
	case PromoteStudent:
		for _, s := range state.Students {
			if s.StudentId == c.StudentId {
				if rank(c.Grade) != rank(s.Grade)+1 {
					return reject(InvalidPromotion)
				}
				return accept(StudentPromoted{c.StudentId, c.Grade})
			}
		}
		return reject(UnknownStudent)
	case OpenClass:
		if c.ClassId == "" {
			return reject(InvalidId)
		}
		if c.Capacity < 1 || c.Capacity > 10 {
			return reject(InvalidCapacity)
		}
		for _, room := range state.Classes {
			if PlanOf(room).ClassId == c.ClassId {
				return reject(DuplicateClass)
			}
		}
		return accept(ClassOpened{c.ClassId, c.Course, c.Capacity})
	default:
		return decideClass(state, command)
	}
}
func classID(command Command) string {
	switch c := command.(type) {
	case Enroll:
		return c.ClassId
	case Drop:
		return c.ClassId
	case StartClass:
		return c.ClassId
	case Attend:
		return c.ClassId
	case Assess:
		return c.ClassId
	case CompleteClass:
		return c.ClassId
	default:
		panic("invariant")
	}
}
func decideClass(state State, command Command) Decision {
	id := classID(command)
	for _, room := range state.Classes {
		if PlanOf(room).ClassId == id {
			plan := PlanOf(room)
			switch c := command.(type) {
			case Enroll:
				for _, s := range state.Students {
					if s.StudentId == c.StudentId {
						if !recruiting(room) {
							return reject(RegistrationClosed)
						}
						if contains(members(room), c.StudentId) {
							return reject(AlreadyEnrolled)
						}
						if rank(s.Grade) < rank(plan.MinimumGrade) {
							return reject(GradeTooLow)
						}
						eligible := false
						switch p := plan.Prerequisite.(type) {
						case NoPrerequisite:
							eligible = true
						case PassedCourse:
							for _, e := range state.Enrollments {
								if e.StudentId == c.StudentId && e.Course == p.Course {
									if _, ok := e.Progress.(Passed); ok {
										eligible = true
									}
								}
							}
						default:
							panic("unknown prerequisite")
						}
						if !eligible {
							return reject(PrerequisiteMissing)
						}
						if _, full := room.(Full); full {
							return reject(ClassFull)
						}
						return accept(StudentEnrolled{id, c.StudentId})
					}
				}
				return reject(UnknownStudent)
			case Drop:
				if !recruiting(room) {
					return reject(RegistrationClosed)
				}
				if !contains(members(room), c.StudentId) {
					return reject(NotEnrolled)
				}
				return accept(StudentDropped{id, c.StudentId})
			case StartClass:
				if !recruiting(room) {
					return reject(RegistrationClosed)
				}
				if c.StartedOn == "" {
					return reject(InvalidDate)
				}
				return accept(ClassStarted{id, c.StartedOn})
			case Attend:
				return study(state, room, id, c.StudentId, c)
			case Assess:
				return study(state, room, id, c.StudentId, c)
			case CompleteClass:
				if _, ok := room.(Teaching); !ok {
					return reject(NotTeaching)
				}
				for _, e := range state.Enrollments {
					if e.ClassId == id {
						if _, ok := e.Progress.(Studying); ok {
							return reject(UnassessedStudents)
						}
					}
				}
				return accept(ClassCompleted{id})
			default:
				panic("unknown command")
			}
		}
	}
	return reject(UnknownClass)
}
func study(state State, room ClassState, id, student string, command Command) Decision {
	if _, ok := room.(Teaching); !ok {
		return reject(NotTeaching)
	}
	if !contains(members(room), student) {
		return reject(NotEnrolled)
	}
	for _, e := range state.Enrollments {
		if e.ClassId == id && e.StudentId == student {
			p, ok := e.Progress.(Studying)
			if !ok {
				return reject(AlreadyAssessed)
			}
			switch c := command.(type) {
			case Attend:
				return accept(AttendanceRecorded{id, student})
			case Assess:
				if c.Score < 0 || c.Score > 100 {
					return reject(InvalidScore)
				}
				if p.Attendance == 0 {
					return reject(NoAttendance)
				}
				return accept(ResultRecorded{id, student, c.Score})
			default:
				panic("invariant")
			}
		}
	}
	panic("invariant")
}
func Cause(event Event) Command {
	switch e := event.(type) {
	case StudentRegistered:
		return RegisterStudent(e)
	case StudentPromoted:
		return PromoteStudent(e)
	case ClassOpened:
		return OpenClass(e)
	case StudentEnrolled:
		return Enroll(e)
	case StudentDropped:
		return Drop(e)
	case ClassStarted:
		return StartClass(e)
	case AttendanceRecorded:
		return Attend(e)
	case ResultRecorded:
		return Assess(e)
	case ClassCompleted:
		return CompleteClass(e)
	default:
		panic("unknown event")
	}
}
func Evolve(state State, event Event) (State, error) {
	if _, rejected := Decide(state, Cause(event)).(Rejected); rejected {
		return state, fmt.Errorf("InvalidHistory")
	}
	// Own every slice before updating: old snapshots must remain unchanged.
	s := State{append([]Student{}, state.Students...), append([]ClassState{}, state.Classes...), append([]Enrollment{}, state.Enrollments...)}
	switch e := event.(type) {
	case StudentRegistered:
		s.Students = append(s.Students, Student{e.StudentId, e.Grade})
		return s, nil
	case StudentPromoted:
		for i := range s.Students {
			if s.Students[i].StudentId == e.StudentId {
				s.Students[i].Grade = e.Grade
			}
		}
		return s, nil
	case ClassOpened:
		var prerequisite Prerequisite = NoPrerequisite{}
		grade := Year2
		switch e.Course {
		case Foundation:
		case Advanced:
			prerequisite = PassedCourse{Foundation}
			grade = Year3
		default:
			panic("course")
		}
		plan := Plan{e.ClassId, e.Course, grade, prerequisite, e.Capacity}
		s.Classes = append(s.Classes, Available{plan, []string{}})
		return s, nil
	}
	id := classID(Cause(event))
	for index, room := range s.Classes {
		if PlanOf(room).ClassId == id {
			plan := PlanOf(room)
			switch e := event.(type) {
			case StudentEnrolled:
				students := append(members(room), e.StudentId)
				if len(students) == plan.Capacity {
					s.Classes[index] = Full{plan, students}
				} else {
					s.Classes[index] = Available{plan, students}
				}
				records := []Enrollment{}
				for _, r := range s.Enrollments {
					if r.ClassId != id || r.StudentId != e.StudentId {
						records = append(records, r)
					}
				}
				s.Enrollments = append(records, Enrollment{id, e.StudentId, plan.Course, Reserved{}})
			case StudentDropped:
				students := []string{}
				for _, student := range members(room) {
					if student != e.StudentId {
						students = append(students, student)
					}
				}
				s.Classes[index] = Available{plan, students}
				for i, r := range s.Enrollments {
					if r.ClassId == id && r.StudentId == e.StudentId {
						s.Enrollments[i].Progress = Withdrawn{StudentRequest}
					}
				}
			case ClassStarted:
				s.Classes[index] = Teaching{plan, members(room), e.StartedOn}
				for i, r := range s.Enrollments {
					if r.ClassId == id {
						if _, ok := r.Progress.(Reserved); ok {
							s.Enrollments[i].Progress = Studying{0}
						}
					}
				}
			case AttendanceRecorded:
				for i, r := range s.Enrollments {
					if r.ClassId == id && r.StudentId == e.StudentId {
						p := r.Progress.(Studying)
						s.Enrollments[i].Progress = Studying{p.Attendance + 1}
					}
				}
			case ResultRecorded:
				var result Progress = Failed{e.Score, BelowPassMark}
				if e.Score >= 60 {
					credits := 1
					if plan.Course == Advanced {
						credits = 2
					}
					result = Passed{e.Score, credits}
				}
				for i, r := range s.Enrollments {
					if r.ClassId == id && r.StudentId == e.StudentId {
						s.Enrollments[i].Progress = result
					}
				}
			case ClassCompleted:
				results := []ClassResult{}
				for _, r := range s.Enrollments {
					if r.ClassId == id {
						switch r.Progress.(type) {
						case Passed, Failed:
							results = append(results, ClassResult{r.StudentId, r.Progress})
						}
					}
				}
				s.Classes[index] = Completed{plan, results}
			default:
				panic("unknown event")
			}
		}
	}
	return s, nil
}
func Replay(events []Event) (State, error) {
	state := Empty()
	for _, event := range events {
		next, err := Evolve(state, event)
		if err != nil {
			return state, err
		}
		state = next
	}
	return state, nil
}

func Describe(progress Progress) string {
	switch p := progress.(type) {
	case Reserved:
		return "reserved"
	case Studying:
		return fmt.Sprintf("studying:%d", p.Attendance)
	case Passed:
		return fmt.Sprintf("passed:%d:%d", p.Score, p.Credits)
	case Failed:
		return fmt.Sprintf("failed:%d:%s", p.Score, p.Reason)
	case Withdrawn:
		return fmt.Sprintf("withdrawn:%s", p.Reason)
	default:
		panic("unknown progress")
	}
}
