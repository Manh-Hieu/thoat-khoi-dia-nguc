Created At: 2026-06-11T08:47:52Z
Completed At: 2026-06-11T08:47:52Z
File Path: `file:///c:/code/thoat%20khoi%20dia%20nguc/Assets/Editor/SpriteGenerator.cs`
Total Lines: 741
Total Bytes: 27053
Showing lines 1 to 741
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
52
<truncated 27094 bytes>
                 ".RRRRRRRRRR.....",
691:                 "..RRRRRRRR......",
692:                 "...RRRRRR.......",
693:                 "....RRRR........",
694:                 ".....RR.........",
695:                 "................",
696:                 "................",
697:                 "................",
698:                 "................",
699:                 "................"
700:             };
701:             GenerateSpriteFromGrid("Assets/Sprites/UI/Heart.png", heart, heartMap);
702:         }
703: 
704:         private static void SaveTexture(Texture2D tex, string path)
705:         {
706:             byte[] bytes = tex.EncodeToPNG();
707:             File.WriteAllBytes(path, bytes);
708: 
709:             // Force pixel perfect import settings
710:             AssetDatabase.ImportAsset(path);
711:             TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
712:             if (importer != null)
713:             {
714:                 importer.textureType = TextureImporterType.Sprite;
715:                 importer.spriteImportMode = SpriteImportMode.Single;
716:                 importer.spritePixelsPerUnit = 16;
717:                 importer.filterMode = FilterMode.Point;
718:                 importer.textureCompression = TextureImporterCompression.Uncompressed;
719:                 
720:                 // Disable mipmaps
721:                 importer.mipmapEnabled = false;
722: 
723:                 // Save
724:                 EditorUtility.SetDirty(importer);
725:                 importer.SaveAndReimport();
726:             }
727:         }
728: 
729:         private static Color ColorFromHex(string hex)
730:         {
731:             Color col;
732:             if (ColorUtility.TryParseHtmlString(hex, out col))
733:             {
734:                 return col;
735:             }
736:             return Color.white;
737:         }
738:     }
739: }
740: #endif
741: 
The above content shows the entire, complete file contents of the requested file.

