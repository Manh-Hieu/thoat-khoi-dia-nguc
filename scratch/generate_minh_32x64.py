import os
from PIL import Image

def hex_to_rgba(hex_str):
    hex_str = hex_str.lstrip('#')
    r = int(hex_str[0:2], 16)
    g = int(hex_str[2:4], 16)
    b = int(hex_str[4:6], 16)
    return (r, g, b, 255)

# 16-bit professional pixel art color palette
palette = {
    '.': (0, 0, 0, 0),
    # Hair (Dark Charcoal/Black with warm highlight)
    'H': hex_to_rgba('#1a1108'), # Base hair
    'h': hex_to_rgba('#3c2714'), # Hair highlight
    'o': hex_to_rgba('#0d0804'), # Hair outline / dark shadow
    # Skin (Peach, shaded)
    'S': hex_to_rgba('#fcd4b4'), # Main skin
    's': hex_to_rgba('#e2a884'), # Skin shadow
    'c': hex_to_rgba('#ffabb2'), # Cheek blush
    'E': hex_to_rgba('#1c1105'), # Eyes / Pupils
    'e': hex_to_rgba('#ffffff'), # Eye whites
    # Hoodie (Slate Blue)
    'B': hex_to_rgba('#2b5d92'), # Main blue hoodie
    'b': hex_to_rgba('#487bb0'), # Hoodie highlight
    'd': hex_to_rgba('#163558'), # Hoodie shadow
    'O': hex_to_rgba('#0b1a2c'), # Clothing outline
    'w': hex_to_rgba('#e6e6e6'), # Drawstring / Zipper (white)
    # Pants (Steel Grey)
    'P': hex_to_rgba('#505a66'), # Main grey pants
    'p': hex_to_rgba('#343b44'), # Pants shadow
    'l': hex_to_rgba('#727e8c'), # Pants highlight
    # Shoes (Brown Leather)
    'K': hex_to_rgba('#7e4e30'), # Main boot brown
    'k': hex_to_rgba('#52321c'), # Boot shadow
    'n': hex_to_rgba('#121212'), # Boot sole (black)
}

# --- FRONT MODULAR PIECES ---
HEAD_FRONT = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    "................................", # 4
    "................................", # 5
    "................................", # 6
    "............oo.oo.oo............", # 7 (Modern messy top spikes - no giant gap)
    "..........oohhHHHHhhoo..........", # 8 (Layered hair volume with highlights)
    ".........oohHHHHHHHHhoo.........", # 9
    "........oohHHHHHHHHHHHhoo.......", # 10
    "........oohHHHHHHHHHHHhoo.......", # 11
    "........ooHHHHHHHHHHHHoo........", # 12
    "........ooHhShHSSHhShHoo........", # 13 (Symmetric messy bangs falling over face)
    "........ooHhSSSSSSSShHoo........", # 14 (Strands frame the face)
    "........ooHSSSSSSSSSSHoo........", # 15
    "........ooHSooSSSSooSHoo........", # 16 (Eyebrows)
    "........ooHSeESSSSEeSHoo........", # 17 (Eyes: Left eE, Right Ee)
    "........ooHSSSSSSSSSSHoo........", # 18
    ".........ooHSSSSSSSSHoo.........", # 19
    "..........ooHSSssSSHoo..........", # 20 (Chin/Mouth shadow)
    "...........ooHSSSShoo...........", # 21
    "..............oSSo.............."  # 22 (Neck)
]

TORSO_FRONT = [
    "..............OOOO..............", # 23 (Collar)
    "...........OOObbbbOOO...........", # 24 (Shoulders)
    "..........OObbbbbbbOO...........", # 25 (Fixed length: added a dot at the end)
    ".........OObbbbbbbbbOO..........", # 26
    ".........OObBBBBBBBBbOO.........", # 27
    "........OObbbBddddddBbbOO.......", # 28 (Arms start separating)
    "........ObBBdBBBBBBdBBbO........", # 29 (Arm sleeves + body)
    "........ObBBdBBwwBBdBBbO........", # 30 (Zipper/strings start)
    "........ObBBdBBwwBBdBBbO........", # 31
    "........ObBBdBBwwBBdBBbO........", # 32
    "........ObBBdBBwwBBdBBbO........", # 33
    "........ObBBdBBBBBBdBBbO........", # 34
    "........OSSSdBBBBBBdSSSO........", # 35 (Sleeve ends, hands appear)
    "........OSSSdOOOOOOdSSSO........"  # 36 (Waistband / hoodie hem)
]

LEGS_FRONT_IDLE = [
    "........OSSSdOOOOOOdSSSO........", # 37 (Hands continue)
    "........OSSSdOOOOOOdSSSO........", # 38
    "........OSSSdOOOOOOdSSSO........", # 39
    "...........OOpPPPPppO...........", # 40 (Pants waist / hips start)
    "..........OOpPPPPPPppO..........", # 41
    "..........OOpPPPPPPppO..........", # 42
    "..........OpPPp..pPPpO..........", # 43 (Legs shifted left 1px for symmetric alignment)
    "..........OpPPp..pPPpO..........", # 44
    "..........OpPPp..pPPpO..........", # 45
    "..........OpPPp..pPPpO..........", # 46
    "..........OpPPp..pPPpO..........", # 47
    "..........OpPPp..pPPpO..........", # 48
    "..........OpPPp..pPPpO..........", # 49
    "..........OpPPp..pPPpO..........", # 50
    "..........OpPPp..pPPpO..........", # 51
    "..........OpPPp..pPPpO..........", # 52
    "..........OpPPp..pPPpO..........", # 53
    "..........OpPPp..pPPpO..........", # 54
    "..........OpPPp..pPPpO..........", # 55
    "..........OpPPp..pPPpO..........", # 56
    "..........OpPPp..pPPpO..........", # 57
    "..........OpKKp..pKKpO..........", # 58 (Boots start)
    "..........OKKKp..pKKKO..........", # 59
    "..........OKKKp..pKKKO..........", # 60
    "..........Onnnp..pnnnO..........", # 61 (Soles)
    "................................", # 62
    "................................"  # 63
]

LEGS_FRONT_WALK1 = [
    "........OSSSdOOOOOOdSSSO........", # 37
    "........OSSSdOOOOOOdSSSO........", # 38
    "........OSSSdOOOOOOdSSSO........", # 39
    "...........OOpPPPPppO...........", # 40
    "..........OOpPPPPPPppO..........", # 41
    "..........OOpPPPPPPppO..........", # 42
    "..........OpPPp..pPPpO..........", # 43
    "..........OpPPp..pPPpO..........", # 44
    "..........OpPPp..pPPpO..........", # 45
    "..........OpPPp..pPPpO..........", # 46
    "..........OpPPp..pPPpO..........", # 47
    "..........OpPPp..pPPpO..........", # 48
    "..........OpPPp...pPPpO.........", # 49 (Right knee bends outward)
    "..........OpPPp...pPPpO.........", # 50
    "..........OpPPp...pPPpO.........", # 51
    "..........OpPPp...pPPpO.........", # 52
    "..........OpPPp...pPPpO.........", # 53
    "..........OpPPp...pPPpO.........", # 54
    "..........OpPPp..pPPpO..........", # 55
    "..........OpPPp..pPPpO..........", # 56
    "..........OpPPp..pPPpO..........", # 57
    "..........OpKKp...pKKpO.........", # 58 (Right boot shifted outward)
    "..........OKKKp...pnnnO.........", # 59 (Right sole lifted/shifted)
    "..........OKKKp.................", # 60 (Left foot down, right empty)
    "..........Onnnp.................", # 61 (Left sole down, right empty)
    "................................", # 62
    "................................"  # 63
]

LEGS_FRONT_WALK2 = [
    "........OSSSdOOOOOOdSSSO........", # 37
    "........OSSSdOOOOOOdSSSO........", # 38
    "........OSSSdOOOOOOdSSSO........", # 39
    "...........OOpPPPPppO...........", # 40
    "..........OOpPPPPPPppO..........", # 41
    "..........OOpPPPPPPppO..........", # 42
    "..........OpPPp..pPPpO..........", # 43
    "..........OpPPp..pPPpO..........", # 44
    "..........OpPPp..pPPpO..........", # 45
    "..........OpPPp..pPPpO..........", # 46
    "..........OpPPp..pPPpO..........", # 47
    "..........OpPPp..pPPpO..........", # 48
    ".........OpPPp...pPPpO..........", # 49 (Left knee bends outward)
    ".........OpPPp...pPPpO..........", # 50
    ".........OpPPp...pPPpO..........", # 51
    ".........OpPPp...pPPpO..........", # 52
    ".........OpPPp...pPPpO..........", # 53
    ".........OpPPp...pPPpO..........", # 54
    "..........OpPPp..pPPpO..........", # 55
    "..........OpPPp..pPPpO..........", # 56
    "..........OpPPp..pPPpO..........", # 57
    ".........OpKKp...pKKpO..........", # 58 (Left boot shifted outward)
    ".........Onnnp...pKKKO..........", # 59 (Left sole lifted/shifted)
    ".................pKKKO..........", # 60 (Right boot down, left empty)
    "................pnnnO...........", # 61 (Right sole down, left empty)
    "................................", # 62
    "................................"  # 63
]

# --- BACK MODULAR PIECES ---
HEAD_BACK = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    "................................", # 4
    "................................", # 5
    "................................", # 6
    "............oo.oo.oo............", # 7
    "..........oohhHHHHhhoo..........", # 8
    ".........oohHHHHHHHHhoo.........", # 9
    "........oohHHHHHHHHHHHhoo.......", # 10
    "........oohHHHHHHHHHHHhoo.......", # 11
    "........ooHHHHHHHHHHHHoo........", # 12
    "........ooHHHHHHHHHHHHoo........", # 13
    "........ooHHHhHHhHHHhHoo........", # 14
    "........ooHHHHHHHHHHHHoo........", # 15
    "........ooHHHhHHhHHHhHoo........", # 16
    "........ooHHHHHHHHHHHHoo........", # 17
    "........ooHHHHHHHHHHHHoo........", # 18
    ".........ooHHHHHHHHHHoo.........", # 19
    "..........ooHHHHHHHHoo..........", # 20
    "...........ooHHHHHhoo...........", # 21
    "..............oHHo.............."  # 22 (Neck)
]

TORSO_BACK = [
    "..............OOOO..............", # 23 (Collar)
    "...........OOObbbbOOO...........", # 24 (Shoulders)
    "..........OObbbbbbbOO...........", # 25 (Fixed length: added a dot at the end)
    ".........OObbbbbbbbbOO..........", # 26
    ".........OObBBBBBBBBbOO.........", # 27
    "........OObbbBddddddBbbOO.......", # 28
    "........ObBBdBBBBBBdBBbO........", # 29
    "........ObBBdBBddBBdBBbO........", # 30
    "........ObBBdBBddBBdBBbO........", # 31
    "........ObBBdBBddBBdBBbO........", # 32
    "........ObBBdBBddBBdBBbO........", # 33
    "........ObBBdBBBBBBdBBbO........", # 34
    "........OSSSdBBBBBBdSSSO........", # 35 (Hands)
    "........OSSSdOOOOOOdSSSO........"  # 36
]

LEGS_BACK_IDLE = LEGS_FRONT_IDLE
LEGS_BACK_WALK1 = LEGS_FRONT_WALK1
LEGS_BACK_WALK2 = LEGS_FRONT_WALK2

# --- LEFT SIDE MODULAR PIECES ---
HEAD_LEFT = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "................................", # 3
    "................................", # 4
    "................................", # 5
    "................................", # 6
    "............oo.oo.oo............", # 7
    "..........oohhHHHHhhoo..........", # 8
    ".........oohHHHHHHHHhoo.........", # 9
    "........oohHHHHHHHHHHHhoo.......", # 10
    "........oohHHHHHHHHHHHhoo.......", # 11
    "........ooHHHHHHHHHHHHoo........", # 12
    "........ooSSSSSHhHhHHHoo........", # 13 (Face skin on left, textured hair on right)
    "........ooSSSSSSHHhHHHoo........", # 14
    "........ooSSoSSSHHhHHHoo........", # 15 (Eyebrow on left)
    "........ooSSeSSSHHhHHHoo........", # 16 (Eye white and pupil)
    "........ooSSESSSHHhHHHoo........", # 17
    ".........ooSSSSSHhHHoo..........", # 18
    "..........ooSSSSShHoo...........", # 19
    "...........ooSSShhoo............", # 20
    "............ooSSSoo.............", # 21
    "..............oSSo.............."  # 22 (Neck)
]

TORSO_LEFT = [
    "..............OOOO..............", # 23 (Collar)
    "............OOObbbOOO...........", # 24 (Shoulders side)
    "...........OObbbbbbbOO..........", # 25
    "..........OObbbbbbbbbOO.........", # 26
    "..........OObBBBBBBBBbOO........", # 27
    "..........OObBBddddBBbOO........", # 28 (Arm begins to form)
    ".........OBBdBBbOBBbO...........", # 29 (Arm sleeve in middle)
    ".........OBBdBBbOBBbO...........", # 30
    ".........OBBdBBbOBBbO...........", # 31
    ".........OBBdBBbOBBbO...........", # 32
    ".........OBBdBBbOBBbO...........", # 33
    ".........OBBdBBbOBBbO...........", # 34
    ".........OBBdSSoOBBbO...........", # 35 (Hand appears)
    ".........OOBdSSoOOBbO..........."  # 36 (Hoodie hem)
]

LEGS_LEFT_IDLE = [
    ".........OOBdSSoOOBbO...........", # 37 (Hand continues)
    ".........OOBdSSoOOBbO...........", # 38
    ".........OOBdSSoOOBbO...........", # 39
    "..........OOpPPPPppO............", # 40 (Hips side start - shifted 2px left)
    "..........OOpPPPPppO............", # 41
    "..........OOpPPPPppO............", # 42
    "..........OOpPPPPppO............", # 43 (Legs overlapping, no gap)
    "..........OOpPPPPppO............", # 44
    "..........OOpPPPPppO............", # 45
    "..........OOpPPPPppO............", # 46
    "..........OOpPPPPppO............", # 47
    "..........OOpPPPPppO............", # 48
    "..........OOpPPPPppO............", # 49
    "..........OOpPPPPppO............", # 50
    "..........OOpPPPPppO............", # 51
    "..........OOpPPPPppO............", # 52
    "..........OOpPPPPppO............", # 53
    "..........OOpPPPPppO............", # 54
    "..........OOpPPPPppO............", # 55
    "..........OOpPPPPppO............", # 56
    "..........OOpPPPPppO............", # 57
    "..........OOpKKKKppO............", # 58 (Boots side start)
    "..........OOKKKKKkOO............", # 59
    "..........OOKKKKKkOO............", # 60
    "..........OOnnnnnnkO............", # 61 (Soles side)
    "................................", # 62
    "................................"  # 63
]

LEGS_LEFT_WALK1 = [
    ".........OOBdSSoOOBbO...........", # 37
    ".........OOBdSSoOOBbO...........", # 38
    ".........OOBdSSoOOBbO...........", # 39
    "..........OOpPPPPppO............", # 40
    ".........OOpPPPPPPppO...........", # 41 (Subtle walking stance - narrow stride)
    ".........OpPPPPP.PPppO..........", # 42 (Compressed đùi width)
    ".........OpPPp.pPPpO............", # 43 (Symmetric narrow 1px stride gap!)
    ".........OpPPp.pPPpO............", # 44
    ".........OpPPp.pPPpO............", # 45
    ".........OpPPp.pPPpO............", # 46
    ".........OpPPp.pPPpO............", # 47
    ".........OpPPp.pPPpO............", # 48
    ".........OpPPp.pPPpO............", # 49
    ".........OpPPp.pPPpO............", # 50
    ".........OpPPp.pPPpO............", # 51
    ".........OpPPp.pPPpO............", # 52
    ".........OpPPp.pPPpO............", # 53
    ".........OpPPp.pPPpO............", # 54
    ".........OpPPp.pPPpO............", # 55
    ".........OpPPp.pPPpO............", # 56
    ".........OpPPp.pPPpO............", # 57
    ".........OpKKp.pKKpO............", # 58 (Boots separate)
    ".........OKKKp.pKKKO............", # 59
    ".........OKKKp.pnnnO............", # 60 (Right foot lifted, sole here)
    ".........Onnnp..................", # 61 (Left foot down, right empty)
    "................................", # 62
    "................................"  # 63
]

LEGS_LEFT_WALK2 = [
    ".........OOBdSSoOOBbO...........", # 37
    ".........OOBdSSoOOBbO...........", # 38
    ".........OOBdSSoOOBbO...........", # 39
    "..........OOpPPPPppO............", # 40
    ".........OOpPPPPPPppO...........", # 41 (Subtle walking stance - narrow stride)
    ".........OpPPPPP.PPppO..........", # 42 (Compressed đùi width)
    ".........OpPPp.pPPpO............", # 43 (Symmetric narrow 1px stride gap!)
    ".........OpPPp.pPPpO............", # 44
    ".........OpPPp.pPPpO............", # 45
    ".........OpPPp.pPPpO............", # 46
    ".........OpPPp.pPPpO............", # 47
    ".........OpPPp.pPPpO............", # 48
    ".........OpPPp.pPPpO............", # 49
    ".........OpPPp.pPPpO............", # 50
    ".........OpPPp.pPPpO............", # 51
    ".........OpPPp.pPPpO............", # 52
    ".........OpPPp.pPPpO............", # 53
    ".........OpPPp.pPPpO............", # 54
    ".........OpPPp.pPPpO............", # 55
    ".........OpPPp.pPPpO............", # 56
    ".........OpPPp.pPPpO............", # 57
    ".........OpKKp.pKKpO............", # 58
    ".........OKKKp.pKKKO............", # 59
    ".........Onnnp.pKKKO............", # 60 (Left foot lifted, sole here)
    "...............pnnnO............", # 61 (Right foot down, left empty)
    "................................", # 62
    "................................"  # 63
]

def assemble_sprite(head, torso, legs, shift_down=False):
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
            
        if shift_down and y < 37:
            if y == 0:
                row_str = "." * 32
            else:
                prev_y = y - 1
                if prev_y < 23:
                    row_str = head[prev_y]
                else:
                    row_str = torso[prev_y - 23]
        else:
            row_str = part[py]
            
        for x in range(32):
            char = row_str[x]
            color = palette.get(char, (0, 0, 0, 0))
            pixels[x, y] = color
            
    return img

def main():
    dest_dir = r'c:\code\thoat khoi dia nguc\Assets\Sprites\Characters\Minh'
    os.makedirs(dest_dir, exist_ok=True)
    
    # 1. Idle Sprites
    assemble_sprite(HEAD_FRONT, TORSO_FRONT, LEGS_FRONT_IDLE).save(os.path.join(dest_dir, "Minh_Idle_Down.png"))
    assemble_sprite(HEAD_BACK, TORSO_BACK, LEGS_BACK_IDLE).save(os.path.join(dest_dir, "Minh_Idle_Up.png"))
    
    left_idle = assemble_sprite(HEAD_LEFT, TORSO_LEFT, LEGS_LEFT_IDLE)
    left_idle.save(os.path.join(dest_dir, "Minh_Idle_Left.png"))
    left_idle.transpose(Image.FLIP_LEFT_RIGHT).save(os.path.join(dest_dir, "Minh_Idle_Right.png"))
    
    # 2. Walk Sprites (with 1px head bobbing shift_down=True)
    assemble_sprite(HEAD_FRONT, TORSO_FRONT, LEGS_FRONT_WALK1, shift_down=True).save(os.path.join(dest_dir, "Minh_Walk_Down_1.png"))
    assemble_sprite(HEAD_FRONT, TORSO_FRONT, LEGS_FRONT_WALK2, shift_down=True).save(os.path.join(dest_dir, "Minh_Walk_Down_2.png"))
    
    assemble_sprite(HEAD_BACK, TORSO_BACK, LEGS_BACK_WALK1, shift_down=True).save(os.path.join(dest_dir, "Minh_Walk_Up_1.png"))
    assemble_sprite(HEAD_BACK, TORSO_BACK, LEGS_BACK_WALK2, shift_down=True).save(os.path.join(dest_dir, "Minh_Walk_Up_2.png"))
    
    left_walk1 = assemble_sprite(HEAD_LEFT, TORSO_LEFT, LEGS_LEFT_WALK1, shift_down=True)
    left_walk1.save(os.path.join(dest_dir, "Minh_Walk_Left_1.png"))
    left_walk1.transpose(Image.FLIP_LEFT_RIGHT).save(os.path.join(dest_dir, "Minh_Walk_Right_1.png"))
    
    left_walk2 = assemble_sprite(HEAD_LEFT, TORSO_LEFT, LEGS_LEFT_WALK2, shift_down=True)
    left_walk2.save(os.path.join(dest_dir, "Minh_Walk_Left_2.png"))
    left_walk2.transpose(Image.FLIP_LEFT_RIGHT).save(os.path.join(dest_dir, "Minh_Walk_Right_2.png"))
    
    print("Symmetric redesigned 32x64 sprites successfully generated on disk!")

if __name__ == '__main__':
    main()
