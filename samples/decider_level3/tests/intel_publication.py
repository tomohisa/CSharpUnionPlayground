#!/usr/bin/env python3
"""Publication must reject partial/mixed cohorts before replacing prior results."""
import hashlib
import importlib.util
import json
from pathlib import Path
import sys
import tempfile
import unittest

ROOT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(ROOT/'benchmark'))
spec=importlib.util.spec_from_file_location('intel_benchmark',ROOT/'benchmark/intel.py')
intel=importlib.util.module_from_spec(spec);spec.loader.exec_module(intel)

class PublicationTests(unittest.TestCase):
    def bundle(self,source,mutate=lambda d:None):
        entries=[]
        for file in ['macos-arm64.json','macos-arm64-large.json']:
            data=json.loads((ROOT/'benchmark/observed'/file).read_text())
            data.update(machine='x86_64',platform='macOS-test',sourceCommit='test-commit',sourceManifest={'test':'same'},runtimeVersions={'test':'same'},hardware={'cpu':'Intel test fixture','memoryBytes':str(14*1024**3)},actions={'runUrl':'https://example.invalid/run/1','runnerLabel':'macos-15-intel','repository':'owner/repository'})
            mutate(data)
            name=f"students-{data['students']}.json";path=source/name;path.write_text(json.dumps(data))
            entries.append({'name':name,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()})
        (source/'manifest.json').write_text(json.dumps({'formatVersion':1,'files':entries}))
    def test_publish_links_and_provenance(self):
        with tempfile.TemporaryDirectory()as temp:
            source=Path(temp)/'source';dest=Path(temp)/'observed/intel';source.mkdir();self.bundle(source)
            intel.publish(source,dest)
            self.assertIn('Intel test fixture',(dest/'README.md').read_text())
            self.assertIn('https://example.invalid/run/1',(dest/'README.md').read_text())
            report=(dest/'students-22900.md').read_text()
            self.assertNotIn('(../../csharp)',report)
            self.assertEqual(len(intel.load_bundle(dest)),2)
    def test_linux_bundle_and_architecture(self):
        with tempfile.TemporaryDirectory() as temp:
            source=Path(temp)/'source';dest=Path(temp)/'linux';source.mkdir()
            self.bundle(source,lambda d:d.update(platform='Linux-7-test',actions={**d['actions'],'runnerLabel':'ubuntu-26.04'}))
            intel.publish(source,dest)
            self.assertIn('Linux x64 GitHub Actions',(dest/'README.md').read_text())
            self.assertIn('TypeScript + Zod (Node)',(dest/'README.md').read_text())
            self.assertIn('Linux-7-test',(dest/'README.md').read_text())
            self.assertEqual(len(intel.load_bundle(dest)),2)
            self.bundle(source,lambda d:d.update(machine='aarch64',platform='Linux-test'))
            with self.assertRaisesRegex(ValueError,'x64'):intel.load_bundle(source)
    def test_reject_partial_without_modifying_previous_page(self):
        with tempfile.TemporaryDirectory()as temp:
            source=Path(temp)/'source';dest=Path(temp)/'dest';source.mkdir();dest.mkdir();(dest/'README.md').write_text('previous complete run')
            self.bundle(source,lambda d:d['results'].pop())
            with self.assertRaisesRegex(ValueError,'Complete seven-language'):intel.publish(source,dest)
            self.assertEqual((dest/'README.md').read_text(),'previous complete run')
    def test_reject_mixed_runs(self):
        with tempfile.TemporaryDirectory()as temp:
            source=Path(temp);self.bundle(source,lambda d:d['actions'].update(runUrl=str(d['students'])))
            with self.assertRaisesRegex(ValueError,'Mixed runs'):intel.load_bundle(source)
    def test_reject_corrupted_artifact(self):
        with tempfile.TemporaryDirectory()as temp:
            source=Path(temp);self.bundle(source);next(source.glob('students-*.json')).write_text('{}')
            with self.assertRaisesRegex(ValueError,'digest mismatch'):intel.load_bundle(source)

if __name__=='__main__':unittest.main()
