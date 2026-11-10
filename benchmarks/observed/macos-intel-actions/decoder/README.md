# Intel JSON deserialization diagnostic

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz; RAM 14 GiB. [Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42787344724). Source commit: `20f58b03a9a1135e3724314bf432cfaea6073ba5`.

200,000 roots / 600,000 nodes per batch; 5 large batches. Times are decode medians only. The same executable selects metadata or direct decoding; writer, model, nesting and validation are shared.

| implementation | Native decode s | WASI AOT decode s | Native allocated MiB | WASI allocated MiB |
|---|---:|---:|---:|---:|
| [STJ generated metadata](code/positional.cs) | 2.779 | 9.912 | 440.2 | 300.1 |
| [Parameterless JsonConstructor / init](code/parameterless.cs) | 1.936 | 12.591 | 362.4 | 254.3 |
| [Schema-specific direct reader](code/positional.cs) | 1.590 | 3.016 | 362.4 | 254.3 |
| [Rust / serde_json](code/rust.rs) | 1.552 | 1.406 | not measured | not measured |

[Raw samples, allocation, RSS, startup/preparation/exit, source hashes and AOT proofs](report.json).

Allocation is cumulative current-thread decode allocation, not live heap or GC time. WASI is 32-bit; Native is 64-bit. RSS includes Wasmtime compilation/runtime. These are sequential hosted-VM measurements; no confidence interval or universal ranking is claimed.

The parameterless direct-reader control is also retained in the raw report. The direct reader is benchmark-schema-specific; it does not reproduce every JsonSerializer contract/option.

[Method, implementation and reproduction commands](../../../union100/diagnostics/README.md).
