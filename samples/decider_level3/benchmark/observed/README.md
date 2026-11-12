# Local ES benchmark observations

Apple M4 Max / 128 GiB RAM / macOS arm64。7言語を同じホストで順番に実行しました。各実装・各storeにつき1回の大きな業務負荷であり、統計的な順位ではありません。

| 共通学生数 | 結果 | Raw data | 校正 |
|---:|---|---|---|
| 22,900 | [SQLite／メモリJSON](macos-arm64.md) | [JSON](macos-arm64.json) | 43,050人の実測から共通件数を選定 |
| 43,050 | [大きなデータ](macos-arm64-large.md) | [JSON](macos-arm64-large.json) | 小規模な予備計測から外挿した最初の実行 |

## データ量による変化

同じコード・DB設定で、SQLiteの一連の業務負荷と最大RSSを比較します。件数の増加に対して時間は単純な比例にはなっていません。これだけで原因をSQLite・GCなどのどれかに断定はできません。

| 実装 | 22,900人 wall s | 43,050人 wall s | 時間比 | 22,900人 RSS MiB | 43,050人 RSS MiB |
|---|---:|---:|---:|---:|---:|
| [csharp](../../csharp) | 38.145 | 139.706 | 3.66× | 1077.9 | 2093.9 |
| [fsharp](../../fsharp) | 43.555 | 174.050 | 4.00× | 1374.4 | 2507.2 |
| [rust](../../rust) | 35.910 | 166.534 | 4.64× | 661.5 | 1296.6 |
| [ts-zod](../../typescript) | 64.291 | 214.771 | 3.34× | 1703.2 | 2384.7 |
| [java](../../java) | 89.207 | 170.276 | 1.91× | 2017.6 | 3173.6 |
| [go](../../go) | 105.095 | 221.120 | 2.10× | 828.9 | 1524.8 |
| [swift](../../swift) | 95.970 | 187.094 | 1.95× | 866.5 | 1605.5 |

## 読み方

- wallは入力JSONの復元から応答・close・終了までです。コマンドループ、タグ検索、Projectionのポイントクエリ＋JSON、全件読み取り・再構築、4 KiB文書更新、別プロセスでの再起動も個別にraw dataへ保存しています。
- RSSには入力コマンド、監査trace、イベントの読み取り、複数のProjection、ランタイムが含まれます。Projectionだけの増分メモリではありません。
- SQLiteとメモリJSONの両方で、payloadのシリアライズ・デシリアライズを実行します。各規模で最終状態のハッシュとcodec回数が7言語間で一致しています。
- 目標は1言語あたり約90秒ですが、データ量と速度の関係が非線形で、同じ件数では全実装を厳密に1〜2分に揃えられません。短い実装へ待ち時間を足してはいません。
- 22,900人のSQLite版は約36〜105秒、43,050人では約140〜221秒でした。繰り返し回数で時間を引き延ばす方式ではありません。

## 使用したランタイム・SQLite

| 言語 | ランタイム／コンパイラー | ドライバー | SQLite |
|---|---|---|---|
| csharp | .NET SDK 11.0.100 / .NET 11.0.0 | Microsoft.Data.Sqlite 11.0.0 | 3.53.4 |
| fsharp | .NET SDK 11.0.100 / .NET 11.0.0 | Microsoft.Data.Sqlite 11.0.0 | 3.53.4 |
| rust | rustc 1.98.0 (88d9e12ae 2026-08-18) (Homebrew) | rusqlite 0.42.0 / bundled | 3.54.1 |
| ts-zod | v24.18.0 | node:sqlite / built-in | 3.53.1 |
| java | openjdk version "21.0.6" 2025-01-21 LTS | sqlite-jdbc-3.54.1.0.jar | 3.54.1 |
| go | go version go1.23.4 darwin/arm64 | github.com/mattn/go-sqlite3 v1.14.53 / CGO | 3.54.1 |
| swift | Apple Swift version 6.4 (swiftlang-6.4.0.34.1 clang-2100.3.34.1) | system SQLite3 | 3.51.0 |

C#・F#はRelease JIT＋Dynamic PGOです。C# Native AOTはCLIで選択でき、SQLite・メモリJSON・再起動をスモークテストしています。このページの大規模比較にはAOTの数値を混ぜていません。

小さい規模は[8768ae1](https://github.com/tomohisa/CSharpUnionPlayground/commit/8768ae1)、大きい規模は[024b318](https://github.com/tomohisa/CSharpUnionPlayground/commit/024b318)の実装です。校正・レポート処理の変更であり、7言語のnativeソースは同じです。各raw JSONの`sourceManifest`でファイル単位のSHA256を確認できます。

[負荷と測定方法](../README.md) / [実装・機能テスト・コード量](../../README.md)
