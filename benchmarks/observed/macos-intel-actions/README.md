# Intel GitHub Actions observations

Native and WASM results are grouped by workload. Each implementation has one Native row and, where available, one WASM row. Native means host execution, including Node/JVM where applicable.

**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.

For C# and Rust, the overview uses the optimized Native control measured in the same run as WASM. Earlier host-only values remain in the detailed reports. This selects a build configuration, not the fastest observed sample across runs.

## JSON run

200,000 roots / 600,000 nodes per batch; 5 large batches per row.

[Host-only details](union100.md) / [Paired Native–WASM details](union100-wasi.md).

- [json measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42771617697): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-json.json).
  [Generated source code](code/json/README.md) / [Source SHA256 manifest](code/json/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42773487767): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

| implementation | execution | build | median batch s | encode s | decode s | peak RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | Native | Host defaults | 1.387 | 0.358 | 0.836 | 1738.5 | [json](environment-json.json) |
| [ts-manual](code/json/typescript/benchmark.ts) | Native | Host defaults | 1.834 | 0.421 | 1.178 | 1740.4 | [json](environment-json.json) |
| [rust](code/wasi/json/rust/main.rs) | WASM AOT | Release / LTO / SIMD | 2.205 | 0.562 | 1.610 | 902.9 | [wasi](environment-wasi.json) |
| [rust](code/wasi/json/rust/main.rs) | Native | Release / LTO / native ISA | 2.364 | 0.328 | 1.997 | 1192.8 | [wasi](environment-wasi.json) |
| [java](code/json/java/Benchmark.java) | Native | Host defaults | 2.620 | 0.865 | 1.103 | 3112.3 | [json](environment-json.json) |
| [csharp-closed-inferred](code/wasi/json/csharp-closed-inferred/Program.cs) | Native | Native AOT Speed / native ISA | 4.023 | 0.613 | 2.976 | 2150.2 | [wasi](environment-wasi.json) |
| [csharp-closed-explicit](code/wasi/json/csharp-closed-explicit/Program.cs) | Native | Native AOT Speed / native ISA | 4.490 | 0.734 | 3.162 | 2151.4 | [wasi](environment-wasi.json) |
| [csharp-abstract](code/wasi/json/csharp-abstract/Program.cs) | Native | Native AOT Speed / native ISA | 4.759 | 0.731 | 3.409 | 2151.7 | [wasi](environment-wasi.json) |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | Native | Host defaults | 5.114 | 0.584 | 4.279 | 2350.0 | [json](environment-json.json) |
| [go](code/json/go/benchmark.go) | Native | Host defaults | 7.408 | 2.190 | 5.114 | 1562.3 | [json](environment-json.json) |
| [csharp-union-custom](code/wasi/json/csharp-union-custom/Program.cs) | Native | Native AOT Speed / native ISA | 8.181 | 0.649 | 7.177 | 2019.9 | [wasi](environment-wasi.json) |
| [csharp-union-generated](code/wasi/json/csharp-union-generated/Program.cs) | Native | Native AOT Speed / native ISA | 8.604 | 0.998 | 7.022 | 2158.7 | [wasi](environment-wasi.json) |
| [csharp-closed-explicit](code/wasi/json/csharp-closed-explicit/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 19.782 | 2.809 | 13.405 | 2154.3 | [wasi](environment-wasi.json) |
| [csharp-closed-inferred](code/wasi/json/csharp-closed-inferred/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 20.205 | 2.710 | 13.782 | 2203.2 | [wasi](environment-wasi.json) |
| [csharp-abstract](code/wasi/json/csharp-abstract/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 23.337 | 2.987 | 15.095 | 2211.7 | [wasi](environment-wasi.json) |
| [swift](code/json/swift/Benchmark.swift) | Native | Host defaults | 29.482 | 14.850 | 14.192 | 2127.2 | [json](environment-json.json) |
| [csharp-union-custom](code/wasi/json/csharp-union-custom/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 35.617 | 3.404 | 28.768 | 2396.3 | [wasi](environment-wasi.json) |
| [csharp-union-generated](code/wasi/json/csharp-union-generated/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 44.120 | 10.966 | 29.718 | 2469.9 | [wasi](environment-wasi.json) |

Median batch time includes encode, decode and switch traversal. TypeScript runtime validation differs by variant; wire representations differ between some implementations. See the source and raw reports.

### JSON read-path diagnostic

[Intel: STJ metadata vs direct reader vs Rust](decoder/README.md). Same 100-case workload, separate matched Native/WASI AOT run; decode allocation and full lifecycle are retained.

## Projection run

`ts-simple` performs the projection without runtime payload validation. `ts-zod` validates/clones payload data in matching handlers, inside the projection timer. Unknown event kinds are ignored. This test does not parse JSON.

[Host domain details](projection-domain.md) / [Host fanout details](projection-fanout.md) / [Native–WASM domain details](projection-wasi-domain.md) / [Native–WASM fanout details](projection-wasi-fanout.md).

- [projection measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42775352886): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-projection.json).
  [Generated source code](code/projection/README.md) / [Source SHA256 manifest](code/projection/manifest.json).
- [wasi measurement](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42773487767): Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz, 14 GiB; [environment](environment-wasi.json).
  [Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

### Domain routing: ten million events, dense

| implementation | execution | build | append s | copy s | append RSS MiB | measurement |
|---|---|---|---:|---|---:|---|
| [csharp](code/wasi/projection/csharp/Program.cs) | Native | Native AOT Speed / native ISA | 0.920 | not-run-after-budget | 24.8 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | Native | Release / LTO / native ISA | 2.797 | timeout | 15.2 | [wasi](environment-wasi.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 1.251 | timeout | 170.0 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 2.387 | timeout | 178.3 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 0.985 | timeout | 272.3 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 1.163 | timeout | 25.0 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 8.627 | timeout | 18.9 | [projection](environment-projection.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 2.893 | not-run-after-budget | 154.2 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 3.128 | timeout | 20.5 | [wasi](environment-wasi.json) |

### Sequential fanout: one million events, indexed-append

| implementation | execution | build | dense s | wide s | dense RSS MiB | wide RSS MiB | measurement |
|---|---|---|---:|---:|---:|---:|---|
| [csharp](code/wasi/projection/csharp/Program.cs) | Native | Native AOT Speed / native ISA | 0.334 | 0.540 | 11.3 | 65.0 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | Native | Release / LTO / native ISA | 1.956 | 2.029 | 2.7 | 54.3 | [wasi](environment-wasi.json) |
| [ts-simple](code/projection/typescript/projection.ts) | Native | Host defaults | 0.426 | 1.623 | 98.3 | 224.3 | [projection](environment-projection.json) |
| [ts-zod](code/projection/typescript-zod/projection-zod.ts) | Native | Host defaults | 0.483 | 1.212 | 102.0 | 262.9 | [projection](environment-projection.json) |
| [java](code/projection/java/Projection.java) | Native | Host defaults | 0.478 | 0.588 | 90.8 | 163.4 | [projection](environment-projection.json) |
| [go](code/projection/go/projection.go) | Native | Host defaults | 0.299 | 0.327 | 7.6 | 53.7 | [projection](environment-projection.json) |
| [swift](code/projection/swift/Projection.swift) | Native | Host defaults | 5.876 | 5.834 | 2.5 | 29.9 | [projection](environment-projection.json) |
| [csharp](code/wasi/projection/csharp/Program.cs) | WASM AOT | LLVMOnly AOT / SIMD / -O3 | 0.751 | 0.794 | 144.8 | 194.8 | [wasi](environment-wasi.json) |
| [rust](code/wasi/projection/rust/projection.rs) | WASM AOT | Release / LTO / SIMD | 3.027 | 3.409 | 11.2 | 40.5 | [wasi](environment-wasi.json) |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

C# WASM uses LLVMOnly AOT with SIMD/-O3, without interpreter fallback. WASM RSS includes Wasmtime compilation/runtime; it is not guest heap alone. Host VM load, OS, toolchain and build differences affect results. Earlier host-only Native measurements remain available in the detail files and raw JSON.
