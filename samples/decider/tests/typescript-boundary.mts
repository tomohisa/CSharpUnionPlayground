import assert from "node:assert/strict";
import { test } from "node:test";
import { input, state } from "../typescript/wire.ts";
import { decide } from "../typescript/decider.ts";

test("reject malformed command fields and unknown tags at the JSON boundary", () => {
  for (const command of [
    { $type: "Unknown" },
    { $type: "Enroll", studentId: 123 },
    { $type: "Drop" },
    { $type: "Create", name: "Lab", capacity: 1.5 },
    { $type: "Create", name: "Lab", capacity: Number.MAX_SAFE_INTEGER + 1 },
    null
  ]) assert.throws(() => input({ commands: [command] }));
  assert.throws(() => input({ commands: [], history: [{ $type: "StudentEnrolled", studentId: null }] }));
  assert.throws(() => state({ $type: "Full", roster: { name: "Lab", capacity: 2, students: [42] } }));
});
test("preserve null history and extra-field handling", () => {
  assert.deepEqual(input({ commands: [], history: null, extra: true }), { commands: [], history: [] });
});
test("business rejection stays in the decider after structural parsing", () => {
  const parsed = input({ commands: [{ $type: "Create", name: "Lab", capacity: 0 }] });
  assert.deepEqual(decide({ $type: "NotCreated" }, parsed.commands[0]), { ok: false, error: "invalid_capacity" });
});
test("readonly schemas freeze parsed state, roster and student array", () => {
  const parsed = state({ $type: "Available", roster: { name: "Lab", capacity: 2, students: [] } });
  assert.ok(Object.isFrozen(parsed));
  if (parsed.$type !== "NotCreated") {
    assert.ok(Object.isFrozen(parsed.roster));
    assert.ok(Object.isFrozen(parsed.roster.students));
  }
});
