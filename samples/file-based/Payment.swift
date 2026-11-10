// Run: swift samples/file-based/Payment.swift
import Foundation

indirect enum Payment: Codable {
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
