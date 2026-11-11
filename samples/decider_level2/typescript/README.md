# TypeScript＋Zod：履修スキーマから型とJSON検査を揃える

[共通仕様](../README.md)／[コード量](../README.md#コード量の比較)。対象はこの履修ドメインの実装です。言語一般の順位や、測っていない速度の比較ではありません。引用は現在のソースとCIで照合します。

外部JSONを読むこのサンプルではZodを標準にします。スキーマと型を別々に保守しないことが採用理由です。手書き検証でも実現できますが、今回はネストした状態が増えるため共有宣言の利点が大きくなります。

## この複雑さでよかったこと

### 1. 履修状態の追加が、型と実行時検証の両方に反映される

出典：[typescript/models.ts](models.ts#L26-L33)

```typescript
export const ProgressSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Reserved") }),
  z.object({ $type: z.literal("Studying"), attendance: z.int() }),
  z.object({ $type: z.literal("Passed"), score: z.int(), credits: z.int() }),
  z.object({ $type: z.literal("Failed"), score: z.int(), reason: FailureReasonSchema }),
  z.object({ $type: z.literal("Withdrawn"), reason: WithdrawalReasonSchema }),
]).readonly();
export type Progress = z.infer<typeof ProgressSchema>;
```

z.inferでProgress型を得るので、JSON復元用のタグswitchはありません。[java/Models.java](../java/Models.java#L70)のpermitsとJsonSubTypesの二重管理を避けています。z.intは構造上の整数検査に使い、成績などの業務判断はDeciderに残します。

### 2. ネストした前提科目も、分岐後の必要フィールドだけ読める

出典：[typescript/decider.ts](decider.ts#L57-L60)

```typescript
          const prerequisite = plan.prerequisite;
          const eligible = prerequisite.$type === "NoPrerequisite" || state.enrollments.some(e =>
            e.studentId === command.studentId && e.course === prerequisite.course && e.progress.$type === "Passed");
          if (!eligible) return reject("PrerequisiteMissing");
```

関連データがあるPassedCourseだけでcourseを使います。コールバック内でも絞り込みを保てるよう、prerequisiteをローカル変数へ取り出しました。型アサーションで押し切らずに書けます。describeはneverを使い、ケース追加時の未対応をtscで確認します。

### 3. 複数の状態配列でも差分更新を短く表せる

出典：[typescript/decider.ts](decider.ts#L107-L107)

```typescript
    case "StudentPromoted": return { ...state, students: state.students.map(s => s.studentId === event.studentId ? { ...s, grade: event.grade } : s) };
```

未変更のClasses／Enrollmentsはそのまま残します。[swift/decider.swift](../swift/decider.swift#L100)のようにStateの全フィールドを再指定する必要はありません。ただし配列をたどる処理とコピーは残ります。

## 実装で増えた負担・残る制約

### 1. スキーマで業務上の拒否まで一括処理しない調整が必要だった

出典：[typescript/decider.ts](decider.ts#L43-L45)

```typescript
      if (command.capacity < 1 || command.capacity > 10) return reject("InvalidCapacity");
      return state.classes.some(c => planOf(c).classId === command.classId) ? reject("DuplicateClass")
        : accept({ ...command, $type: "ClassOpened" });
```

capacity: 0は構造上は整数なので読めても、InvalidCapacityとして拒否します。Zodのmin/maxへ全部寄せると、他言語と揃えた業務判断の結果ではなくパースエラーになるためです。前提科目合格や定員との関係もStateを使うDeciderの責務です。

### 2. readonlyは解析した値と新しく作った値で効き方が違う

出典：[typescript/models.ts](models.ts#L49-L50)

```typescript
export const StateSchema = z.object({ students: z.array(StudentSchema).readonly(), classes: z.array(ClassStateSchema).readonly(), enrollments: z.array(EnrollmentSchema).readonly() }).readonly();
export type State = z.infer<typeof StateSchema>;
```

Zodの.readonly()は指定した対象を解析時に凍結しますが、任意のネストを自動で深く凍結しません。配列にも個別に指定しています。Deciderのspreadで作った新しい値を再parseしていないため、それらは型上readonlyでも実行時に凍結されません。[csharp/Models.cs](../csharp/Models.cs#L25)のImmutableArrayと同じ保護ではありません。[Zod仕様](https://zod.dev/api#readonly)。

### 3. スキーマがあっても全コレクションの走査・再構築は必要

出典：[typescript/decider.ts](decider.ts#L121-L122)

```typescript
  const change = (studentId: string, progress: (e: Enrollment) => Progress) =>
    records.map(e => e.classId === event.classId && e.studentId === studentId ? { ...e, progress: progress(e) } : e);
```

席と履修の整合性を保つため、classesの検索、enrollmentsのfilter／mapを組み合わせています。Zodはこの更新処理を作ってくれません。parseの検査・値の複製という実行コストと外部依存も増えます。この機能比較から速度の優劣は結論にしません。
