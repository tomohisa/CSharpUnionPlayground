#!/usr/bin/env python3
"""Merge one completed Intel run without replacing the other benchmark suite."""
import argparse
import json
from pathlib import Path
import shutil

PROJECTION_FILES = ['projection-fanout.json', 'projection-fanout.md',
                    'projection-domain.json', 'projection-domain.md']
JSON_FILES = ['union100.json', 'union100.md']
WASI_FILES = ['union100-wasi.json', 'union100-wasi.md', 'projection-wasi-fanout.json', 'projection-wasi-fanout.md', 'projection-wasi-domain.json', 'projection-wasi-domain.md']


def read(path):
    return json.loads(path.read_text())


def render_index(destination):
    lines = ['# Intel GitHub Actions observations', '',
             'JSON serialization and memory projection are separate benchmarks. '
             'Each suite retains its own latest run and environment metadata.', '']
    for suite in ['json', 'projection', 'wasi']:
        path = destination / f'environment-{suite}.json'
        if path.exists():
            env = read(path)
            lines += [f"## {'JSON' if suite == 'json' else 'WASI' if suite == 'wasi' else 'Projection'} run", '',
                      f"CPU: {env['cpu']}. RAM: {env['physicalMemoryBytes']/1024**3:.0f} GiB. "
                      f"[Actions run]({env['runUrl']}) / [Environment]({path.name}).", '']
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
                lines.append(f"| {row['implementation']} | {row['medianSeconds']:.3f} | "
                             f"{row['encodeMedianSeconds']:.3f} | {row['decodeMedianSeconds']:.3f} | "
                             f"{row['peakRssMiB']:.1f} |")
            lines += ['', 'Counts, wire representations, runtime validation and runtime versions differ '
                      'between implementations and ARM runs; inspect the raw report before comparing.', '']
        projection_name = 'projection-wasi' if suite == 'wasi' else 'projection'
        if suite in ['projection', 'wasi'] and (destination / (projection_name + '-domain.json')).exists():
            report = read(destination / (projection_name + '-domain.json'))
            if suite == 'projection':
                lines += ['`typescript-native` is the simple projection. `typescript-zod-native` additionally validates/clones payload data only for matching handlers; this cost is included in projection time. Unknown event kinds are still ignored. No JSON parsing is included in this projection test.', '']
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
                lines.append(f'| {name} | {seconds} | {copied} | {rss} |')
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
    render_index(destination)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('incoming', type=Path)
    parser.add_argument('destination', type=Path)
    args = parser.parse_args()
    publish(args.incoming, args.destination)
