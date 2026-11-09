// Run: python3 scripts/run_payment_java.py (Java 21+, Jackson)
import com.fasterxml.jackson.annotation.JsonSubTypes;
import com.fasterxml.jackson.annotation.JsonTypeInfo;
import com.fasterxml.jackson.databind.ObjectMapper;
import java.util.Objects;

class PaymentDemo {
    @JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
    @JsonSubTypes({
        @JsonSubTypes.Type(value = Cash.class, name = "cash"),
        @JsonSubTypes.Type(value = Card.class, name = "card")
    })
    sealed interface Payment permits Cash, Card {}
    record Cash(long amount, Payment next) implements Payment {}
    record Card(String cardNumber, long amount, Payment next) implements Payment {}

    // No default: javac checks every permitted case. null input throws NPE.
    static String describe(Payment payment) {
        return switch (payment) {
            case Cash cash -> "cash:" + cash.amount();
            case Card card -> "card:" + card.cardNumber();
        };
    }

    public static void main(String[] args) throws Exception {
        var mapper = new ObjectMapper();
        Payment original = new Cash(100, new Card("4111", 250, null));
        String json = mapper.writerFor(Payment.class).writeValueAsString(original);
        Payment restored = Objects.requireNonNull(mapper.readValue(json, Payment.class));
        System.out.println(json);
        if (!original.equals(restored)) throw new AssertionError("Round trip failed.");
        if (!(restored instanceof Cash cash) || !describe(cash).equals("cash:100")
                || !describe(cash.next()).equals("card:4111"))
            throw new AssertionError("Case matching failed.");
        System.out.println("switch: " + describe(restored) + " -> " + describe(((Cash) restored).next()));
        boolean rejected = false;
        try {
            mapper.readValue("{\"$type\":\"cash\",\"amount\":100,\"next\":{\"$type\":\"unknown\"}}", Payment.class);
        } catch (com.fasterxml.jackson.core.JsonProcessingException expected) { rejected = true; }
        if (!rejected) throw new AssertionError("Invalid nested Payment accepted.");
        System.out.println("PASS");
    }
}
