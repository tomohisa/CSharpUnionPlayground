import playground.dcb.Dcb;
import java.util.*;

final class SchoolProjection {
    record Key(String kind,String a,String b,int node){Key(String kind,String a,String b){this(kind,a,b,0);}Key(String kind,String a){this(kind,a,"",0);}}
    sealed interface Row permits StudentRow,Room,EnrollmentRow,Member,MemberIndex {}
    record StudentRow(Student value) implements Row {}
    record Room(ClassState phase,int first,int last,int count) implements Row {}
    record EnrollmentRow(Enrollment value) implements Row {}
    record Member(String student,int previous,int next) implements Row {}
    record MemberIndex(int node) implements Row {}
    private final Dcb.IndexedProjection<Key,Row,Event> index=new Dcb.IndexedProjection<>(SchoolProjection::apply);
    Student student(String id){return index.read(rows->((StudentRow)rows.get(new Key("student",id))).value()).state();}
    int checkpoint(){return index.checkpoint();}int rowCount(){return index.rowCount();}long work(){return index.work();}
    boolean accept(List<Dcb.Recorded<Event>> events){return index.accept(events);}
    static Key node(int n){return new Key("node","","",n);}
    static void apply(Dcb.IndexBatch<Key,Row> tx,Dcb.Recorded<Event> recorded){
        var e=recorded.payload();
        switch(e){
            case StudentRegistered x->{tx.put(new Key("student",x.studentId()),new StudentRow(new Student(x.studentId(),x.grade())));return;}
            case StudentPromoted x->{tx.put(new Key("student",x.studentId()),new StudentRow(new Student(x.studentId(),x.grade())));return;}
            case ClassOpened x->{var phase=Projection.reduce(Projection.Registers.empty(),e).classes().get(x.classId());tx.put(new Key("class",x.classId()),new Room(phase,0,0,0));return;}
            default->{}
        }
        String id=switch(e){case StudentEnrolled x->x.classId();case StudentDropped x->x.classId();case ClassStarted x->x.classId();case ClassCompleted x->x.classId();case StudyStarted x->x.classId();case AttendanceRecorded x->x.classId();case ResultRecorded x->x.classId();default->throw new IllegalStateException();};
        var ck=new Key("class",id);var found=tx.find(ck);
        if(found.isPresent()){
            var c=(Room)found.get();var plan=Decider.planOf(c.phase());
            switch(e){
                case StudentEnrolled x->{
                    int n=recorded.position();if(c.last()!=0){var m=(Member)tx.get(node(c.last()));tx.put(node(c.last()),new Member(m.student(),m.previous(),n));}
                    tx.put(node(n),new Member(x.studentId(),c.last(),0));tx.put(new Key("member",id,x.studentId()),new MemberIndex(n));int count=c.count()+1;
                    c=new Room(count==plan.capacity()?new Full(plan,List.of()):new Available(plan,List.of()),c.first()==0?n:c.first(),n,count);
                }
                case StudentDropped x->{
                    var member=tx.find(new Key("member",id,x.studentId()));
                    if(member.isPresent()){
                        int n=((MemberIndex)member.get()).node();var m=(Member)tx.get(node(n));
                        if(m.previous()!=0){var p=(Member)tx.get(node(m.previous()));tx.put(node(m.previous()),new Member(p.student(),p.previous(),m.next()));}
                        if(m.next()!=0){var p=(Member)tx.get(node(m.next()));tx.put(node(m.next()),new Member(p.student(),m.previous(),p.next()));}
                        tx.delete(node(n));tx.delete(new Key("member",id,x.studentId()));c=new Room(c.phase(),c.first()==n?m.next():c.first(),c.last()==n?m.previous():c.last(),c.count()-1);
                    }
                    c=new Room(new Available(plan,List.of()),c.first(),c.last(),c.count());
                }
                case ClassStarted x->c=new Room(new Teaching(plan,List.of(),SchoolDate.fromEvent(x.startedOn()).value()),c.first(),c.last(),c.count());
                case ClassCompleted x->c=new Room(new Completed(plan,x.results()),c.first(),c.last(),c.count());
                default->{}
            }
            if(e instanceof StudentEnrolled || e instanceof StudentDropped || e instanceof ClassStarted || e instanceof ClassCompleted)tx.put(ck,c);
        }
        String student=switch(e){case StudentEnrolled x->x.studentId();case StudentDropped x->x.studentId();case StudyStarted x->x.studentId();case AttendanceRecorded x->x.studentId();case ResultRecorded x->x.studentId();default->"";};
        if(!(e instanceof StudentEnrolled || e instanceof StudentDropped || e instanceof StudyStarted || e instanceof AttendanceRecorded || e instanceof ResultRecorded))return;
        var ek=new Key("enrollment",id,student);
        if(e instanceof StudentEnrolled x){tx.put(ek,new EnrollmentRow(new Enrollment(id,student,x.course(),new Reserved())));return;}
        var existing=tx.find(ek);if(existing.isEmpty())return;var value=((EnrollmentRow)existing.get()).value();
        Progress progress=switch(e){case StudentDropped x->new Withdrawn(WithdrawalReason.StudentRequest);case StudyStarted x->new Studying(0);
            case AttendanceRecorded x->{if(!(value.progress() instanceof Studying p))throw new IllegalStateException("invalid committed event");yield new Studying(p.attendance()+1);}
            case ResultRecorded x->x.score()>=60?new Passed(AssessmentScore.fromEvent(x.score()).value(),value.course()==Course.Foundation?1:2):new Failed(AssessmentScore.fromEvent(x.score()).value(),FailureReason.BelowPassMark);default->value.progress();};
        tx.put(ek,new EnrollmentRow(new Enrollment(id,student,value.course(),progress)));
    }
    Dcb.Snapshot<SchoolList> snapshot(){return index.read(rows->{
        var students=new ArrayList<Student>();var classes=new ArrayList<ClassState>();var enrollments=new ArrayList<Enrollment>();
        for(var row:rows.values())switch(row){case StudentRow x->students.add(x.value());case EnrollmentRow x->enrollments.add(x.value());case Room c->{
            var ids=new ArrayList<String>();if(!(c.phase() instanceof Completed))for(int n=c.first();n!=0;){var m=(Member)rows.get(node(n));ids.add(m.student());n=m.next();}
            classes.add(switch(c.phase()){case Available x->new Available(x.plan(),ids);case Full x->new Full(x.plan(),ids);case Teaching x->new Teaching(x.plan(),ids,x.startedOn());case Completed x->x;});}default->{}}
        students.sort(Comparator.comparing(Student::studentId,Projection::compareIds));classes.sort(Comparator.comparing(x->Decider.planOf(x).classId(),Projection::compareIds));enrollments.sort(Comparator.comparing(Enrollment::classId,Projection::compareIds).thenComparing(Enrollment::studentId,Projection::compareIds));
        return new SchoolList(students,classes,enrollments,enrollments.stream().map(x->Decider.describe(x.progress())).toList());
    });}
}
