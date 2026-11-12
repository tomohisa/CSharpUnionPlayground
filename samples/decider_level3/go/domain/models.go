package domain

type Grade string

const (
	Year1 Grade = "Year1"
	Year2 Grade = "Year2"
	Year3 Grade = "Year3"
)

type Course string

const (
	Foundation Course = "Foundation"
	Advanced   Course = "Advanced"
)

type FailureReason string

const (
	BelowPassMark FailureReason = "BelowPassMark"
)

type WithdrawalReason string

const (
	StudentRequest WithdrawalReason = "StudentRequest"
)

type Rejection string

const (
	InvalidId           Rejection = "InvalidId"
	InvalidCapacity     Rejection = "InvalidCapacity"
	InvalidScore        Rejection = "InvalidScore"
	InvalidDate         Rejection = "InvalidDate"
	DuplicateStudent    Rejection = "DuplicateStudent"
	UnknownStudent      Rejection = "UnknownStudent"
	InvalidPromotion    Rejection = "InvalidPromotion"
	DuplicateClass      Rejection = "DuplicateClass"
	UnknownClass        Rejection = "UnknownClass"
	GradeTooLow         Rejection = "GradeTooLow"
	PrerequisiteMissing Rejection = "PrerequisiteMissing"
	AlreadyEnrolled     Rejection = "AlreadyEnrolled"
	ClassFull           Rejection = "ClassFull"
	RegistrationClosed  Rejection = "RegistrationClosed"
	NotEnrolled         Rejection = "NotEnrolled"
	NotTeaching         Rejection = "NotTeaching"
	NoAttendance        Rejection = "NoAttendance"
	AlreadyAssessed     Rejection = "AlreadyAssessed"
	UnassessedStudents  Rejection = "UnassessedStudents"
	InvalidHistory      Rejection = "InvalidHistory"
    InvalidCommand Rejection = "InvalidCommand"
)

type Student struct {
	StudentId string `json:"studentId"`
	Grade     Grade  `json:"grade"`
}
type Plan struct {
	ClassId      string       `json:"classId"`
	Course       Course       `json:"course"`
	MinimumGrade Grade        `json:"minimumGrade"`
	Prerequisite Prerequisite `json:"prerequisite"`
	Capacity     int          `json:"capacity"`
}
type Enrollment struct {
	ClassId   string   `json:"classId"`
	StudentId string   `json:"studentId"`
	Course    Course   `json:"course"`
	Progress  Progress `json:"progress"`
}
type ClassResult struct {
	StudentId string   `json:"studentId"`
	Progress  Progress `json:"progress"`
}
type DecisionModel struct {
	Students    []Student    `json:"students"`
	Classes     []ClassState `json:"classes"`
	Enrollments []Enrollment `json:"enrollments"`
}
type SchoolList struct {
	Students    []Student    `json:"students"`
	Classes     []ClassState `json:"classes"`
	Enrollments []Enrollment `json:"enrollments"`
	Summaries   []string     `json:"summaries"`
}
type Trace struct {
	Operation string      `json:"operation"`
	Status    string      `json:"status"`
	Conflicts int         `json:"conflicts"`
	Appended  int         `json:"appended"`
	Position  int         `json:"position"`
	Errors    []Rejection `json:"errors"`
}
type Prerequisite interface{ isPrerequisite() }
type NoPrerequisite struct {
}

func (NoPrerequisite) isPrerequisite() {}

type PassedCourse struct {
	Course Course `json:"course"`
}

func (PassedCourse) isPrerequisite() {}

type Progress interface{ isProgress() }
type Reserved struct {
}

func (Reserved) isProgress() {}

type Studying struct {
	Attendance int `json:"attendance"`
}

func (Studying) isProgress() {}

type Passed struct {
	Score   int `json:"score"`
	Credits int `json:"credits"`
}

func (Passed) isProgress() {}

type Failed struct {
	Score  int           `json:"score"`
	Reason FailureReason `json:"reason"`
}

func (Failed) isProgress() {}

type Withdrawn struct {
	Reason WithdrawalReason `json:"reason"`
}

func (Withdrawn) isProgress() {}

type ClassState interface{ isClassState() }
type Available struct {
	Plan     Plan     `json:"plan"`
	Students []string `json:"students"`
}

func (Available) isClassState() {}

type Full struct {
	Plan     Plan     `json:"plan"`
	Students []string `json:"students"`
}

func (Full) isClassState() {}

type Teaching struct {
	Plan      Plan     `json:"plan"`
	Students  []string `json:"students"`
	StartedOn string   `json:"startedOn"`
}

func (Teaching) isClassState() {}

type Completed struct {
	Plan    Plan          `json:"plan"`
	Results []ClassResult `json:"results"`
}

func (Completed) isClassState() {}

type Command interface{ isCommand() }
type RegisterStudent struct {
	StudentId string `json:"studentId"`
	Grade     Grade  `json:"grade"`
}

func (RegisterStudent) isCommand() {}

type PromoteStudent struct {
	StudentId string `json:"studentId"`
	Grade     Grade  `json:"grade"`
}

func (PromoteStudent) isCommand() {}

type OpenClass struct {
	ClassId  string `json:"classId"`
	Course   Course `json:"course"`
	Capacity int    `json:"capacity"`
}

func (OpenClass) isCommand() {}

type Enroll struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
}

func (Enroll) isCommand() {}

type Drop struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
}

func (Drop) isCommand() {}

type StartClass struct {
	ClassId   string `json:"classId"`
	StartedOn string `json:"startedOn"`
}

func (StartClass) isCommand() {}

type Attend struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
}

func (Attend) isCommand() {}

type Assess struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
	Score     int    `json:"score"`
}

func (Assess) isCommand() {}

type CompleteClass struct {
	ClassId string `json:"classId"`
}

func (CompleteClass) isCommand() {}

type Event interface{ isEvent() }
type StudentRegistered struct {
	StudentId string `json:"studentId"`
	Grade     Grade  `json:"grade"`
}

func (StudentRegistered) isEvent() {}

type StudentPromoted struct {
	StudentId string `json:"studentId"`
	Grade     Grade  `json:"grade"`
}

func (StudentPromoted) isEvent() {}

type ClassOpened struct {
	ClassId  string `json:"classId"`
	Course   Course `json:"course"`
	Capacity int    `json:"capacity"`
}

func (ClassOpened) isEvent() {}

type StudentEnrolled struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
	Course    Course `json:"course"`
}

func (StudentEnrolled) isEvent() {}

type StudentDropped struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
}

func (StudentDropped) isEvent() {}

type ClassStarted struct {
	ClassId   string `json:"classId"`
	StartedOn string `json:"startedOn"`
}

func (ClassStarted) isEvent() {}

type AttendanceRecorded struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
}

func (AttendanceRecorded) isEvent() {}

type ResultRecorded struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
	Score     int    `json:"score"`
}

func (ResultRecorded) isEvent() {}

type ClassCompleted struct {
	ClassId string        `json:"classId"`
	Results []ClassResult `json:"results"`
}

func (ClassCompleted) isEvent() {}

type StudyStarted struct {
	ClassId   string `json:"classId"`
	StudentId string `json:"studentId"`
}

func (StudyStarted) isEvent() {}

type Decision interface{ isDecision() }
type Accepted struct {
	Events []Event `json:"events"`
}

func (Accepted) isDecision() {}

type Rejected struct {
	Code Rejection `json:"code"`
}

func (Rejected) isDecision() {}

type Operation interface{ isOperation() }
type Execute struct {
	CommandId string  `json:"commandId"`
	Command   Command `json:"command"`
}

func (Execute) isOperation() {}

type Prepare struct {
	Key       string  `json:"key"`
	CommandId string  `json:"commandId"`
	Command   Command `json:"command"`
}

func (Prepare) isOperation() {}

type Commit struct {
	Key string `json:"key"`
}

func (Commit) isOperation() {}

type ListAll struct {
}

func (ListAll) isOperation() {}

type Rebuild struct {
}

func (Rebuild) isOperation() {}

type Redeliver struct {
}

func (Redeliver) isOperation() {}
