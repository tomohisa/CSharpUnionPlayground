| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0512 | 512.0 | 30.1 | 1000 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.1807 | 180.7 | 31.9 | 10000 | 0.55 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 1.1884 | 118.8 | 45.5 | 100000 | 0.82 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0624 | 623.9 | 30.3 | 1000 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.9854 | 985.4 | 32.4 | 10000 | 1.20 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0237 | 237.5 | 0.8 | 1000 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2489 | 248.9 | 2.6 | 10000 | 1.02 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 2.3730 | 237.3 | 15.3 | 100000 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0248 | 248.4 | 1.0 | 1000 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.7454 | 745.4 | 5.2 | 10000 | 1.48 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.0346 | 346.4 | 82.1 | 1000 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.1651 | 165.1 | 97.2 | 10000 | 0.68 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000000 | 1.3743 | 137.4 | 169.1 | 100000 | 0.92 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.0813 | 813.0 | 82.9 | 1000 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 3.1329 | 3132.9 | 205.0 | 10000 | 1.59 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.0648 | 648.5 | 90.4 | 1000 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.3205 | 320.5 | 104.0 | 10000 | 0.69 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000000 | 2.8435 | 284.4 | 180.9 | 100000 | 0.95 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.1234 | 1234.1 | 91.7 | 1000 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 3.5866 | 3586.6 | 213.7 | 10000 | 1.46 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.2439 | 2439.1 | 54.9 | 1000 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.3372 | 337.2 | 89.5 | 10000 | 0.14 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000000 | 1.0865 | 108.6 | 274.0 | 100000 | 0.51 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.2252 | 2252.1 | 86.6 | 1000 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 4.1632 | 4163.2 | 1223.8 | 10000 | 1.27 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0276 | 276.2 | 6.4 | 1000 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.1425 | 142.5 | 7.8 | 10000 | 0.71 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000000 | 1.2749 | 127.5 | 23.2 | 100000 | 0.95 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0255 | 254.9 | 7.8 | 1000 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 2.3832 | 2383.2 | 24.1 | 10000 | 1.97 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0756 | 755.9 | 1.1 | 1000 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.7308 | 730.8 | 2.5 | 10000 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000000 | 7.6071 | 760.7 | 18.9 | 100000 | 1.02 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0791 | 790.9 | 1.0 | 1000 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 1.0171 | 1017.1 | 2.7 | 10000 | 1.11 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0677 | 0.0052 | 0.0043 | 0.1335 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0484 | 0.0036 | 0.0044 | 0.2417 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.0640 | 0.0043 | 0.0061 | 1.2704 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0549 | 0.0037 | 0.0049 | 0.1314 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.0616 | 0.0044 | 0.0059 | 1.0634 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0113 | 0.0000 | 0.0009 | 0.0359 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0119 | 0.0000 | 0.0011 | 0.2620 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0125 | 0.0000 | 0.0020 | 2.3882 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0089 | 0.0000 | 0.0007 | 0.0344 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0088 | 0.0000 | 0.0010 | 0.7553 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.1317 | 0.0002 | 0.0101 | 0.1774 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.1230 | 0.0002 | 0.0074 | 0.2977 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000000 | 0.1227 | 0.0002 | 0.0185 | 1.5315 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1547 | 0.0003 | 0.0105 | 0.2475 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 0.1310 | 0.0002 | 0.0146 | 3.2806 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.2384 | 0.0004 | 0.0131 | 0.3176 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.2130 | 0.0003 | 0.0094 | 0.5456 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000000 | 0.2676 | 0.0002 | 0.0264 | 3.1529 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.2805 | 0.0002 | 0.0154 | 0.4204 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 0.2234 | 0.0002 | 0.0203 | 3.8329 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.1526 | 0.0240 | 0.0463 | 0.5059 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.1328 | 0.0164 | 0.0354 | 0.5724 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000000 | 0.1299 | 0.0166 | 0.0328 | 1.3331 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.1114 | 0.0147 | 0.0155 | 0.4013 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 0.1431 | 0.0181 | 0.0805 | 4.4589 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0264 | 0.0000 | 0.0013 | 0.0580 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.0155 | 0.0000 | 0.0016 | 0.1599 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000000 | 0.0123 | 0.0000 | 0.0017 | 1.2898 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0104 | 0.0000 | 0.0013 | 0.0374 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 0.0114 | 0.0000 | 0.0018 | 2.3970 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0093 | 0.0000 | 0.0012 | 0.0863 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.0089 | 0.0000 | 0.0010 | 0.7410 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000000 | 0.0135 | 0.0000 | 0.0013 | 7.6222 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0109 | 0.0000 | 0.0007 | 0.0910 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.0107 | 0.0000 | 0.0009 | 1.0290 |
