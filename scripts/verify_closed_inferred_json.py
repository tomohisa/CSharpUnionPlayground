#!/usr/bin/env python3
"""Verify standard closed-polymorphism inference using the real one-file sample."""
from pathlib import Path
import re
import subprocess
import tempfile

root = Path(__file__).resolve().parents[1]
source = (root / 'samples/file-based/with-verification/ClosedInferredJson.cs').read_text()
added = source + '\npublic sealed record Crypto(string Wallet) : Payment;\n'
handled = added.replace('    Card card => $"card:{card.CardNumber}"',
                        '    Card card => $"card:{card.CardNumber}",\n    Crypto crypto => $"crypto:{crypto.Wallet}"')
handled = handled.replace('Payment original =', '''Payment extra = new Crypto("wallet-1");
string extraJson = JsonSerializer.Serialize(extra, PaymentContext.Default.Payment);
if (JsonSerializer.Deserialize(extraJson, PaymentContext.Default.Payment) is not Crypto { Wallet: "wallet-1" })
    throw new Exception("New case was not inferred.");
Console.WriteLine(extraJson);
Payment original =''')
global_setting = source.replace('[JsonPolymorphic(InferClosedTypePolymorphism = true)]', '').replace(
    'PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)',
    'PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, InferClosedTypePolymorphism = true)')
fixtures = [
    ('type-level inference', source, None, '"$type":"Cash"'),
    ('source-generation-level inference', global_setting, None, '"$type":"Cash"'),
    ('new case without switch update', added, 'CS8509', None),
    ('new case with switch update, no JSON registrations', handled, None, '"$type":"Crypto"'),
    ('renamed Card changes inferred tag', re.sub(r'\bCard\b', 'DebitCard', source), None, '"$type":"DebitCard"'),
]
with tempfile.TemporaryDirectory(prefix='cupj-inferred-json-') as directory:
    folder = Path(directory)
    (folder / 'global.json').write_text((root / 'global.json').read_text())
    fixture = folder / 'Fixture.cs'
    for label, text, diagnostic, expected_json in fixtures:
        fixture.write_text(text)
        if diagnostic:
            command = ['dotnet', 'build', str(fixture), '--nologo', '-v:q']
        else:
            command = ['dotnet', 'run', '--file', str(fixture)]
        result = subprocess.run(command, cwd=folder, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        if diagnostic:
            passed = result.returncode != 0 and f'error {diagnostic}:' in result.stdout
        else:
            passed = result.returncode == 0 and 'PASS' in result.stdout and expected_json in result.stdout
        if not passed:
            raise RuntimeError(f'{label}: unexpected result:\n{result.stdout}')
        print(f'PASS: {label}', flush=True)
print(f'PASS: {len(fixtures)} closed inference fixtures')
