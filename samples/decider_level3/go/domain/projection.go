package domain

import (
	"example.com/dcb-school/dcb"
	"sort"
)

type EnrollmentKey struct{ Room, Student string }
type Registers struct {
	Students    map[string]Student
	Classes     map[string]ClassState
	Enrollments map[EnrollmentKey]Enrollment
}

func EmptyRegisters() Registers {
	return Registers{map[string]Student{}, map[string]ClassState{}, map[EnrollmentKey]Enrollment{}}
}
func CopyRegisters(s Registers) Registers {
	n := EmptyRegisters()
	for k, v := range s.Students {
		n.Students[k] = v
	}
	for k, v := range s.Classes {
		switch c := v.(type) {
		case Available:
			c.Students = append([]string{}, c.Students...)
			n.Classes[k] = c
		case Full:
			c.Students = append([]string{}, c.Students...)
			n.Classes[k] = c
		case Teaching:
			c.Students = append([]string{}, c.Students...)
			n.Classes[k] = c
		case Completed:
			c.Results = append([]ClassResult{}, c.Results...)
			n.Classes[k] = c
		default:
			panic("unknown class")
		}
	}
	for k, v := range s.Enrollments {
		n.Enrollments[k] = v
	}
	return n
}
func ApplyStudent(s Registers, event Event) {
	switch e := event.(type) {
	case StudentRegistered:
		s.Students[e.StudentId] = Student{e.StudentId, e.Grade}
	case StudentPromoted:
		s.Students[e.StudentId] = Student{e.StudentId, e.Grade}
	}
}
func ApplyClass(s Registers, event Event) {
	if e, ok := event.(ClassOpened); ok {
		var prerequisite Prerequisite = NoPrerequisite{}
		grade := Year2
		if e.Course == Advanced {
			prerequisite = PassedCourse{Foundation}
			grade = Year3
		}
		s.Classes[e.ClassId] = Available{Plan{e.ClassId, e.Course, grade, prerequisite, e.Capacity}, []string{}}
		return
	}
	var id string
	switch e := event.(type) {
	case StudentEnrolled:
		id = e.ClassId
	case StudentDropped:
		id = e.ClassId
	case ClassStarted:
		id = e.ClassId
	case ClassCompleted:
		id = e.ClassId
	default:
		return
	}
	room, ok := s.Classes[id]
	if !ok {
		return
	}
	plan := PlanOf(room)
	switch e := event.(type) {
	case StudentEnrolled:
		students := append(members(room), e.StudentId)
		if len(students) == plan.Capacity {
			s.Classes[id] = Full{plan, students}
		} else {
			s.Classes[id] = Available{plan, students}
		}
	case StudentDropped:
		students := []string{}
		for _, v := range members(room) {
			if v != e.StudentId {
				students = append(students, v)
			}
		}
		s.Classes[id] = Available{plan, students}
	case ClassStarted:
		s.Classes[id] = Teaching{plan, members(room), e.StartedOn}
	case ClassCompleted:
		s.Classes[id] = Completed{plan, append([]ClassResult{}, e.Results...)}
	}
}
func ApplyEnrollment(s Registers, event Event) {
	if e, ok := event.(StudentEnrolled); ok {
		s.Enrollments[EnrollmentKey{e.ClassId, e.StudentId}] = Enrollment{e.ClassId, e.StudentId, e.Course, Reserved{}}
		return
	}
	var key EnrollmentKey
	switch e := event.(type) {
	case StudentDropped:
		key = EnrollmentKey{e.ClassId, e.StudentId}
	case StudyStarted:
		key = EnrollmentKey{e.ClassId, e.StudentId}
	case AttendanceRecorded:
		key = EnrollmentKey{e.ClassId, e.StudentId}
	case ResultRecorded:
		key = EnrollmentKey{e.ClassId, e.StudentId}
	default:
		return
	}
	r, ok := s.Enrollments[key]
	if !ok {
		return
	}
	switch e := event.(type) {
	case StudentDropped:
		r.Progress = Withdrawn{StudentRequest}
	case StudyStarted:
		r.Progress = Studying{0}
	case AttendanceRecorded:
		p := r.Progress.(Studying)
		r.Progress = Studying{p.Attendance + 1}
	case ResultRecorded:
		if e.Score >= 60 {
			credits := 1
			if r.Course == Advanced {
				credits = 2
			}
			r.Progress = Passed{e.Score, credits}
		} else {
			r.Progress = Failed{e.Score, BelowPassMark}
		}
	}
	s.Enrollments[key] = r
}
func Reduce(s Registers, e Event) Registers {
	ApplyStudent(s, e)
	ApplyClass(s, e)
	ApplyEnrollment(s, e)
	return s
}
func Fold(events []dcb.Recorded[Event]) Registers {
	s := EmptyRegisters()
	for _, e := range events {
		s = Reduce(s, e.Payload)
	}
	return s
}
func (s Registers) DecisionModel() DecisionModel {
	model := DecisionModel{[]Student{}, []ClassState{}, []Enrollment{}}
	for _, v := range s.Students {
		model.Students = append(model.Students, v)
	}
	for _, v := range s.Classes {
		model.Classes = append(model.Classes, v)
	}
	for _, v := range s.Enrollments {
		model.Enrollments = append(model.Enrollments, v)
	}
	sort.Slice(model.Students, func(i, j int) bool { return model.Students[i].StudentId < model.Students[j].StudentId })
	sort.Slice(model.Classes, func(i, j int) bool { return PlanOf(model.Classes[i]).ClassId < PlanOf(model.Classes[j]).ClassId })
	sort.Slice(model.Enrollments, func(i, j int) bool {
		a, b := model.Enrollments[i], model.Enrollments[j]
		if a.ClassId != b.ClassId {
			return a.ClassId < b.ClassId
		}
		return a.StudentId < b.StudentId
	})
	return model
}
func (s Registers) List() SchoolList {
	m := s.DecisionModel()
	summaries := []string{}
	for _, e := range m.Enrollments {
		summaries = append(summaries, Describe(e.Progress))
	}
	return SchoolList{m.Students, m.Classes, m.Enrollments, summaries}
}
