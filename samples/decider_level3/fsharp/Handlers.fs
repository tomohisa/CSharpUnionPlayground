namespace SchoolDomain
open Playground.Dcb
module Handlers =
    let school:CommandHandler<Command,Event,Rejection> = {
        Validate=ValidatedCommand.validate
        QueryFor=Routing.queryFor
        Handle=fun events command ->
            match ValidatedCommand.create command with
            | Error error -> Reject error
            | Ok validated ->
                match Decider.decide (Registers.decisionModel (Registers.fold events)) validated with
                | Decision.Accepted events -> Emit events
                | Decision.Rejected code -> Reject code }
