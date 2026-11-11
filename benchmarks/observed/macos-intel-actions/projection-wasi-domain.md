| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0355 | 355.1 | 138.3 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.2753 | 275.3 | 140.6 | 10000 | 0.89 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 2.4939 | 249.4 | 154.3 | 100000 | 0.96 | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 1.4239 | 14238.6 | 156.6 | 1000 | — | ok |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0252 | 252.2 | 10.0 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.2866 | 286.6 | 11.2 | 10000 | 1.06 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 2.7117 | 271.2 | 20.5 | 100000 | 0.98 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0299 | 299.4 | 10.0 | 1000 | — | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.6037 | 603.7 | 12.2 | 10000 | 1.30 | ok |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.2675 | 0.0080 | 1.5328 | 1.8500 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.2542 | 0.0097 | 1.4245 | 1.9700 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.2194 | 0.0063 | 1.2398 | 3.9656 |
| [csharp-wasi](code/wasi/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2365 | 0.0089 | 1.3108 | 2.9862 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 100000 | 0.0280 | 0.0000 | 0.0024 | 0.0557 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 1000000 | 0.0278 | 0.0000 | 0.0024 | 0.3171 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-append | dense | 10000000 | 0.0310 | 0.0000 | 0.0033 | 2.7469 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 100000 | 0.0283 | 0.0000 | 0.0026 | 0.0610 |
| [rust-wasi](code/wasi/projection/rust/projection.rs) | indexed-copy | dense | 1000000 | 0.0292 | 0.0000 | 0.0027 | 0.6358 |
