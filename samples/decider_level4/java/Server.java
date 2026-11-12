import com.sun.net.httpserver.*;
import com.fasterxml.jackson.databind.*;
import com.fasterxml.jackson.databind.node.*;
import playground.dcb.Dcb;
import playground.dcb.JsonStore;
import java.nio.file.*;
import java.net.*;
import java.util.*;
import java.util.concurrent.*;

public final class Server {
    static final ObjectMapper mapper=new ObjectMapper().enable(DeserializationFeature.FAIL_ON_NULL_FOR_PRIMITIVES,DeserializationFeature.FAIL_ON_MISSING_CREATOR_PROPERTIES);
    static JsonNode schemas;
    static final class HttpError extends RuntimeException{final int code;HttpError(int code,String message){super(message);this.code=code;}}
    static void bad(){throw new HttpError(400,"InvalidRequest");}
    static void validate(JsonNode body,JsonNode schema){
        if(schema.has("$ref")){validate(body,schemas.get(schema.get("$ref").asText().replace("#/components/schemas/","")));return;}
        if(schema.has("oneOf")){
            if(!body.isObject()||!body.path("$type").isTextual())bad();String kind=body.path("$type").asText();boolean found=false;
            for(var c:schema.get("oneOf"))if(c.get("$ref").asText().equals("#/components/schemas/"+kind))found=true;if(!found)bad();validate(body,schemas.get(kind));return;
        }
        switch(schema.get("type").asText()){
            case "object"->{if(!body.isObject())bad();for(var k:schema.get("required"))if(!body.has(k.asText()))bad();var fields=body.fields();while(fields.hasNext()){var f=fields.next();if(!schema.get("properties").has(f.getKey()))bad();validate(f.getValue(),schema.get("properties").get(f.getKey()));}}
            case "string"->{if(!body.isTextual())bad();var s=body.asText();if(schema.has("const")&&!schema.get("const").asText().equals(s))bad();if(schema.has("enum")){boolean found=false;for(var e:schema.get("enum"))if(e.asText().equals(s))found=true;if(!found)bad();}if(schema.has("minLength")&&(s.isBlank()||s.codePointCount(0,s.length())>128))bad();}
            case "integer"->{if(!body.isNumber()||body.doubleValue()!=Math.rint(body.doubleValue())||body.doubleValue()<Integer.MIN_VALUE||body.doubleValue()>Integer.MAX_VALUE)bad();}
        }
    }
    record Ticket(long created,Dcb.PreparedCommand<Rejection> command){}
    record Reply(int code,Object body){}
    public static void main(String[]args)throws Exception{
        schemas=mapper.readTree(Files.readString(Path.of(System.getenv("API_SPEC")))).get("components").get("schemas");
        var path=System.getenv("DECIDER_PERSISTENT");
        var store=new JsonStore<Event>(path==null?new JsonStore.Memory():new JsonStore.Sqlite(path,"FULL"),Routing::describe,
            e->{try{return mapper.writerFor(Event.class).writeValueAsString(e);}catch(Exception ex){throw new IllegalStateException(ex);}},
            s->{try{return mapper.readValue(s,Event.class);}catch(Exception ex){throw new IllegalStateException(ex);}});
        var projection=new SchoolProjection();store.subscribe(projection::accept);var executor=new Dcb.DcbExecutor<Event,Rejection>(store);var handler=new SchoolHandler();
        var tickets=new HashMap<String,Ticket>();var ticketGate=new Object();long ttl=(long)(Double.parseDouble(System.getenv().getOrDefault("TICKET_TTL_SECONDS","60"))*1e9);
        java.util.function.Consumer<Void> expire=ignored->tickets.entrySet().removeIf(e->System.nanoTime()-e.getValue().created()>=ttl);
        java.util.function.Function<Dcb.PreparedCommand<Rejection>,Reply> commit=t->{var r=t.commit();return new Reply(r.status().equals("Committed")?200:r.status().equals("Rejected")?422:409,Map.of("status",r.status(),"conflicts",r.conflicts(),"appended",r.appended(),"position",r.position(),"projectionPosition",projection.checkpoint(),"errors",r.errors()));};
        var server=HttpServer.create(new InetSocketAddress("127.0.0.1",Integer.parseInt(System.getenv().getOrDefault("PORT","8080"))),128);
        var pool=Executors.newVirtualThreadPerTaskExecutor();server.setExecutor(pool);
        server.createContext("/",exchange->{
            Reply reply;
            try{
                var route=exchange.getRequestURI().getPath();boolean get=Set.of("/health","/school","/status").contains(route)||route.startsWith("/students/");boolean post=Set.of("/commands","/tickets").contains(route)||route.matches("/tickets/[^/]+/commit");
                if(!get&&!post)throw new HttpError(404,"NotFound");String method=get?"GET":"POST";if(!exchange.getRequestMethod().equals(method)){exchange.getResponseHeaders().set("Allow",method);throw new HttpError(405,"MethodNotAllowed");}
                if(get){
                    store.refresh();if(store.listenerFaults()!=0)throw new IllegalStateException("projection unavailable");
                    if(route.equals("/health"))reply=new Reply(200,Map.of("status","ok"));
                    else if(route.equals("/status"))reply=new Reply(200,Map.of("eventPosition",store.read(new Dcb.Query(List.of())).after(),"projectionPosition",projection.checkpoint(),"listenerFaults",store.listenerFaults()));
                    else if(route.equals("/school")){var s=projection.snapshot();reply=new Reply(200,Map.of("position",s.position(),"state",s.state()));}
                    else{var s=projection.findStudent(route.substring(10));reply=new Reply(200,Map.of("position",s.position(),"student",s.state().orElseThrow(()->new HttpError(404,"NotFound"))));}
                }else if(route.equals("/commands")||route.equals("/tickets")){
                    var content=exchange.getRequestHeaders().getFirst("Content-Type");if(content==null||!content.split(";")[0].trim().equals("application/json"))throw new HttpError(415,"UnsupportedMediaType");
                    var raw=exchange.getRequestBody().readNBytes(1024*1024+1);if(raw.length>1024*1024)throw new HttpError(413,"PayloadTooLarge");
                    JsonNode input;try{input=mapper.readTree(raw);if(input==null)throw new Exception();validate(input,schemas.get("CommandRequest"));}catch(HttpError e){throw e;}catch(Exception e){throw new HttpError(400,"InvalidRequest");}
                    Command command;try{command=mapper.treeToValue(input.get("command"),Command.class);}catch(Exception e){throw new HttpError(400,"InvalidRequest");}
                    var t=executor.prepare(input.get("commandId").asText(),command,handler);
                    if(route.equals("/commands")||!t.wasRead())reply=commit.apply(t);
                    else{synchronized(ticketGate){expire.accept(null);if(tickets.size()>=1024)throw new HttpError(429,"TooManyTickets");var id=UUID.randomUUID().toString();tickets.put(id,new Ticket(System.nanoTime(),t));reply=new Reply(201,Map.of("ticket",id,"position",t.after()));}}
                }else{Ticket t;synchronized(ticketGate){expire.accept(null);t=tickets.remove(route.split("/")[2]);if(t==null)throw new HttpError(404,"NotFound");}reply=commit.apply(t.command());}
            }catch(HttpError e){reply=new Reply(e.code,Map.of("error",e.getMessage()));}catch(Exception e){e.printStackTrace();reply=new Reply(500,Map.of("error","InternalError"));}
            byte[]raw=mapper.writeValueAsBytes(reply.body());exchange.getResponseHeaders().set("Content-Type","application/json");exchange.sendResponseHeaders(reply.code(),raw.length);try(var output=exchange.getResponseBody()){output.write(raw);}exchange.close();
        });
        Runtime.getRuntime().addShutdownHook(new Thread(()->{server.stop(0);pool.close();store.close();}));server.start();
    }
}
