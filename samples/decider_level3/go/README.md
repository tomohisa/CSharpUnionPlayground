# Go：型パラメーターと小さなストアinterfaceで分ける

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

## この設計でよかったこと

### 1. 共通ストアのpayloadはanyの値ではなくE

出典：[go/dcb/store.go](dcb/store.go#L72-L75)

```go
type CommandStore[E any] interface {
	Read(Query) ReadResult[E]
	Append(string, Query, int, []E) AppendResult[E]
}
```

出典：[go/domain/routing.go](domain/routing.go#L65-L73)

```go
func PrepareDecision(events []dcb.Recorded[Event], command Command) dcb.Preparation[Event, Rejection] {
	switch decision := Decide(Fold(events).DecisionModel(), command).(type) {
	case Accepted:
		return dcb.Preparation[Event, Rejection]{Events: decision.Events, Errors: []Rejection{}}
	case Rejected:
		return dcb.Preparation[Event, Rejection]{Events: []Event{}, Errors: []Rejection{decision.Code}}
	default:
		panic("unknown decision")
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

出典：[go/dcb/store.go](dcb/store.go#L110-L114)

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

出典：[go/dcb/store.go](dcb/store.go#L150-L157)

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
