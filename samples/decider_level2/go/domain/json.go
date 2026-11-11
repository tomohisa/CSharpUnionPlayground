package domain

import (
	"bytes"
	"encoding/json"
	"fmt"
)

func fields(data []byte, names ...string) (map[string]json.RawMessage, error) {
	var values map[string]json.RawMessage
	if err := json.Unmarshal(data, &values); err != nil {
		return values, err
	}
	for _, name := range names {
		raw, ok := values[name]
		if !ok || bytes.Equal(bytes.TrimSpace(raw), []byte("null")) {
			return values, fmt.Errorf("missing %s", name)
		}
	}
	return values, nil
}
func (value *Grade) UnmarshalJSON(data []byte) error {
	var text string
	if err := json.Unmarshal(data, &text); err != nil {
		return err
	}
	switch text {
	case "Year1", "Year2", "Year3":
		*value = Grade(text)
		return nil
	default:
		return fmt.Errorf("unknown Grade %s", text)
	}
}
func (value *Course) UnmarshalJSON(data []byte) error {
	var text string
	if err := json.Unmarshal(data, &text); err != nil {
		return err
	}
	switch text {
	case "Foundation", "Advanced":
		*value = Course(text)
		return nil
	default:
		return fmt.Errorf("unknown Course %s", text)
	}
}
func (value *FailureReason) UnmarshalJSON(data []byte) error {
	var text string
	if err := json.Unmarshal(data, &text); err != nil {
		return err
	}
	switch text {
	case "BelowPassMark":
		*value = FailureReason(text)
		return nil
	default:
		return fmt.Errorf("unknown FailureReason %s", text)
	}
}
func (value *WithdrawalReason) UnmarshalJSON(data []byte) error {
	var text string
	if err := json.Unmarshal(data, &text); err != nil {
		return err
	}
	switch text {
	case "StudentRequest":
		*value = WithdrawalReason(text)
		return nil
	default:
		return fmt.Errorf("unknown WithdrawalReason %s", text)
	}
}
func (value *Rejection) UnmarshalJSON(data []byte) error {
	var text string
	if err := json.Unmarshal(data, &text); err != nil {
		return err
	}
	switch text {
	case "InvalidId", "InvalidCapacity", "InvalidScore", "InvalidDate", "DuplicateStudent", "UnknownStudent", "InvalidPromotion", "DuplicateClass", "UnknownClass", "GradeTooLow", "PrerequisiteMissing", "AlreadyEnrolled", "ClassFull", "RegistrationClosed", "NotEnrolled", "NotTeaching", "NoAttendance", "AlreadyAssessed", "UnassessedStudents", "InvalidHistory":
		*value = Rejection(text)
		return nil
	default:
		return fmt.Errorf("unknown Rejection %s", text)
	}
}
func DecodePrerequisite(data []byte) (Prerequisite, error) {
	values, err := fields(data, "$type")
	if err != nil {
		return nil, err
	}
	var tag string
	if err := json.Unmarshal(values["$type"], &tag); err != nil {
		return nil, err
	}
	switch tag {
	case "NoPrerequisite":
		return decodeAs[NoPrerequisite](data)
	case "PassedCourse":
		return decodeAs[PassedCourse](data, "course")
	default:
		return nil, fmt.Errorf("unknown Prerequisite %s", tag)
	}
}
func (value NoPrerequisite) MarshalJSON() ([]byte, error) {
	type plain NoPrerequisite
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"NoPrerequisite", plain(value)})
}
func (value PassedCourse) MarshalJSON() ([]byte, error) {
	type plain PassedCourse
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"PassedCourse", plain(value)})
}
func DecodeProgress(data []byte) (Progress, error) {
	values, err := fields(data, "$type")
	if err != nil {
		return nil, err
	}
	var tag string
	if err := json.Unmarshal(values["$type"], &tag); err != nil {
		return nil, err
	}
	switch tag {
	case "Reserved":
		return decodeAs[Reserved](data)
	case "Studying":
		return decodeAs[Studying](data, "attendance")
	case "Passed":
		return decodeAs[Passed](data, "score", "credits")
	case "Failed":
		return decodeAs[Failed](data, "score", "reason")
	case "Withdrawn":
		return decodeAs[Withdrawn](data, "reason")
	default:
		return nil, fmt.Errorf("unknown Progress %s", tag)
	}
}
func (value Reserved) MarshalJSON() ([]byte, error) {
	type plain Reserved
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Reserved", plain(value)})
}
func (value Studying) MarshalJSON() ([]byte, error) {
	type plain Studying
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Studying", plain(value)})
}
func (value Passed) MarshalJSON() ([]byte, error) {
	type plain Passed
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Passed", plain(value)})
}
func (value Failed) MarshalJSON() ([]byte, error) {
	type plain Failed
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Failed", plain(value)})
}
func (value Withdrawn) MarshalJSON() ([]byte, error) {
	type plain Withdrawn
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Withdrawn", plain(value)})
}
func DecodeClassState(data []byte) (ClassState, error) {
	values, err := fields(data, "$type")
	if err != nil {
		return nil, err
	}
	var tag string
	if err := json.Unmarshal(values["$type"], &tag); err != nil {
		return nil, err
	}
	switch tag {
	case "Available":
		return decodeAs[Available](data, "plan", "students")
	case "Full":
		return decodeAs[Full](data, "plan", "students")
	case "Teaching":
		return decodeAs[Teaching](data, "plan", "students", "startedOn")
	case "Completed":
		return decodeAs[Completed](data, "plan", "results")
	default:
		return nil, fmt.Errorf("unknown ClassState %s", tag)
	}
}
func (value Available) MarshalJSON() ([]byte, error) {
	type plain Available
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Available", plain(value)})
}
func (value Full) MarshalJSON() ([]byte, error) {
	type plain Full
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Full", plain(value)})
}
func (value Teaching) MarshalJSON() ([]byte, error) {
	type plain Teaching
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Teaching", plain(value)})
}
func (value Completed) MarshalJSON() ([]byte, error) {
	type plain Completed
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Completed", plain(value)})
}
func DecodeCommand(data []byte) (Command, error) {
	values, err := fields(data, "$type")
	if err != nil {
		return nil, err
	}
	var tag string
	if err := json.Unmarshal(values["$type"], &tag); err != nil {
		return nil, err
	}
	switch tag {
	case "RegisterStudent":
		return decodeAs[RegisterStudent](data, "studentId", "grade")
	case "PromoteStudent":
		return decodeAs[PromoteStudent](data, "studentId", "grade")
	case "OpenClass":
		return decodeAs[OpenClass](data, "classId", "course", "capacity")
	case "Enroll":
		return decodeAs[Enroll](data, "classId", "studentId")
	case "Drop":
		return decodeAs[Drop](data, "classId", "studentId")
	case "StartClass":
		return decodeAs[StartClass](data, "classId", "startedOn")
	case "Attend":
		return decodeAs[Attend](data, "classId", "studentId")
	case "Assess":
		return decodeAs[Assess](data, "classId", "studentId", "score")
	case "CompleteClass":
		return decodeAs[CompleteClass](data, "classId")
	default:
		return nil, fmt.Errorf("unknown Command %s", tag)
	}
}
func (value RegisterStudent) MarshalJSON() ([]byte, error) {
	type plain RegisterStudent
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"RegisterStudent", plain(value)})
}
func (value PromoteStudent) MarshalJSON() ([]byte, error) {
	type plain PromoteStudent
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"PromoteStudent", plain(value)})
}
func (value OpenClass) MarshalJSON() ([]byte, error) {
	type plain OpenClass
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"OpenClass", plain(value)})
}
func (value Enroll) MarshalJSON() ([]byte, error) {
	type plain Enroll
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Enroll", plain(value)})
}
func (value Drop) MarshalJSON() ([]byte, error) {
	type plain Drop
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Drop", plain(value)})
}
func (value StartClass) MarshalJSON() ([]byte, error) {
	type plain StartClass
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"StartClass", plain(value)})
}
func (value Attend) MarshalJSON() ([]byte, error) {
	type plain Attend
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Attend", plain(value)})
}
func (value Assess) MarshalJSON() ([]byte, error) {
	type plain Assess
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Assess", plain(value)})
}
func (value CompleteClass) MarshalJSON() ([]byte, error) {
	type plain CompleteClass
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"CompleteClass", plain(value)})
}
func DecodeEvent(data []byte) (Event, error) {
	values, err := fields(data, "$type")
	if err != nil {
		return nil, err
	}
	var tag string
	if err := json.Unmarshal(values["$type"], &tag); err != nil {
		return nil, err
	}
	switch tag {
	case "StudentRegistered":
		return decodeAs[StudentRegistered](data, "studentId", "grade")
	case "StudentPromoted":
		return decodeAs[StudentPromoted](data, "studentId", "grade")
	case "ClassOpened":
		return decodeAs[ClassOpened](data, "classId", "course", "capacity")
	case "StudentEnrolled":
		return decodeAs[StudentEnrolled](data, "classId", "studentId")
	case "StudentDropped":
		return decodeAs[StudentDropped](data, "classId", "studentId")
	case "ClassStarted":
		return decodeAs[ClassStarted](data, "classId", "startedOn")
	case "AttendanceRecorded":
		return decodeAs[AttendanceRecorded](data, "classId", "studentId")
	case "ResultRecorded":
		return decodeAs[ResultRecorded](data, "classId", "studentId", "score")
	case "ClassCompleted":
		return decodeAs[ClassCompleted](data, "classId")
	default:
		return nil, fmt.Errorf("unknown Event %s", tag)
	}
}
func (value StudentRegistered) MarshalJSON() ([]byte, error) {
	type plain StudentRegistered
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"StudentRegistered", plain(value)})
}
func (value StudentPromoted) MarshalJSON() ([]byte, error) {
	type plain StudentPromoted
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"StudentPromoted", plain(value)})
}
func (value ClassOpened) MarshalJSON() ([]byte, error) {
	type plain ClassOpened
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"ClassOpened", plain(value)})
}
func (value StudentEnrolled) MarshalJSON() ([]byte, error) {
	type plain StudentEnrolled
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"StudentEnrolled", plain(value)})
}
func (value StudentDropped) MarshalJSON() ([]byte, error) {
	type plain StudentDropped
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"StudentDropped", plain(value)})
}
func (value ClassStarted) MarshalJSON() ([]byte, error) {
	type plain ClassStarted
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"ClassStarted", plain(value)})
}
func (value AttendanceRecorded) MarshalJSON() ([]byte, error) {
	type plain AttendanceRecorded
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"AttendanceRecorded", plain(value)})
}
func (value ResultRecorded) MarshalJSON() ([]byte, error) {
	type plain ResultRecorded
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"ResultRecorded", plain(value)})
}
func (value ClassCompleted) MarshalJSON() ([]byte, error) {
	type plain ClassCompleted
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"ClassCompleted", plain(value)})
}
func DecodeDecision(data []byte) (Decision, error) {
	values, err := fields(data, "$type")
	if err != nil {
		return nil, err
	}
	var tag string
	if err := json.Unmarshal(values["$type"], &tag); err != nil {
		return nil, err
	}
	switch tag {
	case "Accepted":
		return decodeAs[Accepted](data, "events")
	case "Rejected":
		return decodeAs[Rejected](data, "code")
	default:
		return nil, fmt.Errorf("unknown Decision %s", tag)
	}
}
func (value Accepted) MarshalJSON() ([]byte, error) {
	type plain Accepted
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Accepted", plain(value)})
}
func (value Rejected) MarshalJSON() ([]byte, error) {
	type plain Rejected
	return json.Marshal(struct {
		Type string `json:"$type"`
		plain
	}{"Rejected", plain(value)})
}
func decodeAs[T any](data []byte, names ...string) (T, error) {
	var value T
	if _, err := fields(data, names...); err != nil {
		return value, err
	}
	err := json.Unmarshal(data, &value)
	return value, err
}
func (value *Plan) UnmarshalJSON(data []byte) error {
	values, err := fields(data, "classId", "course", "minimumGrade", "prerequisite", "capacity")
	if err != nil {
		return err
	}
	if err := json.Unmarshal(values["classId"], &value.ClassId); err != nil {
		return err
	}
	if err := json.Unmarshal(values["course"], &value.Course); err != nil {
		return err
	}
	if err := json.Unmarshal(values["minimumGrade"], &value.MinimumGrade); err != nil {
		return err
	}
	value.Prerequisite, err = DecodePrerequisite(values["prerequisite"])
	if err != nil {
		return err
	}
	if err := json.Unmarshal(values["capacity"], &value.Capacity); err != nil {
		return err
	}
	return nil
}
func (value *Enrollment) UnmarshalJSON(data []byte) error {
	values, err := fields(data, "classId", "studentId", "course", "progress")
	if err != nil {
		return err
	}
	if err := json.Unmarshal(values["classId"], &value.ClassId); err != nil {
		return err
	}
	if err := json.Unmarshal(values["studentId"], &value.StudentId); err != nil {
		return err
	}
	if err := json.Unmarshal(values["course"], &value.Course); err != nil {
		return err
	}
	value.Progress, err = DecodeProgress(values["progress"])
	if err != nil {
		return err
	}
	return nil
}
func (value *ClassResult) UnmarshalJSON(data []byte) error {
	values, err := fields(data, "studentId", "progress")
	if err != nil {
		return err
	}
	if err := json.Unmarshal(values["studentId"], &value.StudentId); err != nil {
		return err
	}
	value.Progress, err = DecodeProgress(values["progress"])
	if err != nil {
		return err
	}
	return nil
}
func (value *State) UnmarshalJSON(data []byte) error {
	values, err := fields(data, "students", "classes", "enrollments")
	if err != nil {
		return err
	}
	if err := json.Unmarshal(values["students"], &value.Students); err != nil {
		return err
	}
	var classes []json.RawMessage
	if err := json.Unmarshal(values["classes"], &classes); err != nil {
		return err
	}
	value.Classes = make([]ClassState, 0, len(classes))
	for _, raw := range classes {
		item, err := DecodeClassState(raw)
		if err != nil {
			return err
		}
		value.Classes = append(value.Classes, item)
	}
	if err := json.Unmarshal(values["enrollments"], &value.Enrollments); err != nil {
		return err
	}
	return nil
}
func (value *Accepted) UnmarshalJSON(data []byte) error {
	values, err := fields(data, "events")
	if err != nil {
		return err
	}
	var events []json.RawMessage
	if err := json.Unmarshal(values["events"], &events); err != nil {
		return err
	}
	value.Events = make([]Event, 0, len(events))
	for _, raw := range events {
		item, err := DecodeEvent(raw)
		if err != nil {
			return err
		}
		value.Events = append(value.Events, item)
	}
	return nil
}
