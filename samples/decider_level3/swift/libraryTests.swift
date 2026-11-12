import Foundation
import Dispatch
// Not Codable and not a school event: conditional Codable must stay at the boundary.
private struct Counter:Sendable{let key:String;let delta:Int}
private func counterTag(_ key:String)->DcbTag{DcbTag(category:"counter",keys:[key])}
private func counterQuery(_ key:String)->Query{Query(items:[QueryItem(tags:[counterTag(key)],types:[])])}
private func describeCounter(_ e:Counter)->EventDescriptor{EventDescriptor(eventType:"CounterDelta",tags:[counterTag(e.key),DcbTag(category:"scope",keys:["test"])])}
private final class Wins:@unchecked Sendable{
    private let gate=NSLock();private var value=0
    func add(){gate.lock();value+=1;gate.unlock()}
    func get()->Int{gate.lock();defer{gate.unlock()};return value}
}
private struct CounterHandler:CommandHandler{
    func validate(_ n:Int)->[String]{n>0 ? []:["positive delta required"]}
    func queryFor(_ n:Int)->Query{precondition(n>0);return counterQuery("executor")}
    func handle(_ events:[Recorded<Counter>],_ n:Int)->Preparation<Counter,String>{.emit([Counter(key:"executor",delta:n)])}
}
func libraryTests(){
    let indexed=IndexedProjection<String,Int,Counter>{tx,e in if e.payload.delta<0{throw IndexFailure.invalid};tx.put(e.payload.key,(tx.find(e.payload.key) ?? 0)+e.payload.delta)}
    func item(_ n:Int,_ delta:Int)->Recorded<Counter>{Recorded(position:n,commandId:"index",eventType:"CounterDelta",tags:[],payload:Counter(key:"x",delta:delta))}
    precondition(indexed.accept([item(1,3)]) && indexed.read{$0["x"] ?? 0}.1==3);let indexWork=indexed.work
    precondition(!indexed.accept([item(2,4),item(3,-1)]) && indexed.checkpoint==1 && indexed.work==indexWork && indexed.read{$0["x"] ?? 0}.1==3)
    precondition(!indexed.accept([item(2,4),item(4,5)]) && indexed.checkpoint==1);precondition(indexed.accept([item(1,3)]) && indexed.work==indexWork)

    let executorStore=EventStore(describe:describeCounter,copyPayload:{$0}),executor=DcbExecutor(store:executorStore)
    precondition(executor.execute("valid",5,CounterHandler()).appended==1)
    precondition(executor.execute("invalid",0,CounterHandler()).status=="Rejected" && executorStore.read(Query(items:[])).after==1)
    precondition(counterTag("\u{00e9}") != counterTag("e\u{0301}"))
    let store=EventStore(describe:describeCounter,copyPayload:{$0}),p=LiveProjection<Counter,Int>(0,{$0+$1.delta},{$0});store.subscribe(p.accept)
    store.subscribe{$0.isEmpty} // A reported consumer failure must not turn commit into failure.
    let first=store.append("first",counterQuery("a"),0,[Counter(key:"a",delta:1),Counter(key:"a",delta:2)])
    guard case .committed(let events)=first else{preconditionFailure("unexpected conflict")}
    precondition(events.count==2&&store.listenerFaults==1&&p.snapshot().0==2&&p.snapshot().1==3)
    guard case .committed=store.append("other",counterQuery("b"),0,[Counter(key:"b",delta:9)])else{preconditionFailure("unrelated conflict")}
    guard case .conflict=store.append("stale",counterQuery("a"),0,[Counter(key:"a",delta:99)])else{preconditionFailure("stale write accepted")}
    let all=store.read(Query(items:[]));precondition(all.after==3&&p.snapshot().1==12)
    precondition(store.read(Query(items:[QueryItem(tags:[counterTag("a"),DcbTag(category:"scope",keys:["test"])],types:[])])).events.count==2)
    precondition(store.read(Query(items:counterQuery("a").items+counterQuery("b").items)).events.count==3)
    precondition(store.read(Query(items:[QueryItem(tags:[counterTag("a")],types:["OtherType"])])).events.isEmpty)
    precondition(p.accept(all.events)&&p.snapshot().1==12)
    let gap=LiveProjection<Counter,Int>(0,{$0+$1.delta},{$0});precondition(!gap.accept([all.events[0],all.events[2]])&&gap.snapshot().0==0&&gap.snapshot().1==0)
    let late=LiveProjection<Counter,Int>(0,{$0+$1.delta},{$0});store.subscribe(late.accept);precondition(late.snapshot().1==12)
    let processor=CommandProcessor<Counter,Counter,String>(store:store,queryFor:{counterQuery($0.key)},decide:{_,c in c.delta>0 ? .emit([c]) : .reject("NegativeDelta")},maxConflicts:3)
    let ticket=processor.prepare("retry",Counter(key:"a",delta:1));_ = store.append("interfere",counterQuery("a"),3,[Counter(key:"a",delta:1)])
    let result=processor.commit(ticket);precondition(result.status=="Committed"&&result.conflicts==1&&p.snapshot().0==5&&p.snapshot().1==14&&late.snapshot().1==14)
    precondition(processor.commit(processor.prepare("reject",Counter(key:"a",delta:-1))).status=="Rejected")
    let restored=EventStore(describe:describeCounter,copyPayload:{$0});restored.load(store.read(Query(items:[])).events);precondition(restored.read(Query(items:[])).after==5)
    let racing=EventStore(describe:describeCounter,copyPayload:{$0}),ready=DispatchGroup(),done=DispatchGroup(),start=DispatchSemaphore(value:0),wins=Wins()
    for i in 0..<2{ready.enter();done.enter();DispatchQueue.global().async{
        let read=racing.read(counterQuery("a"));ready.leave();start.wait()
        if case .committed=racing.append("race-\(i)",counterQuery("a"),read.after,[Counter(key:"a",delta:1)]){wins.add()}
        done.leave()
    }}
    ready.wait();start.signal();start.signal();done.wait();precondition(wins.get()==1&&racing.read(Query(items:[])).after==1)
}
