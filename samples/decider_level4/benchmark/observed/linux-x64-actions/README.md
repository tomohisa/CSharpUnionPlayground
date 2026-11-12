# Linux x64 Actions HTTP benchmark

CPU: AMD EPYC 7763 64-Core Processor / RAM: 15.6084 GiB / `ubuntu-26.04`。

OS: `Linux-7.0.0-1019-azure-x86_64-with-glibc2.43` / logical CPUs: 4。

[Actions run](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/43204001771) / [Measured source](https://github.com/tomohisa/CSharpUnionPlayground/tree/0660eb069c55f369b88d643c1daaf48a034e3526/samples/decider_level4).

全7言語・SQLite／メモリJSONを同じLinux x64ジョブで順次測定しました。ビルドと機能テストは計測外、C#・F#はRelease JIT＋Dynamic PGOです。

C#はKestrel上のminimal hosting構成で、コントローラーを使いません。現実装はapp.Runによる共通ディスパッチャーであり、MapGet／MapPostでエンドポイントを定義する形式ではありません。

| runtime / compiler | measured version |
|---|---|
| dotnet | .NET SDK: /  Version:           11.0.100 /  Commit:            297acf930f |
| node | v26.13.1 |
| rust | rustc 1.99.1 (58cf8f9c3 2026-10-13) |
| java | openjdk version "27.0.1.0.2" 2026-10-21 |
| go | go version go1.27.4 linux/amd64 |
| swift | Swift version 6.4.2 (swift-6.4.2-RELEASE) |

## 10,000 students / concurrency 8

[遅延・起動・終了の全結果](students-10000.md) / [Raw data](students-10000.json)。

| 実装 | store | wall s | req/s | p95 ms | p99 ms | unexpected error % | expected rejection | sampled RSS MiB |
|---|---|---:|---:|---:|---:|---:|---:|---:|
| [csharp](../../../csharp) | sqlite | 53.103 | 1586.6 | 9.567 | 15.221 | 0.00 | 1050 | 303.9 |
| [csharp](../../../csharp) | memory-json | 27.470 | 3067.1 | 5.867 | 11.030 | 0.00 | 1050 | 309.6 |
| [fsharp](../../../fsharp) | sqlite | 51.792 | 1626.7 | 9.273 | 14.360 | 0.00 | 1050 | 336.0 |
| [fsharp](../../../fsharp) | memory-json | 27.047 | 3115.0 | 5.578 | 10.270 | 0.00 | 1050 | 383.5 |
| [rust](../../../rust) | sqlite | 44.060 | 1912.2 | 7.984 | 10.426 | 0.00 | 1050 | 158.3 |
| [rust](../../../rust) | memory-json | 19.723 | 4271.9 | 3.388 | 4.371 | 0.00 | 1050 | 181.7 |
| [ts-zod](../../../typescript) | sqlite | 82.145 | 1025.7 | 15.482 | 20.459 | 0.00 | 1050 | 288.1 |
| [ts-zod](../../../typescript) | memory-json | 45.895 | 1835.8 | 8.576 | 11.392 | 0.00 | 1050 | 311.5 |
| [java](../../../java) | sqlite | 59.287 | 1421.1 | 9.994 | 12.509 | 0.00 | 1050 | 375.0 |
| [java](../../../java) | memory-json | 23.673 | 3559.0 | 4.412 | 7.379 | 0.00 | 1050 | 840.5 |
| [go](../../../go) | sqlite | 64.250 | 1311.3 | 10.856 | 12.903 | 0.00 | 1050 | 132.7 |
| [go](../../../go) | memory-json | 33.147 | 2541.8 | 6.343 | 8.161 | 0.00 | 1050 | 145.2 |
| [swift](../../../swift) | sqlite | 72.922 | 1155.4 | 12.121 | 14.526 | 0.00 | 1050 | 366.3 |
| [swift](../../../swift) | memory-json | 53.396 | 1577.9 | 8.898 | 10.842 | 0.00 | 1050 | 409.9 |

全構成でイベント数・HTTP件数・期待した拒否・競合数・状態ハッシュが一致し、SQLite再起動後の復元も検証済みです。予期しない失敗・不完全なcohortは公開しません。

RSSは100ms間隔のサーバーPID観測値です。起動・終了・再起動は別計測で、クライアントCPU時間もraw dataに含みます。固定同時接続数の閉ループ測定であり、HTTPスタック・JSON codec・SQLiteドライバーを含む実装の比較です。言語単体の最大性能・統計的順位を示すものではありません。

[測定方法](../../README.md) / [別環境のローカル結果](../README.md)
