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

出典：[src/handlers.rs](src/handlers.rs#L1-L14)

```rust
use crate::{dcb::*,models::*,routing,decider,projection,value_objects::ValidatedCommand};
pub struct SchoolHandler;
impl CommandHandler<Command,Event,Rejection> for SchoolHandler {
    fn validate(&self,c:&Command)->Vec<Rejection>{ValidatedCommand::create(c.clone()).err().into_iter().collect()}
    fn query_for(&self,c:&Command)->Query{routing::query_for(c)}
    fn handle(&self,events:&[Recorded<Event>],command:&Command)->Preparation<Event,Rejection>{
        match ValidatedCommand::create(command.clone()){
            Err(error)=>Preparation::Reject(error),
            Ok(validated)=>match decider::decide(&projection::decision_model(&projection::fold(events)),&validated){
                Decision::Accepted{events}=>Preparation::Emit(events),Decision::Rejected{code}=>Preparation::Reject(code)
            }
        }
    }
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

## コマンド・検証・履歴復元の更新

privateフィールドのnewtypeとTryFrom、Resultの?で制約を合成します。所有権を持つPreparedCommandはcommitで消費されます。学校型をライブラリへ登録しません。借用を保ったHandlerとストアの寿命が型へ現れる分、Executorのシグネチャとテスト用portはC#やJavaより複雑です。

[src/handlers.rs](src/handlers.rs)は入力検証とDecider接続、[src/value_objects.rs](src/value_objects.rs)は値の制約と検証済み入力、[src/domain_tests.rs](src/domain_tests.rs)はドメインテストです。Routingはタグ・読み取り条件・イベントのメタ情報を担当し、業務ハンドラーとは別分類です。

現在のポリシーで値を検証するcreate系factoryと、過去の事実を再検証しないfromEvent／restore系factoryを区別します。イベント・JSONの保存形と一覧DTOはプリミティブを保ち、投影で復元した値を取り出して使います。全てのStateの全フィールドを名目的なVOへ置き換えた実装ではありません。ValidatedCommandも永続化せず、新しいコマンドを受ける境界だけに使います。

76ケースのGWTではExecutor経由の結果と正確なイベントpayload、拒否時の無追記、投影と再構築の一致を確認します。これとは別に、ストアを使わないDeciderの3例、VOのfactoryと履歴復元、最後の1席の競合再判断もテストします。言語固有の不正入力ケースも追加しています。テスト全体をコード量の別枠へ計上しています。

## 大きな一覧Projectionの更新

学校全体のライブ一覧は[src/school_projection.rs](src/school_projection.rs)へ分離しました。既存のProjectionにある不変MapやコピーするReducerは、タグで絞ったコマンド判断用モデルと参照再生に残しています。ライブ一覧の更新には使いません。

ライブ一覧は汎用IndexedProjectionのハッシュ索引を使い、バッチ中に触った行だけを差分へ積み、全イベントを処理できてから反映します。クラスの名簿はイベント位置を内部ノードIDにした連結索引です。追加・取消で名簿全体をコピーせず、前後のノードだけを更新します。削除したノードは実際に索引から削除するため、同じ生徒の取消と再登録を繰り返してもノードを蓄積しません。

一覧要求時だけ名簿の配列／listと並び順を持つDTOを組み立てます。過去のDTOはその後のイベントで変わりません。全件Snapshotは出力サイズに比例する仕事があり、ロック中の読み取りもあるため、大規模な並行クエリまで一定時間という保証ではありません。頻繁な大量一覧の提供には、ページングや公開用Snapshotの別管理が必要です。

[src/projection_tests.rs](src/projection_tests.rs)では1千・1万・10万件を作り、更新と名簿拡大の時間を全件Snapshotと分けて測ります。途中失敗・位置の欠落で部分更新しないこと、再配送、過去Snapshotの不変性、取消と再登録後の順序と行数も検証します。[ローカルの測定値](../observed/projection-scaling.md)は言語の総合性能順位ではなく、この更新経路のサイズ依存を見るためのものです。
