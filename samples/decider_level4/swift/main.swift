import CoreFoundation
import Foundation
import NIOCore
import NIOHTTP1
import NIOPosix

struct HttpError: Error {
  let code: Int
  let message: String
}
func bad() throws -> Never { throw HttpError(code: 400, message: "InvalidRequest") }
func validate(_ body: Any, _ schema: [String: Any], _ schemas: [String: Any]) throws {
  if let ref = schema["$ref"] as? String {
    try validate(body, schemas[String(ref.split(separator: "/").last!)] as! [String: Any], schemas)
    return
  }
  if let choices = schema["oneOf"] as? [[String: Any]] {
    guard let obj = body as? [String: Any], let kind = obj["$type"] as? String,
      choices.contains(where: { $0["$ref"] as? String == "#/components/schemas/" + kind })
    else { try bad() }
    try validate(body, schemas[kind] as! [String: Any], schemas)
    return
  }
  switch schema["type"] as? String {
  case "object":
    guard let obj = body as? [String: Any], let properties = schema["properties"] as? [String: Any]
    else { try bad() }
    for key in schema["required"] as! [String] { if obj[key] == nil { try bad() } }
    for (key, value) in obj {
      guard let child = properties[key] as? [String: Any] else { try bad() }
      try validate(value, child, schemas)
    }
  case "string":
    guard let s = body as? String else { try bad() }
    if let c = schema["const"] as? String, c != s { try bad() }
    if let values = schema["enum"] as? [String], !values.contains(s) { try bad() }
    if schema["minLength"] != nil
      && (s.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty || s.unicodeScalars.count > 128)
    {
      try bad()
    }
  case "integer":
    guard let n = body as? NSNumber, CFGetTypeID(n) != CFBooleanGetTypeID(),
      n.doubleValue.rounded() == n.doubleValue, n.doubleValue >= (-2_147_483_648),
      n.doubleValue <= 2_147_483_647
    else { try bad() }
  default: try bad()
  }
}
final class Engine: @unchecked Sendable {
  let store: JsonEventStore<Event>
  let projection = SchoolProjection()
  let schemas: [String: Any]
  let ticketGate = NSLock()
  var tickets: [String: (TimeInterval, PreparedCommand<Rejection>)] = [:]
  let ttl = Double(ProcessInfo.processInfo.environment["TICKET_TTL_SECONDS"] ?? "60")!
  init() throws {
    let spec =
      try JSONSerialization.jsonObject(
        with: Data(
          contentsOf: URL(fileURLWithPath: ProcessInfo.processInfo.environment["API_SPEC"]!)))
      as! [String: Any]
    schemas = (spec["components"] as! [String: Any])["schemas"] as! [String: Any]
    let journal: any JsonJournal
    if let path = ProcessInfo.processInfo.environment["DECIDER_PERSISTENT"] {
      journal = SqliteJournal(path)
    } else {
      journal = MemoryJsonJournal()
    }
    store = JsonEventStore(
      journal, { describeEvent($0) },
      { String(decoding: try! JSONEncoder().encode($0), as: UTF8.self) },
      { try! JSONDecoder().decode(Event.self, from: Data($0.utf8)) })
    store.subscribe(projection.accept)
  }
  func expire() {
    let now = ProcessInfo.processInfo.systemUptime
    for (id, t) in tickets where now - t.0 >= ttl { tickets.removeValue(forKey: id) }
  }
  func commit(_ t: PreparedCommand<Rejection>) throws -> (Int, Data) {
    let r = t.commit()
    let errors = try JSONSerialization.jsonObject(with: JSONEncoder().encode(r.errors))
    return (
      r.status == "Committed" ? 200 : r.status == "Rejected" ? 422 : 409,
      try JSONSerialization.data(withJSONObject: [
        "status": r.status, "conflicts": r.conflicts, "appended": r.appended,
        "position": r.position, "projectionPosition": projection.checkpoint, "errors": errors,
      ])
    )
  }
  // NIOThreadPool workers outlive each request. Drain Foundation temporaries
  // per request on Apple platforms rather than retaining them until thread exit.
  func request(_ method: String, _ uri: String, _ content: String, _ raw: Data) -> (Int, Data) {
    #if canImport(ObjectiveC)
      return autoreleasepool { handle(method, uri, content, raw) }
    #else
      return handle(method, uri, content, raw)
    #endif
  }
  func handle(_ method: String, _ uri: String, _ content: String, _ raw: Data) -> (Int, Data) {
    do { return try route(method, uri, content, raw) } catch let e as HttpError {
      return (e.code, try! JSONSerialization.data(withJSONObject: ["error": e.message]))
    } catch {
      FileHandle.standardError.write(Data("\(error)\n".utf8))
      return (500, Data("{\"error\":\"InternalError\"}".utf8))
    }
  }
  func route(_ method: String, _ uri: String, _ content: String, _ raw: Data) throws -> (Int, Data)
  {
    let path =
      String(uri.split(separator: "?", omittingEmptySubsequences: false)[0]).removingPercentEncoding
      ?? uri
    let get = ["/health", "/school", "/status"].contains(path) || path.hasPrefix("/students/")
    let post =
      ["/commands", "/tickets"].contains(path)
      || (path.range(of: "^/tickets/[^/]+/commit$", options: .regularExpression) != nil)
    if !get && !post { throw HttpError(code: 404, message: "NotFound") }
    if method != (get ? "GET" : "POST") { throw HttpError(code: 405, message: "MethodNotAllowed") }
    if get {
      store.refresh()
      if store.listenerFaults != 0 { throw HttpError(code: 500, message: "InternalError") }
      let body: [String: Any]
      switch path {
      case "/health": body = ["status": "ok"]
      case "/status":
        body = [
          "eventPosition": store.read(Query(items: [])).after,
          "projectionPosition": projection.checkpoint, "listenerFaults": store.listenerFaults,
        ]
      case "/school":
        let (p, s) = projection.snapshot()
        body = [
          "position": p, "state": try JSONSerialization.jsonObject(with: JSONEncoder().encode(s)),
        ]
      default:
        let (p, s) = projection.findStudent(String(path.dropFirst(10)))
        guard let student = s else { throw HttpError(code: 404, message: "NotFound") }
        body = [
          "position": p,
          "student": try JSONSerialization.jsonObject(with: JSONEncoder().encode(student)),
        ]
      }
      return (200, try JSONSerialization.data(withJSONObject: body))
    }
    if path == "/commands" || path == "/tickets" {
      if content.split(separator: ";", omittingEmptySubsequences: false)[0].trimmingCharacters(
        in: .whitespaces) != "application/json"
      {
        throw HttpError(code: 415, message: "UnsupportedMediaType")
      }
      let input: Any
      do { input = try JSONSerialization.jsonObject(with: raw, options: [.fragmentsAllowed]) } catch
      { try bad() }
      try validate(input, schemas["CommandRequest"] as! [String: Any], schemas)
      let obj = input as! [String: Any]
      let command: Command
      do {
        command = try JSONDecoder().decode(
          Command.self, from: JSONSerialization.data(withJSONObject: obj["command"]!))
      } catch { try bad() }
      let t = DcbExecutor(store: store).prepare(
        obj["commandId"] as! String, command, SchoolHandler())
      if path == "/commands" || !t.wasRead { return try commit(t) }
      ticketGate.lock()
      defer { ticketGate.unlock() }
      expire()
      if tickets.count >= 1024 { throw HttpError(code: 429, message: "TooManyTickets") }
      let id = UUID().uuidString
      tickets[id] = (ProcessInfo.processInfo.systemUptime, t)
      return (201, try JSONSerialization.data(withJSONObject: ["ticket": id, "position": t.after]))
    }
    let t: PreparedCommand<Rejection>
    ticketGate.lock()
    expire()
    let id = String(path.split(separator: "/")[1])
    let found = tickets.removeValue(forKey: id)
    ticketGate.unlock()
    guard let found = found else { throw HttpError(code: 404, message: "NotFound") }
    t = found.1
    return try commit(t)
  }
}
final class HTTPHandler: ChannelInboundHandler {
  typealias InboundIn = HTTPServerRequestPart
  typealias OutboundOut = HTTPServerResponsePart
  let engine: Engine
  let workers: NIOThreadPool
  var head: HTTPRequestHead?
  var body = Data()
  var tooLarge = false
  init(_ engine: Engine, _ workers: NIOThreadPool) {
    self.engine = engine
    self.workers = workers
  }
  func channelRead(context: ChannelHandlerContext, data: NIOAny) {
    switch unwrapInboundIn(data) {
    case .head(let h):
      head = h
      body.removeAll(keepingCapacity: true)
      tooLarge = false
    case .body(var bytes):
      if body.count + bytes.readableBytes > 1_048_576 {
        tooLarge = true
      } else if !tooLarge {
        body.append(contentsOf: bytes.readBytes(length: bytes.readableBytes)!)
      }
    case .end:
      guard let h = head else { return }
      let data = body
      let oversized = tooLarge
      head = nil
      workers.runIfActive(eventLoop: context.eventLoop) { [engine] in
        oversized
          ? (413, Data("{\"error\":\"PayloadTooLarge\"}".utf8))
          : engine.request(
            h.method.rawValue, h.uri, h.headers.first(name: "content-type") ?? "", data)
      }.whenComplete { result in
        let (code, raw) = (try? result.get()) ?? (500, Data("{\"error\":\"InternalError\"}".utf8))
        var headers = HTTPHeaders()
        headers.add(name: "content-type", value: "application/json")
        headers.add(name: "content-length", value: String(raw.count))
        context.write(
          self.wrapOutboundOut(
            .head(
              HTTPResponseHead(
                version: h.version, status: HTTPResponseStatus(statusCode: code), headers: headers))
          ), promise: nil)
        var buffer = context.channel.allocator.buffer(capacity: raw.count)
        buffer.writeBytes(raw)
        context.write(self.wrapOutboundOut(.body(.byteBuffer(buffer))), promise: nil)
        context.writeAndFlush(self.wrapOutboundOut(.end(nil)), promise: nil)
      }
    }
  }
  func errorCaught(context: ChannelHandlerContext, error: Error) { context.close(promise: nil) }
}
let group = MultiThreadedEventLoopGroup(numberOfThreads: 2)
let workers = NIOThreadPool(numberOfThreads: 8)
workers.start()
#if canImport(ObjectiveC)
  let engine = try autoreleasepool { try Engine() }
#else
  let engine = try Engine()
#endif
let channel = try ServerBootstrap(group: group).serverChannelOption(
  ChannelOptions.backlog, value: 128
).serverChannelOption(ChannelOptions.socketOption(.so_reuseaddr), value: 1).childChannelInitializer
{ channel in
  channel.pipeline.configureHTTPServerPipeline().flatMap {
    channel.pipeline.addHandler(HTTPHandler(engine, workers))
  }
}.bind(host: "127.0.0.1", port: Int(ProcessInfo.processInfo.environment["PORT"] ?? "8080")!).wait()
try channel.closeFuture.wait()
try workers.syncShutdownGracefully()
try group.syncShutdownGracefully()
engine.store.close()
