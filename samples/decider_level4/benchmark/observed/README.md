# Level4 ローカルHTTPベンチマーク

[Linux x64 Actionsの結果](linux-x64-actions/README.md)は別ページです。以下はローカルの独立した測定です。

Apple M4 Max / RAM 128 GiB / `macOS-27.2.1-arm64-arm-64bit`。

[10,000人・同時接続8の全結果](macos-arm64.md)／[Raw data](macos-arm64.json)。同じホストで7言語×SQLite・メモリJSONを順次測定しました。

[測定ソース](https://github.com/tomohisa/CSharpUnionPlayground/tree/20fc80244370cca9949c4eaca5072188781e1a3a/samples/decider_level4)。ソースに未コミットの変更がない状態で実測し、ソースSHA-256もraw dataに保存しています。

| 言語 | 実測ツールチェーン |
|---|---|
| C# / F# | .NET SDK 11.0.100 / Release JIT＋Dynamic PGO |
| TypeScript＋Zod | v24.18.0 |
| Rust | rustc 1.98.0 (88d9e12ae 2026-08-18) (Homebrew) |
| Java | openjdk version "21.0.6" 2025-01-21 LTS |
| Go | go version go1.23.4 darwin/arm64 |
| Swift | Apple Swift version 6.4 (swiftlang-6.4.0.34.1 clang-2100.3.34.1) |

## 同じ業務負荷での結果

| 実装 | SQLite s | memory JSON s | SQLite sampled RSS MiB | memory sampled RSS MiB |
|---|---:|---:|---:|---:|
| [csharp](../../csharp) | 19.626 | 12.341 | 347.4 | 275.3 |
| [fsharp](../../fsharp) | 18.015 | 11.498 | 395.9 | 391.4 |
| [rust](../../rust) | 14.917 | 7.903 | 154.3 | 149.8 |
| [ts-zod](../../typescript) | 31.508 | 19.266 | 362.2 | 365.0 |
| [java](../../java) | 24.427 | 9.515 | 1419.4 | 1464.8 |
| [go](../../go) | 25.446 | 23.029 | 150.3 | 161.5 |
| [swift](../../swift) | 122.013 | 41.644 | 161.9 | 184.9 |

各構成でHTTP 84,253件、イベント73,830件、期待した拒否1,050件、競合50件、状態ハッシュが一致しています。予期しないエラーは全構成で0です。100人のウォームアップを状態・メモリには含み、処理時間とHTTP遅延から外しています。

起動・終了・再起動、p50/p95/p99、API別の遅延は全結果・raw dataへ分けています。RSSは100ms間隔のサーバーPID観測値で、OSのhigh-water markではありません。

クライアントも同じPCで動かしています。memory JSONではクライアントCPU時間／wall時間が約1以上の構成もあり、負荷側が速度の上限に影響しています。1回の一連の処理の観測であり、言語単体の最大性能・統計的順位ではありません。

SwiftはApple Foundationの一時オブジェクトをrequest・起動時のautoreleasepoolで解放する対策を含みます。最終表には対策後の同じ一連の測定だけを使い、予備計測の速い行を混ぜていません。

[測定方法](../README.md)／[共通HTTPテスト](../../tests/http_tests.py)／[別測定：Level3 Linuxコンソール](../../../decider_level3/benchmark/observed/linux-x64-actions/README.md)
