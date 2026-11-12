# Go: Level4 HTTP API

[共通仕様・実行・コード量](../README.md)／[OpenAPI](../openapi.json)／[再利用するLevel3](../../decider_level3/go)。学校ドメイン・VO・DeciderはLevel3から参照し、コピーしていません。

## 今回良かったところ

net/httpとLevel3のHandler・JSONストアを接続する構成は単純です。学生クエリはFindStudentのfoundを検査して404へ変換し、投影全体を走査しません。SQLiteのCGOドライバーも同じままで、永続化の整合性をHTTP用に実装し直す必要はありませんでした。

[go/main.go:139–142](main.go#L139-L142)

```go
	executor := dcb.DcbExecutor[d.Command, d.Event, d.Rejection]{Store: store}
	handler := d.SchoolHandler{}
	tickets := map[string]ticket{}
	var gate sync.Mutex
```

## 実装で増えた仕事

HTTPの追加より、interfaceで表したコマンド・イベント・フェーズのJSON実装を再利用する部分の量が大きくなります。type switchはenum/DUのswitchと違ってケース追加の網羅性を強制しません。DecodeCommandが作る型を限定し、VO・Handler検証、テストを併用しています。FindStudentのような値＋boolのAPIも、戻り値を無視すればゼロ値を使えてしまいます。

[../decider_level3/go/domain/school_projection.go:197–203](../../decider_level3/go/domain/school_projection.go#L197-L203)

```go
func (p *SchoolProjection) FindStudent(id string) (int, Student, bool) {
	var student Student
	var found bool
	position := p.index.Read(func(rows map[indexKey]indexRow) {
		if row, ok := rows[indexKey{Kind: "student", A: id}].(studentRow); ok {
			student = row.Value
			found = true
```

## 他の実装と比較すると

この小さなアダプターはHTTPエラーをrecoverでまとめています。ドメイン拒否はpanicにせず、ExecutorのHandlingResultから422へ変換します。net/httpが並行に処理しても、SQLiteの書き込みと投影更新のロックは共有されます。goroutineの数だけストアの書き込みが並列になる構成ではありません。

[共通HTTPテスト](../tests/http_tests.py)では入力拒否・競合再判断・投影反映・SQLite再起動と相互復元を検証します。[ローカルの測定結果](../benchmark/observed/README.md)は、このホスト・codec・ストアを含む一連の実装の比較です。
