# Intel GitHub Actions observations

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz. Runner: `macos-15-intel`. RAM: 14 GiB.

[Workflow run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42751950702). Source commit: `885c6a55cbfaf8a4f00331ae2cf02c96def9fc89`.

Projection reports completed and passed validation. The larger workflow was subsequently cancelled during the additional JSON suite; only completed projection data is published here. See environment.json for provenance.

## Ten million events, domain routing, dense

| implementation | append seconds | copy seconds | append RSS MiB |
|---|---:|---|---:|
| csharp-native | 1.006 | not-run-after-budget | 24.7 |
| rust-native | 2.868 | timeout | 15.3 |
| typescript-native | 2.198 | timeout | 171.3 |
| java-native | 1.379 | timeout | 273.1 |
| go-native | 1.737 | timeout | 22.9 |
| swift-native | 11.550 | timeout | 18.8 |

[Domain details](projection-domain.md) / [Fanout details](projection-fanout.md).

`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget; it is not a measured timeout at this size. C# copying exceeded the 8s projection budget at one million events, so ten million was not run. Swift append completed ten million, but exceeded the 8s projection budget.

Fanout: 139 successful conditions, 5 timeouts. Domain: 30 successful conditions, 5 timeouts, 1 point not run after the budget. No runtime errors or checksum failures.

RSS is the process peak, not retained-state size. Compare CPU, host VM, OS, toolchain versions and counts before comparing with ARM; this experiment does not isolate CPU architecture alone.
