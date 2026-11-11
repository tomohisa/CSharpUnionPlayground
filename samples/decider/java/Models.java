import com.fasterxml.jackson.annotation.JsonSubTypes;
import com.fasterxml.jackson.annotation.JsonTypeInfo;
import java.util.List;

record Roster(String name, int capacity, List<String> students) {
    Roster { students = List.copyOf(students); }
}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = NotCreated.class, name = "NotCreated"),
    @JsonSubTypes.Type(value = Available.class, name = "Available"),
    @JsonSubTypes.Type(value = Full.class, name = "Full")
})
sealed interface State permits NotCreated, Available, Full {}
record NotCreated() implements State {}
record Available(Roster roster) implements State {}
record Full(Roster roster) implements State {}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = Create.class, name = "Create"),
    @JsonSubTypes.Type(value = Enroll.class, name = "Enroll"),
    @JsonSubTypes.Type(value = Drop.class, name = "Drop")
})
sealed interface Command permits Create, Enroll, Drop {}
record Create(String name, int capacity) implements Command {}
record Enroll(String studentId) implements Command {}
record Drop(String studentId) implements Command {}

@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = RoomCreated.class, name = "RoomCreated"),
    @JsonSubTypes.Type(value = StudentEnrolled.class, name = "StudentEnrolled"),
    @JsonSubTypes.Type(value = StudentDropped.class, name = "StudentDropped")
})
sealed interface Event permits RoomCreated, StudentEnrolled, StudentDropped {}
record RoomCreated(String name, int capacity) implements Event {}
record StudentEnrolled(String studentId) implements Event {}
record StudentDropped(String studentId) implements Event {}

sealed interface Decision permits Accepted, Rejected {}
record Accepted(List<Event> events) implements Decision {
    Accepted { events = List.copyOf(events); }
}
record Rejected(String code) implements Decision {}
