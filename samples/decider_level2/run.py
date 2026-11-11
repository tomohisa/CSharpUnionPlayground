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
        out = BUILD/'java'
        out.mkdir(exist_ok=True)
        run(['javac', '--release', '21', '--class-path', cp, '-d', out, *sorted((HERE/'java').glob('*.java'))])
        return ['java', '--class-path', str(out)+os.pathsep+cp, 'Main']
    if language == 'go':
        out = BUILD/'go-decider'
        run(['go', 'build', '-o', out, '.'], cwd=HERE/'go')
        return [str(out)]
    if language == 'swift':
        out = BUILD/'swift-decider'
        run(['swiftc', '-O', *sorted((HERE/'swift').glob('*.swift')), '-o', out])
        return [str(out)]
    raise ValueError(language)


def canonical(value, field=''):
    # .NET 11 writes F# fieldless DUs as strings; object form is also accepted.
    if isinstance(value, str) and field in ['prerequisite', 'progress'] and value in ['NoPrerequisite', 'Reserved']:
        return {'$type': value}
    if isinstance(value, dict): return {k: canonical(v, k) for k, v in value.items()}
    if isinstance(value, list): return [canonical(v, field) for v in value]
    return value


def execute(command, payload):
    return run(command, input=json.dumps(payload, ensure_ascii=False), cwd=REPO)


def no_null(value):
    if value is None: raise AssertionError('domain output contains null')
    if isinstance(value, dict):
        for v in value.values(): no_null(v)
    if isinstance(value, list):
        for v in value: no_null(v)


def verify(command, expected):
    for name, golden in expected.items():
        payload = json.loads((HERE/'fixtures'/f'{name}.json').read_text())
        result = canonical(json.loads(execute(command, payload).stdout))
        no_null(result)
        for key, value in golden.items():
            if result.get(key) != value:
                raise AssertionError(f'{name}/{key}: expected {value!r}, got {result.get(key)!r}')
        if result['restored'] != golden['state'] or result['replayed'] != golden['state']:
            raise AssertionError(name+': snapshot/history round trip failed')
        # Re-encode this implementation's events, restore typed payloads, and continue from the log.
        resumed = canonical(json.loads(execute(command, {'commands': [], 'history': result['events']}).stdout))
        if resumed['state'] != golden['state']:
            raise AssertionError(name+': stored log replay failed')
    for path in sorted((HERE/'fixtures/invalid').glob('*.json')):
        result = subprocess.run(command, input=path.read_text(), text=True,
                                capture_output=True, cwd=REPO, timeout=60)
        if result.returncode == 0:
            raise AssertionError(path.name+': invalid history/tag accepted')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--language', default='all', help='all or comma-separated language names')
    parser.add_argument('--scenario', default='lifecycle', choices=['empty','lifecycle','validation','resume','unicode','metadata-order','withdraw','available','mixed-results'])
    parser.add_argument('--verify', action='store_true')
    parser.add_argument('--native-aot', action='store_true', help='Publish C# as Native AOT')
    args = parser.parse_args()
    languages = LANGUAGES if args.language == 'all' else args.language.split(',')
    if any(name not in LANGUAGES for name in languages):
        parser.error('Unknown language')
    expected = json.loads((HERE/'fixtures/expected.json').read_text())
    commands = {}
    for language in languages:
        print('Building '+language, file=sys.stderr, flush=True)
        commands[language] = build(language, args.native_aot)
    if args.verify:
        invalid_count = len(list((HERE/'fixtures/invalid').glob('*.json')))
        for language, command in commands.items():
            verify(command, expected)
            print(f'{language}: PASS ({len(expected)} golden scenarios, {len(expected)} stored-log replays, {invalid_count} invalid inputs)', flush=True)
        # Every producer's actual event JSON becomes every reader's input.
        origin = json.loads((HERE/'fixtures/lifecycle.json').read_text())
        for writer, command in commands.items():
            log = json.loads(execute(command, origin).stdout)['events']
            for reader, target in commands.items():
                payload = {'history': log, 'commands': [{'$type':'OpenClass','classId':'b2','course':'Advanced','capacity':1},
                                                       {'$type':'Enroll','classId':'b2','studentId':'bob'}]}
                try:
                    result = canonical(json.loads(execute(target, payload).stdout))
                except subprocess.CalledProcessError as error:
                    raise RuntimeError(writer+' -> '+reader+': '+error.stderr) from error
                if result['state'] != expected['resume']['state']: raise AssertionError(writer+' -> '+reader)
        print(f'Cross-language log handoff: {len(commands)**2} pair(s) PASS')
    else:
        payload = json.loads((HERE/'fixtures'/f'{args.scenario}.json').read_text())
        for language, command in commands.items():
            print(language+':')
            print(json.dumps(canonical(json.loads(execute(command, payload).stdout)), indent=2, ensure_ascii=False))


if __name__ == '__main__':
    try:
        main()
    except subprocess.CalledProcessError as error:
        print(error.stdout or '', file=sys.stderr)
        print(error.stderr or '', file=sys.stderr)
        raise SystemExit(error.returncode or 1)
