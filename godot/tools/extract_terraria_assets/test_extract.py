#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""Test extraction of a few Terraria XNB files."""
from __future__ import print_function
import os, sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import extract_xnb as ex

content = r"D:\Program Files (x86)\Steam\steamapps\common\Terraria\Content"
out_dir = "test_output"
if not os.path.isdir(out_dir):
    os.makedirs(out_dir)

files = [
    ("Images", "Item_0.xnb"),
    ("Images", "NPC_0.xnb"),
    ("Images", "Tiles_0.xnb"),
    ("Images", "Projectile_0.xnb"),
]

for subdir, fn in files:
    fpath = os.path.join(content, subdir, fn)
    if not os.path.exists(fpath):
        print("  SKIP  %s  (not found)" % fn)
        continue

    with open(fpath, "rb") as f:
        raw = f.read()
    print("\n%s: %d bytes" % (fn, len(raw)))

    obj = ex.parse_xnb(raw)
    if obj is None:
        print("  FAIL  parse returned None")
        continue

    typ = obj.get("_type", "?")
    print("  type = %s" % typ)

    if typ == "texture2d":
        try:
            rgba, w, h = ex.texture_to_rgba(obj)
            stem = fn.replace(".xnb", "")
            out_path = os.path.join(out_dir, stem + ".png")
            ex._write_png(rgba, w, h, out_path)
            sz = os.path.getsize(out_path)
            print("  OK    %s  %dx%d  %d bytes" % (out_path, w, h, sz))
        except Exception as e:
            print("  FAIL  %s" % e)

    elif typ == "soundeffect":
        try:
            wav = ex.soundeffect_to_wav(obj)
            stem = fn.replace(".xnb", "")
            out_path = os.path.join(out_dir, stem + ".wav")
            with open(out_path, "wb") as f:
                f.write(wav)
            sr = obj.get("sample_rate", "?")
            ch = obj.get("channels", "?")
            print("  OK    %s  %d bytes  %sHz %sch" % (out_path, len(wav), sr, ch))
        except Exception as e:
            print("  FAIL  %s" % e)

    else:
        print("  SKIP  unknown type")

# Try one SoundEffect
print()
sounds_dir = os.path.join(content, "Sounds")
if os.path.isdir(sounds_dir):
    found = False
    for fn in sorted(os.listdir(sounds_dir)):
        if not fn.endswith(".xnb"):
            continue
        fpath = os.path.join(sounds_dir, fn)
        with open(fpath, "rb") as f:
            raw = f.read()
        obj = ex.parse_xnb(raw)
        if obj and obj.get("_type") == "soundeffect":
            wav = ex.soundeffect_to_wav(obj)
            stem = fn.replace(".xnb", "")
            out_path = os.path.join(out_dir, stem + ".wav")
            with open(out_path, "wb") as f:
                f.write(wav)
            print("%s -> %s.wav  %d bytes" % (fn, stem, len(wav)))
            found = True
            break
    if not found:
        print("No SoundEffect XNB found in Sounds/")

print("\nDone. test_output/ contents:")
for fn in sorted(os.listdir(out_dir)):
    sz = os.path.getsize(os.path.join(out_dir, fn))
    print("  %-30s %8d bytes" % (fn, sz))
