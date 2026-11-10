| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0282 | 282.3 | 138.3 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.2051 | 205.1 | 140.6 | 10000 | 0.86 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 1.9881 | 198.8 | 154.3 | 100000 | 0.99 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 1.1025 | 11024.8 | 156.6 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0351 | 350.7 | 9.9 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2977 | 297.7 | 11.1 | 10000 | 0.93 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 2.3558 | 235.6 | 20.6 | 100000 | 0.90 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0258 | 257.9 | 10.0 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.5143 | 514.3 | 12.2 | 10000 | 1.30 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 1.1229 | 0.0108 | 1.2331 | 2.4003 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.1923 | 0.0057 | 1.0567 | 1.4652 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.1788 | 0.0048 | 1.0590 | 3.2370 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.1767 | 0.0050 | 0.9693 | 2.2576 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0629 | 0.0000 | 0.0029 | 0.1010 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0453 | 0.0000 | 0.0026 | 0.3457 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0417 | 0.0001 | 0.0026 | 2.4010 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0230 | 0.0000 | 0.0024 | 0.0513 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0246 | 0.0000 | 0.0022 | 0.5412 |
