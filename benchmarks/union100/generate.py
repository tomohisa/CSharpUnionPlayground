#!/usr/bin/env python3
"""Generate comparable 100-case models; generated sources live in the build directory."""
from pathlib import Path
import json

HERE = Path(__file__).resolve().parent
COUNT = 100
NAMES = [f'Case{i:03}' for i in range(COUNT)]

def template(name, **values):
    text = (HERE / 'templates' / name).read_text()
    for key, value in values.items():
        text = text.replace('@@' + key + '@@', str(value))
    return text

def generate(output, repo):
    output.mkdir(parents=True, exist_ok=True)
    for mode in ['abstract', 'closed-explicit', 'closed-inferred', 'union-custom', 'union-generated']:
        directory = output / ('csharp-' + mode)
        directory.mkdir(exist_ok=True)
        union = mode.startswith('union')
        attrs = ''
        if mode == 'union-generated':
            attrs = '[PlaygroundTaggedUnion]\n' + '\n'.join(f'[PlaygroundUnionCase(typeof({n}), "{n}")]' for n in NAMES)
        elif union:
            attrs = '[JsonConverter(typeof(PaymentConverter))]'
        else:
            attrs = '[JsonPolymorphic(InferClosedTypePolymorphism = true)]' if mode == 'closed-inferred' else '[JsonPolymorphic]\n' + '\n'.join(f'[JsonDerivedType(typeof({n}), "{n}")]' for n in NAMES)
        declaration = ('public union Payment(' + ', '.join(NAMES) + ');' if union else
                       'public ' + ('abstract' if mode == 'abstract' else 'closed') + ' record Payment;')
        cases = '\n'.join(f'public sealed record {n}(Payload Data)' + (';' if union else ' : Payment;') for n in NAMES)
        create = '\n'.join(f'    {i} => new {n}(data),' for i, n in enumerate(NAMES))
        dispatch = '\n'.join(f'    {n} c => (c.Data, {i}),' for i, n in enumerate(NAMES))
        dispatch += '\n    ' + ('null => throw new Exception("Empty union"),' if union else '_ => throw new Exception("Unknown case"),' if mode == 'abstract' else '')
        registrations = '\n'.join(f'[JsonSerializable(typeof({n}))]' for n in NAMES) if union else ''
        converter = ''
        if mode == 'union-custom':
            read = '\n'.join(f'            "{n}" => new Payment(JsonSerializer.Deserialize(data, (JsonTypeInfo<{n}>)options.GetTypeInfo(typeof({n})))!),' for n in NAMES)
            write = '\n'.join(f'            case {n} item: writer.WriteString("$type", "{n}"); writer.WritePropertyName("data"); JsonSerializer.Serialize(writer, item, (JsonTypeInfo<{n}>)options.GetTypeInfo(typeof({n}))); break;' for n in NAMES)
            converter = template('converter.cs', READ=read, WRITE=write)
        registration = 'options.Converters.Add(new PaymentPlaygroundJsonConverter());' if mode == 'union-generated' else ''
        code = template('benchmark.cs', ATTRS=attrs, DECLARATION=declaration, CASES=cases, CREATE=create,
                        DISPATCH=dispatch, REGISTRATIONS=registrations, CONVERTER=converter,
                        CONVERTER_REGISTRATION=registration)
        if mode != 'union-generated':
            code = code.replace('using CSharpUnionPlayground.Experimental.Json;\n', '')
        (directory / 'Program.cs').write_text(code)
        analyzer = f'<ProjectReference Include="{repo / "src/PlaygroundTaggedJson.Generator/PlaygroundTaggedJson.Generator.csproj"}" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />' if mode == 'union-generated' else ''
        (directory / 'Benchmark.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net11.0</TargetFramework><LangVersion>15.0</LangVersion><Nullable>enable</Nullable><ImplicitUsings>enable</ImplicitUsings><PublishAot>true</PublishAot><JsonSerializerIsReflectionEnabledByDefault>false</JsonSerializerIsReflectionEnabledByDefault><NoWarn>SYSLIB1227</NoWarn><WarningsAsErrors>CS8509;CS8655;IL2026;IL3050</WarningsAsErrors></PropertyGroup>
<ItemGroup>{analyzer}</ItemGroup></Project>''')
    rust = output / 'rust'; rust.mkdir(exist_ok=True)
    (rust / 'Cargo.toml').write_text('''[package]
name="union100"
version="0.0.0"
edition="2024"
publish=false
[dependencies]
serde={version="1",features=["derive"]}
serde_json="1"
[[bin]]
name="union100"
path="main.rs"
''')
    (rust / 'Cargo.lock').write_text((HERE / 'Cargo.lock').read_text())
    (rust / 'main.rs').write_text(template('benchmark.rs', CASES='\n'.join(f'    {n}({n}),' for n in NAMES),
        TYPES='\n'.join(f'#[derive(Serialize, Deserialize)]\nstruct {n} {{ data: Payload }}' for n in NAMES),
        CREATE='\n'.join(f'        {i} => Payment::{n}({n} {{ data }}),' for i,n in enumerate(NAMES)),
        DISPATCH='\n'.join(f'        Payment::{n}(v) => (&v.data, {i}),' for i,n in enumerate(NAMES))))
    (output / 'benchmark.ts').write_text(template('benchmark.ts',
        TYPES='\n'.join(f'type {n} = {{ $type: "{n}"; data: Payload }};' for n in NAMES),
        UNION=' | '.join(NAMES), CREATE='\n'.join(f'    case {i}: return {{ $type: "{n}", data }};' for i,n in enumerate(NAMES)),
        DISPATCH='\n'.join(f'    case "{n}": index = {i}; break;' for i,n in enumerate(NAMES))))
    ts = (output / 'benchmark.ts').read_text()
    start = ts.index('// JSON.parse alone')
    end = ts.index('console.log(JSON.stringify({event:"ready"}))', start)
    simple = 'function parse(json:string):Payment[] { return JSON.parse(json) as Payment[]; }\n'
    (output / 'benchmark-simple.ts').write_text(ts[:start] + simple + ts[end:])
    schema = """const PayloadSchema:z.ZodType<Payload> = z.object({
  id:z.number().int(), label:z.string(), values:z.array(z.number().int()),
  tags:z.record(z.string(),z.string()), next:z.lazy(()=>PaymentSchema).nullable()
});
const PaymentSchema:z.ZodType<Payment> = z.discriminatedUnion("$type", [
""" + ',\n'.join(f'  z.object({{$type:z.literal("{n}"),data:PayloadSchema}})' for n in NAMES) + """
]);
const BatchSchema = z.array(PaymentSchema);
function parse(json:string):Payment[] { return BatchSchema.parse(JSON.parse(json)); }
"""
    (output / 'benchmark-zod.ts').write_text('import { z } from "zod";\n' + ts[:start] + schema + ts[end:])
    java = output / 'java';java.mkdir(exist_ok=True)
    (java / 'Benchmark.java').write_text(template('Benchmark.java',
        ATTRS=',\n'.join(f'        @JsonSubTypes.Type(value={n}.class, name="{n}")' for n in NAMES),
        UNION=', '.join(NAMES), TYPES='\n'.join(f'    record {n}(Payload data) implements Payment {{}}' for n in NAMES),
        CREATE='\n'.join(f'            case {i} -> new {n}(data);' for i,n in enumerate(NAMES)),
        DISPATCH='\n'.join(f'            case {n} v -> payloadScore(v.data(), {i});' for i,n in enumerate(NAMES))))
    (output / 'benchmark.go').write_text(template('benchmark.go',
        TYPES='\n'.join(f'type {n} struct {{ Data Payload `json:"data"` }}\nfunc ({n}) isPayment() {{}}' for n in NAMES),
        CREATE='\n'.join(f'    case {i}: return {n}{{data}}' for i,n in enumerate(NAMES)),
        DISPATCH='\n'.join(f'    case {n}: return payloadScore(v.Data, {i})' for i,n in enumerate(NAMES)),
        ENCODE='\n'.join(f'    case {n}: name, data = "{n}", v.Data' for n in NAMES),
        DECODE='\n'.join(f'    case "{n}": return {n}{{data}}, nil' for n in NAMES)))
    (output / 'Benchmark.swift').write_text(template('Benchmark.swift',
        TYPES='\n'.join(f'struct {n}: Codable {{ let data: Payload }}' for n in NAMES),
        CASES='\n'.join(f'    case c{i:03}({n})' for i,n in enumerate(NAMES)),
        CREATE='\n'.join(f'    case {i}: return .c{i:03}({n}(data: data))' for i,n in enumerate(NAMES)),
        DISPATCH='\n'.join(f'    case let .c{i:03}(v): return payloadScore(v.data, {i})' for i,n in enumerate(NAMES))))

if __name__ == '__main__':
    import argparse
    parser=argparse.ArgumentParser();parser.add_argument('output',type=Path)
    args=parser.parse_args();generate(args.output.resolve(),HERE.parents[1])
