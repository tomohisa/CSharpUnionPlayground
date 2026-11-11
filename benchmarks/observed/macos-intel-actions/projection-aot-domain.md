| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0144 | 144.2 | 9.6 | 1000 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0768 | 76.8 | 11.2 | 10000 | 0.73 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.8730 | 87.3 | 24.8 | 100000 | 1.06 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2028 | 2028.2 | 9.9 | 1000 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 20.9663 | 20966.3 | 16.7 | 10000 | 2.01 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0192 | 0.0016 | 0.0020 | 0.0374 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0129 | 0.0010 | 0.0015 | 0.0924 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 10000000 | 0.0164 | 0.0011 | 0.0029 | 0.8942 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0135 | 0.0010 | 0.0016 | 0.2191 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.0151 | 0.0009 | 0.0023 | 20.9849 |
