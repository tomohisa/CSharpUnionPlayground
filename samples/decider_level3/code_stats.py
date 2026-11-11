#!/usr/bin/env python3
"""Count maintained sources, separating domain code from reusable DCB infrastructure."""
import argparse
import json
from pathlib import Path
import sys
HERE=Path(__file__).resolve().parent
SOURCES=[('C#','csharp','.cs','Models.cs','Decider.cs','Dcb.cs','Projection.cs',['Routing.cs','Application.cs']),
 ('F#','fsharp','.fs','Models.fs','Decider.fs','Dcb.fs','Projection.fs',['Routing.fs','Application.fs']),
 ('Rust','rust/src','.rs','models.rs','decider.rs','dcb.rs','projection.rs',['routing.rs','application.rs']),
 ('TypeScript＋Zod','typescript','.ts','models.ts','decider.ts','dcb.ts','projection.ts',['routing.ts','application.ts']),
 ('Java','java','.java','Models.java','Decider.java','playground/dcb/Dcb.java','Projection.java',['Routing.java','Application.java']),
 ('Go','go','.go','domain/models.go','domain/decider.go','dcb/store.go','domain/projection.go',['domain/routing.go','application.go']),
 ('Swift','swift','.swift','models.swift','decider.swift','dcb.swift','projection.swift',['routing.swift','application.swift'])]
SKIP={'bin','obj','target','node_modules','.build'}
TESTS={'LibraryTests.cs','LibraryTests.fs','dcb_tests.rs','dcb.test.ts','DcbTests.java','libraryTests.swift'}
ROLES=['Model','Decider','Library','Projection','Application','Boundary']
START,END='<!-- code-stats:start -->','<!-- code-stats:end -->'
def collect():
 rows=[]
 for language,root,suffix,model,decider,library,projection,application in SOURCES:
  directory=HERE/root;groups={r:{'lines':0,'characters':0} for r in ROLES};details=[]
  for path in sorted(directory.rglob('*')):
   if not path.is_file() or path.suffix!=suffix or SKIP.intersection(path.relative_to(directory).parts) or path.name in TESTS or path.name.endswith('_test.go'):continue
   name=path.relative_to(directory).as_posix()
   role='Model' if name==model else 'Decider' if name==decider else 'Library' if name==library else 'Projection' if name==projection else 'Application' if name in application else 'Boundary'
   text=path.read_text();metrics={'lines':len(text.splitlines()),'characters':sum(not c.isspace() for c in text)}
   for k,v in metrics.items():groups[role][k]+=v
   details.append({'path':path.relative_to(HERE).as_posix(),'role':role,**metrics})
  for name in [model,decider,library,projection,*application]:
   if not any(f['path']==(directory/name).relative_to(HERE).as_posix() for f in details):raise ValueError(language+': required source missing '+name)
  groups['その他']={k:sum(groups[r][k] for r in ROLES[2:]) for k in ['lines','characters']}
  groups['合計']={k:sum(groups[r][k] for r in ROLES) for k in ['lines','characters']}
  rows.append({'language':language,'groups':groups,'files':details})
 return rows

def markdown(rows):
 out=[]
 for metric,title in [('lines','行数（空行を含む）'),('characters','空白以外の文字数')]:
  for detail in [False,True]:
   columns=ROLES[2:] if detail else ['Model','Decider','その他','合計']
   labels=['共通ライブラリ','投影','アプリ接続','JSON・CLI'] if detail else columns
   out += ['### '+title+('：その他の内訳' if detail else ''),'','| 言語 | '+' | '.join(labels)+' |','|---|'+ '|'.join('---:' for _ in columns)+'|']
   for row in rows:out.append('| '+row['language']+' | '+' | '.join(f"{row['groups'][r][metric]:,}" for r in columns)+' |')
   out.append('')
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
