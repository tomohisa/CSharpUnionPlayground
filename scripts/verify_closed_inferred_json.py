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

explicit = source.replace('[JsonPolymorphic(InferClosedTypePolymorphism = true)]',
    '[JsonPolymorphic(InferClosedTypePolymorphism = true)]\n[JsonDerivedType(typeof(Cash), "cash-fixed")]\n[JsonDerivedType(typeof(Card), "card-fixed")]')
explicit = explicit.replace('nameof(Cash)', '"cash-fixed"').replace('nameof(Card)', '"card-fixed"')
explicit = explicit.replace('    Card card => $"card:{card.CardNumber}"',
    '    Card card => $"card:{card.CardNumber}",\n    Crypto crypto => $"crypto:{crypto.Wallet}"')
explicit = explicit.replace('Payment original =', """try
{
    JsonSerializer.Serialize<Payment>(new Crypto("wallet-1"), PaymentContext.Default.Payment);
    throw new Exception("Unregistered case was inferred despite explicit registrations.");
}
catch (NotSupportedException) { }
Payment original =""")
explicit += '\npublic sealed record Crypto(string Wallet) : Payment;\n'
runtime_only = source.replace('[JsonPolymorphic(InferClosedTypePolymorphism = true)]', '')
runtime_only = runtime_only.replace('Payment original =', """var runtimeOptions = new JsonSerializerOptions { InferClosedTypePolymorphism = true };
var runtimeContext = new PaymentContext(runtimeOptions);
Payment original =""")
runtime_only = runtime_only.replace('string json = JsonSerializer.Serialize(original, PaymentContext.Default.Payment);', """try
{
    JsonSerializer.Serialize(original, runtimeContext.Payment);
    throw new Exception("Runtime option unexpectedly supplied missing generated metadata.");
}
catch (InvalidOperationException error) when (error.Message.Contains("derived type metadata was not generated"))
{
    Console.WriteLine("PASS: runtime-only inference rejected");
}
string json = "{}";
""")
# The remaining round-trip code is unreachable only at runtime; keep its types
# available for the generator while exiting after the expected boundary check.
runtime_only = runtime_only.replace('string json = "{}";', 'if (args.Length == 0) return;\nstring json = "{}";')
fixtures = [
    ('explicit registrations replace inferred list', explicit, None, '"$type":"cash-fixed"'),
    ('runtime-only option cannot supply generated case metadata', runtime_only, None, 'runtime-only inference rejected'),
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
