package main

import (
	"encoding/json"
	"example.com/dcb-school/dcb"
	d "example.com/dcb-school/domain"
	"fmt"
)

type Input struct {
	History    []dcb.Recorded[d.Event]
	Operations []d.Operation
}

func decodeInput(data []byte) (Input, error) {
	var raw struct {
		History    []json.RawMessage `json:"history"`
		Operations []json.RawMessage `json:"operations"`
	}
	if err := json.Unmarshal(data, &raw); err != nil {
		return Input{}, err
	}
	if raw.History == nil || raw.Operations == nil {
		return Input{}, fmt.Errorf("input arrays required")
	}
	input := Input{[]dcb.Recorded[d.Event]{}, []d.Operation{}}
	for _, data := range raw.History {
		var e struct {
			Position  int             `json:"position"`
			CommandId string          `json:"commandId"`
			EventType string          `json:"eventType"`
			Tags      []dcb.Tag       `json:"tags"`
			Payload   json.RawMessage `json:"payload"`
		}
		if err := json.Unmarshal(data, &e); err != nil {
			return Input{}, err
		}
		payload, err := d.DecodeEvent(e.Payload)
		if err != nil {
			return Input{}, err
		}
		input.History = append(input.History, dcb.Recorded[d.Event]{e.Position, e.CommandId, e.EventType, e.Tags, payload})
	}
	for _, data := range raw.Operations {
		op, err := d.DecodeOperation(data)
		if err != nil {
			return Input{}, err
		}
		input.Operations = append(input.Operations, op)
	}
	return input, nil
}
