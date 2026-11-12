import Foundation
func queryBenchmark(_ store:JsonEventStore<Event>,_ projection:SchoolProjection){let n=Int(ProcessInfo.processInfo.environment["DECIDER_STUDENTS"] ?? "0") ?? 0;if n<=0{return};var start=ProcessInfo.processInfo.systemUptime
 for i in 0..<1000{let id=String(format:"student-%08d",i*7919%n);precondition(!store.read(Query(items:[QueryItem(tags:[DcbTag(category:"student",keys:[id])],types:[])])).events.isEmpty)}
 FileHandle.standardError.write(Data(("PHASE tagged_point \(ProcessInfo.processInfo.systemUptime-start)\n").utf8));start=ProcessInfo.processInfo.systemUptime
 for i in 0..<1000{let id=String(format:"student-%08d",i*7919%n),s=try! JSONDecoder().decode(Student.self,from:JSONEncoder().encode(projection.student(id)));precondition(s.studentId==id)}
 FileHandle.standardError.write(Data(("PHASE projection_point_json \(ProcessInfo.processInfo.systemUptime-start)\n").utf8))
}
