#!/usr/bin/env python3
"""Sequential seven-host HTTP benchmark, same work and HTTP client."""
import argparse,concurrent.futures,hashlib,http.client,importlib.util,json,math,os,platform,signal,subprocess,tempfile,threading,time
from pathlib import Path
HERE=Path(__file__).resolve().parent;ROOT=HERE.parent
spec=importlib.util.spec_from_file_location('http_tests',ROOT/'tests/http_tests.py');tests=importlib.util.module_from_spec(spec);spec.loader.exec_module(tests)
builder=tests.builder

class Load:
    def __init__(self,port):
        self.port=port;self.local=threading.local();self.gate=threading.Lock();self.samples=[];self.connections=[];self.errors=[];self.expected_rejections=0;self.conflicts=0;self.bytes=0
    def request(self,method,path,body=None,expected=200,category='command'):
        if not hasattr(self.local,'connection'):
            self.local.connection=http.client.HTTPConnection('127.0.0.1',self.port,timeout=60)
            with self.gate:self.connections.append(self.local.connection)
        conn=self.local.connection;raw=json.dumps(body,separators=(',',':')).encode()if body is not None else None;start=time.perf_counter();code=0;result=None;error=None;size=0
        try:
            conn.request(method,path,raw,{'Content-Type':'application/json'});response=conn.getresponse();code=response.status;data=response.read();size=len(data);result=json.loads(data)
            if code!=expected:error=f'{method} {path}: expected {expected}, received {code}: {result}'
        except Exception as e:error=repr(e);conn.close();del self.local.connection
        with self.gate:
            self.samples.append((category,time.perf_counter()-start,code));self.bytes+=size
            if error:self.errors.append(error)
            elif code>=400:self.expected_rejections+=1
            if isinstance(result,dict):self.conflicts+=result.get('conflicts',0)
        if error:raise RuntimeError(error)
        return result
    def command(self,kind,expected=200,**fields):
        return self.request('POST','/commands',{'commandId':kind+'-'+str(time.monotonic_ns()),'command':{'$type':kind,**fields}},expected,'rejection'if expected!=200 else'command')
    def group(self,group,prefix='student'):
        room=f'{prefix}-class-{group:06d}'
        self.command('OpenClass',classId=room,course='Foundation',capacity=10)
        for i in range(group*10,group*10+10):
            student=f'{prefix}-{i:08d}'
            self.command('RegisterStudent',studentId=student,grade='Year3')
            self.command('Enroll',classId=room,studentId=student)
            result=self.request('GET','/students/'+student,category='query');assert result['student']=={'studentId':student,'grade':'Year3'}
            self.command('Drop',classId=room,studentId=student);self.command('Enroll',classId=room,studentId=student)
        self.command('StartClass',classId=room,startedOn='2026-09-01')
        for i in range(group*10,group*10+10):
            student=f'{prefix}-{i:08d}'
            self.command('Attend',classId=room,studentId=student);self.command('Assess',classId=room,studentId=student,score=80)
        self.command('CompleteClass',classId=room)
        rejected=self.command('OpenClass',expected=422,classId='invalid-'+room,course='Foundation',capacity=0);assert rejected['errors']==['InvalidCapacity']
    def close_all(self):
        for connection in self.connections:connection.close()
        self.connections.clear();self.close()
    def close(self):
        if hasattr(self.local,'connection'):self.local.connection.close();del self.local.connection

def percentiles(samples):
    values=sorted(x[1]*1000 for x in samples)
    return {k:values[max(0,math.ceil(len(values)*p)-1)] for k,p in [('p50',.5),('p95',.95),('p99',.99)]}if values else{}

def measure(language,command,store,n,workers):
    with tempfile.TemporaryDirectory(prefix='level4-bench-')as temp:
        db=Path(temp)/'school.db'if store=='sqlite'else None
        with tests.server(language,command,db)as(client,process,startup):
            load=Load(client.port)
            # Warm the HTTP and domain paths in this same process; retain and
            # explicitly account for these 100 students in final state checks.
            for g in range(10):load.group(g,'warmup')
            load.close_all();load.samples.clear();load.errors.clear();load.expected_rejections=0;load.conflicts=0;load.bytes=0
            stop=threading.Event();rss=[]
            def sample():
                while not stop.is_set():
                    ps=subprocess.run(['ps','-p',str(process.pid),'-o','rss='],capture_output=True,text=True)
                    if ps.returncode==0 and ps.stdout.strip():rss.append(int(ps.stdout.strip())/1024)
                    stop.wait(.1)
            sampler=threading.Thread(target=sample);sampler.start();start=time.perf_counter();cpu=time.process_time();phases={}
            try:
                # One large workload, not repeated timing of an identical object.
                batch_start=time.perf_counter()
                with concurrent.futures.ThreadPoolExecutor(max_workers=workers)as pool:list(pool.map(load.group,range(n//10)))
                load.close_all()
                phases['lifecycleSeconds']=time.perf_counter()-batch_start
                conflict_start=time.perf_counter();rounds=min(50,n//10)
                for i in range(rounds):
                    room=f'race-{i:06d}';load.command('OpenClass',classId=room,course='Advanced',capacity=1)
                    tickets=[]
                    for student in ['student-00000000','student-00000001']:
                        tickets.append(load.request('POST','/tickets',{'commandId':f'race-{i}-{student}','command':{'$type':'Enroll','classId':room,'studentId':student}},201,'prepare'))
                    first=load.request('POST','/tickets/'+tickets[0]['ticket']+'/commit',category='commit')
                    second=load.request('POST','/tickets/'+tickets[1]['ticket']+'/commit',expected=422,category='rejection')
                    assert first['projectionPosition']>=first['position']and second['errors']==['ClassFull']and second['conflicts']==1
                load.close_all()
                phases['conflictSeconds']=time.perf_counter()-conflict_start
                query_start=time.perf_counter()
                with concurrent.futures.ThreadPoolExecutor(max_workers=workers)as pool:
                    list(pool.map(lambda i:load.request('GET',f'/students/student-{i:08d}',category='query'),range(n)))
                for _ in range(3):school=load.request('GET','/school',category='list')
                phases['querySeconds']=time.perf_counter()-query_start
                duration=time.perf_counter()-start;client_cpu=time.process_time()-cpu
                expected_events=73*(n//10+10)+2*rounds
                assert school['position']==expected_events,(school['position'],expected_events)
                assert len(school['state']['students'])==n+100
                assert len(school['state']['classes'])==n//10+10+rounds
                assert len(school['state']['enrollments'])==n+100+rounds
                status=client.request('GET','/status')[1];assert status=={'eventPosition':expected_events,'projectionPosition':expected_events,'listenerFaults':0}
                state_hash=tests.digest(school)
            finally:stop.set();sampler.join();load.close_all()
            shutdown_start=time.perf_counter();os.killpg(process.pid,signal.SIGTERM);process.wait(timeout=10);shutdown=time.perf_counter()-shutdown_start
        restart=None
        if db:
            with tests.server(language,command,db)as(client,_,ready):
                restored=client.request('GET','/school')[1];assert tests.digest(restored)==state_hash;restart=ready
        by_category={category:{'requests':len(rows),'latencyMs':percentiles(rows)}for category in sorted({x[0]for x in load.samples})for rows in [[x for x in load.samples if x[0]==category]]}
        return {'language':language,'store':store,'students':n,'warmupStudents':100,'concurrency':workers,'requests':len(load.samples),'seconds':duration,'requestsPerSecond':len(load.samples)/duration,'latencyMs':percentiles(load.samples),'unexpectedErrors':len(load.errors),'errorRate':len(load.errors)/len(load.samples),'expectedRejections':load.expected_rejections,'conflicts':load.conflicts,'sampledPeakRssMiB':max(rss),'rssSamples':len(rss),'startupSeconds':startup,'shutdownSeconds':shutdown,'restartReadySeconds':restart,'clientCpuSeconds':client_cpu,'clientCpuPerWall':client_cpu/duration,'responseBytes':load.bytes,'phases':phases,'categories':by_category,'stateSha256':state_hash,'events':expected_events}

def report(data,path):
    path.parent.mkdir(parents=True,exist_ok=True);path.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n')
    title='Linux x64 Actions HTTP API benchmark' if data.get('actions') else 'Local HTTP API benchmark'
    lines=['# '+title,'',f"`{data['platform']}` / `{data['machine']}`。同じホストで言語・ストアを順次測定。各ストアのデータ量・リクエスト構成・同時接続数は共通です。",'',
           'SQLiteとメモリJSONは別プロセス・別ストアです。100人の業務ウォームアップ後に測定し、その100人も状態検証とメモリには含めます。ビルド・起動・終了・SQLite再起動はwall計測から分離しています。','',
           '| 実装 | store | wall s | req/s | p50 ms | p95 ms | p99 ms | unexpected error % | expected rejection | sampled RSS MiB | start s | stop s |','|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|']
    for r in data['results']:
        name='ts-zod'if r['language']=='typescript'else r['language'];label=f"[{name}]({os.path.relpath(ROOT/r['language'],path.parent)})";p=r['latencyMs']
        lines.append(f"| {label} | {r['store']} | {r['seconds']:.3f} | {r['requestsPerSecond']:.1f} | {p['p50']:.3f} | {p['p95']:.3f} | {p['p99']:.3f} | {100*r['errorRate']:.2f} | {r['expectedRejections']} | {r['sampledPeakRssMiB']:.1f} | {r['startupSeconds']:.3f} | {r['shutdownSeconds']:.3f} |")
    lines+=['','全行で状態ハッシュ・イベント数・リクエスト数・期待した拒否数・競合数を照合済みです。SQLiteでは再起動後の状態ハッシュも一致します。詳細なフェーズ、API種別ごとの遅延、実行環境・版、クライアントCPU負荷は[raw data]('+path.name+')を参照してください。','',
            'RSSはサーバープロセスを100ms間隔で観測した最大値であり、OSのhigh-water markではありません。stopは負荷終了後のSIGTERMからプロセス終了までです。ウォームアップを含むサーバー全体のメモリです。','',
            '閉ループの固定同時接続数で測っています。飽和時の到着率・キュー待ちの保証はありません。Python負荷側のCPUが律速になる可能性があるため、clientCpuPerWallとホストごとのHTTP実装も併せて見てください。1回の一連の業務の観測であり、言語単体の順位ではありません。','']
    path.with_suffix('.md').write_text('\n'.join(lines))

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--language',default='all');p.add_argument('--students',type=int,default=10000);p.add_argument('--concurrency',type=int,default=8);p.add_argument('--store',choices=['both','sqlite','memory-json'],default='both');p.add_argument('--no-build',action='store_true');p.add_argument('--output',type=Path,default=HERE/'observed/macos-arm64.json');a=p.parse_args()
    if a.students<10 or a.students%10 or a.concurrency<1:p.error('students must be a positive multiple of ten; concurrency must be positive')
    languages=builder.LANGUAGES if a.language=='all'else a.language.split(',');commands={}
    for language in languages:
        cache=ROOT/'.build'/f'{language}-command.json';commands[language]=json.loads(cache.read_text())if a.no_build else builder.build(language);cache.write_text(json.dumps(commands[language]))
    versions={}
    for name,cmd in {'dotnet':['dotnet','--info'],'node':['node','--version'],'rust':['rustc','--version'],'java':['java','-version'],'go':['go','version'],'swift':['swift','--version']}.items():
        r=subprocess.run(cmd,capture_output=True,text=True);versions[name]=(r.stdout+r.stderr).strip()
    hardware={'cpu':platform.processor(),'memoryBytes':os.sysconf('SC_PAGE_SIZE')*os.sysconf('SC_PHYS_PAGES'),'logicalCpuCount':os.cpu_count()}
    if platform.system()=='Darwin':hardware['cpu']=subprocess.run(['sysctl','-n','machdep.cpu.brand_string'],capture_output=True,text=True).stdout.strip()
    elif Path('/proc/cpuinfo').exists():hardware['cpu']=next((s.split(':',1)[1].strip()for s in Path('/proc/cpuinfo').read_text().splitlines()if s.startswith('model name')),platform.processor())
    if platform.system()=='Linux':hardware.update(osRelease=Path('/etc/os-release').read_text(),kernel=platform.release())
    data={'formatVersion':1,'platform':platform.platform(),'machine':platform.machine(),'hardware':hardware,'sourceDirty':bool(subprocess.run(['git','status','--porcelain','--',str(ROOT),str(ROOT.parent/'decider_level3')],capture_output=True,text=True).stdout.strip()),'measuredAt':time.strftime('%Y-%m-%dT%H:%M:%SZ',time.gmtime()),'sourceCommit':subprocess.run(['git','rev-parse','HEAD'],capture_output=True,text=True).stdout.strip(),'students':a.students,'concurrency':a.concurrency,'commands':commands,'runtimeVersions':versions,'openapiSha256':hashlib.sha256((ROOT/'openapi.json').read_bytes()).hexdigest(),'sourceManifest':{str(f.relative_to(ROOT.parent)):hashlib.sha256(f.read_bytes()).hexdigest()for folder in [ROOT,ROOT.parent/'decider_level3']for f in folder.rglob('*')if f.is_file()and not any(x in f.parts for x in ['.build','bin','obj','target','node_modules','__pycache__','observed'])and f.suffix in ['.cs','.fs','.rs','.ts','.go','.java','.swift','.json','.toml','.lock','.mod','.sum','.py','.csproj','.fsproj','.resolved','.sql','.h','.modulemap']},'results':[]}
    for language in languages:
        for store in ['sqlite','memory-json']if a.store=='both'else[a.store]:
            print(f'Measuring {language} {store}',flush=True);r=measure(language,commands[language],store,a.students,a.concurrency);data['results'].append(r);print(json.dumps({k:r[k]for k in ['language','store','seconds','requestsPerSecond','unexpectedErrors','sampledPeakRssMiB']}),flush=True)
    for key in ['stateSha256','events','requests','expectedRejections','conflicts']:
        assert len({r[key]for r in data['results']})==1,('cohort mismatch',key)
    report(data,a.output)
