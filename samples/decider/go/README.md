# Go：interface＋struct＋明示的なJSON境界

外部JSONライブラリなしで、判断・更新・復元の処理を追えます。一方、この題材ではケースの網羅性とJSONタグの対応を手動で保守する部分が多くなっています。

対象はこのClassroom Deciderの実装です。言語一般の順位ではありません。[共通仕様・検証](../README.md)／[コード量の集計](../README.md#コード量の比較)も併せて参照してください。以下の引用は現行ソースの抜粋で、出典リンクから該当行へ移動できます。

## Pros

### 1. 外部JSONライブラリなしで、成功／失敗を通常のerrorとして扱える

出典：[go/domain/decider.go](domain/decider.go#L8-L9)

```go
func Decide(s State, c Command) ([]Event, error) {
	switch v := c.(type) {
```
戻り値は[]Eventとerrorで、呼び出し側は通常のGoの形で扱います。[csharp/Models.cs](../csharp/Models.cs#L26)や[java/Models.java](../java/Models.java#L42)のような専用の成功／失敗クラスは不要です。JSONは[go/wire.go](wire.go#L4)を使い、Java版のJackson、Rust版のSerdeに相当する外部依存を追加していません。SwiftとC#も今回のJSONは標準機能です。

### 2. sliceの共有をどこで切るかが明示されている

出典：[go/domain/models.go](domain/models.go#L30-L33)

```go
func Snapshot(s State) (string, int, []string) {
	r, _ := data(s)
	return r.name, r.capacity, append([]string{}, r.students...)
}
```
出典：[go/domain/decider.go](domain/decider.go#L76-L76)

```go
		r.students = append(append([]string{}, r.students...), v.StudentID)
```
外へ返す時と更新時の両方で、sliceの背後の配列を共有したまま書き換えないようコピーしています。[typescript/models.ts](../typescript/models.ts#L3)のZod解析時の凍結とは違い、更新時にも実際の配列を分けています。反面、このコードを外すと過去のStateへ影響し得るので、自動的な不変コレクションではありません。

### 3. Snapshot復元時にフェーズの整合性まで確認している

出典：[go/wire.go](wire.go#L96-L99)

```go
	if d.Phase(state) != v.Type {
		return nil, fmt.Errorf("invalid_state")
	}
	return state, nil
```
このdecodeStateは作成／登録イベントを使って再構成し、タグと人数が食い違えば拒否します。[swift/wire.swift](../swift/wire.swift#L51)の直接構築などより、この部分で多くの仕事をしています。Goの「その他」が長い理由の一部は、この実装選択にもあります。

## Cons

### 1. interfaceを定義しても、全ケース処理を検査してもらえない

出典：[go/domain/decider.go](domain/decider.go#L21-L27)

```go
	case Enroll:
		return change(s, v.StudentID, true)
	case Drop:
		return change(s, v.StudentID, false)
	default:
		return reject("unknown_command") // Go has no exhaustive type switch.
	}
```
caseを追加し忘れても、このdefaultが実行時に拒否する形です。[rust/src/decider.rs](../rust/src/decider.rs#L3)のmatch、[java/Decider.java](../java/Decider.java#L6)のsealed switch、[typescript/decider.ts](../typescript/decider.ts#L32)のnever検査とは違います。Stateの非公開phaseメソッドも、コンパイラーにケース一覧を宣言する仕組みではありません。EventのTagメソッドは公開なので、別パッケージからの新しい実装も可能です。

### 2. JSONのタグと各structを手動で対応させる

出典：[go/wire.go](wire.go#L26-L35)

```go
	switch v.Type {
	case "Create":
		return d.Create{Name: v.Name, Capacity: v.Capacity}, nil
	case "Enroll":
		return d.Enroll{StudentID: v.StudentID}, nil
	case "Drop":
		return d.Drop{StudentID: v.StudentID}, nil
	default:
		return nil, fmt.Errorf("unknown_command")
	}
```
モデル、Tagメソッド、wireのdecode／encodeを保守します。[rust/src/models.rs](../rust/src/models.rs#L11)や[csharp/Models.cs](../csharp/Models.cs#L8)に比べ、今回のサンプルでは手書きが多くなっています。逆に、そのまま標準JSONへinterfaceを渡せば自動判別できる、という前提を隠していません。

### 3. struct値のコピーだけでは、安全性や型の閉じ方は揃わない

出典：[go/domain/models.go](domain/models.go#L11-L15)

```go
type Available struct{ data roster }
type Full struct{ data roster }

func (NotCreated) phase() string { return "NotCreated" }
func (Available) phase() string  { return "Available" }
```
この例はstruct値をtype switchに渡します。値レシーバーのメソッドはポインター側のメソッド集合にも入りますが、case Fullと*Fullは別の動的型です。ポインターへAPIを変えるならswitchも揃える必要があります（[Go仕様](https://go.dev/ref/spec#Method_sets)）。

また、Full{}というゼロ値は作れるので「Full型だから満員」とは保証しません。[]Eventとerrorの2戻り値も、[rust/src/decider.rs](../rust/src/decider.rs#L17)のResultのように成功／失敗を排他的な型へまとめたものではありません。サンプルの関数がその規約を守っています。
