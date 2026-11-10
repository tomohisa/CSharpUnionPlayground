// Setup: npm ci --prefix samples/file-based
// Run: node samples/file-based/PaymentZod.ts
import { z } from "zod";

const PaymentSchema = z.discriminatedUnion("$type", [
  z.object({
    $type: z.literal("cash"), amount: z.number(),
    get next(): z.ZodNullable<typeof PaymentSchema> { return PaymentSchema.nullable(); }
  }),
  z.object({
    $type: z.literal("card"), cardNumber: z.string(), amount: z.number(),
    get next(): z.ZodNullable<typeof PaymentSchema> { return PaymentSchema.nullable(); }
  })
]);
type Payment = z.infer<typeof PaymentSchema>;

function describe(payment: Payment): string {
  switch (payment.$type) {
    case "cash": return `cash:${payment.amount}`;
    case "card": return `card:${payment.cardNumber}`;
    default: {
      const exhaustive: never = payment;
      throw new Error(`Unhandled Payment: ${exhaustive}`);
    }
  }
}

const original: Payment = {
  $type: "cash", amount: 100,
  next: { $type: "card", cardNumber: "4111", amount: 250, next: null }
};
const json = JSON.stringify(original);
const restored = PaymentSchema.parse(JSON.parse(json));
console.log(json);
console.log(describe(restored));
