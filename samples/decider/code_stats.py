#!/usr/bin/env python3
"""Count maintained language sources, grouped by file role, for the README."""
import argparse
import json
from pathlib import Path
import sys

HERE = Path(__file__).resolve().parent
# Language, source root, suffix, Model file, Decider file (relative to source root).
SOURCES = [
    ("C#", "csharp", ".cs", "Models.cs", "Decider.cs"),
    ("Rust", "rust/src", ".rs", "models.rs", "decider.rs"),
    ("TypeScript", "typescript", ".ts", "models.ts", "decider.ts"),
    ("Java", "java", ".java", "Models.java", "Decider.java"),
    ("Go", "go", ".go", "domain/models.go", "domain/decider.go"),
    ("Swift", "swift", ".swift", "models.swift", "decider.swift"),
]
SKIP = {"bin", "obj", "target", "node_modules", ".build"}
GROUPS = ["Model", "Decider", "その他"]
START, END = "<!-- code-stats:start -->", "<!-- code-stats:end -->"


def collect():
    rows = []
    for language, root, suffix, model, decider in SOURCES:
        directory = HERE / root
        files = sorted(path for path in directory.rglob("*")
                       if path.is_file() and path.suffix == suffix
                       and not SKIP.intersection(path.relative_to(directory).parts))
        if not all(directory / name in files for name in [model, decider]):
            raise ValueError(language + ": Model/Decider file is missing")
        groups = {group: {"lines": 0, "characters": 0} for group in GROUPS}
        details = []
        for path in files:
            relative = path.relative_to(directory).as_posix()
            group = "Model" if relative == model else "Decider" if relative == decider else "その他"
            text = path.read_text(encoding="utf-8")
            metrics = {
                "lines": text.count("\n") + int(bool(text) and not text.endswith("\n")),
                "characters": sum(not char.isspace() for char in text),
            }
            for key, value in metrics.items():
                groups[group][key] += value
            details.append({"path": path.relative_to(HERE).as_posix(), "group": group, **metrics})
        groups["合計"] = {key: sum(groups[g][key] for g in GROUPS)
                          for key in ["lines", "characters"]}
        rows.append({"language": language, "groups": groups, "files": details})
    return rows


def markdown(rows):
    lines = []
    for metric, heading in [("lines", "行数（空行を含む）"),
                            ("characters", "空白以外の文字数")]:
        lines += ["### " + heading, "",
                  "| 言語 | Model | Decider | その他 | 合計 |",
                  "|---|---:|---:|---:|---:|"]
        for row in rows:
            values = [f'{row["groups"][group][metric]:,}' for group in [*GROUPS, "合計"]]
            lines.append("| " + row["language"] + " | " + " | ".join(values) + " |")
        lines.append("")
    return "\n".join(lines).rstrip() + "\n"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group()
    mode.add_argument("--write", action="store_true", help="Update the README tables")
    mode.add_argument("--check", action="store_true", help="Fail if README tables are stale")
    mode.add_argument("--json", action="store_true", help="Show groups and per-file counts")
    args = parser.parse_args()
    rows = collect()
    generated = markdown(rows)
    if args.json:
        print(json.dumps(rows, ensure_ascii=False, indent=2))
    elif args.write or args.check:
        path = HERE / "README.md"
        text = path.read_text(encoding="utf-8")
        if text.count(START) != 1 or text.count(END) != 1:
            raise ValueError("README must contain one code-stats marker pair")
        before, rest = text.split(START, 1)
        current, after = rest.split(END, 1)
        expected = "\n\n" + generated + "\n"
        if args.write:
            path.write_text(before + START + expected + END + after, encoding="utf-8")
        elif current != expected:
            print("Code statistics are stale. Run: python3 samples/decider/code_stats.py --write",
                  file=sys.stderr)
            return 1
        else:
            print("README code statistics: up to date")
    else:
        print(generated, end="")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
