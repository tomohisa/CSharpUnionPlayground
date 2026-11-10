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

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. RAM: 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42751950702) / [Environment](environment-projection.json).

Both projection reports completed and passed validation before cancellation during the additional JSON suite. JSON results from that run are excluded. Projection reports are imported from the run artifact; this snapshot was not committed by the cancelled workflow publication job.

[Domain details](projection-domain.md) / [Fanout details](projection-fanout.md).

Ten million events, domain routing, dense:

| implementation | append seconds | copy seconds | append RSS MiB |
|---|---:|---|---:|
| csharp-native | 1.006 | not-run-after-budget | 24.7 |
| rust-native | 2.868 | timeout | 15.3 |
| typescript-native | 2.198 | timeout | 171.3 |
| java-native | 1.379 | timeout | 273.1 |
| go-native | 1.737 | timeout | 22.9 |
| swift-native | 11.550 | timeout | 18.8 |

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.

RSS is the whole-process peak, not retained-state size. Hosted VM, OS and runtime differences mean these results do not isolate CPU architecture alone.
