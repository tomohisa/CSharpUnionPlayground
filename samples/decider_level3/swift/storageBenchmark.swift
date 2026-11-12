import Foundation
private struct DocumentRevision:Codable,Sendable{let documentId:String;let revision:Int;let body:String}
func storageBenchmark(){
 let count=Int(ProcessInfo.processInfo.environment["DECIDER_DOCUMENTS"] ?? "0") ?? 0;if count<=0{return}
 let path=ProcessInfo.processInfo.environment["DECIDER_PERSISTENT"]
 func store()->JsonEventStore<DocumentRevision>{let j:any JsonJournal;if let path=path{j=SqliteJournal(path+".documents")}else{j=MemoryJsonJournal()};return JsonEventStore(j,{e in EventDescriptor(eventType:"DocumentRevision",tags:[DcbTag(category:"document",keys:[e.documentId])])},{String(data:try! JSONEncoder().encode($0),encoding:.utf8)!},{try! JSONDecoder().decode(DocumentRevision.self,from:Data($0.utf8))})}
 let start=ProcessInfo.processInfo.systemUptime
 let projection=IndexedProjection<String,Int,DocumentRevision>{rows,e in rows.put(e.payload.documentId,e.payload.revision)}
 let s=store();s.subscribe(projection.accept);let body=String(repeating:"x",count:4096)
 for revision in 1...16{for i in 0..<count{let id=String(format:"document-%08d",i),q=Query(items:[QueryItem(tags:[DcbTag(category:"document",keys:[id])],types:[])]),r=s.read(q);precondition(r.events.count==revision-1);guard case .committed=s.append("doc-\(i)-\(revision)",q,r.after,[DocumentRevision(documentId:id,revision:revision,body:body)])else{preconditionFailure("document conflict")}}}
 let all=s.read(Query(items:[]));precondition(all.after==16*count && s.listenerFaults==0 && all.events.allSatisfy{$0.payload.body.utf8.count==4096});let latest=projection.read{Array($0.values)};precondition(latest.1.count==count && latest.1.allSatisfy{$0==16});s.close()
 if path != nil{let reopened=store(),events=reopened.read(Query(items:[])).events;precondition(events.count==16*count && events.allSatisfy{$0.payload.body.utf8.count==4096});reopened.close()}
 FileHandle.standardError.write(Data(("DOCUMENT \(ProcessInfo.processInfo.systemUptime-start) \(count) \(all.after) \(s.encoded) \(s.decoded)\n").utf8))
}
