# .NET 11正式版での検証（2026-11-10）

Microsoftの[正式版リリース](https://devblogs.microsoft.com/dotnet/announcing-dotnet-11/)と[公式SDKメタデータ](https://builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json)に基づき、SDK 11.0.100 / runtime 11.0.0で再検証しました。配布物のSHA512を検証し、macOS arm64で実行しています。

global.jsonは正式版に固定し、allowPrereleaseはfalse。csprojと生成ベンチマークはLangVersion 15.0、file-based appはnet11.0既定のC# 15を使います。サンプルからpreview指定を削除しました。

## 結果

| 対象 | 結果 |
|---|---|
| xUnit全件 | 86件中77成功、9失敗、skip 0 |
| closedの網羅性 | 8/8 |
| closed JSON自動推論 | 7/7 |
| abstract / closed / unionのswitch比較 | 13/13 |
| 属性Generatorのコンパイラー診断 | 6/6 |
| ネストしたPaymentのNative AOT | 5方式すべて成功 |
| 属性GeneratorのNative AOT smoke | 40/40、動的コード・JSONリフレクション無効 |
| GA WASI LLVMOnly AOT | 100ルート×1バッチ、metadata / directの両方成功 |

[実行記録](verification/dotnet11-ga.json)に各チェックの出力、失敗したテスト名、WASIのSDK版・ソースハッシュ・AOTビルド証跡を含めています。小さいWASI実行は動作確認用で、性能評価には使いません。

WASIコンパイラーは、選択したSDKのMSBuild.runtimeconfig.jsonのランタイム版と一致するものだけを選びます。複数SDKのあるPCで10.0.2やRC版を誤って使わないための回帰テストも追加しました。wasi-experimentalは正式版SDK上でもその名前のワークロードです。

## 残る9件

RC3と同じ失敗集合でした。期待値を緩めたりskipにしたりはしていません。

- ResultUnionの6件: StringResultとIntResultがともにvalueを持ち、構造判別器はプロパティ値のトークンまで見ません。契約全体の拒否でListResultを含む書き込みにも影響します。
- Paymentの3件: unionとして書いたJSONには基底型復元用の$typeがないため、abstract Paymentとしては読めません。

回避策は型判別子付きの共通契約、専用コンバーター、値トークンを見るカスタムclassifierなどです。closed階層＋JsonPolymorphicの例は、unionから基底型への変換を必要とせずに成功しています。

## 再実行

```bash
dotnet --version # 11.0.100
dotnet test src/UnionTest.Tests/UnionTest.Tests.csproj
python3 scripts/verify_closed_payment_exhaustiveness.py
python3 scripts/verify_closed_inferred_json.py
python3 scripts/verify_payment_switch_comparison.py
python3 scripts/verify_tagged_json_diagnostics.py

dotnet publish samples/file-based/with-verification/ClosedInferredJson.cs \
  -c Release -r osx-arm64 -o /tmp/payment-ga
/tmp/payment-ga/ClosedInferredJson --require-native

dotnet publish src/PlaygroundTaggedJson.AotSmoke -c Release -r osx-arm64 -o /tmp/tagged-ga
/tmp/tagged-ga/PlaygroundTaggedJson.AotSmoke --require-native
```

Generator参照のあるfile-based appにCLI引数でグローバルな `-p:PublishAot=true` を渡すと、netstandard2.0のAnalyzerにも伝播してNETSDK1207になります。file-based publishの既定のAOT、またはアプリ側csprojのPublishAotを使い、`--require-native` で実際のネイティブ実行を確認しています。

## Intelでの性能再計測

同じGA対応コミット `3644b56` で、次の実行も完了しました。

- [通常JSON・Projection](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880008976)
- [Native / WASM AOT対照](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880014627)
- [JSON読み取り診断](https://github.com/tomohisa/CSharpUnionPlayground/actions/runs/42880019482)

[観測ページ](../benchmarks/observed/macos-intel-actions/README.md)の各測定元はSDK 11.0.100です。Native/WASM JSONの12条件はすべて完走し、エラーはありません。Projectionのtimeout / not-run-after-budgetは、設定した予算による観測としてそのまま記録しています。C#のWASMビルド証跡は11.0.0のクロスコンパイラーとLLVMOnly AOTを示します。

通常実行、対照実行、読み取り診断は別VMです。RC3との差をSDK変更だけの効果とは解釈しません。RC3のアーカイブの版名や数値は変更していません。
