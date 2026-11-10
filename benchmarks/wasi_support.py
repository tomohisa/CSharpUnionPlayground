"""Build the unchanged C# benchmark source with the experimental Mono WASI SDK."""
from pathlib import Path
import os
import shutil
import subprocess
import xml.etree.ElementTree as ET


def build_csharp(project, output, dotnet, wasi_sdk, wasmtime='wasmtime'):
    project, output, dotnet, wasi_sdk = map(lambda p: Path(p).resolve(), (project, output, dotnet, wasi_sdk))
    output.mkdir(parents=True, exist_ok=True)
    if not dotnet.is_file():
        raise RuntimeError(f'WASI dotnet SDK not found: {dotnet}')
    cross = sorted((dotnet.parent / 'packs').glob('Microsoft.NETCore.App.Runtime.AOT.*.Cross.wasi-wasm/*/tools/mono-aot-cross'))
    if not cross:
        raise RuntimeError('Install wasi-experimental in the dedicated dotnet SDK.')
    root = ET.parse(project).getroot()
    root.set('Sdk', 'Microsoft.NET.Sdk;Microsoft.NET.Runtime.MonoTargets.Sdk.net11;Microsoft.NET.Runtime.WebAssembly.Wasi.Sdk.net11')
    props = ET.SubElement(root, 'PropertyGroup')
    for key, value in {'RuntimeIdentifier': 'wasi-wasm', 'PublishAot': 'false',
                       'WasmBuildNative': 'false', 'PublishTrimmed': 'false',
                       'WasmGenerateAppBundle': 'true', 'InvariantGlobalization': 'false',
                       '_MonoAotCrossCompilerPath': str(cross[0])}.items():
        ET.SubElement(props, key).text = value
    ET.SubElement(ET.SubElement(root, 'ItemGroup'), 'MonoAotCrossCompiler',
                  Include=str(cross[0]), RuntimeIdentifier='wasi-wasm')
    target = output / project.name
    ET.ElementTree(root).write(target, encoding='unicode')
    shutil.copy2(project.parent / 'Program.cs', output / 'Program.cs')
    env = {**os.environ, 'WASI_SDK_PATH': str(wasi_sdk) + os.sep, 'DOTNET_ROOT': str(dotnet.parent)}
    result = subprocess.run([str(dotnet), 'publish', str(target), '-c', 'Release'],
                            text=True, capture_output=True, env=env, timeout=1800)
    (output / 'build.log').write_text(result.stdout + result.stderr)
    if result.returncode:
        raise subprocess.CalledProcessError(result.returncode, result.args, result.stdout, result.stderr)
    binary = output / 'bin/Release/net11.0/wasi-wasm/AppBundle/dotnet.wasm'
    if not binary.exists():
        raise RuntimeError('WASI AppBundle missing after publish')
    return [str(wasmtime), 'run', '-S', 'http', '--dir', str(binary.parent) + '::.',
            str(binary), project.stem]
