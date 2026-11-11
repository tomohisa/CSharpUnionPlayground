# Decider Level 3：7言語のインメモリES／DCB

[Level 2](../decider_level2/README.md)の履修ドメインを、タグで読む学籍・クラス・履修の判断モデルと、全イベントを受けるライブ一覧投影に分けました。C#、F#、Rust、TypeScript＋Zod、Java、Go、Swiftで、同じ機能とJSON契約を実装します。言語ごとの実コード、行数、空白以外の文字数を比較するためのサンプルです。

## 共通部品とドメインを分ける

| 言語 | 抽出できるライブラリ | 学校の型／判断 | 学校のタグ規則／コマンド接続 | 学校の投影 | 比較 |
|---|---|---|---|---|---|
| C# | [Dcb.cs](csharp/Dcb.cs) | [Models](csharp/Models.cs) / [Decider](csharp/Decider.cs) | [Routing](csharp/Routing.cs) / [Application](csharp/Application.cs) | [Projection](csharp/Projection.cs) | [README](csharp/README.md) |
| F# | [Dcb.fs](fsharp/Dcb.fs) | [Models](fsharp/Models.fs) / [Decider](fsharp/Decider.fs) | [Routing](fsharp/Routing.fs) / [Application](fsharp/Application.fs) | [Projection](fsharp/Projection.fs) | [README](fsharp/README.md) |
| Rust | [dcb.rs](rust/src/dcb.rs) | [models](rust/src/models.rs) / [decider](rust/src/decider.rs) | [routing](rust/src/routing.rs) / [application](rust/src/application.rs) | [projection](rust/src/projection.rs) | [README](rust/README.md) |
| TypeScript＋Zod | [dcb.ts](typescript/dcb.ts) | [models](typescript/models.ts) / [decider](typescript/decider.ts) | [routing](typescript/routing.ts) / [application](typescript/application.ts) | [projection](typescript/projection.ts) | [README](typescript/README.md) |
| Java | [Dcb.java](java/playground/dcb/Dcb.java) | [Models](java/Models.java) / [Decider](java/Decider.java) | [Routing](java/Routing.java) / [Application](java/Application.java) | [Projection](java/Projection.java) | [README](java/README.md) |
| Go | [dcb/store.go](go/dcb/store.go) | [models](go/domain/models.go) / [decider](go/domain/decider.go) | [routing](go/domain/routing.go) / [application](go/application.go) | [projection](go/domain/projection.go) | [README](go/README.md) |
| Swift | [dcb.swift](swift/dcb.swift) | [models](swift/models.swift) / [decider](swift/decider.swift) | [routing](swift/routing.swift) / [application](swift/application.swift) | [projection](swift/projection.swift) | [README](swift/README.md) |

共通部品はSchoolDomain、科目、学年、履修状態を参照しません。イベントをobject／Any／IEventPayloadへ変換せず、EventStore<E>とRecorded<E>にドメインの閉じたイベント型を渡します。コマンドCと拒否Rも型パラメーターです。ドメインのswitch／matchでは元のケースを列挙できます。

コマンド実行ループは具象メモリストアではなく、読み取りと条件付き追記を定義するinterface／trait／protocolに依存します。学校側が渡すのは、コマンドのQuery、イベントのEventDescriptor、判断関数、更新関数、コピー関数です。

[独立ビルドテスト](tests/library_isolation.py)では学校のModels・Deciderを一切含めず、カウンター用のイベント・コマンドでライブラリをコンパイルして実行します。Javaは別パッケージ、Swiftは実際の別モジュールから利用します。Goはdcbパッケージだけを-race付きでテストします。

複数のドメインのイベントを同じ型付きログへ載せる場合は、ホストアプリがそれらをまとめる閉じたイベント集合とメタ情報の関数を定義します。その集合の更新はホストの責任で、ライブラリへ学校や請求などのケースを追加する必要はありません。任意のプラグインによる無制限な拡張と、固定ケースの網羅性を同時に保証する仕組みではありません。

## 学籍・クラス・履修の分離

1つの学校全体のStateをコマンド処理のたびに復元する方式をやめ、別々の更新関数で3つのレジストリを作ります。

| レジストリ | キー | 担当する情報 |
|---|---|---|
| StudentRegister | studentId | 学年・進級 |
| ClassRegister | classId | 科目、定員、名簿、募集／授業中／終了 |
| EnrollmentRegister | (classId, studentId) | 予約、出席、合否、取消、取得単位 |

インメモリの検索は線形走査で、タグ索引はまだ実装していません。判断側へ返すイベントだけをタグで絞ります。

ログ自体は1本です。集約ごとの独立した永続ストリームへ分割するのではなく、DCBのタグで必要な事実を選びます。CommandProcessorはその読み取り結果だけから一時的なDecisionModelを作ります。ライブ一覧の全校データはコマンドの判断元にしません。

例えばbobのBへの登録では、student:bobの学年・過去の履修と、class:b1の状態をORクエリで読みます。別の生徒の学籍や、無関係なクラスの開講イベントまで読む必要はありません。タグで読む履修には科目が必要なので、StudentEnrolledへcourseを持たせています。

授業開始ではClassStartedと受講者ごとのStudyStartedを同じバッチに入れます。これによりクラスの授業開始と、各生徒の履修開始を別々のタグ履歴から復元できます。ClassCompletedには結果一覧を持たせ、クラスの終了状態の復元で別の学籍モデルを参照しません。

Level 2の業務ルールは維持しています。FoundationはYear2以上、AdvancedはYear3以上かつFoundation合格済み。定員は1〜10、採点には出席が必要で60点以上が合格、開始後の登録・取消は拒否します。履修フェーズはReserved／Studying／Passed／Failed／Withdrawnで、未確定値をnullで埋めません。

## タグ・型・コマンドのメタ情報

タグは表示用の文字列を連結せず、構造を持ちます。

```json
{"category":"enrollment","keys":["class-id","student-id"]}
```

タグ文字列はUnicodeスカラーの並びとして一致を判定します。学校ドメインのIDは、呼び出し元が同じ正規化形（NFC）で与える契約です。ドメイン側のSwiftのString辞書には正規等価性があるため、正規化形を混在させる入力は比較対象にしません。

一覧のキー順はUTF-8のバイト順に揃えています。カルチャ依存の比較やUTF-16順との違いは、大文字小文字と補助文字を含む回帰シナリオで確認します。

keysの順序は意味を持ちます。classId="c"とstudentId="a:b"、classId="c:a"とstudentId="b"は別のキーです。内部の履修キーにもtuple／struct／JSONタプル表現を使います。

| イベント | 自動付与するタグ |
|---|---|
| StudentRegistered／StudentPromoted | student(studentId) |
| ClassOpened／ClassStarted／ClassCompleted | class(classId) |
| StudentEnrolled／StudentDropped／StudyStarted／AttendanceRecorded／ResultRecorded | student、class、enrollment(classId, studentId) |

呼び出し側はタグや位置を手入力して追記しません。ストアに渡したドメインのdescribe関数がイベント型名とタグを返し、ストアが位置とcommandIdを付けます。読み込むログのメタ情報も同じ関数で検査します。enrollmentタグは個別履修の検索・診断に使えるメタ情報で、今回の業務コマンドの整合性条件はstudent／classタグを使います。

```text
Recorded<E>
  position  : ストアが採番する1からの連番
  commandId : コマンドから引き継ぐ識別子
  eventType : ドメインのdescribe関数が決める論理イベント型
  tags      : ドメインのdescribe関数が作る構造化タグ
  payload   : 元のドメインのイベント型E
```

commandIdは追跡用で、永続的な冪等キーではありません。同じIDのコマンドをもう一度送ることを、ストア全体で自動的に抑止する実装ではありません。

## 条件付き追記と再判断

[DCB公式仕様](https://dcb.events/specification/)のQueryとAppend Conditionの考え方を、小さな同期APIで実装しています。

- QueryItem内のtagsはAND、typesは指定された型のOR。typesが空なら型を限定しません。
- QueryItem同士はOR。itemsが空のQueryは全件です。
- Readは一致したイベントと、同じロックで取得したログ全体のafterを返します。afterは最後に一致したイベントの位置ではありません。
- Appendはafterより後に条件Queryへ一致するイベントがないかを検査し、なければバッチ全体を同じロックで追加します。
- 競合したバッチは追加も配信もしません。再読み取り・モデル更新・再判断を行い、再び拒否または新しいバッチを作ります。競合は最大3回で打ち切ります。

| コマンド | 読み取り・追記条件の範囲 |
|---|---|
| RegisterStudent／PromoteStudent | studentタグ |
| OpenClass／StartClass／CompleteClass | classタグ |
| Enroll | studentタグ OR classタグ |
| Drop／Attend／Assess | classタグ |

```mermaid
sequenceDiagram
    participant A as 書き手A
    participant B as 書き手B
    participant S as EventStore
    participant P as LiveProjection
    A->>S: Read(student:A OR class:f)
    B->>S: Read(student:B OR class:f)
    Note over A,B: 同じ残席1を読む
    A->>S: Append(events, query, after)
    S->>S: 条件検査＋バッチ全体を確定
    S->>P: コミット済みバッチを自動配信
    S-->>A: Committed
    B->>S: Append(old events, query, after)
    S-->>B: Conflict（追加・配信なし）
    B->>S: Read(query)
    B->>B: 再判断→ClassFull
```

Prepare／Commit操作は、この競合をsleepなしで再現するテスト用スケジューラーです。業務コマンドそのものはRegisterStudent、Enrollなどの閉じたCommand型です。通常はExecuteが読み取りから再試行まで行います。準備時のRejectedは読み取り時点の判断として返し、書き込みを伴わない拒否に条件付き追記は行いません。

## 一覧はライブプロジェクションから返す

LiveProjection<E,S>も共通部品です。ドメインのreduceとcopyStateを受け取り、チェックポイントと状態を管理します。学校側は3つのレジストリを更新し、それらをSchoolListへ変換します。

- 起動時は全イベントを再生してから購読を登録します。再生と登録は追記と同じロックで行い、間にイベントが抜ける窓を作りません。
- 追記が成功したら、ストアがコミット済みバッチを自動配信します。アプリがその都度投影更新を呼ぶ方式ではありません。
- バッチを投影へ反映する場合も、更新後の状態と位置を一緒に公開します。
- 同じ位置の再配信は無視し、位置の欠落は拒否します。途中まで適用してチェックポイントだけ残すことはしません。
- ListAllは投影のスナップショットを返します。初期に取得した一覧が後続の更新で書き換わらないことも確認します。
- 全ログから別の投影を再構築し、ライブ投影と一致することを確認します。CLIのRebuildは照合用の再構築で、購読の自動復旧コマンドではありません。

購読関数が失敗を返した場合は切り離し、listenerFaultsへ記録します。C#・F#・Java・TypeScriptは例外、Goはpanic、Rustはunwindするpanicも通知失敗として分離します。追記済みの事実を追記失敗へ戻さず、他の購読者への配信を続けます。Swiftの購読関数は非throwingでfalseを返す契約です。Swiftのfatal errorやabort方式のpanicなど、プロセス自体の異常終了を回復するものではありません。

学校アプリは投影障害があれば古い一覧を黙って返さず、利用不可にします。コピー関数と更新関数は、有効なデータに対して純粋に動く必要があります。不変値はidentityコピーでよいですが、可変な配列やオブジェクトを使う場合は独立したコピーを渡す契約です。

## 言語ごとの選択

| 言語 | 今回使った特徴と制約 |
|---|---|
| C# | closed record、ImmutableSortedDictionary、ジェネリックinterface、Source Generation。Tagには構造的Equals／HashCodeを明示 |
| F# | DU、不変Map、ジェネリックinterface、関数注入。.NET 11 Release JIT＋Dynamic PGO |
| Rust | enum、BTreeMap、trait、所有したコピーとMutex。設定を捕捉するクロージャも受ける |
| TypeScript | Zodから型を導出、構造的なport、Map＋明示的コピー。同期処理は単一イベントループ内で原子的 |
| Java | sealed record、公開パッケージのgeneric interface、コピーしたMap／List、synchronized |
| Go | generic interface、structの複合キー、明示的コピー、Mutex。標準type switchの網羅性検査はない |
| Swift | 関連値付きenum、値セマンティクス、primary associated typeのprotocol、Sendable、NSLock。CodableはRecordedに条件付きで適合 |

TypeScriptの競合プローブは、同じ読み取りから準備した2つの追記をPromiseで実行する論理的競合です。Workersで同じストアを共有する並列処理ではありません。他の6言語では実際の2スレッド／goroutineを使います。Goではrace detectorも実行します。

## 実行と検証

```bash
python3 samples/decider_level3/run.py --language csharp --scenario last-seat
python3 samples/decider_level3/run.py --language fsharp --scenario start-redecide
python3 samples/decider_level3/run.py --language typescript --scenario live-snapshots
python3 samples/decider_level3/run.py --verify
python3 samples/decider_level3/run.py --language csharp --native-aot --verify
python3 samples/decider_level3/tests/library_isolation.py
python3 samples/decider_level3/tests/check_exhaustiveness.py
```

.NETはglobal.json固定の11.0.100です。F#はJIT＋PGO前提で、今回WASM・Native AOTは対象にしません。C#はJSONリフレクションを無効にしたNative AOTでも同じ機能を検証します。Nodeは24、Javaは21、Goは1.23以降、Rustはedition 2024対応stable、SwiftはFoundationを使います。

TypeScriptの型チェックには次を使います。

```bash
npm ci --prefix samples/file-based
npm ci --prefix samples/decider_level3/typescript
samples/file-based/node_modules/.bin/tsc -p samples/decider_level3/typescript/tsconfig.json
```

[固定期待値](fixtures/expected.json)を持つ17シナリオには、履修全体、残席競合、未作成時の競合、無関係なタグの更新、競合後に開始バッチを作り直すケース、区切り文字を含む複合キー、過去の一覧保持が含まれます。全シナリオでコミット済み履歴の再取り込みと投影再構築を確認し、全7言語の49組でメタ情報付きのログをそのまま交換してBへの履修を継続します。

readsはアプリから初回準備したQuery・after・件数の監査表示です。内部再試行の全読み取りログではありません。traceのconflictsと、再判断後に変わったイベントバッチも照合します。

別のカウンタードメインの独立ビルドでは、AND／OR／型フィルター、バッチ追記、競合時に追加しないこと、購読失敗の分離、遅い購読開始の追いつき、二重配信、欠落位置の拒否、再判断、ログのメタ情報検査、2つの書き手の勝者が1つになることを確認します。ケース追加の実験では、ライブラリを通してもドメインの網羅性検査が残ることを検証します。

## 範囲と参考にした設計

永続DB、ネットワーク、非同期I/O、outbox、分散購読、バックプレッシャー、コマンドIDの永続的冪等性、イベント・制度変更の版管理は対象外です。購読はストアのロック内で同期的に動きます。購読者はストアを再入呼び出しせず、I/Oせず、速い純粋な消費処理を行う契約です。これは配信順と単純さのための制約で、汎用の本番用イベントバスではありません。

再取り込みは型付きペイロード・位置・メタ情報を検査しますが、過去のすべての業務コマンドを再検証する処理ではありません。保存済みの事実として再生します。学年・科目条件は固定ポリシーで、公開モデルを不正に直接構築できないようにするsmart constructorも未実装です。

参考にしたのは[DCB仕様](https://dcb.events/specification/)と、[Sekibanのイベント候補](https://github.com/J-Tech-Japan/Sekiban/blob/main/dcb/src/Sekiban.Dcb.Core/Events/SerializableEventCandidate.cs)、[整合性タグ情報](https://github.com/J-Tech-Japan/Sekiban/blob/main/dcb/src/Sekiban.Dcb.Core/Commands/ConsistencyTagEntry.cs)、[メタ情報](https://github.com/J-Tech-Japan/Sekiban/blob/main/dcb/src/Sekiban.Dcb.Core.Model/Events/EventMetadata.cs)の分離です。Sekibanパッケージは使わず、タグ予約・Orleansのactor・SortableUniqueIdを移植していません。今回はタグごとの位置ではなく、全体位置とQueryの組合せで競合を判定します。

[Claude Opusの設計レビューと反映内容](reviews/opus-design-review.md)も残しています。

## コード量の比較

共通ライブラリを別枠にし、ライブラリを除いた**アプリ側合計**を表示します。ファイル単位ではなく、混在するファイルはソース範囲単位で次の責務に分けます。

| 分類 | 含めるコード |
|---|---|
| Model | 学籍・履修・クラスの状態、値、イベント、拒否理由、判断入力のDecisionModel |
| Decider | 純粋な業務判断と補助関数、判断結果のAccepted／Rejected型 |
| コマンド・ハンドラー | Command型、学校固有のタグ／イベントメタ情報、読み取り条件、状態復元とDeciderを接続するRouting |
| ドメイン投影 | 学籍・履修・クラスのreducer、一覧生成、SchoolList型。判断用の状態復元にも共用する投影を含む |
| ユースケース | Execute／Prepare／Commit／一覧／再構築／再配送のOperation型と実行制御、Trace型、チケット・実行履歴管理 |
| アプリ設定・JSON・CLI | ストア／投影／processorの生成と購読接続、入出力DTO、JSON変換、CLI、Application内のimport・namespace等の共通宣言 |
| 共通ライブラリ（別枠） | 学校に依存しないタグ・イベントエンベロープ・ストアport・条件付き追記・再試行processor・ライブ投影基盤 |

汎用CommandProcessorの競合再判断ループは共通ライブラリ、学校の読み取り条件と判断への接続はコマンド・ハンドラーです。Application内の初期接続はアプリ設定、その後の実行制御はユースケースとして区別します。型に付随するJSON属性・Zodスキーマ・コンストラクター検証はその型の分類に含めます。独立した手書きJSON変換はアプリ設定・JSON・CLIに含めるため、JSON関連の総量を単独で表す列ではありません。

[分類定義](code_stats_classification.json)は各ソースの既定分類と上書き範囲（1始まり、両端を含む）を保持します。**各物理行を一度だけ分類し、その同じ行から行数と空白以外の文字数を加算**します。上書き範囲内の空行・括弧もその範囲の分類へ、それ以外の空行・括弧・import等はファイルの既定分類へ計上します。範囲指定のないファイル追加・範囲重複・削除・ソースSHA256の変化を検出して集計を停止するため、コード変更時には分類も再確認してください。末尾のソース範囲一覧で分類を確認できます。

- 空行・コメント・括弧だけの行を含む物理行数を数えます。
- 文字数はUnicodeの空白をすべて除いた文字数です。文字列・コメント中の空白も除き、バイト数とは区別します。
- ライブラリやJSON変換を含め、維持する各言語のソースを一度ずつ数えます。
- カウンターのテスト本体、検証Python、fixtures、README、設定・依存ロック、コンパイラー生成物は除外します。Main内のテスト起動分岐はMainの一部として含めます。
- 自動整形や1行の長さで行数が変わるため、文字数と内訳を併せて比較します。このサンプルは速度・メモリのベンチマークではありません。

<!-- code-stats:start -->

### 行数（空行を含む）

| 言語 | Model | Decider | コマンド・ハンドラー | ドメイン投影 | ユースケース | アプリ設定・JSON・CLI | アプリ側合計（ライブラリ除外） | 共通ライブラリ（別枠） |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| C# | 61 | 126 | 52 | 78 | 44 | 32 | 393 | 130 |
| F# | 88 | 98 | 46 | 59 | 54 | 39 | 384 | 90 |
| Rust | 103 | 112 | 49 | 46 | 40 | 22 | 372 | 110 |
| TypeScript＋Zod | 69 | 101 | 54 | 52 | 32 | 20 | 328 | 93 |
| Java | 205 | 125 | 100 | 61 | 65 | 20 | 576 | 107 |
| Go | 230 | 291 | 145 | 196 | 113 | 805 | 1,780 | 295 |
| Swift | 62 | 102 | 46 | 55 | 32 | 353 | 650 | 106 |

### 空白以外の文字数

| 言語 | Model | Decider | コマンド・ハンドラー | ドメイン投影 | ユースケース | アプリ設定・JSON・CLI | アプリ側合計（ライブラリ除外） | 共通ライブラリ（別枠） |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| C# | 3,075 | 5,504 | 2,638 | 4,381 | 2,701 | 1,930 | 20,229 | 5,677 |
| F# | 3,604 | 5,024 | 2,858 | 3,376 | 2,711 | 1,532 | 19,105 | 4,462 |
| Rust | 3,055 | 5,645 | 3,107 | 3,889 | 3,116 | 1,406 | 20,218 | 6,657 |
| TypeScript＋Zod | 3,938 | 5,218 | 3,774 | 3,792 | 3,041 | 1,281 | 21,044 | 5,841 |
| Java | 6,302 | 6,066 | 4,517 | 4,935 | 3,877 | 1,429 | 27,126 | 6,543 |
| Go | 4,167 | 5,412 | 4,054 | 4,279 | 3,056 | 17,849 | 38,817 | 6,117 |
| Swift | 2,597 | 5,620 | 2,940 | 3,468 | 2,511 | 13,374 | 30,510 | 7,170 |

### 分類したソース範囲

同じファイルの中も責務で分けています。以下は行数・文字数に共通の分類です。

| 言語 | 分類 | ソース範囲 |
|---|---|---|
| C# | Model | [csharp/Models.cs:1–25](csharp/Models.cs#L1-L25) / [csharp/Models.cs:28–48](csharp/Models.cs#L28-L48) / [csharp/Models.cs:60–73](csharp/Models.cs#L60-L73) / [csharp/Models.cs:78–78](csharp/Models.cs#L78-L78) |
| C# | Decider | [csharp/Decider.cs:1–122](csharp/Decider.cs#L1-L122) / [csharp/Models.cs:74–77](csharp/Models.cs#L74-L77) |
| C# | コマンド・ハンドラー | [csharp/Models.cs:49–59](csharp/Models.cs#L49-L59) / [csharp/Routing.cs:1–41](csharp/Routing.cs#L1-L41) |
| C# | ドメイン投影 | [csharp/Models.cs:26–26](csharp/Models.cs#L26-L26) / [csharp/Projection.cs:1–77](csharp/Projection.cs#L1-L77) |
| C# | ユースケース | [csharp/Application.cs:7–10](csharp/Application.cs#L7-L10) / [csharp/Application.cs:17–47](csharp/Application.cs#L17-L47) / [csharp/Models.cs:27–27](csharp/Models.cs#L27-L27) / [csharp/Models.cs:79–86](csharp/Models.cs#L79-L86) |
| C# | アプリ設定・JSON・CLI | [csharp/Application.cs:1–6](csharp/Application.cs#L1-L6) / [csharp/Application.cs:11–16](csharp/Application.cs#L11-L16) / [csharp/Program.cs:1–20](csharp/Program.cs#L1-L20) |
| C# | 共通ライブラリ | [csharp/Dcb.cs:1–130](csharp/Dcb.cs#L1-L130) |
| F# | Model | [fsharp/Models.fs:1–71](fsharp/Models.fs#L1-L71) / [fsharp/Models.fs:73–73](fsharp/Models.fs#L73-L73) / [fsharp/Models.fs:75–75](fsharp/Models.fs#L75-L75) / [fsharp/Models.fs:87–100](fsharp/Models.fs#L87-L100) / [fsharp/Models.fs:105–105](fsharp/Models.fs#L105-L105) |
| F# | Decider | [fsharp/Decider.fs:1–94](fsharp/Decider.fs#L1-L94) / [fsharp/Models.fs:101–104](fsharp/Models.fs#L101-L104) |
| F# | コマンド・ハンドラー | [fsharp/Models.fs:76–86](fsharp/Models.fs#L76-L86) / [fsharp/Routing.fs:1–35](fsharp/Routing.fs#L1-L35) |
| F# | ドメイン投影 | [fsharp/Models.fs:72–72](fsharp/Models.fs#L72-L72) / [fsharp/Projection.fs:1–58](fsharp/Projection.fs#L1-L58) |
| F# | ユースケース | [fsharp/Application.fs:6–7](fsharp/Application.fs#L6-L7) / [fsharp/Application.fs:13–55](fsharp/Application.fs#L13-L55) / [fsharp/Models.fs:74–74](fsharp/Models.fs#L74-L74) / [fsharp/Models.fs:106–113](fsharp/Models.fs#L106-L113) |
| F# | アプリ設定・JSON・CLI | [fsharp/Application.fs:1–5](fsharp/Application.fs#L1-L5) / [fsharp/Application.fs:8–12](fsharp/Application.fs#L8-L12) / [fsharp/Program.fs:1–29](fsharp/Program.fs#L1-L29) |
| F# | 共通ライブラリ | [fsharp/Dcb.fs:1–90](fsharp/Dcb.fs#L1-L90) |
| Rust | Model | [rust/src/models.rs:1–58](rust/src/models.rs#L1-L58) / [rust/src/models.rs:67–67](rust/src/models.rs#L67-L67) / [rust/src/models.rs:78–104](rust/src/models.rs#L78-L104) / [rust/src/models.rs:118–133](rust/src/models.rs#L118-L133) / [rust/src/models.rs:140–140](rust/src/models.rs#L140-L140) |
| Rust | Decider | [rust/src/decider.rs:1–106](rust/src/decider.rs#L1-L106) / [rust/src/models.rs:134–139](rust/src/models.rs#L134-L139) |
| Rust | コマンド・ハンドラー | [rust/src/models.rs:105–117](rust/src/models.rs#L105-L117) / [rust/src/routing.rs:1–36](rust/src/routing.rs#L1-L36) |
| Rust | ドメイン投影 | [rust/src/models.rs:59–66](rust/src/models.rs#L59-L66) / [rust/src/projection.rs:1–38](rust/src/projection.rs#L1-L38) |
| Rust | ユースケース | [rust/src/application.rs:9–15](rust/src/application.rs#L9-L15) / [rust/src/application.rs:20–32](rust/src/application.rs#L20-L32) / [rust/src/models.rs:68–77](rust/src/models.rs#L68-L77) / [rust/src/models.rs:141–150](rust/src/models.rs#L141-L150) |
| Rust | アプリ設定・JSON・CLI | [rust/src/application.rs:1–8](rust/src/application.rs#L1-L8) / [rust/src/application.rs:16–19](rust/src/application.rs#L16-L19) / [rust/src/main.rs:1–10](rust/src/main.rs#L1-L10) |
| Rust | 共通ライブラリ | [rust/src/dcb.rs:1–110](rust/src/dcb.rs#L1-L110) |
| TypeScript＋Zod | Model | [typescript/models.ts:1–48](typescript/models.ts#L1-L48) / [typescript/models.ts:51–51](typescript/models.ts#L51-L51) / [typescript/models.ts:54–54](typescript/models.ts#L54-L54) / [typescript/models.ts:67–84](typescript/models.ts#L67-L84) / [typescript/models.ts:100–100](typescript/models.ts#L100-L100) |
| TypeScript＋Zod | Decider | [typescript/decider.ts:1–96](typescript/decider.ts#L1-L96) / [typescript/models.ts:85–89](typescript/models.ts#L85-L89) |
| TypeScript＋Zod | コマンド・ハンドラー | [typescript/models.ts:55–66](typescript/models.ts#L55-L66) / [typescript/routing.ts:1–42](typescript/routing.ts#L1-L42) |
| TypeScript＋Zod | ドメイン投影 | [typescript/models.ts:49–50](typescript/models.ts#L49-L50) / [typescript/projection.ts:1–50](typescript/projection.ts#L1-L50) |
| TypeScript＋Zod | ユースケース | [typescript/application.ts:7–7](typescript/application.ts#L7-L7) / [typescript/application.ts:12–30](typescript/application.ts#L12-L30) / [typescript/models.ts:52–53](typescript/models.ts#L52-L53) / [typescript/models.ts:90–99](typescript/models.ts#L90-L99) |
| TypeScript＋Zod | アプリ設定・JSON・CLI | [typescript/application.ts:1–6](typescript/application.ts#L1-L6) / [typescript/application.ts:8–11](typescript/application.ts#L8-L11) / [typescript/main.ts:1–5](typescript/main.ts#L1-L5) / [typescript/models.ts:101–101](typescript/models.ts#L101-L101) / [typescript/wire.ts:1–4](typescript/wire.ts#L1-L4) |
| TypeScript＋Zod | 共通ライブラリ | [typescript/dcb.ts:1–93](typescript/dcb.ts#L1-L93) |
| Java | Model | [java/Models.java:1–45](java/Models.java#L1-L45) / [java/Models.java:61–143](java/Models.java#L61-L143) / [java/Models.java:210–285](java/Models.java#L210-L285) / [java/Models.java:303–303](java/Models.java#L303-L303) |
| Java | Decider | [java/Decider.java:1–108](java/Decider.java#L1-L108) / [java/Models.java:286–302](java/Models.java#L286-L302) |
| Java | コマンド・ハンドラー | [java/Models.java:144–209](java/Models.java#L144-L209) / [java/Routing.java:1–34](java/Routing.java#L1-L34) |
| Java | ドメイン投影 | [java/Models.java:46–53](java/Models.java#L46-L53) / [java/Projection.java:1–53](java/Projection.java#L1-L53) |
| Java | ユースケース | [java/Application.java:6–6](java/Application.java#L6-L6) / [java/Application.java:10–26](java/Application.java#L10-L26) / [java/Models.java:54–60](java/Models.java#L54-L60) / [java/Models.java:304–343](java/Models.java#L304-L343) |
| Java | アプリ設定・JSON・CLI | [java/Application.java:1–5](java/Application.java#L1-L5) / [java/Application.java:7–9](java/Application.java#L7-L9) / [java/Main.java:1–12](java/Main.java#L1-L12) |
| Java | 共通ライブラリ | [java/playground/dcb/Dcb.java:1–107](java/playground/dcb/Dcb.java#L1-L107) |
| Go | Model | [go/domain/models.go:1–80](go/domain/models.go#L1-L80) / [go/domain/models.go:95–168](go/domain/models.go#L95-L168) / [go/domain/models.go:233–307](go/domain/models.go#L233-L307) / [go/domain/models.go:320–320](go/domain/models.go#L320-L320) |
| Go | Decider | [go/domain/decider.go:1–279](go/domain/decider.go#L1-L279) / [go/domain/models.go:308–319](go/domain/models.go#L308-L319) |
| Go | コマンド・ハンドラー | [go/domain/models.go:169–232](go/domain/models.go#L169-L232) / [go/domain/routing.go:1–81](go/domain/routing.go#L1-L81) |
| Go | ドメイン投影 | [go/domain/models.go:81–86](go/domain/models.go#L81-L86) / [go/domain/projection.go:1–190](go/domain/projection.go#L1-L190) |
| Go | ユースケース | [go/application.go:25–25](go/application.go#L25-L25) / [go/application.go:31–98](go/application.go#L31-L98) / [go/domain/models.go:87–94](go/domain/models.go#L87-L94) / [go/domain/models.go:321–356](go/domain/models.go#L321-L356) |
| Go | アプリ設定・JSON・CLI | [go/application.go:1–24](go/application.go#L1-L24) / [go/application.go:26–30](go/application.go#L26-L30) / [go/domain/json.go:1–694](go/domain/json.go#L1-L694) / [go/main.go:1–30](go/main.go#L1-L30) / [go/wire.go:1–52](go/wire.go#L1-L52) |
| Go | 共通ライブラリ | [go/dcb/store.go:1–295](go/dcb/store.go#L1-L295) |
| Swift | Model | [swift/models.swift:1–33](swift/models.swift#L1-L33) / [swift/models.swift:48–64](swift/models.swift#L48-L64) / [swift/models.swift:76–87](swift/models.swift#L76-L87) |
| Swift | Decider | [swift/decider.swift:1–98](swift/decider.swift#L1-L98) / [swift/models.swift:88–91](swift/models.swift#L88-L91) |
| Swift | コマンド・ハンドラー | [swift/models.swift:65–75](swift/models.swift#L65-L75) / [swift/routing.swift:1–35](swift/routing.swift#L1-L35) |
| Swift | ドメイン投影 | [swift/models.swift:34–39](swift/models.swift#L34-L39) / [swift/projection.swift:1–49](swift/projection.swift#L1-L49) |
| Swift | ユースケース | [swift/application.swift:4–4](swift/application.swift#L4-L4) / [swift/application.swift:8–22](swift/application.swift#L8-L22) / [swift/models.swift:40–47](swift/models.swift#L40-L47) / [swift/models.swift:92–99](swift/models.swift#L92-L99) |
| Swift | アプリ設定・JSON・CLI | [swift/application.swift:1–3](swift/application.swift#L1-L3) / [swift/application.swift:5–7](swift/application.swift#L5-L7) / [swift/main.swift:1–9](swift/main.swift#L1-L9) / [swift/wire.swift:1–338](swift/wire.swift#L1-L338) |
| Swift | 共通ライブラリ | [swift/dcb.swift:1–106](swift/dcb.swift#L1-L106) |

<!-- code-stats:end -->

```bash
python3 samples/decider_level3/code_stats.py --write
python3 samples/decider_level3/code_stats.py --check
python3 samples/decider_level3/code_stats.py --json
python3 samples/decider_level3/check_readmes.py
```
