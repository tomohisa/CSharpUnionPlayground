"""Explicit C# host configurations shared by both benchmark workloads."""
import json
from pathlib import Path

PGO_ENV = {'DOTNET_TieredCompilation': '1', 'DOTNET_TieredPGO': '1',
           'DOTNET_TC_QuickJit': '1', 'DOTNET_TC_QuickJitForLoops': '1',
           'DOTNET_ReadyToRun': '1'}
NET10_SDK = '10.0.401'
NET10_RUNTIME = '10.0.12'


def jit_publish_args(project, net10=False):
    project = Path(project)
    if net10:
        text = project.read_text().replace('net11.0', 'net10.0').replace('<LangVersion>15.0</LangVersion>', '<LangVersion>14.0</LangVersion>')
        project.write_text(text)
        (project.parent/'global.json').write_text(json.dumps({'sdk': {
            'version': NET10_SDK, 'rollForward': 'disable', 'allowPrerelease': False}})+'\n')
    return ['--self-contained', 'false', '-p:PublishAot=false', '-p:PublishTrimmed=false',
            '-p:PublishReadyToRun=false'] + (['-p:RuntimeFrameworkVersion='+NET10_RUNTIME] if net10 else [])


def jit_command(output, assembly="Benchmark"):
    """Keep runtime settings local to this measured process."""
    return ['env', *[k+'='+v for k,v in PGO_ENV.items()], 'dotnet', str(Path(output)/(assembly+'.dll'))]
