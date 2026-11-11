"""Independent shared golden values, query audits and cross-language store-log handoff."""
import json
from pathlib import Path
import subprocess
from run import canonical, execute, no_null
HERE=Path(__file__).resolve().parent

def matches(query,event):
    def tag(t):return (t['category'],tuple(t['keys']))
    actual=set(map(tag,event['tags']))
    return not query['items'] or any((not item['types'] or event['eventType'] in item['types']) and set(map(tag,item['tags'])).issubset(actual) for item in query['items'])

def verify_one(command,expected):
    for name,golden in expected.items():
        payload=json.loads((HERE/'fixtures'/f'{name}.json').read_text())
        result=canonical(json.loads(execute(command,payload).stdout))
        no_null(result)
        for key,want in golden.items():
            if result.get(key)!=want:
                got=result.get(key)
                raise AssertionError(f'{name}/{key}: expected {json.dumps(want,ensure_ascii=False)[:1800]}, got {json.dumps(got,ensure_ascii=False)[:1800]}')
        if result['rebuilt']!=golden['list'] or result['listenerFaults']!=0:raise AssertionError(name+': live/rebuilt projection health')
        for read in result['reads']:
            if not read['query']['items']:raise AssertionError(name+': command used all-events query')
            count=sum(e['position']<=read['after'] and matches(read['query'],e) for e in result['events'])
            if count!=read['returned']:raise AssertionError(name+': query audit not the actual decision snapshot')
        resumed=canonical(json.loads(execute(command,{'history':result['events'],'operations':[{'$type':'ListAll'}]}).stdout))
        if resumed['list']!=golden['list']:raise AssertionError(name+': imported store log replay')
    for path in (HERE/'fixtures/invalid').glob('*.json'):
        result=subprocess.run(command,input=path.read_text(),text=True,capture_output=True,timeout=60)
        if result.returncode==0:raise AssertionError(path.name+': invalid input accepted')

def verify_all(commands):
    expected=json.loads((HERE/'fixtures/expected.json').read_text())
    for language,command in commands.items():
        verify_one(command,expected)
        print(f'{language}: {len(expected)} golden ES/DCB scenarios and store replays PASS',flush=True)
    origin=json.loads((HERE/'fixtures/lifecycle.json').read_text())
    continuation=[{'$type':'Execute','commandId':'cross-open','command':{'$type':'OpenClass','classId':'b2','course':'Advanced','capacity':1}},
                  {'$type':'Execute','commandId':'cross-enroll','command':{'$type':'Enroll','classId':'b2','studentId':'bob'}},{'$type':'ListAll'}]
    want=expected['resume']['list']
    for writer,command in commands.items():
        history=json.loads(execute(command,origin).stdout)['events']
        for reader,target in commands.items():
            result=canonical(json.loads(execute(target,{'history':history,'operations':continuation}).stdout))
            if result['list']!=want or result['rebuilt']!=want or result['listenerFaults']!=0:raise AssertionError(writer+' -> '+reader)
    print(f'Cross-language tagged event-store handoff: {len(commands)**2} pairs PASS',flush=True)
