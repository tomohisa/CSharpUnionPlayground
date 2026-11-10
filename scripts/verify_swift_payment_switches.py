#!/usr/bin/env python3
"""Typecheck mutations of the default Swift Payment sample."""
from pathlib import Path
import subprocess
import tempfile

root = Path(__file__).resolve().parents[1]
source = (root / 'samples/file-based/Payment.swift').read_text()
arm = '    case let .card(cardNumber, _, _): return "card:\\(cardNumber)"\n'
added = source.replace('indirect enum Payment: Codable {',
                       'indirect enum Payment: Codable {\n    case crypto(amount: Int)')
fixtures = [
    ('complete switch', source, None),
    ('omit Card', source.replace(arm, ''), 'switch must be exhaustive'),
    ('add Crypto', added, 'switch must be exhaustive'),
    ('default masks added case', added.replace(arm, arm + '    default: fatalError("Unhandled case")\n'), None),
    ('non-optional Payment rejects nil', source + '\nlet absent: Payment = nil\n', "'nil' cannot initialize specified type 'Payment'"),
    ('optional Payment accepts nil', source + '\nlet absent: Payment? = nil\n', None),
]
with tempfile.TemporaryDirectory(prefix='cupj-swift-switch-') as directory:
    fixture = Path(directory) / 'Payment.swift'
    for label, text, diagnostic in fixtures:
        fixture.write_text(text)
        result = subprocess.run(['swiftc', '-typecheck', str(fixture)],
                                text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        passed = result.returncode == 0 if diagnostic is None else result.returncode != 0 and diagnostic in result.stdout
        if not passed:
            raise RuntimeError(f'{label}: unexpected result:\n{result.stdout}')
        print(f'PASS: {label}', flush=True)
print(f'PASS: {len(fixtures)} Swift compiler fixtures')
