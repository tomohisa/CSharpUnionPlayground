#!/usr/bin/env python3
"""Compile mutated real Rust/TS/Java/Go samples; distinguish checks from runtime guards."""
from pathlib import Path
import os
import subprocess
import tempfile
from run_payment_java import jackson_classpath

root = Path(__file__).resolve().parents[1]
samples = root / 'samples/file-based'
checks = 0

def verify(label, command, cwd, diagnostic=None):
    global checks
    result = subprocess.run(command, cwd=cwd, text=True, stdout=subprocess.PIPE,
                            stderr=subprocess.STDOUT, env={**os.environ, 'LC_ALL': 'C'})
    passed = result.returncode == 0 if diagnostic is None else result.returncode != 0 and diagnostic in result.stdout
    if not passed:
        raise RuntimeError(f'{label}: unexpected result:\n{result.stdout}')
    checks += 1
    print(f'PASS: {label}: {diagnostic or "build succeeds"}', flush=True)

with tempfile.TemporaryDirectory(prefix='cupj-cross-switch-') as directory:
    folder = Path(directory)
    # Use one Cargo target directory for all Rust mutations.
    rust = (samples / 'Payment.rs').read_text()
    fixture = folder / 'Payment.rs'
    (folder / 'Cargo.toml').write_text('''[package]
name = "payment-switch-fixtures"
version = "0.0.0"
edition = "2024"
[dependencies]
serde = { version = "1", features = ["derive"] }
serde_json = "1"
[[bin]]
name = "payment"
path = "Payment.rs"
''')
    arm = '        Payment::Card { card_number, .. } => format!("card:{card_number}"),\n'
    added = rust.replace('enum Payment {', 'enum Payment {\n    Crypto { amount: u64 },')
    for label, text, diagnostic in [
        ('complete', rust, None),
        ('omit Card', rust.replace(arm, ''), 'E0004'),
        ('add Crypto', added, 'E0004'),
        ('wildcard masks added case', added.replace(arm, arm + '        _ => panic!("unhandled"),\n'), None),
    ]:
        fixture.write_text(text)
        verify('Rust: ' + label, ['cargo', 'check', '--quiet'], folder, diagnostic)

    ts = (samples / 'Payment.ts').read_text()
    fixture = folder / 'Payment.ts'
    arm = '    case "card": return `card:${payment.cardNumber}`;\n'
    added = ts.replace('type Payment =', 'type Payment =\n  | { $type: "crypto"; amount: number; next: Payment | null }')
    for label, text, diagnostic in [
        ('complete', ts, None),
        ('omit Card', ts.replace(arm, ''), 'TS2322'),
        ('add Crypto', added, 'TS2322'),
        ('ordinary default masks added case', added.replace('const exhaustive: never = payment;', 'const exhaustive = payment;'), None),
    ]:
        fixture.write_text(text)
        verify('TypeScript: ' + label, ['npx', '--yes', '--package', 'typescript@7.0.2', 'tsc', '--noEmit', '--strict', '--target', 'ES2022', str(fixture)], folder, diagnostic)

    java = (samples / 'Payment.java').read_text()
    fixture = folder / 'Payment.java'
    classpath = jackson_classpath(folder)
    arm = '            case Card card -> "card:" + card.cardNumber();\n'
    added = java.replace('permits Cash, Card', 'permits Cash, Card, Crypto').replace(
        '    // No default:', '    record Crypto(long amount) implements Payment {}\n\n    // No default:')
    for label, text, diagnostic in [
        ('complete', java, None),
        ('omit Card', java.replace(arm, ''), 'compiler.err.not.exhaustive'),
        ('add Crypto', added, 'compiler.err.not.exhaustive'),
        ('default masks added case', added.replace(arm, arm + '            default -> throw new IllegalArgumentException();\n'), None),
    ]:
        fixture.write_text(text)
        verify('Java: ' + label, ['javac', '-XDrawDiagnostics', '--class-path', classpath, '-d', str(folder / 'classes'), str(fixture)], folder, diagnostic)

    go = (samples / 'Payment.go').read_text()
    fixture = folder / 'Payment.go'
    # Replace only describe, not the JSON encoder's separate type switch.
    start, end = go.index('func describe('), go.index('// Standard encoding/json')
    describe = go[start:end]
    arm = '\tcase Card:\n\t\treturn "card:" + p.CardNumber\n'
    added = go + '\ntype Crypto struct { Amount int64 }\nfunc (Crypto) isPayment() {}\n'
    no_default = describe.replace('\tdefault:\n\t\tpanic("unknown or nil payment") // Runtime guard, not an exhaustiveness check.\n', '').replace('\t}\n}\n', '\t}\n\treturn "unhandled"\n}\n')
    for label, text in [
        ('complete', go),
        ('omit Card', go[:start] + describe.replace(arm, '') + go[end:]),
        ('add Crypto', added),
        ('no default, fallback return still compiles', go[:start] + no_default + go[end:]),
    ]:
        fixture.write_text(text)
        verify('Go: ' + label, ['go', 'build', '-o', str(folder / 'payment-go'), str(fixture)], folder)
print(f'PASS: {checks} cross-language compiler fixtures')
