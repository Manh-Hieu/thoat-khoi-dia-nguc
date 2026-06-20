from PIL import Image

try:
    img = Image.open("Assets/Sprites/Characters/Minh/Minh_Idle_Down.png").convert("LA") # convert to grayscale with alpha
    width, height = img.size
    print(f"Player sprite dimensions: {width}x{height}")
    
    total_val = 0
    count = 0
    for y in range(height):
        for x in range(width):
            val, alpha = img.getpixel((x, y))
            if alpha > 10: # only count non-transparent pixels
                total_val += val
                count += 1
                
    if count > 0:
        print(f"Player average pixel value (opaque): {total_val / count:.2f}")
    else:
        print("No opaque pixels found!")
        
except Exception as e:
    print(f"Error: {e}")
