#!/usr/bin/env python3
"""Compare switches in the four real C# file-based samples with compiler mutations."""
from pathlib import Path
import subprocess
import tempfile

root = Path(__file__).resolve().parents[1]
card_arm = '    Card card => $"card:{card.CardNumber}"'
checks = 0
with tempfile.TemporaryDirectory(prefix='cupj-payment-switch-') as directory:
    folder = Path(directory)
    (folder / 'global.json').write_text((root / 'global.json').read_text())
    for name in ['JsonDerived', 'ClosedJsonDerived', 'CustomConverter', 'SourceGenerated']:
        source = (root / f'samples/file-based/{name}.cs').read_text()
        source = source.replace('../../src/PlaygroundTaggedJson.Generator/', str(root / 'src/PlaygroundTaggedJson.Generator') + '/')
        if name == 'JsonDerived':
            missing = source.replace(card_arm + ',\n', '')
            added = source + '\npublic record Crypto(decimal Amount) : Payment;\n'
            expected = None  # _ swallows both missing and newly added cases.
        elif name == 'ClosedJsonDerived':
            missing = source.replace(',\n' + card_arm, '')
            added = source + '\npublic sealed record Crypto(decimal Amount) : Payment;\n'
            expected = 'CS8509'
        else:
            missing = source.replace(card_arm + ',\n', '')
            added = source.replace('public union Payment(Cash, Card);',
                                   'public union Payment(Cash, Card, Crypto);')
            if name == 'SourceGenerated':
                added = added.replace('public union Payment(Cash, Card, Crypto);',
                                      '[PlaygroundUnionCase(typeof(Crypto), "crypto")]\npublic union Payment(Cash, Card, Crypto);')
            added += '\npublic record Crypto(decimal Amount);\n'
            expected = 'CS8509'
        fixtures = [('complete switch', source, None),
                    ('omit Card arm', missing, expected),
                    ('add unhandled Crypto', added, expected)]
        if name == 'JsonDerived':
            fixtures.append(('remove discard arm', source.replace(',\n    _ => throw new InvalidOperationException("Unknown payment case.")', ''), 'CS8509'))
        for label, text, diagnostic in fixtures:
            fixture = folder / f'{name}.cs'
            fixture.write_text(text)
            result = subprocess.run(['dotnet', 'build', str(fixture), '--nologo', '-v:q'],
                                    cwd=folder, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
            passed = (result.returncode == 0 if diagnostic is None else
                      result.returncode != 0 and f'error {diagnostic}:' in result.stdout)
            if not passed:
                raise RuntimeError(f'{name}: {label}: unexpected result:\n{result.stdout}')
            checks += 1
            print(f'PASS: {name}: {label}: {diagnostic or "build succeeds"}', flush=True)
print(f'PASS: {checks} switch comparison compiler fixtures')
