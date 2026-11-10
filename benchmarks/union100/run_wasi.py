#!/usr/bin/env python3
"""Compare the existing 100-case JSON workload on native and WASI backends."""
import argparse
import datetime
import hashlib
import json
import os
from pathlib import Path
import platform
import shutil
import statistics
import subprocess
import sys

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE))
import run as harness
sys.path.insert(0, str(HERE.parent))
from wasi_support import build_csharp

NAMES = [n for n in harness.NAMES if n.startswith('csharp-') or n == 'rust']


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--implementations', default=','.join(NAMES))
    parser.add_argument('--engines', default='native,wasi')
    parser.add_argument('--roots', type=int, default=200000)
    parser.add_argument('--samples', type=int, default=5)
    parser.add_argument('--timeout', type=float, default=1800)
    parser.add_argument('--quick', action='store_true')
    parser.add_argument('--skip-build', action='store_true')
    parser.add_argument('--build-dir', type=Path, default=HERE / '.build/wasi-comparison')
    parser.add_argument('--output', type=Path, default=HERE / 'results/wasi-latest.json')
    parser.add_argument('--wasi-dotnet', type=Path, default=HERE.parent / 'projection100/.build/dotnet/dotnet')
    parser.add_argument('--wasi-sdk', type=Path, default=HERE.parent / 'projection100/.build/tools/wasi-sdk-33.0-arm64-macos')
    args = parser.parse_args()
    names, engines = args.implementations.split(','), args.engines.split(',')
    if args.quick:
        args.roots, args.samples = 100, 1
    if any(n not in NAMES for n in names) or any(e not in ['native', 'wasi'] for e in engines) or args.roots < 100 or args.samples < 1 or args.timeout <= 0:
        parser.error('Invalid implementation, engine or count')
    build_dir = args.build_dir.resolve()
    build_dir.mkdir(parents=True, exist_ok=True)
    rid = ('osx' if sys.platform == 'darwin' else 'linux') + '-' + ('arm64' if platform.machine() in ['arm64', 'aarch64'] else 'x64')
    fingerprint = hashlib.sha256()
    paths = [HERE / 'run.py', HERE / 'run_wasi.py', HERE / 'generate.py', HERE / 'Cargo.lock', HERE.parent / 'wasi_support.py', harness.REPO / 'global.json', *list((HERE / 'templates').iterdir()), *list((harness.REPO / 'src/PlaygroundTaggedJson.Generator').glob('*.cs'))]
    for path in sorted(paths):
        fingerprint.update(path.read_bytes())
    key = {'source': fingerprint.hexdigest(), 'names': names, 'engines': engines, 'rid': rid,
           'dotnet': str(args.wasi_dotnet.resolve()), 'wasiSdk': str(args.wasi_sdk.resolve())}
    cache = build_dir / 'commands.json'
    if args.skip_build:
        saved = json.loads(cache.read_text())
        if saved['key'] != key:
            raise RuntimeError('Build configuration changed; run without --skip-build.')
        commands, errors = saved['commands'], saved['errors']
    else:
        commands, errors = {}, {}
        sources = build_dir / 'sources'
        harness.generate(sources, harness.REPO)
        if 'native' in engines:
            for name in names:
                if name == 'rust':
                    continue
                command = harness.build([name], build_dir, rid, native_properties={'OptimizationPreference':'Speed','IlcInstructionSet':'native'})[name]
                commands[name + '-native'] = command
        cargo = ['rustup', 'run', 'stable', 'cargo'] if shutil.which('rustup') else ['cargo']
        for name in names:
            for engine in engines:
                full_name = name + '-' + engine
                if name == 'rust':
                    command = cargo + ['build', '--release', '--locked', '--manifest-path', str(sources / 'rust/Cargo.toml')]
                    if engine == 'wasi':
                        command += ['--target', 'wasm32-wasip1']
                    rust_env = dict(os.environ)
                    if shutil.which('rustup'):
                        rust_env['RUSTC'] = harness.run(['rustup', 'which', '--toolchain', 'stable', 'rustc']).stdout.strip()
                    rust_env.update(CARGO_PROFILE_RELEASE_OPT_LEVEL='3', CARGO_PROFILE_RELEASE_LTO='fat', CARGO_PROFILE_RELEASE_CODEGEN_UNITS='1')
                    rust_env['RUSTFLAGS'] = '-C target-feature=+simd128' if engine == 'wasi' else '-C target-cpu=native'
                    subprocess.run(command, cwd=harness.REPO, env=rust_env, capture_output=True, text=True, check=True, timeout=1800)
                    binary = sources / ('rust/target/wasm32-wasip1/release/union100.wasm' if engine == 'wasi' else 'rust/target/release/union100')
                    commands[full_name] = (['wasmtime', 'run', '-O', 'opt-level=2'] if engine == 'wasi' else []) + [str(binary)]
                elif engine == 'wasi':
                    try:
                        commands[full_name] = build_csharp(sources / name / 'Benchmark.csproj',
                            build_dir / 'wasi' / name, args.wasi_dotnet, args.wasi_sdk)
                    except (RuntimeError, subprocess.CalledProcessError) as error:
                        errors[full_name] = {'status': 'build-error', 'error': str(error), 'stderr': getattr(error, 'stderr', None), 'stdout': getattr(error, 'stdout', None)}
        cache.write_text(json.dumps({'key': key, 'commands': commands, 'errors': errors}, indent=2))
    results = {}
    versions = {}
    for name, command in {'dotnet': ['dotnet', '--version'], 'wasi-dotnet': [str(args.wasi_dotnet.resolve()), '--version'],
                          'rust': ['rustup', 'run', 'stable', 'rustc', '--version'], 'wasmtime': ['wasmtime', '--version']}.items():
        result = harness.run(command)
        versions[name] = (result.stdout + result.stderr).strip()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    proofs = {name: json.loads((build_dir/'wasi'/name.removesuffix('-wasi')/'build-proof.json').read_text()) for name in commands if name.startswith('csharp-') and name.endswith('-wasi')}
    report = {'buildProofs': proofs, 'optimization': {'csharpNative':'Native AOT Speed, native ISA','csharpWasi':'LLVMOnly AOT, SIMD, clang -O3','rust':'opt-level=3, fat LTO, codegen-units=1, host native ISA / Wasm SIMD','wasmtime':'opt-level=2'}, 'timestampUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'platform': platform.platform(),
              'machine': platform.machine(), 'buildKey': key, 'roots': args.roots, 'nodesPerBatch': args.roots * 3,
              'samples': args.samples, 'toolchains': versions, 'commands': commands, 'results': results, 'errors': errors,
              'summary': [], 'notes': ['Native and WASI use the same generated Program.cs/main.rs and payloads, warm-up, validation and timed phases.',
              'C# native is Native AOT Speed/native ISA; C# WASI is LLVMOnly AOT with SIMD and clang -O3, no interpreter fallback. Rust uses optimized release/LTO with host native ISA or Wasm SIMD.',
              'WASI peak RSS includes Wasmtime compilation/runtime and is not guest heap usage. Startup includes Wasmtime compilation.',
              'C# union wire envelopes differ from polymorphic record/Rust formats, as in the existing benchmark.']}
    if engines==['wasi']:
        report['notes'].insert(0,'WASM-only measurements. Native results belong to the separate host report; combine only when job/attempt metadata matches. C# WASM uses .NET 11 AOT even when the Native abstract implementation uses .NET 10 JIT.')
    def save():
        args.output.write_text(json.dumps(report, indent=2) + '\n')
    save()
    for name, command in commands.items():
        try:
            results[name] = harness.measure(name, commands, args.roots, args.samples, args.timeout,
                                           args.output.parent / (args.output.stem + '.progress'))
            result = results[name]
            entries = result['measurements']
            report['summary'].append({'implementation': name, 'medianSeconds': statistics.median(e['totalSeconds'] for e in entries),
                'encodeMedianSeconds': statistics.median(e['encodeSeconds'] for e in entries),
                'decodeMedianSeconds': statistics.median(e['decodeSeconds'] for e in entries),
                'switchMedianSeconds': statistics.median(e['switchSeconds'] for e in entries),
                'aggregateSeconds': sum(e['totalSeconds'] for e in entries), 'bytes': entries[0]['bytes'],
                'peakRssMiB': result['peakRssBytes'] / 1048576,
                **{k: result[k] for k in ['startupSeconds', 'preparationSeconds', 'exitSeconds', 'wallSeconds']}})
        except (RuntimeError, subprocess.SubprocessError) as error:
            errors[name] = {'status': 'timeout' if isinstance(error, subprocess.TimeoutExpired) else 'runtime-error',
                           'error': str(error), 'stderr': getattr(error, 'stderr', None), 'stdout': getattr(error, 'stdout', None)}
        save()
    lines = ['| implementation | batch s | encode s | decode s | switch s | JSON MiB | RSS MiB |', '|---|---:|---:|---:|---:|---:|---:|']
    for row in report['summary']:
        lines.append(f"| {row['implementation']} | {row['medianSeconds']:.4f} | {row['encodeMedianSeconds']:.4f} | {row['decodeMedianSeconds']:.4f} | {row['switchMedianSeconds']:.4f} | {row['bytes']/1048576:.2f} | {row['peakRssMiB']:.1f} |")
    lines += ['', '| implementation | startup s | preparation s | exit s | wall s |', '|---|---:|---:|---:|---:|']
    for row in report['summary']:
        lines.append(f"| {row['implementation']} | {row['startupSeconds']:.4f} | {row['preparationSeconds']:.4f} | {row['exitSeconds']:.4f} | {row['wallSeconds']:.3f} |")
    if errors:
        lines += ['', '## Unsuccessful conditions', '']
        lines += [f"- {name}: {error['status']}. See JSON error details." for name, error in errors.items()]
    lines += ['', *report['notes']]
    args.output.with_suffix('.md').write_text('\n'.join(lines) + '\n')
    print('\n'.join(lines))


if __name__ == '__main__':
    try:
        main()
    except subprocess.CalledProcessError as error:
        print(error.stdout or '', file=sys.stderr)
        print(error.stderr or '', file=sys.stderr)
        raise SystemExit(error.returncode)
