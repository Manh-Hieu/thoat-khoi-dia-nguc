import os
from PIL import Image

def main():
    bg_path = "Assets/Sprites/Backgrounds/Chapter2_Casino_Bg.png"
    if not os.path.exists(bg_path):
        print("Background not found!")
        return
        
    img = Image.open(bg_path).convert("RGBA")
    width, height = img.size
    
    # Create output image
    out_img = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    
    # Let's inspect each pixel and copy it to out_img if it belongs to the railing system
    for x in range(width):
        for y in range(height):
            keep = False
            
            # 1. Left landing wall / left edge details
            if x >= 0 and x <= 12 and y >= 295 and y <= 390:
                keep = True
                
            # 2. Horizontal Top Railing (X from 0 to 240)
            elif x >= 0 and x <= 240:
                # Top horizontal handrail
                if y >= 302 and y <= 319:
                    keep = True
                # Bottom horizontal base rail
                elif y >= 376 and y <= 392:
                    keep = True
                # Horizontal balusters (spaced every 20px, starting at 18)
                elif y > 319 and y < 376:
                    for k in range(13):
                        bal_x = 18 + 20 * k
                        if abs(x - bal_x) <= 2:
                            keep = True
                            break
                            
            # 3. Top newel post (around x=235 to 248)
            if x >= 232 and x <= 248 and y >= 295 and y <= 398:
                keep = True
                
            # 4. Diagonal Railing (X from 240 to 750)
            if x >= 240 and x <= 750:
                # Top handrail: y_center = 305 + (x - 240) * 0.75
                y_center_top = 305 + (x - 240) * 0.75
                # Bottom base rail: y_center = 390 + (x - 240) * 0.75
                y_center_bot = 390 + (x - 240) * 0.75
                
                # Check top handrail thickness (8px half-width)
                if abs(y - y_center_top) <= 8:
                    keep = True
                # Check bottom base rail thickness (8px half-width)
                elif abs(y - y_center_bot) <= 8:
                    keep = True
                    
                # Check diagonal balusters (spaced every 20px, starting at 238)
                elif y > y_center_top + 8 and y < y_center_bot - 8:
                    for k in range(27):
                        bal_x = 238 + 20 * k
                        if abs(x - bal_x) <= 2:
                            keep = True
                            break
                            
            # 5. Bottom newel post (x=740 to 765, y=675 to 810)
            if x >= 740 and x <= 765 and y >= 675 and y <= 810:
                keep = True
                
            if keep:
                out_img.putpixel((x, y), img.getpixel((x, y)))
                
    os.makedirs("scratch", exist_ok=True)
    out_img.save("scratch/stairs_railing_precise.png")
    print("Saved scratch/stairs_railing_precise.png")

if __name__ == "__main__":
    main()
