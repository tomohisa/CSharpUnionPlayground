// Run: node samples/file-based/Payment.ts
// JSONの形をそのままunionで定義。型アサーションだけでは読み込みを検証できない。
type Payment =
  | { $type: "cash"; amount: number; next: Payment | null }
  | { $type: "card"; cardNumber: string; amount: number; next: Payment | null };

function isPayment(value: unknown): value is Payment {
  if (typeof value !== "object" || value === null) return false;
  const p = value as Record<string, unknown>;
  return typeof p.amount === "number" && Number.isFinite(p.amount)
    && (p.$type === "cash" || (p.$type === "card" && typeof p.cardNumber === "string"))
    && (p.next === null || isPayment(p.next));
}

function deserialize(json: string): Payment {
  const value: unknown = JSON.parse(json);
  if (!isPayment(value)) throw new Error("Invalid Payment JSON.");
  return value;
}

const original: Payment = {
  $type: "cash", amount: 100,
  next: { $type: "card", cardNumber: "4111", amount: 250, next: null }
};
const json = JSON.stringify(original);
const restored = deserialize(json);
console.log(json);
if (restored.$type !== "cash" || restored.amount !== 100 ||
    restored.next?.$type !== "card" || restored.next.cardNumber !== "4111" ||
    restored.next.amount !== 250 || restored.next.next !== null)
  throw new Error("Round trip failed.");
// 内側のケースも検証される。
let rejected = false;
try { deserialize('{"$type":"cash","amount":100,"next":{"$type":"unknown"}}'); }
catch { rejected = true; }
if (!rejected) throw new Error("Invalid nested Payment was accepted.");
console.log("PASS");
