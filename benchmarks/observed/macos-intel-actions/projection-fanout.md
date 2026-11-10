| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| csharp-native | indexed-append | dense | 1000 | 0.0003 | 301.8 | 3.4 | 10 | — | ok |
| csharp-native | indexed-append | dense | 10000 | 0.0029 | 294.5 | 4.0 | 100 | 0.99 | ok |
| csharp-native | indexed-append | dense | 100000 | 0.0280 | 280.2 | 9.6 | 1000 | 0.98 | ok |
| csharp-native | indexed-append | dense | 1000000 | 0.2671 | 267.1 | 11.3 | 10000 | 0.98 | ok |
| csharp-native | indexed-append | wide | 1000 | 0.0004 | 357.0 | 3.5 | 1 | — | ok |
| csharp-native | indexed-append | wide | 10000 | 0.0032 | 319.7 | 4.5 | 1 | 0.95 | ok |
| csharp-native | indexed-append | wide | 100000 | 0.0432 | 431.6 | 14.8 | 1 | 1.13 | ok |
| csharp-native | indexed-append | wide | 1000000 | 0.8353 | 835.3 | 65.0 | 1 | 1.29 | ok |
| csharp-native | indexed-copy | dense | 1000 | 0.0004 | 405.5 | 3.4 | 10 | — | ok |
| csharp-native | indexed-copy | dense | 10000 | 0.0058 | 583.4 | 5.3 | 100 | 1.16 | ok |
| csharp-native | indexed-copy | dense | 100000 | 0.3273 | 3272.8 | 10.0 | 1000 | 1.75 | ok |
| csharp-native | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| csharp-native | indexed-copy | wide | 1000 | 0.0004 | 389.7 | 3.4 | 1 | — | ok |
| csharp-native | indexed-copy | wide | 10000 | 0.0045 | 445.7 | 4.6 | 1 | 1.06 | ok |
| csharp-native | indexed-copy | wide | 100000 | 0.0509 | 508.5 | 14.5 | 1 | 1.06 | ok |
| csharp-native | indexed-copy | wide | 1000000 | 0.5047 | 504.7 | 63.5 | 1 | 1.00 | ok |
| csharp-native | linear-append | dense | 1000 | 0.0003 | 323.4 | 3.3 | 10 | — | ok |
| csharp-native | linear-append | dense | 10000 | 0.0028 | 277.6 | 4.0 | 100 | 0.93 | ok |
| csharp-native | linear-append | dense | 100000 | 0.0300 | 299.7 | 9.6 | 1000 | 1.03 | ok |
| csharp-native | linear-append | dense | 1000000 | 0.2760 | 276.0 | 11.3 | 10000 | 0.96 | ok |
| csharp-native | linear-append | wide | 1000 | 0.0004 | 426.4 | 3.4 | 1 | — | ok |
| csharp-native | linear-append | wide | 10000 | 0.0059 | 587.2 | 4.4 | 1 | 1.14 | ok |
| csharp-native | linear-append | wide | 100000 | 0.2894 | 2894.4 | 12.2 | 1 | 1.69 | ok |
| csharp-native | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| rust-native | indexed-append | dense | 1000 | 0.0021 | 2092.3 | 0.7 | 10 | — | ok |
| rust-native | indexed-append | dense | 10000 | 0.0190 | 1899.0 | 0.7 | 100 | 0.96 | ok |
| rust-native | indexed-append | dense | 100000 | 0.1850 | 1849.8 | 0.8 | 1000 | 0.99 | ok |
| rust-native | indexed-append | dense | 1000000 | 1.9658 | 1965.8 | 2.6 | 10000 | 1.03 | ok |
| rust-native | indexed-append | wide | 1000 | 0.0017 | 1737.5 | 0.7 | 1 | — | ok |
| rust-native | indexed-append | wide | 10000 | 0.0189 | 1891.1 | 1.1 | 1 | 1.04 | ok |
| rust-native | indexed-append | wide | 100000 | 0.1802 | 1801.8 | 7.3 | 1 | 0.98 | ok |
| rust-native | indexed-append | wide | 1000000 | 1.9367 | 1936.7 | 54.3 | 1 | 1.03 | ok |
| rust-native | indexed-copy | dense | 1000 | 0.0022 | 2229.4 | 0.7 | 10 | — | ok |
| rust-native | indexed-copy | dense | 10000 | 0.0211 | 2113.6 | 0.8 | 100 | 0.98 | ok |
| rust-native | indexed-copy | dense | 100000 | 0.2078 | 2078.4 | 1.3 | 1000 | 0.99 | ok |
| rust-native | indexed-copy | dense | 1000000 | 2.5430 | 2543.0 | 6.3 | 10000 | 1.09 | ok |
| rust-native | indexed-copy | wide | 1000 | 0.0018 | 1786.5 | 0.7 | 1 | — | ok |
| rust-native | indexed-copy | wide | 10000 | 0.0200 | 2000.3 | 1.2 | 1 | 1.05 | ok |
| rust-native | indexed-copy | wide | 100000 | 0.1864 | 1863.9 | 7.3 | 1 | 0.97 | ok |
| rust-native | indexed-copy | wide | 1000000 | 2.2539 | 2253.9 | 54.3 | 1 | 1.08 | ok |
| rust-native | linear-append | dense | 1000 | 0.0020 | 1953.3 | 0.7 | 10 | — | ok |
| rust-native | linear-append | dense | 10000 | 0.0171 | 1712.3 | 0.7 | 100 | 0.94 | ok |
| rust-native | linear-append | dense | 100000 | 0.1975 | 1975.4 | 0.8 | 1000 | 1.06 | ok |
| rust-native | linear-append | dense | 1000000 | 1.9211 | 1921.1 | 2.7 | 10000 | 0.99 | ok |
| rust-native | linear-append | wide | 1000 | 0.0018 | 1782.1 | 0.7 | 1 | — | ok |
| rust-native | linear-append | wide | 10000 | 0.0177 | 1772.4 | 1.1 | 1 | 1.00 | ok |
| rust-native | linear-append | wide | 100000 | 0.2351 | 2350.9 | 4.7 | 1 | 1.12 | ok |
| rust-native | linear-append | wide | 1000000 | 12.7969 | 12796.9 | 43.1 | 1 | 1.74 | ok |
| typescript-native | indexed-append | dense | 1000 | 0.0057 | 5664.8 | 73.6 | 10 | — | ok |
| typescript-native | indexed-append | dense | 10000 | 0.0134 | 1342.2 | 74.2 | 100 | 0.37 | ok |
| typescript-native | indexed-append | dense | 100000 | 0.0624 | 623.9 | 85.0 | 1000 | 0.67 | ok |
| typescript-native | indexed-append | dense | 1000000 | 0.4677 | 467.7 | 99.9 | 10000 | 0.87 | ok |
| typescript-native | indexed-append | wide | 1000 | 0.0070 | 6980.1 | 73.2 | 1 | — | ok |
| typescript-native | indexed-append | wide | 10000 | 0.0161 | 1607.1 | 75.4 | 1 | 0.36 | ok |
| typescript-native | indexed-append | wide | 100000 | 0.0666 | 665.7 | 98.6 | 1 | 0.62 | ok |
| typescript-native | indexed-append | wide | 1000000 | 0.6363 | 636.3 | 230.5 | 1 | 0.98 | ok |
| typescript-native | indexed-copy | dense | 1000 | 0.0251 | 25113.8 | 73.7 | 10 | — | ok |
| typescript-native | indexed-copy | dense | 10000 | 0.0229 | 2294.5 | 74.2 | 100 | -0.04 | ok |
| typescript-native | indexed-copy | dense | 100000 | 0.1319 | 1319.0 | 85.7 | 1000 | 0.76 | ok |
| typescript-native | indexed-copy | dense | 1000000 | 4.2520 | 4252.0 | 209.2 | 10000 | 1.51 | ok |
| typescript-native | indexed-copy | wide | 1000 | 0.0100 | 10038.6 | 74.6 | 1 | — | ok |
| typescript-native | indexed-copy | wide | 10000 | 0.0171 | 1713.0 | 75.2 | 1 | 0.23 | ok |
| typescript-native | indexed-copy | wide | 100000 | 0.0829 | 829.1 | 95.6 | 1 | 0.68 | ok |
| typescript-native | indexed-copy | wide | 1000000 | 0.6843 | 684.3 | 192.7 | 1 | 0.92 | ok |
| typescript-native | linear-append | dense | 1000 | 0.0067 | 6667.3 | 75.4 | 10 | — | ok |
| typescript-native | linear-append | dense | 10000 | 0.0166 | 1663.3 | 75.0 | 100 | 0.40 | ok |
| typescript-native | linear-append | dense | 100000 | 0.1248 | 1247.6 | 87.3 | 1000 | 0.88 | ok |
| typescript-native | linear-append | dense | 1000000 | 0.4718 | 471.8 | 99.9 | 10000 | 0.58 | ok |
| typescript-native | linear-append | wide | 1000 | 0.0064 | 6444.6 | 73.6 | 1 | — | ok |
| typescript-native | linear-append | wide | 10000 | 0.0289 | 2891.2 | 75.2 | 1 | 0.65 | ok |
| typescript-native | linear-append | wide | 100000 | 0.1729 | 1729.3 | 99.6 | 1 | 0.78 | ok |
| typescript-native | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| java-native | indexed-append | dense | 1000 | 0.1265 | 126516.0 | 38.9 | 10 | — | ok |
| java-native | indexed-append | dense | 10000 | 0.2189 | 21893.4 | 53.8 | 100 | 0.24 | ok |
| java-native | indexed-append | dense | 100000 | 0.3451 | 3450.7 | 56.4 | 1000 | 0.20 | ok |
| java-native | indexed-append | dense | 1000000 | 0.6202 | 620.2 | 89.9 | 10000 | 0.25 | ok |
| java-native | indexed-append | wide | 1000 | 0.1125 | 112507.0 | 39.6 | 1 | — | ok |
| java-native | indexed-append | wide | 10000 | 0.1935 | 19348.7 | 53.0 | 1 | 0.24 | ok |
| java-native | indexed-append | wide | 100000 | 0.3058 | 3058.4 | 60.7 | 1 | 0.20 | ok |
| java-native | indexed-append | wide | 1000000 | 0.6680 | 668.0 | 143.4 | 1 | 0.34 | ok |
| java-native | indexed-copy | dense | 1000 | 0.1070 | 106979.4 | 39.1 | 10 | — | ok |
| java-native | indexed-copy | dense | 10000 | 0.2303 | 23029.7 | 40.0 | 100 | 0.33 | ok |
| java-native | indexed-copy | dense | 100000 | 0.3154 | 3154.3 | 86.5 | 1000 | 0.14 | ok |
| java-native | indexed-copy | dense | 1000000 | 2.4051 | 2405.1 | 1038.5 | 10000 | 0.88 | ok |
| java-native | indexed-copy | wide | 1000 | 0.1254 | 125434.3 | 38.2 | 1 | — | ok |
| java-native | indexed-copy | wide | 10000 | 0.3093 | 30932.4 | 52.3 | 1 | 0.39 | ok |
| java-native | indexed-copy | wide | 100000 | 0.3645 | 3644.8 | 60.8 | 1 | 0.07 | ok |
| java-native | indexed-copy | wide | 1000000 | 0.6412 | 641.2 | 148.9 | 1 | 0.25 | ok |
| java-native | linear-append | dense | 1000 | 0.1234 | 123417.8 | 39.4 | 10 | — | ok |
| java-native | linear-append | dense | 10000 | 0.1969 | 19691.9 | 51.7 | 100 | 0.20 | ok |
| java-native | linear-append | dense | 100000 | 0.2862 | 2862.4 | 56.1 | 1000 | 0.16 | ok |
| java-native | linear-append | dense | 1000000 | 0.5720 | 572.0 | 90.9 | 10000 | 0.30 | ok |
| java-native | linear-append | wide | 1000 | 0.1279 | 127906.6 | 38.8 | 1 | — | ok |
| java-native | linear-append | wide | 10000 | 0.2579 | 25788.6 | 52.8 | 1 | 0.30 | ok |
| java-native | linear-append | wide | 100000 | 0.3909 | 3909.3 | 61.5 | 1 | 0.18 | ok |
| java-native | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| go-native | indexed-append | dense | 1000 | 0.0004 | 366.9 | 2.1 | 10 | — | ok |
| go-native | indexed-append | dense | 10000 | 0.0031 | 313.2 | 2.6 | 100 | 0.93 | ok |
| go-native | indexed-append | dense | 100000 | 0.0307 | 307.0 | 6.1 | 1000 | 0.99 | ok |
| go-native | indexed-append | dense | 1000000 | 0.2949 | 294.9 | 7.6 | 10000 | 0.98 | ok |
| go-native | indexed-append | wide | 1000 | 0.0004 | 387.2 | 2.2 | 1 | — | ok |
| go-native | indexed-append | wide | 10000 | 0.0036 | 357.3 | 3.0 | 1 | 0.97 | ok |
| go-native | indexed-append | wide | 100000 | 0.0358 | 357.8 | 7.9 | 1 | 1.00 | ok |
| go-native | indexed-append | wide | 1000000 | 0.3575 | 357.5 | 48.8 | 1 | 1.00 | ok |
| go-native | indexed-copy | dense | 1000 | 0.0004 | 364.6 | 2.1 | 10 | — | ok |
| go-native | indexed-copy | dense | 10000 | 0.0033 | 332.9 | 3.1 | 100 | 0.96 | ok |
| go-native | indexed-copy | dense | 100000 | 0.0421 | 420.6 | 7.6 | 1000 | 1.10 | ok |
| go-native | indexed-copy | dense | 1000000 | 2.1799 | 2179.9 | 32.8 | 10000 | 1.71 | ok |
| go-native | indexed-copy | wide | 1000 | 0.0004 | 400.1 | 2.2 | 1 | — | ok |
| go-native | indexed-copy | wide | 10000 | 0.0036 | 359.0 | 3.1 | 1 | 0.95 | ok |
| go-native | indexed-copy | wide | 100000 | 0.0364 | 364.1 | 7.9 | 1 | 1.01 | ok |
| go-native | indexed-copy | wide | 1000000 | 0.3652 | 365.2 | 49.0 | 1 | 1.00 | ok |
| go-native | linear-append | dense | 1000 | 0.0004 | 357.2 | 2.2 | 10 | — | ok |
| go-native | linear-append | dense | 10000 | 0.0031 | 307.0 | 2.6 | 100 | 0.93 | ok |
| go-native | linear-append | dense | 100000 | 0.0313 | 312.9 | 6.4 | 1000 | 1.01 | ok |
| go-native | linear-append | dense | 1000000 | 0.3047 | 304.7 | 7.5 | 10000 | 0.99 | ok |
| go-native | linear-append | wide | 1000 | 0.0004 | 418.5 | 2.3 | 1 | — | ok |
| go-native | linear-append | wide | 10000 | 0.0037 | 371.9 | 2.9 | 1 | 0.95 | ok |
| go-native | linear-append | wide | 100000 | 0.0794 | 793.9 | 7.0 | 1 | 1.33 | ok |
| go-native | linear-append | wide | 1000000 | 15.2364 | 15236.4 | 44.6 | 1 | 2.28 | ok |
| swift-native | indexed-append | dense | 1000 | 0.0063 | 6262.1 | 0.8 | 10 | — | ok |
| swift-native | indexed-append | dense | 10000 | 0.0637 | 6373.4 | 0.9 | 100 | 1.01 | ok |
| swift-native | indexed-append | dense | 100000 | 0.6474 | 6473.9 | 1.0 | 1000 | 1.01 | ok |
| swift-native | indexed-append | dense | 1000000 | 6.6344 | 6634.4 | 2.6 | 10000 | 1.01 | ok |
| swift-native | indexed-append | wide | 1000 | 0.0066 | 6564.9 | 0.9 | 1 | — | ok |
| swift-native | indexed-append | wide | 10000 | 0.0644 | 6441.4 | 1.2 | 1 | 0.99 | ok |
| swift-native | indexed-append | wide | 100000 | 0.6570 | 6570.0 | 4.0 | 1 | 1.01 | ok |
| swift-native | indexed-append | wide | 1000000 | 7.7884 | 7788.4 | 29.9 | 1 | 1.07 | ok |
| swift-native | indexed-copy | dense | 1000 | 0.0069 | 6942.0 | 0.9 | 10 | — | ok |
| swift-native | indexed-copy | dense | 10000 | 0.0764 | 7639.3 | 0.9 | 100 | 1.04 | ok |
| swift-native | indexed-copy | dense | 100000 | 1.0468 | 10467.6 | 1.4 | 1000 | 1.14 | ok |
| swift-native | indexed-copy | dense | 1000000 | 8.8753 | 8875.3 | 3.2 | 10000 | 0.93 | ok |
| swift-native | indexed-copy | wide | 1000 | 0.0090 | 8956.4 | 0.9 | 1 | — | ok |
| swift-native | indexed-copy | wide | 10000 | 0.0761 | 7610.5 | 1.1 | 1 | 0.93 | ok |
| swift-native | indexed-copy | wide | 100000 | 0.7593 | 7592.9 | 3.9 | 1 | 1.00 | ok |
| swift-native | indexed-copy | wide | 1000000 | 8.4849 | 8484.9 | 29.9 | 1 | 1.05 | ok |
| swift-native | linear-append | dense | 1000 | 0.0077 | 7675.5 | 0.8 | 10 | — | ok |
| swift-native | linear-append | dense | 10000 | 0.0688 | 6878.0 | 0.9 | 100 | 0.95 | ok |
| swift-native | linear-append | dense | 100000 | 0.6711 | 6710.6 | 1.0 | 1000 | 0.99 | ok |
| swift-native | linear-append | dense | 1000000 | 8.3626 | 8362.6 | 2.5 | 10000 | 1.10 | ok |
| swift-native | linear-append | wide | 1000 | 0.0104 | 10403.3 | 0.9 | 1 | — | ok |
| swift-native | linear-append | wide | 10000 | 0.1028 | 10281.3 | 1.1 | 1 | 0.99 | ok |
| swift-native | linear-append | wide | 100000 | 2.1650 | 21650.3 | 3.5 | 1 | 1.32 | ok |
| swift-native | linear-append | wide | 1000000 | — | — | — | — | — | timeout |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| csharp-native | indexed-append | dense | 1000 | 0.0202 | 0.0012 | 0.0012 | 0.0230 |
| csharp-native | indexed-append | dense | 10000 | 0.0131 | 0.0010 | 0.0011 | 0.0182 |
| csharp-native | indexed-append | dense | 100000 | 0.0125 | 0.0009 | 0.0016 | 0.0432 |
| csharp-native | indexed-append | dense | 1000000 | 0.0171 | 0.0011 | 0.0026 | 0.2882 |
| csharp-native | indexed-append | wide | 1000 | 0.0236 | 0.0008 | 0.0011 | 0.0259 |
| csharp-native | indexed-append | wide | 10000 | 0.0144 | 0.0009 | 0.0012 | 0.0201 |
| csharp-native | indexed-append | wide | 100000 | 0.0181 | 0.0015 | 0.0028 | 0.0780 |
| csharp-native | indexed-append | wide | 1000000 | 0.0259 | 0.0020 | 0.0076 | 0.8942 |
| csharp-native | indexed-copy | dense | 1000 | 0.0186 | 0.0010 | 0.0012 | 0.0214 |
| csharp-native | indexed-copy | dense | 10000 | 0.0145 | 0.0012 | 0.0015 | 0.0231 |
| csharp-native | indexed-copy | dense | 100000 | 0.0181 | 0.0012 | 0.0075 | 0.3542 |
| csharp-native | indexed-copy | wide | 1000 | 0.0162 | 0.0009 | 0.0011 | 0.0187 |
| csharp-native | indexed-copy | wide | 10000 | 0.0168 | 0.0009 | 0.0014 | 0.0239 |
| csharp-native | indexed-copy | wide | 100000 | 0.0179 | 0.0009 | 0.0020 | 0.0731 |
| csharp-native | indexed-copy | wide | 1000000 | 0.0172 | 0.0012 | 0.0060 | 0.5730 |
| csharp-native | linear-append | dense | 1000 | 0.0190 | 0.0009 | 0.0009 | 0.0214 |
| csharp-native | linear-append | dense | 10000 | 0.0179 | 0.0011 | 0.0011 | 0.0230 |
| csharp-native | linear-append | dense | 100000 | 0.0183 | 0.0009 | 0.0017 | 0.0511 |
| csharp-native | linear-append | dense | 1000000 | 0.0181 | 0.0010 | 0.0017 | 0.2970 |
| csharp-native | linear-append | wide | 1000 | 0.0232 | 0.0012 | 0.0014 | 0.0262 |
| csharp-native | linear-append | wide | 10000 | 0.0191 | 0.0010 | 0.0013 | 0.0275 |
| csharp-native | linear-append | wide | 100000 | 0.0192 | 0.0012 | 0.0020 | 0.3136 |
| rust-native | indexed-append | dense | 1000 | 0.0152 | 0.0000 | 0.0007 | 0.0180 |
| rust-native | indexed-append | dense | 10000 | 0.0129 | 0.0000 | 0.0008 | 0.0327 |
| rust-native | indexed-append | dense | 100000 | 0.0131 | 0.0000 | 0.0011 | 0.1991 |
| rust-native | indexed-append | dense | 1000000 | 0.0122 | 0.0000 | 0.0010 | 1.9791 |
| rust-native | indexed-append | wide | 1000 | 0.0131 | 0.0000 | 0.0007 | 0.0155 |
| rust-native | indexed-append | wide | 10000 | 0.0135 | 0.0000 | 0.0010 | 0.0335 |
| rust-native | indexed-append | wide | 100000 | 0.0148 | 0.0000 | 0.0035 | 0.1991 |
| rust-native | indexed-append | wide | 1000000 | 0.0141 | 0.0000 | 0.0490 | 2.0087 |
| rust-native | indexed-copy | dense | 1000 | 0.0161 | 0.0000 | 0.0010 | 0.0194 |
| rust-native | indexed-copy | dense | 10000 | 0.0167 | 0.0000 | 0.0009 | 0.0387 |
| rust-native | indexed-copy | dense | 100000 | 0.0148 | 0.0000 | 0.0008 | 0.2234 |
| rust-native | indexed-copy | dense | 1000000 | 0.0149 | 0.0000 | 0.0014 | 2.5593 |
| rust-native | indexed-copy | wide | 1000 | 0.0087 | 0.0000 | 0.0006 | 0.0111 |
| rust-native | indexed-copy | wide | 10000 | 0.0101 | 0.0000 | 0.0009 | 0.0310 |
| rust-native | indexed-copy | wide | 100000 | 0.0115 | 0.0000 | 0.0036 | 0.2021 |
| rust-native | indexed-copy | wide | 1000000 | 0.0139 | 0.0000 | 0.0412 | 2.3173 |
| rust-native | linear-append | dense | 1000 | 0.0139 | 0.0000 | 0.0008 | 0.0167 |
| rust-native | linear-append | dense | 10000 | 0.0122 | 0.0000 | 0.0007 | 0.0300 |
| rust-native | linear-append | dense | 100000 | 0.0145 | 0.0000 | 0.0012 | 0.2132 |
| rust-native | linear-append | dense | 1000000 | 0.0149 | 0.0000 | 0.0009 | 1.9371 |
| rust-native | linear-append | wide | 1000 | 0.0096 | 0.0000 | 0.0006 | 0.0119 |
| rust-native | linear-append | wide | 10000 | 0.0094 | 0.0001 | 0.0008 | 0.0280 |
| rust-native | linear-append | wide | 100000 | 0.0145 | 0.0000 | 0.0028 | 0.2526 |
| rust-native | linear-append | wide | 1000000 | 0.0138 | 0.0000 | 0.0371 | 12.8537 |
| typescript-native | indexed-append | dense | 1000 | 0.1966 | 0.0014 | 0.0157 | 0.2201 |
| typescript-native | indexed-append | dense | 10000 | 0.1650 | 0.0013 | 0.0120 | 0.1924 |
| typescript-native | indexed-append | dense | 100000 | 0.1809 | 0.0010 | 0.0096 | 0.2547 |
| typescript-native | indexed-append | dense | 1000000 | 0.2112 | 0.0018 | 0.0117 | 0.6952 |
| typescript-native | indexed-append | wide | 1000 | 0.2109 | 0.0012 | 0.0194 | 0.2391 |
| typescript-native | indexed-append | wide | 10000 | 0.1953 | 0.0014 | 0.0084 | 0.2239 |
| typescript-native | indexed-append | wide | 100000 | 0.1622 | 0.0019 | 0.0114 | 0.2539 |
| typescript-native | indexed-append | wide | 1000000 | 0.1607 | 0.0014 | 0.0262 | 0.9405 |
| typescript-native | indexed-copy | dense | 1000 | 0.1930 | 0.0014 | 0.0257 | 0.2457 |
| typescript-native | indexed-copy | dense | 10000 | 0.2149 | 0.0016 | 0.0117 | 0.2520 |
| typescript-native | indexed-copy | dense | 100000 | 0.1726 | 0.0014 | 0.0287 | 0.3354 |
| typescript-native | indexed-copy | dense | 1000000 | 0.2042 | 0.0018 | 0.0214 | 4.4847 |
| typescript-native | indexed-copy | wide | 1000 | 0.1794 | 0.0020 | 0.0134 | 0.2055 |
| typescript-native | indexed-copy | wide | 10000 | 0.2965 | 0.0018 | 0.0108 | 0.3289 |
| typescript-native | indexed-copy | wide | 100000 | 0.1797 | 0.0013 | 0.0166 | 0.2932 |
| typescript-native | indexed-copy | wide | 1000000 | 0.1768 | 0.0011 | 0.0243 | 0.9263 |
| typescript-native | linear-append | dense | 1000 | 0.1612 | 0.0011 | 0.0149 | 0.1844 |
| typescript-native | linear-append | dense | 10000 | 0.2064 | 0.0013 | 0.0138 | 0.2389 |
| typescript-native | linear-append | dense | 100000 | 0.1680 | 0.0012 | 0.0136 | 0.3084 |
| typescript-native | linear-append | dense | 1000000 | 0.1783 | 0.0015 | 0.0151 | 0.6696 |
| typescript-native | linear-append | wide | 1000 | 0.1665 | 0.0015 | 0.0163 | 0.1919 |
| typescript-native | linear-append | wide | 10000 | 0.2011 | 0.0015 | 0.0099 | 0.2435 |
| typescript-native | linear-append | wide | 100000 | 0.1781 | 0.0013 | 0.0128 | 0.3800 |
| java-native | indexed-append | dense | 1000 | 0.1735 | 0.0166 | 0.0066 | 0.3840 |
| java-native | indexed-append | dense | 10000 | 0.1394 | 0.0168 | 0.0453 | 0.4573 |
| java-native | indexed-append | dense | 100000 | 0.1823 | 0.0159 | 0.0204 | 0.6037 |
| java-native | indexed-append | dense | 1000000 | 0.1275 | 0.0150 | 0.0219 | 0.8360 |
| java-native | indexed-append | wide | 1000 | 0.1245 | 0.0158 | 0.0475 | 0.3469 |
| java-native | indexed-append | wide | 10000 | 0.1203 | 0.0153 | 0.0165 | 0.3836 |
| java-native | indexed-append | wide | 100000 | 0.1336 | 0.0177 | 0.0065 | 0.5713 |
| java-native | indexed-append | wide | 1000000 | 0.1331 | 0.0167 | 0.0766 | 0.9753 |
| java-native | indexed-copy | dense | 1000 | 0.1176 | 0.0145 | 0.0373 | 0.3128 |
| java-native | indexed-copy | dense | 10000 | 0.1266 | 0.0164 | 0.0320 | 0.4433 |
| java-native | indexed-copy | dense | 100000 | 0.1432 | 0.0117 | 0.0518 | 0.5548 |
| java-native | indexed-copy | dense | 1000000 | 0.1322 | 0.0160 | 0.0794 | 2.6852 |
| java-native | indexed-copy | wide | 1000 | 0.2205 | 0.0190 | 0.0274 | 0.4685 |
| java-native | indexed-copy | wide | 10000 | 0.1516 | 0.0207 | 0.0083 | 0.5737 |
| java-native | indexed-copy | wide | 100000 | 0.1696 | 0.0181 | 0.0352 | 0.6614 |
| java-native | indexed-copy | wide | 1000000 | 0.1455 | 0.0171 | 0.0186 | 0.9045 |
| java-native | linear-append | dense | 1000 | 0.1361 | 0.0191 | 0.0536 | 0.3781 |
| java-native | linear-append | dense | 10000 | 0.1409 | 0.0161 | 0.0315 | 0.4251 |
| java-native | linear-append | dense | 100000 | 0.1577 | 0.0179 | 0.0060 | 0.5618 |
| java-native | linear-append | dense | 1000000 | 0.1325 | 0.0177 | 0.0081 | 0.7758 |
| java-native | linear-append | wide | 1000 | 0.1443 | 0.0210 | 0.0165 | 0.3549 |
| java-native | linear-append | wide | 10000 | 0.1651 | 0.0214 | 0.0406 | 0.5350 |
| java-native | linear-append | wide | 100000 | 0.1432 | 0.0184 | 0.0141 | 0.6261 |
| go-native | indexed-append | dense | 1000 | 0.0105 | 0.0000 | 0.0013 | 0.0123 |
| go-native | indexed-append | dense | 10000 | 0.0100 | 0.0000 | 0.0007 | 0.0140 |
| go-native | indexed-append | dense | 100000 | 0.0099 | 0.0000 | 0.0009 | 0.0417 |
| go-native | indexed-append | dense | 1000000 | 0.0098 | 0.0000 | 0.0010 | 0.3060 |
| go-native | indexed-append | wide | 1000 | 0.0099 | 0.0000 | 0.0008 | 0.0112 |
| go-native | indexed-append | wide | 10000 | 0.0096 | 0.0000 | 0.0007 | 0.0143 |
| go-native | indexed-append | wide | 100000 | 0.0099 | 0.0000 | 0.0010 | 0.0486 |
| go-native | indexed-append | wide | 1000000 | 0.0099 | 0.0000 | 0.0028 | 0.4091 |
| go-native | indexed-copy | dense | 1000 | 0.0102 | 0.0000 | 0.0008 | 0.0115 |
| go-native | indexed-copy | dense | 10000 | 0.0098 | 0.0000 | 0.0009 | 0.0143 |
| go-native | indexed-copy | dense | 100000 | 0.0097 | 0.0000 | 0.0011 | 0.0531 |
| go-native | indexed-copy | dense | 1000000 | 0.0098 | 0.0000 | 0.0019 | 2.1919 |
| go-native | indexed-copy | wide | 1000 | 0.0100 | 0.0000 | 0.0008 | 0.0113 |
| go-native | indexed-copy | wide | 10000 | 0.0100 | 0.0000 | 0.0008 | 0.0147 |
| go-native | indexed-copy | wide | 100000 | 0.0100 | 0.0000 | 0.0010 | 0.0495 |
| go-native | indexed-copy | wide | 1000000 | 0.0100 | 0.0000 | 0.0029 | 0.4182 |
| go-native | linear-append | dense | 1000 | 0.0098 | 0.0000 | 0.0009 | 0.0112 |
| go-native | linear-append | dense | 10000 | 0.0107 | 0.0000 | 0.0009 | 0.0147 |
| go-native | linear-append | dense | 100000 | 0.0102 | 0.0000 | 0.0010 | 0.0426 |
| go-native | linear-append | dense | 1000000 | 0.0102 | 0.0000 | 0.0014 | 0.3167 |
| go-native | linear-append | wide | 1000 | 0.0102 | 0.0000 | 0.0008 | 0.0115 |
| go-native | linear-append | wide | 10000 | 0.0096 | 0.0000 | 0.0007 | 0.0143 |
| go-native | linear-append | wide | 100000 | 0.0098 | 0.0000 | 0.0009 | 0.0918 |
| go-native | linear-append | wide | 1000000 | 0.0107 | 0.0000 | 0.0037 | 15.2755 |
| swift-native | indexed-append | dense | 1000 | 0.0093 | 0.0000 | 0.0006 | 0.0163 |
| swift-native | indexed-append | dense | 10000 | 0.0088 | 0.0000 | 0.0007 | 0.0733 |
| swift-native | indexed-append | dense | 100000 | 0.0091 | 0.0000 | 0.0007 | 0.6574 |
| swift-native | indexed-append | dense | 1000000 | 0.0093 | 0.0000 | 0.0008 | 6.6447 |
| swift-native | indexed-append | wide | 1000 | 0.0093 | 0.0000 | 0.0006 | 0.0166 |
| swift-native | indexed-append | wide | 10000 | 0.0095 | 0.0000 | 0.0007 | 0.0750 |
| swift-native | indexed-append | wide | 100000 | 0.0096 | 0.0000 | 0.0009 | 0.6699 |
| swift-native | indexed-append | wide | 1000000 | 0.0108 | 0.0000 | 0.0027 | 7.8473 |
| swift-native | indexed-copy | dense | 1000 | 0.0130 | 0.0000 | 0.0009 | 0.0210 |
| swift-native | indexed-copy | dense | 10000 | 0.0112 | 0.0000 | 0.0008 | 0.0886 |
| swift-native | indexed-copy | dense | 100000 | 0.0125 | 0.0000 | 0.0010 | 1.0606 |
| swift-native | indexed-copy | dense | 1000000 | 0.0129 | 0.0000 | 0.0014 | 8.8898 |
| swift-native | indexed-copy | wide | 1000 | 0.0139 | 0.0000 | 0.0011 | 0.0241 |
| swift-native | indexed-copy | wide | 10000 | 0.0110 | 0.0000 | 0.0008 | 0.0882 |
| swift-native | indexed-copy | wide | 100000 | 0.0106 | 0.0000 | 0.0011 | 0.7737 |
| swift-native | indexed-copy | wide | 1000000 | 0.0140 | 0.0000 | 0.0024 | 8.5414 |
| swift-native | linear-append | dense | 1000 | 0.0109 | 0.0000 | 0.0008 | 0.0194 |
| swift-native | linear-append | dense | 10000 | 0.0109 | 0.0000 | 0.0008 | 0.0806 |
| swift-native | linear-append | dense | 100000 | 0.0099 | 0.0000 | 0.0007 | 0.6819 |
| swift-native | linear-append | dense | 1000000 | 0.0172 | 0.0000 | 0.0014 | 8.3817 |
| swift-native | linear-append | wide | 1000 | 0.0143 | 0.0000 | 0.0010 | 0.0258 |
| swift-native | linear-append | wide | 10000 | 0.0118 | 0.0000 | 0.0009 | 0.1158 |
| swift-native | linear-append | wide | 100000 | 0.0130 | 0.0000 | 0.0012 | 2.1823 |
