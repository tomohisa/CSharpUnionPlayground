# Intel GitHub Actions observations

Native and WASM results are grouped by workload. C# has separate Native AOT and JIT+PGO rows, plus a WASM row where available. Other implementations have one host row and a WASM row where available. Native means host execution, including Node/JVM where applicable.

**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.

JIT Native rows come from the host report; C# AOT rows come from the matched AOT report. WASM rows are included only when both reports belong to the same combined job/attempt. No minimum is selected across runs or C# runtime modes.

## JSON run

200,000 roots / 600,000 nodes per batch; 5 large batches per row.

[Host-only details](union100.md) / [WASM / legacy paired details](union100-wasi.md).

[C# Native AOT details](union100-aot.md).

- [json measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42924186934): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-json.json).
  [Generated source code](code/json/README.md) / [Source SHA256 manifest](code/json/manifest.json).
- [aot measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42924186934): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-aot.json).
  [Generated source code](code/aot/README.md) / [Source SHA256 manifest](code/aot/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42924186934): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

**One combined job / one VM / sequential processes.** C# Native compares JIT + Dynamic PGO and Native AOT side by side (abstract: .NET 10 for both; closed/union: .NET 11). WASM remains .NET 11 LLVMOnly AOT. These comparisons include runtime/version differences, not only the execution backend.

| implementation | execution | build | median batch s | encode s | decode s | peak RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [ts-manual](code/json/typescript/benchmark.ts) | Native | Host defaults | 1.574 | 0.388 | 1.003 | 1740.8 | [json](environment-json.json) |
| [rust](code/wasi/json/rust/main.rs) | WASM AOT | Release / LTO / SIMD | 1.785 | 0.456 | 1.308 | 902.6 | [wasi](environment-wasi.json) |
| [rust](code/json/rust/main.rs) | Native | Host defaults | 1.995 | 0.341 | 1.590 | 1195.6 | [json](environment-json.json) |
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | Native | Host defaults | 2.266 | 0.582 | 1.312 | 1738.2 | [json](environment-json.json) |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | Native | Host defaults | 3.583 | 0.490 | 2.912 | 2355.4 | [json](environment-json.json) |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | Native | .NET 10 / Release JIT + Dynamic PGO | 3.781 | 0.563 | 3.084 | 2811.4 | [json](environment-json.json) |
| [csharp-closed-explicit](code/aot/json/csharp-closed-explicit/Program.cs) | Native | .NET 11 / Native AOT Speed / native ISA | 3.790 | 0.595 | 3.030 | 2150.3 | [aot](environment-aot.json) |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 3.797 | 0.676 | 2.926 | 2328.2 | [json](environment-json.json) |
| [csharp-closed-inferred](code/aot/json/csharp-closed-inferred/Program.cs) | Native | .NET 11 / Native AOT Speed / native ISA | 4.118 | 0.729 | 3.154 | 2152.3 | [aot](environment-aot.json) |
| [csharp-abstract](code/aot/json/csharp-abstract/Program.cs) | Native | .NET 10 / Native AOT Speed / native ISA | 4.356 | 0.678 | 3.057 | 2607.0 | [aot](environment-aot.json) |
| [java](code/json/java/Benchmark.java) | Native | Host defaults | 4.450 | 0.906 | 1.433 | 3261.4 | [json](environment-json.json) |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 5.545 | 0.396 | 5.077 | 2362.9 | [json](environment-json.json) |
| [csharp-union-generated](code/aot/json/csharp-union-generated/Program.cs) | Native | .NET 11 / Native AOT Speed / native ISA | 6.309 | 0.844 | 5.087 | 2182.1 | [aot](environment-aot.json) |
| [csharp-union-custom](code/aot/json/csharp-union-custom/Program.cs) | Native | .NET 11 / Native AOT Speed / native ISA | 6.918 | 0.631 | 6.061 | 2085.2 | [aot](environment-aot.json) |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 8.815 | 1.891 | 6.838 | 2362.4 | [json](environment-json.json) |
| [go](code/json/go/benchmark.go) | Native | Host defaults | 9.937 | 3.293 | 6.515 | 1592.6 | [json](environment-json.json) |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 10.416 | 2.385 | 7.242 | 1823.6 | [json](environment-json.json) |
| [csharp-abstract](code/wasi/json/csharp-abstract/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 14.134 | 1.940 | 9.436 | 2249.7 | [wasi](environment-wasi.json) |
| [csharp-closed-explicit](code/wasi/json/csharp-closed-explicit/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 14.427 | 2.010 | 9.650 | 2165.3 | [wasi](environment-wasi.json) |
| [csharp-closed-inferred](code/wasi/json/csharp-closed-inferred/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 15.339 | 2.096 | 10.047 | 2168.6 | [wasi](environment-wasi.json) |
| [csharp-union-custom](code/wasi/json/csharp-union-custom/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 25.805 | 2.621 | 20.114 | 2543.0 | [wasi](environment-wasi.json) |
| [swift](code/json/swift/Benchmark.swift) | Native | Host defaults | 29.606 | 14.515 | 15.472 | 2122.5 | [json](environment-json.json) |
| [csharp-union-generated](code/wasi/json/csharp-union-generated/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 30.889 | 7.494 | 20.494 | 2598.8 | [wasi](environment-wasi.json) |

Median batch time includes encode, decode and switch traversal. TypeScript runtime validation differs by variant; wire representations differ between some implementations. See the source and raw reports.

### JSON read-path diagnostic

[Intel: STJ metadata vs direct reader vs Rust](decoder/README.md). Same 100-case workload, separate matched Native/WASI AOT run; decode allocation and full lifecycle are retained.

## Projection run

`ts-simple` performs the projection without runtime payload validation. `ts-zod` validates/clones payload data in matching handlers, inside the projection timer. Unknown event kinds are ignored. This test does not parse JSON.

[Host domain details](projection-domain.md) / [Host fanout details](projection-fanout.md) / [WASM domain details](projection-wasi-domain.md) / [WASM fanout details](projection-wasi-fanout.md).

[C# AOT domain details](projection-aot-domain.md) / [C# AOT fanout details](projection-aot-fanout.md).

- [projection measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42924186934): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-projection.json).
  [Generated source code](code/projection/README.md) / [Source SHA256 manifest](code/projection/manifest.json).
- [aot measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42924186934): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-aot.json).
  [Generated source code](code/aot/README.md) / [Source SHA256 manifest](code/aot/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42924186934): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

### Domain routing: ten million events, dense

| implementation | execution | build | append s | copy s | append RSS MiB | measurement |
|---|---|---|---:|---|---:|---|
| [csharp](code/projection/csharp/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 1.874 | timeout | 45.3 | [projection](environment-projection.json) |
| [rust](code/projection/rust/projection.rs) | Native | Host defaults | 2.916 | timeout | 15.3 | [projection](environment-projection.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 1.995 | timeout | 169.3 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 3.619 | timeout | 177.4 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 1.241 | timeout | 272.5 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 1.423 | timeout | 21.9 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 8.298 | timeout | 18.8 | [projection](environment-projection.json) |
| [csharp](code/aot/projection/csharp/Program.cs) | Native | .NET 11 / Native AOT Speed / native ISA | 0.873 | not-run-after-budget | 24.8 | [aot](environment-aot.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 2.494 | not-run-after-budget | 154.3 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 2.712 | timeout | 20.5 | [wasi](environment-wasi.json) |

### Sequential fanout: one million events, indexed-append

| implementation | execution | build | dense s | wide s | dense RSS MiB | wide RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [csharp](code/projection/csharp/Program.cs) | Native | .NET 11 / Release JIT + Dynamic PGO | 1.869 | 2.259 | 32.5 | 86.2 | [projection](environment-projection.json) |
| [rust](code/projection/rust/projection.rs) | Native | Host defaults | 1.889 | 2.475 | 2.6 | 54.3 | [projection](environment-projection.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 0.589 | 0.802 | 96.4 | 224.3 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 0.890 | 0.843 | 99.0 | 263.4 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 0.683 | 0.703 | 89.1 | 144.7 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 0.528 | 0.523 | 7.6 | 53.9 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 10.455 | 10.943 | 2.6 | 30.0 | [projection](environment-projection.json) |
| [csharp](code/aot/projection/csharp/Program.cs) | Native | .NET 11 / Native AOT Speed / native ISA | 0.262 | 0.484 | 11.2 | 65.0 | [aot](environment-aot.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 0.573 | 0.655 | 140.6 | 193.1 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 2.868 | 3.029 | 11.1 | 40.4 | [wasi](environment-wasi.json) |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

C# WASM uses LLVMOnly AOT with SIMD/-O3, without interpreter fallback. WASM RSS includes Wasmtime compilation/runtime; it is not guest heap alone. Host VM load, OS, toolchain and build differences affect results. Earlier host-only Native measurements remain available in the detail files and raw JSON.
