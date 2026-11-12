# Level 3 ES benchmark observations

Measured: 2026-11-12T02:41:52.270893+00:00

Platform: `macOS-26.7.2-arm64-arm-64bit`. All measured sequentially on this machine. Students: 43,050.

| implementation | build | store | wall s | operations s | read/rebuild/list s | 4KiB revisions s | restart wall s | peak RSS MiB | DB MiB | SQLite |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| [csharp](../../csharp) | Release JIT + Dynamic PGO | sqlite | 139.706 | 129.081 | 1.621 | 7.548 | 5.371 | 2093.9 | 382.4 | 3.53.4 |
| [fsharp](../../fsharp) | Release JIT + Dynamic PGO | sqlite | 174.050 | 157.003 | 2.316 | 12.207 | 5.193 | 2507.2 | 382.4 | 3.53.4 |
| [rust](../../rust) | Release optimized | sqlite | 166.534 | 157.777 | 1.013 | 7.181 | 2.303 | 1296.6 | 382.4 | 3.54.1 |
| [ts-zod](../../typescript) | Node + Zod | sqlite | 214.771 | 189.220 | 4.042 | 20.015 | 12.608 | 2384.7 | 382.4 | 3.53.1 |
| [java](../../java) | JVM defaults | sqlite | 170.276 | 157.042 | 1.314 | 10.492 | 4.908 | 3173.6 | 382.4 | 3.54.1 |
| [go](../../go) | go build | sqlite | 221.120 | 194.335 | 2.555 | 21.428 | 5.279 | 1524.8 | 382.4 | 3.54.1 |
| [swift](../../swift) | swiftc -O | sqlite | 187.094 | 166.332 | 4.206 | 14.195 | 8.987 | 1605.5 | 382.4 | 3.51.0 |
| [csharp](../../csharp) | Release JIT + Dynamic PGO | memory-json | 31.234 | 25.930 | 1.473 | 1.490 | — | 1885.8 | — | — |
| [fsharp](../../fsharp) | Release JIT + Dynamic PGO | memory-json | 30.156 | 23.685 | 2.107 | 1.513 | — | 2030.3 | — | — |
| [rust](../../rust) | Release optimized | memory-json | 20.328 | 18.336 | 0.739 | 0.720 | — | 1480.4 | — | — |
| [ts-zod](../../typescript) | Node + Zod | memory-json | 72.684 | 66.409 | 3.183 | 2.174 | — | 2708.5 | — | — |
| [java](../../java) | JVM defaults | memory-json | 23.892 | 19.754 | 0.781 | 1.752 | — | 3803.7 | — | — |
| [go](../../go) | go build | memory-json | 67.495 | 55.275 | 2.720 | 6.292 | — | 1559.9 | — | — |
| [swift](../../swift) | swiftc -O | memory-json | 107.231 | 96.243 | 4.319 | 4.241 | — | 1501.6 | — | — |

| implementation | store | tagged reads 1000 s | projection + JSON 1000 s | input/open/replay s | response/close/exit s |
| --- | --- | ---: | ---: | ---: | ---: |
| csharp | sqlite | 0.050994 | 0.001966 | 0.961 | 0.439 |
| fsharp | sqlite | 0.042131 | 0.002328 | 1.243 | 1.231 |
| rust | sqlite | 0.044292 | 0.000658 | 0.168 | 0.349 |
| ts-zod | sqlite | 0.183004 | 0.003795 | 0.515 | 0.785 |
| java | sqlite | 0.055517 | 0.005633 | 0.894 | 0.451 |
| go | sqlite | 0.072636 | 0.001114 | 2.382 | 0.347 |
| swift | sqlite | 0.112607 | 0.005240 | 0.707 | 1.523 |
| csharp | memory-json | 0.042979 | 0.002305 | 1.729 | 0.565 |
| fsharp | memory-json | 0.019170 | 0.003396 | 1.433 | 1.377 |
| rust | memory-json | 0.016332 | 0.000525 | 0.158 | 0.358 |
| ts-zod | memory-json | 0.067309 | 0.002954 | 0.322 | 0.524 |
| java | memory-json | 0.021094 | 0.006023 | 0.669 | 0.899 |
| go | memory-json | 0.044248 | 0.001102 | 2.787 | 0.375 |
| swift | memory-json | 0.081522 | 0.005557 | 0.653 | 1.673 |

RSS includes the runtime, parsed input, audit traces, event reads, and live/rebuilt projections. It is not the incremental projection size.
Operations include commands, validation/rejections, conflicts/rejudgment, full list queries and duplicate delivery. JSON summary/list encoding, replay verification and process close/exit are included in wall time.
SQLite versions differ by driver. This compares complete implementations, not isolated language speed. No synthetic delays are used.

[Raw data](macos-arm64-large.json)
