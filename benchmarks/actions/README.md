# Intel GitHub Actions

[Intel benchmarks](../../.github/workflows/benchmarks-intel.yml) is a manually dispatched workflow on `macos-15-intel`. The driver verifies both x86_64 and the actual Intel CPU brand before measuring. It does not use a generic Linux x64 label, which would not guarantee Intel hardware.

From the Actions tab choose **Intel benchmarks → Run workflow**, or use:

```bash
# The projection comparison from the observed tables (all six languages)
gh workflow run benchmarks-intel.yml -f suite=projection -F publish=true

# JSON only: all ten implementations; retain the previous projection results
gh workflow run benchmarks-intel.yml -f suite=json -f json_roots=200000 -F publish=true

# Run both suites (fixed JSON input count for this smaller host)
gh workflow run benchmarks-intel.yml -f suite=all -f json_roots=200000 -F publish=true
```

Projection includes simple TypeScript and TypeScript with Zod payload validation, alongside the other five languages. It runs the fanout dense/wide sweep up to one million events and the domain/dense append/copy sweep up to ten million. Both use two samples, a 25s process timeout and an 8s projection-time stopping budget. A timeout in an intentionally slow mode is recorded as an observation. Unexpected runtime errors, unavailable host implementations and incorrect results fail the job and prevent publication.

The JSON suite uses five large batches and a configurable fixed root count (default 200,000); it does not calibrate to ten seconds on this 14GB runner. Set 729,000 explicitly to request the same root count as the ARM run, considering that peak memory and runtime may be considerably higher. Counts are saved with the results. The host-only suites omit WASI; the dedicated wasi suite below retains paired AOT results separately.

Results are always uploaded as a 90-day artifact, including partial output on failure, and Markdown tables appear in the job summary. On a successful dispatch **from main** with `publish=true`, the second job commits the complete latest Intel snapshot to `benchmarks/observed/macos-intel-actions/` on main. Running one suite updates only its own reports and environment metadata; the other suite is retained. The index links both sets of results. Prior snapshots remain in Git history. The publication job only stages this directory. ARM observations are never replaced. Repository branch rules may reject automatic publication; the artifact still contains the measured data.

The workflow has no scheduled trigger and no PR/push trigger. Publishing is a contents-write job separate from the read-only benchmark job. CPU brand, actual RAM, OS, source commit, workflow URL and attempt are retained in `environment-projection.json` / `environment-json.json`; per-suite JSON contains runtime/compiler versions and all measurements. Versions of the hosted image and stable Rust can change; record and compare the captured versions. Host VM and runtime differences mean this does not isolate the effect of CPU architecture alone.

## C# / Rust: WASIとホストの同一ソース比較

```bash
gh workflow run benchmarks-intel.yml -f suite=wasi -f wasi_json_roots=200000 -F publish=true
```

公式WASI SDK 33、Wasmtime 41.0.1、専用.NET RC3＋`wasi-experimental`、Rustの`wasm32-wasip1`を導入します。WASI SDK / WasmtimeのIntel配布物は公式リリースのSHA256で検証します。C#の5方式とRustのJSON往復、およびC# / RustのProjectionを実行します。ソース・入力・回数は各Native/WASIペアで同じです。

Projectionは既存と同じサイズ、fanout/domain配送、モード、予算です。JSONは両バックエンドとも20万ルート×5回で、既存Intelと同じ件数です。C#の処理本体をLLVMOnly AOTでWasmへコンパイルし、SIMD・clang -O3を有効にします。通常実行もNative AOT Speed/ホスト命令セット、Rustは両方LTOとターゲット向け最適化です。`wasi_json_roots`で増やせます。1プロセスのJSONタイムアウトは30分、ジョブ上限は180分です。タイムアウトを完了時間として扱いません。

結果は`union100-wasi.md` / `.json`、`projection-wasi-fanout.md` / `.json`、`projection-wasi-domain.md` / `.json`、`environment-wasi.json`です。通常のJSON・Projection結果を保持して追加します。ホストのC#はNative AOT、WASIはLLVMOnly AOTであり、WASI RSSはWasmtimeのコンパイル・ランタイムも含みます。

Different suites may measure on separate Intel runner VMs concurrently. Same-suite runs and result publication are serialized; updates retain the other suites. Projection event handlers themselves remain sequential.

表示名は`ts-simple`（検証なし）、`ts-manual`（手書き検証）、`ts-zod`（Zod）です。CLIと生JSONの実装IDは互換性のため従来名を保持します。結果表の実装名から`code/<suite>/`の生成済み実ソースへ移動できます。各計測でコンパイラー／Nodeへ渡したファイルを保存し、コミットとSHA256を`manifest.json`に記録します。C# WASIはAOTの入力ソースSHA256との一致も確認します。実行は元のCLIで行うと、依存・最適化フラグを再現できます。

結果一覧は実行環境ではなく、`JSON run` / `Projection run`の2区分です。各表のexecution列でNative / WASM AOTを区別します。Nativeの重複を避けるため、C#とRustはWASMと同時計測した最適化Native対照値を採用し、以前のホスト単独計測は詳細ファイルに保持します。異なる計測の最小値を選ぶことはしません。ビルド条件・計測元・コードへのリンクを各行に残します。
