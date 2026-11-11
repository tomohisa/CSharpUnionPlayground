#!/usr/bin/env python3
"""Add a progress case in disposable source copies and check compiler diagnostics."""
import argparse
import importlib.util
import json
from pathlib import Path
import shutil
import subprocess
import sys

HERE = Path(__file__).resolve().parents[1]
REPO = HERE.parents[1]
BUILD = HERE / '.build' / 'exhaustiveness'
LANGUAGES = ['csharp', 'fsharp', 'rust', 'typescript', 'java', 'go', 'swift']
PATCHES = {
    'csharp': ('Models.cs', 'public closed record Progress;', 'public closed record Progress;\npublic sealed record PendingReview() : Progress;', 'CS8509'),
    'fsharp': ('Models.fs', 'type Progress =', 'type Progress =\n    | PendingReview', 'FS0025'),
    'rust': ('src/models.rs', 'pub enum Progress {', 'pub enum Progress {\n    PendingReview,', 'E0004'),
    'typescript': ('models.ts', 'export const ProgressSchema = z.discriminatedUnion("$type", [',
                   'export const ProgressSchema = z.discriminatedUnion("$type", [\n  z.object({ $type: z.literal("PendingReview") }),', 'never'),
    'java': ('Models.java', 'sealed interface Progress permits Reserved, Studying, Passed, Failed, Withdrawn {}',
             'sealed interface Progress permits Reserved, Studying, Passed, Failed, Withdrawn, PendingReview {}\nrecord PendingReview() implements Progress {}', 'does not cover'),
    'go': ('domain/models.go', 'type Progress interface{ isProgress() }',
           'type Progress interface{ isProgress() }\ntype PendingReview struct{}\nfunc (PendingReview) isProgress() {}', ''),
    'swift': ('models.swift', 'enum Progress {', 'enum Progress {\n    case pendingReview', 'must be exhaustive'),
}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--language', default='all')
    args = parser.parse_args()
    languages = LANGUAGES if args.language == 'all' else args.language.split(',')
    spec = importlib.util.spec_from_file_location('level2_runner', HERE / 'run.py')
    runner = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(runner)
    for language in languages:
        if language not in LANGUAGES: parser.error('Unknown language')
        root = BUILD / language
        target = root / language
        if root.exists(): shutil.rmtree(root)
        shutil.copytree(HERE / language, target, ignore=shutil.ignore_patterns('node_modules', 'bin', 'obj', 'target'))
        file, old, new, diagnostic = PATCHES[language]
        path = target / file
        text = path.read_text()
        if text.count(old) != 1: raise AssertionError(language + ': patch anchor changed')
        path.write_text(text.replace(old, new))
        runner.HERE, runner.BUILD = root, root / '.build'
        try:
            if language == 'typescript':
                (target / 'node_modules').symlink_to(HERE / 'typescript/node_modules', target_is_directory=True)
                config = json.loads((target / 'tsconfig.json').read_text())
                config['compilerOptions']['typeRoots'] = [str(REPO / 'samples/file-based/node_modules/@types')]
                (target / 'tsconfig.json').write_text(json.dumps(config))
                runner.run([REPO / 'samples/file-based/node_modules/.bin/tsc', '-p', target / 'tsconfig.json'])
            else:
                runner.build(language)
        except subprocess.CalledProcessError as error:
            output = (error.stdout or '') + (error.stderr or '')
            if language == 'go' or diagnostic not in output:
                raise AssertionError(language + ': unexpected compilation failure\n' + output) from error
            print(language + ': missing PendingReview rejected by compiler (' + diagnostic + ')', flush=True)
            continue
        if language != 'go': raise AssertionError(language + ': missing case compiled successfully')
        # A private marker interface does not give Go an exhaustive type switch.
        (target / 'domain/probe_test.go').write_text('''package domain
import "testing"
func TestMissingCase(t *testing.T) {
    defer func() { if recover() == nil { t.Fatal("expected runtime default panic") } }()
    Describe(PendingReview{})
}
''')
        runner.run(['go', 'test', './domain'], cwd=target)
        print('go: new case compiles; missing branch reaches runtime default (confirmed)', flush=True)
    return 0


if __name__ == '__main__':
    sys.exit(main())
