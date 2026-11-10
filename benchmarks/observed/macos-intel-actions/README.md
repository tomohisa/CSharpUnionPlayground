# Intel GitHub Actions observations

JSON serialization and memory projection are separate benchmarks. Each suite retains its own latest run and environment metadata.

**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.

## JSON run

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. RAM: 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42771617697) / [Environment](environment-json.json).

[Generated source code](code/json/README.md) / [Source SHA256 manifest](code/json/manifest.json).

200,000 roots / 600,000 nodes per batch, 5 large batches. [Full timings and lifecycle](union100.md) / [Raw data](union100.json).

| implementation | median batch s | encode s | decode s | peak RSS MiB |
|---|---:|---:|---:|---:|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 1.387 | 0.358 | 0.836 | 1738.5 |
| [ts-manual](code/json/typescript/benchmark.ts) | 1.834 | 0.421 | 1.178 | 1740.4 |
| [rust](code/json/rust/main.rs) | 1.919 | 0.325 | 1.553 | 1180.2 |
| [java](code/json/java/Benchmark.java) | 2.620 | 0.865 | 1.103 | 3112.3 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 3.254 | 0.529 | 2.561 | 2137.6 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 3.437 | 0.554 | 2.715 | 2123.8 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 5.114 | 0.584 | 4.279 | 2350.0 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 5.302 | 1.258 | 3.814 | 2149.0 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 5.955 | 0.720 | 4.878 | 2156.6 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 6.926 | 0.548 | 6.196 | 2150.1 |
| [go](code/json/go/benchmark.go) | 7.408 | 2.190 | 5.114 | 1562.3 |
| [swift](code/json/swift/Benchmark.swift) | 29.482 | 14.850 | 14.192 | 2127.2 |

Counts, wire representations, runtime validation and runtime versions differ between implementations and ARM runs; inspect the raw report before comparing.

## Projection run

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. RAM: 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42775352886) / [Environment](environment-projection.json).

[Generated source code](code/projection/README.md) / [Source SHA256 manifest](code/projection/manifest.json).

`ts-simple (native)` is the simple projection. `ts-zod (native)` additionally validates/clones payload data only for matching handlers; this cost is included in projection time. Unknown event kinds are still ignored. No JSON parsing is included in this projection test.

[Domain details](projection-domain.md) / [Fanout details](projection-fanout.md).

Ten million events, domain routing, dense:

| implementation | append seconds | copy seconds | append RSS MiB |
|---|---:|---|---:|
| [csharp-native](code/projection/csharp/Program.cs) | 0.688 | not-run-after-budget | 24.7 |
| [rust-native](code/projection/rust/projection.rs) | 1.987 | timeout | 15.3 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | 1.251 | timeout | 170.0 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | 2.387 | timeout | 178.3 |
| [java-native](code/projection/java/Projection.java) | 0.985 | timeout | 272.3 |
| [go-native](code/projection/go/projection.go) | 1.163 | timeout | 25.0 |
| [swift-native](code/projection/swift/Projection.swift) | 8.627 | timeout | 18.9 |

One million events, sequential fanout, indexed-append:

| implementation | dense seconds | wide seconds | dense RSS MiB | wide RSS MiB |
|---|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | 0.317 | 1.373 | 11.3 | 65.0 |
| [rust-native](code/projection/rust/projection.rs) | 1.886 | 2.340 | 2.6 | 54.3 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | 0.426 | 1.623 | 98.3 | 224.3 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | 0.483 | 1.212 | 102.0 | 262.9 |
| [java-native](code/projection/java/Projection.java) | 0.478 | 0.588 | 90.8 | 163.4 |
| [go-native](code/projection/go/projection.go) | 0.299 | 0.327 | 7.6 | 53.7 |
| [swift-native](code/projection/swift/Projection.swift) | 5.876 | 5.834 | 2.5 | 29.9 |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

## WASI run

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. RAM: 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42773487767) / [Environment](environment-wasi.json).

[Generated source code](code/wasi/README.md) / [Source SHA256 manifest](code/wasi/manifest.json).

200,000 roots / 600,000 nodes per batch, 5 large batches. [Full timings and lifecycle](union100-wasi.md) / [Raw data](union100-wasi.json).

| implementation | median batch s | encode s | decode s | peak RSS MiB |
|---|---:|---:|---:|---:|
| [csharp-abstract-native](code/wasi/json/csharp-abstract/Program.cs) | 4.759 | 0.731 | 3.409 | 2151.7 |
| [csharp-closed-explicit-native](code/wasi/json/csharp-closed-explicit/Program.cs) | 4.490 | 0.734 | 3.162 | 2151.4 |
| [csharp-closed-inferred-native](code/wasi/json/csharp-closed-inferred/Program.cs) | 4.023 | 0.613 | 2.976 | 2150.2 |
| [csharp-union-custom-native](code/wasi/json/csharp-union-custom/Program.cs) | 8.181 | 0.649 | 7.177 | 2019.9 |
| [csharp-union-generated-native](code/wasi/json/csharp-union-generated/Program.cs) | 8.604 | 0.998 | 7.022 | 2158.7 |
| [csharp-abstract-wasi](code/wasi/json/csharp-abstract/Program.cs) | 23.337 | 2.987 | 15.095 | 2211.7 |
| [csharp-closed-explicit-wasi](code/wasi/json/csharp-closed-explicit/Program.cs) | 19.782 | 2.809 | 13.405 | 2154.3 |
| [csharp-closed-inferred-wasi](code/wasi/json/csharp-closed-inferred/Program.cs) | 20.205 | 2.710 | 13.782 | 2203.2 |
| [csharp-union-custom-wasi](code/wasi/json/csharp-union-custom/Program.cs) | 35.617 | 3.404 | 28.768 | 2396.3 |
| [csharp-union-generated-wasi](code/wasi/json/csharp-union-generated/Program.cs) | 44.120 | 10.966 | 29.718 | 2469.9 |
| [rust-native](code/wasi/json/rust/main.rs) | 2.364 | 0.328 | 1.997 | 1192.8 |
| [rust-wasi](code/wasi/json/rust/main.rs) | 2.205 | 0.562 | 1.610 | 902.9 |

Counts, wire representations, runtime validation and runtime versions differ between implementations and ARM runs; inspect the raw report before comparing.

C# native is speed-optimized Native AOT; C# WASI is LLVMOnly AOT with SIMD/-O3, with no interpreter fallback. Rust uses optimized release/LTO with native ISA or Wasm SIMD. Wasmtime RSS includes the host runtime, not just guest memory.

[Domain details](projection-wasi-domain.md) / [Fanout details](projection-wasi-fanout.md).

Ten million events, domain routing, dense:

| implementation | append seconds | copy seconds | append RSS MiB |
|---|---:|---|---:|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | 0.920 | not-run-after-budget | 24.8 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | 2.893 | not-run-after-budget | 154.2 |
| [rust-native](code/wasi/projection/rust/projection.rs) | 2.797 | timeout | 15.2 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | 3.128 | timeout | 20.5 |

One million events, sequential fanout, indexed-append:

| implementation | dense seconds | wide seconds | dense RSS MiB | wide RSS MiB |
|---|---:|---:|---:|---:|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | 0.334 | 0.540 | 11.3 | 65.0 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | 0.751 | 0.794 | 144.8 | 194.8 |
| [rust-native](code/wasi/projection/rust/projection.rs) | 1.956 | 2.029 | 2.7 | 54.3 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | 3.027 | 3.409 | 11.2 | 40.5 |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

RSS is the whole-process peak, not retained-state size. Hosted VM, OS and runtime differences mean these results do not isolate CPU architecture alone.
