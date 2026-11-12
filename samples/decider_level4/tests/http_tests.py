#!/usr/bin/env python3
"""Same Given/When/Then HTTP contract and ES tests for all native hosts."""
from urllib.parse import quote
import argparse,concurrent.futures,contextlib,hashlib,http.client,importlib.util,json,os,signal,socket,subprocess,sys,tempfile,time
from pathlib import Path
HERE=Path(__file__).resolve().parents[1]
spec=importlib.util.spec_from_file_location('http_builder',HERE/'run.py');builder=importlib.util.module_from_spec(spec);spec.loader.exec_module(builder)

SCHEMAS=json.loads((HERE/'openapi.json').read_text())['components']['schemas']
def validate_json(value,schema):
    if '$ref'in schema:return validate_json(value,SCHEMAS[schema['$ref'].split('/')[-1]])
    if 'oneOf'in schema:
        choices=[c for c in schema['oneOf']if c['$ref'].split('/')[-1]==value.get('$type')]
        assert len(choices)==1,(value,schema)
        return validate_json(value,choices[0])
    if 'const'in schema:assert value==schema['const'],(value,schema)
    if 'enum'in schema:assert value in schema['enum'],(value,schema)
    kind=schema.get('type')
    if kind=='object':
        assert isinstance(value,dict),(value,schema)
        assert set(schema.get('required',[]))<=value.keys(),(value,schema)
        if schema.get('additionalProperties')is False:assert value.keys()<=schema.get('properties',{}).keys(),(value,schema)
        for k,v in value.items():
            if k in schema.get('properties',{}):validate_json(v,schema['properties'][k])
    elif kind=='array':
        assert isinstance(value,list),(value,schema)
        for item in value:validate_json(item,schema.get('items',{}))
    elif kind=='string':assert isinstance(value,str),(value,schema)
    elif kind=='integer':assert isinstance(value,int)and not isinstance(value,bool),(value,schema)
    if 'minimum'in schema:assert value>=schema['minimum']
    if 'maximum'in schema:assert value<=schema['maximum']

def response_schema(method,path,code):
    if code in [409,422]:return SCHEMAS['CommandResult']
    if code>=400:return SCHEMAS['Error']
    name='CommandResult'if path=='/commands'or path.endswith('/commit')else 'Ticket'if path=='/tickets'else 'Health'if path=='/health'else 'Status'if path=='/status'else 'SchoolSnapshot'if path=='/school'else 'StudentSnapshot'
    return SCHEMAS[name]

class Client:
    def __init__(self,port):self.port=port
    def request(self,method,path,body=None,raw=None,content='application/json'):
        connection=http.client.HTTPConnection('127.0.0.1',self.port,timeout=30)
        data=raw if raw is not None else json.dumps(body,separators=(',',':')).encode()if body is not None else None
        try:
            connection.request(method,path,body=data,headers={'Content-Type':content})
            response=connection.getresponse();status=response.status;result=json.loads(response.read());validate_json(result,response_schema(method,path,status));return status,result
        finally:connection.close()
    def command(self,kind,expected=200,**fields):
        command={'$type':kind,**fields};code,result=self.request('POST','/commands',{'commandId':'test-'+str(time.monotonic_ns()),'command':command});assert code==expected,(command,code,result);return result

@contextlib.contextmanager
def server(language,command,sqlite=None,ttl=60):
    with socket.socket()as sock:sock.bind(('127.0.0.1',0));port=sock.getsockname()[1]
    env={**os.environ,'PORT':str(port),'API_SPEC':str(HERE/'openapi.json'),'TICKET_TTL_SECONDS':str(ttl),'DOTNET_TieredCompilation':'1','DOTNET_TieredPGO':'1'}
    if sqlite:env['DECIDER_PERSISTENT']=str(sqlite)
    else:env.pop('DECIDER_PERSISTENT',None)
    logpath=HERE/'.build'/f'{language}-server.log'
    with logpath.open('w')as log:
        start=time.perf_counter();process=subprocess.Popen(command,env=env,stdout=log,stderr=log,start_new_session=True);client=Client(port)
        try:
            while True:
                if process.poll()is not None:raise RuntimeError(logpath.read_text())
                try:
                    if client.request('GET','/health')==(200,{'status':'ok'}):break
                except(OSError,ValueError):pass
                if time.perf_counter()-start>40:raise RuntimeError('startup timeout '+logpath.read_text())
                time.sleep(.02)
            yield client,process,time.perf_counter()-start
        finally:
            if process.poll()is None:
                os.killpg(process.pid,signal.SIGTERM)
                try:process.wait(timeout=10)
                except subprocess.TimeoutExpired:os.killpg(process.pid,signal.SIGKILL);process.wait()

def digest(value):return hashlib.sha256(json.dumps(value,sort_keys=True,separators=(',',':'),ensure_ascii=False).encode()).hexdigest()

def scenario(client):
    checks=0
    def check(condition,*details):
        nonlocal checks
        assert condition,details;checks+=1
    for student,grade in [('a','Year3'),('b','Year3'),('c','Year1')]:client.command('RegisterStudent',studentId=student,grade=grade)
    client.command('OpenClass',classId='foundation',course='Foundation',capacity=1)
    check(client.request('GET','/students/missing')[0]==404)
    check(client.request('GET','/students/a')[1]['student']=={'studentId':'a','grade':'Year3'})
    # Given two commands prepared against the same version, When committed,
    # Then the loser rereads and decides ClassFull rather than overbooking.
    prepared=[]
    for student in ['a','b']:
        code,t=client.request('POST','/tickets',{'commandId':'ticket-'+student,'command':{'$type':'Enroll','classId':'foundation','studentId':student}});check(code==201,code,t);prepared.append(t)
    check(prepared[0]['position']==prepared[1]['position'])
    code,first=client.request('POST','/tickets/'+prepared[0]['ticket']+'/commit');check(code==200,first)
    code,second=client.request('POST','/tickets/'+prepared[1]['ticket']+'/commit');check(code==422 and second['conflicts']==1 and second['errors']==['ClassFull'],second)
    check(client.request('POST','/tickets/'+prepared[0]['ticket']+'/commit')[0]==404)
    state=client.request('GET','/school')[1];check(state['position']==first['projectionPosition'],state,first);check(state['state']['classes'][0]['students']==['a'],state)
    client.command('Drop',classId='foundation',studentId='a');client.command('Enroll',classId='foundation',studentId='b')
    client.command('StartClass',classId='foundation',startedOn='2026-09-01')
    client.command('Assess',expected=422,classId='foundation',studentId='b',score=80)
    client.command('Attend',classId='foundation',studentId='b');client.command('Assess',classId='foundation',studentId='b',score=80);client.command('CompleteClass',classId='foundation')
    client.command('OpenClass',classId='advanced',course='Advanced',capacity=2)
    check(client.command('Enroll',expected=422,classId='advanced',studentId='a')['errors']==['PrerequisiteMissing'])
    check(client.command('Enroll',expected=422,classId='advanced',studentId='c')['errors']==['GradeTooLow'])
    r=client.command('Enroll',classId='advanced',studentId='b');check(r['projectionPosition']>=r['position'])
    before=client.request('GET','/status')[1]
    for kind,fields,error in [('RegisterStudent',{'studentId':'','grade':'Year1'},'InvalidId'),('OpenClass',{'classId':'bad','course':'Foundation','capacity':0},'InvalidCapacity'),('StartClass',{'classId':'advanced','startedOn':'bad'},'InvalidDate'),('Assess',{'classId':'advanced','studentId':'b','score':101},'InvalidScore')]:
        result=client.command(kind,expected=422,**fields);check(error in result['errors'],result)
    for body in [None,[],{}, {'commandId':'x','command':None},{'commandId':'x','command':{'$type':'Unknown'}},{'commandId':'x','command':{'$type':'RegisterStudent','studentId':'d'}},{'commandId':'x','command':{'$type':'RegisterStudent','studentId':'d','grade':'Year9'}},{'commandId':'x','command':{'$type':'OpenClass','classId':'bad','course':'Foundation','capacity':True}},{'commandId':'x','command':{'$type':'RegisterStudent','studentId':'d','grade':'Year1','extra':1}}]:
        code,result=client.request('POST','/commands',raw=json.dumps(body).encode());check(code==400,body,code,result)
    for raw in [b'{',b'',b'not-json']:
        check(client.request('POST','/commands',raw=raw)[0]==400,raw)
    check(client.request('POST','/commands',body={},content='text/plain')[0]==415)
    check(client.request('POST','/commands',raw=b' '*1048577)[0]==413)
    check(client.request('GET','/commands')[0]==405);check(client.request('GET','/unknown')[0]==404)
    check(client.request('GET','/status')[1]==before,'invalid commands must not append')
    # JSON Schema integers include mathematically integral decimal tokens.
    check(client.request('POST','/commands',raw=b'{"commandId":"decimal","command":{"$type":"OpenClass","classId":"decimal","course":"Foundation","capacity":1.0}}')[0]==200)
    check(client.request('POST','/commands',raw=b'{"commandId":"fraction","command":{"$type":"OpenClass","classId":"fraction","course":"Foundation","capacity":1.5}}')[0]==400)
    for body in [{'commandId':'x','command':{'$type':'OpenClass','classId':'overflow','course':'Foundation','capacity':2147483648}}]:
        check(client.request('POST','/commands',body)[0]==400)
    unicode_id='学生 + résumé'
    client.command('RegisterStudent',studentId=unicode_id,grade='Year2')
    check(client.request('GET','/students/'+quote(unicode_id,safe=''))[1]['student']['studentId']==unicode_id)
    # Concurrent requests target one seat. Final state and projection must agree.
    client.command('OpenClass',classId='race',course='Foundation',capacity=1)
    for i in range(12):client.command('RegisterStudent',studentId='race-'+str(i),grade='Year2')
    def enroll(i):return client.request('POST','/commands',{'commandId':'race-'+str(i),'command':{'$type':'Enroll','classId':'race','studentId':'race-'+str(i)}})
    with concurrent.futures.ThreadPoolExecutor(max_workers=12)as pool:results=list(pool.map(enroll,range(12)))
    check(sum(code==200 for code,_ in results)==1,results)
    check(all(code in [200,422,409]for code,_ in results),results)
    status=client.request('GET','/status')[1];check(status['eventPosition']==status['projectionPosition']and status['listenerFaults']==0,status)
    school=client.request('GET','/school')[1];room=next(c for c in school['state']['classes']if c['plan']['classId']=='race');check(len(room['students'])==1,room)
    # Race winner is intentionally nondeterministic: normalize only this field
    # when comparing across languages, never omit events from replay checks.
    return checks,school

def run_language(language,command):
    summaries=[]
    for store in ['memory-json','sqlite']:
        with tempfile.TemporaryDirectory(prefix='level4-test-')as temp:
            path=Path(temp)/'school.db'if store=='sqlite'else None
            with server(language,command,path)as(client,_,startup):checks,state=scenario(client)
            if path:
                with server(language,command,path)as(client,_,_):
                    restored=client.request('GET','/school')[1];assert restored==state,('restart replay',language);checks+=1
            summaries.append({'language':language,'store':store,'assertions':checks,'startupSeconds':startup,'stateSha256':digest(state)})
    with server(language,command,ttl=.1)as(client,_,_):
        client.command('RegisterStudent',studentId='expire',grade='Year2')
        code,t=client.request('POST','/tickets',{'commandId':'ttl','command':{'$type':'PromoteStudent','studentId':'expire','grade':'Year3'}});assert code==201
        time.sleep(.15);assert client.request('POST','/tickets/'+t['ticket']+'/commit')[0]==404
    with server(language,command) as (client,_,_):
        client.command('RegisterStudent',studentId='bounded',grade='Year2')
        request={'commandId':'bounded','command':{'$type':'PromoteStudent','studentId':'bounded','grade':'Year3'}}
        first=None
        for _ in range(1024):
            code,t=client.request('POST','/tickets',request);assert code==201
            if first is None:first=t
        assert client.request('POST','/tickets',request)[0]==429
        assert client.request('POST','/tickets/extra/'+first['ticket']+'/commit')[0]==404
        assert client.request('POST','/tickets/'+first['ticket']+'/commit')[0]==200
        assert client.request('POST','/tickets',request)[0]==201
    return summaries

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('--language',default='all');p.add_argument('--no-build',action='store_true');a=p.parse_args();languages=builder.LANGUAGES if a.language=='all'else a.language.split(',');results=[]
    for language in languages:
        print('HTTP testing '+language,flush=True)
        cache=HERE/'.build'/f'{language}-command.json';command=json.loads(cache.read_text())if a.no_build else builder.build(language);cache.write_text(json.dumps(command))
        rows=run_language(language,command);results.extend(rows);print(json.dumps(rows),flush=True)
    # Same SQLite JSON events replay through every native domain decoder.
    if len(languages)==7:
        with tempfile.TemporaryDirectory(prefix='level4-cross-replay-') as temp:
            path=Path(temp)/'shared.db'
            command=json.loads((HERE/'.build'/f'{languages[0]}-command.json').read_text())
            with server(languages[0],command,path) as (client,_,_):_,expected=scenario(client)
            for language in languages:
                command=json.loads((HERE/'.build'/f'{language}-command.json').read_text())
                with server(language,command,path) as (client,_,_):
                    actual=client.request('GET','/school')[1]
                    assert actual==expected,('cross-language JSON replay',language)
            print('Cross-language SQLite replay: 7/7 PASS',flush=True)
    (HERE/'.build/http-tests.json').write_text(json.dumps(results,indent=2)+'\n')
