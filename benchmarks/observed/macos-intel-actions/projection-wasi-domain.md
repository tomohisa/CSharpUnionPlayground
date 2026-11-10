| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0124 | 124.0 | 9.6 | 1000 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.1104 | 110.4 | 11.3 | 10000 | 0.95 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.8437 | 84.4 | 24.8 | 100000 | 0.88 | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2115 | 2114.7 | 9.9 | 1000 | — | ok |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0406 | 406.0 | 138.3 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.3560 | 356.0 | 140.6 | 10000 | 0.94 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 3.4692 | 346.9 | 154.3 | 100000 | 0.99 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 1.7704 | 17704.2 | 156.6 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0334 | 334.3 | 0.8 | 1000 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2917 | 291.7 | 2.5 | 10000 | 0.94 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 2.6057 | 260.6 | 15.3 | 100000 | 0.95 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0307 | 306.9 | 0.9 | 1000 | — | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.9319 | 931.9 | 5.4 | 10000 | 1.48 | ok |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0302 | 301.5 | 9.9 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2916 | 291.6 | 11.1 | 10000 | 0.99 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 3.4081 | 340.8 | 20.6 | 100000 | 1.07 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0449 | 448.6 | 10.1 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.6294 | 629.4 | 12.1 | 10000 | 1.15 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0160 | 0.0010 | 0.0019 | 0.0314 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0195 | 0.0014 | 0.0023 | 0.1339 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.0151 | 0.0010 | 0.0024 | 0.8631 |
| [csharp-native](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0133 | 0.0010 | 0.0016 | 0.2276 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.3197 | 0.0092 | 1.7092 | 2.0865 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.3003 | 0.0098 | 1.9287 | 2.6031 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.3090 | 0.0081 | 1.6870 | 5.4834 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2893 | 0.0073 | 1.6795 | 3.7516 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0140 | 0.0000 | 0.0010 | 0.0485 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0140 | 0.0000 | 0.0030 | 0.3087 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0111 | 0.0000 | 0.0019 | 2.6192 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0121 | 0.0000 | 0.0009 | 0.0437 |
| [rust-native](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0109 | 0.0000 | 0.0011 | 0.9440 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0305 | 0.0000 | 0.0026 | 0.0635 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0327 | 0.0000 | 0.0028 | 0.3273 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0325 | 0.0000 | 0.0035 | 3.4450 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0376 | 0.0000 | 0.0035 | 0.0862 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0338 | 0.0000 | 0.0026 | 0.6660 |
