# TypeScript＋Zod：型付きイベントと構造的なportを使う

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。速度・メモリのベンチマークではありません。

## この設計でよかったこと

### 1. Zodのドメイン型を、Zodに依存しない共通部品へ渡せる

出典：[typescript/dcb.ts](dcb.ts#L12-L15)

```typescript
export interface CommandStore<E> {
  read(query:Query):ReadResult<E>;
  append(commandId:string,condition:Query,after:number,batch:readonly E[]):AppendResult<E>;
}
```

出典：[typescript/routing.ts](routing.ts#L39-L42)

```typescript
export function prepareDecision(events: readonly Recorded<Event>[], command: Command): Preparation<Event,Rejection> {
  const decision=decide(decisionModel(fold(events)),command);
  return decision.$type === "Accepted" ? {kind:"Emit",events:decision.events} : {kind:"Reject",error:decision.code};
}
```

ライブラリのdcb.tsにはZodのimportも学校の型もありません。学校側はスキーマから得たEvent／Command／Rejectionを渡すだけです。デシリアライズはwire.tsの境界に残します。

### 2. 生の閉じたイベントを返したまま、タグ登録を一か所へ集められる

出典：[typescript/routing.ts](routing.ts#L24-L28)

```typescript
export function describeEvent(event: Event): EventDescriptor {
  switch(event.$type) {
    case "StudentRegistered": return {eventType:"StudentRegistered",tags:[studentTag(event.studentId)]};
    case "StudentPromoted": return {eventType:"StudentPromoted",tags:[studentTag(event.studentId)]};
    case "ClassOpened": return {eventType:"ClassOpened",tags:[classTag(event.classId)]};
```

すべてのイベントを処理した後はneverで未対応を検出できます。型アサーションで汎用Eventから戻す方式にしません。Storedの位置・コマンドID・タグ検査はライブラリ側へ集めています。

### 3. 複合キーと投影コピーを標準の機能で表せる

出典：[typescript/projection.ts](projection.ts#L7-L8)

```typescript
export const copy = (s: Registers): Registers => structuredClone(s);
const key = (room:string,student:string) => JSON.stringify([room,student]);
```

Mapのコピーと更新を明示し、JSONのtuple表現で履修キーを区別します。取得済みの一覧が後で変わらないこと、構造化キーが衝突しないことを共通シナリオで確認します。

## 実装で大変だったこと・残る制約

### 1. 共通メタ情報の実行時スキーマとの接続は残る

出典：[typescript/wire.ts](wire.ts#L3-L4)

```typescript
export const InputSchema=z.object({history:z.array(z.object({position:z.int().positive(),commandId:z.string().min(1),eventType:z.string().min(1),
  tags:z.array(z.object({category:z.string(),keys:z.array(z.string())})),payload:EventSchema})),operations:z.array(OperationSchema)});
```

ドメインのケースはZodから導出できますが、ライブラリをZod非依存にした分、エンベロープのJSONスキーマを境界で記述しています。構造はtsc、タグとペイロードの意味の一致はStore.loadのdescribeで確認します。

### 2. ジェネリック関数の推論にも調整が必要だった

出典：[typescript/dcb.ts](dcb.ts#L24-L24)

```typescript
  constructor(describe:(event:E)=>EventDescriptor,copyPayload:(event:NoInfer<E>)=>NoInfer<E>){this.describe=describe;this.copyPayload=copyPayload;}
```

structuredCloneのようなジェネリック関数からEがunknownへ引っ張られないよう、コピー引数にNoInferを使いました。ドメイン型の情報を落とさないAPIにするための調整です。またNodeの型消去モードで動くよう、コンストラクターのparameter propertyは使っていません。

### 3. 原子性は1つのJavaScriptイベントループの範囲

出典：[typescript/dcb.ts](dcb.ts#L16-L17)

```typescript
// Operations have no await boundary: atomic within one JavaScript event loop.
// This store is not shared across Workers/processes. Consumers must not reenter it.
```

同期のcheck-and-appendにawaitを挟まないため、その範囲では原子的です。Promiseで同じ読み取りから競合させますが、他6言語の2スレッド実験と同じ並列メモリ共有ではありません。Workersやプロセス間で使うには別のストア実装が必要です。
