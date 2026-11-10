#!/usr/bin/env python3
"""Build and compare large, single-pass 100-case JSON workloads across ten implementations."""
from pathlib import Path
import argparse
import datetime
import hashlib
import json
import math
import os
import platform
import random
import re
import shutil
import statistics
import subprocess
import sys
import time
import signal
import tempfile
import threading

HERE=Path(__file__).resolve().parent
REPO=HERE.parents[1]
sys.path.insert(0,str(REPO/'scripts'))
from run_payment_java import jackson_classpath
from generate import generate
NAMES=['csharp-abstract','csharp-closed-explicit','csharp-closed-inferred','csharp-union-custom','csharp-union-generated','rust','typescript','java','go','swift']

def run(command, *, cwd=None, timeout=1800):
    return subprocess.run([str(c) for c in command],cwd=cwd or REPO,text=True,
        stdout=subprocess.PIPE,stderr=subprocess.PIPE,check=True,timeout=timeout)

def checksum(roots):
    total=0
    for seed in range(roots):
        for depth in range(3):
            i=seed+37*depth
            total+=i%100+i+len('item-'+str(i))+4*i+6+len('group')+len('g'+str(i%7))+len('note')+len('bench')
    return total

def build(names,build_dir,rid):
    source=build_dir/'sources'
    staging=build_dir/'generated-next'
    if staging.exists():shutil.rmtree(staging)
    generate(staging,REPO)
    for path in staging.rglob('*'):
        if path.is_file():
            target=source/path.relative_to(staging);target.parent.mkdir(parents=True,exist_ok=True)
            if not target.exists() or target.read_bytes()!=path.read_bytes():shutil.copy2(path,target)
    shutil.rmtree(staging)
    commands={}
    for name in names:
        print('Building '+name,flush=True)
        out=build_dir/'bin'/name;out.mkdir(parents=True,exist_ok=True)
        if name.startswith('csharp-'):
            project=source/name/'Benchmark.csproj'
            result=run(['dotnet','publish',project,'-c','Release','-r',rid,'-o',out])
            (out/'build.log').write_text(result.stdout+result.stderr)
            commands[name]=[str(out/'Benchmark')]
        elif name=='rust':
            result=run(['cargo','build','--release','--manifest-path',source/'rust/Cargo.toml'])
            (out/'build.log').write_text(result.stdout+result.stderr)
            commands[name]=[str(source/'rust/target/release/union100')]
        elif name=='typescript':commands[name]=['node','--max-old-space-size=6144',str(source/'benchmark.ts')]
        elif name=='java':
            jars=build_dir/'jars';jars.mkdir(exist_ok=True)
            cp=jackson_classpath(jars)
            run(['javac','--class-path',cp,'-d',out,source/'java/Benchmark.java'])
            commands[name]=['java','-Xmx6g','--class-path',str(out)+os.pathsep+cp,'Benchmark']
        elif name=='go':
            run(['go','build','-o',out/'union100',source/'benchmark.go']);commands[name]=[str(out/'union100')]
        elif name=='swift':
            run(['swiftc','-O',source/'Benchmark.swift','-o',out/'union100']);commands[name]=[str(out/'union100')]
    return commands

def measure(name,commands,roots,samples,timeout,progress_dir,expected_sink=None,invocation_args=None,workload_label=None):
    description=workload_label or f'{roots:,} roots / {roots*3:,} union nodes x {samples} large batches'
    print(f'Running {name}: {description}',flush=True)
    started=time.monotonic()
    command=commands[name]+([str(v) for v in invocation_args] if invocation_args is not None else [str(roots),str(samples)])
    if sys.platform=='darwin':
        command=['/usr/bin/time','-l']+command
    elif sys.platform.startswith('linux'):
        command=['/usr/bin/time','-v']+command
    else:raise RuntimeError('Peak RSS measurement currently supports macOS and Linux only.')
    lines=[]
    with tempfile.TemporaryFile(mode='w+t') as stderr:
        process=subprocess.Popen(command,cwd=REPO,text=True,stdout=subprocess.PIPE,stderr=stderr,start_new_session=True)
        def collect():
            for line in process.stdout:
                lines.append((time.monotonic(),line))
        reader=threading.Thread(target=collect,daemon=True);reader.start()
        status={}
        def wait_for_exit():
            status['code']=process.wait()
            status['exited']=time.monotonic()
        waiter=threading.Thread(target=wait_for_exit,daemon=True);waiter.start()
        try:waiter.join(timeout)
        except BaseException:
            if process.poll() is None:os.killpg(process.pid,signal.SIGKILL)
            waiter.join();reader.join();process.stdout.close()
            raise
        if waiter.is_alive():
            os.killpg(process.pid,signal.SIGKILL);waiter.join();reader.join();process.stdout.close()
            raise subprocess.TimeoutExpired(command,timeout)
        code=status['code'];exited=status['exited'];reader.join();process.stdout.close();stderr.seek(0);error_output=stderr.read()
    if code:
        raise subprocess.CalledProcessError(code,command,''.join(line for _,line in lines),error_output)
    ready=prepared=finished=None;parsed=None
    for received,line in lines:
        event=json.loads(line)
        if event.get('event')=='ready':ready=received
        elif event.get('event')=='prepared':prepared=received
        else:finished,parsed=received,event
    if parsed is None or ready is None or prepared is None or finished is None:
        raise RuntimeError(name+': lifecycle markers/result missing')
    parsed['startupSeconds']=max(0,ready-started)
    parsed['preparationSeconds']=max(0,prepared-ready)
    parsed['workAndValidationSeconds']=max(0,finished-prepared)
    parsed['exitSeconds']=max(0,exited-finished)
    parsed['wallSeconds']=exited-started
    if parsed['roots']!=roots or parsed['samples']!=samples or parsed['sink']!=(checksum(roots)*samples if expected_sink is None else expected_sink) or len(parsed['measurements'])!=samples:
        raise RuntimeError(name+': checksum/count validation failed')
    for entry in parsed['measurements']:
        if entry['totalSeconds']<=0 or entry['bytes']<=0:raise RuntimeError(name+': invalid timing')
    pattern=(r'(\d+)\s+maximum resident set size' if sys.platform=='darwin' else r'Maximum resident set size \(kbytes\):\s*(\d+)')
    match=re.search(pattern,error_output)
    if not match:raise RuntimeError(name+': OS peak RSS was not reported')
    parsed['peakRssBytes']=int(match.group(1))*(1 if sys.platform=='darwin' else 1024)
    progress_dir.mkdir(parents=True,exist_ok=True)
    (progress_dir/f'{name}-{roots}-{samples}.json').write_text(json.dumps(parsed,indent=2)+'\n')
    print(f'Completed {name}: median {statistics.median(e["totalSeconds"] for e in parsed["measurements"]):.3f}s/batch; peak RSS {parsed["peakRssBytes"]/1048576:.1f} MiB',flush=True)
    return parsed

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--implementations',default=','.join(NAMES),help='comma-separated names; default: all ten')
    p.add_argument('--roots',type=int,help='fixed large batch size; skips calibration')
    p.add_argument('--samples',type=int,default=5,help='large single-pass batches, not tiny loop repetitions')
    p.add_argument('--target-seconds',type=float,default=10,help='target fastest aggregate measured time across samples')
    p.add_argument('--min-batch-seconds',type=float,default=1,help='target fastest individual batch duration')
    p.add_argument('--max-roots',type=int,default=1000000,help='memory safety ceiling; fail rather than silently under-run target')
    p.add_argument('--quick',action='store_true',help='correctness smoke run: 100 roots, one batch, no calibration')
    p.add_argument('--rid',default=(('osx' if sys.platform=='darwin' else 'linux')+'-'+('arm64' if platform.machine() in ['arm64','aarch64'] else 'x64')))
    p.add_argument('--build-dir',type=Path,default=HERE/'.build')
    p.add_argument('--output',type=Path,default=HERE/'results/latest.json')
    p.add_argument('--skip-build',action='store_true',help='reuse previously built binaries only when source fingerprints match')
    p.add_argument('--timeout',type=int,default=1800,help='timeout per implementation invocation, seconds')
    args=p.parse_args()
    names=args.implementations.split(',')
    if len(set(names))!=len(names) or any(n not in NAMES for n in names):p.error('unknown or duplicate implementation')
    if args.quick:args.roots,args.samples=100,1
    if args.samples<1 or args.target_seconds<=0 or args.min_batch_seconds<=0 or args.max_roots<100:p.error('counts and durations must be positive')
    if args.roots is not None and not 100<=args.roots<=args.max_roots:p.error('roots must be between 100 and max-roots')
    build_dir=args.build_dir.resolve();build_dir.mkdir(parents=True,exist_ok=True)
    fingerprint=hashlib.sha256()
    for path in sorted([HERE/'generate.py',HERE/'run.py',HERE/'Cargo.lock',REPO/'global.json',*list((HERE/'templates').iterdir()),*list((REPO/'src/PlaygroundTaggedJson.Generator').glob('*.cs')),REPO/'src/PlaygroundTaggedJson.Generator/PlaygroundTaggedJson.Generator.csproj']):
        fingerprint.update(str(path.relative_to(REPO)).encode());fingerprint.update(path.read_bytes())
    digest=fingerprint.hexdigest()
    manifest=build_dir/'commands.json'
    if args.skip_build:
        cached=json.loads(manifest.read_text())
        if cached['fingerprint']!=digest or cached['rid']!=args.rid or any(n not in cached['commands'] for n in names):
            raise RuntimeError('Build cache differs from sources, RID or requested implementations; run without --skip-build.')
        commands={name:cached['commands'][name] for name in names}
    else:
        commands=build(names,build_dir,args.rid)
        manifest.write_text(json.dumps({'fingerprint':digest,'rid':args.rid,'commands':commands},indent=2))
    progress_dir=args.output.parent/(args.output.stem+'.progress')
    calibration=[]
    roots=args.roots
    if roots is None:
        probe=1000
        target_batch=max(args.min_batch_seconds,args.target_seconds/args.samples)
        # Two size-based probes reduce cold/JIT and very-small-batch extrapolation errors.
        for step in range(2):
            results={name:measure(name,commands,probe,2,args.timeout,progress_dir) for name in names}
            calibration.append({'roots':probe,'results':results})
            fastest=min(statistics.median(e['totalSeconds'] for e in r['measurements']) for r in results.values())
            candidate=int(math.ceil(probe*target_batch/fastest*1.05/100)*100)
            if candidate>args.max_roots and step==1:
                raise RuntimeError(f'Target needs about {candidate:,} roots, above --max-roots {args.max_roots:,}; raise the ceiling only if memory permits, or lower target durations.')
            roots=max(100,candidate)
            if step==0:probe=min(roots,20000,args.max_roots)
        print(f'Calibration chose {roots:,} roots per batch; target fastest batch ~{target_batch:.2f}s.',flush=True)
    order=list(names);random.Random(100).shuffle(order)
    results={name:measure(name,commands,roots,args.samples,args.timeout,progress_dir) for name in order}
    summary=[]
    for name in names:
        entries=results[name]['measurements']
        summary.append({'implementation':name,'medianSeconds':statistics.median(e['totalSeconds'] for e in entries),
            'minSeconds':min(e['totalSeconds'] for e in entries),'maxSeconds':max(e['totalSeconds'] for e in entries),
            'aggregateSeconds':sum(e['totalSeconds'] for e in entries),
            'encodeMedianSeconds':statistics.median(e['encodeSeconds'] for e in entries),
            'decodeMedianSeconds':statistics.median(e['decodeSeconds'] for e in entries),
            'switchMedianSeconds':statistics.median(e['switchSeconds'] for e in entries),'bytes':entries[0]['bytes'],'peakRssMiB':results[name]['peakRssBytes']/1048576,'startupSeconds':results[name]['startupSeconds'],'preparationSeconds':results[name]['preparationSeconds'],'exitSeconds':results[name]['exitSeconds'],'wallSeconds':results[name]['wallSeconds']})
    summary.sort(key=lambda r:r['medianSeconds'])
    source_hash=hashlib.sha256()
    for path in sorted((HERE/'templates').iterdir()):source_hash.update(path.read_bytes())
    versions={}
    for name,command in {'dotnet':['dotnet','--version'],'rust':['rustc','--version'],'node':['node','--version'],'java':['java','-version'],'go':['go','version'],'swift':['swift','--version']}.items():
        if name=='dotnet' and not any(n.startswith('csharp-') for n in names):continue
        if name!='dotnet' and not (name in names or name=='node' and 'typescript' in names):continue
        version=run(command);versions[name]=(version.stdout+version.stderr).strip()
    report={'timestampUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'platform':platform.platform(),
        'machine':platform.machine(),'rid':args.rid,'toolchains':versions,'buildFingerprint':digest,'roots':roots,'nodesPerBatch':roots*3,'samples':args.samples,
        'targetAggregateSeconds':args.target_seconds,'minBatchSeconds':args.min_batch_seconds,
        'templateSha256':source_hash.hexdigest(),'commands':commands,'calibration':calibration,'results':results,'summary':summary,
        'lifecycleMetric':'Parent-observed launch to ready, ready to prepared, result received to process exit. Includes OS time-wrapper and pipe scheduling overhead.',
        'memoryMetric':'OS process peak resident set size; includes setup, warm-up and correctness checks, not just measured phases.',
        'notes':['Startup, compilation, data generation, warm-up and correctness checks are excluded from measured phases.',
                 'Each sample encodes/decodes the entire large batch once and traverses all decoded nodes once.',
                 'TypeScript includes recursive runtime validation; Swift uses synthesized enum envelopes and sorted keys; C# unions wrap case data. Wire bytes differ.',
                 'This measures these implementations and serializers, not language speed or isolated union overhead. GC, JIT state and host load affect results.']}
    args.output.parent.mkdir(parents=True,exist_ok=True);args.output.write_text(json.dumps(report,indent=2)+'\n')
    md=['| implementation | median batch s | encode s | decode s | switch s | aggregate s | JSON MiB | peak RSS MiB |','|---|---:|---:|---:|---:|---:|---:|---:|']
    for r in summary:
        md.append(f"| {r['implementation']} | {r['medianSeconds']:.4f} | {r['encodeMedianSeconds']:.4f} | {r['decodeMedianSeconds']:.4f} | {r['switchMedianSeconds']:.4f} | {r['aggregateSeconds']:.2f} | {r['bytes']/1048576:.2f} | {r['peakRssMiB']:.1f} |")
    md+=['','| implementation | startup s | preparation s | exit s | total wall s |','|---|---:|---:|---:|---:|']
    for r in summary:
        md.append(f"| {r['implementation']} | {r['startupSeconds']:.4f} | {r['preparationSeconds']:.3f} | {r['exitSeconds']:.4f} | {r['wallSeconds']:.2f} |")
    print('\n'.join(md));args.output.with_suffix('.md').write_text('\n'.join(md)+'\n')
    fastest=min(r['medianSeconds'] for r in summary)
    if args.roots is None and fastest<args.min_batch_seconds:
        print('NOTE: actual fastest batch fell below requested duration; increase --roots for a repeat using the recorded result.',file=sys.stderr)
    print(f'Fastest median batch: {fastest:.3f}s; fastest aggregate measured time: {min(r["aggregateSeconds"] for r in summary):.3f}s',flush=True)
    print('Saved '+str(args.output),flush=True)

if __name__=='__main__':
    try:main()
    except KeyboardInterrupt:
        print('Interrupted; completed measurements remain in the progress directory.',file=sys.stderr);raise SystemExit(130)
    except (RuntimeError,FileNotFoundError,subprocess.TimeoutExpired) as e:
        print(str(e),file=sys.stderr);raise SystemExit(1)
    except subprocess.CalledProcessError as e:
        print(e.stdout or '',file=sys.stderr);print(e.stderr or '',file=sys.stderr);raise SystemExit(e.returncode)
