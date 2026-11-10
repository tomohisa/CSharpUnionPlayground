"""Retain the generated workload sources alongside the observations they produced."""
import hashlib
import json
from pathlib import Path
import shutil


def capture_sources(destination, suite, source_commit, source_roots):
    destination = Path(destination)
    code = destination / 'code' / suite
    if code.exists():
        shutil.rmtree(code)
    code.mkdir(parents=True)
    manifest = {'sourceCommit': source_commit, 'suite': suite, 'files': {}}
    kinds = ['json', 'projection'] if suite in ['all', 'wasi', 'aot'] else [suite]
    for kind in kinds:
        report_name = ('union100' if kind == 'json' else 'projection-domain')
        if suite == 'aot':
            report_name = 'union100-aot' if kind == 'json' else 'projection-aot-domain'
        if suite == 'wasi':
            report_name = 'union100-wasi' if kind == 'json' else 'projection-wasi-domain'
        report = json.loads((destination / (report_name + '.json')).read_text())
        entries = manifest['files'][kind] = {}
        for implementation in report['commands']:
            base = implementation
            if kind == 'projection' or suite == 'wasi':
                base = base.removesuffix('-native').removesuffix('-wasi')
            if kind == 'json':
                if base.startswith('csharp-'):
                    relative = Path(base) / 'Program.cs'
                else:
                    relative = Path({'rust': 'rust/main.rs', 'typescript': 'benchmark.ts',
                                     'typescript-simple': 'benchmark-simple.ts', 'typescript-zod': 'benchmark-zod.ts',
                                     'java': 'java/Benchmark.java', 'go': 'benchmark.go', 'swift': 'Benchmark.swift'}[base])
            else:
                relative = Path({'csharp': 'Program.cs', 'rust': 'projection.rs', 'typescript': 'projection.ts',
                                 'typescript-zod': 'projection-zod.ts', 'java': 'Projection.java',
                                 'go': 'projection.go', 'swift': 'Projection.swift'}[base])
            source = Path(source_roots[kind]) / relative
            target = code / (kind if suite in ['wasi','aot'] else '') / base / source.name
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, target)
            entries[implementation] = {'path': target.relative_to(destination).as_posix(),
                                       'sha256': hashlib.sha256(source.read_bytes()).hexdigest()}
            if suite == 'wasi' and implementation in report.get('buildProofs', {}):
                if report['buildProofs'][implementation]['sourceSha256'] != entries[implementation]['sha256']:
                    raise RuntimeError('Source snapshot differs from compiled AOT source: ' + implementation)
    (code / 'manifest.json').write_text(json.dumps(manifest, indent=2) + '\n')
    lines = ['# Generated benchmark sources', '', f'Measured source commit: `{source_commit}`.', '',
             'These are the generated workload files supplied to the compilers/runtimes. '
             'Build through the benchmark CLI to obtain the pinned dependencies and recorded optimization settings.', '']
    for kind, entries in manifest['files'].items():
        lines += [f'## {kind}', '']
        for name, entry in entries.items():
            relative = Path(entry['path']).relative_to(Path('code') / suite).as_posix()
            lines.append(f'- [{name}]({relative})')
    lines += ['', 'The experimental C# converter generator is implemented in '
              f'[PlaygroundTaggedJson.Generator](https://github.com/tomohisa/CSharpUnionPlayground/tree/{source_commit}/src/PlaygroundTaggedJson.Generator).']
    (code / 'README.md').write_text('\n'.join(lines) + '\n')
