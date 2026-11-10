#!/usr/bin/env python3
"""Run host benchmarks on an Intel Mac and retain provenance with the results."""
import argparse
import datetime
import json
import os
from pathlib import Path
import platform
import subprocess
import sys

REPO = Path(__file__).resolve().parents[2]
OUT = REPO / 'benchmarks/observed/macos-intel-actions'


def capture(*command):
    return subprocess.check_output(command, cwd=REPO, text=True).strip()


def run_suite(command, output, *, projection=False):
    subprocess.run([sys.executable, *command, '--output', str(output)], cwd=REPO, check=True)
    report = json.loads(output.read_text())
    if projection:
        failures = [r for r in report['results'] if r['status'] == 'error']
        if failures or report['unavailable']:
            raise RuntimeError(f'Unexpected projection failure: {failures}; unavailable: {report["unavailable"]}')
        # Timeouts in deliberately quadratic modes are valid observations.
        for implementation in report['commands']:
            if not any(r['implementation'] == implementation and r['mode'] == 'indexed-append'
                       and r['status'] == 'ok' for r in report['results']):
                raise RuntimeError(f'{implementation}: no valid append measurement')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--suite', choices=['projection', 'json', 'all'], default='projection')
    parser.add_argument('--json-roots', type=int, default=200000)
    args = parser.parse_args()
    if not 100 <= args.json_roots <= 729000:
        parser.error('json-roots must be between 100 and 729000')
    if platform.system() != 'Darwin' or platform.machine() != 'x86_64':
        raise RuntimeError('This workflow requires an Intel macOS runner.')
    cpu = capture('sysctl', '-n', 'machdep.cpu.brand_string')
    if 'Intel' not in cpu:
        raise RuntimeError(f'Expected Intel CPU, found {cpu}')
    OUT.mkdir(parents=True, exist_ok=True)
    # A partial/failed run must not retain another run's files as fresh results.
    for path in OUT.iterdir():
        if path.is_file():
            path.unlink()
    run_id = os.environ.get('GITHUB_RUN_ID', '')
    run_url = f'https://github.com/{os.environ.get("GITHUB_REPOSITORY", "")}/actions/runs/{run_id}'
    environment = {
        'timestampUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(),
        'sourceCommit': capture('git', 'rev-parse', 'HEAD'),
        'runner': 'macos-15-intel', 'cpu': cpu,
        'logicalCpuCount': int(capture('sysctl', '-n', 'hw.logicalcpu')),
        'physicalMemoryBytes': int(capture('sysctl', '-n', 'hw.memsize')),
        'os': capture('sw_vers'), 'runUrl': run_url,
        'runAttempt': os.environ.get('GITHUB_RUN_ATTEMPT'),
        'suite': args.suite, 'jsonRoots': args.json_roots,
    }
    (OUT / 'environment.json').write_text(json.dumps(environment, indent=2) + '\n')
    (OUT / 'README.md').write_text(
        '# Intel GitHub Actions observations\n\n'
        f'CPU: {cpu}. Runner: macos-15-intel. [Workflow run]({run_url}).\n\n'
        f'Source commit: `{environment["sourceCommit"]}`. Suite: `{args.suite}`.\n\n'
        'See environment.json for host provenance and each report for toolchain versions, '
        'raw samples, counts, budgets and process lifecycle timings.\n\n'
        'These are hosted VM measurements; ARM and Intel results also differ in OS, '
        'runtime versions and host resources. They do not isolate CPU architecture alone. '
        'Timeout is a process budget, not a measured completion time.\n')
    if args.suite in ['projection', 'all']:
        base = ['benchmarks/projection100/run.py', '--engines', 'native', '--samples', '2',
                '--timeout', '25', '--max-projection-seconds', '8']
        run_suite(base + ['--sizes', '1000,10000,100000,1000000'], OUT / 'projection-fanout.json', projection=True)
        run_suite(base + ['--skip-build', '--routing', 'domain', '--profiles', 'dense',
                         '--modes', 'indexed-append,indexed-copy', '--sizes', '100000,1000000,10000000'],
                  OUT / 'projection-domain.json', projection=True)
        report = json.loads((OUT / 'projection-domain.json').read_text())
        lines = ['\n## Ten million events, domain routing, dense\n',
                 '| implementation | append seconds | copy seconds | append RSS MiB |',
                 '|---|---:|---|---:|']
        for name in report['commands']:
            rows = {r['mode']: r for r in report['results']
                    if r['implementation'] == name and r['events'] == 10000000}
            append = rows['indexed-append']
            copied = rows['indexed-copy']
            seconds = f"{append['projectionSeconds']:.3f}" if append['status'] == 'ok' else append['status']
            copy_seconds = f"{copied['projectionSeconds']:.3f}" if copied['status'] == 'ok' else copied['status']
            rss = f"{append['peakRssMiB']:.1f}" if append['status'] == 'ok' else '—'
            lines.append(f'| {name} | {seconds} | {copy_seconds} | {rss} |')
        with (OUT / 'README.md').open('a') as stream:
            stream.write('\n'.join(lines) + '\n\n[Domain details](projection-domain.md) / [Fanout details](projection-fanout.md).\n\n'
                         '`timeout` means the 25s process budget expired; `not-run-after-budget` means a smaller size exceeded a budget and this size was not measured. The projection stopping budget is 8s.\n')
    if args.suite in ['json', 'all']:
        run_suite(['benchmarks/union100/run.py', '--roots', str(args.json_roots), '--samples', '5'],
                  OUT / 'union100.json')
    (OUT / 'environment.json').write_text(json.dumps({**environment, 'completed': True}, indent=2) + '\n')


if __name__ == '__main__':
    main()
