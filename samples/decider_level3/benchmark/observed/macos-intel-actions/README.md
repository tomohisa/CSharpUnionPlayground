# Intel GitHub Actions ES benchmark

CPU: Intel(R) Core(TM) i7-8700B CPU @ 3.20GHz / RAM: 14 GiB / `macos-15-intel`。

[Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/43160423918) / [Measured source](https://github.com/tomohisa/CSharpUnionPlayground/tree/c1d5048cae711943869c897e314d7ca4e41f3117/samples/decider_level3).

全サイズ・7言語のSQLite／メモリJSONを、同じIntelジョブで順次測定しました。ビルドは計測外です。C#・F#はRelease JIT＋Dynamic PGOです。

| 学生数 | 結果 | Raw data |
|---:|---|---|
| 22,900 | [SQLite／メモリJSON](students-22900.md) | [JSON](students-22900.json) |
| 43,050 | [SQLite／メモリJSON](students-43050.md) | [JSON](students-43050.json) |

## データ量による変化

| 実装 | 22,900人 SQLite秒 / RSS MiB | 43,050人 SQLite秒 / RSS MiB |
|---|---:|---:|
| csharp | 211.630 / 977.1 | 397.885 / 2278.6 |
| fsharp | 202.904 / 1333.2 | 380.004 / 2264.1 |
| rust | 167.784 / 718.4 | 308.061 / 1363.9 |
| ts-zod | 274.240 / 1505.1 | 537.196 / 2072.4 |
| java | 176.826 / 1305.4 | 414.055 / 2688.2 |
| go | 196.983 / 836.0 | 413.330 / 1373.3 |
| swift | 479.550 / 809.0 | 725.226 / 1538.6 |

各規模で全実装の状態ハッシュ・JSON往復回数が一致し、SQLiteの整合性・索引・再起動後の状態を検証済みです。RSSは入力や監査trace、ランタイム、複数のProjectionを含み、投影だけのメモリではありません。

SQLiteの版・ドライバー・ランタイムはraw dataに記録しています。各構成1回の大きな処理の観測であり、言語単体の統計的な順位ではありません。ARMローカルとはCPU・RAM・OSが違うため、表を混ぜていません。

[測定方法](../../README.md) / [ARMローカル結果](../README.md)
