package main

import (
	"encoding/json"
	d "example.com/enrollment-decider/domain"
	"fmt"
)

type Input struct {
	Commands []d.Command
	History  []d.Event
}
type Output struct {
	Summaries []string     `json:"summaries"`
	Initial   d.State      `json:"initial"`
	State     d.State      `json:"state"`
	Events    []d.Event    `json:"events"`
	Steps     []d.Decision `json:"steps"`
	Restored  d.State      `json:"restored"`
	Replayed  d.State      `json:"replayed"`
}

func decodeEvents(data []byte) ([]d.Event, error) {
	var raw []json.RawMessage
	if err := json.Unmarshal(data, &raw); err != nil {
		return nil, err
	}
	events := []d.Event{}
	for _, value := range raw {
		event, err := d.DecodeEvent(value)
		if err != nil {
			return nil, err
		}
		events = append(events, event)
	}
	return events, nil
}
func decodeInput(data []byte) (Input, error) {
	var raw struct {
		Commands json.RawMessage `json:"commands"`
		History  json.RawMessage `json:"history"`
	}
	if err := json.Unmarshal(data, &raw); err != nil {
		return Input{}, err
	}
	if len(raw.Commands) == 0 || len(raw.History) == 0 || string(raw.Commands) == "null" || string(raw.History) == "null" {
		return Input{}, fmt.Errorf("input arrays required")
	}
	var commands []json.RawMessage
	if err := json.Unmarshal(raw.Commands, &commands); err != nil {
		return Input{}, err
	}
	input := Input{Commands: []d.Command{}}
	for _, value := range commands {
		command, err := d.DecodeCommand(value)
		if err != nil {
			return Input{}, err
		}
		input.Commands = append(input.Commands, command)
	}
	history, err := decodeEvents(raw.History)
	input.History = history
	return input, err
}
