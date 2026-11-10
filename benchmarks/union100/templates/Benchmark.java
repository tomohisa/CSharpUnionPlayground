import com.fasterxml.jackson.annotation.*;
import com.fasterxml.jackson.databind.*;
import java.util.*;

class Benchmark {
    record Payload(int id, String label, int[] values, Map<String,String> tags, Payment next) {}
    @JsonTypeInfo(use=JsonTypeInfo.Id.NAME, property="$type")
    @JsonSubTypes({
@@ATTRS@@
    })
    sealed interface Payment permits @@UNION@@ {}
@@TYPES@@
    static Payment create(int seed,int depth) {
        var tags=new LinkedHashMap<String,String>();tags.put("group","g"+(seed%7));tags.put("note","bench");
        var data=new Payload(seed,"item-"+seed,new int[]{seed,seed+1,seed+2,seed+3},tags,depth>1?create(seed+37,depth-1):null);
        return switch(seed%100){
@@CREATE@@
            default -> throw new IllegalArgumentException();
        };
    }
    static long score(Payment payment) {
        return switch(payment){
@@DISPATCH@@
        };
    }
    static long payloadScore(Payload data,int index){
        long sum=index+data.id()+data.label().length();
        for(int v:data.values())sum+=v;
        for(var entry:data.tags().entrySet())sum+=entry.getKey().length()+entry.getValue().length();
        return sum+(data.next()!=null?score(data.next()):0);
    }
    static Payment[] batch(int roots){var result=new Payment[roots];for(int i=0;i<roots;i++)result[i]=create(i,3);return result;}
    public static void main(String[] args)throws Exception{
        System.out.println("{\"event\":\"ready\"}");System.out.flush();
        int roots=Integer.parseInt(args[0]),samples=Integer.parseInt(args[1]);
        var mapper=new ObjectMapper();var writer=mapper.writerFor(Payment[].class);
        mapper.readValue(writer.writeValueAsBytes(batch(Math.min(roots,1000))),Payment[].class);
        var original=batch(roots);long expected=0,sink=0;for(var p:original)expected+=score(p);
        var measurements=new ArrayList<Map<String,Object>>();
        System.out.println("{\"event\":\"prepared\"}");System.out.flush();
        for(int trial=0;trial<samples;trial++){
            long start=System.nanoTime();byte[] json=writer.writeValueAsBytes(original);long encoded=System.nanoTime();
            var copy=mapper.readValue(json,Payment[].class);long decoded=System.nanoTime();
            long sum=0;for(var p:copy)sum+=score(p);long end=System.nanoTime();
            if(sum!=expected||copy.length!=roots)throw new AssertionError("Incorrect round trip");
            if(trial==0&&!Arrays.equals(json,writer.writeValueAsBytes(copy)))throw new AssertionError("Data changed");
            sink+=sum;
            measurements.add(Map.of("encodeSeconds",(encoded-start)/1e9,"decodeSeconds",(decoded-encoded)/1e9,"switchSeconds",(end-decoded)/1e9,"totalSeconds",(end-start)/1e9,"bytes",json.length));
        }
        System.out.println(mapper.writeValueAsString(Map.of("roots",roots,"samples",samples,"sink",sink,"measurements",measurements)));
    }
}
