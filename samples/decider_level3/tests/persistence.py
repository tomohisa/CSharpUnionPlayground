#!/usr/bin/env python3
"""Seven native adapters, SQLite restart/handoff, and competing CLI writers."""
import argparse
import concurrent.futures
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
import run as runner
sys.path.insert(0,str(runner.HERE/'benchmark'))
from workload import generate,validate


def execute(command,payload,path):
    result=subprocess.run(command,input=json.dumps(payload),text=True,capture_output=True,timeout=120,
                          env={**os.environ,'DECIDER_PERSISTENT':str(path)})
    if result.returncode:raise AssertionError(result.stderr)
    return runner.canonical(json.loads(result.stdout))


def main():
    p=argparse.ArgumentParser();p.add_argument('--language',default='all');a=p.parse_args()
    languages=runner.LANGUAGES if a.language=='all' else a.language.split(',')
    commands={l:runner.build(l)for l in languages}
    payload,expected=generate(100);hashes=set()
    with tempfile.TemporaryDirectory(prefix='level3-persistence-')as folder:
        root=Path(folder)
        for source in languages:
            path=root/(source+'.db');result=execute(commands[source],payload,path);hashes.add(validate(result,expected))
            for target in languages:
                replay=execute(commands[target],{'history':[],'operations':[{'$type':'ListAll'}]},path)
                assert validate(replay,expected,restart=True) in hashes
            print(source+': SQLite write / seven-language JSON replay PASS',flush=True)
        assert len(hashes)==1
        for language in languages:
            path=root/(language+'-race.db')
            def op(kind,**fields):return {'$type':'Execute','commandId':kind+str(fields),'command':{'$type':kind,**fields}}
            seed={'history':[],'operations':[op('RegisterStudent',studentId='a',grade='Year3'),op('RegisterStudent',studentId='b',grade='Year3'),op('OpenClass',classId='race',course='Foundation',capacity=1)]}
            execute(commands[language],seed,path)
            with concurrent.futures.ThreadPoolExecutor(2)as pool:
                outputs=list(pool.map(lambda id:execute(commands[language],{'history':[],'operations':[op('Enroll',classId='race',studentId=id)]},path),['a','b']))
            statuses=sorted(out['steps'][0]['status'] for out in outputs)
            assert statuses==['Committed','Rejected'],(language,statuses)
            rejected=next(out['steps'][0]for out in outputs if out['steps'][0]['status']=='Rejected')
            assert rejected['errors']==['ClassFull']
            final=execute(commands[language],{'history':[],'operations':[{'$type':'ListAll'}]},path)
            assert len(final['list']['enrollments'])==1 and len(final['events'])==4
            assert final['list']==final['rebuilt'] and final['listenerFaults']==0
            print(language+': two-process writers / no oversubscription PASS',flush=True)
    print('SQLite cross-language persistence and concurrency PASS')

if __name__=='__main__':main()
