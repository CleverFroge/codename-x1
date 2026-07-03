#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Terraria XNB Asset Extractor  v1.1
===================================
Extract all graphic/audio resources from Terraria's Content/ folder.

Usage:
    python extract_xnb.py --output ./terraria_assets
    python extract_xnb.py -i "D:\\Games\\Terraria\\Content" -o ./extracted

Dependencies: pip install pillow

Output:
    terraria_assets/
        Images/      *.png
        Sounds/      *.wav
        Fonts/       *.png
"""

from __future__ import print_function
import os, sys, struct, io, zlib, argparse

__version__ = "1.1"

# ── helpers ────────────────────────────────────────────────────────

def _u8(buf, off):
    b = buf[off]
    if isinstance(b, str):
        return ord(b)
    return b

def _u32l(buf, off):
    return struct.unpack_from("<I", buf, off)[0]

def _u16l(buf, off):
    return struct.unpack_from("<H", buf, off)[0]

def _7bit(buf, off):
    val = shift = 0
    while True:
        b = _u8(buf, off)
        off += 1
        val |= (b & 0x7F) << shift
        shift += 7
        if (b & 0x80) == 0:
            break
    return val, off

def _xnb_str(buf, off):
    length, off = _7bit(buf, off)
    if length == 0:
        return "", off
    s = buf[off:off+length].decode("utf-8", errors="replace")
    return s, off + length

def _adler32(data):
    a, b = 1, 0
    for byte in data:
        if isinstance(byte, str):
            byte = ord(byte)
        a = (a + byte) % 65521
        b = (b + a) % 65521
    return (b << 16) | a

# ── CRC32 ──────────────────────────────────────────────────────────

_CRC_TABLE = None
def _crc32(data):
    global _CRC_TABLE
    if _CRC_TABLE is None:
        _CRC_TABLE = []
        for n in range(256):
            c = n
            for _ in range(8):
                if c & 1:
                    c = 0xEDB88320 ^ (c >> 1)
                else:
                    c = c >> 1
            _CRC_TABLE.append(c)
    crc = 0xFFFFFFFF
    for b in data:
        if isinstance(b, str):
            b = ord(b)
        crc = _CRC_TABLE[(crc ^ b) & 0xFF] ^ (crc >> 8)
    return crc ^ 0xFFFFFFFF

# ── minimal PNG writer ─────────────────────────────────────────────

def _write_png(rgba, w, h, path):
    """Write RGBA bytes to PNG file (pure Python)."""
    stride = 1 + w * 4
    raw = bytearray(h * stride)
    for y in range(h):
        off = y * stride
        raw[off] = 0
        src_off = y * w * 4
        raw[off+1:off+1+w*4] = rgba[src_off:src_off+w*4]

    compressor = zlib.compressobj(level=9, method=zlib.DEFLATED,
                                  wbits=-zlib.MAX_WBITS)
    compressed = compressor.compress(bytes(raw)) + compressor.flush()
    zlib_data = b'\x78\x9c' + compressed + struct.pack('>I', _adler32(raw))

    with open(path, 'wb') as f:
        f.write(b'\x89PNG\r\n\x1a\n')
        # IHDR
        ihdr = struct.pack('>IIBBBBB', w, h, 8, 6, 0, 0, 0)
        f.write(struct.pack('>I', 13))
        f.write(b'IHDR')
        f.write(ihdr)
        f.write(struct.pack('>I', _crc32(b'IHDR' + ihdr)))
        # IDAT
        f.write(struct.pack('>I', len(zlib_data)))
        f.write(b'IDAT')
        f.write(zlib_data)
        f.write(struct.pack('>I', _crc32(b'IDAT' + zlib_data)))
        # IEND
        f.write(struct.pack('>I', 0))
        f.write(b'IEND')
        f.write(struct.pack('>I', _crc32(b'IEND')))

# ── XNB Parser ─────────────────────────────────────────────────────

def parse_xnb(data):
    """Parse XNB file, return dict or None."""
    if data[:3] != b'XNB':
        return None

    pos = 3
    # platform
    _u8(data, pos); pos += 1
    # version
    _u8(data, pos); pos += 1
    flags = _u8(data, pos); pos += 1
    compressed = bool(flags & 0x80)

    # file size
    _u32l(data, pos); pos += 4

    # Decompress if needed
    if compressed:
        comp_size   = _u32l(data, pos); pos += 4
        decomp_size = _u32l(data, pos); pos += 4
        comp_data = data[pos:pos + comp_size]; pos += comp_size
        data = _lzx_decompress(comp_data, decomp_size)
        pos = 0

    # Type readers
    num_readers, pos = _7bit(data, pos)
    readers = []
    for _ in range(num_readers):
        name, pos = _xnb_str(data, pos)
        # reader_version = _u32l(data, pos); pos += 4
        pos += 4
        readers.append(name)

    if not readers:
        return None

    # Primary object
    reader_idx, pos = _7bit(data, pos)
    reader_name = readers[reader_idx] if reader_idx < len(readers) else ""

    return _read_typed_object(data, pos, reader_name)


def _read_typed_object(data, pos, reader):
    r = reader.lower()
    if 'texture2d' in r:
        return _read_texture2d(data, pos)
    if 'soundeffect' in r:
        return _read_soundeffect(data, pos)
    if 'spritefont' in r:
        return _read_spritefont(data, pos)
    return {"_type": "unknown", "_reader": reader, "_raw": data[pos:]}


def _read_texture2d(data, pos):
    surface_fmt = _u32l(data, pos); pos += 4
    width       = _u32l(data, pos); pos += 4
    height      = _u32l(data, pos); pos += 4
    mip_count   = _u32l(data, pos); pos += 4

    result = {
        "_type": "texture2d",
        "fmt":   surface_fmt,
        "w":     width,
        "h":     height,
        "mips":  mip_count,
        "data":  b'',
    }
    if mip_count == 0:
        return result

    if surface_fmt == 0:        # Color (BGRA32)
        data_size = width * height * 4
    elif surface_fmt == 1:      # Bgr565
        data_size = width * height * 2
    elif surface_fmt == 2:      # Bgra5551
        data_size = width * height * 2
    elif surface_fmt == 3:      # Bgra4444
        data_size = width * height * 2
    elif surface_fmt == 4:      # Dxt1
        data_size = max(1, (width + 3) // 4) * max(1, (height + 3) // 4) * 8
    elif surface_fmt == 5:      # Dxt3
        data_size = max(1, (width + 3) // 4) * max(1, (height + 3) // 4) * 16
    elif surface_fmt == 6:      # Dxt5
        data_size = max(1, (width + 3) // 4) * max(1, (height + 3) // 4) * 16
    elif surface_fmt == 12:     # Alpha8
        data_size = width * height
    else:
        data_size = width * height * 4

    result["data"] = data[pos:pos + data_size]
    return result


def _read_soundeffect(data, pos):
    result = {"_type": "soundeffect"}
    result["num_bytes"]   = _u32l(data, pos); pos += 4
    result["sample_rate"] = _u32l(data, pos); pos += 4
    result["avg_bps"]     = _u32l(data, pos); pos += 4
    result["block_align"] = _u32l(data, pos); pos += 4
    result["bits"]        = _u16l(data, pos); pos += 2
    result["channels"]    = _u16l(data, pos); pos += 2
    audio_size = _u32l(data, pos); pos += 4
    result["audio_data"]  = data[pos:pos + audio_size]
    return result


def _read_spritefont(data, pos):
    return {"_type": "spritefont", "_raw": data[pos:]}


# ── LZX decompression (stub) ───────────────────────────────────────

def _lzx_decompress(comp, decomp_size):
    try:
        import lzx as _lzx
        return _lzx.decompress(comp, decomp_size)
    except ImportError:
        pass
    raise RuntimeError(
        "LZX decompression required. Run:  pip install lzx"
    )


# ── Converter: Texture2D -> RGBA ───────────────────────────────────

def texture_to_rgba(tex):
    w = tex["w"]
    h = tex["h"]
    raw = tex["data"]
    fmt = tex["fmt"]

    if fmt == 0:  # Color (BGRA32)
        rgba = bytearray(len(raw))
        for i in range(0, len(raw), 4):
            rgba[i+0] = raw[i+2]  # R <- B
            rgba[i+1] = raw[i+1]  # G <- G
            rgba[i+2] = raw[i+0]  # B <- R
            rgba[i+3] = raw[i+3]  # A <- A
        return (bytes(rgba), w, h)

    if fmt == 12:  # Alpha8
        rgba = bytearray(w * h * 4)
        for i in range(w * h):
            rgba[i*4+0] = 255
            rgba[i*4+1] = 255
            rgba[i*4+2] = 255
            rgba[i*4+3] = raw[i] if i < len(raw) else 255
        return (bytes(rgba), w, h)

    if fmt in (4, 5, 6):  # DXT
        return _dxt_to_rgba(raw, w, h, fmt)

    if fmt == 1:  # Bgr565
        rgba = bytearray(w * h * 4)
        for i in range(w * h):
            off = i * 2
            if off + 2 > len(raw):
                break
            val = _u16l(raw, off)
            r = ((val >> 11) & 0x1F) << 3
            g = ((val >> 5)  & 0x3F) << 2
            b = (val & 0x1F) << 3
            rgba[i*4+0] = r
            rgba[i*4+1] = g
            rgba[i*4+2] = b
            rgba[i*4+3] = 255
        return (bytes(rgba), w, h)

    # Fallback: try Pillow DDS decode
    return _dxt_to_rgba(raw, w, h, fmt)


def _dxt_to_rgba(raw, w, h, fmt):
    from PIL import Image
    dxt_map = {4: b'DXT1', 5: b'DXT3', 6: b'DXT5'}
    fourcc = dxt_map.get(fmt, b'DXT5')

    dds = bytearray()
    dds += struct.pack('<I', 0x20534444)  # 'DDS '
    dds += struct.pack('<I', 124)
    dds += struct.pack('<I', 0x00000001 | 0x00000002 | 0x00000004 | 0x00001000)
    dds += struct.pack('<IIII', h, w,
        max(1, (w+3)//4 * (h+3)//4 * (8 if fmt==4 else 16)), 0, 1)
    dds += b'\x00' * 44
    dds += struct.pack('<I', 32)
    dds += struct.pack('<I', 0x00000004)
    dds += fourcc + b'\x00' * 4
    dds += struct.pack('<IIII', 0, 0, 0, 0)
    dds += struct.pack('<I', 0x00001000)
    dds += b'\x00' * 16
    dds += raw

    img = Image.open(io.BytesIO(dds))
    img_rgba = img.convert('RGBA')
    return (img_rgba.tobytes(), w, h)


# ── Converter: SoundEffect -> WAV ──────────────────────────────────

def soundeffect_to_wav(sfx):
    sr   = sfx["sample_rate"]
    bits = sfx["bits"]
    ch   = sfx["channels"]
    data = sfx["audio_data"]
    br   = sr * ch * (bits // 8)
    ba   = ch * (bits // 8)

    buf = io.BytesIO()
    buf.write(b'RIFF')
    buf.write(struct.pack('<I', 36 + len(data)))
    buf.write(b'WAVE')
    buf.write(b'fmt ')
    buf.write(struct.pack('<IHHIIHH', 16, 1, ch, sr, br, ba, bits))
    buf.write(b'data')
    buf.write(struct.pack('<I', len(data)))
    buf.write(data)
    return buf.getvalue()


# ── XWB (XACT Wave Bank) extractor ─────────────────────────────────

def extract_xwb(xwb_path, out_dir):
    with open(xwb_path, 'rb') as f:
        raw = f.read()
    if raw[:4] != b'WBND':
        raise RuntimeError("Not an XWB file: %r" % raw[:4])

    pos = 4
    # version
    _u32l(raw, pos); pos += 4

    segs = []
    for _ in range(6):
        off = _u32l(raw, pos); pos += 4
        length = _u32l(raw, pos); pos += 4
        segs.append((off, length))

    idx_off, idx_len = segs[0]
    data_off, data_len = segs[1]
    if idx_off == 0 or data_off == 0:
        raise RuntimeError("Empty index or data segment")

    ip = idx_off
    num_regions = _u32l(raw, ip); ip += 4
    ip += num_regions * 8

    num_entries = _u32l(raw, ip); ip += 4
    if not os.path.isdir(out_dir):
        os.makedirs(out_dir)

    count = 0
    for _ in range(num_entries):
        flags  = _u32l(raw, ip); ip += 4
        offset = _u32l(raw, ip); ip += 4
        size   = _u32l(raw, ip); ip += 4
        # loop offset/size/playlist size
        ip += 12
        name = raw[ip:ip+64].split(b'\x00')[0].decode('ascii', errors='replace')
        ip += 64

        if size == 0:
            continue

        abs_offset = data_off + offset
        wave_data = raw[abs_offset:abs_offset + size]
        wav_path = os.path.join(out_dir, name + ".wav")

        if wave_data[:4] == b'RIFF':
            with open(wav_path, 'wb') as f:
                f.write(wave_data)
        else:
            wav = _raw_to_wav(wave_data, flags)
            with open(wav_path, 'wb') as f:
                f.write(wav)
        count += 1
    return count


def _raw_to_wav(audio, flags):
    is_adpcm = (flags & 0x02) != 0
    channels = 2 if (flags & 0x04) else 1
    sample_rate = 44100
    if is_adpcm:
        bits = 4
        block_align = (channels + 2) * channels
    else:
        bits = 16 if (flags & 0x10) else 8
        block_align = channels * (bits // 8)
    byte_rate = sample_rate * block_align
    fmt_tag = 2 if is_adpcm else 1

    buf = io.BytesIO()
    buf.write(b'RIFF')
    buf.write(struct.pack('<I', 36 + len(audio)))
    buf.write(b'WAVE')
    buf.write(b'fmt ')
    if is_adpcm:
        extra = struct.pack('<HH', block_align, 7)
        buf.write(struct.pack('<IHHIIHH', 18, fmt_tag, channels,
                              sample_rate, byte_rate, block_align, bits))
        buf.write(extra)
    else:
        buf.write(struct.pack('<IHHIIHH', 16, fmt_tag, channels,
                              sample_rate, byte_rate, block_align, bits))
    buf.write(b'data')
    buf.write(struct.pack('<I', len(audio)))
    buf.write(audio)
    return buf.getvalue()


# ── Main extraction engine ─────────────────────────────────────────

def extract_all(content_dir, output_dir, verbose=False, skip_xwb=False):
    content_dir = os.path.abspath(content_dir)
    output_dir  = os.path.abspath(output_dir)

    if not os.path.isdir(content_dir):
        raise OSError("Not a directory: %s" % content_dir)

    stats = {"total": 0, "png": 0, "wav": 0, "skip": 0, "err": 0}

    for root, dirs, files in os.walk(content_dir):
        rel = os.path.relpath(root, content_dir)
        if rel == '.':
            rel = ''

        for fn in files:
            if fn.lower().endswith('.xnb'):
                stats["total"] += 1
                xnb_path = os.path.join(root, fn)
                stem = fn[:-4]
                rel_stem = os.path.join(rel, stem) if rel else stem

                if verbose:
                    sys.stdout.write("  [%4d] %s.xnb ... " % (stats['total'], rel_stem))
                    sys.stdout.flush()

                try:
                    with open(xnb_path, 'rb') as f:
                        raw = f.read()
                except OSError as e:
                    stats["err"] += 1
                    if verbose:
                        print("IOERR: %s" % e)
                    continue

                obj = parse_xnb(raw)
                if obj is None:
                    stats["skip"] += 1
                    if verbose:
                        print("skip")
                    continue

                typ = obj.get("_type", "")

                try:
                    if typ == "texture2d":
                        rgba, w, h = texture_to_rgba(obj)
                        out_path = os.path.join(output_dir, rel_stem) + ".png"
                        out_dirname = os.path.dirname(out_path)
                        if out_dirname and not os.path.isdir(out_dirname):
                            os.makedirs(out_dirname)
                        _write_png(rgba, w, h, out_path)
                        stats["png"] += 1
                        if verbose:
                            print("PNG %dx%d" % (w, h))

                    elif typ == "soundeffect":
                        wav = soundeffect_to_wav(obj)
                        out_path = os.path.join(output_dir, rel_stem) + ".wav"
                        out_dirname = os.path.dirname(out_path)
                        if out_dirname and not os.path.isdir(out_dirname):
                            os.makedirs(out_dirname)
                        with open(out_path, 'wb') as f:
                            f.write(wav)
                        stats["wav"] += 1
                        if verbose:
                            print("WAV %dB" % len(wav))

                    else:
                        stats["skip"] += 1
                        if verbose:
                            print("skip (%s)" % typ)

                except ImportError:
                    stats["err"] += 1
                    if verbose:
                        print("NEED_PKG: pip install pillow")
                except Exception as e:
                    stats["err"] += 1
                    if verbose:
                        print("ERR: %s" % e)
                continue

            if not skip_xwb and fn.lower().endswith('.xwb'):
                xwb_path = os.path.join(root, fn)
                stem = fn[:-4]
                if rel:
                    xwb_out = os.path.join(output_dir, rel, stem)
                else:
                    xwb_out = os.path.join(output_dir, stem)

                if verbose:
                    sys.stdout.write("  [XWB] %s ... " % fn)
                    sys.stdout.flush()
                try:
                    n = extract_xwb(xwb_path, xwb_out)
                    if verbose:
                        print("%d waves" % n)
                except Exception as e:
                    if verbose:
                        print("ERR: %s" % e)

    return stats


def _find_content():
    cands = [
        r"D:\Program Files (x86)\Steam\steamapps\common\Terraria\Content",
        r"C:\Program Files (x86)\Steam\steamapps\common\Terraria\Content",
        r"D:\SteamLibrary\steamapps\common\Terraria\Content",
        r"C:\Program Files\Steam\steamapps\common\Terraria\Content",
        r"E:\Steam\steamapps\common\Terraria\Content",
    ]
    if os.name == 'nt':
        pf = os.environ.get("ProgramFiles(x86)", "")
        if pf:
            cands.append(os.path.join(pf, r"Steam\steamapps\common\Terraria\Content"))
    for c in cands:
        if os.path.isdir(c):
            return c
    return None


def main():
    ap = argparse.ArgumentParser(
        description="Terraria XNB Asset Extractor v%s" % __version__)
    ap.add_argument('-i', '--input', default=None,
                    help="Terraria Content/ path (auto-detect if omitted)")
    ap.add_argument('-o', '--output', default='./terraria_assets',
                    help="Output path (default: ./terraria_assets)")
    ap.add_argument('-v', '--verbose', action='store_true',
                    help="Print per-file progress")
    ap.add_argument('--skip-xwb', action='store_true',
                    help="Skip XWB wave banks")
    args = ap.parse_args()

    content = args.input or _find_content()
    if not content:
        print("ERROR: Cannot find Terraria Content/. Specify --input")
        return 1
    if not os.path.isdir(content):
        print("ERROR: Not a directory: %s" % content)
        return 1

    abspath = os.path.abspath(args.output)
    print("Content: %s" % content)
    print("Output:  %s" % abspath)
    print()

    stats = extract_all(content, args.output,
                        verbose=args.verbose, skip_xwb=args.skip_xwb)

    print()
    print("=" * 55)
    print("  Processed: %d XNB files" % stats['total'])
    print("  Images (PNG):   %d" % stats['png'])
    print("  Sounds (WAV):   %d" % stats['wav'])
    print("  Skipped:  %d" % stats['skip'])
    print("  Errors:   %d" % stats['err'])
    print("=" * 55)
    print("  Saved to: %s" % abspath)
    return 0 if stats['err'] == 0 else 1


if __name__ == '__main__':
    sys.exit(main())
