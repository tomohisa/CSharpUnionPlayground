import Foundation
struct ReadAudit:Codable {let query:Query;let after:Int;let returned:Int}
struct RunOutput:Codable {let events:[Recorded<Event>];let steps:[Trace];let lists:[SchoolList];let list:SchoolList;let rebuilt:SchoolList;let checkpoint:Int;let reads:[ReadAudit];let listenerFaults:Int}
func runSchool(_ history:[Recorded<Event>],_ operations:[Operation])->RunOutput{
    let store=EventStore<Event>(describe:describeEvent,copyPayload:{$0})
    store.load(history);let projection=SchoolProjection();store.subscribe(projection.accept)
    let executor=DcbExecutor(store:store),handler=SchoolHandler()
    var tickets:[String:PreparedCommand<Rejection>]=[:],steps:[Trace]=[],lists:[SchoolList]=[],reads:[ReadAudit]=[]
    func prepare(_ id:String,_ c:Command)->PreparedCommand<Rejection>{let t=executor.prepare(id,c,handler);if t.wasRead{reads.append(ReadAudit(query:t.query,after:t.after,returned:t.returned))};return t}
    func commit(_ op:String,_ t:PreparedCommand<Rejection>){let r=t.commit();steps.append(Trace(operation:op,status:r.status,conflicts:r.conflicts,appended:r.appended,position:r.position,errors:r.errors))}
    for operation in operations{switch operation{
    case .execute(let id,let c):commit("Execute",prepare(id,c))
    case .prepare(let key,let id,let c):precondition(tickets[key] == nil,"duplicate ticket");let t=prepare(id,c);tickets[key]=t;steps.append(Trace(operation:"Prepare",status:"Prepared",conflicts:0,appended:0,position:t.after,errors:[]))
    case .commit(let key):guard let t=tickets.removeValue(forKey:key) else{preconditionFailure("unknown ticket")};commit("Commit",t)
    case .listAll:precondition(store.listenerFaults == 0,"projection unavailable; rebuild required");let(position,s)=projection.snapshot();lists.append(s);steps.append(Trace(operation:"ListAll",status:"Listed",conflicts:0,appended:0,position:position,errors:[]))
    case .rebuild:let fresh=SchoolProjection();precondition(fresh.accept(store.read(Query(items:[])).events));let(position,s)=fresh.snapshot();lists.append(s);steps.append(Trace(operation:"Rebuild",status:"Rebuilt",conflicts:0,appended:0,position:position,errors:[]))
    case .redeliver:precondition(projection.accept(store.read(Query(items:[])).events));steps.append(Trace(operation:"Redeliver",status:"Redelivered",conflicts:0,appended:0,position:projection.checkpoint,errors:[]))
    }}
    let events=store.read(Query(items:[])).events,fresh=SchoolProjection();precondition(fresh.accept(events));let(position,s)=projection.snapshot()
    precondition(store.listenerFaults == 0,"projection unavailable; rebuild required")
    return RunOutput(events:events,steps:steps,lists:lists,list:s,rebuilt:fresh.snapshot().1,checkpoint:position,reads:reads,listenerFaults:store.listenerFaults)
}
