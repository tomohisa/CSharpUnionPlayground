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

    def test_overview_combines_backends_without_duplicate_native_rows(self):
        def entry(name, seconds):
            return {'implementation':name,'medianSeconds':seconds,'encodeMedianSeconds':0.1,
                    'decodeMedianSeconds':0.2,'peakRssMiB':10}
        host = {'roots':100,'nodesPerBatch':300,'samples':5,
                'summary':[entry('rust',1),entry('typescript-simple',2)]}
        paired = {**host,'summary':[entry('rust-native',3),entry('rust-wasi',4)]}
        self.write(self.target/'environment-json.json',{**self.env,'comparisonGroup':'same-job','runAttempt':'1','sourceCommit':'abc'})
        self.write(self.target/'environment-wasi.json',{**self.env,'comparisonGroup':'same-job','runAttempt':'1','sourceCommit':'abc'})
        self.write(self.target/'union100.json',host)
        self.write(self.target/'union100-wasi.json',paired)
        publisher.render_index(self.target)
        index = (self.target/'README.md').read_text()
        self.assertIn('## JSON run',index)
        self.assertNotIn('## WASI run',index)
        self.assertEqual(index.count('| rust | Native |'),1)
        self.assertEqual(index.count('| rust | WASM AOT |'),1)
        self.assertIn('| rust | Native | Host defaults | 1.000 |',index)
        self.assertIn('| ts-simple | Native |',index)
        self.assertEqual(publisher.read(self.target/'union100.json')['summary'][0]['medianSeconds'],1)

    def test_unmatched_vms_are_not_combined_in_ranking(self):
        row={'implementation':'rust-wasi','medianSeconds':2,'encodeMedianSeconds':1,'decodeMedianSeconds':1,'peakRssMiB':10}
        self.write(self.target/'union100-wasi.json',{'roots':100,'nodesPerBatch':300,'samples':5,'summary':[row]})
        publisher.publish(self.source,self.target)
        index=(self.target/'README.md').read_text()
        self.assertIn('Unmatched runs',index)
        self.assertNotIn('| rust | WASM AOT |',index)

    def test_jit_and_framework_are_visible_in_build_column(self):
        row={'implementation':'csharp-abstract','medianSeconds':1,'encodeMedianSeconds':.1,'decodeMedianSeconds':.8,'peakRssMiB':10}
        self.write(self.source/'union100.json',{'roots':100,'nodesPerBatch':300,'samples':5,'summary':[row],
            'nativeConfiguration':{'csharpMode':'jit-pgo','abstractNet10':True}})
        publisher.publish(self.source,self.target)
        self.assertIn('.NET 10 / Release JIT + Dynamic PGO',(self.target/'README.md').read_text())

    def test_combined_partial_bundle_is_rejected_atomically(self):
        self.write(self.source/'environment.json',{**self.env,'suite':'combined','comparisonGroup':'job'})
        with self.assertRaises(ValueError):publisher.publish(self.source,self.target)
        self.assertFalse((self.target/'union100.json').exists())

    def combined_input(self):
        import hashlib
        env={**self.env,'suite':'combined','comparisonGroup':'one-job#1','sourceCommit':'abc','runAttempt':'1'}
        self.write(self.source/'environment.json',env)
        names=['csharp-abstract','csharp-closed-explicit','csharp-closed-inferred','csharp-union-custom','csharp-union-generated','rust','typescript-simple','typescript','typescript-zod','java','go','swift']
        def row(name):return {'implementation':name,'medianSeconds':1,'encodeMedianSeconds':.1,'decodeMedianSeconds':.8,'peakRssMiB':10}
        host={'roots':100,'nodesPerBatch':300,'samples':5,'summary':[row(n) for n in names],
              'nativeConfiguration':{'csharpMode':'jit-pgo','abstractNet10':True}}
        wasm={**host,'summary':[row(n+'-wasi') for n in names[:6]],'errors':{}}
        self.write(self.source/'union100.json',host)
        self.write(self.source/'union100-wasi.json',wasm)
        for name in publisher.PROJECTION_FILES+publisher.WASI_FILES+publisher.AOT_FILES:
            if name.startswith('union100'):continue
            if name.endswith('.json'):self.write(self.source/name,{'commands':{},'results':[],'nativeConfiguration':{'csharpMode':'jit-pgo'}})
            else:(self.source/name).write_text('projection')
        (self.source/'union100-wasi.md').write_text('WASM only')
        (self.source/'union100-aot.md').write_text('AOT only')
        self.write(self.source/'union100-aot.json',{**host,'summary':[row(n) for n in names[:5]],'nativeConfiguration':{'csharpMode':'aot','abstractNet10':True}})
        for name in ['projection-aot-domain.json','projection-aot-fanout.json']:
            self.write(self.source/name,{'commands':{'csharp-native':[]},'results':[],'nativeConfiguration':{'csharpMode':'aot'}})
        for suite in ['json','projection','wasi','aot']:
            kinds=['json','projection'] if suite in ['wasi','aot'] else [suite]
            manifest={'sourceCommit':'abc','files':{}}
            for kind in kinds:
                items=names[:5] if kind=='json' and suite=='aot' else ['csharp'] if suite=='aot' else names if kind=='json' and suite!='wasi' else names[:6] if kind=='json' else ['csharp','rust']
                entries=manifest['files'][kind]={}
                for item in items:
                    key=item+('-wasi' if suite=='wasi' else '-native' if kind=='projection' else '')
                    path=Path('code')/suite/kind/item/'source.txt'
                    (self.source/path).parent.mkdir(parents=True,exist_ok=True)
                    data=(kind+item).encode();(self.source/path).write_bytes(data)
                    entries[key]={'path':path.as_posix(),'sha256':hashlib.sha256(data).hexdigest()}
            self.write(self.source/'code'/suite/'manifest.json',manifest)

    def test_complete_combined_bundle_is_published_as_one_group(self):
        self.combined_input();publisher.publish(self.source,self.target)
        index=(self.target/'README.md').read_text()
        self.assertIn('One combined job / one VM',index)
        self.assertNotIn('Unmatched runs',index)
        self.assertEqual(index.count('| WASM AOT |'),6)
        self.assertEqual(index.count('/ Native AOT Speed / native ISA |'),5)
        self.assertIn('.NET 10 / Native AOT Speed / native ISA',index)
        for suite in ['json','projection','wasi','aot']:
            self.assertEqual(publisher.read(self.target/f'environment-{suite}.json')['comparisonGroup'],'one-job#1')

    def test_combined_count_mismatch_is_rejected_before_writes(self):
        self.combined_input()
        report=publisher.read(self.source/'union100-wasi.json');report['roots']=200
        self.write(self.source/'union100-wasi.json',report)
        with self.assertRaisesRegex(ValueError,'counts differ'):publisher.publish(self.source,self.target)
        self.assertFalse((self.target/'union100.json').exists())

    def test_combined_rejects_wrong_csharp_profile(self):
        self.combined_input()
        report=publisher.read(self.source/'union100.json');report['nativeConfiguration']['csharpMode']='aot'
        self.write(self.source/'union100.json',report)
        with self.assertRaisesRegex(ValueError,'JIT\+PGO'):publisher.publish(self.source,self.target)
        self.assertFalse((self.target/'union100.json').exists())

    def test_combined_rejects_aot_abstract_on_different_framework(self):
        self.combined_input()
        report=publisher.read(self.source/'union100-aot.json');report['nativeConfiguration']['abstractNet10']=False
        self.write(self.source/'union100-aot.json',report)
        with self.assertRaisesRegex(ValueError,'AOT abstract'):publisher.publish(self.source,self.target)
        self.assertFalse((self.target/'union100.json').exists())

    def test_projection_keeps_both_native_modes_in_same_table(self):
        env={**self.env,'comparisonGroup':'job','runAttempt':'1','sourceCommit':'abc'}
        for suite in ['projection','aot']:
            self.write(self.target/f'environment-{suite}.json',env)
        def projection(mode,seconds):
            return {'commands':{'csharp-native':[]},'nativeConfiguration':{'csharpMode':mode},
                    'results':[{'implementation':'csharp-native','events':10000000,'profile':'dense',
                                'mode':m,'status':'ok','projectionSeconds':seconds,'peakRssMiB':20}
                               for m in ['indexed-append','indexed-copy']]}
        self.write(self.target/'projection-domain.json',projection('jit-pgo',1))
        self.write(self.target/'projection-aot-domain.json',projection('aot',.5))
        publisher.render_index(self.target)
        index=(self.target/'README.md').read_text()
        self.assertIn('| csharp | Native | .NET 11 / Release JIT + Dynamic PGO | 1.000 |',index)
        self.assertIn('| csharp | Native | .NET 11 / Native AOT Speed / native ISA | 0.500 |',index)

    def decoder_input(self):
        import hashlib
        self.write(self.source/'environment.json', {**self.env, 'suite':'decoder', 'sourceCommit':'abc'})
        (self.source/'decoder/code').mkdir(parents=True)
        sources={}
        for name in ['positional.cs','parameterless.cs','rust.rs']:
            data=name.encode()
            (self.source/'decoder/code'/name).write_bytes(data)
            sources[name]=hashlib.sha256(data).hexdigest()
        # checksum(100) from the independent workload reference.
        import sys
        sys.path.insert(0, str(Path(__file__).resolve().parents[1]/'union100'))
        from run import checksum
        result={'roots':100,'samples':1,'sink':checksum(100),
                'measurements':[{'bytes':1000,'decodeSeconds':1}]}
        report={'host':{'sourceCommit':'abc'}, 'sources':sources}
        for group, count in [('positional',4),('parameterless',4),('rust',2)]:
            report[group]={'roots':100,'samples':1,'results':{str(i):result for i in range(count)}}
            if group != 'rust':
                report[group].update(sourceSha256=sources[group+'.cs'],
                    buildProof={'sourceSha256':sources[group+'.cs'],'settings':{'AOTMode':'LLVMOnly'}})
        self.write(self.source/'decoder/report.json',report)
        (self.source/'decoder/README.md').write_text('Diagnostic results')

    def test_decoder_update_retains_other_suites_and_links_result(self):
        publisher.publish(self.source,self.target)
        original=(self.target/'union100.json').read_bytes()
        self.decoder_input()
        publisher.publish(self.source,self.target)
        self.assertEqual((self.target/'union100.json').read_bytes(),original)
        self.assertTrue((self.target/'decoder/code/rust.rs').exists())
        self.assertIn('decoder/README.md',(self.target/'README.md').read_text())
        self.assertEqual(publisher.read(self.target/'environment-decoder.json')['sourceCommit'],'abc')

    def test_decoder_hash_mismatch_blocks_publication_before_mutating(self):
        self.decoder_input()
        (self.source/'decoder/code/positional.cs').write_text('modified after measurement')
        with self.assertRaises(ValueError):
            publisher.publish(self.source,self.target)
        self.assertFalse((self.target/'decoder').exists())

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
