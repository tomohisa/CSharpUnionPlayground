| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0005 | 470.1 | 3.4 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0044 | 438.2 | 4.0 | 100 | 0.97 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0417 | 416.6 | 9.6 | 1000 | 0.98 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.3701 | 370.1 | 11.3 | 10000 | 0.95 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0005 | 506.8 | 3.4 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0053 | 525.1 | 4.5 | 1 | 1.02 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0574 | 574.5 | 14.8 | 1 | 1.04 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.9229 | 922.9 | 64.9 | 1 | 1.21 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0005 | 516.5 | 3.4 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0065 | 652.8 | 5.3 | 100 | 1.10 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2982 | 2981.7 | 9.9 | 1000 | 1.66 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0005 | 508.8 | 3.4 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0053 | 527.5 | 4.6 | 1 | 1.02 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0723 | 723.1 | 14.5 | 1 | 1.14 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.7048 | 704.8 | 63.5 | 1 | 0.99 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0004 | 369.7 | 3.5 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0038 | 376.4 | 4.0 | 100 | 1.01 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0337 | 337.2 | 9.6 | 1000 | 0.95 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.3423 | 342.3 | 11.3 | 10000 | 1.01 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0004 | 411.5 | 3.4 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0078 | 779.5 | 4.4 | 1 | 1.28 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.3631 | 3630.8 | 12.2 | 1 | 1.67 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0022 | 2224.4 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0225 | 2254.8 | 0.7 | 100 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.2084 | 2083.7 | 0.8 | 1000 | 0.97 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 2.2893 | 2289.3 | 2.7 | 10000 | 1.04 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0024 | 2447.8 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0222 | 2220.6 | 1.1 | 1 | 0.96 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.2316 | 2316.3 | 7.3 | 1 | 1.02 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 2.3309 | 2330.9 | 54.3 | 1 | 1.00 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0023 | 2336.2 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0217 | 2165.6 | 0.8 | 100 | 0.97 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.2323 | 2323.4 | 1.1 | 1000 | 1.03 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 3.1173 | 3117.3 | 6.3 | 10000 | 1.13 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0019 | 1874.8 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0234 | 2340.2 | 1.2 | 1 | 1.10 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.2397 | 2396.7 | 7.3 | 1 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 2.3494 | 2349.4 | 54.3 | 1 | 0.99 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0024 | 2389.2 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0227 | 2273.7 | 0.7 | 100 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.2269 | 2268.9 | 0.9 | 1000 | 1.00 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 2.3924 | 2392.4 | 2.6 | 10000 | 1.02 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0024 | 2412.3 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0237 | 2365.6 | 1.0 | 1 | 0.99 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.2911 | 2911.1 | 4.7 | 1 | 1.09 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 17.8951 | 17895.1 | 43.1 | 1 | 1.79 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.0106 | 10629.4 | 69.1 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.0139 | 1386.5 | 69.4 | 100 | 0.12 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.0920 | 919.6 | 82.5 | 1000 | 0.82 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.5078 | 507.8 | 97.8 | 10000 | 0.74 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.0080 | 7976.5 | 69.7 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.0189 | 1886.4 | 70.3 | 1 | 0.37 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.1097 | 1097.5 | 94.0 | 1 | 0.76 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 1.5058 | 1505.8 | 223.8 | 1 | 1.14 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.0091 | 9118.7 | 68.7 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.0200 | 2001.0 | 69.5 | 100 | 0.34 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1393 | 1393.5 | 82.4 | 1000 | 0.84 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 4.2622 | 4262.2 | 204.8 | 10000 | 1.49 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.0075 | 7500.0 | 68.8 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.0202 | 2015.6 | 70.2 | 1 | 0.43 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.0891 | 891.5 | 90.5 | 1 | 0.65 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 0.6432 | 643.2 | 189.5 | 1 | 0.86 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.0068 | 6807.2 | 67.9 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.0171 | 1706.0 | 68.6 | 100 | 0.40 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.0703 | 702.7 | 83.5 | 1000 | 0.61 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.5961 | 596.1 | 96.4 | 10000 | 0.93 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.0080 | 7960.7 | 69.6 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.0359 | 3586.3 | 71.7 | 1 | 0.65 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.3796 | 3796.3 | 93.0 | 1 | 1.02 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.0104 | 10448.0 | 75.6 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.0232 | 2322.1 | 81.4 | 100 | 0.35 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.0830 | 829.6 | 92.8 | 1000 | 0.55 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.5861 | 586.1 | 100.6 | 10000 | 0.85 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.0106 | 10616.4 | 76.4 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.1186 | 11859.3 | 81.0 | 1 | 1.05 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.1533 | 1533.1 | 101.6 | 1 | 0.11 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 0.8732 | 873.2 | 266.0 | 1 | 0.76 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.0186 | 18625.0 | 76.8 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.0404 | 4040.0 | 80.5 | 100 | 0.34 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.2182 | 2181.8 | 91.9 | 1000 | 0.73 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 6.1680 | 6168.0 | 210.6 | 10000 | 1.45 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.0147 | 14716.0 | 76.1 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.0397 | 3967.8 | 80.9 | 1 | 0.43 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.2154 | 2154.3 | 94.9 | 1 | 0.73 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 1.0587 | 1058.7 | 222.8 | 1 | 0.69 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.0162 | 16243.3 | 75.6 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.0398 | 3976.5 | 80.7 | 100 | 0.39 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.1875 | 1875.0 | 92.8 | 1000 | 0.67 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.8590 | 859.0 | 101.7 | 10000 | 0.66 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.0168 | 16839.7 | 75.9 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.0506 | 5055.7 | 80.9 | 1 | 0.48 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.3040 | 3039.8 | 100.9 | 1 | 0.78 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.1843 | 184315.3 | 40.0 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.3111 | 31105.8 | 52.6 | 100 | 0.23 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.4032 | 4031.8 | 55.5 | 1000 | 0.11 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.9392 | 939.2 | 90.9 | 10000 | 0.37 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.1772 | 177232.1 | 39.6 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.3066 | 30661.2 | 53.0 | 1 | 0.24 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.4689 | 4688.6 | 61.9 | 1 | 0.18 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 1.0064 | 1006.4 | 163.0 | 1 | 0.33 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.1630 | 162951.0 | 39.5 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.3187 | 31868.9 | 51.2 | 100 | 0.29 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.6521 | 6520.5 | 85.2 | 1000 | 0.31 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 3.3444 | 3344.4 | 1074.2 | 10000 | 0.71 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.2019 | 201855.0 | 38.8 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.5506 | 55057.9 | 52.1 | 1 | 0.44 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.5098 | 5098.4 | 60.4 | 1 | -0.03 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 0.8583 | 858.3 | 165.2 | 1 | 0.23 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.1507 | 150684.7 | 39.3 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.2785 | 27845.7 | 52.9 | 100 | 0.27 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.3812 | 3812.4 | 56.0 | 1000 | 0.14 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.7171 | 717.1 | 89.8 | 10000 | 0.27 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.1680 | 168017.9 | 39.2 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.2897 | 28969.6 | 53.3 | 1 | 0.24 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.6205 | 6205.2 | 60.9 | 1 | 0.33 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0010 | 952.6 | 2.3 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0042 | 418.9 | 2.6 | 100 | 0.64 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0418 | 417.9 | 6.4 | 1000 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.3827 | 382.7 | 7.6 | 10000 | 0.96 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0007 | 739.4 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0050 | 495.5 | 3.0 | 1 | 0.83 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0463 | 462.7 | 7.9 | 1 | 0.97 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.4291 | 429.1 | 49.0 | 1 | 0.97 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0004 | 403.7 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0038 | 379.8 | 3.0 | 100 | 0.97 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0596 | 596.4 | 8.1 | 1000 | 1.20 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 2.7966 | 2796.6 | 26.7 | 10000 | 1.67 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0006 | 615.6 | 2.2 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0044 | 443.5 | 3.0 | 1 | 0.86 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0430 | 430.3 | 7.9 | 1 | 0.99 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.4995 | 499.5 | 51.6 | 1 | 1.06 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0006 | 582.3 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0192 | 1915.9 | 2.6 | 100 | 1.52 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0447 | 446.8 | 6.3 | 1000 | 0.37 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.5058 | 505.8 | 7.5 | 10000 | 1.05 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0006 | 626.2 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0059 | 592.7 | 2.9 | 1 | 0.98 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.1459 | 1459.4 | 7.4 | 1 | 1.39 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 19.8105 | 19810.5 | 44.5 | 1 | 2.13 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0080 | 7970.4 | 0.9 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0675 | 6749.4 | 0.9 | 100 | 0.93 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.6753 | 6753.4 | 1.0 | 1000 | 1.00 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 8.1281 | 8128.1 | 2.6 | 10000 | 1.08 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0077 | 7734.1 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.0897 | 8972.1 | 1.1 | 1 | 1.06 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 0.8284 | 8283.8 | 3.9 | 1 | 0.97 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 8.5143 | 8514.3 | 29.9 | 1 | 1.01 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0080 | 8019.3 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.0790 | 7900.1 | 0.9 | 100 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.8016 | 8015.7 | 1.3 | 1000 | 1.01 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 9.0518 | 9051.8 | 3.4 | 10000 | 1.05 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0101 | 10095.6 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.0954 | 9537.6 | 1.2 | 1 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 0.9076 | 9075.7 | 3.9 | 1 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 8.0679 | 8067.9 | 29.9 | 1 | 0.95 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0078 | 7812.4 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.1132 | 11324.4 | 0.9 | 100 | 1.16 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 0.6776 | 6775.5 | 1.0 | 1000 | 0.78 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 8.1356 | 8135.6 | 2.5 | 10000 | 1.08 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0078 | 7778.1 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.0883 | 8834.6 | 1.1 | 1 | 1.06 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 1.8607 | 18607.2 | 3.5 | 1 | 1.32 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0321 | 0.0020 | 0.0014 | 0.0362 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0207 | 0.0014 | 0.0016 | 0.0283 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0227 | 0.0014 | 0.0023 | 0.0683 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0751 | 0.0006 | 0.0026 | 0.4043 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0239 | 0.0013 | 0.0015 | 0.0274 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0246 | 0.0015 | 0.0018 | 0.0336 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0251 | 0.0015 | 0.0034 | 0.1277 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.0834 | 0.0009 | 0.0128 | 1.0007 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0191 | 0.0014 | 0.0017 | 0.0228 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0189 | 0.0013 | 0.0015 | 0.0283 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0223 | 0.0014 | 0.0028 | 0.3247 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0244 | 0.0015 | 0.0016 | 0.0282 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0229 | 0.0015 | 0.0021 | 0.0322 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0237 | 0.0016 | 0.0030 | 0.1028 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.0254 | 0.0011 | 0.0098 | 0.7987 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0203 | 0.0014 | 0.0013 | 0.0234 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0165 | 0.0012 | 0.0017 | 0.0233 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0158 | 0.0013 | 0.0018 | 0.0527 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.0186 | 0.0012 | 0.0022 | 0.3645 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0171 | 0.0011 | 0.0015 | 0.0201 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0183 | 0.0012 | 0.0015 | 0.0291 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.0157 | 0.0009 | 0.0023 | 0.3837 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0137 | 0.0000 | 0.0010 | 0.0169 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0145 | 0.0000 | 0.0011 | 0.0381 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0132 | 0.0000 | 0.0008 | 0.2225 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0138 | 0.0000 | 0.0014 | 2.3045 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0175 | 0.0000 | 0.0010 | 0.0210 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0175 | 0.0000 | 0.0014 | 0.0413 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0171 | 0.0000 | 0.0054 | 0.2553 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0161 | 0.0000 | 0.0571 | 2.4161 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0147 | 0.0000 | 0.0010 | 0.0180 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0155 | 0.0000 | 0.0010 | 0.0381 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0168 | 0.0000 | 0.0010 | 0.2501 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0157 | 0.0000 | 0.0018 | 3.1349 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0145 | 0.0000 | 0.0008 | 0.0171 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0158 | 0.0000 | 0.0012 | 0.0405 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0173 | 0.0000 | 0.0050 | 0.2629 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0144 | 0.0000 | 0.0643 | 2.4388 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0181 | 0.0000 | 0.0011 | 0.0216 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0171 | 0.0000 | 0.0010 | 0.0408 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0165 | 0.0000 | 0.0010 | 0.2445 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0170 | 0.0000 | 0.0014 | 2.4109 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0177 | 0.0000 | 0.0009 | 0.0212 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0159 | 0.0000 | 0.0014 | 0.0411 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0174 | 0.0000 | 0.0040 | 0.3128 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0178 | 0.0000 | 0.0486 | 17.9697 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.3552 | 0.0005 | 0.0157 | 0.3827 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.1748 | 0.0003 | 0.0127 | 0.2022 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.2047 | 0.0004 | 0.0131 | 0.3111 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.2410 | 0.0003 | 0.0126 | 0.7641 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.1770 | 0.0004 | 0.0095 | 0.1956 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.1749 | 0.0004 | 0.0112 | 0.2085 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.2242 | 0.0032 | 0.0247 | 0.3896 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 0.3020 | 0.0005 | 0.0854 | 2.2441 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.2063 | 0.0005 | 0.0099 | 0.2264 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.1758 | 0.0005 | 0.0099 | 0.2069 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1891 | 0.0006 | 0.0126 | 0.3424 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 0.1886 | 0.0003 | 0.0226 | 4.4762 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.1895 | 0.0003 | 0.0089 | 0.2069 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.1905 | 0.0004 | 0.0081 | 0.2221 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.1653 | 0.0004 | 0.0152 | 0.2819 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 0.1785 | 0.0003 | 0.0260 | 0.8874 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.1663 | 0.0003 | 0.0075 | 0.1814 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.1368 | 0.0005 | 0.0085 | 0.1639 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.1498 | 0.0003 | 0.0117 | 0.2329 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.1949 | 0.0002 | 0.0134 | 0.8085 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.1856 | 0.0005 | 0.0088 | 0.2035 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.1734 | 0.0005 | 0.0074 | 0.2194 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.2930 | 0.0004 | 0.0156 | 0.7149 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.2428 | 0.0004 | 0.0082 | 0.2622 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.2169 | 0.0003 | 0.0118 | 0.2527 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.2217 | 0.0003 | 0.0138 | 0.3194 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.2158 | 0.0003 | 0.0095 | 0.8138 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.2170 | 0.0003 | 0.0076 | 0.2360 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.3611 | 0.0007 | 0.0257 | 0.5108 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.5551 | 0.0004 | 0.0210 | 0.7459 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 0.2388 | 0.0003 | 0.0342 | 1.2690 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.4490 | 0.0006 | 0.0139 | 0.4827 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.3977 | 0.0006 | 0.0489 | 0.4883 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.2932 | 0.0006 | 0.0257 | 0.5386 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 0.3619 | 0.0006 | 0.0327 | 6.5664 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.2994 | 0.0004 | 0.0106 | 0.3258 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.3469 | 0.0005 | 0.0163 | 0.4067 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.3510 | 0.0005 | 0.0456 | 0.6376 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 0.2882 | 0.0004 | 0.0351 | 1.4453 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.3779 | 0.0004 | 0.0105 | 0.4059 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.3578 | 0.0005 | 0.0183 | 0.4172 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.3341 | 0.0005 | 0.0191 | 0.5423 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.2945 | 0.0005 | 0.0106 | 1.1677 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.2784 | 0.0006 | 0.0111 | 0.3081 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.3060 | 0.0004 | 0.0162 | 0.3770 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.2648 | 0.0004 | 0.0168 | 0.5948 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.2353 | 0.0307 | 0.0075 | 0.5401 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.2475 | 0.0264 | 0.0286 | 0.6720 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.2354 | 0.0246 | 0.0639 | 0.7826 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.2645 | 0.0270 | 0.0310 | 1.3328 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.1798 | 0.0279 | 0.0475 | 0.4946 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.2210 | 0.0264 | 0.0511 | 0.6615 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.2337 | 0.0270 | 0.0202 | 0.8289 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 0.1970 | 0.0291 | 0.0655 | 1.4066 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.1730 | 0.0264 | 0.0812 | 0.4991 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.1898 | 0.0257 | 0.0192 | 0.6765 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.1765 | 0.0268 | 0.0522 | 0.9714 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 0.2046 | 0.0267 | 0.0905 | 3.7404 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.1956 | 0.0282 | 0.0315 | 0.5301 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.1886 | 0.0299 | 0.0417 | 0.8786 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.1670 | 0.0231 | 0.0159 | 0.7903 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 0.1626 | 0.0256 | 0.0200 | 1.2107 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.1575 | 0.0189 | 0.0326 | 0.4157 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.1521 | 0.0215 | 0.0534 | 0.5589 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.1523 | 0.0233 | 0.0548 | 0.6627 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.1498 | 0.0212 | 0.0501 | 0.9979 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.1660 | 0.0233 | 0.0417 | 0.4617 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.1700 | 0.0250 | 0.0488 | 0.5854 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.1929 | 0.0292 | 0.0312 | 0.9584 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0235 | 0.0002 | 0.0016 | 0.0274 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0144 | 0.0000 | 0.0012 | 0.0199 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0149 | 0.0000 | 0.0015 | 0.0585 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.0143 | 0.0000 | 0.0012 | 0.3983 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0146 | 0.0000 | 0.0012 | 0.0169 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0145 | 0.0000 | 0.0012 | 0.0212 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0141 | 0.0000 | 0.0014 | 0.0643 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.0126 | 0.0000 | 0.0034 | 0.4909 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0116 | 0.0000 | 0.0010 | 0.0132 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0124 | 0.0000 | 0.0011 | 0.0175 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0137 | 0.0000 | 0.0015 | 0.0751 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 0.0158 | 0.0000 | 0.0025 | 2.8156 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0135 | 0.0000 | 0.0012 | 0.0154 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0126 | 0.0000 | 0.0009 | 0.0184 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0116 | 0.0000 | 0.0015 | 0.0587 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.0133 | 0.0000 | 0.0067 | 0.5893 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0176 | 0.0000 | 0.0012 | 0.0195 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0424 | 0.0000 | 0.0042 | 0.0664 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0139 | 0.0000 | 0.0026 | 0.0613 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.0171 | 0.0000 | 0.0016 | 0.5248 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0159 | 0.0000 | 0.0014 | 0.0181 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0167 | 0.0000 | 0.0013 | 0.0245 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.0235 | 0.0000 | 0.0022 | 0.1758 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 0.0151 | 0.0000 | 0.0034 | 19.8596 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0140 | 0.0000 | 0.0011 | 0.0231 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0118 | 0.0000 | 0.0008 | 0.0802 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0119 | 0.0000 | 0.0008 | 0.6882 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.0105 | 0.0000 | 0.0012 | 8.1402 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0132 | 0.0000 | 0.0011 | 0.0221 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.0126 | 0.0000 | 0.0010 | 0.1037 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 0.0126 | 0.0000 | 0.0010 | 0.8448 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 0.0133 | 0.0000 | 0.0026 | 8.5787 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0126 | 0.0000 | 0.0013 | 0.0220 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.0129 | 0.0000 | 0.0009 | 0.0929 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0125 | 0.0000 | 0.0012 | 0.8155 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.0126 | 0.0000 | 0.0012 | 9.0659 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0154 | 0.0000 | 0.0017 | 0.0275 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.0154 | 0.0000 | 0.0009 | 0.1121 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 0.0124 | 0.0000 | 0.0011 | 0.9239 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 0.0113 | 0.0000 | 0.0393 | 8.3061 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0140 | 0.0000 | 0.0008 | 0.0228 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.0116 | 0.0000 | 0.0009 | 0.1259 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 0.0117 | 0.0000 | 0.0010 | 0.6904 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 0.0105 | 0.0000 | 0.0011 | 8.1475 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0143 | 0.0000 | 0.0008 | 0.0230 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.0136 | 0.0000 | 0.0008 | 0.1032 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 0.0121 | 0.0000 | 0.0013 | 1.8765 |
