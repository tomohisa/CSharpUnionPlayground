#!/usr/bin/env python3
"""Isolate metadata binding from direct UTF8 binding, with unchanged model/wire/score."""
import argparse
import datetime
import hashlib
import json
import os
import platform
from pathlib import Path
import statistics
import subprocess
import sys

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE.parent))
import generate
import run as harness
sys.path.insert(0, str(HERE.parents[1]))
from wasi_support import build_csharp


def replace_once(code, old, new):
    if code.count(old) != 1:
        raise RuntimeError(f'Benchmark template changed: expected one occurrence of {old!r}')
    return code.replace(old, new)


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--roots', type=int, default=200000)
    p.add_argument('--samples', type=int, default=5)
    p.add_argument('--skip-build', action='store_true')
    sdk = harness.REPO / 'benchmarks/projection100/.build'
    arch = 'arm64' if platform.machine() in ['arm64', 'aarch64'] else 'x86_64'
    p.add_argument('--wasi-dotnet', type=Path, default=os.environ.get('WASI_DOTNET', sdk/'dotnet/dotnet'))
    p.add_argument('--wasi-sdk', type=Path, default=os.environ.get('WASI_SDK_PATH', sdk/f'tools/wasi-sdk-33.0-{arch}-macos'))
    p.add_argument('--parameterless', action='store_true', help='Use explicit parameterless JsonConstructor with init setters')
    p.add_argument('--with-rust', action='store_true', help='Build and measure same-source optimized Rust Native/WASI controls')
    p.add_argument('--output', type=Path)
    args = p.parse_args()
    if args.output is None:
        args.output = HERE.parent / ('results/decoder-setters.json' if args.parameterless else 'results/decoder-diagnostic.json')
    if args.roots < 100 or args.samples < 1:
        p.error('roots must be >= 100 and samples >= 1')
    build = HERE.parent / ('.build/decoder-diagnostic-setters' if args.parameterless else '.build/decoder-diagnostic')
    sources = build / 'sources'
    cache = build / 'commands.json'
    inputs = build / 'inputs'
    generate.generate(inputs, harness.REPO)
    input_project = inputs / 'csharp-closed-inferred/Benchmark.csproj'
    project = sources / 'csharp-closed-inferred/Benchmark.csproj'
    source = project.with_name('Program.cs')
    code = input_project.with_name('Program.cs').read_text()
    if args.parameterless:
        code = replace_once(code, 'public record Payload(int Id, string Label, int[] Values, Dictionary<string,string> Tags, Payment? Next);',
            'public record Payload(int Id, string Label, int[] Values, Dictionary<string,string> Tags, Payment? Next) { [JsonConstructor] public Payload() : this(0, null!, null!, null!, null) {} }')
        for name in generate.NAMES:
            code = replace_once(code, f'public sealed record {name}(Payload Data) : Payment;',
                f'public sealed record {name}(Payload Data) : Payment {{ [JsonConstructor] public {name}() : this((Payload)null!) {{}} }}')
    # The same executable runs both decoders: identical writer, model and score.
    code = replace_once(code, 'var copy = JsonSerializer.Deserialize(json, context.PaymentArray)!;',
                        'var copy = args[2] == "direct" ? DirectReader.Decode(json) : JsonSerializer.Deserialize(json, context.PaymentArray)!;')
    code = replace_once(code, 'var original =', '''var probe = JsonSerializer.SerializeToUtf8Bytes(warm, context.PaymentArray);
if (!probe.AsSpan().SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(DirectReader.Decode(probe), context.PaymentArray))) throw new Exception("Direct reader probe failed");
var original =''')
    cases = '\n'.join(f'            "{n}" => new {n}(data),' for n in generate.NAMES)
    code = replace_once(code, 'long encoded = Stopwatch.GetTimestamp();', 'long encoded = Stopwatch.GetTimestamp();\n    long allocated = GC.GetAllocatedBytesForCurrentThread();')
    code = replace_once(code, 'long decoded = Stopwatch.GetTimestamp();', 'long decodeAllocated = GC.GetAllocatedBytesForCurrentThread() - allocated;\n    long decoded = Stopwatch.GetTimestamp();')
    code = replace_once(code, 'json.Length));', 'json.Length, decodeAllocated));')
    code = replace_once(code, 'int Bytes);', 'int Bytes, long DecodeAllocatedBytes);')
    code += '\n' + (HERE / 'reader.cs').read_text().replace('@@CASES@@', cases)
    if args.skip_build and source.read_text() != code:
        raise RuntimeError('Decoder/model changed; run without --skip-build')
    if not args.skip_build:
        project.parent.mkdir(parents=True, exist_ok=True)
        project.write_text(input_project.read_text())
        source.write_text(code)
        rid = ('osx' if sys.platform == 'darwin' else 'linux') + '-' + ('arm64' if platform.machine() in ['arm64','aarch64'] else 'x64')
        out = build / 'native'
        subprocess.run(['dotnet','publish',str(project),'-c','Release','-r',rid,'-o',str(out),'-p:OptimizationPreference=Speed','-p:IlcInstructionSet=native'],check=True)
        wasm = build_csharp(project, build/'wasi', args.wasi_dotnet, args.wasi_sdk)
        cache.write_text(json.dumps({'native':[str(out/'Benchmark')], 'wasi':wasm},indent=2))
    commands = json.loads(cache.read_text())
    source_hash = hashlib.sha256((sources/'csharp-closed-inferred/Program.cs').read_bytes()).hexdigest()
    proof = json.loads((build/'wasi/build-proof.json').read_text())
    if proof['sourceSha256'] != source_hash:
        raise RuntimeError('WASI binary/source mismatch; rebuild')
    results = {}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    report = {'parameterlessConstructors':args.parameterless, 'platform':platform.platform(), 'timestampUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(), 'roots':args.roots,'samples':args.samples,
              'sourceSha256':source_hash, 'buildProof':proof, 'commands':commands,
              'toolchains':{name:subprocess.check_output(command,text=True).strip() for name,command in
                            {'dotnet':['dotnet','--version'], 'wasiDotnet':[str(args.wasi_dotnet),'--version'], 'wasmtime':['wasmtime','--version']}.items()},
              'results':results}
    args.output.write_text(json.dumps(report,indent=2)+'\n')
    for engine, command in commands.items():
        for decoder in ['metadata','direct']:
            name = engine+'-'+decoder
            results[name] = harness.measure(name, {name:command}, args.roots,args.samples,1800,args.output.parent/'decoder-progress',invocation_args=[args.roots,args.samples,decoder])
            args.output.write_text(json.dumps(report,indent=2)+'\n')
    if args.with_rust:
        rust_output = args.output.with_name(args.output.stem + '-rust.json')
        command = [sys.executable, str(HERE.parent/'run_wasi.py'), '--implementations','rust',
                   '--roots',str(args.roots),'--samples',str(args.samples),'--build-dir',str(build/'rust-control'),
                   '--output',str(rust_output), '--wasi-dotnet',str(args.wasi_dotnet),'--wasi-sdk',str(args.wasi_sdk)]
        if args.skip_build and (build/'rust-control/commands.json').exists():
            command.append('--skip-build')
        subprocess.run(command,check=True)
        rust = json.loads(rust_output.read_text())
        if rust['errors'] or len(rust['results']) != 2:
            raise RuntimeError('Rust control did not finish successfully')
        report['rustControl'] = rust
        args.output.write_text(json.dumps(report,indent=2)+'\n')
    for name, result in results.items():
        print(name, 'decode median', statistics.median(m['decodeSeconds'] for m in result['measurements']), flush=True)

if __name__ == '__main__':
    main()
