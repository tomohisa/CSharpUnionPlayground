# Go：型パラメーターと小さなストアinterfaceで分ける

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。このページでは記述を比較し、速度・メモリは[ベンチマーク](../benchmark/README.md)で比較します。

## この設計でよかったこと

### 1. 共通ストアのpayloadはanyの値ではなくE

出典：[dcb/store.go:73–76](dcb/store.go#L73-L76)

```go
type CommandStore[E any] interface {
	Read(Query) ReadResult[E]
	Append(string, Query, int, []E) AppendResult[E]
}
```

出典：[domain/handlers.go:1–25](domain/handlers.go#L1-L25)

```go
package domain

import "example.com/dcb-school/dcb"

type SchoolHandler struct{}

func (SchoolHandler) Validate(command Command) []Rejection {
	_, errors := NewValidatedCommand(command)
	return errors
}
func (SchoolHandler) QueryFor(command Command) dcb.Query { return QueryFor(command) }
func (SchoolHandler) Handle(events []dcb.Recorded[Event], command Command) dcb.Preparation[Event, Rejection] {
	validated, errors := NewValidatedCommand(command)
	if len(errors) > 0 {
		return dcb.Preparation[Event, Rejection]{Events: []Event{}, Errors: errors}
	}
	switch decision := Decide(Fold(events).DecisionModel(), validated).(type) {
	case Accepted:
		return dcb.Preparation[Event, Rejection]{Events: decision.Events, Errors: []Rejection{}}
	case Rejected:
		return dcb.Preparation[Event, Rejection]{Events: []Event{}, Errors: []Rejection{decision.Code}}
	default:
		panic("unknown decision")
	}
}
```

anyは型パラメーターの制約で、保存するフィールド自体はEです。学校のEvent interfaceをライブラリへ組み込む必要はなく、Counter structだけでdcbパッケージをテストできます。

### 2. 履修のキーとコピーの責任を明示できる

出典：[domain/projection.go:8–10](domain/projection.go#L8-L10)

```go
type EnrollmentKey struct{ Room, Student string }
type Registers struct {
	Students    map[string]Student
```

出典：[domain/projection.go:18–22](domain/projection.go#L18-L22)

```go
func CopyRegisters(s Registers) Registers {
	n := EmptyRegisters()
	for k, v := range s.Students {
		n.Students[k] = v
	}
```

複合キーをstructにし、取得済み一覧と更新中状態がsliceを共有して変わらないようコピーを揃えました。値型のstructだけでは内部配列の独立性まで保証しない点に注意が要りました。

### 3. race detectorを使ってストアのロックを検証できる

出典：[dcb/store.go:111–115](dcb/store.go#L111-L115)

```go
func (s *EventStore[E]) Append(commandId string, q Query, after int, batch []E) AppendResult[E] {
	s.gate.Lock()
	defer s.gate.Unlock()
	if len(batch) == 0 || commandId == "" || after < 0 || after > len(s.log) {
		panic("invalid append")
```

読み取りと条件付き追記をMutexで守り、別goroutineの2つの書き手を同じ読み取り位置から走らせます。学校とは独立したdcbパッケージだけを-race付きで検証しています。

## 実装で大変だったこと・残る制約

### 1. generic化しても、学校のtype switchの網羅性は得られない

出典：[domain/routing.go:39–44](domain/routing.go#L39-L44)

```go
func DescribeEvent(event Event) dcb.EventDescriptor {
	switch e := event.(type) {
	case StudentRegistered:
		return dcb.EventDescriptor{EventType: "StudentRegistered", Tags: []dcb.Tag{StudentTag(e.StudentId)}}
	case StudentPromoted:
		return dcb.EventDescriptor{EventType: "StudentPromoted", Tags: []dcb.Tag{StudentTag(e.StudentId)}}
```

新しいイベントのタグ規則やProgressの分岐を、標準コンパイラーは全ケース処理として検査しません。ドメインのinterfaceを公開ライブラリのanyへ置き換えてはいませんが、Go自体のこの制約は残ります。

### 2. JSONのinterface復元は境界で書く必要がある

出典：[wire.go:38–41](wire.go#L38-L41)

```go
		payload, err := d.DecodeEvent(e.Payload)
		if err != nil {
			return Input{}, err
		}
```

Recorded[Event]のpayloadをencoding/jsonだけで具体型へ戻せないため、RawMessageからドメインのdecoderへ渡します。型パラメーターで保てることと、実行時に具体型を選べることは別でした。domain/json.goのコード量もJSON・CLIへ含めています。

### 3. 購読panicを隔離し、失敗した投影を黙って使わない必要がある

出典：[dcb/store.go:151–158](dcb/store.go#L151-L158)

```go
func notify[E any](listener func([]Recorded[E]) bool, events []Recorded[E]) (ok bool) {
	defer func() {
		if recover() != nil {
			ok = false
		}
	}()
	return listener(events)
}
```

panicを追記失敗に見せないようrecoverで通知失敗へ変換します。公開sliceやinterfaceのnilを言語だけで禁止してはいないため、コピー・境界検証・非再入の購読契約はライブラリ利用側にも必要です。

## コマンド・検証・履歴復元の更新

公開factoryと非公開フィールド、errorによるスマートコンストラクターを使います。ただしGoではゼロ値を完全には禁止できません。ValidatedCommandの未初期化値はValueで拒否し、ID・定員のアクセサーにも未初期化ガードを置きます。0点は正しい値なのでAssessmentScoreのゼロ値は許容します。型switchの網羅性をコンパイラーは保証しないため、HandlerのdefaultはInvalidCommandとして拒否します。

[domain/handlers.go](domain/handlers.go)は入力検証とDecider接続、[domain/value_objects.go](domain/value_objects.go)は値の制約と検証済み入力、[domain_test.go](domain_test.go)はドメインテストです。Routingはタグ・読み取り条件・イベントのメタ情報を担当し、業務ハンドラーとは別分類です。

現在のポリシーで値を検証するcreate系factoryと、過去の事実を再検証しないfromEvent／restore系factoryを区別します。イベント・JSONの保存形と一覧DTOはプリミティブを保ち、投影で復元した値を取り出して使います。全てのStateの全フィールドを名目的なVOへ置き換えた実装ではありません。ValidatedCommandも永続化せず、新しいコマンドを受ける境界だけに使います。

76ケースのGWTではExecutor経由の結果と正確なイベントpayload、拒否時の無追記、投影と再構築の一致を確認します。これとは別に、ストアを使わないDeciderの3例、VOのfactoryと履歴復元、最後の1席の競合再判断もテストします。言語固有の不正入力ケースも追加しています。テスト全体をコード量の別枠へ計上しています。

## 大きな一覧Projectionの更新

学校全体のライブ一覧は[domain/school_projection.go](domain/school_projection.go)へ分離しました。既存のProjectionにある不変MapやコピーするReducerは、タグで絞ったコマンド判断用モデルと参照再生に残しています。ライブ一覧の更新には使いません。

ライブ一覧は汎用IndexedProjectionのハッシュ索引を使い、バッチ中に触った行だけを差分へ積み、全イベントを処理できてから反映します。クラスの名簿はイベント位置を内部ノードIDにした連結索引です。追加・取消で名簿全体をコピーせず、前後のノードだけを更新します。削除したノードは実際に索引から削除するため、同じ生徒の取消と再登録を繰り返してもノードを蓄積しません。

一覧要求時だけ名簿の配列／listと並び順を持つDTOを組み立てます。過去のDTOはその後のイベントで変わりません。全件Snapshotは出力サイズに比例する仕事があり、ロック中の読み取りもあるため、大規模な並行クエリまで一定時間という保証ではありません。頻繁な大量一覧の提供には、ページングや公開用Snapshotの別管理が必要です。

[projection_tests.go](projection_tests.go)では1千・1万・10万件を作り、更新と名簿拡大の時間を全件Snapshotと分けて測ります。途中失敗・位置の欠落で部分更新しないこと、再配送、過去Snapshotの不変性、取消と再登録後の順序と行数も検証します。[ローカルの測定値](../observed/projection-scaling.md)は言語の総合性能順位ではなく、この更新経路のサイズ依存を見るためのものです。

## JSON／SQLiteと計測

mattn/go-sqlite3とdatabase/sql。CGOを使うためCコンパイラーが必要です。学校のJSON復元処理はdomain側に残し、dcbパッケージはcodec関数だけを受け取ります。

[共通ベンチマーク](../benchmark/README.md)では、履修コマンド・拒否・競合後の再判断、索引Projectionのクエリ、JSON応答、再起動を同じ件数で実行します。SQLite版とメモリJSON版の両方でシリアライズ・デシリアライズを通します。インフラ障害は業務のRejectedと混ぜず、プロセスを失敗させます。外部接続のイベントは明示的なRefreshまたは次の自分のcommitで取り込みます。
