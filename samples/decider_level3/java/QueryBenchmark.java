import playground.dcb.Dcb.*;
import playground.dcb.JsonStore;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.*;
final class QueryBenchmark{
 static void run(JsonStore<Event> store,SchoolProjection projection){int n=Integer.parseInt(Optional.ofNullable(System.getenv("DECIDER_STUDENTS")).orElse("0"));if(n<=0)return;long start=System.nanoTime();for(int i=0;i<1000;i++){var id=String.format(Locale.ROOT,"student-%08d",(long)i*7919%n);if(store.read(new Query(List.of(new QueryItem(List.of(new Tag("student",List.of(id))),List.of())))).events().isEmpty())throw new IllegalStateException("point read missing");}System.err.println("PHASE tagged_point "+(System.nanoTime()-start)/1e9);start=System.nanoTime();var mapper=new ObjectMapper();for(int i=0;i<1000;i++){var id=String.format(Locale.ROOT,"student-%08d",(long)i*7919%n);try{if(!mapper.readValue(mapper.writeValueAsString(projection.student(id)),Student.class).studentId().equals(id))throw new IllegalStateException("point query mismatch");}catch(Exception e){throw new IllegalStateException(e);}}System.err.println("PHASE projection_point_json "+(System.nanoTime()-start)/1e9);}
}
