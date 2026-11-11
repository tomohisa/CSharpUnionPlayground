| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0042 | 4175.5 | 499.0 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0089 | 888.7 | 137.5 | 100 | 0.33 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0548 | 547.9 | 138.2 | 1000 | 0.79 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.5733 | 573.3 | 140.6 | 10000 | 1.02 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0049 | 4939.7 | 141.6 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0113 | 1128.8 | 137.4 | 1 | 0.36 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0618 | 617.5 | 143.9 | 1 | 0.74 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.6547 | 654.7 | 193.1 | 1 | 1.03 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0071 | 7051.4 | 137.5 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0221 | 2214.0 | 137.5 | 100 | 0.50 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 1.1518 | 11517.6 | 156.6 | 1000 | 1.72 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0063 | 6271.4 | 139.1 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0130 | 1301.6 | 137.4 | 1 | 0.32 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.1018 | 1017.6 | 143.9 | 1 | 0.89 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.8960 | 896.0 | 192.8 | 1 | 0.94 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0147 | 14665.3 | 137.4 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0133 | 1334.6 | 137.4 | 100 | -0.04 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0682 | 682.0 | 142.4 | 1000 | 0.71 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.6757 | 675.7 | 140.7 | 10000 | 1.00 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0090 | 8971.2 | 137.4 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0198 | 1977.6 | 141.5 | 1 | 0.34 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 1.4857 | 14857.3 | 141.0 | 1 | 1.88 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0036 | 3618.5 | 24.1 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0296 | 2958.5 | 9.9 | 100 | 0.91 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.3004 | 3004.2 | 10.1 | 1000 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 2.8683 | 2868.3 | 11.1 | 10000 | 0.98 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0028 | 2805.9 | 9.8 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0313 | 3126.0 | 10.1 | 1 | 1.05 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.3437 | 3437.0 | 13.5 | 1 | 1.04 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 3.0294 | 3029.4 | 40.4 | 1 | 0.95 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0034 | 3416.2 | 9.8 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0563 | 5634.3 | 9.9 | 100 | 1.22 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.3448 | 3448.2 | 10.0 | 1000 | 0.79 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 3.2408 | 3240.8 | 12.2 | 10000 | 0.97 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0029 | 2921.3 | 9.8 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0284 | 2840.3 | 10.0 | 1 | 0.99 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.2830 | 2830.1 | 13.5 | 1 | 1.00 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 2.9219 | 2921.9 | 40.4 | 1 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0032 | 3196.3 | 9.8 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0269 | 2693.6 | 9.8 | 100 | 0.93 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.2857 | 2856.7 | 9.9 | 1000 | 1.03 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 2.8564 | 2856.4 | 11.1 | 10000 | 1.00 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0031 | 3125.4 | 9.8 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0285 | 2850.9 | 10.0 | 1 | 0.96 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.3860 | 3860.0 | 12.1 | 1 | 1.13 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 14.7023 | 14702.3 | 35.1 | 1 | 1.58 | ok |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 4.2139 | 0.0053 | 1.3277 | 5.5556 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.2196 | 0.0050 | 1.1988 | 1.4377 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.1919 | 0.0050 | 1.2739 | 1.5299 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.1978 | 0.0051 | 1.1306 | 1.9119 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.2117 | 0.0062 | 1.2026 | 1.4311 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.2074 | 0.0061 | 1.1662 | 1.4001 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.2068 | 0.0051 | 1.1558 | 1.4604 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.1900 | 0.0052 | 1.1334 | 2.2023 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.2100 | 0.0053 | 1.1913 | 1.4190 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.1933 | 0.0056 | 1.1345 | 1.3602 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.1829 | 0.0051 | 1.0029 | 2.3473 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.2114 | 0.0069 | 1.2151 | 1.4446 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.2064 | 0.0058 | 1.2750 | 1.5095 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.2639 | 0.0077 | 1.4161 | 1.8263 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.2201 | 0.0067 | 1.3467 | 2.7223 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.2737 | 0.0153 | 1.5289 | 1.8493 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.2813 | 0.0083 | 1.7161 | 2.0258 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.2426 | 0.0064 | 1.4024 | 1.7251 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.2441 | 0.0078 | 1.4080 | 2.3415 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.3916 | 0.0143 | 1.9091 | 2.3418 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.2321 | 0.0065 | 1.5854 | 1.8528 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.3045 | 0.0079 | 1.6902 | 3.6402 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0994 | 0.0001 | 0.0081 | 0.1114 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0349 | 0.0000 | 0.0027 | 0.0675 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0372 | 0.0003 | 0.0027 | 0.3407 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0278 | 0.0001 | 0.0026 | 2.8991 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0298 | 0.0000 | 0.0026 | 0.0353 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0324 | 0.0000 | 0.0030 | 0.0670 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0553 | 0.0003 | 0.0045 | 0.4045 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0286 | 0.0001 | 0.0297 | 3.0989 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0343 | 0.0034 | 0.0030 | 0.0442 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0635 | 0.0001 | 0.0040 | 0.1241 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0501 | 0.0000 | 0.0026 | 0.3977 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0284 | 0.0000 | 0.0033 | 3.2729 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0299 | 0.0000 | 0.0024 | 0.0354 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0285 | 0.0000 | 0.0024 | 0.0596 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0302 | 0.0003 | 0.0037 | 0.3180 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0288 | 0.0000 | 0.0274 | 2.9902 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0284 | 0.0000 | 0.0026 | 0.0344 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0296 | 0.0000 | 0.0023 | 0.0590 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0304 | 0.0001 | 0.0025 | 0.3188 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0301 | 0.0000 | 0.0027 | 2.8895 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0304 | 0.0001 | 0.0025 | 0.0361 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0289 | 0.0001 | 0.0028 | 0.0606 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0336 | 0.0000 | 0.0033 | 0.4235 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0314 | 0.0000 | 0.0179 | 14.7574 |
