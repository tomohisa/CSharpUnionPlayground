# Level 3 live projection scaling

Local single-process observations; timings are not language rankings.
Updates exclude command handling, store reads, JSON and full-list queries. Each enrollment pair is one StudentRegistered plus one StudentEnrolled.
Each process also checks rejected-batch rollback, gaps, redelivery, old snapshots, and full-list correctness.

| language | existing students / roster | attendance updates (1,000), ms | add 1,000 students + enrollments, ms | full snapshot, ms | update work/event | growth work/pair |
|---|---:|---:|---:|---:|---:|---:|
| csharp | 1,000 | 0.869 | 2.580 | 3.741 | 3 | 8 |
| csharp | 10,000 | 0.830 | 2.645 | 23.810 | 3 | 8 |
| csharp | 100,000 | 0.124 | 7.725 | 40.465 | 3 | 8 |
| fsharp | 1,000 | 0.455 | 1.519 | 4.081 | 3 | 9 |
| fsharp | 10,000 | 0.449 | 1.609 | 8.938 | 3 | 9 |
| fsharp | 100,000 | 0.157 | 0.891 | 142.110 | 3 | 9 |
| rust | 1,000 | 0.311 | 1.151 | 0.507 | 3 | 8 |
| rust | 10,000 | 0.306 | 1.036 | 3.390 | 3 | 8 |
| rust | 100,000 | 0.320 | 1.283 | 49.662 | 3 | 8 |
| typescript | 1,000 | 0.687 | 2.099 | 5.196 | 3 | 8 |
| typescript | 10,000 | 0.428 | 2.343 | 21.633 | 3 | 8 |
| typescript | 100,000 | 0.449 | 2.651 | 213.942 | 3 | 8 |
| java | 1,000 | 1.239 | 3.827 | 5.347 | 3 | 8 |
| java | 10,000 | 2.071 | 2.769 | 21.441 | 3 | 8 |
| java | 100,000 | 1.978 | 1.320 | 115.756 | 3 | 8 |
| go | 1,000 | 0.268 | 1.527 | 1.106 | 3 | 8 |
| go | 10,000 | 0.290 | 1.235 | 6.031 | 3 | 8 |
| go | 100,000 | 0.312 | 1.467 | 67.278 | 3 | 8 |
| swift | 1,000 | 1.041 | 4.677 | 1.615 | 3 | 8 |
| swift | 10,000 | 1.052 | 4.243 | 9.283 | 3 | 8 |
| swift | 100,000 | 0.961 | 4.154 | 106.381 | 3 | 8 |

Hash table resizing, allocation, GC, caches and JIT can change latency. Bounded index operations are an algorithmic check, not a hard real-time guarantee. Full snapshots intentionally scale with output size.
See [raw data](projection-scaling.json) for samples, environment and source fingerprints.
