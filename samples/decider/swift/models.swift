struct Roster: Codable, Equatable {
    let name: String
    let capacity: Int
    let students: [String]
}
enum State: Equatable {
    case notCreated
    case available(Roster)
    case full(Roster)

    var phase: String {
        switch self {
        case .notCreated: return "NotCreated"
        case .available: return "Available"
        case .full: return "Full"
        }
    }
    var roster: Roster? {
        switch self {
        case .notCreated: return nil
        case .available(let r), .full(let r): return r
        }
    }
}
enum Command {
    case create(name: String, capacity: Int)
    case enroll(String)
    case drop(String)
}
enum Event {
    case roomCreated(name: String, capacity: Int)
    case studentEnrolled(String)
    case studentDropped(String)

    var tag: String {
        switch self {
        case .roomCreated: return "RoomCreated"
        case .studentEnrolled: return "StudentEnrolled"
        case .studentDropped: return "StudentDropped"
        }
    }
}
enum Rejection: String, Error {
    case notCreated = "not_created"
    case alreadyCreated = "already_created"
    case invalidName = "invalid_name"
    case invalidCapacity = "invalid_capacity"
    case invalidStudent = "invalid_student"
    case alreadyEnrolled = "already_enrolled"
    case full = "full"
    case notEnrolled = "not_enrolled"
}
enum HistoryError: Error { case invalidHistory }
