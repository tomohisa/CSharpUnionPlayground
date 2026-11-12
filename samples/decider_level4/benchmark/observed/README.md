# Level4 ローカルHTTPベンチマーク

[10,000人・同時接続8の結果](macos-arm64.md)／[Raw data](macos-arm64.json)。同じホスト上で7言語×SQLite・メモリJSONを順番に測定します。

[計測器](../run.py)と[共通HTTPテスト](../../tests/http_tests.py)を使います。起動・終了・再起動、HTTP遅延、期待した拒否、予期しないエラー、投影の位置、状態の一致、サーバーRSSを区別して記録します。

これはHTTP APIの実装全体の比較です。[Level3 Linuxコンソール結果](../../../decider_level3/benchmark/observed/linux-x64-actions/README.md)は別の測定です。
