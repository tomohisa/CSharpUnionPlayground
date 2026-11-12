import playground.dcb.Dcb;
import java.util.List;
final class SchoolHandler implements Dcb.CommandHandler<Command,Event,Rejection> {
    public List<Rejection> validate(Command command){return ValidatedCommand.validate(command);}
    public Dcb.Query queryFor(Command command){return Routing.queryFor(command);}
    public Dcb.Preparation<Event,Rejection> handle(List<Dcb.Recorded<Event>> events,Command command){
        return switch(Decider.decide(Projection.fold(events).decisionModel(),ValidatedCommand.create(command))){
            case Accepted a->new Dcb.Emit<>(a.events()); case Rejected r->new Dcb.Reject<>(r.code());
        };
    }
}
