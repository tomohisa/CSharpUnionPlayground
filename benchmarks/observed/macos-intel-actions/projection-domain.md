| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0781 | 780.8 | 30.5 | 1000 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.3788 | 378.8 | 32.2 | 10000 | 0.69 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 1.8744 | 187.4 | 45.3 | 100000 | 0.69 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0957 | 957.4 | 30.7 | 1000 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 1.3475 | 1347.5 | 32.3 | 10000 | 1.15 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0288 | 288.0 | 0.9 | 1000 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.3034 | 303.4 | 2.6 | 10000 | 1.02 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 2.9161 | 291.6 | 15.3 | 100000 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0284 | 283.7 | 1.0 | 1000 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 1.0498 | 1049.8 | 5.3 | 10000 | 1.57 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.0560 | 560.1 | 74.0 | 1000 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.2453 | 245.3 | 97.5 | 10000 | 0.64 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000000 | 1.9955 | 199.5 | 169.3 | 100000 | 0.91 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.0988 | 987.8 | 83.0 | 1000 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 4.4055 | 4405.5 | 205.8 | 10000 | 1.65 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.1484 | 1484.2 | 90.3 | 1000 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.5451 | 545.1 | 102.9 | 10000 | 0.56 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000000 | 3.6187 | 361.9 | 177.4 | 100000 | 0.82 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.1302 | 1302.3 | 92.1 | 1000 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 4.4849 | 4484.9 | 213.4 | 10000 | 1.54 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.2338 | 2338.5 | 54.8 | 1000 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.3582 | 358.2 | 89.0 | 10000 | 0.19 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000000 | 1.2414 | 124.1 | 272.5 | 100000 | 0.54 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.2565 | 2565.4 | 86.9 | 1000 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 2.4420 | 2442.0 | 1380.2 | 10000 | 0.98 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0163 | 163.2 | 6.3 | 1000 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.1386 | 138.6 | 7.6 | 10000 | 0.93 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000000 | 1.4228 | 142.3 | 21.9 | 100000 | 1.01 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0414 | 414.0 | 11.1 | 1000 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 2.3215 | 2321.5 | 35.0 | 10000 | 1.75 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0771 | 771.3 | 1.1 | 1000 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.7315 | 731.5 | 2.5 | 10000 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000000 | 8.2978 | 829.8 | 18.8 | 100000 | 1.05 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0814 | 813.6 | 1.0 | 1000 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.9878 | 987.8 | 2.7 | 10000 | 1.08 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.1463 | 0.0066 | 0.0059 | 0.2450 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.1090 | 0.0063 | 0.0063 | 0.5078 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.1161 | 0.0063 | 0.0075 | 2.0165 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0976 | 0.0060 | 0.0064 | 0.2135 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.1095 | 0.0061 | 0.0071 | 1.4794 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0169 | 0.0000 | 0.0012 | 0.0469 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0124 | 0.0000 | 0.0012 | 0.3171 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0131 | 0.0000 | 0.0020 | 2.9317 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0099 | 0.0000 | 0.0007 | 0.0390 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0106 | 0.0000 | 0.0017 | 1.0622 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.3472 | 0.0007 | 0.0160 | 0.4214 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.1597 | 0.0004 | 0.0112 | 0.4201 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000000 | 0.1649 | 0.0004 | 0.0202 | 2.1996 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1601 | 0.0003 | 0.0129 | 0.2730 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 0.1728 | 0.0003 | 0.0227 | 4.6051 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.2896 | 0.0002 | 0.0555 | 0.4946 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.2503 | 0.0003 | 0.0135 | 0.8120 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000000 | 0.2710 | 0.0004 | 0.0228 | 3.9331 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.2504 | 0.0004 | 0.0146 | 0.3966 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 0.2367 | 0.0003 | 0.0268 | 4.7513 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.1437 | 0.0187 | 0.0128 | 0.4507 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.1440 | 0.0196 | 0.0582 | 0.6364 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000000 | 0.1318 | 0.0221 | 0.0618 | 1.5342 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.1346 | 0.0176 | 0.0628 | 0.5146 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 0.1282 | 0.0162 | 0.0851 | 2.7309 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0140 | 0.0000 | 0.0011 | 0.0317 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.0121 | 0.0000 | 0.0014 | 0.1523 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000000 | 0.0126 | 0.0000 | 0.0018 | 1.4382 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0129 | 0.0000 | 0.0017 | 0.0561 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 0.0118 | 0.0000 | 0.0024 | 2.3362 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0098 | 0.0000 | 0.0011 | 0.0882 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.0097 | 0.0000 | 0.0008 | 0.7422 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000000 | 0.0111 | 0.0000 | 0.0025 | 8.3125 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0090 | 0.0000 | 0.0007 | 0.0913 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.0096 | 0.0000 | 0.0008 | 0.9984 |
