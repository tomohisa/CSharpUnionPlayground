| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0494 | 49422.4 | 24.0 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0689 | 6894.7 | 24.7 | 100 | 0.14 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.1609 | 1609.3 | 30.6 | 1000 | 0.37 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 1.8687 | 1868.7 | 32.5 | 10000 | 1.06 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0487 | 48669.9 | 24.1 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0589 | 5888.9 | 25.1 | 1 | 0.08 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.1542 | 1542.4 | 35.6 | 1 | 0.42 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 2.2586 | 2258.6 | 86.2 | 1 | 1.17 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0667 | 66706.9 | 24.1 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0689 | 6894.1 | 25.3 | 100 | 0.01 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.1730 | 1730.2 | 30.8 | 1000 | 0.40 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 3.4637 | 3463.7 | 32.4 | 10000 | 1.30 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0592 | 59247.7 | 24.3 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.1166 | 11655.9 | 25.2 | 1 | 0.29 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.2310 | 2309.9 | 35.3 | 1 | 0.30 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 2.0513 | 2051.3 | 84.9 | 1 | 0.95 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0678 | 67836.8 | 24.1 | 10 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0888 | 8880.2 | 24.9 | 100 | 0.12 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.2143 | 2142.8 | 30.6 | 1000 | 0.38 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.8875 | 887.5 | 32.8 | 10000 | 0.62 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0584 | 58420.0 | 24.0 | 1 | — | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0853 | 8533.9 | 25.1 | 1 | 0.16 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.7659 | 7658.8 | 33.7 | 1 | 0.95 | ok |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0019 | 1906.5 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0270 | 2699.1 | 0.7 | 100 | 1.15 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.1993 | 1993.0 | 0.9 | 1000 | 0.87 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 1.8894 | 1889.4 | 2.6 | 10000 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0021 | 2065.1 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0209 | 2087.4 | 1.1 | 1 | 1.00 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.1976 | 1975.9 | 7.3 | 1 | 0.98 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 2.4748 | 2474.8 | 54.3 | 1 | 1.10 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0021 | 2104.7 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0213 | 2126.0 | 0.8 | 100 | 1.00 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.2342 | 2342.1 | 1.6 | 1000 | 1.04 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 2.9746 | 2974.6 | 6.4 | 10000 | 1.10 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0019 | 1892.7 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0161 | 1614.2 | 1.0 | 1 | 0.93 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.1669 | 1668.6 | 7.4 | 1 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 1.9507 | 1950.7 | 54.3 | 1 | 1.07 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0020 | 2010.1 | 0.7 | 10 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0162 | 1621.6 | 0.7 | 100 | 0.91 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.1673 | 1672.6 | 0.8 | 1000 | 1.01 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 1.7472 | 1747.2 | 2.6 | 10000 | 1.02 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0019 | 1860.4 | 0.7 | 1 | — | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0170 | 1704.9 | 1.1 | 1 | 0.96 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.2081 | 2081.4 | 4.6 | 1 | 1.09 | ok |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 14.5683 | 14568.3 | 43.1 | 1 | 1.85 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.0099 | 9929.2 | 68.2 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.0186 | 1860.0 | 69.3 | 100 | 0.27 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.0912 | 912.2 | 80.6 | 1000 | 0.69 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.5888 | 588.8 | 96.4 | 10000 | 0.81 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.0091 | 9074.2 | 68.7 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.0209 | 2094.0 | 70.8 | 1 | 0.36 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.1003 | 1003.2 | 94.1 | 1 | 0.68 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 0.8020 | 802.0 | 224.3 | 1 | 0.90 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.0092 | 9215.2 | 68.5 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.0204 | 2036.5 | 68.7 | 100 | 0.34 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1439 | 1438.5 | 81.5 | 1000 | 0.85 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 5.6108 | 5610.8 | 204.0 | 10000 | 1.59 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.0095 | 9488.3 | 68.8 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.0220 | 2204.2 | 70.9 | 1 | 0.37 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.1069 | 1069.1 | 91.8 | 1 | 0.69 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 0.9301 | 930.1 | 189.6 | 1 | 0.94 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.0094 | 9444.9 | 68.6 | 10 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.0184 | 1836.8 | 69.1 | 100 | 0.29 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.0815 | 814.8 | 82.4 | 1000 | 0.65 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.5937 | 593.7 | 96.2 | 10000 | 0.86 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.0114 | 11351.8 | 68.9 | 1 | — | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.0314 | 3140.2 | 70.8 | 1 | 0.44 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.3235 | 3235.2 | 94.7 | 1 | 1.01 | ok |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.0136 | 13603.8 | 76.1 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.0301 | 3010.8 | 80.7 | 100 | 0.35 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.1256 | 1255.6 | 91.4 | 1000 | 0.62 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.8895 | 889.5 | 99.0 | 10000 | 0.85 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.0156 | 15609.2 | 75.9 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.0389 | 3893.8 | 82.5 | 1 | 0.40 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.1477 | 1476.9 | 102.8 | 1 | 0.58 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 0.8429 | 842.9 | 263.4 | 1 | 0.76 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.0185 | 18505.0 | 76.3 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.0382 | 3823.2 | 81.4 | 100 | 0.32 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.1890 | 1889.6 | 92.1 | 1000 | 0.69 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 4.6114 | 4611.4 | 212.1 | 10000 | 1.39 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.0152 | 15234.2 | 75.5 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.0286 | 2860.8 | 80.7 | 1 | 0.27 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.1620 | 1619.8 | 99.1 | 1 | 0.75 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 0.9844 | 984.4 | 223.7 | 1 | 0.78 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.0271 | 27135.8 | 75.8 | 10 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.0278 | 2776.7 | 81.1 | 100 | 0.01 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.0969 | 969.5 | 92.5 | 1000 | 0.54 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.6755 | 675.5 | 101.9 | 10000 | 0.84 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.0114 | 11441.7 | 76.5 | 1 | — | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.0328 | 3279.5 | 82.3 | 1 | 0.46 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.2255 | 2254.8 | 103.7 | 1 | 0.84 | ok |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.1352 | 135158.7 | 39.4 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.2421 | 24211.5 | 51.1 | 100 | 0.25 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.4448 | 4448.0 | 55.6 | 1000 | 0.26 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.6830 | 683.0 | 89.1 | 10000 | 0.19 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.1468 | 146815.3 | 39.4 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.4226 | 42258.6 | 50.8 | 1 | 0.46 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.3797 | 3796.8 | 60.8 | 1 | -0.05 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 0.7027 | 702.7 | 144.7 | 1 | 0.27 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.1231 | 123136.8 | 39.9 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.3055 | 30554.1 | 50.5 | 100 | 0.39 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 1.0107 | 10106.7 | 82.1 | 1000 | 0.52 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 8.6628 | 8662.8 | 1328.0 | 10000 | 0.93 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.1164 | 116407.0 | 39.6 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.3299 | 32986.4 | 52.0 | 1 | 0.45 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.5098 | 5098.1 | 61.2 | 1 | 0.19 | ok |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 1.2974 | 1297.4 | 152.6 | 1 | 0.41 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.1300 | 129977.9 | 38.6 | 10 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.2642 | 26423.1 | 51.6 | 100 | 0.31 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.3491 | 3490.6 | 56.1 | 1000 | 0.12 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.7200 | 720.0 | 89.9 | 10000 | 0.31 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.1271 | 127063.0 | 38.8 | 1 | — | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.2603 | 26034.8 | 51.1 | 1 | 0.31 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.4147 | 4146.9 | 61.0 | 1 | 0.20 | ok |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0006 | 596.0 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0051 | 514.5 | 2.5 | 100 | 0.94 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0514 | 513.7 | 6.4 | 1000 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.5284 | 528.4 | 7.6 | 10000 | 1.01 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0006 | 607.0 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0055 | 546.7 | 3.0 | 1 | 0.95 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0549 | 548.9 | 7.8 | 1 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.5231 | 523.1 | 53.9 | 1 | 0.98 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0005 | 539.1 | 2.3 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0051 | 513.2 | 3.1 | 100 | 0.98 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0639 | 639.0 | 8.3 | 1000 | 1.10 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 2.8393 | 2839.3 | 27.3 | 10000 | 1.65 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0006 | 567.5 | 2.3 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0053 | 527.0 | 3.0 | 1 | 0.97 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0528 | 528.3 | 7.8 | 1 | 1.00 | ok |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.5363 | 536.3 | 49.1 | 1 | 1.01 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0005 | 520.2 | 2.2 | 10 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0045 | 453.6 | 2.5 | 100 | 0.94 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0464 | 464.4 | 6.4 | 1000 | 1.01 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.4064 | 406.4 | 7.5 | 10000 | 0.94 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0006 | 601.0 | 2.2 | 1 | — | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0056 | 557.0 | 2.9 | 1 | 0.97 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.1164 | 1163.7 | 7.1 | 1 | 1.32 | ok |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 23.1253 | 23125.3 | 45.4 | 1 | 2.30 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0103 | 10322.9 | 0.9 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0814 | 8138.4 | 0.9 | 100 | 0.90 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.9163 | 9162.6 | 1.1 | 1000 | 1.05 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 10.4554 | 10455.4 | 2.6 | 10000 | 1.06 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0114 | 11435.7 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.1232 | 12316.4 | 1.2 | 1 | 1.03 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 1.0074 | 10074.3 | 4.1 | 1 | 0.91 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 10.9434 | 10943.4 | 30.0 | 1 | 1.04 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0124 | 12403.2 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.1023 | 10229.6 | 0.9 | 100 | 0.92 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 1.0045 | 10044.7 | 1.6 | 1000 | 0.99 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 11.9089 | 11908.9 | 3.4 | 10000 | 1.07 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0108 | 10774.7 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.1474 | 14739.6 | 1.2 | 1 | 1.14 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 1.4113 | 14112.6 | 4.0 | 1 | 0.98 | ok |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 12.6480 | 12648.0 | 29.9 | 1 | 0.95 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0096 | 9592.0 | 0.8 | 10 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.1010 | 10103.0 | 0.9 | 100 | 1.02 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 1.2906 | 12905.6 | 1.1 | 1000 | 1.11 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 10.6070 | 10607.0 | 2.5 | 10000 | 0.91 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0108 | 10840.7 | 0.9 | 1 | — | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.1199 | 11990.7 | 1.1 | 1 | 1.04 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 2.3283 | 23282.7 | 3.6 | 1 | 1.29 | ok |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.1227 | 0.0062 | 0.0051 | 0.1889 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.1062 | 0.0061 | 0.0066 | 0.1982 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.2023 | 0.0087 | 0.0064 | 0.3867 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.1372 | 0.0072 | 0.0098 | 2.0311 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0973 | 0.0063 | 0.0057 | 0.1627 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0725 | 0.0054 | 0.0061 | 0.1484 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.1179 | 0.0063 | 0.0266 | 0.3557 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.1141 | 0.0067 | 0.0187 | 2.4722 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.1225 | 0.0049 | 0.0068 | 0.2121 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.1204 | 0.0072 | 0.0062 | 0.2124 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.1493 | 0.0082 | 0.0081 | 0.3537 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.2234 | 0.0209 | 0.0199 | 3.7357 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.1214 | 0.0081 | 0.0065 | 0.2008 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.1215 | 0.0066 | 0.0194 | 0.2753 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.1767 | 0.0195 | 0.0072 | 0.4434 |
| [csharp-native](code/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.1288 | 0.0075 | 0.0246 | 2.3043 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.1430 | 0.0074 | 0.0078 | 0.2313 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.1109 | 0.0075 | 0.0318 | 0.2475 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0984 | 0.0165 | 0.0096 | 0.3437 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.1653 | 0.0065 | 0.0069 | 1.0731 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.1197 | 0.0065 | 0.0060 | 0.1966 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.1520 | 0.0062 | 0.0061 | 0.2571 |
| [csharp-native](code/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.1679 | 0.0062 | 0.0074 | 0.9558 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0186 | 0.0000 | 0.0009 | 0.0215 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0218 | 0.0000 | 0.0012 | 0.0500 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0160 | 0.0000 | 0.0008 | 0.2161 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0118 | 0.0000 | 0.0012 | 1.9025 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0138 | 0.0000 | 0.0010 | 0.0170 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0159 | 0.0000 | 0.0012 | 0.0380 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0152 | 0.0000 | 0.0037 | 0.2172 |
| [rust-native](code/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0289 | 0.0000 | 0.0606 | 2.5763 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0169 | 0.0000 | 0.0008 | 0.0199 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0169 | 0.0000 | 0.0010 | 0.0393 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0167 | 0.0000 | 0.0010 | 0.2519 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0165 | 0.0000 | 0.0013 | 2.9924 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0120 | 0.0000 | 0.0009 | 0.0148 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0098 | 0.0000 | 0.0009 | 0.0269 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0114 | 0.0000 | 0.0038 | 0.1827 |
| [rust-native](code/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0098 | 0.0000 | 0.0418 | 2.0103 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0149 | 0.0000 | 0.0009 | 0.0178 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0139 | 0.0000 | 0.0008 | 0.0309 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0139 | 0.0000 | 0.0008 | 0.1821 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0148 | 0.0000 | 0.0010 | 1.7632 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0151 | 0.0000 | 0.0008 | 0.0178 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0143 | 0.0000 | 0.0010 | 0.0324 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0131 | 0.0000 | 0.0028 | 0.2244 |
| [rust-native](code/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0124 | 0.0000 | 0.0364 | 14.6238 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000 | 0.2488 | 0.0005 | 0.0135 | 0.2735 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 10000 | 0.2085 | 0.0005 | 0.0149 | 0.2432 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 100000 | 0.2082 | 0.0005 | 0.0140 | 0.3149 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | dense | 1000000 | 0.2060 | 0.0005 | 0.0147 | 0.8132 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000 | 0.1975 | 0.0005 | 0.0110 | 0.2188 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 10000 | 0.2154 | 0.0005 | 0.0115 | 0.2515 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 100000 | 0.1979 | 0.0005 | 0.0180 | 0.3349 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-append | wide | 1000000 | 0.1916 | 0.0004 | 0.0832 | 1.1634 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000 | 0.2070 | 0.0003 | 0.0100 | 0.2272 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 10000 | 0.1766 | 0.0005 | 0.0116 | 0.2098 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 100000 | 0.1601 | 0.0004 | 0.0147 | 0.3201 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | dense | 1000000 | 0.1829 | 0.0005 | 0.0230 | 5.8210 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000 | 0.1735 | 0.0004 | 0.0093 | 0.1936 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 10000 | 0.2332 | 0.0005 | 0.0100 | 0.2692 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 100000 | 0.1949 | 0.0005 | 0.0164 | 0.3351 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | indexed-copy | wide | 1000000 | 0.1849 | 0.0005 | 0.0319 | 1.2122 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000 | 0.1764 | 0.0005 | 0.0097 | 0.1968 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 10000 | 0.1638 | 0.0005 | 0.0100 | 0.1933 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 100000 | 0.1702 | 0.0005 | 0.0151 | 0.2681 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | dense | 1000000 | 0.1856 | 0.0005 | 0.0120 | 0.7955 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 1000 | 0.2103 | 0.0005 | 0.0102 | 0.2335 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 10000 | 0.2154 | 0.0005 | 0.0091 | 0.2588 |
| [ts-simple (native)](code/projection/typescript/projection.ts) | linear-append | wide | 100000 | 0.2595 | 0.0005 | 0.0145 | 0.6198 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000 | 0.2560 | 0.0004 | 0.0313 | 0.3018 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 10000 | 0.2731 | 0.0004 | 0.0149 | 0.3192 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 100000 | 0.3763 | 0.0004 | 0.0303 | 0.5336 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | dense | 1000000 | 0.3912 | 0.0009 | 0.0121 | 1.2969 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000 | 0.3763 | 0.0005 | 0.0116 | 0.4048 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 10000 | 0.4208 | 0.0004 | 0.0184 | 0.4809 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 100000 | 0.3059 | 0.0004 | 0.0107 | 0.4713 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-append | wide | 1000000 | 0.2760 | 0.0004 | 0.0291 | 1.2141 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000 | 0.4228 | 0.0005 | 0.0141 | 0.4572 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 10000 | 0.3862 | 0.0007 | 0.0173 | 0.4432 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 100000 | 0.3186 | 0.0006 | 0.0187 | 0.5279 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | dense | 1000000 | 0.3721 | 0.0005 | 0.0263 | 5.0132 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000 | 0.3187 | 0.0004 | 0.0099 | 0.3451 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 10000 | 0.3084 | 0.0005 | 0.0139 | 0.3537 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 100000 | 0.3727 | 0.0004 | 0.0184 | 0.5651 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | indexed-copy | wide | 1000000 | 0.2765 | 0.0004 | 0.0353 | 1.3684 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000 | 0.3297 | 0.0004 | 0.0219 | 0.3797 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 10000 | 0.2634 | 0.0004 | 0.0137 | 0.3059 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 100000 | 0.2673 | 0.0004 | 0.0140 | 0.3793 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | dense | 1000000 | 0.2419 | 0.0004 | 0.0102 | 0.9301 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 1000 | 0.2316 | 0.0003 | 0.0088 | 0.2528 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 10000 | 0.2040 | 0.0003 | 0.0111 | 0.2504 |
| [ts-zod (native)](code/projection/typescript-zod/projection-zod.ts) | linear-append | wide | 100000 | 0.2252 | 0.0003 | 0.0128 | 0.4722 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000 | 0.1956 | 0.0200 | 0.0110 | 0.4372 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 10000 | 0.1536 | 0.0237 | 0.0154 | 0.4761 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 100000 | 0.3182 | 0.0262 | 0.0393 | 0.9228 |
| [java-native](code/projection/java/Projection.java) | indexed-append | dense | 1000000 | 0.1375 | 0.0201 | 0.0216 | 0.9840 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000 | 0.2290 | 0.0209 | 0.0324 | 0.4754 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 10000 | 0.2468 | 0.0605 | 0.0145 | 0.8623 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 100000 | 0.1581 | 0.0193 | 0.0080 | 0.6831 |
| [java-native](code/projection/java/Projection.java) | indexed-append | wide | 1000000 | 0.1865 | 0.0215 | 0.0220 | 1.0437 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000 | 0.1940 | 0.0205 | 0.0423 | 0.4911 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 10000 | 0.1958 | 0.0194 | 0.0521 | 0.7536 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 100000 | 0.2444 | 0.0425 | 0.0192 | 1.4465 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | dense | 1000000 | 0.1432 | 0.0178 | 0.0873 | 8.9737 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000 | 0.1289 | 0.0164 | 0.0115 | 0.3211 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 10000 | 0.1512 | 0.0165 | 0.0453 | 0.5988 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 100000 | 0.1490 | 0.0190 | 0.0318 | 0.7651 |
| [java-native](code/projection/java/Projection.java) | indexed-copy | wide | 1000000 | 0.2116 | 0.0270 | 0.0251 | 1.7307 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000 | 0.1944 | 0.0165 | 0.0067 | 0.3965 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 10000 | 0.1561 | 0.0245 | 0.0105 | 0.5952 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 100000 | 0.1622 | 0.0245 | 0.0348 | 0.6703 |
| [java-native](code/projection/java/Projection.java) | linear-append | dense | 1000000 | 0.1301 | 0.0193 | 0.0398 | 0.9694 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 1000 | 0.1452 | 0.0158 | 0.0407 | 0.3728 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 10000 | 0.1324 | 0.0162 | 0.0573 | 0.5146 |
| [java-native](code/projection/java/Projection.java) | linear-append | wide | 100000 | 0.1376 | 0.0158 | 0.0307 | 0.6581 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000 | 0.0418 | 0.0000 | 0.0009 | 0.0432 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 10000 | 0.0214 | 0.0000 | 0.0016 | 0.0283 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 100000 | 0.0212 | 0.0000 | 0.0017 | 0.0745 |
| [go-native](code/projection/go/projection.go) | indexed-append | dense | 1000000 | 0.0247 | 0.0000 | 0.0024 | 0.5558 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000 | 0.0257 | 0.0000 | 0.0018 | 0.0281 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 10000 | 0.0198 | 0.0000 | 0.0015 | 0.0272 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 100000 | 0.0196 | 0.0000 | 0.0012 | 0.0797 |
| [go-native](code/projection/go/projection.go) | indexed-append | wide | 1000000 | 0.0179 | 0.0000 | 0.0045 | 0.6089 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000 | 0.0183 | 0.0000 | 0.0010 | 0.0201 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 10000 | 0.0194 | 0.0000 | 0.0012 | 0.0259 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 100000 | 0.0191 | 0.0000 | 0.0015 | 0.0848 |
| [go-native](code/projection/go/projection.go) | indexed-copy | dense | 1000000 | 0.0184 | 0.0000 | 0.0028 | 2.8613 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000 | 0.0186 | 0.0000 | 0.0012 | 0.0205 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 10000 | 0.0192 | 0.0000 | 0.0013 | 0.0263 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 100000 | 0.0185 | 0.0000 | 0.0014 | 0.0772 |
| [go-native](code/projection/go/projection.go) | indexed-copy | wide | 1000000 | 0.0177 | 0.0000 | 0.0043 | 0.6176 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000 | 0.0189 | 0.0000 | 0.0011 | 0.0207 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 10000 | 0.0171 | 0.0000 | 0.0010 | 0.0229 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 100000 | 0.0174 | 0.0000 | 0.0014 | 0.0655 |
| [go-native](code/projection/go/projection.go) | linear-append | dense | 1000000 | 0.0159 | 0.0000 | 0.0015 | 0.4240 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000 | 0.0183 | 0.0000 | 0.0012 | 0.0203 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 10000 | 0.0188 | 0.0000 | 0.0012 | 0.0261 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 100000 | 0.0173 | 0.0000 | 0.0014 | 0.1383 |
| [go-native](code/projection/go/projection.go) | linear-append | wide | 1000000 | 0.0194 | 0.0000 | 0.0043 | 23.2265 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000 | 0.0172 | 0.0000 | 0.0009 | 0.0287 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 10000 | 0.0125 | 0.0000 | 0.0008 | 0.0949 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 100000 | 0.0125 | 0.0000 | 0.0009 | 0.9299 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | dense | 1000000 | 0.0203 | 0.0000 | 0.0016 | 10.4777 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000 | 0.0236 | 0.0000 | 0.0008 | 0.0361 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 10000 | 0.0194 | 0.0000 | 0.0009 | 0.1440 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 100000 | 0.0164 | 0.0000 | 0.0012 | 1.0281 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-append | wide | 1000000 | 0.0171 | 0.0000 | 0.0034 | 11.0306 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000 | 0.0193 | 0.0000 | 0.0009 | 0.0328 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 10000 | 0.0183 | 0.0000 | 0.0009 | 0.1218 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 100000 | 0.0158 | 0.0001 | 0.0011 | 1.0217 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | dense | 1000000 | 0.0183 | 0.0000 | 0.0015 | 11.9291 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000 | 0.0202 | 0.0000 | 0.0009 | 0.0317 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 10000 | 0.0413 | 0.0001 | 0.0021 | 0.1898 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 100000 | 0.0252 | 0.0000 | 0.0017 | 1.4431 |
| [swift-native](code/projection/swift/Projection.swift) | indexed-copy | wide | 1000000 | 0.0181 | 0.0000 | 0.0431 | 13.0706 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000 | 0.0340 | 0.0000 | 0.0007 | 0.0400 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 10000 | 0.0192 | 0.0000 | 0.0011 | 0.1212 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 100000 | 0.0218 | 0.0000 | 0.0013 | 1.3139 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | dense | 1000000 | 0.0142 | 0.0000 | 0.0014 | 10.6229 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 1000 | 0.0180 | 0.0000 | 0.0009 | 0.0299 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 10000 | 0.0173 | 0.0000 | 0.0010 | 0.1386 |
| [swift-native](code/projection/swift/Projection.swift) | linear-append | wide | 100000 | 0.0178 | 0.0000 | 0.0037 | 2.3524 |
