# 100種類のイベントからメモリプロジェクションを作る

JSON往復とは別のテストです。10ドメイン×10イベント型を、開いた `IEventPayload` 相当の型で受け取ります。各プロジェクションは自分の10種類を処理し、それ以外をdefaultで無視します。閉じた100ケースunionの網羅性には依存しません。

```bash
# 6言語、件数を段階的に増やす（既定: 1千～1千万、各3回）
python3 benchmarks/projection100/run.py

# 小規模の動作確認
python3 benchmarks/projection100/run.py --quick

# ディスパッチをドメインで絞った場合も比較
python3 benchmarks/projection100/run.py --routing domain

# WASI / Wasmtimeと、同じソースのホスト実行を比較
python3 benchmarks/projection100/run.py \
  --languages csharp,rust,go,swift --engines native,wasi \
  --sizes 1000,10000,100000 --samples 2

# 件数と時間上限を指定
python3 benchmarks/projection100/run.py \
  --sizes 100000,1000000,10000000 --timeout 30 --max-projection-seconds 20
```

`--skip-build`はソース・ツール設定・実装一覧が一致するビルドだけを再利用します。結果は`results/latest.json`と`.md`、途中結果も逐次保存します。ビルド用ファイルは`.build/`です。

## 何を測るか

イベントは生成して直ちに投影し、入力ログ全体を保持しません。イベント生成、型判定、状態更新を計測に含めます。Created / Renamed / BalanceAdded / ItemAppended / FlagToggled / CountAdded / TagChanged / Seen / Revised / Archivedを各ドメインに定義しています。状態は数値、文字列、フラグ、タグ辞書、履歴配列を持ちます。最後に未知のイベントも送り、無視できることを確認します。

既定の`fanout`は各イベントを10プロジェクションへ渡し、9個が無視します。`domain`は該当ドメインの1個にだけ渡します。RustのAny downcast、Swiftのprotocol existential、C#等の型switch、TSの文字列switchは同じコストではありません。これは具体的な実装の比較であり、言語自体の順位ではありません。

| mode | 状態の検索 | 履歴追加 | 大きくなるときの傾向 |
|---|---|---|---|
| indexed-append | 辞書 | 可変配列append | 平均で総処理量O(N)、配列再確保時には一時的なコピー |
| indexed-copy | 辞書 | 毎回配列全体をコピー | 1状態の履歴が伸びると総処理量O(N²) |
| linear-append | 全状態を線形探索 | 可変配列append | 状態数も増えると総処理量O(N²) |

`dense`はドメインごとに1状態を持ち、履歴を伸ばします。`wide`は100イベントごとに新しいaggregateに切り替え、状態数を増やします。状態数、保持する履歴要素数、最大履歴長も出力します。Swiftは参照型のStateを使い、辞書値の更新のたびに意図せず配列のcopy-on-writeを起こさないようにしています。

`growth p`は隣り合うサイズの実測から求めたlog(T₂/T₁)/log(N₂/N₁)です。1に近ければ総処理時間が概ね線形、2に近ければ概ね二次です。小規模では固定費やJITの影響が大きく、理論計算量の証明にはなりません。

## 時間・メモリ・正当性

各サンプルは独立したプロセスです。プロジェクション時間と、起動・準備・終了・プロセス全体時間を別々に保存します。結果検証と出力は投影タイマー外です。最終状態のフィールド、タグ、履歴全要素を読むchecksumと状態数などをPythonの参照実装と照合します。失敗を性能値として扱いません。

RSSは`/usr/bin/time`で取得するプロセス全体の最大常駐メモリです。GCの余裕、ランタイム、コード、入力の一時オブジェクトを含み、純粋な状態サイズや総割当量ではありません。WASIではWasmtimeのコンパイルとランタイムも含むため、guestヒープの使用量とは区別してください。Java/Nodeのヒープ上限は6GiBです。

1回のプロセスが`--timeout`を超えた場合、または投影時間が`--max-projection-seconds`を超えた場合、その組合せの大きいサイズを停止します。`testedCapacity`は**指定予算内で実際に成功した最大のテスト点**であり、言語やマシンの最大容量ではありません。OOMに達するまで試すものでもありません。

CQRS/ESの適性を判断するには永続ログ、IO、並行更新、スナップショット、再開、GC停止時間なども必要です。このテストが扱うのは単一プロセスのインメモリ再生です。

## ビルド要件とWASI

ホスト用は.NET 11 RC3 Native AOT、Rust、Node 24、Java 21以上、Go、Swiftを使用します。`native`という名前でもJava/TSはホスト上のJVM/Node実行です。全言語の依存ツールを自動インストールするスクリプトではありません。

WASIにはWasmtime、Rustの`wasm32-wasip1`ターゲット、Goの`GOOS=wasip1 GOARCH=wasm`、Swiftの対応する公式Wasm SDKが必要です。Rustはrustupがあればstableを使います。SwiftはコンパイラーとSDKの版を揃えるため、両エンジン指定時にswiftlyの6.3.1を使います（`--swift-toolchain` / `--swift-sdk`で変更）。

C#は実験的な`wasi-experimental` workloadを持つ専用.NET SDKを`--wasi-dotnet`で指定します。既定は`.build/dotnet/dotnet`です。WASI SDK 33は`--wasi-sdk`で指定します。既定パスは`.build/tools/wasi-sdk-33.0-arm64-macos`です。ホストやSDKの版が違えば適切なパスを指定してください。専用SDKを配置した後は次のようにworkloadを追加できます。

```bash
benchmarks/projection100/.build/dotnet/dotnet workload install wasi-experimental
rustup target add wasm32-wasip1 --toolchain stable
```

今回使ったSwift SDK名は`swift-6.3.1-RELEASE_wasm`、Wasmtimeは41系です。SDKの取得URL・チェックサム・コンパイラーとの対応は上記の公式手順で確認してください。

今回の.NET RC3ではネイティブ再リンクに必要なWITファイルが不足したため、公式のビルド済みMono WASIランタイムを使います（WasmBuildNative=false、トリミングなし）。C#のWASIは**Native AOTではなくMonoインタープリター**です。ランタイムのimport解決に`wasmtime -S http`が必要ですが、テストはHTTPを利用しません。報告JSONは数値を直接書き、STJのreflectionに依存しません。

TypeScript/NodeとJava/JVMはそのままWASIモジュールにならないため、このバックエンドは未実装として理由を出力します。別コンパイラーやWasm内JSエンジンの測定値を同一実装の結果として混ぜません。

公式資料: [Go WASI](https://go.dev/wiki/WebAssembly)、[Swift Wasm](https://www.swift.org/documentation/articles/wasm-getting-started.html)、[.NET WASI experimental](https://github.com/dotnet/runtime/blob/main/src/mono/wasi/README.md)。

## 保存した計測

`observed/`にこのMacでの実測を保存しています。各JSONにコマンド、ソース指紋、件数、個々の測定値を含みます。比較時はroutingとツールチェーンを揃えてください。古いホスト計測のSwiftはApple Swift 6.4、WASI比較の両エンジンは公式Swift 6.3.1であり、別の系列です。
