"""Build unchanged C# benchmark sources as optimized, LLVM-only WASI AOT modules."""
from pathlib import Path
import hashlib
import json
import os
import re
import shutil
import subprocess
import xml.etree.ElementTree as ET



def find_cross_compiler(dotnet):
    """Match the active SDK's runtime; never select a legacy compiler by path sort."""
    dotnet = Path(dotnet).resolve()
    sdk = subprocess.check_output([str(dotnet), '--version'], text=True).strip()
    config = json.loads((dotnet.parent/'sdk'/sdk/'MSBuild.runtimeconfig.json').read_text())
    version = config['runtimeOptions']['framework']['version']
    cross = sorted((dotnet.parent/'packs').glob(
        'Microsoft.NETCore.App.Runtime.AOT.*.Cross.wasi-wasm/*/tools/mono-aot-cross'))
    matching = [path for path in cross if path.parent.parent.name == version]
    if len(matching) != 1:
        raise RuntimeError(f'Expected one WASI cross compiler matching SDK {sdk} runtime {version}; install wasi-experimental for that SDK.')
    return matching[0]


def build_csharp(project, output, dotnet, wasi_sdk, wasmtime='wasmtime'):
    project, output, dotnet, wasi_sdk = map(lambda p: Path(p).resolve(), (project, output, dotnet, wasi_sdk))
    output.mkdir(parents=True, exist_ok=True)
    if not dotnet.is_file():
        raise RuntimeError(f'WASI dotnet SDK not found: {dotnet}')
    cross = find_cross_compiler(dotnet)
    root = ET.parse(project).getroot()
    root.set('Sdk', 'Microsoft.NET.Sdk;Microsoft.NET.Runtime.MonoTargets.Sdk.net11;Microsoft.NET.Runtime.WebAssembly.Wasi.Sdk.net11')
    root.insert(0, ET.Element('Import', Project='Sdk.props', Sdk='Microsoft.NET.Runtime.MonoAOTCompiler.Task.net11'))
    props = ET.SubElement(root, 'PropertyGroup')
    settings = {'RuntimeIdentifier': 'wasi-wasm', 'PublishAot': 'false',
                'RunAOTCompilation': 'true', 'AOTMode': 'LLVMOnly', 'WasmEnableSIMD': 'true',
                'WasmBuildNative': 'true', 'PublishTrimmed': 'true', 'TrimMode': 'full',
                'WasmGenerateAppBundle': 'true', 'WasmBuildOnlyAfterPublish': 'true',
                'InvariantGlobalization': 'false', 'WasmNativeStrip': 'true',
                'WasiClangCompileOptimizationFlag': '-O3', 'WasiBitcodeCompileOptimizationFlag': '-O3',
                'WasiClangLinkOptimizationFlag': '-O3', '_MonoAotCrossCompilerPath': str(cross)}
    for key, value in settings.items():
        ET.SubElement(props, key).text = value
    ET.SubElement(ET.SubElement(root, 'ItemGroup'), 'MonoAotCrossCompiler',
                  Include=str(cross), RuntimeIdentifier='wasi-wasm')
    # RC3's glob-based WASI bundler can retain stale, untrimmed framework DLLs.
    # Use only the actual linker outputs; never patch the installed SDK.
    target = ET.SubElement(root, 'Target', Name='UseTrimmedWasiAssemblies',
                           AfterTargets='_GetDefaultWasmAssembliesToBundle',
                           Condition="'$(WasmBuildingForNestedPublish)' == 'true' and '$(PublishTrimmed)' == 'true'")
    group = ET.SubElement(target, 'ItemGroup')
    ET.SubElement(group, 'WasmAssembliesToBundle', Remove='@(WasmAssembliesToBundle)')
    ET.SubElement(group, 'WasmAssembliesToBundle', Include='$(IntermediateOutputPath)linked/*.dll')
    ET.SubElement(target, 'Error', Condition="'@(WasmAssembliesToBundle)' == ''", Text='No trimmed WASI assemblies found.')
    # RC3 also links an absent HTTP component file even for apps with no HTTP.
    target = ET.SubElement(root, 'Target', Name='RemoveMissingUnusedHttpWorld',
                           BeforeTargets='_WasiLinkDotNet',
                           Condition="!Exists('$(MicrosoftNetCoreAppRuntimePackRidNativeDir)WasiHttpWorld_component_type.wit')")
    group = ET.SubElement(target, 'ItemGroup')
    ET.SubElement(group, '_BenchmarkHttpAssembly', Include='@(WasmAssembliesToBundle)',
                  Condition="'%(WasmAssembliesToBundle.Filename)' == 'System.Net.Http'")
    ET.SubElement(target, 'Error', Condition="'@(_BenchmarkHttpAssembly)' != ''", Text='HTTP is used but the SDK HTTP component file is missing.')
    task = ET.SubElement(target, 'ReadLinesFromFile', File='$(_WasmLinkRsp)')
    ET.SubElement(task, 'Output', TaskParameter='Lines', ItemName='_BenchmarkLinkLines')
    group = ET.SubElement(target, 'ItemGroup')
    ET.SubElement(group, '_BenchmarkLinkLines', Remove='@(_BenchmarkLinkLines)',
                  Condition="$([System.String]::Copy('%(_BenchmarkLinkLines.Identity)').Contains('WasiHttpWorld_component_type.wit'))")
    ET.SubElement(target, 'WriteLinesToFile', File='$(_WasmLinkRsp)', Lines='@(_BenchmarkLinkLines)', Overwrite='true')
    target = output / project.name
    ET.ElementTree(root).write(target, encoding='unicode')
    shutil.copy2(project.parent / 'Program.cs', output / 'Program.cs')
    # A renamed assembly or a prior untrimmed build must not contaminate AOT inputs.
    for generated in ['obj/Release/net11.0/wasi-wasm', 'bin/Release/net11.0/wasi-wasm']:
        shutil.rmtree(output / generated, ignore_errors=True)
    env = {**os.environ, 'WASI_SDK_PATH': str(wasi_sdk) + os.sep, 'DOTNET_ROOT': str(dotnet.parent)}
    result = subprocess.run([str(dotnet), 'publish', str(target), '-c', 'Release'],
                            text=True, capture_output=True, env=env, timeout=3600)
    (output / 'build.log').write_text(result.stdout + result.stderr)
    if result.returncode:
        raise subprocess.CalledProcessError(result.returncode, result.args, result.stdout, result.stderr)
    binary = verify_aot_build(output, project.stem, settings)
    return [str(wasmtime), 'run', '-O', 'opt-level=2', '--dir', str(binary.parent) + '::.', str(binary), project.stem]


def verify_aot_build(output, assembly, settings):
    intermediate = output / 'obj/Release/net11.0/wasi-wasm/wasm/for-publish'
    binary = output / 'bin/Release/net11.0/wasi-wasm/AppBundle/dotnet.wasm'
    linked = intermediate / 'dotnet.wasm'
    driver = (intermediate / 'driver-gen.c').read_text()
    compile_flags = (intermediate / 'wasi-compile.rsp').read_text()
    module = 'mono_aot_module_' + re.sub(r'[^A-Za-z0-9_]', '_', assembly) + '_info'
    digest = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
    registered = re.search(r'mono_aot_register_module\s*\(\s*' + re.escape(module) + r'\s*\)', driver)
    if not registered or '-DENABLE_AOT=1' not in compile_flags or 'EE_MODE_LLVMONLY_INTERP' in compile_flags:
        raise RuntimeError('WASI AOT-only evidence missing; refusing interpreter fallback.')
    if not binary.exists() or digest(binary) != digest(linked):
        raise RuntimeError('Packaged WASI binary differs from the AOT link output.')
    proof = {'mode': 'Mono LLVMOnly AOT', 'settings': settings,
             'sourceSha256': digest(output / 'Program.cs'), 'wasmSha256': digest(binary),
             'wasmBytes': binary.stat().st_size, 'aotModule': module,
             'registeredAotModules': len(re.findall(r'mono_aot_register_module\s*\(', driver)),
             'compileFlags': compile_flags, 'httpSdkWorkaround': 'Unused missing HTTP world only; fails if System.Net.Http is bundled.'}
    (output / 'build-proof.json').write_text(json.dumps(proof, indent=2) + '\n')
    return binary
