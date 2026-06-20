import os
from PIL import Image, ImageDraw

def main():
    bg_path = "Assets/Sprites/Backgrounds/Chapter2_Casino_Bg.png"
    if not os.path.exists(bg_path):
        print("Background not found!")
        return
        
    img = Image.open(bg_path)
    # We want to crop the stairs area: X from 0 to 750, Y from 300 to 850
    stairs_area = img.crop((0, 300, 750, 850))
    
    # Let's draw a pixel grid of 50px interval on it for reference
    draw = ImageDraw.Draw(stairs_area)
    for x in range(0, 750, 50):
        draw.line([(x, 0), (x, 550)], fill=(255, 0, 0, 128), width=1)
        draw.text((x + 2, 2), f"x={x}", fill=(255, 0, 0))
        
    for y in range(0, 550, 50):
        draw.line([(0, y), (750, y)], fill=(255, 0, 0, 128), width=1)
        draw.text((2, y + 2), f"y={y+300}", fill=(255, 0, 0))
        
    os.makedirs("scratch", exist_ok=True)
    stairs_area.save("scratch/stairs_grid.png")
    print("Saved scratch/stairs_grid.png")

if __name__ == "__main__":
    main()
