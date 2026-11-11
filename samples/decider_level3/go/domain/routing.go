package domain

import "example.com/dcb-school/dcb"

func StudentTag(id string) dcb.Tag      { return dcb.Tag{Category: "student", Keys: []string{id}} }
func ClassTag(id string) dcb.Tag        { return dcb.Tag{Category: "class", Keys: []string{id}} }
func EnrollmentTag(c, s string) dcb.Tag { return dcb.Tag{Category: "enrollment", Keys: []string{c, s}} }
func one(tags ...dcb.Tag) dcb.Query {
	items := []dcb.QueryItem{}
	for _, t := range tags {
		items = append(items, dcb.QueryItem{Tags: []dcb.Tag{t}, Types: []string{}})
	}
	return dcb.Query{Items: items}
}
func QueryFor(command Command) dcb.Query {
	switch c := command.(type) {
	case RegisterStudent:
		return one(StudentTag(c.StudentId))
	case PromoteStudent:
		return one(StudentTag(c.StudentId))
	case OpenClass:
		return one(ClassTag(c.ClassId))
	case Enroll:
		return one(StudentTag(c.StudentId), ClassTag(c.ClassId))
	case Drop:
		return one(ClassTag(c.ClassId))
	case StartClass:
		return one(ClassTag(c.ClassId))
	case Attend:
		return one(ClassTag(c.ClassId))
	case Assess:
		return one(ClassTag(c.ClassId))
	case CompleteClass:
		return one(ClassTag(c.ClassId))
	default:
		panic("unknown command")
	}
}
func DescribeEvent(event Event) dcb.EventDescriptor {
	switch e := event.(type) {
	case StudentRegistered:
		return dcb.EventDescriptor{EventType: "StudentRegistered", Tags: []dcb.Tag{StudentTag(e.StudentId)}}
	case StudentPromoted:
		return dcb.EventDescriptor{EventType: "StudentPromoted", Tags: []dcb.Tag{StudentTag(e.StudentId)}}
	case ClassOpened:
		return dcb.EventDescriptor{EventType: "ClassOpened", Tags: []dcb.Tag{ClassTag(e.ClassId)}}
	case StudentEnrolled:
		return dcb.EventDescriptor{EventType: "StudentEnrolled", Tags: []dcb.Tag{StudentTag(e.StudentId), ClassTag(e.ClassId), EnrollmentTag(e.ClassId, e.StudentId)}}
	case StudentDropped:
		return dcb.EventDescriptor{EventType: "StudentDropped", Tags: []dcb.Tag{StudentTag(e.StudentId), ClassTag(e.ClassId), EnrollmentTag(e.ClassId, e.StudentId)}}
	case ClassStarted:
		return dcb.EventDescriptor{EventType: "ClassStarted", Tags: []dcb.Tag{ClassTag(e.ClassId)}}
	case AttendanceRecorded:
		return dcb.EventDescriptor{EventType: "AttendanceRecorded", Tags: []dcb.Tag{StudentTag(e.StudentId), ClassTag(e.ClassId), EnrollmentTag(e.ClassId, e.StudentId)}}
	case ResultRecorded:
		return dcb.EventDescriptor{EventType: "ResultRecorded", Tags: []dcb.Tag{StudentTag(e.StudentId), ClassTag(e.ClassId), EnrollmentTag(e.ClassId, e.StudentId)}}
	case ClassCompleted:
		return dcb.EventDescriptor{EventType: "ClassCompleted", Tags: []dcb.Tag{ClassTag(e.ClassId)}}
	case StudyStarted:
		return dcb.EventDescriptor{EventType: "StudyStarted", Tags: []dcb.Tag{StudentTag(e.StudentId), ClassTag(e.ClassId), EnrollmentTag(e.ClassId, e.StudentId)}}
	default:
		panic("unknown event")
	}
}
func PrepareDecision(events []dcb.Recorded[Event], command Command) dcb.Preparation[Event, Rejection] {
	switch decision := Decide(Fold(events).DecisionModel(), command).(type) {
	case Accepted:
		return dcb.Preparation[Event, Rejection]{Events: decision.Events, Errors: []Rejection{}}
	case Rejected:
		return dcb.Preparation[Event, Rejection]{Events: []Event{}, Errors: []Rejection{decision.Code}}
	default:
		panic("unknown decision")
	}
}
func CopyEvent(e Event) Event {
	if x, ok := e.(ClassCompleted); ok {
		x.Results = append([]ClassResult{}, x.Results...)
		return x
	}
	return e
}
