| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0613 | 61280.2 | 24.2 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.1099 | 10993.5 | 25.0 | 100 | 0.25 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.1319 | 1318.6 | 30.4 | 1000 | 0.08 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 1.3037 | 1303.7 | 32.5 | 10000 | 1.00 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0664 | 66423.2 | 24.2 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.1095 | 10949.6 | 25.3 | 1 | 0.22 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.1523 | 1523.1 | 35.5 | 1 | 0.14 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 1.7217 | 1721.7 | 86.2 | 1 | 1.05 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0629 | 62885.4 | 24.1 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0586 | 5863.3 | 25.4 | 100 | -0.03 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.1529 | 1529.4 | 30.7 | 1000 | 0.42 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 2.2520 | 2252.0 | 32.4 | 10000 | 1.17 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0481 | 48108.5 | 24.2 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0591 | 5910.5 | 25.2 | 1 | 0.09 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.1494 | 1493.6 | 35.2 | 1 | 0.40 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 1.5269 | 1526.9 | 84.9 | 1 | 1.01 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0418 | 41783.3 | 24.1 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0525 | 5253.4 | 24.9 | 100 | 0.10 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.1405 | 1404.8 | 30.4 | 1000 | 0.43 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.8050 | 805.0 | 32.7 | 10000 | 0.76 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0542 | 54210.3 | 24.3 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0533 | 5327.5 | 25.0 | 1 | -0.01 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.8855 | 8854.6 | 33.6 | 1 | 1.22 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0020 | 1953.2 | 0.8 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0192 | 1917.9 | 0.7 | 100 | 0.99 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.2369 | 2368.9 | 0.9 | 1000 | 1.09 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 2.0676 | 2067.6 | 2.6 | 10000 | 0.94 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0021 | 2110.9 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0203 | 2032.1 | 1.1 | 1 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.2077 | 2076.6 | 7.3 | 1 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 2.1131 | 2113.1 | 54.3 | 1 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0020 | 2024.2 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0202 | 2024.2 | 0.8 | 100 | 1.00 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.2120 | 2120.2 | 1.5 | 1000 | 1.02 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 3.4285 | 3428.5 | 6.4 | 10000 | 1.21 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0020 | 2013.4 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0202 | 2022.8 | 1.1 | 1 | 1.00 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.2086 | 2086.3 | 7.4 | 1 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 2.3648 | 2364.8 | 54.3 | 1 | 1.05 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0021 | 2104.7 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0200 | 1999.7 | 0.7 | 100 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.1967 | 1967.1 | 0.8 | 1000 | 0.99 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 1.9691 | 1969.1 | 2.6 | 10000 | 1.00 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0022 | 2227.1 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0201 | 2012.1 | 1.1 | 1 | 0.96 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.2445 | 2444.6 | 4.7 | 1 | 1.08 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 16.4698 | 16469.8 | 43.1 | 1 | 1.83 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.0090 | 9022.7 | 68.5 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.0174 | 1742.5 | 68.6 | 100 | 0.29 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.0716 | 716.0 | 80.4 | 1000 | 0.61 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.5136 | 513.6 | 97.2 | 10000 | 0.86 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.0093 | 9297.4 | 69.6 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.0198 | 1981.9 | 70.1 | 1 | 0.33 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.1048 | 1048.3 | 94.0 | 1 | 0.72 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 0.7349 | 734.9 | 223.8 | 1 | 0.85 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.0099 | 9937.5 | 68.5 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.0199 | 1985.8 | 71.1 | 100 | 0.30 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1982 | 1982.4 | 81.8 | 1000 | 1.00 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 6.1866 | 6186.6 | 203.9 | 10000 | 1.49 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.0086 | 8567.2 | 68.8 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.0208 | 2078.1 | 69.2 | 1 | 0.38 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.1003 | 1003.3 | 90.2 | 1 | 0.68 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 1.0509 | 1050.9 | 186.5 | 1 | 1.02 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.0162 | 16211.2 | 67.8 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.0279 | 2786.4 | 69.3 | 100 | 0.24 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.0943 | 943.0 | 83.1 | 1000 | 0.53 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.5667 | 566.7 | 96.0 | 10000 | 0.78 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.0065 | 6488.9 | 69.3 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.0255 | 2552.2 | 70.5 | 1 | 0.59 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.2285 | 2285.2 | 94.1 | 1 | 0.95 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.0222 | 22235.1 | 75.1 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.0253 | 2527.6 | 80.8 | 100 | 0.06 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.1070 | 1070.3 | 91.0 | 1000 | 0.63 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 1.0822 | 1082.2 | 101.8 | 10000 | 1.00 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.0185 | 18468.0 | 76.3 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.0274 | 2737.4 | 81.4 | 1 | 0.17 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.1489 | 1489.0 | 101.1 | 1 | 0.74 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 3.6409 | 3640.9 | 264.8 | 1 | 1.39 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.0133 | 13276.3 | 77.2 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.0674 | 6739.8 | 82.3 | 100 | 0.71 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.1948 | 1948.4 | 90.9 | 1000 | 0.46 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 5.7271 | 5727.1 | 214.9 | 10000 | 1.47 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.0268 | 26809.8 | 75.2 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.0336 | 3361.2 | 81.6 | 1 | 0.10 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.2077 | 2076.9 | 100.7 | 1 | 0.79 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 0.9659 | 965.9 | 219.2 | 1 | 0.67 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.0174 | 17410.8 | 76.0 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.0426 | 4258.5 | 80.9 | 100 | 0.39 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.0997 | 997.2 | 92.2 | 1000 | 0.37 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.9144 | 914.4 | 101.9 | 10000 | 0.96 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.0128 | 12760.7 | 75.4 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.0537 | 5374.6 | 81.3 | 1 | 0.62 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.3398 | 3398.2 | 102.1 | 1 | 0.80 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.1493 | 149287.0 | 38.9 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.2505 | 25053.3 | 49.9 | 100 | 0.22 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.3947 | 3947.2 | 55.8 | 1000 | 0.20 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.7751 | 775.1 | 91.4 | 10000 | 0.29 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.1474 | 147395.5 | 38.1 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.2200 | 21999.2 | 52.8 | 1 | 0.17 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.5077 | 5077.0 | 60.5 | 1 | 0.36 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 0.7889 | 788.9 | 145.2 | 1 | 0.19 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.2066 | 206607.0 | 38.3 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.3456 | 34558.5 | 52.8 | 100 | 0.22 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.3424 | 3423.8 | 84.9 | 1000 | -0.00 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 5.3496 | 5349.6 | 1045.1 | 10000 | 1.19 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.1023 | 102337.4 | 39.2 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.1783 | 17830.6 | 52.9 | 1 | 0.24 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.2960 | 2960.3 | 61.2 | 1 | 0.22 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 0.5462 | 546.2 | 152.7 | 1 | 0.27 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.1000 | 100045.1 | 39.5 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.2064 | 20642.9 | 53.5 | 100 | 0.31 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.2778 | 2777.7 | 56.4 | 1000 | 0.13 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.5627 | 562.7 | 92.0 | 10000 | 0.31 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.1058 | 105808.8 | 38.2 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.2226 | 22260.1 | 53.2 | 1 | 0.32 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.3958 | 3958.0 | 61.0 | 1 | 0.25 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0004 | 386.6 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0059 | 591.9 | 2.6 | 100 | 1.18 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0367 | 367.4 | 6.4 | 1000 | 0.79 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.3944 | 394.4 | 7.5 | 10000 | 1.03 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0014 | 1376.9 | 2.2 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0106 | 1060.7 | 2.9 | 1 | 0.89 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0435 | 435.0 | 7.9 | 1 | 0.61 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.5145 | 514.5 | 50.6 | 1 | 1.07 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0005 | 515.5 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0046 | 464.5 | 3.1 | 100 | 0.95 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0550 | 550.1 | 7.6 | 1000 | 1.07 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 2.9152 | 2915.2 | 31.4 | 10000 | 1.72 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0034 | 3361.9 | 2.2 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0036 | 356.5 | 2.9 | 1 | 0.03 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0415 | 415.3 | 7.9 | 1 | 1.07 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.5083 | 508.3 | 52.5 | 1 | 1.09 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0006 | 564.1 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0036 | 360.4 | 2.5 | 100 | 0.81 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0307 | 307.1 | 6.3 | 1000 | 0.93 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.3333 | 333.3 | 7.5 | 10000 | 1.04 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0004 | 412.8 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0079 | 791.7 | 2.8 | 1 | 1.28 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.0791 | 791.2 | 7.4 | 1 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 16.2100 | 16210.0 | 46.2 | 1 | 2.31 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0146 | 14570.1 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0653 | 6528.7 | 0.9 | 100 | 0.65 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.6338 | 6337.9 | 1.0 | 1000 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 6.5637 | 6563.7 | 2.5 | 10000 | 1.02 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0060 | 6042.0 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.0604 | 6040.9 | 1.2 | 1 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 0.6727 | 6727.4 | 4.0 | 1 | 1.05 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 6.8286 | 6828.6 | 29.9 | 1 | 1.01 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0062 | 6163.3 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.0824 | 8242.0 | 0.9 | 100 | 1.13 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.7249 | 7248.5 | 1.0 | 1000 | 0.94 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 6.8558 | 6855.8 | 3.1 | 10000 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0062 | 6246.8 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.0621 | 6206.7 | 1.2 | 1 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 0.6252 | 6251.8 | 4.0 | 1 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 6.6394 | 6639.4 | 30.0 | 1 | 1.03 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0073 | 7309.5 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.0746 | 7455.4 | 0.9 | 100 | 1.01 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 0.7148 | 7148.5 | 1.1 | 1000 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 6.8386 | 6838.6 | 2.5 | 10000 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0063 | 6283.7 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.0760 | 7604.6 | 1.2 | 1 | 1.08 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 1.8462 | 18461.9 | 3.5 | 1 | 1.39 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.1208 | 0.0064 | 0.0606 | 0.2547 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.1038 | 0.0055 | 0.0396 | 0.2665 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.1046 | 0.0066 | 0.0068 | 0.2585 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.1134 | 0.0056 | 0.0090 | 1.4585 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.1290 | 0.0028 | 0.0463 | 0.3070 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0966 | 0.0068 | 0.0095 | 0.2581 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.1330 | 0.0065 | 0.0077 | 0.3227 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.1080 | 0.0050 | 0.0144 | 1.8958 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.1109 | 0.0064 | 0.0066 | 0.1928 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0897 | 0.0064 | 0.0060 | 0.1671 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0769 | 0.0073 | 0.0070 | 0.2522 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.0816 | 0.0062 | 0.0070 | 2.3556 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0806 | 0.0060 | 0.0066 | 0.1461 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0824 | 0.0063 | 0.0060 | 0.1599 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0684 | 0.0042 | 0.0063 | 0.2357 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.1114 | 0.0055 | 0.0142 | 1.7266 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0673 | 0.0045 | 0.0051 | 0.1223 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0750 | 0.0060 | 0.0056 | 0.1451 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0908 | 0.0063 | 0.0057 | 0.2494 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.0592 | 0.0042 | 0.0059 | 0.8802 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.1063 | 0.0041 | 0.0060 | 0.1742 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0647 | 0.0050 | 0.0047 | 0.1316 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.0610 | 0.0052 | 0.0074 | 0.9678 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0177 | 0.0000 | 0.0008 | 0.0205 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0176 | 0.0000 | 0.0008 | 0.0376 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0194 | 0.0001 | 0.0010 | 0.2572 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0173 | 0.0000 | 0.0012 | 2.0861 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0160 | 0.0000 | 0.0011 | 0.0192 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0172 | 0.0000 | 0.0011 | 0.0387 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0157 | 0.0000 | 0.0051 | 0.2295 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0159 | 0.0000 | 0.0589 | 2.1984 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0170 | 0.0000 | 0.0011 | 0.0201 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0179 | 0.0000 | 0.0012 | 0.0392 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0184 | 0.0000 | 0.0010 | 0.2315 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0189 | 0.0000 | 0.0021 | 3.4495 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0166 | 0.0000 | 0.0009 | 0.0195 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0158 | 0.0000 | 0.0012 | 0.0374 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0173 | 0.0000 | 0.0053 | 0.2322 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0152 | 0.0000 | 0.0648 | 2.4554 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0163 | 0.0000 | 0.0010 | 0.0195 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0169 | 0.0000 | 0.0010 | 0.0380 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0173 | 0.0000 | 0.0011 | 0.2150 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0182 | 0.0000 | 0.0016 | 1.9891 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0183 | 0.0000 | 0.0010 | 0.0216 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0152 | 0.0000 | 0.0010 | 0.0365 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0130 | 0.0000 | 0.0027 | 0.2604 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0134 | 0.0000 | 0.0502 | 16.5464 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.2255 | 0.0004 | 0.0126 | 0.2482 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.2511 | 0.0004 | 0.0161 | 0.2857 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.1513 | 0.0004 | 0.0107 | 0.2349 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.1856 | 0.0004 | 0.0142 | 0.7166 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.2487 | 0.0004 | 0.0119 | 0.2710 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.2007 | 0.0005 | 0.0109 | 0.2351 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.2078 | 0.0005 | 0.0570 | 0.4016 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 0.2291 | 0.0005 | 0.0311 | 1.0929 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.1793 | 0.0004 | 0.0093 | 0.1996 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.1799 | 0.0002 | 0.0103 | 0.2089 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.2401 | 0.0005 | 0.0114 | 0.4514 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 0.2032 | 0.0005 | 0.0290 | 6.4267 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.1717 | 0.0003 | 0.0087 | 0.1905 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.1617 | 0.0002 | 0.0085 | 0.1949 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.2041 | 0.0005 | 0.0144 | 0.3379 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 0.1894 | 0.0005 | 0.0381 | 1.3414 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.2264 | 0.0005 | 0.0108 | 0.2547 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.3303 | 0.0005 | 0.0120 | 0.3715 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.2195 | 0.0005 | 0.0163 | 0.3318 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.1820 | 0.0004 | 0.0118 | 0.7643 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.1677 | 0.0003 | 0.0083 | 0.1834 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.1868 | 0.0005 | 0.0138 | 0.2285 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.1853 | 0.0005 | 0.0136 | 0.4458 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.3429 | 0.0006 | 0.0100 | 0.3763 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.2969 | 0.0002 | 0.0156 | 0.3383 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.5675 | 0.0004 | 0.0181 | 0.6938 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.2844 | 0.0005 | 0.0171 | 1.3866 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.2706 | 0.0002 | 0.0085 | 0.2888 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.2737 | 0.0002 | 0.0149 | 0.3180 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.2743 | 0.0003 | 0.0197 | 0.4517 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 0.6132 | 0.0006 | 0.1236 | 4.6827 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.4026 | 0.0000 | 0.0210 | 0.4385 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.3231 | 0.0003 | 0.0262 | 0.4157 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.3441 | 0.0002 | 0.0185 | 0.5578 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 0.3249 | 0.0004 | 0.0805 | 6.1495 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.3485 | 0.0007 | 0.0093 | 0.3861 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.3444 | 0.0003 | 0.0183 | 0.3984 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.4965 | 0.0007 | 0.0160 | 0.7317 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 0.3172 | 0.0004 | 0.0318 | 1.3693 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.3035 | 0.0006 | 0.0119 | 0.3342 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.4480 | 0.0005 | 0.0168 | 0.5092 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.2795 | 0.0006 | 0.0170 | 0.3977 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.2957 | 0.0003 | 0.0260 | 1.2407 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.4025 | 0.0003 | 0.0089 | 0.4253 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.3849 | 0.0001 | 0.0631 | 0.5043 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.4269 | 0.0004 | 0.0186 | 0.7968 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.1828 | 0.0275 | 0.0057 | 0.4333 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.1383 | 0.0147 | 0.0525 | 0.4957 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.1169 | 0.0152 | 0.0254 | 0.6073 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.1294 | 0.0179 | 0.0090 | 1.0160 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.1352 | 0.0245 | 0.0191 | 0.3753 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.1293 | 0.0179 | 0.0316 | 0.4448 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.1531 | 0.0199 | 0.0100 | 0.7826 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 0.1469 | 0.0207 | 0.0272 | 1.0937 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.1534 | 0.0206 | 0.0175 | 0.4510 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.1639 | 0.0213 | 0.0125 | 0.6009 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.1335 | 0.0167 | 0.0178 | 0.5515 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 0.1533 | 0.0163 | 0.1498 | 5.7263 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.1297 | 0.0142 | 0.0317 | 0.3140 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.1402 | 0.0066 | 0.0054 | 0.3585 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.1086 | 0.0140 | 0.0763 | 0.5349 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 0.1142 | 0.0152 | 0.0427 | 0.7893 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.1177 | 0.0148 | 0.0051 | 0.2780 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.1147 | 0.0141 | 0.0185 | 0.3869 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.1511 | 0.0166 | 0.0471 | 0.5301 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.1226 | 0.0148 | 0.0073 | 0.7511 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.1216 | 0.0153 | 0.0341 | 0.3137 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.1295 | 0.0181 | 0.0409 | 0.4558 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.1522 | 0.0160 | 0.0436 | 0.6640 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0283 | 0.0000 | 0.0010 | 0.0297 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0192 | 0.0000 | 0.0014 | 0.0268 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0152 | 0.0000 | 0.0010 | 0.0531 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.0148 | 0.0000 | 0.0015 | 0.4109 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0418 | 0.0000 | 0.0032 | 0.0468 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0361 | 0.0000 | 0.0021 | 0.0496 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0166 | 0.0000 | 0.0012 | 0.0641 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.0150 | 0.0000 | 0.0041 | 0.6116 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0183 | 0.0000 | 0.0011 | 0.0200 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0166 | 0.0000 | 0.0012 | 0.0227 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0176 | 0.0000 | 0.0014 | 0.0741 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 0.0149 | 0.0000 | 0.0025 | 2.9329 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0207 | 0.0000 | 0.0017 | 0.0260 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0102 | 0.0000 | 0.0009 | 0.0153 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0145 | 0.0000 | 0.0011 | 0.0596 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.0168 | 0.0000 | 0.0044 | 0.5873 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0213 | 0.0000 | 0.0013 | 0.0234 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0183 | 0.0000 | 0.0010 | 0.0230 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0127 | 0.0000 | 0.0010 | 0.0446 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.0137 | 0.0000 | 0.0010 | 0.3483 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0156 | 0.0000 | 0.0008 | 0.0168 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0239 | 0.0000 | 0.0014 | 0.0340 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.0133 | 0.0000 | 0.0011 | 0.0959 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 0.0098 | 0.0000 | 0.0050 | 16.2672 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0175 | 0.0000 | 0.0008 | 0.0331 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0111 | 0.0000 | 0.0009 | 0.0774 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0110 | 0.0000 | 0.0007 | 0.6457 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.0119 | 0.0000 | 0.0010 | 6.5770 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0101 | 0.0000 | 0.0007 | 0.0169 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.0096 | 0.0000 | 0.0007 | 0.0711 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 0.0097 | 0.0000 | 0.0011 | 0.6866 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 0.0128 | 0.0000 | 0.0033 | 6.8857 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0142 | 0.0000 | 0.0007 | 0.0210 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.0129 | 0.0000 | 0.0012 | 0.0969 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0133 | 0.0000 | 0.0008 | 0.7392 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.0126 | 0.0000 | 0.0009 | 6.8697 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0102 | 0.0000 | 0.0007 | 0.0173 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.0111 | 0.0000 | 0.0006 | 0.0742 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 0.0096 | 0.0000 | 0.0008 | 0.6378 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 0.0097 | 0.0000 | 0.0033 | 6.6927 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0328 | 0.0000 | 0.0008 | 0.0410 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.0155 | 0.0000 | 0.0010 | 0.0912 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 0.0143 | 0.0000 | 0.0011 | 0.7309 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 0.0108 | 0.0000 | 0.0010 | 6.8509 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0099 | 0.0000 | 0.0007 | 0.0170 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.0113 | 0.0000 | 0.0007 | 0.0885 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 0.0100 | 0.0000 | 0.0012 | 1.8606 |
