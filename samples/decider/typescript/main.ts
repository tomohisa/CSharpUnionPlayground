import { readFileSync } from "node:fs";
import { decide, evolve, phase, replay } from "./decider.ts";
import * as wire from "./wire.ts";

const input = wire.input(JSON.parse(readFileSync(0, "utf8")));
const history = [...input.history];
let state = replay(history);
const initial = state;
const steps: { phase: string; emitted: string[]; error: string | null }[] = [];
for (const command of input.commands) {
  const decision = decide(state, command);
  if (decision.ok) {
    for (const event of decision.events) {
      state = evolve(state, event);
      history.push(event);
    }
    steps.push({ phase: phase(state), emitted: decision.events.map(e => e.$type), error: null });
  } else {
    steps.push({ phase: phase(state), emitted: [], error: decision.error });
  }
}
const events = wire.events(JSON.parse(JSON.stringify(history)));
const restored = wire.state(JSON.parse(JSON.stringify(state)));
console.log(JSON.stringify({ initial, state, events, steps, restored, replayed: replay(events) }));
