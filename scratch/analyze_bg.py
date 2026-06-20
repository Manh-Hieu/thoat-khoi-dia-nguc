from PIL import Image

try:
    img = Image.open("Assets/Sprites/Backgrounds/Chapter1_Room_Bg.png").convert("L") # convert to grayscale
    width, height = img.size
    print(f"Image dimensions: {width}x{height}")
    
    # Calculate average pixel value for left and right half manually using getpixel
    left_sum = 0
    right_sum = 0
    left_count = 0
    right_count = 0
    
    for y in range(height):
        for x in range(width):
            val = img.getpixel((x, y))
            if x < width // 2:
                left_sum += val
                left_count += 1
            else:
                right_sum += val
                right_count += 1
                
    print(f"Left half average pixel value: {left_sum / left_count:.2f}")
    print(f"Right half average pixel value: {right_sum / right_count:.2f}")
    
except Exception as e:
    print(f"Error: {e}")
