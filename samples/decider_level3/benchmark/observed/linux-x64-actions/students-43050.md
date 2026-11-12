# Level 3 ES benchmark observations

Measured: 2026-11-12T08:26:35.393826+00:00

Platform: `Linux-7.0.0-1019-azure-x86_64-with-glibc2.43`. All measured sequentially on this machine. Students: 43,050.

| implementation | build | store | wall s | operations s | read/rebuild/list s | 4KiB revisions s | restart wall s | peak RSS MiB | DB MiB | SQLite |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | sqlite | 163.397 | 141.511 | 3.168 | 15.778 | 8.986 | 2339.5 | 382.4 | 3.53.4 |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | sqlite | 165.009 | 140.321 | 4.534 | 15.985 | 10.239 | 2492.9 | 382.4 | 3.53.4 |
| [rust](../../../rust) | Release optimized | sqlite | 134.987 | 120.791 | 1.271 | 11.832 | 3.424 | 1062.0 | 382.4 | 3.54.1 |
| [ts-zod](../../../typescript) | Node + Zod | sqlite | 262.956 | 239.345 | 6.204 | 15.724 | 13.062 | 2553.4 | 382.4 | 3.54.1 |
| [java](../../../java) | JVM defaults | sqlite | 176.990 | 156.508 | 2.417 | 16.020 | 6.992 | 3265.7 | 382.4 | 3.54.1 |
| [go](../../../go) | go build | sqlite | 223.409 | 196.763 | 4.159 | 18.476 | 8.887 | 1518.2 | 382.4 | 3.54.1 |
| [swift](../../../swift) | swiftc -O | sqlite | 241.582 | 213.922 | 6.232 | 17.007 | 15.507 | 1181.3 | 382.4 | 3.46.1 |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | memory-json | 46.879 | 39.211 | 2.631 | 2.450 | — | 1797.7 | — | — |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | memory-json | 47.584 | 37.232 | 3.950 | 2.344 | — | 1940.1 | — | — |
| [rust](../../../rust) | Release optimized | memory-json | 27.691 | 23.823 | 1.301 | 1.177 | — | 1414.3 | — | — |
| [ts-zod](../../../typescript) | Node + Zod | memory-json | 131.610 | 121.136 | 5.881 | 2.995 | — | 2127.3 | — | — |
| [java](../../../java) | JVM defaults | memory-json | 38.440 | 32.408 | 1.371 | 2.759 | — | 3057.2 | — | — |
| [go](../../../go) | go build | memory-json | 88.974 | 76.657 | 4.071 | 4.217 | — | 1821.2 | — | — |
| [swift](../../../swift) | swiftc -O | memory-json | 154.320 | 137.064 | 6.211 | 6.341 | — | 1139.6 | — | — |

| implementation | store | tagged reads 1000 s | projection + JSON 1000 s | input/open/replay s | response/close/exit s |
| --- | --- | ---: | ---: | ---: | ---: |
| csharp | sqlite | 0.140710 | 0.003751 | 1.804 | 0.985 |
| fsharp | sqlite | 0.077048 | 0.005681 | 1.744 | 2.333 |
| rust | sqlite | 0.053341 | 0.000772 | 0.248 | 0.790 |
| ts-zod | sqlite | 0.158988 | 0.007097 | 0.629 | 0.884 |
| java | sqlite | 0.090909 | 0.009434 | 1.092 | 0.843 |
| go | sqlite | 0.110577 | 0.002031 | 3.023 | 0.875 |
| swift | sqlite | 0.153228 | 0.007524 | 1.142 | 3.088 |
| csharp | memory-json | 0.060966 | 0.003968 | 1.626 | 0.892 |
| fsharp | memory-json | 0.030540 | 0.005406 | 1.764 | 2.248 |
| rust | memory-json | 0.021355 | 0.000739 | 0.245 | 1.121 |
| ts-zod | memory-json | 0.119700 | 0.006932 | 0.629 | 0.841 |
| java | memory-json | 0.048619 | 0.012705 | 1.017 | 0.815 |
| go | memory-json | 0.061574 | 0.002145 | 3.050 | 0.915 |
| swift | memory-json | 0.106162 | 0.007696 | 1.143 | 3.436 |

RSS includes the runtime, parsed input, audit traces, event reads, and live/rebuilt projections. It is not the incremental projection size.
Operations include commands, validation/rejections, conflicts/rejudgment, full list queries and duplicate delivery. JSON summary/list encoding, replay verification and process close/exit are included in wall time.
SQLite versions differ by driver. This compares complete implementations, not isolated language speed. No synthetic delays are used.

[Raw data](students-43050.json)
