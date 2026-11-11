func decide(_ state: State, _ command: Command) -> Result<[Event], Rejection> {
    switch command {
    case .create(let name, let capacity):
        guard case .notCreated = state else { return .failure(.alreadyCreated) }
        if name.isEmpty { return .failure(.invalidName) }
        if !(1...10).contains(capacity) { return .failure(.invalidCapacity) }
        return .success([.roomCreated(name: name, capacity: capacity)])
    case .enroll(let student): return change(state, student, enrolling: true)
    case .drop(let student): return change(state, student, enrolling: false)
    }
}
private func change(_ state: State, _ student: String, enrolling: Bool) -> Result<[Event], Rejection> {
    guard let roster = state.roster else { return .failure(.notCreated) }
    if student.isEmpty { return .failure(.invalidStudent) }
    let member = roster.students.contains(student)
    if enrolling {
        if member { return .failure(.alreadyEnrolled) }
        if case .full = state { return .failure(.full) }
        return .success([.studentEnrolled(student)])
    }
    return member ? .success([.studentDropped(student)]) : .failure(.notEnrolled)
}
private func classify(_ roster: Roster) -> State {
    roster.students.count == roster.capacity ? .full(roster) : .available(roster)
}
func evolve(_ state: State, _ event: Event) throws -> State {
    switch event {
    case .roomCreated(let name, let capacity):
        guard case .notCreated = state, !name.isEmpty, (1...10).contains(capacity)
            else { throw HistoryError.invalidHistory }
        return .available(Roster(name: name, capacity: capacity, students: []))
    case .studentEnrolled(let student):
        guard case .available(let r) = state, !student.isEmpty, !r.students.contains(student)
            else { throw HistoryError.invalidHistory }
        return classify(Roster(name: r.name, capacity: r.capacity, students: r.students + [student]))
    case .studentDropped(let student):
        guard let r = state.roster, r.students.contains(student) else { throw HistoryError.invalidHistory }
        return classify(Roster(name: r.name, capacity: r.capacity, students: r.students.filter { $0 != student }))
    }
}
func replay(_ events: [Event]) throws -> State {
    try events.reduce(State.notCreated, evolve)
}
