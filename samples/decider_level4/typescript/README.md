# TypeScript＋Zod: Level4 HTTP API

[共通仕様・実行・コード量](../README.md)／[OpenAPI](../openapi.json)／[再利用するLevel3](../../decider_level3/typescript)。学校ドメイン・VO・DeciderはLevel3から参照し、コピーしていません。

## 今回良かったところ

HTTPのJSONを型assertionだけでCommandと扱わず、CommandSchema.parseしてから既存Handlerへ渡します。イベントの復元もEventSchema.parseする既存ストアを再利用しています。任意のJSON入力を受けるAPIになったので、Zodを使う理由がLevel3より明確になりました。

[typescript/server.ts:52–54](server.ts#L52-L54)

```typescript
      const t=executor.prepare(input.commandId,command,schoolHandler);
      if(route==="/commands"||!t.wasRead)({code,body}=commit(t));
      else{expire();if(tickets.size>=1024)throw new HttpError(429,"TooManyTickets");const id=randomUUID();tickets.set(id,{created:performance.now(),command:t});code=201;body={ticket:id,position:t.after};}
```

## 実装で増えた仕事

型検査だけでは入力JSONを守れません。OpenAPIによる構造・追加プロパティのチェックとZodによる型復元は目的が重なりますが、共通のHTTP応答と実装内部の型を両方揃えるため残しています。また、node:sqliteとドメイン処理は同期で、リクエスト処理中はNodeのイベントループを占有します。Promiseやasync関数にしただけでCPU処理やSQLiteが並列になるわけではありません。

[typescript/server.ts:51–52](server.ts#L51-L52)

```typescript
      validate(input,schemas.CommandRequest);const command=CommandSchema.parse(input.command);
      const t=executor.prepare(input.commandId,command,schoolHandler);
```

## 他の実装と比較すると

Java・Goのように複数リクエストのDeciderを別スレッドで走らせる構成ではありません。準備と確定を分けたAPIで古い版の再判断は検証できますが、このホストでは通常のコマンド処理はイベントループ上で直列です。worker threadsを使う別実装の性能は測っていません。

[共通HTTPテスト](../tests/http_tests.py)では入力拒否・競合再判断・投影反映・SQLite再起動と相互復元を検証します。[ローカルの測定結果](../benchmark/observed/README.md)は、このホスト・codec・ストアを含む一連の実装の比較です。
