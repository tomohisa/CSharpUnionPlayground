#!/usr/bin/env python3
"""Run native Given/When/Then tests through each language's in-memory executor."""
import argparse
import os
import subprocess
from pathlib import Path
import sys
sys.path.insert(0,str(Path(__file__).resolve().parents[1]))
from run import BUILD,HERE,LANGUAGES,REPO,build,run

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--language',default='all')
    parser.add_argument('--native-aot',action='store_true')
    args=parser.parse_args()
    languages=LANGUAGES if args.language=='all' else args.language.split(',')
    for language in languages:
        if language not in LANGUAGES:parser.error('unknown language')
        print('Domain testing '+language,flush=True)
        if language=='go':
            result=run(['go','test','-race','-v','.'],cwd=HERE/'go',env={**os.environ,'GOCACHE':os.environ.get('GOCACHE',str(BUILD/'go-cache'))})
        else:result=run([*build(language,args.native_aot),'--domain-test'],cwd=REPO)
        print(result.stdout,flush=True)
if __name__=='__main__':
    try:main()
    except subprocess.CalledProcessError as error:
        print(error.stdout or '',file=sys.stderr);print(error.stderr or '',file=sys.stderr)
        raise SystemExit(error.returncode or 1)
