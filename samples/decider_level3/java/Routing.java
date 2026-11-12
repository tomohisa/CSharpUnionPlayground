import playground.dcb.Dcb;
import java.util.List;
final class Routing {
    static Dcb.Tag student(String id){return new Dcb.Tag("student",List.of(id));}
    static Dcb.Tag classroom(String id){return new Dcb.Tag("class",List.of(id));}
    static Dcb.Tag enrollment(String c,String s){return new Dcb.Tag("enrollment",List.of(c,s));}
    static Dcb.Query one(Dcb.Tag... tags){return new Dcb.Query(java.util.Arrays.stream(tags).map(t->new Dcb.QueryItem(List.of(t),List.of())).toList());}
    static Dcb.Query queryFor(Command command){return switch(command){
        case RegisterStudent c->one(student(c.studentId()));
        case PromoteStudent c->one(student(c.studentId()));
        case OpenClass c->one(classroom(c.classId()));
        case Enroll c->one(student(c.studentId()),classroom(c.classId()));
        case Drop c->one(classroom(c.classId()));
        case StartClass c->one(classroom(c.classId()));
        case Attend c->one(classroom(c.classId()));
        case Assess c->one(classroom(c.classId()));
        case CompleteClass c->one(classroom(c.classId()));
    };}
    static Dcb.EventDescriptor describe(Event event){return switch(event){
        case StudentRegistered e->new Dcb.EventDescriptor("StudentRegistered",List.of(student(e.studentId())));
        case StudentPromoted e->new Dcb.EventDescriptor("StudentPromoted",List.of(student(e.studentId())));
        case ClassOpened e->new Dcb.EventDescriptor("ClassOpened",List.of(classroom(e.classId())));
        case StudentEnrolled e->new Dcb.EventDescriptor("StudentEnrolled",List.of(student(e.studentId()),classroom(e.classId()),enrollment(e.classId(),e.studentId())));
        case StudentDropped e->new Dcb.EventDescriptor("StudentDropped",List.of(student(e.studentId()),classroom(e.classId()),enrollment(e.classId(),e.studentId())));
        case ClassStarted e->new Dcb.EventDescriptor("ClassStarted",List.of(classroom(e.classId())));
        case AttendanceRecorded e->new Dcb.EventDescriptor("AttendanceRecorded",List.of(student(e.studentId()),classroom(e.classId()),enrollment(e.classId(),e.studentId())));
        case ResultRecorded e->new Dcb.EventDescriptor("ResultRecorded",List.of(student(e.studentId()),classroom(e.classId()),enrollment(e.classId(),e.studentId())));
        case ClassCompleted e->new Dcb.EventDescriptor("ClassCompleted",List.of(classroom(e.classId())));
        case StudyStarted e->new Dcb.EventDescriptor("StudyStarted",List.of(student(e.studentId()),classroom(e.classId()),enrollment(e.classId(),e.studentId())));
    };}
}
