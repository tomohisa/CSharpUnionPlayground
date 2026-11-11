package main

import (
	"encoding/json"
	d "example.com/classroom-decider/domain"
	"os"
)

type input struct {
	Commands []json.RawMessage `json:"commands"`
	History  []json.RawMessage `json:"history"`
}
type step struct {
	Phase   string   `json:"phase"`
	Emitted []string `json:"emitted"`
	Error   *string  `json:"error"`
}

func main() {
	var input input
	must(json.NewDecoder(os.Stdin).Decode(&input))
	history := []d.Event{}
	for _, raw := range input.History {
		event, err := decodeEvent(raw)
		must(err)
		history = append(history, event)
	}
	state, err := d.Replay(history)
	must(err)
	initial := state
	steps := []step{}
	for _, raw := range input.Commands {
		command, err := decodeCommand(raw)
		must(err)
		events, rejection := d.Decide(state, command)
		emitted := []string{}
		var code *string
		if rejection != nil {
			text := rejection.Error()
			code = &text
		} else {
			for _, event := range events {
				state, err = d.Evolve(state, event)
				must(err)
				history = append(history, event)
				emitted = append(emitted, event.Tag())
			}
		}
		steps = append(steps, step{d.Phase(state), emitted, code})
	}
	wireEvents := []map[string]any{}
	for _, event := range history {
		wireEvents = append(wireEvents, eventWire(event))
	}
	encoded, err := json.Marshal(wireEvents)
	must(err)
	var rawEvents []json.RawMessage
	must(json.Unmarshal(encoded, &rawEvents))
	stored := []d.Event{}
	for _, raw := range rawEvents {
		event, err := decodeEvent(raw)
		must(err)
		stored = append(stored, event)
	}
	snapshot, err := json.Marshal(stateWire(state))
	must(err)
	restored, err := decodeState(snapshot)
	must(err)
	replayed, err := d.Replay(stored)
	must(err)
	must(json.NewEncoder(os.Stdout).Encode(map[string]any{
		"initial": stateWire(initial), "state": stateWire(state), "events": wireEvents, "steps": steps,
		"restored": stateWire(restored), "replayed": stateWire(replayed),
	}))
}
func must(err error) {
	if err != nil {
		panic(err)
	}
}
