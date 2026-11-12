# C#: Level4 HTTP API

[共通仕様・実行・コード量](../README.md)／[OpenAPI](../openapi.json)／[再利用するLevel3](../../decider_level3/csharp)。学校ドメイン・VO・DeciderはLevel3から参照し、コピーしていません。

ホストはASP.NET Core／Kestrelのminimal hostingです。HTTP処理はapp.Runの共通RequestDelegateでディスパッチしており、MapGet／MapPostでエンドポイントを定義する形式ではありません。

## 今回良かったところ

良かったのは、closed recordのコマンド・イベントとproperty attributeによる入力検証を変更せずに、ASP.NET Coreから同じExecutorを呼べたことです。失敗時にイベントが出ないという保証をHTTPホストへ書き直す必要がありません。JSONも既存のclosed階層をSource Generationで扱い、ケースごとのコンバーターを追加していません。

[csharp/Program.cs:19–21](Program.cs#L19-L21)

```csharp
var executor = new DcbExecutor<Event, Rejection>(store); var handler = new SchoolHandler();
var tickets = new Dictionary<string, (long Created, PreparedCommand<Rejection> Command)>(); var ticketGate = new object();
var ttl = double.Parse(Environment.GetEnvironmentVariable("TICKET_TTL_SECONDS") ?? "60", System.Globalization.CultureInfo.InvariantCulture);
```

## 実装で増えた仕事

HTTPにしたことで、JSONの構造エラー400とドメイン入力の拒否422を分ける仕事は増えました。Source Generationの登録だけでは未知のプロパティ、追加のAPI制約、全言語共通のエラー応答まで揃いません。OpenAPIから読む構造検査と応答用JSONの組み立てを境界に置いています。準備チケットは参照型で、Commitを型だけで一度に制限しません。HTTP側で辞書から除去してから確定します。

[csharp/Program.cs:89–92](Program.cs#L89-L92)

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, AllowOutOfOrderMetadataProperties = true, RespectNullableAnnotations = true, RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(Command))]
[JsonSerializable(typeof(Event))]
[JsonSerializable(typeof(SchoolList))]
```

## 他の実装と比較すると

F#と同じ.NETストア・SQLiteドライバー・ASP.NET Coreを使っています。C#ではcommandのclosed階層、Handler、イベントの型はそのまま使え、nullable制約とrequired constructorの検査もJSON境界で有効にできます。F#にはケースに伴うデータをDUでまとめる利点がありますが、今回の共通wire形式ではfieldless DUをオブジェクトへ合わせる処理が必要でした。C#はこの変換を必要としない一方、JSON contextのルート型登録は残ります。どちらもこのLevel4ではRelease JIT＋Dynamic PGOで、Native AOTのHTTP測定を行ったとは主張していません。

[共通HTTPテスト](../tests/http_tests.py)では入力拒否・競合再判断・投影反映・SQLite再起動と相互復元を検証します。[ローカルの測定結果](../benchmark/observed/README.md)は、このホスト・codec・ストアを含む一連の実装の比較です。
