#!/usr/bin/env python3
"""Compile the actual Zod sample and mutations with its pinned TypeScript compiler."""
from pathlib import Path
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[1]
SAMPLE = ROOT / 'samples/file-based/PaymentZod.ts'
TSC = SAMPLE.parent / 'node_modules/.bin/tsc'


def replace_once(source, old, new):
    if source.count(old) != 1:
        raise RuntimeError('PaymentZod.ts changed; update the mutation fixture')
    return source.replace(old, new)


def main():
    if not TSC.exists():
        raise SystemExit('Run npm ci --prefix samples/file-based first')
    source = SAMPLE.read_text()
    omitted = replace_once(source, '    case "card": return `card:${payment.cardNumber}`;\n', '')
    added = replace_once(source, ']);\ntype Payment',
        ', z.object({ $type: z.literal("crypto"), amount: z.number(), next: z.null() })\n]);\ntype Payment')
    with tempfile.TemporaryDirectory(prefix='payment-zod-switch-') as directory:
        path = Path(directory)
        (path/'node_modules').symlink_to(SAMPLE.parent/'node_modules', target_is_directory=True)
        (path/'package.json').write_text('{"type":"module"}')
        for name, code, expected in [('all-cases', source, None),
                                      ('omit-card', omitted, 'TS2322'),
                                      ('add-crypto', added, 'TS2322')]:
            entry = path/(name+'.ts')
            entry.write_text(code)
            result = subprocess.run([str(TSC), '--noEmit', '--strict', '--target', 'ES2022',
                                     '--module', 'NodeNext', str(entry)], capture_output=True, text=True)
            output = result.stdout + result.stderr
            if (expected is None and result.returncode != 0) or (expected is not None and
                    (result.returncode == 0 or expected not in output)):
                raise RuntimeError(f'{name}: unexpected compiler result\n{output}')
            print(f'PASS {name}: {expected or "compiles"}')
    print('PASS: 3 Zod-derived Payment exhaustiveness fixtures')


if __name__ == '__main__':
    main()
