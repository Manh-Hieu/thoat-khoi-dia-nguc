import os
from PIL import Image

# Guard Color Palette (Navy Uniform, Gold Details, Badass Glasses)
palette_guard = {
    '.': (0, 0, 0, 0),
    'o': (13, 8, 4, 255),       # Hair/face black outline
    'O': (11, 26, 44, 255),     # Clothing dark outline
    'C': (28, 40, 51, 255),     # Navy blue base (cap/pants)
    'c': (46, 64, 83, 255),     # Navy blue highlight
    'd': (27, 38, 49, 255),     # Navy blue shadow
    'e': (65, 90, 119, 255),    # Pants highlight
    'U': (174, 214, 241, 255),  # Shirt light blue
    'u': (133, 193, 233, 255),  # Shirt shadow
    'T': (17, 23, 32, 255),     # Tie/Belt black
    'B': (244, 208, 63, 255),   # Epaulets/Badge gold
    'W': (242, 243, 244, 255),  # Silver badge
    'S': (255, 223, 208, 255),  # Skin main
    's': (232, 188, 167, 255),  # Skin shadow
    'H': (45, 32, 26, 255),     # Hair dark brown
    'G': (17, 17, 17, 255),     # Sunglasses lens
    'E': (255, 255, 255, 255),  # Sunglasses reflection
    'K': (26, 26, 30, 255),     # Boot base
    'k': (15, 15, 18, 255),     # Boot shadow
    'n': (9, 9, 11, 255)        # Boot sole
}

# Recruiter Color Palette (Sleek Charcoal Suit, Confident Sharp Face, Red Tie)
palette_recruiter = {
    '.': (0, 0, 0, 0),
    'o': (13, 8, 4, 255),       # Hair/face black outline
    'O': (27, 38, 49, 255),     # Clothing dark outline
    'C': (44, 62, 80, 255),     # Suit jacket (dark charcoal)
    'c': (52, 73, 94, 255),     # Suit highlight
    'd': (26, 37, 48, 255),     # Suit shadow
    'e': (52, 73, 94, 255),     # Highlight
    'W': (255, 255, 255, 255),  # Shirt white
    'w': (230, 230, 230, 255),  # Shirt shadow (light grey)
    'R': (231, 76, 60, 255),    # Red tie
    'r': (192, 57, 43, 255),    # Red tie shadow
    'T': (17, 23, 32, 255),     # Belt black
    'B': (244, 208, 63, 255),   # Belt buckle gold
    'S': (255, 223, 208, 255),  # Skin main
    's': (232, 188, 167, 255),  # Skin shadow
    'H': (26, 26, 30, 255),     # Slicked back black hair base
    'h': (63, 66, 87, 255),     # Slicked hair highlight
    'E': (28, 17, 5, 255),      # Eyes pupil
    'K': (26, 26, 26, 255),     # Shoes black
    'k': (13, 13, 13, 255),     # Shoes shadow
    'n': (7, 7, 7, 255)         # Shoes sole
}

# ==================== GUARD GRIDS ====================

GH_DOWN = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    ".............oooooo.............", # 4 (Cap top)
    "...........ooCCCCCCoo...........", # 5
    "..........ooCCCCCCCCoo..........", # 6
    ".........ooCCCBBBBCCCoo.........", # 7 (Gold badge)
    ".........ooCCCBBBBCCCoo.........", # 8
    "........ooCCCCcccccccCCoo.......", # 9 (Visor brim)
    "........oocccccccccccccoo.......", # 10
    "........ooHSSSSSSSSSSSHoo.......", # 11 (Head/ear width)
    "........ooHSSSSSSSSSSSHoo.......", # 12
    "........ooHSooSSSSooHSHoo.......", # 13
    "........ooHSGGEEEGGESSHoo.......", # 14 (badass sunglasses)
    "........ooHSGGeeeGGESSHoo.......", # 15
    "........ooHSSSSSSSSSSSHoo.......", # 16
    "........ooHSSSSSSSSSSSHoo.......", # 17
    ".........ooHSSSSSSSSSHoo.........", # 18
    "..........ooHSSssSSHoo..........", # 19
    "...........ooHSSSShoo...........", # 20
    "..............oSSo..............", # 21 (Neck)
    "..............oSSo.............."  # 22
]

TORSO_GUARD_DOWN = [
    "...........OOoTTTToOO...........", # 23
    ".........OOOBBBTTTBBBOOO........", # 24 (Epaulets B, broad shoulders)
    "........OOBBBBBuuTuuBBbBOO......", # 25
    "......OObbUUUUuuTuuUUUUbbOO......", # 26 (Broad blue shirt, tie T)
    "......OObbUUUUuuTuuUUUUbbOO......", # 27
    ".....OObbUUUUWuuTuuuUUUUbbOO.....", # 28 (Silver badge W)
    ".....OObbUUUUWuuTuuuUUUUbbOO.....", # 29
    ".....OObbUUUUuuuTuuuuUUUbbOO.....", # 30
    ".....OObbUUUUuuuTuuuuUUUbbOO.....", # 31
    ".....OObbUUUUuuuTuuuuUUUbbOO.....", # 32
    ".....OObbUUUUuuuuuuuuUUUbbOO.....", # 33
    "......OSSSUUUUUUUUUUUUUUUSSSO.....", # 34 (Hands S start)
    "......OSSSOOOOOOOOOOOOOOOSSSO.....", # 35
    "......OSSSOOOOOOOOOOOOOOOSSSO....."  # 36 (Black belt)
]

LEGS_GUARD_DOWN = [
    "......OSSSOOOOOOOOOOOOOOOSSSO.....", # 37
    "......OSSSOooTTTTTTTTTooOSSSO.....", # 38
    "......OSSSOooTTTBBTTTooOSSSO.....", # 39 (Gold buckle)
    ".........OOoCCCCdCCCCoOO........", # 40 (Navy pants C)
    "........OOCeCCCCdCCCCeCOO.......", # 41
    "........OOCeCCCCdCCCCeCOO.......", # 42
    "........OCeCCCd....dCCeCCO......", # 43 (Stride gap)
    "........OCeCCCd....dCCeCCO......", # 44
    "........OCeCCCd....dCCeCCO......", # 45
    "........OCeCCCd....dCCeCCO......", # 46
    "........OCeCCCd....dCCeCCO......", # 47
    "........OCeCCCd....dCCeCCO......", # 48
    "........OCeCCCd....dCCeCCO......", # 49
    "........OCeCCCd....dCCeCCO......", # 50
    "........OCeCCCd....dCCeCCO......", # 51
    "........OCeCCCd....dCCeCCO......", # 52
    "........OCeCCCd....dCCeCCO......", # 53
    "........OCeCCCd....dCCeCCO......", # 54
    "........OCeCCCd....dCCeCCO......", # 55
    "........OCeCCCd....dCCeCCO......", # 56
    "........OCeCCCd....dCCeCCO......", # 57
    "........Okkkkkd....dkkkkkO......", # 58 (Boots k)
    ".......OKKKKKKd....dKKKKKKO.....", # 59 (Wide boots for "lực" look)
    ".......OKKKKKKd....dKKKKKKO.....", # 60
    ".......Onnnnnnd....dnnnnnnO.....", # 61
    "................................", # 62
    "................................"  # 63
]

GH_UP = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    "-------------oooooo-------------", # 4 (Cap top back)
    "-----------ooCCCCCCoo-----------", # 5
    "----------ooCCCCCCCCoo----------", # 6
    "---------ooCCCCCCCCCCoo---------", # 7
    "---------ooCCCCCCCCCCoo---------", # 8
    "--------ooCCCCCCCCCCCCoo--------", # 9
    "--------oocccccccccccccoo-------", # 10
    "--------ooHHHHHHHHHHHHHoo-------", # 11 (Back hair)
    "--------ooHHHHHHHHHHHHHoo-------", # 12
    "--------ooHHHHHHHHHHHHHoo-------", # 13
    "--------ooHHHHHHHHHHHHHoo-------", # 14
    "--------ooHHHHHHHHHHHHHoo-------", # 15
    "--------ooHHHHHHHHHHHHHoo-------", # 16
    "--------ooHHHHHHHHHHHHHoo-------", # 17
    "---------ooHHHHHHHHHHHoo---------", # 18
    "----------ooHHHHHHHHHoo----------", # 19
    "-----------ooHHHHHHHoo-----------", # 20
    "--------------oSSo--------------", # 21 (Neck)
    "--------------oSSo--------------"  # 22
]

TORSO_GUARD_UP = [
    "...........OOooooooOO...........", # 23
    ".........OOOBBBdddBbBOOO........", # 24 (Back epaulets)
    "........OOBBBBBdddddBbBBOO......", # 25
    "......OObbUUUUddddddUUUUbbOO......", # 26
    "......OObbUUUUddddddUUUUbbOO......", # 27
    ".....OObbUUUUUdddddddUUUUbbOO.....", # 28
    ".....OObbUUUUUdddddddUUUUbbOO.....", # 29
    ".....OObbUUUUUdddddddUUUUbbOO.....", # 30
    ".....OObbUUUUUdddddddUUUUbbOO.....", # 31
    ".....OObbUUUUUdddddddUUUUbbOO.....", # 32
    ".....OObbUUUUUdddddddUUUUbbOO.....", # 33
    "......OSSSUUUUUUUUUUUUUUUSSSO.....", # 34
    "......OSSSOOOOOOOOOOOOOOOSSSO.....", # 35
    "......OSSSOOOOOOOOOOOOOOOSSSO....."  # 36
]

LEGS_GUARD_UP = [
    "......OSSSOOOOOOOOOOOOOOOSSSO.....", # 37
    "......OSSSOooTTTTTTTTTooOSSSO.....", # 38
    "......OSSSOooTTTTTTTTTooOSSSO.....", # 39
    ".........OOoCCCCdCCCCoOO........", # 40
    "........OOCeCCCCdCCCCeCOO.......", # 41
    "........OOCeCCCCdCCCCeCOO.......", # 42
    "........OCeCCCd....dCCeCCO......", # 43
    "........OCeCCCd....dCCeCCO......", # 44
    "........OCeCCCd....dCCeCCO......", # 45
    "........OCeCCCd....dCCeCCO......", # 46
    "........OCeCCCd....dCCeCCO......", # 47
    "........OCeCCCd....dCCeCCO......", # 48
    "........OCeCCCd....dCCeCCO......", # 49
    "........OCeCCCd....dCCeCCO......", # 50
    "........OCeCCCd....dCCeCCO......", # 51
    "........OCeCCCd....dCCeCCO......", # 52
    "........OCeCCCd....dCCeCCO......", # 53
    "........OCeCCCd....dCCeCCO......", # 54
    "........OCeCCCd....dCCeCCO......", # 55
    "........OCeCCCd....dCCeCCO......", # 56
    "........OCeCCCd....dCCeCCO......", # 57
    "........Okkkkkd....dkkkkkO......", # 58
    ".......OKKKKKKd....dKKKKKKO.....", # 59
    ".......OKKKKKKd....dKKKKKKO.....", # 60
    ".......Onnnnnnd....dnnnnnnO.....", # 61
    "................................", # 62
    "................................"  # 63
]

GH_GUARD_LEFT = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    "-------------oooooo-------------", # 4
    "-----------ooCCCCCCo------------", # 5
    "----------ooCCCCCCCCo-----------", # 6
    ".......ooooCCCBBBBCCCoo---------", # 7 (Visor cap left jutting out)
    "......ooccccccBBBBCCCoo---------", # 8
    "......oocccccccccccccc----------", # 9
    ".......oocccccccccco------------", # 10
    ".........ooHSSSSSSSHoo----------", # 11 (Badass profile)
    ".........ooHSSGGSSSsHoo---------", # 12 (Sunglasses profile)
    ".........ooHSGGEESSsHoo---------", # 13
    ".........ooHSSGGSSSsHoo---------", # 14
    ".........ooHSSSSSSSsHoo---------", # 15
    ".........ooHSSSSSSSsHoo---------", # 16
    ".........ooHSSSSSSSsHoo---------", # 17
    "..........ooHSSSSSsHoo----------", # 18
    "...........ooHSSssHoo-----------", # 19
    "............ooHSShoo------------", # 20
    "--------------oSSo--------------", # 21
    "--------------oSSo--------------"  # 22
]

GT_GUARD_LEFT = [
    "...........OOoTTTTToOO..........", # 23
    ".........OOOBBBTTTBBBOOO........", # 24
    "........OOBBBBBuuTuuBBbBOO......", # 25
    ".......OObbUUUUuuTuuUUUbbOO.....", # 26
    ".......OObbUUUUuuTuuUUUbbOO.....", # 27
    "......OObbUUUUUuuTuuUUUUbbOO....", # 28
    "......OObbUUUUUuuTuuUUUUbbOO....", # 29
    "......OObbUUUUUuuTuuUUUUbbOO....", # 30
    "......OObbUUUUUuuTuuUUUUbbOO....", # 31
    "......OObbUUUUUuuTuuUUUUbbOO....", # 32
    "......OObbUUUUUuuTuuUUUUbbOO....", # 33
    ".......OSSSUUUUUUUUUUUUUSSSO....", # 34
    ".......OSSSOOOOOOOOOOOOOSSSO....", # 35
    ".......OSSSOOOOOOOOOOOOOSSSO...."  # 36
]

GL_GUARD_LEFT = [
    ".......OSSSOOOOOOOOOOOOOSSSO....", # 37
    ".......OSSSOooTTTTTTTooOSSSO....", # 38
    ".......OSSSOooTTTTTTTooOSSSO....", # 39
    ".........OOoCCCCdCCCCoOO........", # 40
    "........OOCeCCCCdCCCCeCOO.......", # 41
    "........OOCeCCCCdCCCCeCOO.......", # 42
    "........OCeCCCd....dCCeCCO......", # 43
    "........OCeCCCd....dCCeCCO......", # 44
    "........OCeCCCd....dCCeCCO......", # 45
    "........OCeCCCd....dCCeCCO......", # 46
    "........OCeCCCd....dCCeCCO......", # 47
    "........OCeCCCd....dCCeCCO......", # 48
    "........OCeCCCd....dCCeCCO......", # 49
    "........OCeCCCd....dCCeCCO......", # 50
    "........OCeCCCd....dCCeCCO......", # 51
    "........OCeCCCd....dCCeCCO......", # 52
    "........OCeCCCd....dCCeCCO......", # 53
    "........OCeCCCd....dCCeCCO......", # 54
    "........OCeCCCd....dCCeCCO......", # 55
    "........OCeCCCd....dCCeCCO......", # 56
    "........OCeCCCd....dCCeCCO......", # 57
    "........Okkkkkd....dkkkkkO......", # 58
    ".......OKKKKKKd....dKKKKKKO.....", # 59
    ".......OKKKKKKd....dKKKKKKO.....", # 60
    ".......Onnnnnnd....dnnnnnnO.....", # 61
    "................................", # 62
    "................................"  # 63
]

# ==================== RECRUITER GRIDS ====================

RH_DOWN = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    ".............oooooo.............", # 4
    "...........oohhhhhhoo...........", # 5 (slicked hair top)
    "..........oohHHHHHHHhoo.........", # 6
    ".........oohHHHHHHHHHhoo........", # 7
    ".........ooHHHHHHHHHHHoo........", # 8
    "........oohHHHHHHHHHHHhoo.......", # 9
    "........ooHhShHSSHhShHoo........", # 10 (slicked hairline framing face)
    "........ooHhSSSsSSShHoo.........", # 11
    "........ooHSSSSSSSSSSHoo........", # 12
    "........ooHSooSSSSooSHoo........", # 13
    "........ooHSeESSSSEeSHoo........", # 14 (confident sharp dark eyes)
    "........ooHSSSSSSSSSSHoo........", # 15
    "........ooHSSSSSSSSSSHoo........", # 16
    "........ooHSSSSSSSSSSHoo........", # 17
    ".........ooHSSSSSSSSSHoo.........", # 18
    "..........ooHSSssSSHoo..........", # 19
    "...........ooHSSSShoo...........", # 20
    "--------------oSSo--------------", # 21
    "--------------oSSo--------------"  # 22
]

TORSO_RECRUITER_DOWN = [
    "...........OOoWWRRWoOO..........", # 23 (Collar white W, red tie R)
    ".........OOOCCWRRWCCOOO.........", # 24 (Charcoal suit jacket C, broad shoulders)
    "........OOCCCCWwRRwCCCCOOO......", # 25
    "......OObbCCCCWwRRwCCCCbbOO.....", # 26 (Secret agent V-taper)
    "......OObbCCCCWwRRwCCCCbbOO.....", # 27
    ".....OObbCCCCCWwRRwCCCCCbbOO....", # 28
    ".....OObbCCCCCCRRCCCCCCbbOO.....", # 29
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 30
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 31
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 32
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 33
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 34
    "......OSSSCCCCCCCCCCCCCCSSSO....", # 35 (Hands S)
    "......OSSSCCCCCCCCCCCCCCSSSO...."  # 36
]

LEGS_RECRUITER_DOWN = [
    "......OSSSCCCCCCCCCCCCCCSSSO....", # 37
    "......OSSSOooTTTTTTTTTooOSSSO....", # 38
    "......OSSSOooTTTBBTTTooOSSSO....", # 39
    ".........OOoCCCCdCCCCoOO........", # 40
    "........OOCeCCCCdCCCCeCOO.......", # 41
    "........OOCeCCCCdCCCCeCOO.......", # 42
    "........OCeCCCd....dCCeCCO......", # 43
    "........OCeCCCd....dCCeCCO......", # 44
    "........OCeCCCd....dCCeCCO......", # 45
    "........OCeCCCd....dCCeCCO......", # 46
    "........OCeCCCd....dCCeCCO......", # 47
    "........OCeCCCd....dCCeCCO......", # 48
    "........OCeCCCd....dCCeCCO......", # 49
    "........OCeCCCd....dCCeCCO......", # 50
    "........OCeCCCd....dCCeCCO......", # 51
    "........OCeCCCd....dCCeCCO......", # 52
    "........OCeCCCd....dCCeCCO......", # 53
    "........OCeCCCd....dCCeCCO......", # 54
    "........OCeCCCd....dCCeCCO......", # 55
    "........OCeCCCd....dCCeCCO......", # 56
    "........OCeCCCd....dCCeCCO......", # 57
    "........Okkkkkd....dkkkkkO......", # 58
    ".......OKKKKKKd....dKKKKKKO.....", # 59
    ".......OKKKKKKd....dKKKKKKO.....", # 60
    ".......Onnnnnnd....dnnnnnnO.....", # 61
    "................................", # 62
    "................................"  # 63
]

RH_UP = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    ".............oooooo.............", # 4
    "...........oohhhhhhoo...........", # 5 (Back slicked hair)
    "..........oohHHHHHHHhoo.........", # 6
    ".........oohHHHHHHHHHhoo........", # 7
    ".........ooHHHHHHHHHHHoo........", # 8
    "........ooHHHHHHHHHHHHHoo.......", # 9
    "........ooHHHHHHHHHHHHHoo.......", # 10
    "--------ooHHHHHHHHHHHHHoo-------", # 11
    "--------ooHHHHHHHHHHHHHoo-------", # 12
    "--------ooHHHHHHHHHHHHHoo-------", # 13
    "--------ooHHHHHHHHHHHHHoo-------", # 14
    "--------ooHHHHHHHHHHHHHoo-------", # 15
    "--------ooHHHHHHHHHHHHHoo-------", # 16
    "--------ooHHHHHHHHHHHHHoo-------", # 17
    "---------ooHHHHHHHHHHHoo--------", # 18
    "----------ooHHHHHHHHHoo---------", # 19
    "-----------ooHHHHHHHoo----------", # 20
    "--------------oSSo--------------", # 21
    "--------------oSSo--------------"  # 22
]

TORSO_RECRUITER_UP = [
    "...........OOooooooOO...........", # 23
    ".........OOOCCddddCCCOO.........", # 24 (Back of broad charcoal suit)
    "........OOCCCCddddddCCCCOOO.....", # 25
    "......OObbCCCCddddddCCCCbbOO....", # 26
    "......OObbCCCCddddddCCCCbbOO....", # 27
    ".....OObbCCCCCdddddddCCCCCbbOO..", # 28
    ".....OObbCCCCCCdddddddCCCCCbbOO.", # 29
    ".....OObbCCCCCCdddddddCCCCCbbOO.", # 30
    ".....OObbCCCCCCdddddddCCCCCbbOO.", # 31
    ".....OObbCCCCCCdddddddCCCCCbbOO.", # 32
    ".....OObbCCCCCCdddddddCCCCCbbOO.", # 33
    "......OSSSCCCCCCCCCCCCCCSSSO....", # 34
    "......OSSSCCCCCCCCCCCCCCSSSO....", # 35
    "......OSSSCCCCCCCCCCCCCCSSSO...."  # 36
]

LEGS_RECRUITER_UP = [
    "......OSSSCCCCCCCCCCCCCCSSSO....", # 37
    "......OSSSOooTTTTTTTTTooOSSSO....", # 38
    "......OSSSOooTTTTTTTTTooOSSSO....", # 39
    ".........OOoCCCCdCCCCoOO........", # 40
    "........OOCeCCCCdCCCCeCOO.......", # 41
    "........OOCeCCCCdCCCCeCOO.......", # 42
    "........OCeCCCd....dCCeCCO......", # 43
    "........OCeCCCd....dCCeCCO......", # 44
    "........OCeCCCd....dCCeCCO......", # 45
    "........OCeCCCd....dCCeCCO......", # 46
    "........OCeCCCd....dCCeCCO......", # 47
    "........OCeCCCd....dCCeCCO......", # 48
    "........OCeCCCd....dCCeCCO......", # 49
    "........OCeCCCd....dCCeCCO......", # 50
    "........OCeCCCd....dCCeCCO......", # 51
    "........OCeCCCd....dCCeCCO......", # 52
    "........OCeCCCd....dCCeCCO......", # 53
    "........OCeCCCd....dCCeCCO......", # 54
    "........OCeCCCd....dCCeCCO......", # 55
    "........OCeCCCd....dCCeCCO......", # 56
    "........OCeCCCd....dCCeCCO......", # 57
    "........Okkkkkd....dkkkkkO......", # 58
    ".......OKKKKKKd....dKKKKKKO.....", # 59
    ".......OKKKKKKd....dKKKKKKO.....", # 60
    ".......Onnnnnnd....dnnnnnnO.....", # 61
    "................................", # 62
    "................................"  # 63
]

RH_RECRUITER_LEFT = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    ".............oooooo.............", # 4
    "...........oohhhhhho............", # 5
    "..........oohHHHHHHHo...........", # 6
    ".........ooHHHHHHHHHoo..........", # 7
    ".........ooHHHHHHHHHoo..........", # 8
    "........oohHHHHHHHHHhoo.........", # 9
    "........ooHhShHSSHhSHoo.........", # 10
    "........ooHhSSSSSSSShoo.........", # 11
    "........ooHSSSSSsSSSHoo.........", # 12
    "........ooHSSeESSsSSHoo.........", # 13 (sharp eye left)
    "........ooHSSSSSSSSSHoo.........", # 14
    "........ooHSSSSSSSSSHoo.........", # 15
    "........ooHSSSSSSSSSHoo.........", # 16
    "........ooHSSSSSSSSSHoo.........", # 17
    ".........ooHSSSSSSSHoo..........", # 18
    "..........ooHSSssSHoo...........", # 19
    "...........ooHSSShoo............", # 20
    "--------------oSSo--------------", # 21
    "--------------oSSo--------------"  # 22
]

RT_RECRUITER_LEFT = [
    "...........OOoWWRRWoOO..........", # 23
    ".........OOOCCWRRWCCOOO.........", # 24
    "........OOCCCCWwRRwCCCCOOO......", # 25
    "......OObbCCCCWwRRwCCCCbbOO.....", # 26
    "......OObbCCCCWwRRwCCCCbbOO.....", # 27
    ".....OObbCCCCCWwRRwCCCCCbbOO....", # 28
    ".....OObbCCCCCCRRCCCCCCbbOO.....", # 29
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 30
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 31
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 32
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 33
    ".....OObbCCdCCdCCdCCdCCbbOO.....", # 34
    "......OSSSCCCCCCCCCCCCCCSSSO....", # 35
    "......OSSSCCCCCCCCCCCCCCSSSO...."  # 36
]

RL_RECRUITER_LEFT = [
    "......OSSSCCCCCCCCCCCCCCSSSO....", # 37
    "......OSSSOooTTTTTTTTTooOSSSO....", # 38
    "......OSSSOooTTTTTTTTTooOSSSO....", # 39
    ".........OOoCCCCdCCCCoOO........", # 40
    "........OOCeCCCCdCCCCeCOO.......", # 41
    "........OOCeCCCCdCCCCeCOO.......", # 42
    "........OCeCCCd....dCCeCCO......", # 43
    "........OCeCCCd....dCCeCCO......", # 44
    "........OCeCCCd....dCCeCCO......", # 45
    "........OCeCCCd....dCCeCCO......", # 46
    "........OCeCCCd....dCCeCCO......", # 47
    "........OCeCCCd....dCCeCCO......", # 48
    "........OCeCCCd....dCCeCCO......", # 49
    "........OCeCCCd....dCCeCCO......", # 50
    "........OCeCCCd....dCCeCCO......", # 51
    "........OCeCCCd....dCCeCCO......", # 52
    "........OCeCCCd....dCCeCCO......", # 53
    "........OCeCCCd....dCCeCCO......", # 54
    "........OCeCCCd....dCCeCCO......", # 55
    "........OCeCCCd....dCCeCCO......", # 56
    "........OCeCCCd....dCCeCCO......", # 57
    "........Okkkkkd....dkkkkkO......", # 58
    ".......OKKKKKKd....dKKKKKKO.....", # 59
    ".......OKKKKKKd....dKKKKKKO.....", # 60
    ".......Onnnnnnd....dnnnnnnO.....", # 61
    "................................", # 62
    "................................"  # 63
]

def assemble_sprite(head, torso, legs, palette):
    img = Image.new("RGBA", (32, 64), (0, 0, 0, 0))
    pixels = img.load()
    
    for y in range(64):
        if y < 23:
            part = head
            py = y
        elif y < 37:
            part = torso
            py = y - 23
        else:
            part = legs
            py = y - 37
            
        row_str = part[py]
        # Robust padding and truncation to exactly 32 chars
        if len(row_str) < 32:
            row_str = row_str + "." * (32 - len(row_str))
        elif len(row_str) > 32:
            row_str = row_str[:32]
            
        for x in range(32):
            char = row_str[x]
            color = palette.get(char, (0, 0, 0, 0))
            pixels[x, y] = color
            
    return img

def main():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    project_root = os.path.dirname(script_dir)
    dest_dir = os.path.join(project_root, "Assets", "Sprites", "Characters", "Guards")
    os.makedirs(dest_dir, exist_ok=True)

    print(f"Generating high-res 32x64 Guard & Recruiter sprites in: {dest_dir}")

    # 1. Guard Sprites
    assemble_sprite(GH_DOWN, TORSO_GUARD_DOWN, LEGS_GUARD_DOWN, palette_guard).save(os.path.join(dest_dir, "Guard_Idle_Down.png"))
    assemble_sprite(GH_UP, TORSO_GUARD_UP, LEGS_GUARD_UP, palette_guard).save(os.path.join(dest_dir, "Guard_Idle_Up.png"))
    
    left_guard = assemble_sprite(GH_GUARD_LEFT, GT_GUARD_LEFT, GL_GUARD_LEFT, palette_guard)
    left_guard.save(os.path.join(dest_dir, "Guard_Idle_Left.png"))
    left_guard.transpose(Image.FLIP_LEFT_RIGHT).save(os.path.join(dest_dir, "Guard_Idle_Right.png"))

    # 2. Recruiter Sprites
    assemble_sprite(RH_DOWN, TORSO_RECRUITER_DOWN, LEGS_RECRUITER_DOWN, palette_recruiter).save(os.path.join(dest_dir, "Recruiter_Idle_Down.png"))
    assemble_sprite(RH_UP, TORSO_RECRUITER_UP, LEGS_RECRUITER_UP, palette_recruiter).save(os.path.join(dest_dir, "Recruiter_Idle_Up.png"))
    
    left_recruiter = assemble_sprite(RH_RECRUITER_LEFT, RT_RECRUITER_LEFT, RL_RECRUITER_LEFT, palette_recruiter)
    left_recruiter.save(os.path.join(dest_dir, "Recruiter_Idle_Left.png"))
    left_recruiter.transpose(Image.FLIP_LEFT_RIGHT).save(os.path.join(dest_dir, "Recruiter_Idle_Right.png"))

    print("Badass 32x64 NPC sprites successfully generated!")

if __name__ == '__main__':
    main()
