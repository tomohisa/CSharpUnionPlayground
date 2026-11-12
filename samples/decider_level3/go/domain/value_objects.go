package domain

import (
	"fmt"
	"strings"
	"time"
)

type InputViolation struct{ Reason Rejection }

func (e InputViolation) Error() string { return string(e.Reason) }

type StudentId struct {
	value       string
	initialized bool
}

func NewStudentId(value string) (StudentId, error) {
	if strings.TrimSpace(value) == "" {
		return StudentId{}, InputViolation{InvalidId}
	}
	return StudentId{value, true}, nil
}
func (v StudentId) Value() string {
	if !v.initialized {
		panic("uninitialized StudentId")
	}
	return v.value
}

type ClassId struct {
	value       string
	initialized bool
}

func NewClassId(value string) (ClassId, error) {
	if strings.TrimSpace(value) == "" {
		return ClassId{}, InputViolation{InvalidId}
	}
	return ClassId{value, true}, nil
}
func (v ClassId) Value() string {
	if !v.initialized {
		panic("uninitialized ClassId")
	}
	return v.value
}

type SeatCapacity struct {
	value       int
	initialized bool
}

func NewSeatCapacity(value int) (SeatCapacity, error) {
	if value < 1 || value > 10 {
		return SeatCapacity{}, InputViolation{InvalidCapacity}
	}
	return SeatCapacity{value, true}, nil
}
func (v SeatCapacity) Value() int {
	if !v.initialized {
		panic("uninitialized SeatCapacity")
	}
	return v.value
}

type AssessmentScore struct{ value int }

func NewAssessmentScore(value int) (AssessmentScore, error) {
	if value < 0 || value > 100 {
		return AssessmentScore{}, InputViolation{InvalidScore}
	}
	return AssessmentScore{value}, nil
}
func (v AssessmentScore) Value() int { return v.value }

type SchoolDate struct{ value string }

func NewSchoolDate(value string) (SchoolDate, error) {
	date, err := time.Parse("2006-01-02", value)
	if err != nil || len(value) != 10 || date.Year() < 1 {
		return SchoolDate{}, InputViolation{InvalidDate}
	}
	return SchoolDate{value}, nil
}
func (v SchoolDate) Value() string              { return v.value }
func RestoreSchoolDate(value string) SchoolDate { return SchoolDate{value} }

type ValidatedCommand struct{ value Command }

func (v ValidatedCommand) Value() Command {
	if v.value == nil {
		panic("uninitialized ValidatedCommand")
	}
	return v.value
}
func NewValidatedCommand(command Command) (ValidatedCommand, []Rejection) {
	var errors []error
	student := func(s string) { _, e := NewStudentId(s); errors = append(errors, e) }
	class := func(s string) { _, e := NewClassId(s); errors = append(errors, e) }
	switch c := command.(type) {
	case RegisterStudent:
		student(c.StudentId)
		if c.Grade != Year1 && c.Grade != Year2 && c.Grade != Year3 {
			return ValidatedCommand{}, []Rejection{InvalidCommand}
		}
	case PromoteStudent:
		student(c.StudentId)
		if c.Grade != Year1 && c.Grade != Year2 && c.Grade != Year3 {
			return ValidatedCommand{}, []Rejection{InvalidCommand}
		}
	case OpenClass:
		class(c.ClassId)
		_, e := NewSeatCapacity(c.Capacity)
		errors = append(errors, e)
		if c.Course != Foundation && c.Course != Advanced {
			return ValidatedCommand{}, []Rejection{InvalidCommand}
		}
	case Enroll:
		class(c.ClassId)
		student(c.StudentId)
	case Drop:
		class(c.ClassId)
		student(c.StudentId)
	case StartClass:
		class(c.ClassId)
		_, e := NewSchoolDate(c.StartedOn)
		errors = append(errors, e)
	case Attend:
		class(c.ClassId)
		student(c.StudentId)
	case Assess:
		class(c.ClassId)
		student(c.StudentId)
		_, e := NewAssessmentScore(c.Score)
		errors = append(errors, e)
	case CompleteClass:
		class(c.ClassId)
	default:
		return ValidatedCommand{}, []Rejection{InvalidCommand}
	}
	for _, err := range errors {
		if err != nil {
			violation, ok := err.(InputViolation)
			if !ok {
				panic(fmt.Sprintf("unexpected validation error: %v", err))
			}
			return ValidatedCommand{}, []Rejection{violation.Reason}
		}
	}
	return ValidatedCommand{command}, []Rejection{}
}

func RestoreStudentId(value string) StudentId { return StudentId{value, true} }

func RestoreClassId(value string) ClassId { return ClassId{value, true} }

func RestoreSeatCapacity(value int) SeatCapacity { return SeatCapacity{value, true} }

func RestoreAssessmentScore(value int) AssessmentScore { return AssessmentScore{value} }
