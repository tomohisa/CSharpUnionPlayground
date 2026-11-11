#!/usr/bin/env python3
"""Build the same classroom decider in six languages and run shared JSON scenarios."""
import argparse
import json
import os
from pathlib import Path
import subprocess
import sys

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[1]
BUILD = HERE / '.build'
LANGUAGES = ['csharp', 'rust', 'typescript', 'java', 'go', 'swift']


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
    if language == 'rust':
        run(['cargo', 'build', '--release', '--manifest-path', HERE/'rust/Cargo.toml',
             '--target-dir', BUILD/'rust', *(['--locked'] if (HERE/'rust/Cargo.lock').exists() else [])], cwd=REPO)
        return [str(BUILD/'rust/release/classroom-decider')]
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


def execute(command, payload):
    return run(command, input=json.dumps(payload, ensure_ascii=False), cwd=REPO)


def verify(command, expected):
    for name, golden in expected.items():
        payload = json.loads((HERE/'fixtures'/f'{name}.json').read_text())
        result = json.loads(execute(command, payload).stdout)
        for key, value in golden.items():
            if result.get(key) != value:
                raise AssertionError(f'{name}/{key}: expected {value!r}, got {result.get(key)!r}')
        if result['restored'] != golden['state'] or result['replayed'] != golden['state']:
            raise AssertionError(name+': snapshot/history round trip failed')
        # Re-encode this implementation's events, restore typed payloads, and continue from the log.
        resumed = json.loads(execute(command, {'commands': [], 'history': result['events']}).stdout)
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
    parser.add_argument('--scenario', default='lifecycle', choices=['empty','lifecycle','validation','resume','unicode','metadata-order'])
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
        for language, command in commands.items():
            verify(command, expected)
            print(f'{language}: PASS ({len(expected)} golden scenarios, {len(expected)} stored-log replays, 7 invalid inputs)', flush=True)
        # Cross-language event-store handoff, with a full -> available -> full transition.
        origin = json.loads((HERE/'fixtures/resume.json').read_text())
        for writer, command in commands.items():
            log = json.loads(execute(command, origin).stdout)['events']
            for reader, target in commands.items():
                payload = {'history': log, 'commands': [{'$type':'Drop','studentId':'carol'}]}
                try:
                    result = json.loads(execute(target, payload).stdout)
                except subprocess.CalledProcessError as error:
                    raise RuntimeError(writer+' -> '+reader+': '+error.stderr) from error
                want = {'$type':'Available','roster':{'name':'Lab','capacity':2,'students':['alice']}}
                if result['state'] != want: raise AssertionError(writer+' -> '+reader)
        print(f'Cross-language log handoff: {len(commands)**2} pair(s) PASS')
    else:
        payload = json.loads((HERE/'fixtures'/f'{args.scenario}.json').read_text())
        for language, command in commands.items():
            print(language+':')
            print(json.dumps(json.loads(execute(command, payload).stdout), indent=2, ensure_ascii=False))


if __name__ == '__main__':
    try:
        main()
    except subprocess.CalledProcessError as error:
        print(error.stdout or '', file=sys.stderr)
        print(error.stderr or '', file=sys.stderr)
        raise SystemExit(error.returncode or 1)
