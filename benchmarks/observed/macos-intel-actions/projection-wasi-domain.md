| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0122 | 122.3 | 9.6 | 1000 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0899 | 89.9 | 11.3 | 10000 | 0.87 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.9202 | 92.0 | 24.8 | 100000 | 1.01 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2354 | 2354.0 | 9.9 | 1000 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0474 | 473.5 | 138.2 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.2830 | 283.0 | 140.6 | 10000 | 0.78 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 2.8930 | 289.3 | 154.2 | 100000 | 1.01 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 1.7279 | 17278.9 | 156.4 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0252 | 251.8 | 0.8 | 1000 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2636 | 263.6 | 2.4 | 10000 | 1.02 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 2.7970 | 279.7 | 15.2 | 100000 | 1.03 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0315 | 315.0 | 0.9 | 1000 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.9493 | 949.3 | 5.4 | 10000 | 1.48 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0284 | 284.4 | 9.9 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2954 | 295.4 | 11.1 | 10000 | 1.02 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 3.1278 | 312.8 | 20.5 | 100000 | 1.02 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0366 | 366.1 | 10.1 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.6744 | 674.4 | 12.1 | 10000 | 1.27 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0175 | 0.0014 | 0.0017 | 0.0329 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0157 | 0.0011 | 0.0025 | 0.1094 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.0183 | 0.0010 | 0.0039 | 0.9443 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0163 | 0.0010 | 0.0019 | 0.2548 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.8094 | 0.0098 | 1.5772 | 2.4529 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.2344 | 0.0068 | 1.4972 | 2.0281 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.2497 | 0.0062 | 1.4271 | 4.5843 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2522 | 0.0071 | 1.4059 | 3.3983 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0128 | 0.0002 | 0.0008 | 0.0390 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0127 | 0.0000 | 0.0010 | 0.2775 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0131 | 0.0000 | 0.0018 | 2.8126 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0126 | 0.0000 | 0.0010 | 0.0450 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0111 | 0.0000 | 0.0015 | 0.9620 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0333 | 0.0000 | 0.0027 | 0.0647 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0342 | 0.0000 | 0.0028 | 0.3326 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0385 | 0.0000 | 0.0032 | 3.1698 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0322 | 0.0001 | 0.0025 | 0.0716 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0289 | 0.0000 | 0.0031 | 0.7065 |
