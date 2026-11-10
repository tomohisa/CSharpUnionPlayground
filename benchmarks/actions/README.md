# Intel GitHub Actions

[Intel benchmarks](../../.github/workflows/benchmarks-intel.yml) is a manually dispatched workflow on `macos-15-intel`. The driver verifies both x86_64 and the actual Intel CPU brand before measuring. It does not use a generic Linux x64 label, which would not guarantee Intel hardware.

From the Actions tab choose **Intel benchmarks → Run workflow**, or use:

```bash
# The projection comparison from the observed tables (all six languages)
gh workflow run benchmarks-intel.yml -f suite=projection -F publish=true

# Also run all ten JSON implementations (fixed input count for this smaller host)
gh workflow run benchmarks-intel.yml -f suite=all -f json_roots=200000 -F publish=true
```

Projection runs the fanout dense/wide sweep up to one million events and the domain/dense append/copy sweep up to ten million. Both use two samples, a 25s process timeout and an 8s projection-time stopping budget. A timeout in an intentionally slow mode is recorded as an observation. Unexpected runtime errors, unavailable host implementations and incorrect results fail the job and prevent publication.

The JSON suite uses five large batches and a configurable fixed root count (default 200,000); it does not calibrate to ten seconds on this 14GB runner. Set 729,000 explicitly to request the same root count as the ARM run, considering that peak memory and runtime may be considerably higher. Counts are saved with the results. WASI is outside this workflow; the existing ARM WASI results are retained separately.

Results are always uploaded as a 90-day artifact, including partial output on failure, and Markdown tables appear in the job summary. On a successful dispatch **from main** with `publish=true`, the second job commits the complete latest Intel snapshot to `benchmarks/observed/macos-intel-actions/` on main. Selecting a different suite replaces this snapshot with that suite's files. Prior snapshots remain in Git history. The publication job only stages this directory. ARM observations are never replaced. Repository branch rules may reject automatic publication; the artifact still contains the measured data.

The workflow has no scheduled trigger and no PR/push trigger. Publishing is a contents-write job separate from the read-only benchmark job. CPU brand, actual RAM, OS, source commit, workflow URL and attempt are retained in `environment.json`; per-suite JSON contains runtime/compiler versions and all measurements. Versions of the hosted image and stable Rust can change; record and compare the captured versions. Host VM and runtime differences mean this does not isolate the effect of CPU architecture alone.
