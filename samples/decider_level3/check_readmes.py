#!/usr/bin/env python3
"""Check that comparative READMEs quote the exact current source at their links."""
from pathlib import Path
import re

HERE = Path(__file__).resolve().parent
LANGUAGES = ["csharp", "fsharp", "rust", "typescript", "java", "go", "swift"]
FENCE = chr(96) * 3
QUOTE = re.compile(
    r"出典：\[[^\]]+\]\(([^)\n]+)#L(\d+)-L(\d+)\)\n\n"
    + FENCE + r"[a-z]+\n(.*?)\n" + FENCE, re.DOTALL)
LINK = re.compile(r"\[[^\]]*\]\(([^)\n]+)\)")


def main():
    count = 0
    for language in LANGUAGES:
        readme = HERE / language / "README.md"
        text = readme.read_text(encoding="utf-8")
        quotes = list(QUOTE.finditer(text))
        if not quotes:
            raise ValueError(f"{readme}: source excerpts missing")
        for quote in quotes:
            relative, first, last, excerpt = quote.groups()
            source = (readme.parent / relative).resolve()
            source.relative_to(HERE)
            lines = source.read_text(encoding="utf-8").splitlines()
            first, last = int(first), int(last)
            if not 1 <= first <= last <= len(lines):
                raise ValueError(f"{readme}: invalid quote range {relative}:{first}-{last}")
            if "\n".join(lines[first - 1:last]) != excerpt:
                raise ValueError(f"{readme}: stale source excerpt {relative}:{first}-{last}")
            count += 1
        for link in LINK.findall(re.sub(FENCE + r".*?" + FENCE, "", text, flags=re.DOTALL)):
            if "://" in link:
                continue
            path, _, fragment = link.partition("#")
            target = (readme.parent / path).resolve()
            if not target.is_file():
                raise ValueError(f"{readme}: broken link {link}")
            if fragment.startswith("L"):
                match = re.fullmatch(r"L(\d+)(?:-L(\d+))?", fragment)
                if not match or not 1 <= int(match[1]) <= len(target.read_text().splitlines()):
                    raise ValueError(f"{readme}: invalid source anchor {link}")
    print(f"Language READMEs: {count} exact source excerpts and local links PASS")


if __name__ == "__main__":
    main()
