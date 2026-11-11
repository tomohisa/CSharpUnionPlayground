import { readFileSync } from "node:fs";
import { decide, describe, evolve, replay } from "./decider.ts";
import type { Decision } from "./models.ts";
import * as wire from "./wire.ts";
const input = wire.input(JSON.parse(readFileSync(0, "utf8")));
const history = [...input.history];
let state = replay(history);
const initial = state, steps: Decision[] = [];
for (const command of input.commands) {
  const decision = decide(state, command);
  steps.push(decision);
  if (decision.$type === "Accepted")
    for (const event of decision.events) { state = evolve(state, event); history.push(event); }
}
const events = wire.events(JSON.parse(JSON.stringify(history)));
const restored = wire.state(JSON.parse(JSON.stringify(state)));
console.log(JSON.stringify({ initial, state, events, steps, restored, replayed: replay(events), summaries: state.enrollments.map(e => describe(e.progress)) }));
