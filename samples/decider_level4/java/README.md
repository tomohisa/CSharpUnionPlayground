# Java: Level4 HTTP API

[共通仕様・実行・コード量](../README.md)／[OpenAPI](../openapi.json)／[再利用するLevel3](../../decider_level3/java)。学校ドメイン・VO・DeciderはLevel3から参照し、コピーしていません。

## 今回良かったところ

Level3のsealed interfaceとrecord、共通Executorを変更せずにJDK HttpServerへ接続できました。virtual threadsで接続を処理し、共有ストアの条件付き追記とチケット辞書の同期だけを明示しています。ドメインロジックとHTTPの例外処理は分離できます。

[java/Server.java:41–43](Server.java#L41-L43)

```java
        var pool=Executors.newVirtualThreadPerTaskExecutor();server.setExecutor(pool);
        server.createContext("/",exchange->{
            Reply reply;
```

## 実装で増えた仕事

Jacksonはイベント・コマンドの型判別子を扱えますが、HTTP入力を全言語と同じ規則で拒否するためにはJsonNodeでの構造チェックも必要でした。可変のMapで応答を組み立てる箇所にはsealed階層の網羅性検査は及びません。C#のSource Generationと違い、このホストのJSON境界はreflectionを使うObjectMapperです。

[java/Server.java:12–13](Server.java#L12-L13)

```java
    static final ObjectMapper mapper=new ObjectMapper().enable(DeserializationFeature.FAIL_ON_NULL_FOR_PRIMITIVES,DeserializationFeature.FAIL_ON_MISSING_CREATOR_PROPERTIES);
    static JsonNode schemas;
```

## 他の実装と比較すると

HTTPの追加は比較的少ないコードで済みましたが、学校モデルのsealed/recordの宣言量はC#・F#・Rustより多く残っています。ここではJDK HttpServerを使い、SpringやNettyの挙動・性能は含めていません。Javaのvirtual threadsとRust/Swiftの固定8ワーカーは同じ実装ではありません。負荷側の同時接続数を共通にして比較します。

[共通HTTPテスト](../tests/http_tests.py)では入力拒否・競合再判断・投影反映・SQLite再起動と相互復元を検証します。[ローカルの測定結果](../benchmark/observed/README.md)は、このホスト・codec・ストアを含む一連の実装の比較です。
