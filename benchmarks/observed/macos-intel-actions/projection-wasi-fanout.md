| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |
|---|---|---|---:|---:|---:|---:|---:|---:|---|
| csharp-native | indexed-append | dense | 1000 | 0.0004 | 422.9 | 3.5 | 10 | — | ok |
| csharp-native | indexed-append | dense | 10000 | 0.0034 | 339.6 | 4.0 | 100 | 0.90 | ok |
| csharp-native | indexed-append | dense | 100000 | 0.0323 | 323.1 | 9.6 | 1000 | 0.98 | ok |
| csharp-native | indexed-append | dense | 1000000 | 0.3342 | 334.2 | 11.3 | 10000 | 1.01 | ok |
| csharp-native | indexed-append | wide | 1000 | 0.0006 | 554.6 | 3.4 | 1 | — | ok |
| csharp-native | indexed-append | wide | 10000 | 0.0037 | 370.4 | 4.5 | 1 | 0.82 | ok |
| csharp-native | indexed-append | wide | 100000 | 0.0519 | 519.0 | 14.8 | 1 | 1.15 | ok |
| csharp-native | indexed-append | wide | 1000000 | 0.5400 | 540.0 | 65.0 | 1 | 1.02 | ok |
| csharp-native | indexed-copy | dense | 1000 | 0.0006 | 584.8 | 3.4 | 10 | — | ok |
| csharp-native | indexed-copy | dense | 10000 | 0.0069 | 688.5 | 5.3 | 100 | 1.07 | ok |
| csharp-native | indexed-copy | dense | 100000 | 0.2826 | 2825.9 | 9.9 | 1000 | 1.61 | ok |
| csharp-native | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| csharp-native | indexed-copy | wide | 1000 | 0.0004 | 394.0 | 3.4 | 1 | — | ok |
| csharp-native | indexed-copy | wide | 10000 | 0.0315 | 3147.4 | 4.7 | 1 | 1.90 | ok |
| csharp-native | indexed-copy | wide | 100000 | 0.0479 | 479.1 | 14.6 | 1 | 0.18 | ok |
| csharp-native | indexed-copy | wide | 1000000 | 0.5484 | 548.4 | 63.5 | 1 | 1.06 | ok |
| csharp-native | linear-append | dense | 1000 | 0.0005 | 534.7 | 3.5 | 10 | — | ok |
| csharp-native | linear-append | dense | 10000 | 0.0030 | 303.4 | 4.0 | 100 | 0.75 | ok |
| csharp-native | linear-append | dense | 100000 | 0.0389 | 388.8 | 9.6 | 1000 | 1.11 | ok |
| csharp-native | linear-append | dense | 1000000 | 0.3657 | 365.7 | 11.3 | 10000 | 0.97 | ok |
| csharp-native | linear-append | wide | 1000 | 0.0005 | 480.1 | 3.4 | 1 | — | ok |
| csharp-native | linear-append | wide | 10000 | 0.0052 | 519.1 | 4.5 | 1 | 1.03 | ok |
| csharp-native | linear-append | wide | 100000 | 0.3004 | 3004.4 | 12.3 | 1 | 1.76 | ok |
| csharp-native | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| csharp-wasi | indexed-append | dense | 1000 | 0.0069 | 6929.3 | 503.3 | 10 | — | ok |
| csharp-wasi | indexed-append | dense | 10000 | 0.0111 | 1106.3 | 137.4 | 100 | 0.20 | ok |
| csharp-wasi | indexed-append | dense | 100000 | 0.0748 | 747.6 | 139.9 | 1000 | 0.83 | ok |
| csharp-wasi | indexed-append | dense | 1000000 | 0.7512 | 751.2 | 144.8 | 10000 | 1.00 | ok |
| csharp-wasi | indexed-append | wide | 1000 | 0.0063 | 6256.4 | 137.5 | 1 | — | ok |
| csharp-wasi | indexed-append | wide | 10000 | 0.0143 | 1427.7 | 137.4 | 1 | 0.36 | ok |
| csharp-wasi | indexed-append | wide | 100000 | 0.0790 | 790.3 | 143.9 | 1 | 0.74 | ok |
| csharp-wasi | indexed-append | wide | 1000000 | 0.7937 | 793.7 | 194.8 | 1 | 1.00 | ok |
| csharp-wasi | indexed-copy | dense | 1000 | 0.0086 | 8601.0 | 137.5 | 10 | — | ok |
| csharp-wasi | indexed-copy | dense | 10000 | 0.0333 | 3330.0 | 137.5 | 100 | 0.59 | ok |
| csharp-wasi | indexed-copy | dense | 100000 | 1.3921 | 13920.5 | 160.7 | 1000 | 1.62 | ok |
| csharp-wasi | indexed-copy | dense | 1000000 | — | — | — | — | — | timeout |
| csharp-wasi | indexed-copy | wide | 1000 | 0.0057 | 5702.9 | 137.3 | 1 | — | ok |
| csharp-wasi | indexed-copy | wide | 10000 | 0.0129 | 1294.2 | 137.4 | 1 | 0.36 | ok |
| csharp-wasi | indexed-copy | wide | 100000 | 0.0970 | 970.3 | 143.8 | 1 | 0.87 | ok |
| csharp-wasi | indexed-copy | wide | 1000000 | 0.8965 | 896.5 | 192.7 | 1 | 0.97 | ok |
| csharp-wasi | linear-append | dense | 1000 | 0.0050 | 4956.7 | 137.3 | 10 | — | ok |
| csharp-wasi | linear-append | dense | 10000 | 0.0118 | 1182.8 | 137.4 | 100 | 0.38 | ok |
| csharp-wasi | linear-append | dense | 100000 | 0.0691 | 691.0 | 142.4 | 1000 | 0.77 | ok |
| csharp-wasi | linear-append | dense | 1000000 | 0.7137 | 713.7 | 140.5 | 10000 | 1.01 | ok |
| csharp-wasi | linear-append | wide | 1000 | 0.0037 | 3735.7 | 141.4 | 1 | — | ok |
| csharp-wasi | linear-append | wide | 10000 | 0.0236 | 2358.5 | 137.3 | 1 | 0.80 | ok |
| csharp-wasi | linear-append | wide | 100000 | 1.2543 | 12543.2 | 140.9 | 1 | 1.73 | ok |
| csharp-wasi | linear-append | wide | 1000000 | — | — | — | — | — | timeout |
| rust-native | indexed-append | dense | 1000 | 0.0021 | 2087.2 | 0.7 | 10 | — | ok |
| rust-native | indexed-append | dense | 10000 | 0.0194 | 1937.4 | 0.7 | 100 | 0.97 | ok |
| rust-native | indexed-append | dense | 100000 | 0.1965 | 1965.0 | 0.8 | 1000 | 1.01 | ok |
| rust-native | indexed-append | dense | 1000000 | 1.9559 | 1955.9 | 2.7 | 10000 | 1.00 | ok |
| rust-native | indexed-append | wide | 1000 | 0.0021 | 2140.3 | 0.7 | 1 | — | ok |
| rust-native | indexed-append | wide | 10000 | 0.0227 | 2267.1 | 1.0 | 1 | 1.02 | ok |
| rust-native | indexed-append | wide | 100000 | 0.2253 | 2252.6 | 7.3 | 1 | 1.00 | ok |
| rust-native | indexed-append | wide | 1000000 | 2.0288 | 2028.8 | 54.3 | 1 | 0.95 | ok |
| rust-native | indexed-copy | dense | 1000 | 0.0019 | 1876.1 | 0.7 | 10 | — | ok |
| rust-native | indexed-copy | dense | 10000 | 0.0191 | 1912.2 | 0.7 | 100 | 1.01 | ok |
| rust-native | indexed-copy | dense | 100000 | 0.1922 | 1921.7 | 0.9 | 1000 | 1.00 | ok |
| rust-native | indexed-copy | dense | 1000000 | 2.5812 | 2581.2 | 5.6 | 10000 | 1.13 | ok |
| rust-native | indexed-copy | wide | 1000 | 0.0020 | 2016.4 | 0.7 | 1 | — | ok |
| rust-native | indexed-copy | wide | 10000 | 0.0179 | 1789.2 | 1.0 | 1 | 0.95 | ok |
| rust-native | indexed-copy | wide | 100000 | 0.1843 | 1843.3 | 7.3 | 1 | 1.01 | ok |
| rust-native | indexed-copy | wide | 1000000 | 1.8739 | 1873.9 | 54.3 | 1 | 1.01 | ok |
| rust-native | linear-append | dense | 1000 | 0.0018 | 1786.0 | 0.7 | 10 | — | ok |
| rust-native | linear-append | dense | 10000 | 0.0176 | 1759.5 | 0.7 | 100 | 0.99 | ok |
| rust-native | linear-append | dense | 100000 | 0.1749 | 1749.4 | 0.8 | 1000 | 1.00 | ok |
| rust-native | linear-append | dense | 1000000 | 1.9516 | 1951.6 | 2.5 | 10000 | 1.05 | ok |
| rust-native | linear-append | wide | 1000 | 0.0019 | 1941.5 | 0.7 | 1 | — | ok |
| rust-native | linear-append | wide | 10000 | 0.0193 | 1931.0 | 1.0 | 1 | 1.00 | ok |
| rust-native | linear-append | wide | 100000 | 0.2302 | 2301.6 | 4.6 | 1 | 1.08 | ok |
| rust-native | linear-append | wide | 1000000 | 12.0928 | 12092.8 | 43.1 | 1 | 1.72 | ok |
| rust-wasi | indexed-append | dense | 1000 | 0.0030 | 3032.0 | 23.8 | 10 | — | ok |
| rust-wasi | indexed-append | dense | 10000 | 0.0299 | 2992.0 | 9.9 | 100 | 0.99 | ok |
| rust-wasi | indexed-append | dense | 100000 | 0.2962 | 2962.4 | 9.9 | 1000 | 1.00 | ok |
| rust-wasi | indexed-append | dense | 1000000 | 3.0273 | 3027.3 | 11.2 | 10000 | 1.01 | ok |
| rust-wasi | indexed-append | wide | 1000 | 0.0030 | 2989.6 | 9.9 | 1 | — | ok |
| rust-wasi | indexed-append | wide | 10000 | 0.0295 | 2947.5 | 10.0 | 1 | 0.99 | ok |
| rust-wasi | indexed-append | wide | 100000 | 0.3101 | 3100.8 | 13.5 | 1 | 1.02 | ok |
| rust-wasi | indexed-append | wide | 1000000 | 3.4092 | 3409.2 | 40.5 | 1 | 1.04 | ok |
| rust-wasi | indexed-copy | dense | 1000 | 0.0036 | 3644.1 | 9.8 | 10 | — | ok |
| rust-wasi | indexed-copy | dense | 10000 | 0.0349 | 3494.6 | 9.9 | 100 | 0.98 | ok |
| rust-wasi | indexed-copy | dense | 100000 | 0.3525 | 3524.9 | 10.0 | 1000 | 1.00 | ok |
| rust-wasi | indexed-copy | dense | 1000000 | 4.4414 | 4441.4 | 12.1 | 10000 | 1.10 | ok |
| rust-wasi | indexed-copy | wide | 1000 | 0.0036 | 3568.6 | 9.8 | 1 | — | ok |
| rust-wasi | indexed-copy | wide | 10000 | 0.0310 | 3099.2 | 10.0 | 1 | 0.94 | ok |
| rust-wasi | indexed-copy | wide | 100000 | 0.3684 | 3684.0 | 13.5 | 1 | 1.08 | ok |
| rust-wasi | indexed-copy | wide | 1000000 | 4.0141 | 4014.1 | 40.4 | 1 | 1.04 | ok |
| rust-wasi | linear-append | dense | 1000 | 0.0038 | 3830.4 | 9.8 | 10 | — | ok |
| rust-wasi | linear-append | dense | 10000 | 0.0440 | 4404.6 | 9.8 | 100 | 1.06 | ok |
| rust-wasi | linear-append | dense | 100000 | 0.4098 | 4097.5 | 9.9 | 1000 | 0.97 | ok |
| rust-wasi | linear-append | dense | 1000000 | 4.0656 | 4065.6 | 11.1 | 10000 | 1.00 | ok |
| rust-wasi | linear-append | wide | 1000 | 0.0046 | 4600.1 | 9.8 | 1 | — | ok |
| rust-wasi | linear-append | wide | 10000 | 0.0380 | 3801.8 | 10.2 | 1 | 0.92 | ok |
| rust-wasi | linear-append | wide | 100000 | 0.4569 | 4569.1 | 12.2 | 1 | 1.08 | ok |
| rust-wasi | linear-append | wide | 1000000 | 20.3883 | 20388.3 | 35.2 | 1 | 1.65 | ok |

| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |
|---|---|---|---:|---:|---:|---:|---:|
| csharp-native | indexed-append | dense | 1000 | 0.0286 | 0.0021 | 0.0013 | 0.0327 |
| csharp-native | indexed-append | dense | 10000 | 0.0200 | 0.0012 | 0.0016 | 0.0264 |
| csharp-native | indexed-append | dense | 100000 | 0.0163 | 0.0011 | 0.0019 | 0.0519 |
| csharp-native | indexed-append | dense | 1000000 | 0.0256 | 0.0017 | 0.0023 | 0.3639 |
| csharp-native | indexed-append | wide | 1000 | 0.0246 | 0.0016 | 0.0016 | 0.0285 |
| csharp-native | indexed-append | wide | 10000 | 0.0209 | 0.0012 | 0.0013 | 0.0275 |
| csharp-native | indexed-append | wide | 100000 | 0.0242 | 0.0015 | 0.0023 | 0.0950 |
| csharp-native | indexed-append | wide | 1000000 | 0.0226 | 0.0011 | 0.0084 | 0.5993 |
| csharp-native | indexed-copy | dense | 1000 | 0.0225 | 0.0013 | 0.0014 | 0.0260 |
| csharp-native | indexed-copy | dense | 10000 | 0.0209 | 0.0012 | 0.0014 | 0.0304 |
| csharp-native | indexed-copy | dense | 100000 | 0.0209 | 0.0011 | 0.0022 | 0.3069 |
| csharp-native | indexed-copy | wide | 1000 | 0.0151 | 0.0010 | 0.0011 | 0.0177 |
| csharp-native | indexed-copy | wide | 10000 | 0.0199 | 0.0086 | 0.0012 | 0.0616 |
| csharp-native | indexed-copy | wide | 100000 | 0.0148 | 0.0010 | 0.0020 | 0.0670 |
| csharp-native | indexed-copy | wide | 1000000 | 0.0155 | 0.0010 | 0.0055 | 0.6142 |
| csharp-native | linear-append | dense | 1000 | 0.0201 | 0.0036 | 0.0019 | 0.0257 |
| csharp-native | linear-append | dense | 10000 | 0.0233 | 0.0013 | 0.0018 | 0.0294 |
| csharp-native | linear-append | dense | 100000 | 0.0211 | 0.0011 | 0.0017 | 0.0629 |
| csharp-native | linear-append | dense | 1000000 | 0.0209 | 0.0012 | 0.0025 | 0.3906 |
| csharp-native | linear-append | wide | 1000 | 0.0228 | 0.0012 | 0.0012 | 0.0259 |
| csharp-native | linear-append | wide | 10000 | 0.0251 | 0.0014 | 0.0018 | 0.0339 |
| csharp-native | linear-append | wide | 100000 | 0.0243 | 0.0020 | 0.0022 | 0.3311 |
| csharp-wasi | indexed-append | dense | 1000 | 6.6818 | 0.0083 | 1.4191 | 8.1221 |
| csharp-wasi | indexed-append | dense | 10000 | 0.2372 | 0.0073 | 1.3662 | 1.6272 |
| csharp-wasi | indexed-append | dense | 100000 | 0.2529 | 0.0069 | 1.4652 | 1.8064 |
| csharp-wasi | indexed-append | dense | 1000000 | 0.2338 | 0.0079 | 1.5131 | 2.5122 |
| csharp-wasi | indexed-append | wide | 1000 | 0.2686 | 0.0069 | 1.6943 | 1.9840 |
| csharp-wasi | indexed-append | wide | 10000 | 0.2606 | 0.0069 | 1.5047 | 1.7972 |
| csharp-wasi | indexed-append | wide | 100000 | 0.2367 | 0.0063 | 1.3286 | 1.6882 |
| csharp-wasi | indexed-append | wide | 1000000 | 0.2630 | 0.0083 | 1.4113 | 2.7399 |
| csharp-wasi | indexed-copy | dense | 1000 | 0.2846 | 0.0077 | 1.5951 | 1.9031 |
| csharp-wasi | indexed-copy | dense | 10000 | 0.2433 | 0.0075 | 1.4225 | 1.7126 |
| csharp-wasi | indexed-copy | dense | 100000 | 0.2231 | 0.0064 | 1.2237 | 2.8502 |
| csharp-wasi | indexed-copy | wide | 1000 | 0.2118 | 0.0057 | 1.2314 | 1.4596 |
| csharp-wasi | indexed-copy | wide | 10000 | 0.2426 | 0.0072 | 1.4680 | 1.7410 |
| csharp-wasi | indexed-copy | wide | 100000 | 0.2586 | 0.0079 | 1.3971 | 1.7973 |
| csharp-wasi | indexed-copy | wide | 1000000 | 0.2617 | 0.0071 | 1.4127 | 2.8438 |
| csharp-wasi | linear-append | dense | 1000 | 0.2397 | 0.0068 | 1.5823 | 1.8392 |
| csharp-wasi | linear-append | dense | 10000 | 0.2556 | 0.0076 | 1.4285 | 1.7096 |
| csharp-wasi | linear-append | dense | 100000 | 0.2488 | 0.0064 | 1.3952 | 1.7253 |
| csharp-wasi | linear-append | dense | 1000000 | 0.2459 | 0.0067 | 1.4531 | 2.4246 |
| csharp-wasi | linear-append | wide | 1000 | 0.2400 | 0.0067 | 1.4070 | 1.6626 |
| csharp-wasi | linear-append | wide | 10000 | 0.2666 | 0.0074 | 1.4394 | 1.7466 |
| csharp-wasi | linear-append | wide | 100000 | 0.2472 | 0.0072 | 1.4223 | 2.9623 |
| rust-native | indexed-append | dense | 1000 | 0.0113 | 0.0000 | 0.0008 | 0.0142 |
| rust-native | indexed-append | dense | 10000 | 0.0124 | 0.0000 | 0.0008 | 0.0327 |
| rust-native | indexed-append | dense | 100000 | 0.0118 | 0.0000 | 0.0007 | 0.2090 |
| rust-native | indexed-append | dense | 1000000 | 0.0130 | 0.0000 | 0.0013 | 1.9699 |
| rust-native | indexed-append | wide | 1000 | 0.0131 | 0.0000 | 0.0009 | 0.0162 |
| rust-native | indexed-append | wide | 10000 | 0.0138 | 0.0000 | 0.0011 | 0.0377 |
| rust-native | indexed-append | wide | 100000 | 0.0141 | 0.0000 | 0.0042 | 0.2444 |
| rust-native | indexed-append | wide | 1000000 | 0.0135 | 0.0000 | 0.0328 | 2.0821 |
| rust-native | indexed-copy | dense | 1000 | 0.0097 | 0.0000 | 0.0007 | 0.0123 |
| rust-native | indexed-copy | dense | 10000 | 0.0099 | 0.0000 | 0.0007 | 0.0297 |
| rust-native | indexed-copy | dense | 100000 | 0.0124 | 0.0000 | 0.0011 | 0.2058 |
| rust-native | indexed-copy | dense | 1000000 | 0.0131 | 0.0000 | 0.0012 | 2.5956 |
| rust-native | indexed-copy | wide | 1000 | 0.0100 | 0.0000 | 0.0007 | 0.0127 |
| rust-native | indexed-copy | wide | 10000 | 0.0096 | 0.0000 | 0.0009 | 0.0285 |
| rust-native | indexed-copy | wide | 100000 | 0.0092 | 0.0000 | 0.0027 | 0.1968 |
| rust-native | indexed-copy | wide | 1000000 | 0.0104 | 0.0001 | 0.0300 | 1.9204 |
| rust-native | linear-append | dense | 1000 | 0.0095 | 0.0000 | 0.0006 | 0.0119 |
| rust-native | linear-append | dense | 10000 | 0.0100 | 0.0000 | 0.0007 | 0.0283 |
| rust-native | linear-append | dense | 100000 | 0.0100 | 0.0000 | 0.0007 | 0.1858 |
| rust-native | linear-append | dense | 1000000 | 0.0124 | 0.0000 | 0.0010 | 1.9649 |
| rust-native | linear-append | wide | 1000 | 0.0117 | 0.0000 | 0.0008 | 0.0144 |
| rust-native | linear-append | wide | 10000 | 0.0111 | 0.0000 | 0.0009 | 0.0313 |
| rust-native | linear-append | wide | 100000 | 0.0107 | 0.0000 | 0.0026 | 0.2438 |
| rust-native | linear-append | wide | 1000000 | 0.0102 | 0.0000 | 0.0271 | 12.1347 |
| rust-wasi | indexed-append | dense | 1000 | 0.0668 | 0.0000 | 0.0028 | 0.0728 |
| rust-wasi | indexed-append | dense | 10000 | 0.0279 | 0.0000 | 0.0023 | 0.0603 |
| rust-wasi | indexed-append | dense | 100000 | 0.0286 | 0.0000 | 0.0023 | 0.3273 |
| rust-wasi | indexed-append | dense | 1000000 | 0.0264 | 0.0000 | 0.0024 | 3.0563 |
| rust-wasi | indexed-append | wide | 1000 | 0.0301 | 0.0000 | 0.0024 | 0.0356 |
| rust-wasi | indexed-append | wide | 10000 | 0.0273 | 0.0000 | 0.0025 | 0.0594 |
| rust-wasi | indexed-append | wide | 100000 | 0.0280 | 0.0000 | 0.0034 | 0.3422 |
| rust-wasi | indexed-append | wide | 1000000 | 0.0284 | 0.0000 | 0.0282 | 3.4744 |
| rust-wasi | indexed-copy | dense | 1000 | 0.0356 | 0.0000 | 0.0022 | 0.0416 |
| rust-wasi | indexed-copy | dense | 10000 | 0.0328 | 0.0000 | 0.0031 | 0.0711 |
| rust-wasi | indexed-copy | dense | 100000 | 0.0397 | 0.0000 | 0.0029 | 0.3953 |
| rust-wasi | indexed-copy | dense | 1000000 | 0.0457 | 0.0000 | 0.0115 | 4.4988 |
| rust-wasi | indexed-copy | wide | 1000 | 0.0353 | 0.0000 | 0.0028 | 0.0418 |
| rust-wasi | indexed-copy | wide | 10000 | 0.0303 | 0.0001 | 0.0025 | 0.0641 |
| rust-wasi | indexed-copy | wide | 100000 | 0.0312 | 0.0000 | 0.0045 | 0.4052 |
| rust-wasi | indexed-copy | wide | 1000000 | 0.0352 | 0.0000 | 0.0404 | 4.1399 |
| rust-wasi | linear-append | dense | 1000 | 0.0337 | 0.0000 | 0.0030 | 0.0407 |
| rust-wasi | linear-append | dense | 10000 | 0.0452 | 0.0002 | 0.0046 | 0.0943 |
| rust-wasi | linear-append | dense | 100000 | 0.0420 | 0.0000 | 0.0039 | 0.4559 |
| rust-wasi | linear-append | dense | 1000000 | 0.0414 | 0.0001 | 0.0098 | 4.1176 |
| rust-wasi | linear-append | wide | 1000 | 0.0528 | 0.0000 | 0.0036 | 0.0611 |
| rust-wasi | linear-append | wide | 10000 | 0.0440 | 0.0000 | 0.0029 | 0.0853 |
| rust-wasi | linear-append | wide | 100000 | 0.0344 | 0.0000 | 0.0036 | 0.4950 |
| rust-wasi | linear-append | wide | 1000000 | 0.1389 | 0.0000 | 0.0698 | 20.6463 |
