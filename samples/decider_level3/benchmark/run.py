#!/usr/bin/env python3
"""Run identical ES workloads sequentially in seven native runtimes."""
import argparse
import datetime
import hashlib
import json
import os
from pathlib import Path
import platform
import re
import shutil
import signal
import sqlite3
import subprocess
import sys
import tempfile
import threading
import time

HERE=Path(__file__).resolve().parent
LEVEL=HERE.parent
sys.path.insert(0,str(LEVEL))
import run as build_runner
from workload import generate,validate


def database_check(path,expected,documents):
    with sqlite3.connect(path) as db:
        assert db.execute('PRAGMA integrity_check').fetchone()[0]=='ok'
        with sqlite3.connect(':memory:') as reference:
            reference.executescript((HERE/'schema.sql').read_text())
            for table in ['events','event_tags']:
                assert db.execute(f'PRAGMA table_info({table})').fetchall()==reference.execute(f'PRAGMA table_info({table})').fetchall()
        assert db.execute('SELECT COUNT(*) FROM events').fetchone()[0]==expected['events']
        assert db.execute('SELECT MIN(position),MAX(position) FROM events').fetchone()==(1,expected['events'])
        assert not db.execute('PRAGMA foreign_key_check').fetchall()
        for kind,body in db.execute('SELECT event_type,payload_json FROM events'):
            assert json.loads(body)['$type']==kind
        plan=' '.join(str(r) for r in db.execute('EXPLAIN QUERY PLAN SELECT position FROM event_tags WHERE category=? AND keys_key=? AND position>? AND position<=?',('student',b'',0,expected['events'])))
        assert 'tags_match' in plan,plan
        count_tags=db.execute('SELECT COUNT(*) FROM event_tags').fetchone()[0]
    with sqlite3.connect(str(path)+'.documents') as db:
        assert db.execute('PRAGMA integrity_check').fetchone()[0]=='ok'
        assert db.execute('SELECT COUNT(*) FROM events').fetchone()[0]==16*documents
        for revisions, in db.execute("SELECT COUNT(DISTINCT json_extract(payload_json,'$.revision')) FROM events GROUP BY json_extract(payload_json,'$.documentId')"):
            assert revisions==16
        for body, in db.execute('SELECT payload_json FROM events'):
            assert len(json.loads(body)['body'].encode())==4096
    return {'events':expected['events'],'tagRows':count_tags,'queryPlan':plan}


def invoke(command,payload,folder,store,documents,timeout,students=0):
    inp=folder/'input.json';out=folder/'output.json';err=folder/'stderr.txt';stats=folder/'time.txt'
    inp.write_text(json.dumps(payload,ensure_ascii=False,separators=(',',':')))
    env={**os.environ,'DECIDER_BENCHMARK':'1','DECIDER_BENCHMARK_SUMMARY':'1','DECIDER_DOCUMENTS':str(documents),'DECIDER_STUDENTS':str(students),
         'DOTNET_TieredCompilation':'1','DOTNET_TieredPGO':'1'}
    env.pop('DECIDER_PERSISTENT',None)
    if store=='sqlite':env['DECIDER_PERSISTENT']=str(folder.parent/'school.db')
    timer=['/usr/bin/time',*(['-l'] if sys.platform=='darwin' else ['-v']),'-o',str(stats)]
    marks=[];start=time.monotonic()
    with inp.open('rb') as source,out.open('wb') as target,err.open('w') as errors:
        process=subprocess.Popen(timer+command,stdin=source,stdout=target,stderr=subprocess.PIPE,env=env,text=True,start_new_session=True)
        def collect():
            for line in process.stderr:
                marks.append((time.monotonic()-start,line.strip()));errors.write(line);errors.flush()
        reader=threading.Thread(target=collect);reader.start()
        try:code=process.wait(timeout=timeout)
        except (subprocess.TimeoutExpired,KeyboardInterrupt):
            os.killpg(process.pid,signal.SIGKILL);process.wait();reader.join();raise RuntimeError(f'benchmark timeout; see {err}')
        reader.join()
    duration=time.monotonic()-start
    if code:raise RuntimeError(f'benchmark exited {code}; {err.read_text()[-4000:]}')
    text=stats.read_text();rss=re.search(r'(\d+)\s+maximum resident set size',text)
    if rss:peak=int(rss.group(1))/(1024**2 if sys.platform=='darwin' else 1024)
    else:
        rss=re.search(r'Maximum resident set size \(kbytes\):\s*(\d+)',text)
        if not rss:raise RuntimeError('maximum RSS unavailable: '+text)
        peak=int(rss.group(1))/1024
    phases={};codec=[];engine='';doc={};ready=None;last_marker=0
    for elapsed,line in marks:
        if line=='READY':ready=elapsed
        elif line.startswith('STORE '):engine=line[6:]
        elif line.startswith('PHASE '):_,name,seconds=line.split();phases[name]=float(seconds);last_marker=elapsed
        elif line.startswith('CODEC '):codec=list(map(int,line.split()[1:]));last_marker=elapsed
        elif line.startswith('DOCUMENT '):
            _,seconds,count,events,encoded,decoded=line.split()
            doc={'seconds':float(seconds),'documents':int(count),'events':int(events),'encoded':int(encoded),'decoded':int(decoded)};last_marker=elapsed
    result=build_runner.canonical(json.loads(out.read_text()))
    assert engine and ready is not None and codec
    return result,{'wallSeconds':duration,'inputOpenReplaySeconds':ready,'responseCloseExitSeconds':max(0,duration-last_marker),
                   'phases':phases,'peakRssMiB':peak,'sqliteVersion':engine,'codec':dict(zip(['encoded','decoded','encodedBytes','decodedBytes'],codec)),
                   'document':doc,'inputBytes':inp.stat().st_size,'outputBytes':out.stat().st_size}


def measure(language,command,students,store,root,timeout):
    payload,expected=generate(students);docs=max(1,students//20)
    folder=root/f'{language}-{store}-{students}';folder.mkdir()
    first=folder/'first';first.mkdir()
    result,metrics=invoke(command,payload,first,store,docs,timeout,students)
    digest=validate(result,expected)
    assert metrics['codec']['encoded']>=expected['events']
    assert metrics['document']['encoded']==16*docs and metrics['document']['decoded']==168*docs
    assert 'tagged_point' in metrics['phases'] and 'projection_point_json' in metrics['phases']
    build={'csharp':'Release Native AOT' if any('csharp-aot' in c for c in command) else 'Release JIT + Dynamic PGO','fsharp':'Release JIT + Dynamic PGO','rust':'Release optimized','typescript':'Node + Zod','java':'JVM defaults','go':'go build','swift':'swiftc -O'}[language]
    metrics.update(language=language,build=build,store=store,students=students,expected=expected,stateSha256=digest)
    if store=='sqlite':
        metrics['databaseValidation']=database_check(folder/'school.db',expected,docs)
        metrics['databaseBytes']=sum(p.stat().st_size for p in folder.glob('school.db*'))
        restart=folder/'restart';restart.mkdir()
        replay,replay_metrics=invoke(command,{'history':[],'operations':[{'$type':'ListAll'}]},restart,store,0,timeout)
        assert validate(replay,expected,restart=True)==digest
        metrics['restart']=replay_metrics
    # Native output is intentionally materialized once. Exclude it from retained
    # observations; hashes/counts and raw timings keep the report reviewable.
    for p in folder.rglob('*.json'):p.unlink()
    return metrics


def report(data,path):
    path.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n')
    lines=['# Level 3 ES benchmark observations','',f"Measured: {data['measuredAt']}",'',
           f"Platform: `{data['platform']}`. All measured sequentially on this machine. Students: {data['students']:,}.",'',
           '| implementation | build | store | wall s | operations s | read/rebuild/list s | 4KiB revisions s | restart wall s | peak RSS MiB | DB MiB | SQLite |',
           '| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |']
    for r in data['results']:
        p=r['phases'];restart=r.get('restart',{}).get('wallSeconds');db=r.get('databaseBytes')
        lines.append(f"| [{'ts-zod' if r['language']=='typescript' else r['language']}](../../{r['language']}) | {r['build']} | {r['store']} | {r['wallSeconds']:.3f} | {p['operations']:.3f} | {p['read_rebuild_snapshot']:.3f} | {r['document']['seconds']:.3f} | {restart:.3f} | {r['peakRssMiB']:.1f} | {db/1048576:.1f} | {r['sqliteVersion']} |" if restart is not None else f"| [{'ts-zod' if r['language']=='typescript' else r['language']}](../../{r['language']}) | {r['build']} | {r['store']} | {r['wallSeconds']:.3f} | {p['operations']:.3f} | {p['read_rebuild_snapshot']:.3f} | {r['document']['seconds']:.3f} | — | {r['peakRssMiB']:.1f} | — | — |")
    lines+=['','| implementation | store | tagged reads 1000 s | projection + JSON 1000 s | input/open/replay s | response/close/exit s |',
            '| --- | --- | ---: | ---: | ---: | ---: |']
    for r in data['results']:
        p=r['phases']
        lines.append(f"| {'ts-zod' if r['language']=='typescript' else r['language']} | {r['store']} | {p.get('tagged_point',0):.6f} | {p.get('projection_point_json',0):.6f} | {r['inputOpenReplaySeconds']:.3f} | {r['responseCloseExitSeconds']:.3f} |")
    lines+=['','RSS includes the runtime, parsed input, audit traces, event reads, and live/rebuilt projections. It is not the incremental projection size.',
            'Operations include commands, validation/rejections, conflicts/rejudgment, full list queries and duplicate delivery. JSON summary/list encoding, replay verification and process close/exit are included in wall time.',
            'SQLite versions differ by driver. This compares complete implementations, not isolated language speed. No synthetic delays are used.','',f"[Raw data]({path.name})",'']
    path.with_suffix('.md').write_text('\n'.join(lines))


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--language',default='all');p.add_argument('--students',type=int)
    p.add_argument('--target-seconds',type=float,default=90);p.add_argument('--quick',action='store_true')
    p.add_argument('--store',choices=['sqlite','memory-json','both'],default='sqlite')
    p.add_argument('--timeout',type=int,default=1800);p.add_argument('--output',type=Path,default=HERE/'observed/latest.json')
    p.add_argument('--keep-databases',action='store_true');p.add_argument('--no-build',action='store_true')
    p.add_argument('--csharp-native-aot',action='store_true')
    a=p.parse_args();languages=build_runner.LANGUAGES if a.language=='all' else a.language.split(',')
    if any(l not in build_runner.LANGUAGES for l in languages):p.error('unknown language')
    if a.students is not None and (a.students<10 or a.students%10):p.error('--students must be a positive multiple of ten')
    if a.target_seconds<=0 or a.timeout<=0:p.error('target and timeout must be positive')
    commands={}
    for l in languages:
        print('Building '+l,flush=True)
        cache=LEVEL/'.build'/f"benchmark-command-{l}{'-aot' if a.csharp_native_aot and l=='csharp' else ''}.json"
        commands[l]=json.loads(cache.read_text()) if a.no_build else build_runner.build(l,a.csharp_native_aot and l=='csharp')
        cache.write_text(json.dumps(commands[l]))
    versions={}
    for name,command in {'dotnet':['dotnet','--info'],'node':['node','--version'],'rust':['rustc','--version'],'java':['java','-version'],'go':['go','version'],'swift':['swift','--version']}.items():
        try:
            v=subprocess.run(command,capture_output=True,text=True);versions[name]=(v.stdout+v.stderr).strip()
        except OSError as error:versions[name]='unavailable: '+str(error)
    manifest={str(f.relative_to(LEVEL)):hashlib.sha256(f.read_bytes()).hexdigest() for f in LEVEL.rglob('*') if f.is_file() and not any(part in f.parts for part in ['.build','bin','obj','node_modules','target','observed','__pycache__']) and f.suffix in ['.cs','.fs','.rs','.ts','.go','.swift','.java','.py','.json','.toml','.lock','.sql','.csproj','.fsproj','.mod','.sum']}
    root=Path(tempfile.mkdtemp(prefix='level3-es-'));data={'formatVersion':1,'measuredAt':datetime.datetime.now(datetime.timezone.utc).isoformat(),'platform':platform.platform(),'machine':platform.machine(),'processor':platform.processor(),'commands':commands,'runtimeVersions':versions,'sourceManifest':manifest,'csharpNativeAot':a.csharp_native_aot,'sourceCommit':subprocess.run(['git','rev-parse','HEAD'],cwd=LEVEL,capture_output=True,text=True).stdout.strip(),'results':[]}
    import xml.etree.ElementTree as ET
    driver=lambda project:next(p.attrib['Version'] for p in ET.parse(project).iter('PackageReference') if p.attrib.get('Include')=='Microsoft.Data.Sqlite')
    data['drivers']={'csharp':'Microsoft.Data.Sqlite '+driver(LEVEL/'csharp/Decider.csproj'),'fsharp':'Microsoft.Data.Sqlite '+driver(LEVEL/'fsharp/Decider.fsproj'),
                     'rust':'rusqlite '+re.search(r'rusqlite\s*=\s*\{\s*version\s*=\s*"=?([^"\s]+)',(LEVEL/'rust/Cargo.toml').read_text())[1]+' / bundled',
                     'typescript':'node:sqlite / built-in','java':[Path(c).name for c in os.pathsep.join(commands.get('java',[])).split(os.pathsep) if 'sqlite-jdbc-' in c and c.endswith('.jar')],
                     'go':re.search(r'github.com/mattn/go-sqlite3\s+(\S+)',(LEVEL/'go/go.mod').read_text())[0]+' / CGO','swift':'system SQLite3'}
    if sys.platform=='darwin':
        data['hardware']={name:subprocess.run(['sysctl','-n',key],capture_output=True,text=True).stdout.strip() for name,key in [('cpu','machdep.cpu.brand_string'),('memoryBytes','hw.memsize')]}
    else:
        data['hardware']={'memoryBytes':os.sysconf('SC_PAGE_SIZE')*os.sysconf('SC_PHYS_PAGES')}
    try:
        n=100 if a.quick else a.students
        if n is None:
            pilot=root/'pilot';pilot.mkdir();pilots=[]
            for l in languages:
                print('Calibrating '+l,flush=True);m=measure(l,commands[l],2000,'sqlite',pilot,a.timeout);pilots.append(m)
            # One common count based on the median implementation, never a
            # different count for each language. Slow implementations may exceed
            # the target; retaining equal work takes precedence over equal time.
            def rate(m):return (m['phases']['operations']+m['document']['seconds'])/m['students']
            ordered=sorted(pilots,key=rate);median=ordered[len(ordered)//2]
            n=max(10,min(100000,round(a.target_seconds/rate(median)/10)*10))
            calibration={'targetSeconds':a.target_seconds,'pilotStudents':2000,'results':pilots,'initialStudents':n,'medianImplementation':median['language']}
            # A small DB fits different cache/IO conditions. Refine on a larger
            # database before selecting the one common count for every runtime.
            refinement_students=min(20000,max(2000,round(n/2/10)*10))
            if refinement_students>2000:
                refinement=root/'refinement';refinement.mkdir()
                language=median['language'];print(f'Refining calibration: {language}, {refinement_students:,} students',flush=True)
                measured=measure(language,commands[language],refinement_students,'sqlite',refinement,a.timeout)
                calibration['refinement']=measured
                n=max(10,min(100000,round(a.target_seconds/rate(measured)/10)*10))
            calibration['selectedStudents']=n;data['calibration']=calibration
        data['students']=n;data['workload']=json.loads((HERE/'workload.json').read_text())
        data['workloadSha256']=hashlib.sha256((HERE/'workload.py').read_bytes()).hexdigest()
        stores=['sqlite','memory-json'] if a.store=='both' else [a.store]
        for store in stores:
            hashes=set()
            for l in languages:
                print(f'Measuring {l} {store}: {n:,} students',flush=True)
                m=measure(l,commands[l],n,store,root,a.timeout);data['results'].append(m);hashes.add(m['stateSha256'])
                print(f"  {m['wallSeconds']:.3f}s / RSS {m['peakRssMiB']:.1f} MiB / PASS",flush=True)
                a.output.parent.mkdir(parents=True,exist_ok=True);report(data,a.output)
            assert len(hashes)==1,'cross-language final states differ'
        assert len({m['stateSha256'] for m in data['results']})==1,'store variants differ'
        print('Results: '+str(a.output),flush=True)
    finally:
        if a.keep_databases:print('Databases: '+str(root))
        else:shutil.rmtree(root)

if __name__=='__main__':
    try:main()
    except subprocess.CalledProcessError as e:
        print(e.stdout or '',file=sys.stderr);print(e.stderr or '',file=sys.stderr);raise SystemExit(e.returncode or 1)
