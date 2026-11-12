# Rust: Level4 HTTP API

[共通仕様・実行・コード量](../README.md)／[OpenAPI](../openapi.json)／[再利用するLevel3](../../decider_level3/rust)。学校ドメイン・VO・DeciderはLevel3から参照し、コピーしていません。

## 今回良かったところ

既存のenumとserdeのタグ付きJSONをそのまま使えます。HTTPをまたいで保持する準備チケットにはOwnedPreparedを共通DCB側に追加しました。Arcでストア・Handlerを保持し、クロージャーはSendに限定します。commit(self)がチケットを消費するため、一度取り出したチケットの二重Commitをコンパイラーで防げます。

[rust/src/main.rs:172–176](src/main.rs#L172-L176)

```rust
            let t = prepare_owned(
                self.store.clone(),
                input["commandId"].as_str().unwrap().into(),
                command,
                Arc::new(handlers::SchoolHandler),
```

## 実装で増えた仕事

Level3の借用を持つPreparedCommandをそのまま共有辞書へ入れることはできませんでした。ストアをリークしたり、ドメイン型へ依存する汎用Eventを作ったりせず、所有権を持つ準備APIをライブラリに追加する必要がありました。C#・F#・Javaよりこの部分の設計負担は大きいです。加えてHTTP入力からserdeへ渡す前にOpenAPIの構造検査、URLのpercent decode、整数表現の調整を行っています。

[../decider_level3/rust/src/dcb.rs:168–172](../../decider_level3/rust/src/dcb.rs#L168-L172)

```rust
pub struct OwnedPrepared<R> {
    pub after:usize, pub was_read:bool,
    action:Box<dyn FnOnce()->HandlingResult<R>+Send>
}
impl<R> OwnedPrepared<R>{pub fn commit(self)->HandlingResult<R>{(self.action)()}}
```

## 他の実装と比較すると

LinuxではAxum/Tokioで持続接続を処理し、SQLite・Decider・投影の同期処理をspawn_blockingへ移しています。Semaphoreで同時実行を8に制限し、asyncの実行スレッドを同期I/Oで塞ぎません。以前のローカル結果はtiny_http版であり、Linuxの結果と実装差があります。今回の速度はこのホストとSQLite・serde・投影を含む実装の観測です。enumの型安全性と所有権の恩恵は大きいですが、HTTP framework選択までRustという言語の性能だと読み替えないでください。

[共通HTTPテスト](../tests/http_tests.py)では入力拒否・競合再判断・投影反映・SQLite再起動と相互復元を検証します。[ローカルの測定結果](../benchmark/observed/README.md)は、このホスト・codec・ストアを含む一連の実装の比較です。
