#!/usr/bin/env python3
"""Compile invalid fixtures with the real pinned compiler; require CUPJ001."""
from pathlib import Path
import subprocess
import tempfile
from xml.sax.saxutils import escape

root = Path(__file__).resolve().parents[1]
generator = root / 'src/PlaygroundTaggedJson.Generator/PlaygroundTaggedJson.Generator.csproj'
fixtures = {
    'duplicate tag': '''[PlaygroundTaggedUnion]
[PlaygroundUnionCase(typeof(A), "same")][PlaygroundUnionCase(typeof(B), "same")]
public union Invalid(A, B);''',
    'missing case': '''[PlaygroundTaggedUnion][PlaygroundUnionCase(typeof(A), "a")]
public union Invalid(A, B);''',
    'unrelated type': '''[PlaygroundTaggedUnion]
[PlaygroundUnionCase(typeof(A), "a")][PlaygroundUnionCase(typeof(string), "s")]
public union Invalid(A, B);''',
    'not a union': '[PlaygroundTaggedUnion] public struct Invalid { }',
    'reserved discriminator': '''[PlaygroundTaggedUnion(Discriminator = "data")]
[PlaygroundUnionCase(typeof(A), "a")][PlaygroundUnionCase(typeof(B), "b")]
public union Invalid(A, B);''',
    'generic union': '''[PlaygroundTaggedUnion]
[PlaygroundUnionCase(typeof(A), "a")][PlaygroundUnionCase(typeof(B), "b")]
public union Invalid<T>(A, B);''',
}
with tempfile.TemporaryDirectory(prefix='cupj-diagnostics-') as directory:
    folder = Path(directory)
    # Preserve this repository's SDK selection even though fixtures live in /tmp.
    (folder / 'global.json').write_text((root / 'global.json').read_text())
    (folder / 'Fixture.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><TargetFramework>net11.0</TargetFramework><LangVersion>preview</LangVersion></PropertyGroup>
<ItemGroup><ProjectReference Include="{escape(str(generator))}" OutputItemType="Analyzer" ReferenceOutputAssembly="false" /></ItemGroup>
</Project>''')
    for name, fixture in fixtures.items():
        (folder / 'Fixture.cs').write_text('''using CSharpUnionPlayground.Experimental.Json;
public record A(string Value);
public record B(int Value);
''' + fixture)
        result = subprocess.run(['dotnet', 'build', '--nologo', '-v:q'], cwd=folder,
                                text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        if result.returncode == 0 or 'error CUPJ001:' not in result.stdout:
            raise RuntimeError(f'{name}: expected CUPJ001, got:\n{result.stdout}')
        print(f'PASS: {name}')
print(f'PASS: {len(fixtures)} generator diagnostic fixtures')
