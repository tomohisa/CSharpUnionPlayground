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
             'JSON serialization and memory projection are separate benchmarks. '
             'Each suite retains its own latest run and environment metadata.', '',
             '**TypeScript labels:** `ts-simple` = no runtime validation; `ts-manual` = handwritten validation; `ts-zod` = Zod validation. Raw JSON and CLI retain their original implementation IDs.', '']
    for suite in ['json', 'projection', 'wasi']:
        path = destination / f'environment-{suite}.json'
        if path.exists():
            env = read(path)
            lines += [f"## {'JSON' if suite == 'json' else 'WASI' if suite == 'wasi' else 'Projection'} run", '',
                      f"CPU: {env['cpu']}. RAM: {env['physicalMemoryBytes']/1024**3:.0f} GiB. "
                      f"[Actions run]({env['runUrl']}) / [Environment]({path.name}).", '']
            if (destination/'code'/suite/'README.md').exists():
                lines += [f'[Generated source code](code/{suite}/README.md) / [Source SHA256 manifest](code/{suite}/manifest.json).', '']
            if env.get('completionNote'):
                lines += [env['completionNote'].replace('JSON results are excluded.', 'JSON results from that run are excluded.'), '']
        json_name = 'union100-wasi' if suite == 'wasi' else 'union100'
        if suite in ['json', 'wasi'] and (destination / (json_name + '.json')).exists():
            report = read(destination / (json_name + '.json'))
            lines += [f"{report['roots']:,} roots / {report['nodesPerBatch']:,} nodes per batch, "
                      f"{report['samples']} large batches. "
                      f'[Full timings and lifecycle]({json_name}.md) / [Raw data]({json_name}.json).', '',
                      '| implementation | median batch s | encode s | decode s | peak RSS MiB |',
                      '|---|---:|---:|---:|---:|']
            for row in report['summary']:
                lines.append(f"| {source_label(destination,suite,'json',row['implementation'])} | {row['medianSeconds']:.3f} | "
                             f"{row['encodeMedianSeconds']:.3f} | {row['decodeMedianSeconds']:.3f} | "
                             f"{row['peakRssMiB']:.1f} |")
            lines += ['', 'Counts, wire representations, runtime validation and runtime versions differ '
                      'between implementations and ARM runs; inspect the raw report before comparing.', '']
        projection_name = 'projection-wasi' if suite == 'wasi' else 'projection'
        if suite in ['projection', 'wasi'] and (destination / (projection_name + '-domain.json')).exists():
            report = read(destination / (projection_name + '-domain.json'))
            if suite == 'projection':
                lines += ['`ts-simple (native)` is the simple projection. `ts-zod (native)` additionally validates/clones payload data only for matching handlers; this cost is included in projection time. Unknown event kinds are still ignored. No JSON parsing is included in this projection test.', '']
            if suite == 'wasi':
                lines += ['C# native is speed-optimized Native AOT; C# WASI is LLVMOnly AOT with SIMD/-O3, with no interpreter fallback. Rust uses optimized release/LTO with native ISA or Wasm SIMD. Wasmtime RSS includes the host runtime, not just guest memory.', '']
            lines += [f'[Domain details]({projection_name}-domain.md) / [Fanout details]({projection_name}-fanout.md).', '',
                      'Ten million events, domain routing, dense:', '',
                      '| implementation | append seconds | copy seconds | append RSS MiB |',
                      '|---|---:|---|---:|']
            for name in report['commands']:
                rows = {r['mode']: r for r in report['results']
                        if r['implementation'] == name and r['events'] == 10000000}
                if 'indexed-append' not in rows or 'indexed-copy' not in rows:
                    continue
                a, c = rows['indexed-append'], rows['indexed-copy']
                seconds = f"{a['projectionSeconds']:.3f}" if a['status'] == 'ok' else a['status']
                copied = f"{c['projectionSeconds']:.3f}" if c['status'] == 'ok' else c['status']
                rss = f"{a['peakRssMiB']:.1f}" if a['status'] == 'ok' else '—'
                lines.append(f'| {source_label(destination,suite,"projection",name)} | {seconds} | {copied} | {rss} |')
            fanout_path = destination / (projection_name + '-fanout.json')
            if fanout_path.exists():
                fanout = read(fanout_path)
                lines += ['', 'One million events, sequential fanout, indexed-append:', '',
                          '| implementation | dense seconds | wide seconds | dense RSS MiB | wide RSS MiB |',
                          '|---|---:|---:|---:|---:|']
                for name in fanout['commands']:
                    cases = {r['profile']: r for r in fanout['results'] if r['implementation'] == name
                             and r['events'] == 1000000 and r['mode'] == 'indexed-append'}
                    if not cases:
                        continue
                    times, memory = [], []
                    for profile in ['dense', 'wide']:
                        row = cases.get(profile, {'status': 'not-measured'})
                        times.append(f"{row['projectionSeconds']:.3f}" if row['status'] == 'ok' else row['status'])
                        memory.append(f"{row['peakRssMiB']:.1f}" if row['status'] == 'ok' else '—')
                    lines.append(f'| {source_label(destination,suite,"projection",name)} | {times[0]} | {times[1]} | {memory[0]} | {memory[1]} |')
            lines += ['', '`timeout` means the 25s process budget expired. `not-run-after-budget` means '
                      'an earlier smaller size exceeded a budget and this point was not run. '
                      'The projection stopping budget is 8s.', '']
    lines += ['RSS is the whole-process peak, not retained-state size. Hosted VM, OS and runtime '
              'differences mean these results do not isolate CPU architecture alone.']
    (destination / 'README.md').write_text('\n'.join(lines) + '\n')


def publish(incoming, destination):
    env = read(incoming / 'environment.json')
    if env.get('completed') is not True or env.get('suite') not in ['projection', 'json', 'all', 'wasi']:
        raise ValueError('Only a completed benchmark run can be published automatically.')
    suites = ['projection', 'json'] if env['suite'] == 'all' else [env['suite']]
    files = (PROJECTION_FILES if 'projection' in suites else []) + (JSON_FILES if 'json' in suites else []) + (WASI_FILES if 'wasi' in suites else [])
    # Validate the entire incoming set before changing any tracked data.
    for name in files:
        if not (incoming / name).is_file():
            raise ValueError(f'Completed run is missing {name}')
    for name in files:
        if name.endswith('.json'):
            read(incoming / name)
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
