import os
import re
from PIL import Image

def hex_to_rgba(hex_str):
    hex_str = hex_str.lstrip('#')
    if len(hex_str) == 6:
        r = int(hex_str[0:2], 16)
        g = int(hex_str[2:4], 16)
        b = int(hex_str[4:6], 16)
        return (r, g, b, 255)
    elif len(hex_str) == 8:
        r = int(hex_str[0:2], 16)
        g = int(hex_str[2:4], 16)
        b = int(hex_str[4:6], 16)
        a = int(hex_str[6:8], 16)
        return (r, g, b, a)
    return (0, 0, 0, 0)

def parse_and_generate():
    cs_path = r'c:\code\thoat khoi dia nguc\Assets\Editor\SpriteGenerator.cs'
    with open(cs_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find colorMap section in GenerateMinhSprites
    # Match the block containing colorMap dictionary definition
    color_map = {'.': (0, 0, 0, 0)}
    # Find colorMap block specifically inside GenerateMinhSprites
    minh_start = content.find("void GenerateMinhSprites()")
    if minh_start == -1:
        print("Could not find GenerateMinhSprites")
        return
    
    # Extract colorMap entries
    colormap_entries = re.findall(r"\{\s*'([^'])'\s*,\s*ColorFromHex\(\"([^\"]+)\"\)\s*\}", content[minh_start:minh_start+5000])
    for char, hex_val in colormap_entries:
        color_map[char] = hex_to_rgba(hex_val)

    print("Color map parsed:", color_map)

    # Find all grid definitions in the GenerateMinhSprites section
    # Let's extract grid definitions like: string[] name = { ... };
    grids = {}
    grid_matches = re.finditer(r'string\[\]\s+(\w+)\s*=\s*\{(.*?)\};', content[minh_start:minh_start+20000], re.DOTALL)
    for gm in grid_matches:
        grid_name = gm.group(1)
        grid_data = gm.group(2)
        rows = []
        for line in grid_data.strip().split('\n'):
            line_m = re.search(r'"([^"]+)"', line)
            if line_m:
                rows.append(line_m.group(1))
        grids[grid_name] = rows
        print(f"Parsed grid {grid_name} with {len(rows)} rows, width {len(rows[0]) if rows else 0}")

    # Find GenerateSpriteFromGrid calls for Minh
    calls = re.finditer(r'GenerateSpriteFromGrid\(\"([^\"]+)\"\s*,\s*(\w+)\s*,\s*colorMap\s*,\s*(\d+)\s*,\s*(\d+)\)', content[minh_start:minh_start+25000])
    for call in calls:
        dest_path = call.group(1)
        grid_var = call.group(2)
        width = int(call.group(3))
        height = int(call.group(4))

        full_dest = os.path.join(r'c:\code\thoat khoi dia nguc', dest_path.replace('/', '\\'))
        os.makedirs(os.path.dirname(full_dest), exist_ok=True)

        if grid_var not in grids:
            print(f"Grid variable {grid_var} not found in parsed grids")
            continue

        grid = grids[grid_var]
        # Create image
        img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
        for y in range(height):
            # C# row 0 matches python row 0, because row 0 in C# grid array is top of sprite
            row = grid[y]
            for x in range(width):
                char = row[x]
                color = color_map.get(char, (0, 0, 0, 0))
                img.putpixel((x, y), color)

        img.save(full_dest)
        print(f"Generated sprite: {dest_path} -> {img.size}")

if __name__ == '__main__':
    parse_and_generate()
