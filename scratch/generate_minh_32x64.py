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
    'H': hex_to_rgba('#1c1c24'), # Base hair
    'h': hex_to_rgba('#3a3a4a'), # Hair highlight
    'o': hex_to_rgba('#0c0c10'), # Hair outline / dark shadow
    # Skin (Peach, shaded)
    'S': hex_to_rgba('#fcd4b4'), # Main skin
    's': hex_to_rgba('#e2a884'), # Skin shadow
    'c': hex_to_rgba('#ffabb2'), # Cheek blush
    'E': hex_to_rgba('#0d0d12'), # Eyes / Pupils
    'e': hex_to_rgba('#ffffff'), # Eye whites
    # Hoodie (Slate Blue)
    'B': hex_to_rgba('#2b5d92'), # Main blue hoodie
    'b': hex_to_rgba('#487bb0'), # Hoodie highlight
    'd': hex_to_rgba('#1c3b5d'), # Hoodie shadow
    'O': hex_to_rgba('#0b1724'), # Hoodie outline
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
    "............oooooooo............", # 3
    "..........oohhhhhhhhoo..........", # 4
    "........oohhHHHHHHHHhhoo........", # 5
    ".......ooHHHHHHHHHHHHHHoo.......", # 6
    "......ooHHHHHHHHHHHHHHHHoo......", # 7
    "......ooHHHHHHHHHHHHHHHHoo......", # 8
    "......ooHHHSSSSSSSSSSHHHoo......", # 9
    "......ooHSSSSeSSSeSSSSHHoo......", # 10 (Eyes at cols 13, 17)
    "......ooHSSSSEsSSEsSSSHHoo......", # 11 (Eyes pupil/shadow)
    "......ooHSSSSSsSSsSSSSHHoo......", # 12
    ".......ooHSSssSSssSSsHHoo.......", # 13
    "........ooHSSSSSSSSSHHoo........", # 14
    ".........oohHSSSSSHhoo..........", # 15
    "...........ooSSSSSoo............", # 16 (Neck)
    "............oSSSSo..............", # 17
    "................................", # 18
    "................................"  # 19
]

TORSO_FRONT = [
    "............OOOO................", # 20
    "..........OOObbbOOO.............", # 21
    "........OOObbbbbbbOOO...........", # 22
    ".......OObbbbbbbbbbbOO..........", # 23
    "......OObbBBBBBBBBbbbbOO........", # 24
    ".....OObBBBBBBBBBBBBbbOO........", # 25
    "....OObBBBDDDDDDDDBBBbbOO.......", # 26
    "....OObBBDDDDDDDDDDBBBbOO.......", # 27
    "....OObBBDDDwDDwDDDBBBbOO.......", # 28
    "....OObBBDDDwDDwDDDBBBbOO.......", # 29
    "....OObBBDDDDDDDDDDBBBbOO.......", # 30
    "....OObBBDDDDDDDDDDBBBbOO.......", # 31
    ".....OOBBDDDDDDDDDDBBBOO........", # 32
    "......OOBBDDDDDDDDBBOO..........", # 33
    ".......OOBBBBBBBBBBOO...........", # 34
    "........OOOOOOOOOOOO............"  # 35
]

LEGS_FRONT_IDLE = [
    ".........OOOOOOOOOO.............", # 36
    "........OOpPPPPPPppO............", # 37
    ".......OOpPPPPPPPPppO...........", # 38
    ".......OOpPPPPPPPPppO...........", # 39
    "......OOpPPPPPPPPPPppO..........", # 40
    "......OOpPPPP..PPPPppO..........", # 41
    "......OOpPPP....PPPppO..........", # 42
    "......OOpPPP....PPPppO..........", # 43
    "......OOpPPP....PPPppO..........", # 44
    "......OOpPPP....PPPppO..........", # 45
    "......OOpPPP....PPPppO..........", # 46
    "......OOpPPP....PPPppO..........", # 47
    "......OOpPPP....PPPppO..........", # 48
    "......OOpPPP....PPPppO..........", # 49
    "......OOpPPP....PPPppO..........", # 50
    "......OOpPPP....PPPppO..........", # 51
    "......OOpPPP....PPPppO..........", # 52
    "......OOpPPP....PPPppO..........", # 53
    "......OOpPPP....PPPppO..........", # 54
    "......OOpPPP....PPPppO..........", # 55
    "......OOpPPP....PPPppO..........", # 56
    "......OOpPPP....PPPppO..........", # 57
    "......OOpPPP....PPPppO..........", # 58
    "......OOpKKK....KKKppO..........", # 59
    ".....OOKKKKK...KKKKKppO.........", # 60
    "....OOKKKKKK..OOKKKKKKpO........", # 61
    "....OOnnnnnn...nnnnnnnO.........", # 62
    "................................"  # 63
]

LEGS_FRONT_WALK1 = [
    ".........OOOOOOOOOO.............",
    "........OOpPPPPPPppO............",
    ".......OOpPPPPPPPPppO...........",
    ".......OOpPPPPPPPPppO...........",
    "......OOpPPPPPPPPPPppO..........",
    "......OOpPPPP..PPPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpKKK....KKKppO..........",
    ".....OOKKKKK...KKKKKppO.........", # Left leg down, right leg starts lifting
    "....OOKKKKKK....KKKKKpO.........", 
    "....OOnnnnnn....nnnnnO..........", # Left foot touches bottom
    "................................"
]

LEGS_FRONT_WALK2 = [
    ".........OOOOOOOOOO.............",
    "........OOpPPPPPPppO............",
    ".......OOpPPPPPPPPppO...........",
    ".......OOpPPPPPPPPppO...........",
    "......OOpPPPPPPPPPPppO..........",
    "......OOpPPPP..PPPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpPPP....PPPppO..........",
    "......OOpKKK....KKKppO..........",
    ".....OOKKKKK...KKKKKppO.........", # Right leg down, left leg starts lifting
    ".....OOKKKKK....KKKKKpO.........", 
    "......nnnnn....OOnnnnnnO........", # Right foot touches bottom
    "................................"
]


# --- BACK MODULAR PIECES ---
HEAD_BACK = [
    "................................", # 0
    "................................", # 1
    "................................", # 2
    "............oooooooo............", # 3
    "..........oohhhhhhhhoo..........", # 4
    "........oohhHHHHHHHHhhoo........", # 5
    ".......ooHHHHHHHHHHHHHHoo.......", # 6
    "......ooHHHHHHHHHHHHHHHHoo......", # 7
    "......ooHHHHHHHHHHHHHHHHoo......", # 8
    "......ooHHHHHHHHHHHHHHHHoo......", # 9 (Full hair back)
    "......ooHHHHHHHHHHHHHHHHoo......", # 10
    "......ooHHHHHHHHHHHHHHHHoo......", # 11
    "......ooHHHHHHHHHHHHHHHHoo......", # 12
    ".......ooHHHHHHHHHHHHHHoo.......", # 13
    "........ooHHHHHHHHHHHHoo........", # 14
    ".........oohHHHHHHHhoo..........", # 15
    "...........ooHHHHHoo............", # 16 (Neck is covered by hair)
    "............oHHHHo..............", # 17
    "................................", # 18
    "................................"  # 19
]

TORSO_BACK = [
    "............OOOO................", # 20
    "..........OOObbbOOO.............", # 21
    "........OOObbbbbbbOOO...........", # 22
    ".......OObbbbbbbbbbbOO..........", # 23
    "......OObbBBBBBBBBbbbbOO........", # 24
    ".....OObBBBBBBBBBBBBbbOO........", # 25
    "....OObBBBBBBBBBBBBBBbbOO.......", # 26 (Back is full blue)
    "....OObBBBBBBBBBBBBBBbOO........", # 27
    "....OObBBDDDDDDDDDDBBBbOO.......", # 28 (Shadow near bottom)
    "....OObBBDDDDDDDDDDBBBbOO.......", # 29
    "....OObBBDDDDDDDDDDBBBbOO.......", # 30
    "....OObBBDDDDDDDDDDBBBbOO.......", # 31
    ".....OOBBDDDDDDDDDDBBBOO........", # 32
    "......OOBBDDDDDDDDBBOO..........", # 33
    ".......OOBBBBBBBBBBOO...........", # 34
    "........OOOOOOOOOOOO............"  # 35
]

# Back legs are identical to front legs layout but with back boots and back seams
LEGS_BACK_IDLE = LEGS_FRONT_IDLE
LEGS_BACK_WALK1 = LEGS_FRONT_WALK1
LEGS_BACK_WALK2 = LEGS_FRONT_WALK2


# --- LEFT SIDE MODULAR PIECES ---
HEAD_LEFT = [
    "................................",
    "................................",
    "................................",
    "............oooooo..............",
    "..........oohhhhhhho............",
    "........oohhHHHHHHHHo...........",
    ".......ooHHHHHHHHHHHHo..........",
    "......ooHHHHHHHHHHHHHHo.........",
    "......oHHHHHHHHHHHHHHHo.........",
    "......oHHHHSSSSSSSHHHHo.........", # Face on the left
    "......oHHSSeESSSSSSHHHo.........", # Eye on left (col 12)
    "......oHHSSSEsSSSHHHHHo.........",
    "......oHHSSSSSSSSHHHHHo.........",
    ".......oHSSSssSSHHHHoo..........",
    "........oHSSSHHHHHHoo...........",
    ".........oohHHHHHhoo............",
    "...........ooSSSoo..............", # Neck
    "............oSSo................",
    "................................",
    "................................"
]

TORSO_LEFT = [
    "............OOO.................",
    "..........OOObbOO...............",
    "........OOObbbbbOO..............",
    ".......OObbbbbbbbOO.............",
    "......OObbBBBBBBbbOO............",
    ".....OObBBBBBBBBBbbOO...........",
    "....OObBBBBBDDDBBBbbOO..........", # Arm sleeve in middle
    "....OObBBBBDDDDBBBbbOO..........",
    "....OObBBBBDDDDBBBbOO...........",
    "....OObBBBBDDDDBBBbOO...........",
    "....OObBBBBDDDDBBBbOO...........",
    "....OObBBBBDDDDBBBbOO...........",
    ".....OOBBBDDDBBBBBOO............",
    "......OOBBDDDBBBOO..............",
    ".......OOBBBBBBOO...............",
    "........OOOOOOOO................"
]

LEGS_LEFT_IDLE = [
    ".........OOOOOOOO...............",
    "........OOpPPPPppO..............",
    ".......OOpPPPPPPPppO............",
    ".......OOpPPPPPPPppO............",
    "......OOpPPPPPPPPPppO...........",
    "......OOpPPPPPPPPPppO...........",
    "......OOpPPPPpPpppppO...........", # Foreground and background overlap
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpPPPPpPpppppO...........",
    "......OOpKKKpKKKppppO...........", # Boots
    ".....OOKKKKKkKKKKppO............",
    "....OOKKKKKKkKKKKKpO............",
    "....OOnnnnnnknnnnnO.............", # Soles
    "................................"
]

LEGS_LEFT_WALK1 = [
    ".........OOOOOOOO...............",
    "........OOpPPPPppO..............",
    ".......OOpPPPPPPPppO............",
    ".......OOpPPPPPPPppO............",
    "......OOpPPPPPPPPPppO...........",
    "......OOpPPPPPPPPPppO...........",
    "......OOpPPPPPppppppO...........",
    ".....OOpPPPP...ppppppO..........", # Separation starts
    ".....OOpPPP.....pppppO..........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpPPP.......pppppO.........",
    "....OOpKKK.......KKKppO.........",
    "....OOKKKK.......KKKKpO.........", # Left forward, right back
    "....OOnnnn.......nnnnO..........", 
    "...OOnnnn.........nnO...........", # Left sole touches bottom, right lifted
    "................................"
]

LEGS_LEFT_WALK2 = [
    ".........OOOOOOOO...............",
    "........OOpPPPPppO..............",
    ".......OOpPPPPPPPppO............",
    ".......OOpPPPPPPPppO............",
    "......OOpPPPPPPPPPppO...........",
    "......OOpPPPPPPPPPppO...........",
    "......OOpPPPPPppppppO...........",
    ".....OOppppp...PPPPppO..........", # Opposite separation
    ".....OOppppp.....PPPppO.........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOppppp.......PPPppO........",
    "....OOpKKK.......KKKppO.........",
    "....OOKKKK.......KKKKpO.........", # Right forward, left back
    "....OOnnnn.......nnnnO..........", 
    ".....nn.........nnnnnO..........", # Right sole touches bottom, left lifted
    "................................"
]


def assemble_sprite(head, torso, legs, shift_down=False):
    # Assemble 32x64 image
    img = Image.new("RGBA", (32, 64), (0, 0, 0, 0))
    pixels = img.load()
    
    # Grid lines to copy
    for y in range(64):
        # Determine source part and line
        if y < 20:
            part = head
            py = y
        elif y < 36:
            part = torso
            py = y - 20
        else:
            part = legs
            py = y - 36
            
        # Shift body down by 1px if head bobbing is active
        if shift_down and y < 36:
            # Shift head and torso down by 1 pixel (leaving row 0 empty)
            if y == 0:
                row_str = "." * 32
            else:
                # Get the previous row
                prev_y = y - 1
                if prev_y < 20:
                    row_str = head[prev_y]
                else:
                    row_str = torso[prev_y - 20]
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
    # Mirror left to get right
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
    
    print("Redesigned 32x64 sprites successfully generated on disk!")

if __name__ == '__main__':
    main()
