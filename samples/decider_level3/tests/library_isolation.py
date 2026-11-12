#!/usr/bin/env python3
"""Compile reusable DCB code + an unrelated counter, without school model sources."""
from pathlib import Path
import argparse
import html
import json
import os
import shutil
import subprocess
import sys
HERE=Path(__file__).resolve().parents[1]
REPO=HERE.parents[1]
BUILD=HERE/'.build/library-isolation'
LANGUAGES=['csharp','fsharp','rust','typescript','java','go','swift']
LIBRARIES={'csharp':'csharp/Dcb.cs','fsharp':'fsharp/Dcb.fs','rust':'rust/src/dcb.rs','typescript':'typescript/dcb.ts','java':'java/playground/dcb/Dcb.java','go':'go/dcb/store.go','swift':'swift/dcb.swift'}
def run(args,**kwargs):
    result=subprocess.run(list(map(str,args)),text=True,capture_output=True,timeout=300,**kwargs)
    if result.returncode:raise RuntimeError(' '.join(map(str,args))+'\n'+result.stdout+'\n'+result.stderr)
    return result

def verify(language):
    root=BUILD/language;root.mkdir(parents=True,exist_ok=True)
    text=(HERE/LIBRARIES[language]).read_text()
    forbidden=['SchoolDomain','crate::models','./models.ts','dcb-school/domain','StudentRegistered','ClassOpened','EnrollmentRegister']
    if any(word in text for word in forbidden):raise AssertionError(language+': reusable library depends on school')
    if language=='csharp':
        for file in ['Dcb.cs','LibraryTests.cs']:shutil.copyfile(HERE/language/file,root/file)
        (root/'Main.cs').write_text('LibraryTests.Run();System.Console.WriteLine("PASS");\n')
        (root/'Probe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net11.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup></Project>')
        run(['dotnet','build',root/'Probe.csproj','-c','Release'],cwd=REPO);run(['dotnet',root/'bin/Release/net11.0/Probe.dll'])
    elif language=='fsharp':
        for file in ['Dcb.fs','LibraryTests.fs']:shutil.copyfile(HERE/language/file,root/file)
        (root/'Main.fs').write_text('module Probe\nopen Playground.Dcb\n[<EntryPoint>]\nlet main _ = LibraryTests.run();printfn "PASS";0\n')
        (root/'Probe.fsproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net11.0</TargetFramework><Nullable>enable</Nullable><WarningsAsErrors>25;3261</WarningsAsErrors></PropertyGroup><ItemGroup><Compile Include="Dcb.fs"/><Compile Include="LibraryTests.fs"/><Compile Include="Main.fs"/></ItemGroup></Project>')
        run(['dotnet','build',root/'Probe.fsproj','-c','Release'],cwd=REPO);run(['dotnet',root/'bin/Release/net11.0/Probe.dll'],env={**os.environ,'DOTNET_TieredPGO':'1','DOTNET_TieredCompilation':'1'})
    elif language=='rust':
        (root/'src').mkdir(exist_ok=True)
        for file in ['dcb.rs','dcb_tests.rs']:shutil.copyfile(HERE/'rust/src'/file,root/'src'/file)
        (root/'src/main.rs').write_text('mod dcb;mod dcb_tests;fn main(){dcb_tests::run();println!("PASS");}\n')
        for file in ['Cargo.toml','Cargo.lock']:(root/file).write_text((HERE/'rust'/file).read_text().replace('enrollment-decider','dcb-isolation-probe'))
        run(['cargo','run','--release','--locked','--offline','--manifest-path',root/'Cargo.toml','--target-dir',BUILD/'rust-target'],cwd=REPO)
    elif language=='typescript':
        for file in ['dcb.ts','dcb.test.ts']:shutil.copyfile(HERE/language/file,root/file)
        (root/'package.json').write_text('{"type":"module","private":true}')
        (root/'tsconfig.json').write_text(json.dumps({'compilerOptions':{'strict':True,'noEmit':True,'target':'ES2022','module':'NodeNext','allowImportingTsExtensions':True,'types':['node'],'typeRoots':[str(REPO/'samples/file-based/node_modules/@types')]},'include':['*.ts']}))
        run([REPO/'samples/file-based/node_modules/.bin/tsc','-p',root/'tsconfig.json']);run(['node','--test',root/'dcb.test.ts'])
    elif language=='java':
        shutil.copyfile(HERE/'java/playground/dcb/Dcb.java',root/'Dcb.java')
        shutil.copyfile(HERE/'java/DcbTests.java',root/'DcbTests.java')
        (root/'Probe.java').write_text('public class Probe{public static void main(String[]args)throws Exception{DcbTests.run();System.out.println("PASS");}}\n')
        run(['javac','--release','21','-d',root,root/'Dcb.java',root/'DcbTests.java',root/'Probe.java']);run(['java','--class-path',root,'Probe'])
    elif language=='go':
        # Only this package and the standard library are compiled; -race checks real goroutines.
        run(['go','test','-race','./dcb'],cwd=HERE/'go',env={**os.environ,'GOCACHE':os.environ.get('GOCACHE',str(HERE/'.build/go-cache'))})
        dependencies=run(['go','list','-deps','./dcb'],cwd=HERE/'go',env={**os.environ,'GOCACHE':os.environ.get('GOCACHE',str(HERE/'.build/go-cache'))}).stdout
        if 'dcb-school/domain' in dependencies:raise AssertionError('Go library imports school')
    elif language=='swift':
        shutil.copyfile(HERE/'swift/dcb.swift',root/'dcb.swift')
        (root/'libraryTests.swift').write_text('import PlaygroundDcb\n'+(HERE/'swift/libraryTests.swift').read_text())
        (root/'main.swift').write_text('libraryTests();print("PASS")\n')
        run(['swiftc','-O','-module-cache-path',root/'modules','-emit-library','-emit-module','-module-name','PlaygroundDcb',root/'dcb.swift','-o',root/('libPlaygroundDcb.dylib' if sys.platform=='darwin' else 'libPlaygroundDcb.so'),'-emit-module-path',root/'PlaygroundDcb.swiftmodule'])
        run(['swiftc','-O','-module-cache-path',root/'modules','-I',root,'-L',root,'-lPlaygroundDcb',root/'libraryTests.swift',root/'main.swift','-o',root/'probe','-Xlinker','-rpath','-Xlinker',root]);run([root/'probe'])
    print(language+': standalone counter + library isolation + conditional race PASS',flush=True)

def main():
    parser=argparse.ArgumentParser(description=__doc__);parser.add_argument('--language',default='all');args=parser.parse_args()
    languages=LANGUAGES if args.language=='all' else args.language.split(',')
    for language in languages:
        if language not in LANGUAGES:parser.error('unknown language')
        verify(language)
if __name__=='__main__':main()
