#!/usr/bin/env python3
"""Guard the shared line/character partition and the separate test/library totals."""
import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

ROOT=Path(__file__).resolve().parents[1]
spec=importlib.util.spec_from_file_location('code_stats',ROOT/'code_stats.py')
stats=importlib.util.module_from_spec(spec)
spec.loader.exec_module(stats)

class StatisticsTests(unittest.TestCase):
    def test_application_excludes_library_and_domain_tests(self):
        rows=stats.collect()
        self.assertEqual(len(rows),7)
        for row in rows:
            for metric in ['lines','characters']:
                groups=row['groups']
                self.assertGreater(groups['DomainTest'][metric],0)
                self.assertEqual(groups['Total'][metric],groups['ApplicationTotal'][metric]+groups['Library'][metric]+groups['DomainTest'][metric])
            tests=[file for file in row['files'] if any(s['role']=='DomainTest' for s in file['segments'])]
            self.assertEqual(len(tests),1,row['language'])
            self.assertTrue(all(s['role']=='DomainTest' for s in tests[0]['segments']))

    def test_same_ranges_for_blank_lines_and_unicode_characters(self):
        with tempfile.TemporaryDirectory() as name:
            root=Path(name)
            (root/'example').mkdir()
            source=root/'example/Sample.cs'
            source.write_text('class A {}\n\n// 日本 語\n test();\n')
            import hashlib
            manifest={'example/Sample.cs':{'role':'Model','sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'ranges':[{'start':2,'end':3,'role':'DomainTest'},{'start':4,'end':4,'role':'Library'}]}}
            (root/'code_stats_classification.json').write_text(json.dumps(manifest))
            with patch.object(stats,'HERE',root),patch.object(stats,'SOURCES',[('example','example','.cs')]):
                groups=stats.collect()[0]['groups']
                self.assertEqual(groups['ApplicationTotal'],{'lines':1,'characters':8})
                self.assertEqual(groups['DomainTest'],{'lines':2,'characters':5})
                self.assertEqual(groups['Library'],{'lines':1,'characters':7})
                manifest['example/Sample.cs']['ranges'].append({'start':3,'end':3,'role':'Command'})
                (root/'code_stats_classification.json').write_text(json.dumps(manifest))
                with self.assertRaisesRegex(ValueError,'Overlapping range'):stats.collect()

if __name__=='__main__':unittest.main()
