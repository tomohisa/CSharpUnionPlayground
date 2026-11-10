import importlib.util
import json
from pathlib import Path
import tempfile
import unittest

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


if __name__ == '__main__':
    unittest.main()
