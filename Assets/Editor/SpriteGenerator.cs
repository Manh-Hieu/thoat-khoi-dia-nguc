#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace EscapeFromHell.Editor
{
    public class SpriteGenerator : EditorWindow
    {
        [MenuItem("Escape From Hell/Generate All Sprites")]
        public static void GenerateAll()
        {
            Debug.Log("Starting Sprite Generation...");

            // Make sure directories exist
            EnsureDirectories();

            // Generate characters
            GenerateMinhSprites();
            GenerateGuardSprites();

            // Generate tiles
            GenerateTileSprites();

            // Generate items
            GenerateItemSprites();

            // Generate UI
            GenerateUISprites();

            AssetDatabase.Refresh();
            Debug.Log("Sprite Generation Completed Successfully!");
        }

        private static void EnsureDirectories()
        {
            string[] paths = {
                "Assets/Sprites/Characters/Minh",
                "Assets/Sprites/Characters/Guards",
                "Assets/Sprites/Tiles/Room",
                "Assets/Sprites/Tiles/Compound",
                "Assets/Sprites/Tiles/Forest",
                "Assets/Sprites/Items",
                "Assets/Sprites/UI"
            };

            foreach (string p in paths)
            {
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }
        }

        private static void GenerateSpriteFromGrid(string path, string[] grid, Dictionary<char, Color> colorMap, int width = 16, int height = 16)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            {
                // Invert y because texture coordinates start at bottom left (y=0 is bottom)
                // but our grid array row 0 represents the top of the sprite (y=height-1)
                string row = grid[height - 1 - y];
                for (int x = 0; x < width; x++)
                {
                    char c = row[x];
                    Color col = colorMap.ContainsKey(c) ? colorMap[c] : Color.clear;
                    tex.SetPixel(x, y, col);
                }
            }
            tex.Apply();
            SaveTexture(tex, path);
        }

        private static void GenerateMinhSprites()
        {
            var colorMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'H', ColorFromHex("#1F1610") }, // Hair (Dark Brown)
                { 'h', ColorFromHex("#3D2E25") }, // Hair Highlight
                { 'S', ColorFromHex("#FAD2B0") }, // Skin (Peach)
                { 's', ColorFromHex("#DCA382") }, // Skin Shadow
                { 'E', ColorFromHex("#1A1A1A") }, // Eyes (Dark Gray)
                { 'B', ColorFromHex("#3B5998") }, // Shirt (Blue)
                { 'b', ColorFromHex("#5A7EC7") }, // Shirt Highlight
                { 'D', ColorFromHex("#1D2E54") }, // Dark Blue shadow
                { 'P', ColorFromHex("#34495E") }, // Jeans (Blue-Gray)
                { 'p', ColorFromHex("#1F2E3E") }, // Jeans Shadow
                { 'K', ColorFromHex("#5D4037") }, // Shoes (Brown)
                { 'k', ColorFromHex("#3E2723") }  // Background Shoes (Dark Brown shadow)
            };

            // IDLE DOWN - facing camera
            string[] down = {
                "................",
                "................",
                "......HHHH......",
                "....HHHHHHHH....",
                "....HHhHHhHH....",
                "....HSSSSSSH....",
                "....HSESSSESH...",
                "....HSSSSssH....",
                ".....sSSss......",
                "......SSSS......", // neck
                "....BBBBBBBB....", // shoulders
                "...BBbBBBBbBB...", // torso
                "...BBBBBBBBBB...",
                "...BBDDDDDDBB...",
                "...BBDDDDDDBB...",
                "....SDDDDDDS....", // hands
                "....sDDDDDDs....",
                ".....PPPPPP.....", // waist
                ".....PPPPPP.....",
                ".....PPppPP.....", // thighs
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....KK..KK.....",
                "....KKK..KKK....",
                "................",
                "................"
            };

            // WALK DOWN 1
            string[] down_walk1 = {
                "................",
                "................",
                "......HHHH......",
                "....HHHHHHHH....",
                "....HHhHHhHH....",
                "....HSSSSSSH....",
                "....HSESSSESH...",
                "....HSSSSssH....",
                ".....sSSss......",
                "......SSSS......",
                "....BBBBBBBB....",
                "...BBbBBBBbBB...",
                "...BBBBBBBBBB...",
                "...BBDDDDDDBB...",
                "...SSDDDDDDSS...", // hands swinging
                "....sDDDDDDs....",
                ".....PPPPPP.....",
                ".....PPPPPP.....",
                ".....PPppPP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP..........",
                "....KK....KK....",
                "...KKK...KKK....",
                "...KKK..........",
                "................",
                "................"
            };

            // WALK DOWN 2
            string[] down_walk2 = {
                "................",
                "................",
                "......HHHH......",
                "....HHHHHHHH....",
                "....HHhHHhHH....",
                "....HSSSSSSH....",
                "....HSESSSESH...",
                "....HSSSSssH....",
                ".....sSSss......",
                "......SSSS......",
                "....BBBBBBBB....",
                "...BBbBBBBbBB...",
                "...BBBBBBBBBB...",
                "...BBDDDDDDBB...",
                "...SSDDDDDDSS...",
                "....sDDDDDDs....",
                ".....PPPPPP.....",
                ".....PPPPPP.....",
                ".....PPppPP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "..........PP....",
                "....KK....KK....",
                "...KKK...KKK....",
                ".........KKK....",
                "................",
                "................"
            };

            // IDLE UP - facing away
            string[] up = {
                "................",
                "................",
                "......HHHH......",
                "....HHHHHHHH....",
                "....HHhHHhHH....",
                "....HHHHHHHH....",
                "....HHHHHHHH....",
                "....HHHHHHHH....",
                ".....HHHHHH.....",
                "......HHHH......",
                "....BBBBBBBB....",
                "...BBbBBBBbBB...",
                "...BBBBBBBBBB...",
                "...BBDDDDDDBB...",
                "...BBDDDDDDBB...",
                "....SDDDDDDS....",
                "....sDDDDDDs....",
                ".....PPPPPP.....",
                ".....PPPPPP.....",
                ".....PPppPP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                ".....KK..KK.....",
                "....KKK..KKK....",
                "................",
                "................"
            };

            // WALK UP 1
            string[] up_walk1 = {
                "................",
                "................",
                "......HHHH......",
                "....HHHHHHHH....",
                "....HHhHHhHH....",
                "....HHHHHHHH....",
                "....HHHHHHHH....",
                "....HHHHHHHH....",
                ".....HHHHHH.....",
                "......HHHH......",
                "....BBBBBBBB....",
                "...BBbBBBBbBB...",
                "...BBBBBBBBBB...",
                "...BBDDDDDDBB...",
                "...SSDDDDDDSS...",
                "....sDDDDDDs....",
                ".....PPPPPP.....",
                ".....PPPPPP.....",
                ".....PPppPP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP..........",
                "....KK....KK....",
                "...KKK...KKK....",
                "...KKK..........",
                "................",
                "................"
            };

            // WALK UP 2
            string[] up_walk2 = {
                "................",
                "................",
                "......HHHH......",
                "....HHHHHHHH....",
                "....HHhHHhHH....",
                "....HHHHHHHH....",
                "....HHHHHHHH....",
                "....HHHHHHHH....",
                ".....HHHHHH.....",
                "......HHHH......",
                "....BBBBBBBB....",
                "...BBbBBBBbBB...",
                "...BBBBBBBBBB...",
                "...BBDDDDDDBB...",
                "...SSDDDDDDSS...",
                "....sDDDDDDs....",
                ".....PPPPPP.....",
                ".....PPPPPP.....",
                ".....PPppPP.....",
                ".....PP..PP.....",
                ".....PP..PP.....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "..........PP....",
                "....KK....KK....",
                "...KKK...KKK....",
                ".........KKK....",
                "................",
                "................"
            };

            // IDLE RIGHT
            string[] right = {
                "................",
                "................",
                "....HHHHHH......",
                "...HHHHHHHH.....",
                "...HHHhHHhHH....",
                "..HHHHHHSSHH....",
                "..HHHHSSSESs....",
                "...HHSSSSSSH....",
                "...HsSSSSSs.....",
                ".....SSSSS......",
                "...BBBBBBBB.....",
                "..BBbBBBBbBB....",
                "..BBBBBBBBBB....",
                "..BBDDDDDDBB....",
                "..BBDDDDDDBB....",
                "...SDDDDDDS.....", // Skin hands
                "...sDDDDDDs.....", // Skin hand shadows
                "....PPPPPP......",
                "....PPPPPP......",
                "....PPPPPP......",
                "...PPPPPPPP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "...KK....KK.....",
                "..KKK....KKK....",
                "................",
                "................"
            };

            // WALK RIGHT 1
            string[] right_walk1 = {
                "................",
                "................",
                "....HHHHHH......",
                "...HHHHHHHH.....",
                "...HHHhHHhHH....",
                "..HHHHHHSSHH....",
                "..HHHHSSSESs....",
                "...HHSSSSSSH....",
                "...HsSSSSSs.....",
                ".....SSSSS......",
                "...BBBBBBBB.....",
                "..BBbBBBBbBB....",
                "..BBBBBBBBBB....",
                "..BBDDDDDDBB....",
                "..SSDDDDDDSS....",
                "...sDDDDDDs.....",
                "....PPPPPP......",
                "....PPPPPP......",
                "....PPPPPP......",
                "...PPPPPPPP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "...KK....KK.....",
                "..KKK....KKK....",
                "..........KKK...",
                "................",
                "................"
            };

            // WALK RIGHT 2
            string[] right_walk2 = {
                "................",
                "................",
                "....HHHHHH......",
                "...HHHHHHHH.....",
                "...HHHhHHhHH....",
                "..HHHHHHSSHH....",
                "..HHHHSSSESs....",
                "...HHSSSSSSH....",
                "...HsSSSSSs.....",
                ".....SSSSS......",
                "...BBBBBBBB.....",
                "..BBbBBBBbBB....",
                "..BBBBBBBBBB....",
                "..BBDDDDDDBB....",
                "..SSDDDDDDSS....",
                "...sDDDDDDs.....",
                "....PPPPPP......",
                "....PPPPPP......",
                "....PPPPPP......",
                "...PPPPPPPP.....",
                "...PP....PP.....",
                "...PP....PP.....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "....PP....PP....",
                "...KK....KK.....",
                "..KKK....KKK....",
                "..KKK...........",
                "................",
                "................"
            };

            // Mirror right arrays to get left arrays
            string[] left = MirrorGrid(right);
            string[] left_walk1 = MirrorGrid(right_walk1);
            string[] left_walk2 = MirrorGrid(right_walk2);

            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Idle_Down.png", down, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Idle_Up.png", up, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Idle_Left.png", left, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Idle_Right.png", right, colorMap, 16, 32);

            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Down_1.png", down_walk1, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Down_2.png", down_walk2, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Up_1.png", up_walk1, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Up_2.png", up_walk2, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Left_1.png", left_walk1, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Left_2.png", left_walk2, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Right_1.png", right_walk1, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Minh/Minh_Walk_Right_2.png", right_walk2, colorMap, 16, 32);
        }

        private static string[] MirrorGrid(string[] grid)
        {
            string[] mirrored = new string[grid.Length];
            for (int i = 0; i < grid.Length; i++)
            {
                char[] charArray = grid[i].ToCharArray();
                System.Array.Reverse(charArray);
                mirrored[i] = new string(charArray);
            }
            return mirrored;
        }
        private static void GenerateGuardSprites()
        {
            var colorMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'H', ColorFromHex("#2C3E50") }, // Helmet (Steel Blue)
                { 'S', ColorFromHex("#E0AE85") }, // Skin (Peach-Tan)
                { 'E', ColorFromHex("#EF4444") }, // Visor Glow (Red)
                { 'R', ColorFromHex("#C0392B") }, // Shirt (Crimson)
                { 'D', ColorFromHex("#7B241C") }, // Shirt Shadow
                { 'P', ColorFromHex("#1A252F") }, // Pants (Navy-Black)
                { 'p', ColorFromHex("#111922") }, // Pants Shadow
                { 'K', ColorFromHex("#111111") }  // Boots (Black)
            };

            string[] down = {
                "................",
                "................",
                "......HHHHHH....",
                "....HHHHHHHHHH..",
                "....HHEESSEESHH.",
                "....HHHHHHHHHH..",
                "....HSSSSSSSSSH.",
                "....HSSSSSSSSSH.",
                ".....sSSSSSSs...",
                "......SSSSSS....",
                "....RRRRRRRRRR..",
                "...RRrRRRRRRrRR.",
                "...RRRRRRRRRRRR.",
                "...RRDDDDDDDDRR.",
                "...RRDDDDDDDDRR.",
                "....SDDDDDDDDS..", // Skin hands
                "....sDDDDDDDDs..", // Skin hand shadows
                ".....PPPPPPPP...",
                ".....PPPPPPPP...",
                "....PPPPPPPPPP..",
                "....PPpPPPPpPP..",
                "....PPpPPPPpPP..",
                "....PPpPPPPpPP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....KK.KKKK.KK..",
                "....KKK.KKK.KKK.",
                "................",
                "................"
            };

            string[] up = {
                "................",
                "................",
                "......HHHHHH....",
                "....HHHHHHHHHH..",
                "....HHHHHHHHHH..",
                "....HHHHHHHHHH..",
                "....HHHHHHHHHH..",
                "....HHHHHHHHHH..",
                ".....HHHHHHHH...",
                "......HHHHHH....",
                "....RRRRRRRRRR..",
                "...RRrRRRRRRrRR.",
                "...RRRRRRRRRRRR.",
                "...RRDDDDDDDDRR.",
                "...RRDDDDDDDDRR.",
                "....SDDDDDDDDS..", // Skin hands
                "....sDDDDDDDDs..", // Skin hand shadows
                ".....PPPPPPPP...",
                ".....PPPPPPPP...",
                "....PPPPPPPPPP..",
                "....PPpPPPPpPP..",
                "....PPpPPPPpPP..",
                "....PPpPPPPpPP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....PP.PPPP.PP..",
                "....KK.KKKK.KK..",
                "....KKK.KKK.KKK.",
                "................",
                "................"
            };

            string[] left = {
                "................",
                "................",
                "......HHHHHH....",
                ".....HHHHHHHH...",
                "....HHEEHHHHH...",
                "....HHHHHHHHHH..",
                "....HSSSSSSHHH..",
                "....HSSSSSSHHH..",
                ".....sSSSSSsH...",
                "......SSSSS.....",
                ".....RRRRRRRR...",
                "....RRrRRRRrRR..",
                "....RRRRRRRRRR..",
                "....RRDDDDDDRR..",
                "....SSDDDDDDSS..", // Skin hands
                ".....sDDDDDDs...", // Skin hand shadows
                "......PPPPPP....",
                "......PPPPPP....",
                ".....PPPPPPPP...",
                ".....PPpPPpPP...",
                ".....PPpPPpPP...",
                ".....PPpPPpPP...",
                ".....PP.PP.PP...",
                ".....PP.PP.PP...",
                ".....PP.PP.PP...",
                ".....PP.PP.PP...",
                ".....PP.PP.PP...",
                ".....KK.KK.KK...",
                ".....KKK.K.KK...",
                "................",
                "................",
                "................"
            };

            string[] right = {
                "................",
                "................",
                "....HHHHHH......",
                "...HHHHHHHH.....",
                "...HHHHHEEHH....",
                "..HHHHHHHHHH....",
                "..HHHSSSSSSH....",
                "..HHHSSSSSSH....",
                "...HSSSSSsH.....",
                ".....SSSSS......",
                "...RRRRRRRR.....",
                "..RRrRRRRrRR....",
                "..RRRRRRRRRR....",
                "..RRDDDDDDRR....",
                "..SSDDDDDDSS....", // Skin hand
                "...sDDDDDDs.....", // Skin hand shadow
                "....PPPPPP......",
                "....PPPPPP......",
                "...PPPPPPPP.....",
                "...PPpPPpPP.....",
                "...PPpPPpPP.....",
                "...PPpPPpPP.....",
                "...PP.PP.PP.....",
                "...PP.PP.PP.....",
                "...PP.PP.PP.....",
                "...PP.PP.PP.....",
                "...PP.PP.PP.....",
                "...KK.KK.KK.....",
                "...KK.K.KKK.....",
                "................",
                "................",
                "................"
            };

            GenerateSpriteFromGrid("Assets/Sprites/Characters/Guards/Guard_Idle_Down.png", down, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Guards/Guard_Idle_Up.png", up, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Guards/Guard_Idle_Left.png", left, colorMap, 16, 32);
            GenerateSpriteFromGrid("Assets/Sprites/Characters/Guards/Guard_Idle_Right.png", right, colorMap, 16, 32);
        }

        private static void GenerateTileSprites()
        {
            // Room Floor (Wood Planks)
            var floorMap = new Dictionary<char, Color>
            {
                { '1', ColorFromHex("#D7A15C") }, // Light Wood
                { '2', ColorFromHex("#C58F49") }, // Medium Wood
                { '3', ColorFromHex("#9A6B30") }, // Dark Wood border
                { '4', ColorFromHex("#6D4617") }  // Shadow joint lines
            };
            string[] roomFloor = {
                "4444444444444444",
                "4111111142222224",
                "4111111142222224",
                "4111111142222224",
                "4444444444444444",
                "4222411111114224",
                "4222411111114224",
                "4222411111114224",
                "4444444444444444",
                "4111111142222224",
                "4111111142222224",
                "4111111142222224",
                "4444444444444444",
                "4222411111114224",
                "4222411111114224",
                "4444444444444444"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Tiles/Room/Floor.png", roomFloor, floorMap);

            // Room Wall (Warm Bricks)
            var wallMap = new Dictionary<char, Color>
            {
                { '1', ColorFromHex("#A37561") },
                { '2', ColorFromHex("#8D5B46") },
                { '3', ColorFromHex("#744633") },
                { '4', ColorFromHex("#4D2E21") },
                { '5', ColorFromHex("#BC917E") }
            };
            string[] roomWall = {
                "4444444444444444",
                "4555555545555554",
                "4511111345111114",
                "4512223345122234",
                "4333333343333334",
                "4444444444444444",
                "4554555555545554",
                "4514511111345114",
                "4524512223345124",
                "4334333333343334",
                "4444444444444444",
                "4555555545555554",
                "4511111345111114",
                "4512223345122234",
                "4333333343333334",
                "4444444444444444"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Tiles/Room/Wall.png", roomWall, wallMap);

            // Compound Floor (Concrete Grid)
            var compFloorMap = new Dictionary<char, Color>
            {
                { '1', ColorFromHex("#4F5D73") }, // Concrete Gray
                { '2', ColorFromHex("#65758F") }, // Light Gray highlight
                { '3', ColorFromHex("#313B4C") }, // Dark panel borders
                { '4', ColorFromHex("#222936") }  // Joints/Grout
            };
            string[] compFloor = {
                "3333333333333333",
                "3222222222222223",
                "3211111111111123",
                "3211111111311123",
                "3211111113111123",
                "3211111131111123",
                "3211111111111123",
                "3211111111111123",
                "3211111111111123",
                "3211111111111123",
                "3211111111111123",
                "3211111111111123",
                "3211111111111123",
                "3222222222222223",
                "3333333333333333",
                "4444444444444444"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Tiles/Compound/Floor.png", compFloor, compFloorMap);

            // Compound Wall (Steel Panels with Rust)
            var compWallMap = new Dictionary<char, Color>
            {
                { '1', ColorFromHex("#7F8C8D") }, // Base plate
                { '2', ColorFromHex("#95A5A6") }, // Highlight edge
                { '3', ColorFromHex("#34495E") }, // Shade edge
                { '4', ColorFromHex("#2C3E50") }, // Rivets/Gaps
                { '5', ColorFromHex("#D35400") }  // Rust leaks
            };
            string[] compWall = {
                "4444444444444444",
                "4111111111111124",
                "4122222222222224",
                "4124422222442224",
                "4124422222442224",
                "4122222222222224",
                "4122222552222224",
                "4122225252222224",
                "4122222552222224",
                "4122222222222224",
                "4122222222222224",
                "4122222222222224",
                "4122222222222224",
                "4122222222222224",
                "4333333333333334",
                "4444444444444444"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Tiles/Compound/Wall.png", compWall, compWallMap);

            // Forest Floor (Grass tufts)
            var forestFloorMap = new Dictionary<char, Color>
            {
                { '1', ColorFromHex("#2ECC71") }, // Grass green
                { '2', ColorFromHex("#27AE60") }, // Shadow green
                { '3', ColorFromHex("#1B4D3E") }, // Deep shade
                { '4', ColorFromHex("#875A3C") }  // Dirt undercoat
            };
            string[] forestFloor = {
                "2212221222122212",
                "2111211121112111",
                "1131113111311131",
                "3333333333333333",
                "2212221222122212",
                "2111211121112111",
                "1131113111311131",
                "3333333333333333",
                "2212221222122212",
                "2111211121112111",
                "1131113111311131",
                "3333333333333333",
                "2212221222122212",
                "2111211121112111",
                "1131113111311131",
                "3333333333333333"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Tiles/Forest/Floor.png", forestFloor, forestFloorMap);

            // Forest Wall (Dense tree trunks and bushes)
            var forestWallMap = new Dictionary<char, Color>
            {
                { '1', ColorFromHex("#1E4620") }, // Leaves
                { '2', ColorFromHex("#5C4033") }, // Bark trunk
                { '3', ColorFromHex("#0A1C0C") }, // Dark void
                { '4', ColorFromHex("#2D6A4F") }  // Mossy highlights
            };
            string[] forestWall = {
                "3333333333333333",
                "3111331113311133",
                "1141111411114111",
                "1444114441144411",
                "3141331413314133",
                "3313333133331333",
                "3323333233332333",
                "3323333233332333",
                "3323333233332333",
                "3323333233332333",
                "3323333233332333",
                "3323333233332333",
                "3323333233332333",
                "3323333233332333",
                "3333333333333333",
                "3333333333333333"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Tiles/Forest/Wall.png", forestWall, forestWallMap);
        }

        private static void GenerateItemSprites()
        {
            // Laptop
            var laptopMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'L', ColorFromHex("#5A6368") }, // Case
                { 'C', ColorFromHex("#3A3E41") }, // Case Shadow
                { 'S', ColorFromHex("#A3E8FC") }, // Screen glow
                { 'W', ColorFromHex("#FFFFFF") }, // White shine
                { 'K', ColorFromHex("#1F2326") }  // Keyboard
            };
            string[] laptop = {
                "................",
                "................",
                "....LLLLLLLL....",
                "...LSSSSSSSWL...",
                "..LSSSSSSSSWL...",
                "..LSSSSSSSSWL...",
                "..LSSSSSSSSWL...",
                "..LCCCCCCCCCL...",
                "..LLLLLLLLLLL...",
                ".KKKKKKKKKKKKK..",
                ".K...K.K.K...K..",
                ".KKKKKKKKKKKKK..",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Laptop.png", laptop, laptopMap);

            // Phone
            GeneratePhoneSpriteFromImage();

            // Document (Bills)
            var docMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'P', ColorFromHex("#F5F6F8") },
                { 'S', ColorFromHex("#D1D5DB") },
                { 'T', ColorFromHex("#4B5563") },
                { 'R', ColorFromHex("#EF4444") }
            };
            string[] doc = {
                "................",
                "....PPPPPPPP....",
                "...PPPPPPPPPP...",
                "..PPPPPPPPPPSS..",
                "..PPTTTTTTTTSS..",
                "..PPPTTTTTTTSS..",
                "..PPTTTTTTTTSS..",
                "..PPTTRRRRTTSS..",
                "..PPPTTRRRRTSS..",
                "..PPTTRRRRTTSS..",
                "..PPTTTTTTTTSS..",
                "..PPPTTTTTTTSS..",
                "..PPPPPPPPPPSS..",
                "...SSSSSSSSSS...",
                "....SSSSSSSS....",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Document.png", doc, docMap);

            // Key
            var keyMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'Y', ColorFromHex("#F1C40F") },
                { 'O', ColorFromHex("#D68910") }
            };
            string[] key = {
                "................",
                "......YYYY......",
                "....YYOOOOYY....",
                "...YYOO..OOYY...",
                "...YYO....OYY...",
                "...YYOO..OOYY...",
                "....YYOOOOYY....",
                "......YYYY......",
                "......YYYY......",
                "......YOYO......",
                "......YYYYYY....",
                "......YOYOYY....",
                "......YYYY......",
                "......YOYO......",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Key.png", key, keyMap);

            // Map
            var mapMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'M', ColorFromHex("#EBDFCE") },
                { 'S', ColorFromHex("#C7BAA8") },
                { 'R', ColorFromHex("#E74C3C") },
                { 'B', ColorFromHex("#3498DB") }
            };
            string[] gameMap = {
                "................",
                "....MMMMMMMM....",
                "...MMMMMMMMMM...",
                "..MMMMMMMMMMSS..",
                "..MMBMMMMMMMSS..",
                "..MMBBMMMMMMSS..",
                "..MMMBBMMMRRSS..",
                "..MMMMBBMRRRSS..",
                "..MMMMMBBMRRSS..",
                "..MMMMMMBBMMSS..",
                "..MMMMMMMBBMSS..",
                "..MMMMMMMMMMSS..",
                "...SSSSSSSSSS...",
                "....SSSSSSSS....",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Map.png", gameMap, mapMap);

            // Rope
            var ropeMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'R', ColorFromHex("#C8AD7F") },
                { 'D', ColorFromHex("#967D54") }
            };
            string[] rope = {
                "................",
                "......RRDD......",
                ".....RRDD.......",
                "....RRDD........",
                "...RRDD.........",
                "..RRDD..RRDD....",
                ".RRDD..RRDD.....",
                "......RRDD......",
                ".....RRDD..RRDD.",
                "....RRDD..RRDD..",
                "...RRDD..RRDD...",
                "..RRDD..........",
                ".RRDD...........",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Rope.png", rope, ropeMap);

            // Cutter
            var cutterMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'R', ColorFromHex("#E74C3C") },
                { 'D', ColorFromHex("#922B21") },
                { 'M', ColorFromHex("#BDC3C7") },
                { 'S', ColorFromHex("#7F8C8D") }
            };
            string[] cutter = {
                "................",
                "..........MM....",
                ".........MMS....",
                "........MMS.....",
                ".......MMS......",
                "......RRS.......",
                ".....RRDS.......",
                "....RRDS........",
                "...RRDS.........",
                "..RRDS..........",
                ".RRDS...........",
                "................",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Cutter.png", cutter, cutterMap);

            // Flashlight
            var flashlightMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'F', ColorFromHex("#34495E") },
                { 'D', ColorFromHex("#2C3E50") },
                { 'Y', ColorFromHex("#F1C40F") },
                { 'W', ColorFromHex("#FFFFFF") }
            };
            string[] flashlight = {
                "................",
                "..........YY....",
                ".........FDDYY..",
                "........FDDYYYY.",
                ".......FDDYYYYW.",
                "......FDD....WW.",
                ".....FDD........",
                "....FDD.........",
                "...FDD..........",
                "..FDD...........",
                "................",
                "................",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Flashlight.png", flashlight, flashlightMap);

            // Trash (Recycle Bin)
            var trashMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'T', ColorFromHex("#7F8C8D") }, // Base body
                { 'L', ColorFromHex("#BDC3C7") }, // Lid
                { 'K', ColorFromHex("#2C3E50") }  // Lines
            };
            string[] trash = {
                "................",
                ".....LLLLLL.....",
                "....LLLLLLLL....",
                "....KKLLLLKK....",
                "....TTTTTTTT....",
                "...TKKTTTKKTT...",
                "...TKKTTTKKTT...",
                "...TKKTTTKKTT...",
                "...TKKTTTKKTT...",
                "...TKKTTTKKTT...",
                "...TKKTTTKKTT...",
                "...TKKTTTKKTT...",
                "....TTTTTTTT....",
                ".....TTTTTT.....",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Trash.png", trash, trashMap);

            // Chrome
            var chromeMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'R', ColorFromHex("#EA4335") },
                { 'G', ColorFromHex("#34A853") },
                { 'Y', ColorFromHex("#FBBC05") },
                { 'B', ColorFromHex("#4285F4") },
                { 'W', ColorFromHex("#FFFFFF") }
            };
            string[] chrome = {
                "................",
                "......RRRR......",
                "....RRRRRRRR....",
                "...RRRRRRRRRR...",
                "..RRRRWWWWRYYY..",
                "..RRRWWBBWWYYY..",
                "..RRRWWBBWWYYY..",
                "..RGGWWWWWWYYY..",
                "..GGGGWWWWYYYY..",
                "...GGGGGGYYYY...",
                "....GGGGGGGG....",
                "......GGGG......",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/Items/Chrome.png", chrome, chromeMap);
        }

        private static void GenerateUISprites()
        {
            var heartMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'R', ColorFromHex("#EF4444") },
                { 'D', ColorFromHex("#B91C1C") },
                { 'W', ColorFromHex("#FFFFFF") }
            };
            string[] heart = {
                "................",
                "................",
                "...RR..RR.......",
                "..RWRRRWRR......",
                ".RWRRRRWRRR.....",
                ".RRRRRRRRRR.....",
                ".RRRRRRRRRR.....",
                "..RRRRRRRR......",
                "...RRRRRR.......",
                "....RRRR........",
                ".....RR.........",
                "................",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/Heart.png", heart, heartMap);

            // Windows Logo
            var winLogoMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'B', ColorFromHex("#0078D4") } // Windows Blue
            };
            string[] winLogo = {
                "................",
                "................",
                "...BBBB.BBBB....",
                "...BBBB.BBBB....",
                "...BBBB.BBBB....",
                "...BBBB.BBBB....",
                "................",
                "...BBBB.BBBB....",
                "...BBBB.BBBB....",
                "...BBBB.BBBB....",
                "...BBBB.BBBB....",
                "................",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/WinLogo.png", winLogo, winLogoMap);

            // 1. NotesIcon
            var notesMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'Y', ColorFromHex("#F1C40F") }, // Yellow page
                { 'T', ColorFromHex("#D35400") }, // Orange top
                { 'K', ColorFromHex("#34495E") }  // Lines
            };
            string[] notes = {
                "................",
                "....TTTTTTTT....",
                "...TYYYYYYYYT...",
                "..TYYYYYYYYYYT..",
                "..TYYYYYYYYYYT..",
                "..TYYKKKKKKYYT..",
                "..TYYYYYYYYYYT..",
                "..TYYKKKKKKYYT..",
                "..TYYYYYYYYYYT..",
                "..TYYKKKKKKYYT..",
                "..TYYYYYYYYYYT..",
                "..TYYYYYYYYYYT..",
                "...TYYYYYYYYT...",
                "....TTTTTTTT....",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/NotesIcon.png", notes, notesMap);

            // 2. PhotosIcon
            var photosMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'B', ColorFromHex("#3498DB") }, // Sky Blue
                { 'G', ColorFromHex("#2ECC71") }, // Grass Green
                { 'Y', ColorFromHex("#F1C40F") }, // Sun Yellow
                { 'W', ColorFromHex("#ECF0F1") }  // White frame
            };
            string[] photos = {
                "................",
                "...WWWWWWWWWW...",
                "..WBBBBBBBBBBW..",
                "..WBBBYYBBBBBW..",
                "..WBBBYYBBBBBW..",
                "..WBBBBBBBBBBW..",
                "..WBBBBBBGGGBW..",
                "..WBBBBGGGGGBW..",
                "..WBBBGGGGGGBW..",
                "..WBBGGGGGGGBW..",
                "..WGGGGGGGGGBW..",
                "..WGGGGGGGGGBW..",
                "..WGGGGGGGGGBW..",
                "..WWWWWWWWWWWW..",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/PhotosIcon.png", photos, photosMap);

            // 3. WeatherIcon
            var weatherMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'B', ColorFromHex("#5DADE2") }, // Light Blue sky
                { 'Y', ColorFromHex("#F39C12") }, // Sun Yellow
                { 'W', ColorFromHex("#F2F3F4") }  // Cloud White
            };
            string[] weather = {
                "................",
                "......YYYY......",
                "....YYYYYYYY....",
                "...YYYYYYYYYY...",
                "..YYYYWWWWYYYY..",
                "..YYYWWWWWWYYY..",
                "...WWWWWWWWWW...",
                "...WWWWWWWWWW...",
                "..WWWWWWWWWWWW..",
                "..WWWWWWWWWWWW..",
                "...WWWWWWWWWW...",
                "....WWWWWWWW....",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/WeatherIcon.png", weather, weatherMap);

            // 4. SettingsIcon
            var settingsMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'G', ColorFromHex("#7F8C8D") }, // Gear Grey
                { 'K', ColorFromHex("#34495E") }  // Hole Dark
            };
            string[] settings = {
                "................",
                "......GGG.......",
                "....GGGGGGG.....",
                "...GGGGKGGGG....",
                "..GGGKKKKKGGG...",
                "..GGKKKKKKKGG...",
                "..GGKKKKKKKGG...",
                "..GGGKKKKKGGG...",
                "...GGGGKGGGG....",
                "....GGGGGGG.....",
                "......GGG.......",
                "................",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/SettingsIcon.png", settings, settingsMap);

            // 5. MessagesIcon
            var messagesMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'G', ColorFromHex("#2ECC71") }, // Green bubble
                { 'W', ColorFromHex("#FFFFFF") }  // White text lines
            };
            string[] messages = {
                "................",
                "....GGGGGGGG....",
                "...GGGGGGGGGG...",
                "..GGGGGGGGGGGG..",
                "..GGWWWWWWWWGG..",
                "..GGGGGGGGGGGG..",
                "..GGWWWWWWWWGG..",
                "..GGGGGGGGGGGG..",
                "..GGGGGGGGGGGG..",
                "...GGGGGGGGGG...",
                "....GGGGGGGG....",
                ".....GGGGG......",
                "......GGG.......",
                ".......G........",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/MessagesIcon.png", messages, messagesMap);

            // 6. BrowserIcon
            var browserMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'B', ColorFromHex("#3498DB") }, // Blue center
                { 'R', ColorFromHex("#E74C3C") }, // Red
                { 'Y', ColorFromHex("#F1C40F") }, // Yellow
                { 'G', ColorFromHex("#2ECC71") }  // Green
            };
            string[] browser = {
                "................",
                "......RRRR......",
                "....RRRRRRRR....",
                "...RRRRRRRRRR...",
                "..RRRRBBBBRYYY..",
                "..RRRBBBBBBYYY..",
                "..RRRBBBBBBYYY..",
                "..RGGBBBBBBYYY..",
                "..GGGGBBBBYYYY..",
                "...GGGGGGYYYY...",
                "....GGGGGGGG....",
                "......GGGG......",
                "................",
                "................",
                "................",
                "................"
            };
            GenerateSpriteFromGrid("Assets/Sprites/UI/BrowserIcon.png", browser, browserMap);

            // Procedural iOS 7 Style App Icon Backgrounds
            GenerateRoundedGradientTexture("Assets/Sprites/UI/AppIconBg_Green.png", ColorFromHex("#54f07a"), ColorFromHex("#22d050"), 64, 64, 12f);
            GenerateRoundedGradientTexture("Assets/Sprites/UI/AppIconBg_Blue.png", ColorFromHex("#52a5ff"), ColorFromHex("#1973f0"), 64, 64, 12f);
            GenerateRoundedGradientTexture("Assets/Sprites/UI/AppIconBg_Orange.png", ColorFromHex("#ffcc33"), ColorFromHex("#ff9500"), 64, 64, 12f);
            GenerateRoundedGradientTexture("Assets/Sprites/UI/AppIconBg_Light.png", ColorFromHex("#ffffff"), ColorFromHex("#f2f2f7"), 64, 64, 12f);
            GenerateRoundedGradientTexture("Assets/Sprites/UI/AppIconBg_Cyan.png", ColorFromHex("#64d8ff"), ColorFromHex("#1ca0f0"), 64, 64, 12f);
            GenerateRoundedGradientTexture("Assets/Sprites/UI/AppIconBg_Grey.png", ColorFromHex("#bdc3c7"), ColorFromHex("#8e9eab"), 64, 64, 12f);

            // Procedural iOS 7 Style Phone Wallpaper (replaces old narrow phone_wallpaper.png)
            GenerateWallpaperTexture("Assets/Sprites/UI/phone_wallpaper.png", ColorFromHex("#70d2f6"), ColorFromHex("#1c3b70"), 360, 640);

            // Procedural Red Circular Notification Dot
            GenerateCircleTexture("Assets/Sprites/UI/NotificationDot.png", ColorFromHex("#ff3b30"), 32);
        }

        private static void GenerateCircleTexture(string path, Color color, int size = 32)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size / 2.0f;
            float radiusSq = (size / 2.0f) * (size / 2.0f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - center;
                    float dy = y + 0.5f - center;
                    if (dx * dx + dy * dy <= radiusSq)
                    {
                        tex.SetPixel(x, y, color);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GenerateWallpaperTexture(string path, Color topColor, Color bottomColor, int width = 360, int height = 640)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / (height - 1);
                Color col = Color.Lerp(bottomColor, topColor, t);
                for (int x = 0; x < width; x++)
                {
                    tex.SetPixel(x, y, col);
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GenerateRoundedGradientTexture(string path, Color topColor, Color bottomColor, int width = 64, int height = 64, float cornerRadius = 12f)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float rSq = cornerRadius * cornerRadius;
            float cx = width / 2.0f;
            float cy = height / 2.0f;
            float rx = width / 2.0f - cornerRadius;
            float ry = height / 2.0f - cornerRadius;

            for (int y = 0; y < height; y++)
            {
                float t = (float)y / (height - 1);
                Color col = Color.Lerp(bottomColor, topColor, t);

                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Max(0, Mathf.Abs(x + 0.5f - cx) - rx);
                    float dy = Mathf.Max(0, Mathf.Abs(y + 0.5f - cy) - ry);

                    if (dx * dx + dy * dy > rSq)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        tex.SetPixel(x, y, col);
                    }
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void SaveUITexture(Texture2D tex, string path, bool pixelArt = false)
        {
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            AssetDatabase.ImportAsset(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = pixelArt ? FilterMode.Point : FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }

        private static void SaveTexture(Texture2D tex, string path)
        {
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            // Force pixel perfect import settings
            AssetDatabase.ImportAsset(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 16;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                
                // Disable mipmaps
                importer.mipmapEnabled = false;

                // Save
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }

        private static Color ColorFromHex(string hex)
        {
            Color col;
            if (ColorUtility.TryParseHtmlString(hex, out col))
            {
                return col;
            }
            return Color.white;
        }

        private static void GeneratePhoneSpriteFromImage()
        {
            string sourcePath = @"C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\modern_phone_sprite_1781175375128.png";
            string destPath = "Assets/Sprites/Items/Phone.png";

            if (!File.Exists(sourcePath))
            {
                Debug.LogError($"Source image not found: {sourcePath}");
                return;
            }

            byte[] fileData = File.ReadAllBytes(sourcePath);
            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(fileData))
            {
                Debug.LogError("Failed to load source image texture.");
                return;
            }

            int width = texture.width;
            int height = texture.height;
            Color[] pixels = texture.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                Color c = pixels[i];
                // Since the phone is centered on a pure white background, make white pixels transparent
                if (c.r > 0.95f && c.g > 0.95f && c.b > 0.95f)
                {
                    pixels[i] = Color.clear;
                }
            }

            Texture2D transparentTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            transparentTexture.SetPixels(pixels);
            transparentTexture.Apply();

            byte[] pngData = transparentTexture.EncodeToPNG();
            File.WriteAllBytes(destPath, pngData);

            AssetDatabase.ImportAsset(destPath);
            TextureImporter importer = AssetImporter.GetAtPath(destPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 1024; // High-res matching width so it imports at 1 unit size
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            Debug.Log($"Processed phone image saved to {destPath}");
        }
    }
}
#endif
