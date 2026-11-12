import playground.dcb.Dcb;
import java.util.*;
final class ProjectionTests{
    static Dcb.Recorded<Event> envelope(int position,Event e){return new Dcb.Recorded<>(position,"scale","projection-test",List.of(),e);}
    static void require(boolean value){if(!value)throw new AssertionError("projection scaling invariant failed");}
    static void run(int size){
        int updates=1000;var p=new SchoolProjection();int[] position={0};java.util.function.Consumer<Event> send=e->require(p.accept(List.of(envelope(++position[0],e))));
        long start=System.nanoTime();send.accept(new ClassOpened("a",Course.Foundation,size+updates+1));
        for(int i=0;i<size;i++){String id="s"+i;send.accept(new StudentRegistered(id,Grade.Year3));send.accept(new StudentEnrolled("a",id,Course.Foundation));}
        double seed=(System.nanoTime()-start)/1e9;var old=p.snapshot().state();int before=p.checkpoint();long work=p.work();
        require(!p.accept(List.of(envelope(before+1,new StudentPromoted("s0",Grade.Year1)),envelope(before+2,new AttendanceRecorded("a","s0")))));
        require(p.checkpoint()==before && p.work()==work && p.snapshot().state().students().stream().filter(s->s.studentId().equals("s0")).findFirst().orElseThrow().grade()==Grade.Year3);
        require(!p.accept(List.of(envelope(before+1,new StudentPromoted("s0",Grade.Year1)),envelope(before+3,new StudentPromoted("s0",Grade.Year1)))));
        require(p.checkpoint()==before && p.work()==work && p.accept(List.of(envelope(1,new ClassOpened("ignored",Course.Foundation,1)))) && p.work()==work);
        int stableRows=p.rowCount();for(int i=0;i<100;i++){send.accept(new StudentDropped("a","s1"));send.accept(new StudentEnrolled("a","s1",Course.Foundation));}
        var order=Decider.members(p.snapshot().state().classes().getFirst());require(p.rowCount()==stableRows && order.size()==size && order.getFirst().equals("s0") && order.get(1).equals("s2") && order.getLast().equals("s1"));
        start=System.nanoTime();work=p.work();
        for(int i=size;i<size+updates;i++){String id="s"+i;send.accept(new StudentRegistered(id,Grade.Year3));send.accept(new StudentEnrolled("a",id,Course.Foundation));}
        double growth=(System.nanoTime()-start)/1e9;long growthWork=p.work()-work;require(growthWork==8L*updates && Decider.members(old.classes().getFirst()).size()==size && old.students().size()==size);
        send.accept(new ClassStarted("a","2026-04-01"));send.accept(new StudyStarted("a","s0"));start=System.nanoTime();work=p.work();
        for(int i=0;i<updates;i++)send.accept(new AttendanceRecorded("a","s0"));
        double update=(System.nanoTime()-start)/1e9;long updateWork=p.work()-work;require(updateWork==3L*updates);
        start=System.nanoTime();var snapshot=p.snapshot().state();double query=(System.nanoTime()-start)/1e9;
        require(snapshot.students().size()==size+updates && Decider.members(snapshot.classes().getFirst()).size()==size+updates && snapshot.enrollments().stream().filter(e->e.studentId().equals("s0")).findFirst().orElseThrow().progress().equals(new Studying(updates)));
        System.out.printf(Locale.ROOT,"{\"size\":%d,\"updates\":%d,\"rows\":%d,\"seedSeconds\":%.9f,\"updateSeconds\":%.9f,\"growthSeconds\":%.9f,\"snapshotSeconds\":%.9f,\"updateWork\":%d,\"growthWork\":%d}%n",size,updates,p.rowCount(),seed,update,growth,query,updateWork,growthWork);
    }
}
