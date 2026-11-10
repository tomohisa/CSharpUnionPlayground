import com.fasterxml.jackson.annotation.*;
import com.fasterxml.jackson.databind.*;
import java.util.*;

class Benchmark {
    record Payload(int id, String label, int[] values, Map<String,String> tags, Payment next) {}
    @JsonTypeInfo(use=JsonTypeInfo.Id.NAME, property="$type")
    @JsonSubTypes({
        @JsonSubTypes.Type(value=Case000.class, name="Case000"),
        @JsonSubTypes.Type(value=Case001.class, name="Case001"),
        @JsonSubTypes.Type(value=Case002.class, name="Case002"),
        @JsonSubTypes.Type(value=Case003.class, name="Case003"),
        @JsonSubTypes.Type(value=Case004.class, name="Case004"),
        @JsonSubTypes.Type(value=Case005.class, name="Case005"),
        @JsonSubTypes.Type(value=Case006.class, name="Case006"),
        @JsonSubTypes.Type(value=Case007.class, name="Case007"),
        @JsonSubTypes.Type(value=Case008.class, name="Case008"),
        @JsonSubTypes.Type(value=Case009.class, name="Case009"),
        @JsonSubTypes.Type(value=Case010.class, name="Case010"),
        @JsonSubTypes.Type(value=Case011.class, name="Case011"),
        @JsonSubTypes.Type(value=Case012.class, name="Case012"),
        @JsonSubTypes.Type(value=Case013.class, name="Case013"),
        @JsonSubTypes.Type(value=Case014.class, name="Case014"),
        @JsonSubTypes.Type(value=Case015.class, name="Case015"),
        @JsonSubTypes.Type(value=Case016.class, name="Case016"),
        @JsonSubTypes.Type(value=Case017.class, name="Case017"),
        @JsonSubTypes.Type(value=Case018.class, name="Case018"),
        @JsonSubTypes.Type(value=Case019.class, name="Case019"),
        @JsonSubTypes.Type(value=Case020.class, name="Case020"),
        @JsonSubTypes.Type(value=Case021.class, name="Case021"),
        @JsonSubTypes.Type(value=Case022.class, name="Case022"),
        @JsonSubTypes.Type(value=Case023.class, name="Case023"),
        @JsonSubTypes.Type(value=Case024.class, name="Case024"),
        @JsonSubTypes.Type(value=Case025.class, name="Case025"),
        @JsonSubTypes.Type(value=Case026.class, name="Case026"),
        @JsonSubTypes.Type(value=Case027.class, name="Case027"),
        @JsonSubTypes.Type(value=Case028.class, name="Case028"),
        @JsonSubTypes.Type(value=Case029.class, name="Case029"),
        @JsonSubTypes.Type(value=Case030.class, name="Case030"),
        @JsonSubTypes.Type(value=Case031.class, name="Case031"),
        @JsonSubTypes.Type(value=Case032.class, name="Case032"),
        @JsonSubTypes.Type(value=Case033.class, name="Case033"),
        @JsonSubTypes.Type(value=Case034.class, name="Case034"),
        @JsonSubTypes.Type(value=Case035.class, name="Case035"),
        @JsonSubTypes.Type(value=Case036.class, name="Case036"),
        @JsonSubTypes.Type(value=Case037.class, name="Case037"),
        @JsonSubTypes.Type(value=Case038.class, name="Case038"),
        @JsonSubTypes.Type(value=Case039.class, name="Case039"),
        @JsonSubTypes.Type(value=Case040.class, name="Case040"),
        @JsonSubTypes.Type(value=Case041.class, name="Case041"),
        @JsonSubTypes.Type(value=Case042.class, name="Case042"),
        @JsonSubTypes.Type(value=Case043.class, name="Case043"),
        @JsonSubTypes.Type(value=Case044.class, name="Case044"),
        @JsonSubTypes.Type(value=Case045.class, name="Case045"),
        @JsonSubTypes.Type(value=Case046.class, name="Case046"),
        @JsonSubTypes.Type(value=Case047.class, name="Case047"),
        @JsonSubTypes.Type(value=Case048.class, name="Case048"),
        @JsonSubTypes.Type(value=Case049.class, name="Case049"),
        @JsonSubTypes.Type(value=Case050.class, name="Case050"),
        @JsonSubTypes.Type(value=Case051.class, name="Case051"),
        @JsonSubTypes.Type(value=Case052.class, name="Case052"),
        @JsonSubTypes.Type(value=Case053.class, name="Case053"),
        @JsonSubTypes.Type(value=Case054.class, name="Case054"),
        @JsonSubTypes.Type(value=Case055.class, name="Case055"),
        @JsonSubTypes.Type(value=Case056.class, name="Case056"),
        @JsonSubTypes.Type(value=Case057.class, name="Case057"),
        @JsonSubTypes.Type(value=Case058.class, name="Case058"),
        @JsonSubTypes.Type(value=Case059.class, name="Case059"),
        @JsonSubTypes.Type(value=Case060.class, name="Case060"),
        @JsonSubTypes.Type(value=Case061.class, name="Case061"),
        @JsonSubTypes.Type(value=Case062.class, name="Case062"),
        @JsonSubTypes.Type(value=Case063.class, name="Case063"),
        @JsonSubTypes.Type(value=Case064.class, name="Case064"),
        @JsonSubTypes.Type(value=Case065.class, name="Case065"),
        @JsonSubTypes.Type(value=Case066.class, name="Case066"),
        @JsonSubTypes.Type(value=Case067.class, name="Case067"),
        @JsonSubTypes.Type(value=Case068.class, name="Case068"),
        @JsonSubTypes.Type(value=Case069.class, name="Case069"),
        @JsonSubTypes.Type(value=Case070.class, name="Case070"),
        @JsonSubTypes.Type(value=Case071.class, name="Case071"),
        @JsonSubTypes.Type(value=Case072.class, name="Case072"),
        @JsonSubTypes.Type(value=Case073.class, name="Case073"),
        @JsonSubTypes.Type(value=Case074.class, name="Case074"),
        @JsonSubTypes.Type(value=Case075.class, name="Case075"),
        @JsonSubTypes.Type(value=Case076.class, name="Case076"),
        @JsonSubTypes.Type(value=Case077.class, name="Case077"),
        @JsonSubTypes.Type(value=Case078.class, name="Case078"),
        @JsonSubTypes.Type(value=Case079.class, name="Case079"),
        @JsonSubTypes.Type(value=Case080.class, name="Case080"),
        @JsonSubTypes.Type(value=Case081.class, name="Case081"),
        @JsonSubTypes.Type(value=Case082.class, name="Case082"),
        @JsonSubTypes.Type(value=Case083.class, name="Case083"),
        @JsonSubTypes.Type(value=Case084.class, name="Case084"),
        @JsonSubTypes.Type(value=Case085.class, name="Case085"),
        @JsonSubTypes.Type(value=Case086.class, name="Case086"),
        @JsonSubTypes.Type(value=Case087.class, name="Case087"),
        @JsonSubTypes.Type(value=Case088.class, name="Case088"),
        @JsonSubTypes.Type(value=Case089.class, name="Case089"),
        @JsonSubTypes.Type(value=Case090.class, name="Case090"),
        @JsonSubTypes.Type(value=Case091.class, name="Case091"),
        @JsonSubTypes.Type(value=Case092.class, name="Case092"),
        @JsonSubTypes.Type(value=Case093.class, name="Case093"),
        @JsonSubTypes.Type(value=Case094.class, name="Case094"),
        @JsonSubTypes.Type(value=Case095.class, name="Case095"),
        @JsonSubTypes.Type(value=Case096.class, name="Case096"),
        @JsonSubTypes.Type(value=Case097.class, name="Case097"),
        @JsonSubTypes.Type(value=Case098.class, name="Case098"),
        @JsonSubTypes.Type(value=Case099.class, name="Case099")
    })
    sealed interface Payment permits Case000, Case001, Case002, Case003, Case004, Case005, Case006, Case007, Case008, Case009, Case010, Case011, Case012, Case013, Case014, Case015, Case016, Case017, Case018, Case019, Case020, Case021, Case022, Case023, Case024, Case025, Case026, Case027, Case028, Case029, Case030, Case031, Case032, Case033, Case034, Case035, Case036, Case037, Case038, Case039, Case040, Case041, Case042, Case043, Case044, Case045, Case046, Case047, Case048, Case049, Case050, Case051, Case052, Case053, Case054, Case055, Case056, Case057, Case058, Case059, Case060, Case061, Case062, Case063, Case064, Case065, Case066, Case067, Case068, Case069, Case070, Case071, Case072, Case073, Case074, Case075, Case076, Case077, Case078, Case079, Case080, Case081, Case082, Case083, Case084, Case085, Case086, Case087, Case088, Case089, Case090, Case091, Case092, Case093, Case094, Case095, Case096, Case097, Case098, Case099 {}
    record Case000(Payload data) implements Payment {}
    record Case001(Payload data) implements Payment {}
    record Case002(Payload data) implements Payment {}
    record Case003(Payload data) implements Payment {}
    record Case004(Payload data) implements Payment {}
    record Case005(Payload data) implements Payment {}
    record Case006(Payload data) implements Payment {}
    record Case007(Payload data) implements Payment {}
    record Case008(Payload data) implements Payment {}
    record Case009(Payload data) implements Payment {}
    record Case010(Payload data) implements Payment {}
    record Case011(Payload data) implements Payment {}
    record Case012(Payload data) implements Payment {}
    record Case013(Payload data) implements Payment {}
    record Case014(Payload data) implements Payment {}
    record Case015(Payload data) implements Payment {}
    record Case016(Payload data) implements Payment {}
    record Case017(Payload data) implements Payment {}
    record Case018(Payload data) implements Payment {}
    record Case019(Payload data) implements Payment {}
    record Case020(Payload data) implements Payment {}
    record Case021(Payload data) implements Payment {}
    record Case022(Payload data) implements Payment {}
    record Case023(Payload data) implements Payment {}
    record Case024(Payload data) implements Payment {}
    record Case025(Payload data) implements Payment {}
    record Case026(Payload data) implements Payment {}
    record Case027(Payload data) implements Payment {}
    record Case028(Payload data) implements Payment {}
    record Case029(Payload data) implements Payment {}
    record Case030(Payload data) implements Payment {}
    record Case031(Payload data) implements Payment {}
    record Case032(Payload data) implements Payment {}
    record Case033(Payload data) implements Payment {}
    record Case034(Payload data) implements Payment {}
    record Case035(Payload data) implements Payment {}
    record Case036(Payload data) implements Payment {}
    record Case037(Payload data) implements Payment {}
    record Case038(Payload data) implements Payment {}
    record Case039(Payload data) implements Payment {}
    record Case040(Payload data) implements Payment {}
    record Case041(Payload data) implements Payment {}
    record Case042(Payload data) implements Payment {}
    record Case043(Payload data) implements Payment {}
    record Case044(Payload data) implements Payment {}
    record Case045(Payload data) implements Payment {}
    record Case046(Payload data) implements Payment {}
    record Case047(Payload data) implements Payment {}
    record Case048(Payload data) implements Payment {}
    record Case049(Payload data) implements Payment {}
    record Case050(Payload data) implements Payment {}
    record Case051(Payload data) implements Payment {}
    record Case052(Payload data) implements Payment {}
    record Case053(Payload data) implements Payment {}
    record Case054(Payload data) implements Payment {}
    record Case055(Payload data) implements Payment {}
    record Case056(Payload data) implements Payment {}
    record Case057(Payload data) implements Payment {}
    record Case058(Payload data) implements Payment {}
    record Case059(Payload data) implements Payment {}
    record Case060(Payload data) implements Payment {}
    record Case061(Payload data) implements Payment {}
    record Case062(Payload data) implements Payment {}
    record Case063(Payload data) implements Payment {}
    record Case064(Payload data) implements Payment {}
    record Case065(Payload data) implements Payment {}
    record Case066(Payload data) implements Payment {}
    record Case067(Payload data) implements Payment {}
    record Case068(Payload data) implements Payment {}
    record Case069(Payload data) implements Payment {}
    record Case070(Payload data) implements Payment {}
    record Case071(Payload data) implements Payment {}
    record Case072(Payload data) implements Payment {}
    record Case073(Payload data) implements Payment {}
    record Case074(Payload data) implements Payment {}
    record Case075(Payload data) implements Payment {}
    record Case076(Payload data) implements Payment {}
    record Case077(Payload data) implements Payment {}
    record Case078(Payload data) implements Payment {}
    record Case079(Payload data) implements Payment {}
    record Case080(Payload data) implements Payment {}
    record Case081(Payload data) implements Payment {}
    record Case082(Payload data) implements Payment {}
    record Case083(Payload data) implements Payment {}
    record Case084(Payload data) implements Payment {}
    record Case085(Payload data) implements Payment {}
    record Case086(Payload data) implements Payment {}
    record Case087(Payload data) implements Payment {}
    record Case088(Payload data) implements Payment {}
    record Case089(Payload data) implements Payment {}
    record Case090(Payload data) implements Payment {}
    record Case091(Payload data) implements Payment {}
    record Case092(Payload data) implements Payment {}
    record Case093(Payload data) implements Payment {}
    record Case094(Payload data) implements Payment {}
    record Case095(Payload data) implements Payment {}
    record Case096(Payload data) implements Payment {}
    record Case097(Payload data) implements Payment {}
    record Case098(Payload data) implements Payment {}
    record Case099(Payload data) implements Payment {}
    static Payment create(int seed,int depth) {
        var tags=new LinkedHashMap<String,String>();tags.put("group","g"+(seed%7));tags.put("note","bench");
        var data=new Payload(seed,"item-"+seed,new int[]{seed,seed+1,seed+2,seed+3},tags,depth>1?create(seed+37,depth-1):null);
        return switch(seed%100){
            case 0 -> new Case000(data);
            case 1 -> new Case001(data);
            case 2 -> new Case002(data);
            case 3 -> new Case003(data);
            case 4 -> new Case004(data);
            case 5 -> new Case005(data);
            case 6 -> new Case006(data);
            case 7 -> new Case007(data);
            case 8 -> new Case008(data);
            case 9 -> new Case009(data);
            case 10 -> new Case010(data);
            case 11 -> new Case011(data);
            case 12 -> new Case012(data);
            case 13 -> new Case013(data);
            case 14 -> new Case014(data);
            case 15 -> new Case015(data);
            case 16 -> new Case016(data);
            case 17 -> new Case017(data);
            case 18 -> new Case018(data);
            case 19 -> new Case019(data);
            case 20 -> new Case020(data);
            case 21 -> new Case021(data);
            case 22 -> new Case022(data);
            case 23 -> new Case023(data);
            case 24 -> new Case024(data);
            case 25 -> new Case025(data);
            case 26 -> new Case026(data);
            case 27 -> new Case027(data);
            case 28 -> new Case028(data);
            case 29 -> new Case029(data);
            case 30 -> new Case030(data);
            case 31 -> new Case031(data);
            case 32 -> new Case032(data);
            case 33 -> new Case033(data);
            case 34 -> new Case034(data);
            case 35 -> new Case035(data);
            case 36 -> new Case036(data);
            case 37 -> new Case037(data);
            case 38 -> new Case038(data);
            case 39 -> new Case039(data);
            case 40 -> new Case040(data);
            case 41 -> new Case041(data);
            case 42 -> new Case042(data);
            case 43 -> new Case043(data);
            case 44 -> new Case044(data);
            case 45 -> new Case045(data);
            case 46 -> new Case046(data);
            case 47 -> new Case047(data);
            case 48 -> new Case048(data);
            case 49 -> new Case049(data);
            case 50 -> new Case050(data);
            case 51 -> new Case051(data);
            case 52 -> new Case052(data);
            case 53 -> new Case053(data);
            case 54 -> new Case054(data);
            case 55 -> new Case055(data);
            case 56 -> new Case056(data);
            case 57 -> new Case057(data);
            case 58 -> new Case058(data);
            case 59 -> new Case059(data);
            case 60 -> new Case060(data);
            case 61 -> new Case061(data);
            case 62 -> new Case062(data);
            case 63 -> new Case063(data);
            case 64 -> new Case064(data);
            case 65 -> new Case065(data);
            case 66 -> new Case066(data);
            case 67 -> new Case067(data);
            case 68 -> new Case068(data);
            case 69 -> new Case069(data);
            case 70 -> new Case070(data);
            case 71 -> new Case071(data);
            case 72 -> new Case072(data);
            case 73 -> new Case073(data);
            case 74 -> new Case074(data);
            case 75 -> new Case075(data);
            case 76 -> new Case076(data);
            case 77 -> new Case077(data);
            case 78 -> new Case078(data);
            case 79 -> new Case079(data);
            case 80 -> new Case080(data);
            case 81 -> new Case081(data);
            case 82 -> new Case082(data);
            case 83 -> new Case083(data);
            case 84 -> new Case084(data);
            case 85 -> new Case085(data);
            case 86 -> new Case086(data);
            case 87 -> new Case087(data);
            case 88 -> new Case088(data);
            case 89 -> new Case089(data);
            case 90 -> new Case090(data);
            case 91 -> new Case091(data);
            case 92 -> new Case092(data);
            case 93 -> new Case093(data);
            case 94 -> new Case094(data);
            case 95 -> new Case095(data);
            case 96 -> new Case096(data);
            case 97 -> new Case097(data);
            case 98 -> new Case098(data);
            case 99 -> new Case099(data);
            default -> throw new IllegalArgumentException();
        };
    }
    static long score(Payment payment) {
        return switch(payment){
            case Case000 v -> payloadScore(v.data(), 0);
            case Case001 v -> payloadScore(v.data(), 1);
            case Case002 v -> payloadScore(v.data(), 2);
            case Case003 v -> payloadScore(v.data(), 3);
            case Case004 v -> payloadScore(v.data(), 4);
            case Case005 v -> payloadScore(v.data(), 5);
            case Case006 v -> payloadScore(v.data(), 6);
            case Case007 v -> payloadScore(v.data(), 7);
            case Case008 v -> payloadScore(v.data(), 8);
            case Case009 v -> payloadScore(v.data(), 9);
            case Case010 v -> payloadScore(v.data(), 10);
            case Case011 v -> payloadScore(v.data(), 11);
            case Case012 v -> payloadScore(v.data(), 12);
            case Case013 v -> payloadScore(v.data(), 13);
            case Case014 v -> payloadScore(v.data(), 14);
            case Case015 v -> payloadScore(v.data(), 15);
            case Case016 v -> payloadScore(v.data(), 16);
            case Case017 v -> payloadScore(v.data(), 17);
            case Case018 v -> payloadScore(v.data(), 18);
            case Case019 v -> payloadScore(v.data(), 19);
            case Case020 v -> payloadScore(v.data(), 20);
            case Case021 v -> payloadScore(v.data(), 21);
            case Case022 v -> payloadScore(v.data(), 22);
            case Case023 v -> payloadScore(v.data(), 23);
            case Case024 v -> payloadScore(v.data(), 24);
            case Case025 v -> payloadScore(v.data(), 25);
            case Case026 v -> payloadScore(v.data(), 26);
            case Case027 v -> payloadScore(v.data(), 27);
            case Case028 v -> payloadScore(v.data(), 28);
            case Case029 v -> payloadScore(v.data(), 29);
            case Case030 v -> payloadScore(v.data(), 30);
            case Case031 v -> payloadScore(v.data(), 31);
            case Case032 v -> payloadScore(v.data(), 32);
            case Case033 v -> payloadScore(v.data(), 33);
            case Case034 v -> payloadScore(v.data(), 34);
            case Case035 v -> payloadScore(v.data(), 35);
            case Case036 v -> payloadScore(v.data(), 36);
            case Case037 v -> payloadScore(v.data(), 37);
            case Case038 v -> payloadScore(v.data(), 38);
            case Case039 v -> payloadScore(v.data(), 39);
            case Case040 v -> payloadScore(v.data(), 40);
            case Case041 v -> payloadScore(v.data(), 41);
            case Case042 v -> payloadScore(v.data(), 42);
            case Case043 v -> payloadScore(v.data(), 43);
            case Case044 v -> payloadScore(v.data(), 44);
            case Case045 v -> payloadScore(v.data(), 45);
            case Case046 v -> payloadScore(v.data(), 46);
            case Case047 v -> payloadScore(v.data(), 47);
            case Case048 v -> payloadScore(v.data(), 48);
            case Case049 v -> payloadScore(v.data(), 49);
            case Case050 v -> payloadScore(v.data(), 50);
            case Case051 v -> payloadScore(v.data(), 51);
            case Case052 v -> payloadScore(v.data(), 52);
            case Case053 v -> payloadScore(v.data(), 53);
            case Case054 v -> payloadScore(v.data(), 54);
            case Case055 v -> payloadScore(v.data(), 55);
            case Case056 v -> payloadScore(v.data(), 56);
            case Case057 v -> payloadScore(v.data(), 57);
            case Case058 v -> payloadScore(v.data(), 58);
            case Case059 v -> payloadScore(v.data(), 59);
            case Case060 v -> payloadScore(v.data(), 60);
            case Case061 v -> payloadScore(v.data(), 61);
            case Case062 v -> payloadScore(v.data(), 62);
            case Case063 v -> payloadScore(v.data(), 63);
            case Case064 v -> payloadScore(v.data(), 64);
            case Case065 v -> payloadScore(v.data(), 65);
            case Case066 v -> payloadScore(v.data(), 66);
            case Case067 v -> payloadScore(v.data(), 67);
            case Case068 v -> payloadScore(v.data(), 68);
            case Case069 v -> payloadScore(v.data(), 69);
            case Case070 v -> payloadScore(v.data(), 70);
            case Case071 v -> payloadScore(v.data(), 71);
            case Case072 v -> payloadScore(v.data(), 72);
            case Case073 v -> payloadScore(v.data(), 73);
            case Case074 v -> payloadScore(v.data(), 74);
            case Case075 v -> payloadScore(v.data(), 75);
            case Case076 v -> payloadScore(v.data(), 76);
            case Case077 v -> payloadScore(v.data(), 77);
            case Case078 v -> payloadScore(v.data(), 78);
            case Case079 v -> payloadScore(v.data(), 79);
            case Case080 v -> payloadScore(v.data(), 80);
            case Case081 v -> payloadScore(v.data(), 81);
            case Case082 v -> payloadScore(v.data(), 82);
            case Case083 v -> payloadScore(v.data(), 83);
            case Case084 v -> payloadScore(v.data(), 84);
            case Case085 v -> payloadScore(v.data(), 85);
            case Case086 v -> payloadScore(v.data(), 86);
            case Case087 v -> payloadScore(v.data(), 87);
            case Case088 v -> payloadScore(v.data(), 88);
            case Case089 v -> payloadScore(v.data(), 89);
            case Case090 v -> payloadScore(v.data(), 90);
            case Case091 v -> payloadScore(v.data(), 91);
            case Case092 v -> payloadScore(v.data(), 92);
            case Case093 v -> payloadScore(v.data(), 93);
            case Case094 v -> payloadScore(v.data(), 94);
            case Case095 v -> payloadScore(v.data(), 95);
            case Case096 v -> payloadScore(v.data(), 96);
            case Case097 v -> payloadScore(v.data(), 97);
            case Case098 v -> payloadScore(v.data(), 98);
            case Case099 v -> payloadScore(v.data(), 99);
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
