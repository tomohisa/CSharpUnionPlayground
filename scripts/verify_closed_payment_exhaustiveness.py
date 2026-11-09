#!/usr/bin/env python3
"""Mutate the real file-based sample and check compiler exhaustiveness diagnostics."""
from pathlib import Path
import subprocess
import tempfile

root = Path(__file__).resolve().parents[1]
source = (root / 'samples/file-based/ClosedJsonDerived.cs').read_text()
card_arm = '    Card card => $"card:{card.CardNumber}"'
nullable = source.replace('Describe(Payment payment)', 'Describe(Payment? payment)')
fixtures = [
    ('all cases, no discard', source, None),
    ('explicit abstract closed is rejected', source.replace('public closed record', 'public abstract closed record'), 'CS9384'),
    ('missing Card', source.replace(',\n' + card_arm, ''), 'CS8509'),
    ('new unhandled Crypto case', source + '\npublic sealed record Crypto(decimal Amount) : Payment;\n', 'CS8509'),
    ('nullable input without null arm', nullable, 'CS8655'),
    ('nullable input with null arm', nullable.replace(card_arm, card_arm + ',\n    null => "unset"'), None),
    ('ordinary abstract base without closed', source.replace('public closed record', 'public abstract record'), 'CS8509'),
]
with tempfile.TemporaryDirectory(prefix='cupj-closed-switch-') as directory:
    folder = Path(directory)
    (folder / 'global.json').write_text((root / 'global.json').read_text())
    fixture = folder / 'Fixture.cs'
    for name, text, diagnostic in fixtures:
        fixture.write_text(text)
        result = subprocess.run(['dotnet', 'build', str(fixture), '--nologo', '-v:q'],
                                cwd=folder, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        if diagnostic is None:
            passed = result.returncode == 0 and 'CS8509' not in result.stdout and 'CS8655' not in result.stdout
        else:
            passed = result.returncode != 0 and f'error {diagnostic}:' in result.stdout
        if not passed:
            raise RuntimeError(f'{name}: unexpected build result:\n{result.stdout}')
        print(f'PASS: {name}' + (f' ({diagnostic})' if diagnostic else ''))
print(f'PASS: {len(fixtures)} exhaustiveness compiler fixtures')
