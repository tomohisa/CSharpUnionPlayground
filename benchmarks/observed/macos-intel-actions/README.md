# Intel GitHub Actions observations

Native and WASM results are grouped by workload. Each implementation has one Native row and, where available, one WASM row. Native means host execution, including Node/JVM where applicable.

**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.

The canonical Native rows come from the host report. WASM rows are included only when both reports belong to the same combined job/attempt. No minimum is selected across runs.

## JSON run

200,000 roots / 600,000 nodes per batch; 5 large batches per row.

[Host-only details](union100.md) / [WASM / legacy paired details](union100-wasi.md).

- [json measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880008976): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-json.json).
  [Generated source code](code/json/README.md) / [Source SHA256 manifest](code/json/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880014627): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

**Unmatched runs:** this table shows host Native results only. WASM details are retained at the link above, but are excluded from this ranking until one completed combined job supplies both backends.

| implementation | execution | build | median batch s | encode s | decode s | peak RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | Native | Host defaults | 1.870 | 0.485 | 1.075 | 1738.7 | [json](environment-json.json) |
| [ts-manual](code/json/typescript/benchmark.ts) | Native | Host defaults | 1.874 | 0.461 | 1.216 | 1740.6 | [json](environment-json.json) |
| [rust](code/json/rust/main.rs) | Native | Host defaults | 2.137 | 0.359 | 1.741 | 1268.6 | [json](environment-json.json) |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | Native | Release Native AOT / SDK defaults | 3.700 | 0.748 | 2.807 | 2126.7 | [json](environment-json.json) |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | Native | Host defaults | 4.015 | 0.451 | 3.176 | 2350.7 | [json](environment-json.json) |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | Native | Release Native AOT / SDK defaults | 4.423 | 0.701 | 3.674 | 2122.7 | [json](environment-json.json) |
| [java](code/json/java/Benchmark.java) | Native | Host defaults | 5.553 | 1.324 | 1.917 | 3341.8 | [json](environment-json.json) |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | Native | Release Native AOT / SDK defaults | 7.304 | 0.535 | 6.574 | 1990.4 | [json](environment-json.json) |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | Native | Release Native AOT / SDK defaults | 7.602 | 1.647 | 6.127 | 2145.2 | [json](environment-json.json) |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | Native | Release Native AOT / SDK defaults | 8.097 | 0.944 | 6.895 | 2168.7 | [json](environment-json.json) |
| [go](code/json/go/benchmark.go) | Native | Host defaults | 8.686 | 2.517 | 5.960 | 1586.7 | [json](environment-json.json) |
| [swift](code/json/swift/Benchmark.swift) | Native | Host defaults | 34.980 | 18.099 | 16.089 | 2118.2 | [json](environment-json.json) |

Median batch time includes encode, decode and switch traversal. TypeScript runtime validation differs by variant; wire representations differ between some implementations. See the source and raw reports.

### JSON read-path diagnostic

[Intel: STJ metadata vs direct reader vs Rust](decoder/README.md). Same 100-case workload, separate matched Native/WASI AOT run; decode allocation and full lifecycle are retained.

## Projection run

`ts-simple` performs the projection without runtime payload validation. `ts-zod` validates/clones payload data in matching handlers, inside the projection timer. Unknown event kinds are ignored. This test does not parse JSON.

[Host domain details](projection-domain.md) / [Host fanout details](projection-fanout.md) / [WASM domain details](projection-wasi-domain.md) / [WASM fanout details](projection-wasi-fanout.md).

- [projection measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880008976): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-projection.json).
  [Generated source code](code/projection/README.md) / [Source SHA256 manifest](code/projection/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880014627): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

**Unmatched runs:** only host Native rows are ranked here. Separate WASM measurements remain in the detail reports.

### Domain routing: ten million events, dense

| implementation | execution | build | append s | copy s | append RSS MiB | measurement |
|---|---|---|---:|---|---:|---|
| [csharp](code/projection/csharp/Program.cs) | Native | Release Native AOT / SDK defaults | 1.067 | not-run-after-budget | 24.8 | [projection](environment-projection.json) |
| [rust](code/projection/rust/projection.rs) | Native | Host defaults | 2.692 | timeout | 15.3 | [projection](environment-projection.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 2.345 | timeout | 169.4 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 3.192 | timeout | 176.1 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 1.116 | not-run-after-budget | 310.1 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 1.475 | timeout | 27.0 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 8.939 | timeout | 18.8 | [projection](environment-projection.json) |

### Sequential fanout: one million events, indexed-append

| implementation | execution | build | dense s | wide s | dense RSS MiB | wide RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [csharp](code/projection/csharp/Program.cs) | Native | Release Native AOT / SDK defaults | 0.370 | 0.923 | 11.3 | 64.9 | [projection](environment-projection.json) |
| [rust](code/projection/rust/projection.rs) | Native | Host defaults | 2.289 | 2.331 | 2.7 | 54.3 | [projection](environment-projection.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 0.508 | 1.506 | 97.8 | 223.8 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 0.586 | 0.873 | 100.6 | 266.0 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 0.939 | 1.006 | 90.9 | 163.0 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 0.383 | 0.429 | 7.6 | 49.0 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 8.128 | 8.514 | 2.6 | 29.9 | [projection](environment-projection.json) |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

C# WASM uses LLVMOnly AOT with SIMD/-O3, without interpreter fallback. WASM RSS includes Wasmtime compilation/runtime; it is not guest heap alone. Host VM load, OS, toolchain and build differences affect results. Earlier host-only Native measurements remain available in the detail files and raw JSON.
