import Foundation

do { try run() }
catch {
    FileHandle.standardError.write(Data((String(describing: error) + "\n").utf8))
    exit(1)
}

func run() throws {
    let input = try JSONDecoder().decode(Input.self, from: FileHandle.standardInput.readDataToEndOfFile())
    var history = input.history ?? []
    var state = try replay(history)
    let initial = state
    var steps: [Step] = []
    for command in input.commands {
        switch decide(state, command) {
        case .success(let events):
            for event in events { state = try evolve(state, event); history.append(event) }
            steps.append(Step(phase: state.phase, emitted: events.map(\.tag), error: nil))
        case .failure(let error):
            steps.append(Step(phase: state.phase, emitted: [], error: error.rawValue))
        }
    }
    let encoder = JSONEncoder()
    let decoder = JSONDecoder()
    let stored = try decoder.decode([Event].self, from: encoder.encode(history))
    let restored = try decoder.decode(State.self, from: encoder.encode(state))
    let output = Output(initial: initial, state: state, events: stored, steps: steps, restored: restored, replayed: try replay(stored))
    print(String(decoding: try encoder.encode(output), as: UTF8.self))
}
