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
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string scriptPath = Path.Combine(projectRoot, "scratch", "generate_minh_32x64.py");
            
            if (File.Exists(scriptPath))
            {
                Debug.Log("Running Python Sprite Generator...");
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = Application.platform == RuntimePlatform.WindowsEditor ? "python" : "python3",
                    Arguments = $"\"{scriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    if (process.ExitCode != 0)
                    {
                        Debug.LogError($"Python script failed with code {process.ExitCode}: {error}");
                    }
                    else
                    {
                        Debug.Log($"Python script output: {output}");
                    }
                }
            }
            else
            {
                Debug.LogError($"Python script not found at {scriptPath}!");
            }
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

        private static void ConfigureImportSettings(string path, int ppu)
        {
            AssetDatabase.ImportAsset(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = ppu;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }

        private static void GenerateGuardSprites()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string scriptPath = Path.Combine(projectRoot, "scratch", "generate_npcs_32x64.py");
            
            if (File.Exists(scriptPath))
            {
                Debug.Log("Running Python NPC Generator...");
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = Application.platform == RuntimePlatform.WindowsEditor ? "python" : "python3",
                    Arguments = $"\"{scriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    process.WaitForExit();
                    string error = process.StandardError.ReadToEnd();
                    if (process.ExitCode != 0)
                    {
                        Debug.LogError($"Python NPC script failed: {error}");
                    }
                }
            }

            string[] paths = {
                "Assets/Sprites/Characters/Guards/Guard_Idle_Down.png",
                "Assets/Sprites/Characters/Guards/Guard_Idle_Up.png",
                "Assets/Sprites/Characters/Guards/Guard_Idle_Left.png",
                "Assets/Sprites/Characters/Guards/Guard_Idle_Right.png",
                "Assets/Sprites/Characters/Guards/Recruiter_Idle_Down.png",
                "Assets/Sprites/Characters/Guards/Recruiter_Idle_Up.png",
                "Assets/Sprites/Characters/Guards/Recruiter_Idle_Left.png",
                "Assets/Sprites/Characters/Guards/Recruiter_Idle_Right.png"
            };

            foreach (var path in paths)
            {
                ConfigureImportSettings(path, 38);
            }
            AssetDatabase.Refresh();
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

            // Procedural Premium Abstract Phone Wallpaper
            GeneratePremiumWallpaper("Assets/Sprites/UI/phone_wallpaper.png", 360, 640);

            // Procedural Phone Mask and Frame Overlay for rounded corners and modern Dynamic Island bezel
            GenerateRoundedMaskTexture("Assets/Sprites/UI/phone_mask.png", 360, 640, 38f);
            GeneratePhoneFrameTexture("Assets/Sprites/UI/phone_frame.png", 360, 640, 38f, 8f);

            // Procedural Red Circular Notification Dot
            GenerateCircleTexture("Assets/Sprites/UI/NotificationDot.png", ColorFromHex("#ff3b30"), 32);

            // Procedural White Rounded Rect for buttons/inputs
            GenerateRoundedGradientTexture("Assets/Sprites/UI/RoundedRect.png", Color.white, Color.white, 64, 64, 8f);

            // Procedural Dice Faces
            for (int i = 1; i <= 6; i++)
            {
                GenerateDiceFace($"Assets/Sprites/UI/Dice_{i}.png", i, 64);
            }

            // Procedural Suits
            GenerateSuitHeart("Assets/Sprites/UI/Suit_Heart.png", 32);
            GenerateSuitDiamond("Assets/Sprites/UI/Suit_Diamond.png", 32);
            GenerateSuitSpade("Assets/Sprites/UI/Suit_Spade.png", 32);
            GenerateSuitClub("Assets/Sprites/UI/Suit_Club.png", 32);

            // Procedural Roulette Wheel
            GenerateRouletteWheel("Assets/Sprites/UI/Roulette_Wheel.png", 256);

            // Procedural Card Background
            GenerateCardBackground("Assets/Sprites/UI/Card_Background.png", 64, 96, 6f);

            // Procedural Tài Xỉu Bowl Cover
            GenerateBowl("Assets/Sprites/UI/TaiXiu_Bowl.png", 220, 120);
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
            string destPath = "Assets/Sprites/Items/Phone.png";

            // 32x32 pixel art phone — PPU=16 → renders at 2x2 world units (clearly visible)
            var phoneMap = new Dictionary<char, Color>
            {
                { '.', Color.clear },
                { 'B', ColorFromHex("#1c1c1e") }, // Dark bezel
                { 'b', ColorFromHex("#2c2c2e") }, // Lighter titanium frame
                { 'D', ColorFromHex("#050505") }, // Dynamic Island black
                { 'V', ColorFromHex("#120c1f") }, // Wallpaper Deep Violet/Navy
                { 'M', ColorFromHex("#ff2a85") }, // Pink glow
                { 'C', ColorFromHex("#00f0ff") }, // Cyan glow
                { 'P', ColorFromHex("#8a2be2") }, // Purple
                { 'w', ColorFromHex("#ffffff") }  // Home indicator bar
            };

            string[] phone = {
                ".....bbbbbbbbbbbbbbbbbbbbbb.....", // 31
                ".....bBBBBBBBBBBBBBBBBBBBBb.....", // 30
                ".....bBBBBBBBBBBBBBBBBBBBBb.....", // 29
                ".....bBBBBBBDDDDDDDDBBBBBBb.....", // 28 - Dynamic Island
                ".....bBMPPVVVVVVVVVVVVVVVVBb.....", // 27 - Pink glow
                ".....bBMPPPVVVVVVVVVVVVVVBb.....", // 26
                ".....bBPPPVVVVVVVVVVVVVVVVBb.....", // 25
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 24
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 23
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 22
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 21
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 20
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 19
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 18
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 17
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 16
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 15
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 14
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 13
                ".....bBVVVVVVVVVVVVVVVCCCBb.....", // 12 - Cyan glow
                ".....bBVVVVVVVVVVVVVVCCCCBb.....", // 11
                ".....bBVVVVVVVVVVVVVVVCCCBb.....", // 10
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 9
                ".....bBVVVVVVVVVVVVVVVVVVBb.....", // 8
                ".....bBVVVVVVwwwwwwVVVVVVBb.....", // 7 - Home Indicator
                ".....bBBBBBBBBBBBBBBBBBBBBb.....", // 6
                ".....bBBBBBBBBBBBBBBBBBBBBb.....", // 5
                ".....bbbbbbbbbbbbbbbbbbbbbb.....", // 4
                "................................", // 3
                "................................", // 2
                "................................", // 1
                "................................"  // 0
            };

            GenerateSpriteFromGrid(destPath, phone, phoneMap, 32, 32);

            AssetDatabase.ImportAsset(destPath);
            TextureImporter importer = AssetImporter.GetAtPath(destPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 16; // 32px / 16 PPU = 2 world units — visible on bed!
                importer.filterMode = FilterMode.Point; // crisp pixel art
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            Debug.Log($"Phone sprite generated (32x32, PPU=16) at {destPath}");
        }

        private static void GeneratePremiumWallpaper(string path, int width = 360, int height = 640)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            {
                float ny = (float)y / (height - 1);
                for (int x = 0; x < width; x++)
                {
                    float nx = (float)x / (width - 1);
                    
                    // Base gradient: diagonal from bottom-left (dark navy) to top-right (lavender gray)
                    float t = (nx + ny) / 2f;
                    Color baseCol = Color.Lerp(ColorFromHex("#120c1f"), ColorFromHex("#2d124d"), t);
                    
                    // Add a glowing magenta/pink light at top-left
                    float distToTopLeft = Mathf.Sqrt(nx * nx + (1f - ny) * (1f - ny));
                    float glow1 = Mathf.Max(0f, 1f - distToTopLeft / 0.8f);
                    glow1 = glow1 * glow1; // smoothstep-like
                    Color glow1Col = ColorFromHex("#ff2a85");
                    baseCol = Color.Lerp(baseCol, glow1Col, glow1 * 0.45f);
                    
                    // Add a glowing cyan/blue light at bottom-right
                    float distToBottomRight = Mathf.Sqrt((1f - nx) * (1f - nx) + ny * ny);
                    float glow2 = Mathf.Max(0f, 1f - distToBottomRight / 0.9f);
                    glow2 = glow2 * glow2;
                    Color glow2Col = ColorFromHex("#00f0ff");
                    baseCol = Color.Lerp(baseCol, glow2Col, glow2 * 0.4f);
                    
                    // Add a soft golden highlight near the top center
                    float distToTopCenter = Mathf.Sqrt((nx - 0.5f) * (nx - 0.5f) * 2f + (1f - ny) * (1f - ny));
                    float glow3 = Mathf.Max(0f, 1f - distToTopCenter / 0.6f);
                    glow3 = glow3 * glow3;
                    Color glow3Col = ColorFromHex("#ffb347");
                    baseCol = Color.Lerp(baseCol, glow3Col, glow3 * 0.25f);
                    
                    tex.SetPixel(x, y, baseCol);
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GenerateRoundedMaskTexture(string path, int width = 360, int height = 640, float cornerRadius = 38f)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float cx = width / 2.0f;
            float cy = height / 2.0f;
            float rSq = cornerRadius * cornerRadius;
            float rx = width / 2.0f - cornerRadius;
            float ry = height / 2.0f - cornerRadius;
            
            for (int y = 0; y < height; y++)
            {
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
                        tex.SetPixel(x, y, Color.white);
                    }
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GeneratePhoneFrameTexture(string path, int width = 360, int height = 640, float cornerRadius = 38f, float bezelWidth = 8f)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float cx = width / 2.0f;
            float cy = height / 2.0f;
            
            // Bezel colors
            Color outerBezelColor = ColorFromHex("#1e1e24"); // Titanium gray
            Color innerBezelColor = ColorFromHex("#08080a"); // Pure black bezel
            Color transparent = Color.clear;
            
            float outerR = cornerRadius;
            float innerR = cornerRadius - bezelWidth;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dxOuter = Mathf.Max(0, Mathf.Abs(x + 0.5f - cx) - (width / 2.0f - outerR));
                    float dyOuter = Mathf.Max(0, Mathf.Abs(y + 0.5f - cy) - (height / 2.0f - outerR));
                    float distOuterSq = dxOuter * dxOuter + dyOuter * dyOuter;
                    
                    float dxInner = Mathf.Max(0, Mathf.Abs(x + 0.5f - cx) - (width / 2.0f - bezelWidth - innerR));
                    float dyInner = Mathf.Max(0, Mathf.Abs(y + 0.5f - cy) - (height / 2.0f - bezelWidth - innerR));
                    float distInnerSq = dxInner * dxInner + dyInner * dyInner;
                    
                    // Dynamic Island (Pill-shaped cutout) near the top
                    float diX = width / 2.0f;
                    float diY = height - 28f;
                    float diW = 74f;
                    float diH = 18f;
                    float diR = 9f;
                    
                    float dxDi = Mathf.Max(0, Mathf.Abs(x + 0.5f - diX) - (diW / 2.0f - diR));
                    float dyDi = Mathf.Max(0, Mathf.Abs(y + 0.5f - diY) - (diH / 2.0f - diR));
                    bool inDynamicIsland = (dxDi * dxDi + dyDi * dyDi <= diR * diR);
                    
                    // Speaker grill at the very top (optional, but let's add it for extra realism!)
                    bool inSpeaker = (x >= width / 2.0f - 20f && x <= width / 2.0f + 20f && y >= height - 9f && y <= height - 7f);
                    
                    if (inDynamicIsland)
                    {
                        float camX = diX - 20f;
                        float camY = diY;
                        float dxCam = x + 0.5f - camX;
                        float dyCam = y + 0.5f - camY;
                        if (dxCam * dxCam + dyCam * dyCam <= 9f)
                        {
                            tex.SetPixel(x, y, ColorFromHex("#0d1117"));
                        }
                        else
                        {
                            tex.SetPixel(x, y, ColorFromHex("#050505"));
                        }
                    }
                    else if (inSpeaker)
                    {
                        tex.SetPixel(x, y, ColorFromHex("#2a2a2a"));
                    }
                    else if (distInnerSq <= innerR * innerR && 
                             x >= bezelWidth && x < width - bezelWidth && 
                             y >= bezelWidth && y < height - bezelWidth)
                    {
                        tex.SetPixel(x, y, transparent);
                    }
                    else if (distOuterSq <= outerR * outerR)
                    {
                        float outerRimDist = outerR - 1.5f;
                        if (dxOuter * dxOuter + dyOuter * dyOuter > outerRimDist * outerRimDist || 
                            x < 1 || x >= width - 1 || y < 1 || y >= height - 1)
                        {
                            tex.SetPixel(x, y, outerBezelColor);
                        }
                        else
                        {
                            tex.SetPixel(x, y, innerBezelColor);
                        }
                    }
                    else
                    {
                        tex.SetPixel(x, y, transparent);
                    }
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GenerateDiceFace(string path, int faceNumber, int size = 64)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cornerRadius = 10f;
            float cx = size / 2.0f;
            float cy = size / 2.0f;
            float rx = size / 2.0f - cornerRadius;
            float ry = size / 2.0f - cornerRadius;
            float rSq = cornerRadius * cornerRadius;

            Color bgCol = Color.white;
            Color borderCol = ColorFromHex("#BDC3C7");
            Color dotCol = ColorFromHex("#2C3E50");
            Color redDotCol = ColorFromHex("#E74C3C");

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0, Mathf.Abs(x + 0.5f - cx) - rx);
                    float dy = Mathf.Max(0, Mathf.Abs(y + 0.5f - cy) - ry);

                    if (dx * dx + dy * dy > rSq)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        if (x < 2 || x >= size - 2 || y < 2 || y >= size - 2 || (dx * dx + dy * dy > (cornerRadius - 2) * (cornerRadius - 2)))
                        {
                            tex.SetPixel(x, y, borderCol);
                        }
                        else
                        {
                            tex.SetPixel(x, y, bgCol);
                        }
                    }
                }
            }

            float dotRadius = size * 0.08f;
            float dotRadiusSq = dotRadius * dotRadius;

            List<Vector2> dotCoords = new List<Vector2>();
            Color finalDotCol = dotCol;

            if (faceNumber == 1)
            {
                dotCoords.Add(new Vector2(0.5f, 0.5f));
                finalDotCol = redDotCol;
                dotRadius = size * 0.12f;
                dotRadiusSq = dotRadius * dotRadius;
            }
            else if (faceNumber == 2)
            {
                dotCoords.Add(new Vector2(0.25f, 0.25f));
                dotCoords.Add(new Vector2(0.75f, 0.75f));
            }
            else if (faceNumber == 3)
            {
                dotCoords.Add(new Vector2(0.25f, 0.25f));
                dotCoords.Add(new Vector2(0.5f, 0.5f));
                dotCoords.Add(new Vector2(0.75f, 0.75f));
            }
            else if (faceNumber == 4)
            {
                dotCoords.Add(new Vector2(0.25f, 0.25f));
                dotCoords.Add(new Vector2(0.25f, 0.75f));
                dotCoords.Add(new Vector2(0.75f, 0.25f));
                dotCoords.Add(new Vector2(0.75f, 0.75f));
            }
            else if (faceNumber == 5)
            {
                dotCoords.Add(new Vector2(0.25f, 0.25f));
                dotCoords.Add(new Vector2(0.25f, 0.75f));
                dotCoords.Add(new Vector2(0.5f, 0.5f));
                dotCoords.Add(new Vector2(0.75f, 0.25f));
                dotCoords.Add(new Vector2(0.75f, 0.75f));
            }
            else if (faceNumber == 6)
            {
                dotCoords.Add(new Vector2(0.25f, 0.25f));
                dotCoords.Add(new Vector2(0.25f, 0.5f));
                dotCoords.Add(new Vector2(0.25f, 0.75f));
                dotCoords.Add(new Vector2(0.75f, 0.25f));
                dotCoords.Add(new Vector2(0.75f, 0.5f));
                dotCoords.Add(new Vector2(0.75f, 0.75f));
            }

            foreach (Vector2 p in dotCoords)
            {
                float px = p.x * size;
                float py = p.y * size;

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float dx = x + 0.5f - px;
                        float dy = y + 0.5f - py;
                        if (dx * dx + dy * dy <= dotRadiusSq)
                        {
                            tex.SetPixel(x, y, finalDotCol);
                        }
                    }
                }
            }

            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GenerateSuitHeart(string path, int size = 32)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color red = ColorFromHex("#EF4444");
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f - size / 2f) / (size / 2f);
                    float ny = (y + 0.5f - size / 2f) / (size / 2f);
                    
                    float xVal = nx * 1.2f;
                    float yVal = ny * 1.2f - 0.2f;
                    float f = xVal * xVal + yVal * yVal - 0.7f;
                    if (f * f * f - xVal * xVal * yVal * yVal * yVal <= 0.0f)
                    {
                        tex.SetPixel(x, y, red);
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

        private static void GenerateSuitDiamond(string path, int size = 32)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color red = ColorFromHex("#EF4444");
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f - size / 2f) / (size / 2f);
                    float ny = (y + 0.5f - size / 2f) / (size / 2f);
                    if (Mathf.Abs(nx) + Mathf.Abs(ny) <= 0.8f)
                    {
                        tex.SetPixel(x, y, red);
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

        private static void GenerateSuitSpade(string path, int size = 32)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color black = ColorFromHex("#2C3E50");
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f - size / 2f) / (size / 2f);
                    float ny = (y + 0.5f - size / 2f) / (size / 2f);
                    
                    float xVal = nx * 1.2f;
                    float yVal = -ny * 1.2f - 0.2f;
                    float f = xVal * xVal + yVal * yVal - 0.7f;
                    bool inHeart = (f * f * f - xVal * xVal * yVal * yVal * yVal <= 0.0f);
                    
                    bool inStem = (ny <= -0.4f && ny >= -0.8f && Mathf.Abs(nx) <= (-ny - 0.4f) * 0.6f);

                    if (inHeart || inStem)
                    {
                        tex.SetPixel(x, y, black);
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

        private static void GenerateSuitClub(string path, int size = 32)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color black = ColorFromHex("#2C3E50");
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f - size / 2f) / (size / 2f);
                    float ny = (y + 0.5f - size / 2f) / (size / 2f);
                    
                    float distTop = Mathf.Sqrt(nx * nx + (ny - 0.25f) * (ny - 0.25f));
                    float distLeft = Mathf.Sqrt((nx + 0.25f) * (nx + 0.25f) + (ny + 0.15f) * (ny + 0.15f));
                    float distRight = Mathf.Sqrt((nx - 0.25f) * (nx - 0.25f) + (ny + 0.15f) * (ny + 0.15f));
                    
                    bool inCircles = (distTop <= 0.28f || distLeft <= 0.28f || distRight <= 0.28f);
                    
                    bool inStem = (ny <= -0.1f && ny >= -0.8f && Mathf.Abs(nx) <= (-ny - 0.1f) * 0.4f);

                    if (inCircles || inStem)
                    {
                        tex.SetPixel(x, y, black);
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

        private static void GenerateCardBackground(string path, int width = 64, int height = 96, float cornerRadius = 6f)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float rSq = cornerRadius * cornerRadius;
            float cx = width / 2.0f;
            float cy = height / 2.0f;
            float rx = width / 2.0f - cornerRadius;
            float ry = height / 2.0f - cornerRadius;
            Color bgCol = Color.white;
            Color borderCol = ColorFromHex("#D1D5DB");

            for (int y = 0; y < height; y++)
            {
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
                        if (x < 1 || x >= width - 1 || y < 1 || y >= height - 1 || (dx * dx + dy * dy > (cornerRadius - 1) * (cornerRadius - 1)))
                        {
                            tex.SetPixel(x, y, borderCol);
                        }
                        else
                        {
                            tex.SetPixel(x, y, bgCol);
                        }
                    }
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GenerateBowl(string path, int width = 220, int height = 120)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color transparent = Color.clear;
            Color ceramicWhite = ColorFromHex("#F5F6F8");
            Color ceramicShadow = ColorFromHex("#D1D5DB");
            Color royalBlue = ColorFromHex("#1F3A60");
            Color gold = ColorFromHex("#D4AF37");

            float cx = width / 2f;
            float cy = 15f; // Bottom offset

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;

                    float rx = width / 2f - 6f;
                    float ry = height - 25f;

                    if (dy < 0)
                    {
                        if (dy >= -8f && Mathf.Abs(dx) <= rx)
                        {
                            tex.SetPixel(x, y, gold);
                        }
                        else
                        {
                            tex.SetPixel(x, y, transparent);
                        }
                    }
                    else
                    {
                        float val = (dx * dx) / (rx * rx) + (dy * dy) / (ry * ry);
                        if (val <= 1.0f)
                        {
                            if (y >= height - 15f && Mathf.Abs(dx) <= 15f)
                            {
                                tex.SetPixel(x, y, gold); // Handle knob
                            }
                            else if (val >= 0.94f)
                            {
                                tex.SetPixel(x, y, gold); // Rim/Edge border
                            }
                            else if (y >= 45f && y <= 55f)
                            {
                                tex.SetPixel(x, y, royalBlue); // Porcelain pattern band
                            }
                            else
                            {
                                float shadow = Mathf.Clamp01((dx + rx) / (2f * rx));
                                Color col = Color.Lerp(ceramicShadow, ceramicWhite, shadow);
                                tex.SetPixel(x, y, col);
                            }
                        }
                        else
                        {
                            tex.SetPixel(x, y, transparent);
                        }
                    }
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }

        private static void GenerateRouletteWheel(string path, int size = 256)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size / 2f;
            float outerRadius = size / 2f - 2f;
            float innerRadius = size * 0.15f;
            
            Color gold = ColorFromHex("#D4AF37");
            Color goldDark = ColorFromHex("#996515");
            Color red = ColorFromHex("#C0392B");
            Color black = ColorFromHex("#1C2833");
            Color green = ColorFromHex("#27AE60");
            Color white = Color.white;

            int[] wheelSequence = { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };
            int[] redNums = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x + 0.5f - center;
                    float dy = y + 0.5f - center;
                    float distSq = dx * dx + dy * dy;
                    float dist = Mathf.Sqrt(distSq);

                    if (dist > outerRadius)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (dist > outerRadius - 6f)
                    {
                        tex.SetPixel(x, y, gold);
                    }
                    else if (dist > outerRadius - 10f)
                    {
                        tex.SetPixel(x, y, goldDark);
                    }
                    else if (dist < innerRadius)
                    {
                        if (dist < 8f) tex.SetPixel(x, y, white);
                        else tex.SetPixel(x, y, gold);
                    }
                    else
                    {
                        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                        if (angle < 0) angle += 360f;

                        int slot = Mathf.FloorToInt(angle / (360f / 37f));
                        slot = Mathf.Clamp(slot, 0, 36);

                        int num = wheelSequence[slot];
                        if (num == 0)
                        {
                            tex.SetPixel(x, y, green);
                        }
                        else if (System.Array.IndexOf(redNums, num) >= 0)
                        {
                            tex.SetPixel(x, y, red);
                        }
                        else
                        {
                            tex.SetPixel(x, y, black);
                        }

                        float nextAngle = (slot + 1) * (360f / 37f);
                        float diff = Mathf.Abs(angle - nextAngle);
                        if (diff < 1.0f || angle < 0.5f)
                        {
                            if (dist > innerRadius + 10f)
                            {
                                tex.SetPixel(x, y, goldDark);
                            }
                        }
                    }
                }
            }
            tex.Apply();
            SaveUITexture(tex, path, false);
        }
    }
}
#endif
