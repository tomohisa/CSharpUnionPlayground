# TypeScript＋Zod：型付きイベントと構造的なportを使う

[共通仕様・検証](../README.md)／[Model・Decider・共通部品のコード量](../README.md#コード量の比較)。このES／DCB実装で助かった点と増えた負担を比較します。このページでは記述を比較し、速度・メモリは[ベンチマーク](../benchmark/README.md)で比較します。

## この設計でよかったこと

### 1. Zodのドメイン型を、Zodに依存しない共通部品へ渡せる

出典：[dcb.ts:12–15](dcb.ts#L12-L15)

```typescript
export interface CommandStore<E> {
  read(query:Query):ReadResult<E>;
  append(commandId:string,condition:Query,after:number,batch:readonly E[]):AppendResult<E>;
}
```

出典：[handlers.ts:1–16](handlers.ts#L1-L16)

```typescript
import type {CommandHandler} from "./dcb.ts";
import {CommandSchema} from "./models.ts";
import type {Command,Event,Rejection} from "./models.ts";
import {queryFor} from "./routing.ts";
import {decide} from "./decider.ts";
import {fold,decisionModel} from "./projection.ts";
import {ValidatedCommand} from "./valueObjects.ts";
export const schoolHandler:CommandHandler<Command,Event,Rejection> = {
  validate:command=>ValidatedCommand.validate(command),
  snapshot:command=>CommandSchema.parse(command),
  queryFor,
  handle(events,command){
    const decision=decide(decisionModel(fold(events)),ValidatedCommand.create(command));
    return decision.$type==="Accepted"?{kind:"Emit",events:decision.events}:{kind:"Reject",error:decision.code};
  }
};
```

ライブラリのdcb.tsにはZodのimportも学校の型もありません。学校側はスキーマから得たEvent／Command／Rejectionを渡すだけです。デシリアライズはwire.tsの境界に残します。

### 2. 生の閉じたイベントを返したまま、タグ登録を一か所へ集められる

出典：[routing.ts:24–28](routing.ts#L24-L28)

```typescript
export function describeEvent(event: Event): EventDescriptor {
  switch(event.$type) {
    case "StudentRegistered": return {eventType:"StudentRegistered",tags:[studentTag(event.studentId)]};
    case "StudentPromoted": return {eventType:"StudentPromoted",tags:[studentTag(event.studentId)]};
    case "ClassOpened": return {eventType:"ClassOpened",tags:[classTag(event.classId)]};
```

すべてのイベントを処理した後はneverで未対応を検出できます。型アサーションで汎用Eventから戻す方式にしません。Storedの位置・コマンドID・タグ検査はライブラリ側へ集めています。

### 3. 複合キーと投影コピーを標準の機能で表せる

出典：[projection.ts:8–9](projection.ts#L8-L9)

```typescript
export const copy = (s: Registers): Registers => structuredClone(s);
const key = (room:string,student:string) => JSON.stringify([room,student]);
```

Mapのコピーと更新を明示し、JSONのtuple表現で履修キーを区別します。取得済みの一覧が後で変わらないこと、構造化キーが衝突しないことを共通シナリオで確認します。

## 実装で大変だったこと・残る制約

### 1. 共通メタ情報の実行時スキーマとの接続は残る

出典：[wire.ts:3–4](wire.ts#L3-L4)

```typescript
export const InputSchema=z.object({history:z.array(z.object({position:z.int().positive(),commandId:z.string().min(1),eventType:z.string().min(1),
  tags:z.array(z.object({category:z.string(),keys:z.array(z.string())})),payload:EventSchema})),operations:z.array(OperationSchema)});
```

ドメインのケースはZodから導出できますが、ライブラリをZod非依存にした分、エンベロープのJSONスキーマを境界で記述しています。構造はtsc、タグとペイロードの意味の一致はStore.loadのdescribeで確認します。

### 2. ジェネリック関数の推論にも調整が必要だった

出典：[dcb.ts:24–24](dcb.ts#L24-L24)

```typescript
  constructor(describe:(event:E)=>EventDescriptor,copyPayload:(event:NoInfer<E>)=>NoInfer<E>){this.describe=describe;this.copyPayload=copyPayload;}
```

structuredCloneのようなジェネリック関数からEがunknownへ引っ張られないよう、コピー引数にNoInferを使いました。ドメイン型の情報を落とさないAPIにするための調整です。またNodeの型消去モードで動くよう、コンストラクターのparameter propertyは使っていません。

### 3. 原子性は1つのJavaScriptイベントループの範囲

出典：[dcb.ts:16–17](dcb.ts#L16-L17)

```typescript
// Operations have no await boundary: atomic within one JavaScript event loop.
// This store is not shared across Workers/processes. Consumers must not reenter it.
```

同期のcheck-and-appendにawaitを挟まないため、その範囲では原子的です。Promiseで同じ読み取りから競合させますが、他6言語の2スレッド実験と同じ並列メモリ共有ではありません。Workersやプロセス間で使うには別のストア実装が必要です。

## コマンド・検証・履歴復元の更新

ZodのbrandでStudentIdとClassIdを名目的に分け、safeParseで実行時にも検証します。CLIだけでなくExecutorからの直接呼び出しでも検証します。prepare時にCommandSchema.parseでコピーを凍結するため、呼び出し元が元オブジェクトを変更しても再判断の入力は変わりません。asによる型の偽装そのものは可能なので、入口の実行時検証が必要です。

[handlers.ts](handlers.ts)は入力検証とDecider接続、[valueObjects.ts](valueObjects.ts)は値の制約と検証済み入力、[domain.test.ts](domain.test.ts)はドメインテストです。Routingはタグ・読み取り条件・イベントのメタ情報を担当し、業務ハンドラーとは別分類です。

現在のポリシーで値を検証するcreate系factoryと、過去の事実を再検証しないfromEvent／restore系factoryを区別します。イベント・JSONの保存形と一覧DTOはプリミティブを保ち、投影で復元した値を取り出して使います。全てのStateの全フィールドを名目的なVOへ置き換えた実装ではありません。ValidatedCommandも永続化せず、新しいコマンドを受ける境界だけに使います。

76ケースのGWTではExecutor経由の結果と正確なイベントpayload、拒否時の無追記、投影と再構築の一致を確認します。これとは別に、ストアを使わないDeciderの3例、VOのfactoryと履歴復元、最後の1席の競合再判断もテストします。言語固有の不正入力ケースも追加しています。テスト全体をコード量の別枠へ計上しています。

## 大きな一覧Projectionの更新

学校全体のライブ一覧は[schoolProjection.ts](schoolProjection.ts)へ分離しました。既存のProjectionにある不変MapやコピーするReducerは、タグで絞ったコマンド判断用モデルと参照再生に残しています。ライブ一覧の更新には使いません。

ライブ一覧は汎用IndexedProjectionのハッシュ索引を使い、バッチ中に触った行だけを差分へ積み、全イベントを処理できてから反映します。クラスの名簿はイベント位置を内部ノードIDにした連結索引です。追加・取消で名簿全体をコピーせず、前後のノードだけを更新します。削除したノードは実際に索引から削除するため、同じ生徒の取消と再登録を繰り返してもノードを蓄積しません。

一覧要求時だけ名簿の配列／listと並び順を持つDTOを組み立てます。過去のDTOはその後のイベントで変わりません。全件Snapshotは出力サイズに比例する仕事があり、ロック中の読み取りもあるため、大規模な並行クエリまで一定時間という保証ではありません。頻繁な大量一覧の提供には、ページングや公開用Snapshotの別管理が必要です。

[projection.test.ts](projection.test.ts)では1千・1万・10万件を作り、更新と名簿拡大の時間を全件Snapshotと分けて測ります。途中失敗・位置の欠落で部分更新しないこと、再配送、過去Snapshotの不変性、取消と再登録後の順序と行数も検証します。[ローカルの測定値](../observed/projection-scaling.md)は言語の総合性能順位ではなく、この更新経路のサイズ依存を見るためのものです。

## JSON／SQLiteと計測

Nodeのnode:sqlite。JSON復元後のイベントはZodを通します。SQLiteとJSONの同期APIを直列に使うため、このサンプルは非同期I/Oのスループット比較ではありません。

[共通ベンチマーク](../benchmark/README.md)では、履修コマンド・拒否・競合後の再判断、索引Projectionのクエリ、JSON応答、再起動を同じ件数で実行します。SQLite版とメモリJSON版の両方でシリアライズ・デシリアライズを通します。インフラ障害は業務のRejectedと混ぜず、プロセスを失敗させます。外部接続のイベントは明示的なRefreshまたは次の自分のcommitで取り込みます。
