# Java：sealed recordで履修を分け、不変コレクションで保持する

[共通仕様](../README.md)／[コード量](../README.md#コード量の比較)。対象はこの履修ドメインの実装です。言語一般の順位や、測っていない速度の比較ではありません。引用は現在のソースとCIで照合します。

## この複雑さでよかったこと

### 1. 各状態のデータをdefaultなしのswitchで読める

出典：[java/Decider.java](Decider.java#L12-L18)

```java
    static String describe(Progress progress) {
        return switch (progress) {
            case Reserved p -> "reserved"; case Studying p -> "studying:" + p.attendance();
            case Passed p -> "passed:" + p.score() + ":" + p.credits();
            case Failed p -> "failed:" + p.score() + ":" + p.reason(); case Withdrawn p -> "withdrawn:" + p.reason();
        };
    }
```

Reservedに点数を置かず、Passed／Failedだけからscoreを読みます。PendingReview追加時に未対応switchがコンパイルエラーになることを実験します。[csharp/Decider.cs](../csharp/Decider.cs#L16)と似た記述で、ネストした業務状態にも使えました。

### 2. コレクションの防御的コピーをモデル側に集められる

出典：[java/Models.java](Models.java#L38-L44)

```java
record State(List<Student> students, List<ClassState> classes, List<Enrollment> enrollments) {
    public State {
        students = List.copyOf(students);
        classes = List.copyOf(classes);
        enrollments = List.copyOf(enrollments);
    }
}
```

List.copyOfで呼び出し元の可変リストをそのまま保持しません。Enrollment／Planもrecordなので、今回の参照先は更新せず置き換えます。[typescript/models.ts](../typescript/models.ts#L49)のreadonly型だけの値に比べ、生成したリストにも実行時の保護があります。

### 3. 拒否理由と前提科目を、nullなしの別ケースにできる

出典：[java/Decider.java](Decider.java#L56-L60)

```java
                        boolean eligible = switch (plan.prerequisite()) {
                            case NoPrerequisite p -> true;
                            case PassedCourse p -> state.enrollments().stream().anyMatch(e -> e.studentId().equals(c.studentId()) && e.course() == p.course() && e.progress() instanceof Passed);
                        };
                        if (!eligible) return reject(Rejection.PrerequisiteMissing);
```

前提科目の有無をnull判定で表さず、NoPrerequisiteとPassedCourseに分けました。RejectedはRejection enumなので、文字列の綴りを呼び出し側ごとに決める必要もありません。

## 実装で増えた負担・残る制約

### 1. 階層が増えるほどJSON属性とケース一覧も増えた

出典：[java/Models.java](Models.java#L70-L72)

```java
sealed interface Progress permits Reserved, Studying, Passed, Failed, Withdrawn {}
record Reserved() implements Progress {
    public Reserved {
```

Prerequisite／Progress／ClassState／Command／Event／DecisionそれぞれにJsonSubTypesとpermitsを対応させています。[rust/src/models.rs](../rust/src/models.rs#L68)やZodの一つのスキーマに比べ、ケース追加の保守箇所が多くなりました。null拒否のObjects.requireNonNullもモデル側で明示しています。

### 2. 一部分の更新でも親レコードを再構築する

出典：[java/Decider.java](Decider.java#L110-L110)

```java
            case StudentPromoted x -> { return new State(s.students().stream().map(v -> v.studentId().equals(x.studentId()) ? new Student(v.studentId(), x.grade()) : v).toList(), s.classes(), s.enrollments()); }
```

クラス、履修、生徒の配列を持つようになり、変更しないフィールドの再指定が増えました。[fsharp/Decider.fs](../fsharp/Decider.fs#L133)やC#のwithと比べた今回の負担です。ローカルArrayListは使いますが、最後はコピーした新しいStateを返します。

### 3. Jacksonの境界設定と、モデルの業務条件は分かれる

出典：[java/Main.java](Main.java#L14-L15)

```java
        var mapper = new ObjectMapper().enable(DeserializationFeature.FAIL_ON_NULL_FOR_PRIMITIVES, DeserializationFeature.FAIL_ON_MISSING_CREATOR_PROPERTIES);
        var input = mapper.readValue(System.in, Input.class);
```

欠落やprimitiveのnullを拒否する設定はMain、参照のnull拒否はモデル、学年・点数・履歴の条件はDeciderに分かれています。sealedにするだけで不整合なFullを構築できなくなるわけではありません。今回はJackson依存のJVM版で、JavaのNative AOTを検証したものではありません。
