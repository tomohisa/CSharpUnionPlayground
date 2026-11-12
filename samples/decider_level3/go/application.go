package main

import (
	"encoding/json"
	"example.com/dcb-school/dcb"
	d "example.com/dcb-school/domain"
	"fmt"
	"os"
	"time"
)

type ReadAudit struct {
	Query    dcb.Query `json:"query"`
	After    int       `json:"after"`
	Returned int       `json:"returned"`
}
type RunOutput struct {
	Events         []dcb.Recorded[d.Event] `json:"events"`
	Steps          []d.Trace               `json:"steps"`
	Lists          []d.SchoolList          `json:"lists"`
	List           d.SchoolList            `json:"list"`
	Rebuilt        d.SchoolList            `json:"rebuilt"`
	Checkpoint     int                     `json:"checkpoint"`
	ListenerFaults int                     `json:"listenerFaults"`
	Reads          []ReadAudit             `json:"reads"`
}

func runSchool(history []dcb.Recorded[d.Event], operations []d.Operation) (RunOutput, error) {
	var journal dcb.JsonJournal = dcb.NewMemoryJsonJournal()
	if path := os.Getenv("DECIDER_PERSISTENT"); path != "" {
		journal = dcb.NewSqliteJournal(path, "FULL")
	}
	store := dcb.NewJsonEventStore(journal, d.DescribeEvent, func(e d.Event) string {
		b, err := json.Marshal(e)
		if err != nil {
			panic(err)
		}
		return string(b)
	}, func(s string) d.Event {
		e, err := d.DecodeEvent([]byte(s))
		if err != nil {
			panic(err)
		}
		return e
	})
	defer store.Close()
	if len(history) > 0 {
		store.Load(history)
	}
	if os.Getenv("DECIDER_BENCHMARK") == "1" {
		fmt.Fprintln(os.Stderr, "STORE "+store.EngineVersion())
	}
	projection := d.NewSchoolProjection()
	store.Subscribe(projection.Accept)
	var executor dcb.Executor[d.Command, d.Event, d.Rejection] = dcb.DcbExecutor[d.Command, d.Event, d.Rejection]{Store: store}
	handler := d.SchoolHandler{}
	tickets := map[string]dcb.PreparedCommand[d.Rejection]{}
	steps := []d.Trace{}
	lists := []d.SchoolList{}
	reads := []ReadAudit{}
	prepare := func(id string, c d.Command) dcb.PreparedCommand[d.Rejection] {
		t := executor.Prepare(id, c, handler)
		if t.WasRead {
			reads = append(reads, ReadAudit{t.Query, t.After, t.Returned})
		}
		return t
	}
	commit := func(op string, t dcb.PreparedCommand[d.Rejection]) {
		r := t.Commit()
		steps = append(steps, d.Trace{op, r.Status, r.Conflicts, r.Appended, r.Position, r.Errors})
	}
	bench := os.Getenv("DECIDER_BENCHMARK") == "1"
	start := time.Now()
	if bench {
		fmt.Fprintln(os.Stderr, "READY")
	}
	for _, operation := range operations {
		switch x := operation.(type) {
		case d.Execute:
			commit("Execute", prepare(x.CommandId, x.Command))
		case d.Prepare:
			if _, ok := tickets[x.Key]; ok {
				return RunOutput{}, fmt.Errorf("duplicate ticket")
			}
			t := prepare(x.CommandId, x.Command)
			tickets[x.Key] = t
			steps = append(steps, d.Trace{"Prepare", "Prepared", 0, 0, t.After, []d.Rejection{}})
		case d.Commit:
			t, ok := tickets[x.Key]
			if !ok {
				return RunOutput{}, fmt.Errorf("unknown ticket")
			}
			delete(tickets, x.Key)
			commit("Commit", t)
		case d.ListAll:
			store.Refresh()
			if store.ListenerFaults() != 0 {
				return RunOutput{}, fmt.Errorf("projection unavailable; rebuild required")
			}
			position, s := projection.Snapshot()
			lists = append(lists, s)
			steps = append(steps, d.Trace{"ListAll", "Listed", 0, 0, position, []d.Rejection{}})
		case d.Rebuild:
			fresh := d.NewSchoolProjection()
			if !fresh.Accept(store.Read(dcb.Query{Items: []dcb.QueryItem{}}).Events) {
				return RunOutput{}, fmt.Errorf("rebuild failed")
			}
			position, s := fresh.Snapshot()
			lists = append(lists, s)
			steps = append(steps, d.Trace{"Rebuild", "Rebuilt", 0, 0, position, []d.Rejection{}})
		case d.Redeliver:
			if !projection.Accept(store.Read(dcb.Query{Items: []dcb.QueryItem{}}).Events) {
				return RunOutput{}, fmt.Errorf("duplicate delivery failed")
			}
			position := projection.Checkpoint()
			steps = append(steps, d.Trace{"Redeliver", "Redelivered", 0, 0, position, []d.Rejection{}})
		default:
			return RunOutput{}, fmt.Errorf("unknown operation")
		}
	}
	if bench {
		fmt.Fprintln(os.Stderr, "PHASE operations", time.Since(start).Seconds())
	}
	start = time.Now()
	store.Refresh()
	queryBenchmark(store, projection)
	start = time.Now()
	events := store.Read(dcb.Query{Items: []dcb.QueryItem{}}).Events
	fresh := d.NewSchoolProjection()
	if !fresh.Accept(events) {
		return RunOutput{}, fmt.Errorf("rebuild failed")
	}
	position, s := projection.Snapshot()
	_, rebuilt := fresh.Snapshot()
	if store.ListenerFaults() != 0 {
		return RunOutput{}, fmt.Errorf("projection unavailable; rebuild required")
	}
	if bench {
		fmt.Fprintln(os.Stderr, "PHASE read_rebuild_snapshot", time.Since(start).Seconds())
		fmt.Fprintln(os.Stderr, "CODEC", store.Encoded, store.Decoded, store.EncodedBytes, store.DecodedBytes)
	}
	storageBenchmark()
	return RunOutput{events, steps, lists, s, rebuilt, position, store.ListenerFaults(), reads}, nil
}
