#!/usr/bin/env python3
"""Run and publish complete Intel Actions cohorts; never combine different runs."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import platform
import re
import shutil
import subprocess
import sys
import tempfile
import importlib.util
_spec=importlib.util.spec_from_file_location("level3_benchmark",Path(__file__).with_name("run.py"))
benchmark=importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(benchmark)

HERE=Path(__file__).resolve().parent
DEFAULT=HERE/'observed/macos-intel-actions'
LANGUAGES=benchmark.build_runner.LANGUAGES


def validate_cohort(data):
    if data['machine'] not in ['x86_64','AMD64'] or not data['platform'].startswith(('macOS','Linux')):
        raise ValueError('Native macOS or Linux x64 results required')
    pairs={(l,s)for l in LANGUAGES for s in ['sqlite','memory-json']}
    rows=data['results']
    if len(rows)!=14 or {(r['language'],r['store'])for r in rows}!=pairs:
        raise ValueError('Complete seven-language SQLite/memory cohort required')
    if len({r['stateSha256']for r in rows})!=1:raise ValueError('State hashes differ')
    for store in ['sqlite','memory-json']:
        selected=[r for r in rows if r['store']==store]
        if len({tuple(sorted(r['codec'].items()))for r in selected})!=1:raise ValueError('Codec counts differ')
    for row in rows:
        if row['students']!=data['students'] or row['wallSeconds']<=0 or row['peakRssMiB']<=0:raise ValueError('Invalid measurement')
        if row['store']=='sqlite' and ('restart'not in row or 'databaseValidation'not in row):raise ValueError('Missing persistence validation')
    if not data.get('actions',{}).get('runUrl'):raise ValueError('Actions provenance required')


def load_bundle(folder):
    manifest=json.loads((folder/'manifest.json').read_text())
    results=[]
    if manifest['formatVersion']!=1 or not manifest['files']:raise ValueError('Invalid manifest')
    for entry in manifest['files']:
        name=entry['name']
        if Path(name).name!=name or not name.startswith('students-') or not name.endswith('.json'):raise ValueError('Invalid artifact path')
        path=folder/name
        if hashlib.sha256(path.read_bytes()).hexdigest()!=entry['sha256']:raise ValueError('Artifact digest mismatch')
        data=json.loads(path.read_text());validate_cohort(data);results.append((name,data))
    if len({d['students']for _,d in results})!=len(results):raise ValueError('Duplicate size')
    for key in ['sourceCommit','sourceManifest','workloadSha256','runtimeVersions','hardware','actions']:
        if any(d[key]!=results[0][1][key]for _,d in results):raise ValueError('Mixed runs or sources: '+key)
    return results


def render_index(results,folder):
    data=results[0][1];a=data['actions'];cpu=data['hardware']['cpu'];ram=int(data['hardware']['memoryBytes'])/1024**3
    title='Linux x64' if data['platform'].startswith('Linux') else 'Intel'
    lines=['# '+title+' GitHub Actions ES benchmark','',f"CPU: {cpu} / RAM: {ram:g} GiB / `{a['runnerLabel']}`。",'',
           f"[Actions run]({a['runUrl']}) / [Measured source](https://github.com/{a['repository']}/tree/{data['sourceCommit']}/samples/decider_level3).",'',
           '全サイズ・7言語のSQLite／メモリJSONを、同じx64ジョブで順次測定しました。ビルドは計測外です。C#・F#はRelease JIT＋Dynamic PGOです。','',
           '| 学生数 | 結果 | Raw data |','|---:|---|---|']
    versions=data['runtimeVersions']
    dotnet=versions.get('dotnet','')
    sdk=re.search(r'Version:\s+(\S+)',dotnet)
    runtime=re.search(r'Host:\s+Version:\s+(\S+)',dotnet)
    configuration=['','## 測定環境','',f"OS: `{data['platform']}` / architecture: `{data['machine']}`。",'',
                   '| 言語 | 実測ツールチェーン |','|---|---|']
    configuration.append('| C# / F# | .NET SDK '+(sdk[1] if sdk else 'see raw data')+' / runtime '+(runtime[1] if runtime else 'see raw data')+' |')
    for label,key in [('TypeScript + Zod (Node)','node'),('Rust','rust'),('Java','java'),('Go','go'),('Swift','swift')]:
        value=versions.get(key,'unavailable').splitlines()[0]
        configuration.append(f'| {label} | {value.replace("|","/")} |')
    lines[6:6]=configuration+['','## 結果','']
    for name,d in results:lines.append(f"| {d['students']:,} | [SQLite／メモリJSON]({Path(name).with_suffix('.md').name}) | [JSON]({name}) |")
    lines+=['','## データ量による変化','','| 実装 | '+' | '.join(f"{d['students']:,}人 SQLite秒 / RSS MiB"for _,d in results)+' |', '|---|'+ '|'.join('---:'for _ in results)+'|']
    for language in LANGUAGES:
        cells=[]
        for _,d in results:
            r=next(r for r in d['results']if r['language']==language and r['store']=='sqlite')
            cells.append(f"{r['wallSeconds']:.3f} / {r['peakRssMiB']:.1f}")
        label='ts-zod'if language=='typescript'else language
        lines.append('| '+label+' | '+' | '.join(cells)+' |')
    lines+=['','各規模で全実装の状態ハッシュ・JSON往復回数が一致し、SQLiteの整合性・索引・再起動後の状態を検証済みです。RSSは入力や監査trace、ランタイム、複数のProjectionを含み、投影だけのメモリではありません。','',
            'SQLiteの版・ドライバー・ランタイムはraw dataに記録しています。各構成1回の大きな処理の観測であり、言語単体の統計的な順位ではありません。ARMローカルとはCPU・RAM・OSが違うため、表を混ぜていません。','',
            '[測定方法](../../README.md) / [ARMローカル結果](../README.md)','']
    (folder/'README.md').write_text('\n'.join(lines))


def publish(source,destination):
    results=load_bundle(source) # Validate everything before modifying tracked output.
    destination.mkdir(parents=True,exist_ok=True)
    expected={'manifest.json','README.md'}
    for name,data in results:
        benchmark.report(data,destination/name);shutil.copyfile(source/name,destination/name);expected.update([name,Path(name).with_suffix('.md').name])
    shutil.copyfile(source/'manifest.json',destination/'manifest.json')
    render_index(results,destination)
    for path in destination.iterdir():
        if path.is_file() and path.name not in expected and path.suffix in ['.json','.md']:path.unlink()


def run(sizes,output):
    if platform.system() not in ['Darwin','Linux'] or platform.machine()!='x86_64':raise ValueError('Run on native Linux or macOS x64')
    if not sizes or len(set(sizes))!=len(sizes) or any(n<10 or n%10 for n in sizes):raise ValueError('Sizes must be distinct positive multiples of ten')
    repository=os.environ['GITHUB_REPOSITORY'];run_id=os.environ['GITHUB_RUN_ID'];server=os.environ.get('GITHUB_SERVER_URL','https://github.com')
    actions={'repository':repository,'runId':run_id,'attempt':os.environ.get('GITHUB_RUN_ATTEMPT','1'),'runUrl':f'{server}/{repository}/actions/runs/{run_id}','runnerLabel':os.environ.get('BENCHMARK_RUNNER_LABEL','macos-15-intel'),'runnerImage':os.environ.get('ImageOS',''),'runnerImageVersion':os.environ.get('ImageVersion','')}
    output.mkdir(parents=True,exist_ok=True)
    # Stage a whole bundle: a failed later size must not publish a partial run.
    with tempfile.TemporaryDirectory(prefix='level3-intel-')as temp:
        stage=Path(temp);entries=[]
        for i,n in enumerate(sizes):
            path=stage/f'students-{n}.json'
            command=[sys.executable,str(HERE/'run.py'),'--students',str(n),'--store','both','--output',str(path)]
            if i:command.append('--no-build')
            subprocess.run(command,check=True)
            data=json.loads(path.read_text());data['actions']=actions;validate_cohort(data)
            path.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n')
            entries.append({'name':path.name,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()})
        (stage/'manifest.json').write_text(json.dumps({'formatVersion':1,'files':entries},indent=2)+'\n')
        publish(stage,output)

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);sub=p.add_subparsers(dest='action',required=True)
    r=sub.add_parser('run');r.add_argument('--sizes',default='22900,43050');r.add_argument('--output',type=Path,default=DEFAULT)
    u=sub.add_parser('publish');u.add_argument('source',type=Path);u.add_argument('destination',type=Path,nargs='?',default=DEFAULT)
    a=p.parse_args()
    if a.action=='run':run([int(n)for n in a.sizes.split(',')],a.output)
    else:publish(a.source,a.destination)
