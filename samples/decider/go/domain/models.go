package domain

// The roster is private; access returns a copy of the slice.
type roster struct {
	name     string
	capacity int
	students []string
}
type State interface{ phase() string }
type NotCreated struct{}
type Available struct{ data roster }
type Full struct{ data roster }

func (NotCreated) phase() string { return "NotCreated" }
func (Available) phase() string  { return "Available" }
func (Full) phase() string       { return "Full" }
func Phase(s State) string       { return s.phase() }
func data(s State) (roster, bool) {
	switch v := s.(type) {
	case NotCreated:
		return roster{}, false
	case Available:
		return v.data, true
	case Full:
		return v.data, true
	default:
		panic("unknown_state")
	}
}
func Snapshot(s State) (string, int, []string) {
	r, _ := data(s)
	return r.name, r.capacity, append([]string{}, r.students...)
}

type Command interface{ command() }
type Create struct {
	Name     string
	Capacity int
}
type Enroll struct{ StudentID string }
type Drop struct{ StudentID string }

func (Create) command() {}
func (Enroll) command() {}
func (Drop) command()   {}

type Event interface{ Tag() string }
type RoomCreated struct {
	Name     string
	Capacity int
}
type StudentEnrolled struct{ StudentID string }
type StudentDropped struct{ StudentID string }

func (RoomCreated) Tag() string     { return "RoomCreated" }
func (StudentEnrolled) Tag() string { return "StudentEnrolled" }
func (StudentDropped) Tag() string  { return "StudentDropped" }
