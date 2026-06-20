import os
from PIL import Image

def main():
    railing_path = "scratch/stairs_railing_precise.png"
    if not os.path.exists(railing_path):
        print("Precise railing not found!")
        return
        
    img = Image.open(railing_path)
    width, height = img.size
    
    dest_dir = "Assets/Sprites/Backgrounds/StairsRailing"
    os.makedirs(dest_dir, exist_ok=True)
    
    # Slice into 24 columns of 32x1024 pixels
    for i in range(24):
        x_min = i * 32
        x_max = (i + 1) * 32
        
        # Crop the slice
        slice_img = img.crop((x_min, 0, x_max, height))
        
        # Save slice
        slice_path = os.path.join(dest_dir, f"Railing_{i}.png")
        slice_img.save(slice_path)
        print(f"Saved {slice_path}")

if __name__ == "__main__":
    main()
