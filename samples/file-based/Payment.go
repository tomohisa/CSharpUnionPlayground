// Run: go run samples/file-based/Payment.go
package main

import (
	"bytes"
	"encoding/json"
	"fmt"
)

// A marker interface groups cases; Go does not make it an exhaustive sum type.
type Payment interface{ isPayment() }
type Cash struct {
	Amount int64
	Next   Payment
}
type Card struct {
	CardNumber string
	Amount     int64
	Next       Payment
}

func (Cash) isPayment() {}
func (Card) isPayment() {}

func describe(payment Payment) string {
	switch p := payment.(type) {
	case Cash:
		return fmt.Sprintf("cash:%d", p.Amount)
	case Card:
		return "card:" + p.CardNumber
	default:
		panic("unknown or nil payment") // Runtime guard, not an exhaustiveness check.
	}
}

// Standard encoding/json needs explicit dispatch to restore an interface's type.
type wirePayment struct {
	Type       string          `json:"$type"`
	Amount     int64           `json:"amount"`
	CardNumber string          `json:"cardNumber,omitempty"`
	Next       json.RawMessage `json:"next"`
}

func marshal(payment Payment) ([]byte, error) {
	if payment == nil {
		return []byte("null"), nil
	}
	var wire wirePayment
	var next Payment
	switch p := payment.(type) {
	case Cash:
		wire.Type, wire.Amount, next = "cash", p.Amount, p.Next
	case Card:
		wire.Type, wire.Amount, wire.CardNumber, next = "card", p.Amount, p.CardNumber, p.Next
	default:
		return nil, fmt.Errorf("unknown payment: %T", payment)
	}
	data, err := marshal(next)
	if err != nil {
		return nil, err
	}
	wire.Next = data
	return json.Marshal(wire)
}
func unmarshal(data []byte) (Payment, error) {
	if bytes.Equal(bytes.TrimSpace(data), []byte("null")) {
		return nil, nil
	}
	var wire wirePayment
	if err := json.Unmarshal(data, &wire); err != nil {
		return nil, err
	}
	next, err := unmarshal(wire.Next)
	if err != nil {
		return nil, err
	}
	switch wire.Type {
	case "cash":
		return Cash{wire.Amount, next}, nil
	case "card":
		return Card{wire.CardNumber, wire.Amount, next}, nil
	default:
		return nil, fmt.Errorf("unknown payment tag: %q", wire.Type)
	}
}
func main() {
	var original Payment = Cash{100, Card{"4111", 250, nil}}
	data, err := marshal(original)
	if err != nil {
		panic(err)
	}
	restored, err := unmarshal(data)
	if err != nil {
		panic(err)
	}
	fmt.Println(string(data))
	fmt.Println(describe(restored))
}
