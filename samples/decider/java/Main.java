import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.ArrayList;
import java.util.List;

public final class Main {
    record Input(List<Command> commands, List<Event> history) {}
    record Step(String phase, List<String> emitted, String error) {}
    record Output(State initial, State state, List<Event> events, List<Step> steps, State restored, State replayed) {}

    public static void main(String[] args) throws Exception {
        var mapper = new ObjectMapper();
        var input = mapper.readValue(System.in, Input.class);
        var history = new ArrayList<>(input.history() == null ? List.<Event>of() : input.history());
        State state = Decider.replay(history);
        State initial = state;
        var steps = new ArrayList<Step>();
        for (var command : input.commands()) {
            switch (Decider.decide(state, command)) {
                case Accepted a -> {
                    for (var event : a.events()) {
                        state = Decider.evolve(state, event);
                        history.add(event);
                    }
                    steps.add(new Step(Decider.phase(state), a.events().stream().map(Main::tag).toList(), null));
                }
                case Rejected r -> steps.add(new Step(Decider.phase(state), List.of(), r.code()));
            }
        }
        var type = new TypeReference<List<Event>>() {};
        var stored = mapper.readValue(mapper.writerFor(type).writeValueAsString(history), type);
        var restored = mapper.readValue(mapper.writerFor(State.class).writeValueAsString(state), State.class);
        System.out.println(mapper.writeValueAsString(
            new Output(initial, state, stored, steps, restored, Decider.replay(stored))));
    }
    static String tag(Event event) {
        return switch (event) {
            case RoomCreated e -> "RoomCreated";
            case StudentEnrolled e -> "StudentEnrolled";
            case StudentDropped e -> "StudentDropped";
        };
    }
}
