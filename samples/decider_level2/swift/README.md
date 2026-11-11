# Swift：関連値付きenumで履修フェーズを表す

[共通仕様](../README.md)／[コード量](../README.md#コード量の比較)。対象はこの履修ドメインの実装です。言語一般の順位や、測っていない速度の比較ではありません。引用は現在のソースとCIで照合します。

## この複雑さでよかったこと

### 1. ケースと保持データを一つのenumで追える

出典：[swift/models.swift](models.swift#L38-L44)

```swift
enum Progress {
    case reserved
    case studying(attendance: Int)
    case passed(score: Int, credits: Int)
    case failed(score: Int, reason: FailureReason)
    case withdrawn(reason: WithdrawalReason)
}
```

未確定な点数をOptionalで持つより、StudyingとPassedを別ケースにできます。describeのswitchにはdefaultがなく、新状態の追加漏れがコンパイルエラーになります。[csharp/Models.cs](../csharp/Models.cs#L33)の複数recordより関連するケースをまとめて読めます。

### 2. 検索のOptionalを、その場で意味のある拒否に変換できる

出典：[swift/decider.swift](decider.swift#L27-L28)

```swift
        guard let student = state.students.first(where: { $0.studentId == id }) else { return reject(.unknownStudent) }
        return rank(grade) == rank(student.grade) + 1 ? accept(.studentPromoted(studentId: id, grade: grade)) : reject(.invalidPromotion)
```

Optionalは検索の境界だけで使い、Studentの存在しないダミー値を返しません。UnknownStudentとInvalidPromotionは別の拒否です。ドメインのStudent／ProgressにOptionalフィールドはありません。

### 3. 古いStateを保ちつつ、ローカルの更新を素直に書ける

出典：[swift/decider.swift](decider.swift#L112-L115)

```swift
    var updated = room, records = state.enrollments
    func change(_ student: String, _ progress: (Progress) -> Progress) -> [Enrollment] {
        records.map { e in e.classId == id && e.studentId == student ? Enrollment(classId: id, studentId: student, course: e.course, progress: progress(e.progress)) : e }
    }
```

structと配列の値セマンティクスで、結果用のローカル変数を更新して最後に新しいStateを返します。[go/domain/decider.go](../go/domain/decider.go#L266)のような共有sliceの切り離しを同じ形で書く必要はありません。ただし配列のコピーや割当の有無・量をこのテストでは測っていません。

## 実装で増えた負担・残る制約

### 1. 共通$type形式へのCodable対応が6種類に増えた

出典：[swift/wire.swift](wire.swift#L31-L43)

```swift
extension Progress: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case attendance
        case credits
        case reason
        case score
    }
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        let tag = try c.decode(String.self, forKey: .tag)
        switch tag {
        case "Reserved": self = .reserved
```

既定のCodable合成形式を使わず、他言語と同じタグ形式に合わせています。[rust/src/models.rs](../rust/src/models.rs#L68)や[typescript/models.ts](../typescript/models.ts#L26)よりJSON境界の記述が増えました。typedなencodeのswitchは検査できますが、Stringを読むdecodeのdefaultは新しいケースの対応漏れを検査しません。

### 2. 差分更新でも変更しないフィールドの再指定が多い

出典：[swift/decider.swift](decider.swift#L100-L100)

```swift
        return State(students: state.students.map { $0.studentId == id ? Student(studentId: id, grade: grade) : $0 }, classes: state.classes, enrollments: state.enrollments)
```

State、Enrollment、ClassStateを複合して更新するため、名前付き引数で作り直す箇所が増えました。[csharp/Decider.cs](../csharp/Decider.cs#L125)のwithやTypeScriptのspreadに比べた、この実装での負担です。

### 3. enumでも業務条件や壊れたStateへの安全性は別

出典：[swift/decider.swift](decider.swift#L71-L72)

```swift
    guard let record = state.enrollments.first(where: { $0.classId == id && $0.studentId == student }) else { preconditionFailure("invariant") }
    guard case .studying(let attendance) = record.progress else { return reject(.alreadyAssessed) }
```

検証したイベント経路で必ず存在する履修を、内部の不変条件として扱っています。公開モデルから不整合なStateを組み立てて渡すとpreconditionFailureへ進む余地があります。Intの点数・定員やStringのIDも、型だけで範囲や存在を保証する設計ではありません。
