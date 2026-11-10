import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

spec = importlib.util.spec_from_file_location('wasi', Path(__file__).with_name('wasi_support.py'))
wasi = importlib.util.module_from_spec(spec)
spec.loader.exec_module(wasi)


class AotProofTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.output = Path(self.temp.name)
        self.intermediate = self.output / 'obj/Release/net11.0/wasi-wasm/wasm/for-publish'
        self.bundle = self.output / 'bin/Release/net11.0/wasi-wasm/AppBundle'
        self.intermediate.mkdir(parents=True)
        self.bundle.mkdir(parents=True)
        (self.output / 'Program.cs').write_text('Console.WriteLine("test");')
        (self.intermediate / 'driver-gen.c').write_text('mono_aot_register_module (mono_aot_module_Benchmark_info);')
        (self.intermediate / 'wasi-compile.rsp').write_text('-DENABLE_AOT=1\n-msimd128\n')
        for parent in [self.intermediate, self.bundle]:
            (parent / 'dotnet.wasm').write_bytes(b'\0asm\1\0\0\0')

    def verify(self):
        return wasi.verify_aot_build(self.output, 'Benchmark', {'AOTMode': 'LLVMOnly'})

    def test_matched_aot_artifact_records_proof(self):
        self.assertEqual(self.verify(), self.bundle / 'dotnet.wasm')
        proof = json.loads((self.output / 'build-proof.json').read_text())
        self.assertEqual(proof['registeredAotModules'], 1)
        self.assertEqual(proof['mode'], 'Mono LLVMOnly AOT')

    def test_interpreter_compile_flags_are_rejected(self):
        (self.intermediate / 'wasi-compile.rsp').write_text('-DENABLE_AOT=1\n-DEE_MODE_LLVMONLY_INTERP=1\n')
        with self.assertRaises(RuntimeError):
            self.verify()

    def test_unregistered_application_module_is_rejected(self):
        (self.intermediate / 'driver-gen.c').write_text('extern void *mono_aot_module_Benchmark_info;')
        with self.assertRaises(RuntimeError):
            self.verify()

    def test_stale_packaged_runtime_is_rejected(self):
        (self.bundle / 'dotnet.wasm').write_bytes(b'prebuilt interpreter runtime')
        with self.assertRaises(RuntimeError):
            self.verify()


class CompilerSelectionTests(unittest.TestCase):
    def test_stable_sdk_ignores_older_and_preview_compilers(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory);dotnet=root/'dotnet';dotnet.touch()
            sdk=root/'sdk/11.0.100';sdk.mkdir(parents=True)
            (sdk/'MSBuild.runtimeconfig.json').write_text(json.dumps(
                {'runtimeOptions':{'framework':{'version':'11.0.0'}}}))
            for version in ['10.0.2','11.0.0-rc.3.26522.148','11.0.0']:
                compiler=root/'packs/Microsoft.NETCore.App.Runtime.AOT.osx-arm64.Cross.wasi-wasm'/version/'tools/mono-aot-cross'
                compiler.parent.mkdir(parents=True);compiler.touch()
            with patch.object(wasi.subprocess,'check_output',return_value='11.0.100'):
                self.assertEqual(wasi.find_cross_compiler(dotnet).parent.parent.name,'11.0.0')

    def test_missing_matching_compiler_does_not_fall_back(self):
        with tempfile.TemporaryDirectory() as directory:
            root=Path(directory);dotnet=root/'dotnet';dotnet.touch()
            sdk=root/'sdk/11.0.100';sdk.mkdir(parents=True)
            (sdk/'MSBuild.runtimeconfig.json').write_text(json.dumps(
                {'runtimeOptions':{'framework':{'version':'11.0.0'}}}))
            compiler=root/'packs/Microsoft.NETCore.App.Runtime.AOT.osx-arm64.Cross.wasi-wasm/10.0.2/tools/mono-aot-cross'
            compiler.parent.mkdir(parents=True);compiler.touch()
            with patch.object(wasi.subprocess,'check_output',return_value='11.0.100'):
                with self.assertRaises(RuntimeError):wasi.find_cross_compiler(dotnet)


if __name__ == '__main__':
    unittest.main()
