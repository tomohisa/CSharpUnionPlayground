| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0003 | 315.0 | 3.4 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0030 | 296.0 | 4.0 | 100 | 0.97 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0316 | 316.0 | 9.6 | 1000 | 1.03 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.3169 | 316.9 | 11.3 | 10000 | 1.00 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0004 | 432.2 | 3.4 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0038 | 379.1 | 4.4 | 1 | 0.94 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0385 | 384.8 | 14.8 | 1 | 1.01 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 1.3727 | 1372.7 | 65.0 | 1 | 1.55 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0005 | 469.5 | 3.4 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0068 | 680.4 | 5.3 | 100 | 1.16 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2424 | 2424.2 | 9.9 | 1000 | 1.55 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 23.0058 | 23005.8 | 35.7 | 10000 | 1.98 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0004 | 401.7 | 3.4 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0042 | 418.2 | 4.6 | 1 | 1.02 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0533 | 533.2 | 14.6 | 1 | 1.11 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.5200 | 520.0 | 63.4 | 1 | 0.99 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0003 | 253.9 | 3.3 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0024 | 236.5 | 4.0 | 100 | 0.97 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0258 | 258.1 | 9.6 | 1000 | 1.04 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.2766 | 276.6 | 11.3 | 10000 | 1.03 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0003 | 324.5 | 3.4 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0062 | 615.2 | 4.4 | 1 | 1.28 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.5825 | 5825.2 | 12.2 | 1 | 1.98 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0023 | 2278.4 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0241 | 2412.2 | 0.7 | 100 | 1.02 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.1727 | 1727.1 | 0.8 | 1000 | 0.85 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 1.8855 | 1885.5 | 2.6 | 10000 | 1.04 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0016 | 1603.5 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0174 | 1741.3 | 1.1 | 1 | 1.04 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.1697 | 1697.3 | 7.3 | 1 | 0.99 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 2.3403 | 2340.3 | 54.3 | 1 | 1.14 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0020 | 2017.0 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0183 | 1834.5 | 0.8 | 100 | 0.96 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.1813 | 1812.7 | 1.4 | 1000 | 0.99 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 2.4907 | 2490.7 | 6.2 | 10000 | 1.14 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0016 | 1562.6 | 0.8 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0173 | 1726.6 | 1.1 | 1 | 1.04 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.1570 | 1569.8 | 7.3 | 1 | 0.96 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 2.1771 | 2177.1 | 54.3 | 1 | 1.14 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0018 | 1848.2 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0176 | 1758.2 | 0.7 | 100 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.2106 | 2105.5 | 0.9 | 1000 | 1.08 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 1.7130 | 1713.0 | 2.6 | 10000 | 0.91 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0018 | 1832.1 | 0.8 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0158 | 1578.4 | 1.0 | 1 | 0.94 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.2235 | 2234.9 | 4.7 | 1 | 1.15 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 13.5211 | 13521.1 | 43.1 | 1 | 1.78 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.0062 | 6208.0 | 68.7 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.0183 | 1831.5 | 68.8 | 100 | 0.47 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.0553 | 553.4 | 81.7 | 1000 | 0.48 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.4255 | 425.5 | 98.3 | 10000 | 0.89 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.0084 | 8387.0 | 68.5 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.0171 | 1705.1 | 70.5 | 1 | 0.31 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.0937 | 936.6 | 92.7 | 1 | 0.74 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 1.6234 | 1623.4 | 224.3 | 1 | 1.24 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.0086 | 8583.6 | 68.5 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.0540 | 5399.7 | 68.7 | 100 | 0.80 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.0907 | 907.3 | 81.8 | 1000 | 0.23 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 4.7652 | 4765.2 | 202.7 | 10000 | 1.72 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.0091 | 9141.5 | 68.2 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.0189 | 1892.1 | 70.4 | 1 | 0.32 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.0960 | 960.2 | 91.6 | 1 | 0.71 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 0.7048 | 704.8 | 187.9 | 1 | 0.87 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.0155 | 15548.7 | 69.8 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.0167 | 1667.5 | 70.1 | 100 | 0.03 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.0755 | 754.7 | 80.6 | 1000 | 0.66 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.5907 | 590.7 | 94.9 | 10000 | 0.89 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.0089 | 8904.8 | 68.2 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.0196 | 1963.6 | 70.6 | 1 | 0.34 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.1563 | 1562.7 | 93.1 | 1 | 0.90 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.0091 | 9091.9 | 76.2 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.0223 | 2231.0 | 81.5 | 100 | 0.39 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.0747 | 747.4 | 88.5 | 1000 | 0.53 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.4835 | 483.5 | 102.0 | 10000 | 0.81 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.0089 | 8947.9 | 76.0 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.0203 | 2029.7 | 81.0 | 1 | 0.36 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.0932 | 932.3 | 101.0 | 1 | 0.66 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 1.2116 | 1211.6 | 262.9 | 1 | 1.11 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.0088 | 8777.1 | 76.1 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.0217 | 2168.6 | 80.7 | 100 | 0.39 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.1120 | 1120.3 | 91.9 | 1000 | 0.71 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 3.4298 | 3429.8 | 215.0 | 10000 | 1.49 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.0232 | 23188.9 | 75.8 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.0201 | 2005.9 | 81.0 | 1 | -0.06 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.0884 | 884.3 | 101.1 | 1 | 0.64 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 0.7624 | 762.4 | 218.7 | 1 | 0.94 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.0095 | 9452.3 | 76.0 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.0210 | 2102.2 | 80.4 | 100 | 0.35 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.0733 | 732.6 | 91.2 | 1000 | 0.54 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.4770 | 477.0 | 101.0 | 10000 | 0.81 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.0087 | 8722.8 | 76.2 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.0245 | 2454.2 | 81.2 | 1 | 0.45 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.1712 | 1712.2 | 101.3 | 1 | 0.84 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000000 | 15.2697 | 15269.7 | 257.2 | 1 | 1.95 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.0903 | 90259.1 | 39.4 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.1547 | 15473.0 | 52.7 | 100 | 0.23 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.2537 | 2536.7 | 55.3 | 1000 | 0.21 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.4780 | 478.0 | 90.8 | 10000 | 0.28 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.0853 | 85292.8 | 39.1 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.1522 | 15222.5 | 52.8 | 1 | 0.25 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.2662 | 2662.2 | 60.8 | 1 | 0.24 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 0.5878 | 587.8 | 163.4 | 1 | 0.34 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.0884 | 88395.0 | 39.1 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.1557 | 15575.0 | 53.8 | 100 | 0.25 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.2351 | 2351.2 | 85.4 | 1000 | 0.18 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 6.0940 | 6094.0 | 1011.1 | 10000 | 1.41 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.0953 | 95252.0 | 39.2 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.1701 | 17010.5 | 51.7 | 1 | 0.25 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.2304 | 2304.0 | 60.2 | 1 | 0.13 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 0.4932 | 493.2 | 152.8 | 1 | 0.33 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.0834 | 83414.6 | 39.1 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.1484 | 14838.9 | 52.9 | 100 | 0.25 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.2167 | 2166.7 | 56.5 | 1000 | 0.16 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.4209 | 420.9 | 90.0 | 10000 | 0.29 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.0884 | 88361.5 | 39.4 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.1581 | 15809.1 | 52.2 | 1 | 0.25 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.2600 | 2599.5 | 61.8 | 1 | 0.22 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000000 | 21.0619 | 21061.9 | 115.4 | 1 | 1.91 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0004 | 364.4 | 2.3 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0028 | 282.3 | 2.5 | 100 | 0.89 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0281 | 280.6 | 6.4 | 1000 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.2986 | 298.6 | 7.6 | 10000 | 1.03 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0004 | 388.9 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0033 | 327.1 | 2.9 | 1 | 0.92 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0327 | 326.7 | 7.9 | 1 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.3273 | 327.3 | 53.7 | 1 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0003 | 323.6 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0032 | 323.1 | 3.0 | 100 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0382 | 382.1 | 7.6 | 1000 | 1.07 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 1.7329 | 1732.9 | 34.4 | 10000 | 1.66 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0004 | 356.1 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0032 | 320.3 | 2.9 | 1 | 0.95 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0334 | 334.2 | 7.9 | 1 | 1.02 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.3213 | 321.3 | 49.6 | 1 | 0.98 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0003 | 309.3 | 2.1 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0028 | 276.8 | 2.5 | 100 | 0.95 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0280 | 279.6 | 6.3 | 1000 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.2651 | 265.1 | 7.4 | 10000 | 0.98 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0004 | 374.7 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0034 | 342.1 | 2.9 | 1 | 0.96 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.0706 | 705.6 | 7.2 | 1 | 1.31 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 13.4293 | 13429.3 | 43.7 | 1 | 2.28 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0060 | 6016.9 | 1.0 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0587 | 5868.4 | 0.9 | 100 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.5729 | 5728.8 | 1.0 | 1000 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 5.8765 | 5876.5 | 2.5 | 10000 | 1.01 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0059 | 5929.6 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.0583 | 5826.0 | 1.2 | 1 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 0.5866 | 5866.5 | 4.0 | 1 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 5.8337 | 5833.7 | 29.9 | 1 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0059 | 5898.1 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.0577 | 5770.8 | 0.9 | 100 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.5786 | 5785.8 | 1.1 | 1000 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 6.0059 | 6005.9 | 3.4 | 10000 | 1.02 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0061 | 6078.1 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.0584 | 5835.4 | 1.1 | 1 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 0.5835 | 5835.2 | 4.0 | 1 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 5.9777 | 5977.7 | 29.9 | 1 | 1.01 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0059 | 5901.8 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.0579 | 5793.6 | 0.9 | 100 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 0.5771 | 5770.9 | 1.1 | 1000 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 5.8845 | 5884.5 | 2.5 | 10000 | 1.01 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0060 | 6032.9 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.0674 | 6744.2 | 1.1 | 1 | 1.05 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 1.4608 | 14608.0 | 3.5 | 1 | 1.34 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0323 | 0.0018 | 0.0010 | 0.0355 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0166 | 0.0011 | 0.0011 | 0.0219 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0188 | 0.0010 | 0.0022 | 0.0539 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0215 | 0.0007 | 0.0022 | 0.3414 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0192 | 0.0012 | 0.0012 | 0.0221 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0201 | 0.0010 | 0.0013 | 0.0264 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0205 | 0.0011 | 0.0066 | 0.0836 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.0314 | 0.0028 | 0.0413 | 1.5042 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0214 | 0.0014 | 0.0016 | 0.0249 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0215 | 0.0014 | 0.0016 | 0.0317 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0203 | 0.0011 | 0.0023 | 0.2663 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.0195 | 0.0012 | 0.0030 | 23.0299 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0168 | 0.0015 | 0.0013 | 0.0200 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0142 | 0.0010 | 0.0018 | 0.0217 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0165 | 0.0010 | 0.0026 | 0.0752 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.0168 | 0.0013 | 0.0068 | 0.5874 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0120 | 0.0008 | 0.0010 | 0.0141 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0120 | 0.0008 | 0.0011 | 0.0165 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0131 | 0.0010 | 0.0019 | 0.0420 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.0178 | 0.0010 | 0.0019 | 0.2977 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0161 | 0.0009 | 0.0013 | 0.0188 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0164 | 0.0013 | 0.0014 | 0.0257 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.0191 | 0.0015 | 0.0024 | 0.6084 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0192 | 0.0000 | 0.0011 | 0.0227 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0221 | 0.0000 | 0.0015 | 0.0478 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0169 | 0.0000 | 0.0007 | 0.1904 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0131 | 0.0000 | 0.0016 | 1.9003 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0139 | 0.0000 | 0.0008 | 0.0163 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0154 | 0.0000 | 0.0010 | 0.0339 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0137 | 0.0000 | 0.0032 | 0.1873 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0142 | 0.0000 | 0.0548 | 2.4263 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0156 | 0.0000 | 0.0009 | 0.0185 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0149 | 0.0000 | 0.0009 | 0.0341 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0148 | 0.0000 | 0.0008 | 0.1969 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0143 | 0.0000 | 0.0017 | 2.5068 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0150 | 0.0000 | 0.0007 | 0.0173 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0109 | 0.0000 | 0.0010 | 0.0293 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0093 | 0.0000 | 0.0029 | 0.1698 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0112 | 0.0000 | 0.0577 | 2.2572 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0122 | 0.0000 | 0.0010 | 0.0152 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0159 | 0.0000 | 0.0011 | 0.0327 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0122 | 0.0000 | 0.0006 | 0.2235 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0131 | 0.0000 | 0.0012 | 1.7275 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0116 | 0.0000 | 0.0008 | 0.0143 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0111 | 0.0000 | 0.0009 | 0.0279 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0124 | 0.0000 | 0.0030 | 0.2392 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0114 | 0.0000 | 0.0439 | 13.5840 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.1515 | 0.0003 | 0.0093 | 0.1679 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.1685 | 0.0002 | 0.0114 | 0.1992 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.1391 | 0.0003 | 0.0105 | 0.2059 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.1415 | 0.0003 | 0.0140 | 0.5839 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.2454 | 0.0004 | 0.0131 | 0.2680 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.1970 | 0.0004 | 0.0121 | 0.2297 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.2136 | 0.0004 | 0.0238 | 0.3593 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 0.1950 | 0.0004 | 0.0491 | 2.2084 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.1687 | 0.0005 | 0.0095 | 0.1879 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.1765 | 0.0004 | 0.0197 | 0.2512 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1402 | 0.0004 | 0.0098 | 0.2417 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 0.1512 | 0.0003 | 0.0351 | 4.9545 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.1771 | 0.0005 | 0.0083 | 0.1957 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.1641 | 0.0004 | 0.0082 | 0.1944 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.1788 | 0.0004 | 0.0180 | 0.3070 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 0.1771 | 0.0005 | 0.0301 | 0.9588 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.1745 | 0.0005 | 0.0140 | 0.2053 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.1715 | 0.0004 | 0.0111 | 0.2004 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.1745 | 0.0005 | 0.0159 | 0.2673 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.1990 | 0.0004 | 0.0419 | 0.8373 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.2448 | 0.0004 | 0.0090 | 0.2639 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.1667 | 0.0003 | 0.0077 | 0.1960 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.1446 | 0.0003 | 0.0101 | 0.3248 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.1983 | 0.0002 | 0.0073 | 0.2154 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.1883 | 0.0003 | 0.0113 | 0.2227 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.1832 | 0.0003 | 0.0120 | 0.2709 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.1817 | 0.0003 | 0.0091 | 0.6765 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.1839 | 0.0003 | 0.0070 | 0.2008 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.1790 | 0.0003 | 0.0112 | 0.2122 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.1881 | 0.0003 | 0.0109 | 0.2986 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 0.1990 | 0.0003 | 0.0370 | 1.5214 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.1908 | 0.0003 | 0.0068 | 0.2072 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.3137 | 0.0002 | 0.0119 | 0.3481 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.2078 | 0.0003 | 0.0386 | 0.3594 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 0.1843 | 0.0003 | 0.0218 | 3.6381 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.1956 | 0.0003 | 0.0172 | 0.2388 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.1842 | 0.0002 | 0.0104 | 0.2162 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.1705 | 0.0003 | 0.0103 | 0.2752 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 0.2270 | 0.0002 | 0.0222 | 1.0486 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.1823 | 0.0003 | 0.0074 | 0.1999 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.1896 | 0.0003 | 0.0112 | 0.2226 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.1804 | 0.0003 | 0.0155 | 0.2701 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.1776 | 0.0003 | 0.0079 | 0.6645 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.1797 | 0.0003 | 0.0064 | 0.1955 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.1764 | 0.0002 | 0.0102 | 0.2130 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.1743 | 0.0002 | 0.0098 | 0.3613 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000000 | 0.1746 | 0.0002 | 0.0302 | 15.5133 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.1364 | 0.0152 | 0.0426 | 0.3219 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.1072 | 0.0129 | 0.0398 | 0.3446 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.1125 | 0.0143 | 0.0203 | 0.4406 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.1055 | 0.0152 | 0.0059 | 0.6423 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.1075 | 0.0131 | 0.0699 | 0.3070 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.1094 | 0.0136 | 0.0450 | 0.3518 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.1096 | 0.0129 | 0.0383 | 0.4710 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 0.1180 | 0.0275 | 0.0558 | 0.8623 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.1059 | 0.0144 | 0.0460 | 0.2863 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.1077 | 0.0132 | 0.0727 | 0.3801 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.1076 | 0.0131 | 0.0369 | 0.4228 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 0.1157 | 0.0133 | 0.0923 | 6.3807 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.1234 | 0.0142 | 0.0485 | 0.3122 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.1125 | 0.0138 | 0.0489 | 0.3779 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.1059 | 0.0135 | 0.0133 | 0.3970 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 0.1097 | 0.0131 | 0.0196 | 0.6951 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.1071 | 0.0132 | 0.0277 | 0.2608 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.1065 | 0.0132 | 0.0427 | 0.3404 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.1065 | 0.0131 | 0.0546 | 0.4215 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.1053 | 0.0129 | 0.0064 | 0.5806 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.1037 | 0.0130 | 0.0342 | 0.2716 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.1076 | 0.0133 | 0.0253 | 0.3354 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.1076 | 0.0129 | 0.0391 | 0.4571 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000000 | 0.1072 | 0.0135 | 0.0519 | 21.2871 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0114 | 0.0000 | 0.0009 | 0.0127 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0091 | 0.0000 | 0.0010 | 0.0130 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0090 | 0.0000 | 0.0009 | 0.0382 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.0092 | 0.0000 | 0.0010 | 0.3090 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0091 | 0.0000 | 0.0008 | 0.0104 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0091 | 0.0000 | 0.0008 | 0.0135 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0096 | 0.0000 | 0.0010 | 0.0453 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.0092 | 0.0000 | 0.0026 | 0.3774 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0092 | 0.0000 | 0.0008 | 0.0104 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0091 | 0.0000 | 0.0007 | 0.0132 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0090 | 0.0000 | 0.0010 | 0.0484 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 0.0089 | 0.0000 | 0.0019 | 1.7440 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0089 | 0.0000 | 0.0006 | 0.0100 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0088 | 0.0000 | 0.0014 | 0.0137 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0094 | 0.0000 | 0.0009 | 0.0456 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.0089 | 0.0000 | 0.0026 | 0.3709 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0090 | 0.0000 | 0.0008 | 0.0102 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0090 | 0.0000 | 0.0008 | 0.0126 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0092 | 0.0000 | 0.0009 | 0.0382 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.0094 | 0.0000 | 0.0009 | 0.2757 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0091 | 0.0000 | 0.0010 | 0.0105 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0090 | 0.0000 | 0.0007 | 0.0134 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.0091 | 0.0000 | 0.0009 | 0.0819 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 0.0091 | 0.0000 | 0.0028 | 13.4690 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0090 | 0.0000 | 0.0011 | 0.0162 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0087 | 0.0000 | 0.0016 | 0.0691 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0087 | 0.0000 | 0.0006 | 0.5824 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.0088 | 0.0000 | 0.0012 | 5.8868 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0084 | 0.0000 | 0.0005 | 0.0149 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.0088 | 0.0000 | 0.0006 | 0.0680 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 0.0085 | 0.0000 | 0.0008 | 0.5982 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 0.0088 | 0.0000 | 0.0018 | 5.8751 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0087 | 0.0000 | 0.0006 | 0.0153 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.0086 | 0.0000 | 0.0006 | 0.0670 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0086 | 0.0000 | 0.0007 | 0.5880 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.0083 | 0.0000 | 0.0008 | 6.0152 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0087 | 0.0000 | 0.0007 | 0.0156 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.0088 | 0.0000 | 0.0006 | 0.0682 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 0.0086 | 0.0000 | 0.0008 | 0.5951 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 0.0090 | 0.0000 | 0.0019 | 6.0198 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0085 | 0.0000 | 0.0006 | 0.0150 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.0088 | 0.0000 | 0.0006 | 0.0675 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 0.0084 | 0.0000 | 0.0007 | 0.5864 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 0.0090 | 0.0000 | 0.0008 | 5.8945 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0087 | 0.0000 | 0.0006 | 0.0154 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.0085 | 0.0000 | 0.0007 | 0.0769 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 0.0087 | 0.0000 | 0.0008 | 1.4721 |
