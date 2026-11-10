| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| csharp-native | indexed-append | dense | 100000 | 0.0024 | 24.4 | 17.2 | 1000 | — | ok |
| csharp-native | indexed-append | dense | 1000000 | 0.0200 | 20.0 | 20.1 | 10000 | 0.91 | ok |
| csharp-native | indexed-append | dense | 10000000 | 0.1997 | 20.0 | 33.8 | 100000 | 1.00 | ok |
| csharp-native | indexed-copy | dense | 100000 | 0.0382 | 382.3 | 18.7 | 1000 | — | ok |
| csharp-native | indexed-copy | dense | 1000000 | 3.5981 | 3598.1 | 24.3 | 10000 | 1.97 | ok |
| csharp-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| rust-native | indexed-append | dense | 100000 | 0.0086 | 85.8 | 1.9 | 1000 | — | ok |
| rust-native | indexed-append | dense | 1000000 | 0.0631 | 63.1 | 3.5 | 10000 | 0.87 | ok |
| rust-native | indexed-append | dense | 10000000 | 0.6234 | 62.3 | 13.2 | 100000 | 0.99 | ok |
| rust-native | indexed-copy | dense | 100000 | 0.0079 | 78.7 | 3.4 | 1000 | — | ok |
| rust-native | indexed-copy | dense | 1000000 | 0.2035 | 203.5 | 7.9 | 10000 | 1.41 | ok |
| rust-native | indexed-copy | dense | 10000000 | 9.1261 | 912.6 | 54.4 | 100000 | 1.65 | ok |
| typescript-native | indexed-append | dense | 100000 | 0.0095 | 94.7 | 113.6 | 1000 | — | ok |
| typescript-native | indexed-append | dense | 1000000 | 0.0454 | 45.4 | 131.2 | 10000 | 0.68 | ok |
| typescript-native | indexed-append | dense | 10000000 | 0.3598 | 36.0 | 204.6 | 100000 | 0.90 | ok |
| typescript-native | indexed-copy | dense | 100000 | 0.0221 | 220.9 | 116.4 | 1000 | — | ok |
| typescript-native | indexed-copy | dense | 1000000 | 1.0985 | 1098.5 | 241.4 | 10000 | 1.70 | ok |
| typescript-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| java-native | indexed-append | dense | 100000 | 0.0518 | 518.2 | 76.4 | 1000 | — | ok |
| java-native | indexed-append | dense | 1000000 | 0.0730 | 73.0 | 131.3 | 10000 | 0.15 | ok |
| java-native | indexed-append | dense | 10000000 | 0.2347 | 23.5 | 369.2 | 100000 | 0.51 | ok |
| java-native | indexed-copy | dense | 100000 | 0.0559 | 558.6 | 124.8 | 1000 | — | ok |
| java-native | indexed-copy | dense | 1000000 | 0.2531 | 253.1 | 1302.8 | 10000 | 0.66 | ok |
| java-native | indexed-copy | dense | 10000000 | 11.3206 | 1132.1 | 1501.7 | 100000 | 1.65 | ok |
| go-native | indexed-append | dense | 100000 | 0.0035 | 35.5 | 8.5 | 1000 | — | ok |
| go-native | indexed-append | dense | 1000000 | 0.0326 | 32.6 | 9.7 | 10000 | 0.96 | ok |
| go-native | indexed-append | dense | 10000000 | 0.3203 | 32.0 | 28.0 | 100000 | 0.99 | ok |
| go-native | indexed-copy | dense | 100000 | 0.0065 | 64.5 | 9.5 | 1000 | — | ok |
| go-native | indexed-copy | dense | 1000000 | 0.3260 | 326.0 | 15.7 | 10000 | 1.70 | ok |
| go-native | indexed-copy | dense | 10000000 | 17.5075 | 1750.8 | 668.2 | 100000 | 1.73 | ok |
| swift-native | indexed-append | dense | 100000 | 0.0209 | 209.2 | 2.3 | 1000 | — | ok |
| swift-native | indexed-append | dense | 1000000 | 0.1918 | 191.8 | 3.4 | 10000 | 0.96 | ok |
| swift-native | indexed-append | dense | 10000000 | 1.9470 | 194.7 | 18.7 | 100000 | 1.01 | ok |
| swift-native | indexed-copy | dense | 100000 | 0.0205 | 205.1 | 2.3 | 1000 | — | ok |
| swift-native | indexed-copy | dense | 1000000 | 0.3111 | 311.1 | 4.5 | 10000 | 1.18 | ok |
| swift-native | indexed-copy | dense | 10000000 | 7.5866 | 758.7 | 35.0 | 100000 | 1.39 | ok |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| csharp-native | indexed-append | dense | 100000 | 0.1942 | 0.0040 | 0.0012 | 0.2019 |
| csharp-native | indexed-append | dense | 1000000 | 0.0070 | 0.0036 | 0.0017 | 0.0324 |
| csharp-native | indexed-append | dense | 10000000 | 0.0072 | 0.0035 | 0.0015 | 0.2122 |
| csharp-native | indexed-copy | dense | 100000 | 0.0062 | 0.0033 | 0.0015 | 0.0493 |
| csharp-native | indexed-copy | dense | 1000000 | 0.0067 | 0.0034 | 0.0014 | 3.6098 |
| rust-native | indexed-append | dense | 100000 | 0.0732 | 0.0000 | 0.0006 | 0.0824 |
| rust-native | indexed-append | dense | 1000000 | 0.0041 | 0.0000 | 0.0006 | 0.0678 |
| rust-native | indexed-append | dense | 10000000 | 0.0046 | 0.0000 | 0.0007 | 0.6288 |
| rust-native | indexed-copy | dense | 100000 | 0.0047 | 0.0000 | 0.0005 | 0.0131 |
| rust-native | indexed-copy | dense | 1000000 | 0.0047 | 0.0000 | 0.0007 | 0.2090 |
| rust-native | indexed-copy | dense | 10000000 | 0.0044 | 0.0000 | 0.0013 | 9.1320 |
| typescript-native | indexed-append | dense | 100000 | 0.0609 | 0.0000 | 0.0045 | 0.0751 |
| typescript-native | indexed-append | dense | 1000000 | 0.0614 | 0.0000 | 0.0036 | 0.1114 |
| typescript-native | indexed-append | dense | 10000000 | 0.0600 | 0.0000 | 0.0063 | 0.4315 |
| typescript-native | indexed-copy | dense | 100000 | 0.0604 | 0.0000 | 0.0040 | 0.0869 |
| typescript-native | indexed-copy | dense | 1000000 | 0.0610 | 0.0000 | 0.0061 | 1.1666 |
| java-native | indexed-append | dense | 100000 | 0.0341 | 0.0028 | 0.0144 | 0.1110 |
| java-native | indexed-append | dense | 1000000 | 0.0345 | 0.0031 | 0.0163 | 0.1379 |
| java-native | indexed-append | dense | 10000000 | 0.0355 | 0.0030 | 0.0189 | 0.3099 |
| java-native | indexed-copy | dense | 100000 | 0.0352 | 0.0027 | 0.0153 | 0.1164 |
| java-native | indexed-copy | dense | 1000000 | 0.0351 | 0.0028 | 0.0278 | 0.3296 |
| java-native | indexed-copy | dense | 10000000 | 0.0382 | 0.0036 | 0.0243 | 11.4043 |
| go-native | indexed-append | dense | 100000 | 0.0203 | 0.0000 | 0.0006 | 0.0245 |
| go-native | indexed-append | dense | 1000000 | 0.0047 | 0.0000 | 0.0007 | 0.0380 |
| go-native | indexed-append | dense | 10000000 | 0.0047 | 0.0000 | 0.0011 | 0.3264 |
| go-native | indexed-copy | dense | 100000 | 0.0049 | 0.0000 | 0.0007 | 0.0120 |
| go-native | indexed-copy | dense | 1000000 | 0.0050 | 0.0000 | 0.0008 | 0.3319 |
| go-native | indexed-copy | dense | 10000000 | 0.0050 | 0.0000 | 0.0063 | 17.5192 |
| swift-native | indexed-append | dense | 100000 | 0.1501 | 0.0000 | 0.0007 | 0.1718 |
| swift-native | indexed-append | dense | 1000000 | 0.0050 | 0.0000 | 0.0006 | 0.1975 |
| swift-native | indexed-append | dense | 10000000 | 0.0048 | 0.0000 | 0.0008 | 1.9530 |
| swift-native | indexed-copy | dense | 100000 | 0.0051 | 0.0000 | 0.0006 | 0.0262 |
| swift-native | indexed-copy | dense | 1000000 | 0.0049 | 0.0000 | 0.0007 | 0.3168 |
| swift-native | indexed-copy | dense | 10000000 | 0.0048 | 0.0000 | 0.0010 | 7.5927 |
