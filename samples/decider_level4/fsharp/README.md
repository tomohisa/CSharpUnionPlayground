# F#: Level4 HTTP API

[共通仕様・実行・コード量](../README.md)／[OpenAPI](../openapi.json)／[再利用するLevel3](../../decider_level3/fsharp)。学校ドメイン・VO・DeciderはLevel3から参照し、コピーしていません。

## 今回良かったところ

DUによるフェーズ・イベント・コマンドと純粋なDeciderを保ち、HTTPからもHandlers.schoolをExecutorへ渡せます。JSONのwire形式とHTTPの都合がDeciderへ入らないのは良かった点です。nullable警告をエラーにしているため、環境変数・JSONデコードなどの境界でnonNullや明示的な分岐が必要になり、ドメイン内部へのnullの持ち込みも見つけやすくなりました。

[fsharp/Program.fs:69–71](Program.fs#L69-L71)

```fsharp
let executor=DcbExecutor<Event,Rejection>(store,3)
let tickets=Dictionary<string,int64*PreparedCommand<Rejection>>()
let ticketGate=obj()
```

## 実装で増えた仕事

C#と同じHTTPスタックを使えても、HTTPのtask、ASP.NETのRequestDelegate、可変のチケット辞書、ロックは命令的なコードになります。DUを使えばHTTPホストまで短くなる、という結果にはなりませんでした。特に.NET 11のfieldless DUのJSONはReservedやNoPrerequisiteを文字列にするため、共通の{$type:...}へ合わせる再帰的なnormalizeが必要です。一覧ではその変換も測定時間に含まれます。

[fsharp/Program.fs:48–54](Program.fs#L48-L54)

```fsharp
let rec normalize name (value:JsonNode)=
    match value with
    | :? JsonObject as obj->
        for key in obj |> Seq.map(fun p->p.Key) |> Seq.toArray do
            let child=obj[key] |> nonNull
            obj.Remove(key) |> ignore
            obj[key]<-normalize key child
```

## 他の実装と比較すると

C#のclosed record＋inferred polymorphismはこのwire契約と素直に合います。F#はモデルとDeciderの記述に向き、Resultやoptionで欠けた値を型に表せますが、今回のHTTP用JSONではそのままのシリアライズ結果を採用できませんでした。C#の専用context登録と、F#のreflectionを使う既定codec＋normalizeは異なる実装です。HTTPスタックと.NET世代は共通でも、結果の差を言語構文だけの差にはできません。

[共通HTTPテスト](../tests/http_tests.py)では入力拒否・競合再判断・投影反映・SQLite再起動と相互復元を検証します。[ローカルの測定結果](../benchmark/observed/README.md)は、このホスト・codec・ストアを含む一連の実装の比較です。
