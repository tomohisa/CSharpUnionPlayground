import java.time.LocalDate;
import java.time.format.DateTimeParseException;
import java.util.List;
final class InputViolation extends IllegalArgumentException {
    final Rejection reason;
    InputViolation(Rejection reason){super(reason.name());this.reason=reason;}
}

final class StudentId {
    private final String value;private StudentId(String value){this.value=value;}
    String value(){return value;}
    static StudentId fromEvent(String value){return new StudentId(value);}
    static StudentId create(String value){if(value==null || value.isBlank())throw new InputViolation(Rejection.InvalidId);return new StudentId(value);}
}
final class ClassId {
    private final String value;private ClassId(String value){this.value=value;}
    String value(){return value;}
    static ClassId fromEvent(String value){return new ClassId(value);}
    static ClassId create(String value){if(value==null || value.isBlank())throw new InputViolation(Rejection.InvalidId);return new ClassId(value);}
}
final class SeatCapacity {
    private final int value;private SeatCapacity(int value){this.value=value;}
    int value(){return value;}
    static SeatCapacity fromEvent(int value){return new SeatCapacity(value);}
    static SeatCapacity create(int value){if(value<1 || value>10)throw new InputViolation(Rejection.InvalidCapacity);return new SeatCapacity(value);}
}
final class AssessmentScore {
    private final int value;private AssessmentScore(int value){this.value=value;}
    int value(){return value;}
    static AssessmentScore fromEvent(int value){return new AssessmentScore(value);}
    static AssessmentScore create(int value){if(value<0 || value>100)throw new InputViolation(Rejection.InvalidScore);return new AssessmentScore(value);}
}
final class SchoolDate {
    private final String value;private SchoolDate(String value){this.value=value;}
    String value(){return value;}
    static SchoolDate fromEvent(String value){return new SchoolDate(value);}
    static SchoolDate parse(String text){
        try{if(text==null || text.length()!=10 || LocalDate.parse(text).getYear()<1)throw new InputViolation(Rejection.InvalidDate);return new SchoolDate(text);}
        catch(DateTimeParseException error){throw new InputViolation(Rejection.InvalidDate);}
    }
}
final class ValidatedCommand {
    private final Command value;
    private ValidatedCommand(Command value){this.value=value;}
    Command value(){return value;}
    static ValidatedCommand create(Command command){
        if(command==null)throw new InputViolation(Rejection.InvalidCommand);
        switch(command){
            case RegisterStudent c->StudentId.create(c.studentId());
            case PromoteStudent c->StudentId.create(c.studentId());
            case OpenClass c->{ClassId.create(c.classId());SeatCapacity.create(c.capacity());}
            case Enroll c->{ClassId.create(c.classId());StudentId.create(c.studentId());}
            case Drop c->{ClassId.create(c.classId());StudentId.create(c.studentId());}
            case StartClass c->{ClassId.create(c.classId());SchoolDate.parse(c.startedOn());}
            case Attend c->{ClassId.create(c.classId());StudentId.create(c.studentId());}
            case Assess c->{ClassId.create(c.classId());StudentId.create(c.studentId());AssessmentScore.create(c.score());}
            case CompleteClass c->ClassId.create(c.classId());
        }
        return new ValidatedCommand(command);
    }
    static List<Rejection> validate(Command command){
        try{create(command);return List.of();}catch(InputViolation error){return List.of(error.reason);}
    }
}
