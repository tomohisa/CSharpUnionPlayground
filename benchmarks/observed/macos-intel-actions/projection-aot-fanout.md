| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0004 | 385.3 | 3.4 | 10 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0026 | 262.9 | 4.0 | 100 | 0.83 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0537 | 536.8 | 9.6 | 1000 | 1.31 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.2615 | 261.5 | 11.2 | 10000 | 0.69 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0003 | 320.3 | 3.4 | 1 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0032 | 322.9 | 4.5 | 1 | 1.00 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0416 | 416.1 | 14.8 | 1 | 1.11 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.4843 | 484.3 | 65.0 | 1 | 1.07 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0004 | 404.9 | 3.4 | 10 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0053 | 525.3 | 5.3 | 100 | 1.11 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.2373 | 2373.1 | 9.9 | 1000 | 1.65 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 21.4312 | 21431.2 | 17.0 | 10000 | 1.96 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0004 | 395.6 | 3.4 | 1 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0029 | 292.0 | 4.6 | 1 | 0.87 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0412 | 412.2 | 14.6 | 1 | 1.15 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.4785 | 478.5 | 63.5 | 1 | 1.06 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0003 | 253.5 | 3.3 | 10 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0024 | 235.5 | 4.0 | 100 | 0.97 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0241 | 241.1 | 9.6 | 1000 | 1.01 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.2304 | 230.4 | 11.3 | 10000 | 0.98 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0003 | 347.6 | 3.4 | 1 | — | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0033 | 325.8 | 4.4 | 1 | 0.97 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.1025 | 1024.6 | 12.2 | 1 | 1.50 | ok |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | wide | 1000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 1000 | 0.0220 | 0.0015 | 0.0013 | 0.0252 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 10000 | 0.0156 | 0.0011 | 0.0012 | 0.0206 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 100000 | 0.0160 | 0.0011 | 0.0021 | 0.0731 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | dense | 1000000 | 0.0145 | 0.0012 | 0.0018 | 0.2793 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 1000 | 0.0160 | 0.0009 | 0.0011 | 0.0184 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 10000 | 0.0151 | 0.0011 | 0.0011 | 0.0208 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 100000 | 0.0168 | 0.0013 | 0.0022 | 0.0737 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-append | wide | 1000000 | 0.0158 | 0.0012 | 0.0058 | 0.5297 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 1000 | 0.0152 | 0.0011 | 0.0013 | 0.0180 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 10000 | 0.0150 | 0.0009 | 0.0014 | 0.0226 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 100000 | 0.0159 | 0.0012 | 0.0023 | 0.2569 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | dense | 1000000 | 0.0171 | 0.0010 | 0.0022 | 21.4521 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 1000 | 0.0143 | 0.0009 | 0.0012 | 0.0169 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 10000 | 0.0123 | 0.0009 | 0.0011 | 0.0175 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 100000 | 0.0137 | 0.0009 | 0.0018 | 0.0587 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | indexed-copy | wide | 1000000 | 0.0150 | 0.0017 | 0.0048 | 0.5372 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 1000 | 0.0114 | 0.0008 | 0.0009 | 0.0134 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 10000 | 0.0113 | 0.0008 | 0.0010 | 0.0155 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 100000 | 0.0119 | 0.0009 | 0.0016 | 0.0386 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | dense | 1000000 | 0.0145 | 0.0009 | 0.0019 | 0.2479 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | wide | 1000 | 0.0124 | 0.0011 | 0.0011 | 0.0149 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | wide | 10000 | 0.0119 | 0.0008 | 0.0011 | 0.0173 |
| [csharp-native](code/aot/projection/csharp/Program.cs) | linear-append | wide | 100000 | 0.0126 | 0.0009 | 0.0016 | 0.1190 |
