| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| csharp-native | indexed-append | dense | 100000 | 0.0095 | 95.4 | 9.6 | 1000 | — | ok |
| csharp-native | indexed-append | dense | 1000000 | 0.0716 | 71.6 | 11.3 | 10000 | 0.88 | ok |
| csharp-native | indexed-append | dense | 10000000 | 0.6883 | 68.8 | 24.7 | 100000 | 0.98 | ok |
| csharp-native | indexed-copy | dense | 100000 | 0.1771 | 1771.2 | 9.9 | 1000 | — | ok |
| csharp-native | indexed-copy | dense | 1000000 | 17.2358 | 17235.8 | 18.3 | 10000 | 1.99 | ok |
| csharp-native | indexed-copy | dense | 10000000 | — | — | — | — | — | not-run-after-budget |
| rust-native | indexed-append | dense | 100000 | 0.0189 | 189.3 | 0.8 | 1000 | — | ok |
| rust-native | indexed-append | dense | 1000000 | 0.1969 | 196.9 | 2.5 | 10000 | 1.02 | ok |
| rust-native | indexed-append | dense | 10000000 | 1.9871 | 198.7 | 15.3 | 100000 | 1.00 | ok |
| rust-native | indexed-copy | dense | 100000 | 0.0231 | 231.0 | 1.0 | 1000 | — | ok |
| rust-native | indexed-copy | dense | 1000000 | 0.6695 | 669.5 | 5.4 | 10000 | 1.46 | ok |
| rust-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| typescript-native | indexed-append | dense | 100000 | 0.0305 | 304.8 | 83.0 | 1000 | — | ok |
| typescript-native | indexed-append | dense | 1000000 | 0.1547 | 154.7 | 96.7 | 10000 | 0.71 | ok |
| typescript-native | indexed-append | dense | 10000000 | 1.2510 | 125.1 | 170.0 | 100000 | 0.91 | ok |
| typescript-native | indexed-copy | dense | 100000 | 0.0650 | 649.7 | 81.8 | 1000 | — | ok |
| typescript-native | indexed-copy | dense | 1000000 | 2.8928 | 2892.8 | 205.0 | 10000 | 1.65 | ok |
| typescript-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| typescript-zod-native | indexed-append | dense | 100000 | 0.0509 | 508.8 | 92.5 | 1000 | — | ok |
| typescript-zod-native | indexed-append | dense | 1000000 | 0.2778 | 277.8 | 103.1 | 10000 | 0.74 | ok |
| typescript-zod-native | indexed-append | dense | 10000000 | 2.3873 | 238.7 | 178.3 | 100000 | 0.93 | ok |
| typescript-zod-native | indexed-copy | dense | 100000 | 0.0815 | 814.8 | 90.3 | 1000 | — | ok |
| typescript-zod-native | indexed-copy | dense | 1000000 | 2.9828 | 2982.8 | 213.1 | 10000 | 1.56 | ok |
| typescript-zod-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| java-native | indexed-append | dense | 100000 | 0.1958 | 1957.7 | 58.5 | 1000 | — | ok |
| java-native | indexed-append | dense | 1000000 | 0.2943 | 294.3 | 88.9 | 10000 | 0.18 | ok |
| java-native | indexed-append | dense | 10000000 | 0.9852 | 98.5 | 272.3 | 100000 | 0.52 | ok |
| java-native | indexed-copy | dense | 100000 | 0.2102 | 2101.9 | 86.6 | 1000 | — | ok |
| java-native | indexed-copy | dense | 1000000 | 1.3085 | 1308.5 | 1175.6 | 10000 | 0.79 | ok |
| java-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| go-native | indexed-append | dense | 100000 | 0.0124 | 124.5 | 6.3 | 1000 | — | ok |
| go-native | indexed-append | dense | 1000000 | 0.1430 | 143.0 | 7.7 | 10000 | 1.06 | ok |
| go-native | indexed-append | dense | 10000000 | 1.1626 | 116.3 | 25.0 | 100000 | 0.91 | ok |
| go-native | indexed-copy | dense | 100000 | 0.0237 | 237.4 | 8.4 | 1000 | — | ok |
| go-native | indexed-copy | dense | 1000000 | 1.8588 | 1858.8 | 33.5 | 10000 | 1.89 | ok |
| go-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |
| swift-native | indexed-append | dense | 100000 | 0.0704 | 703.7 | 1.1 | 1000 | — | ok |
| swift-native | indexed-append | dense | 1000000 | 0.7052 | 705.2 | 2.4 | 10000 | 1.00 | ok |
| swift-native | indexed-append | dense | 10000000 | 8.6267 | 862.7 | 18.9 | 100000 | 1.09 | ok |
| swift-native | indexed-copy | dense | 100000 | 0.0945 | 945.5 | 1.0 | 1000 | — | ok |
| swift-native | indexed-copy | dense | 1000000 | 1.1935 | 1193.5 | 3.1 | 10000 | 1.10 | ok |
| swift-native | indexed-copy | dense | 10000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| csharp-native | indexed-append | dense | 100000 | 0.0115 | 0.0009 | 0.0013 | 0.0233 |
| csharp-native | indexed-append | dense | 1000000 | 0.0110 | 0.0007 | 0.0015 | 0.0850 |
| csharp-native | indexed-append | dense | 10000000 | 0.0114 | 0.0008 | 0.0021 | 0.7033 |
| csharp-native | indexed-copy | dense | 100000 | 0.0107 | 0.0008 | 0.0015 | 0.1902 |
| csharp-native | indexed-copy | dense | 1000000 | 0.0108 | 0.0007 | 0.0016 | 17.2490 |
| rust-native | indexed-append | dense | 100000 | 0.0076 | 0.0000 | 0.0006 | 0.0271 |
| rust-native | indexed-append | dense | 1000000 | 0.0076 | 0.0000 | 0.0007 | 0.2053 |
| rust-native | indexed-append | dense | 10000000 | 0.0074 | 0.0000 | 0.0020 | 1.9970 |
| rust-native | indexed-copy | dense | 100000 | 0.0074 | 0.0000 | 0.0011 | 0.0316 |
| rust-native | indexed-copy | dense | 1000000 | 0.0075 | 0.0000 | 0.0009 | 0.6778 |
| typescript-native | indexed-append | dense | 100000 | 0.1105 | 0.0002 | 0.0092 | 0.1510 |
| typescript-native | indexed-append | dense | 1000000 | 0.1139 | 0.0001 | 0.0080 | 0.2781 |
| typescript-native | indexed-append | dense | 10000000 | 0.1134 | 0.0002 | 0.0158 | 1.3925 |
| typescript-native | indexed-copy | dense | 100000 | 0.1242 | 0.0003 | 0.0082 | 0.1982 |
| typescript-native | indexed-copy | dense | 1000000 | 0.1192 | 0.0002 | 0.0143 | 3.0285 |
| typescript-zod-native | indexed-append | dense | 100000 | 0.1736 | 0.0002 | 0.0105 | 0.2359 |
| typescript-zod-native | indexed-append | dense | 1000000 | 0.1780 | 0.0002 | 0.0084 | 0.4666 |
| typescript-zod-native | indexed-append | dense | 10000000 | 0.1746 | 0.0002 | 0.0159 | 2.5903 |
| typescript-zod-native | indexed-copy | dense | 100000 | 0.1790 | 0.0002 | 0.0113 | 0.2726 |
| typescript-zod-native | indexed-copy | dense | 1000000 | 0.1775 | 0.0002 | 0.0187 | 3.1810 |
| java-native | indexed-append | dense | 100000 | 0.1199 | 0.0148 | 0.0163 | 0.3824 |
| java-native | indexed-append | dense | 1000000 | 0.1271 | 0.0167 | 0.0368 | 0.5233 |
| java-native | indexed-append | dense | 10000000 | 0.1174 | 0.0164 | 0.0160 | 1.1943 |
| java-native | indexed-copy | dense | 100000 | 0.1057 | 0.0136 | 0.0284 | 0.3878 |
| java-native | indexed-copy | dense | 1000000 | 0.1117 | 0.0135 | 0.0971 | 1.5706 |
| go-native | indexed-append | dense | 100000 | 0.0100 | 0.0000 | 0.0010 | 0.0236 |
| go-native | indexed-append | dense | 1000000 | 0.0097 | 0.0000 | 0.0010 | 0.1539 |
| go-native | indexed-append | dense | 10000000 | 0.0098 | 0.0000 | 0.0018 | 1.1749 |
| go-native | indexed-copy | dense | 100000 | 0.0094 | 0.0000 | 0.0011 | 0.0344 |
| go-native | indexed-copy | dense | 1000000 | 0.0096 | 0.0000 | 0.0020 | 1.8707 |
| swift-native | indexed-append | dense | 100000 | 0.0093 | 0.0000 | 0.0007 | 0.0805 |
| swift-native | indexed-append | dense | 1000000 | 0.0085 | 0.0000 | 0.0008 | 0.7148 |
| swift-native | indexed-append | dense | 10000000 | 0.0097 | 0.0000 | 0.0023 | 8.6400 |
| swift-native | indexed-copy | dense | 100000 | 0.0125 | 0.0000 | 0.0011 | 0.1082 |
| swift-native | indexed-copy | dense | 1000000 | 0.0119 | 0.0000 | 0.0008 | 1.2063 |
