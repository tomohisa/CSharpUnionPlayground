# Intelでの一括比較

一覧の比較には `combined` を使います（Actionsの既定値）。全言語のNative JSON / Projectionと、対応するC#・RustのWASMを**同じジョブ・同じVMで順次測定**します。Nativeは各実装1回だけ測定し、別ジョブの値や最速値を混ぜません。

```bash
gh workflow run benchmarks-intel.yml -f suite=combined -f json_roots=200000 -F publish=true
```

| 対象 | 実行設定 |
|---|---|
| C# abstract Native JSON | .NET 10 SDK 10.0.401 / runtime 10.0.12、Release JIT＋Dynamic PGO |
| C# closed / union Native JSON | .NET 11正式版、Release JIT＋Dynamic PGO |
| C# Native Projection | .NET 11正式版、Release JIT＋Dynamic PGO |
| C# WASM | .NET 11 LLVMOnly AOT / SIMD / clang -O3（インタープリターなし） |
| 他言語のNative | 既存のRelease／ホスト設定 |
| Rust WASM | Release / LTO / SIMD |

C# JITのアプリはILで発行し、標準フレームワークのReadyToRunは維持します。`DOTNET_TieredCompilation=1`、`DOTNET_TieredPGO=1`、`DOTNET_TC_QuickJit=1`、`DOTNET_TC_QuickJitForLoops=1` をコマンドごとに設定します。型・シリアライザーは変更せず、専用readerは使いません。従来どおり1000ルートのウォームアップと5つの大きいバッチを使います。PGO有効化は定常性能への収束を保証しないため、生データには初回を含む全バッチを残します。

Native abstractとWASM abstractは同じProgram.csですが、.NET 10 JITと.NET 11 AOTの比較です。バックエンドだけの差を測っているとは解釈しません。

`union100.md` は全言語Nativeの正規結果、`union100-wasi.md` はWASMのみです。一覧のNative行は前者をそのまま使います。JSON12 Native＋6 WASM、Projection7 Native＋2 WASMを1つの実行として公開します。各environmentに共通の `comparisonGroup`（run URL＋attempt）を保存し、入力件数・ケースの欠落・Native/WASMソースハッシュ・成果物一式を検証してからまとめて反映します。別ジョブの結果しかない場合は、WASMを同じ順位表に混ぜず詳細へのリンクを残します。

同じVMでも測定時点の負荷やGCによる変動はあります。プロセスを同時に走らせないことで、ベンチマーク同士のCPU・メモリ競合を避けます。スイート間のActions実行も直列化します。

## 個別測定と過去の方式

以下は個別診断用で、一括比較とは区別します。過去の観測値を新しい実行方式へ読み替えません。

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

公式WASI SDK 33、Wasmtime 41.0.1、専用.NET 11 GA＋`wasi-experimental`、Rustの`wasm32-wasip1`を導入します。WASI SDK / WasmtimeのIntel配布物は公式リリースのSHA256で検証します。C#の5方式とRustのJSON往復、およびC# / RustのProjectionを実行します。ソース・入力・回数は各Native/WASIペアで同じです。

Projectionは既存と同じサイズ、fanout/domain配送、モード、予算です。JSONは両バックエンドとも20万ルート×5回で、既存Intelと同じ件数です。C#の処理本体をLLVMOnly AOTでWasmへコンパイルし、SIMD・clang -O3を有効にします。通常実行もNative AOT Speed/ホスト命令セット、Rustは両方LTOとターゲット向け最適化です。`wasi_json_roots`で増やせます。1プロセスのJSONタイムアウトは30分、ジョブ上限は180分です。タイムアウトを完了時間として扱いません。

結果は`union100-wasi.md` / `.json`、`projection-wasi-fanout.md` / `.json`、`projection-wasi-domain.md` / `.json`、`environment-wasi.json`です。通常のJSON・Projection結果を保持して追加します。ホストのC#はNative AOT、WASIはLLVMOnly AOTであり、WASI RSSはWasmtimeのコンパイル・ランタイムも含みます。

Individual suite runs may use different VMs. Workflow runs are serialized, and only a completed combined run supplies a matched overview. Projection event handlers remain sequential.

表示名は`ts-simple`（検証なし）、`ts-manual`（手書き検証）、`ts-zod`（Zod）です。CLIと生JSONの実装IDは互換性のため従来名を保持します。結果表の実装名から`code/<suite>/`の生成済み実ソースへ移動できます。各計測でコンパイラー／Nodeへ渡したファイルを保存し、コミットとSHA256を`manifest.json`に記録します。C# WASIはAOTの入力ソースSHA256との一致も確認します。実行は元のCLIで行うと、依存・最適化フラグを再現できます。

結果一覧は実行環境ではなく、`JSON run` / `Projection run`の2区分です。各表のexecution列でNative / WASM AOTを区別します。一覧は同一combinedジョブのNative正規結果とWASMだけを組み合わせます。異なる計測の最小値を選ぶことはしません。ビルド条件・計測元・コードへのリンクを各行に残します。
