# Intel GitHub Actions observations

JSON serialization and memory projection are separate benchmarks. Each suite retains its own latest run and environment metadata.

## JSON run

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. RAM: 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42755191904) / [Environment](environment-json.json).

200,000 roots / 600,000 nodes per batch, 5 large batches. [Full timings and lifecycle](union100.md) / [Raw data](union100.json).

| implementation | median batch s | encode s | decode s | peak RSS MiB |
|---|---:|---:|---:|---:|
| typescript | 1.788 | 0.392 | 1.218 | 1658.2 |
| rust | 1.886 | 0.325 | 1.526 | 1219.8 |
| csharp-closed-explicit | 3.860 | 0.618 | 3.065 | 2162.1 |
| csharp-abstract | 3.967 | 0.613 | 3.098 | 2122.5 |
| csharp-closed-inferred | 3.975 | 0.610 | 3.175 | 2125.7 |
| java | 4.159 | 1.087 | 2.293 | 2910.0 |
| csharp-union-generated | 7.449 | 0.825 | 6.044 | 2167.0 |
| go | 9.545 | 2.972 | 6.465 | 1615.6 |
| csharp-union-custom | 10.793 | 1.190 | 9.329 | 2189.1 |
| swift | 32.337 | 15.928 | 16.154 | 2112.7 |

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
