#!/usr/bin/env python3
"""Measure and publish complete, unmixed Linux x64 HTTP cohorts."""
import argparse,hashlib,importlib.util,json,math,os,platform,shutil,subprocess,sys,tempfile
from pathlib import Path
HERE=Path(__file__).resolve().parent
sys.path.insert(0,str(HERE))
spec=importlib.util.spec_from_file_location('level4_http_benchmark',HERE/'run.py')
benchmark=importlib.util.module_from_spec(spec);spec.loader.exec_module(benchmark)
LANGUAGES=benchmark.builder.LANGUAGES
DEFAULT=HERE/'observed/linux-x64-actions'

def validate(data):
    if data['machine'] not in ['x86_64','AMD64']or not data['platform'].startswith('Linux'):raise ValueError('Linux x64 measurements required')
    if data.get('sourceDirty')is not False:raise ValueError('Clean measured source required')
    rows=data['results'];pairs={(l,s)for l in LANGUAGES for s in ['sqlite','memory-json']}
    if len(rows)!=14 or {(r['language'],r['store'])for r in rows}!=pairs:raise ValueError('Complete seven-language two-store cohort required')
    n=data['students'];groups=n//10;rounds=min(50,groups)
    if n<10 or n%10 or data['concurrency']<1:raise ValueError('Invalid workload')
    if len({r['stateSha256']for r in rows})!=1:raise ValueError('State hashes differ')
    for r in rows:
        expected={'students':n,'warmupStudents':100,'concurrency':data['concurrency'],'events':73*(groups+10)+2*rounds,'requests':84*groups+5*rounds+3,'expectedRejections':groups+rounds,'conflicts':rounds,'unexpectedErrors':0,'errorRate':0}
        if any(r[k]!=v for k,v in expected.items()):raise ValueError('Workload or error counters differ')
        for key in ['seconds','requestsPerSecond','sampledPeakRssMiB','startupSeconds','shutdownSeconds']:
            if not math.isfinite(r[key])or r[key]<=0:raise ValueError('Invalid measurement: '+key)
        if r['rssSamples']<1 or (r['store']=='sqlite'and not(r['restartReadySeconds']and r['restartReadySeconds']>0)):raise ValueError('Missing memory or restart verification')
    if not data.get('actions',{}).get('runUrl'):raise ValueError('Actions provenance required')

def load_bundle(folder):
    manifest=json.loads((folder/'manifest.json').read_text());rows=[]
    if manifest['formatVersion']!=1 or not manifest['files']:raise ValueError('Invalid manifest')
    for item in manifest['files']:
        name=item['name']
        if Path(name).name!=name or not name.startswith('students-')or not name.endswith('.json'):raise ValueError('Invalid artifact path')
        raw=(folder/name).read_bytes()
        if hashlib.sha256(raw).hexdigest()!=item['sha256']:raise ValueError('Artifact digest mismatch')
        data=json.loads(raw);validate(data);rows.append((name,data))
    if len({d['students']for _,d in rows})!=len(rows):raise ValueError('Duplicate size')
    for key in ['sourceCommit','sourceManifest','openapiSha256','runtimeVersions','hardware','concurrency','actions']:
        if any(d[key]!=rows[0][1][key]for _,d in rows):raise ValueError('Mixed runs or sources: '+key)
    return rows

def render_index(rows,folder):
    d=rows[0][1];a=d['actions'];h=d['hardware'];versions=d['runtimeVersions']
    lines=['# Linux x64 Actions HTTP benchmark','',f"CPU: {h['cpu']} / RAM: {int(h['memoryBytes'])/1024**3:g} GiB / `{a['runnerLabel']}`。",'',f"OS: `{d['platform']}` / logical CPUs: {h.get('logicalCpuCount','see raw data')}。",'',f"[Actions run]({a['runUrl']}) / [Measured source](https://github.com/{a['repository']}/tree/{d['sourceCommit']}/samples/decider_level4).",'',
    '全7言語・SQLite／メモリJSONを同じLinux x64ジョブで順次測定しました。ビルドと機能テストは計測外、C#・F#はRelease JIT＋Dynamic PGOです。','',
    'C#はKestrel上のminimal hosting構成で、コントローラーを使いません。現実装はapp.Runによる共通ディスパッチャーであり、MapGet／MapPostでエンドポイントを定義する形式ではありません。','',
    '| runtime / compiler | measured version |','|---|---|']
    for key,v in versions.items():lines.append('| '+key+' | '+(' / '.join(v.splitlines()[:3])if key=='dotnet'else v.splitlines()[0]).replace('|','/')+' |')
    for name,data in rows:
        lines+=['',f"## {data['students']:,} students / concurrency {data['concurrency']}",'',f"[遅延・起動・終了の全結果]({Path(name).with_suffix('.md').name}) / [Raw data]({name})。",'',
        '| 実装 | store | wall s | req/s | p95 ms | p99 ms | unexpected error % | expected rejection | sampled RSS MiB |','|---|---|---:|---:|---:|---:|---:|---:|---:|']
        for r in data['results']:
            label='ts-zod'if r['language']=='typescript'else r['language'];source=os.path.relpath(benchmark.ROOT/r['language'],folder)
            lines.append(f"| [{label}]({source}) | {r['store']} | {r['seconds']:.3f} | {r['requestsPerSecond']:.1f} | {r['latencyMs']['p95']:.3f} | {r['latencyMs']['p99']:.3f} | {100*r['errorRate']:.2f} | {r['expectedRejections']} | {r['sampledPeakRssMiB']:.1f} |")
    lines+=['','全構成でイベント数・HTTP件数・期待した拒否・競合数・状態ハッシュが一致し、SQLite再起動後の復元も検証済みです。予期しない失敗・不完全なcohortは公開しません。','',
    'RSSは100ms間隔のサーバーPID観測値です。起動・終了・再起動は別計測で、クライアントCPU時間もraw dataに含みます。固定同時接続数の閉ループ測定であり、HTTPスタック・JSON codec・SQLiteドライバーを含む実装の比較です。言語単体の最大性能・統計的順位を示すものではありません。','',
    '[測定方法](../../README.md) / [別環境のローカル結果](../README.md)','']
    (folder/'README.md').write_text('\n'.join(lines))

def publish(source,destination):
    rows=load_bundle(source)
    destination.mkdir(parents=True,exist_ok=True);expected={'README.md','manifest.json'}
    for name,data in rows:
        benchmark.report(data,destination/name)
        shutil.copyfile(source/name,destination/name)
        expected.update([name,Path(name).with_suffix('.md').name])
    shutil.copyfile(source/'manifest.json',destination/'manifest.json');render_index(rows,destination)
    for p in destination.iterdir():
        if p.is_file()and p.name not in expected and p.suffix in ['.md','.json']:p.unlink()

def run(sizes,concurrency,output):
    if platform.system()!='Linux'or platform.machine()!='x86_64':raise ValueError('Native Linux x64 required')
    if not sizes or len(set(sizes))!=len(sizes)or any(n<10 or n%10 for n in sizes)or concurrency<1:raise ValueError('Invalid workload')
    repo=os.environ['GITHUB_REPOSITORY'];runid=os.environ['GITHUB_RUN_ID'];server=os.environ.get('GITHUB_SERVER_URL','https://github.com')
    provenance={'repository':repo,'runId':runid,'attempt':os.environ.get('GITHUB_RUN_ATTEMPT','1'),'runUrl':f'{server}/{repo}/actions/runs/{runid}','runnerLabel':os.environ['BENCHMARK_RUNNER_LABEL'],'image':os.environ.get('ImageOS',''),'imageVersion':os.environ.get('ImageVersion','')}
    with tempfile.TemporaryDirectory(prefix='level4-actions-')as temp:
        stage=Path(temp);entries=[]
        for n in sizes:
            path=stage/f'students-{n}.json'
            subprocess.run([sys.executable,str(HERE/'run.py'),'--students',str(n),'--concurrency',str(concurrency),'--store','both','--no-build','--output',str(path)],check=True)
            data=json.loads(path.read_text());data['actions']=provenance;validate(data);path.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n')
            entries.append({'name':path.name,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()})
        (stage/'manifest.json').write_text(json.dumps({'formatVersion':1,'files':entries},indent=2)+'\n');publish(stage,output)

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);sub=p.add_subparsers(dest='action',required=True)
    r=sub.add_parser('run');r.add_argument('--sizes',default='10000');r.add_argument('--concurrency',type=int,default=8);r.add_argument('--output',type=Path,default=DEFAULT)
    u=sub.add_parser('publish');u.add_argument('source',type=Path);u.add_argument('destination',type=Path,nargs='?',default=DEFAULT)
    a=p.parse_args()
    if a.action=='run':run([int(n)for n in a.sizes.split(',')],a.concurrency,a.output)
    else:publish(a.source,a.destination)
