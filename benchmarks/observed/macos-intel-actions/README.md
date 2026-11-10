# Intel GitHub Actions observations

Native and WASM results are grouped by workload. Each implementation has one Native row and, where available, one WASM row. Native means host execution, including Node/JVM where applicable.

**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.

The canonical Native rows come from the host report. WASM rows are included only when both reports belong to the same combined job/attempt. No minimum is selected across runs.

## JSON run

200,000 roots / 600,000 nodes per batch; 5 large batches per row.

[Host-only details](union100.md) / [WASM / legacy paired details](union100-wasi.md).

- [json measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42904383468): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-json.json).
  [Generated source code](code/json/README.md) / [Source SHA256 manifest](code/json/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42904383468): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

**One combined job / one VM / sequential processes.** C# Native uses JIT + Dynamic PGO (abstract: .NET 10; closed/union: .NET 11). WASM remains .NET 11 LLVMOnly AOT. These comparisons include runtime/version differences, not only the execution backend.

| implementation | execution | build | median batch s | encode s | decode s | peak RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | Native | Host defaults | 1.676 | 0.414 | 0.985 | 1739.1 | [json](environment-json.json) |
| [rust](code/wasi/json/rust/main.rs) | WASM AOT | Release / LTO / SIMD | 1.762 | 0.441 | 1.290 | 902.6 | [wasi](environment-wasi.json) |
| [ts-manual](code/json/typescript/benchmark.ts) | Native | Host defaults | 1.787 | 0.453 | 1.157 | 1740.2 | [json](environment-json.json) |
| [rust](code/json/rust/main.rs) | Native | Host defaults | 2.021 | 0.465 | 1.574 | 1144.9 | [json](environment-json.json) |
| [java](code/json/java/Benchmark.java) | Native | Host defaults | 3.163 | 0.984 | 1.492 | 3375.5 | [json](environment-json.json) |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | Native | Host defaults | 3.561 | 0.393 | 2.789 | 2358.1 | [json](environment-json.json) |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 3.647 | 0.698 | 2.832 | 2331.8 | [json](environment-json.json) |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | Native | .NET 10 / Release JIT + Dynamic PGO | 3.911 | 0.687 | 3.237 | 2815.1 | [json](environment-json.json) |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 5.669 | 0.507 | 5.098 | 2358.9 | [json](environment-json.json) |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 6.889 | 0.801 | 5.962 | 2364.8 | [json](environment-json.json) |
| [go](code/json/go/benchmark.go) | Native | Host defaults | 8.662 | 2.531 | 6.065 | 1734.9 | [json](environment-json.json) |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 11.338 | 3.031 | 8.638 | 2307.7 | [json](environment-json.json) |
| [csharp-closed-inferred](code/wasi/json/csharp-closed-inferred/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 15.323 | 2.211 | 10.086 | 2170.8 | [wasi](environment-wasi.json) |
| [csharp-closed-explicit](code/wasi/json/csharp-closed-explicit/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 15.684 | 2.119 | 10.849 | 2137.7 | [wasi](environment-wasi.json) |
| [csharp-abstract](code/wasi/json/csharp-abstract/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 18.351 | 2.485 | 12.332 | 2115.9 | [wasi](environment-wasi.json) |
| [swift](code/json/swift/Benchmark.swift) | Native | Host defaults | 29.790 | 14.120 | 14.521 | 2138.0 | [json](environment-json.json) |
| [csharp-union-generated](code/wasi/json/csharp-union-generated/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 30.126 | 7.423 | 20.027 | 2208.4 | [wasi](environment-wasi.json) |
| [csharp-union-custom](code/wasi/json/csharp-union-custom/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 30.358 | 2.811 | 24.084 | 2374.4 | [wasi](environment-wasi.json) |

Median batch time includes encode, decode and switch traversal. TypeScript runtime validation differs by variant; wire representations differ between some implementations. See the source and raw reports.

### JSON read-path diagnostic

[Intel: STJ metadata vs direct reader vs Rust](decoder/README.md). Same 100-case workload, separate matched Native/WASI AOT run; decode allocation and full lifecycle are retained.

## Projection run

`ts-simple` performs the projection without runtime payload validation. `ts-zod` validates/clones payload data in matching handlers, inside the projection timer. Unknown event kinds are ignored. This test does not parse JSON.

[Host domain details](projection-domain.md) / [Host fanout details](projection-fanout.md) / [WASM domain details](projection-wasi-domain.md) / [WASM fanout details](projection-wasi-fanout.md).

- [projection measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42904383468): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-projection.json).
  [Generated source code](code/projection/README.md) / [Source SHA256 manifest](code/projection/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42904383468): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

### Domain routing: ten million events, dense

| implementation | execution | build | append s | copy s | append RSS MiB | measurement |
|---|---|---|---:|---|---:|---|
| [csharp](code/projection/csharp/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 1.188 | timeout | 45.5 | [projection](environment-projection.json) |
| [rust](code/projection/rust/projection.rs) | Native | Host defaults | 2.373 | timeout | 15.3 | [projection](environment-projection.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 1.374 | timeout | 169.1 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 2.844 | timeout | 180.9 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 1.086 | timeout | 274.0 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 1.275 | timeout | 23.2 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 7.607 | timeout | 18.9 | [projection](environment-projection.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 1.988 | not-run-after-budget | 154.3 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 2.356 | timeout | 20.6 | [wasi](environment-wasi.json) |

### Sequential fanout: one million events, indexed-append

| implementation | execution | build | dense s | wide s | dense RSS MiB | wide RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [csharp](code/projection/csharp/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 1.304 | 1.722 | 32.5 | 86.2 | [projection](environment-projection.json) |
| [rust](code/projection/rust/projection.rs) | Native | Host defaults | 2.068 | 2.113 | 2.6 | 54.3 | [projection](environment-projection.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 0.514 | 0.735 | 97.2 | 223.8 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 1.082 | 3.641 | 101.8 | 264.8 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 0.775 | 0.789 | 91.4 | 145.2 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 0.394 | 0.515 | 7.5 | 50.6 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 6.564 | 6.829 | 2.5 | 29.9 | [projection](environment-projection.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 0.568 | 0.770 | 144.8 | 193.2 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 2.907 | 3.187 | 11.1 | 40.4 | [wasi](environment-wasi.json) |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

C# WASM uses LLVMOnly AOT with SIMD/-O3, without interpreter fallback. WASM RSS includes Wasmtime compilation/runtime; it is not guest heap alone. Host VM load, OS, toolchain and build differences affect results. Earlier host-only Native measurements remain available in the detail files and raw JSON.
