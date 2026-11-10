#!/usr/bin/env python3
"""Replay 100 open event types into ten projections; compare growth algorithms and native/WASI."""
from pathlib import Path
import argparse
import datetime
import functools
import hashlib
import importlib.util
import json
import math
import os
import platform
import shutil
import statistics
import subprocess
import sys

HERE=Path(__file__).resolve().parent;REPO=HERE.parents[1]
spec=importlib.util.spec_from_file_location('projection_generator',HERE/'generate.py');generator=importlib.util.module_from_spec(spec);spec.loader.exec_module(generator)
sys.path.insert(0,str(HERE.parent/'union100'))
spec=importlib.util.spec_from_file_location('union_harness',HERE.parent/'union100/run.py');harness=importlib.util.module_from_spec(spec);spec.loader.exec_module(harness)
sys.path.insert(0,str(HERE.parent))
from wasi_support import build_csharp
LANGUAGES=['csharp','rust','typescript','typescript-zod','java','go','swift']
MODES=['indexed-append','indexed-copy','linear-append']

@functools.lru_cache(maxsize=64)
def reference(events,aggregates,routing):
    states={}
    for i in range(events):
        domain=(i%100)//10;kind=i%10;key=(domain,(i//100)%aggregates);v=i+1
        s=states.setdefault(key,[0,0,0,0,0,False,False,False,0,0,{}])
        # balance, counter, seen, revision, name length, exists, flag, archived, item sum/count, tags
        if kind==0:s[5]=True
        elif kind==1:s[4]=len('name-'+str(v))
        elif kind==2:s[0]+=v
        elif kind==3:s[8]+=v;s[9]+=1
        elif kind==4:s[6]=not s[6]
        elif kind==5:s[1]+=v
        elif kind==6:s[10][v%16]=v
        elif kind==7:s[2]=v
        elif kind==8:s[3]=v
        elif kind==9:s[7]=True
    return {'sink':sum(domain+id+sum(s[:10])+sum(k+v for k,v in s[10].items()) for (domain,id),s in states.items()),
            'stateCount':len(states),'retainedItems':sum(s[9] for s in states.values()),
            'maxHistoryLength':max(s[9] for s in states.values()),'ignored':(events*9 if routing=='fanout' else 0)+10}

def build(languages,engines,build_dir,rid,swift_sdk,swift_toolchain,wasi_dotnet,wasi_sdk):
    sources=build_dir/'sources';generator.generate(sources)
    commands={};unavailable={}
    for language in languages:
        out=build_dir/'bin'/language;out.mkdir(parents=True,exist_ok=True)
        print('Building '+language,flush=True)
        if language=='csharp':
            if 'native' in engines:
                harness.run(['dotnet','publish',sources/'Projection.csproj','-c','Release','-r',rid,'-o',out]+(['-p:OptimizationPreference=Speed','-p:IlcInstructionSet=native'] if 'wasi' in engines else []));commands['csharp-native']=[str(out/'Projection')]
            if 'wasi' in engines:
                try:
                    commands['csharp-wasi']=build_csharp(sources/'Projection.csproj',build_dir/'wasi-csharp',wasi_dotnet,wasi_sdk,shutil.which('wasmtime') or str(Path.home()/'.wasmtime/bin/wasmtime'))
                except (RuntimeError,subprocess.SubprocessError) as e:unavailable['csharp-wasi']=str(e)+'\n'+str(getattr(e,'stdout',''))+'\n'+str(getattr(e,'stderr',''))
        elif language=='rust':
            if 'native' in engines:
                harness.run((['rustup','run','stable','rustc'] if shutil.which('rustup') else ['rustc'])+['--edition=2024','-O']+(['-C','lto=fat','-C','codegen-units=1','-C','target-cpu=native'] if 'wasi' in engines else [])+[sources/'projection.rs','-o',out/'projection']);commands['rust-native']=[str(out/'projection')]
            if 'wasi' in engines:
                try:
                    harness.run((['rustup','run','stable','rustc'] if shutil.which('rustup') else ['rustc'])+['--edition=2024','-O','-C','lto=fat','-C','codegen-units=1','-C','target-feature=+simd128','--target','wasm32-wasip1',sources/'projection.rs','-o',out/'projection.wasm'])
                    commands['rust-wasi']=[shutil.which('wasmtime') or str(Path.home()/'.wasmtime/bin/wasmtime'),'run','-O','opt-level=2',str(out/'projection.wasm')]
                except subprocess.CalledProcessError as e:unavailable['rust-wasi']=e.stderr
        elif language in ['typescript','typescript-zod']:
            if 'native' in engines:
                suffix='-zod' if language=='typescript-zod' else ''
                if suffix:
                    for manifest_name in ['package.json','package-lock.json']:
                        shutil.copy2(REPO/'samples/file-based'/manifest_name,sources/manifest_name)
                    harness.run(['npm','ci','--omit=dev','--ignore-scripts','--no-audit','--no-fund','--prefix',sources])
                commands[language+'-native']=['node','--max-old-space-size=6144',str(sources/('projection'+suffix+'.ts'))]
            if 'wasi' in engines:unavailable[language+'-wasi']='Node TypeScript/JavaScript is not a WASI module. AssemblyScript or an embedded JS engine would be a different implementation.'
        elif language=='java':
            if 'native' in engines:
                harness.run(['javac','-d',out,sources/'Projection.java']);commands['java-native']=['java','-Xmx6g','--class-path',str(out),'Projection']
            if 'wasi' in engines:unavailable['java-wasi']='This Java/JVM implementation is not a WASI module; a separate Java-to-Wasm backend is not configured.'
        elif language=='go':
            if 'native' in engines:
                harness.run(['go','build','-o',out/'projection',sources/'projection.go']);commands['go-native']=[str(out/'projection')]
            if 'wasi' in engines:
                env={**os.environ,'GOOS':'wasip1','GOARCH':'wasm'}
                result=subprocess.run(['go','build','-o',str(out/'projection.wasm'),str(sources/'projection.go')],text=True,capture_output=True,env=env)
                if result.returncode:unavailable['go-wasi']=result.stderr
                else:commands['go-wasi']=[shutil.which('wasmtime') or str(Path.home()/'.wasmtime/bin/wasmtime'),'run','-O','opt-level=2',str(out/'projection.wasm')]
        elif language=='swift':
            swift='swift';swiftc='swiftc'
            if 'wasi' in engines and shutil.which('swiftly'):
                try:
                    swift=harness.run(['swiftly','run','/usr/bin/which','swift','+'+swift_toolchain]).stdout.strip().splitlines()[-1]
                    swiftc=str(Path(swift).with_name('swiftc'))
                except subprocess.CalledProcessError as e:unavailable['swift-wasi']=e.stderr
            if 'native' in engines:
                harness.run([swiftc,'-O',sources/'Projection.swift','-o',out/'projection']);commands['swift-native']=[str(out/'projection')]
            if 'wasi' in engines:
                package=build_dir/'swift-package';(package/'Sources/Projection').mkdir(parents=True,exist_ok=True)
                shutil.copy2(sources/'Projection.swift',package/'Sources/Projection/main.swift')
                (package/'Package.swift').write_text('// swift-tools-version: 6.2\nimport PackageDescription\nlet package=Package(name:"Projection",products:[.executable(name:"Projection",targets:["Projection"])],targets:[.executableTarget(name:"Projection")])\n')
                try:
                    harness.run([swift,'build','-c','release','--package-path',package,'--swift-sdk',swift_sdk])
                    binary=next((package/'.build').rglob('Projection.wasm'))
                    commands['swift-wasi']=[shutil.which('wasmtime') or str(Path.home()/'.wasmtime/bin/wasmtime'),'run',str(binary)]
                except (subprocess.CalledProcessError,StopIteration) as e:unavailable['swift-wasi']=getattr(e,'stderr',str(e))
    return commands,unavailable

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--languages',default=','.join(LANGUAGES));p.add_argument('--engines',default='native',help='native,wasi or both comma separated')
    p.add_argument('--sizes',default='1000,10000,100000,1000000,10000000');p.add_argument('--samples',type=int,default=3)
    p.add_argument('--modes',default=','.join(MODES));p.add_argument('--profiles',default='dense,wide')
    p.add_argument('--routing',choices=['fanout','domain'],default='fanout');p.add_argument('--timeout',type=float,default=30)
    p.add_argument('--max-projection-seconds',type=float,default=20);p.add_argument('--quick',action='store_true')
    p.add_argument('--swift-sdk',default='swift-6.3.1-RELEASE_wasm')
    p.add_argument('--swift-toolchain',default='6.3.1')
    p.add_argument('--wasi-dotnet',type=Path,help='SDK with wasi-experimental; defaults to build-dir/dotnet/dotnet')
    p.add_argument('--wasi-sdk',type=Path,help='official wasi-sdk 33 directory')
    p.add_argument('--build-dir',type=Path,default=HERE/'.build');p.add_argument('--output',type=Path,default=HERE/'results/latest.json')
    p.add_argument('--skip-build',action='store_true')
    p.add_argument('--rid',default=(('osx' if sys.platform=='darwin' else 'linux')+'-'+('arm64' if platform.machine() in ['arm64','aarch64'] else 'x64')))
    args=p.parse_args();languages=args.languages.split(',');engines=args.engines.split(',');modes=args.modes.split(',');profiles=args.profiles.split(',');sizes=sorted(set(map(int,args.sizes.split(','))))
    if args.quick:sizes,args.samples=[1000],1
    if any(l not in LANGUAGES for l in languages)or any(e not in ['native','wasi']for e in engines)or any(m not in MODES for m in modes)or any(f not in ['dense','wide']for f in profiles)or min(sizes)<100 or args.samples<1 or args.timeout<=0 or args.max_projection_seconds<=0:p.error('Invalid implementation, engine, mode, profile or count')
    args.build_dir=args.build_dir.resolve();args.build_dir.mkdir(parents=True,exist_ok=True)
    fingerprint=hashlib.sha256()
    for path in sorted([HERE/'generate.py',HERE/'run.py',HERE.parent/'union100/run.py',HERE.parent/'wasi_support.py',REPO/'samples/file-based/package.json',REPO/'samples/file-based/package-lock.json',REPO/'global.json',*list((HERE/'templates').iterdir())]):fingerprint.update(path.read_bytes())
    wasi_dotnet=(args.wasi_dotnet or args.build_dir/'dotnet/dotnet').resolve()
    wasi_sdk=(args.wasi_sdk or args.build_dir/'tools/wasi-sdk-33.0-arm64-macos').resolve()
    key={'source':fingerprint.hexdigest(),'languages':languages,'engines':engines,'rid':args.rid,'swiftSdk':args.swift_sdk,'swiftToolchain':args.swift_toolchain,'wasiDotnet':str(wasi_dotnet),'wasiSdk':str(wasi_sdk)}
    cache=args.build_dir/'commands.json'
    if args.skip_build:
        saved=json.loads(cache.read_text())
        if saved['key']!=key:raise RuntimeError('Build configuration changed; run without --skip-build.')
        commands,unavailable=saved['commands'],saved['unavailable']
    else:
        commands,unavailable=build(languages,engines,args.build_dir,args.rid,args.swift_sdk,args.swift_toolchain,wasi_dotnet,wasi_sdk)
        cache.write_text(json.dumps({'key':key,'commands':commands,'unavailable':unavailable},indent=2))
    versions={}
    checks={'csharp':['dotnet','--version'],'rust':(['rustup','run','stable','rustc','--version'] if shutil.which('rustup') else ['rustc','--version']),'typescript':['node','--version'],'typescript-zod':['node','--version'],'java':['java','-version'],'go':['go','version'],'swift':['swift','--version']}
    if 'wasi' in engines:
     checks['wasmtime']=[shutil.which('wasmtime') or str(Path.home()/'.wasmtime/bin/wasmtime'),'--version']
     if 'csharp' in languages:checks['wasi-dotnet']=[str(wasi_dotnet),'--version']
     if 'swift' in languages and shutil.which('swiftly'):checks['swift']=['swiftly','run','swift','--version','+'+args.swift_toolchain]
    for name,command in checks.items():
     if name in LANGUAGES and name not in languages:continue
     try:
      version=harness.run(command);versions[name]=(version.stdout+version.stderr).strip()
     except (OSError,subprocess.CalledProcessError) as e:versions[name]=str(e)
    if 'typescript-zod' in languages:versions['zod']=json.loads((REPO/'samples/file-based/package.json').read_text())['dependencies']['zod']
    rows=[];args.output.parent.mkdir(parents=True,exist_ok=True)
    def save():args.output.write_text(json.dumps({'timestampUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'toolchains':versions,'timeoutSeconds':args.timeout,'maxProjectionSeconds':args.max_projection_seconds,'buildProofs':({'csharp-wasi':json.loads((args.build_dir/'wasi-csharp/build-proof.json').read_text())} if 'csharp-wasi' in commands else {}),'optimization':('C# Native AOT Speed/native ISA and LLVMOnly WASI AOT/SIMD/-O3; Rust release/LTO/native ISA or SIMD; Wasmtime opt-level=2' if 'wasi' in engines else 'Host defaults'),'platform':platform.platform(),'routing':args.routing,'sizes':sizes,'samples':args.samples,'buildKey':key,'commands':commands,'unavailable':unavailable,'results':rows,'notes':['Peak RSS includes the entire process. For WASI it includes Wasmtime compilation/runtime, not guest allocations alone.','Events are generated and projected in one streaming pass; event construction and dispatch are inside the projection timer. typescript-native is simple unchecked projection; typescript-zod-native validates/clones payload data only in matching handlers, inside the same timer, and ignores unknown event kinds.','Capacity is the largest tested point within the time budget, not a language or hardware maximum.']},indent=2)+'\n')
    save()
    for name in commands:
      for mode in modes:
       for profile in profiles:
        previous=None;stop=False
        for events in sizes:
         row={'implementation':name,'mode':mode,'profile':profile,'events':events};rows.append(row)
         if stop:row['status']='not-run-after-budget';save();continue
         aggregates=1 if profile=='dense' else math.ceil(events/100);expected=reference(events,aggregates,args.routing)
         measurements=[]
         try:
          for sample in range(args.samples):
           result=harness.measure(name,commands,events,1,args.timeout,args.output.parent/(args.output.stem+'.progress')/mode/profile/str(sample),expected_sink=expected['sink'],invocation_args=[events,aggregates,mode,args.routing],workload_label=f'{events:,} events, {mode}, {profile}')
           if any(result[k]!=v for k,v in expected.items()):raise RuntimeError(name+': projection state differs from reference')
           measurements.append(result)
          seconds=statistics.median(r['measurements'][0]['totalSeconds']for r in measurements)
          row.update({'status':'ok','projectionSeconds':seconds,'eventsPerSecond':events/seconds,'nsPerEvent':seconds/events*1e9,'peakRssMiB':max(r['peakRssBytes']for r in measurements)/1048576,'startupSeconds':statistics.median(r['startupSeconds']for r in measurements),'preparationSeconds':statistics.median(r['preparationSeconds']for r in measurements),'wallSeconds':statistics.median(r['wallSeconds']for r in measurements),'exitSeconds':statistics.median(r['exitSeconds']for r in measurements),'stateCount':expected['stateCount'],'retainedItems':expected['retainedItems'],'maxHistoryLength':expected['maxHistoryLength'],'samples':measurements})
          if previous:row['growthExponent']=math.log(seconds/previous[1])/math.log(events/previous[0])
          previous=(events,seconds);stop=seconds>args.max_projection_seconds
         except subprocess.TimeoutExpired:row['status']='timeout';stop=True
         except (subprocess.CalledProcessError,RuntimeError,OSError,ValueError) as e:
          row.update(status='error',error=str(e),stderr=getattr(e,'stderr',None),stdout=getattr(e,'stdout',None));stop=True
         save()
    lines=['| implementation | mode | profile | events | seconds | ns/event | peak RSS MiB | max history | growth p | status |','|---|---|---|---:|---:|---:|---:|---:|---:|---|']
    for r in rows:lines.append('| '+ ' | '.join([r['implementation'],r['mode'],r['profile'],str(r['events']),f"{r.get('projectionSeconds',0):.4f}" if r['status']=='ok' else '—',f"{r.get('nsPerEvent',0):.1f}" if r['status']=='ok' else '—',f"{r.get('peakRssMiB',0):.1f}"if r['status']=='ok' else '—',str(r.get('maxHistoryLength','—')),f"{r['growthExponent']:.2f}"if 'growthExponent'in r else '—',r['status']])+' |')
    lines+=['', '| implementation | mode | profile | events | startup s | preparation s | exit s | process wall s |','|---|---|---|---:|---:|---:|---:|---:|']
    for r in rows:
     if r['status']=='ok':lines.append(f"| {r['implementation']} | {r['mode']} | {r['profile']} | {r['events']} | {r['startupSeconds']:.4f} | {r['preparationSeconds']:.4f} | {r['exitSeconds']:.4f} | {r['wallSeconds']:.4f} |")
    args.output.with_suffix('.md').write_text('\n'.join(lines)+'\n');print('\n'.join(lines));print('Saved '+str(args.output))
    capacities=[]
    for name in commands:
     for mode in modes:
      for profile in profiles:
       good=[r for r in rows if r['implementation']==name and r['mode']==mode and r['profile']==profile and r['status']=='ok' and r['projectionSeconds']<=args.max_projection_seconds]
       capacities.append({'implementation':name,'mode':mode,'profile':profile,'largestTestedWithinTimeBudget':max((r['events'] for r in good),default=None),'timeBudgetSeconds':args.max_projection_seconds})
    report=json.loads(args.output.read_text());report['testedCapacity']=capacities;args.output.write_text(json.dumps(report,indent=2)+'\n')
    for name,reason in unavailable.items():print('UNAVAILABLE '+name+': '+reason[:500])

if __name__=='__main__':
 try:main()
 except subprocess.CalledProcessError as e:print(e.stdout or '',file=sys.stderr);print(e.stderr or '',file=sys.stderr);raise SystemExit(e.returncode)
