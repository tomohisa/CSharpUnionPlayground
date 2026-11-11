package main

import (
	"encoding/json"
	d "example.com/classroom-decider/domain"
	"fmt"
)

type tagged struct {
	Type      string `json:"$type"`
	Name      string `json:"name"`
	Capacity  int    `json:"capacity"`
	StudentID string `json:"studentId"`
	Roster    struct {
		Name     string   `json:"name"`
		Capacity int      `json:"capacity"`
		Students []string `json:"students"`
	} `json:"roster"`
}

func decodeCommand(raw json.RawMessage) (d.Command, error) {
	var v tagged
	if err := json.Unmarshal(raw, &v); err != nil {
		return nil, err
	}
	switch v.Type {
	case "Create":
		return d.Create{Name: v.Name, Capacity: v.Capacity}, nil
	case "Enroll":
		return d.Enroll{StudentID: v.StudentID}, nil
	case "Drop":
		return d.Drop{StudentID: v.StudentID}, nil
	default:
		return nil, fmt.Errorf("unknown_command")
	}
}
func decodeEvent(raw json.RawMessage) (d.Event, error) {
	var v tagged
	if err := json.Unmarshal(raw, &v); err != nil {
		return nil, err
	}
	switch v.Type {
	case "RoomCreated":
		return d.RoomCreated{Name: v.Name, Capacity: v.Capacity}, nil
	case "StudentEnrolled":
		return d.StudentEnrolled{StudentID: v.StudentID}, nil
	case "StudentDropped":
		return d.StudentDropped{StudentID: v.StudentID}, nil
	default:
		return nil, fmt.Errorf("unknown_event")
	}
}
func eventWire(event d.Event) map[string]any {
	switch v := event.(type) {
	case d.RoomCreated:
		return map[string]any{"$type": v.Tag(), "name": v.Name, "capacity": v.Capacity}
	case d.StudentEnrolled:
		return map[string]any{"$type": v.Tag(), "studentId": v.StudentID}
	case d.StudentDropped:
		return map[string]any{"$type": v.Tag(), "studentId": v.StudentID}
	default:
		panic("unknown_event")
	}
}
func stateWire(state d.State) map[string]any {
	result := map[string]any{"$type": d.Phase(state)}
	if _, initial := state.(d.NotCreated); !initial {
		name, capacity, students := d.Snapshot(state)
		result["roster"] = map[string]any{"name": name, "capacity": capacity, "students": students}
	}
	return result
}

// Reconstruct the phase through the same validated transition rules.
func decodeState(raw []byte) (d.State, error) {
	var v tagged
	if err := json.Unmarshal(raw, &v); err != nil {
		return nil, err
	}
	if v.Type == "NotCreated" {
		return d.NotCreated{}, nil
	}
	if v.Type != "Available" && v.Type != "Full" {
		return nil, fmt.Errorf("unknown_state")
	}
	state, err := d.Evolve(d.NotCreated{}, d.RoomCreated{Name: v.Roster.Name, Capacity: v.Roster.Capacity})
	if err != nil {
		return nil, err
	}
	for _, id := range v.Roster.Students {
		state, err = d.Evolve(state, d.StudentEnrolled{StudentID: id})
		if err != nil {
			return nil, err
		}
	}
	if d.Phase(state) != v.Type {
		return nil, fmt.Errorf("invalid_state")
	}
	return state, nil
}
