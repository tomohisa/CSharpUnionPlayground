# Swift：関連値付きenumを公開generic moduleへ渡す

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

## この設計でよかったこと

### 1. primary associated typeでストアのpayloadを保てる

出典：[swift/dcb.swift](dcb.swift#L16-L20)

```swift
public protocol CommandStore<Payload>: Sendable {
    associatedtype Payload: Sendable
    func read(_ query:Query)->ReadResult<Payload>
    func append(_ commandId:String,_ condition:Query,_ after:Int,_ batch:[Payload])->AppendResult<Payload>
}
```

出典：[handlers.swift](handlers.swift#L1-L11)

```swift
import Foundation
struct SchoolHandler:CommandHandler {
    func validate(_ command:Command)->[Rejection]{ValidatedCommand.validate(command)}
    func queryFor(_ command:Command)->Query{schoolQueryFor(command)}
    func handle(_ events:[Recorded<Event>],_ command:Command)->Preparation<Event,Rejection>{
        do{
            let validated=try ValidatedCommand.create(command)
            switch decide(fold(events).decisionModel(),validated){case .accepted(let events):return .emit(events);case .rejected(let code):return .reject(code)}
        }catch let error as Rejection{return .reject(error)}catch{preconditionFailure("unexpected validator error")}
    }
}
```

学校のEvent enumのままCommandStore<Event>を利用します。汎用Anyからケースを取り出す処理はありません。学校コードなしでライブラリを別モジュールとしてビルドし、Counterだけのクライアントから呼ぶことも確認します。

### 2. Codableをライブラリ全体の制約にしなくてよかった

出典：[swift/dcb.swift](dcb.swift#L13-L14)

```swift
extension Recorded: Codable where E: Codable {}
public struct ReadResult<E:Sendable>{public let after:Int;public let events:[Recorded<E>];public init(after:Int,events:[Recorded<E>]){self.after=after;self.events=events}}
```

保存・コマンド処理に必要なのはSendableなEで、JSON化するRecordedだけをE: Codableの場合に適合させます。独立プローブのCounterはCodableにしていません。generic化で不要なシリアライズ制約をドメインへ押し込まない工夫です。

### 3. 値セマンティクスで投影の過去のスナップショットを保てる

出典：[swift/projection.swift](projection.swift#L4-L8)

```swift
struct Registers: Sendable {
    var students:[String:Student]=[:]
    var classes:[String:ClassState]=[:]
    var enrollments:[EnrollmentKey:Enrollment]=[:]
    func decisionModel()->DecisionModel {
```

Dictionaryとstruct／enumで状態を持ち、更新するローカル値を作ります。Goのsliceのような共有配列のコピーを同じ形で記述せずに済みました。ただし実際のコピー量や速度を今回測ったわけではありません。

## 実装で大変だったこと・残る制約

### 1. 公開モジュールの初期化子とSendableの契約を揃える必要があった

出典：[swift/dcb.swift](dcb.swift#L23-L26)

```swift
public final class EventStore<E: Sendable>: @unchecked Sendable, CommandStore {
    public typealias Payload=E
    private let gate = NSLock()
    private var log: [Recorded<E>] = []
```

publicなstructでも外部向けのmemberwise initializerが自動でpublicになるわけではないため、明示した初期化子が増えました。さらにEとclosureのSendable、ロック付き取得を揃え、unchecked Sendableをロックで守るという根拠を明確にしています。

### 2. 共通JSON形式へのCodable対応は引き続き長い

出典：[swift/wire.swift](wire.swift#L295-L302)

```swift
extension Operation: Codable {
    enum Keys: String, CodingKey {
        case tag = "$type"
        case command
        case commandId
        case key
    }
    init(from decoder: Decoder) throws {
```

学校のenumとOperationを共通$type形式へ対応させています。RustのSerdeやF#の標準DU対応に比べ、ライブラリ化しても境界のコードが自動的に消えません。typedなencodeとStringを読むdecodeでは、対応漏れの検出の仕方も異なります。

### 3. 同期ロック型の実装で、actorの非同期APIではない

出典：[swift/dcb.swift](dcb.swift#L21-L22)

```swift
// The lock covers check-and-append and synchronous notification. Listeners are
// non-reentrant consumers and return false to report a delivery failure.
```

このサンプルは他言語と同じ同期の読み取り・追記・購読契約をNSLockで実装します。購読は非throwingでfalseを返し、fatal errorを回復する仕組みではありません。actorや分散ストアへ移すなら、非同期I/Oと配信順の契約を別に設計する必要があります。

## コマンド・検証・履歴復元の更新

名目的なstructの検証付きinitと、保存済みの値を扱うfromEventを分けます。ValidatedCommandはprivate initを持ち、Deciderへの入口を制約します。Resultの網羅的matchより、ここではthrowsと型付きのRejectionへの変換が短く書けました。Codableの手書き部分は引き続き必要で、検証コードが短くてもJSONの総量は小さくありません。

[handlers.swift](handlers.swift)は入力検証とDecider接続、[valueObjects.swift](valueObjects.swift)は値の制約と検証済み入力、[domainTests.swift](domainTests.swift)はドメインテストです。Routingはタグ・読み取り条件・イベントのメタ情報を担当し、業務ハンドラーとは別分類です。

現在のポリシーで値を検証するcreate系factoryと、過去の事実を再検証しないfromEvent／restore系factoryを区別します。イベント・JSONの保存形と一覧DTOはプリミティブを保ち、投影で復元した値を取り出して使います。全てのStateの全フィールドを名目的なVOへ置き換えた実装ではありません。ValidatedCommandも永続化せず、新しいコマンドを受ける境界だけに使います。

76ケースのGWTではExecutor経由の結果と正確なイベントpayload、拒否時の無追記、投影と再構築の一致を確認します。これとは別に、ストアを使わないDeciderの3例、VOのfactoryと履歴復元、最後の1席の競合再判断もテストします。言語固有の不正入力ケースも追加しています。テスト全体をコード量の別枠へ計上しています。

## 大きな一覧Projectionの更新

学校全体のライブ一覧は[schoolProjection.swift](schoolProjection.swift)へ分離しました。既存のProjectionにある不変MapやコピーするReducerは、タグで絞ったコマンド判断用モデルと参照再生に残しています。ライブ一覧の更新には使いません。

ライブ一覧は汎用IndexedProjectionのハッシュ索引を使い、バッチ中に触った行だけを差分へ積み、全イベントを処理できてから反映します。クラスの名簿はイベント位置を内部ノードIDにした連結索引です。追加・取消で名簿全体をコピーせず、前後のノードだけを更新します。削除したノードは実際に索引から削除するため、同じ生徒の取消と再登録を繰り返してもノードを蓄積しません。

一覧要求時だけ名簿の配列／listと並び順を持つDTOを組み立てます。過去のDTOはその後のイベントで変わりません。全件Snapshotは出力サイズに比例する仕事があり、ロック中の読み取りもあるため、大規模な並行クエリまで一定時間という保証ではありません。頻繁な大量一覧の提供には、ページングや公開用Snapshotの別管理が必要です。

[projectionTests.swift](projectionTests.swift)では1千・1万・10万件を作り、更新と名簿拡大の時間を全件Snapshotと分けて測ります。途中失敗・位置の欠落で部分更新しないこと、再配送、過去Snapshotの不変性、取消と再登録後の順序と行数も検証します。[ローカルの測定値](../observed/projection-scaling.md)は言語の総合性能順位ではなく、この更新経路のサイズ依存を見るためのものです。
