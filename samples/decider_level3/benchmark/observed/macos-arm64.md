# Level 3 ES benchmark observations

Measured: 2026-11-12T03:26:47.634180+00:00

Platform: `macOS-26.7.2-arm64-arm-64bit`. All measured sequentially on this machine. Students: 22,900.

| implementation | build | store | wall s | operations s | read/rebuild/list s | 4KiB revisions s | restart wall s | peak RSS MiB | DB MiB | SQLite |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| [csharp](../../csharp) | Release JIT + Dynamic PGO | sqlite | 38.145 | 31.836 | 1.306 | 3.949 | 3.792 | 1077.9 | 203.5 | 3.53.4 |
| [fsharp](../../fsharp) | Release JIT + Dynamic PGO | sqlite | 43.555 | 35.776 | 1.475 | 4.554 | 4.204 | 1374.4 | 203.5 | 3.53.4 |
| [rust](../../rust) | Release optimized | sqlite | 35.910 | 32.600 | 0.422 | 2.575 | 1.045 | 661.5 | 203.5 | 3.54.1 |
| [ts-zod](../../typescript) | Node + Zod | sqlite | 64.291 | 54.570 | 1.527 | 7.684 | 3.422 | 1703.2 | 203.5 | 3.53.1 |
| [java](../../java) | JVM defaults | sqlite | 89.207 | 84.141 | 0.530 | 3.475 | 2.750 | 2017.6 | 203.5 | 3.54.1 |
| [go](../../go) | go build | sqlite | 105.095 | 96.589 | 1.233 | 5.860 | 2.627 | 828.9 | 203.5 | 3.54.1 |
| [swift](../../swift) | swiftc -O | sqlite | 95.970 | 84.518 | 2.059 | 8.088 | 4.888 | 866.5 | 203.5 | 3.51.0 |
| [csharp](../../csharp) | Release JIT + Dynamic PGO | memory-json | 12.570 | 10.532 | 0.554 | 0.622 | — | 1013.4 | — | — |
| [fsharp](../../fsharp) | Release JIT + Dynamic PGO | memory-json | 12.553 | 9.799 | 0.952 | 0.477 | — | 1128.6 | — | — |
| [rust](../../rust) | Release optimized | memory-json | 9.623 | 8.642 | 0.368 | 0.367 | — | 787.4 | — | — |
| [ts-zod](../../typescript) | Node + Zod | memory-json | 32.772 | 29.904 | 1.363 | 1.011 | — | 1528.8 | — | — |
| [java](../../java) | JVM defaults | memory-json | 10.797 | 9.132 | 0.374 | 0.740 | — | 2579.4 | — | — |
| [go](../../go) | go build | memory-json | 30.075 | 24.686 | 1.113 | 2.849 | — | 934.8 | — | — |
| [swift](../../swift) | swiftc -O | memory-json | 44.933 | 40.205 | 1.754 | 1.787 | — | 799.0 | — | — |

| implementation | store | tagged reads 1000 s | projection + JSON 1000 s | input/open/replay s | response/close/exit s |
| --- | --- | ---: | ---: | ---: | ---: |
| csharp | sqlite | 0.098706 | 0.002732 | 0.642 | 0.306 |
| fsharp | sqlite | 0.047280 | 0.003142 | 0.867 | 0.826 |
| rust | sqlite | 0.034305 | 0.000493 | 0.094 | 0.184 |
| ts-zod | sqlite | 0.078739 | 0.002699 | 0.221 | 0.205 |
| java | sqlite | 0.046140 | 0.004449 | 0.788 | 0.215 |
| go | sqlite | 0.066010 | 0.001068 | 1.178 | 0.168 |
| swift | sqlite | 0.100971 | 0.004924 | 0.325 | 0.869 |
| csharp | memory-json | 0.021533 | 0.001870 | 0.539 | 0.298 |
| fsharp | memory-json | 0.014703 | 0.002249 | 0.586 | 0.713 |
| rust | memory-json | 0.016270 | 0.000509 | 0.069 | 0.159 |
| ts-zod | memory-json | 0.058519 | 0.002721 | 0.204 | 0.228 |
| java | memory-json | 0.020432 | 0.004757 | 0.289 | 0.230 |
| go | memory-json | 0.036397 | 0.000988 | 1.184 | 0.205 |
| swift | memory-json | 0.061400 | 0.004275 | 0.317 | 0.798 |

RSS includes the runtime, parsed input, audit traces, event reads, and live/rebuilt projections. It is not the incremental projection size.
Operations include commands, validation/rejections, conflicts/rejudgment, full list queries and duplicate delivery. JSON summary/list encoding, replay verification and process close/exit are included in wall time.
SQLite versions differ by driver. This compares complete implementations, not isolated language speed. No synthetic delays are used.

[Raw data](macos-arm64.json)
