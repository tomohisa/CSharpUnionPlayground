package main

import (
	"bytes"
	"encoding/json"
	d "example.com/dcb-school/domain"
)

type BenchmarkSummary struct {
	List           d.SchoolList   `json:"list"`
	RebuildMatches bool           `json:"rebuildMatches"`
	EventCount     int            `json:"eventCount"`
	Checkpoint     int            `json:"checkpoint"`
	ListenerFaults int            `json:"listenerFaults"`
	StepCounts     map[string]int `json:"stepCounts"`
	Errors         map[string]int `json:"errors"`
	Conflicts      int            `json:"conflicts"`
}

func benchmarkOutput(o RunOutput) ([]byte, error) {
	live, err := json.Marshal(o.List)
	if err != nil {
		return nil, err
	}
	rebuilt, err := json.Marshal(o.Rebuilt)
	if err != nil {
		return nil, err
	}
	s := BenchmarkSummary{List: o.List, RebuildMatches: bytes.Equal(live, rebuilt), EventCount: len(o.Events), Checkpoint: o.Checkpoint, ListenerFaults: o.ListenerFaults, StepCounts: map[string]int{}, Errors: map[string]int{}}
	for _, step := range o.Steps {
		s.StepCounts[step.Status]++
		for _, e := range step.Errors {
			s.Errors[string(e)]++
		}
		s.Conflicts += step.Conflicts
	}
	return json.Marshal(s)
}
