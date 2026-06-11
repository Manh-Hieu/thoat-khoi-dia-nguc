Created At: 2026-06-11T10:56:23Z
Completed At: 2026-06-11T10:56:23Z
File Path: `file:///c:/code/thoat%20khoi%20dia%20nguc/Assets/Editor/SpriteGenerator.cs`
Total Lines: 1530
Total Bytes: 57624
Showing lines 1 to 60
The following code has been modified to include a line number before every line, in the format: <line_number>: <original_line>. Please note that any changes targeting the original code should remove the line number, colon, and leading space.
1: #if UNITY_EDITOR
2: using UnityEngine;
3: using UnityEditor;
4: using System.IO;
5: using System.Collections.Generic;
6: 
7: namespace EscapeFromHell.Editor
8: {
9:     public class SpriteGenerator : EditorWindow
10:     {
11:         [MenuItem("Escape From Hell/Generate All Sprites")]
12:         public static void GenerateAll()
13:         {
14:             Debug.Log("Starting Sprite Generation...");
15: 
16:             // Make sure directories exist
17:             EnsureDirectories();
18: 
19:             // Generate characters
20:             GenerateMinhSprites();
21:             GenerateGuardSprites();
22: 
23:             // Generate tiles
24:             GenerateTileSprites();
25: 
26:             // Generate items
27:             GenerateItemSprites();
28: 
29:             // Generate UI
30:             GenerateUISprites();
31: 
32:             AssetDatabase.Refresh();
33:             Debug.Log("Sprite Generation Completed Successfully!");
34:         }
35: 
36:         private static void EnsureDirectories()
37:         {
38:             string[] paths = {
39:                 "Assets/Sprites/Characters/Minh",
40:                 "Assets/Sprites/Characters/Guards",
41:                 "Assets/Sprites/Tiles/Room",
42:                 "Assets/Sprites/Tiles/Compound",
43:                 "Assets/Sprites/Tiles/Forest",
44:                 "Assets/Sprites/Items",
45:                 "Assets/Sprites/UI"
46:             };
47: 
48:             foreach (string p in paths)
49:             {
50:                 if (!Directory.Exists(p))
51:                 {
52:                     Directory.CreateDirectory(p);
53:                 }
54:             }
55:         }
56: 
57:         private static void GenerateSpriteFromGrid(string path, string[] grid, Dictionary<char, Color> colorMap, int width = 16, int height = 16)
58:         {
59:             Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
60:             for (int y = 0; y < height; y++)
The above content does NOT show the entire file contents. If you need to view any lines of the file which were not shown to complete your task, call this tool again to view those lines.

