// Package dcb is domain-independent. Payload copies and reducers are supplied by callers.
package dcb

import (
	"slices"
	"sync"
)

type Tag struct {
	Category string   `json:"category"`
	Keys     []string `json:"keys"`
}

func (a Tag) Same(b Tag) bool { return a.Category == b.Category && slices.Equal(a.Keys, b.Keys) }

type QueryItem struct {
	Tags  []Tag    `json:"tags"`
	Types []string `json:"types"`
}
type Query struct {
	Items []QueryItem `json:"items"`
}

func (q Query) Matches(kind string, tags []Tag) bool {
	if len(q.Items) == 0 {
		return true
	}
	for _, item := range q.Items {
		if len(item.Types) > 0 && !slices.Contains(item.Types, kind) {
			continue
		}
		match := true
		for _, want := range item.Tags {
			found := false
			for _, tag := range tags {
				if want.Same(tag) {
					found = true
				}
			}
			if !found {
				match = false
				break
			}
		}
		if match {
			return true
		}
	}
	return false
}

type EventDescriptor struct {
	EventType string
	Tags      []Tag
}
type Recorded[E any] struct {
	Position  int    `json:"position"`
	CommandId string `json:"commandId"`
	EventType string `json:"eventType"`
	Tags      []Tag  `json:"tags"`
	Payload   E      `json:"payload"`
}
type ReadResult[E any] struct {
	After  int
	Events []Recorded[E]
}
type AppendResult[E any] struct {
	Committed bool
	Events    []Recorded[E]
}

type CommandStore[E any] interface {
	Read(Query) ReadResult[E]
	Append(string, Query, int, []E) AppendResult[E]
}

// Listeners are synchronous, non-reentrant consumers; false means disable delivery.
type EventStore[E any] struct {
	gate        sync.Mutex
	log         []Recorded[E]
	listeners   []func([]Recorded[E]) bool
	copyPayload func(E) E
	describe    func(E) EventDescriptor
	faults      int
}

func NewStore[E any](describe func(E) EventDescriptor, copyPayload func(E) E) *EventStore[E] {
	return &EventStore[E]{log: []Recorded[E]{}, copyPayload: copyPayload, describe: describe}
}
func (s *EventStore[E]) copy(e Recorded[E]) Recorded[E] {
	e.Payload = s.copyPayload(e.Payload)
	tags := []Tag{}
	for _, t := range e.Tags {
		tags = append(tags, Tag{t.Category, append([]string{}, t.Keys...)})
	}
	e.Tags = tags
	return e
}
func (s *EventStore[E]) Read(q Query) ReadResult[E] {
	s.gate.Lock()
	defer s.gate.Unlock()
	result := ReadResult[E]{len(s.log), []Recorded[E]{}}
	for _, e := range s.log {
		if q.Matches(e.EventType, e.Tags) {
			result.Events = append(result.Events, s.copy(e))
		}
	}
	return result
}
func (s *EventStore[E]) Append(commandId string, q Query, after int, batch []E) AppendResult[E] {
	s.gate.Lock()
	defer s.gate.Unlock()
	if len(batch) == 0 || commandId == "" || after < 0 || after > len(s.log) {
		panic("invalid append")
	}
	for _, e := range s.log {
		if e.Position > after && q.Matches(e.EventType, e.Tags) {
			return AppendResult[E]{false, []Recorded[E]{}}
		}
	}
	committed := []Recorded[E]{}
	for i, payload := range batch {
		copied := s.copyPayload(payload)
		e := s.describe(copied)
		if e.EventType == "" {
			panic("event type required")
		}
		committed = append(committed, s.copy(Recorded[E]{len(s.log) + i + 1, commandId, e.EventType, e.Tags, copied}))
	}
	s.log = append(s.log, committed...)
	active := []func([]Recorded[E]) bool{}
	for _, listener := range s.listeners {
		copy := []Recorded[E]{}
		for _, e := range committed {
			copy = append(copy, s.copy(e))
		}
		if notify(listener, copy) {
			active = append(active, listener)
		} else {
			s.faults++
		}
	}
	s.listeners = active
	copy := []Recorded[E]{}
	for _, e := range committed {
		copy = append(copy, s.copy(e))
	}
	return AppendResult[E]{true, copy}
}
func notify[E any](listener func([]Recorded[E]) bool, events []Recorded[E]) (ok bool) {
	defer func() {
		if recover() != nil {
			ok = false
		}
	}()
	return listener(events)
}
func (s *EventStore[E]) valid(e Recorded[E]) bool {
	d := s.describe(e.Payload)
	if d.EventType != e.EventType || len(d.Tags) != len(e.Tags) {
		return false
	}
	for _, t := range d.Tags {
		found := false
		for _, u := range e.Tags {
			if t.Same(u) {
				found = true
			}
		}
		if !found {
			return false
		}
	}
	return true
}
func (s *EventStore[E]) Subscribe(listener func([]Recorded[E]) bool) {
	s.gate.Lock()
	defer s.gate.Unlock()
	copy := []Recorded[E]{}
	for _, e := range s.log {
		copy = append(copy, s.copy(e))
	}
	if !notify(listener, copy) {
		panic("subscription replay failed")
	}
	s.listeners = append(s.listeners, listener)
}
func (s *EventStore[E]) Load(events []Recorded[E]) {
	s.gate.Lock()
	defer s.gate.Unlock()
	if len(s.log) > 0 || len(s.listeners) > 0 {
		panic("load only into an empty store")
	}
	copy := []Recorded[E]{}
	for i, e := range events {
		if e.Position != i+1 || e.CommandId == "" || e.EventType == "" || !s.valid(e) {
			panic("invalid imported log")
		}
		copy = append(copy, s.copy(e))
	}
	s.log = copy
}
func (s *EventStore[E]) ListenerFaults() int { s.gate.Lock(); defer s.gate.Unlock(); return s.faults }

type LiveProjection[E, S any] struct {
	gate       sync.Mutex
	state      S
	checkpoint int
	reduce     func(S, E) S
	copyState  func(S) S
}

func NewProjection[E, S any](initial S, reduce func(S, E) S, copyState func(S) S) *LiveProjection[E, S] {
	return &LiveProjection[E, S]{state: initial, reduce: reduce, copyState: copyState}
}
func (p *LiveProjection[E, S]) Accept(batch []Recorded[E]) bool {
	p.gate.Lock()
	defer p.gate.Unlock()
	next, position := p.copyState(p.state), p.checkpoint
	for _, e := range batch {
		if e.Position <= position {
			continue
		}
		if e.Position != position+1 {
			return false
		}
		next = p.reduce(next, e.Payload)
		position = e.Position
	}
	p.state = next
	p.checkpoint = position
	return true
}
func (p *LiveProjection[E, S]) Snapshot() (int, S) {
	p.gate.Lock()
	defer p.gate.Unlock()
	return p.checkpoint, p.copyState(p.state)
}

// A sum represented by a validated pair of slices: exactly one side is nonempty.
type Preparation[E, R any] struct {
	Events []E
	Errors []R
}
type Ticket[C, E, R any] struct {
	CommandId string
	Command   C
	Query     Query
	After     int
	Returned  int
	Decision  Preparation[E, R]
}
type HandlingResult[R any] struct {
	Status    string
	Conflicts int
	Appended  int
	Position  int
	Errors    []R
}
type CommandProcessor[C, E, R any] struct {
	Store        CommandStore[E]
	QueryFor     func(C) Query
	Decide       func([]Recorded[E], C) Preparation[E, R]
	MaxConflicts int
}

func (p CommandProcessor[C, E, R]) Prepare(id string, command C) Ticket[C, E, R] {
	query := p.QueryFor(command)
	read := p.Store.Read(query)
	decision := p.Decide(read.Events, command)
	if (len(decision.Events) == 0) == (len(decision.Errors) == 0) {
		panic("invalid preparation")
	}
	return Ticket[C, E, R]{id, command, query, read.After, len(read.Events), decision}
}
func (p CommandProcessor[C, E, R]) Commit(ticket Ticket[C, E, R]) HandlingResult[R] {
	if (len(ticket.Decision.Events) == 0) == (len(ticket.Decision.Errors) == 0) {
		panic("invalid preparation")
	}
	conflicts := 0
	for {
		if len(ticket.Decision.Errors) > 0 {
			return HandlingResult[R]{"Rejected", conflicts, 0, ticket.After, ticket.Decision.Errors}
		}
		result := p.Store.Append(ticket.CommandId, ticket.Query, ticket.After, ticket.Decision.Events)
		if result.Committed {
			return HandlingResult[R]{"Committed", conflicts, len(result.Events), result.Events[len(result.Events)-1].Position, []R{}}
		}
		conflicts++
		if conflicts >= p.MaxConflicts {
			return HandlingResult[R]{"RetryExhausted", conflicts, 0, ticket.After, []R{}}
		}
		ticket = p.Prepare(ticket.CommandId, ticket.Command)
	}
}
