#!/usr/bin/env python3
"""Verify every relative Markdown link in the generated handbook resolves to an
existing file. External (http/https) and pure-anchor links are skipped."""
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
DOCS = os.path.join(ROOT, "Docs")
LINK_RE = re.compile(r"\[[^\]]+\]\(([^)]+)\)")

md_files = []
for base, _dirs, files in os.walk(DOCS):
    for f in files:
        if f.endswith(".md"):
            md_files.append(os.path.join(base, f))

broken = []
checked = 0
for md in md_files:
    d = os.path.dirname(md)
    for m in LINK_RE.finditer(open(md, encoding="utf-8").read()):
        target = m.group(1).strip()
        if target.startswith(("http://", "https://", "#", "mailto:")):
            continue
        path = target.split("#", 1)[0]
        if not path:
            continue
        checked += 1
        resolved = os.path.normpath(os.path.join(d, path))
        if not os.path.exists(resolved):
            broken.append((os.path.relpath(md, ROOT), target))

print(f"md_files={len(md_files)} links_checked={checked} broken={len(broken)}")
for src, tgt in broken:
    print(f"  BROKEN  {src}  ->  {tgt}")
sys.exit(1 if broken else 0)
