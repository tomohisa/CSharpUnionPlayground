import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

public final class Main {
    record Input(List<Command> commands, List<Event> history) {
        public Input { commands = List.copyOf(commands); history = List.copyOf(history); }
    }
    record Output(State initial, State state, List<Event> events, List<Decision> steps, State restored, State replayed, List<String> summaries) {}
    public static void main(String[] args) throws Exception {
        var mapper = new ObjectMapper().enable(DeserializationFeature.FAIL_ON_NULL_FOR_PRIMITIVES, DeserializationFeature.FAIL_ON_MISSING_CREATOR_PROPERTIES);
        var input = mapper.readValue(System.in, Input.class);
        var history = new ArrayList<>(input.history());
        State state = Decider.replay(history), initial = state;
        var steps = new ArrayList<Decision>();
        for (var command : input.commands()) {
            var decision = Decider.decide(state, command); steps.add(decision);
            if (decision instanceof Accepted a) for (var event : a.events()) { state = Decider.evolve(state, event); history.add(event); }
        }
        var eventType = new TypeReference<List<Event>>() {};
        var stored = mapper.readValue(mapper.writerFor(eventType).writeValueAsString(history), eventType);
        var restored = mapper.readValue(mapper.writerFor(State.class).writeValueAsString(state), State.class);
        System.out.println(mapper.writeValueAsString(new Output(initial, state, stored, steps, restored, Decider.replay(stored), state.enrollments().stream().map(e -> Decider.describe(e.progress())).toList())));
    }
}
