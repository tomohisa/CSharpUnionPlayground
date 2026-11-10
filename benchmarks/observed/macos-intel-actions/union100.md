| implementation | median batch s | encode s | decode s | switch s | aggregate s | JSON MiB | peak RSS MiB |
|---|---:|---:|---:|---:|---:|---:|---:|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 1.8696 | 0.4845 | 1.0754 | 0.2370 | 9.12 | 82.59 | 1738.7 |
| [ts-manual](code/json/typescript/benchmark.ts) | 1.8744 | 0.4615 | 1.2157 | 0.2231 | 9.84 | 82.59 | 1740.6 |
| [rust](code/json/rust/main.rs) | 2.1370 | 0.3591 | 1.7408 | 0.0371 | 10.35 | 82.59 | 1268.6 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 3.6999 | 0.7479 | 2.8065 | 0.1753 | 20.09 | 82.59 | 2126.7 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 4.0151 | 0.4506 | 3.1756 | 0.2238 | 18.82 | 82.59 | 2350.7 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 4.4226 | 0.7008 | 3.6742 | 0.1752 | 23.29 | 82.59 | 2122.7 |
| [java](code/json/java/Benchmark.java) | 5.5528 | 1.3240 | 1.9166 | 2.5288 | 58.47 | 82.59 | 3341.8 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 7.3038 | 0.5348 | 6.5737 | 0.2021 | 36.40 | 87.74 | 1990.4 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 7.6021 | 1.6473 | 6.1269 | 0.2975 | 43.08 | 82.59 | 2145.2 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 8.0968 | 0.9436 | 6.8954 | 0.2356 | 42.05 | 87.74 | 2168.7 |
| [go](code/json/go/benchmark.go) | 8.6861 | 2.5169 | 5.9605 | 0.0604 | 44.01 | 82.59 | 1586.7 |
| [swift](code/json/swift/Benchmark.swift) | 34.9805 | 18.0985 | 16.0888 | 0.2630 | 172.00 | 79.16 | 2118.2 |

| implementation | startup s | preparation s | exit s | total wall s |
|---|---:|---:|---:|---:|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 0.1494 | 0.493 | 0.1292 | 11.16 |
| [ts-manual](code/json/typescript/benchmark.ts) | 0.1662 | 0.532 | 0.1380 | 11.31 |
| [rust](code/json/rust/main.rs) | 0.0183 | 0.533 | 0.4890 | 13.69 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 0.0483 | 1.159 | 0.1074 | 22.25 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 0.2916 | 0.573 | 0.2316 | 20.58 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 0.1391 | 1.513 | 0.1275 | 25.77 |
| [java](code/json/java/Benchmark.java) | 0.1594 | 4.191 | 0.2228 | 64.02 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 0.0775 | 1.370 | 0.1258 | 38.49 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 0.0513 | 1.274 | 0.2113 | 46.21 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 0.0675 | 1.336 | 0.0938 | 44.49 |
| [go](code/json/go/benchmark.go) | 0.0126 | 0.530 | 0.0810 | 47.24 |
| [swift](code/json/swift/Benchmark.swift) | 0.0163 | 0.892 | 0.0816 | 189.08 |
