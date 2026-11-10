| implementation | batch s | encode s | decode s | switch s | JSON MiB | RSS MiB |
|---|---:|---:|---:|---:|---:|---:|
| [csharp-abstract-native](code/wasi/json/csharp-abstract/Program.cs) | 5.4782 | 0.9014 | 3.9746 | 0.2541 | 82.59 | 2159.6 |
| [csharp-closed-explicit-native](code/wasi/json/csharp-closed-explicit/Program.cs) | 5.1850 | 0.7896 | 3.9793 | 0.2548 | 82.59 | 2153.4 |
| [csharp-closed-inferred-native](code/wasi/json/csharp-closed-inferred/Program.cs) | 5.4088 | 0.7772 | 4.1071 | 0.2658 | 82.59 | 2153.4 |
| [csharp-union-custom-native](code/wasi/json/csharp-union-custom/Program.cs) | 8.7118 | 0.6297 | 7.5614 | 0.2151 | 87.74 | 2220.4 |
| [csharp-union-generated-native](code/wasi/json/csharp-union-generated/Program.cs) | 10.0446 | 1.2950 | 8.6835 | 0.2457 | 87.74 | 2181.5 |
| [csharp-abstract-wasi](code/wasi/json/csharp-abstract/Program.cs) | 25.8395 | 3.7001 | 17.5894 | 4.6672 | 82.59 | 2152.4 |
| [csharp-closed-explicit-wasi](code/wasi/json/csharp-closed-explicit/Program.cs) | 25.9352 | 3.0779 | 17.3763 | 4.7848 | 82.59 | 2148.8 |
| [csharp-closed-inferred-wasi](code/wasi/json/csharp-closed-inferred/Program.cs) | 21.4608 | 3.5034 | 13.8141 | 3.6782 | 82.59 | 2116.0 |
| [csharp-union-custom-wasi](code/wasi/json/csharp-union-custom/Program.cs) | 41.1980 | 4.0011 | 32.8157 | 4.8411 | 87.74 | 2367.6 |
| [csharp-union-generated-wasi](code/wasi/json/csharp-union-generated/Program.cs) | 56.7815 | 12.8219 | 37.8872 | 4.3354 | 87.74 | 2244.7 |
| [rust-native](code/wasi/json/rust/main.rs) | 3.1166 | 0.4347 | 2.5921 | 0.0417 | 82.59 | 1210.7 |
| [rust-wasi](code/wasi/json/rust/main.rs) | 2.5655 | 0.6681 | 1.8844 | 0.0360 | 82.59 | 902.7 |

| implementation | startup s | preparation s | exit s | wall s |
|---|---:|---:|---:|---:|
| [csharp-abstract-native](code/wasi/json/csharp-abstract/Program.cs) | 0.0504 | 1.4889 | 0.2751 | 37.073 |
| [csharp-closed-explicit-native](code/wasi/json/csharp-closed-explicit/Program.cs) | 0.0492 | 1.7691 | 0.1540 | 28.053 |
| [csharp-closed-inferred-native](code/wasi/json/csharp-closed-inferred/Program.cs) | 0.0716 | 1.4280 | 0.2012 | 29.260 |
| [csharp-union-custom-native](code/wasi/json/csharp-union-custom/Program.cs) | 0.0768 | 1.5317 | 0.1501 | 52.537 |
| [csharp-union-generated-native](code/wasi/json/csharp-union-generated/Program.cs) | 0.0763 | 1.5706 | 0.1562 | 54.008 |
| [csharp-abstract-wasi](code/wasi/json/csharp-abstract/Program.cs) | 14.1132 | 7.7604 | 3.1284 | 158.298 |
| [csharp-closed-explicit-wasi](code/wasi/json/csharp-closed-explicit/Program.cs) | 18.9308 | 8.0259 | 3.1736 | 159.311 |
| [csharp-closed-inferred-wasi](code/wasi/json/csharp-closed-inferred/Program.cs) | 13.0935 | 7.1203 | 2.1723 | 134.227 |
| [csharp-union-custom-wasi](code/wasi/json/csharp-union-custom/Program.cs) | 9.5112 | 6.1845 | 3.0798 | 227.125 |
| [csharp-union-generated-wasi](code/wasi/json/csharp-union-generated/Program.cs) | 14.5925 | 7.2521 | 3.7452 | 314.225 |
| [rust-native](code/wasi/json/rust/main.rs) | 0.0550 | 0.9784 | 0.5449 | 19.984 |
| [rust-wasi](code/wasi/json/rust/main.rs) | 0.3122 | 0.4368 | 0.1660 | 15.963 |

Native and WASI use the same generated Program.cs/main.rs and payloads, warm-up, validation and timed phases.
C# native is Native AOT Speed/native ISA; C# WASI is LLVMOnly AOT with SIMD and clang -O3, no interpreter fallback. Rust uses optimized release/LTO with host native ISA or Wasm SIMD.
WASI peak RSS includes Wasmtime compilation/runtime and is not guest heap usage. Startup includes Wasmtime compilation.
C# union wire envelopes differ from polymorphic record/Rust formats, as in the existing benchmark.
