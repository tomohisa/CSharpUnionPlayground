# Java：sealed interface＋record＋Jackson

クラス／recordの形を保ち、switch式でケースを検査できます。今回はJSONタグを明示的に固定する代わりに、型とJSONの登録リストを両方持っています。

対象はこのClassroom Deciderの実装です。言語一般の順位ではありません。[共通仕様・検証](../README.md)／[コード量の集計](../README.md#コード量の比較)も併せて参照してください。以下の引用は現行ソースの抜粋で、出典リンクから該当行へ移動できます。

## Pros

### 1. ケース別recordと、defaultのないswitch式を使える

出典：[java/Models.java](Models.java#L15-L18)

```java
sealed interface State permits NotCreated, Available, Full {}
record NotCreated() implements State {}
record Available(Roster roster) implements State {}
record Full(Roster roster) implements State {}
```
出典：[java/Decider.java](Decider.java#L6-L10)

```java
        return switch (state) {
            case NotCreated n -> "NotCreated";
            case Available a -> "Available";
            case Full f -> "Full";
        };
```
C#のclosed階層に近く、各ケースに固有の型があります。[go/domain/models.go](../go/domain/models.go#L9)のメソッドだけで表す集合と異なり、permitsで直接の実装型を宣言します。今回はJava 21を対象にしており、C#版の.NET 11／C# 15とは環境条件も異なります。

### 2. 防御的コピーをコンストラクターで揃えられる

出典：[java/Models.java](Models.java#L5-L7)

```java
record Roster(String name, int capacity, List<String> students) {
    Roster { students = List.copyOf(students); }
}
```
recordのフィールドがfinalでも、List自身を変更不可にする処理は別です。この例は[List.copyOf](https://docs.oracle.com/en/java/javase/21/docs/api/java.base/java/util/List.html#copyOf(java.util.Collection))を通しています。呼び出し元の可変ArrayListを保持し続けず、[typescript/models.ts](../typescript/models.ts#L1)のreadonly型注釈だけの場合より実行時の保護があります。要素はこの例では不変なStringです。

### 3. JSONのタグをJavaの型名から分離できる

出典：[java/Models.java](Models.java#L12-L12)

```java
    @JsonSubTypes.Type(value = Available.class, name = "Available"),
```
valueのクラス名とnameの文字列を別に管理できます。[csharp/Models.cs](../csharp/Models.cs#L8)や[rust/src/models.rs](../rust/src/models.rs#L12)の今回の自動命名より、クラス改名時にタグを維持する場所が明示的です。

## Cons

### 1. 型とJSONのリストが重複する

出典：[java/Models.java](Models.java#L9-L15)

```java
@JsonTypeInfo(use = JsonTypeInfo.Id.NAME, property = "$type")
@JsonSubTypes({
    @JsonSubTypes.Type(value = NotCreated.class, name = "NotCreated"),
    @JsonSubTypes.Type(value = Available.class, name = "Available"),
    @JsonSubTypes.Type(value = Full.class, name = "Full")
})
sealed interface State permits NotCreated, Available, Full {}
```
State、Command、Eventそれぞれに同様のリストがあります。新ケース追加時はpermitsとJsonSubTypesを更新します。sealedのswitchを直しても、Jacksonの登録漏れまでコンパイラーが検出する構成ではありません。[csharp/Models.cs](../csharp/Models.cs#L8)のclosed推論や[rust/src/models.rs](../rust/src/models.rs#L11)のderiveより、今回の方式の宣言は多くなっています。これはJavaで取り得る全方式の比較ではありません。

### 2. 不変更新で同じフィールドの再指定が増える

出典：[java/Decider.java](Decider.java#L51-L54)

```java
                var r = a.roster();
                var students = new ArrayList<>(r.students());
                students.add(e.studentId());
                yield classify(new Roster(r.name(), r.capacity(), students));
```
コピー、追加、新Roster構築が別の文です。[csharp/Decider.cs](../csharp/Decider.cs#L60)のwithや[typescript/decider.ts](../typescript/decider.ts#L61)のspreadより、変更しないnameとcapacityも書きます。過去のStateを変えないための処理であり、単にrecordを使えば消えるものではありません。

### 3. 外部JSONライブラリとnull・エラーコードの扱いが残る

[java/Main.java](Main.java#L12)はJacksonを使います。[swift/wire.swift](../swift/wire.swift#L1)や[go/wire.go](../go/wire.go#L4)の標準機能だけの構成と違い、依存を実行器が用意します。C# Native AOTと同等のデプロイ形態をこのJava版で検証したわけでもありません。

出典：[java/Models.java](Models.java#L46-L46)

```java
record Rejected(String code) implements Decision {}
```
CodeはStringで、[swift/models.swift](../swift/models.swift#L43)のような限定されたenumではありません。sealed／recordでもnull参照や不正なFullの構築は別問題です。今回の共通テストは任意の不正JSONすべてについて同じ拒否形式を保証していません。
