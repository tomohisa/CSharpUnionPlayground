#!/usr/bin/env python3
"""One reviewed line partition for both line and non-whitespace counts."""
import argparse,hashlib,json
from pathlib import Path
HERE=Path(__file__).resolve().parent
ROLES=['Model','Decider','Command','Connection','Projection','UseCase','Boundary','DomainTest','HttpTest','Library']
LABELS=['Model・VO','Decider','コマンド・Handler・検証','DCB接続・メタ情報','ドメイン投影','ユースケース','HTTP設定・JSON・プロトコル','ドメインテスト','共通HTTPテスト','共通ライブラリ（別枠）']
LANGUAGES=['C#','F#','Rust','TypeScript＋Zod','Java','Go','Swift']
START,END='<!-- code-stats:start -->','<!-- code-stats:end -->'
def collect():
    manifest=json.loads((HERE/'code_stats_classification.json').read_text());rows=[]
    for language in LANGUAGES:
        groups={r:{'lines':0,'characters':0}for r in ROLES};files=[]
        for item in manifest[language]:
            path=(HERE/item['path']).resolve();raw=path.read_bytes()
            if hashlib.sha256(raw).hexdigest()!=item['sha256']:raise ValueError('Review classification after source change: '+item['path'])
            lines=raw.decode().splitlines();roles=[item['role']]*len(lines);assigned=set()
            for region in item['ranges']:
                a,b=region['start'],region['end'];assert 1<=a<=b<=len(lines)
                assert lines[a-1]==region['startText']and lines[b-1]==region['endText'],item['path']
                for index in range(a-1,b):
                    assert index not in assigned;assigned.add(index);roles[index]=region['role']
            segments=[]
            for index,(line,role)in enumerate(zip(lines,roles),1):
                assert role in groups,(item['path'],role)
                groups[role]['lines']+=1;groups[role]['characters']+=sum(not c.isspace()for c in line)
                if segments and segments[-1]['role']==role:segments[-1]['end']=index
                else:segments.append({'start':index,'end':index,'role':role})
            files.append({'path':item['path'],'segments':segments,'lines':len(lines),'characters':sum(not c.isspace()for c in raw.decode())})
        groups['ApplicationTotal']={key:sum(groups[r][key]for r in ROLES if r!='Library')for key in ['lines','characters']}
        for key in ['lines','characters']:assert sum(groups[r][key]for r in ROLES)==sum(f[key]for f in files)
        rows.append({'language':language,'groups':groups,'files':files})
    return rows

def markdown(rows):
    lines=['Level3を再利用するソースも含めた、このHTTPアプリに必要な維持コードの比較です。Level3の古いコンソール入口・Application・ベンチマークは除外します。共有ファイル内の未使用の宣言をコンパイラーのdead-code eliminationで差し引く集計ではありません。','',
           'ドメインテストに加え、全言語へ適用する同じPythonの共通HTTPテストを各アプリのテスト費用として含めています。この共有ファイルをリポジトリ内で7つ実装しているという意味ではありません。共通ライブラリは別枠で、アプリ側合計に含めません。OpenAPI、ビルド設定、ロックファイル、README、共通ビルドランチャー、計測器は言語別のコード表から除外します。','']
    for metric,title in [('lines','行数（空行を含む）'),('characters','空白以外の文字数')]:
        columns=ROLES[:-1]+['ApplicationTotal','Library'];labels=LABELS[:-1]+['アプリ側合計（テスト含む）',LABELS[-1]]
        lines+=['### '+title,'','| 言語 | '+' | '.join(labels)+' |','|---|'+'|'.join('---:'for _ in columns)+'|']
        for row in rows:lines.append('| '+row['language']+' | '+' | '.join(f"{row['groups'][k][metric]:,}"for k in columns)+' |')
        lines.append('')
    lines+=['### 同じ分類に使ったソース範囲','','| 言語 | 分類 | 実コード |','|---|---|---|']
    for row in rows:
        for role,label in zip(ROLES,LABELS):
            refs=[]
            for file in row['files']:
                for s in file['segments']:
                    if s['role']==role:refs.append(f"[{file['path']}:{s['start']}–{s['end']}]({file['path']}#L{s['start']}-L{s['end']})")
            lines.append('| '+row['language']+' | '+label+' | '+' / '.join(refs)+' |')
    return '\n'.join(lines)+'\n'
if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--write',action='store_true');p.add_argument('--check',action='store_true');p.add_argument('--json',action='store_true');a=p.parse_args();rows=collect()
    if a.json:print(json.dumps(rows,ensure_ascii=False,indent=2))
    else:
        result=markdown(rows)
        if a.write or a.check:
            path=HERE/'README.md';text=path.read_text();before,rest=text.split(START);old,after=rest.split(END);wanted='\n\n'+result+'\n'
            if a.write:path.write_text(before+START+wanted+END+after)
            else:assert old==wanted,'Run code_stats.py --write';print('Level4 statistics PASS')
        else:print(result)
