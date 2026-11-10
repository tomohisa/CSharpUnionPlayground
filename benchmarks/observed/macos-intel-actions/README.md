# Intel GitHub Actions observations

Native and WASM results are grouped by workload. Each implementation has one Native row and, where available, one WASM row. Native means host execution, including Node/JVM where applicable.

**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.

For C# and Rust, the overview uses the optimized Native control measured in the same run as WASM. Earlier host-only values remain in the detailed reports. This selects a build configuration, not the fastest observed sample across runs.

## JSON run

200,000 roots / 600,000 nodes per batch; 5 large batches per row.

[Host-only details](union100.md) / [Paired Native–WASM details](union100-wasi.md).

- [json measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880008976): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-json.json).
  [Generated source code](code/json/README.md) / [Source SHA256 manifest](code/json/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880014627): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

| implementation | execution | build | median batch s | encode s | decode s | peak RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | Native | Host defaults | 1.870 | 0.485 | 1.075 | 1738.7 | [json](environment-json.json) |
| [ts-manual](code/json/typescript/benchmark.ts) | Native | Host defaults | 1.874 | 0.461 | 1.216 | 1740.6 | [json](environment-json.json) |
| [rust](code/wasi/json/rust/main.rs) | WASM AOT | Release / LTO / SIMD | 2.565 | 0.668 | 1.884 | 902.7 | [wasi](environment-wasi.json) |
| [rust](code/wasi/json/rust/main.rs) | Native | Release / LTO / native ISA | 3.117 | 0.435 | 2.592 | 1210.7 | [wasi](environment-wasi.json) |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | Native | Host defaults | 4.015 | 0.451 | 3.176 | 2350.7 | [json](environment-json.json) |
| [csharp-closed-explicit](code/wasi/json/csharp-closed-explicit/Program.cs) | Native | Native AOT Speed / native ISA | 5.185 | 0.790 | 3.979 | 2153.4 | [wasi](environment-wasi.json) |
| [csharp-closed-inferred](code/wasi/json/csharp-closed-inferred/Program.cs) | Native | Native AOT Speed / native ISA | 5.409 | 0.777 | 4.107 | 2153.4 | [wasi](environment-wasi.json) |
| [csharp-abstract](code/wasi/json/csharp-abstract/Program.cs) | Native | Native AOT Speed / native ISA | 5.478 | 0.901 | 3.975 | 2159.6 | [wasi](environment-wasi.json) |
| [java](code/json/java/Benchmark.java) | Native | Host defaults | 5.553 | 1.324 | 1.917 | 3341.8 | [json](environment-json.json) |
| [go](code/json/go/benchmark.go) | Native | Host defaults | 8.686 | 2.517 | 5.960 | 1586.7 | [json](environment-json.json) |
| [csharp-union-custom](code/wasi/json/csharp-union-custom/Program.cs) | Native | Native AOT Speed / native ISA | 8.712 | 0.630 | 7.561 | 2220.4 | [wasi](environment-wasi.json) |
| [csharp-union-generated](code/wasi/json/csharp-union-generated/Program.cs) | Native | Native AOT Speed / native ISA | 10.045 | 1.295 | 8.683 | 2181.5 | [wasi](environment-wasi.json) |
| [csharp-closed-inferred](code/wasi/json/csharp-closed-inferred/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 21.461 | 3.503 | 13.814 | 2116.0 | [wasi](environment-wasi.json) |
| [csharp-abstract](code/wasi/json/csharp-abstract/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 25.839 | 3.700 | 17.589 | 2152.4 | [wasi](environment-wasi.json) |
| [csharp-closed-explicit](code/wasi/json/csharp-closed-explicit/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 25.935 | 3.078 | 17.376 | 2148.8 | [wasi](environment-wasi.json) |
| [swift](code/json/swift/Benchmark.swift) | Native | Host defaults | 34.980 | 18.099 | 16.089 | 2118.2 | [json](environment-json.json) |
| [csharp-union-custom](code/wasi/json/csharp-union-custom/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 41.198 | 4.001 | 32.816 | 2367.6 | [wasi](environment-wasi.json) |
| [csharp-union-generated](code/wasi/json/csharp-union-generated/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 56.782 | 12.822 | 37.887 | 2244.7 | [wasi](environment-wasi.json) |

Median batch time includes encode, decode and switch traversal. TypeScript runtime validation differs by variant; wire representations differ between some implementations. See the source and raw reports.

### JSON read-path diagnostic

[Intel: STJ metadata vs direct reader vs Rust](decoder/README.md). Same 100-case workload, separate matched Native/WASI AOT run; decode allocation and full lifecycle are retained.

## Projection run

`ts-simple` performs the projection without runtime payload validation. `ts-zod` validates/clones payload data in matching handlers, inside the projection timer. Unknown event kinds are ignored. This test does not parse JSON.

[Host domain details](projection-domain.md) / [Host fanout details](projection-fanout.md) / [Native–WASM domain details](projection-wasi-domain.md) / [Native–WASM fanout details](projection-wasi-fanout.md).

- [projection measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880008976): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-projection.json).
  [Generated source code](code/projection/README.md) / [Source SHA256 manifest](code/projection/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880014627): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

### Domain routing: ten million events, dense

| implementation | execution | build | append s | copy s | append RSS MiB | measurement |
|---|---|---|---:|---|---:|---|
| [csharp](code/wasi/projection/csharp/Program.cs) | Native | Native AOT Speed / native ISA | 0.844 | not-run-after-budget | 24.8 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | Native | Release / LTO / native ISA | 2.606 | timeout | 15.3 | [wasi](environment-wasi.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 2.345 | timeout | 169.4 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 3.192 | timeout | 176.1 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 1.116 | not-run-after-budget | 310.1 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 1.475 | timeout | 27.0 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 8.939 | timeout | 18.8 | [projection](environment-projection.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 3.469 | not-run-after-budget | 154.3 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 3.408 | timeout | 20.6 | [wasi](environment-wasi.json) |

### Sequential fanout: one million events, indexed-append

| implementation | execution | build | dense s | wide s | dense RSS MiB | wide RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [csharp](code/wasi/projection/csharp/Program.cs) | Native | Native AOT Speed / native ISA | 0.372 | 0.782 | 11.3 | 65.0 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | Native | Release / LTO / native ISA | 2.644 | 2.923 | 2.6 | 54.3 | [wasi](environment-wasi.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 0.508 | 1.506 | 97.8 | 223.8 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 0.586 | 0.873 | 100.6 | 266.0 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 0.939 | 1.006 | 90.9 | 163.0 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 0.383 | 0.429 | 7.6 | 49.0 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 8.128 | 8.514 | 2.6 | 29.9 | [projection](environment-projection.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 1.374 | 1.873 | 140.7 | 193.3 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 3.672 | 4.012 | 11.1 | 40.4 | [wasi](environment-wasi.json) |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

C# WASM uses LLVMOnly AOT with SIMD/-O3, without interpreter fallback. WASM RSS includes Wasmtime compilation/runtime; it is not guest heap alone. Host VM load, OS, toolchain and build differences affect results. Earlier host-only Native measurements remain available in the detail files and raw JSON.
