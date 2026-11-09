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
        System.out.println(describe(restored));
    }
}
