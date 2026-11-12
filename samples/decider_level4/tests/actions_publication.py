#!/usr/bin/env python3
"""Keep complete Linux cohorts separate from local observations."""
import copy,hashlib,importlib.util,json,tempfile,unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
spec=importlib.util.spec_from_file_location('level4_actions',ROOT/'benchmark/actions.py');actions=importlib.util.module_from_spec(spec);spec.loader.exec_module(actions)
class PublicationTests(unittest.TestCase):
    def bundle(self,source,mutate=lambda d:None):
        d=json.loads((ROOT/'benchmark/observed/macos-arm64.json').read_text())
        d.update(platform='Linux-test-x64',machine='x86_64',sourceDirty=False,actions={'runUrl':'https://example.invalid/run/1','repository':'owner/repo','runnerLabel':'ubuntu-26.04'})
        mutate(d);name='students-10000.json';raw=json.dumps(d).encode();(source/name).write_bytes(raw)
        (source/'manifest.json').write_text(json.dumps({'formatVersion':1,'files':[{'name':name,'sha256':hashlib.sha256(raw).hexdigest()}]}))
    def test_publish_and_reload_digest_links(self):
        with tempfile.TemporaryDirectory()as temp:
            source=Path(temp)/'source';dest=Path(temp)/'observed/linux-x64-actions';source.mkdir();self.bundle(source);actions.publish(source,dest)
            self.assertEqual(len(actions.load_bundle(dest)),1)
            self.assertIn('app.Run',(dest/'README.md').read_text())
            self.assertIn('https://example.invalid/run/1',(dest/'README.md').read_text())
            self.assertNotIn('(../../csharp)',(dest/'students-10000.md').read_text())
    def test_reject_bad_cohort_before_touching_old_page(self):
        mutations=[lambda d:d['results'].pop(),lambda d:d.update(machine='aarch64'),lambda d:d.update(sourceDirty=True),lambda d:d['results'][0].update(unexpectedErrors=1),lambda d:d['results'][0].update(stateSha256='different'),lambda d:d['results'][0].update(restartReadySeconds=None),lambda d:d['results'][0].update(sampledPeakRssMiB=float('nan'))]
        for mutate in mutations:
            with self.subTest(mutate=mutate),tempfile.TemporaryDirectory()as temp:
                source=Path(temp)/'source';dest=Path(temp)/'dest';source.mkdir();dest.mkdir();(dest/'README.md').write_text('previous complete run');self.bundle(source,mutate)
                with self.assertRaises(ValueError):actions.publish(source,dest)
                self.assertEqual((dest/'README.md').read_text(),'previous complete run')
    def test_corrupt_or_mixed_artifacts(self):
        with tempfile.TemporaryDirectory()as temp:
            source=Path(temp);self.bundle(source);(source/'students-10000.json').write_text('{}')
            with self.assertRaisesRegex(ValueError,'digest'):actions.load_bundle(source)
            self.bundle(source);d=json.loads((source/'students-10000.json').read_text());d['sourceCommit']='other';raw=json.dumps(d).encode();(source/'students-other.json').write_bytes(raw)
            manifest=json.loads((source/'manifest.json').read_text());manifest['files'].append({'name':'students-other.json','sha256':hashlib.sha256(raw).hexdigest()});(source/'manifest.json').write_text(json.dumps(manifest))
            with self.assertRaises(ValueError):actions.load_bundle(source)
if __name__=='__main__':unittest.main()
