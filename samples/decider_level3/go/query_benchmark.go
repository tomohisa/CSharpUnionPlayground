package main

import (
	"encoding/json"
	"example.com/dcb-school/dcb"
	d "example.com/dcb-school/domain"
	"fmt"
	"os"
	"strconv"
	"time"
)

func queryBenchmark(store *dcb.JsonEventStore[d.Event], projection *d.SchoolProjection) {
	n, _ := strconv.Atoi(os.Getenv("DECIDER_STUDENTS"))
	if n <= 0 {
		return
	}
	start := time.Now()
	for i := 0; i < 1000; i++ {
		id := fmt.Sprintf("student-%08d", i*7919%n)
		if len(store.Read(dcb.Query{Items: []dcb.QueryItem{{Tags: []dcb.Tag{{Category: "student", Keys: []string{id}}}}}}).Events) == 0 {
			panic("point read missing")
		}
	}
	fmt.Fprintln(os.Stderr, "PHASE tagged_point", time.Since(start).Seconds())
	start = time.Now()
	for i := 0; i < 1000; i++ {
		id := fmt.Sprintf("student-%08d", i*7919%n)
		b, err := json.Marshal(projection.Student(id))
		if err != nil {
			panic(err)
		}
		var s d.Student
		if err = json.Unmarshal(b, &s); err != nil {
			panic(err)
		}
		if s.StudentId != id {
			panic("point query mismatch")
		}
	}
	fmt.Fprintln(os.Stderr, "PHASE projection_point_json", time.Since(start).Seconds())
}
