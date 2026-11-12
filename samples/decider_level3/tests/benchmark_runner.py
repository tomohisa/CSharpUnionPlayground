#!/usr/bin/env python3
"""Exercise calibration reuse through the actual CLI, including mismatch rejection."""
import argparse
import json
from pathlib import Path
import subprocess
import sys
import tempfile

ROOT=Path(__file__).resolve().parents[1]

def main():
    p=argparse.ArgumentParser();p.add_argument('--observations',type=Path,required=True);a=p.parse_args()
    original=json.loads(a.observations.read_text())
    with tempfile.TemporaryDirectory(prefix='level3-calibration-tests-') as folder:
        root=Path(folder)
        def invoke(data):
            source=root/'source.json';source.write_text(json.dumps(data))
            return subprocess.run([sys.executable,str(ROOT/'benchmark/run.py'),'--no-build','--calibrate-only','--calibrate-from',str(source),'--output',str(root/'plan.json')],capture_output=True,text=True,timeout=60)
        good=invoke(original);assert good.returncode==0,good.stderr
        plan=json.loads((root/'plan.json').read_text())
        assert plan['students']>=10 and plan['students']%10==0 and plan['results']==[]
        assert plan['calibration']['method']=='previous-complete-SQLite-run'
        cases=[('platform','other-host','different host/environment'),('workloadSha256','0'*64,'different workload'),('csharpNativeAot',not original['csharpNativeAot'],'different C# build mode')]
        for field,value,message in cases:
            data={**original,field:value};bad=invoke(data)
            assert bad.returncode!=0 and message in bad.stderr,(field,bad.stderr)
        data={**original,'sourceManifest':{**original['sourceManifest'],'csharp/Dcb.cs':'0'*64}}
        bad=invoke(data);assert bad.returncode!=0 and 'source code differs' in bad.stderr,bad.stderr
        data={**original,'runtimeVersions':{**original['runtimeVersions'],'node':'other-runtime'}}
        bad=invoke(data);assert bad.returncode!=0 and 'different runtime/compiler versions' in bad.stderr,bad.stderr
    print('Benchmark calibration CLI: common count / host / workload / source / runtime / build-mode checks PASS')

if __name__=='__main__':main()
