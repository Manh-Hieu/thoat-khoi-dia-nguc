#!/usr/bin/env python3
"""Minh anime chibi - thick bangs covering forehead, dark hair, red eyes, black hoodie"""
import struct, zlib, os

def make_png(w, h, pixels):
    def pack_row(rp):
        row = bytearray([0])
        for r,g,b,a in rp: row += bytes([r,g,b,a])
        return bytes(row)
    raw = b''.join(pack_row(pixels[y*w:(y+1)*w]) for y in range(h))
    comp = zlib.compress(raw, 9)
    def chunk(name, data):
        c = name + data
        return struct.pack('>I', len(data)) + c + struct.pack('>I', zlib.crc32(c) & 0xffffffff)
    sig = b'\x89PNG\r\n\x1a\n'
    ihdr = chunk(b'IHDR', struct.pack('>IIBBBBB', w, h, 8, 6, 0, 0, 0))
    return sig + ihdr + chunk(b'IDAT', comp) + chunk(b'IEND', b'')

def hex_rgba(h, a=255):
    h = h.lstrip('#')
    return (int(h[:2],16), int(h[2:4],16), int(h[4:],16), a)

COLOR = {
    '.': (0,0,0,0),
    'H': hex_rgba('#1C1714'),  # hair near-black
    'h': hex_rgba('#3A2828'),  # hair highlight
    'g': hex_rgba('#2B1E1E'),  # hair shadow
    'S': hex_rgba('#FFF2EA'),  # skin pale
    's': hex_rgba('#D8B8A0'),  # skin shadow
    'R': hex_rgba('#CC1A1A'),  # eye red
    'r': hex_rgba('#880000'),  # eye dark
    'W': hex_rgba('#FFFFFF'),  # sparkle
    'C': hex_rgba('#1A1A1A'),  # hoodie black
    'c': hex_rgba('#2E2E2E'),  # hoodie lighter
    'J': hex_rgba('#CC1A1A'),  # red accent
    'j': hex_rgba('#880000'),  # red dark
    'L': hex_rgba('#DDDDDD'),  # string
    'P': hex_rgba('#111111'),  # pants
    'p': hex_rgba('#0A0A0A'),  # pants shadow
    'K': hex_rgba('#222222'),  # shoes
    'k': hex_rgba('#0A0A0A'),  # shoes dark
}

def px(grid): return [COLOR.get(ch, (0,0,0,0)) for row in grid for ch in row]
def mirror(grid): return [r[::-1] for r in grid]

def save(path, grid):
    assert len(grid)==32, f"{path}: {len(grid)} rows"
    for i,r in enumerate(grid):
        assert len(r)==16, f"{path} row{i}: {len(r)} '{r}'"
    with open(path, 'wb') as f:
        f.write(make_png(16, 32, px(grid)))
    print(f"  ✓ {os.path.basename(path)}")

BASE = "Assets/Sprites/Characters/Minh"

# ── FRONT HEAD: 12 rows. Big hair, bangs cover forehead ──────
FH = [
    "....HHHHHHHH....",  # 0  hair top
    "..HHHHHHHHHHHH..",  # 1  hair wide
    ".HHHHHhHHHHHHH..",  # 2  bangs sweeping across
    "HHHHhSSSSSHHHHHH",  # 3  bangs: only 5 skin in center
    "HHHHSSSSSSSSHHhH",  # 4  face open
    "HHHHSRrssRrsSHHH",  # 5  red eyes
    "HHHHSWRssWRsWHHH",  # 6  sparkle
    "HHHHSSSSSSSSSHgH",  # 7  lower face
    "HHHHSSSsSSSSSHHH",  # 8  cheeks
    ".HHHhSSSSSSHHHH.",  # 9  chin
    "..HHhSSSSSsHHHh.",  # 10 chin narrow
    "....SSSSSSSS....",  # 11 neck
]

# ── BACK HEAD: 12 rows ────────────────────────────────────────
BH = [
    "....HHHHHHHH....",  # 0
    "..HHHHHHHHHHHH..",  # 1
    ".HHHHHHHHHHHHHH.",  # 2
    "HHHHHhHhHHHHHHHH",  # 3
    "HHHHHHHHHHHHHHgH",  # 4
    "HHHHHHHHHHHHHHgH",  # 5
    "HHHHHHHhHHHHHHgH",  # 6
    "HHHHHHHHHHHHHHgH",  # 7
    ".HHHHHhHHHHHHHH.",  # 8
    "..HHHhHHHHHHHH..",  # 9
    "....HHHHHHHH....",  # 10
    "....SSSSSSSS....",  # 11 neck
]

# ── RIGHT SIDE HEAD: 12 rows ──────────────────────────────────
RH = [
    "................",  # 0
    ".....HHHHHHH....",  # 1
    "....HHHhHHHHHH..",  # 2
    "...HHHhSSSSSHHH.",  # 3 bangs
    "..HHHHSSSSSSHHhH",  # 4
    "..HHHHSRrSSSHHhH",  # 5 eye
    "..HHHHSWRsSSSHHH",  # 6 sparkle
    "..HHHHSSSSSSHHgH",  # 7
    "..HHHHSSSSSsHHH.",  # 8
    "...HHHhSSSSSHHH.",  # 9
    "....HHhSSSSSHH..",  # 10
    "....SSSSSSSS....",  # 11 neck
]

# ── HOODIE FRONT: 7 rows ──────────────────────────────────────
HF = [
    "....CJSSSSJCC...",  # 12
    "...CCJLssLJCCC..",  # 13
    "..CCcCCCCCCcCC..",  # 14
    "..CCCCCCCCCCcC..",  # 15
    "..CCCJjJjJCCCC..",  # 16
    "...PPPPPPPPPP...",  # 17
    "....PPpPPpPP....",  # 18
]

# ── HOODIE SIDE: 7 rows ───────────────────────────────────────
HS = [
    "....CJSSSJCC....",  # 12
    "...CCJLsLJCCC...",  # 13
    "...CCcCCCCcCCC..",  # 14
    "...CCCCCCCCcCC..",  # 15
    "...CCCJjJCCCC...",  # 16
    "....PPPPPPPP....",  # 17
    "....PPpPPpPP....",  # 18
]

# ── LEGS: 4 rows per style ────────────────────────────────────
LN  = ["....PP...PP.....", "....PP...PP.....",
       "...KKKk.kKKKK...", "..KKKKk..kKKKK.."]
LA  = ["...PP.....PP....", "...PP.....PP....",
       "..KKKk...kKKKK..", ".KKKKk...kKKKKk."]
LT  = [".....PP.PP......", ".....PP.PP......",
       "....KKk.kKK.....", "...KKKk.kKKKK..."]
LRN = ["....PP...PP.....", "....PP...PP.....",
       "...KKKk.kKKKK...", "..KKKKk..kKKKK.."]
LRA = ["....PP....PP....", "...PP.....PP....",
       "..KKKk...kKKKK..", ".KKKKk...kKKKKk."]
LRT = [".....PP..PP.....", "....PP...PP.....",
       "....KKk.kKKK....", "...KKKk.kKKKK..."]

# ── VALIDATE ALL ──────────────────────────────────────────────
all_parts = {'FH':FH,'BH':BH,'RH':RH,'HF':HF,'HS':HS,
             'LN':LN,'LA':LA,'LT':LT,'LRN':LRN,'LRA':LRA,'LRT':LRT}
for name, rows in all_parts.items():
    for i, r in enumerate(rows):
        assert len(r)==16, f"{name}[{i}] len={len(r)}: '{r}'"
    print(f"  {name}: {len(rows)} rows OK")

E = ["................"] * 9

def make(head, body, legs):
    rows = head + body + legs + E
    assert len(rows)==32, f"Total={len(rows)}: head={len(head)} body={len(body)} legs={len(legs)} E=9"
    return rows

os.makedirs(BASE, exist_ok=True)
print("\nGenerating sprites...")

lh = mirror(RH)
sprites = {
    'Minh_Idle_Down':    make(FH, HF, LN),
    'Minh_Walk_Down_1':  make(FH, HF, LA),
    'Minh_Walk_Down_2':  make(FH, HF, LT),
    'Minh_Idle_Up':      make(BH, HF, LN),
    'Minh_Walk_Up_1':    make(BH, HF, LA),
    'Minh_Walk_Up_2':    make(BH, HF, LT),
    'Minh_Idle_Right':   make(RH, HS, LRN),
    'Minh_Walk_Right_1': make(RH, HS, LRA),
    'Minh_Walk_Right_2': make(RH, HS, LRT),
    'Minh_Idle_Left':    make(lh, HS, LRN),
    'Minh_Walk_Left_1':  make(lh, HS, LRA),
    'Minh_Walk_Left_2':  make(lh, HS, LRT),
}

for name, grid in sprites.items():
    save(f"{BASE}/{name}.png", grid)

print(f"\nDone! {len(sprites)} sprites.")
