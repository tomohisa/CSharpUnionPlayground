#!/usr/bin/env python3
"""Measure live-index update/growth separately from full-list materialization."""
import argparse
import hashlib
import json
import platform
import statistics
import subprocess
import sys
from datetime import datetime,timezone
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
from run import HERE,REPO,LANGUAGES,build,run

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--language',default='all')
    parser.add_argument('--sizes',default='1000,10000,100000')
    parser.add_argument('--samples',type=int,default=3)
    parser.add_argument('--native-aot',action='store_true')
    parser.add_argument('--write',action='store_true')
    args=parser.parse_args()
    languages=LANGUAGES if args.language=='all' else args.language.split(',')
    sizes=[int(s) for s in args.sizes.split(',')]
    if args.samples<1 or any(n<3 for n in sizes):parser.error('samples >= 1 and sizes >= 3 required')
    measurements=[]
    for language in languages:
        if language not in LANGUAGES:parser.error('unknown language')
        command=build(language,args.native_aot)
        work=None
        for size in sizes:
            samples=[]
            for trial in range(args.samples):
                result=run([*command,'--projection-scale',size],cwd=REPO)
                row=json.loads(result.stdout)
                assert row['size']==size and row['rows']==4*(size+row['updates'])+1,row
                current=(row['updateWork']/row['updates'],row['growthWork']/row['updates'])
                assert current[0]==3 and current[1]<=10,row
                if work is None:work=current
                assert current==work,(language,size,'work grew with projection size',current,work)
                samples.append(row)
            median={key:statistics.median(row[key] for row in samples) for key in samples[0]}
            measurements.append({'language':language,'size':size,'median':median,'samples':samples})
            print(f"{language} size={size:,}: update={median['updateSeconds']:.6f}s, grow={median['growthSeconds']:.6f}s, full snapshot={median['snapshotSeconds']:.6f}s; work/event={current[0]:g}, work/enrollment pair={current[1]:g}",flush=True)
    if args.write:
        sources={str(p.relative_to(HERE)):hashlib.sha256(p.read_bytes()).hexdigest() for p in HERE.rglob('*') if p.is_file() and p.suffix in ['.cs','.fs','.rs','.ts','.java','.go','.swift'] and not set(p.relative_to(HERE).parts)&{'.build','node_modules','target','bin','obj'}}
        report={'createdUtc':datetime.now(timezone.utc).isoformat(),'platform':platform.platform(),'machine':platform.machine(),'nativeAot':args.native_aot,'samples':args.samples,'sizes':sizes,'sourceSha256':sources,'measurements':measurements}
        out=HERE/'observed';out.mkdir(exist_ok=True)
        (out/'projection-scaling.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
        lines=['# Level 3 live projection scaling','','Local single-process observations; timings are not language rankings.','Updates exclude command handling, store reads, JSON and full-list queries. Each enrollment pair is one StudentRegistered plus one StudentEnrolled.','Each process also checks rejected-batch rollback, gaps, redelivery, old snapshots, and full-list correctness.','','| language | existing students / roster | attendance updates (1,000), ms | add 1,000 students + enrollments, ms | full snapshot, ms | update work/event | growth work/pair |','|---|---:|---:|---:|---:|---:|---:|']
        for row in measurements:
            m=row['median'];lines.append(f"| {row['language']} | {row['size']:,} | {m['updateSeconds']*1000:.3f} | {m['growthSeconds']*1000:.3f} | {m['snapshotSeconds']*1000:.3f} | {m['updateWork']/m['updates']:g} | {m['growthWork']/m['updates']:g} |")
        lines+=['','Hash table resizing, allocation, GC, caches and JIT can change latency. Bounded index operations are an algorithmic check, not a hard real-time guarantee. Full snapshots intentionally scale with output size.','See [raw data](projection-scaling.json) for samples, environment and source fingerprints.']
        (out/'projection-scaling.md').write_text('\n'.join(lines)+'\n')
if __name__=='__main__':
    try:main()
    except subprocess.CalledProcessError as error:
        print(error.stdout or '',file=sys.stderr);print(error.stderr or '',file=sys.stderr);raise SystemExit(error.returncode or 1)
