package domain

import "example.com/dcb-school/dcb"
import "sort"

type indexKey struct {
	Kind, A, B string
	Node       int
}
type indexRow interface{ indexRow() }
type studentRow struct{ Value Student }

func (studentRow) indexRow() {}

type classRow struct {
	Phase              ClassState
	First, Last, Count int
}

func (classRow) indexRow() {}

type enrollmentRow struct{ Value Enrollment }

func (enrollmentRow) indexRow() {}

type memberRow struct {
	Student        string
	Previous, Next int
}

func (memberRow) indexRow() {}

type memberIndex struct{ Node int }

func (memberIndex) indexRow()           {}
func rowKey(kind, a, b string) indexKey { return indexKey{Kind: kind, A: a, B: b} }
func nodeKey(n int) indexKey            { return indexKey{Kind: "node", Node: n} }
func applyIndex(tx *dcb.IndexBatch[indexKey, indexRow], recorded dcb.Recorded[Event]) {
	e := recorded.Payload
	switch x := e.(type) {
	case StudentRegistered:
		tx.Put(rowKey("student", x.StudentId, ""), studentRow{Student{x.StudentId, x.Grade}})
		return
	case StudentPromoted:
		tx.Put(rowKey("student", x.StudentId, ""), studentRow{Student{x.StudentId, x.Grade}})
		return
	case ClassOpened:
		state := Reduce(EmptyRegisters(), e)
		tx.Put(rowKey("class", x.ClassId, ""), classRow{Phase: state.Classes[x.ClassId]})
		return
	}
	var id string
	switch x := e.(type) {
	case StudentEnrolled:
		id = x.ClassId
	case StudentDropped:
		id = x.ClassId
	case ClassStarted:
		id = x.ClassId
	case ClassCompleted:
		id = x.ClassId
	case StudyStarted:
		id = x.ClassId
	case AttendanceRecorded:
		id = x.ClassId
	case ResultRecorded:
		id = x.ClassId
	default:
		panic("unknown event")
	}
	ck := rowKey("class", id, "")
	if found, ok := tx.Find(ck); ok {
		c := found.(classRow)
		plan := PlanOf(c.Phase)
		switch x := e.(type) {
		case StudentEnrolled:
			n := recorded.Position
			if c.Last != 0 {
				m := tx.Get(nodeKey(c.Last)).(memberRow)
				m.Next = n
				tx.Put(nodeKey(c.Last), m)
			}
			tx.Put(nodeKey(n), memberRow{x.StudentId, c.Last, 0})
			tx.Put(rowKey("member", id, x.StudentId), memberIndex{n})
			if c.First == 0 {
				c.First = n
			}
			c.Last = n
			c.Count++
			if c.Count == plan.Capacity {
				c.Phase = Full{plan, []string{}}
			} else {
				c.Phase = Available{plan, []string{}}
			}
			tx.Put(ck, c)
		case StudentDropped:
			if found, ok := tx.Find(rowKey("member", id, x.StudentId)); ok {
				n := found.(memberIndex).Node
				m := tx.Get(nodeKey(n)).(memberRow)
				if m.Previous != 0 {
					p := tx.Get(nodeKey(m.Previous)).(memberRow)
					p.Next = m.Next
					tx.Put(nodeKey(m.Previous), p)
				}
				if m.Next != 0 {
					p := tx.Get(nodeKey(m.Next)).(memberRow)
					p.Previous = m.Previous
					tx.Put(nodeKey(m.Next), p)
				}
				tx.Delete(nodeKey(n))
				tx.Delete(rowKey("member", id, x.StudentId))
				if c.First == n {
					c.First = m.Next
				}
				if c.Last == n {
					c.Last = m.Previous
				}
				c.Count--
			}
			c.Phase = Available{plan, []string{}}
			tx.Put(ck, c)
		case ClassStarted:
			c.Phase = Teaching{plan, []string{}, RestoreSchoolDate(x.StartedOn).Value()}
			tx.Put(ck, c)
		case ClassCompleted:
			c.Phase = Completed{plan, append([]ClassResult{}, x.Results...)}
			tx.Put(ck, c)
		}
	}
	var student string
	switch x := e.(type) {
	case StudentEnrolled:
		student = x.StudentId
	case StudentDropped:
		student = x.StudentId
	case StudyStarted:
		student = x.StudentId
	case AttendanceRecorded:
		student = x.StudentId
	case ResultRecorded:
		student = x.StudentId
	default:
		return
	}
	ek := rowKey("enrollment", id, student)
	if x, ok := e.(StudentEnrolled); ok {
		tx.Put(ek, enrollmentRow{Enrollment{id, student, x.Course, Reserved{}}})
		return
	}
	found, ok := tx.Find(ek)
	if !ok {
		return
	}
	value := found.(enrollmentRow).Value
	switch x := e.(type) {
	case StudentDropped:
		value.Progress = Withdrawn{StudentRequest}
	case StudyStarted:
		value.Progress = Studying{0}
	case AttendanceRecorded:
		p, ok := value.Progress.(Studying)
		if !ok {
			panic("invalid committed event")
		}
		value.Progress = Studying{p.Attendance + 1}
	case ResultRecorded:
		score := RestoreAssessmentScore(x.Score).Value()
		if score >= 60 {
			credits := 1
			if value.Course == Advanced {
				credits = 2
			}
			value.Progress = Passed{score, credits}
		} else {
			value.Progress = Failed{score, BelowPassMark}
		}
	}
	tx.Put(ek, enrollmentRow{value})
}

type SchoolProjection struct {
	index *dcb.IndexedProjection[indexKey, indexRow, Event]
}

func NewSchoolProjection() *SchoolProjection {
	return &SchoolProjection{dcb.NewIndexedProjection(applyIndex)}
}
func (p *SchoolProjection) Accept(events []dcb.Recorded[Event]) bool { return p.index.Accept(events) }
func (p *SchoolProjection) Checkpoint() int                          { return p.index.Checkpoint() }
func (p *SchoolProjection) RowCount() int                            { return p.index.RowCount() }
func (p *SchoolProjection) Work() int64                              { return p.index.Work() }
func (p *SchoolProjection) Student(id string) Student {
	var result Student
	p.index.Read(func(rows map[indexKey]indexRow) { result = rows[indexKey{Kind: "student", A: id}].(studentRow).Value })
	return result
}
func (p *SchoolProjection) Snapshot() (int, SchoolList) {
	list := SchoolList{Students: []Student{}, Classes: []ClassState{}, Enrollments: []Enrollment{}, Summaries: []string{}}
	position := p.index.Read(func(rows map[indexKey]indexRow) {
		for _, row := range rows {
			switch x := row.(type) {
			case studentRow:
				list.Students = append(list.Students, x.Value)
			case enrollmentRow:
				list.Enrollments = append(list.Enrollments, x.Value)
			case classRow:
				ids := []string{}
				if _, completed := x.Phase.(Completed); !completed {
					for n := x.First; n != 0; {
						m := rows[nodeKey(n)].(memberRow)
						ids = append(ids, m.Student)
						n = m.Next
					}
				}
				var phase ClassState
				switch c := x.Phase.(type) {
				case Available:
					phase = Available{c.Plan, ids}
				case Full:
					phase = Full{c.Plan, ids}
				case Teaching:
					phase = Teaching{c.Plan, ids, c.StartedOn}
				case Completed:
					phase = Completed{c.Plan, append([]ClassResult{}, c.Results...)}
				}
				list.Classes = append(list.Classes, phase)
			}
		}
	})
	sort.Slice(list.Students, func(i, j int) bool { return list.Students[i].StudentId < list.Students[j].StudentId })
	sort.Slice(list.Classes, func(i, j int) bool { return PlanOf(list.Classes[i]).ClassId < PlanOf(list.Classes[j]).ClassId })
	sort.Slice(list.Enrollments, func(i, j int) bool {
		a, b := list.Enrollments[i], list.Enrollments[j]
		if a.ClassId == b.ClassId {
			return a.StudentId < b.StudentId
		}
		return a.ClassId < b.ClassId
	})
	for _, e := range list.Enrollments {
		list.Summaries = append(list.Summaries, Describe(e.Progress))
	}
	return position, list
}
