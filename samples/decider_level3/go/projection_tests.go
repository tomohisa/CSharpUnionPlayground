package main

import (
	"encoding/json"
	"example.com/dcb-school/dcb"
	d "example.com/dcb-school/domain"
	"fmt"
	"time"
)

func projectionTests(size int) {
	updates := 1000
	p := d.NewSchoolProjection()
	position := 0
	require := func(ok bool) {
		if !ok {
			panic("projection scaling invariant failed")
		}
	}
	envelope := func(n int, e d.Event) dcb.Recorded[d.Event] {
		return dcb.Recorded[d.Event]{Position: n, CommandId: "scale", EventType: "projection-test", Tags: []dcb.Tag{}, Payload: e}
	}
	send := func(e d.Event) { position++; require(p.Accept([]dcb.Recorded[d.Event]{envelope(position, e)})) }
	members := func(c d.ClassState) []string {
		switch x := c.(type) {
		case d.Available:
			return x.Students
		case d.Full:
			return x.Students
		case d.Teaching:
			return x.Students
		default:
			panic("not a roster")
		}
	}
	start := time.Now()
	send(d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: size + updates + 1})
	for i := 0; i < size; i++ {
		id := fmt.Sprint("s", i)
		send(d.StudentRegistered{StudentId: id, Grade: d.Year3})
		send(d.StudentEnrolled{ClassId: "a", StudentId: id, Course: d.Foundation})
	}
	seed := time.Since(start).Seconds()
	_, old := p.Snapshot()
	before, work := p.Checkpoint(), p.Work()
	require(!p.Accept([]dcb.Recorded[d.Event]{envelope(before+1, d.StudentPromoted{StudentId: "s0", Grade: d.Year1}), envelope(before+2, d.AttendanceRecorded{ClassId: "a", StudentId: "s0"})}))
	_, after := p.Snapshot()
	require(p.Checkpoint() == before && p.Work() == work)
	for _, s := range after.Students {
		if s.StudentId == "s0" {
			require(s.Grade == d.Year3)
		}
	}
	require(!p.Accept([]dcb.Recorded[d.Event]{envelope(before+1, d.StudentPromoted{StudentId: "s0", Grade: d.Year1}), envelope(before+3, d.StudentPromoted{StudentId: "s0", Grade: d.Year1})}))
	require(p.Checkpoint() == before && p.Work() == work && p.Accept([]dcb.Recorded[d.Event]{envelope(1, d.ClassOpened{ClassId: "ignored", Course: d.Foundation, Capacity: 1})}) && p.Work() == work)
	stableRows := p.RowCount()
	for i := 0; i < 100; i++ {
		send(d.StudentDropped{ClassId: "a", StudentId: "s1"})
		send(d.StudentEnrolled{ClassId: "a", StudentId: "s1", Course: d.Foundation})
	}
	_, current := p.Snapshot()
	order := members(current.Classes[0])
	require(p.RowCount() == stableRows && len(order) == size && order[0] == "s0" && order[1] == "s2" && order[len(order)-1] == "s1")
	start = time.Now()
	work = p.Work()
	for i := size; i < size+updates; i++ {
		id := fmt.Sprint("s", i)
		send(d.StudentRegistered{StudentId: id, Grade: d.Year3})
		send(d.StudentEnrolled{ClassId: "a", StudentId: id, Course: d.Foundation})
	}
	growth, growthWork := time.Since(start).Seconds(), p.Work()-work
	require(growthWork == int64(8*updates) && len(members(old.Classes[0])) == size && len(old.Students) == size)
	send(d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"})
	send(d.StudyStarted{ClassId: "a", StudentId: "s0"})
	start = time.Now()
	work = p.Work()
	for i := 0; i < updates; i++ {
		send(d.AttendanceRecorded{ClassId: "a", StudentId: "s0"})
	}
	update, updateWork := time.Since(start).Seconds(), p.Work()-work
	require(updateWork == int64(3*updates))
	start = time.Now()
	_, snapshot := p.Snapshot()
	query := time.Since(start).Seconds()
	require(len(snapshot.Students) == size+updates && len(members(snapshot.Classes[0])) == size+updates)
	for _, e := range snapshot.Enrollments {
		if e.StudentId == "s0" {
			require(e.Progress.(d.Studying).Attendance == updates)
		}
	}
	snapshot.Students[0].Grade = d.Year1
	_, detached := p.Snapshot()
	require(detached.Students[0].Grade == d.Year3)
	out, err := json.Marshal(map[string]any{"size": size, "updates": updates, "rows": p.RowCount(), "seedSeconds": seed, "updateSeconds": update, "growthSeconds": growth, "snapshotSeconds": query, "updateWork": updateWork, "growthWork": growthWork})
	if err != nil {
		panic(err)
	}
	fmt.Println(string(out))
}
