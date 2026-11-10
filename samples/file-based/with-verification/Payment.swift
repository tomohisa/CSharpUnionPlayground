// Run: swift samples/file-based/with-verification/Payment.swift
import Foundation

indirect enum Payment: Codable, Equatable {
    case cash(amount: Int, next: Payment?)
    case card(cardNumber: String, amount: Int, next: Payment?)
}

// No default: adding a case requires updating this switch.
func describe(_ payment: Payment) -> String {
    switch payment {
    case let .cash(amount, _): return "cash:\(amount)"
    case let .card(cardNumber, _, _): return "card:\(cardNumber)"
    }
}

let original = Payment.cash(amount: 100, next: .card(cardNumber: "4111", amount: 250, next: nil))
let encoder = JSONEncoder()
encoder.outputFormatting = [.sortedKeys]
let json = try encoder.encode(original)
let restored = try JSONDecoder().decode(Payment.self, from: json)
print(String(decoding: json, as: UTF8.self))
print(describe(restored))

// Verification belongs only in this version.
precondition(original == restored, "Nested case/value round trip failed.")
guard case let .cash(amount, .some(inner)) = restored else {
    fatalError("Expected Cash containing another Payment.")
}
precondition(amount == 100 && describe(restored) == "cash:100")
precondition(describe(inner) == "card:4111")
let root = try JSONSerialization.jsonObject(with: json) as! [String: Any]
let cash = root["cash"] as! [String: Any]
let next = cash["next"] as! [String: Any]
precondition(next["card"] != nil, "Synthesized case-name envelope was not emitted.")
for invalid in [
    "null",
    "{\"unknown\":{}}",
    "{\"cash\":{\"amount\":100,\"next\":{\"unknown\":{}}}}"
] {
    do {
        _ = try JSONDecoder().decode(Payment.self, from: Data(invalid.utf8))
        fatalError("Invalid Payment was accepted.")
    } catch is DecodingError { }
}
print("PASS")
