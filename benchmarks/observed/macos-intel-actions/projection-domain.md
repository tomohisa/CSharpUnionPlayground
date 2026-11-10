| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0137 | 137.0 | 9.6 | 1000 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0998 | 99.8 | 11.3 | 10000 | 0.86 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 1.0669 | 106.7 | 24.8 | 100000 | 1.03 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2318 | 2318.2 | 9.9 | 1000 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 22.8036 | 22803.6 | 16.2 | 10000 | 1.99 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0287 | 286.9 | 0.8 | 1000 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2646 | 264.6 | 2.6 | 10000 | 0.96 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 2.6918 | 269.2 | 15.3 | 100000 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0292 | 292.5 | 1.0 | 1000 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.8977 | 897.7 | 5.6 | 10000 | 1.49 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.1571 | 1571.3 | 82.4 | 1000 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.3664 | 366.4 | 97.4 | 10000 | 0.37 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000000 | 2.3447 | 234.5 | 169.4 | 100000 | 0.81 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.0989 | 989.2 | 82.7 | 1000 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 4.3117 | 4311.7 | 208.3 | 10000 | 1.64 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.0703 | 703.2 | 92.6 | 1000 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.3796 | 379.6 | 101.4 | 10000 | 0.73 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000000 | 3.1924 | 319.2 | 176.1 | 100000 | 0.92 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.1007 | 1006.8 | 91.9 | 1000 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 3.9440 | 3944.0 | 215.8 | 10000 | 1.59 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.2226 | 2226.3 | 55.1 | 1000 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.3181 | 318.1 | 90.0 | 10000 | 0.15 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000000 | 1.1164 | 111.6 | 310.1 | 100000 | 0.55 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.2853 | 2852.9 | 86.6 | 1000 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 10.0426 | 10042.6 | 1358.6 | 10000 | 1.55 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0162 | 162.1 | 6.3 | 1000 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.1561 | 156.1 | 7.5 | 10000 | 0.98 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000000 | 1.4749 | 147.5 | 27.0 | 100000 | 0.98 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0503 | 502.5 | 8.1 | 1000 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 2.4908 | 2490.8 | 32.2 | 10000 | 1.70 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0826 | 825.9 | 1.0 | 1000 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.8354 | 835.4 | 2.5 | 10000 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000000 | 8.9390 | 893.9 | 18.8 | 100000 | 1.03 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.1031 | 1031.5 | 1.0 | 1000 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 1.3346 | 1334.6 | 3.0 | 10000 | 1.11 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0196 | 0.0031 | 0.0026 | 0.0392 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0181 | 0.0017 | 0.0026 | 0.1218 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.0289 | 0.0015 | 0.0027 | 1.1010 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0160 | 0.0012 | 0.0020 | 0.2512 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.0188 | 0.0015 | 0.0021 | 22.8262 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0148 | 0.0000 | 0.0009 | 0.0445 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0106 | 0.0000 | 0.0009 | 0.2762 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0132 | 0.0000 | 0.0018 | 2.7074 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0112 | 0.0000 | 0.0008 | 0.0412 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0119 | 0.0000 | 0.0015 | 0.9110 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.7299 | 0.0005 | 0.0491 | 0.9442 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.1871 | 0.0004 | 0.0129 | 0.5690 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000000 | 0.1928 | 0.0005 | 0.0214 | 2.5767 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1689 | 0.0003 | 0.0139 | 0.2830 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 0.1871 | 0.0004 | 0.0221 | 4.5243 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.2547 | 0.0003 | 0.0134 | 0.3394 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.2236 | 0.0002 | 0.0107 | 0.6167 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000000 | 0.2633 | 0.0003 | 0.0213 | 3.4937 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.2399 | 0.0002 | 0.0153 | 0.3570 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 0.2236 | 0.0001 | 0.0193 | 4.1886 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.1351 | 0.0220 | 0.0453 | 0.4676 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.1275 | 0.0197 | 0.0547 | 0.5651 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000000 | 0.1338 | 0.0196 | 0.0851 | 1.4162 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.1401 | 0.0166 | 0.0610 | 0.5537 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 0.2273 | 0.0177 | 0.1082 | 10.4910 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0236 | 0.0000 | 0.0010 | 0.0412 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.0153 | 0.0000 | 0.0015 | 0.1732 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000000 | 0.0182 | 0.0000 | 0.0023 | 1.4961 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0142 | 0.0000 | 0.0017 | 0.0662 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 0.0131 | 0.0000 | 0.0032 | 2.5073 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0113 | 0.0000 | 0.0008 | 0.0948 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.0109 | 0.0000 | 0.0008 | 0.8475 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000000 | 0.0113 | 0.0000 | 0.0021 | 8.9536 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0135 | 0.0000 | 0.0008 | 0.1177 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.0126 | 0.0000 | 0.0017 | 1.3492 |
