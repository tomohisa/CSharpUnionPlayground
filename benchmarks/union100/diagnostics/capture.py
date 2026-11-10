"""Package a same-host diagnostic with exact measured source snapshots."""
import hashlib
import json
from pathlib import Path
import shutil
import statistics


def capture(positional, parameterless, destination, sources, host):
    destination = Path(destination)
    a = json.loads(Path(positional).read_text())
    b = json.loads(Path(parameterless).read_text())
    rust = a.pop('rustControl', None) or b.pop('rustControl', None)
    if rust is None or rust['errors']:
        raise ValueError('A complete Rust Native/WASI control is required')
    report = {'host': host, 'positional': a, 'parameterless': b, 'rust': rust, 'sources': {}}
    destination.mkdir(parents=True, exist_ok=True)
    (destination/'code').mkdir(exist_ok=True)
    for name, source in sources.items():
        data = Path(source).read_bytes()
        report['sources'][name] = hashlib.sha256(data).hexdigest()
        (destination/'code'/name).write_bytes(data)
    validate(report)
    (destination/'report.json').write_text(json.dumps(report, indent=2)+'\n')
    lines = ['# Intel JSON deserialization diagnostic', '',
             f"CPU: {host['cpu']}; RAM {host['physicalMemoryBytes']/1024**3:.0f} GiB. [Actions run]({host['runUrl']}). Source commit: `{host['sourceCommit']}`.", '',
             f"{a['roots']:,} roots / {a['roots']*3:,} nodes per batch; {a['samples']} large batches. Times are decode medians only. The same executable selects metadata or direct decoding; writer, model, nesting and validation are shared.", '',
             '| implementation | Native decode s | WASI AOT decode s | Native allocated MiB | WASI allocated MiB |',
             '|---|---:|---:|---:|---:|']
    for group, decoder, label, file in [('positional','metadata','STJ generated metadata','positional.cs'),
                                       ('parameterless','metadata','Parameterless JsonConstructor / init','parameterless.cs'),
                                       ('positional','direct','Schema-specific direct reader','positional.cs')]:
        values=[]
        for engine in ['native','wasi']:
            ms=report[group]['results'][engine+'-'+decoder]['measurements']
            values.append((statistics.median(m['decodeSeconds'] for m in ms),statistics.median(m['decodeAllocatedBytes'] for m in ms)/1048576))
        lines.append(f'| [{label}](code/{file}) | {values[0][0]:.3f} | {values[1][0]:.3f} | {values[0][1]:.1f} | {values[1][1]:.1f} |')
    native=statistics.median(m['decodeSeconds'] for m in rust['results']['rust-native']['measurements'])
    wasi=statistics.median(m['decodeSeconds'] for m in rust['results']['rust-wasi']['measurements'])
    lines += [f'| [Rust / serde_json](code/rust.rs) | {native:.3f} | {wasi:.3f} | not measured | not measured |', '',
              '[Raw samples, allocation, RSS, startup/preparation/exit, source hashes and AOT proofs](report.json).', '',
              'Allocation is cumulative current-thread decode allocation, not live heap or GC time. WASI is 32-bit; Native is 64-bit. RSS includes Wasmtime compilation/runtime. These are sequential hosted-VM measurements; no confidence interval or universal ranking is claimed.', '',
              'The parameterless direct-reader control is also retained in the raw report. The direct reader is benchmark-schema-specific; it does not reproduce every JsonSerializer contract/option.', '',
              '[Method, implementation and reproduction commands](../../../union100/diagnostics/README.md).']
    (destination/'README.md').write_text('\n'.join(lines)+'\n')
    return report


def validate(report):
    # Independent checksum is shared with the existing Python workload reference.
    import sys
    sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
    from run import checksum
    counts = set()
    sizes = set()
    for group, expected in [('positional',4),('parameterless',4),('rust',2)]:
        suite=report[group]
        counts.add((suite['roots'],suite['samples']))
        if len(suite['results']) != expected:
            raise ValueError(f'Incomplete diagnostic: {group}')
        for result in suite['results'].values():
            if (result['roots'] != suite['roots'] or result['samples'] != suite['samples']
                    or result['sink'] != checksum(suite['roots'])*suite['samples']
                    or len(result['measurements']) != suite['samples']):
                raise ValueError('Invalid diagnostic counts/checksum')
            for m in result['measurements']:
                if m['decodeSeconds'] <= 0 or m['bytes'] <= 0:
                    raise ValueError('Invalid diagnostic timing')
                sizes.add(m['bytes'])
    if len(counts) != 1 or len(sizes) != 1:
        raise ValueError('Diagnostic workloads differ')
    for group in ['positional','parameterless']:
        suite=report[group]
        proof=suite['buildProof']
        if (suite['sourceSha256'] != report['sources'][group+'.cs']
                or proof['sourceSha256'] != suite['sourceSha256']
                or proof['settings']['AOTMode'] != 'LLVMOnly'):
            raise ValueError('Diagnostic source/AOT proof mismatch')
