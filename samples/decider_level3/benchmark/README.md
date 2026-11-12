# Level 3 ES benchmark

[ローカルの実測結果（2規模・7言語・SQLite／メモリJSON）](observed/README.md)

7言語で同じ履修コマンド、件数、SQLスキーマ、SQLite設定を使うコンソールベンチマークです。

```bash
# まず結果の一致を確認。ビルド時間は計測に含めない
python3 samples/decider_level3/benchmark/run.py --quick --store both

# 予備計測から共通件数を決定し、7言語を順番に測定
python3 samples/decider_level3/benchmark/run.py --target-seconds 90 --store both

# 同じPC・同じnative実装の完了済みデータを校正に使う
python3 samples/decider_level3/benchmark/run.py --target-seconds 90 --store both \
  --calibrate-from samples/decider_level3/benchmark/observed/macos-arm64-large.json

# 同じ件数で再現、またはデータ量を変えて比較
python3 samples/decider_level3/benchmark/run.py --students 10000 --store both --output /tmp/es-10000.json
python3 samples/decider_level3/benchmark/run.py --students 20000 --store both --output /tmp/es-20000.json

# C#をNative AOTに切り替える（F#はJIT＋PGO）
python3 samples/decider_level3/benchmark/run.py --students 10000 --language csharp --csharp-native-aot
```

結果は `observed/latest.json` と `observed/latest.md` に保存します。`--output` で変更できます。既存の結果を上書きしたくない場合は別名を指定してください。DBは実行ごとに作る一時フォルダだけを使い、終了時に削除します。`--keep-databases` で保持できます。`--no-build` は直前のビルドを再利用するための指定です。`--calibrate-only` は校正と件数選定までを行い、本計測を始めません。

## 測定する処理

[共通負荷の生成と期待結果](workload.py)、[実行・整合性検証・レポート生成](run.py)、[DBスキーマ](schema.sql)、[固定の負荷・DB契約](workload.json)を分けています。

- 学籍登録、開講、履修、授業開始、出席2回、採点、修了、次の科目への履修を実際のDecider・共通Handler・Executorで実行します。修了者の7割が合格、3割が不合格です。
- 不正なID・定員、前提科目不足を拒否します。残席1を読んだ2つの準備済みコマンドを順にcommitし、後者の競合と再判断によるClassFullも検証します。
- 自動更新される索引Projectionに対して全件一覧3回、重複配信、別Projectionへの再構築を行います。コマンドループ時間にはこれらも含みます。
- タグで学生1人の履歴を読むポイントクエリ1000件と、Projectionから学生1人を取得してJSON応答を往復する1000件を別に測ります。
- 最後に全イベントのJSON復元、独立したProjectionへの再生、一覧の生成を測ります。ライブProjectionと再構築した状態の一致も確認します。
- 学校とは別の `DocumentRevision` 型で、4 KiB本文を16回更新します。文書ごとに小さな履歴を読んでrevisionを確認し、条件付き追記と最新revisionのライブ投影を行います。ここはストレージ用の負荷であり、学校のコマンドHandlerを通りません。
- SQLiteでは学校DB・文書DBを閉じて開き直します。学校は別プロセスで全ログからProjectionを再生し、同じ最終状態になることを確認します。文書の本文サイズとイベント数も検証します。

学生数Nは10の倍数です。クラス数はN/10、競合グループ数Rはmax(1,N/100)。学校イベント数は7.1N＋4R、文書数はmax(1,N/20)、文書イベント数はその16倍です。期待するcommit・拒否・競合・合格・不合格の件数を式から検証します。7言語の最終状態のSHA256も一致しなければ失敗します。

## 同じ条件にする範囲

デフォルトはSQLite、`--store memory-json` はJSON文字列を保持する索引メモリストアです。両方とも追加前のシリアライズと、読み出し・配信時のデシリアライズを通ります。元のオブジェクトをそのまま保持するストアとの比較ではありません。

SQLiteはWAL、synchronous=FULL、busy_timeout=5000、cache_size=-2048、wal_autocheckpoint=1000、foreign_keys=ONです。1コマンドのイベントバッチが1トランザクションです。条件確認とイベント・タグの追記を `BEGIN IMMEDIATE` から `COMMIT` まで同じトランザクションで行います。SQL文は再利用し、タグ検索は索引からpositionを選びます。全ログを毎回走査して条件を確認する実装ではありません。

タグのkeysは各文字列のUTF-8バイト長をbig endianの32 bitで前置したBLOBです。JSONのUnicodeエスケープや、Swiftの文字列の正規等価性でタグ判定が変わらないようにしています。JSONのタグメタ情報も保存し、復元したpayloadから導いたメタ情報と照合します。

学校と文書は別の型付きログなので、SQLiteファイルも分けます。文書型のcodec・メタ情報はアプリから注入し、ライブラリは学校のEventを参照しません。[学校型を含めずにビルドする検証](../tests/json_library_isolation.py)も用意しています。

予備計測は各言語2000人で行います。その中央値の実装について一旦見積もった規模でも測り、DBが大きくなったときの時間を使って約90秒になる共通件数を選びます。全言語へ同じ件数を渡すため、全てを厳密に1〜2分に揃えることはできません。速いものを待たせる処理や、遅い言語だけ件数を減らす処理はありません。実測時間と選定根拠はraw JSONに残します。大きなDBの実測がすでにある場合は `--calibrate-from` で再利用できます。ホストとnativeソース・依存設定・負荷のハッシュを照合し、別の条件のデータを黙って再利用しません。

C#・F#の標準計測はRelease JIT＋Dynamic PGO、Rust・Go・Swiftは最適化ビルド、Javaは標準JVM、TypeScriptはNode＋Zodです。ウォームアップ専用の同じ処理を何千回も回す方式ではなく、増え続けるイベントを一連の業務として処理します。

## 時間とメモリの読み方

`wallSeconds` はプロセス全体の経過時間です。`inputOpenReplaySeconds` はプロセス開始から入力JSONの復元・DBオープン・初期Projection再生まで、`responseCloseExitSeconds` は最後の計測マーカーから応答JSON出力・残るclose・終了までです。純粋なランタイム起動時間や純粋なclose時間ではありません。ネイティブ側のフェーズ時間も別に保存します。計測モードの応答は最終一覧と検証サマリーに絞り、イベント全件・監査traceを巨大なJSONとして出力しません。再構築の一致確認には各言語で一覧JSONを2回生成して比較します。

最大RSSはOSの `/usr/bin/time` で測り、ランタイム、入力、監査用のtrace、イベント全件読み取り、複数のProjectionを含みます。Projectionだけの増分メモリではありません。SQLのDBサイズは接続を閉じた後の学校・文書ファイルの合計です。

ドライバーに内蔵されるSQLiteの版は言語間で異なるため、実バージョンを記録します。この結果はランタイム・JSON・ドライバー・SQLiteを含む構成の比較です。言語単体の性能順位とは扱いません。C#は標準JSONを使い、型専用の手書きreaderで有利にする変更はしていません。

## 実装コード

| 言語 | JSON・SQLiteライブラリ | 学校での接続 | 文書の負荷 | ポイントクエリ |
|---|---|---|---|---|
| C# | [JsonStore.cs](../csharp/JsonStore.cs) / [SqliteJournal.cs](../csharp/SqliteJournal.cs) | [Application.cs](../csharp/Application.cs) | [StorageBenchmark.cs](../csharp/StorageBenchmark.cs) | [QueryBenchmark.cs](../csharp/QueryBenchmark.cs) |
| F# | [JsonStore.fs](../fsharp/JsonStore.fs) / [SqliteJournal.fs](../fsharp/SqliteJournal.fs) | [Application.fs](../fsharp/Application.fs) | [StorageBenchmark.fs](../fsharp/StorageBenchmark.fs) | [QueryBenchmark.fs](../fsharp/QueryBenchmark.fs) |
| Rust | [json_store.rs](../rust/src/json_store.rs) / [sqlite_journal.rs](../rust/src/sqlite_journal.rs) | [application.rs](../rust/src/application.rs) | [storage_benchmark.rs](../rust/src/storage_benchmark.rs) | [query_benchmark.rs](../rust/src/query_benchmark.rs) |
| TypeScript＋Zod | [jsonStore.ts](../typescript/jsonStore.ts) | [application.ts](../typescript/application.ts) | [storageBenchmark.ts](../typescript/storageBenchmark.ts) | [queryBenchmark.ts](../typescript/queryBenchmark.ts) |
| Java | [JsonStore.java](../java/playground/dcb/JsonStore.java) | [Application.java](../java/Application.java) | [StorageBenchmark.java](../java/StorageBenchmark.java) | [QueryBenchmark.java](../java/QueryBenchmark.java) |
| Go | [json_store.go](../go/dcb/json_store.go) | [application.go](../go/application.go) | [storage_benchmark.go](../go/storage_benchmark.go) | [query_benchmark.go](../go/query_benchmark.go) |
| Swift | [JsonStore.swift](../swift/JsonStore.swift) | [application.swift](../swift/application.swift) | [storageBenchmark.swift](../swift/storageBenchmark.swift) | [queryBenchmark.swift](../swift/queryBenchmark.swift) |

## Intel GitHub Actions

`Decider Level 3 Intel benchmark` を手動実行すると、`macos-15-intel` の1ジョブで全7言語・全サイズを順次測定します。標準件数はローカルと同じ22,900人／43,050人です。C#・F#はRelease JIT＋Dynamic PGO、TypeScriptはZod版です。

`publish: true` でmainから実行した場合、全構成の検証成功後に[Intel専用ページ](observed/macos-intel-actions/README.md)とraw JSONを自動更新します。Actionsのsummary・artifactにも保存します。失敗・不完全な構成や異なるジョブの混在を検出した場合は公開しません。
