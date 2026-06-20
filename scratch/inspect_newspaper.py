from PIL import Image

try:
    img = Image.open("Assets/Sprites/Props/Newspaper.png")
    width, height = img.size
    print(f"Format: {img.format}, Size: {width}x{height}")
    
    # Inspect top-left corner 20x20 pixels
    colors = set()
    for y in range(20):
        for x in range(20):
            colors.add(img.getpixel((x, y)))
            
    print("Distinct colors in top-left 20x20 corner:")
    for c in list(colors)[:10]:
        print(c)
        
except Exception as e:
    print(f"Error: {e}")
