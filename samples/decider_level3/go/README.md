# Go：型パラメーターと小さなストアinterfaceで分ける

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

## この設計でよかったこと

### 1. 共通ストアのpayloadはanyの値ではなくE

出典：[go/dcb/store.go](dcb/store.go#L73-L76)

```go
type CommandStore[E any] interface {
	Read(Query) ReadResult[E]
	Append(string, Query, int, []E) AppendResult[E]
}
```

出典：[domain/handlers.go](domain/handlers.go#L1-L25)

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

出典：[go/domain/projection.go](domain/projection.go#L8-L10)

```go
type EnrollmentKey struct{ Room, Student string }
type Registers struct {
	Students    map[string]Student
```

出典：[go/domain/projection.go](domain/projection.go#L18-L22)

```go
func CopyRegisters(s Registers) Registers {
	n := EmptyRegisters()
	for k, v := range s.Students {
		n.Students[k] = v
	}
```

複合キーをstructにし、取得済み一覧と更新中状態がsliceを共有して変わらないようコピーを揃えました。値型のstructだけでは内部配列の独立性まで保証しない点に注意が要りました。

### 3. race detectorを使ってストアのロックを検証できる

出典：[go/dcb/store.go](dcb/store.go#L111-L115)

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

出典：[go/domain/routing.go](domain/routing.go#L39-L44)

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

出典：[go/wire.go](wire.go#L38-L41)

```go
		payload, err := d.DecodeEvent(e.Payload)
		if err != nil {
			return Input{}, err
		}
```

Recorded[Event]のpayloadをencoding/jsonだけで具体型へ戻せないため、RawMessageからドメインのdecoderへ渡します。型パラメーターで保てることと、実行時に具体型を選べることは別でした。domain/json.goのコード量もJSON・CLIへ含めています。

### 3. 購読panicを隔離し、失敗した投影を黙って使わない必要がある

出典：[go/dcb/store.go](dcb/store.go#L151-L158)

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
