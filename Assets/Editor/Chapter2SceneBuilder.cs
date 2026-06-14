#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;
using EscapeFromHell.Core;
using EscapeFromHell.Player;
using EscapeFromHell.Systems;
using EscapeFromHell.Chapter;

namespace EscapeFromHell.Editor
{
    public class Chapter2SceneBuilder : EditorWindow
    {
        [MenuItem("Escape From Hell/Build Chapter 2 Scene")]
        public static void BuildChapter2()
        {
            Debug.Log("Starting Chapter 2 Scene Build...");
            AssetDatabase.Refresh();
            BuildChapter2Scene();
            AssetDatabase.Refresh();
            Debug.Log("Chapter 2 Scene Build Completed successfully!");
        }

        [MenuItem("Escape From Hell/Build Chapter 3 Scene")]
        public static void BuildChapter3()
        {
            Debug.Log("Starting Chapter 3 Scene Build...");
            AssetDatabase.Refresh();
            BuildChapter3PlaceholderScene();
            AssetDatabase.Refresh();
            Debug.Log("Chapter 3 Scene Build Completed successfully!");
        }

        [MenuItem("Escape From Hell/Build All Scenes")]
        public static void BuildAllScenes()
        {
            Debug.Log("=== STARTING BUILD ALL SCENES ===");
            
            Debug.Log("1/4: Building Main Menu...");
            MainMenuSceneBuilder.BuildMainMenuScene();
            
            Debug.Log("2/4: Building Chapter 1...");
            Chapter1SceneBuilder.BuildScene();
            
            Debug.Log("3/4: Building Chapter 2...");
            BuildChapter2Scene();
            
            Debug.Log("4/4: Building Chapter 3...");
            BuildChapter3PlaceholderScene();
            
            Debug.Log("=== BUILD ALL SCENES COMPLETED ===");
        }

        private static void BuildChapter2Scene()
        {
            Debug.Log("Building Chapter 2 Casino Scene...");

            // 1. Create a new Scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            newScene.name = "Chapter2_Cutscene";

            // 2. Configure Camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.position = new Vector3(0, 0, -10);
                mainCam.transform.rotation = Quaternion.identity;
                mainCam.orthographic = true;
                mainCam.orthographicSize = 4f;
                mainCam.backgroundColor = Color.black;
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                
                // Add CameraFollow
                CameraFollow camFollow = mainCam.gameObject.AddComponent<CameraFollow>();
            }

            // Global violet-neon light for sòng bạc atmosphere
            GameObject globalLightObj = new GameObject("GlobalLight2D");
            var globalLight = globalLightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            globalLight.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;
            globalLight.color = new Color(0.45f, 0.42f, 0.6f, 1f); // Neon violet/dark gray
            globalLight.intensity = 0.4f;

            Light dirLight = FindAnyObjectByType<Light>();
            if (dirLight != null)
            {
                dirLight.intensity = 0.4f;
                dirLight.color = new Color(0.45f, 0.42f, 0.6f, 1f);
            }

            // 3. Create Environment
            GameObject environment = new GameObject("Environment");

            // Background Image
            string bgPath = "Assets/Sprites/Backgrounds/Chapter2_Casino_Bg.png";
            AssetDatabase.ImportAsset(bgPath);
            TextureImporter bgImporter = AssetImporter.GetAtPath(bgPath) as TextureImporter;
            if (bgImporter != null)
            {
                bgImporter.textureType = TextureImporterType.Sprite;
                bgImporter.spriteImportMode = SpriteImportMode.Single;
                bgImporter.spritePixelsPerUnit = 64;
                bgImporter.filterMode = FilterMode.Point;
                bgImporter.textureCompression = TextureImporterCompression.Uncompressed;
                bgImporter.mipmapEnabled = false;
                EditorUtility.SetDirty(bgImporter);
                bgImporter.SaveAndReimport();
            }

            GameObject bgObj = new GameObject("Casino_Background");
            bgObj.transform.parent = environment.transform;
            bgObj.transform.position = Vector3.zero;
            SpriteRenderer bgSR = bgObj.AddComponent<SpriteRenderer>();
            bgSR.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);
            bgSR.sortingOrder = -20;

            float bgScale = 1.0f;
            if (bgSR.sprite != null)
            {
                float spriteWidth = bgSR.sprite.rect.width / bgSR.sprite.pixelsPerUnit;
                float targetWidth = 16f;
                bgScale = targetWidth / spriteWidth;
                bgObj.transform.localScale = new Vector3(bgScale, bgScale, 1);
            }
            else
            {
                Debug.LogError("Failed to load Casino Background Sprite at path: " + bgPath);
            }

            // Boundary colliders for walking area in sòng bạc (Wider room)
            CreateBoundary(environment.transform, "Boundary_Left", new Vector3(-8.44f, 0.11f, 0f), new Vector2(1f, 16f));
            CreateBoundary(environment.transform, "Boundary_Right", new Vector3(8.4f, -0.01f, 0f), new Vector2(1f, 16f));
            CreateBoundary(environment.transform, "Boundary_Bottom", new Vector3(0f, -8.58f, 0f), new Vector2(16f, 1f));
            CreateBoundary(environment.transform, "Boundary_Top", new Vector3(-1.77f, -1.94f, 0.13f), new Vector2(7.5f, 1f), Quaternion.Euler(49.742f, -86.269f, 96.859f));
            CreateBoundary(environment.transform, "Boundary_Top (1)", new Vector3(-4.8f, -1.47f, 0.41f), new Vector2(2f, 1f), Quaternion.Euler(46.66f, -89.363f, 90.299f));

            // Custom boundaries configured by user in editor
            CreateBoundary(environment.transform, "Boundary_Top (2)", new Vector3(-2.53f, -3.87f, -0.08f), new Vector2(4f, 1f), Quaternion.Euler(-35.161f, 93.175f, -87.697f));
            CreateBoundary(environment.transform, "Boundary_Top (3)", new Vector3(-5.82f, -0.57f, 0.89f), new Vector2(2f, 1f), Quaternion.Euler(-86.985f, 7.12f, -1.389f));
            CreateBoundary(environment.transform, "Boundary_Top (4)", new Vector3(-5.38f, -3.81f, 0.34f), new Vector2(5f, 1f), Quaternion.Euler(-86.985f, 7.12f, -1.389f));
            // Shift Boundary_Top (5) to the right to leave a gap at X=1.73f for the entrance door
            CreateBoundary(environment.transform, "Boundary_Top (5)", new Vector3(4.25f, -3.75f, -0.52f), new Vector2(2.5f, 1f), Quaternion.Euler(-86.985f, 7.12f, -1.389f));

            // 4. Create Player (Minh)
            GameObject playerObj = new GameObject("Player");
            playerObj.tag = "Player";
            playerObj.transform.position = new Vector3(1.73f, -3.5f, 0); // Start near the casino entrance door

            SpriteRenderer playerSR = playerObj.AddComponent<SpriteRenderer>();
            Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Idle_Down.png");
            playerSR.sprite = playerSprite;
            playerSR.sortingOrder = 5;

            Rigidbody2D playerRB = playerObj.AddComponent<Rigidbody2D>();
            playerRB.gravityScale = 0f;
            playerRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            playerRB.interpolation = RigidbodyInterpolation2D.Interpolate;
            playerRB.freezeRotation = true;

            BoxCollider2D playerCollider = playerObj.AddComponent<BoxCollider2D>();
            playerCollider.size = new Vector2(0.6f, 0.3f);
            playerCollider.offset = new Vector2(0f, -0.7f);

            playerObj.AddComponent<PlayerController>();
            playerObj.AddComponent<PlayerInteraction>();

            // Interaction Trigger child object
            GameObject interactionTriggerObj = new GameObject("InteractionTrigger");
            interactionTriggerObj.transform.parent = playerObj.transform;
            interactionTriggerObj.transform.localPosition = Vector3.zero;
            CircleCollider2D cc = interactionTriggerObj.AddComponent<CircleCollider2D>();
            cc.isTrigger = true;
            cc.radius = 1.0f;

            // Link player animator controller
            Animator animator = playerObj.GetComponent<Animator>();
            if (animator != null)
            {
                string controllerPath = "Assets/Animations/PlayerController.controller";
                var playerAnimController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
                if (playerAnimController != null)
                {
                    animator.runtimeAnimatorController = playerAnimController;
                }
            }

            // 5. Create Props Parent & Spawn 3 Casino Tables dynamically
            GameObject propsParent = new GameObject("Props");
            propsParent.transform.position = Vector3.zero;

            // Ensure custom sprites exist
            if (!Directory.Exists("Assets/Sprites/Casino"))
            {
                Directory.CreateDirectory("Assets/Sprites/Casino");
            }
            GenerateCasinoSprites();

            // Configure sprite import settings for premium high-res sprites
            ConfigureSpriteImportSettings("Assets/Sprites/Casino/Casino_Table.png", 100);
            ConfigureSpriteImportSettings("Assets/Sprites/Casino/NPC_Dealer.png", 100);
            ConfigureSpriteImportSettings("Assets/Sprites/Casino/NPC_Gambler_Back.png", 100);
            ConfigureSpriteImportSettings("Assets/Sprites/Casino/NPC_Gambler_Left.png", 100);
            ConfigureSpriteImportSettings("Assets/Sprites/Casino/NPC_Gambler_Right.png", 100);

            // Spawn the 3 gaming tables (Spaced out in wider room)
            SpawnCasinoTable(propsParent.transform, "PokerTable", new Vector3(-4.05f, -6.24f, 0f), Chapter2PropType.PokerTable, "Chơi Blackjack", true, false);
            SpawnCasinoTable(propsParent.transform, "TaiXiuTableProp", new Vector3(0.47f, -6.71f, 0f), Chapter2PropType.TaiXiuTable, "Chơi Tài Xỉu", false, true);
            SpawnCasinoTable(propsParent.transform, "RouletteTable", new Vector3(4.3f, -6.38f, 0f), Chapter2PropType.RouletteTable, "Chơi Roulette", true, false);

            // Left Exit Door (placed exactly over the casino entrance door shown in the background illustration)
            SpawnExitDoor(propsParent.transform, "LeftExitDoor", new Vector3(1.73f, -2.3f, 0f), 2.0f);

            // Left Door Guard 1 (stands to the left of the door, matching the left column background elements)
            SpawnGuard(propsParent.transform, "LeftDoorGuard1", new Vector3(-0.9f, -2.4f, 0f), Chapter2PropType.LeftDoorGuard1, "Hỏi thăm bảo vệ");

            // Left Door Guard 2 (stands to the right of the door, matching the right column background elements)
            SpawnGuard(propsParent.transform, "LeftDoorGuard2", new Vector3(4.4f, -2.4f, 0f), Chapter2PropType.LeftDoorGuard2, "Hỏi thăm bảo vệ");

            // Recruiter NPC (standing on the far right side)
            SpawnGuard(propsParent.transform, "RecruiterNPC", new Vector3(7.0f, -2.4f, 0f), Chapter2PropType.Recruiter, "Nói chuyện");

            // VIP Stairs Guard (guarding stairs to VIP floor)
            // Use wider collider width (1.5f) to prevent bypassing the stairs
            SpawnGuard(propsParent.transform, "VipStairsGuardNPC", new Vector3(-7.05f, 1.13f, 0f), Chapter2PropType.VipStairsGuard, "Hỏi thăm bảo vệ", 1.5f);

            // 6. Create UI Canvas
            GameObject uiRoot = new GameObject("UI_Canvas");
            Canvas canvas = uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = uiRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            uiRoot.AddComponent<GraphicRaycaster>();

            // EventSystem
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Dialogue Panel Setup (Same layout as Chapter 1)
            GameObject dialPanel = CreatePanel(uiRoot, "DialoguePanel", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 20), new Vector2(-40, 150), new Color(0f, 0f, 0f, 0.88f));
            Outline panelOutline = dialPanel.AddComponent<Outline>();
            panelOutline.effectColor = new Color(0.8f, 0.1f, 0.1f, 0.6f);
            panelOutline.effectDistance = new Vector2(2, 2);

            // Portrait
            GameObject portraitObj = CreatePanel(dialPanel, "Portrait", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(20, 0), new Vector2(100, 100), Color.clear);
            Image portImg = portraitObj.GetComponent<Image>();

            // Speaker Name
            GameObject nameObj = CreateText(dialPanel, "SpeakerName", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(140, -15), new Vector2(-160, 30), "Minh", 20, new Color(0.9f, 0.6f, 0.1f), TextAlignmentOptions.Left);
            TextMeshProUGUI nameText = nameObj.GetComponent<TextMeshProUGUI>();
            nameText.fontStyle = FontStyles.Bold;

            // Dialogue Text
            GameObject bodyObj = CreateText(dialPanel, "DialogueText", new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(140, -45), new Vector2(-160, -60), "", 16, Color.white, TextAlignmentOptions.Left);
            TextMeshProUGUI bodyText = bodyObj.GetComponent<TextMeshProUGUI>();

            // Continue Indicator
            GameObject indicatorObj = CreateText(dialPanel, "ContinueIndicator", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-15, 10), new Vector2(120, 20), "[Nhấn Space/Chuột]", 11, new Color(0.6f, 0.6f, 0.6f), TextAlignmentOptions.Right);

            // Choice Panel
            GameObject choicesPanel = CreatePanel(uiRoot, "ChoicesPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(300, 200), new Color(0f, 0f, 0f, 0.9f));
            choicesPanel.AddComponent<VerticalLayoutGroup>();

            // Instantiate DialogueSystem Singleton
            GameObject sysObj = new GameObject("DialogueSystem");
            DialogueSystem ds = sysObj.AddComponent<DialogueSystem>();
            SetPrivateField(ds, "dialoguePanel", dialPanel);
            SetPrivateField(ds, "speakerNameText", nameText);
            SetPrivateField(ds, "dialogueText", bodyText);
            SetPrivateField(ds, "portraitImage", portImg);
            SetPrivateField(ds, "continueIndicator", indicatorObj);
            SetPrivateField(ds, "choicesPanel", choicesPanel);
            SetPrivateField(ds, "choicesContainer", choicesPanel.transform);

            // 7. Create Tài Xỉu Mini-game UI
            GameObject taiXiuPanel = CreatePanel(uiRoot, "TaiXiuPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 500), new Color(0f, 0f, 0f, 0.94f));
            Outline txOutline = taiXiuPanel.AddComponent<Outline>();
            txOutline.effectColor = new Color(0.9f, 0.7f, 0.1f, 0.7f); // Beautiful Gold border outline
            txOutline.effectDistance = new Vector2(2, 2);

            // Title
            CreateText(taiXiuPanel, "TitleText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(500, 40), "SÒNG BẠC HOÀNG GIA - TÀI XỈU", 24, new Color(0.9f, 0.7f, 0.1f));

            // Cash Display Text
            GameObject cashTxtObj = CreateText(taiXiuPanel, "CashText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -65), new Vector2(500, 30), "Tiền mặt: 500.000đ", 18, Color.white);
            TextMeshProUGUI cashTextComponent = cashTxtObj.GetComponent<TextMeshProUGUI>();

            // Bet Choice Label
            CreateText(taiXiuPanel, "BetChoiceLabel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -110), new Vector2(500, 25), "LỰA CHỌN CỬA ĐẶT", 13, new Color(0.7f, 0.7f, 0.7f));

            // Bet Type Buttons (Tai / Xiu)
            Button taiBtn = CreateButton(taiXiuPanel, "TaiButton", new Vector2(-130, -150), new Vector2(220, 45), "TÀI (11-18)", 16);
            Button xiuBtn = CreateButton(taiXiuPanel, "XiuButton", new Vector2(130, -150), new Vector2(220, 45), "XỈU (3-10)", 16);

            // Bet Amount Label
            CreateText(taiXiuPanel, "BetAmountLabel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -195), new Vector2(500, 25), "MỨC TIỀN CƯỢC", 13, new Color(0.7f, 0.7f, 0.7f));

            // Bet Amount Selection Buttons
            CreateButton(taiXiuPanel, "Bet100kButton", new Vector2(-180, -230), new Vector2(100, 35), "100.000đ", 13);
            CreateButton(taiXiuPanel, "Bet200kButton", new Vector2(-60, -230), new Vector2(100, 35), "200.000đ", 13);
            CreateButton(taiXiuPanel, "Bet500kButton", new Vector2(60, -230), new Vector2(100, 35), "500.000đ", 13);
            CreateButton(taiXiuPanel, "BetAllInButton", new Vector2(180, -230), new Vector2(100, 35), "Tất Tay", 13);

            // Status Panel Labels
            CreateText(taiXiuPanel, "StatusLabel1", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-190, -35), new Vector2(100, 25), "Đặt cửa:", 14, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            GameObject betTypeTxtObj = CreateText(taiXiuPanel, "BetTypeText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-120, -35), new Vector2(120, 25), "Chưa chọn", 14, new Color(0.9f, 0.7f, 0.1f), TextAlignmentOptions.Left);
            TextMeshProUGUI betTypeComponent = betTypeTxtObj.GetComponent<TextMeshProUGUI>();

            CreateText(taiXiuPanel, "StatusLabel2", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(40, -35), new Vector2(100, 25), "Số tiền:", 14, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            GameObject betAmountTxtObj = CreateText(taiXiuPanel, "BetAmountText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(110, -35), new Vector2(120, 25), "Chưa đặt", 14, new Color(0.9f, 0.7f, 0.1f), TextAlignmentOptions.Left);
            TextMeshProUGUI betAmountComponent = betAmountTxtObj.GetComponent<TextMeshProUGUI>();

            // Dice Result Label & Boxes
            CreateText(taiXiuPanel, "DiceResultLabel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -80), new Vector2(500, 25), "KẾT QUẢ XÚC XẮC", 13, new Color(0.7f, 0.7f, 0.7f));

            TextMeshProUGUI dice1TextComponent = CreateDiceBox(taiXiuPanel, "Dice1Text", new Vector2(-60, -125));
            TextMeshProUGUI dice2TextComponent = CreateDiceBox(taiXiuPanel, "Dice2Text", new Vector2(0, -125));
            TextMeshProUGUI dice3TextComponent = CreateDiceBox(taiXiuPanel, "Dice3Text", new Vector2(60, -125));

            // Result Message Area
            GameObject resultTxtObj = CreateText(taiXiuPanel, "ResultText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -170), new Vector2(520, 45), "Chọn Tài/Xỉu & mức cược rồi bấm Xốc!", 15, new Color(0.9f, 0.7f, 0.1f));
            TextMeshProUGUI resultComponent = resultTxtObj.GetComponent<TextMeshProUGUI>();

            // Action Buttons
            Button rollBtn = CreateButton(taiXiuPanel, "RollButton", new Vector2(-110, -220), new Vector2(200, 42), "XỐC XÚC XẮC!", 15);
            // Highlight roll button outline with bright gold
            Outline rollOutline = rollBtn.GetComponent<Outline>();
            if (rollOutline != null) rollOutline.effectColor = new Color(0.9f, 0.7f, 0.1f, 1f);

            Button closeBtn = CreateButton(taiXiuPanel, "CloseButton", new Vector2(110, -220), new Vector2(160, 42), "ĐÓNG BÀN", 14);

            // 7b. Create Blackjack UI Panel
            GameObject blackjackPanel = CreatePanel(uiRoot, "BlackjackPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 500), new Color(0f, 0f, 0f, 0.94f));
            Outline bjOutline = blackjackPanel.AddComponent<Outline>();
            bjOutline.effectColor = new Color(0.9f, 0.7f, 0.1f, 0.7f);
            bjOutline.effectDistance = new Vector2(2, 2);

            // Title
            CreateText(blackjackPanel, "TitleText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(500, 40), "SÒNG BẠC HOÀNG GIA - BLACKJACK (XÌ DÁCH)", 22, new Color(0.9f, 0.7f, 0.1f));

            // Cash Display Text
            CreateText(blackjackPanel, "CashText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -60), new Vector2(500, 30), "Tiền mặt: 500.000đ", 18, Color.white);

            // Cards display area
            CreateText(blackjackPanel, "DealerCardsLabel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-150, 110), new Vector2(150, 25), "Bài nhà cái:", 14, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            CreateText(blackjackPanel, "DealerCardsText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20, 110), new Vector2(300, 30), "-", 16, Color.white, TextAlignmentOptions.Left);

            CreateText(blackjackPanel, "PlayerCardsLabel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-150, 60), new Vector2(150, 25), "Bài của bạn:", 14, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            CreateText(blackjackPanel, "PlayerCardsText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20, 60), new Vector2(300, 30), "-", 16, Color.white, TextAlignmentOptions.Left);

            // Bet Selection Label
            CreateText(blackjackPanel, "BetLabel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 10), new Vector2(500, 25), "MỨC TIỀN CƯỢC", 13, new Color(0.7f, 0.7f, 0.7f));

            // Bet Buttons
            CreateButton(blackjackPanel, "Bet100kButton", new Vector2(-180, -30), new Vector2(100, 35), "100.000đ", 13);
            CreateButton(blackjackPanel, "Bet200kButton", new Vector2(-60, -30), new Vector2(100, 35), "200.000đ", 13);
            CreateButton(blackjackPanel, "Bet500kButton", new Vector2(60, -30), new Vector2(100, 35), "500.000đ", 13);
            CreateButton(blackjackPanel, "BetAllInButton", new Vector2(180, -30), new Vector2(100, 35), "Tất Tay", 13);

            // Selected Bet Status
            CreateText(blackjackPanel, "StatusLabel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-80, -75), new Vector2(120, 25), "Tiền đặt cược:", 14, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            CreateText(blackjackPanel, "BetAmountText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20, -75), new Vector2(150, 25), "Chưa đặt", 14, new Color(0.9f, 0.7f, 0.1f), TextAlignmentOptions.Left);

            // Result Text
            CreateText(blackjackPanel, "ResultText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -125), new Vector2(500, 35), "Đặt tiền cược rồi bấm Chia Bài!", 15, new Color(0.9f, 0.7f, 0.1f));

            // Action Buttons
            Button hitBtn = CreateButton(blackjackPanel, "HitButton", new Vector2(-180, -200), new Vector2(120, 42), "RÚT BÀI", 14);
            Button standBtn = CreateButton(blackjackPanel, "StandButton", new Vector2(-40, -200), new Vector2(120, 42), "DẰN BÀI", 14);
            Button dealBtn = CreateButton(blackjackPanel, "DealButton", new Vector2(110, -200), new Vector2(140, 42), "CHIA BÀI", 14);
            Outline dealOutline = dealBtn.GetComponent<Outline>();
            if (dealOutline != null) dealOutline.effectColor = new Color(0.9f, 0.7f, 0.1f, 1f);

            // Close button at top-right
            CreateButton(blackjackPanel, "CloseButton", new Vector2(245, 215), new Vector2(70, 30), "ĐÓNG", 12);


            // 7c. Create Roulette UI Panel
            GameObject roulettePanel = CreatePanel(uiRoot, "RoulettePanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 500), new Color(0f, 0f, 0f, 0.94f));
            Outline rlOutline = roulettePanel.AddComponent<Outline>();
            rlOutline.effectColor = new Color(0.9f, 0.7f, 0.1f, 0.7f);
            rlOutline.effectDistance = new Vector2(2, 2);

            // Title
            CreateText(roulettePanel, "TitleText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(500, 40), "SÒNG BẠC HOÀNG GIA - VÒNG QUAY ROULETTE", 20, new Color(0.9f, 0.7f, 0.1f));

            // Cash Display Text
            CreateText(roulettePanel, "CashText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -60), new Vector2(500, 30), "Tiền mặt: 500.000đ", 18, Color.white);

            // Bet Choice Label
            CreateText(roulettePanel, "BetChoiceLabel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 115), new Vector2(500, 25), "LỰA CHỌN CỬA ĐẶT", 13, new Color(0.7f, 0.7f, 0.7f));

            // Bet Type Buttons (Red / Black / Even / Odd)
            Button rBtn = CreateButton(roulettePanel, "RedButton", new Vector2(-180, 75), new Vector2(100, 35), "ĐỎ", 14);
            Outline rOutline = rBtn.GetComponent<Outline>();
            if (rOutline != null) rOutline.effectColor = Color.red;

            Button bBtn = CreateButton(roulettePanel, "BlackButton", new Vector2(-60, 75), new Vector2(100, 35), "ĐEN", 14);
            Outline bOutline = bBtn.GetComponent<Outline>();
            if (bOutline != null) bOutline.effectColor = Color.black;

            CreateButton(roulettePanel, "EvenButton", new Vector2(60, 75), new Vector2(100, 35), "CHẴN", 14);
            CreateButton(roulettePanel, "OddButton", new Vector2(180, 75), new Vector2(100, 35), "LẺ", 14);

            // Bet Amount Label
            CreateText(roulettePanel, "BetAmountLabel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(500, 25), "MỨC TIỀN CƯỢC", 13, new Color(0.7f, 0.7f, 0.7f));

            // Bet Amount Selection Buttons
            CreateButton(roulettePanel, "Bet100kButton", new Vector2(-180, -20), new Vector2(100, 35), "100.000đ", 13);
            CreateButton(roulettePanel, "Bet200kButton", new Vector2(-60, -20), new Vector2(100, 35), "200.000đ", 13);
            CreateButton(roulettePanel, "Bet500kButton", new Vector2(60, -20), new Vector2(100, 35), "500.000đ", 13);
            CreateButton(roulettePanel, "BetAllInButton", new Vector2(180, -20), new Vector2(100, 35), "Tất Tay", 13);

            // Status Panel Labels
            CreateText(roulettePanel, "StatusLabel1", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-190, -65), new Vector2(100, 25), "Đặt cửa:", 14, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            CreateText(roulettePanel, "BetTypeText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-120, -65), new Vector2(120, 25), "Chưa chọn", 14, new Color(0.9f, 0.7f, 0.1f), TextAlignmentOptions.Left);

            CreateText(roulettePanel, "StatusLabel2", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(40, -65), new Vector2(100, 25), "Số tiền:", 14, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            CreateText(roulettePanel, "BetAmountText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(110, -65), new Vector2(120, 25), "Chưa đặt", 14, new Color(0.9f, 0.7f, 0.1f), TextAlignmentOptions.Left);

            // Result Message Area
            CreateText(roulettePanel, "ResultText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -115), new Vector2(520, 35), "Chọn cửa đặt & tiền rồi bấm Quay!", 15, new Color(0.9f, 0.7f, 0.1f));

            // Spin Button
            Button spinBtn = CreateButton(roulettePanel, "SpinButton", new Vector2(-100, -200), new Vector2(220, 42), "QUAY VÒNG QUAY", 15);
            Outline spinOutline = spinBtn.GetComponent<Outline>();
            if (spinOutline != null) spinOutline.effectColor = new Color(0.9f, 0.7f, 0.1f, 1f);

            // Close button at bottom-right
            CreateButton(roulettePanel, "CloseButton", new Vector2(120, -200), new Vector2(140, 42), "ĐÓNG BÀN", 14);


            // 8. Create Chapter2Controller
            GameObject controllerObj = new GameObject("Chapter2Controller");
            Chapter2Controller c2 = controllerObj.AddComponent<Chapter2Controller>();

            // 9. GameManager setup in scene
            GameObject gmObj = new GameObject("GameManager");
            GameManager gm = gmObj.AddComponent<GameManager>();
            gm.ChangeState(GameState.Playing);

            // 10. Save the scene
            if (!Directory.Exists("Assets/Scenes"))
            {
                Directory.CreateDirectory("Assets/Scenes");
            }
            EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Chapter2_Cutscene.unity");
            Debug.Log("Chapter 2 Scene saved to Assets/Scenes/Chapter2_Cutscene.unity!");

            // Register in Build Settings
            AddSceneToBuildSettings("Assets/Scenes/Chapter2_Cutscene.unity");
        }

        private static void BuildChapter3PlaceholderScene()
        {
            Debug.Log("Building Chapter 3 Placeholder Scene...");

            // 1. Create a new Scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            newScene.name = "Chapter3_Compound";

            // 2. Configure Camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.position = new Vector3(0, 0, -10);
                mainCam.backgroundColor = new Color(0.04f, 0.04f, 0.06f, 1f); // Dark black/blue
                mainCam.clearFlags = CameraClearFlags.SolidColor;
            }

            // 3. Create Canvas
            GameObject uiRoot = new GameObject("Canvas");
            Canvas canvas = uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = uiRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            uiRoot.AddComponent<GraphicRaycaster>();

            // Panel Background
            CreatePanel(uiRoot, "Background", new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.05f, 0.05f, 0.08f, 1f));

            // Outline Box Panel
            GameObject boxPanel = CreatePanel(uiRoot, "BoxPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700, 400), new Color(0.08f, 0.08f, 0.12f, 1f));
            Outline boxOutline = boxPanel.AddComponent<Outline>();
            boxOutline.effectColor = new Color(0.8f, 0.1f, 0.1f, 0.7f); // Dark Red outline
            boxOutline.effectDistance = new Vector2(2, 2);

            // Title Text
            CreateText(boxPanel, "TitleText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -40), new Vector2(600, 50), "CHƯƠNG 3: CAMPUCHIA (DEMO)", 28, new Color(0.8f, 0.1f, 0.1f));

            // Narrative monologues / description
            string storyDesc = "Minh đã chấp nhận lời mời của tên cò mồi lạ mặt...\n\n" +
                               "Anh lên xe ngay trong đêm để vượt biên sang Campuchia, tin tưởng vào mức lương 40 triệu/tháng để gửi tiền về phẫu thuật chân cho Bố.\n\n" +
                               "Nhưng đằng sau những lời chào mời ngon ngọt ấy là một cạm bẫy địa ngục kinh hoàng đang chờ đón anh...\n\n\n" +
                               "<color=#aaaaaa><size=13>Bản chơi thử (Demo) Chương 2 kết thúc tại đây.\nBấm phím ESC để quay lại Màn hình chính.</size></color>";
            CreateText(boxPanel, "StoryText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -10), new Vector2(620, 260), storyDesc, 15, Color.white);

            // 4. Attach Placeholder script
            GameObject holderObj = new GameObject("Chapter3Placeholder");
            holderObj.AddComponent<Chapter3Placeholder>();

            // 5. Save the scene
            EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Chapter3_Compound.unity");
            Debug.Log("Chapter 3 Scene saved to Assets/Scenes/Chapter3_Compound.unity!");

            // Register in Build Settings
            AddSceneToBuildSettings("Assets/Scenes/Chapter3_Compound.unity");
        }

        private static void CreateBoundary(Transform parent, string name, Vector3 pos, Vector2 size)
        {
            CreateBoundary(parent, name, pos, size, Quaternion.identity);
        }

        private static void CreateBoundary(Transform parent, string name, Vector3 pos, Vector2 size, Quaternion rotation)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = pos;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = Vector3.one;
            BoxCollider2D bc = obj.AddComponent<BoxCollider2D>();
            bc.size = size;
        }

        private static GameObject CreatePanel(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent.transform, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            
            Image img = obj.AddComponent<Image>();
            img.color = color;
            return obj;
        }

        private static GameObject CreateText(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, string text, int fontSize, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent.transform, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
            
            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            return obj;
        }

        private static Button CreateButton(GameObject parent, string name, Vector2 pos, Vector2 size, string labelText, int fontSize)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent.transform, false);
            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.12f, 0.12f, 0.16f, 1f); // Sleek dark body
            
            // Set sliced border using RoundedRect sprite if available
            Sprite roundedRect = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRect.png");
            if (roundedRect != null)
            {
                img.sprite = roundedRect;
                img.type = Image.Type.Sliced;
            }

            Outline outline = btnObj.AddComponent<Outline>();
            outline.effectColor = new Color(0.9f, 0.7f, 0.1f, 0.4f); // Subtle Gold border outline
            outline.effectDistance = new Vector2(1, 1);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;

            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.9f, 0.7f, 0.1f, 1.0f); // Bright Gold on hover
            cb.pressedColor = new Color(0.7f, 0.5f, 0.08f, 1.0f);
            cb.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            cb.selectedColor = Color.white;
            btn.colors = cb;

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            // Text Label
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            RectTransform txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmText = txtObj.AddComponent<TextMeshProUGUI>();
            tmText.text = labelText;
            tmText.fontSize = fontSize;
            tmText.alignment = TextAlignmentOptions.Center;
            tmText.color = Color.white;
            tmText.raycastTarget = false; // CRITICAL: Disable raycast to avoid blocking clicks!

            return btn;
        }

        private static TextMeshProUGUI CreateDiceBox(GameObject parent, string name, Vector2 pos)
        {
            GameObject boxObj = new GameObject(name);
            boxObj.transform.SetParent(parent.transform, false);
            RectTransform rect = boxObj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(45, 45);

            Image img = boxObj.AddComponent<Image>();
            img.color = new Color(0.18f, 0.18f, 0.22f, 1f); // Dark dice box face
            
            Outline outline = boxObj.AddComponent<Outline>();
            outline.effectColor = new Color(0.9f, 0.7f, 0.1f, 0.4f);
            outline.effectDistance = new Vector2(1, 1);

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(boxObj.transform, false);
            RectTransform txtRect = textObj.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = "-";
            tmpText.fontSize = 24;
            tmpText.fontStyle = FontStyles.Bold;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.white;
            tmpText.raycastTarget = false;

            return tmpText;
        }

        private static void SpawnCasinoTable(Transform parent, string tableName, Vector3 tablePos, Chapter2PropType propType, string interactionPrompt, bool hasGamblersSide = false, bool hasGamblersBack = false)
        {
            GameObject tableGroup = new GameObject(tableName);
            tableGroup.transform.parent = parent;
            tableGroup.transform.position = tablePos;

            Sprite tableSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Casino/Casino_Table.png");
            Sprite dealerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Casino/NPC_Dealer.png");
            Sprite gBack = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Casino/NPC_Gambler_Back.png");
            Sprite gLeft = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Casino/NPC_Gambler_Left.png");
            Sprite gRight = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Casino/NPC_Gambler_Right.png");

            // 1. Table Visual (Scaled down to match proportions)
            GameObject tableVisual = new GameObject("TableVisual");
            tableVisual.transform.parent = tableGroup.transform;
            tableVisual.transform.localPosition = Vector3.zero;
            tableVisual.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
            SpriteRenderer tableSR = tableVisual.AddComponent<SpriteRenderer>();
            tableSR.sprite = tableSprite;
            tableSR.sortingOrder = 3;

            // Physical boundary collider (at group level for 1:1 scale)
            BoxCollider2D bc = tableGroup.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(2.5f, 0.8f);
            bc.offset = new Vector2(0f, -0.4f);

            // Interaction Trigger (at group level)
            BoxCollider2D trigger = tableGroup.AddComponent<BoxCollider2D>();
            trigger.size = new Vector2(3.2f, 1.6f);
            trigger.offset = new Vector2(0f, -0.3f);
            trigger.isTrigger = true;
            Chapter2Prop prop = tableGroup.AddComponent<Chapter2Prop>();
            prop.Initialize(propType, interactionPrompt);

            // 2. Dealer NPC (Behind the table)
            GameObject dealer = new GameObject("Dealer");
            dealer.transform.parent = tableGroup.transform;
            dealer.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            dealer.transform.localScale = new Vector3(0.14f, 0.14f, 1f);
            SpriteRenderer dealerSR = dealer.AddComponent<SpriteRenderer>();
            dealerSR.sprite = dealerSprite;
            dealerSR.sortingOrder = 2; // Render behind table felt

            // 3. Sitting Gamblers (contain stools already)
            if (hasGamblersBack)
            {
                GameObject gamblerL = new GameObject("Gambler_Left");
                gamblerL.transform.parent = tableGroup.transform;
                gamblerL.transform.localPosition = new Vector3(-0.6f, -0.3f, 0f);
                gamblerL.transform.localScale = new Vector3(0.13f, 0.13f, 1f);
                gamblerL.AddComponent<SpriteRenderer>().sprite = gBack;
                gamblerL.GetComponent<SpriteRenderer>().sortingOrder = 4; // Render in front of table

                GameObject gamblerR = new GameObject("Gambler_Right");
                gamblerR.transform.parent = tableGroup.transform;
                gamblerR.transform.localPosition = new Vector3(0.6f, -0.3f, 0f);
                gamblerR.transform.localScale = new Vector3(0.13f, 0.13f, 1f);
                gamblerR.AddComponent<SpriteRenderer>().sprite = gBack;
                gamblerR.GetComponent<SpriteRenderer>().sortingOrder = 4;
            }

            if (hasGamblersSide)
            {
                GameObject gamblerL = new GameObject("Gambler_Left");
                gamblerL.transform.parent = tableGroup.transform;
                gamblerL.transform.localPosition = new Vector3(-0.9f, -0.1f, 0f);
                gamblerL.transform.localScale = new Vector3(0.13f, 0.13f, 1f);
                gamblerL.AddComponent<SpriteRenderer>().sprite = gRight; // Face right towards table
                gamblerL.GetComponent<SpriteRenderer>().sortingOrder = 4;

                GameObject gamblerR = new GameObject("Gambler_Right");
                gamblerR.transform.parent = tableGroup.transform;
                gamblerR.transform.localPosition = new Vector3(0.9f, -0.1f, 0f);
                gamblerR.transform.localScale = new Vector3(0.13f, 0.13f, 1f);
                gamblerR.AddComponent<SpriteRenderer>().sprite = gLeft; // Face left towards table
                gamblerR.GetComponent<SpriteRenderer>().sortingOrder = 4;
            }
        }

        private static void SpawnGuard(Transform parent, string guardName, Vector3 guardPos, Chapter2PropType propType, string interactionPrompt, float colliderWidth = 0.6f)
        {
            GameObject guardObj = new GameObject(guardName);
            guardObj.transform.parent = parent;
            guardObj.transform.position = guardPos;

            SpriteRenderer guardSR = guardObj.AddComponent<SpriteRenderer>();
            guardSR.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Guards/Guard_Idle_Down.png");
            guardSR.sortingOrder = 5;

            BoxCollider2D guardCollider = guardObj.AddComponent<BoxCollider2D>();
            guardCollider.size = new Vector2(colliderWidth, 0.3f);
            guardCollider.offset = new Vector2(0f, -0.7f); // Physical body blocker

            BoxCollider2D guardTrigger = guardObj.AddComponent<BoxCollider2D>();
            guardTrigger.size = new Vector2(1.8f, 1.8f);
            guardTrigger.isTrigger = true; // Proximity trigger

            Chapter2Prop guardProp = guardObj.AddComponent<Chapter2Prop>();
            guardProp.Initialize(propType, interactionPrompt);
        }

        private static void SpawnExitDoor(Transform parent, string doorName, Vector3 doorPos, float scale = 2.0f)
        {
            GameObject doorObj = new GameObject(doorName);
            doorObj.transform.parent = parent;
            doorObj.transform.position = doorPos;
            doorObj.transform.localScale = new Vector3(scale, scale, 1f);

            // Do not add a visible SpriteRenderer, because the beautiful casino doors are already drawn on the background!
            // This leaves the door visual as drawn by the background but provides the required interactive zone.

            BoxCollider2D doorTrigger = doorObj.AddComponent<BoxCollider2D>();
            doorTrigger.size = new Vector2(2.5f, 1.5f); // Larger collider box matching the double doors width
            doorTrigger.isTrigger = true;

            Chapter2Prop doorProp = doorObj.AddComponent<Chapter2Prop>();
            doorProp.Initialize(Chapter2PropType.LeftExitDoor, "Đi về");
        }

        private static void GenerateCasinoSprites()
        {
            // Using premium generated casino sprites processed via Python PIL
            Debug.Log("Using premium generated casino sprites.");
        }

        private static void GenerateSpriteFromGrid(string path, string[] grid, Dictionary<char, Color> colorMap, int width, int height, int ppu)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            for (int y = 0; y < height; y++)
            {
                int gridY = height - 1 - y;
                string row = grid[gridY];
                for (int x = 0; x < width; x++)
                {
                    char c = x < row.Length ? row[x] : '.';
                    Color col = colorMap.ContainsKey(c) ? colorMap[c] : Color.clear;
                    tex.SetPixel(x, y, col);
                }
            }

            tex.Apply();
            SaveTexture(tex, path, ppu);
            DestroyImmediate(tex);
        }

        private static void SaveTexture(Texture2D tex, string path, int ppu)
        {
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

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

        private static Color ColorFromHex(string hex)
        {
            Color col;
            if (ColorUtility.TryParseHtmlString(hex, out col))
            {
                return col;
            }
            return Color.white;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                Debug.LogWarning($"Field {fieldName} not found on {target.GetType().Name}");
            }
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            
            bool exists = false;
            foreach (var s in scenes)
            {
                if (s.path == scenePath)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
                Debug.Log($"Added scene {scenePath} to Build Settings.");
            }
        }

        private static void ConfigureSpriteImportSettings(string path, int ppu)
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
    }
}
#endif
