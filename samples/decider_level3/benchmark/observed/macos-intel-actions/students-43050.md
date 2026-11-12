# Level 3 ES benchmark observations

Measured: 2026-11-12T06:06:10.693922+00:00

Platform: `macOS-15.7.9-x86_64-i386-64bit-Mach-O`. All measured sequentially on this machine. Students: 43,050.

| implementation | build | store | wall s | operations s | read/rebuild/list s | 4KiB revisions s | restart wall s | peak RSS MiB | DB MiB | SQLite |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | sqlite | 397.885 | 337.596 | 5.330 | 50.708 | 15.704 | 2278.6 | 382.4 | 3.53.4 |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | sqlite | 380.004 | 332.699 | 6.711 | 33.253 | 18.740 | 2264.1 | 382.4 | 3.53.4 |
| [rust](../../../rust) | Release optimized | sqlite | 308.061 | 282.017 | 2.666 | 21.300 | 6.684 | 1363.9 | 382.4 | 3.54.1 |
| [ts-zod](../../../typescript) | Node + Zod | sqlite | 537.196 | 478.889 | 10.318 | 40.203 | 27.421 | 2072.4 | 382.4 | 3.53.3 |
| [java](../../../java) | JVM defaults | sqlite | 414.055 | 337.716 | 11.747 | 60.746 | 10.632 | 2688.2 | 382.4 | 3.54.1 |
| [go](../../../go) | go build | sqlite | 413.330 | 367.304 | 5.975 | 30.736 | 12.257 | 1373.3 | 382.4 | 3.54.1 |
| [swift](../../../swift) | swiftc -O | sqlite | 725.226 | 647.674 | 15.361 | 52.912 | 38.498 | 1538.6 | 382.4 | 3.43.2 |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | memory-json | 67.209 | 54.270 | 3.804 | 4.039 | — | 1779.8 | — | — |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | memory-json | 78.638 | 61.901 | 6.178 | 3.913 | — | 1923.1 | — | — |
| [rust](../../../rust) | Release optimized | memory-json | 69.139 | 61.169 | 2.821 | 2.969 | — | 1490.7 | — | — |
| [ts-zod](../../../typescript) | Node + Zod | memory-json | 230.737 | 212.862 | 9.898 | 5.042 | — | 1867.2 | — | — |
| [java](../../../java) | JVM defaults | memory-json | 76.091 | 61.191 | 2.259 | 8.116 | — | 2076.9 | — | — |
| [go](../../../go) | go build | memory-json | 173.554 | 145.005 | 7.739 | 12.105 | — | 1597.1 | — | — |
| [swift](../../../swift) | swiftc -O | memory-json | 486.726 | 445.337 | 16.573 | 15.283 | — | 1373.8 | — | — |

| implementation | store | tagged reads 1000 s | projection + JSON 1000 s | input/open/replay s | response/close/exit s |
| --- | --- | ---: | ---: | ---: | ---: |
| csharp | sqlite | 0.207542 | 0.006221 | 2.465 | 1.561 |
| fsharp | sqlite | 0.127812 | 0.006838 | 3.139 | 4.056 |
| rust | sqlite | 0.090293 | 0.001040 | 0.377 | 1.610 |
| ts-zod | sqlite | 0.318240 | 0.011067 | 1.599 | 5.839 |
| java | sqlite | 0.205867 | 0.026373 | 1.937 | 1.608 |
| go | sqlite | 0.165357 | 0.002384 | 8.186 | 0.961 |
| swift | sqlite | 0.443244 | 0.023938 | 1.775 | 6.958 |
| csharp | memory-json | 0.068557 | 0.005872 | 3.700 | 1.318 |
| fsharp | memory-json | 0.069759 | 0.009562 | 2.627 | 3.922 |
| rust | memory-json | 0.054524 | 0.001838 | 0.348 | 1.775 |
| ts-zod | memory-json | 0.208773 | 0.009646 | 1.189 | 1.526 |
| java | memory-json | 0.089068 | 0.023938 | 1.528 | 2.850 |
| go | memory-json | 0.169277 | 0.004358 | 7.398 | 1.133 |
| swift | memory-json | 0.285964 | 0.021027 | 2.261 | 6.915 |

RSS includes the runtime, parsed input, audit traces, event reads, and live/rebuilt projections. It is not the incremental projection size.
Operations include commands, validation/rejections, conflicts/rejudgment, full list queries and duplicate delivery. JSON summary/list encoding, replay verification and process close/exit are included in wall time.
SQLite versions differ by driver. This compares complete implementations, not isolated language speed. No synthetic delays are used.

[Raw data](students-43050.json)
