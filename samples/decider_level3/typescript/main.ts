import {benchmarkOutput} from "./benchmarkOutput.ts";
import {runProjectionTests} from "./projection.test.ts";
import {runDomainTests} from "./domain.test.ts";
import {readFileSync} from "node:fs";
import {InputSchema} from "./wire.ts";
import {run} from "./application.ts";
if(process.argv[2]==="--projection-scale"){runProjectionTests(Number(process.argv[3]));}else if(process.argv.includes("--domain-test")){runDomainTests();}else{
const input=InputSchema.parse(JSON.parse(readFileSync(0,"utf8")));
const output=run(input.history,input.operations);
console.log(process.env.DECIDER_BENCHMARK_SUMMARY==="1"?benchmarkOutput(output):JSON.stringify(output));

}
