package domain

import "example.com/dcb-school/dcb"

type SchoolHandler struct{}

func (SchoolHandler) Validate(command Command) []Rejection {
	_, errors := NewValidatedCommand(command)
	return errors
}
func (SchoolHandler) QueryFor(command Command) dcb.Query { return QueryFor(command) }
func (SchoolHandler) Handle(events []dcb.Recorded[Event], command Command) dcb.Preparation[Event, Rejection] {
	validated, errors := NewValidatedCommand(command)
	if len(errors) > 0 {
		return dcb.Preparation[Event, Rejection]{Events: []Event{}, Errors: errors}
	}
	switch decision := Decide(Fold(events).DecisionModel(), validated).(type) {
	case Accepted:
		return dcb.Preparation[Event, Rejection]{Events: decision.Events, Errors: []Rejection{}}
	case Rejected:
		return dcb.Preparation[Event, Rejection]{Events: []Event{}, Errors: []Rejection{decision.Code}}
	default:
		panic("unknown decision")
	}
}
