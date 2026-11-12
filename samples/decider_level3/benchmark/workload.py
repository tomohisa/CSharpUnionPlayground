"""Deterministic, shared school command workload; no per-language shortcuts."""
import hashlib
import json
from collections import Counter


def generate(students):
    if students < 10 or students % 10:
        raise ValueError('students must be a positive multiple of ten')
    n=students; classes=n//10; races=max(1,n//100)
    ops=[]
    def command(kind,**fields):
        c={'$type':kind,**fields}
        ops.append({'$type':'Execute','commandId':f'cmd-{len(ops):09}', 'command':c})
    def sid(i): return f'student-{i:08}'
    def cid(i,advanced=False): return f'{"advanced" if advanced else "foundation"}-{i:08}'
    for i in range(n+2*races): command('RegisterStudent',studentId=sid(i),grade='Year3')
    for i in range(classes):
        for advanced in [False,True]: command('OpenClass',classId=cid(i,advanced),course='Advanced' if advanced else 'Foundation',capacity=10)
        command('RegisterStudent',studentId=' ',grade='Year3')
        command('OpenClass',classId=f'invalid-{i}',course='Foundation',capacity=25)
    for i in range(n): command('Enroll',classId=cid(i//10),studentId=sid(i))
    for i in range(classes): command('StartClass',classId=cid(i),startedOn='2026-04-01')
    for _ in range(2):
        for i in range(n): command('Attend',classId=cid(i//10),studentId=sid(i))
    for i in range(n): command('Assess',classId=cid(i//10),studentId=sid(i),score=80 if i%10<7 else 50)
    for i in range(classes): command('CompleteClass',classId=cid(i))
    for i in range(n): command('Enroll',classId=cid(i//10,True),studentId=sid(i))
    for i in range(races):
        key=f'race-{i}'
        command('OpenClass',classId=key,course='Foundation',capacity=1)
        for side in range(2):
            ops.append({'$type':'Prepare','key':f'{key}-{side}','commandId':f'race-command-{i}-{side}',
                        'command':{'$type':'Enroll','classId':key,'studentId':sid(n+2*i+side)}})
        for side in range(2): ops.append({'$type':'Commit','key':f'{key}-{side}'})
    # Full list query, duplicate delivery, and independent replay exercise the
    # same live projection used by commands, instead of a benchmark-only fold.
    ops.extend([{'$type':'ListAll'} for _ in range(3)])
    ops.extend([{'$type':'Redeliver'},{'$type':'Rebuild'}])
    expected={'students':n+2*races,'classes':2*classes+races,'enrollments':17*n//10+races,
              'events':71*n//10+4*races,'committed':61*n//10+4*races,
              'rejected':n//2+races,'conflicts':races,'passed':7*n//10,'failed':3*n//10}
    payload={'history':[],'operations':ops}
    return payload,expected


def validate(result,expected,restart=False):
    state=result['list']
    for name in ['students','classes','enrollments']:
        assert len(state[name])==expected[name], (name,len(state[name]),expected[name])
    summary='eventCount' in result
    assert (result['eventCount'] if summary else len(result['events']))==expected['events']
    assert result['checkpoint']==expected['events'] and result['listenerFaults']==0
    assert result['rebuildMatches'] if summary else result['list']==result['rebuilt'], 'live and rebuilt projection differ'
    if not restart:
        for name,status in [('committed','Committed'),('rejected','Rejected')]:
            assert (result['stepCounts'].get(status,0) if summary else sum(s['status']==status for s in result['steps']))==expected[name],name
        assert (result['conflicts'] if summary else sum(s['conflicts'] for s in result['steps']))==expected['conflicts']
        errors=Counter(result['errors']) if summary else Counter(e for s in result['steps'] for e in s['errors'])
        n=(expected['passed']+expected['failed']);r=max(1,n//100)
        assert errors==Counter(InvalidId=n//10,InvalidCapacity=n//10,PrerequisiteMissing=3*n//10,ClassFull=r),errors
    for name in ['passed','failed']:
        count=sum(e['progress']['$type']==name.title() for e in state['enrollments'])
        assert count==expected[name], (name,count,expected[name])
    return hashlib.sha256(json.dumps(state,ensure_ascii=False,sort_keys=True,separators=(',',':')).encode()).hexdigest()
