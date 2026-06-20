import os
from PIL import Image, ImageDraw

def main():
    bg_path = "Assets/Sprites/Backgrounds/Chapter2_Casino_Bg.png"
    if not os.path.exists(bg_path):
        print("Background not found!")
        return
        
    img = Image.open(bg_path).convert("RGBA")
    width, height = img.size
    
    # Create mask image
    mask = Image.new("L", (width, height), 0)
    draw = ImageDraw.Draw(mask)
    
    # 1. Top horizontal railing: x from 0 to 240, y from 300 to 395
    draw.rectangle([0, 300, 240, 395], fill=255)
    
    # 2. Diagonal railing: polygon with corners
    # Corner 1: (240, 300) - top-left top
    # Corner 2: (745, 680) - bottom-right top
    # Corner 3: (770, 700) - bottom-right right
    # Corner 4: (770, 810) - bottom-right bottom
    # Corner 5: (730, 810) - bottom-right left
    # Corner 6: (240, 400) - top-left bottom
    poly = [
        (240, 300),
        (745, 680),
        (770, 680),
        (770, 810),
        (725, 810),
        (240, 400)
    ]
    draw.polygon(poly, fill=255)
    
    # Apply mask
    out_img = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    out_img.paste(img, mask=mask)
    
    os.makedirs("scratch", exist_ok=True)
    out_img.save("scratch/stairs_railing.png")
    print("Saved scratch/stairs_railing.png")

if __name__ == "__main__":
    main()
