using System.Collections.Immutable;
using System.Globalization;
using Playground.Dcb;
namespace SchoolDomain;

public sealed class SchoolHandler : ICommandHandler<Command,Event,Rejection>
{
    public ImmutableArray<Rejection> Validate(Command command)=>ValidatedCommand.Validate(command);
    public Query QueryFor(Command command)=>Routing.QueryFor(command);
    public Preparation<Event,Rejection> Handle(ImmutableArray<Recorded<Event>> events,Command command) =>
        Decider.Decide(Registers.Fold(events).DecisionModel(),ValidatedCommand.Create(command)) switch
        {
            Accepted a=>new Emit<Event,Rejection>(a.Events),
            Rejected r=>new Reject<Event,Rejection>(r.Code)
        };
}
