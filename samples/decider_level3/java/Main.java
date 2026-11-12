import playground.dcb.Dcb;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.List;
public final class Main{
    record Input(List<Dcb.Recorded<Event>> history,List<Operation> operations){public Input{history=List.copyOf(history);operations=List.copyOf(operations);}}
    public static void main(String[]args)throws Exception{
        if(java.util.Arrays.asList(args).contains("--domain-test")){DomainTests.run();return;}
        if(java.util.Arrays.asList(args).contains("--self-test")){DcbTests.run();System.out.println("{\"library\":\"PASS\"}");return;}
        var mapper=new ObjectMapper().enable(DeserializationFeature.FAIL_ON_NULL_FOR_PRIMITIVES,DeserializationFeature.FAIL_ON_MISSING_CREATOR_PROPERTIES);
        var input=mapper.readValue(System.in,Input.class);System.out.println(mapper.writeValueAsString(Application.run(input.history(),input.operations())));
    }
}
