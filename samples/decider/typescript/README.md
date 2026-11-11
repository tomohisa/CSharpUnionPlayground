# TypeScript：Zodスキーマから導出する判別共用体

このDeciderのTypeScript版はZodを標準にします。外部JSONを読むため、型定義と実行時検証を同じスキーマで保守する利点を優先しました。以前の手書き検証もタグ・フィールド・安全な整数を確認していました。「Zodなしでは検証できない」という理由ではありません。

[Zodの型導出とparse](https://zod.dev/basics)を使い、スキーマからz.inferで型を得ます。[discriminatedUnion](https://zod.dev/api#discriminated-unions)で$typeのケースを宣言し、未知のタグや欠落・型違いをJSON境界で拒否します。型だけを付けたJSON.parseの結果を信用しません。

対象はこのClassroom Deciderの実装です。言語一般の順位ではありません。[共通仕様・検証](../README.md)／[コード量の集計](../README.md#コード量の比較)も参照してください。

## Pros

### 1. ケース宣言と実行時検証を一か所に集められる

出典：[typescript/models.ts](models.ts#L6-L10)

```typescript
export const StateSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("NotCreated") }),
  z.object({ $type: z.literal("Available"), roster: RosterSchema }),
  z.object({ $type: z.literal("Full"), roster: RosterSchema })
]).readonly();
```
出典：[typescript/models.ts](models.ts#L22-L24)

```typescript
export type State = z.infer<typeof StateSchema>;
export type Command = z.infer<typeof CommandSchema>;
export type Event = z.infer<typeof EventSchema>;
```

Stateのケース追加はスキーマを変更すれば型にも反映されます。以前のwire.tsのタグ別switchは不要です。[JavaのpermitsとJsonSubTypes](../java/Models.java#L11)のような別リストの同期がありません。[RustのSerde](../rust/src/models.rs#L11)と同様に宣言をまとめられますが、Zodは実行時ライブラリです。

### 2. 型付きswitchの網羅性を保てる

出典：[typescript/decider.ts](decider.ts#L32-L32)

```typescript
    default: return unreachable(command);
```
出典：[typescript/models.ts](models.ts#L28-L30)

```typescript
export function unreachable(value: never): never {
  throw new Error("Unknown case: " + JSON.stringify(value));
}
```

z.inferで導出したCommandも判別共用体なので、未処理ケースをneverへ渡すと型エラーになります。[Goの実行時default](../go/domain/decider.go#L26)とは違います。Nodeでの実行だけでは型検査されないため、CIではtscのstrict検査も実行します。

### 3. 更新は通常のオブジェクトで書ける

出典：[typescript/decider.ts](decider.ts#L61-L61)

```typescript
      return classify({ ...state.roster, students: [...state.roster.students, event.studentId] });
```

Zod専用のクラスを継承せずにspreadで更新できます。[JavaのRoster再構築](../java/Decider.java#L52)より未変更フィールドの繰り返しが少なく、JSON出力もJSON.stringifyです。コピーや割当がなくなるわけではありません。

## Cons

### 1. 依存と実行時の検証コストが増える

出典：[typescript/wire.ts](wire.ts#L4-L11)

```typescript
export const InputSchema = z.object({
  commands: z.array(CommandSchema),
  history: z.array(EventSchema).nullish().transform(value => value ?? [])
});
export const EventsSchema = z.array(EventSchema);
export const input = (value: unknown) => InputSchema.parse(value);
export const events = (value: unknown) => EventsSchema.parse(value);
export const state = (value: unknown) => StateSchema.parse(value);
```

[package.json](package.json)とロックファイルにZodを固定します。[C#の標準STJ](../csharp/Program.cs#L62)やSwiftのFoundationと違い、外部依存です。parseは入力を検査して別の値を作るので、型アサーションだけのJSON.parseと同じ実行コストではありません。このDeciderでは速度を測っていません。既存のunion100ベンチマークの結果を、この処理の速度として引用しません。

通常のz.objectは余分なキーを除去します。以前の手書き復元と同じ方針です。余分なキーも拒否する契約にしたいならz.strictObjectへ変更する必要があります。ZodErrorの詳細は得られますが、他言語とエラーメッセージの形式を揃える実装ではありません。

### 2. スキーマ検査と業務上の整合性は別

出典：[typescript/models.ts](models.ts#L11-L15)

```typescript
export const CommandSchema = z.discriminatedUnion("$type", [
  z.object({ $type: z.literal("Create"), name: z.string(), capacity: z.int() }),
  z.object({ $type: z.literal("Enroll"), studentId: z.string() }),
  z.object({ $type: z.literal("Drop"), studentId: z.string() })
]).readonly();
```

z.intで安全な整数まで検査しますが、定員1〜10や空の名前の拒否はdecide、イベント履歴の順序・重複はevolveが担当します。たとえばcapacity: 0のCreateはJSONとして読めてもinvalid_capacityになります。Fullのスキーマに合うだけで「登録者数＝定員」まで保証する設計ではありません。直接作ったオブジェクトはparseを通らず、TypeScriptのnumber型も整数の範囲を表しません。[C#のclosed record](../csharp/Models.cs#L9)でも型の閉じ方と業務条件は別です。

### 3. readonlyの範囲を理解する必要がある

出典：[typescript/models.ts](models.ts#L3-L5)

```typescript
export const RosterSchema = z.object({
  name: z.string(), capacity: z.int(), students: z.array(z.string()).readonly()
}).readonly();
```

この版では配列・Roster・各unionに.readonly()を指定しています。[Zodのreadonly](https://zod.dev/api#readonly)はparseした対象をObject.freezeし、readonly型も導出します。State→Roster→studentsは各段で指定しているため解析後はそれぞれ凍結されます。ただし任意のネストを自動で深く凍結する機能ではありません。

Deciderがspreadで新しく作るState／配列にはparseを再実行していないため、実行時には凍結されません。更新しない規約と型チェックは引き続き必要です。[C#のImmutableArray](../csharp/Models.cs#L6)のように、生成経路によらず不変なコレクションになるわけではありません。

## 検証と実行

```bash
npm ci --prefix samples/file-based
npm ci --prefix samples/decider/typescript
samples/file-based/node_modules/.bin/tsc -p samples/decider/typescript/tsconfig.json
node --test samples/decider/tests/typescript-boundary.mts
python3 samples/decider/run.py --language typescript --verify
```

[境界テスト](../tests/typescript-boundary.mts)で欠落・型違い・小数・安全な整数範囲外・未知タグ・ネストした不正値、履歴null、余分なキー、業務拒否との分担、解析後の凍結を確認します。共通実行器でも全言語のJSON契約と保存済みイベントの再生を検証します。
