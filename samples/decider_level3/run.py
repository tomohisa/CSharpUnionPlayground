#!/usr/bin/env python3
"""Build the same classroom decider in seven languages and run shared JSON scenarios."""
import argparse
import json
import os
from pathlib import Path
import subprocess
import sys

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[1]
BUILD = HERE / '.build'
LANGUAGES = ['csharp', 'fsharp', 'rust', 'typescript', 'java', 'go', 'swift']


def run(command, **kwargs):
    return subprocess.run([str(x) for x in command], text=True, capture_output=True,
                          check=True, timeout=300, **kwargs)


def build(language, native_aot=False):
    BUILD.mkdir(exist_ok=True)
    if language == 'csharp':
        out = BUILD / ('csharp-aot' if native_aot else 'csharp')
        args = ['dotnet', 'publish', HERE/'csharp/Decider.csproj', '-c', 'Release', '-o', out]
        if native_aot:
            import platform
            rid = ('osx-' if sys.platform == 'darwin' else 'linux-') + ('arm64' if platform.machine() in ['arm64','aarch64'] else 'x64')
            args += ['-r', rid, '-p:PublishAot=true']
        run(args, cwd=REPO)
        return ['env', 'DECIDER_REQUIRE_NATIVE=1', str(out/'Decider')] if native_aot else ['dotnet', str(out/'Decider.dll')]
    if language == 'fsharp':
        out = BUILD / 'fsharp'
        run(['dotnet', 'publish', HERE/'fsharp/Decider.fsproj', '-c', 'Release', '-o', out], cwd=REPO)
        return ['env', 'DOTNET_TieredCompilation=1', 'DOTNET_TieredPGO=1', 'dotnet', str(out/'Decider.dll')]
    if language == 'rust':
        run(['cargo', 'build', '--release', '--manifest-path', HERE/'rust/Cargo.toml',
             '--target-dir', BUILD/'rust', *(['--locked'] if (HERE/'rust/Cargo.lock').exists() else [])], cwd=REPO)
        return [str(BUILD/'rust/release/enrollment-decider')]
    if language == 'typescript':
        run(['npm', 'ci', '--ignore-scripts', '--no-audit', '--no-fund'], cwd=HERE/'typescript')
        return ['node', str(HERE/'typescript/main.ts')]
    if language == 'java':
        sys.path.insert(0, str(REPO/'scripts'))
        from run_payment_java import jackson_classpath
        jars = BUILD/'jars'
        jars.mkdir(exist_ok=True)
        cached = sorted(jars.glob('jackson-*.jar'))
        cp = os.pathsep.join(map(str,cached)) if len(cached)==3 else jackson_classpath(jars)
        import urllib.request
        for artifact,version,group in [('sqlite-jdbc','3.54.1.0','org/xerial'),('slf4j-api','2.0.17','org/slf4j'),('slf4j-nop','2.0.17','org/slf4j')]:
            jar=jars/f'{artifact}-{version}.jar'
            if not jar.exists():
                urllib.request.urlretrieve(f'https://repo.maven.apache.org/maven2/{group}/{artifact}/{version}/{jar.name}',jar)
        cp += os.pathsep+os.pathsep.join(str(p) for p in sorted(jars.glob('*.jar')) if not p.name.startswith('jackson-'))
        out = BUILD/'java'
        out.mkdir(exist_ok=True)
        run(['javac', '--release', '21', '--class-path', cp, '-d', out, *sorted((HERE/'java').rglob('*.java'))])
        return ['java', '--class-path', str(out)+os.pathsep+cp, 'Main']
    if language == 'go':
        out = BUILD/'go-decider'
        run(['go', 'build', '-o', out, '.'], cwd=HERE/'go', env={**os.environ,'GOCACHE':os.environ.get('GOCACHE',str(BUILD/'go-cache'))})
        return [str(out)]
    if language == 'swift':
        out = BUILD/'swift-decider'
        run(['swiftc', '-O', '-module-cache-path', BUILD/'swift-modules', *sorted((HERE/'swift').glob('*.swift')), '-o', out])
        return [str(out)]
    raise ValueError(language)


def canonical(value, field=''):
    # .NET 11 writes F# fieldless DUs as strings; object form is also accepted.
    if isinstance(value, str) and field in ['prerequisite', 'progress'] and value in ['NoPrerequisite', 'Reserved']:
        return {'$type': value}
    if isinstance(value, dict): return {k: canonical(v, k) for k, v in value.items()}
    if isinstance(value, list): return [canonical(v, field) for v in value]
    return value


def execute(command, payload, persistent=None):
    env=dict(os.environ)
    if persistent is not None:env['DECIDER_PERSISTENT']=str(Path(persistent).resolve())
    return run(command, input=json.dumps(payload, ensure_ascii=False), cwd=REPO,env=env)


def no_null(value):
    if value is None: raise AssertionError('domain output contains null')
    if isinstance(value, dict):
        for v in value.values(): no_null(v)
    if isinstance(value, list):
        for v in value: no_null(v)



def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--language', default='all', help='all or comma-separated language names')
    parser.add_argument('--scenario',default='lifecycle')
    parser.add_argument('--verify', action='store_true')
    parser.add_argument('--native-aot', action='store_true', help='Publish C# as Native AOT')
    parser.add_argument('--persistent',type=Path,help='SQLite database file (one language, normal scenario mode)')
    args = parser.parse_args()
    languages = LANGUAGES if args.language == 'all' else args.language.split(',')
    if any(name not in LANGUAGES for name in languages):
        parser.error('Unknown language')
    if args.persistent and (args.verify or len(languages)!=1):parser.error('--persistent requires one language and scenario mode')
    if args.verify:
        from verify import verify_all
        commands = {}
        for language in languages:
            print('Building '+language, file=sys.stderr, flush=True)
            commands[language] = build(language, args.native_aot)
        verify_all(commands)
    else:
        payload=json.loads((HERE/'fixtures'/f'{args.scenario}.json').read_text())
        for language in languages:
            command=build(language,args.native_aot)
            print(language+':')
            print(json.dumps(json.loads(execute(command,payload,args.persistent).stdout),indent=2,ensure_ascii=False))


if __name__ == '__main__':
    try: main()
    except subprocess.CalledProcessError as error:
        print(error.stdout or '',file=sys.stderr);print(error.stderr or '',file=sys.stderr)
        raise SystemExit(error.returncode or 1)
