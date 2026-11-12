#!/usr/bin/env python3
import importlib.util,re,unittest
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
spec=importlib.util.spec_from_file_location('level4_stats',ROOT/'code_stats.py');stats=importlib.util.module_from_spec(spec);spec.loader.exec_module(stats)
class SourceTests(unittest.TestCase):
    def test_same_partition_and_test_inclusive_total(self):
        rows=stats.collect();self.assertEqual(len(rows),7)
        for row in rows:
            self.assertEqual(len({f['path']for f in row['files']}),len(row['files']))
            for key in ['lines','characters']:
                g=row['groups'];self.assertEqual(g['ApplicationTotal'][key],sum(g[r][key]for r in stats.ROLES if r!='Library'))
                self.assertGreater(g['DomainTest'][key],0);self.assertGreater(g['HttpTest'][key],0);self.assertGreater(g['Library'][key],0)
    def test_comparative_readmes_quote_actual_sources(self):
        for directory in ['csharp','fsharp','rust','typescript','java','go','swift']:
            path=ROOT/directory/'README.md';text=path.read_text();quotes=re.findall(r'\[[^\]]+:(\d+)–(\d+)\]\(([^)]+)#L\d+-L\d+\)\n\n```\w+\n(.*?)\n```',text,re.S)
            self.assertTrue(quotes,directory)
            for start,end,target,excerpt in quotes:
                lines=(path.parent/target).read_text().splitlines();self.assertEqual(excerpt,'\n'.join(lines[int(start)-1:int(end)]),(directory,target))
if __name__=='__main__':unittest.main()
