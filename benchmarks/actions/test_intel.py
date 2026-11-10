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


if __name__ == '__main__':
    unittest.main()
