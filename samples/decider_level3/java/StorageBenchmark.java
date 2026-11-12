import playground.dcb.Dcb.*;
import playground.dcb.JsonStore;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.*;
final class StorageBenchmark{
 record DocumentRevision(String documentId,int revision,String body){}
 static void run(){int count=Integer.parseInt(Optional.ofNullable(System.getenv("DECIDER_DOCUMENTS")).orElse("0"));if(count<=0)return;var path=System.getenv("DECIDER_PERSISTENT");var mapper=new ObjectMapper();java.util.function.Supplier<JsonStore<DocumentRevision>>open=()->new JsonStore<>(path==null?new JsonStore.Memory():new JsonStore.Sqlite(path+".documents","FULL"),e->new EventDescriptor("DocumentRevision",List.of(new Tag("document",List.of(e.documentId())))),e->{try{return mapper.writeValueAsString(e);}catch(Exception error){throw new IllegalStateException(error);}},json->{try{return mapper.readValue(json,DocumentRevision.class);}catch(Exception error){throw new IllegalStateException(error);}});
 long start=System.nanoTime(),encoded,decoded;int position;var latest=new HashMap<String,Integer>();try(var s=open.get()){s.subscribe(events->{for(var e:events)latest.put(e.payload().documentId(),e.payload().revision());return true;});var body="x".repeat(4096);for(int revision=1;revision<=16;revision++)for(int i=0;i<count;i++){var id=String.format(Locale.ROOT,"document-%08d",i);var q=new Query(List.of(new QueryItem(List.of(new Tag("document",List.of(id))),List.of())));var r=s.read(q);if(r.events().size()!=revision-1||!s.append("doc-"+i+"-"+revision,q,r.after(),List.of(new DocumentRevision(id,revision,body))).committed())throw new IllegalStateException("document revision mismatch");}var all=s.read(new Query(List.of()));position=all.after();encoded=s.encoded;decoded=s.decoded;if(position!=16*count||latest.size()!=count||latest.values().stream().anyMatch(r->r!=16)||s.listenerFaults()!=0||all.events().stream().anyMatch(e->e.payload().body().length()!=4096))throw new IllegalStateException("document projection mismatch");}
 if(path!=null)try(var s=open.get()){var events=s.read(new Query(List.of())).events();if(events.size()!=16*count||events.stream().anyMatch(e->e.payload().body().length()!=4096))throw new IllegalStateException("document restart mismatch");}
 System.err.println("DOCUMENT "+(System.nanoTime()-start)/1e9+" "+count+" "+position+" "+encoded+" "+decoded);
 }
}
