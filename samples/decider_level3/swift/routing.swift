import Foundation
func studentTag(_ id:String)->DcbTag{DcbTag(category:"student",keys:[id])}
func classTag(_ id:String)->DcbTag{DcbTag(category:"class",keys:[id])}
func enrollmentTag(_ c:String,_ s:String)->DcbTag{DcbTag(category:"enrollment",keys:[c,s])}
func one(_ tags:DcbTag...)->Query{Query(items:tags.map{QueryItem(tags:[$0],types:[])})}
func queryFor(_ command:Command)->Query{switch command{
case .registerStudent(let studentId,_):return one(studentTag(studentId))
case .promoteStudent(let studentId,_):return one(studentTag(studentId))
case .openClass(let classId,_,_):return one(classTag(classId))
case .enroll(let classId,let studentId):return one(studentTag(studentId),classTag(classId))
case .drop(let classId,let studentId):return one(classTag(classId))
case .startClass(let classId,_):return one(classTag(classId))
case .attend(let classId,let studentId):return one(classTag(classId))
case .assess(let classId,let studentId,_):return one(classTag(classId))
case .completeClass(let classId):return one(classTag(classId))
}}
func describeEvent(_ event:Event)->EventDescriptor{
let type:String;let tags:[DcbTag]
switch event{
case .studentRegistered(let studentId,_):type="StudentRegistered";tags=[studentTag(studentId)]
case .studentPromoted(let studentId,_):type="StudentPromoted";tags=[studentTag(studentId)]
case .classOpened(let classId,_,_):type="ClassOpened";tags=[classTag(classId)]
case .studentEnrolled(let classId,let studentId,_):type="StudentEnrolled";tags=[studentTag(studentId),classTag(classId),enrollmentTag(classId,studentId)]
case .studentDropped(let classId,let studentId):type="StudentDropped";tags=[studentTag(studentId),classTag(classId),enrollmentTag(classId,studentId)]
case .classStarted(let classId,_):type="ClassStarted";tags=[classTag(classId)]
case .attendanceRecorded(let classId,let studentId):type="AttendanceRecorded";tags=[studentTag(studentId),classTag(classId),enrollmentTag(classId,studentId)]
case .resultRecorded(let classId,let studentId,_):type="ResultRecorded";tags=[studentTag(studentId),classTag(classId),enrollmentTag(classId,studentId)]
case .classCompleted(let classId,_):type="ClassCompleted";tags=[classTag(classId)]
case .studyStarted(let classId,let studentId):type="StudyStarted";tags=[studentTag(studentId),classTag(classId),enrollmentTag(classId,studentId)]
}
return EventDescriptor(eventType:type,tags:tags)
}
func prepareDecision(_ events:[Recorded<Event>],_ command:Command)->Preparation<Event,Rejection>{
switch decide(fold(events).decisionModel(),command){case .accepted(let events):return .emit(events);case .rejected(let code):return .reject(code)}
}
