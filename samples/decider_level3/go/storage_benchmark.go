package main

import (
	"encoding/json"
	"example.com/dcb-school/dcb"
	"fmt"
	"os"
	"strconv"
	"strings"
	"time"
)

type DocumentRevision struct {
	DocumentId string `json:"documentId"`
	Revision   int    `json:"revision"`
	Body       string `json:"body"`
}

func storageBenchmark() {
	count, _ := strconv.Atoi(os.Getenv("DECIDER_DOCUMENTS"))
	if count <= 0 {
		return
	}
	path := os.Getenv("DECIDER_PERSISTENT")
	open := func() dcb.JsonJournal {
		if path != "" {
			return dcb.NewSqliteJournal(path+".documents", "FULL")
		}
		return dcb.NewMemoryJsonJournal()
	}
	store := func(j dcb.JsonJournal) *dcb.JsonEventStore[DocumentRevision] {
		return dcb.NewJsonEventStore(j, func(e DocumentRevision) dcb.EventDescriptor {
			return dcb.EventDescriptor{EventType: "DocumentRevision", Tags: []dcb.Tag{{Category: "document", Keys: []string{e.DocumentId}}}}
		}, func(e DocumentRevision) string {
			b, err := json.Marshal(e)
			if err != nil {
				panic(err)
			}
			return string(b)
		}, func(s string) DocumentRevision {
			var e DocumentRevision
			if err := json.Unmarshal([]byte(s), &e); err != nil {
				panic(err)
			}
			return e
		})
	}
	start := time.Now()
	s := store(open())
	latest := map[string]int{}
	s.Subscribe(func(events []dcb.Recorded[DocumentRevision]) bool {
		for _, e := range events {
			latest[e.Payload.DocumentId] = e.Payload.Revision
		}
		return true
	})
	body := strings.Repeat("x", 4096)
	for revision := 1; revision <= 16; revision++ {
		for i := 0; i < count; i++ {
			id := fmt.Sprintf("document-%08d", i)
			q := dcb.Query{Items: []dcb.QueryItem{{Tags: []dcb.Tag{{Category: "document", Keys: []string{id}}}}}}
			r := s.Read(q)
			if len(r.Events) != revision-1 || !s.Append(fmt.Sprintf("doc-%d-%d", i, revision), q, r.After, []DocumentRevision{{id, revision, body}}).Committed {
				panic("document revision mismatch")
			}
		}
	}
	all := s.Read(dcb.Query{})
	if all.After != 16*count || len(latest) != count || s.ListenerFaults() != 0 {
		panic("document projection mismatch")
	}
	for _, r := range latest {
		if r != 16 {
			panic("document revision mismatch")
		}
	}
	for _, e := range all.Events {
		if len(e.Payload.Body) != 4096 {
			panic("document body mismatch")
		}
	}
	s.Close()
	if path != "" {
		reopened := store(open())
		r := reopened.Read(dcb.Query{})
		if len(r.Events) != 16*count {
			panic("document restart mismatch")
		}
		for _, e := range r.Events {
			if len(e.Payload.Body) != 4096 {
				panic("document body mismatch")
			}
		}
		reopened.Close()
	}
	fmt.Fprintln(os.Stderr, "DOCUMENT", time.Since(start).Seconds(), count, all.After, s.Encoded, s.Decoded)
}
