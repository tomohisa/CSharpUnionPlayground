# UNION 100ケース・大きなバッチのJSON比較ベンチマーク

C# 5方式、Rust、TypeScript 3方式、Java、Go、Swiftをコマンドラインで比較します。各実装に100種類のケース型を生成し、ネストした同じ論理データを使います。

**小さいJSONを何千回も繰り返すベンチマークではありません。** 大きな配列全体のencode → decode → switch/matchによる全走査を、1バッチとして数回だけ測定します。

## コマンド

リポジトリのルートから:

```bash
# 全12実装をビルドし、予備計測で件数を決めて本計測
python3 benchmarks/union100/run.py

# 同じビルドを再利用（ソース・RID・実装の一致を確認）
python3 benchmarks/union100/run.py --skip-build

# まず小規模で正当性と環境だけ確認する。性能比較には使用しない
python3 benchmarks/union100/run.py --quick

# 実装を絞る（指定した中の最速に合わせてデータ量を調整）
python3 benchmarks/union100/run.py \
  --implementations csharp-closed-inferred,csharp-union-generated,rust,swift

# 自動調整を使わず、大きなバッチ件数と回数を指定
python3 benchmarks/union100/run.py --skip-build --roots 200000 --samples 3
```

既定は5バッチ。最速の実装の計測合計が約10秒、1バッチが約2秒になる件数を予備計測から見積もります。`--min-batch-seconds` の既定は1秒で、target/samplesと比較して長い方を目標にします。目標は予測値なので実測は変動します。最速の実測時間も最後に表示します。

```bash
# 3バッチ、最速の計測合計は約12秒＝1バッチ約4秒を目標
python3 benchmarks/union100/run.py --skip-build --samples 3 --target-seconds 12
```

`--quick` は100件・1バッチで、通常の時間目標を意図的に使いません。`--roots` は固定件数で、自動調整と時間目標を使いません。どちらもCLIの動作確認や、別のホストで同じ件数を比較するときに使えます。

## モデルと計測単位

- 全言語で100種類（Case000〜Case099）。Rust/Swiftは100ケースのenum＋100種類のケース用struct、TypeScriptは100種類の型のunionです。
- 各ケースは共通構造のPayloadを持ちます。100種類のまったく異なるスキーマを想定した結果ではありません。
- Payloadには整数ID・文字列・4要素の数値配列・2要素の文字列辞書・次のケースが入ります。
- 1ルートは3段のネスト。Nルートならケースオブジェクトは3N個、さらにPayload・配列・辞書が存在します。
- 100ケースが均等に混在します。ルートごとにIDや文字列を変え、各ルートを個別に生成します。
- 計測中は配列全体を一度だけシリアライズし、その結果全体を一度だけデシリアライズし、復元した全ケースをswitch/matchで一度走査します。

各言語の単調時計で、encode・decode・switchの所要秒数と合計秒数を記録します。CLIでは中央値と計測合計、JSONのMiB、ピークRSSを表示し、JSONファイルには全サンプル・最小/最大・予備計測・実行環境・コマンド・ソース指紋を残します。

## メモリ・起動・終了の別計測

- **peak RSS MiB**: OSが報告するプロセス全体のピーク常駐メモリ。macOSでは `/usr/bin/time -l`、Linuxでは `/usr/bin/time -v` を使用します。元データ生成・ウォームアップ・区間外の検証も含む最大値であり、JSON処理中だけの割り当て量ではありません。言語間で同じRSS指標を使い、GCヒープサイズと混同しません。
- **startup**: 親がプロセスを起動してから、実装側の最初のコードのready通知を受信するまで。
- **preparation**: readyから、大きなモデル生成・ウォームアップ・期待値計算を終えたprepared通知まで。
- **exit**: 最終結果を受信してからOSのプロセス終了まで。
- **total wall**: 起動から終了までの全時間。準備・区間外の検証・バッチ間の解放なども含み、計測合計より長くなります。

速度・メモリの表と、起動・準備・終了の表を分けて表示します。親から観測した時刻には、計測用timeラッパーやパイプのスケジューリングの小さなオーバーヘッドが含まれます。CPU負荷の高いホストでは誤差が増えます。OS待機は別スレッドで行い、終了時刻にタイムアウト待機のポーリング遅延を加えない構成です。

完了した各実装の結果は `.progress` ディレクトリへ逐次保存します。長い計測が途中で失敗しても、完了分を失いません。タイムアウトやCtrl+Cでは実行中の計測プロセスグループを終了させます。

## 比較対象

| CLI名 | 実装 |
|---|---|
| csharp-abstract | 通常のabstract record＋JsonDerivedType |
| csharp-closed-explicit | closed record＋JsonDerivedType |
| csharp-closed-inferred | closed record＋InferClosedTypePolymorphism |
| csharp-union-custom | union＋生成した100分岐の手書き相当Converter |
| csharp-union-generated | union＋このリポジトリの実験版Source Generator |
| rust | enum＋Serde/serde_json、Release最適化 |
| typescript | Node.jsのJSON＋全ノードの再帰的な実行時検証＋switch |
| java | sealed interface＋Jackson＋switch、通常のJIT |
| go | interface＋標準encoding/json＋型判別dispatch＋type switch |
| swift | indirect enum＋標準Codable＋switch、swiftc -O |

C#はすべてRelease Native AOT、標準JSONソース生成、リフレクション無効です。JsonDerivedType一覧やcase型はPythonで生成するので、100型を手作業で管理する必要はありません。生成ソースは `.build/sources` に残ります。通常の短い利用例とは独立しており、既存サンプルは変更しません。

## 計測に含めないもの

ビルド、プロセス起動、元データの生成、最大1,000ルートのウォームアップ、正当性検証は計測区間の外です。各バッチで復元データを走査したチェックサムは必ず消費し、期待値と照合します。最初のバッチでは復元結果を再エンコードして全フィールドの一致も確認します。その再エンコードは計測時間に含めません。CLI側も全実装のチェックサムが同じであることを検査します。

計測結果は **今回のモデル・JSONライブラリ・実装の組み合わせ** の比較です。union自体の処理速度や、言語全体の性能順位とは解釈しないでください。

- JSON契約は完全には同じではありません。C# union版は追加のdataラッパー、Swiftは合成enumラッパーとnil省略があるため、バイト数を併記します。
- TypeScriptのdecodeには型検証を含めます。JSON.parseに型アサーションを付けるだけの処理とは区別します。
- Swiftは決定的なJSON比較のためsortedKeysを有効にしています。GoのJSONも辞書キーをソートします。
- Goとunion Converterは中間JSON表現を使います。これは現在の実装コストとして計測します。
- Java/NodeのJITの状態、GC/ARC、ホスト負荷、CPU温度は結果に影響します。1,000ルートのウォームアップが全実装の完全な定常状態を保証するわけではありません。
- バッチ間の解放や、区間外の検証を含むプロセス全体の時間は、各計測合計より長くなります。
- 外部サービスへのアクセスやネットワークは計測しません。

## 必要環境・生成物

このリポジトリの.NET 11 GA (SDK 11.0.100)、Rust/Cargo、Node.js 24以降、Java 21以降、Go、Swiftが必要です。初回ビルドではNuGet/Serde/Jacksonの依存取得が必要です。Jacksonは既存サンプルと同じ固定バージョンを使います。Rustの依存解決は同梱のCargo.lockを生成先へコピーして固定します。

macOS arm64で検証しています。他ホストでは必要なNative AOTコンパイラーとSwift/Foundation環境を用意し、必要に応じて `--rid` を指定してください。ビルド・生成物は `.build/`、結果は `results/` に保存され、Gitには含めません。

**大きなデータを一度に保持するため数GBのメモリが必要になり得ます。** Java/Nodeのヒープ上限は6GiB、ルートの自動調整上限は1,000,000件です。上限を超える予測なら、目標未達の小さな測定を黙って行わず終了します。メモリに余裕がある場合だけ `--max-roots` を増やしてください。実装ごとに別プロセスで順番に計測し、全言語を同時実行しません。

```bash
python3 benchmarks/union100/run.py --help
```

終了コード0は全実装のビルド・正当性検査・計測・結果保存が成功したことを表します。依存やコード、チェックサム、タイムアウトなどの問題では非0で終了します。

結果ファイルは既定で `benchmarks/union100/results/latest.json` と `latest.md`。`--output path/result.json` で変更できます。

## ランナーの検証

```bash
python3 benchmarks/union100/test_runner.py
```

既知の遅延を持つ専用プロセスで起動・準備・終了の境界とピークRSSを確認し、不正チェックサムの拒否とタイムアウト時の終了処理を検査します。

## このホストでの実測

[生データ](observed/macos-arm64.json) / [表](observed/macos-arm64.md) を保存しています。729,000ルート（2,187,000ケース）を5バッチ計測し、最速の中央値は約2.15秒/バッチ、最速の計測合計は約10.53秒でした。128GiBメモリのmacOS arm64ホストでの結果です。別ホストの性能や最大保持可能件数を保証するものではありません。

生データのbuildFingerprintは計測時のランナーを含む指紋です。計測後の変更は、消費済みstdoutパイプを明示的にcloseする後処理のみです。生成した各言語のモデル・計測コードは同じです。

Intelでの追加計測は[Actionsの観測データ](../observed/macos-intel-actions/)に保存します。JSONを含むsuiteで実行した場合は`union100.md` / `.json`があり、入力件数も記録します。既存のARM結果とはOS・ツール・件数などの条件も異なるため、環境情報を併せて比較してください。

## C# / Rustの最適化Native対WASI AOT

同じ生成済み`Program.cs` / `main.rs`、同じ入力件数・バッチ回数・検証を使って比較します。C#の5方式とRustが対象です。

```bash
python3 benchmarks/union100/run_wasi.py --quick
python3 benchmarks/union100/run_wasi.py --roots 200000 --samples 5
# SDKを別の場所へ導入した環境
python3 benchmarks/union100/run_wasi.py --roots 200000 --samples 5 \
  --wasi-dotnet /path/to/dotnet --wasi-sdk /path/to/wasi-sdk
```

WasmtimeとRustの`wasm32-wasip1`ターゲット、専用.NET 11 GA (SDK 11.0.100)の`wasi-experimental` workloadが必要です。既定のSDKパスはProjectionテストのローカル導入先です。C#の通常実行はNative AOTの`OptimizationPreference=Speed`とホスト命令セット、WASIは`RunAOTCompilation=true` / `AOTMode=LLVMOnly` / SIMD / トリミング / clang `-O3`です。Rustは両方opt-level=3・fat LTO・codegen-units=1で、通常実行はホスト命令セット、WasmはSIMDを有効にします。Wasmtimeは最適化レベル2です。

C#は処理本体もWasmへ事前コンパイルします。共通ビルダーはアプリのAOTモジュール登録、コンパイルフラグ、リンク成果物と実行WasmのSHA256一致を確認し、`build-proof.json`を作ります。結果の`buildProofs`に含めます。インタープリターのフォールバックを主比較に使いません。

RC3 SDKの互換対応として、トリミング済みDLLのみをAOT入力にし、HTTPを使わない場合だけ欠落したHTTP WIT参照を除外します。SDK本体やベンチマークの処理コードは変更しません。HTTPのDLLが必要な場合はエラーにします。

起動にはWasmtimeによるコンパイルも含まれ、RSSはWasmtime全体です。WASIの失敗・時間超過はJSONの`errors`に理由を記録し、成功した計測値として扱いません。20万ルート×5回が標準で、元のIntel JSON比較と同じ件数です。ARMの72.9万ルートの表とは件数が違います。

## ts-simple / ts-manual / ts-zod のコードの違い

3方式とも入力・JSON.stringify・switch・件数・回数は同じです。違うのは、JSONを読み込む`parse`の部分です。表示名とCLIの実装IDは次の対応です。

| 表示名 | CLI ID | decodeで行うこと | 計測に使った生成済みソース |
|---|---|---|---|
| ts-simple | `typescript-simple` | JSON.parseだけ。型アサーションであり、実行時の型検証なし | [全コード](../observed/macos-intel-actions/code/json/typescript-simple/benchmark-simple.ts) |
| ts-manual | `typescript` | JSON.parse＋手書きの再帰チェック | [全コード](../observed/macos-intel-actions/code/json/typescript/benchmark.ts) |
| ts-zod | `typescript-zod` | JSON.parse＋Zodの再帰スキーマ検証・結果オブジェクト生成 | [全コード](../observed/macos-intel-actions/code/json/typescript-zod/benchmark-zod.ts) |

### ts-simple

```typescript
function parse(json: string): Payment[] {
  return JSON.parse(json) as Payment[];
}
```

`as Payment[]`は型検証を行いません。不正な入力でも、この関数だけでは拒否しません。

### ts-manual

```typescript
function parse(json: string): Payment[] {
  const nodes: unknown = JSON.parse(json);
  function validate(p: any): void {
    if (!p || typeof p.$type !== "string" || !/^Case\d{3}$/.test(p.$type)
        || Number(p.$type.slice(4)) >= 100) throw Error("Invalid case");
    const d = p.data;
    if (!d || !Number.isInteger(d.id) || typeof d.label !== "string"
        || !Array.isArray(d.values) || !d.values.every(Number.isInteger)
        || !d.tags || Array.isArray(d.tags) || typeof d.tags !== "object"
        || !Object.values(d.tags).every(v => typeof v === "string"))
      throw Error("Invalid payload");
    if (d.next !== null) validate(d.next);
  }
  if (!Array.isArray(nodes)) throw Error("Invalid batch");
  nodes.forEach(validate);
  return nodes as Payment[];
}
```

読み込んだオブジェクトを再帰的にチェックし、そのオブジェクトを返します。従来の`typescript`という行はこの方式です。Zodの数値ではありません。

### ts-zod

```typescript
import { z } from "zod";

const PayloadSchema: z.ZodType<Payload> = z.object({
  id: z.number().int(), label: z.string(), values: z.array(z.number().int()),
  tags: z.record(z.string(), z.string()),
  next: z.lazy(() => PaymentSchema).nullable()
});
const PaymentSchema: z.ZodType<Payment> = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Case000"), data: PayloadSchema }),
  // 実コードではCase001〜Case099も同様に列挙
]);
const BatchSchema = z.array(PaymentSchema);
function parse(json: string): Payment[] {
  return BatchSchema.parse(JSON.parse(json));
}
```

Zodは型判別子とネストした各フィールドを検証し、検証済みのオブジェクトを作ります。この検証・オブジェクト生成もdecode時間とメモリ使用量に含めます。スキーマ構築とパッケージ導入は計測区間外です。Zodの版はサンプルのpackage-lockに固定し、結果のtoolchainsにも記録します。

```bash
python3 benchmarks/union100/run.py \
  --implementations typescript-simple,typescript-zod,typescript \
  --roots 200000 --samples 5
```

[結果一覧](../observed/macos-intel-actions/README.md)の各実装名にも、その計測の生成済みソースへのリンクがあります。単純な2ケースの利用例は[通常TS](../../samples/file-based/Payment.ts) / [Zod版](../../samples/file-based/PaymentZod.ts)です。

## WASIデシリアライズの改善調査

[原因の切り分け・実測・改善用reader・再実行CLI](diagnostics/README.md)を追加しました。同じ100ケース・60万ノードで、標準STJの読み取りと型専用の直接readerをNative/WASI AOTで比較しています。ローカルARMでの診断結果をIntel Actionsの結果とは分けて保存しています。
