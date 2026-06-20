import os
from PIL import Image

def main():
    bg_path = "Assets/Sprites/Backgrounds/Chapter2_Casino_Bg.png"
    if not os.path.exists(bg_path):
        print("Background not found!")
        return
        
    img = Image.open(bg_path).convert("RGBA")
    
    # We sample a few points:
    # 1. Handrail (thick brown bar) at x=400, y=425
    # 2. Base rail (thick brown bar) at x=400, y=505
    # 3. Baluster (vertical post) at x=400, y=460
    # 4. Step (light tan) at x=400, y=480
    # 5. Wall (dark purple) at x=400, y=350
    points = {
        "handrail": (400, 425),
        "base_rail": (400, 505),
        "baluster": (400, 460),
        "step": (400, 480),
        "wall": (400, 350)
    }
    
    for name, (x, y) in points.items():
        r, g, b, a = img.getpixel((x, y))
        # Print color details and hex
        hex_color = f"#{r:02X}{g:02X}{b:02X}"
        print(f"{name} at ({x}, {y}): RGBA=({r}, {g}, {b}, {a}) Hex={hex_color}")

if __name__ == "__main__":
    main()
