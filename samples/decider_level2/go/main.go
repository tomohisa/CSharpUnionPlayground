package main

import (
	"encoding/json"
	d "example.com/enrollment-decider/domain"
	"fmt"
	"io"
	"os"
)

func run() error {
	data, err := io.ReadAll(os.Stdin)
	if err != nil {
		return err
	}
	input, err := decodeInput(data)
	if err != nil {
		return err
	}
	history := append([]d.Event{}, input.History...)
	state, err := d.Replay(history)
	if err != nil {
		return err
	}
	initial := state
	steps := []d.Decision{}
	for _, command := range input.Commands {
		decision := d.Decide(state, command)
		steps = append(steps, decision)
		if accepted, ok := decision.(d.Accepted); ok {
			for _, event := range accepted.Events {
				state, err = d.Evolve(state, event)
				if err != nil {
					return err
				}
				history = append(history, event)
			}
		}
	}
	saved, err := json.Marshal(history)
	if err != nil {
		return err
	}
	events, err := decodeEvents(saved)
	if err != nil {
		return err
	}
	snapshot, err := json.Marshal(state)
	if err != nil {
		return err
	}
	var restored d.State
	if err := json.Unmarshal(snapshot, &restored); err != nil {
		return err
	}
	replayed, err := d.Replay(events)
	if err != nil {
		return err
	}
	summaries := []string{}
	for _, e := range state.Enrollments {
		summaries = append(summaries, d.Describe(e.Progress))
	}
	return json.NewEncoder(os.Stdout).Encode(Output{summaries, initial, state, events, steps, restored, replayed})
}
func main() {
	if err := run(); err != nil {
		fmt.Fprintln(os.Stderr, err)
		os.Exit(1)
	}
}
