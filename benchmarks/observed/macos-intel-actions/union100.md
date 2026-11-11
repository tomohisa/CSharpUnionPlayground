C# Native: jit-pgo; abstract .NET 10: True.

| implementation | median batch s | encode s | decode s | switch s | aggregate s | JSON MiB | peak RSS MiB |
|---|---:|---:|---:|---:|---:|---:|---:|
| [ts-manual](code/json/typescript/benchmark.ts) | 1.5745 | 0.3877 | 1.0028 | 0.1818 | 8.75 | 82.59 | 1740.8 |
| [rust](code/json/rust/main.rs) | 1.9951 | 0.3406 | 1.5902 | 0.0355 | 9.84 | 82.59 | 1195.6 |
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 2.2662 | 0.5822 | 1.3119 | 0.3164 | 12.29 | 82.59 | 1738.2 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 3.5835 | 0.4902 | 2.9121 | 0.2459 | 19.00 | 82.59 | 2355.4 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 3.7811 | 0.5629 | 3.0839 | 0.0742 | 19.42 | 82.59 | 2811.4 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 3.7973 | 0.6757 | 2.9256 | 0.0743 | 20.63 | 82.59 | 2328.2 |
| [java](code/json/java/Benchmark.java) | 4.4496 | 0.9058 | 1.4332 | 0.8350 | 28.53 | 82.59 | 3261.4 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 5.5455 | 0.3958 | 5.0768 | 0.0825 | 29.26 | 87.74 | 2362.9 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 8.8155 | 1.8915 | 6.8382 | 0.0858 | 47.56 | 87.74 | 2362.4 |
| [go](code/json/go/benchmark.go) | 9.9369 | 3.2933 | 6.5154 | 0.0597 | 50.95 | 82.59 | 1592.6 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 10.4157 | 2.3846 | 7.2422 | 0.1164 | 56.63 | 82.59 | 1823.6 |
| [swift](code/json/swift/Benchmark.swift) | 29.6059 | 14.5150 | 15.4720 | 0.2586 | 151.16 | 79.16 | 2122.5 |

| implementation | startup s | preparation s | exit s | total wall s |
|---|---:|---:|---:|---:|
| [ts-manual](code/json/typescript/benchmark.ts) | 0.1562 | 0.557 | 0.1647 | 10.15 |
| [rust](code/json/rust/main.rs) | 0.0226 | 0.641 | 0.4164 | 13.31 |
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 0.1829 | 0.588 | 0.2073 | 13.88 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 0.3612 | 0.730 | 0.2937 | 21.01 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 0.2037 | 1.655 | 0.1529 | 22.13 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 0.2817 | 1.967 | 0.1720 | 24.71 |
| [java](code/json/java/Benchmark.java) | 0.1516 | 2.569 | 0.2953 | 32.71 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 0.0842 | 1.846 | 0.1329 | 31.75 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 0.0987 | 1.738 | 0.1455 | 50.30 |
| [go](code/json/go/benchmark.go) | 0.0151 | 0.717 | 0.0969 | 59.63 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 0.0757 | 1.483 | 0.3196 | 62.35 |
| [swift](code/json/swift/Benchmark.swift) | 0.0263 | 1.099 | 0.1118 | 170.66 |
