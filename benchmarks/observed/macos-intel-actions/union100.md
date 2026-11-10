| implementation | median batch s | encode s | decode s | switch s | aggregate s | JSON MiB | peak RSS MiB |
|---|---:|---:|---:|---:|---:|---:|---:|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 1.3865 | 0.3577 | 0.8360 | 0.1929 | 6.99 | 82.59 | 1738.5 |
| [ts-manual](code/json/typescript/benchmark.ts) | 1.8337 | 0.4208 | 1.1778 | 0.2305 | 9.46 | 82.59 | 1740.4 |
| [rust](code/json/rust/main.rs) | 1.9188 | 0.3251 | 1.5535 | 0.0391 | 10.01 | 82.59 | 1180.2 |
| [java](code/json/java/Benchmark.java) | 2.6203 | 0.8645 | 1.1026 | 0.6345 | 22.81 | 82.59 | 3112.3 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 3.2536 | 0.5287 | 2.5608 | 0.1585 | 16.08 | 82.59 | 2137.6 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 3.4369 | 0.5543 | 2.7146 | 0.1728 | 17.48 | 82.59 | 2123.8 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 5.1141 | 0.5839 | 4.2788 | 0.3464 | 28.80 | 82.59 | 2350.0 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 5.3024 | 1.2583 | 3.8135 | 0.2621 | 26.60 | 82.59 | 2149.0 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 5.9553 | 0.7204 | 4.8785 | 0.1683 | 29.67 | 87.74 | 2156.6 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 6.9259 | 0.5475 | 6.1957 | 0.1828 | 34.91 | 87.74 | 2150.1 |
| [go](code/json/go/benchmark.go) | 7.4081 | 2.1903 | 5.1139 | 0.0541 | 37.83 | 82.59 | 1562.3 |
| [swift](code/json/swift/Benchmark.swift) | 29.4815 | 14.8495 | 14.1922 | 0.2286 | 144.92 | 79.16 | 2127.2 |

| implementation | startup s | preparation s | exit s | total wall s |
|---|---:|---:|---:|---:|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 0.1513 | 0.442 | 0.1226 | 8.17 |
| [ts-manual](code/json/typescript/benchmark.ts) | 0.1934 | 0.540 | 0.1587 | 10.90 |
| [rust](code/json/rust/main.rs) | 0.0460 | 0.671 | 0.4302 | 13.39 |
| [java](code/json/java/Benchmark.java) | 0.2034 | 1.787 | 0.1567 | 25.71 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 0.0537 | 1.011 | 0.1061 | 17.81 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 0.0328 | 1.070 | 0.1183 | 19.26 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 0.3318 | 0.690 | 0.5832 | 31.27 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 0.0479 | 1.134 | 0.1186 | 28.98 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 0.0349 | 1.069 | 0.0798 | 31.61 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 0.0463 | 1.198 | 0.1105 | 36.85 |
| [go](code/json/go/benchmark.go) | 0.0159 | 0.538 | 0.0636 | 40.81 |
| [swift](code/json/swift/Benchmark.swift) | 0.0186 | 0.959 | 0.0901 | 166.91 |
