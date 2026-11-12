# Level 3 ES benchmark observations

Measured: 2026-11-12T05:25:27.310310+00:00

Platform: `macOS-15.7.9-x86_64-i386-64bit-Mach-O`. All measured sequentially on this machine. Students: 22,900.

| implementation | build | store | wall s | operations s | read/rebuild/list s | 4KiB revisions s | restart wall s | peak RSS MiB | DB MiB | SQLite |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | sqlite | 211.630 | 179.764 | 3.957 | 22.884 | 13.981 | 977.1 | 203.5 | 3.53.4 |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | sqlite | 202.904 | 172.916 | 3.848 | 20.817 | 11.441 | 1333.2 | 203.5 | 3.53.4 |
| [rust](../../../rust) | Release optimized | sqlite | 167.784 | 148.943 | 1.935 | 15.557 | 5.678 | 718.4 | 203.5 | 3.54.1 |
| [ts-zod](../../../typescript) | Node + Zod | sqlite | 274.240 | 228.113 | 9.500 | 32.170 | 15.813 | 1505.1 | 203.5 | 3.53.3 |
| [java](../../../java) | JVM defaults | sqlite | 176.826 | 155.563 | 1.276 | 15.803 | 5.711 | 1305.4 | 203.5 | 3.54.1 |
| [go](../../../go) | go build | sqlite | 196.983 | 165.599 | 4.023 | 23.412 | 7.992 | 836.0 | 203.5 | 3.54.1 |
| [swift](../../../swift) | swiftc -O | sqlite | 479.550 | 424.182 | 10.043 | 39.489 | 22.333 | 809.0 | 203.5 | 3.43.2 |
| [csharp](../../../csharp) | Release JIT + Dynamic PGO | memory-json | 37.701 | 29.854 | 2.261 | 3.000 | — | 946.1 | — | — |
| [fsharp](../../../fsharp) | Release JIT + Dynamic PGO | memory-json | 37.265 | 27.493 | 4.208 | 1.933 | — | 1074.9 | — | — |
| [rust](../../../rust) | Release optimized | memory-json | 28.751 | 25.811 | 1.027 | 1.043 | — | 773.5 | — | — |
| [ts-zod](../../../typescript) | Node + Zod | memory-json | 109.219 | 99.773 | 4.704 | 2.836 | — | 1203.0 | — | — |
| [java](../../../java) | JVM defaults | memory-json | 35.266 | 29.765 | 1.167 | 2.308 | — | 1369.0 | — | — |
| [go](../../../go) | go build | memory-json | 90.323 | 77.466 | 2.983 | 5.244 | — | 866.8 | — | — |
| [swift](../../../swift) | swiftc -O | memory-json | 232.446 | 210.006 | 9.080 | 8.672 | — | 706.3 | — | — |

| implementation | store | tagged reads 1000 s | projection + JSON 1000 s | input/open/replay s | response/close/exit s |
| --- | --- | ---: | ---: | ---: | ---: |
| csharp | sqlite | 0.218376 | 0.005516 | 3.634 | 1.150 |
| fsharp | sqlite | 0.144816 | 0.007590 | 2.801 | 2.356 |
| rust | sqlite | 0.116634 | 0.001375 | 0.218 | 1.013 |
| ts-zod | sqlite | 0.301284 | 0.010985 | 1.011 | 3.126 |
| java | sqlite | 0.161401 | 0.017958 | 3.293 | 0.683 |
| go | sqlite | 0.213663 | 0.003298 | 3.129 | 0.602 |
| swift | sqlite | 0.484759 | 0.030967 | 1.072 | 4.167 |
| csharp | memory-json | 0.073906 | 0.004124 | 1.768 | 0.735 |
| fsharp | memory-json | 0.043296 | 0.005739 | 1.685 | 1.885 |
| rust | memory-json | 0.038428 | 0.001000 | 0.177 | 0.653 |
| ts-zod | memory-json | 0.211125 | 0.011297 | 0.680 | 1.003 |
| java | memory-json | 0.076572 | 0.020082 | 1.129 | 0.775 |
| go | memory-json | 0.091418 | 0.002985 | 3.920 | 0.616 |
| swift | memory-json | 0.372761 | 0.028935 | 0.955 | 3.311 |

RSS includes the runtime, parsed input, audit traces, event reads, and live/rebuilt projections. It is not the incremental projection size.
Operations include commands, validation/rejections, conflicts/rejudgment, full list queries and duplicate delivery. JSON summary/list encoding, replay verification and process close/exit are included in wall time.
SQLite versions differ by driver. This compares complete implementations, not isolated language speed. No synthetic delays are used.

[Raw data](students-22900.json)
