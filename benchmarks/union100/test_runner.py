"""Test lifecycle boundaries, OS RSS parsing, validation and child cleanup."""
from pathlib import Path
import contextlib
import io
import json
import subprocess
import sys
import tempfile
import time
import unittest

import run as runner

class RunnerTests(unittest.TestCase):
    def test_lifecycle_and_peak_rss(self):
        with tempfile.TemporaryDirectory() as temp:
            directory=Path(temp)
            result={'roots':100,'samples':1,'sink':runner.checksum(100),
                    'measurements':[{'encodeSeconds':0.02,'decodeSeconds':0.02,
                        'switchSeconds':0.01,'totalSeconds':0.05,'bytes':1000}]}
            source=directory/'fixture.py'
            source.write_text('import json,time\n'
                'time.sleep(0.03)\nprint(json.dumps({"event":"ready"}),flush=True)\n'
                'time.sleep(0.04)\nprint(json.dumps({"event":"prepared"}),flush=True)\n'
                'time.sleep(0.05)\nprint('+repr(json.dumps(result))+',flush=True)\n'
                'time.sleep(0.08)\n')
            with contextlib.redirect_stdout(io.StringIO()):
                measured=runner.measure('fixture',{'fixture':[sys.executable,str(source)]},100,1,5,directory/'progress')
            self.assertGreaterEqual(measured['startupSeconds'],0.025)
            self.assertGreaterEqual(measured['preparationSeconds'],0.03)
            self.assertGreaterEqual(measured['workAndValidationSeconds'],0.04)
            self.assertGreaterEqual(measured['exitSeconds'],0.065)
            self.assertGreater(measured['peakRssBytes'],0)
            self.assertAlmostEqual(sum(measured[k] for k in ['startupSeconds','preparationSeconds','workAndValidationSeconds','exitSeconds']),measured['wallSeconds'],places=5)
            self.assertTrue((directory/'progress/fixture-100-1.json').exists())

    def test_incorrect_results_are_rejected(self):
        with tempfile.TemporaryDirectory() as temp:
            directory=Path(temp);source=directory/'invalid.py'
            source.write_text('print(\'{"event":"ready"}\')\nprint(\'{"event":"prepared"}\')\n'
                'print(\'{"roots":100,"samples":1,"sink":0,"measurements":[]}\')\n')
            with contextlib.redirect_stdout(io.StringIO()),self.assertRaisesRegex(RuntimeError,'checksum/count'):
                runner.measure('invalid',{'invalid':[sys.executable,str(source)]},100,1,5,directory)

    def test_timeout_terminates_process_group(self):
        with tempfile.TemporaryDirectory() as temp:
            source=Path(temp)/'slow.py';source.write_text('import time\ntime.sleep(10)\n')
            start=time.monotonic()
            with contextlib.redirect_stdout(io.StringIO()),self.assertRaises(subprocess.TimeoutExpired):
                runner.measure('slow',{'slow':[sys.executable,str(source)]},100,1,0.1,Path(temp))
            self.assertLess(time.monotonic()-start,3)

if __name__=='__main__':unittest.main()
