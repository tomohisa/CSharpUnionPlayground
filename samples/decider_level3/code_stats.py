#!/usr/bin/env python3
"""Count maintained sources, separating domain code from reusable DCB infrastructure."""
import hashlib
import argparse
import json
from pathlib import Path
import sys
HERE=Path(__file__).resolve().parent
SOURCES=[('C#','csharp','.cs'),('F#','fsharp','.fs'),
 ('Rust','rust/src','.rs'),('TypeScript＋Zod','typescript','.ts'),
 ('Java','java','.java'),('Go','go','.go'),('Swift','swift','.swift')]
SKIP={'bin','obj','target','node_modules','.build'}
TESTS={'LibraryTests.cs','LibraryTests.fs','dcb_tests.rs','dcb.test.ts','DcbTests.java','libraryTests.swift'}
ROLES=['Model','Decider','Command','Connection','Projection','UseCase','Boundary','Library','DomainTest']
LABELS=['Model・値オブジェクト','Decider','コマンド・共通Handler・入力検証','DCB接続・メタ情報','ドメイン投影','ユースケース','アプリ設定・JSON・CLI','共通ライブラリ','ドメインテスト']
START,END='<!-- code-stats:start -->','<!-- code-stats:end -->'
def collect():
 manifest=json.loads((HERE/'code_stats_classification.json').read_text())
 seen=set();rows=[]
 for language,root,suffix in SOURCES:
  directory=HERE/root;groups={r:{'lines':0,'characters':0} for r in ROLES};details=[]
  for path in sorted(directory.rglob('*')):
   if not path.is_file() or path.suffix!=suffix or SKIP.intersection(path.relative_to(directory).parts) or path.name in TESTS or (path.name.endswith('_test.go') and path.name!='domain_test.go'):continue
   name=path.relative_to(HERE).as_posix();seen.add(name)
   if name not in manifest:raise ValueError('Source needs classification: '+name)
   entry=manifest[name];raw=path.read_bytes()
   if hashlib.sha256(raw).hexdigest()!=entry['sha256']:raise ValueError('Review classification and refresh SHA256 after source change: '+name)
   lines=raw.decode().splitlines();roles=[entry['role']]*len(lines);assigned=set()
   for region in entry['ranges']:
    start,end,role=region['start'],region['end'],region['role']
    if not 1<=start<=end<=len(lines):raise ValueError('Invalid range: '+name)
    if region.get('startText',lines[start-1])!=lines[start-1] or region.get('endText',lines[end-1])!=lines[end-1]:raise ValueError('Range boundary changed: '+name)
    for index in range(start-1,end):
     if index in assigned:raise ValueError('Overlapping range: '+name)
     assigned.add(index);roles[index]=role
   segments=[]
   for index,(line,role) in enumerate(zip(lines,roles),1):
    if role not in ROLES:raise ValueError('Unknown role: '+role)
    groups[role]['lines']+=1
    groups[role]['characters']+=sum(not c.isspace() for c in line)
    if segments and segments[-1]['role']==role:segments[-1]['end']=index
    else:segments.append({'start':index,'end':index,'role':role})
   details.append({'path':name,'lines':len(lines),'characters':sum(not c.isspace() for c in raw.decode()),'segments':segments})
  for key in ['lines','characters']:
   groups.setdefault('ApplicationTotal',{})[key]=sum(groups[r][key] for r in ROLES if r not in ['Library','DomainTest'])
   groups.setdefault('Total',{})[key]=sum(groups[r][key] for r in ROLES)
   if groups['Total'][key]!=sum(f[key] for f in details):raise ValueError('Partition does not match source total')
  rows.append({'language':language,'groups':groups,'files':details})
 if seen!=set(manifest):raise ValueError('Stale classification: '+str(set(manifest)-seen))
 return rows

def markdown(rows):
 out=[]
 for metric,title in [('lines','行数（空行を含む）'),('characters','空白以外の文字数')]:
  columns=ROLES[:-2]+['ApplicationTotal','Library','DomainTest']
  labels=LABELS[:-2]+['アプリ側合計（ライブラリ・テスト除外）','共通ライブラリ（別枠）','ドメインテスト（別枠）']
  out += ['### '+title,'','| 言語 | '+' | '.join(labels)+' |','|---|'+ '|'.join('---:' for _ in columns)+'|']
  for row in rows:out.append('| '+row['language']+' | '+' | '.join(f"{row['groups'][r][metric]:,}" for r in columns)+' |')
  out.append('')
 out+=['### 分類したソース範囲','','同じファイルの中も責務で分けています。以下は行数・文字数に共通の分類です。','', '| 言語 | 分類 | ソース範囲 |','|---|---|---|']
 for row in rows:
  for role,label in zip(ROLES,LABELS):
   links=[]
   for f in row['files']:
    for segment in f['segments']:
     if segment['role']==role:
      start,end=segment['start'],segment['end'];path=f['path']
      links.append(f'[{path}:{start}–{end}]({path}#L{start}-L{end})')
   out.append('| '+row['language']+' | '+label+' | '+' / '.join(links)+' |')
 return '\n'.join(out).rstrip()+'\n'

def main():
 p=argparse.ArgumentParser(description=__doc__);m=p.add_mutually_exclusive_group();m.add_argument('--write',action='store_true');m.add_argument('--check',action='store_true');m.add_argument('--json',action='store_true');a=p.parse_args();rows=collect();generated=markdown(rows)
 if a.json:print(json.dumps(rows,ensure_ascii=False,indent=2));return 0
 if not(a.write or a.check):print(generated,end='');return 0
 path=HERE/'README.md';text=path.read_text();before,rest=text.split(START,1);current,after=rest.split(END,1);wanted='\n\n'+generated+'\n'
 if a.write:path.write_text(before+START+wanted+END+after)
 elif current!=wanted:print('Run python3 samples/decider_level3/code_stats.py --write',file=sys.stderr);return 1
 else:print('Level 3 code statistics: up to date')
 return 0
if __name__=='__main__':raise SystemExit(main())
