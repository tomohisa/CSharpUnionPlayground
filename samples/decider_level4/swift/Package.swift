// swift-tools-version: 6.0
import PackageDescription

var products: [Target.Dependency] = [
  .product(name: "NIOCore", package: "swift-nio"), .product(name: "NIOPosix", package: "swift-nio"),
  .product(name: "NIOHTTP1", package: "swift-nio"),
]
var targets: [Target] = []
#if os(Linux)
  products.append("SQLite3")
  targets.append(.systemLibrary(name: "SQLite3", path: "Sources/SQLite3"))
#endif
targets.append(.executableTarget(name: "Server", dependencies: products))
let package = Package(
  name: "SchoolHTTP",
  dependencies: [.package(url: "https://github.com/apple/swift-nio.git", exact: "2.105.0")],
  targets: targets, swiftLanguageModes: [.v5])
