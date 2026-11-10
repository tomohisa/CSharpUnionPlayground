| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| csharp-native | indexed-append | dense | 100000 | 0.0256 | 255.9 | 9.6 | 1000 | — | ok |
| csharp-native | indexed-append | dense | 1000000 | 0.0968 | 96.8 | 11.3 | 10000 | 0.58 | ok |
| csharp-native | indexed-append | dense | 10000000 | 1.0064 | 100.6 | 24.7 | 100000 | 1.02 | ok |
| csharp-native | indexed-copy | dense | 100000 | 0.2325 | 2324.7 | 9.9 | 1000 | — | ok |
| csharp-native | indexed-copy | dense | 1000000 | 24.1433 | 24143.3 | 16.6 | 10000 | 2.02 | ok |
| csharp-native | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| rust-native | indexed-append | dense | 100000 | 0.0297 | 297.4 | 0.8 | 1000 | — | ok |
| rust-native | indexed-append | dense | 1000000 | 0.3015 | 301.5 | 2.6 | 10000 | 1.01 | ok |
| rust-native | indexed-append | dense | 10000000 | 2.8684 | 286.8 | 15.3 | 100000 | 0.98 | ok |
| rust-native | indexed-copy | dense | 100000 | 0.0336 | 335.7 | 1.0 | 1000 | — | ok |
| rust-native | indexed-copy | dense | 1000000 | 0.9614 | 961.4 | 5.4 | 10000 | 1.46 | ok |
| rust-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| typescript-native | indexed-append | dense | 100000 | 0.0482 | 481.6 | 82.4 | 1000 | — | ok |
| typescript-native | indexed-append | dense | 1000000 | 0.2303 | 230.3 | 100.8 | 10000 | 0.68 | ok |
| typescript-native | indexed-append | dense | 10000000 | 2.1977 | 219.8 | 171.3 | 100000 | 0.98 | ok |
| typescript-native | indexed-copy | dense | 100000 | 0.1065 | 1065.3 | 86.1 | 1000 | — | ok |
| typescript-native | indexed-copy | dense | 1000000 | 4.5777 | 4577.7 | 209.2 | 10000 | 1.63 | ok |
| typescript-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| java-native | indexed-append | dense | 100000 | 0.2459 | 2459.4 | 55.2 | 1000 | — | ok |
| java-native | indexed-append | dense | 1000000 | 0.3831 | 383.1 | 89.3 | 10000 | 0.19 | ok |
| java-native | indexed-append | dense | 10000000 | 1.3791 | 137.9 | 273.1 | 100000 | 0.56 | ok |
| java-native | indexed-copy | dense | 100000 | 0.4119 | 4118.6 | 86.6 | 1000 | — | ok |
| java-native | indexed-copy | dense | 1000000 | 4.7917 | 4791.7 | 1323.6 | 10000 | 1.07 | ok |
| java-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| go-native | indexed-append | dense | 100000 | 0.0136 | 136.3 | 6.3 | 1000 | — | ok |
| go-native | indexed-append | dense | 1000000 | 0.1314 | 131.4 | 7.5 | 10000 | 0.98 | ok |
| go-native | indexed-append | dense | 10000000 | 1.7375 | 173.7 | 22.9 | 100000 | 1.12 | ok |
| go-native | indexed-copy | dense | 100000 | 0.0359 | 359.1 | 7.9 | 1000 | — | ok |
| go-native | indexed-copy | dense | 1000000 | 2.6925 | 2692.5 | 16.5 | 10000 | 1.87 | ok |
| go-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| swift-native | indexed-append | dense | 100000 | 0.1255 | 1255.4 | 1.0 | 1000 | — | ok |
| swift-native | indexed-append | dense | 1000000 | 1.1977 | 1197.7 | 2.6 | 10000 | 0.98 | ok |
| swift-native | indexed-append | dense | 10000000 | 11.5496 | 1155.0 | 18.8 | 100000 | 0.98 | ok |
| swift-native | indexed-copy | dense | 100000 | 0.1130 | 1130.1 | 1.0 | 1000 | — | ok |
| swift-native | indexed-copy | dense | 1000000 | 1.3384 | 1338.4 | 3.2 | 10000 | 1.07 | ok |
| swift-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| csharp-native | indexed-append | dense | 100000 | 0.0318 | 0.0028 | 0.0037 | 0.0641 |
| csharp-native | indexed-append | dense | 1000000 | 0.0159 | 0.0010 | 0.0020 | 0.1159 |
| csharp-native | indexed-append | dense | 10000000 | 0.0168 | 0.0011 | 0.0032 | 1.0284 |
| csharp-native | indexed-copy | dense | 100000 | 0.0168 | 0.0013 | 0.0017 | 0.2524 |
| csharp-native | indexed-copy | dense | 1000000 | 0.0158 | 0.0011 | 0.0025 | 24.1633 |
| rust-native | indexed-append | dense | 100000 | 0.0133 | 0.0000 | 0.0010 | 0.0440 |
| rust-native | indexed-append | dense | 1000000 | 0.0122 | 0.0000 | 0.0011 | 0.3148 |
| rust-native | indexed-append | dense | 10000000 | 0.0126 | 0.0000 | 0.0018 | 2.8833 |
| rust-native | indexed-copy | dense | 100000 | 0.0116 | 0.0000 | 0.0011 | 0.0463 |
| rust-native | indexed-copy | dense | 1000000 | 0.0116 | 0.0000 | 0.0013 | 0.9743 |
| typescript-native | indexed-append | dense | 100000 | 0.1938 | 0.0014 | 0.0135 | 0.2577 |
| typescript-native | indexed-append | dense | 1000000 | 0.1895 | 0.0017 | 0.0121 | 0.4364 |
| typescript-native | indexed-append | dense | 10000000 | 0.1671 | 0.0019 | 0.0226 | 2.4070 |
| typescript-native | indexed-copy | dense | 100000 | 0.2015 | 0.0016 | 0.0134 | 0.3241 |
| typescript-native | indexed-copy | dense | 1000000 | 0.1956 | 0.0020 | 0.0205 | 4.7986 |
| java-native | indexed-append | dense | 100000 | 0.1500 | 0.0199 | 0.0468 | 0.5082 |
| java-native | indexed-append | dense | 1000000 | 0.1597 | 0.0198 | 0.0463 | 0.6683 |
| java-native | indexed-append | dense | 10000000 | 0.1572 | 0.0207 | 0.0286 | 1.6903 |
| java-native | indexed-copy | dense | 100000 | 0.1692 | 0.0207 | 0.0513 | 0.7151 |
| java-native | indexed-copy | dense | 1000000 | 0.2031 | 0.0199 | 0.1072 | 5.1913 |
| go-native | indexed-append | dense | 100000 | 0.0105 | 0.0000 | 0.0014 | 0.0256 |
| go-native | indexed-append | dense | 1000000 | 0.0099 | 0.0000 | 0.0012 | 0.1428 |
| go-native | indexed-append | dense | 10000000 | 0.0136 | 0.0000 | 0.0026 | 1.7548 |
| go-native | indexed-copy | dense | 100000 | 0.0125 | 0.0000 | 0.0015 | 0.0502 |
| go-native | indexed-copy | dense | 1000000 | 0.0144 | 0.0000 | 0.0022 | 2.7095 |
| swift-native | indexed-append | dense | 100000 | 0.0133 | 0.0000 | 0.0011 | 0.1401 |
| swift-native | indexed-append | dense | 1000000 | 0.0147 | 0.0000 | 0.0013 | 1.2141 |
| swift-native | indexed-append | dense | 10000000 | 0.0139 | 0.0000 | 0.0020 | 11.5666 |
| swift-native | indexed-copy | dense | 100000 | 0.0133 | 0.0000 | 0.0011 | 0.1275 |
| swift-native | indexed-copy | dense | 1000000 | 0.0126 | 0.0000 | 0.0011 | 1.3527 |
