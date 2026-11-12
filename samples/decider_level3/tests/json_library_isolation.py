#!/usr/bin/env python3
"""Compile JSON journals with only an unrelated document workload, no school model."""
import argparse
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
import run as runner
HERE=runner.HERE;BUILD=HERE/'.build/json-library-isolation'

def run(command,**kw):
    result=subprocess.run(list(map(str,command)),capture_output=True,text=True,timeout=300,**kw)
    if result.returncode:raise RuntimeError(result.stdout+'\n'+result.stderr)
    return result


def verify(language):
    root=BUILD/language
    if root.exists():shutil.rmtree(root)
    root.mkdir(parents=True)
    def copy(file,dest=None):
        target=root/(dest or Path(file).name);target.parent.mkdir(parents=True,exist_ok=True);shutil.copyfile(HERE/file,target)
    if language=='csharp':
        for f in ['Dcb.cs','JsonStore.cs','SqliteJournal.cs','StorageBenchmark.cs','Decider.csproj']:copy('csharp/'+f)
        (root/'Main.cs').write_text('SchoolDomain.StorageBenchmark.Run();')
        run(['dotnet','publish',root/'Decider.csproj','-c','Release','-o',root/'out']);command=['dotnet',root/'out/Decider.dll']
    elif language=='fsharp':
        for f in ['Dcb.fs','JsonStore.fs','SqliteJournal.fs','StorageBenchmark.fs']:copy('fsharp/'+f)
        (root/'Main.fs').write_text('module Probe\n[<EntryPoint>]\nlet main _ = SchoolDomain.StorageBenchmark.run();0\n')
        project=(HERE/'fsharp/Decider.fsproj').read_text();import re
        project=re.sub(r'<Compile Include="[^"]+"\s*/>','',project)
        project=project.replace('</Project>','<ItemGroup>'+''.join(f'<Compile Include="{f}"/>'for f in ['Dcb.fs','JsonStore.fs','SqliteJournal.fs','StorageBenchmark.fs','Main.fs'])+'</ItemGroup></Project>')
        (root/'Probe.fsproj').write_text(project);run(['dotnet','publish',root/'Probe.fsproj','-c','Release','-o',root/'out']);command=['dotnet',root/'out/Probe.dll']
    elif language=='rust':
        for f in ['dcb.rs','json_store.rs','sqlite_journal.rs','storage_benchmark.rs']:copy('rust/src/'+f,'src/'+f)
        for f in ['Cargo.toml','Cargo.lock']:copy('rust/'+f)
        (root/'src/main.rs').write_text('mod dcb;mod json_store;mod sqlite_journal;mod storage_benchmark;fn main(){storage_benchmark::run();}')
        target=BUILD/'rust-target';run(['cargo','build','--release','--locked','--offline','--manifest-path',root/'Cargo.toml','--target-dir',target]);command=[target/'release/enrollment-decider']
    elif language=='typescript':
        for f in ['dcb.ts','jsonStore.ts','storageBenchmark.ts','package.json']:copy('typescript/'+f)
        (root/'node_modules').symlink_to(HERE/'typescript/node_modules',target_is_directory=True)
        (root/'main.ts').write_text('import {storageBenchmark} from "./storageBenchmark.ts";storageBenchmark();')
        command=['node',root/'main.ts']
    elif language=='java':
        for f in ['playground/dcb/Dcb.java','playground/dcb/JsonStore.java','StorageBenchmark.java']:copy('java/'+f,f)
        (root/'Main.java').write_text('public class Main{public static void main(String[]args){StorageBenchmark.run();}}')
        cp=os.pathsep.join(str(p)for p in sorted((HERE/'.build/jars').glob('*.jar')))
        run(['javac','--release','21','--class-path',cp,'-d',root/'out',*root.rglob('*.java')]);command=['java','--class-path',str(root/'out')+os.pathsep+cp,'Main']
    elif language=='go':
        for f in ['dcb/store.go','dcb/json_store.go','storage_benchmark.go','go.mod','go.sum']:copy('go/'+f,f)
        (root/'main.go').write_text('package main\nfunc main(){storageBenchmark()}')
        run(['go','build','-o',root/'probe','.'],cwd=root,env={**os.environ,'GOCACHE':str(HERE/'.build/go-cache')});command=[root/'probe']
    elif language=='swift':
        for f in ['dcb.swift','JsonStore.swift','storageBenchmark.swift']:copy('swift/'+f)
        (root/'main.swift').write_text('storageBenchmark()')
        run(['swiftc','-O','-module-cache-path',root/'modules',*root.glob('*.swift'),'-o',root/'probe']);command=[root/'probe']
    for path in root.rglob('*'):
        if path.is_file() and path.suffix in ['.cs','.fs','.rs','.ts','.java','.go','.swift'] and not any(p in path.parts for p in ['node_modules','obj','bin','out','modules']):
            if any(word in path.read_text()for word in ['StudentRegistered','ClassOpened','./models.ts','crate::models','dcb-school/domain']):raise AssertionError('school dependency in '+str(path))
    result=run(command,env={**os.environ,'DECIDER_DOCUMENTS':'2','DECIDER_PERSISTENT':str(root/'documents.db')})
    assert any(line.startswith('DOCUMENT ') and line.split()[2:4]==['2','32']for line in result.stderr.splitlines()),result.stderr
    print(language+': JSON library without school model / SQLite document replay PASS',flush=True)

if __name__=='__main__':
    p=argparse.ArgumentParser();p.add_argument('--language',default='all');a=p.parse_args()
    for language in runner.LANGUAGES if a.language=='all' else a.language.split(','):verify(language)
