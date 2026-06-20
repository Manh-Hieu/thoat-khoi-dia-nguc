from PIL import Image

try:
    img = Image.open("Assets/Sprites/Props/Newspaper.png").convert("RGBA")
    width, height = img.size
    
    # Create a new image
    new_img = Image.new("RGBA", (width, height))
    
    transparent_count = 0
    total_count = width * height
    
    for y in range(height):
        for x in range(width):
            r, g, b, a = img.getpixel((x, y))
            
            # Calculate difference between color channels (neutrality/saturation)
            diff = max(r, g, b) - min(r, g, b)
            
            # If the color is very neutral (gray or white checkerboard) and not very dark (not ink outline)
            # Or if it is extremely bright white (close to 255)
            is_background = False
            
            if diff < 20 and r > 70:
                is_background = True
            elif r > 240 and g > 240 and b > 240:
                is_background = True
                
            if is_background:
                new_img.putpixel((x, y), (0, 0, 0, 0))
                transparent_count += 1
            else:
                new_img.putpixel((x, y), (r, g, b, 255))
                
    # Save the result as a PNG
    new_img.save("Assets/Sprites/Props/Newspaper_Clean.png", "PNG")
    print(f"Finished processing! Made {transparent_count} out of {total_count} pixels transparent ({transparent_count/total_count*100:.2f}%)")
    
except Exception as e:
    print(f"Error: {e}")
