import playground.dcb.Dcb;
import java.util.*;

final class Projection {
    static int compareIds(String a,String b){return Arrays.compareUnsigned(a.getBytes(java.nio.charset.StandardCharsets.UTF_8),b.getBytes(java.nio.charset.StandardCharsets.UTF_8));}
    record Key(String room,String student) implements Comparable<Key> {
        public int compareTo(Key k){int c=compareIds(room,k.room);return c!=0?c:compareIds(student,k.student);}
    }
    record Registers(Map<String,Student> students,Map<String,ClassState> classes,Map<Key,Enrollment> enrollments){
        public Registers{students=Map.copyOf(students);classes=Map.copyOf(classes);enrollments=Map.copyOf(enrollments);}
        static Registers empty(){return new Registers(Map.of(),Map.of(),Map.of());}
        DecisionModel decisionModel(){return new DecisionModel(students.values().stream().sorted(Comparator.comparing(Student::studentId,Projection::compareIds)).toList(),classes.values().stream().sorted(Comparator.comparing((ClassState c)->Decider.planOf(c).classId(),Projection::compareIds)).toList(),enrollments.values().stream().sorted(Comparator.comparing(Enrollment::classId,Projection::compareIds).thenComparing(Enrollment::studentId,Projection::compareIds)).toList());}
        SchoolList list(){var m=decisionModel();return new SchoolList(m.students(),m.classes(),m.enrollments(),m.enrollments().stream().map(e->Decider.describe(e.progress())).toList());}
    }
    static Map<String,Student> student(Map<String,Student> old,Event event){
        var next=new HashMap<>(old);
        switch(event){case StudentRegistered e->next.put(e.studentId(),new Student(e.studentId(),e.grade()));case StudentPromoted e->next.put(e.studentId(),new Student(e.studentId(),e.grade()));default->{return old;}}
        return next;
    }
    static Map<String,ClassState> classroom(Map<String,ClassState> old,Event event){
        var next=new HashMap<>(old);
        if(event instanceof ClassOpened e){
            Plan plan=switch(e.course()){
                case Foundation->new Plan(e.classId(),e.course(),Grade.Year2,new NoPrerequisite(),SeatCapacity.fromEvent(e.capacity()).value());
                case Advanced->new Plan(e.classId(),e.course(),Grade.Year3,new PassedCourse(Course.Foundation),SeatCapacity.fromEvent(e.capacity()).value());
            };next.put(e.classId(),new Available(plan,List.of()));return next;
        }
        if(!(event instanceof StudentEnrolled || event instanceof StudentDropped || event instanceof ClassStarted || event instanceof ClassCompleted))return old;
        String id=switch(event){case StudentEnrolled e->e.classId();case StudentDropped e->e.classId();case ClassStarted e->e.classId();case ClassCompleted e->e.classId();default->throw new IllegalStateException();};
        var found=Optional.ofNullable(old.get(id));if(found.isEmpty())return old;
        var room=found.get();var plan=Decider.planOf(room);
        ClassState updated=switch(event){
            case StudentEnrolled e->{var members=new ArrayList<>(Decider.members(room));members.add(e.studentId());yield members.size()==plan.capacity()?new Full(plan,members):new Available(plan,members);}
            case StudentDropped e->new Available(plan,Decider.members(room).stream().filter(s->!s.equals(e.studentId())).toList());
            case ClassStarted e->new Teaching(plan,Decider.members(room),SchoolDate.fromEvent(e.startedOn()).value());case ClassCompleted e->new Completed(plan,e.results());default->throw new IllegalStateException();
        };next.put(id,updated);return next;
    }
    static Map<Key,Enrollment> enrollment(Map<Key,Enrollment> old,Event event){
        var next=new HashMap<>(old);
        if(event instanceof StudentEnrolled e){next.put(new Key(e.classId(),e.studentId()),new Enrollment(e.classId(),e.studentId(),e.course(),new Reserved()));return next;}
        if(!(event instanceof StudentDropped || event instanceof StudyStarted || event instanceof AttendanceRecorded || event instanceof ResultRecorded))return old;
        Key key=switch(event){case StudentDropped e->new Key(e.classId(),e.studentId());case StudyStarted e->new Key(e.classId(),e.studentId());case AttendanceRecorded e->new Key(e.classId(),e.studentId());case ResultRecorded e->new Key(e.classId(),e.studentId());default->throw new IllegalStateException();};
        var found=Optional.ofNullable(old.get(key));if(found.isEmpty())return old;var record=found.get();
        Progress progress=switch(event){
            case StudentDropped e->new Withdrawn(WithdrawalReason.StudentRequest);case StudyStarted e->new Studying(0);
            case AttendanceRecorded e-> {if(!(record.progress() instanceof Studying p))throw new IllegalStateException("invalid committed event");yield new Studying(p.attendance()+1);}
            case ResultRecorded e->e.score()>=60?new Passed(AssessmentScore.fromEvent(e.score()).value(),record.course()==Course.Foundation?1:2):new Failed(AssessmentScore.fromEvent(e.score()).value(),FailureReason.BelowPassMark);
            default->throw new IllegalStateException();
        };next.put(key,new Enrollment(record.classId(),record.studentId(),record.course(),progress));return next;
    }
    static Registers reduce(Registers s,Event e){return new Registers(student(s.students(),e),classroom(s.classes(),e),enrollment(s.enrollments(),e));}
    static Registers fold(List<Dcb.Recorded<Event>> events){var s=Registers.empty();for(var e:events)s=reduce(s,e.payload());return s;}
}
