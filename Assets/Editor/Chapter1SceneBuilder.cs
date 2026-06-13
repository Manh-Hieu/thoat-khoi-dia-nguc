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
    public class Chapter1SceneBuilder : EditorWindow
    {
        private static void PrepareSprites()
        {
            string[] paths = new string[] {
                "Assets/Sprites/UI/NotificationDot.png",
                "Assets/Sprites/UI/AppIconBg_Grey.png",
                "Assets/Sprites/UI/NotesIcon.png",
                "Assets/Sprites/UI/PhotosIcon.png",
                "Assets/Sprites/UI/WeatherIcon.png",
                "Assets/Sprites/UI/SettingsIcon.png",
                "Assets/Sprites/UI/MessagesIcon.png",
                "Assets/Sprites/UI/BrowserIcon.png",
                "Assets/Sprites/UI/TikTokIcon.png",
                "Assets/Sprites/UI/FacebookIcon.png",
                "Assets/Sprites/UI/MapsIcon.png",
                "Assets/Sprites/UI/BankIcon.png",
                "Assets/Sprites/UI/FinCreditIcon.png",
                "Assets/Sprites/UI/phone_mask.png",
                "Assets/Sprites/UI/phone_frame.png",
                "Assets/Sprites/UI/RoundedRect.png"
            };

            bool changed = false;
            foreach (string p in paths)
            {
                if (File.Exists(p))
                {
                    TextureImporter ti = AssetImporter.GetAtPath(p) as TextureImporter;
                    if (ti != null)
                    {
                        bool tiChanged = false;
                        if (ti.textureType != TextureImporterType.Sprite)
                        {
                            ti.textureType = TextureImporterType.Sprite;
                            tiChanged = true;
                        }
                        if (p.EndsWith("RoundedRect.png") && ti.spriteBorder == Vector4.zero)
                        {
                            ti.spriteBorder = new Vector4(16, 16, 16, 16);
                            tiChanged = true;
                        }
                        if (tiChanged)
                        {
                            ti.SaveAndReimport();
                            changed = true;
                        }
                    }
                }
            }
            if (changed)
            {
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Escape From Hell/Build Chapter 1 Scene")]
        public static void BuildScene()
        {
            Debug.Log("Starting Chapter 1 Scene Build...");

            // Refresh asset database first to ensure all imported changes are captured
            AssetDatabase.Refresh();
            PrepareSprites();

            // Check if sprites exist and load correctly
            Sprite testSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Tiles/Room/Floor.png");

            // 1. Ensure Sprite Assets exist and are loaded correctly
            if (!File.Exists("Assets/Sprites/Characters/Minh/Minh_Idle_Down.png") || 
                !File.Exists("Assets/Sprites/Items/Trash.png") ||
                !File.Exists("Assets/Sprites/UI/NotesIcon.png") ||
                !File.Exists("Assets/Sprites/UI/WinLogo.png") ||
                !File.Exists("Assets/Sprites/UI/phone_mask.png") ||
                !File.Exists("Assets/Sprites/UI/phone_frame.png") ||
                !File.Exists("Assets/Sprites/UI/RoundedRect.png") ||
                testSprite == null)
            {
                Debug.LogWarning("Sprites not found or not imported correctly. Generating sprites...");
                SpriteGenerator.GenerateAll();
                AssetDatabase.Refresh();
            }

            // 2. Create Dialogue Assets
            EnsureDialogueDirectories();
            DialogueData intro = CreateIntroDialogue();
            DialogueData phone = CreatePhoneDialogue();
            DialogueData bills = CreateBillsDialogue();
            DialogueData laptop = CreateLaptopDialogue();
            DialogueData jobOffer = CreateJobOfferDialogue();
            DialogueData thanh = CreateThanhDialogue();
            DialogueData hung = CreateHungDialogue();
            DialogueData finCredit = CreateFinCreditDialogue();

            // 3. Create a new Scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            newScene.name = "Chapter1_Room";

            // 4. Configure Camera
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

            // Add URP 2D Global Light with a dim cool blue tint for a gloomy, damp atmosphere
            GameObject globalLightObj = new GameObject("GlobalLight2D");
            var globalLight = globalLightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            globalLight.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;
            globalLight.color = new Color(0.55f, 0.62f, 0.75f, 1f); // Moody cool blue/gray
            globalLight.intensity = 0.35f;

            // Find and dim the Directional Light for Built-in Pipeline
            Light dirLight = FindAnyObjectByType<Light>();
            if (dirLight != null)
            {
                dirLight.intensity = 0.35f;
                dirLight.color = new Color(0.55f, 0.62f, 0.75f, 1f); // Damp moody tint
            }


            // 5. Create Room Layout (Background Image & Boundary Colliders)
            GameObject environment = new GameObject("Environment");
            
            // Configure background sprite import settings
            string bgPath = "Assets/Sprites/Backgrounds/Chapter1_Room_Bg.png";
            AssetDatabase.ImportAsset(bgPath);
            TextureImporter bgImporter = AssetImporter.GetAtPath(bgPath) as TextureImporter;
            if (bgImporter != null)
            {
                bgImporter.textureType = TextureImporterType.Sprite;
                bgImporter.spriteImportMode = SpriteImportMode.Single;
                bgImporter.spritePixelsPerUnit = 64; // Set 64 PPU so it imports nicely
                bgImporter.filterMode = FilterMode.Point;
                bgImporter.textureCompression = TextureImporterCompression.Uncompressed;
                bgImporter.mipmapEnabled = false;
                EditorUtility.SetDirty(bgImporter);
                bgImporter.SaveAndReimport();
            }

            // Create Background object
            GameObject bgObj = new GameObject("Room_Background");
            bgObj.transform.parent = environment.transform;
            bgObj.transform.position = Vector3.zero;
            SpriteRenderer bgSR = bgObj.AddComponent<SpriteRenderer>();
            bgSR.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);
            bgSR.sortingOrder = -20; // Base background

            // Dynamically scale background to a target size (e.g. 10x10 units)
            float bgScale = 1.0f;
            if (bgSR.sprite != null)
            {
                float spriteWidth = bgSR.sprite.rect.width / bgSR.sprite.pixelsPerUnit;
                float targetWidth = 10f;
                bgScale = targetWidth / spriteWidth;
                bgObj.transform.localScale = new Vector3(bgScale, bgScale, 1);
            }
            else
            {
                Debug.LogError("Failed to load Room Background Sprite! Path: " + bgPath);
            }

            // Create invisible boundary colliders
            // Left boundary: x = -5.0, y = -1 (moved out to match wall edge)
            CreateBoundary(environment.transform, new Vector3(-5.0f, -1.0f, 0f), new Vector2(1f, 8f));
            // Right boundary: x = 5.0, y = -1
            CreateBoundary(environment.transform, new Vector3(5.0f, -1.0f, 0f), new Vector2(1f, 8f));
            // Bottom boundary: x = 0, y = -5.2 (moved down to allow walking all the way to the bottom floorboards)
            CreateBoundary(environment.transform, new Vector3(0f, -5.2f, 0f), new Vector2(10f, 1f));
            // Top boundary: x = 0, y = 1.3 (prevent walking up onto back walls, window, or bookshelves)
            CreateBoundary(environment.transform, new Vector3(0f, 1.3f, 0f), new Vector2(10f, 1f));

            // Furniture boundaries (invisible obstacles matching background items)
            // Desk obstacle at top-left (blocks desk bottom area)
            CreateBoundary(environment.transform, "Boundary_Desk", new Vector3(-2.79f, -0.07f, 0f), new Vector2(3.4f, 1.5f));
            // Bed obstacle at right (blocks main bed area)
            CreateBoundary(environment.transform, "Boundary_Bed", new Vector3(2.6f, -1.0f, 0f), new Vector2(1.6f, 4.4f));
            // Small table (lower center-left) - the low table with electronics/gamepad items
            CreateBoundary(environment.transform, "Boundary_TableLeft", new Vector3(-3.19f, -3.85f, 0f), new Vector2(2.8f, 1.2f));
            // Small table (lower right) - the side table near bottom-right with game cartridges/items
            CreateBoundary(environment.transform, "Boundary_TableRight", new Vector3(4.07f, -3.86f, 0f), new Vector2(1.6f, 2.0f));


            // 6. Create Player
            GameObject playerObj = new GameObject("Player");
            playerObj.tag = "Player";
            playerObj.transform.position = new Vector3(0, -1, 0);

            SpriteRenderer playerSR = playerObj.AddComponent<SpriteRenderer>();
            Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Idle_Down.png");
            if (playerSprite == null) Debug.LogError("Player sprite is NULL! Path: Assets/Sprites/Characters/Minh/Minh_Idle_Down.png");
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

            // Setup Animator Controller and Animations
            if (!Directory.Exists("Assets/Animations"))
            {
                Directory.CreateDirectory("Assets/Animations");
            }
            string controllerPath = "Assets/Animations/PlayerController.controller";
            var playerAnimController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(controllerPath);
            if (playerAnimController == null)
            {
                playerAnimController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            }
            if (playerAnimController != null)
            {
                AddParameterIfMissing(playerAnimController, "MoveX", AnimatorControllerParameterType.Float);
                AddParameterIfMissing(playerAnimController, "MoveY", AnimatorControllerParameterType.Float);
                AddParameterIfMissing(playerAnimController, "LastMoveX", AnimatorControllerParameterType.Float);
                AddParameterIfMissing(playerAnimController, "LastMoveY", AnimatorControllerParameterType.Float);
                AddParameterIfMissing(playerAnimController, "IsMoving", AnimatorControllerParameterType.Bool);

                // Build animation clips and blend trees
                SetupPlayerAnimator(playerAnimController);
            }
            Animator animator = playerObj.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = playerAnimController;
            }

            // Add Interaction trigger
            GameObject interactionTriggerObj = new GameObject("InteractionTrigger");
            interactionTriggerObj.transform.parent = playerObj.transform;
            interactionTriggerObj.transform.localPosition = Vector3.zero;
            CircleCollider2D cc = interactionTriggerObj.AddComponent<CircleCollider2D>();
            cc.isTrigger = true;
            cc.radius = 1.0f;

            // 7. Create UI Canvas & Dialogue UI
            GameObject uiRoot = new GameObject("UI_Canvas");
            Canvas canvas = uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = uiRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            uiRoot.AddComponent<GraphicRaycaster>();

            // Create EventSystem (Critical for button clicks and hovers to register)
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Dialogue Panel (bottom screen)
            GameObject dialPanel = new GameObject("DialoguePanel");
            dialPanel.transform.parent = uiRoot.transform;
            RectTransform dialRect = dialPanel.AddComponent<RectTransform>();
            dialRect.anchorMin = new Vector2(0f, 0f);
            dialRect.anchorMax = new Vector2(1f, 0f);
            dialRect.pivot = new Vector2(0.5f, 0f);
            dialRect.anchoredPosition = new Vector2(0, 20);
            dialRect.sizeDelta = new Vector2(-40, 150); // 20px padding left/right

            Image panelImg = dialPanel.AddComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.85f); // Beautiful dark glass look

            // Add simple red border outline to panel
            Outline panelOutline = dialPanel.AddComponent<Outline>();
            panelOutline.effectColor = new Color(0.8f, 0.1f, 0.1f, 0.6f);
            panelOutline.effectDistance = new Vector2(2, 2);

            // Portrait (Left)
            GameObject portraitObj = new GameObject("Portrait");
            portraitObj.transform.parent = dialPanel.transform;
            RectTransform portRect = portraitObj.AddComponent<RectTransform>();
            portRect.anchorMin = new Vector2(0f, 0.5f);
            portRect.anchorMax = new Vector2(0f, 0.5f);
            portRect.pivot = new Vector2(0f, 0.5f);
            portRect.anchoredPosition = new Vector2(20, 0);
            portRect.sizeDelta = new Vector2(100, 100);
            Image portImg = portraitObj.AddComponent<Image>();

            // Speaker Name (Top Left, beside portrait)
            GameObject nameObj = new GameObject("SpeakerName");
            nameObj.transform.parent = dialPanel.transform;
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0f, 1f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.pivot = new Vector2(0f, 1f);
            nameRect.anchoredPosition = new Vector2(140, -15);
            nameRect.sizeDelta = new Vector2(-160, 30);
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Minh";
            nameText.fontSize = 20;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = new Color(0.9f, 0.6f, 0.1f, 1.0f); // Gold name

            // Dialogue Text (Body)
            GameObject bodyObj = new GameObject("DialogueText");
            bodyObj.transform.parent = dialPanel.transform;
            RectTransform bodyRect = bodyObj.AddComponent<RectTransform>();
            bodyRect.anchorMin = new Vector2(0f, 0f);
            bodyRect.anchorMax = new Vector2(1f, 1f);
            bodyRect.pivot = new Vector2(0f, 1f);
            bodyRect.anchoredPosition = new Vector2(140, -45);
            bodyRect.sizeDelta = new Vector2(-160, -60);
            TextMeshProUGUI bodyText = bodyObj.AddComponent<TextMeshProUGUI>();
            bodyText.text = "Xin chào các bạn...";
            bodyText.fontSize = 16;
            bodyText.color = Color.white;

            // Continue Indicator (bottom right)
            GameObject indicatorObj = new GameObject("ContinueIndicator");
            indicatorObj.transform.parent = dialPanel.transform;
            RectTransform indRect = indicatorObj.AddComponent<RectTransform>();
            indRect.anchorMin = new Vector2(1f, 0f);
            indRect.anchorMax = new Vector2(1f, 0f);
            indRect.pivot = new Vector2(1f, 0f);
            indRect.anchoredPosition = new Vector2(-15, 10);
            indRect.sizeDelta = new Vector2(100, 20);
            TextMeshProUGUI indText = indicatorObj.AddComponent<TextMeshProUGUI>();
            indText.text = "[Nhấn Space/Chuột]";
            indText.fontSize = 11;
            indText.alignment = TextAlignmentOptions.Right;
            indText.color = new Color(0.6f, 0.6f, 0.6f);

            // Choice Panel & Choices Container
            GameObject choicesPanel = new GameObject("ChoicesPanel");
            choicesPanel.transform.parent = uiRoot.transform;
            RectTransform choicesRect = choicesPanel.AddComponent<RectTransform>();
            choicesRect.anchorMin = new Vector2(0.5f, 0.5f);
            choicesRect.anchorMax = new Vector2(0.5f, 0.5f);
            choicesRect.pivot = new Vector2(0.5f, 0.5f);
            choicesRect.anchoredPosition = new Vector2(0, 100);
            choicesRect.sizeDelta = new Vector2(300, 200);
            Image choicesPanelImg = choicesPanel.AddComponent<Image>();
            choicesPanelImg.color = new Color(0f, 0f, 0f, 0.9f);
            choicesPanel.AddComponent<VerticalLayoutGroup>(); // Standard layout for choice buttons

            // Create DialogueSystem Singleton on canvas or separate object
            GameObject sysObj = new GameObject("DialogueSystem");
            DialogueSystem ds = sysObj.AddComponent<DialogueSystem>();

            // Hook up UI variables
            SetPrivateField(ds, "dialoguePanel", dialPanel);
            SetPrivateField(ds, "speakerNameText", nameText);
            SetPrivateField(ds, "dialogueText", bodyText);
            SetPrivateField(ds, "portraitImage", portImg);
            SetPrivateField(ds, "continueIndicator", indicatorObj);
            SetPrivateField(ds, "choicesPanel", choicesPanel);
            SetPrivateField(ds, "choicesContainer", choicesPanel.transform);
            // Note: Choice button prefab can be null for this simple demo, or we can instantiate standard buttons

            // Create the Computer UI
            CreateComputerUI(uiRoot);

            // Create the Phone UI
            CreatePhoneUI(uiRoot);

            // 8. Create Props (Invisible triggers matching background illustration)
            GameObject propsParent = new GameObject("Props");
            propsParent.transform.position = Vector3.zero;

            // Laptop on table (placed exactly on the desk surface shown in the illustration)
            GameObject laptopObj = CreateProp(propsParent.transform, "Laptop", new Vector3(-2.2f, 0.9f, 0), Chapter1PropType.Laptop);
            
            // Phone on bed (placed exactly on the lower part of the bed sheets)
            GameObject phoneObj = CreateProp(propsParent.transform, "Phone", new Vector3(2.0f, -1.0f, 0), Chapter1PropType.Phone);
            GameObject phoneSpriteObj = new GameObject("Sprite");
            phoneSpriteObj.transform.SetParent(phoneObj.transform, false);
            phoneSpriteObj.transform.localScale = new Vector3(0.16f, 0.22f, 1f); // Small and slightly elongated like a modern phone
            SpriteRenderer phoneSR = phoneSpriteObj.AddComponent<SpriteRenderer>();
            phoneSR.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Phone.png");
            phoneSR.sortingOrder = 1; // Render on top of background sheets, below player

            // 9. Create Chapter1Controller
            GameObject controllerObj = new GameObject("Chapter1Controller");
            Chapter1Controller c1 = controllerObj.AddComponent<Chapter1Controller>();
            SetPrivateField(c1, "introDialogue", intro);
            SetPrivateField(c1, "phoneDialogue", phone);
            SetPrivateField(c1, "billsDialogue", bills);
            SetPrivateField(c1, "laptopDialogue", laptop);
            SetPrivateField(c1, "jobOfferDialogue", jobOffer);
            SetPrivateField(c1, "thanhDialogue", thanh);
            SetPrivateField(c1, "hungDialogue", hung);
            SetPrivateField(c1, "finCreditDialogue", finCredit);

            // 10. GameManager setup in scene (so it exists)
            GameObject gmObj = new GameObject("GameManager");
            GameManager gm = gmObj.AddComponent<GameManager>();
            gm.ChangeState(GameState.Playing);

            // Save the scene
            if (!Directory.Exists("Assets/Scenes"))
            {
                Directory.CreateDirectory("Assets/Scenes");
            }
            EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Chapter1_Room.unity");
            Debug.Log("Chapter 1 Scene Saved to Assets/Scenes/Chapter1_Room.unity!");

            // Add scene to Build Settings
            AddSceneToBuildSettings("Assets/Scenes/Chapter1_Room.unity");
        }

        private static void CreateBoundary(Transform parent, string name, Vector3 pos, Vector2 size)
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = parent;
            obj.transform.position = pos;
            BoxCollider2D bc = obj.AddComponent<BoxCollider2D>();
            bc.size = size;
        }

        // Legacy overload without name
        private static void CreateBoundary(Transform parent, Vector3 pos, Vector2 size)
        {
            CreateBoundary(parent, "Boundary", pos, size);
        }

        private static GameObject CreateProp(Transform parent, string name, Vector3 pos, Chapter1PropType type)
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = parent;
            obj.transform.position = pos;
            
            // Invisible trigger collider so player can step near it and trigger interactions
            BoxCollider2D bc = obj.AddComponent<BoxCollider2D>();
            bc.size = Vector2.one * 1.2f; // Slightly larger for easier detection
            bc.isTrigger = true;
            
            Chapter1Prop prop = obj.AddComponent<Chapter1Prop>();
            SetPrivateField(prop, "propType", type);
            SetPrivateField(prop, "promptMessage", $"Xem {name}");

            return obj;
        }

        private static void EnsureDialogueDirectories()
        {
            if (!Directory.Exists("Assets/ScriptableObjects/Dialogues"))
            {
                Directory.CreateDirectory("Assets/ScriptableObjects/Dialogues");
            }
        }

        private static DialogueData CreateIntroDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Uầy... Đầu đau búa bổ. Mấy giờ rồi nhỉ?" },
                new DialogueLine { speakerName = "Minh", text = "Lại một ngày nữa thức dậy trong căn phòng trọ chật hẹp này..." },
                new DialogueLine { speakerName = "Minh", text = "Mình cần tìm cách thanh toán đống hóa đơn tháng này, rồi mở laptop lên xem có việc nào mới không. À, kiểm tra cả điện thoại nữa." }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_Intro.asset");
            return data;
        }

        private static DialogueData CreatePhoneDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Mở điện thoại lên xem nào... Có tin nhắn mới từ Mẹ." },
                new DialogueLine { speakerName = "Mẹ", text = "Minh ơi, tháng này dưới quê mất mùa quá, bố con lại đau chân. Con xem có gửi về cho mẹ ít tiền thuốc thang cho bố được không?" },
                new DialogueLine { speakerName = "Minh", text = "Bố mẹ ở quê đang khó khăn như thế... Mà mình vô dụng quá, ngay cả tiền phòng trọ còn chưa đóng nổi, lấy đâu ra tiền gửi về lo thuốc thang cho bố bây giờ..." }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_Phone.asset");
            return data;
        }

        private static DialogueData CreateThanhDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Đến cả Thành cũng nhắn tin đòi tiền..." },
                new DialogueLine { speakerName = "Minh", text = "Lúc trước kẹt quá vay nó 1 triệu... giờ nó cần gấp mà mình không có một xu để trả." },
                new DialogueLine { speakerName = "Minh", text = "Nhìn tin nhắn của nó mà thấy tội lỗi quá... Thôi, giờ có trả lời cũng chẳng giải quyết được gì, mình còn không biết ngày mai lấy gì ăn." }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_Thanh.asset");
            return data;
        }

        private static DialogueData CreateHungDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Hỏi thăm Hùng xem có xoay sở được ít tiền không, ai ngờ nó cũng đang cực khổ..." },
                new DialogueLine { speakerName = "Minh", text = "Công ty nợ lương, mọi người ai cũng đang phải vật lộn. Hỏi mượn tiền lúc này thật sự không đúng chút nào..." }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_Hung.asset");
            return data;
        }

        private static DialogueData CreateFinCreditDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Lại là tin nhắn đòi nợ từ bên app vay tiền..." },
                new DialogueLine { speakerName = "Minh", text = "Họ dọa sẽ liên hệ với gia đình và người thân nếu không trả nợ trước chiều nay." },
                new DialogueLine { speakerName = "Minh", text = "Nếu mẹ biết chuyện mình vay nợ app thế này, mẹ chắc sẽ sốc lắm... Mình phải làm sao đây?" }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_FinCredit.asset");
            return data;
        }

        private static DialogueData CreateBillsDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Tờ thông báo tiền nhà..." },
                new DialogueLine { speakerName = "Hóa Đơn", text = "Tổng tiền nhà + điện + nước tháng này: 3,500,000đ.\nHạn chót thanh toán: ngày 15. Quá hạn sẽ cắt điện nước." },
                new DialogueLine { speakerName = "Minh", text = "Hôm nay đã là ngày 11 rồi... Trong tài khoản chỉ còn đúng 500k. Kiểu này chắc bị đuổi ra đường mất." }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_Bills.asset");
            return data;
        }

        private static DialogueData CreateLaptopDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Mở laptop lên tìm việc làm..." },
                new DialogueLine { speakerName = "Tuyển Dụng", text = "Tuyển dụng Lập trình viên Web mới ra trường.\nYêu cầu: 2 năm kinh nghiệm thực tế, thành thạo React, Node, AWS, Docker.\nLương: 6 - 8 triệu." },
                new DialogueLine { speakerName = "Minh", text = "Thị trường IT bây giờ nát quá... Mới ra trường đòi 2 năm kinh nghiệm mà lương không đủ trả tiền nhà Sài Gòn. Biết sống sao đây?" }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_Laptop.asset");
            return data;
        }

        private static DialogueData CreateJobOfferDialogue()
        {
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();
            data.lines = new List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = "Ủa... Tuyển nhân viên văn phòng ở Campuchia lương 38 đến 50 triệu/tháng cơ á? Lương cao bất thường vậy?" },
                new DialogueLine { speakerName = "Minh", text = "Khoan đã... Campuchia? Bavet? Mình nghe bạn bè cảnh báo khu đó toàn casino lừa đảo và buôn bán người thôi." },
                new DialogueLine { speakerName = "Minh", text = "Số lạ, lương không tưởng, địa điểm đáng nghi... Đây chắc chắn là bẫy lừa đảo rồi. Mình phải xóa tin nhắn này đi." }
            };
            AssetDatabase.CreateAsset(data, "Assets/ScriptableObjects/Dialogues/Chapter1_JobOffer.asset");
            return data;
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
            
            // Check if scene is already added
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

        private static void AddParameterIfMissing(UnityEditor.Animations.AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            foreach (var param in controller.parameters)
            {
                if (param.name == name) return;
            }
            controller.AddParameter(name, type);
        }

        private static void ConfigureSpriteImportSettings(string path)
        {
            if (!System.IO.File.Exists(path)) return;
            AssetDatabase.ImportAsset(path);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear; // Bilinear filtering for clean wallpaper curves
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }
        }

        private static void CreateComputerUI(GameObject uiRoot)
        {
            // Load custom PC wallpaper
            string pcWallpaperPath = "Assets/Sprites/UI/pc_wallpaper.png";
            ConfigureSpriteImportSettings(pcWallpaperPath);
            Sprite pcWallpaperSprite = AssetDatabase.LoadAssetAtPath<Sprite>(pcWallpaperPath);

            // 1. Root Computer UI panel
            GameObject compUIObj = new GameObject("ComputerUI");
            compUIObj.transform.parent = uiRoot.transform;
            
            // Add RectTransform to the root UI container to anchor it properly inside the Canvas
            RectTransform rootRect = compUIObj.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;
            rootRect.anchoredPosition = Vector2.zero;

            ComputerUI compUI = compUIObj.AddComponent<ComputerUI>();

            // Main computer panel (workspace overlay)
            GameObject compPanel = new GameObject("ComputerPanel");
            compPanel.transform.parent = compUIObj.transform;
            RectTransform panelRect = compPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero; // Stretched to full screen overlay
            panelRect.anchoredPosition = Vector2.zero;

            // Stretched dark overlay (behind workspace)
            Image overlayImg = compPanel.AddComponent<Image>();
            overlayImg.color = new Color(0f, 0f, 0f, 0.75f); // Black translucent overlay

            // Workspace desk frame (Modern Desktop Background)
            GameObject workspace = new GameObject("Workspace");
            workspace.transform.parent = compPanel.transform;
            RectTransform workRect = workspace.AddComponent<RectTransform>();
            workRect.anchorMin = Vector2.zero;
            workRect.anchorMax = Vector2.one;
            workRect.sizeDelta = Vector2.zero; // Stretch to fill the screen!
            workRect.anchoredPosition = Vector2.zero;

            Image workImg = workspace.AddComponent<Image>();
            if (pcWallpaperSprite != null)
            {
                workImg.sprite = pcWallpaperSprite;
                workImg.color = Color.white;
            }
            else
            {
                workImg.color = new Color(0f, 0.47f, 0.83f, 1f); // Windows 10 default blue
            }
            Outline workOutline = workspace.AddComponent<Outline>();
            workOutline.effectColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            workOutline.effectDistance = new Vector2(3, 3);


            // --- Desktop Icons Container (Vertical grid) ---
            GameObject iconsContainer = new GameObject("IconsContainer");
            iconsContainer.transform.parent = workspace.transform;
            RectTransform icRect = iconsContainer.AddComponent<RectTransform>();
            icRect.anchorMin = new Vector2(0f, 0f);
            icRect.anchorMax = new Vector2(0f, 1f);
            icRect.pivot = new Vector2(0f, 1f); // pivot at top-left
            icRect.sizeDelta = new Vector2(110, -80); // leave 48px for taskbar + some padding
            icRect.anchoredPosition = new Vector2(25, -25); // 25px margin from left and top
            
            VerticalLayoutGroup vlg = iconsContainer.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 12f;
            vlg.childAlignment = TextAnchor.UpperCenter; // Center children within the 110px width
            vlg.childControlHeight = false;
            vlg.childControlWidth = true; // Control width to stretch the buttons to 110px

            // Load sprites for icons
            Sprite pcIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Laptop.png");
            Sprite trashIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Trash.png");
            Sprite mailIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Document.png");
            Sprite billsIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Document.png");
            Sprite chromeIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Chrome.png");
            Sprite winLogoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/WinLogo.png");

            // 1. This PC Icon
            GameObject pcIconObj = CreateDesktopIconButton(iconsContainer.transform, "ThisPCIcon", pcIconSprite, "This PC");
            Button pcBtn = pcIconObj.GetComponent<Button>();
            pcBtn.onClick.AddListener(() => compUI.OpenThisPC());

            // 2. Recycle Bin Icon
            GameObject trashIconObj = CreateDesktopIconButton(iconsContainer.transform, "RecycleBinIcon", trashIconSprite, "Thùng Rác");
            Button trashBtn = trashIconObj.GetComponent<Button>();
            trashBtn.onClick.AddListener(() => compUI.OpenRecycleBin());

            // 3. Mail Icon Button
            GameObject mailIconObj = CreateDesktopIconButton(iconsContainer.transform, "MailIcon", mailIconSprite, "Hộp Thư (Mail)");
            Button mailBtn = mailIconObj.GetComponent<Button>();
            mailBtn.onClick.AddListener(() => compUI.OpenMail());

            // 4. Bills Icon Button
            GameObject billsIconObj = CreateDesktopIconButton(iconsContainer.transform, "BillsIcon", billsIconSprite, "Hóa Đơn (Bills)");
            Button billsBtn = billsIconObj.GetComponent<Button>();
            billsBtn.onClick.AddListener(() => compUI.OpenBills());

            // 5. Chrome Icon Button
            GameObject chromeIconObj = CreateDesktopIconButton(iconsContainer.transform, "ChromeIcon", chromeIconSprite, "Google Chrome");
            Button chromeBtn = chromeIconObj.GetComponent<Button>();
            chromeBtn.onClick.AddListener(() => compUI.OpenChrome());

            // --- Windows 11 Taskbar (bottom) ---
            GameObject taskbar = new GameObject("Taskbar");
            taskbar.transform.parent = workspace.transform;
            RectTransform tbRect = taskbar.AddComponent<RectTransform>();
            tbRect.anchorMin = new Vector2(0f, 0f);
            tbRect.anchorMax = new Vector2(1f, 0f);
            tbRect.pivot = new Vector2(0.5f, 0f);
            tbRect.sizeDelta = new Vector2(0, 48); // Standard 48px height
            tbRect.anchoredPosition = Vector2.zero;
            Image tbImg = taskbar.AddComponent<Image>();
            tbImg.color = new Color(0.04f, 0.04f, 0.06f, 0.85f); // Translucent acrylic dark tone
            Outline tbOutline = taskbar.AddComponent<Outline>();
            tbOutline.effectColor = new Color(1f, 1f, 1f, 0.08f); // Top border highlight
            tbOutline.effectDistance = new Vector2(0, 1);

            // Centered shortcuts container (Windows 11 Centered style)
            GameObject tbCenterContainer = new GameObject("CenteredContainer");
            tbCenterContainer.transform.parent = taskbar.transform;
            RectTransform tbcRect = tbCenterContainer.AddComponent<RectTransform>();
            tbcRect.anchorMin = new Vector2(0.5f, 0.5f);
            tbcRect.anchorMax = new Vector2(0.5f, 0.5f);
            tbcRect.pivot = new Vector2(0.5f, 0.5f);
            tbcRect.sizeDelta = new Vector2(200, 48);
            tbcRect.anchoredPosition = Vector2.zero;

            HorizontalLayoutGroup tbcHlg = tbCenterContainer.AddComponent<HorizontalLayoutGroup>();
            tbcHlg.spacing = 10f;
            tbcHlg.childAlignment = TextAnchor.MiddleCenter;
            tbcHlg.childControlWidth = false;
            tbcHlg.childControlHeight = false;

            // Taskbar Start Button (Windows logo style)
            GameObject startBtnObj = new GameObject("StartButton");
            startBtnObj.transform.parent = tbCenterContainer.transform;
            RectTransform sbRect = startBtnObj.AddComponent<RectTransform>();
            sbRect.sizeDelta = new Vector2(36, 36);
            
            Image sbImg = startBtnObj.AddComponent<Image>();
            sbImg.color = Color.clear;
            Button sbBtn = startBtnObj.AddComponent<Button>();
            sbBtn.targetGraphic = sbImg;
            
            ColorBlock sbCb = sbBtn.colors;
            sbCb.normalColor = Color.clear;
            sbCb.highlightedColor = new Color(1f, 1f, 1f, 0.08f);
            sbCb.pressedColor = new Color(1f, 1f, 1f, 0.15f);
            sbCb.selectedColor = new Color(1f, 1f, 1f, 0.1f);
            sbBtn.colors = sbCb;

            GameObject sbIconObj = new GameObject("Icon");
            sbIconObj.transform.parent = startBtnObj.transform;
            RectTransform sbiRect = sbIconObj.AddComponent<RectTransform>();
            sbiRect.anchorMin = new Vector2(0.5f, 0.5f);
            sbiRect.anchorMax = new Vector2(0.5f, 0.5f);
            sbiRect.pivot = new Vector2(0.5f, 0.5f);
            sbiRect.sizeDelta = new Vector2(22, 22);
            Image sbiImg = sbIconObj.AddComponent<Image>();
            sbiImg.sprite = winLogoSprite;
            sbiImg.color = Color.white;
            sbiImg.preserveAspect = true;

            // Taskbar App Shortcuts (Explorer, Chrome, Mail) - placed in the center container
            CreateTaskbarShortcut(tbCenterContainer.transform, "ExplorerShortcut", pcIconSprite, () => compUI.OpenThisPC());
            CreateTaskbarShortcut(tbCenterContainer.transform, "ChromeShortcut", chromeIconSprite, () => compUI.OpenChrome());
            CreateTaskbarShortcut(tbCenterContainer.transform, "MailShortcut", mailIconSprite, () => compUI.OpenMail());

            // Taskbar Clock (System Tray - far right)
            GameObject clockObj = new GameObject("Clock");
            clockObj.transform.parent = taskbar.transform;
            RectTransform clockRect = clockObj.AddComponent<RectTransform>();
            clockRect.anchorMin = new Vector2(1f, 0.5f);
            clockRect.anchorMax = new Vector2(1f, 0.5f);
            clockRect.pivot = new Vector2(1f, 0.5f);
            clockRect.sizeDelta = new Vector2(100, 40);
            clockRect.anchoredPosition = new Vector2(-15, 0);
            TextMeshProUGUI clockTxt = clockObj.AddComponent<TextMeshProUGUI>();
            clockTxt.text = "16:02\n11/06/2026";
            clockTxt.fontSize = 11;
            clockTxt.color = Color.white;
            clockTxt.alignment = TextAlignmentOptions.Right;
            clockTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // --- Shutdown Button (Taskbar right next to clock, sleek & round styling) ---
            GameObject shutdownObj = new GameObject("ShutdownButton");
            shutdownObj.transform.SetParent(taskbar.transform, false);
            RectTransform sdRect = shutdownObj.AddComponent<RectTransform>();
            sdRect.anchorMin = new Vector2(1f, 0.5f);
            sdRect.anchorMax = new Vector2(1f, 0.5f);
            sdRect.pivot = new Vector2(1f, 0.5f);
            sdRect.sizeDelta = new Vector2(80, 28);
            sdRect.anchoredPosition = new Vector2(-125, 0); // Placed next to clock (separated from clock left edge)

            Image sdImg = shutdownObj.AddComponent<Image>();
            sdImg.color = new Color(0.9f, 0.2f, 0.2f, 0.12f); // Sleek translucent red background
            Outline sdOutline = shutdownObj.AddComponent<Outline>();
            sdOutline.effectColor = new Color(0.9f, 0.2f, 0.2f, 0.4f);
            sdOutline.effectDistance = new Vector2(1, 1);

            Button sdBtn = shutdownObj.AddComponent<Button>();
            sdBtn.targetGraphic = sdImg;
            ColorBlock sdCb = sdBtn.colors;
            sdCb.normalColor = new Color(0.9f, 0.2f, 0.2f, 0.12f);
            sdCb.highlightedColor = new Color(0.9f, 0.2f, 0.2f, 0.35f); // brighter on hover
            sdCb.pressedColor = new Color(0.9f, 0.2f, 0.2f, 0.6f);
            sdCb.selectedColor = new Color(0.9f, 0.2f, 0.2f, 0.12f);
            sdBtn.colors = sdCb;

            GameObject sdTextObj = new GameObject("Text");
            sdTextObj.transform.SetParent(shutdownObj.transform, false);
            RectTransform sdtRect = sdTextObj.AddComponent<RectTransform>();
            sdtRect.anchorMin = Vector2.zero;
            sdtRect.anchorMax = Vector2.one;
            sdtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI sdtText = sdTextObj.AddComponent<TextMeshProUGUI>();
            sdtText.text = "Tắt Máy";
            sdtText.fontSize = 11;
            sdtText.color = Color.white;
            sdtText.alignment = TextAlignmentOptions.Center;
            sdtText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // --- MAIL WINDOW ---
            GameObject mailWin = new GameObject("MailWindow");
            mailWin.transform.parent = workspace.transform;
            RectTransform mwRect = mailWin.AddComponent<RectTransform>();
            mwRect.anchorMin = new Vector2(0.5f, 0.5f);
            mwRect.anchorMax = new Vector2(0.5f, 0.5f);
            mwRect.sizeDelta = new Vector2(550, 400);
            mwRect.anchoredPosition = new Vector2(80, -10); // Offset to the right
            Image mwImg = mailWin.AddComponent<Image>();
            mwImg.color = new Color(0.06f, 0.06f, 0.09f, 0.88f); // Frosted dark glass
            Outline mwOutline = mailWin.AddComponent<Outline>();
            mwOutline.effectColor = new Color(1f, 1f, 1f, 0.12f); // Sleek glowing border

            // Mail Window Header
            GameObject mwHeader = CreateSubWindowHeader(mailWin.transform, "Hộp Thư Điện Tử", () => compUI.CloseMail());

            // Mail Window Split Workspace
            // Left list container
            GameObject emailList = new GameObject("EmailList");
            emailList.transform.SetParent(mailWin.transform, false);
            RectTransform elRect = emailList.AddComponent<RectTransform>();
            elRect.anchorMin = new Vector2(0f, 0f);
            elRect.anchorMax = new Vector2(0.35f, 1f);
            elRect.offsetMin = new Vector2(10, 10);
            elRect.offsetMax = new Vector2(-5, -42);
            Image elBgImg = emailList.AddComponent<Image>();
            elBgImg.color = new Color(0.1f, 0.1f, 0.13f, 0.6f); // List pane background
            Outline elOutline = emailList.AddComponent<Outline>();
            elOutline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            VerticalLayoutGroup elVlg = emailList.AddComponent<VerticalLayoutGroup>();
            elVlg.padding = new RectOffset(5, 5, 10, 10);
            elVlg.spacing = 10f;
            elVlg.childAlignment = TextAnchor.UpperCenter;
            elVlg.childControlWidth = true;
            elVlg.childControlHeight = false;

            // Email 1 Button
            GameObject e1BtnObj = CreateEmailButton(emailList.transform, "Thư từ chối 1", "HR VinaTech");
            Button e1Btn = e1BtnObj.GetComponent<Button>();

            // Email 2 Button
            GameObject e2BtnObj = CreateEmailButton(emailList.transform, "Thư từ chối 2", "HR G-Star");
            Button e2Btn = e2BtnObj.GetComponent<Button>();

            // Email 3 Button
            GameObject e3BtnObj = CreateEmailButton(emailList.transform, "Thư từ chối 3", "HR FPT Software");
            Button e3Btn = e3BtnObj.GetComponent<Button>();

            // Email 4 Button
            GameObject e4BtnObj = CreateEmailButton(emailList.transform, "Thư từ chối 4", "HR VNG Corp");
            Button e4Btn = e4BtnObj.GetComponent<Button>();

            // Right view pane
            GameObject emailView = new GameObject("EmailView");
            emailView.transform.SetParent(mailWin.transform, false);
            RectTransform evRect = emailView.AddComponent<RectTransform>();
            evRect.anchorMin = new Vector2(0.35f, 0f);
            evRect.anchorMax = new Vector2(1f, 1f);
            evRect.offsetMin = new Vector2(5, 10);
            evRect.offsetMax = new Vector2(-10, -42);
            Image evImg = emailView.AddComponent<Image>();
            evImg.color = new Color(0.06f, 0.06f, 0.08f, 0.6f);
            Outline evOutline = emailView.AddComponent<Outline>();
            evOutline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            GameObject evTextObj = new GameObject("BodyText");
            evTextObj.transform.SetParent(emailView.transform, false);
            RectTransform evtRect = evTextObj.AddComponent<RectTransform>();
            evtRect.anchorMin = Vector2.zero;
            evtRect.anchorMax = Vector2.one;
            evtRect.offsetMin = new Vector2(10, 10);
            evtRect.offsetMax = new Vector2(-10, -10);
            TextMeshProUGUI evBodyText = evTextObj.AddComponent<TextMeshProUGUI>();
            evBodyText.text = "Chọn thư để xem...";
            evBodyText.fontSize = 12;
            evBodyText.color = Color.white;

            // --- BILLS WINDOW ---
            GameObject billsWin = new GameObject("BillsWindow");
            billsWin.transform.parent = workspace.transform;
            RectTransform bwRect = billsWin.AddComponent<RectTransform>();
            bwRect.anchorMin = new Vector2(0.5f, 0.5f);
            bwRect.anchorMax = new Vector2(0.5f, 0.5f);
            bwRect.sizeDelta = new Vector2(420, 380);
            bwRect.anchoredPosition = new Vector2(90, -10);
            Image bwImg = billsWin.AddComponent<Image>();
            bwImg.color = new Color(0.06f, 0.06f, 0.09f, 0.88f);
            Outline bwOutline = billsWin.AddComponent<Outline>();
            bwOutline.effectColor = new Color(1f, 1f, 1f, 0.12f);

            // Bills Window Header
            GameObject bwHeader = CreateSubWindowHeader(billsWin.transform, "Hóa Đơn Điện Tử", () => compUI.CloseBills());

            // Bills Content
            GameObject bwContent = new GameObject("BillsContent");
            bwContent.transform.SetParent(billsWin.transform, false);
            RectTransform bwcRect = bwContent.AddComponent<RectTransform>();
            bwcRect.anchorMin = Vector2.zero;
            bwcRect.anchorMax = Vector2.one;
            bwcRect.offsetMin = new Vector2(15, 15);
            bwcRect.offsetMax = new Vector2(-15, -42);
            Image bwcImg = bwContent.AddComponent<Image>();
            bwcImg.color = new Color(0.08f, 0.08f, 0.1f, 0.6f); // modern dark paper look
            Outline bwcOutline = bwContent.AddComponent<Outline>();
            bwcOutline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            GameObject bwTextObj = new GameObject("Text");
            bwTextObj.transform.SetParent(bwContent.transform, false);
            RectTransform bwtRect = bwTextObj.AddComponent<RectTransform>();
            bwtRect.anchorMin = Vector2.zero;
            bwtRect.anchorMax = Vector2.one;
            bwtRect.offsetMin = new Vector2(10, 10);
            bwtRect.offsetMax = new Vector2(-10, -10);
            TextMeshProUGUI bwText = bwTextObj.AddComponent<TextMeshProUGUI>();
            bwText.text = "<b>THÔNG BÁO THANH TOÁN DỊCH VỤ - THÁNG 6</b><br>" +
                          "------------------------------------------<br>" +
                          "<b>1. Tiền phòng trọ:</b> 3,000,000đ<br>" +
                          "<b>2. Tiền điện (80kWh):</b> 280,000đ<br>" +
                          "<b>3. Tiền nước (3 khối):</b> 60,000đ<br>" +
                          "<b>4. Tiền Internet:</b> 160,000đ<br>" +
                          "------------------------------------------<br>" +
                          "<b>TỔNG CỘNG:</b> <color=#ff4d4d>3,500,000đ</color><br>" +
                          "Hạn chót thanh toán: 15/06/2026.<br><br>" +
                          "<i>Lưu ý: Quá hạn sẽ cắt dịch vụ điện nước và thu hồi phòng trọ.</i><br>" +
                          "<i>Tài khoản hiện tại: 500,000đ (Thiếu: 3,000,000đ).</i>";
            bwText.fontSize = 12;
            bwText.color = Color.white;

            // --- THIS PC WINDOW ---
            GameObject thisPCWin = new GameObject("ThisPCWindow");
            thisPCWin.transform.parent = workspace.transform;
            RectTransform pcwRect = thisPCWin.AddComponent<RectTransform>();
            pcwRect.anchorMin = new Vector2(0.5f, 0.5f);
            pcwRect.anchorMax = new Vector2(0.5f, 0.5f);
            pcwRect.sizeDelta = new Vector2(500, 360);
            pcwRect.anchoredPosition = new Vector2(60, 10);
            Image pcwImg = thisPCWin.AddComponent<Image>();
            pcwImg.color = new Color(0.06f, 0.06f, 0.09f, 0.88f);
            Outline pcwOutline = thisPCWin.AddComponent<Outline>();
            pcwOutline.effectColor = new Color(1f, 1f, 1f, 0.12f);

            GameObject pcwHeader = CreateSubWindowHeader(thisPCWin.transform, "This PC", () => compUI.CloseThisPC());

            GameObject pcwSidebar = new GameObject("Sidebar");
            pcwSidebar.transform.SetParent(thisPCWin.transform, false);
            RectTransform pcsRect = pcwSidebar.AddComponent<RectTransform>();
            pcsRect.anchorMin = new Vector2(0f, 0f);
            pcsRect.anchorMax = new Vector2(0.3f, 1f);
            pcsRect.offsetMin = new Vector2(10, 10);
            pcsRect.offsetMax = new Vector2(-5, -42);
            Image pcsImg = pcwSidebar.AddComponent<Image>();
            pcsImg.color = new Color(0.1f, 0.1f, 0.13f, 0.6f);
            Outline pcsOutline = pcwSidebar.AddComponent<Outline>();
            pcsOutline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            GameObject pcsTextObj = new GameObject("Text");
            pcsTextObj.transform.SetParent(pcwSidebar.transform, false);
            RectTransform pcstRect = pcsTextObj.AddComponent<RectTransform>();
            pcstRect.anchorMin = Vector2.zero;
            pcstRect.anchorMax = Vector2.one;
            pcstRect.offsetMin = new Vector2(10, 10);
            pcstRect.offsetMax = new Vector2(-10, -10);
            TextMeshProUGUI pcsTxt = pcsTextObj.AddComponent<TextMeshProUGUI>();
            pcsTxt.text = "<b>► Quick Access</b>\n" +
                          "  - Desktop\n" +
                          "  - Downloads\n" +
                          "  - Documents\n\n" +
                          "<b>► This PC</b>\n" +
                          "  - Local Disk (C:)";
            pcsTxt.fontSize = 11;
            pcsTxt.color = Color.white;
            pcsTxt.lineSpacing = 12f;

            GameObject pcwContent = new GameObject("Content");
            pcwContent.transform.SetParent(thisPCWin.transform, false);
            RectTransform pcwcRect = pcwContent.AddComponent<RectTransform>();
            pcwcRect.anchorMin = new Vector2(0.3f, 0f);
            pcwcRect.anchorMax = new Vector2(1f, 1f);
            pcwcRect.offsetMin = new Vector2(5, 10);
            pcwcRect.offsetMax = new Vector2(-10, -42);
            Image pcwcImg = pcwContent.AddComponent<Image>();
            pcwcImg.color = new Color(0.06f, 0.06f, 0.08f, 0.6f);
            Outline pcwcOutline = pcwContent.AddComponent<Outline>();
            pcwcOutline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            GameObject pcwcTitle = new GameObject("Title");
            pcwcTitle.transform.SetParent(pcwContent.transform, false);
            RectTransform pcwctRect = pcwcTitle.AddComponent<RectTransform>();
            pcwctRect.anchorMin = new Vector2(0f, 1f);
            pcwctRect.anchorMax = new Vector2(1f, 1f);
            pcwctRect.pivot = new Vector2(0.5f, 1f);
            pcwctRect.sizeDelta = new Vector2(-20, 30);
            pcwctRect.anchoredPosition = new Vector2(0, -10);
            TextMeshProUGUI pcwctTxt = pcwcTitle.AddComponent<TextMeshProUGUI>();
            pcwctTxt.text = "Devices and drives (1)";
            pcwctTxt.fontSize = 12;
            pcwctTxt.fontStyle = FontStyles.Bold;
            pcwctTxt.color = Color.white;

            GameObject driveCBtnObj = new GameObject("DriveCButton");
            driveCBtnObj.transform.SetParent(pcwContent.transform, false);
            RectTransform dcRect = driveCBtnObj.AddComponent<RectTransform>();
            dcRect.anchorMin = new Vector2(0f, 1f);
            dcRect.anchorMax = new Vector2(1f, 1f);
            dcRect.pivot = new Vector2(0.5f, 1f);
            dcRect.sizeDelta = new Vector2(-20, 60);
            dcRect.anchoredPosition = new Vector2(0, -45);
            Image dcImg = driveCBtnObj.AddComponent<Image>();
            dcImg.color = new Color(1f, 1f, 1f, 0.03f);
            Button dcBtn = driveCBtnObj.AddComponent<Button>();
            dcBtn.onClick.AddListener(() => compUI.ClickThisPCDrive());
            Outline dcOutline = driveCBtnObj.AddComponent<Outline>();
            dcOutline.effectColor = new Color(1f, 1f, 1f, 0.08f);

            ColorBlock dcCb = dcBtn.colors;
            dcCb.normalColor = new Color(1f, 1f, 1f, 0.03f);
            dcCb.highlightedColor = new Color(1f, 1f, 1f, 0.08f);
            dcCb.pressedColor = new Color(1f, 1f, 1f, 0.12f);
            dcCb.selectedColor = dcCb.normalColor;
            dcBtn.colors = dcCb;

            GameObject dcIcon = new GameObject("Icon");
            dcIcon.transform.SetParent(driveCBtnObj.transform, false);
            RectTransform dciRect = dcIcon.AddComponent<RectTransform>();
            dciRect.anchorMin = new Vector2(0f, 0.5f);
            dciRect.anchorMax = new Vector2(0f, 0.5f);
            dciRect.pivot = new Vector2(0f, 0.5f);
            dciRect.sizeDelta = new Vector2(28, 28);
            dciRect.anchoredPosition = new Vector2(15, 0);
            Image dciImg = dcIcon.AddComponent<Image>();
            dciImg.sprite = pcIconSprite;
            dciImg.color = Color.white;
            dciImg.preserveAspect = true;

            GameObject dcText = new GameObject("Text");
            dcText.transform.SetParent(driveCBtnObj.transform, false);
            RectTransform dctRect = dcText.AddComponent<RectTransform>();
            dctRect.anchorMin = new Vector2(0f, 0f);
            dctRect.anchorMax = new Vector2(1f, 1f);
            dctRect.sizeDelta = new Vector2(-70, -10);
            dctRect.anchoredPosition = new Vector2(25, 0);
            TextMeshProUGUI dctTxt = dcText.AddComponent<TextMeshProUGUI>();
            dctTxt.text = "<b>Local Disk (C:)</b>\n1.2 GB free of 120 GB";
            dctTxt.fontSize = 11;
            dctTxt.color = Color.white;
            dctTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            GameObject barBg = new GameObject("BarBg");
            barBg.transform.SetParent(driveCBtnObj.transform, false);
            RectTransform bbRect = barBg.AddComponent<RectTransform>();
            bbRect.anchorMin = new Vector2(0f, 0f);
            bbRect.anchorMax = new Vector2(1f, 0f);
            bbRect.pivot = new Vector2(0.5f, 0f);
            bbRect.sizeDelta = new Vector2(-70, 6);
            bbRect.anchoredPosition = new Vector2(25, 8);
            Image bbImg = barBg.AddComponent<Image>();
            bbImg.color = new Color(0.08f, 0.08f, 0.1f, 1f);

            GameObject barFill = new GameObject("BarFill");
            barFill.transform.parent = barBg.transform;
            RectTransform bfRect = barFill.AddComponent<RectTransform>();
            bfRect.anchorMin = new Vector2(0f, 0f);
            bfRect.anchorMax = new Vector2(0.99f, 1f);
            bfRect.sizeDelta = Vector2.zero;
            bfRect.anchoredPosition = Vector2.zero;
            Image bfImg = barFill.AddComponent<Image>();
            bfImg.color = new Color(0.95f, 0.28f, 0.28f, 1f);

            // --- RECYCLE BIN WINDOW ---
            GameObject recycleBinWin = new GameObject("RecycleBinWindow");
            recycleBinWin.transform.parent = workspace.transform;
            RectTransform rbwRect = recycleBinWin.AddComponent<RectTransform>();
            rbwRect.anchorMin = new Vector2(0.5f, 0.5f);
            rbwRect.anchorMax = new Vector2(0.5f, 0.5f);
            rbwRect.sizeDelta = new Vector2(480, 340);
            rbwRect.anchoredPosition = new Vector2(40, 20);
            Image rbwImg = recycleBinWin.AddComponent<Image>();
            rbwImg.color = new Color(0.06f, 0.06f, 0.09f, 0.88f);
            Outline rbwOutline = recycleBinWin.AddComponent<Outline>();
            rbwOutline.effectColor = new Color(1f, 1f, 1f, 0.12f);

            GameObject rbwHeader = CreateSubWindowHeader(recycleBinWin.transform, "Recycle Bin", () => compUI.CloseRecycleBin());

            GameObject rbwContent = new GameObject("Content");
            rbwContent.transform.SetParent(recycleBinWin.transform, false);
            RectTransform rbwcRect = rbwContent.AddComponent<RectTransform>();
            rbwcRect.anchorMin = Vector2.zero;
            rbwcRect.anchorMax = Vector2.one;
            rbwcRect.offsetMin = new Vector2(10, 10);
            rbwcRect.offsetMax = new Vector2(-10, -42);
            Image rbwcImg = rbwContent.AddComponent<Image>();
            rbwcImg.color = new Color(0.08f, 0.08f, 0.1f, 0.6f);
            Outline rbwcOutline = rbwContent.AddComponent<Outline>();
            rbwcOutline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            GridLayoutGroup rbwcGlg = rbwContent.AddComponent<GridLayoutGroup>();
            rbwcGlg.cellSize = new Vector2(90, 95);
            rbwcGlg.spacing = new Vector2(15, 15);
            rbwcGlg.padding = new RectOffset(15, 15, 15, 15);

            CreateRecycleBinFileItem(rbwContent.transform, "BinFile_CV", "PDF", "Minh_CV_v12.pdf", () => compUI.ClickRecycleBinFile("cv"));
            CreateRecycleBinFileItem(rbwContent.transform, "BinFile_Grad", "JPG", "Anh_Tot_Nghiep.jpg", () => compUI.ClickRecycleBinFile("graduation"));

            // --- GOOGLE CHROME WINDOW ---
            GameObject chromeWin = new GameObject("ChromeWindow");
            chromeWin.transform.parent = workspace.transform;
            RectTransform crwRect = chromeWin.AddComponent<RectTransform>();
            crwRect.anchorMin = new Vector2(0.5f, 0.5f);
            crwRect.anchorMax = new Vector2(0.5f, 0.5f);
            crwRect.sizeDelta = new Vector2(600, 420);
            crwRect.anchoredPosition = new Vector2(50, -20);
            Image crwImg = chromeWin.AddComponent<Image>();
            crwImg.color = new Color(0.06f, 0.06f, 0.09f, 0.88f);
            Outline crwOutline = chromeWin.AddComponent<Outline>();
            crwOutline.effectColor = new Color(1f, 1f, 1f, 0.12f);

            GameObject crwHeader = CreateSubWindowHeader(chromeWin.transform, "Google Chrome", () => compUI.CloseChrome());

            GameObject urlBar = new GameObject("UrlBar");
            urlBar.transform.SetParent(chromeWin.transform, false);
            RectTransform urlRect = urlBar.AddComponent<RectTransform>();
            urlRect.anchorMin = new Vector2(0f, 1f);
            urlRect.anchorMax = new Vector2(1f, 1f);
            urlRect.pivot = new Vector2(0.5f, 1f);
            urlRect.sizeDelta = new Vector2(0, 35);
            urlRect.anchoredPosition = new Vector2(0, -32);
            Image urlImg = urlBar.AddComponent<Image>();
            urlImg.color = new Color(0.16f, 0.16f, 0.18f, 1f);

            GameObject navObj = new GameObject("NavSymbols");
            navObj.transform.SetParent(urlBar.transform, false);
            RectTransform navRect = navObj.AddComponent<RectTransform>();
            navRect.anchorMin = new Vector2(0f, 0.5f);
            navRect.anchorMax = new Vector2(0f, 0.5f);
            navRect.pivot = new Vector2(0f, 0.5f);
            navRect.sizeDelta = new Vector2(80, 30);
            navRect.anchoredPosition = new Vector2(10, 0);
            TextMeshProUGUI navTxt = navObj.AddComponent<TextMeshProUGUI>();
            navTxt.text = "<b><  >  R</b>";
            navTxt.fontSize = 14;
            navTxt.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            navTxt.alignment = TextAlignmentOptions.Left;
            navTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            GameObject addrBox = new GameObject("AddressBox");
            addrBox.transform.SetParent(urlBar.transform, false);
            RectTransform addrRect = addrBox.AddComponent<RectTransform>();
            addrRect.anchorMin = new Vector2(0f, 0.5f);
            addrRect.anchorMax = new Vector2(1f, 0.5f);
            addrRect.pivot = new Vector2(0.5f, 0.5f);
            addrRect.sizeDelta = new Vector2(-120, 24);
            addrRect.anchoredPosition = new Vector2(40, 0);
            Image addrImg = addrBox.AddComponent<Image>();
            addrImg.color = new Color(0.2f, 0.2f, 0.22f, 1f);
            Outline addrOutline = addrBox.AddComponent<Outline>();
            addrOutline.effectColor = new Color(0.28f, 0.28f, 0.32f, 0.5f);

            GameObject addrText = new GameObject("Text");
            addrText.transform.SetParent(addrBox.transform, false);
            RectTransform addrtRect = addrText.AddComponent<RectTransform>();
            addrtRect.anchorMin = Vector2.zero;
            addrtRect.anchorMax = Vector2.one;
            addrtRect.sizeDelta = new Vector2(-20, 0);
            addrtRect.anchoredPosition = new Vector2(10, 0);
            TextMeshProUGUI addrtTxt = addrText.AddComponent<TextMeshProUGUI>();
            addrtTxt.text = "<color=#5cb85c>• Secure</color> | https://topcv.vn/tim-kiem?q=unity+fresher";
            addrtTxt.fontSize = 10;
            addrtTxt.color = new Color(0.85f, 0.85f, 0.85f, 1f);
            addrtTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            GameObject chromeContent = new GameObject("Content");
            chromeContent.transform.SetParent(chromeWin.transform, false);
            RectTransform crcRect = chromeContent.AddComponent<RectTransform>();
            crcRect.anchorMin = Vector2.zero;
            crcRect.anchorMax = Vector2.one;
            crcRect.offsetMin = new Vector2(10, 10);
            crcRect.offsetMax = new Vector2(-10, -77);
            Image crcImg = chromeContent.AddComponent<Image>();
            crcImg.color = new Color(0.06f, 0.06f, 0.08f, 0.7f);
            Outline crcOutline = chromeContent.AddComponent<Outline>();
            crcOutline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            GameObject chromeText = new GameObject("PageText");
            chromeText.transform.SetParent(chromeContent.transform, false);
            RectTransform crtRect = chromeText.AddComponent<RectTransform>();
            crtRect.anchorMin = Vector2.zero;
            crtRect.anchorMax = Vector2.one;
            crtRect.offsetMin = new Vector2(12, 10);
            crtRect.offsetMax = new Vector2(-12, -10);
            TextMeshProUGUI crtTxt = chromeText.AddComponent<TextMeshProUGUI>();
            crtTxt.text = "<size=11><color=#9aa0a6>Khoảng 124.000 kết quả (0,32 giây)</color></size><br><br>" +
                          "<color=#8ab4f8><size=13><b>1. Thực Tập Sinh Lập Trình Unity - JoyGame Studio</b></size></color><br>" +
                          "<color=#9aa0a6><size=10>https://joygamestudio.vn/tuyen-dung/intern-unity</size></color><br>" +
                          "• Yêu cầu: Có sản phẩm game tự làm, hiểu rõ C# và cấu trúc dữ liệu.<br>" +
                          "• Lương hỗ trợ: 1.000.000đ/tháng. Thử việc không lương 2 tháng.<br><br>" +
                          "<color=#8ab4f8><size=13><b>2. Lập Trình Viên Web (Fresher) - NovaTech Group</b></size></color><br>" +
                          "<color=#9aa0a6><size=10>https://novatech.com/careers/web-fresher</size></color><br>" +
                          "• Yêu cầu: Có tối thiểu 1 năm kinh nghiệm làm việc thực tế tại doanh nghiệp.<br>" +
                          "• Mức lương: 6.000.000đ - 8.000.000đ. Phỏng vấn kỹ thuật 3 vòng.<br><br>" +
                          "<color=#33cc66><size=13><b>3. [HOT] Việc Làm Nước Ngoài Lương Cao - Tuyển Kỹ Thuật Viên Máy Tính (Campuchia)</b></size></color><br>" +
                          "<color=#9aa0a6><size=10>https://vieclamnuocngoai.com/tuyen-dung/cambodia-tech</size></color><br>" +
                          "• Mức lương: 1.000.000đ - 1.500.000đ/ngày (Bao ăn ở toàn bộ, hỗ trợ xe đưa đón, không phí trung gian).<br>" +
                          "• Yêu cầu: Biết sử dụng máy tính cơ bản, gõ phím nhanh. Không yêu cầu bằng cấp hay kinh nghiệm.<br><br>" +
                          "<color=#ffcc00><i>Cảnh báo thị trường:</i></color> <i>Thị trường tuyển dụng IT năm 2026 đang có mức độ cạnh tranh rất cao. Cảnh giác với các tin tuyển dụng việc nhẹ lương cao ở nước ngoài, nạp tiền cọc làm nhiệm vụ.</i>";
            crtTxt.fontSize = 11;
            crtTxt.color = Color.white;
            crtTxt.lineSpacing = 4f;

            // 1. Job 1 Button (Invisible overlay on Job 1 Title)
            GameObject job1BtnObj = new GameObject("Job1Button");
            job1BtnObj.transform.SetParent(chromeContent.transform, false);
            RectTransform job1Rect = job1BtnObj.AddComponent<RectTransform>();
            job1Rect.anchorMin = new Vector2(0f, 0.83f);
            job1Rect.anchorMax = new Vector2(1f, 0.91f);
            job1Rect.offsetMin = new Vector2(12, 0);
            job1Rect.offsetMax = new Vector2(-12, 0);
            Image job1Img = job1BtnObj.AddComponent<Image>();
            job1Img.color = Color.clear;
            Button job1Btn = job1BtnObj.AddComponent<Button>();
            job1Btn.targetGraphic = job1Img;
            ColorBlock job1Cb = job1Btn.colors;
            job1Cb.normalColor = Color.clear;
            job1Cb.highlightedColor = new Color(1f, 1f, 1f, 0.05f);
            job1Cb.pressedColor = new Color(1f, 1f, 1f, 0.1f);
            job1Cb.selectedColor = Color.clear;
            job1Btn.colors = job1Cb;

            // 2. Job 2 Button (Invisible overlay on Job 2 Title)
            GameObject job2BtnObj = new GameObject("Job2Button");
            job2BtnObj.transform.SetParent(chromeContent.transform, false);
            RectTransform job2Rect = job2BtnObj.AddComponent<RectTransform>();
            job2Rect.anchorMin = new Vector2(0f, 0.66f);
            job2Rect.anchorMax = new Vector2(1f, 0.74f);
            job2Rect.offsetMin = new Vector2(12, 0);
            job2Rect.offsetMax = new Vector2(-12, 0);
            Image job2Img = job2BtnObj.AddComponent<Image>();
            job2Img.color = Color.clear;
            Button job2Btn = job2BtnObj.AddComponent<Button>();
            job2Btn.targetGraphic = job2Img;
            ColorBlock job2Cb = job2Btn.colors;
            job2Cb.normalColor = Color.clear;
            job2Cb.highlightedColor = new Color(1f, 1f, 1f, 0.05f);
            job2Cb.pressedColor = new Color(1f, 1f, 1f, 0.1f);
            job2Cb.selectedColor = Color.clear;
            job2Btn.colors = job2Cb;

            // 3. Scam Job Button (Invisible overlay on Scam Job Title)
            GameObject scamBtnObj = new GameObject("ScamJobButton");
            scamBtnObj.transform.SetParent(chromeContent.transform, false);
            RectTransform scamRect = scamBtnObj.AddComponent<RectTransform>();
            scamRect.anchorMin = new Vector2(0f, 0.32f); // Expanded down to cover the entire block (Title + Details)
            scamRect.anchorMax = new Vector2(1f, 0.58f);
            scamRect.offsetMin = new Vector2(12, 0);
            scamRect.offsetMax = new Vector2(-12, 0);
            Image scamImg = scamBtnObj.AddComponent<Image>();
            scamImg.color = Color.clear;
            Button scamBtn = scamBtnObj.AddComponent<Button>();
            scamBtn.targetGraphic = scamImg;
            ColorBlock scamCb = scamBtn.colors;
            scamCb.normalColor = Color.clear;
            scamCb.highlightedColor = new Color(1f, 1f, 1f, 0.05f);
            scamCb.pressedColor = new Color(1f, 1f, 1f, 0.1f);
            scamCb.selectedColor = Color.clear;
            scamBtn.colors = scamCb;

            // Chrome Back Button (used on detail pages, initially hidden)
            GameObject chromeBackBtnObj = new GameObject("ChromeBackButton");
            chromeBackBtnObj.transform.SetParent(chromeContent.transform, false);
            RectTransform cbRect = chromeBackBtnObj.AddComponent<RectTransform>();
            cbRect.anchorMin = new Vector2(0.78f, 0.04f); // Position at bottom right
            cbRect.anchorMax = new Vector2(0.98f, 0.12f);
            cbRect.anchoredPosition = Vector2.zero;
            cbRect.sizeDelta = Vector2.zero;

            Image cbImg = chromeBackBtnObj.AddComponent<Image>();
            cbImg.color = new Color(0.18f, 0.54f, 0.34f, 1f); // Green button
            Outline cbOutline = chromeBackBtnObj.AddComponent<Outline>();
            cbOutline.effectColor = new Color(1f, 1f, 1f, 0.15f);

            Button cbBtn = chromeBackBtnObj.AddComponent<Button>();
            cbBtn.targetGraphic = cbImg;

            // Highlight hover feedback
            ColorBlock cbBlock = cbBtn.colors;
            cbBlock.normalColor = new Color(0.18f, 0.54f, 0.34f, 1f);
            cbBlock.highlightedColor = new Color(0.24f, 0.65f, 0.42f, 1f);
            cbBlock.pressedColor = new Color(0.14f, 0.45f, 0.28f, 1f);
            cbBlock.selectedColor = new Color(0.18f, 0.54f, 0.34f, 1f);
            cbBtn.colors = cbBlock;

            GameObject cbTextObj = new GameObject("Text");
            cbTextObj.transform.SetParent(chromeBackBtnObj.transform, false);
            RectTransform cbtRect = cbTextObj.AddComponent<RectTransform>();
            cbtRect.anchorMin = Vector2.zero;
            cbtRect.anchorMax = Vector2.one;
            cbtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI cbtText = cbTextObj.AddComponent<TextMeshProUGUI>();
            cbtText.text = "← Quay lại";
            cbtText.fontSize = 10;
            cbtText.color = Color.white;
            cbtText.alignment = TextAlignmentOptions.Center;
            cbtText.verticalAlignment = VerticalAlignmentOptions.Middle;

            chromeBackBtnObj.SetActive(false); // Hidden by default

            // Chrome Save Contact Button (used on detail pages, initially hidden)
            GameObject chromeSaveContactBtnObj = new GameObject("SaveContactButton");
            chromeSaveContactBtnObj.transform.SetParent(chromeContent.transform, false);
            RectTransform scRect = chromeSaveContactBtnObj.AddComponent<RectTransform>();
            scRect.anchorMin = new Vector2(0.55f, 0.04f); // Position at bottom right, left of back button
            scRect.anchorMax = new Vector2(0.75f, 0.12f);
            scRect.anchoredPosition = Vector2.zero;
            scRect.sizeDelta = Vector2.zero;

            Image scImg = chromeSaveContactBtnObj.AddComponent<Image>();
            scImg.color = new Color(0.1f, 0.45f, 0.8f, 1f); // Blue button
            Outline scOutline = chromeSaveContactBtnObj.AddComponent<Outline>();
            scOutline.effectColor = new Color(1f, 1f, 1f, 0.15f);

            Button scBtn = chromeSaveContactBtnObj.AddComponent<Button>();
            scBtn.targetGraphic = scImg;

            // Highlight hover feedback
            ColorBlock scBlock = scBtn.colors;
            scBlock.normalColor = new Color(0.1f, 0.45f, 0.8f, 1f);
            scBlock.highlightedColor = new Color(0.15f, 0.55f, 0.9f, 1f);
            scBlock.pressedColor = new Color(0.08f, 0.35f, 0.7f, 1f);
            scBlock.selectedColor = new Color(0.1f, 0.45f, 0.8f, 1f);
            scBtn.colors = scBlock;

            GameObject scTextObj = new GameObject("Text");
            scTextObj.name = "Text"; // Critical to find and update label text
            scTextObj.transform.SetParent(chromeSaveContactBtnObj.transform, false);
            RectTransform sctRect = scTextObj.AddComponent<RectTransform>();
            sctRect.anchorMin = Vector2.zero;
            sctRect.anchorMax = Vector2.one;
            sctRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI sctText = scTextObj.AddComponent<TextMeshProUGUI>();
            sctText.text = "Lưu số điện thoại";
            sctText.fontSize = 10;
            sctText.color = Color.white;
            sctText.alignment = TextAlignmentOptions.Center;
            sctText.verticalAlignment = VerticalAlignmentOptions.Middle;

            chromeSaveContactBtnObj.SetActive(false); // Hidden by default

            // Set private fields on ComputerUI
            SetPrivateField(compUI, "computerPanel", compPanel);
            SetPrivateField(compUI, "mailWindow", mailWin);
            SetPrivateField(compUI, "billsWindow", billsWin);
            SetPrivateField(compUI, "thisPCWindow", thisPCWin);
            SetPrivateField(compUI, "recycleBinWindow", recycleBinWin);
            SetPrivateField(compUI, "chromeWindow", chromeWin);
            SetPrivateField(compUI, "emailBodyText", evBodyText);
            SetPrivateField(compUI, "email1Button", e1Btn);
            SetPrivateField(compUI, "email2Button", e2Btn);
            SetPrivateField(compUI, "email3Button", e3Btn);
            SetPrivateField(compUI, "email4Button", e4Btn);
        }

        private static GameObject CreateDesktopIconButton(Transform parent, string name, Sprite iconSprite, string labelText)
        {
            GameObject iconObj = new GameObject(name);
            iconObj.transform.parent = parent;
            RectTransform rTrans = iconObj.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(110, 80); // reduced from 100 to fit all 5 icons nicely without overlap

            Image img = iconObj.AddComponent<Image>();
            img.color = Color.clear; // Invisible background
            Button btn = iconObj.AddComponent<Button>();
            btn.targetGraphic = img;

            // Prevent keyboard selection outlines / navigation issues
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            ColorBlock cb = btn.colors;
            cb.normalColor = Color.clear;
            cb.highlightedColor = new Color(1f, 1f, 1f, 0.08f); // subtle hover selection tint
            cb.pressedColor = new Color(1f, 1f, 1f, 0.15f);
            cb.selectedColor = new Color(1f, 1f, 1f, 0.1f);
            cb.disabledColor = Color.clear;
            btn.colors = cb;

            // Icon Sprite Image (centered at top)
            GameObject iconImageObj = new GameObject("IconImage");
            iconImageObj.transform.parent = iconObj.transform;
            RectTransform iconRect = iconImageObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 1f);
            iconRect.anchorMax = new Vector2(0.5f, 1f);
            iconRect.pivot = new Vector2(0.5f, 1f);
            iconRect.anchoredPosition = new Vector2(0, -12); // padding from top of cell
            iconRect.sizeDelta = new Vector2(38, 38); // Standard pixel art size

            Image iconImg = iconImageObj.AddComponent<Image>();
            iconImg.sprite = iconSprite;
            iconImg.color = Color.white;
            iconImg.preserveAspect = true;

            // Label Text (small size, centered at bottom)
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.parent = iconObj.transform;
            RectTransform lblRect = labelObj.AddComponent<RectTransform>();
            lblRect.anchorMin = new Vector2(0f, 0f);
            lblRect.anchorMax = new Vector2(1f, 0.45f);
            lblRect.pivot = new Vector2(0.5f, 1f);
            lblRect.anchoredPosition = new Vector2(0, -5); // space below the icon
            lblRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI lblTxt = labelObj.AddComponent<TextMeshProUGUI>();
            lblTxt.text = labelText;
            lblTxt.fontSize = 11;
            lblTxt.color = Color.white;
            lblTxt.alignment = TextAlignmentOptions.Center;
            lblTxt.verticalAlignment = VerticalAlignmentOptions.Top;
            lblTxt.enableWordWrapping = true; // Auto wrap names

            // Add drop shadow outline for wallpaper contrast
            Outline outline = labelObj.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            outline.effectDistance = new Vector2(1.2f, 1.2f);

            return iconObj;
        }

        private static GameObject CreateSubWindowHeader(Transform parent, string titleText, UnityEngine.Events.UnityAction onClose)
        {
            GameObject header = new GameObject("HeaderBar");
            header.transform.SetParent(parent, false);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.sizeDelta = new Vector2(0, 32); // Slightly more compact
            headerRect.anchoredPosition = Vector2.zero;
            Image headerImg = header.AddComponent<Image>();
            headerImg.color = new Color(0.05f, 0.05f, 0.07f, 0.95f); // Dark acrylic header

            GameObject title = new GameObject("Title");
            title.transform.SetParent(header.transform, false);
            RectTransform tRect = title.AddComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0f, 0f);
            tRect.anchorMax = new Vector2(0.8f, 1f);
            tRect.sizeDelta = new Vector2(-10, 0);
            tRect.anchoredPosition = new Vector2(10, 0);
            TextMeshProUGUI titleTxt = title.AddComponent<TextMeshProUGUI>();
            titleTxt.text = titleText;
            titleTxt.fontSize = 12;
            titleTxt.color = Color.white;
            titleTxt.fontStyle = FontStyles.Bold;
            titleTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Close button [X] (Windows 11 hover style, styled red for clear visibility)
            GameObject closeObj = new GameObject("CloseButton");
            closeObj.transform.SetParent(header.transform, false);
            RectTransform cRect = closeObj.AddComponent<RectTransform>();
            cRect.anchorMin = new Vector2(1f, 0.5f);
            cRect.anchorMax = new Vector2(1f, 0.5f);
            cRect.pivot = new Vector2(1f, 0.5f);
            cRect.sizeDelta = new Vector2(40, 32); // Wider for better usability
            cRect.anchoredPosition = Vector2.zero; // Far right

            Image closeImg = closeObj.AddComponent<Image>();
            closeImg.color = Color.clear; // Transparent by default
            
            Button closeBtn = closeObj.AddComponent<Button>();
            closeBtn.onClick.AddListener(onClose);
            closeBtn.targetGraphic = closeImg;

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            closeBtn.navigation = nav;

            ColorBlock cb = closeBtn.colors;
            cb.normalColor = Color.clear;
            cb.highlightedColor = new Color(0.9f, 0.12f, 0.12f, 0.85f); // Brighter red hover
            cb.pressedColor = new Color(0.7f, 0.08f, 0.08f, 0.95f);
            cb.selectedColor = Color.clear;
            closeBtn.colors = cb;

            GameObject closeTextObj = new GameObject("Text");
            closeTextObj.transform.SetParent(closeObj.transform, false);
            RectTransform ctRect = closeTextObj.AddComponent<RectTransform>();
            ctRect.anchorMin = Vector2.zero;
            ctRect.anchorMax = Vector2.one;
            ctRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI ctTxt = closeTextObj.AddComponent<TextMeshProUGUI>();
            ctTxt.text = "X"; // standard close character
            ctTxt.fontSize = 11;
            ctTxt.fontStyle = FontStyles.Bold;
            ctTxt.color = Color.white;
            ctTxt.alignment = TextAlignmentOptions.Center;
            ctTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            return header;
        }

        private static GameObject CreateEmailButton(Transform parent, string name, string senderName)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.parent = parent;
            RectTransform rTrans = btnObj.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(150, 48);

            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.03f); // Modern card list item
            
            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            ColorBlock cb = btn.colors;
            cb.normalColor = new Color(1f, 1f, 1f, 0.03f);
            cb.highlightedColor = new Color(1f, 1f, 1f, 0.08f); // lighter on hover
            cb.pressedColor = new Color(1f, 1f, 1f, 0.12f);
            cb.selectedColor = new Color(0.0f, 0.48f, 0.8f, 0.25f); // accent blue when active
            btn.colors = cb;

            Outline outline = btnObj.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            // Vertical 4px unread indicator stripe on the left edge
            GameObject unreadBar = new GameObject("UnreadIndicator");
            unreadBar.transform.parent = btnObj.transform;
            RectTransform ubRect = unreadBar.AddComponent<RectTransform>();
            ubRect.anchorMin = new Vector2(0f, 0f);
            ubRect.anchorMax = new Vector2(0f, 1f);
            ubRect.pivot = new Vector2(0f, 0.5f);
            ubRect.sizeDelta = new Vector2(4, 0); // 4px wide, full height
            ubRect.anchoredPosition = Vector2.zero;
            Image ubImg = unreadBar.AddComponent<Image>();
            ubImg.color = new Color(0.0f, 0.48f, 0.8f, 0.85f); // Modern notification blue

            GameObject textObj = new GameObject("Text");
            textObj.transform.parent = btnObj.transform;
            RectTransform tRect = textObj.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.sizeDelta = new Vector2(-15, -5);
            tRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
            txt.text = $"<b>{senderName}</b><br><size=10><color=#cccccc>Thư từ chối...</color></size>";
            txt.fontSize = 11;
            txt.color = Color.white;
            txt.verticalAlignment = VerticalAlignmentOptions.Middle;

            return btnObj;
        }

        private static void CreatePhoneUI(GameObject uiRoot)
        {
            // Load custom Phone wallpaper
            string phoneWallpaperPath = "Assets/Sprites/UI/phone_wallpaper.png";
            ConfigureSpriteImportSettings(phoneWallpaperPath);
            Sprite phoneWallpaperSprite = AssetDatabase.LoadAssetAtPath<Sprite>(phoneWallpaperPath);

            // Load phone mask and frame overlay sprites
            string phoneMaskPath = "Assets/Sprites/UI/phone_mask.png";
            ConfigureSpriteImportSettings(phoneMaskPath);
            Sprite phoneMaskSprite = AssetDatabase.LoadAssetAtPath<Sprite>(phoneMaskPath);

            string phoneFramePath = "Assets/Sprites/UI/phone_frame.png";
            ConfigureSpriteImportSettings(phoneFramePath);
            Sprite phoneFrameSprite = AssetDatabase.LoadAssetAtPath<Sprite>(phoneFramePath);

            // Load phone app icon background sprites
            Sprite bgGreen = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/AppIconBg_Green.png");
            Sprite bgBlue = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/AppIconBg_Blue.png");
            Sprite bgOrange = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/AppIconBg_Orange.png");
            Sprite bgLight = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/AppIconBg_Light.png");
            Sprite bgCyan = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/AppIconBg_Cyan.png");
            Sprite bgGrey = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/AppIconBg_Grey.png");

            // 1. Root Phone UI GameObject
            GameObject phoneUIObj = new GameObject("PhoneUI");
            phoneUIObj.transform.parent = uiRoot.transform;

            RectTransform rootRect = phoneUIObj.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;
            rootRect.anchoredPosition = Vector2.zero;

            PhoneUI phoneUI = phoneUIObj.AddComponent<PhoneUI>();

            // Main phone screen overlay panel (transparent black behind the phone)
            GameObject phonePanel = new GameObject("PhonePanel");
            phonePanel.transform.parent = phoneUIObj.transform;
            RectTransform panelRect = phonePanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;

            Image panelImg = phonePanel.AddComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.6f); // Translucent black overlay

            // Close phone by clicking outside the phone body (ClickOutsideArea)
            GameObject clickOutsideObj = new GameObject("ClickOutsideArea");
            clickOutsideObj.transform.parent = phonePanel.transform;
            RectTransform coRect = clickOutsideObj.AddComponent<RectTransform>();
            coRect.anchorMin = Vector2.zero;
            coRect.anchorMax = Vector2.one;
            coRect.sizeDelta = Vector2.zero;
            coRect.anchoredPosition = Vector2.zero;

            Image coImg = clickOutsideObj.AddComponent<Image>();
            coImg.color = Color.clear;
            coImg.raycastTarget = true;

            Button coBtn = clickOutsideObj.AddComponent<Button>();
            coBtn.onClick.AddListener(() => phoneUI.ClosePhone());

            // Exit Hint Text (floating at the bottom of the screen)
            GameObject exitHint = new GameObject("ExitHint");
            exitHint.transform.parent = phonePanel.transform;
            RectTransform ehRect = exitHint.AddComponent<RectTransform>();
            ehRect.anchorMin = new Vector2(0.5f, 0f);
            ehRect.anchorMax = new Vector2(0.5f, 0f);
            ehRect.pivot = new Vector2(0.5f, 0f);
            ehRect.anchoredPosition = new Vector2(0, 30); // 30px from bottom of the screen
            ehRect.sizeDelta = new Vector2(350, 30);
            
            TextMeshProUGUI ehText = exitHint.AddComponent<TextMeshProUGUI>();
            ehText.text = "Nhấp chuột bên ngoài điện thoại hoặc nhấn ESC để thoát";
            ehText.fontSize = 12;
            ehText.color = new Color(1f, 1f, 1f, 0.7f);
            ehText.alignment = TextAlignmentOptions.Center;
            ehText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Phone Body (looks like a modern smartphone)
            GameObject phoneBody = new GameObject("PhoneBody");
            phoneBody.transform.parent = phonePanel.transform;
            RectTransform bodyRect = phoneBody.AddComponent<RectTransform>();
            bodyRect.anchorMin = new Vector2(0.5f, 0.5f);
            bodyRect.anchorMax = new Vector2(0.5f, 0.5f);
            bodyRect.sizeDelta = new Vector2(360, 640);
            bodyRect.anchoredPosition = Vector2.zero;

            Image bodyImg = phoneBody.AddComponent<Image>();
            // Use rounded-corner mask sprite so corners appear rounded
            if (phoneMaskSprite != null)
            {
                bodyImg.sprite = phoneMaskSprite;
                bodyImg.type = Image.Type.Simple;
            }
            bodyImg.color = new Color(0.06f, 0.06f, 0.08f, 1f); // Near-black modern phone body

            // Mask clips children to the rounded corner shape of the sprite
            Mask bodyMask = phoneBody.AddComponent<Mask>();
            bodyMask.showMaskGraphic = true; // Show the dark body color behind the masked children

            // Dedicated Close (✕) button at the top-right of the phone body
            GameObject phoneCloseBtnObj = new GameObject("PhoneCloseButton");
            phoneCloseBtnObj.transform.SetParent(phoneBody.transform, false);
            RectTransform pcbRect = phoneCloseBtnObj.AddComponent<RectTransform>();
            pcbRect.anchorMin = new Vector2(0.5f, 0.5f);
            pcbRect.anchorMax = new Vector2(0.5f, 0.5f);
            pcbRect.pivot = new Vector2(0.5f, 0.5f);
            pcbRect.sizeDelta = new Vector2(32, 32);
            pcbRect.anchoredPosition = new Vector2(200, 300); // floats on the right side near top, not above it to prevent clipping

            Image pcbImg = phoneCloseBtnObj.AddComponent<Image>();
            Sprite closeDotSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/NotificationDot.png");
            if (closeDotSprite != null)
            {
                pcbImg.sprite = closeDotSprite;
                pcbImg.color = new Color(0.9f, 0.2f, 0.2f, 0.9f); // iOS red
            }
            else
            {
                pcbImg.color = Color.red;
            }

            Button pcbBtn = phoneCloseBtnObj.AddComponent<Button>();
            pcbBtn.onClick.AddListener(() => phoneUI.ClosePhone());

            Outline pcbOutline = phoneCloseBtnObj.AddComponent<Outline>();
            pcbOutline.effectColor = Color.white;
            pcbOutline.effectDistance = new Vector2(1, 1);

            GameObject pcbTextObj = new GameObject("Text");
            pcbTextObj.transform.SetParent(phoneCloseBtnObj.transform, false);
            RectTransform pcbtRect = pcbTextObj.AddComponent<RectTransform>();
            pcbtRect.anchorMin = Vector2.zero;
            pcbtRect.anchorMax = Vector2.one;
            pcbtRect.sizeDelta = Vector2.zero;
            pcbtRect.anchoredPosition = Vector2.zero;

            TextMeshProUGUI pcbtText = pcbTextObj.AddComponent<TextMeshProUGUI>();
            pcbtText.text = "X";
            pcbtText.fontSize = 16;
            pcbtText.fontStyle = FontStyles.Bold;
            pcbtText.color = Color.white;
            pcbtText.alignment = TextAlignmentOptions.Center;
            pcbtText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Top Status Bar — pushed 10px below the top edge to stay clear of the rounded corners
            GameObject statusBar = new GameObject("StatusBar");
            statusBar.transform.parent = phoneBody.transform;
            RectTransform statusRect = statusBar.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0f, 1f);
            statusRect.anchorMax = new Vector2(1f, 1f);
            statusRect.pivot = new Vector2(0.5f, 1f);
            statusRect.sizeDelta = new Vector2(-32, 30); // narrow 16px each side to avoid left/right rounded corners
            statusRect.anchoredPosition = new Vector2(0, -10); // 10px below top = inside safe area

            Image statusImg = statusBar.AddComponent<Image>();
            statusImg.color = Color.clear; // Transparent iOS status bar

            // Status Bar Left: Carrier / Signal — push inward 16px from left edge (avoid rounded corner clipping)
            GameObject statusLeft = new GameObject("StatusLeft");
            statusLeft.transform.parent = statusBar.transform;
            RectTransform slRect = statusLeft.AddComponent<RectTransform>();
            slRect.anchorMin = new Vector2(0f, 0f);
            slRect.anchorMax = new Vector2(0.45f, 1f);
            slRect.pivot = new Vector2(0f, 0.5f);
            slRect.anchoredPosition = new Vector2(16, 0); // 16px from left — clear of rounded corner
            slRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI slText = statusLeft.AddComponent<TextMeshProUGUI>();
            slText.text = "Viettel  ▂▄▆";
            slText.fontSize = 10;
            slText.color = Color.white;
            slText.alignment = TextAlignmentOptions.Left;
            slText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Status Bar Center: Clock Time
            GameObject statusCenter = new GameObject("StatusCenter");
            statusCenter.transform.parent = statusBar.transform;
            RectTransform scRect = statusCenter.AddComponent<RectTransform>();
            scRect.anchorMin = new Vector2(0.35f, 0f);
            scRect.anchorMax = new Vector2(0.65f, 1f);
            scRect.pivot = new Vector2(0.5f, 0.5f);
            scRect.anchoredPosition = Vector2.zero;
            scRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI scText = statusCenter.AddComponent<TextMeshProUGUI>();
            scText.text = "16:02";
            scText.fontSize = 11;
            scText.fontStyle = FontStyles.Bold;
            scText.color = Color.white;
            scText.alignment = TextAlignmentOptions.Center;
            scText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Status Bar Right: Battery percentage + icon
            GameObject statusRight = new GameObject("StatusRight");
            statusRight.transform.parent = statusBar.transform;
            RectTransform srRect = statusRight.AddComponent<RectTransform>();
            srRect.anchorMin = new Vector2(0.55f, 0f);
            srRect.anchorMax = new Vector2(1f, 1f);
            srRect.pivot = new Vector2(1f, 0.5f);
            srRect.anchoredPosition = new Vector2(-16, 0); // 16px from right — clear of rounded corner
            srRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI srText = statusRight.AddComponent<TextMeshProUGUI>();
            srText.text = "84% <color=#aaffaa>▌</color>";
            srText.fontSize = 10;
            srText.color = Color.white;
            srText.alignment = TextAlignmentOptions.Right;
            srText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // --- HOME SCREEN ---
            // Starts at 38px from top (clear of rounded corner + status bar) and extends to bottom
            GameObject homeScreen = new GameObject("HomeScreen");
            homeScreen.transform.parent = phoneBody.transform;
            RectTransform homeRect = homeScreen.AddComponent<RectTransform>();
            homeRect.anchorMin = new Vector2(0f, 0f);
            homeRect.anchorMax = new Vector2(1f, 1f);
            homeRect.sizeDelta = new Vector2(0, -38); // leave 38px at top for status bar inside rounded corners
            homeRect.anchoredPosition = new Vector2(0, -19); // shift down 19px so top gap = 38px, bottom gap = 0px

            Image homeImg = homeScreen.AddComponent<Image>();
            if (phoneWallpaperSprite != null)
            {
                homeImg.sprite = phoneWallpaperSprite;
                homeImg.color = Color.white;
            }
            else
            {
                homeImg.color = new Color(0.12f, 0.12f, 0.15f, 1f);
            }

            // Grid for Apps Container (HomeScreen)
            // homeScreen height ≈ 610px (640 - 30 status bar).
            // Dock at bottom = 95px → dock occupies anchor y 0..0.156 of homeScreen.
            // Grid goes from just above dock to 30px below the top.
            GameObject appsGrid = new GameObject("AppsGrid");
            appsGrid.transform.parent = homeScreen.transform;
            RectTransform gridRect = appsGrid.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.02f, 0.16f); // 0.16 * 610 ≈ 98px above bottom = just above dock
            gridRect.anchorMax = new Vector2(0.98f, 0.96f); // top: leave ~24px for status bar shadow
            gridRect.sizeDelta = Vector2.zero; // fill the anchor area exactly
            gridRect.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup vlg = appsGrid.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8f;
            vlg.padding = new RectOffset(4, 4, 8, 4);
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = false;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;

            // Row 1
            GameObject row1Obj = new GameObject("Row1");
            row1Obj.transform.SetParent(appsGrid.transform, false);
            RectTransform r1Rt = row1Obj.AddComponent<RectTransform>();
            r1Rt.sizeDelta = new Vector2(310, 72);
            HorizontalLayoutGroup hlg1 = row1Obj.AddComponent<HorizontalLayoutGroup>();
            hlg1.spacing = 12f;
            hlg1.childAlignment = TextAnchor.MiddleCenter;
            hlg1.childControlWidth = false;
            hlg1.childControlHeight = false;

            // Row 2
            GameObject row2Obj = new GameObject("Row2");
            row2Obj.transform.SetParent(appsGrid.transform, false);
            RectTransform r2Rt = row2Obj.AddComponent<RectTransform>();
            r2Rt.sizeDelta = new Vector2(310, 72);
            HorizontalLayoutGroup hlg2 = row2Obj.AddComponent<HorizontalLayoutGroup>();
            hlg2.spacing = 12f;
            hlg2.childAlignment = TextAnchor.MiddleCenter;
            hlg2.childControlWidth = false;
            hlg2.childControlHeight = false;



            // Load phone app icon sprites
            Sprite notesIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/NotesIcon.png");
            Sprite photosIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/PhotosIcon.png");
            Sprite weatherIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/WeatherIcon.png");
            Sprite settingsIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/SettingsIcon.png");
            Sprite phoneIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Phone.png");
            Sprite messagesIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/MessagesIcon.png");
            Sprite browserIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/BrowserIcon.png");

            Sprite tiktokIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/TikTokIcon.png");
            Sprite facebookIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/FacebookIcon.png");
            Sprite mapsIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/MapsIcon.png");
            Sprite bankIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/BankIcon.png");
            Sprite finCreditIconSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/FinCreditIcon.png");

            // Row 1: Notes, Photos, Weather, Settings
            CreatePhoneAppIconButton(row1Obj.transform, "NotesAppButton", notesIconSprite, bgOrange, "Ghi Chú", () => phoneUI.OpenNotes());
            CreatePhoneAppIconButton(row1Obj.transform, "PhotosAppButton", photosIconSprite, bgLight, "Thư Viện", () => phoneUI.OpenPhotos());
            CreatePhoneAppIconButton(row1Obj.transform, "WeatherAppButton", weatherIconSprite, bgCyan, "Thời Tiết", () => phoneUI.OpenWeather());
            CreatePhoneAppIconButton(row1Obj.transform, "SettingsAppButton", settingsIconSprite, bgGrey, "Cài Đặt", () => phoneUI.OpenSettings());

            // Row 2: TikTok, Facebook, Maps, FinCredit
            CreatePhoneAppIconButton(row2Obj.transform, "TikTokAppButton", tiktokIconSprite, bgGrey, "TikTok", () => phoneUI.OpenTikTok());
            CreatePhoneAppIconButton(row2Obj.transform, "FacebookAppButton", facebookIconSprite, bgBlue, "Facebook", () => phoneUI.OpenFacebook());
            CreatePhoneAppIconButton(row2Obj.transform, "MapsAppButton", mapsIconSprite, bgGreen, "Bản Đồ", () => phoneUI.OpenMaps());
            CreatePhoneAppIconButton(row2Obj.transform, "FinCreditAppButton", finCreditIconSprite, bgOrange, "FinCredit", () => phoneUI.OpenFinCreditApp());




            // Page Indicator dots
            GameObject pageIndicator = new GameObject("PageIndicator");
            pageIndicator.transform.parent = homeScreen.transform;
            RectTransform piRect = pageIndicator.AddComponent<RectTransform>();
            piRect.anchorMin = new Vector2(0.5f, 0f);
            piRect.anchorMax = new Vector2(0.5f, 0f);
            piRect.pivot = new Vector2(0.5f, 0f);
            piRect.anchoredPosition = new Vector2(0, 115);
            piRect.sizeDelta = new Vector2(120, 15);

            TextMeshProUGUI piText = pageIndicator.AddComponent<TextMeshProUGUI>();
            piText.text = "<color=#ffffff33>•  </color>•<color=#ffffff33>  •  •</color>"; // 4 pages, 2nd page active
            piText.fontSize = 11;
            piText.alignment = TextAlignmentOptions.Center;
            piText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Bottom Smartphone Dock (holds Điện Thoại, Tin Nhắn, Trình Duyệt, MB Bank)
            GameObject dock = new GameObject("Dock");
            dock.transform.parent = homeScreen.transform;
            RectTransform dockRect = dock.AddComponent<RectTransform>();
            dockRect.anchorMin = new Vector2(0f, 0f);
            dockRect.anchorMax = new Vector2(1f, 0f);
            dockRect.pivot = new Vector2(0.5f, 0f);
            dockRect.sizeDelta = new Vector2(0, 95);
            dockRect.anchoredPosition = new Vector2(0, 0); // flush to bottom, no overlap with apps

            Image dockImg = dock.AddComponent<Image>();
            dockImg.color = new Color(1f, 1f, 1f, 0.15f); // frosted glass white
            Outline dockOutline = dock.AddComponent<Outline>();
            dockOutline.effectColor = new Color(1f, 1f, 1f, 0.12f); // glass reflection border
            dockOutline.effectDistance = new Vector2(1, 1);

            // Frosted glass top line separation
            GameObject dockTopLine = new GameObject("TopLine");
            dockTopLine.transform.parent = dock.transform;
            RectTransform lineRect = dockTopLine.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0f, 1f);
            lineRect.anchorMax = new Vector2(1f, 1f);
            lineRect.pivot = new Vector2(0.5f, 1f);
            lineRect.sizeDelta = new Vector2(0, 1);
            lineRect.anchoredPosition = Vector2.zero;
            Image lineImg = dockTopLine.AddComponent<Image>();
            lineImg.color = new Color(1f, 1f, 1f, 0.2f); // subtle white divider line

            // Inner container for dock icons to align perfectly with Row 1 / Row 2
            GameObject dockGrid = new GameObject("DockGrid");
            dockGrid.transform.SetParent(dock.transform, false);
            RectTransform dgRt = dockGrid.AddComponent<RectTransform>();
            dgRt.anchorMin = new Vector2(0.5f, 0.5f);
            dgRt.anchorMax = new Vector2(0.5f, 0.5f);
            dgRt.pivot = new Vector2(0.5f, 0.5f);
            dgRt.sizeDelta = new Vector2(310, 95);
            dgRt.anchoredPosition = Vector2.zero;

            HorizontalLayoutGroup dockHlg = dockGrid.AddComponent<HorizontalLayoutGroup>();
            dockHlg.padding = new RectOffset(0, 0, 5, 10);
            dockHlg.spacing = 12f;
            dockHlg.childAlignment = TextAnchor.MiddleCenter;
            dockHlg.childControlWidth = false;
            dockHlg.childControlHeight = false;

            // Dock Phone App Button
            CreatePhoneAppIconButton(dockGrid.transform, "PhoneAppButton", phoneIconSprite, bgGreen, "Điện Thoại", () => phoneUI.OpenPhoneApp());

            // Dock Messages App Button
            GameObject msgAppBtnObj = CreatePhoneAppIconButton(dockGrid.transform, "MessagesAppButton", messagesIconSprite, bgGreen, "Tin Nhắn", () => phoneUI.OpenMessagesApp());
            
            // Notification Dot for Messages App (placed nicely at top-right of the icon background)
            GameObject msgNotifyDot = CreateNotificationDot(msgAppBtnObj.transform, Vector2.zero);

            // Dock Browser App Button
            CreatePhoneAppIconButton(dockGrid.transform, "BrowserAppButton", browserIconSprite, bgBlue, "Trình Duyệt", () => phoneUI.OpenBrowser());

            // Dock MB Bank App Button
            CreatePhoneAppIconButton(dockGrid.transform, "BankAppButton", bankIconSprite, bgBlue, "MB Bank", () => phoneUI.OpenBankApp());

            // Home/Close Swipe Indicator bar at bottom of phone
            GameObject homeBtnObj = new GameObject("HomeBar");
            homeBtnObj.transform.parent = phoneBody.transform;
            RectTransform hbRect = homeBtnObj.AddComponent<RectTransform>();
            hbRect.anchorMin = new Vector2(0.5f, 0f);
            hbRect.anchorMax = new Vector2(0.5f, 0f);
            hbRect.pivot = new Vector2(0.5f, 0f);
            hbRect.sizeDelta = new Vector2(120, 5);
            hbRect.anchoredPosition = new Vector2(0, 5);

            Image hbImg = homeBtnObj.AddComponent<Image>();
            hbImg.color = new Color(0.8f, 0.8f, 0.8f, 0.8f); // modern white bar
            Button hbBtn = homeBtnObj.AddComponent<Button>();
            hbBtn.onClick.AddListener(() => phoneUI.ClosePhone());

            // --- MESSAGES LIST SCREEN ---
            GameObject msgListScreen = new GameObject("MessagesListScreen");
            msgListScreen.transform.parent = phoneBody.transform;
            RectTransform mlRect = msgListScreen.AddComponent<RectTransform>();
            mlRect.anchorMin = Vector2.zero;
            mlRect.anchorMax = Vector2.one;
            mlRect.sizeDelta = new Vector2(0, -30);
            mlRect.anchoredPosition = new Vector2(0, -15);

            Image mlImg = msgListScreen.AddComponent<Image>();
            mlImg.color = Color.black; // Pure black background

            // Back button at the top-left (instead of bar)
            GameObject mlBackObj = new GameObject("BackButton");
            mlBackObj.transform.SetParent(msgListScreen.transform, false);
            RectTransform mlbRect = mlBackObj.AddComponent<RectTransform>();
            mlbRect.anchorMin = new Vector2(0f, 1f);
            mlbRect.anchorMax = new Vector2(0f, 1f);
            mlbRect.pivot = new Vector2(0f, 1f);
            mlbRect.sizeDelta = new Vector2(30, 30);
            mlbRect.anchoredPosition = new Vector2(15, -15);

            Image mlbImg = mlBackObj.AddComponent<Image>();
            mlbImg.color = Color.clear;
            Button mlbBtn = mlBackObj.AddComponent<Button>();
            mlbBtn.onClick.AddListener(() => phoneUI.CloseMessagesApp());
            
            GameObject mlbTextObj = new GameObject("Text");
            mlbTextObj.transform.SetParent(mlBackObj.transform, false);
            RectTransform mlbtRect = mlbTextObj.AddComponent<RectTransform>();
            mlbtRect.anchorMin = Vector2.zero;
            mlbtRect.anchorMax = Vector2.one;
            mlbtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI mlbtTxt = mlbTextObj.AddComponent<TextMeshProUGUI>();
            mlbtTxt.text = "←";
            mlbtTxt.fontSize = 20;
            mlbtTxt.color = new Color(0.55f, 0.55f, 0.58f, 1f); // light grey back arrow
            mlbtTxt.alignment = TextAlignmentOptions.Center;
            mlbtTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Title "Messaging" (large, bold, left-aligned, below back button)
            GameObject mlTitleObj = new GameObject("Title");
            mlTitleObj.transform.parent = msgListScreen.transform;
            RectTransform mltRect = mlTitleObj.AddComponent<RectTransform>();
            mltRect.anchorMin = new Vector2(0f, 1f);
            mltRect.anchorMax = new Vector2(1f, 1f);
            mltRect.pivot = new Vector2(0.5f, 1f);
            mltRect.sizeDelta = new Vector2(-40, 40);
            mltRect.anchoredPosition = new Vector2(0, -45); // lower than back button

            TextMeshProUGUI mltTxt = mlTitleObj.AddComponent<TextMeshProUGUI>();
            mltTxt.text = "Tin nhắn";
            mltTxt.fontSize = 24;
            mltTxt.fontStyle = FontStyles.Bold;
            mltTxt.color = Color.white;
            mltTxt.alignment = TextAlignmentOptions.Left;
            mltTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Search Bar (rounded rectangle search box, below title)
            GameObject searchBarObj = new GameObject("SearchBar");
            searchBarObj.transform.parent = msgListScreen.transform;
            RectTransform sbRect = searchBarObj.AddComponent<RectTransform>();
            sbRect.anchorMin = new Vector2(0f, 1f);
            sbRect.anchorMax = new Vector2(1f, 1f);
            sbRect.pivot = new Vector2(0.5f, 1f);
            sbRect.sizeDelta = new Vector2(-40, 34); // left/right padding 20px
            sbRect.anchoredPosition = new Vector2(0, -90);

            Image sbImg = searchBarObj.AddComponent<Image>();
            sbImg.color = new Color(0.16f, 0.16f, 0.18f, 1f); // clean grey bg for search box
            Sprite roundedRectSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRect.png");
            if (roundedRectSprite != null)
            {
                sbImg.sprite = roundedRectSprite;
                sbImg.type = Image.Type.Sliced; // prevent corner deformation
            }

            GameObject searchTextObj = new GameObject("SearchText");
            searchTextObj.transform.parent = searchBarObj.transform;
            RectTransform stRect = searchTextObj.AddComponent<RectTransform>();
            stRect.anchorMin = Vector2.zero;
            stRect.anchorMax = Vector2.one;
            stRect.sizeDelta = new Vector2(-24, 0);
            stRect.anchoredPosition = new Vector2(5, 0);

            TextMeshProUGUI stTxt = searchTextObj.AddComponent<TextMeshProUGUI>();
            stTxt.text = "Tìm kiếm...";
            stTxt.fontSize = 13;
            stTxt.color = new Color(0.60f, 0.60f, 0.62f, 1f); // muted placeholder text
            stTxt.alignment = TextAlignmentOptions.Left;
            stTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Container for chats — starts below the search bar
            GameObject chatsContainer = new GameObject("ChatsContainer");
            chatsContainer.transform.parent = msgListScreen.transform;
            RectTransform ccRect = chatsContainer.AddComponent<RectTransform>();
            ccRect.anchorMin = new Vector2(0f, 1f);
            ccRect.anchorMax = new Vector2(1f, 1f);
            ccRect.pivot = new Vector2(0.5f, 1f);
            ccRect.sizeDelta = new Vector2(0, 440); // tall enough for all 6 chats
            ccRect.anchoredPosition = new Vector2(0, -135); // below search bar

            VerticalLayoutGroup ccVlg = chatsContainer.AddComponent<VerticalLayoutGroup>();
            ccVlg.spacing = 0f;
            ccVlg.padding = new RectOffset(0, 0, 0, 0);
            ccVlg.childAlignment = TextAnchor.UpperCenter;
            ccVlg.childControlWidth = true;
            ccVlg.childControlHeight = false;
            ccVlg.childForceExpandWidth = true;
            ccVlg.childForceExpandHeight = false;

            // Chat 1: Mom
            GameObject chat1Obj = CreateChatRowButton(chatsContainer.transform, "Chat_Mom", "MẸ <color=#ffffff><size=9><mark=#3a3a3c> 1 </mark></size></color>", "Minh ơi, tháng này dưới quê...", "15:30", () => phoneUI.OpenChat(1));
            GameObject momNotifyDot = CreateNotificationDot(chat1Obj.transform, Vector2.zero);
            AlignNotificationDotLeft(momNotifyDot);

            // Chat 2: Landlord/Bills
            GameObject chat2Obj = CreateChatRowButton(chatsContainer.transform, "Chat_Bills", "BAN QUẢN LÝ TRỌ <color=#ffffff><size=9><mark=#3a3a3c> 1 </mark></size></color>", "THÔNG BÁO HOÁ ĐƠN THÁNG 6...", "08:15", () => phoneUI.OpenChat(2));
            GameObject billsNotifyDot = CreateNotificationDot(chat2Obj.transform, Vector2.zero);
            AlignNotificationDotLeft(billsNotifyDot);

            // Chat 3: Close Friend (Thành)
            GameObject chat3Obj = CreateChatRowButton(chatsContainer.transform, "Chat_Thanh", "THÀNH <color=#ffffff><size=9><mark=#3a3a3c> 1 </mark></size></color>", "Mày ơi, khi nào trả tao tiền...", "15:30", () => phoneUI.OpenChat(3));
            GameObject thanhNotifyDot = CreateNotificationDot(chat3Obj.transform, Vector2.zero);
            AlignNotificationDotLeft(thanhNotifyDot);

            // Chat 4: Hùng (Đồng nghiệp cũ)
            GameObject chat4Obj = CreateChatRowButton(chatsContainer.transform, "Chat_Hung", "HÙNG <color=#ffffff><size=9><mark=#3a3a3c> 1 </mark></size></color>", "Xin lỗi mày nha Minh, dạo này...", "10:30", () => phoneUI.OpenChat(4));
            GameObject hungNotifyDot = CreateNotificationDot(chat4Obj.transform, Vector2.zero);
            AlignNotificationDotLeft(hungNotifyDot);

            // Chat 5: FinCredit (Nhắc nợ)
            GameObject chat5Obj = CreateChatRowButton(chatsContainer.transform, "Chat_FinCredit", "FINCREDIT <color=#ffffff><size=9><mark=#3a3a3c> 1 </mark></size></color>", "Yêu cầu thanh toán khoản vay quá...", "08:30", () => phoneUI.OpenChat(5));
            GameObject finCreditNotifyDot = CreateNotificationDot(chat5Obj.transform, Vector2.zero);
            AlignNotificationDotLeft(finCreditNotifyDot);

            // Chat 6: Số lạ — tin nhắn lừa đảo Campuchia
            GameObject chat6Obj = CreateChatRowButton(chatsContainer.transform, "Chat_ScamJob", "+84 091 XXX XXXX <color=#ffffff><size=9><mark=#3a3a3c> 1 </mark></size></color>", "Chào bạn! Mình đang tuyển gấp nhân viên...", "07:15", () => phoneUI.OpenChat(6));
            GameObject scamNotifyDot = CreateNotificationDot(chat6Obj.transform, Vector2.zero);
            AlignNotificationDotLeft(scamNotifyDot);


            // --- CHAT VIEW SCREEN ---
            GameObject chatViewScreenObj = new GameObject("ChatViewScreen");
            chatViewScreenObj.transform.parent = phoneBody.transform;
            RectTransform cvRect = chatViewScreenObj.AddComponent<RectTransform>();
            cvRect.anchorMin = Vector2.zero;
            cvRect.anchorMax = Vector2.one;
            cvRect.sizeDelta = new Vector2(0, -30);
            cvRect.anchoredPosition = new Vector2(0, -15);

            Image cvImg = chatViewScreenObj.AddComponent<Image>();
            cvImg.color = new Color(0.08f, 0.08f, 0.08f, 1f);

            // Title Bar
            GameObject cvTitleObj = new GameObject("TitleBar");
            cvTitleObj.transform.parent = chatViewScreenObj.transform;
            RectTransform cvtRect = cvTitleObj.AddComponent<RectTransform>();
            cvtRect.anchorMin = new Vector2(0f, 1f);
            cvtRect.anchorMax = new Vector2(1f, 1f);
            cvtRect.pivot = new Vector2(0.5f, 1f);
            cvtRect.sizeDelta = new Vector2(0, 40);
            cvtRect.anchoredPosition = Vector2.zero;
            Image cvtImg = cvTitleObj.AddComponent<Image>();
            cvtImg.color = new Color(0.12f, 0.12f, 0.14f, 1f);

            GameObject cvtTextObj = new GameObject("TitleText");
            cvtTextObj.transform.parent = cvTitleObj.transform;
            RectTransform cvttRect = cvtTextObj.AddComponent<RectTransform>();
            cvttRect.anchorMin = Vector2.zero;
            cvttRect.anchorMax = Vector2.one;
            cvttRect.sizeDelta = new Vector2(-120, 0);
            cvttRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI cvtTxt = cvtTextObj.AddComponent<TextMeshProUGUI>();
            cvtTxt.text = "Mẹ";
            cvtTxt.fontSize = 14;
            cvtTxt.fontStyle = FontStyles.Bold;
            cvtTxt.color = Color.white;
            cvtTxt.alignment = TextAlignmentOptions.Center;
            cvtTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Back button in chat view (to go back to messages list)
            GameObject cvBackObj = new GameObject("BackButton");
            cvBackObj.transform.SetParent(cvTitleObj.transform, false);
            RectTransform cvbRect = cvBackObj.AddComponent<RectTransform>();
            cvbRect.anchorMin = new Vector2(0f, 0.5f);
            cvbRect.anchorMax = new Vector2(0f, 0.5f);
            cvbRect.pivot = new Vector2(0f, 0.5f);
            cvbRect.sizeDelta = new Vector2(30, 30);
            cvbRect.anchoredPosition = new Vector2(10, 0);

            Image cvbImg = cvBackObj.AddComponent<Image>();
            cvbImg.color = Color.clear;
            Button cvbBtn = cvBackObj.AddComponent<Button>();
            cvbBtn.onClick.AddListener(() => phoneUI.CloseChat());

            GameObject cvbTextObj = new GameObject("Text");
            cvbTextObj.transform.SetParent(cvBackObj.transform, false);
            RectTransform cvbtRect = cvbTextObj.AddComponent<RectTransform>();
            cvbtRect.anchorMin = Vector2.zero;
            cvbtRect.anchorMax = Vector2.one;
            cvbtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI cvbtTxt = cvbTextObj.AddComponent<TextMeshProUGUI>();
            cvbtTxt.text = "←";
            cvbtTxt.fontSize = 20;
            cvbtTxt.color = new Color(0.55f, 0.55f, 0.58f, 1f);
            cvbtTxt.alignment = TextAlignmentOptions.Center;
            cvbtTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Chat content scroll view
            GameObject scrollViewObj = new GameObject("ChatScrollView");
            scrollViewObj.transform.SetParent(chatViewScreenObj.transform, false);
            RectTransform scrollRectTransform = scrollViewObj.AddComponent<RectTransform>();
            scrollRectTransform.anchorMin = Vector2.zero;
            scrollRectTransform.anchorMax = Vector2.one;
            scrollRectTransform.sizeDelta = new Vector2(-24, -60);
            scrollRectTransform.anchoredPosition = new Vector2(0, -25);

            ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            Image scrollImg = scrollViewObj.AddComponent<Image>();
            scrollImg.color = Color.clear;
            scrollViewObj.AddComponent<RectMask2D>();

            // Content container inside ScrollView
            GameObject chatContentObj = new GameObject("ChatContent");
            chatContentObj.transform.SetParent(scrollViewObj.transform, false);
            RectTransform chatContentRect = chatContentObj.AddComponent<RectTransform>();
            chatContentRect.anchorMin = new Vector2(0f, 1f); // top anchor
            chatContentRect.anchorMax = new Vector2(1f, 1f);
            chatContentRect.pivot = new Vector2(0.5f, 1f);
            chatContentRect.sizeDelta = Vector2.zero;
            chatContentRect.anchoredPosition = Vector2.zero;

            scrollRect.content = chatContentRect;

            ContentSizeFitter csf = chatContentObj.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            TextMeshProUGUI cvBodyTxt = chatContentObj.AddComponent<TextMeshProUGUI>();
            cvBodyTxt.text = "Tin nhắn...";
            cvBodyTxt.fontSize = 13;
            cvBodyTxt.color = Color.white;
            cvBodyTxt.verticalAlignment = VerticalAlignmentOptions.Top;

            // --- MOCK APP WINDOWS ---
            TextMeshProUGUI notesContent, photosContent, settingsContent, browserContent, weatherContent, phoneContent;
            TextMeshProUGUI tiktokContent, facebookContent, mapsContent, bankContent, finCreditContent;
            
            GameObject notesWin = CreatePhoneMockAppWindow(phoneBody.transform, "NotesWindow", "Ghi Chú", out notesContent, () => phoneUI.CloseNotes());
            GameObject photosWin = CreatePhoneMockAppWindow(phoneBody.transform, "PhotosWindow", "Thư Viện Ảnh", out photosContent, () => phoneUI.ClosePhotos());
            GameObject settingsWin = CreatePhoneMockAppWindow(phoneBody.transform, "SettingsWindow", "Cài Đặt Hệ Thống", out settingsContent, () => phoneUI.CloseSettings());
            GameObject browserWin = CreatePhoneMockAppWindow(phoneBody.transform, "BrowserWindow", "Trình Duyệt Chrome", out browserContent, () => phoneUI.CloseBrowser());
            GameObject weatherWin = CreatePhoneMockAppWindow(phoneBody.transform, "WeatherWindow", "Thời Tiết", out weatherContent, () => phoneUI.CloseWeather());
            GameObject phoneWin = CreatePhoneMockAppWindow(phoneBody.transform, "PhoneWindow", "Điện Thoại", out phoneContent, () => phoneUI.ClosePhoneApp());
            GameObject tiktokWin = CreatePhoneMockAppWindow(phoneBody.transform, "TikTokWindow", "TokTok", out tiktokContent, () => phoneUI.CloseTikTok());
            GameObject facebookWin = CreatePhoneMockAppWindow(phoneBody.transform, "FacebookWindow", "FaceNet", out facebookContent, () => phoneUI.CloseFacebook());
            GameObject mapsWin = CreatePhoneMockAppWindow(phoneBody.transform, "MapsWindow", "Bản đồ GoMaps", out mapsContent, () => phoneUI.CloseMaps());
            GameObject bankWin = CreatePhoneMockAppWindow(phoneBody.transform, "BankWindow", "MB Bank", out bankContent, () => phoneUI.CloseBankApp());
            GameObject finCreditWin = CreatePhoneMockAppWindow(phoneBody.transform, "FinCreditWindow", "FinCredit - Ví vay nợ", out finCreditContent, () => phoneUI.CloseFinCreditApp());

            // Tùy biến PhoneWindow thành giao diện danh bạ dạng nút bấm
            Transform phoneContentTrans = phoneWin.transform.Find("Content");
            GameObject gstarBtn = null;
            GameObject vinatechBtn = null;
            GameObject scamContactBtn = null;
            if (phoneContentTrans != null)
            {
                // Xóa TextMeshProUGUI thô
                var oldText = phoneContentTrans.GetComponent<TextMeshProUGUI>();
                if (oldText != null) Object.DestroyImmediate(oldText);

                // Thêm VerticalLayoutGroup để tự động căn chỉnh các dòng danh bạ dạng nút bấm
                VerticalLayoutGroup phoneVlg = phoneContentTrans.gameObject.AddComponent<VerticalLayoutGroup>();
                phoneVlg.padding = new RectOffset(10, 10, 10, 10);
                phoneVlg.spacing = 10f;
                phoneVlg.childAlignment = TextAnchor.UpperCenter;
                phoneVlg.childControlHeight = false;
                phoneVlg.childControlWidth = false;
                phoneVlg.childForceExpandHeight = false;
                phoneVlg.childForceExpandWidth = false;

                // Thêm tiêu đề danh bạ
                GameObject headerTextObj = new GameObject("TitleText");
                headerTextObj.transform.SetParent(phoneContentTrans, false);
                RectTransform htRect = headerTextObj.AddComponent<RectTransform>();
                htRect.sizeDelta = new Vector2(300, 24);
                TextMeshProUGUI htText = headerTextObj.AddComponent<TextMeshProUGUI>();
                htText.text = "<b><size=13><color=#5cb85c>DANH BẠ GẦN ĐÂY</color></size></b>";
                htText.alignment = TextAlignmentOptions.Left;

                // Tạo 4 liên hệ mặc định hiển thị đầy đủ số điện thoại không che xxx
                CreateContactButton(phoneContentTrans, "Contact_Mom", "Mẹ", "0912-999-987", "(Hôm qua - Cuộc gọi đi)", new Color(0.7f, 0.7f, 0.7f));
                CreateContactButton(phoneContentTrans, "Contact_Dad", "Bố", "0987-777-543", "(3 ngày trước - Cuộc gọi nhỡ)", new Color(1f, 0.3f, 0.3f));
                CreateContactButton(phoneContentTrans, "Contact_Thang", "Thắng (Bạn)", "0976-111-112", "(2 tuần trước)", new Color(0.7f, 0.7f, 0.7f));
                CreateContactButton(phoneContentTrans, "Contact_Hung", "Hùng (Bạn)", "0963-444-445", "(1 tháng trước)", new Color(0.7f, 0.7f, 0.7f));

                // Tạo 3 liên hệ tuyển dụng, mặc định ẩn, tự động hiện khi Minh xem tin tuyển dụng trên laptop
                gstarBtn = CreateContactButton(phoneContentTrans, "Contact_GStar", "JoyGame Studio (Tuyển dụng)", "028-3333-5555", "(Mới tìm thấy)", new Color(0.5f, 0.8f, 1f));
                gstarBtn.SetActive(false);

                vinatechBtn = CreateContactButton(phoneContentTrans, "Contact_VinaTech", "NovaTech Group (Tuyển dụng)", "024-9999-8888", "(Mới tìm thấy)", new Color(0.5f, 0.8f, 1f));
                vinatechBtn.SetActive(false);

                scamContactBtn = CreateContactButton(phoneContentTrans, "Contact_AnhHungScam", "Anh Hùng (Bavet Campuchia)", "098-765-4321", "(Mới tìm thấy)", new Color(0.3f, 0.8f, 0.4f));
                scamContactBtn.SetActive(false);
            }

            // Set private fields on PhoneUI
            SetPrivateField(phoneUI, "phonePanel", phonePanel);
            SetPrivateField(phoneUI, "homeScreen", homeScreen);
            SetPrivateField(phoneUI, "messagesListScreen", msgListScreen);
            SetPrivateField(phoneUI, "chatViewScreen", chatViewScreenObj);
            
            SetPrivateField(phoneUI, "notesWindow", notesWin);
            SetPrivateField(phoneUI, "photosWindow", photosWin);
            SetPrivateField(phoneUI, "settingsWindow", settingsWin);
            SetPrivateField(phoneUI, "browserWindow", browserWin);
            SetPrivateField(phoneUI, "weatherWindow", weatherWin);
            SetPrivateField(phoneUI, "phoneWindow", phoneWin);
            SetPrivateField(phoneUI, "tiktokWindow", tiktokWin);
            SetPrivateField(phoneUI, "facebookWindow", facebookWin);
            SetPrivateField(phoneUI, "mapsWindow", mapsWin);
            SetPrivateField(phoneUI, "bankWindow", bankWin);
            SetPrivateField(phoneUI, "finCreditWindow", finCreditWin);

            // Set custom app icon sprites
            SetPrivateField(phoneUI, "tiktokIcon", tiktokIconSprite);
            SetPrivateField(phoneUI, "facebookIcon", facebookIconSprite);
            SetPrivateField(phoneUI, "mapsIcon", mapsIconSprite);
            SetPrivateField(phoneUI, "bankIcon", bankIconSprite);
            SetPrivateField(phoneUI, "finCreditIcon", finCreditIconSprite);

            // Gán các nút liên hệ tuyển dụng vào PhoneUI để ẩn hiện
            SetPrivateField(phoneUI, "contactGStar", gstarBtn);
            SetPrivateField(phoneUI, "contactVinaTech", vinatechBtn);
            SetPrivateField(phoneUI, "contactAnhHungScam", scamContactBtn);

            SetPrivateField(phoneUI, "chatTitleText", cvtTxt);
            SetPrivateField(phoneUI, "chatBodyText", cvBodyTxt);
            SetPrivateField(phoneUI, "momNotificationDot", momNotifyDot);
            SetPrivateField(phoneUI, "billsNotificationDot", billsNotifyDot);
            SetPrivateField(phoneUI, "thanhNotificationDot", thanhNotifyDot);
            SetPrivateField(phoneUI, "hungNotificationDot", hungNotifyDot);
            SetPrivateField(phoneUI, "finCreditNotificationDot", finCreditNotifyDot);
            SetPrivateField(phoneUI, "scamNotificationDot", scamNotifyDot);
            SetPrivateField(phoneUI, "messagesAppNotificationDot", msgNotifyDot);

            SetPrivateField(phoneUI, "notesContentText", notesContent);
            SetPrivateField(phoneUI, "photosContentText", photosContent);
            SetPrivateField(phoneUI, "settingsContentText", settingsContent);
            SetPrivateField(phoneUI, "browserContentText", browserContent);
            SetPrivateField(phoneUI, "weatherContentText", weatherContent);
            SetPrivateField(phoneUI, "phoneContentText", phoneContent);
            SetPrivateField(phoneUI, "tiktokContentText", tiktokContent);
            SetPrivateField(phoneUI, "facebookContentText", facebookContent);
            SetPrivateField(phoneUI, "mapsContentText", mapsContent);
            SetPrivateField(phoneUI, "bankContentText", bankContent);
            SetPrivateField(phoneUI, "finCreditContentText", finCreditContent);

            // Create Phone Frame Overlay (drawn on top of all screens inside phoneBody)
            GameObject frameOverlay = new GameObject("PhoneFrameOverlay");
            frameOverlay.transform.SetParent(phoneBody.transform, false);
            RectTransform frameRect = frameOverlay.AddComponent<RectTransform>();
            frameRect.anchorMin = Vector2.zero;
            frameRect.anchorMax = Vector2.one;
            frameRect.sizeDelta = Vector2.zero;
            frameRect.anchoredPosition = Vector2.zero;

            Image frameImg = frameOverlay.AddComponent<Image>();
            if (phoneFrameSprite != null)
            {
                frameImg.sprite = phoneFrameSprite;
                frameImg.color = Color.white;
            }
            else
            {
                frameImg.color = Color.clear;
            }
            frameImg.raycastTarget = false; // Let clicks pass through to elements underneath!
        }

        private static GameObject CreatePhoneMockAppWindow(Transform parent, string name, string title, out TextMeshProUGUI contentText, UnityEngine.Events.UnityAction onClose)
        {
            GameObject win = new GameObject(name);
            win.transform.SetParent(parent, false);
            RectTransform winRect = win.AddComponent<RectTransform>();
            winRect.anchorMin = Vector2.zero;
            winRect.anchorMax = Vector2.one;
            winRect.sizeDelta = new Vector2(0, -30); // below status bar
            winRect.anchoredPosition = new Vector2(0, -15);

            Image img = win.AddComponent<Image>();
            img.color = new Color(0.08f, 0.08f, 0.09f, 1f); // modern dark theme

            // Title bar
            GameObject titleBar = new GameObject("TitleBar");
            titleBar.transform.SetParent(win.transform, false);
            RectTransform tbRect = titleBar.AddComponent<RectTransform>();
            tbRect.anchorMin = new Vector2(0f, 1f);
            tbRect.anchorMax = new Vector2(1f, 1f);
            tbRect.pivot = new Vector2(0.5f, 1f);
            tbRect.sizeDelta = new Vector2(0, 40);
            tbRect.anchoredPosition = Vector2.zero;
            Image tbImg = titleBar.AddComponent<Image>();
            tbImg.color = new Color(0.12f, 0.12f, 0.14f, 1f);

            GameObject titleTextObj = new GameObject("TitleText");
            titleTextObj.transform.SetParent(titleBar.transform, false);
            RectTransform ttRect = titleTextObj.AddComponent<RectTransform>();
            ttRect.anchorMin = Vector2.zero;
            ttRect.anchorMax = Vector2.one;
            ttRect.sizeDelta = new Vector2(-120, 0);
            ttRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI titleTxt = titleTextObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = title;
            titleTxt.fontSize = 15;
            titleTxt.fontStyle = FontStyles.Bold;
            titleTxt.color = Color.white;
            titleTxt.alignment = TextAlignmentOptions.Center;
            titleTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Back button
            GameObject backBtnObj = new GameObject("BackButton");
            backBtnObj.transform.SetParent(titleBar.transform, false);
            RectTransform backRect = backBtnObj.AddComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 0.5f);
            backRect.anchorMax = new Vector2(0f, 0.5f);
            backRect.pivot = new Vector2(0f, 0.5f);
            backRect.sizeDelta = new Vector2(30, 30);
            backRect.anchoredPosition = new Vector2(10, 0);

            Image backImg = backBtnObj.AddComponent<Image>();
            backImg.color = Color.clear;
            Button backBtn = backBtnObj.AddComponent<Button>();
            backBtn.onClick.AddListener(onClose);

            GameObject backTextObj = new GameObject("Text");
            backTextObj.transform.SetParent(backBtnObj.transform, false);
            RectTransform btRect = backTextObj.AddComponent<RectTransform>();
            btRect.anchorMin = Vector2.zero;
            btRect.anchorMax = Vector2.one;
            btRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI btTxt = backTextObj.AddComponent<TextMeshProUGUI>();
            btTxt.text = "←";
            btTxt.fontSize = 20;
            btTxt.color = new Color(0.55f, 0.55f, 0.58f, 1f);
            btTxt.alignment = TextAlignmentOptions.Center;
            btTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Content Area
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(win.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.sizeDelta = new Vector2(-24, -60);
            contentRect.anchoredPosition = new Vector2(0, -25);

            contentText = contentObj.AddComponent<TextMeshProUGUI>();
            contentText.text = "Nội dung...";
            contentText.fontSize = 14;
            contentText.color = Color.white;
            contentText.verticalAlignment = VerticalAlignmentOptions.Top;

            return win;
        }

        private static GameObject CreatePhoneAppIconButton(Transform parent, string name, Sprite iconSprite, Sprite bgSprite, string label, UnityEngine.Events.UnityAction onClick)
        {
            // 1. Container button object
            GameObject btnObj = new GameObject(name);
            btnObj.transform.parent = parent;
            RectTransform rTrans = btnObj.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(60, 80);

            // Invisible background for the button to make the entire slot clickable
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = Color.clear;

            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            // 2. Icon Background Image (the rounded square gradient)
            GameObject iconBgObj = new GameObject("IconBackground");
            iconBgObj.transform.parent = btnObj.transform;
            RectTransform bgRect = iconBgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 1f);
            bgRect.anchorMax = new Vector2(0.5f, 1f);
            bgRect.pivot = new Vector2(0.5f, 1f);
            bgRect.anchoredPosition = Vector2.zero;
            bgRect.sizeDelta = new Vector2(60, 60);

            Image bgImg = iconBgObj.AddComponent<Image>();
            bgImg.sprite = bgSprite;
            bgImg.color = Color.white;
            bgImg.type = Image.Type.Simple;

            // Add subtle outline for contrast
            Outline bgOutline = iconBgObj.AddComponent<Outline>();
            bgOutline.effectColor = new Color(1f, 1f, 1f, 0.15f);
            bgOutline.effectDistance = new Vector2(1, 1);

            // 3. Foreground Icon Glyph (centered inside the icon background)
            GameObject glyphObj = new GameObject("IconGlyph");
            glyphObj.transform.parent = iconBgObj.transform;
            RectTransform glyphRect = glyphObj.AddComponent<RectTransform>();
            glyphRect.anchorMin = new Vector2(0.5f, 0.5f);
            glyphRect.anchorMax = new Vector2(0.5f, 0.5f);
            glyphRect.pivot = new Vector2(0.5f, 0.5f);
            glyphRect.anchoredPosition = Vector2.zero;
            glyphRect.sizeDelta = new Vector2(32, 32);

            Image glyphImg = glyphObj.AddComponent<Image>();
            glyphImg.sprite = iconSprite;
            glyphImg.color = Color.white;
            glyphImg.preserveAspect = true;

            // 4. Label text (below the icon)
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.parent = btnObj.transform;
            RectTransform lblRect = labelObj.AddComponent<RectTransform>();
            lblRect.anchorMin = new Vector2(-0.1f, 0f);
            lblRect.anchorMax = new Vector2(1.1f, 0.25f);
            lblRect.pivot = new Vector2(0.5f, 0f);
            lblRect.anchoredPosition = Vector2.zero;
            lblRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI lblTxt = labelObj.AddComponent<TextMeshProUGUI>();
            lblTxt.text = label;
            lblTxt.fontSize = 11;
            lblTxt.color = Color.white;
            lblTxt.alignment = TextAlignmentOptions.Center;
            lblTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            Outline lblOutline = labelObj.AddComponent<Outline>();
            lblOutline.effectColor = new Color(0f, 0f, 0f, 0.6f);
            lblOutline.effectDistance = new Vector2(1, 1);

            return btnObj;
        }

        private static GameObject CreateNotificationDot(Transform parent, Vector2 pos)
        {
            GameObject dotObj = new GameObject("NotificationDot");
            dotObj.transform.parent = parent;
            RectTransform rTrans = dotObj.AddComponent<RectTransform>();
            rTrans.anchorMin = new Vector2(1f, 0.5f); // right-center anchor
            rTrans.anchorMax = new Vector2(1f, 0.5f);
            rTrans.pivot = new Vector2(1f, 0.5f);
            rTrans.sizeDelta = new Vector2(10, 10); // slightly smaller, clean
            rTrans.anchoredPosition = pos;

            Image img = dotObj.AddComponent<Image>();
            Sprite dotSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/NotificationDot.png");
            if (dotSprite != null)
            {
                img.sprite = dotSprite;
                img.color = Color.white;
            }
            else
            {
                img.color = Color.red;
            }
            return dotObj;
        }

        private static void AlignNotificationDotLeft(GameObject dotObj)
        {
            if (dotObj == null) return;
            // Remove the Image component to prevent rendering solid square if sprite configuration has lag
            Image img = dotObj.GetComponent<Image>();
            if (img != null) UnityEngine.Object.DestroyImmediate(img);

            RectTransform rTrans = dotObj.GetComponent<RectTransform>();
            if (rTrans != null)
            {
                rTrans.anchorMin = new Vector2(0f, 0.5f);
                rTrans.anchorMax = new Vector2(0f, 0.5f);
                rTrans.pivot = new Vector2(0f, 0.5f);
                rTrans.sizeDelta = new Vector2(12, 12);
                rTrans.anchoredPosition = new Vector2(10, 0); // 10px from the left edge of chat row
            }

            // Render a clean grey circular dot using TextMeshPro (guarantees perfect circle and exact grey color)
            TextMeshProUGUI txt = dotObj.GetComponent<TextMeshProUGUI>();
            if (txt == null) txt = dotObj.AddComponent<TextMeshProUGUI>();
            txt.text = "●";
            txt.fontSize = 11;
            txt.color = new Color(0.55f, 0.55f, 0.58f, 1f); // Muted grey
            txt.alignment = TextAlignmentOptions.Center;
            txt.verticalAlignment = VerticalAlignmentOptions.Middle;
        }

        private static GameObject CreateChatRowButton(Transform parent, string name, string sender, string preview, string timeStr, UnityEngine.Events.UnityAction onClick)
        {
            // --- Row container (72px tall) ---
            GameObject btnObj = new GameObject(name);
            btnObj.transform.parent = parent;
            RectTransform rTrans = btnObj.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(0, 72);

            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f); // transparent background, matches screen bg
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            ColorBlock cb = btn.colors;
            cb.normalColor = new Color(0f, 0f, 0f, 0f);
            cb.highlightedColor = new Color(0.11f, 0.11f, 0.13f, 1f); // subtle highlight
            cb.pressedColor = new Color(0.18f, 0.18f, 0.20f, 1f);
            cb.selectedColor = cb.normalColor;
            cb.fadeDuration = 0.1f;
            btn.colors = cb;

            // --- Avatar circle (40x40, centered vertically, 28px from left to make room for red dot) ---
            GameObject avatarObj = new GameObject("Avatar");
            avatarObj.transform.SetParent(btnObj.transform, false);
            RectTransform avatarRect = avatarObj.AddComponent<RectTransform>();
            avatarRect.anchorMin = new Vector2(0f, 0.5f);
            avatarRect.anchorMax = new Vector2(0f, 0.5f);
            avatarRect.pivot = new Vector2(0f, 0.5f);
            avatarRect.sizeDelta = new Vector2(40, 40);
            avatarRect.anchoredPosition = new Vector2(28, 0);
            Image avatarImg = avatarObj.AddComponent<Image>();
            avatarImg.color = new Color(0.24f, 0.24f, 0.27f, 1f); // modern clean grey background

            // Use the rounded rectangle sprite to make avatar square with rounded corners
            Sprite roundedRectSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRect.png");
            if (roundedRectSprite != null)
            {
                avatarImg.sprite = roundedRectSprite;
                avatarImg.type = Image.Type.Sliced;
            }

            // Procedurally draw person silhouette to avoid font-missing-glyph issue (🔲)
            GameObject personIcon = new GameObject("PersonIcon");
            personIcon.transform.SetParent(avatarObj.transform, false);
            RectTransform holderRt = personIcon.AddComponent<RectTransform>();
            holderRt.anchorMin = Vector2.zero;
            holderRt.anchorMax = Vector2.one;
            holderRt.sizeDelta = Vector2.zero;

            // Head (circle)
            GameObject headObj = new GameObject("Head");
            headObj.transform.SetParent(personIcon.transform, false);
            RectTransform headRt = headObj.AddComponent<RectTransform>();
            headRt.anchorMin = new Vector2(0.5f, 0.5f);
            headRt.anchorMax = new Vector2(0.5f, 0.5f);
            headRt.pivot = new Vector2(0.5f, 0.5f);
            headRt.sizeDelta = new Vector2(10, 10);
            headRt.anchoredPosition = new Vector2(0, 5);
            Image headImg = headObj.AddComponent<Image>();
            headImg.color = new Color(0.60f, 0.60f, 0.62f, 1f); // light grey
            Sprite dotSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/NotificationDot.png");
            if (dotSprite != null) headImg.sprite = dotSprite;

            // Body/Shoulders (stretched rounded pill)
            GameObject bodyObj = new GameObject("Body");
            bodyObj.transform.SetParent(personIcon.transform, false);
            RectTransform bodyRt = bodyObj.AddComponent<RectTransform>();
            bodyRt.anchorMin = new Vector2(0.5f, 0.5f);
            bodyRt.anchorMax = new Vector2(0.5f, 0.5f);
            bodyRt.pivot = new Vector2(0.5f, 0.5f);
            bodyRt.sizeDelta = new Vector2(22, 10);
            bodyRt.anchoredPosition = new Vector2(0, -6);
            Image bodyImg = bodyObj.AddComponent<Image>();
            bodyImg.color = new Color(0.60f, 0.60f, 0.62f, 1f); // light grey
            if (dotSprite != null) bodyImg.sprite = dotSprite;

            // --- Text block (sender + preview) anchored to avatar's right ---
            GameObject textObj = new GameObject("TextBlock");
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform tRect = textObj.AddComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0f, 0f);
            tRect.anchorMax = new Vector2(1f, 1f);
            // left edge = 28 (margin) + 40 (avatar) + 12 (gap) = 80
            // right edge = -115 to leave plenty of room for time + delete button
            tRect.offsetMin = new Vector2(80, 0);
            tRect.offsetMax = new Vector2(-115, 0);

            // Sender name (bold, white, top half)
            GameObject senderObj = new GameObject("SenderName");
            senderObj.transform.SetParent(textObj.transform, false);
            RectTransform sRect = senderObj.AddComponent<RectTransform>();
            sRect.anchorMin = new Vector2(0f, 0.5f);
            sRect.anchorMax = new Vector2(1f, 1f);
            sRect.sizeDelta = Vector2.zero;
            sRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI senderTxt = senderObj.AddComponent<TextMeshProUGUI>();
            senderTxt.text = sender;
            senderTxt.fontSize = 14;
            senderTxt.fontStyle = FontStyles.Bold;
            senderTxt.color = Color.white;
            senderTxt.alignment = TextAlignmentOptions.Left;
            senderTxt.verticalAlignment = VerticalAlignmentOptions.Bottom;
            senderTxt.enableWordWrapping = false;
            senderTxt.overflowMode = TextOverflowModes.Ellipsis;

            // Preview text (gray, bottom half)
            GameObject previewObj = new GameObject("PreviewText");
            previewObj.transform.SetParent(textObj.transform, false);
            RectTransform pRect = previewObj.AddComponent<RectTransform>();
            pRect.anchorMin = new Vector2(0f, 0f);
            pRect.anchorMax = new Vector2(1f, 0.5f);
            pRect.sizeDelta = Vector2.zero;
            pRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI previewTxt = previewObj.AddComponent<TextMeshProUGUI>();
            previewTxt.text = preview;
            previewTxt.fontSize = 13;
            previewTxt.color = new Color(0.70f, 0.70f, 0.73f, 1f); // lighter gray for better readability
            previewTxt.alignment = TextAlignmentOptions.Left;
            previewTxt.verticalAlignment = VerticalAlignmentOptions.Top;
            previewTxt.enableWordWrapping = false;
            previewTxt.overflowMode = TextOverflowModes.Ellipsis;

            // --- Bottom separator line (1px, full width) ---
            GameObject sepObj = new GameObject("Separator");
            sepObj.transform.SetParent(btnObj.transform, false);
            RectTransform sepRect = sepObj.AddComponent<RectTransform>();
            sepRect.anchorMin = new Vector2(0f, 0f);
            sepRect.anchorMax = new Vector2(1f, 0f);
            sepRect.pivot = new Vector2(0.5f, 0f);
            sepRect.offsetMin = new Vector2(80, 0); // indent from avatar
            sepRect.offsetMax = new Vector2(0, 1);  // 1px tall
            Image sepImg = sepObj.AddComponent<Image>();
            sepImg.color = new Color(1f, 1f, 1f, 0.05f); // very subtle separator

            // --- Time text (Top-right side) ---
            GameObject timeObj = new GameObject("TimeText");
            timeObj.transform.SetParent(btnObj.transform, false);
            RectTransform timeRect = timeObj.AddComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(1f, 1f); // fixed top-right anchor, no stretch
            timeRect.anchorMax = new Vector2(1f, 1f);
            timeRect.pivot = new Vector2(1f, 1f);
            timeRect.sizeDelta = new Vector2(80, 20); // wider and fixed height
            timeRect.anchoredPosition = new Vector2(-15, -12); // 12px from the top edge
            TextMeshProUGUI timeTxt = timeObj.AddComponent<TextMeshProUGUI>();
            timeTxt.text = timeStr;
            timeTxt.fontSize = 11;
            timeTxt.color = new Color(0.65f, 0.65f, 0.68f, 1f); // slightly brighter gray
            timeTxt.alignment = TextAlignmentOptions.Right;
            timeTxt.verticalAlignment = VerticalAlignmentOptions.Top;
            timeTxt.enableWordWrapping = false;
            timeTxt.overflowMode = TextOverflowModes.Ellipsis;

            // --- Delete button (Bottom-right side, small grey pill with grey-white text) ---
            GameObject delBtnObj = new GameObject("DeleteButton");
            delBtnObj.transform.SetParent(btnObj.transform, false);
            RectTransform delRect = delBtnObj.AddComponent<RectTransform>();
            delRect.anchorMin = new Vector2(1f, 0f); // fixed bottom-right anchor, no stretch
            delRect.anchorMax = new Vector2(1f, 0f);
            delRect.pivot = new Vector2(1f, 0f);
            delRect.sizeDelta = new Vector2(46, 22); // slightly wider, fixed height
            delRect.anchoredPosition = new Vector2(-15, 12); // 12px from the bottom edge (creates 12px gap with time text)

            Image delImg = delBtnObj.AddComponent<Image>();
            delImg.color = new Color(0.24f, 0.24f, 0.27f, 1f); // modern clean grey
            if (roundedRectSprite != null)
            {
                delImg.sprite = roundedRectSprite;
                delImg.type = Image.Type.Sliced;
            }
            
            Button delBtn = delBtnObj.AddComponent<Button>();
            ColorBlock delCb = delBtn.colors;
            delCb.normalColor = new Color(0.24f, 0.24f, 0.27f, 1f);
            delCb.highlightedColor = new Color(0.30f, 0.30f, 0.33f, 1f);
            delCb.pressedColor = new Color(0.36f, 0.36f, 0.40f, 1f);
            delCb.selectedColor = delCb.normalColor;
            delBtn.colors = delCb;

            // Text label
            GameObject delTextObj = new GameObject("Label");
            delTextObj.transform.SetParent(delBtnObj.transform, false);
            RectTransform dtRect = delTextObj.AddComponent<RectTransform>();
            dtRect.anchorMin = Vector2.zero;
            dtRect.anchorMax = Vector2.one;
            dtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI delTxt = delTextObj.AddComponent<TextMeshProUGUI>();
            delTxt.text = "Xoá";
            delTxt.fontSize = 11;
            delTxt.fontStyle = FontStyles.Bold;
            delTxt.color = new Color(0.90f, 0.90f, 0.92f, 1f); // clean off-white
            delTxt.alignment = TextAlignmentOptions.Center;
            delTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            return btnObj;
        }

        private static GameObject CreateTaskbarShortcut(Transform parent, string name, Sprite iconSprite, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.parent = parent;
            RectTransform rTrans = btnObj.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(32, 32);

            Image img = btnObj.AddComponent<Image>();
            img.color = Color.clear; // Invisible background
            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            // Icon Sprite Image
            GameObject iconImageObj = new GameObject("IconImage");
            iconImageObj.transform.parent = btnObj.transform;
            RectTransform iconRect = iconImageObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.sizeDelta = new Vector2(24, 24); // smaller for taskbar

            Image iconImg = iconImageObj.AddComponent<Image>();
            iconImg.sprite = iconSprite;
            iconImg.color = Color.white;
            iconImg.preserveAspect = true;

            return btnObj;
        }

        private static GameObject CreateRecycleBinFileItem(Transform parent, string name, string iconChar, string labelText, UnityEngine.Events.UnityAction onClick)
        {
            GameObject fileObj = new GameObject(name);
            fileObj.transform.parent = parent;
            RectTransform rTrans = fileObj.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(90, 95);

            Image img = fileObj.AddComponent<Image>();
            img.color = Color.clear; // transparent button
            Button btn = fileObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            // Icon (e.g. 📄 or 🖼️)
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.parent = fileObj.transform;
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0.35f);
            iconRect.anchorMax = new Vector2(1f, 1f);
            iconRect.sizeDelta = Vector2.zero;
            iconRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI iconTxt = iconObj.AddComponent<TextMeshProUGUI>();
            iconTxt.text = iconChar;
            iconTxt.fontSize = 18; // Smaller for "PDF" or "JPG" words
            iconTxt.fontStyle = FontStyles.Bold;
            iconTxt.color = new Color(0.7f, 0.7f, 0.7f, 1f); // sleek light grey
            iconTxt.alignment = TextAlignmentOptions.Center;
            iconTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.parent = fileObj.transform;
            RectTransform lblRect = labelObj.AddComponent<RectTransform>();
            lblRect.anchorMin = new Vector2(0f, 0f);
            lblRect.anchorMax = new Vector2(1f, 0.35f);
            lblRect.sizeDelta = Vector2.zero;
            lblRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI lblTxt = labelObj.AddComponent<TextMeshProUGUI>();
            lblTxt.text = labelText;
            lblTxt.fontSize = 9;
            lblTxt.color = Color.white;
            lblTxt.alignment = TextAlignmentOptions.Center;
            lblTxt.verticalAlignment = VerticalAlignmentOptions.Top;

            Outline outline = labelObj.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1, 1);

            return fileObj;
        }

        private static void SetupPlayerAnimator(UnityEditor.Animations.AnimatorController controller)
        {
            var rootStateMachine = controller.layers[0].stateMachine;

            // Clear existing states to rebuild cleanly
            var states = rootStateMachine.states;
            for (int i = 0; i < states.Length; i++)
            {
                rootStateMachine.RemoveState(states[i].state);
            }

            // Ensure directory for player animations exists
            if (!Directory.Exists("Assets/Animations/Player"))
            {
                Directory.CreateDirectory("Assets/Animations/Player");
            }

            // Load sprites
            Sprite idleDown = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Idle_Down.png");
            Sprite idleUp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Idle_Up.png");
            Sprite idleLeft = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Idle_Left.png");
            Sprite idleRight = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Idle_Right.png");

            Sprite walkDown1 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Down_1.png");
            Sprite walkDown2 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Down_2.png");
            Sprite walkUp1 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Up_1.png");
            Sprite walkUp2 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Up_2.png");
            Sprite walkLeft1 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Left_1.png");
            Sprite walkLeft2 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Left_2.png");
            Sprite walkRight1 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Right_1.png");
            Sprite walkRight2 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Minh/Minh_Walk_Right_2.png");

            // Create clips
            AnimationClip clipIdleDown = CreateSpriteClip("Assets/Animations/Player/Minh_Idle_Down.anim", new Sprite[] { idleDown }, 1, true);
            AnimationClip clipIdleUp = CreateSpriteClip("Assets/Animations/Player/Minh_Idle_Up.anim", new Sprite[] { idleUp }, 1, true);
            AnimationClip clipIdleLeft = CreateSpriteClip("Assets/Animations/Player/Minh_Idle_Left.anim", new Sprite[] { idleLeft }, 1, true);
            AnimationClip clipIdleRight = CreateSpriteClip("Assets/Animations/Player/Minh_Idle_Right.anim", new Sprite[] { idleRight }, 1, true);

            AnimationClip clipWalkDown = CreateSpriteClip("Assets/Animations/Player/Minh_Walk_Down.anim", new Sprite[] { walkDown1, idleDown, walkDown2, idleDown }, 6, true);
            AnimationClip clipWalkUp = CreateSpriteClip("Assets/Animations/Player/Minh_Walk_Up.anim", new Sprite[] { walkUp1, idleUp, walkUp2, idleUp }, 6, true);
            AnimationClip clipWalkLeft = CreateSpriteClip("Assets/Animations/Player/Minh_Walk_Left.anim", new Sprite[] { walkLeft1, idleLeft, walkLeft2, idleLeft }, 6, true);
            AnimationClip clipWalkRight = CreateSpriteClip("Assets/Animations/Player/Minh_Walk_Right.anim", new Sprite[] { walkRight1, idleRight, walkRight2, idleRight }, 6, true);

            // Create Idle Blend Tree State
            UnityEditor.Animations.BlendTree idleTree;
            var idleState = controller.CreateBlendTreeInController("Idle", out idleTree, 0);
            idleTree.name = "IdleBlendTree";
            idleTree.blendType = UnityEditor.Animations.BlendTreeType.SimpleDirectional2D;
            idleTree.blendParameter = "LastMoveX";
            idleTree.blendParameterY = "LastMoveY";
            idleTree.AddChild(clipIdleDown, new Vector2(0, -1));
            idleTree.AddChild(clipIdleUp, new Vector2(0, 1));
            idleTree.AddChild(clipIdleLeft, new Vector2(-1, 0));
            idleTree.AddChild(clipIdleRight, new Vector2(1, 0));

            // Create Walk Blend Tree State
            UnityEditor.Animations.BlendTree walkTree;
            var walkState = controller.CreateBlendTreeInController("Walk", out walkTree, 0);
            walkTree.name = "WalkBlendTree";
            walkTree.blendType = UnityEditor.Animations.BlendTreeType.SimpleDirectional2D;
            walkTree.blendParameter = "MoveX";
            walkTree.blendParameterY = "MoveY";
            walkTree.AddChild(clipWalkDown, new Vector2(0, -1));
            walkTree.AddChild(clipWalkUp, new Vector2(0, 1));
            walkTree.AddChild(clipWalkLeft, new Vector2(-1, 0));
            walkTree.AddChild(clipWalkRight, new Vector2(1, 0));

            // Set Transitions
            var idleToWalk = idleState.AddTransition(walkState);
            idleToWalk.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "IsMoving");
            idleToWalk.hasExitTime = false;
            idleToWalk.duration = 0f;

            var walkToIdle = walkState.AddTransition(idleState);
            walkToIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "IsMoving");
            walkToIdle.hasExitTime = false;
            walkToIdle.duration = 0f;

            // Make sure Idle is default state
            rootStateMachine.defaultState = idleState;
        }

        private static AnimationClip CreateSpriteClip(string clipPath, Sprite[] sprites, float frameRate, bool loop)
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = frameRate;

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            EditorCurveBinding spriteBinding = new EditorCurveBinding();
            spriteBinding.type = typeof(SpriteRenderer);
            spriteBinding.path = "";
            spriteBinding.propertyName = "m_Sprite";

            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe();
                keyframes[i].time = i * (1.0f / frameRate);
                keyframes[i].value = sprites[i];
            }

            AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);
            AssetDatabase.CreateAsset(clip, clipPath);
            return clip;
        }

        private static GameObject CreateContactButton(Transform parent, string name, string contactName, string phoneNumber, string statusText, Color statusColor)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rTrans = btnObj.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(310, 52);

            Sprite roundedRectSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRect.png");

            Image img = btnObj.AddComponent<Image>();
            if (roundedRectSprite != null)
            {
                img.sprite = roundedRectSprite;
                img.type = Image.Type.Sliced;
            }
            img.color = new Color(0.12f, 0.12f, 0.15f, 1f); // Dark background for contact item
            Outline outline = btnObj.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.05f);

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = new Color(0.12f, 0.12f, 0.15f, 1f);
            cb.highlightedColor = new Color(0.18f, 0.18f, 0.22f, 1f); // lighter on hover
            cb.pressedColor = new Color(0.08f, 0.08f, 0.10f, 1f);
            cb.selectedColor = new Color(0.12f, 0.12f, 0.15f, 1f);
            btn.colors = cb;

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            // Name and Phone Text (Top line)
            GameObject nameTextObj = new GameObject("NameText");
            nameTextObj.transform.SetParent(btnObj.transform, false);
            RectTransform ntRect = nameTextObj.AddComponent<RectTransform>();
            ntRect.anchorMin = new Vector2(0f, 0.5f);
            ntRect.anchorMax = new Vector2(1f, 1f);
            ntRect.offsetMin = new Vector2(12, 0);
            ntRect.offsetMax = new Vector2(-12, -4);
            TextMeshProUGUI ntText = nameTextObj.AddComponent<TextMeshProUGUI>();
            ntText.text = $"<b>{contactName}</b>: {phoneNumber}";
            ntText.fontSize = 13;
            ntText.color = Color.white;
            ntText.alignment = TextAlignmentOptions.Left;
            ntText.verticalAlignment = VerticalAlignmentOptions.Bottom;

            // Status Text (Bottom line)
            GameObject statusTextObj = new GameObject("StatusText");
            statusTextObj.transform.SetParent(btnObj.transform, false);
            RectTransform stRect = statusTextObj.AddComponent<RectTransform>();
            stRect.anchorMin = new Vector2(0f, 0f);
            stRect.anchorMax = new Vector2(1f, 0.5f);
            stRect.offsetMin = new Vector2(12, 4);
            stRect.offsetMax = new Vector2(-12, 0);
            TextMeshProUGUI stText = statusTextObj.AddComponent<TextMeshProUGUI>();
            stText.text = statusText;
            stText.fontSize = 11;
            stText.color = statusColor;
            stText.alignment = TextAlignmentOptions.Left;
            stText.verticalAlignment = VerticalAlignmentOptions.Top;

            return btnObj;
        }
    }
}
#endif
