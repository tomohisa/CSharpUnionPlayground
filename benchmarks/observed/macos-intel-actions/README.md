# Intel GitHub Actions observations

JSON serialization and memory projection are separate benchmarks. Each suite retains its own latest run and environment metadata.

## JSON run

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. RAM: 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42771617697) / [Environment](environment-json.json).

200,000 roots / 600,000 nodes per batch, 5 large batches. [Full timings and lifecycle](union100.md) / [Raw data](union100.json).

| implementation | median batch s | encode s | decode s | peak RSS MiB |
|---|---:|---:|---:|---:|
| typescript-simple | 1.387 | 0.358 | 0.836 | 1738.5 |
| typescript | 1.834 | 0.421 | 1.178 | 1740.4 |
| rust | 1.919 | 0.325 | 1.553 | 1180.2 |
| java | 2.620 | 0.865 | 1.103 | 3112.3 |
| csharp-closed-inferred | 3.254 | 0.529 | 2.561 | 2137.6 |
| csharp-abstract | 3.437 | 0.554 | 2.715 | 2123.8 |
| typescript-zod | 5.114 | 0.584 | 4.279 | 2350.0 |
| csharp-closed-explicit | 5.302 | 1.258 | 3.814 | 2149.0 |
| csharp-union-generated | 5.955 | 0.720 | 4.878 | 2156.6 |
| csharp-union-custom | 6.926 | 0.548 | 6.196 | 2150.1 |
| go | 7.408 | 2.190 | 5.114 | 1562.3 |
| swift | 29.482 | 14.850 | 14.192 | 2127.2 |

Counts, wire representations, runtime validation and runtime versions differ between implementations and ARM runs; inspect the raw report before comparing.

## Projection run

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. RAM: 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42775352886) / [Environment](environment-projection.json).

`typescript-native` is the simple projection. `typescript-zod-native` additionally validates/clones payload data only for matching handlers; this cost is included in projection time. Unknown event kinds are still ignored. No JSON parsing is included in this projection test.

[Domain details](projection-domain.md) / [Fanout details](projection-fanout.md).

Ten million events, domain routing, dense:

| implementation | append seconds | copy seconds | append RSS MiB |
|---|---:|---|---:|
| csharp-native | 0.688 | not-run-after-budget | 24.7 |
| rust-native | 1.987 | timeout | 15.3 |
| typescript-native | 1.251 | timeout | 170.0 |
| typescript-zod-native | 2.387 | timeout | 178.3 |
| java-native | 0.985 | timeout | 272.3 |
| go-native | 1.163 | timeout | 25.0 |
| swift-native | 8.627 | timeout | 18.9 |

One million events, sequential fanout, indexed-append:

| implementation | dense seconds | wide seconds | dense RSS MiB | wide RSS MiB |
|---|---:|---:|---:|---:|
| csharp-native | 0.317 | 1.373 | 11.3 | 65.0 |
| rust-native | 1.886 | 2.340 | 2.6 | 54.3 |
| typescript-native | 0.426 | 1.623 | 98.3 | 224.3 |
| typescript-zod-native | 0.483 | 1.212 | 102.0 | 262.9 |
| java-native | 0.478 | 0.588 | 90.8 | 163.4 |
| go-native | 0.299 | 0.327 | 7.6 | 53.7 |
| swift-native | 5.876 | 5.834 | 2.5 | 29.9 |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

RSS is the whole-process peak, not retained-state size. Hosted VM, OS and runtime differences mean these results do not isolate CPU architecture alone.
