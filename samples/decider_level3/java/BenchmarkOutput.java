import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.*;
final class BenchmarkOutput{
 record Summary(SchoolList list,boolean rebuildMatches,int eventCount,int checkpoint,int listenerFaults,Map<String,Integer>stepCounts,Map<String,Integer>errors,int conflicts){}
 static String serialize(Application.RunOutput o,ObjectMapper mapper)throws Exception{var steps=new TreeMap<String,Integer>();var errors=new TreeMap<String,Integer>();int conflicts=0;for(var s:o.steps()){steps.merge(s.status(),1,Integer::sum);for(var e:s.errors())errors.merge(e.toString(),1,Integer::sum);conflicts+=s.conflicts();}return mapper.writeValueAsString(new Summary(o.list(),mapper.writeValueAsString(o.list()).equals(mapper.writeValueAsString(o.rebuilt())),o.events().size(),o.checkpoint(),o.listenerFaults(),steps,errors,conflicts));}
}
