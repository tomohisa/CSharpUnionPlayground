# Rust：enumを保ち、traitでコマンドストアを分離する

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

## この設計でよかったこと

### 1. 動的なのは振る舞いで、イベント本体は型Eのまま

出典：[rust/src/dcb.rs](src/dcb.rs#L21-L24)

```rust
pub trait CommandStore<E:Clone+Send>:Sync {
    fn read(&self,query:&Query)->ReadResult<E>;
    fn append(&self,command_id:&str,condition:&Query,after:usize,batch:Vec<E>)->AppendResult<E>;
}
```

出典：[rust/src/routing.rs](src/routing.rs#L32-L35)

```rust
pub fn decide(events:&[Recorded<Event>],command:&Command)->Preparation<Event,Rejection>{
    match decider::decide(&projection::decision_model(&projection::fold(events)),command){
        Decision::Accepted{events}=>Preparation::Emit(events),Decision::Rejected{code}=>Preparation::Reject(code)
    }
```

dyn CommandStoreやクロージャを使っても、payloadをdyn Anyへ変えていません。Event enumとRejectionを型引数として保持します。別のCounter型だけを含む独立crateで同じ部品を検証します。

### 2. 所有権でストアと読み取り結果の境界を明示できる

出典：[rust/src/dcb.rs](src/dcb.rs#L31-L34)

```rust
    fn copy(&self,e:&Recorded<E>)->Recorded<E>{Recorded{position:e.position,command_id:e.command_id.clone(),event_type:e.event_type.clone(),tags:e.tags.clone(),payload:(self.copy_payload)(&e.payload)}}
    pub fn read(&self, query: &Query) -> ReadResult<E> {
        let s = self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner);
        ReadResult {after: s.log.len(), events: s.log.iter().filter(|e| query.matches(&e.event_type,&e.tags)).map(|e|self.copy(e)).collect()}
```

呼び出し側へ返すペイロードのコピー方法を注入します。CloneだけではArcなどの内部可変データの独立性を保証しないため、コピーの契約を明示しました。学校のenumは所有したString／Vecで、Counterも値として複製します。

### 3. 複合キーとBTreeMapで一覧の順序と区別を揃えられる

出典：[rust/src/projection.rs](src/projection.rs#L4-L7)

```rust
pub struct Registers { pub students:BTreeMap<String,Student>, pub classes:BTreeMap<String,ClassState>, pub enrollments:BTreeMap<(String,String),Enrollment> }
pub fn apply_student(s:&mut Registers,e:&Event) {
    match e {Event::StudentRegistered {student_id,grade}|Event::StudentPromoted {student_id,grade}=>{s.students.insert(student_id.clone(),Student{student_id:student_id.clone(),grade:*grade});},_=>{}}
}
```

履修キーは2つのStringのtupleです。区切り文字の連結で衝突しません。Mutexで条件検査とバッチ追加を守り、独立テストでは本当の2スレッドを同じ読み取り位置から競合させています。

## 実装で大変だったこと・残る制約

### 1. 汎用化するとSendとクロージャの所有関係もAPIになる

出典：[rust/src/dcb.rs](src/dcb.rs#L85-L88)

```rust
pub struct CommandProcessor<'a,C,E:Clone+Send,R> {
    pub store:&'a dyn CommandStore<E>, pub query_for:&'a dyn Fn(&C)->Query,
    pub decide:&'a dyn Fn(&[Recorded<E>],&C)->Preparation<E,R>, pub max_conflicts:usize
}
```

設定を捕捉できるようfnポインターだけのAPIから変更しました。C#／F#の関数注入より、Send／Sync、Box、借用期間を表に出す箇所が多くなります。イベント型を失う代わりに簡単にする選択はしていません。

### 2. 投影スナップショットはMapを含めてcloneする

出典：[rust/src/dcb.rs](src/dcb.rs#L80-L80)

```rust
    pub fn snapshot(&self) -> (usize,S) { self.inner.lock().unwrap_or_else(std::sync::PoisonError::into_inner).clone() }
```

独立性を優先した実装です。タグで読むコマンド用のモデルは部分的ですが、一覧投影のコピーは全件です。安全な所有権があることと、複製量が小さいことは別で、このサンプルでは速度やメモリを測っていません。

### 3. ロック中の購読panicを分離しないと、コミット結果とMutexが壊れる

出典：[rust/src/dcb.rs](src/dcb.rs#L48-L50)

```rust
        s.listeners.retain(|listener| std::panic::catch_unwind(std::panic::AssertUnwindSafe(||listener(&committed.iter().map(|e|self.copy(e)).collect::<Vec<_>>()))).unwrap_or(false));
        s.faults += before - s.listeners.len();
        AppendResult::Committed(committed.iter().map(|e|self.copy(e)).collect())
```

レビューを受けてunwindを捕捉し、追記済みなのに失敗に見えたり、Mutexがpoisonされたりすることを防ぎました。abort設定のpanicや不正なコールバック契約まで回復できるわけではありません。
