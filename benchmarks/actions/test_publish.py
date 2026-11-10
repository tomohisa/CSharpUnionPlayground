import importlib.util
import json
from pathlib import Path
import tempfile
import unittest

spec = importlib.util.spec_from_file_location('publisher', Path(__file__).with_name('publish_intel.py'))
publisher = importlib.util.module_from_spec(spec)
spec.loader.exec_module(publisher)


class MergePublicationTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.source = Path(self.temp.name) / 'incoming'
        self.target = Path(self.temp.name) / 'tracked'
        self.source.mkdir()
        self.target.mkdir()
        self.env = {'completed': True, 'suite': 'json', 'cpu': 'Intel test',
                    'physicalMemoryBytes': 14 * 1024**3, 'runUrl': 'https://example.test/run'}
        self.write(self.source / 'environment.json', self.env)
        self.write(self.source / 'union100.json', {'roots': 100, 'nodesPerBatch': 300,
                                                  'samples': 5, 'summary': []})
        (self.source / 'union100.md').write_text('New JSON timings\n')
        self.old_projection = {'commands': {}, 'results': []}
        self.write(self.target / 'projection-domain.json', self.old_projection)
        (self.target / 'projection-domain.md').write_text('Old projection timings\n')
        self.write(self.target / 'environment.json', {**self.env, 'suite': 'projection'})

    @staticmethod
    def write(path, value):
        path.write_text(json.dumps(value))

    def test_json_update_retains_projection_and_its_provenance(self):
        original = (self.target / 'projection-domain.json').read_bytes()
        publisher.publish(self.source, self.target)
        self.assertEqual((self.target / 'projection-domain.json').read_bytes(), original)
        self.assertEqual((self.target / 'projection-domain.md').read_text(), 'Old projection timings\n')
        old_env = publisher.read(self.target / 'environment-projection.json')
        self.assertEqual(old_env['suite'], 'projection')
        self.assertEqual(publisher.read(self.target / 'environment-json.json')['suite'], 'json')
        index = (self.target / 'README.md').read_text()
        self.assertIn('union100.md', index)
        self.assertIn('projection-domain.md', index)

    def test_wasi_update_retains_both_host_suites(self):
        publisher.publish(self.source, self.target)
        before = {name: (self.target / name).read_bytes() for name in ['union100.json', 'projection-domain.json']}
        self.write(self.source / 'environment.json', {**self.env, 'suite': 'wasi'})
        for name in publisher.WASI_FILES:
            if name == 'union100-wasi.json':
                self.write(self.source / name, {'roots': 100, 'nodesPerBatch': 300, 'samples': 5, 'summary': []})
            elif name.endswith('.json'):
                self.write(self.source / name, self.old_projection)
            else:
                (self.source / name).write_text('WASI timings\n')
        publisher.publish(self.source, self.target)
        for name, value in before.items():
            self.assertEqual((self.target / name).read_bytes(), value)
        self.assertIn('union100-wasi.md', (self.target / 'README.md').read_text())

    def test_incomplete_run_does_not_publish(self):
        self.write(self.source / 'environment.json', {**self.env, 'completed': False})
        with self.assertRaises(ValueError):
            publisher.publish(self.source, self.target)
        self.assertFalse((self.target / 'union100.json').exists())

    def test_missing_report_does_not_publish_partial_data(self):
        (self.source / 'union100.md').unlink()
        with self.assertRaises(ValueError):
            publisher.publish(self.source, self.target)
        self.assertFalse((self.target / 'union100.json').exists())

    def test_projection_update_retains_json(self):
        publisher.publish(self.source, self.target)
        original = (self.target / 'union100.json').read_bytes()
        self.write(self.source / 'environment.json', {**self.env, 'suite': 'projection'})
        for name in publisher.PROJECTION_FILES:
            if name.endswith('.json'):
                self.write(self.source / name, self.old_projection)
            else:
                (self.source / name).write_text('Updated projection timings\n')
        publisher.publish(self.source, self.target)
        self.assertEqual((self.target / 'union100.json').read_bytes(), original)


if __name__ == '__main__':
    unittest.main()
