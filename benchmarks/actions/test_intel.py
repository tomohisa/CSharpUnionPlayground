import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

spec = importlib.util.spec_from_file_location('intel', Path(__file__).with_name('run_intel.py'))
intel = importlib.util.module_from_spec(spec)
spec.loader.exec_module(intel)


class IntelPublicationGateTests(unittest.TestCase):
    def check_report(self, rows, unavailable=None):
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / 'result.json'
            output.write_text(json.dumps({'results': rows, 'commands': {'csharp-native': []},
                                          'unavailable': unavailable or {}}))
            with patch.object(intel.subprocess, 'run'):
                intel.run_suite([], output, projection=True)

    def test_expected_slow_mode_timeout_is_an_observation(self):
        self.check_report([
            {'implementation': 'csharp-native', 'mode': 'indexed-append', 'status': 'ok'},
            {'implementation': 'csharp-native', 'mode': 'indexed-copy', 'status': 'timeout'},
        ])

    def test_runtime_error_blocks_publication(self):
        with self.assertRaises(RuntimeError):
            self.check_report([{'status': 'error'}])

    def test_missing_baseline_blocks_publication(self):
        with self.assertRaises(RuntimeError):
            self.check_report([{'implementation': 'csharp-native', 'mode': 'indexed-append', 'status': 'timeout'}])

    def test_missing_language_blocks_publication(self):
        with self.assertRaises(RuntimeError):
            self.check_report([], {'swift-native': 'compiler unavailable'})


class CombinedJobTests(unittest.TestCase):
    def test_combined_runs_host_then_only_wasm_and_captures_all_sources(self):
        calls=[]
        with tempfile.TemporaryDirectory() as directory:
            out=Path(directory)
            def suite(command, output, **kwargs):
                calls.append(command)
                output.write_text(json.dumps({'commands':{},'results':[],'errors':{}}))
            def capture(*command):
                return 'Intel CPU' if command[-1]=='machdep.cpu.brand_string' else '4' if command[-1] in ['hw.logicalcpu','hw.memsize'] else 'abc'
            argv=['run_intel.py','--suite','combined','--json-roots','1000','--wasi-dotnet','/sdk/dotnet','--wasi-sdk','/sdk/wasi']
            with patch.object(intel,'OUT',out), patch.object(intel.platform,'system',return_value='Darwin'), patch.object(intel.platform,'machine',return_value='x86_64'), patch.object(intel,'capture',side_effect=capture), patch.object(intel,'run_suite',side_effect=suite), patch.object(intel,'capture_sources') as sources, patch.object(intel.sys,'argv',argv):
                intel.main()
            self.assertEqual(len(calls),6)
            self.assertIn('--abstract-net10',calls[2])
            self.assertIn('jit-pgo',calls[0])
            self.assertIn('jit-pgo',calls[2])
            for call in calls[3:]:self.assertEqual(call[call.index('--engines')+1],'wasi')
            self.assertEqual({c.args[1] for c in sources.call_args_list},{'json','projection','wasi'})
            env=json.loads((out/'environment.json').read_text())
            self.assertTrue(env['completed'])
            self.assertTrue(env['comparisonGroup'])


if __name__ == '__main__':
    unittest.main()
