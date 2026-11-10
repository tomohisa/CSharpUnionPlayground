| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0053 | 5265.1 | 517.5 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0097 | 971.6 | 141.6 | 100 | 0.27 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0738 | 738.3 | 138.2 | 1000 | 0.88 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.5680 | 568.0 | 144.8 | 10000 | 0.89 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0052 | 5202.3 | 137.4 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0104 | 1038.1 | 137.5 | 1 | 0.30 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0828 | 827.7 | 144.0 | 1 | 0.90 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.7701 | 770.1 | 193.2 | 1 | 0.97 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0075 | 7512.3 | 137.5 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0248 | 2482.3 | 137.4 | 100 | 0.52 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 1.3633 | 13633.3 | 156.6 | 1000 | 1.74 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0060 | 6038.0 | 141.7 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0126 | 1255.8 | 137.4 | 1 | 0.32 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.1193 | 1193.4 | 148.0 | 1 | 0.98 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.7809 | 780.9 | 192.9 | 1 | 0.82 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0052 | 5217.8 | 141.5 | 10 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0087 | 865.1 | 137.4 | 100 | 0.22 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0985 | 985.2 | 138.3 | 1000 | 1.06 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.6464 | 646.4 | 144.7 | 10000 | 0.82 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0036 | 3563.9 | 137.3 | 1 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0177 | 1767.6 | 137.4 | 1 | 0.70 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 1.0966 | 10965.8 | 141.0 | 1 | 1.79 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0031 | 3133.6 | 24.8 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0321 | 3206.2 | 9.8 | 100 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.3182 | 3182.1 | 10.0 | 1000 | 1.00 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 2.9073 | 2907.3 | 11.1 | 10000 | 0.96 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0030 | 2999.5 | 9.8 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0287 | 2865.1 | 10.1 | 1 | 0.98 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.2828 | 2828.1 | 13.5 | 1 | 0.99 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 3.1874 | 3187.4 | 40.4 | 1 | 1.05 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0028 | 2809.9 | 9.9 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0286 | 2857.2 | 9.8 | 100 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.2852 | 2852.3 | 10.0 | 1000 | 1.00 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 3.2270 | 3227.0 | 12.1 | 10000 | 1.05 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0028 | 2841.4 | 9.9 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0296 | 2964.0 | 10.0 | 1 | 1.02 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.3010 | 3010.4 | 13.5 | 1 | 1.01 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 3.1433 | 3143.3 | 40.4 | 1 | 1.02 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0034 | 3446.7 | 9.8 | 10 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0326 | 3256.3 | 9.8 | 100 | 0.98 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.2824 | 2823.6 | 9.9 | 1000 | 0.94 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 2.9490 | 2949.0 | 11.1 | 10000 | 1.02 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0033 | 3298.3 | 9.8 | 1 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0304 | 3040.9 | 10.0 | 1 | 0.96 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.3674 | 3674.0 | 12.1 | 1 | 1.08 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 11.3807 | 11380.7 | 35.0 | 1 | 1.49 | ok |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 3.9781 | 0.0058 | 1.2330 | 5.2269 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.2155 | 0.0060 | 1.2105 | 1.4468 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.2212 | 0.0074 | 1.2397 | 1.5475 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.2042 | 0.0059 | 1.1778 | 1.9610 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.2587 | 0.0075 | 1.6170 | 1.8950 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.2178 | 0.0063 | 1.2101 | 1.4532 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.2205 | 0.0070 | 1.2836 | 1.6326 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.2371 | 0.0067 | 1.3418 | 2.5878 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.2284 | 0.0077 | 1.2746 | 1.5244 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.2105 | 0.0056 | 1.2347 | 1.4808 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2111 | 0.0064 | 1.2243 | 2.8100 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.2299 | 0.0068 | 1.2631 | 1.5117 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.2346 | 0.0073 | 1.2639 | 1.5271 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.2787 | 0.0080 | 1.3346 | 1.8036 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.2078 | 0.0053 | 1.2983 | 2.5358 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.2284 | 0.0077 | 1.5378 | 1.7841 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.2093 | 0.0055 | 1.1990 | 1.4267 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.2673 | 0.0063 | 1.3672 | 1.7446 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.3330 | 0.0089 | 1.2849 | 2.2783 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.2129 | 0.0062 | 1.2107 | 1.4384 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.2196 | 0.0055 | 1.1799 | 1.4305 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.2199 | 0.0064 | 1.2300 | 2.5789 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000 | 0.0781 | 0.0000 | 0.0034 | 0.0849 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000 | 0.0304 | 0.0000 | 0.0026 | 0.0653 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0381 | 0.0001 | 0.0026 | 0.3592 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0326 | 0.0001 | 0.0029 | 2.9431 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000 | 0.0277 | 0.0000 | 0.0024 | 0.0333 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 10000 | 0.0309 | 0.0001 | 0.0025 | 0.0623 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 100000 | 0.0290 | 0.0000 | 0.0036 | 0.3162 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | wide | 1000000 | 0.0280 | 0.0000 | 0.0284 | 3.2525 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000 | 0.0282 | 0.0000 | 0.0023 | 0.0335 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000 | 0.0294 | 0.0000 | 0.0026 | 0.0607 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0304 | 0.0001 | 0.0025 | 0.3183 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0270 | 0.0000 | 0.0026 | 3.2568 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000 | 0.0276 | 0.0000 | 0.0026 | 0.0332 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 10000 | 0.0288 | 0.0000 | 0.0027 | 0.0613 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 100000 | 0.0374 | 0.0001 | 0.0055 | 0.3453 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | wide | 1000000 | 0.0267 | 0.0000 | 0.0342 | 3.2156 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000 | 0.0388 | 0.0000 | 0.0034 | 0.0458 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 10000 | 0.0352 | 0.0000 | 0.0027 | 0.0706 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 100000 | 0.0322 | 0.0000 | 0.0024 | 0.3172 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | dense | 1000000 | 0.0300 | 0.0000 | 0.0026 | 2.9818 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000 | 0.0310 | 0.0000 | 0.0026 | 0.0371 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 10000 | 0.0314 | 0.0000 | 0.0026 | 0.0647 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 100000 | 0.0312 | 0.0000 | 0.0035 | 0.4026 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | linear-append | wide | 1000000 | 0.0265 | 0.0000 | 0.0187 | 11.4313 |
