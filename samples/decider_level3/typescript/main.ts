import {readFileSync} from "node:fs";
import {InputSchema} from "./wire.ts";
import {run} from "./application.ts";
const input=InputSchema.parse(JSON.parse(readFileSync(0,"utf8")));
console.log(JSON.stringify(run(input.history,input.operations)));
