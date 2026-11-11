package domain

import (
	"errors"
	"slices"
)

func Decide(s State, c Command) ([]Event, error) {
	switch v := c.(type) {
	case Create:
		if _, initial := s.(NotCreated); !initial {
			return reject("already_created")
		}
		if v.Name == "" {
			return reject("invalid_name")
		}
		if v.Capacity < 1 || v.Capacity > 10 {
			return reject("invalid_capacity")
		}
		return []Event{RoomCreated{v.Name, v.Capacity}}, nil
	case Enroll:
		return change(s, v.StudentID, true)
	case Drop:
		return change(s, v.StudentID, false)
	default:
		return reject("unknown_command") // Go has no exhaustive type switch.
	}
}
func reject(code string) ([]Event, error) { return nil, errors.New(code) }
func change(s State, student string, enrolling bool) ([]Event, error) {
	r, exists := data(s)
	if !exists {
		return reject("not_created")
	}
	if student == "" {
		return reject("invalid_student")
	}
	member := slices.Contains(r.students, student)
	if enrolling {
		if member {
			return reject("already_enrolled")
		}
		if _, full := s.(Full); full {
			return reject("full")
		}
		return []Event{StudentEnrolled{student}}, nil
	}
	if !member {
		return reject("not_enrolled")
	}
	return []Event{StudentDropped{student}}, nil
}
func classify(r roster) State {
	if len(r.students) == r.capacity {
		return Full{r}
	}
	return Available{r}
}
func Evolve(s State, event Event) (State, error) {
	invalid := func() (State, error) { return nil, errors.New("invalid_history") }
	switch v := event.(type) {
	case RoomCreated:
		if _, initial := s.(NotCreated); !initial {
			return invalid()
		}
		if v.Name == "" || v.Capacity < 1 || v.Capacity > 10 {
			return invalid()
		}
		return Available{roster{v.Name, v.Capacity, []string{}}}, nil
	case StudentEnrolled:
		a, available := s.(Available)
		if !available || v.StudentID == "" || slices.Contains(a.data.students, v.StudentID) {
			return invalid()
		}
		r := a.data
		r.students = append(append([]string{}, r.students...), v.StudentID)
		return classify(r), nil
	case StudentDropped:
		r, exists := data(s)
		if !exists || !slices.Contains(r.students, v.StudentID) {
			return invalid()
		}
		students := []string{}
		for _, id := range r.students {
			if id != v.StudentID {
				students = append(students, id)
			}
		}
		r.students = students
		return classify(r), nil
	default:
		return invalid()
	}
}
func Replay(events []Event) (State, error) {
	var state State = NotCreated{}
	var err error
	for _, event := range events {
		state, err = Evolve(state, event)
		if err != nil {
			return nil, err
		}
	}
	return state, nil
}
