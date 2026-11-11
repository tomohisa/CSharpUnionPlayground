| implementation | batch s | encode s | decode s | switch s | JSON MiB | RSS MiB |
|---|---:|---:|---:|---:|---:|---:|
| [csharp-abstract-wasi](code/wasi/json/csharp-abstract/Program.cs) | 14.1336 | 1.9402 | 9.4356 | 2.7486 | 82.59 | 2249.7 |
| [csharp-closed-explicit-wasi](code/wasi/json/csharp-closed-explicit/Program.cs) | 14.4274 | 2.0095 | 9.6496 | 2.7670 | 82.59 | 2165.3 |
| [csharp-closed-inferred-wasi](code/wasi/json/csharp-closed-inferred/Program.cs) | 15.3395 | 2.0958 | 10.0470 | 2.8241 | 82.59 | 2168.6 |
| [csharp-union-custom-wasi](code/wasi/json/csharp-union-custom/Program.cs) | 25.8047 | 2.6213 | 20.1137 | 2.8622 | 87.74 | 2543.0 |
| [csharp-union-generated-wasi](code/wasi/json/csharp-union-generated/Program.cs) | 30.8891 | 7.4943 | 20.4937 | 2.9213 | 87.74 | 2598.8 |
| [rust-wasi](code/wasi/json/rust/main.rs) | 1.7851 | 0.4560 | 1.3084 | 0.0289 | 82.59 | 902.6 |

| implementation | startup s | preparation s | exit s | wall s |
|---|---:|---:|---:|---:|
| [csharp-abstract-wasi](code/wasi/json/csharp-abstract/Program.cs) | 6.8679 | 4.0031 | 1.8173 | 86.006 |
| [csharp-closed-explicit-wasi](code/wasi/json/csharp-closed-explicit/Program.cs) | 6.9494 | 4.0248 | 1.7918 | 87.238 |
| [csharp-closed-inferred-wasi](code/wasi/json/csharp-closed-inferred/Program.cs) | 6.3501 | 4.0418 | 1.6298 | 91.977 |
| [csharp-union-custom-wasi](code/wasi/json/csharp-union-custom/Program.cs) | 6.6681 | 4.2452 | 2.0019 | 146.754 |
| [csharp-union-generated-wasi](code/wasi/json/csharp-union-generated/Program.cs) | 6.8867 | 4.2584 | 2.2496 | 176.735 |
| [rust-wasi](code/wasi/json/rust/main.rs) | 0.2059 | 0.4113 | 0.1444 | 11.061 |

WASM-only measurements. Native results belong to the separate host report; combine only when job/attempt metadata matches. C# WASM uses .NET 11 AOT even when the Native abstract implementation uses .NET 10 JIT.
Native and WASI use the same generated Program.cs/main.rs and payloads, warm-up, validation and timed phases.
C# native is Native AOT Speed/native ISA; C# WASI is LLVMOnly AOT with SIMD and clang -O3, no interpreter fallback. Rust uses optimized release/LTO with host native ISA or Wasm SIMD.
WASI peak RSS includes Wasmtime compilation/runtime and is not guest heap usage. Startup includes Wasmtime compilation.
C# union wire envelopes differ from polymorphic record/Rust formats, as in the existing benchmark.
