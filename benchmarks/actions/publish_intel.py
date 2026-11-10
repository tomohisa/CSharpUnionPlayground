#!/usr/bin/env python3
"""Merge one completed Intel run without replacing the other benchmark suite."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil

PROJECTION_FILES = ['projection-fanout.json', 'projection-fanout.md',
                    'projection-domain.json', 'projection-domain.md']
JSON_FILES = ['union100.json', 'union100.md']
DECODER_FILES = ['decoder/report.json', 'decoder/README.md', 'decoder/code/positional.cs', 'decoder/code/parameterless.cs', 'decoder/code/rust.rs']
WASI_FILES = ['union100-wasi.json', 'union100-wasi.md', 'projection-wasi-fanout.json', 'projection-wasi-fanout.md', 'projection-wasi-domain.json', 'projection-wasi-domain.md']


def read(path):
    return json.loads(path.read_text())



DISPLAY_NAMES = {
    'typescript-simple': 'ts-simple',
    'typescript': 'ts-manual',
    'typescript-zod': 'ts-zod',
    'typescript-native': 'ts-simple (native)',
    'typescript-zod-native': 'ts-zod (native)',
}


def display_name(name):
    return DISPLAY_NAMES.get(name, name)


def refresh_report_labels(destination):
    reverse = {value:key for key,value in DISPLAY_NAMES.items()}
    for name in PROJECTION_FILES + JSON_FILES + WASI_FILES:
        path = destination / name
        if path.suffix != '.md' or not path.exists():
            continue
        suite = 'wasi' if '-wasi' in name else 'json' if name.startswith('union100') else 'projection'
        kind = 'json' if name.startswith('union100') else 'projection'
        manifest = destination/'code'/suite/'manifest.json'
        implementations = read(manifest).get('files',{}).get(kind,{}) if manifest.exists() else {}
        lines = []
        for line in path.read_text().splitlines(keepends=True):
            cells = line.split('|')
            if line.startswith('| ') and len(cells) > 2:
                raw = reverse.get(cells[1].strip(),cells[1].strip())
                if raw in DISPLAY_NAMES or raw in implementations:
                    cells[1] = ' ' + source_label(destination,suite,kind,raw) + ' '
                    line = '|'.join(cells)
            lines.append(line)
        path.write_text(''.join(lines))


def source_label(destination, suite, kind, implementation):
    label = display_name(implementation)
    manifest = destination / 'code' / suite / 'manifest.json'
    if manifest.exists():
        metadata = destination / f'environment-{suite}.json'
        if metadata.exists() and read(manifest).get('sourceCommit') != read(metadata).get('sourceCommit'):
            return label
        entry = read(manifest).get('files', {}).get(kind, {}).get(implementation)
        if entry and (destination / entry['path']).is_file():
            return f"[{label}]({entry['path']})"
    return label

def render_index(destination):
    lines = ['# Intel GitHub Actions observations', '',
             'Native and WASM results are grouped by workload. Each implementation has one Native row and, where available, one WASM row. Native means host execution, including Node/JVM where applicable.', '',
             '**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.', '',
             'For C# and Rust, the overview uses the optimized Native control measured in the same run as WASM. Earlier host-only values remain in the detailed reports. This selects a build configuration, not the fastest observed sample across runs.', '']
    environments = {}
    for suite in ['json', 'projection', 'wasi']:
        path = destination / f'environment-{suite}.json'
        if path.exists():
            environments[suite] = read(path)

    def provenance(suites):
        for suite in suites:
            if suite not in environments:
                continue
            env = environments[suite]
            lines.extend([f"- [{suite} measurement]({env['runUrl']}): {env['cpu']}, "
                          f"{env['physicalMemoryBytes']/1024**3:.0f} GiB; [environment](environment-{suite}.json)."])
            if (destination/'code'/suite/'README.md').exists():
                lines.append(f'  [Generated source code](code/{suite}/README.md) / [Source SHA256 manifest](code/{suite}/manifest.json).')
        lines.append('')

    def backend(implementation):
        return 'WASM AOT' if implementation.endswith('-wasi') else 'Native'

    def base_name(implementation):
        return implementation.removesuffix('-native').removesuffix('-wasi')

    def label(suite, kind, implementation):
        link = source_label(destination, suite, kind, implementation)
        old = display_name(implementation)
        new = display_name(base_name(implementation))
        # Projection's plain TypeScript ID denotes unchecked, simple projection.
        if kind == 'projection':
            new = 'ts-simple' if base_name(implementation) == 'typescript' else 'ts-zod' if base_name(implementation) == 'typescript-zod' else new
        return link.replace('['+old+']', '['+new+']', 1) if link.startswith('[') else new

    def config(suite, implementation):
        if suite != 'wasi':
            return 'Host defaults'
        if implementation.startswith('csharp-'):
            return 'LLVMOnly AOT / SIMD / -O3' if implementation.endswith('-wasi') else 'Native AOT Speed / native ISA'
        return 'Release / LTO / SIMD' if implementation.endswith('-wasi') else 'Release / LTO / native ISA'

    def measurement(suite):
        return f'[{suite}](environment-{suite}.json)' if suite in environments else suite

    def json_rows():
        native, wasm = {}, []
        for suite, name in [('json','union100'),('wasi','union100-wasi')]:
            path = destination / (name+'.json')
            if not path.exists():
                continue
            report = read(path)
            for row in report['summary']:
                value = (suite, row, report)
                if row['implementation'].endswith('-wasi'):
                    wasm.append(value)
                else:
                    native[base_name(row['implementation'])] = value
        return list(native.values()) + wasm

    rows = json_rows()
    if rows or any((destination/name).exists() for name in ['union100.json','union100-wasi.json']):
        lines += ['## JSON run', '']
        inputs = {(report['roots'], report['nodesPerBatch'], report['samples']) for _,_,report in rows}
        varying = len(inputs) != 1
        if not varying:
            roots, nodes, samples = next(iter(inputs))
            lines += [f'{roots:,} roots / {nodes:,} nodes per batch; {samples} large batches per row.', '']
        lines += ['[Host-only details](union100.md) / [Paired Native–WASM details](union100-wasi.md).', '']
        provenance(['json','wasi'])
        extra_header = ' roots / batches |' if varying else ''
        lines += ['| implementation | execution | build |'+extra_header+' median batch s | encode s | decode s | peak RSS MiB | measurement |',
                  '|---|---|---|'+('---:|' if varying else '')+'---:|---:|---:|---:|---|']
        for suite, row, report in sorted(rows, key=lambda item: item[1]['medianSeconds']):
            implementation = row['implementation']
            extra = f" {report['roots']:,} / {report['samples']} |" if varying else ''
            lines.append(f"| {label(suite,'json',implementation)} | {backend(implementation)} | {config(suite,implementation)} |{extra} {row['medianSeconds']:.3f} | {row['encodeMedianSeconds']:.3f} | {row['decodeMedianSeconds']:.3f} | {row['peakRssMiB']:.1f} | {measurement(suite)} |")
        lines += ['', 'Median batch time includes encode, decode and switch traversal. TypeScript runtime validation differs by variant; wire representations differ between some implementations. See the source and raw reports.', '']

    if (destination/'decoder/README.md').exists():
        lines += ['### JSON read-path diagnostic', '',
                  '[Intel: STJ metadata vs direct reader vs Rust](decoder/README.md). Same 100-case workload, separate matched Native/WASI AOT run; decode allocation and full lifecycle are retained.', '']

    def projection_reports(suffix):
        native, wasm = {}, []
        for suite, prefix in [('projection','projection'),('wasi','projection-wasi')]:
            path = destination / (prefix+'-'+suffix+'.json')
            if not path.exists():
                continue
            report = read(path)
            for implementation in report['commands']:
                selected = [r for r in report['results'] if r['implementation'] == implementation]
                value = (suite, implementation, selected)
                if implementation.endswith('-wasi'):
                    wasm.append(value)
                else:
                    native[base_name(implementation)] = value
        return list(native.values()) + wasm

    domain, fanout = projection_reports('domain'), projection_reports('fanout')
    if domain or fanout or any((destination/name).exists() for name in ['projection-domain.json','projection-wasi-domain.json']):
        lines += ['## Projection run', '',
                  '`ts-simple` performs the projection without runtime payload validation. `ts-zod` validates/clones payload data in matching handlers, inside the projection timer. Unknown event kinds are ignored. This test does not parse JSON.', '',
                  '[Host domain details](projection-domain.md) / [Host fanout details](projection-fanout.md) / [Native–WASM domain details](projection-wasi-domain.md) / [Native–WASM fanout details](projection-wasi-fanout.md).', '']
        provenance(['projection','wasi'])
    if domain:
        lines += ['### Domain routing: ten million events, dense', '',
                  '| implementation | execution | build | append s | copy s | append RSS MiB | measurement |',
                  '|---|---|---|---:|---|---:|---|']
        for suite, implementation, selected in domain:
            cases = {r['mode']:r for r in selected if r['events']==10000000 and r['profile']=='dense'}
            if 'indexed-append' not in cases or 'indexed-copy' not in cases:
                continue
            a, c = cases['indexed-append'], cases['indexed-copy']
            seconds = f"{a['projectionSeconds']:.3f}" if a['status']=='ok' else a['status']
            copied = f"{c['projectionSeconds']:.3f}" if c['status']=='ok' else c['status']
            rss = f"{a['peakRssMiB']:.1f}" if a['status']=='ok' else '—'
            lines.append(f"| {label(suite,'projection',implementation)} | {backend(implementation)} | {config(suite,implementation)} | {seconds} | {copied} | {rss} | {measurement(suite)} |")
    if fanout:
        lines += ['', '### Sequential fanout: one million events, indexed-append', '',
                  '| implementation | execution | build | dense s | wide s | dense RSS MiB | wide RSS MiB | measurement |',
                  '|---|---|---|---:|---:|---:|---:|---|']
        for suite, implementation, selected in fanout:
            cases = {r['profile']:r for r in selected if r['events']==1000000 and r['mode']=='indexed-append'}
            if not cases:
                continue
            times, memory = [], []
            for profile in ['dense','wide']:
                row = cases.get(profile,{'status':'not-measured'})
                times.append(f"{row['projectionSeconds']:.3f}" if row['status']=='ok' else row['status'])
                memory.append(f"{row['peakRssMiB']:.1f}" if row['status']=='ok' else '—')
            lines.append(f"| {label(suite,'projection',implementation)} | {backend(implementation)} | {config(suite,implementation)} | {times[0]} | {times[1]} | {memory[0]} | {memory[1]} | {measurement(suite)} |")
    if domain or fanout or any((destination/name).exists() for name in ['projection-domain.json','projection-wasi-domain.json']):
        lines += ['', '`timeout` means the 25s process budget expired. `not-run-after-budget` means an earlier smaller size exceeded a budget and this point was not run. The projection stopping budget is 8s.', '']
    lines += ['C# WASM uses LLVMOnly AOT with SIMD/-O3, without interpreter fallback. WASM RSS includes Wasmtime compilation/runtime; it is not guest heap alone. Host VM load, OS, toolchain and build differences affect results. Earlier host-only Native measurements remain available in the detail files and raw JSON.']
    (destination/'README.md').write_text('\n'.join(lines)+'\n')

def publish(incoming, destination):
    env = read(incoming / 'environment.json')
    if env.get('completed') is not True or env.get('suite') not in ['projection', 'json', 'all', 'wasi', 'decoder']:
        raise ValueError('Only a completed benchmark run can be published automatically.')
    suites = ['projection', 'json'] if env['suite'] == 'all' else [env['suite']]
    files = (PROJECTION_FILES if 'projection' in suites else []) + (JSON_FILES if 'json' in suites else []) + (WASI_FILES if 'wasi' in suites else []) + (DECODER_FILES if 'decoder' in suites else [])
    # Validate the entire incoming set before changing any tracked data.
    for name in files:
        if not (incoming / name).is_file():
            raise ValueError(f'Completed run is missing {name}')
    for name in files:
        if name.endswith('.json'):
            read(incoming / name)
    if 'decoder' in suites:
        import sys
        sys.path.insert(0, str(Path(__file__).resolve().parents[1]/'union100/diagnostics'))
        from capture import validate
        report = read(incoming/'decoder/report.json')
        validate(report)
        if report['host']['sourceCommit'] != env['sourceCommit']:
            raise ValueError('Diagnostic source commit mismatch')
        for name, digest in report['sources'].items():
            if Path(name).name != name or hashlib.sha256((incoming/'decoder/code'/name).read_bytes()).hexdigest() != digest:
                raise ValueError('Diagnostic source snapshot mismatch')
    for selected in suites:
        source_manifest = incoming/'code'/selected/'manifest.json'
        if source_manifest.exists():
            manifest = read(source_manifest)
            if manifest.get('sourceCommit') != env.get('sourceCommit'):
                raise ValueError('Source snapshot commit does not match its measured run.')
            for entries in manifest['files'].values():
                for entry in entries.values():
                    path = Path(entry['path'])
                    if path.parts[:2] != ('code', selected) or '..' in path.parts:
                        raise ValueError('Invalid source snapshot path')
                    if hashlib.sha256((incoming/path).read_bytes()).hexdigest() != entry['sha256']:
                        raise ValueError('Source snapshot hash mismatch')
    destination.mkdir(parents=True, exist_ok=True)
    legacy = destination / 'environment.json'
    projection_env = destination / 'environment-projection.json'
    if legacy.exists() and not projection_env.exists() and (destination / 'projection-domain.json').exists():
        shutil.copy2(legacy, projection_env)
    for name in files:
        (destination/name).parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(incoming / name, destination / name)
    for suite in suites:
        (destination / f'environment-{suite}.json').write_text(json.dumps(env, indent=2) + '\n')
    if legacy.exists() and projection_env.exists():
        legacy.unlink()
    for selected in suites:
        source_code = incoming / 'code' / selected
        if source_code.exists():
            shutil.rmtree(destination/'code'/selected,ignore_errors=True)
            shutil.copytree(source_code,destination/'code'/selected)
    refresh_report_labels(destination)
    render_index(destination)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('incoming', type=Path)
    parser.add_argument('destination', type=Path)
    args = parser.parse_args()
    publish(args.incoming, args.destination)
