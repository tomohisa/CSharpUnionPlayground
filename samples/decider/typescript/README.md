# TypeScript：判別共用体＋手書きJSON検査

$typeがそのまま型判別と保存形式に使えます。オブジェクトの更新は短い一方、型定義と実行時のJSON検査を別々に保守しています。

対象はこのClassroom Deciderの実装です。言語一般の順位ではありません。[共通仕様・検証](../README.md)／[コード量の集計](../README.md#コード量の比較)も併せて参照してください。以下の引用は現行ソースの抜粋で、出典リンクから該当行へ移動できます。

## Pros

### 1. 継承用の基底クラスなしでフェーズを表せる

出典：[typescript/models.ts](models.ts#L2-L5)

```typescript
export type State =
  | Readonly<{ $type: "NotCreated" }>
  | Readonly<{ $type: "Available"; roster: Roster }>
  | Readonly<{ $type: "Full"; roster: Roster }>;
```
Stateは普通のオブジェクトのunionです。[csharp/Models.cs](../csharp/Models.cs#L9)の派生recordや[java/Models.java](../java/Models.java#L15)のimplements／permitsは不要で、$typeも通常のフィールドです。JSON出力もJSON.stringifyを使い、別のエンコード用switchはありません。

### 2. 更新するフィールドを絞って書ける

出典：[typescript/decider.ts](decider.ts#L61-L61)

```typescript
      return classify({ ...state.roster, students: [...state.roster.students, event.studentId] });
```
rosterの未変更フィールドを繰り返しません。[swift/decider.swift](../swift/decider.swift#L35)のRoster再構築や[java/Decider.java](../java/Decider.java#L52)と対照的です。ただしspreadと配列生成にはコピーがあり、記述が短いことを実行コストの小ささとは扱いません。

### 3. neverで「残ったケース」を検査できる

出典：[typescript/decider.ts](decider.ts#L32-L32)

```typescript
    default: return unreachable(command);
```
出典：[typescript/models.ts](models.ts#L17-L19)

```typescript
export function unreachable(value: never): never {
  throw new Error("Unknown case: " + JSON.stringify(value));
}
```
commandのunionに未処理ケースが残れば、neverへ渡せなくなります。[go/domain/decider.go](../go/domain/decider.go#L26)の単なる実行時拒否とは違います。この検査はNodeで実行するだけでなく、tsconfigのstrict型チェックで行っています。

## Cons

### 1. JSONを読むだけでは型は復元・検証されない

出典：[typescript/wire.ts](wire.ts#L20-L28)

```typescript
export function command(value: unknown): Command {
  const v = object(value);
  switch (v.$type) {
    case "Create": return { $type: "Create", name: text(v.name), capacity: integer(v.capacity) };
    case "Enroll": return { $type: "Enroll", studentId: text(v.studentId) };
    case "Drop": return { $type: "Drop", studentId: text(v.studentId) };
    default: throw new Error("unknown_command");
  }
}
```
未知のJSONから各フィールドを検査してCommandを作り直します。[rust/src/models.rs](../rust/src/models.rs#L20)のSerde deriveや[csharp/Models.cs](../csharp/Models.cs#L15)の標準STJ推論より、手書きの境界コードが増えています。

### 2. モデルと検査のケース一覧を二重に保守する

Eventを増やすとmodels.ts、decider.ts、wire.tsの更新が必要です。Deciderのnever検査は有効ですが、上のwireの入力はunknownから作った辞書です。ドメイン側を更新しても、wireの新しいタグ対応漏れまで自動で網羅性エラーになる構成ではありません。Zod等から型を導出する別案はありますが、この版では使っていません。

### 3. readonlyは実行時の凍結ではない

出典：[typescript/models.ts](models.ts#L1-L1)

```typescript
export type Roster = Readonly<{ name: string; capacity: number; students: readonly string[] }>;
```
Rosterだけでなくstudentsにもreadonlyを指定していますが、Object.freezeは呼んでいません。[公式説明](https://www.typescriptlang.org/docs/handbook/2/objects.html#readonly-properties)の通り、readonlyは型チェック上の制約です。[csharp/Models.cs](../csharp/Models.cs#L6)や[java/Models.java](../java/Models.java#L6)のコレクションと同じ実行時の保証ではありません。anyや型検査を通らない呼び出し元まで保護しません。

さらに、capacityのnumberという型だけでは整数も定員1〜10も表せず、[typescript/wire.ts](wire.ts#L12)とdecideの検査に分けています。Fullタグがあるだけで満員という業務条件まで保証するわけではありません。
