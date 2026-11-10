# WASI JSONデシリアライズの切り分け

初回のARM測定では、C#の読み取りをスキーマ専用の `Utf8JsonReader` コードに置き換えると、この測定では **6.48秒 → 2.83秒（2.29倍）** に改善しました。Rust WASIは1.65秒なので、差は約3.92倍から約1.71倍に縮まりました。既存のSource Generatorの製品コードを変更した結果ではなく、改善できる範囲を確認するために追加した診断実装です。

## Intelでの再計測

[2026-11-10のIntel Actions実行](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42787344724)も完了しました。i7-8700B / RAM 14 GiB、同じ20万ルート・60万ノード×5バッチです。下のARMとは別の測定です。

| 読み取り | Native decode 秒 | WASI AOT decode 秒 | WASI decode割り当て MiB |
|---|---:|---:|---:|
| 標準STJ Source Generation | 2.779 | 9.912 | 300.1 |
| 引数なしJsonConstructor + init | 1.936 | 12.591 | 254.3 |
| スキーマ専用の直接reader | 1.590 | 3.016 | 254.3 |
| Rust serde_json | 1.552 | 1.406 | 未計測 |

同じ実行内のC# WASIは9.912秒から3.016秒、約3.29倍に改善しました。Rustとの差は約7.05倍から約2.15倍に縮まりました。引数なしコンストラクターだけではWASIで改善せず、割り当て量の削減だけで速さが決まらない点も再確認できました。

[Intelの全ソース・生データ・ビルド証跡・起動と終了の内訳](../../observed/macos-intel-actions/decoder/README.md)を別に保存しています。以前のIntel標準STJ 13.782秒から今回の直接reader 3.016秒への比率を改善率には使いません。比較の基準は同じ実行で測り直した9.912秒です。

GitHub Actionsの `Intel benchmarks` を `suite=decoder` で実行すると、同じ結果ページへ追記更新します。ほかのJSON / Projectionの計測結果は保持します。

## 条件と実測

2026-11-10、Apple M4 Max / arm64 / RAM 128 GiB。**Intel Actionsの結果とは別の測定**です。20万ルート、深さ3、100ケース、1バッチ60万ノード、5バッチ。JSONは各バッチ86,602,883 bytes（82.59 MiB）。小さいオブジェクトを何千回も反復する計測ではありません。

C#は.NET 11 RC3。NativeはNative AOT・Speed・native ISA、WASIはMono **LLVMOnly AOT・SIMD・clang -O3**。インタープリターへのフォールバックなしを[ビルド証跡付きの生データ](observed/macos-arm64.json)に記録しました。Rustはrelease・fat LTO・codegen-units=1、Nativeはnative ISA、WasmはSIMD。Wasmtimeはopt-level=2。

表の時間はデシリアライズだけの5バッチ中央値です。エンコード、switch、起動、準備、終了時間、全サンプル、RSSは生データに分離して保存しています。

| 読み取り実装 | Native decode 秒 | WASI decode 秒 | Native decode割り当て MiB | WASI decode割り当て MiB |
|---|---:|---:|---:|---:|
| [closed + STJ Source Generation（既存方式）](observed/code/positional.cs) | 0.813 | 6.481 | 440.2 | 300.1 |
| [引数なしJsonConstructor + init setters](observed/code/parameterless.cs) | 0.843 | 7.236 | 362.4 | 254.3 |
| [同じレコードを直接構築するreader](reader.cs) / [計測した全ソース](observed/code/positional.cs) | 0.694 | 2.833 | 362.4 | 254.3 |
| [Rust serde_json + derive](observed/code/rust.rs) | 0.753 | 1.655 | 未計測 | 未計測 |

直接readerと標準STJは**同じ実行ファイルの引数**で切り替えます。writer・100ケースのレコード・配列・辞書・文字列・ネスト・switch・検証は共通です。全バッチで独立したPythonの期待チェックサム・件数と比較し、最初のバッチでは全フィールドの再シリアライズが元JSONとバイト単位で一致することも確認します。ウォームアップでも直接readerによる往復を確認します。

`DecodeAllocatedBytes` は同期decode中の `GC.GetAllocatedBytesForCurrentThread()` の差分です。累積割り当て量であり、保持ヒープ・最大使用量・GC時間ではありません。WASIの32-bitとNativeの64-bitではオブジェクトのサイズも異なります。Rustの割り当ては測っていないため、ゼロとして比較してはいけません。

直接readerのプロセス全体のpeak RSSは、Native 1964.9 MiB、WASI 1919.9 MiB。標準STJはNative 2309.3 MiB、WASI 1919.6 MiBでした。WASIではRSS削減は確認できません。RSSはWasmtimeのコンパイル・ランタイムやエンコード用バッファ、元データなども含み、decode用のguest heapとは異なります。

## 確認できた原因と、まだ分からないこと

1. **AOTと、型専用の読み取りコード生成は別です。** STJのSource Generationは読み取り側でメタデータを使います。Microsoftの[公式説明](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/reflection-vs-source-generation)にも、fast-path deserializationは未提供とあります。[対応issue](https://github.com/dotnet/runtime/issues/55043)も参照してください。対してSerdeのderiveは[Deserialize traitの実装を生成](https://serde.rs/derive.html)します。どちらも「コード生成」ですが、生成するものが同じではありません。
2. **このモデルでは、汎用バインド処理を省く効果が大きいです。** 直接readerはプロパティ名をUTF-8のまま比較し、Payloadとケースを直接構築します。STJの型別メタデータ、プロパティ用コンバーター、コンストラクター引数の仲介などを通りません。WASIの時間が約56%、割り当てが約15%減りました。これはこれらをまとめて置き換えた効果であり、型判別だけのコストを測ったものではありません。
3. **割り当て量だけでは説明できません。** 引数なしJsonConstructor版は直接readerとほぼ同じ割り当て量まで減りますが、WASIで速くなりませんでした。この変更単独は推奨しません。後半の直接reader測定にも3.03〜4.25秒のばらつきがあり、わずかな差のランキングには使えません。標準の位置パラメーター版直接readerの範囲は2.80〜3.02秒です。
4. **既存unionコンバーターには追加コストがあります。** [手書き版](../templates/converter.cs)と[生成器](../../../src/PlaygroundTaggedJson.Generator/TaggedJsonGenerator.cs)は `JsonDocument.ParseValue` → ケースの `JsonSerializer.Deserialize` を使います。ネストしたサブツリーをDOM化してから読み直します。今回の主比較はDOMを使わないclosed版なので、上記6.48秒の原因をDOMのせいにはできません。Intelのunion版約29秒にも、今回の改善率をそのまま掛けることはできません。unionはJSONのdataラッパーも1段多く、別の条件です。
5. **残る約1.7倍は未分解です。** 直接readerにもUTF-8解析、文字列・配列・辞書の割り当て、GCが残ります。C# Native AOTとMono WASI AOTではランタイム・コード生成が異なり、Rustともメモリ表現が違います。しかし、この測定だけでは残差を「GCが何秒」「SIMD不足が何秒」などと断定できません。CPUサンプリングやGC計測をしていないため、これらは候補です。Rust側の型判別が全くバッファリングしない、という前提も置いていません。

## 改善の方向

通常の使いやすさを優先するなら、既存の `closed + JsonPolymorphic` が出発点です。フラグはすでに最適化済みなので、追加のAOT指定だけで大差が消えるという結果ではありませんでした。

読み取り性能が重要なら、**unionの外側だけでなくPayloadも含めた直接readerを生成する**方向が有望です。[今回のreader](reader.cs)がその実測用実装です。union用の生成器を改善する場合は、まずDOM経由をやめる方法も検討できますが、今回そこは変更・測定していません。

このreaderはベンチマーク専用スキーマに対応する診断コードです。通常プロパティの順序変更・エスケープされた名前・未知プロパティのskipを扱いますが、STJの全設定やnull/missing/duplicateの扱いを完全再現する汎用コンバーターではありません。例えばPayloadのlabel/values/tagsは非nullを要求します。したがって、既存の一般向け生成器へそのまま適用していません。実用化では、属性・nullable・必須項目・数値・重複プロパティ・深さ制限などの契約を定義して検証する必要があります。

## 再実行

[CLI](run.py)は既存の公式SDK、WASI SDK、Wasmtime、Rust toolchainを使います。WASIツールの準備は[既存の手順](../README.md)と同じです。macOS上で実行します。

```bash
# 位置パラメーターのレコード：標準STJ vs 直接reader、Rustの対照も計測
python3 benchmarks/union100/diagnostics/run.py --with-rust

# コンバーター不要の候補も別に計測（結果ファイルも別）
python3 benchmarks/union100/diagnostics/run.py --parameterless

# ビルド済みの同じソースを再利用
python3 benchmarks/union100/diagnostics/run.py --skip-build
```

既定は20万ルート×5バッチ。`--roots 100 --samples 1` は動作確認だけです。`--wasi-dotnet` / `--wasi-sdk` でSDKの場所を指定できます。結果は `benchmarks/union100/results/decoder-diagnostic.json`、引数なし版は `decoder-setters.json` に保存します。ソースが変わった場合の `--skip-build` は拒否します。

[保存した生データ](observed/macos-arm64.json)には、実行したC#ソースとWasmのSHA256、AOT登録、コンパイル設定も含めています。[ソーススナップショット](observed/code)と照合可能です。これらは局所的な診断値であり、既存のIntel Actions結果の置き換えや、言語全体の優劣を意味しません。

保存結果の件数・チェックサム・JSONサイズ・ソースハッシュ・AOT証跡は、`python3 benchmarks/union100/diagnostics/verify_observed.py` で再確認できます。今回保存した10条件・計50バッチは全て成功しました。
