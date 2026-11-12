#!/usr/bin/env python3
"""Build native HTTP hosts; school and DCB sources stay in Level3."""
import argparse,importlib.util,json,os,shutil,subprocess,sys
from pathlib import Path
HERE=Path(__file__).resolve().parent
LEVEL3=HERE.parent/'decider_level3'
BUILD=HERE/'.build'
spec=importlib.util.spec_from_file_location('level3_builder',LEVEL3/'run.py')
l3=importlib.util.module_from_spec(spec);spec.loader.exec_module(l3)
LANGUAGES=l3.LANGUAGES

def call(command,**kwargs):
    subprocess.run(list(map(str,command)),check=True,**kwargs)

def build(language):
    BUILD.mkdir(exist_ok=True)
    if language in ['csharp','fsharp']:
        out=BUILD/language
        call(['dotnet','build',HERE/language/('Api.csproj'if language=='csharp'else'Api.fsproj'),'-c','Release','-o',out])
        return ['dotnet',str(out/'Api.dll')]
    if language=='typescript':
        call(['npm','ci','--ignore-scripts','--no-audit','--no-fund'],cwd=LEVEL3/'typescript')
        return ['node',str(HERE/'typescript/server.ts')]
    if language=='go':
        out=BUILD/'go-api';call(['go','mod','tidy'],cwd=HERE/'go');call(['go','build','-o',out,'.'],cwd=HERE/'go',env={**os.environ,'GOCACHE':str(BUILD/'go-cache')})
        return [str(out)]
    if language=='java':
        java_command=l3.build('java') # Reuse the exact pinned drivers, outside timing.
        cp=os.pathsep.join(part for part in java_command[2].split(os.pathsep) if part.endswith('.jar'))
        out=BUILD/'java';out.mkdir(exist_ok=True)
        files=[LEVEL3/'java'/n for n in ['Models.java','ValueObjects.java','Decider.java','Projection.java','SchoolProjection.java','Routing.java','SchoolHandler.java']]
        call(['javac','--release',os.environ.get('DECIDER_JAVA_RELEASE','21'),'--class-path',cp,'-d',out,*files,*sorted((LEVEL3/'java/playground').rglob('*.java')),HERE/'java/Server.java'])
        return ['java','--class-path',str(out)+os.pathsep+cp,'Server']
    if language=='rust':
        call(['cargo','build','--release','--manifest-path',HERE/'rust/Cargo.toml','--target-dir',BUILD/'rust'])
        return [str(BUILD/'rust/release/dcb-school-http')]
    if language=='swift':
        stage=BUILD/'swift-package';stage.mkdir(exist_ok=True)
        shutil.copyfile(HERE/'swift/Package.swift',stage/'Package.swift')
        if (HERE/'swift/Package.resolved').exists():shutil.copyfile(HERE/'swift/Package.resolved',stage/'Package.resolved')
        source=stage/'Sources/Server';source.mkdir(parents=True,exist_ok=True)
        if sys.platform=='linux':shutil.copytree(LEVEL3/'swift/sqlite-module',stage/'Sources/SQLite3',dirs_exist_ok=True)
        for old in source.iterdir():old.unlink()
        names=['dcb.swift','JsonStore.swift','models.swift','valueObjects.swift','decider.swift','projection.swift','schoolProjection.swift','routing.swift','handlers.swift','wire.swift']
        for name in names:(source/name).symlink_to(LEVEL3/'swift'/name)
        for file in (HERE/'swift').glob('*.swift'):
            if file.name!='Package.swift':(source/file.name).symlink_to(file)
        call(['swift','build','-c','release','--package-path',stage])
        shutil.copyfile(stage/'Package.resolved',HERE/'swift/Package.resolved')
        return [str(stage/'.build/release/Server')]
    raise ValueError(language)

if __name__=='__main__':
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('language',choices=LANGUAGES);p.add_argument('--port',type=int,default=8080);p.add_argument('--sqlite',type=Path);p.add_argument('--build-only',action='store_true');a=p.parse_args()
    command=build(a.language);(BUILD/f'{a.language}-command.json').write_text(json.dumps(command))
    if not a.build_only:
        env={**os.environ,'API_SPEC':str(HERE/'openapi.json'),'PORT':str(a.port),'DOTNET_TieredCompilation':'1','DOTNET_TieredPGO':'1'}
        if a.sqlite:env['DECIDER_PERSISTENT']=str(a.sqlite.resolve())
        else:env.pop('DECIDER_PERSISTENT',None)
        os.execvpe(command[0],command,env)
