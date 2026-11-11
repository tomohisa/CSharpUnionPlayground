# Swift：enum＋値型＋Codableの明示マッピング

フェーズ、関連データ、型付きの失敗をenumで表せます。この例のDeciderは短く読めますが、他言語と同じJSON契約を保つためのWireコードは別に必要です。

対象はこのClassroom Deciderの実装です。言語一般の順位ではありません。[共通仕様・検証](../README.md)／[コード量の集計](../README.md#コード量の比較)も併せて参照してください。以下の引用は現行ソースの抜粋で、出典リンクから該当行へ移動できます。

## Pros

### 1. 関連値付きenumで、フェーズと保持データを一緒に表せる

出典：[swift/models.swift](models.swift#L7-L9)

```swift
    case notCreated
    case available(Roster)
    case full(Roster)
```
NotCreatedにダミーのRosterを持たせません。[csharp/Models.cs](../csharp/Models.cs#L9)や[java/Models.java](../java/Models.java#L15)の基底型と派生型を個別宣言する方式より、ケース一覧がまとまっています。[swift/models.swift](models.swift#L11)とevolveのswitchにdefaultはなく、通常の再コンパイルでケース漏れを検査できる形です。

### 2. Resultとenumで、拒否の集合も表せる

出典：[swift/decider.swift](decider.swift#L1-L1)

```swift
func decide(_ state: State, _ command: Command) -> Result<[Event], Rejection> {
```
出典：[swift/models.swift](models.swift#L43-L52)

```swift
enum Rejection: String, Error {
    case notCreated = "not_created"
    case alreadyCreated = "already_created"
    case invalidName = "invalid_name"
    case invalidCapacity = "invalid_capacity"
    case invalidStudent = "invalid_student"
    case alreadyEnrolled = "already_enrolled"
    case full = "full"
    case notEnrolled = "not_enrolled"
}
```
C#やJavaのRejected(string)、Rustの&'static str、TypeScriptのerror:stringと比べ、今回のSwift版は拒否コードの選択までenumで限定しています。文字列への変換はCLIのrawValueで行い、業務処理では.success／.failureを扱います。

### 3. 入力のStateを直接変更せず、更新後の値を作れる

出典：[swift/decider.swift](decider.swift#L35-L35)

```swift
        return classify(Roster(name: r.name, capacity: r.capacity, students: r.students + [student]))
```
Rosterはletフィールドを持つstructで、studentsは[String]です。[go/domain/decider.go](../go/domain/decider.go#L76)の背後の配列共有を切る処理や、[rust/src/decider.rs](../rust/src/decider.rs#L90)の明示的なRoster全体のcloneとは書き方が違います。Swiftの標準配列は[copy-on-write](https://github.com/swiftlang/swift/blob/main/docs/OptimizationTips.rst)を使いますが、上の連結でコピーや割当が不要になると保証するものではありません。

## Cons

### 1. 同じ$type形式へ揃えるには、Codableを手で書く必要があった

出典：[swift/wire.swift](wire.swift#L45-L54)

```swift
extension State: Codable {
    init(from decoder: Decoder) throws {
        let c = try decoder.container(keyedBy: Keys.self)
        switch try c.decode(String.self, forKey: .type) {
        case "NotCreated": self = .notCreated
        case "Available": self = .available(try c.decode(Roster.self, forKey: .roster))
        case "Full": self = .full(try c.decode(Roster.self, forKey: .roster))
        default: throw unknown(c)
        }
    }
```
このサンプルはCodableの標準合成形式をそのまま使っていません。共通の$type形式へ合わせるため、Command／Event／Stateをwire.swiftで対応させています。[rust/src/models.rs](../rust/src/models.rs#L11)や[csharp/Models.cs](../csharp/Models.cs#L8)と比べて、この契約で必要な手書きコードは多くなっています。SwiftのenumだからこのJSONを自動で読める、とはしていません。

### 2. enumの網羅性と、文字列タグのdecodeは別の管理になる

上のdecodeはStringに対するswitchとdefaultです。Eventの追加で型付きencodeのswitchは検査できますが、decode側の新タグの追加漏れまで自動でコンパイルエラーになるわけではありません。[go/wire.go](../go/wire.go#L37)と共通する保守箇所です。[TypeScriptのZod版](../typescript/models.ts#L16)はスキーマから型を導出するため、この二重管理を避けています。

### 3. Deciderだけの短さは、コード配置にも左右される

出典：[swift/models.swift](models.swift#L18-L24)

```swift
    var roster: Roster? {
        switch self {
        case .notCreated: return nil
        case .available(let r), .full(let r): return r
        }
    }
}
```
SwiftではphaseとrosterアクセサーをModelsに置いています。[csharp/Decider.cs](../csharp/Decider.cs#L14)や[rust/src/decider.rs](../rust/src/decider.rs#L10)では同じ役割がDecider側です。したがって、共通表のDecider行数だけで「Swiftなら同じ処理が最も短い」とは言えません。更新式でもname／capacityは再指定しており、C#のwithのような構文ではありません。

letやenumであっても、不正なRosterを持つFullまで構築不能にはしていません。業務条件は他言語と同様にdecide／evolveで守ります。
