import Foundation
public struct DcbTag: Codable, Equatable, Sendable { public let category:String;public let keys:[String];public static func ==(a:DcbTag,b:DcbTag)->Bool{a.category.unicodeScalars.elementsEqual(b.category.unicodeScalars) && a.keys.count == b.keys.count && zip(a.keys,b.keys).allSatisfy{$0.unicodeScalars.elementsEqual($1.unicodeScalars)}};public init(category:String,keys:[String]){self.category=category;self.keys=keys} }
public struct QueryItem: Codable, Sendable { public let tags:[DcbTag];public let types:[String];public init(tags:[DcbTag],types:[String]){self.tags=tags;self.types=types} }
public struct Query: Codable, Sendable {
    public let items:[QueryItem]
    public init(items:[QueryItem]){self.items=items}
    public func matches(_ type: String, _ tags: [DcbTag]) -> Bool {
        items.isEmpty || items.contains { ($0.types.isEmpty || $0.types.contains(type)) && $0.tags.allSatisfy { tags.contains($0) } }
    }
}
public struct EventDescriptor: Sendable {public let eventType:String;public let tags:[DcbTag];public init(eventType:String,tags:[DcbTag]){self.eventType=eventType;self.tags=tags}}
public struct Recorded<E:Sendable>:Sendable {public let position:Int;public let commandId:String;public let eventType:String;public let tags:[DcbTag];public let payload:E;public init(position:Int,commandId:String,eventType:String,tags:[DcbTag],payload:E){self.position=position;self.commandId=commandId;self.eventType=eventType;self.tags=tags;self.payload=payload}}
extension Recorded: Codable where E: Codable {}
public struct ReadResult<E:Sendable>{public let after:Int;public let events:[Recorded<E>];public init(after:Int,events:[Recorded<E>]){self.after=after;self.events=events}}
public enum AppendResult<E: Sendable> { case committed([Recorded<E>]); case conflict }
public protocol CommandStore<Payload>: Sendable {
    associatedtype Payload: Sendable
    func read(_ query:Query)->ReadResult<Payload>
    func append(_ commandId:String,_ condition:Query,_ after:Int,_ batch:[Payload])->AppendResult<Payload>
}
// The lock covers check-and-append and synchronous notification. Listeners are
// non-reentrant consumers and return false to report a delivery failure.
public final class EventStore<E: Sendable>: @unchecked Sendable, CommandStore {
    public typealias Payload=E
    private let gate = NSLock()
    private var log: [Recorded<E>] = []
    private var listeners: [@Sendable ([Recorded<E>]) -> Bool] = []
    private let copyPayload: @Sendable (E) -> E
    private var faults = 0
    public var listenerFaults:Int{gate.lock();defer{gate.unlock()};return faults}
    private let describe: @Sendable (E)->EventDescriptor
    public init(describe: @escaping @Sendable (E)->EventDescriptor,copyPayload: @escaping @Sendable (E) -> E) {self.describe=describe;self.copyPayload=copyPayload}
    private func copy(_ e: Recorded<E>) -> Recorded<E> { Recorded(position: e.position, commandId: e.commandId, eventType: e.eventType, tags: e.tags, payload: copyPayload(e.payload)) }
    public func read(_ query: Query) -> ReadResult<E> {
        gate.lock(); defer { gate.unlock() }
        return ReadResult(after: log.count, events: log.filter { query.matches($0.eventType,$0.tags) }.map(copy))
    }
    public func append(_ commandId: String, _ condition: Query, _ after: Int, _ batch: [E]) -> AppendResult<E> {
        gate.lock(); defer { gate.unlock() }
        precondition(!batch.isEmpty && !commandId.isEmpty && after >= 0 && after <= log.count, "invalid append")
        if log.contains(where: { $0.position > after && condition.matches($0.eventType,$0.tags) }) { return .conflict }
        let committed = batch.enumerated().map { i,payload in
            let copied=copyPayload(payload),e=describe(copied)
            precondition(!e.eventType.isEmpty, "event type required")
            return Recorded(position: log.count+i+1, commandId: commandId, eventType: e.eventType, tags: e.tags, payload: copied)
        }
        log.append(contentsOf: committed)
        listeners = listeners.filter { listener in let ok=listener(committed.map(copy)); if !ok { faults += 1 }; return ok }
        return .committed(committed.map(copy))
    }
    public func subscribe(_ listener: @escaping @Sendable ([Recorded<E>]) -> Bool) {
        gate.lock(); defer { gate.unlock() }
        precondition(listener(log.map(copy)), "subscription replay failed"); listeners.append(listener)
    }
    private func valid(_ e:Recorded<E>)->Bool{let d=describe(e.payload);return d.eventType == e.eventType && d.tags.count == e.tags.count && d.tags.allSatisfy{e.tags.contains($0)}}
    public func load(_ events: [Recorded<E>]) {
        gate.lock(); defer { gate.unlock() }
        precondition(log.isEmpty && listeners.isEmpty, "load only into an empty store")
        precondition(events.enumerated().allSatisfy { $1.position == $0+1 && !$1.commandId.isEmpty && !$1.eventType.isEmpty && valid($1) }, "invalid imported log")
        log = events.map(copy)
    }
}
public final class LiveProjection<E: Sendable,S: Sendable>: @unchecked Sendable {
    private let gate=NSLock()
    private var state: S
    private var checkpoint=0
    private let reduce: @Sendable (S,E) -> S
    private let copyState: @Sendable (S) -> S
    public init(_ initial: S, _ reduce: @escaping @Sendable (S,E)->S, _ copyState: @escaping @Sendable (S)->S) { state=initial;self.reduce=reduce;self.copyState=copyState }
    public func accept(_ batch: [Recorded<E>]) -> Bool {
        gate.lock(); defer {gate.unlock()}
        var next=copyState(state), position=checkpoint
        for e in batch {if e.position<=position {continue};if e.position != position+1 {return false};next=reduce(next,e.payload);position=e.position}
        state=next;checkpoint=position;return true
    }
    public func snapshot() -> (Int,S) {gate.lock();defer {gate.unlock()};return (checkpoint,copyState(state))}
}
public enum Preparation<E: Sendable,R> { case emit([E]); case reject(R) }
public struct Ticket<C,E:Sendable,R>{public let commandId:String;public let command:C;public let query:Query;public let after:Int;public let returned:Int;public let decision:Preparation<E,R>;public init(commandId:String,command:C,query:Query,after:Int,returned:Int,decision:Preparation<E,R>){self.commandId=commandId;self.command=command;self.query=query;self.after=after;self.returned=returned;self.decision=decision}}
public struct HandlingResult<R>{public let status:String;public let conflicts:Int;public let appended:Int;public let position:Int;public let errors:[R];public init(status:String,conflicts:Int,appended:Int,position:Int,errors:[R]){self.status=status;self.conflicts=conflicts;self.appended=appended;self.position=position;self.errors=errors}}
public struct CommandProcessor<C,E: Sendable,R> {
    public let store:any CommandStore<E>
    public let queryFor:(C)->Query
    public let decide:([Recorded<E>],C)->Preparation<E,R>
    public let maxConflicts:Int
    public init(store:any CommandStore<E>,queryFor:@escaping(C)->Query,decide:@escaping([Recorded<E>],C)->Preparation<E,R>,maxConflicts:Int){self.store=store;self.queryFor=queryFor;self.decide=decide;self.maxConflicts=maxConflicts}
    public func prepare(_ id:String,_ command:C)->Ticket<C,E,R> {
        let query=queryFor(command), read=store.read(query)
        return Ticket(commandId:id,command:command,query:query,after:read.after,returned:read.events.count,decision:decide(read.events,command))
    }
    public func commit(_ original:Ticket<C,E,R>)->HandlingResult<R> {
        var ticket=original, conflicts=0
        while true {
            switch ticket.decision {
            case .reject(let error): return HandlingResult(status:"Rejected",conflicts:conflicts,appended:0,position:ticket.after,errors:[error])
            case .emit(let events):
                switch store.append(ticket.commandId,ticket.query,ticket.after,events) {
                case .committed(let events): return HandlingResult(status:"Committed",conflicts:conflicts,appended:events.count,position:events.last!.position,errors:[])
                case .conflict: conflicts+=1
                }
            }
            if conflicts>=maxConflicts {return HandlingResult(status:"RetryExhausted",conflicts:conflicts,appended:0,position:ticket.after,errors:[])}
            ticket=prepare(ticket.commandId,ticket.command)
        }
    }
}

public protocol CommandHandler {
    associatedtype CommandInput
    associatedtype EventOutput:Sendable
    associatedtype Rejection
    func validate(_ command:CommandInput)->[Rejection]
    func queryFor(_ command:CommandInput)->Query
    func handle(_ events:[Recorded<EventOutput>],_ command:CommandInput)->Preparation<EventOutput,Rejection>
}
public struct PreparedCommand<R> {
    public let query:Query
    public let after:Int
    public let returned:Int
    public let wasRead:Bool
    private let action:()->HandlingResult<R>
    public init(query:Query,after:Int,returned:Int,wasRead:Bool,action:@escaping()->HandlingResult<R>){self.query=query;self.after=after;self.returned=returned;self.wasRead=wasRead;self.action=action}
    public func commit()->HandlingResult<R>{action()}
}
public protocol DcbExecutorApi {
    associatedtype Payload:Sendable
    func prepare<H:CommandHandler>(_ id:String,_ command:H.CommandInput,_ handler:H)->PreparedCommand<H.Rejection> where H.EventOutput == Payload
}
public struct DcbExecutor<E:Sendable>:DcbExecutorApi {
    public typealias Payload=E
    public let store:any CommandStore<E>
    public init(store:any CommandStore<E>){self.store=store}
    public func prepare<H:CommandHandler>(_ id:String,_ command:H.CommandInput,_ handler:H)->PreparedCommand<H.Rejection> where H.EventOutput == E {
        precondition(!id.trimmingCharacters(in:.whitespacesAndNewlines).isEmpty,"command id required")
        let errors=handler.validate(command)
        if !errors.isEmpty{return PreparedCommand(query:Query(items:[]),after:0,returned:0,wasRead:false){HandlingResult(status:"Rejected",conflicts:0,appended:0,position:0,errors:errors)}}
        let processor=CommandProcessor(store:store,queryFor:handler.queryFor,decide:handler.handle,maxConflicts:3)
        let ticket=processor.prepare(id,command)
        return PreparedCommand(query:ticket.query,after:ticket.after,returned:ticket.returned,wasRead:true){processor.commit(ticket)}
    }
    public func execute<H:CommandHandler>(_ id:String,_ command:H.CommandInput,_ handler:H)->HandlingResult<H.Rejection> where H.EventOutput == E {prepare(id,command,handler).commit()}
}

public final class IndexBatch<K:Hashable,V>{
    private let findBase:(K)->V?
    fileprivate var changes:[K:IndexEdit<V>]=[:]
    fileprivate var work:Int64=0
    fileprivate init(_ find:@escaping(K)->V?){findBase=find}
    public func find(_ key:K)->V?{work+=1;if let e=changes[key]{switch e{case .put(let v):return v;case .remove:return nil}};return findBase(key)}
    public func get(_ key:K)throws->V{guard let value=find(key)else{throw IndexFailure.missing};return value}
    public func put(_ key:K,_ value:V){work+=1;changes[key] = .put(value)}
    public func delete(_ key:K){work+=1;changes[key] = .remove}
}
fileprivate enum IndexEdit<V>{case put(V);case remove}
public enum IndexFailure:Error{case missing;case invalid}
public final class IndexedProjection<K:Hashable & Sendable,V:Sendable,E:Sendable>:@unchecked Sendable{
    private let gate=NSLock()
    private var rows:[K:V]=[:]
    private var position=0
    private var operations:Int64=0
    private let reduce:(IndexBatch<K,V>,Recorded<E>)throws->Void
    public init(_ reduce:@escaping(IndexBatch<K,V>,Recorded<E>)throws->Void){self.reduce=reduce}
    public var checkpoint:Int{gate.lock();defer{gate.unlock()};return position}
    public var rowCount:Int{gate.lock();defer{gate.unlock()};return rows.count}
    public var work:Int64{gate.lock();defer{gate.unlock()};return operations}
    public func accept(_ events:[Recorded<E>])->Bool{
        gate.lock();defer{gate.unlock()}
        let batch=IndexBatch<K,V>({self.rows[$0]});var next=position
        do{for e in events{if e.position<=next{continue};if e.position != next+1{return false};try reduce(batch,e);next=e.position}}
        catch{return false}
        for(key,edit)in batch.changes{switch edit{case .put(let value):rows[key]=value;case .remove:rows.removeValue(forKey:key)}}
        position=next;operations+=batch.work;return true
    }
    public func read<T>(_ query:([K:V])->T)->(Int,T){gate.lock();defer{gate.unlock()};return(position,query(rows))}
}
