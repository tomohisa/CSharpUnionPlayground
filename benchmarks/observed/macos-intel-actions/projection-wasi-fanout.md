| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0003 | 332.1 | 3.4 | 10 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0030 | 295.7 | 4.0 | 100 | 0.95 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0316 | 315.9 | 9.6 | 1000 | 1.03 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.3715 | 371.5 | 11.3 | 10000 | 1.07 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0005 | 511.9 | 3.4 | 1 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0045 | 453.7 | 4.5 | 1 | 0.95 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0574 | 573.6 | 14.8 | 1 | 1.10 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.7817 | 781.7 | 65.0 | 1 | 1.13 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0006 | 592.1 | 3.4 | 10 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0095 | 949.0 | 5.3 | 100 | 1.20 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.3736 | 3736.3 | 9.9 | 1000 | 1.60 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0008 | 789.8 | 3.5 | 1 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0051 | 505.0 | 4.6 | 1 | 0.81 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.1344 | 1344.3 | 14.6 | 1 | 1.43 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 1.7385 | 1738.5 | 63.4 | 1 | 1.11 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0010 | 1013.5 | 3.5 | 10 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0046 | 458.6 | 4.0 | 100 | 0.66 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.1407 | 1407.3 | 9.6 | 1000 | 1.49 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.6430 | 643.0 | 11.3 | 10000 | 0.66 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0005 | 512.3 | 3.4 | 1 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0051 | 514.0 | 4.4 | 1 | 1.00 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.2099 | 2099.5 | 12.2 | 1 | 1.61 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0097 | 9728.3 | 513.9 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0238 | 2376.6 | 139.0 | 100 | 0.39 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.1351 | 1351.0 | 138.3 | 1000 | 0.75 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 1.3740 | 1374.0 | 140.7 | 10000 | 1.01 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0102 | 10180.2 | 137.6 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0244 | 2436.3 | 137.6 | 1 | 0.38 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.1933 | 1932.7 | 143.9 | 1 | 0.90 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 1.8729 | 1872.9 | 193.3 | 1 | 0.99 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0444 | 44361.7 | 141.7 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0489 | 4890.5 | 141.7 | 100 | 0.04 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 2.4898 | 24897.9 | 158.1 | 1000 | 1.71 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0118 | 11760.8 | 137.6 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0274 | 2738.8 | 137.6 | 1 | 0.37 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.1899 | 1899.2 | 143.9 | 1 | 0.84 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 1.8796 | 1879.6 | 192.9 | 1 | 1.00 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0073 | 7343.1 | 137.6 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0174 | 1735.1 | 141.5 | 100 | 0.37 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.1269 | 1268.6 | 142.2 | 1000 | 0.86 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 1.1950 | 1195.0 | 140.7 | 10000 | 0.97 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0062 | 6215.0 | 138.9 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0372 | 3716.0 | 137.4 | 1 | 0.78 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 2.0943 | 20942.7 | 141.1 | 1 | 1.75 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0020 | 1999.0 | 0.7 | 10 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0197 | 1965.0 | 0.7 | 100 | 0.99 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.2185 | 2185.2 | 0.8 | 1000 | 1.05 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 2.6437 | 2643.7 | 2.6 | 10000 | 1.08 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0049 | 4876.2 | 0.7 | 1 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0323 | 3233.7 | 1.0 | 1 | 0.82 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.2729 | 2728.6 | 7.3 | 1 | 0.93 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 2.9229 | 2922.9 | 54.3 | 1 | 1.03 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0029 | 2889.7 | 0.7 | 10 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0215 | 2151.5 | 0.8 | 100 | 0.87 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.2671 | 2671.0 | 1.6 | 1000 | 1.09 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 3.6228 | 3622.8 | 6.2 | 10000 | 1.13 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0020 | 1984.2 | 0.7 | 1 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0328 | 3280.7 | 1.0 | 1 | 1.22 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.2692 | 2691.7 | 7.3 | 1 | 0.91 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 2.4868 | 2486.8 | 54.3 | 1 | 0.97 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0019 | 1938.2 | 0.7 | 10 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0198 | 1979.6 | 0.7 | 100 | 1.01 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.1948 | 1947.9 | 0.8 | 1000 | 0.99 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 2.3133 | 2313.3 | 2.5 | 10000 | 1.07 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0023 | 2282.7 | 0.7 | 1 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0215 | 2148.2 | 1.0 | 1 | 0.97 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.2624 | 2623.7 | 4.6 | 1 | 1.09 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 17.7341 | 17734.1 | 43.1 | 1 | 1.83 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0038 | 3778.7 | 24.3 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0363 | 3632.6 | 9.9 | 100 | 0.98 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.3875 | 3875.0 | 9.9 | 1000 | 1.03 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 3.6724 | 3672.4 | 11.1 | 10000 | 0.98 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0042 | 4159.2 | 9.9 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0347 | 3471.9 | 10.0 | 1 | 0.92 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.3928 | 3928.0 | 13.6 | 1 | 1.05 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 4.0117 | 4011.7 | 40.4 | 1 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0037 | 3695.3 | 9.8 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0539 | 5392.9 | 9.8 | 100 | 1.16 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.4188 | 4188.0 | 10.0 | 1000 | 0.89 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 4.9502 | 4950.2 | 12.1 | 10000 | 1.07 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0045 | 4515.1 | 9.9 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0384 | 3838.6 | 10.1 | 1 | 0.93 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.3942 | 3942.4 | 13.5 | 1 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 3.9508 | 3950.8 | 40.4 | 1 | 1.00 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0040 | 4012.3 | 9.8 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0365 | 3648.1 | 9.8 | 100 | 0.96 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.3716 | 3715.9 | 9.9 | 1000 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 4.0388 | 4038.8 | 11.2 | 10000 | 1.04 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0038 | 3785.3 | 9.8 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0433 | 4334.1 | 10.1 | 1 | 1.06 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.5422 | 5422.1 | 12.2 | 1 | 1.10 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 19.0681 | 19068.1 | 35.2 | 1 | 1.55 | ok |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0302 | 0.0018 | 0.0011 | 0.0336 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0158 | 0.0012 | 0.0015 | 0.0216 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0160 | 0.0010 | 0.0016 | 0.0504 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0223 | 0.0015 | 0.0025 | 0.3982 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0196 | 0.0015 | 0.0016 | 0.0234 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0181 | 0.0014 | 0.0017 | 0.0261 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0293 | 0.0017 | 0.0026 | 0.1062 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.0269 | 0.0021 | 0.0096 | 0.8514 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0263 | 0.0016 | 0.0018 | 0.0304 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0250 | 0.0015 | 0.0020 | 0.0381 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0289 | 0.0032 | 0.0033 | 0.4093 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0453 | 0.0023 | 0.0017 | 0.0504 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0340 | 0.0017 | 0.0016 | 0.0431 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0576 | 0.0019 | 0.0209 | 0.2259 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.0511 | 0.0017 | 0.0759 | 1.9769 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0460 | 0.0021 | 0.0022 | 0.0514 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0276 | 0.0016 | 0.0020 | 0.0361 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0415 | 0.0030 | 0.0047 | 0.1899 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.0272 | 0.0099 | 0.0030 | 0.6825 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0228 | 0.0014 | 0.0016 | 0.0264 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0229 | 0.0012 | 0.0017 | 0.0313 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.0222 | 0.0012 | 0.0036 | 0.2402 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 10.4972 | 0.0156 | 3.0626 | 13.5995 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.4701 | 0.0134 | 2.7159 | 3.2352 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.4435 | 0.0127 | 2.4330 | 3.0323 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.4476 | 0.0112 | 3.0780 | 4.9208 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.5081 | 0.0129 | 2.7578 | 3.3023 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.4750 | 0.0146 | 2.8653 | 3.4002 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.4178 | 0.0134 | 2.8092 | 3.5160 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.6130 | 0.0144 | 4.1275 | 7.5967 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.8786 | 0.0244 | 4.1600 | 5.1191 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.4705 | 0.0109 | 3.0553 | 3.6029 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.4465 | 0.0113 | 2.3926 | 5.3550 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.4954 | 0.0106 | 2.4581 | 2.9861 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.4361 | 0.0116 | 2.6285 | 3.1230 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.3800 | 0.0116 | 2.3329 | 2.9747 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.3757 | 0.0115 | 2.1874 | 4.9345 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.3597 | 0.0099 | 2.3302 | 2.7154 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.4178 | 0.0097 | 2.5332 | 2.9865 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.3654 | 0.0088 | 2.2818 | 2.7920 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.3938 | 0.0094 | 2.7559 | 4.3631 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.4425 | 0.0099 | 2.6845 | 3.1573 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.3497 | 0.0101 | 2.3158 | 2.7291 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.3856 | 0.0107 | 2.3950 | 4.9315 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0118 | 0.0000 | 0.0008 | 0.0146 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0109 | 0.0000 | 0.0009 | 0.0314 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0123 | 0.0000 | 0.0011 | 0.2320 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0227 | 0.0000 | 0.0027 | 2.6692 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0418 | 0.0000 | 0.0024 | 0.0490 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0674 | 0.0000 | 0.0015 | 0.1015 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0172 | 0.0000 | 0.0056 | 0.2974 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0193 | 0.0000 | 0.0685 | 3.0254 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0213 | 0.0000 | 0.0010 | 0.0253 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0117 | 0.0000 | 0.0008 | 0.0341 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0163 | 0.0000 | 0.0016 | 0.2851 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0225 | 0.0001 | 0.0029 | 3.6495 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0200 | 0.0000 | 0.0030 | 0.0251 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0161 | 0.0000 | 0.0012 | 0.0500 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0166 | 0.0000 | 0.0058 | 0.2924 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0641 | 0.0000 | 0.0498 | 2.6112 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0117 | 0.0000 | 0.0008 | 0.0145 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0122 | 0.0000 | 0.0009 | 0.0330 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0114 | 0.0000 | 0.0010 | 0.2073 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0116 | 0.0000 | 0.0012 | 2.3263 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0133 | 0.0000 | 0.0010 | 0.0167 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0137 | 0.0000 | 0.0011 | 0.0364 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0137 | 0.0000 | 0.0034 | 0.2799 |
| [rust-native](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0151 | 0.0000 | 0.0415 | 17.7999 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.1116 | 0.0002 | 0.0038 | 0.1193 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0407 | 0.0001 | 0.0031 | 0.0805 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0354 | 0.0000 | 0.0034 | 0.4264 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0430 | 0.0000 | 0.0036 | 3.7193 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0412 | 0.0000 | 0.0036 | 0.0492 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0360 | 0.0000 | 0.0032 | 0.0742 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0488 | 0.0001 | 0.0049 | 0.4474 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0437 | 0.0000 | 0.0464 | 4.1202 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0410 | 0.0000 | 0.0031 | 0.0480 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0420 | 0.0000 | 0.0041 | 0.1010 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0397 | 0.0000 | 0.0037 | 0.4624 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0474 | 0.0000 | 0.0210 | 5.0283 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0702 | 0.0002 | 0.0036 | 0.0788 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0471 | 0.0001 | 0.0038 | 0.0896 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0519 | 0.0002 | 0.0046 | 0.4518 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0454 | 0.0000 | 0.0388 | 4.0497 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0419 | 0.0000 | 0.0032 | 0.0492 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0406 | 0.0000 | 0.0028 | 0.0801 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0391 | 0.0000 | 0.0032 | 0.4142 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0393 | 0.0000 | 0.0033 | 4.0818 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0469 | 0.0000 | 0.0032 | 0.0540 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0424 | 0.0000 | 0.0032 | 0.0892 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0531 | 0.0000 | 0.0048 | 0.6008 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0370 | 0.0000 | 0.0196 | 19.1313 |
