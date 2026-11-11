# Rust：enum＋Serde＋Result

ケースの定義、JSONのタグ、失敗を返す型を少ない宣言に集めています。一方、今回は入力Stateを借用して新Stateを返すため、更新時のcloneがはっきり現れます。

対象はこのClassroom Deciderの実装です。言語一般の順位ではありません。[共通仕様・検証](../README.md)／[コード量の集計](../README.md#コード量の比較)も併せて参照してください。以下の引用は現行ソースの抜粋で、出典リンクから該当行へ移動できます。

## Pros

### 1. ケースとJSON登録を一か所に集められる

出典：[rust/src/models.rs](src/models.rs#L11-L16)

```rust
#[serde(tag = "$type")]
pub enum State {
    NotCreated,
    Available { roster: Roster },
    Full { roster: Roster },
}
```
StateのenumとSerdeのタグが同じ場所にあります。[TypeScriptのZodスキーマ](../typescript/models.ts#L6)と同様にケース宣言をまとめられます。[Java](../java/Models.java#L15)のpermitsとJsonSubTypesの別リスト管理と比べ、復元対象を登録する手間が少ない実装です。Serialize／Deserializeはderiveで生成し、JSONの手書きswitchはありません。

### 2. 入力を変更しないAPIと、失敗の伝播を型に表せる

出典：[rust/src/decider.rs](src/decider.rs#L17-L17)

```rust
pub fn decide(state: &State, command: &Command) -> Result<Vec<Event>, &'static str> {
```
出典：[rust/src/decider.rs](src/decider.rs#L39-L39)

```rust
    let roster = data(state).ok_or("not_created")?;
```
&StateからこのRosterのString／Vecを直接書き換えることはできず、新しいイベントやStateを返します。Resultと?は標準機能で、[csharp/Models.cs](../csharp/Models.cs#L26)や[java/Models.java](../java/Models.java#L42)のような独自の成功／失敗クラスは不要です。[go/domain/decider.go](../go/domain/decider.go#L8)の2つの戻り値と違い、Resultは成功か失敗のどちらかを表します。

### 3. ケース追加と不在を明示的に扱える

[rust/src/decider.rs](src/decider.rs#L3)のmatchにワイルドカードはなく、[rust/src/decider.rs](src/decider.rs#L10)はNotCreatedにOption::Noneを返します。State自身にはnull参照に相当する通常の値がありません。これは空席数などの業務条件まで型が保証するという意味ではありません。

## Cons

### 1. 今回の不変更新はRoster全体をcloneする

出典：[rust/src/decider.rs](src/decider.rs#L90-L92)

```rust
                let mut next = roster.clone();
                next.students.push(student_id.clone());
                Ok(classify(next))
```
RosterはStringとVec<String>を持つため、このcloneはポインターを渡すだけではありません。[VecのClone](https://doc.rust-lang.org/std/vec/struct.Vec.html#impl-Clone-for-Vec%3CT,+A%3E)は要素をcloneします。学生数に応じた複製があり、[swift/decider.swift](../swift/decider.swift#L35)の値型配列の更新と同じコストだとは言えません。この6言語サンプルでは性能を測っていません。所有権を受け取るAPIなどにも変えられますが、ここでは旧Stateを残す共通仕様を優先しています。

### 2. フィールド名・タグ・補助タグの宣言は残る

出典：[rust/src/models.rs](src/models.rs#L19-L24)

```rust
#[serde(tag = "$type", rename_all_fields = "camelCase")]
pub enum Command {
    Create { name: String, capacity: i32 },
    Enroll { student_id: String },
    Drop { student_id: String },
}
```
snake_caseのstudent_idと共通JSONのstudentIdを合わせる設定が必要です。また[rust/src/models.rs](src/models.rs#L34)は出力用のタグを別のmatchで返しています。Serdeがすべての関連コードを消しているわけではありません。デフォルトの型判別子はvariant名なので、保存形式を維持したい改名ではserdeのrename指定も検討対象です。

### 3. 拒否コードは型付きenumにしていない

署名の&\x27static strは文字列を指し、誤った綴りもコンパイルできます。[swift/models.swift](../swift/models.swift#L43)のようにエラー集合を定義した版とは違います。さらに、pubなRosterとStateのvariantから、空の学生リストを持つFullも組み立てられます。enumが閉じていることと、保持データの業務検証は別です。
