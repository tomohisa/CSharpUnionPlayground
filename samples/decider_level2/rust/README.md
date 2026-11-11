# Rust：SerdeとResultで履修条件と履歴検査を共有する

[共通仕様](../README.md)／[コード量](../README.md#コード量の比較)。対象はこの履修ドメインの実装です。言語一般の順位や、測っていない速度の比較ではありません。引用は現在のソースとCIで照合します。

## この複雑さでよかったこと

### 1. ネストした状態にも同じenumとSerdeの宣言を使える

出典：[rust/src/models.rs](src/models.rs#L68-L74)

```rust
pub enum Progress {
    Reserved,
    Studying { attendance: i32 },
    Passed { score: i32, credits: i32 },
    Failed { score: i32, reason: FailureReason },
    Withdrawn { reason: WithdrawalReason },
}
```

class、progress、prerequisiteのケース登録を別のwireファイルへ重複させません。[swift/wire.swift](../swift/wire.swift#L31)やGoのタグ別復元より、このJSON契約を短く扱えました。

### 2. 見つからない状態をResultで伝播できる

出典：[rust/src/decider.rs](src/decider.rs#L25-L26)

```rust
            let student = s.students.iter().find(|x| x.student_id == *student_id).ok_or(UnknownStudent)?;
            if rank(*grade) != rank(student.grade) + 1 { return Err(InvalidPromotion); }
```

OptionをUnknownStudentに変換し、?でそこで判断を返します。validateはResult<Event, Rejection>、CLIへ返すdecideはAccepted／Rejectedです。nullや空のStudentを用意しなくて済みました。

### 3. 履歴検査にも同じルールを使い、変更は所有したコピーに限定できる

出典：[rust/src/decider.rs](src/decider.rs#L107-L110)

```rust
pub fn evolve(state: &State, event: &Event) -> Result<State, Rejection> {
    validate(state, &cause(event)).map_err(|_| Rejection::InvalidHistory)?;
    let mut s = state.clone();
    match event {
```

外から受け取るStateは借用で、更新するsは所有した値です。新しい履修状態のdescribe分岐漏れはE0004で拒否されます。[go/domain/decider.go](../go/domain/decider.go#L266)のようなslice共有の管理とは異なる境界を型で表せます。

## 実装で増えた負担・残る制約

### 1. 旧Stateを保持する単純な実装では学校全体をcloneした

出典：[rust/src/decider.rs](src/decider.rs#L109-L112)

```rust
    let mut s = state.clone();
    match event {
        Event::StudentRegistered {student_id,grade} => { s.students.push(Student {student_id: student_id.clone(), grade: *grade}); return Ok(s); }
        Event::StudentPromoted {student_id,grade} => { s.students.iter_mut().find(|x| x.student_id == *student_id).expect("invariant").grade = *grade; return Ok(s); }
```

StudentsだけでなくClasses・Enrollmentsと内部のString／Vecもcloneします。借用チェックで安全になることと、このコピー量が小さいことは別です。[csharp/Decider.cs](../csharp/Decider.cs#L125)の変更対象中心の更新より大きくコピーする選択です。所有権を受け取るAPIや永続コレクションとの比較はしていません。

### 2. 共通の名簿取得をVecとして返したため検査でもコピーする

出典：[rust/src/decider.rs](src/decider.rs#L8-L13)

```rust
fn members(room: &ClassState) -> Vec<String> {
    match room {
        ClassState::Available {students,..} | ClassState::Full {students,..} | ClassState::Teaching {students,..} => students.clone(),
        ClassState::Completed {results,..} => results.iter().map(|r| r.student_id.clone()).collect()
    }
}
```

登録可否のcontainsでもこのヘルパーを呼びます。実装を揃えるための単純化で、借用したsliceやiteratorを使う改善余地があります。クラス終了後はresultsから生徒IDを作るため、戻り値を一律の借用sliceにできない点もこのドメイン固有の負担です。

### 3. enumは状態を閉じるが、各値の妥当性までは閉じない

出典：[rust/src/models.rs](src/models.rs#L46-L49)

```rust
pub struct ClassResult {
    pub student_id: String,
    pub progress: Progress,
}
```

Progress型にReservedも含まれるため、ClassResultに未採点の値を直接入れることは型だけでは防げません。公開フィールドの点数・定員・IDにもsmart constructorはありません。今回のテストは空のStateから検証したイベントで到達する状態を対象にしています。
