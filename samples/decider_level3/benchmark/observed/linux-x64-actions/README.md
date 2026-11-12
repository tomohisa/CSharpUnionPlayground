# Linux x64 GitHub Actions ES benchmark

CPU: AMD EPYC 7763 64-Core Processor / RAM: 15.6084 GiB / `ubuntu-26.04`。

[Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/43179765108) / [Measured source](https://github.com/tomohisa/CSharpUnionPlayground/tree/b8b5d71533da2c89916beb4969aa7bbc61604eae/samples/decider_level3).


## 測定環境

OS: `Linux-7.0.0-1019-azure-x86_64-with-glibc2.43` / architecture: `x86_64`。

| 言語 | 実測ツールチェーン |
|---|---|
| C# / F# | .NET SDK 11.0.100 / runtime 11.0.0 |
| TypeScript + Zod (Node) | v26.13.1 |
| Rust | rustc 1.99.1 (58cf8f9c3 2026-10-13) |
| Java | openjdk version "27.0.1.0.2" 2026-10-21 |
| Go | go version go1.27.4 linux/amd64 |
| Swift | Swift version 6.4.2 (swift-6.4.2-RELEASE) |

## 結果

全サイズ・7言語のSQLite／メモリJSONを、同じx64ジョブで順次測定しました。ビルドは計測外です。C#・F#はRelease JIT＋Dynamic PGOです。

| 学生数 | 結果 | Raw data |
|---:|---|---|
| 22,900 | [SQLite／メモリJSON](students-22900.md) | [JSON](students-22900.json) |
| 43,050 | [SQLite／メモリJSON](students-43050.md) | [JSON](students-43050.json) |

## データ量による変化

| 実装 | 22,900人 SQLite秒 / RSS MiB | 43,050人 SQLite秒 / RSS MiB |
|---|---:|---:|
| csharp | 85.738 / 1255.3 | 163.397 / 2339.5 |
| fsharp | 85.587 / 1372.3 | 165.009 / 2492.9 |
| rust | 70.956 / 564.5 | 134.987 / 1062.0 |
| ts-zod | 139.869 / 1771.6 | 262.956 / 2553.4 |
| java | 97.430 / 2671.0 | 176.990 / 3265.7 |
| go | 119.650 / 688.6 | 223.409 / 1518.2 |
| swift | 127.648 / 641.9 | 241.582 / 1181.3 |

各規模で全実装の状態ハッシュ・JSON往復回数が一致し、SQLiteの整合性・索引・再起動後の状態を検証済みです。RSSは入力や監査trace、ランタイム、複数のProjectionを含み、投影だけのメモリではありません。

SQLiteの版・ドライバー・ランタイムはraw dataに記録しています。各構成1回の大きな処理の観測であり、言語単体の統計的な順位ではありません。ARMローカルとはCPU・RAM・OSが違うため、表を混ぜていません。

[測定方法](../../README.md) / [ARMローカル結果](../README.md)
