package domain

import "fmt"

func Empty() DecisionModel { return DecisionModel{[]Student{}, []ClassState{}, []Enrollment{}} }
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
func decideOne(state DecisionModel, command Command) Decision {
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
func decideClass(state DecisionModel, command Command) Decision {
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
						return accept(StudentEnrolled{id, c.StudentId, plan.Course})
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
				results := []ClassResult{}
				for _, e := range state.Enrollments {
					if e.ClassId == id {
						switch e.Progress.(type) {
						case Passed, Failed:
							results = append(results, ClassResult{e.StudentId, e.Progress})
						}
					}
				}
				return accept(ClassCompleted{id, results})
			default:
				panic("unknown command")
			}
		}
	}
	return reject(UnknownClass)
}
func study(state DecisionModel, room ClassState, id, student string, command Command) Decision {
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
func Decide(state DecisionModel, command Command) Decision {
	decision := decideOne(state, command)
	if a, ok := decision.(Accepted); ok {
		if e, ok := a.Events[0].(ClassStarted); ok {
			events := append([]Event{}, a.Events...)
			for _, r := range state.Enrollments {
				if r.ClassId == e.ClassId {
					if _, ok := r.Progress.(Reserved); ok {
						events = append(events, StudyStarted{e.ClassId, r.StudentId})
					}
				}
			}
			return Accepted{events}
		}
	}
	return decision
}
