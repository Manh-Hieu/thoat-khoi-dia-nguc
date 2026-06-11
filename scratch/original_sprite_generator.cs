Created At: 2026-06-11T09:20:48Z
Completed At: 2026-06-11T09:20:49Z
File Path: `file:///c:/code/thoat%20khoi%20dia%20nguc/Assets/Editor/SpriteGenerator.cs`
Total Lines: 967
Total Bytes: 35575
Showing lines 1 to 800
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
<truncated 29954 bytes>
    };
759:             GenerateSpriteFromGrid("Assets/Sprites/UI/Heart.png", heart, heartMap);
760: 
761:             // 1. NotesIcon
762:             var notesMap = new Dictionary<char, Color>
763:             {
764:                 { '.', Color.clear },
765:                 { 'Y', ColorFromHex("#F1C40F") }, // Yellow page
766:                 { 'T', ColorFromHex("#D35400") }, // Orange top
767:                 { 'K', ColorFromHex("#34495E") }  // Lines
768:             };
769:             string[] notes = {
770:                 "................",
771:                 "....TTTTTTTT....",
772:                 "...TYYYYYYYYT...",
773:                 "..TYYYYYYYYYYT..",
774:                 "..TYYYYYYYYYYT..",
775:                 "..TYYKKKKKKYYT..",
776:                 "..TYYYYYYYYYYT..",
777:                 "..TYYKKKKKKYYT..",
778:                 "..TYYYYYYYYYYT..",
779:                 "..TYYKKKKKKYYT..",
780:                 "..TYYYYYYYYYYT..",
781:                 "..TYYYYYYYYYYT..",
782:                 "...TYYYYYYYYT...",
783:                 "....TTTTTTTT....",
784:                 "................",
785:                 "................"
786:             };
787:             GenerateSpriteFromGrid("Assets/Sprites/UI/NotesIcon.png", notes, notesMap);
788: 
789:             // 2. PhotosIcon
790:             var photosMap = new Dictionary<char, Color>
791:             {
792:                 { '.', Color.clear },
793:                 { 'B', ColorFromHex("#3498DB") }, // Sky Blue
794:                 { 'G', ColorFromHex("#2ECC71") }, // Grass Green
795:                 { 'Y', ColorFromHex("#F1C40F") }, // Sun Yellow
796:                 { 'W', ColorFromHex("#ECF0F1") }  // White frame
797:             };
798:             string[] photos = {
799:                 "................",
800:                 "...WWWWWWWWWW...",
The above content does NOT show the entire file contents. If you need to view any lines of the file which were not shown to complete your task, call this tool again to view those lines.

