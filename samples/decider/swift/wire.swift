import Foundation

// Explicit Codable mappings keep the shared "$type" wire contract out of the decider.
private enum Keys: String, CodingKey {
    case type = "$type", name, capacity, studentId, roster
}
private func unknown(_ c: KeyedDecodingContainer<Keys>) -> DecodingError {
    .dataCorruptedError(forKey: .type, in: c, debugDescription: "Unknown discriminator")
}
extension Command: Decodable {
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        switch try c.decode(String.self, forKey: .type) {
        case "Create": self = .create(name: try c.decode(String.self, forKey: .name),
                                     capacity: try c.decode(Int.self, forKey: .capacity))
        case "Enroll": self = .enroll(try c.decode(String.self, forKey: .studentId))
        case "Drop": self = .drop(try c.decode(String.self, forKey: .studentId))
        default: throw unknown(c)
        }
    }
}
extension Event: Codable {
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        switch try c.decode(String.self, forKey: .type) {
        case "RoomCreated": self = .roomCreated(name: try c.decode(String.self, forKey: .name),
                                              capacity: try c.decode(Int.self, forKey: .capacity))
        case "StudentEnrolled": self = .studentEnrolled(try c.decode(String.self, forKey: .studentId))
        case "StudentDropped": self = .studentDropped(try c.decode(String.self, forKey: .studentId))
        default: throw unknown(c)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        try c.encode(tag, forKey: .type)
        switch self {
        case .roomCreated(let name, let capacity):
            try c.encode(name, forKey: .name)
            try c.encode(capacity, forKey: .capacity)
        case .studentEnrolled(let id), .studentDropped(let id):
            try c.encode(id, forKey: .studentId)
        }
    }
}
extension State: Codable {
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        switch try c.decode(String.self, forKey: .type) {
        case "NotCreated": self = .notCreated
        case "Available": self = .available(try c.decode(Roster.self, forKey: .roster))
        case "Full": self = .full(try c.decode(Roster.self, forKey: .roster))
        default: throw unknown(c)
        }
    }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: Keys.self)
        try c.encode(phase, forKey: .type)
        switch self {
        case .notCreated: break
        case .available(let r), .full(let r): try c.encode(r, forKey: .roster)
        }
    }
}
struct Input: Decodable {
    let commands: [Command]
    let history: [Event]?
}
struct Step: Encodable {
    let phase: String
    let emitted: [String]
    let error: String?
    private enum CodingKeys: CodingKey { case phase, emitted, error }
    func encode(to encoder: Encoder) throws {
        var c = encoder.container(keyedBy: CodingKeys.self)
        try c.encode(phase, forKey: .phase)
        try c.encode(emitted, forKey: .emitted)
        if let error { try c.encode(error, forKey: .error) }
        else { try c.encodeNil(forKey: .error) }
    }
}
struct Output: Encodable {
    let initial: State
    let state: State
    let events: [Event]
    let steps: [Step]
    let restored: State
    let replayed: State
}
