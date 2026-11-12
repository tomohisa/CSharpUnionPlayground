import Foundation
struct SchoolHandler:CommandHandler {
    func validate(_ command:Command)->[Rejection]{ValidatedCommand.validate(command)}
    func queryFor(_ command:Command)->Query{schoolQueryFor(command)}
    func handle(_ events:[Recorded<Event>],_ command:Command)->Preparation<Event,Rejection>{
        do{
            let validated=try ValidatedCommand.create(command)
            switch decide(fold(events).decisionModel(),validated){case .accepted(let events):return .emit(events);case .rejected(let code):return .reject(code)}
        }catch let error as Rejection{return .reject(error)}catch{preconditionFailure("unexpected validator error")}
    }
}
