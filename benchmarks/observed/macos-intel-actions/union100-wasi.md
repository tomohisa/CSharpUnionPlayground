| implementation | batch s | encode s | decode s | switch s | JSON MiB | RSS MiB |
|---|---:|---:|---:|---:|---:|---:|
| [csharp-abstract-wasi](code/wasi/json/csharp-abstract/Program.cs) | 18.3505 | 2.4854 | 12.3320 | 3.2840 | 82.59 | 2115.9 |
| [csharp-closed-explicit-wasi](code/wasi/json/csharp-closed-explicit/Program.cs) | 15.6843 | 2.1188 | 10.8490 | 2.8379 | 82.59 | 2137.7 |
| [csharp-closed-inferred-wasi](code/wasi/json/csharp-closed-inferred/Program.cs) | 15.3234 | 2.2110 | 10.0861 | 2.9412 | 82.59 | 2170.8 |
| [csharp-union-custom-wasi](code/wasi/json/csharp-union-custom/Program.cs) | 30.3576 | 2.8108 | 24.0843 | 3.2245 | 87.74 | 2374.4 |
| [csharp-union-generated-wasi](code/wasi/json/csharp-union-generated/Program.cs) | 30.1255 | 7.4230 | 20.0273 | 2.8455 | 87.74 | 2208.4 |
| [rust-wasi](code/wasi/json/rust/main.rs) | 1.7618 | 0.4414 | 1.2896 | 0.0295 | 82.59 | 902.6 |

| implementation | startup s | preparation s | exit s | wall s |
|---|---:|---:|---:|---:|
| [csharp-abstract-wasi](code/wasi/json/csharp-abstract/Program.cs) | 9.3747 | 4.9437 | 2.1848 | 110.497 |
| [csharp-closed-explicit-wasi](code/wasi/json/csharp-closed-explicit/Program.cs) | 10.2552 | 4.9935 | 1.7348 | 101.546 |
| [csharp-closed-inferred-wasi](code/wasi/json/csharp-closed-inferred/Program.cs) | 7.3119 | 4.2957 | 2.3484 | 94.765 |
| [csharp-union-custom-wasi](code/wasi/json/csharp-union-custom/Program.cs) | 10.2821 | 5.2954 | 1.8730 | 168.833 |
| [csharp-union-generated-wasi](code/wasi/json/csharp-union-generated/Program.cs) | 9.5143 | 4.8279 | 1.8150 | 181.655 |
| [rust-wasi](code/wasi/json/rust/main.rs) | 0.1659 | 0.3182 | 0.1309 | 10.679 |

WASM-only measurements. Native results belong to the separate host report; combine only when job/attempt metadata matches. C# WASM uses .NET 11 AOT even when the Native abstract implementation uses .NET 10 JIT.
Native and WASI use the same generated Program.cs/main.rs and payloads, warm-up, validation and timed phases.
C# native is Native AOT Speed/native ISA; C# WASI is LLVMOnly AOT with SIMD and clang -O3, no interpreter fallback. Rust uses optimized release/LTO with host native ISA or Wasm SIMD.
WASI peak RSS includes Wasmtime compilation/runtime and is not guest heap usage. Startup includes Wasmtime compilation.
C# union wire envelopes differ from polymorphic record/Rust formats, as in the existing benchmark.
