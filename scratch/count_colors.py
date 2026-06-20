import os
from PIL import Image
from collections import Counter

def main():
    bg_path = "Assets/Sprites/Backgrounds/Chapter2_Casino_Bg.png"
    if not os.path.exists(bg_path):
        print("Background not found!")
        return
        
    img = Image.open(bg_path).convert("RGBA")
    
    # Crop a small region that contains the handrail, balusters, steps, and background:
    # Let's crop X=[390, 410], Y=[420, 510]
    crop = img.crop((390, 420, 410, 510))
    pixels = list(crop.getdata())
    
    # Count unique colors
    counter = Counter(pixels)
    print("Dominant colors in the stairs area:")
    for color, count in counter.most_common(30):
        r, g, b, a = color
        hex_color = f"#{r:02X}{g:02X}{b:02X}"
        print(f"Color: {hex_color} ({r:3d}, {g:3d}, {b:3d}) - Count: {count:4d}")

if __name__ == "__main__":
    main()
