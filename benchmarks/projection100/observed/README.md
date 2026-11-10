# このホストでの結果

IntelのGitHub Actions結果は[別ディレクトリ](../../observed/macos-intel-actions/)に保存します。[domain配送の比較](../../observed/macos-intel-actions/projection-domain.md)と[fanoutの比較](../../observed/macos-intel-actions/projection-fanout.md)を参照してください。以下の数値は従来のARMホストの実測です。

macOS arm64、物理メモリ128GiB。個々のサンプルとライフサイクル時間はJSON、一覧は同名のMarkdownに保存しています。

- `native-scaling`: 6言語、fanout、1千～100万イベント、2回。144条件中143条件成功。Swiftのlinear-append / wide / 100万は15秒のプロセス予算を超過しました。
- `wasi-scaling`: C#・Rust・Go・Swiftのホスト/WASI、fanout、1千～10万イベント、2回。144条件すべて成功。Swiftは両方6.3.1です。
- `native-routed`: 6言語、domain配送、dense、10万～1千万イベント、2回。予算で打ち切った条件は結果に明記しています。

100万イベント、fanoutでの投影秒数:

| 実装 | 辞書＋append dense | 辞書＋毎回copy dense | 線形探索 wide |
|---|---:|---:|---:|
| C# | 0.075 | 3.715 | 10.382 |
| Rust | 0.883 | 1.008 | 3.822 |
| TypeScript | 0.102 | 1.150 | 5.144 |
| Java | 0.134 | 0.321 | 10.925 |
| Go | 0.061 | 0.385 | 2.930 |
| Swift | 2.028 | 2.130 | timeout |

この規模では動的型判定の固定費が大きい実装もあります。単一サイズから計算量を決めず、全サイズとdomain配送の系列を合わせて見てください。コピーの確保戦略やGCも違います。特にJavaのdense copyは100万件でピークRSS約1.3GiBでしたが、これは保持状態だけのサイズではありません。

10万イベント、fanout、辞書＋append、dense:

| 実装 | ホスト 秒 | WASI 秒 | ホスト RSS MiB | WASI RSS MiB |
|---|---:|---:|---:|---:|
| C# | 0.0086 | 0.6791 | 17.2 | 82.6 |
| Rust | 0.0905 | 0.2879 | 1.9 | 17.5 |
| Go | 0.0062 | 0.1798 | 8.3 | 55.5 |
| Swift | 0.2412 | 1.6564 | 2.2 | 107.9 |

C#はホストがNative AOT、WASIが実験的Monoインタープリターです。WASIのRSSはWasmtime全体を含みます。この表だけからWasmそのものの一律の倍率を求めることはできません。

今回分かるのは、具体的な投影の書き方とランタイムで、どの規模から処理時間やRSSが増えるかです。CQRS/ES全体の言語選定には、永続化・並行処理などの別テストも必要です。

1千万イベント、domain配送、dense（25秒のプロセス予算、2回）:

| 実装 | append 秒 | 毎回copy 秒 | append RSS MiB |
|---|---:|---:|---:|
| C# | 0.200 | timeout | 33.8 |
| Rust | 0.623 | 9.126 | 13.2 |
| TypeScript | 0.360 | timeout | 204.6 |
| Java | 0.235 | 11.321 | 369.2 |
| Go | 0.320 | 17.508 | 28.0 |
| Swift | 1.947 | 7.587 | 18.7 |

appendは全6言語で1千万件を成功。10状態それぞれに10万要素、計100万の履歴要素を保持します。このテスト範囲では「100型だから処理できない」状況はなく、配送方法と更新方法による差が大きい結果でした。さらに大きな件数での限界は未測定です。
