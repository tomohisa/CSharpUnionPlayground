#!/usr/bin/env python3
"""Verify the committed diagnostic's source identity and independently computed scores."""
import hashlib
import json
from pathlib import Path
import sys

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE.parent))
from run import checksum


def main():
    observed = HERE / 'observed'
    report = json.loads((observed / 'macos-arm64.json').read_text())
    for name, expected in report['sources'].items():
        actual = hashlib.sha256((observed / 'code' / name).read_bytes()).hexdigest()
        if actual != expected:
            raise ValueError(f'Source hash mismatch: {name}')
    conditions = batches = 0
    for group in ['positional', 'parameterless', 'rust']:
        suite = report[group]
        for name, result in suite['results'].items():
            if (result['roots'] != suite['roots'] or result['samples'] != suite['samples']
                    or result['sink'] != checksum(suite['roots']) * suite['samples']
                    or len(result['measurements']) != suite['samples']):
                raise ValueError(f'Invalid count/checksum: {group}/{name}')
            for measurement in result['measurements']:
                if measurement['bytes'] != 86602883 or measurement['decodeSeconds'] <= 0:
                    raise ValueError(f'Invalid input size/timing: {group}/{name}')
            conditions += 1
            batches += len(result['measurements'])
    for group in ['positional', 'parameterless']:
        suite = report[group]
        if (suite['sourceSha256'] != report['sources'][group + '.cs']
                or suite['sourceSha256'] != suite['buildProof']['sourceSha256']
                or suite['buildProof']['settings']['AOTMode'] != 'LLVMOnly'
                or '-DEE_MODE_LLVMONLY_INTERP' in suite['buildProof']['compileFlags']):
            raise ValueError(f'Invalid source/AOT proof: {group}')
    print(f'PASS: {conditions} conditions, {batches} batches; sources, checksums, input sizes and AOT proofs.')


if __name__ == '__main__':
    main()
