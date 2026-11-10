| implementation | batch s | encode s | decode s | switch s | JSON MiB | RSS MiB |
|---|---:|---:|---:|---:|---:|---:|
| csharp-abstract-native | 4.7589 | 0.7311 | 3.4089 | 0.2441 | 82.59 | 2151.7 |
| csharp-closed-explicit-native | 4.4899 | 0.7337 | 3.1619 | 0.2125 | 82.59 | 2151.4 |
| csharp-closed-inferred-native | 4.0226 | 0.6132 | 2.9763 | 0.2099 | 82.59 | 2150.2 |
| csharp-union-custom-native | 8.1808 | 0.6495 | 7.1770 | 0.2336 | 87.74 | 2019.9 |
| csharp-union-generated-native | 8.6043 | 0.9981 | 7.0225 | 0.2323 | 87.74 | 2158.7 |
| csharp-abstract-wasi | 23.3366 | 2.9874 | 15.0948 | 4.1217 | 82.59 | 2211.7 |
| csharp-closed-explicit-wasi | 19.7822 | 2.8091 | 13.4053 | 3.4687 | 82.59 | 2154.3 |
| csharp-closed-inferred-wasi | 20.2053 | 2.7101 | 13.7816 | 3.6947 | 82.59 | 2203.2 |
| csharp-union-custom-wasi | 35.6167 | 3.4042 | 28.7684 | 3.9542 | 87.74 | 2396.3 |
| csharp-union-generated-wasi | 44.1197 | 10.9664 | 29.7183 | 4.2553 | 87.74 | 2469.9 |
| rust-native | 2.3636 | 0.3276 | 1.9966 | 0.0348 | 82.59 | 1192.8 |
| rust-wasi | 2.2050 | 0.5616 | 1.6095 | 0.0327 | 82.59 | 902.9 |

| implementation | startup s | preparation s | exit s | wall s |
|---|---:|---:|---:|---:|
| csharp-abstract-native | 0.0538 | 1.5470 | 0.1719 | 26.854 |
| csharp-closed-explicit-native | 0.0421 | 1.4176 | 0.1687 | 24.105 |
| csharp-closed-inferred-native | 0.0293 | 1.2308 | 0.1874 | 22.201 |
| csharp-union-custom-native | 0.0333 | 1.2258 | 0.2039 | 41.690 |
| csharp-union-generated-native | 0.0324 | 1.2617 | 0.1351 | 44.813 |
| csharp-abstract-wasi | 11.5529 | 6.2231 | 2.4845 | 136.824 |
| csharp-closed-explicit-wasi | 12.3964 | 6.0313 | 2.6444 | 123.785 |
| csharp-closed-inferred-wasi | 9.6114 | 4.9902 | 2.5180 | 118.835 |
| csharp-union-custom-wasi | 10.2173 | 5.5587 | 2.6117 | 201.699 |
| csharp-union-generated-wasi | 12.5787 | 6.6729 | 3.4452 | 259.047 |
| rust-native | 0.0136 | 0.9217 | 0.5058 | 17.696 |
| rust-wasi | 0.2575 | 0.3921 | 0.1562 | 14.154 |

Native and WASI use the same generated Program.cs/main.rs and payloads, warm-up, validation and timed phases.
C# native is Native AOT Speed/native ISA; C# WASI is LLVMOnly AOT with SIMD and clang -O3, no interpreter fallback. Rust uses optimized release/LTO with host native ISA or Wasm SIMD.
WASI peak RSS includes Wasmtime compilation/runtime and is not guest heap usage. Startup includes Wasmtime compilation.
C# union wire envelopes differ from polymorphic record/Rust formats, as in the existing benchmark.
