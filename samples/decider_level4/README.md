# Level4: 同じHTTP APIから7言語のDCBを使う

[Level3](../decider_level3)のモデル・VO・Decider・Handler・DCB・JSONストア・投影を参照し、HTTPホストを追加しました。ドメインのコピーは持ちません。C#、F#、Rust、TypeScript＋Zod、Java、Go、Swiftに同じ[OpenAPI 3.1仕様](openapi.json)を実装しています。

[Linux x64 Actionsの測定](benchmark/observed/linux-x64-actions/README.md)と[ローカル測定](benchmark/observed/README.md)を別ページにしています。[Level3のLinux測定](../decider_level3/benchmark/observed/linux-x64-actions)とは測定対象が違います。HTTP結果とコンソール結果の表は混ぜません。

## 実行する

```sh
# メモリJSONストア。依存関係を取得し、Releaseでビルドして起動
python3 samples/decider_level4/run.py csharp
# SQLite。再起動してもイベントから投影を復元
python3 samples/decider_level4/run.py rust --port 8081 --sqlite /tmp/school.db
# 同じHTTP・競合・復元テストを全言語で実行
python3 samples/decider_level4/tests/http_tests.py
# 同じ件数を7言語×2ストアで順次測定
python3 samples/decider_level4/benchmark/run.py --students 10000 --concurrency 8
```

言語名は `csharp,fsharp,rust,typescript,java,go,swift`。`typescript` はZodありの実装です。ビルド済みの同じバイナリで再実行する場合はテスト・ベンチマークに `--no-build` を指定します。必要なSDKはLevel3と同じで、SwiftだけHTTP用にSwiftNIOを追加しています。SQLiteはWAL・FULL、同じスキーマ・索引を使います。

```sh
curl -H 'Content-Type: application/json' http://127.0.0.1:8080/commands \
  -d '{"commandId":"register-1","command":{"$type":"RegisterStudent","studentId":"alice","grade":"Year3"}}'
curl http://127.0.0.1:8080/students/alice
curl http://127.0.0.1:8080/school
```

## APIと処理の境界

| API | 内容 |
|---|---|
| `GET /health` | 起動確認 |
| `POST /commands` | 構造検証→VO・入力検証→Handler→Decider→条件付き追記→ライブ投影 |
| `POST /tickets` | 同じExecutorで準備。確定は後で行う |
| `POST /tickets/{id}/commit` | 準備したコマンドを一度だけ確定。競合なら読み直して再判断 |
| `GET /students/{id}` | ライブ投影の索引から学生を取得。全校分のコピーはしない |
| `GET /school` | ライブ投影の一覧をJSON化。全件クエリなので出力件数に応じた処理がある |
| `GET /status` | イベント位置・投影位置・投影通知の失敗数。検証用で、負荷測定外 |

構造検査は、この仕様のrequestで使うobject・ref・oneOf・required・enum・int32などを対象にしています。仕様に制約を足すときは7実装と共通テストを同時に更新します。

HTTPのJSON構造・型・enum違反は400、メディア型違反は415、本文上限1MiBは413です。構造が正しくてもIDが空、席数や日付が不正、前提科目未修得などの場合は、Level3のVO・Handler・Deciderが422で拒否します。追記はしません。正しい数学的整数 `1.0` も整数として受け付け、32bit範囲外・小数・booleanを拒否します。

追記成功の応答には `position` と `projectionPosition` を含めます。このプロセスの同期通知で投影に届いてから応答します。別プロセスが書いた末尾は読み取り時にRefreshします。独立した外部投影サービスや分散配信は追加していません。

`commandId` は監査用で、冪等性キーではありません。確定済み応答を受け取れなかった場合に、同じIDの再送を安全に重複排除する仕組みは今回の範囲にありません。チケットはインプロセスで上限1024、既定60秒の期限付きです。使用済み・期限切れ・未知のIDは404です。ホストはlocalhostにバインドします。

## 同じテストで確認する

[共通HTTPテスト](tests/http_tests.py)は各ホストに対して、入力エラー、本文上限、UnicodeのID、チケットの期限・上限・再使用、全9コマンド、履修条件、投影更新、SQLite再起動を確認します。返されたJSONはOpenAPIのレスポンススキーマに照合します。

同じ版で準備した2つの履修のうち1つを確定したあと、もう一方が競合を1回検知して満席を再判断することを確認します。別に12同時リクエストを1席へ流し、確定が1件だけであることも確認します。並行処理で誰が席を取るかは固定しません。

全言語を指定した実行では、1つのホストが書いたSQLiteイベントを他の6言語も含めて読み、投影が完全一致することを追加確認します。過去イベントのVO復元経路はLevel3のままで、現在のコマンド用ルールを再適用しません。

## ベンチマーク

[ローカル結果](benchmark/observed/README.md)／[実装と計測方法](benchmark/run.py)。通常の固定同時接続数による閉ループ測定です。各クラス10人に対し、登録・履修・辞退・再履修・授業開始・出席・評価・修了を行います。個別クエリ、全件一覧、意図した拒否、競合の再判断を加えています。同じオブジェクトのJSON化だけを繰り返す処理ではありません。

各ホスト・ストアを別プロセスで順番に実行します。処理時間・HTTP遅延・予期しないエラー率・意図した拒否件数・サーバーRSSを記録します。起動・SIGTERMによる終了・SQLite再起動は別項目です。状態ハッシュ・イベント数・HTTP件数・拒否件数・競合数が全行で一致しない場合は結果を公開しません。

| 言語 | ホストと比較のポイント |
|---|---|
| [C#](csharp) | ASP.NET Core。JSON Source Generationとclosed階層をそのまま再利用 |
| [F#](fsharp) | ASP.NET Core。DUとHandlerを再利用し、fieldless DUのwire表現を合わせる |
| [Rust](rust) | tiny_http＋8ワーカースレッド。所有権を持つ準備チケットを共通ライブラリに追加 |
| [TypeScript＋Zod](typescript) | Node HTTP。Zodでコマンドとイベントを検証。同期SQLiteがイベントループを占有 |
| [Java](java) | JDK HttpServer＋virtual threads、Jackson |
| [Go](go) | net/http。既存のinterface union用JSONとHandlerを利用 |
| [Swift](swift) | SwiftNIO。SQLite・ドメイン処理を8ワーカーへ移し、event loopを塞がない |

HTTPスタックも含めたこの実装の比較です。同時接続数8、クライアントCPUやローカルの同一ホスト競合の影響があるため、これだけで言語の最大性能を判断しません。

## コード量

<!-- code-stats:start -->

Level3を再利用するソースも含めた、このHTTPアプリに必要な維持コードの比較です。Level3の古いコンソール入口・Application・ベンチマークは除外します。共有ファイル内の未使用の宣言をコンパイラーのdead-code eliminationで差し引く集計ではありません。

ドメインテストに加え、全言語へ適用する同じPythonの共通HTTPテストを各アプリのテスト費用として含めています。この共有ファイルをリポジトリ内で7つ実装しているという意味ではありません。共通ライブラリは別枠で、アプリ側合計に含めません。OpenAPI、ビルド設定、ロックファイル、README、共通ビルドランチャー、計測器は言語別のコード表から除外します。

### 行数（空行を含む）

| 言語 | Model・VO | Decider | コマンド・Handler・検証 | DCB接続・メタ情報 | ドメイン投影 | ユースケース | HTTP設定・JSON・プロトコル | ドメインテスト | 共通HTTPテスト | アプリ側合計（テスト含む） | 共通ライブラリ（別枠） |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| C# | 126 | 127 | 67 | 35 | 172 | 52 | 51 | 214 | 181 | 1,025 | 454 |
| F# | 122 | 99 | 40 | 31 | 154 | 77 | 84 | 189 | 181 | 977 | 381 |
| Rust | 132 | 113 | 42 | 31 | 109 | 129 | 177 | 158 | 181 | 1,072 | 293 |
| TypeScript＋Zod | 95 | 102 | 49 | 38 | 119 | 37 | 35 | 174 | 181 | 830 | 221 |
| Java | 244 | 126 | 101 | 31 | 131 | 70 | 45 | 165 | 181 | 1,094 | 217 |
| Go | 324 | 293 | 155 | 71 | 450 | 156 | 870 | 310 | 181 | 2,810 | 915 |
| Swift | 101 | 103 | 40 | 32 | 122 | 90 | 520 | 163 | 181 | 1,352 | 226 |

### 空白以外の文字数

| 言語 | Model・VO | Decider | コマンド・Handler・検証 | DCB接続・メタ情報 | ドメイン投影 | ユースケース | HTTP設定・JSON・プロトコル | ドメインテスト | 共通HTTPテスト | アプリ側合計（テスト含む） | 共通ライブラリ（別枠） |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| C# | 5,663 | 5,542 | 4,625 | 1,696 | 10,146 | 3,842 | 3,820 | 18,977 | 12,301 | 66,612 | 21,467 |
| F# | 4,881 | 5,068 | 2,137 | 1,965 | 8,714 | 3,723 | 4,360 | 17,455 | 12,301 | 60,604 | 18,619 |
| Rust | 5,143 | 5,707 | 2,216 | 2,308 | 10,240 | 3,269 | 4,187 | 27,133 | 12,301 | 72,504 | 25,430 |
| TypeScript＋Zod | 5,592 | 5,286 | 2,490 | 2,547 | 9,114 | 2,920 | 2,933 | 22,824 | 12,301 | 66,007 | 19,727 |
| Java | 8,304 | 6,106 | 3,673 | 2,241 | 10,997 | 4,171 | 3,718 | 19,311 | 12,301 | 70,822 | 21,719 |
| Go | 6,114 | 5,449 | 3,333 | 2,620 | 10,121 | 3,254 | 18,764 | 23,095 | 12,301 | 85,051 | 20,214 |
| Swift | 4,670 | 5,657 | 1,941 | 2,277 | 8,823 | 2,817 | 18,235 | 19,725 | 12,301 | 76,446 | 22,174 |

### 同じ分類に使ったソース範囲

| 言語 | 分類 | 実コード |
|---|---|---|
| C# | Model・VO | [../decider_level3/csharp/Models.cs:1–26](../decider_level3/csharp/Models.cs#L1-L26) / [../decider_level3/csharp/Models.cs:29–49](../decider_level3/csharp/Models.cs#L29-L49) / [../decider_level3/csharp/Models.cs:61–74](../decider_level3/csharp/Models.cs#L61-L74) / [../decider_level3/csharp/Models.cs:79–79](../decider_level3/csharp/Models.cs#L79-L79) / [../decider_level3/csharp/ValueObjects.cs:1–64](../decider_level3/csharp/ValueObjects.cs#L1-L64) |
| C# | Decider | [../decider_level3/csharp/Decider.cs:1–123](../decider_level3/csharp/Decider.cs#L1-L123) / [../decider_level3/csharp/Models.cs:75–78](../decider_level3/csharp/Models.cs#L75-L78) |
| C# | コマンド・Handler・検証 | [../decider_level3/csharp/Handlers.cs:1–16](../decider_level3/csharp/Handlers.cs#L1-L16) / [../decider_level3/csharp/Models.cs:50–60](../decider_level3/csharp/Models.cs#L50-L60) / [../decider_level3/csharp/ValueObjects.cs:65–104](../decider_level3/csharp/ValueObjects.cs#L65-L104) |
| C# | DCB接続・メタ情報 | [../decider_level3/csharp/Routing.cs:1–35](../decider_level3/csharp/Routing.cs#L1-L35) |
| C# | ドメイン投影 | [../decider_level3/csharp/Models.cs:27–27](../decider_level3/csharp/Models.cs#L27-L27) / [../decider_level3/csharp/Projection.cs:1–81](../decider_level3/csharp/Projection.cs#L1-L81) / [../decider_level3/csharp/SchoolProjection.cs:1–90](../decider_level3/csharp/SchoolProjection.cs#L1-L90) |
| C# | ユースケース | [../decider_level3/csharp/Models.cs:28–28](../decider_level3/csharp/Models.cs#L28-L28) / [../decider_level3/csharp/Models.cs:80–87](../decider_level3/csharp/Models.cs#L80-L87) / [csharp/Program.cs:44–86](csharp/Program.cs#L44-L86) |
| C# | HTTP設定・JSON・プロトコル | [csharp/Program.cs:1–43](csharp/Program.cs#L1-L43) / [csharp/Program.cs:87–94](csharp/Program.cs#L87-L94) |
| C# | ドメインテスト | [../decider_level3/csharp/DomainTests.cs:1–164](../decider_level3/csharp/DomainTests.cs#L1-L164) / [../decider_level3/csharp/ProjectionTests.cs:1–50](../decider_level3/csharp/ProjectionTests.cs#L1-L50) |
| C# | 共通HTTPテスト | [tests/http_tests.py:1–181](tests/http_tests.py#L1-L181) |
| C# | 共通ライブラリ（別枠） | [../decider_level3/csharp/Dcb.cs:1–206](../decider_level3/csharp/Dcb.cs#L1-L206) / [../decider_level3/csharp/JsonStore.cs:1–148](../decider_level3/csharp/JsonStore.cs#L1-L148) / [../decider_level3/csharp/SqliteJournal.cs:1–100](../decider_level3/csharp/SqliteJournal.cs#L1-L100) |
| F# | Model・VO | [../decider_level3/fsharp/Models.fs:1–72](../decider_level3/fsharp/Models.fs#L1-L72) / [../decider_level3/fsharp/Models.fs:74–74](../decider_level3/fsharp/Models.fs#L74-L74) / [../decider_level3/fsharp/Models.fs:76–76](../decider_level3/fsharp/Models.fs#L76-L76) / [../decider_level3/fsharp/Models.fs:88–101](../decider_level3/fsharp/Models.fs#L88-L101) / [../decider_level3/fsharp/Models.fs:106–106](../decider_level3/fsharp/Models.fs#L106-L106) / [../decider_level3/fsharp/ValueObjects.fs:1–33](../decider_level3/fsharp/ValueObjects.fs#L1-L33) |
| F# | Decider | [../decider_level3/fsharp/Decider.fs:1–95](../decider_level3/fsharp/Decider.fs#L1-L95) / [../decider_level3/fsharp/Models.fs:102–105](../decider_level3/fsharp/Models.fs#L102-L105) |
| F# | コマンド・Handler・検証 | [../decider_level3/fsharp/Handlers.fs:1–13](../decider_level3/fsharp/Handlers.fs#L1-L13) / [../decider_level3/fsharp/Models.fs:77–87](../decider_level3/fsharp/Models.fs#L77-L87) / [../decider_level3/fsharp/ValueObjects.fs:34–49](../decider_level3/fsharp/ValueObjects.fs#L34-L49) |
| F# | DCB接続・メタ情報 | [../decider_level3/fsharp/Routing.fs:1–31](../decider_level3/fsharp/Routing.fs#L1-L31) |
| F# | ドメイン投影 | [../decider_level3/fsharp/Models.fs:73–73](../decider_level3/fsharp/Models.fs#L73-L73) / [../decider_level3/fsharp/Projection.fs:1–58](../decider_level3/fsharp/Projection.fs#L1-L58) / [../decider_level3/fsharp/SchoolProjection.fs:1–95](../decider_level3/fsharp/SchoolProjection.fs#L1-L95) |
| F# | ユースケース | [../decider_level3/fsharp/Models.fs:75–75](../decider_level3/fsharp/Models.fs#L75-L75) / [../decider_level3/fsharp/Models.fs:107–114](../decider_level3/fsharp/Models.fs#L107-L114) / [fsharp/Program.fs:81–148](fsharp/Program.fs#L81-L148) |
| F# | HTTP設定・JSON・プロトコル | [fsharp/Program.fs:1–80](fsharp/Program.fs#L1-L80) / [fsharp/Program.fs:149–152](fsharp/Program.fs#L149-L152) |
| F# | ドメインテスト | [../decider_level3/fsharp/DomainTests.fs:1–141](../decider_level3/fsharp/DomainTests.fs#L1-L141) / [../decider_level3/fsharp/ProjectionTests.fs:1–48](../decider_level3/fsharp/ProjectionTests.fs#L1-L48) |
| F# | 共通HTTPテスト | [tests/http_tests.py:1–181](tests/http_tests.py#L1-L181) |
| F# | 共通ライブラリ（別枠） | [../decider_level3/fsharp/Dcb.fs:1–156](../decider_level3/fsharp/Dcb.fs#L1-L156) / [../decider_level3/fsharp/SqliteJournal.fs:1–92](../decider_level3/fsharp/SqliteJournal.fs#L1-L92) / [../decider_level3/fsharp/JsonStore.fs:1–133](../decider_level3/fsharp/JsonStore.fs#L1-L133) |
| Rust | Model・VO | [../decider_level3/rust/src/models.rs:1–58](../decider_level3/rust/src/models.rs#L1-L58) / [../decider_level3/rust/src/models.rs:67–67](../decider_level3/rust/src/models.rs#L67-L67) / [../decider_level3/rust/src/models.rs:78–104](../decider_level3/rust/src/models.rs#L78-L104) / [../decider_level3/rust/src/models.rs:118–133](../decider_level3/rust/src/models.rs#L118-L133) / [../decider_level3/rust/src/models.rs:140–140](../decider_level3/rust/src/models.rs#L140-L140) / [../decider_level3/rust/src/value_objects.rs:1–29](../decider_level3/rust/src/value_objects.rs#L1-L29) |
| Rust | Decider | [../decider_level3/rust/src/decider.rs:1–107](../decider_level3/rust/src/decider.rs#L1-L107) / [../decider_level3/rust/src/models.rs:134–139](../decider_level3/rust/src/models.rs#L134-L139) |
| Rust | コマンド・Handler・検証 | [../decider_level3/rust/src/handlers.rs:1–14](../decider_level3/rust/src/handlers.rs#L1-L14) / [../decider_level3/rust/src/models.rs:105–117](../decider_level3/rust/src/models.rs#L105-L117) / [../decider_level3/rust/src/value_objects.rs:30–44](../decider_level3/rust/src/value_objects.rs#L30-L44) |
| Rust | DCB接続・メタ情報 | [../decider_level3/rust/src/routing.rs:1–31](../decider_level3/rust/src/routing.rs#L1-L31) |
| Rust | ドメイン投影 | [../decider_level3/rust/src/models.rs:59–66](../decider_level3/rust/src/models.rs#L59-L66) / [../decider_level3/rust/src/projection.rs:1–38](../decider_level3/rust/src/projection.rs#L1-L38) / [../decider_level3/rust/src/school_projection.rs:1–63](../decider_level3/rust/src/school_projection.rs#L1-L63) |
| Rust | ユースケース | [../decider_level3/rust/src/models.rs:68–77](../decider_level3/rust/src/models.rs#L68-L77) / [../decider_level3/rust/src/models.rs:141–150](../decider_level3/rust/src/models.rs#L141-L150) / [rust/src/main.rs:116–224](rust/src/main.rs#L116-L224) |
| Rust | HTTP設定・JSON・プロトコル | [rust/src/main.rs:1–115](rust/src/main.rs#L1-L115) / [rust/src/main.rs:225–286](rust/src/main.rs#L225-L286) |
| Rust | ドメインテスト | [../decider_level3/rust/src/domain_tests.rs:1–131](../decider_level3/rust/src/domain_tests.rs#L1-L131) / [../decider_level3/rust/src/projection_tests.rs:1–27](../decider_level3/rust/src/projection_tests.rs#L1-L27) |
| Rust | 共通HTTPテスト | [tests/http_tests.py:1–181](tests/http_tests.py#L1-L181) |
| Rust | 共通ライブラリ（別枠） | [../decider_level3/rust/src/dcb.rs:1–185](../decider_level3/rust/src/dcb.rs#L1-L185) / [../decider_level3/rust/src/json_store.rs:1–59](../decider_level3/rust/src/json_store.rs#L1-L59) / [../decider_level3/rust/src/sqlite_journal.rs:1–49](../decider_level3/rust/src/sqlite_journal.rs#L1-L49) |
| TypeScript＋Zod | Model・VO | [../decider_level3/typescript/models.ts:1–51](../decider_level3/typescript/models.ts#L1-L51) / [../decider_level3/typescript/models.ts:54–54](../decider_level3/typescript/models.ts#L54-L54) / [../decider_level3/typescript/models.ts:57–57](../decider_level3/typescript/models.ts#L57-L57) / [../decider_level3/typescript/models.ts:70–84](../decider_level3/typescript/models.ts#L70-L84) / [../decider_level3/typescript/models.ts:90–90](../decider_level3/typescript/models.ts#L90-L90) / [../decider_level3/typescript/models.ts:100–100](../decider_level3/typescript/models.ts#L100-L100) / [../decider_level3/typescript/valueObjects.ts:1–19](../decider_level3/typescript/valueObjects.ts#L1-L19) / [../decider_level3/typescript/valueObjects.ts:41–46](../decider_level3/typescript/valueObjects.ts#L41-L46) |
| TypeScript＋Zod | Decider | [../decider_level3/typescript/decider.ts:1–97](../decider_level3/typescript/decider.ts#L1-L97) / [../decider_level3/typescript/models.ts:85–89](../decider_level3/typescript/models.ts#L85-L89) |
| TypeScript＋Zod | コマンド・Handler・検証 | [../decider_level3/typescript/handlers.ts:1–16](../decider_level3/typescript/handlers.ts#L1-L16) / [../decider_level3/typescript/models.ts:58–69](../decider_level3/typescript/models.ts#L58-L69) / [../decider_level3/typescript/valueObjects.ts:20–40](../decider_level3/typescript/valueObjects.ts#L20-L40) |
| TypeScript＋Zod | DCB接続・メタ情報 | [../decider_level3/typescript/routing.ts:1–38](../decider_level3/typescript/routing.ts#L1-L38) |
| TypeScript＋Zod | ドメイン投影 | [../decider_level3/typescript/models.ts:52–53](../decider_level3/typescript/models.ts#L52-L53) / [../decider_level3/typescript/projection.ts:1–51](../decider_level3/typescript/projection.ts#L1-L51) / [../decider_level3/typescript/schoolProjection.ts:1–66](../decider_level3/typescript/schoolProjection.ts#L1-L66) |
| TypeScript＋Zod | ユースケース | [../decider_level3/typescript/models.ts:55–56](../decider_level3/typescript/models.ts#L55-L56) / [../decider_level3/typescript/models.ts:91–99](../decider_level3/typescript/models.ts#L91-L99) / [typescript/server.ts:32–57](typescript/server.ts#L32-L57) |
| TypeScript＋Zod | HTTP設定・JSON・プロトコル | [../decider_level3/typescript/models.ts:101–101](../decider_level3/typescript/models.ts#L101-L101) / [typescript/server.ts:1–31](typescript/server.ts#L1-L31) / [typescript/server.ts:58–60](typescript/server.ts#L58-L60) |
| TypeScript＋Zod | ドメインテスト | [../decider_level3/typescript/domain.test.ts:1–141](../decider_level3/typescript/domain.test.ts#L1-L141) / [../decider_level3/typescript/projection.test.ts:1–33](../decider_level3/typescript/projection.test.ts#L1-L33) |
| TypeScript＋Zod | 共通HTTPテスト | [tests/http_tests.py:1–181](tests/http_tests.py#L1-L181) |
| TypeScript＋Zod | 共通ライブラリ（別枠） | [../decider_level3/typescript/dcb.ts:1–149](../decider_level3/typescript/dcb.ts#L1-L149) / [../decider_level3/typescript/jsonStore.ts:1–72](../decider_level3/typescript/jsonStore.ts#L1-L72) |
| Java | Model・VO | [../decider_level3/java/Models.java:1–45](../decider_level3/java/Models.java#L1-L45) / [../decider_level3/java/Models.java:62–143](../decider_level3/java/Models.java#L62-L143) / [../decider_level3/java/Models.java:211–286](../decider_level3/java/Models.java#L211-L286) / [../decider_level3/java/ValueObjects.java:1–41](../decider_level3/java/ValueObjects.java#L1-L41) |
| Java | Decider | [../decider_level3/java/Decider.java:1–109](../decider_level3/java/Decider.java#L1-L109) / [../decider_level3/java/Models.java:287–303](../decider_level3/java/Models.java#L287-L303) |
| Java | コマンド・Handler・検証 | [../decider_level3/java/Models.java:144–210](../decider_level3/java/Models.java#L144-L210) / [../decider_level3/java/SchoolHandler.java:1–11](../decider_level3/java/SchoolHandler.java#L1-L11) / [../decider_level3/java/ValueObjects.java:42–64](../decider_level3/java/ValueObjects.java#L42-L64) |
| Java | DCB接続・メタ情報 | [../decider_level3/java/Routing.java:1–31](../decider_level3/java/Routing.java#L1-L31) |
| Java | ドメイン投影 | [../decider_level3/java/Models.java:46–53](../decider_level3/java/Models.java#L46-L53) / [../decider_level3/java/Projection.java:1–53](../decider_level3/java/Projection.java#L1-L53) / [../decider_level3/java/SchoolProjection.java:1–70](../decider_level3/java/SchoolProjection.java#L1-L70) |
| Java | ユースケース | [../decider_level3/java/Models.java:54–61](../decider_level3/java/Models.java#L54-L61) / [../decider_level3/java/Models.java:304–343](../decider_level3/java/Models.java#L304-L343) / [java/Server.java:42–63](java/Server.java#L42-L63) |
| Java | HTTP設定・JSON・プロトコル | [java/Server.java:1–41](java/Server.java#L1-L41) / [java/Server.java:64–67](java/Server.java#L64-L67) |
| Java | ドメインテスト | [../decider_level3/java/DomainTests.java:1–138](../decider_level3/java/DomainTests.java#L1-L138) / [../decider_level3/java/ProjectionTests.java:1–27](../decider_level3/java/ProjectionTests.java#L1-L27) |
| Java | 共通HTTPテスト | [tests/http_tests.py:1–181](tests/http_tests.py#L1-L181) |
| Java | 共通ライブラリ（別枠） | [../decider_level3/java/playground/dcb/Dcb.java:1–162](../decider_level3/java/playground/dcb/Dcb.java#L1-L162) / [../decider_level3/java/playground/dcb/JsonStore.java:1–55](../decider_level3/java/playground/dcb/JsonStore.java#L1-L55) |
| Go | Model・VO | [../decider_level3/go/domain/models.go:1–81](../decider_level3/go/domain/models.go#L1-L81) / [../decider_level3/go/domain/models.go:96–169](../decider_level3/go/domain/models.go#L96-L169) / [../decider_level3/go/domain/models.go:235–308](../decider_level3/go/domain/models.go#L235-L308) / [../decider_level3/go/domain/value_objects.go:1–88](../decider_level3/go/domain/value_objects.go#L1-L88) / [../decider_level3/go/domain/value_objects.go:154–160](../decider_level3/go/domain/value_objects.go#L154-L160) |
| Go | Decider | [../decider_level3/go/domain/decider.go:1–280](../decider_level3/go/domain/decider.go#L1-L280) / [../decider_level3/go/domain/models.go:309–321](../decider_level3/go/domain/models.go#L309-L321) |
| Go | コマンド・Handler・検証 | [../decider_level3/go/domain/handlers.go:1–25](../decider_level3/go/domain/handlers.go#L1-L25) / [../decider_level3/go/domain/models.go:170–234](../decider_level3/go/domain/models.go#L170-L234) / [../decider_level3/go/domain/value_objects.go:89–153](../decider_level3/go/domain/value_objects.go#L89-L153) |
| Go | DCB接続・メタ情報 | [../decider_level3/go/domain/routing.go:1–71](../decider_level3/go/domain/routing.go#L1-L71) |
| Go | ドメイン投影 | [../decider_level3/go/domain/models.go:82–87](../decider_level3/go/domain/models.go#L82-L87) / [../decider_level3/go/domain/projection.go:1–190](../decider_level3/go/domain/projection.go#L1-L190) / [../decider_level3/go/domain/school_projection.go:1–254](../decider_level3/go/domain/school_projection.go#L1-L254) |
| Go | ユースケース | [../decider_level3/go/domain/models.go:88–95](../decider_level3/go/domain/models.go#L88-L95) / [../decider_level3/go/domain/models.go:322–357](../decider_level3/go/domain/models.go#L322-L357) / [go/main.go:167–278](go/main.go#L167-L278) |
| Go | HTTP設定・JSON・プロトコル | [../decider_level3/go/domain/json.go:1–694](../decider_level3/go/domain/json.go#L1-L694) / [go/main.go:1–166](go/main.go#L1-L166) / [go/main.go:279–288](go/main.go#L279-L288) |
| Go | ドメインテスト | [../decider_level3/go/domain_test.go:1–211](../decider_level3/go/domain_test.go#L1-L211) / [../decider_level3/go/projection_tests.go:1–99](../decider_level3/go/projection_tests.go#L1-L99) |
| Go | 共通HTTPテスト | [tests/http_tests.py:1–181](tests/http_tests.py#L1-L181) |
| Go | 共通ライブラリ（別枠） | [../decider_level3/go/dcb/store.go:1–428](../decider_level3/go/dcb/store.go#L1-L428) / [../decider_level3/go/dcb/json_store.go:1–487](../decider_level3/go/dcb/json_store.go#L1-L487) |
| Swift | Model・VO | [../decider_level3/swift/models.swift:1–33](../decider_level3/swift/models.swift#L1-L33) / [../decider_level3/swift/models.swift:48–64](../decider_level3/swift/models.swift#L48-L64) / [../decider_level3/swift/models.swift:76–87](../decider_level3/swift/models.swift#L76-L87) / [../decider_level3/swift/valueObjects.swift:1–39](../decider_level3/swift/valueObjects.swift#L1-L39) |
| Swift | Decider | [../decider_level3/swift/decider.swift:1–99](../decider_level3/swift/decider.swift#L1-L99) / [../decider_level3/swift/models.swift:88–91](../decider_level3/swift/models.swift#L88-L91) |
| Swift | コマンド・Handler・検証 | [../decider_level3/swift/handlers.swift:1–11](../decider_level3/swift/handlers.swift#L1-L11) / [../decider_level3/swift/models.swift:65–75](../decider_level3/swift/models.swift#L65-L75) / [../decider_level3/swift/valueObjects.swift:40–57](../decider_level3/swift/valueObjects.swift#L40-L57) |
| Swift | DCB接続・メタ情報 | [../decider_level3/swift/routing.swift:1–32](../decider_level3/swift/routing.swift#L1-L32) |
| Swift | ドメイン投影 | [../decider_level3/swift/models.swift:34–39](../decider_level3/swift/models.swift#L34-L39) / [../decider_level3/swift/projection.swift:1–49](../decider_level3/swift/projection.swift#L1-L49) / [../decider_level3/swift/schoolProjection.swift:1–67](../decider_level3/swift/schoolProjection.swift#L1-L67) |
| Swift | ユースケース | [../decider_level3/swift/models.swift:40–47](../decider_level3/swift/models.swift#L40-L47) / [../decider_level3/swift/models.swift:92–99](../decider_level3/swift/models.swift#L92-L99) / [swift/main.swift:108–181](swift/main.swift#L108-L181) |
| Swift | HTTP設定・JSON・プロトコル | [../decider_level3/swift/wire.swift:1–338](../decider_level3/swift/wire.swift#L1-L338) / [swift/main.swift:1–107](swift/main.swift#L1-L107) / [swift/main.swift:182–256](swift/main.swift#L182-L256) |
| Swift | ドメインテスト | [../decider_level3/swift/domainTests.swift:1–135](../decider_level3/swift/domainTests.swift#L1-L135) / [../decider_level3/swift/projectionTests.swift:1–28](../decider_level3/swift/projectionTests.swift#L1-L28) |
| Swift | 共通HTTPテスト | [tests/http_tests.py:1–181](tests/http_tests.py#L1-L181) |
| Swift | 共通ライブラリ（別枠） | [../decider_level3/swift/dcb.swift:1–175](../decider_level3/swift/dcb.swift#L1-L175) / [../decider_level3/swift/JsonStore.swift:1–51](../decider_level3/swift/JsonStore.swift#L1-L51) |

<!-- code-stats:end -->
