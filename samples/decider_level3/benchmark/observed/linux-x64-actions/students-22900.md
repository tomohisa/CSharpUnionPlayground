# Level 3 ES benchmark observations

Measured: 2026-11-12T08:08:40.615461+00:00

Platform: `Linux-7.0.0-1019-azure-x86_64-with-glibc2.43`. All measured sequentially on this machine. Students: 22,900.

| implementation | build | store | wall s | operations s | read/rebuild/list s | 4KiB revisions s | restart wall s | peak RSS MiB | DB MiB | SQLite |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | sqlite | 85.738 | 74.462 | 1.684 | 8.204 | 5.727 | 1255.3 | 203.5 | 3.53.4 |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | sqlite | 85.587 | 72.618 | 2.374 | 8.155 | 5.727 | 1372.3 | 203.5 | 3.53.4 |
| [rust](../../../rust) | Release optimized | sqlite | 70.956 | 63.506 | 0.655 | 6.224 | 1.718 | 564.5 | 203.5 | 3.54.1 |
| [ts-zod](../../../typescript) | Node + Zod | sqlite | 139.869 | 127.271 | 3.455 | 7.986 | 7.189 | 1771.6 | 203.5 | 3.54.1 |
| [java](../../../java) | JVM defaults | sqlite | 97.430 | 85.348 | 0.936 | 9.395 | 4.631 | 2671.0 | 203.5 | 3.54.1 |
| [go](../../../go) | go build | sqlite | 119.650 | 105.406 | 2.174 | 9.896 | 4.629 | 688.6 | 203.5 | 3.54.1 |
| [swift](../../../swift) | swiftc -O | sqlite | 127.648 | 112.951 | 3.237 | 9.048 | 8.286 | 641.9 | 203.5 | 3.46.1 |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | memory-json | 25.578 | 21.633 | 1.445 | 1.218 | — | 996.5 | — | — |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | memory-json | 26.379 | 20.768 | 2.027 | 1.233 | — | 1090.5 | — | — |
| [rust](../../../rust) | Release optimized | memory-json | 14.451 | 12.524 | 0.631 | 0.601 | — | 749.3 | — | — |
| [ts-zod](../../../typescript) | Node + Zod | memory-json | 67.689 | 61.956 | 3.228 | 1.507 | — | 1219.0 | — | — |
| [java](../../../java) | JVM defaults | memory-json | 22.989 | 19.280 | 0.621 | 1.661 | — | 2145.5 | — | — |
| [go](../../../go) | go build | memory-json | 46.572 | 40.192 | 2.044 | 2.178 | — | 830.1 | — | — |
| [swift](../../../swift) | swiftc -O | memory-json | 81.431 | 72.275 | 3.301 | 3.347 | — | 612.8 | — | — |

| implementation | store | tagged reads 1000 s | projection + JSON 1000 s | input/open/replay s | response/close/exit s |
| --- | --- | ---: | ---: | ---: | ---: |
| csharp | sqlite | 0.145586 | 0.003739 | 0.733 | 0.499 |
| fsharp | sqlite | 0.073528 | 0.015502 | 1.051 | 1.292 |
| rust | sqlite | 0.052258 | 0.000719 | 0.134 | 0.383 |
| ts-zod | sqlite | 0.170850 | 0.007174 | 0.443 | 0.530 |
| java | sqlite | 0.095749 | 0.015048 | 1.058 | 0.573 |
| go | sqlite | 0.112547 | 0.001973 | 1.618 | 0.441 |
| swift | sqlite | 0.148924 | 0.007416 | 0.600 | 1.639 |
| csharp | memory-json | 0.052385 | 0.003815 | 0.691 | 0.531 |
| fsharp | memory-json | 0.030242 | 0.005332 | 1.019 | 1.288 |
| rust | memory-json | 0.020924 | 0.000638 | 0.131 | 0.542 |
| ts-zod | memory-json | 0.118517 | 0.006722 | 0.412 | 0.459 |
| java | memory-json | 0.042626 | 0.013273 | 0.831 | 0.533 |
| go | memory-json | 0.061050 | 0.002001 | 1.624 | 0.472 |
| swift | memory-json | 0.107000 | 0.007475 | 0.609 | 1.778 |

RSS includes the runtime, parsed input, audit traces, event reads, and live/rebuilt projections. It is not the incremental projection size.
Operations include commands, validation/rejections, conflicts/rejudgment, full list queries and duplicate delivery. JSON summary/list encoding, replay verification and process close/exit are included in wall time.
SQLite versions differ by driver. This compares complete implementations, not isolated language speed. No synthetic delays are used.

[Raw data](students-22900.json)
