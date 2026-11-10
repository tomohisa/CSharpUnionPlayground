C# Native: jit-pgo; abstract .NET 10: True.

| implementation | median batch s | encode s | decode s | switch s | aggregate s | JSON MiB | peak RSS MiB |
|---|---:|---:|---:|---:|---:|---:|---:|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 1.6757 | 0.4140 | 0.9851 | 0.2332 | 9.35 | 82.59 | 1739.1 |
| [ts-manual](code/json/typescript/benchmark.ts) | 1.7866 | 0.4527 | 1.1572 | 0.1909 | 9.36 | 82.59 | 1740.2 |
| [rust](code/json/rust/main.rs) | 2.0212 | 0.4651 | 1.5741 | 0.0359 | 11.40 | 82.59 | 1144.9 |
| [java](code/json/java/Benchmark.java) | 3.1634 | 0.9838 | 1.4919 | 0.8389 | 41.39 | 82.59 | 3375.5 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 3.5613 | 0.3932 | 2.7887 | 0.2123 | 17.65 | 82.59 | 2358.1 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 3.6469 | 0.6983 | 2.8320 | 0.0704 | 19.43 | 82.59 | 2331.8 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 3.9107 | 0.6873 | 3.2373 | 0.0903 | 20.39 | 82.59 | 2815.1 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 5.6691 | 0.5069 | 5.0976 | 0.0880 | 30.98 | 87.74 | 2358.9 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 6.8890 | 0.8009 | 5.9621 | 0.0956 | 38.40 | 87.74 | 2364.8 |
| [go](code/json/go/benchmark.go) | 8.6617 | 2.5314 | 6.0651 | 0.0635 | 43.27 | 82.59 | 1734.9 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 11.3380 | 3.0306 | 8.6376 | 0.1161 | 67.08 | 82.59 | 2307.7 |
| [swift](code/json/swift/Benchmark.swift) | 29.7904 | 14.1197 | 14.5208 | 0.2398 | 146.95 | 79.16 | 2138.0 |

| implementation | startup s | preparation s | exit s | total wall s |
|---|---:|---:|---:|---:|
| [ts-simple](code/json/typescript-simple/benchmark-simple.ts) | 0.1364 | 0.440 | 0.1607 | 10.60 |
| [ts-manual](code/json/typescript/benchmark.ts) | 0.1347 | 0.456 | 0.1457 | 10.68 |
| [rust](code/json/rust/main.rs) | 0.0539 | 0.497 | 0.4562 | 14.94 |
| [java](code/json/java/Benchmark.java) | 0.1585 | 2.628 | 0.4511 | 45.57 |
| [ts-zod](code/json/typescript-zod/benchmark-zod.ts) | 0.6130 | 0.631 | 0.2588 | 19.75 |
| [csharp-closed-inferred](code/json/csharp-closed-inferred/Program.cs) | 0.0774 | 1.673 | 0.1767 | 21.98 |
| [csharp-abstract](code/json/csharp-abstract/Program.cs) | 0.2198 | 1.651 | 0.1638 | 23.18 |
| [csharp-union-custom](code/json/csharp-union-custom/Program.cs) | 0.0744 | 1.727 | 0.1210 | 33.37 |
| [csharp-union-generated](code/json/csharp-union-generated/Program.cs) | 0.1035 | 1.846 | 0.1758 | 41.41 |
| [go](code/json/go/benchmark.go) | 0.0563 | 0.567 | 0.0927 | 46.99 |
| [csharp-closed-explicit](code/json/csharp-closed-explicit/Program.cs) | 0.0844 | 1.768 | 0.2101 | 73.73 |
| [swift](code/json/swift/Benchmark.swift) | 0.0573 | 0.767 | 0.0905 | 165.38 |
