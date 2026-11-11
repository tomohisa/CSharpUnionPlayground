# Go：明示的な型分岐とコピーで履修を組み立てる

[共通仕様](../README.md)／[コード量](../README.md#コード量の比較)。対象はこの履修ドメインの実装です。言語一般の順位や、測っていない速度の比較ではありません。引用は現在のソースとCIで照合します。

## この複雑さでよかったこと

### 1. 型ごとに必要なフィールドだけ持つstructを使える

出典：[go/domain/models.go](domain/models.go#L99-L111)

```go
type Studying struct {
	Attendance int `json:"attendance"`
}

func (Studying) isProgress() {}

type Passed struct {
	Score   int `json:"score"`
	Credits int `json:"credits"`
}

func (Passed) isProgress() {}

```

scoreのないStudying、creditsのあるPassed、reasonのあるFailedを分けています。ドメインのフィールドにポインターを置いて未確定値をnilにする構成は取りません。Grade／Course／理由も名前のある型にしています。ただしstringベースの型自体は任意の文字列を構築できます。

### 2. 更新前に所有するsliceを分ける場所が明確

出典：[go/domain/decider.go](domain/decider.go#L266-L267)

```go
	// Own every slice before updating: old snapshots must remain unchanged.
	s := State{append([]Student{}, state.Students...), append([]ClassState{}, state.Classes...), append([]Enrollment{}, state.Enrollments...)}
```

学生情報・クラス・履修の外側sliceをコピーし、名簿更新もmembersで新しいsliceを作ります。学校全体の深いcloneを行う[rust/src/decider.rs](../rust/src/decider.rs#L109)とは、どこを複製するかが異なります。共通テストで初期Stateが変わらないことを確認しています。

### 3. 型付きイベントの構造が同じならコマンドへ変換できる

出典：[go/domain/decider.go](domain/decider.go#L238-L244)

```go
func Cause(event Event) Command {
	switch e := event.(type) {
	case StudentRegistered:
		return RegisterStudent(e)
	case StudentPromoted:
		return PromoteStudent(e)
	case ClassOpened:
```

対応するstructを変換して履歴検査に使えます。タグ文字列をドメインデータとして保持する必要がなく、JSONの$typeは境界で付けます。外部JSONライブラリも使いません。

## 実装で増えた負担・残る制約

### 1. ケース追加の未対応がコンパイルでは見つからない

出典：[go/domain/decider.go](domain/decider.go#L386-L400)

```go
func Describe(progress Progress) string {
	switch p := progress.(type) {
	case Reserved:
		return "reserved"
	case Studying:
		return fmt.Sprintf("studying:%d", p.Attendance)
	case Passed:
		return fmt.Sprintf("passed:%d:%d", p.Score, p.Credits)
	case Failed:
		return fmt.Sprintf("failed:%d:%s", p.Score, p.Reason)
	case Withdrawn:
		return fmt.Sprintf("withdrawn:%s", p.Reason)
	default:
		panic("unknown progress")
	}
```

privateなmarkerメソッドを持つinterfaceでも、Goのtype switchには網羅性検査がありません。PendingReviewを足した一時コピーはコンパイルでき、describeの実行時defaultへ到達することをテストしています。[fsharp/Decider.fs](../fsharp/Decider.fs#L11)の警告をエラーにする方式との差です。

### 2. ネストしたinterfaceのJSON復元が長くなった

出典：[go/domain/json.go](domain/json.go#L119-L127)

```go
func DecodeProgress(data []byte) (Progress, error) {
	values, err := fields(data, "$type")
	if err != nil {
		return nil, err
	}
	var tag string
	if err := json.Unmarshal(values["$type"], &tag); err != nil {
		return nil, err
	}
```

標準JSONはinterfaceの具体型を選ばないため、タグ別decode、型ごとのMarshalJSON、内部にinterfaceを持つrecordのUnmarshalJSONを書いています。復元対象のケースが6階層に増え、その他のコード量が大きくなりました。これは共通JSON契約に合わせたこの実装の費用で、Goのドメイン処理だけの長さではありません。ジェネレーターや別ライブラリとの比較はしていません。

### 3. nilをフィールドに使わなくても、不正値の余地は残る

出典：[go/domain/models.go](domain/models.go#L93-L94)

```go
type Progress interface{ isProgress() }
type Reserved struct {
```

interface自体のnilや、値型とポインター型の動的型の違いは消えません。このAPIではstruct値を渡します。sliceも公開されているので、外部コードがStateの内部配列を直接変更することは防げません。Evolve内部のコピーと、あらゆる利用者に対する不変性保証は別です。
