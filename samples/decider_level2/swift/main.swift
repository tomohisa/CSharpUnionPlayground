import Foundation
struct Input: Codable { let commands: [Command]; let history: [Event] }
struct Output: Codable { let initial: State; let state: State; let events: [Event]; let steps: [Decision]; let restored: State; let replayed: State; let summaries: [String] }
do {
    let decoder = JSONDecoder(), encoder = JSONEncoder()
    let input = try decoder.decode(Input.self, from: FileHandle.standardInput.readDataToEndOfFile())
    var history = input.history, state = try replay(input.history)
    let initial = state
    var steps: [Decision] = []
    for command in input.commands {
        let decision = decide(state, command); steps.append(decision)
        if case .accepted(let events) = decision {
            for event in events { state = try evolve(state, event); history.append(event) }
        }
    }
    let events = try decoder.decode([Event].self, from: encoder.encode(history))
    let restored = try decoder.decode(State.self, from: encoder.encode(state))
    let output = Output(initial: initial, state: state, events: events, steps: steps, restored: restored, replayed: try replay(events), summaries: state.enrollments.map { describe($0.progress) })
    print(String(decoding: try encoder.encode(output), as: UTF8.self))
} catch {
    FileHandle.standardError.write(Data((String(describing: error) + "\n").utf8)); exit(1)
}
