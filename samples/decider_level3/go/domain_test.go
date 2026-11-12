package main

import (
	"example.com/dcb-school/dcb"
	d "example.com/dcb-school/domain"
	"reflect"
	"testing"
)

type countingStore struct {
	inner *dcb.EventStore[d.Event]
	reads int
}

func (p *countingStore) Read(q dcb.Query) dcb.ReadResult[d.Event] { p.reads++; return p.inner.Read(q) }
func (p *countingStore) Append(id string, q dcb.Query, after int, events []d.Event) dcb.AppendResult[d.Event] {
	return p.inner.Append(id, q, after, events)
}

type givenHistory struct {
	store      *dcb.EventStore[d.Event]
	projection *dcb.LiveProjection[d.Event, d.Registers]
	port       *countingStore
	executor   dcb.Executor[d.Command, d.Event, d.Rejection]
}

func givenEvents(events []d.Event) givenHistory {
	store := dcb.NewStore(d.DescribeEvent, d.CopyEvent)
	if len(events) > 0 {
		store.Append("given", dcb.Query{Items: []dcb.QueryItem{}}, 0, events)
	}
	projection := dcb.NewProjection(d.EmptyRegisters(), d.Reduce, d.CopyRegisters)
	store.Subscribe(projection.Accept)
	port := &countingStore{inner: store}
	return givenHistory{store, projection, port, dcb.DcbExecutor[d.Command, d.Event, d.Rejection]{Store: port}}
}
func checkProjection(t *testing.T, g givenHistory) {
	t.Helper()
	events := g.store.Read(dcb.Query{}).Events
	position, state := g.projection.Snapshot()
	if position != len(events) || !reflect.DeepEqual(state.List(), d.Fold(events).List()) {
		t.Fatal("live / replay state")
	}
}
func check(t *testing.T, name string, given []d.Event, when d.Command, thenEvents []d.Event, thenErrors []d.Rejection, invalid bool) {
	t.Helper()
	t.Run(name, func(t *testing.T) {
		g := givenEvents(given)                                         // Given
		prepared := g.executor.Prepare("when", when, d.SchoolHandler{}) // When
		wantReads := 1
		if invalid {
			wantReads = 0
		}
		if prepared.WasRead == invalid || g.port.reads != wantReads {
			t.Fatal("validation before reads")
		}
		result := prepared.Commit()
		events := g.store.Read(dcb.Query{}).Events
		appended := []d.Event{}
		for _, e := range events[len(given):] {
			appended = append(appended, e.Payload)
		}
		status := "Committed"
		if len(thenErrors) > 0 {
			status = "Rejected"
		}
		if result.Status != status || !reflect.DeepEqual(result.Errors, thenErrors) || !reflect.DeepEqual(appended, thenEvents) || result.Appended != len(thenEvents) || result.Conflicts != 0 {
			t.Fatalf("Then: result=%+v events=%+v", result, appended)
		}
		checkProjection(t, g)
	})
}
func TestSchoolDomain(t *testing.T) {
	empty := []d.Event{}
	registered := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}}
	opened := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}}
	enrolled := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}}
	teaching := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}}
	attended := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}, d.AttendanceRecorded{ClassId: "a", StudentId: "s"}}
	passed := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}, d.AttendanceRecorded{ClassId: "a", StudentId: "s"}, d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 60}}
	failed := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}, d.AttendanceRecorded{ClassId: "a", StudentId: "s"}, d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 59}}
	advanced := []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}, d.AttendanceRecorded{ClassId: "a", StudentId: "s"}, d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 60}, d.ClassOpened{ClassId: "b", Course: d.Advanced, Capacity: 1}}
	check(t, "register student", empty, d.RegisterStudent{StudentId: "s", Grade: d.Year3}, []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}}, []d.Rejection{}, false)
	check(t, "duplicate student", registered, d.RegisterStudent{StudentId: "s", Grade: d.Year3}, []d.Event{}, []d.Rejection{d.DuplicateStudent}, false)
	check(t, "promote one grade", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year1}}, d.PromoteStudent{StudentId: "s", Grade: d.Year2}, []d.Event{d.StudentPromoted{StudentId: "s", Grade: d.Year2}}, []d.Rejection{}, false)
	check(t, "promotion cannot skip", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year1}}, d.PromoteStudent{StudentId: "s", Grade: d.Year3}, []d.Event{}, []d.Rejection{d.InvalidPromotion}, false)
	check(t, "promotion cannot go backwards", registered, d.PromoteStudent{StudentId: "s", Grade: d.Year2}, []d.Event{}, []d.Rejection{d.InvalidPromotion}, false)
	check(t, "unknown student promotion", empty, d.PromoteStudent{StudentId: "s", Grade: d.Year2}, []d.Event{}, []d.Rejection{d.UnknownStudent}, false)
	check(t, "open foundation", registered, d.OpenClass{ClassId: "a", Course: d.Foundation, Capacity: 1}, []d.Event{d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}}, []d.Rejection{}, false)
	check(t, "open advanced", registered, d.OpenClass{ClassId: "b", Course: d.Advanced, Capacity: 10}, []d.Event{d.ClassOpened{ClassId: "b", Course: d.Advanced, Capacity: 10}}, []d.Rejection{}, false)
	check(t, "capacity upper bound", empty, d.OpenClass{ClassId: "a", Course: d.Foundation, Capacity: 10}, []d.Event{d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 10}}, []d.Rejection{}, false)
	check(t, "duplicate class", opened, d.OpenClass{ClassId: "a", Course: d.Foundation, Capacity: 1}, []d.Event{}, []d.Rejection{d.DuplicateClass}, false)
	check(t, "enroll", opened, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}}, []d.Rejection{}, false)
	check(t, "unknown class", registered, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.UnknownClass}, false)
	check(t, "unknown student enrollment", []d.Event{d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}}, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.UnknownStudent}, false)
	check(t, "grade below minimum", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year1}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}}, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.GradeTooLow}, false)
	check(t, "grade exactly minimum", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year2}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}}, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}}, []d.Rejection{}, false)
	check(t, "missing prerequisite", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "b", Course: d.Advanced, Capacity: 1}}, d.Enroll{ClassId: "b", StudentId: "s"}, []d.Event{}, []d.Rejection{d.PrerequisiteMissing}, false)
	check(t, "failed prerequisite", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}, d.AttendanceRecorded{ClassId: "a", StudentId: "s"}, d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 59}, d.ClassOpened{ClassId: "b", Course: d.Advanced, Capacity: 1}}, d.Enroll{ClassId: "b", StudentId: "s"}, []d.Event{}, []d.Rejection{d.PrerequisiteMissing}, false)
	check(t, "passed prerequisite", advanced, d.Enroll{ClassId: "b", StudentId: "s"}, []d.Event{d.StudentEnrolled{ClassId: "b", StudentId: "s", Course: d.Advanced}}, []d.Rejection{}, false)
	check(t, "duplicate enrollment", enrolled, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.AlreadyEnrolled}, false)
	check(t, "last seat occupied", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.StudentRegistered{StudentId: "t", Grade: d.Year3}}, d.Enroll{ClassId: "a", StudentId: "t"}, []d.Event{}, []d.Rejection{d.ClassFull}, false)
	check(t, "enrollment after start", teaching, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.RegistrationClosed}, false)
	check(t, "drop reservation", enrolled, d.Drop{ClassId: "a", StudentId: "s"}, []d.Event{d.StudentDropped{ClassId: "a", StudentId: "s"}}, []d.Rejection{}, false)
	check(t, "drop nonmember", opened, d.Drop{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.NotEnrolled}, false)
	check(t, "drop after start", teaching, d.Drop{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.RegistrationClosed}, false)
	check(t, "reenroll after withdrawal", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.StudentDropped{ClassId: "a", StudentId: "s"}}, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}}, []d.Rejection{}, false)
	check(t, "start empty class", opened, d.StartClass{ClassId: "a", StartedOn: "2026-04-01"}, []d.Event{d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}}, []d.Rejection{}, false)
	check(t, "start emits study event atomically", enrolled, d.StartClass{ClassId: "a", StartedOn: "2026-04-01"}, []d.Event{d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}}, []d.Rejection{}, false)
	check(t, "start twice", teaching, d.StartClass{ClassId: "a", StartedOn: "2026-04-01"}, []d.Event{}, []d.Rejection{d.RegistrationClosed}, false)
	check(t, "valid leap date", opened, d.StartClass{ClassId: "a", StartedOn: "2028-02-29"}, []d.Event{d.ClassStarted{ClassId: "a", StartedOn: "2028-02-29"}}, []d.Rejection{}, false)
	check(t, "attendance before start", enrolled, d.Attend{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.NotTeaching}, false)
	check(t, "attendance", teaching, d.Attend{ClassId: "a", StudentId: "s"}, []d.Event{d.AttendanceRecorded{ClassId: "a", StudentId: "s"}}, []d.Rejection{}, false)
	check(t, "attendance nonmember", teaching, d.Attend{ClassId: "a", StudentId: "t"}, []d.Event{}, []d.Rejection{d.NotEnrolled}, false)
	check(t, "assessment without attendance", teaching, d.Assess{ClassId: "a", StudentId: "s", Score: 60}, []d.Event{}, []d.Rejection{d.NoAttendance}, false)
	check(t, "assessment boundary 0", attended, d.Assess{ClassId: "a", StudentId: "s", Score: 0}, []d.Event{d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 0}}, []d.Rejection{}, false)
	check(t, "assessment boundary 59", attended, d.Assess{ClassId: "a", StudentId: "s", Score: 59}, []d.Event{d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 59}}, []d.Rejection{}, false)
	check(t, "assessment boundary 60", attended, d.Assess{ClassId: "a", StudentId: "s", Score: 60}, []d.Event{d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 60}}, []d.Rejection{}, false)
	check(t, "assessment boundary 100", attended, d.Assess{ClassId: "a", StudentId: "s", Score: 100}, []d.Event{d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 100}}, []d.Rejection{}, false)
	check(t, "assessment before start", enrolled, d.Assess{ClassId: "a", StudentId: "s", Score: 60}, []d.Event{}, []d.Rejection{d.NotTeaching}, false)
	check(t, "assessment nonmember", attended, d.Assess{ClassId: "a", StudentId: "t", Score: 60}, []d.Event{}, []d.Rejection{d.NotEnrolled}, false)
	check(t, "assessment twice", passed, d.Assess{ClassId: "a", StudentId: "s", Score: 60}, []d.Event{}, []d.Rejection{d.AlreadyAssessed}, false)
	check(t, "attendance after assessment", passed, d.Attend{ClassId: "a", StudentId: "s"}, []d.Event{}, []d.Rejection{d.AlreadyAssessed}, false)
	check(t, "completion before start", opened, d.CompleteClass{ClassId: "a"}, []d.Event{}, []d.Rejection{d.NotTeaching}, false)
	check(t, "completion with unassessed student", teaching, d.CompleteClass{ClassId: "a"}, []d.Event{}, []d.Rejection{d.UnassessedStudents}, false)
	check(t, "complete passed", passed, d.CompleteClass{ClassId: "a"}, []d.Event{d.ClassCompleted{ClassId: "a", Results: []d.ClassResult{d.ClassResult{StudentId: "s", Progress: d.Passed{Score: 60, Credits: 1}}}}}, []d.Rejection{}, false)
	check(t, "complete failed", failed, d.CompleteClass{ClassId: "a"}, []d.Event{d.ClassCompleted{ClassId: "a", Results: []d.ClassResult{d.ClassResult{StudentId: "s", Progress: d.Failed{Score: 59, Reason: d.BelowPassMark}}}}}, []d.Rejection{}, false)
	check(t, "complete empty class", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}}, d.CompleteClass{ClassId: "a"}, []d.Event{d.ClassCompleted{ClassId: "a", Results: []d.ClassResult{}}}, []d.Rejection{}, false)
	check(t, "unknown class Drop", registered, d.Drop{ClassId: "missing", StudentId: "s"}, []d.Event{}, []d.Rejection{d.UnknownClass}, false)
	check(t, "unknown class StartClass", registered, d.StartClass{ClassId: "missing", StartedOn: "2026-04-01"}, []d.Event{}, []d.Rejection{d.UnknownClass}, false)
	check(t, "unknown class Attend", registered, d.Attend{ClassId: "missing", StudentId: "s"}, []d.Event{}, []d.Rejection{d.UnknownClass}, false)
	check(t, "unknown class Assess", registered, d.Assess{ClassId: "missing", StudentId: "s", Score: 60}, []d.Event{}, []d.Rejection{d.UnknownClass}, false)
	check(t, "unknown class CompleteClass", registered, d.CompleteClass{ClassId: "missing"}, []d.Event{}, []d.Rejection{d.UnknownClass}, false)
	check(t, "invalid id RegisterStudent", empty, d.RegisterStudent{StudentId: " ", Grade: d.Year2}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id PromoteStudent", empty, d.PromoteStudent{StudentId: "", Grade: d.Year2}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id OpenClass", empty, d.OpenClass{ClassId: "", Course: d.Foundation, Capacity: 1}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id Enroll", empty, d.Enroll{ClassId: "a", StudentId: ""}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id Drop", empty, d.Drop{ClassId: "", StudentId: "s"}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id StartClass", empty, d.StartClass{ClassId: "", StartedOn: "2026-04-01"}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id Attend", empty, d.Attend{ClassId: "a", StudentId: "\t"}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id Assess", empty, d.Assess{ClassId: "", StudentId: "s", Score: 60}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid id CompleteClass", empty, d.CompleteClass{ClassId: ""}, []d.Event{}, []d.Rejection{d.InvalidId}, true)
	check(t, "invalid capacity 0", empty, d.OpenClass{ClassId: "a", Course: d.Foundation, Capacity: 0}, []d.Event{}, []d.Rejection{d.InvalidCapacity}, true)
	check(t, "invalid capacity 11", empty, d.OpenClass{ClassId: "a", Course: d.Foundation, Capacity: 11}, []d.Event{}, []d.Rejection{d.InvalidCapacity}, true)
	check(t, "invalid score -1", empty, d.Assess{ClassId: "a", StudentId: "s", Score: -1}, []d.Event{}, []d.Rejection{d.InvalidScore}, true)
	check(t, "invalid score 101", empty, d.Assess{ClassId: "a", StudentId: "s", Score: 101}, []d.Event{}, []d.Rejection{d.InvalidScore}, true)
	check(t, "invalid date ", empty, d.StartClass{ClassId: "a", StartedOn: ""}, []d.Event{}, []d.Rejection{d.InvalidDate}, true)
	check(t, "invalid date 2026-02-29", empty, d.StartClass{ClassId: "a", StartedOn: "2026-02-29"}, []d.Event{}, []d.Rejection{d.InvalidDate}, true)
	check(t, "invalid date 2026-04-31", empty, d.StartClass{ClassId: "a", StartedOn: "2026-04-31"}, []d.Event{}, []d.Rejection{d.InvalidDate}, true)
	check(t, "invalid date 2026-1-01", empty, d.StartClass{ClassId: "a", StartedOn: "2026-1-01"}, []d.Event{}, []d.Rejection{d.InvalidDate}, true)
	check(t, "invalid date 0000-01-01", empty, d.StartClass{ClassId: "a", StartedOn: "0000-01-01"}, []d.Event{}, []d.Rejection{d.InvalidDate}, true)
	check(t, "invalid date 2026-13-01", empty, d.StartClass{ClassId: "a", StartedOn: "2026-13-01"}, []d.Event{}, []d.Rejection{d.InvalidDate}, true)
	check(t, "historical capacity remains replayable", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 25}}, d.Enroll{ClassId: "a", StudentId: "s"}, []d.Event{d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}}, []d.Rejection{}, false)
	check(t, "historical date remains replayable", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "Spring 2020"}, d.StudyStarted{ClassId: "a", StudentId: "s"}}, d.Attend{ClassId: "a", StudentId: "s"}, []d.Event{d.AttendanceRecorded{ClassId: "a", StudentId: "s"}}, []d.Rejection{}, false)
	check(t, "historical score remains replayable", []d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}, d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}, d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}, d.StudyStarted{ClassId: "a", StudentId: "s"}, d.AttendanceRecorded{ClassId: "a", StudentId: "s"}, d.ResultRecorded{ClassId: "a", StudentId: "s", Score: 120}, d.ClassOpened{ClassId: "b", Course: d.Advanced, Capacity: 1}}, d.Enroll{ClassId: "b", StudentId: "s"}, []d.Event{d.StudentEnrolled{ClassId: "b", StudentId: "s", Course: d.Advanced}}, []d.Rejection{}, false)
	check(t, "new command cannot use historical capacity", empty, d.OpenClass{ClassId: "a", Course: d.Foundation, Capacity: 25}, []d.Event{}, []d.Rejection{d.InvalidCapacity}, true)
	check(t, "new command cannot use historical date", empty, d.StartClass{ClassId: "a", StartedOn: "Spring 2020"}, []d.Event{}, []d.Rejection{d.InvalidDate}, true)
	check(t, "new command cannot use historical score", empty, d.Assess{ClassId: "a", StudentId: "s", Score: 120}, []d.Event{}, []d.Rejection{d.InvalidScore}, true)
	g := givenEvents([]d.Event{d.StudentRegistered{StudentId: "s", Grade: d.Year3}, d.StudentRegistered{StudentId: "t", Grade: d.Year3}, d.ClassOpened{ClassId: "a", Course: d.Foundation, Capacity: 1}})
	first := g.executor.Prepare("first", d.Enroll{ClassId: "a", StudentId: "s"}, d.SchoolHandler{})
	second := g.executor.Prepare("second", d.Enroll{ClassId: "a", StudentId: "t"}, d.SchoolHandler{})
	if first.Commit().Status != "Committed" {
		t.Fatal("first last-seat command")
	}
	conflict := second.Commit()
	if conflict.Conflicts != 1 || !reflect.DeepEqual(conflict.Errors, []d.Rejection{d.ClassFull}) || len(g.store.Read(dcb.Query{}).Events) != 4 {
		t.Fatal("retry must redecide")
	}
	checkProjection(t, g)
	check(t, "invalid native grade", empty, d.RegisterStudent{StudentId: "s", Grade: d.Grade("invalid")}, []d.Event{}, []d.Rejection{d.InvalidCommand}, true)
	check(t, "nil interface", empty, nil, []d.Event{}, []d.Rejection{d.InvalidCommand}, true)
	pureRegisters := d.EmptyRegisters()
	for _, e := range opened {
		pureRegisters = d.Reduce(pureRegisters, e)
	}
	pure := pureRegisters.DecisionModel()
	valid := func(command d.Command) d.ValidatedCommand {
		value, errors := d.NewValidatedCommand(command)
		if len(errors) > 0 {
			t.Fatal(errors)
		}
		return value
	}
	if !reflect.DeepEqual(d.Decide(pure, valid(d.Enroll{ClassId: "a", StudentId: "s"})), d.Accepted{Events: []d.Event{d.StudentEnrolled{ClassId: "a", StudentId: "s", Course: d.Foundation}}}) {
		t.Fatal("pure Decider enrollment")
	}
	if !reflect.DeepEqual(d.Decide(pure, valid(d.Enroll{ClassId: "a", StudentId: "missing"})), d.Rejected{Code: d.UnknownStudent}) {
		t.Fatal("pure Decider rejection")
	}
	if !reflect.DeepEqual(d.Decide(pure, valid(d.StartClass{ClassId: "a", StartedOn: "2026-04-01"})), d.Accepted{Events: []d.Event{d.ClassStarted{ClassId: "a", StartedOn: "2026-04-01"}}}) {
		t.Fatal("pure empty-class start")
	}
	if _, err := d.NewStudentId("s"); err != nil {
		t.Fatal(err)
	}
	if _, err := d.NewClassId(" "); err == nil {
		t.Fatal("invalid ID accepted")
	}
	if _, err := d.NewSeatCapacity(25); err == nil {
		t.Fatal("invalid capacity accepted")
	}
	if _, err := d.NewAssessmentScore(120); err == nil {
		t.Fatal("invalid score accepted")
	}
	if _, err := d.NewSchoolDate("Spring 2020"); err == nil {
		t.Fatal("invalid date accepted")
	}
	if d.RestoreSeatCapacity(25).Value() != 25 || d.RestoreAssessmentScore(120).Value() != 120 || d.RestoreSchoolDate("Spring 2020").Value() != "Spring 2020" {
		t.Fatal("event restoration lost historical value")
	}
}
