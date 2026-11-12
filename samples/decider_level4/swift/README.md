# Swift: Level4 HTTP API

[共通仕様・実行・コード量](../README.md)／[OpenAPI](../openapi.json)／[再利用するLevel3](../../decider_level3/swift)。学校ドメイン・VO・DeciderはLevel3から参照し、コピーしていません。

## 今回良かったところ

associated value付きenum、既存のCodable wire、VO・Handler・投影を保ってHTTPを追加できました。SQLiteとDeciderをNIOThreadPoolへ移し、NIOのevent loopを同期I/Oで塞がない構成にしています。optionalの学生検索をguardで処理して、存在しないIDをfatal errorにしない境界も追加しました。

[swift/main.swift:203–207](main.swift#L203-L207)

```swift
      workers.runIfActive(eventLoop: context.eventLoop) { [engine] in
        oversized
          ? (413, Data("{\"error\":\"PayloadTooLarge\"}".utf8))
          : engine.handle(
            h.method.rawValue, h.uri, h.headers.first(name: "content-type") ?? "", data)
```

## 実装で増えた仕事

JSONはCodableの自動生成だけで既定の{$type:...}形式にならず、Level3のwire.swiftを引き続き使います。HTTPサーバーにもSwiftNIOのpipeline・buffer・futureを接続するコードが必要です。F#やRustのenum/DUが簡潔なことと、このJSON・HTTP契約のコード量が少ないことは別でした。

[swift/main.swift:50–54](main.swift#L50-L54)

```swift
final class Engine: @unchecked Sendable {
  let store: JsonEventStore<Event>
  let projection = SchoolProjection()
  let schemas: [String: Any]
  let ticketGate = NSLock()
```

## 他の実装と比較すると

Engineと既存投影の@unchecked Sendableは内部のロックを正しく使うという実装側の約束です。RustのSendを満たすOwnedPreparedほどコンパイラーが同期を証明しているわけではありません。SwiftNIOのevent loopは2、ブロッキング処理は8ワーカー、負荷の同時接続数は他言語と同じ8にしています。

[共通HTTPテスト](../tests/http_tests.py)では入力拒否・競合再判断・投影反映・SQLite再起動と相互復元を検証します。[ローカルの測定結果](../benchmark/observed/README.md)は、このホスト・codec・ストアを含む一連の実装の比較です。
