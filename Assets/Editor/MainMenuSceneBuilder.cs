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
using EscapeFromHell.UI;

namespace EscapeFromHell.Editor
{
    public class MainMenuSceneBuilder : EditorWindow
    {
        [MenuItem("Escape From Hell/Build Main Menu Scene")]
        public static void BuildMainMenuScene()
        {
            Debug.Log("Building Main Menu Scene...");

            // Ensure sprites exist
            if (!File.Exists("Assets/Sprites/UI/WinLogo.png"))
            {
                Debug.LogWarning("Sprites not found. Generating sprites...");
                SpriteGenerator.GenerateAll();
                AssetDatabase.Refresh();
            }

            // Create new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            newScene.name = "MainMenu";

            // Configure Camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.position = new Vector3(0, 0, -10);
                mainCam.backgroundColor = new Color(0.05f, 0.05f, 0.08f, 1f);
                mainCam.clearFlags = CameraClearFlags.SolidColor;
            }

            // ── CANVAS ──────────────────────────────────────────────────────────────
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // ── BACKGROUND ──────────────────────────────────────────────────────────
            GameObject bgObj = CreateUIImage(canvasObj.transform, "Background",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(1920, 1080));
            Image bgImg = bgObj.GetComponent<Image>();
            // Dark gradient-like background using a dark color
            bgImg.color = new Color(0.04f, 0.04f, 0.08f, 1f);

            // Add subtle top gradient overlay
            GameObject gradientTop = CreateUIImage(bgObj.transform, "GradientTop",
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0, -200), new Vector2(0, 400));
            RectTransform gtRect = gradientTop.GetComponent<RectTransform>();
            gtRect.anchorMin = new Vector2(0f, 1f);
            gtRect.anchorMax = new Vector2(1f, 1f);
            gtRect.offsetMin = new Vector2(0, -350);
            gtRect.offsetMax = new Vector2(0, 0);
            Image gradTopImg = gradientTop.GetComponent<Image>();
            gradTopImg.color = new Color(0.6f, 0.1f, 0.05f, 0.18f); // Subtle red glow at top

            // ── FADE OVERLAY (for scene transitions) ────────────────────────────────
            GameObject fadeObj = CreateUIImage(canvasObj.transform, "FadeOverlay",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform fadeRect = fadeObj.GetComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.offsetMin = Vector2.zero;
            fadeRect.offsetMax = Vector2.zero;
            Image fadeImg = fadeObj.GetComponent<Image>();
            fadeImg.color = Color.black;
            CanvasGroup fadeCG = fadeObj.AddComponent<CanvasGroup>();
            fadeCG.alpha = 1f;
            fadeCG.blocksRaycasts = false;
            fadeCG.interactable = false;

            // ── DECORATIVE VERTICAL LINE (left accent) ───────────────────────────────
            GameObject leftLine = CreateUIImage(canvasObj.transform, "LeftAccentLine",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-420, 0), new Vector2(2, 600));
            leftLine.GetComponent<Image>().color = new Color(0.85f, 0.15f, 0.1f, 0.5f);

            // ── MAIN PANEL ───────────────────────────────────────────────────────────
            GameObject mainPanel = new GameObject("MainPanel");
            mainPanel.transform.SetParent(canvasObj.transform, false);
            RectTransform mainRect = mainPanel.AddComponent<RectTransform>();
            mainRect.anchorMin = new Vector2(0.5f, 0.5f);
            mainRect.anchorMax = new Vector2(0.5f, 0.5f);
            mainRect.pivot = new Vector2(0.5f, 0.5f);
            mainRect.anchoredPosition = Vector2.zero;
            mainRect.sizeDelta = new Vector2(800, 700);

            // ── TITLE ────────────────────────────────────────────────────────────────
            GameObject titleObj = new GameObject("Title_ThoatKhoiDiaNguc");
            titleObj.transform.SetParent(mainPanel.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -40);
            titleRect.sizeDelta = new Vector2(800, 130);
            TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "THOÁT KHỎI\nĐỊA NGỤC";
            titleTMP.fontSize = 68;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = new Color(0.95f, 0.85f, 0.75f, 1f); // Warm ivory

            // Sub-title / tagline
            GameObject subtitleObj = new GameObject("Subtitle");
            subtitleObj.transform.SetParent(mainPanel.transform, false);
            RectTransform subRect = subtitleObj.AddComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0.5f, 1f);
            subRect.anchorMax = new Vector2(0.5f, 1f);
            subRect.pivot = new Vector2(0.5f, 1f);
            subRect.anchoredPosition = new Vector2(0, -185);
            subRect.sizeDelta = new Vector2(700, 40);
            TextMeshProUGUI subtitleTMP = subtitleObj.AddComponent<TextMeshProUGUI>();
            subtitleTMP.text = "— Một câu chuyện về bẫy việc làm —";
            subtitleTMP.fontSize = 22;
            subtitleTMP.fontStyle = FontStyles.Italic;
            subtitleTMP.alignment = TextAlignmentOptions.Center;
            subtitleTMP.color = new Color(0.7f, 0.5f, 0.4f, 0.85f);

            // Divider line under title
            GameObject divider = CreateUIImage(mainPanel.transform, "TitleDivider",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -230), new Vector2(380, 2));
            divider.GetComponent<Image>().color = new Color(0.75f, 0.2f, 0.15f, 0.8f);

            // ── BUTTON CONTAINER ─────────────────────────────────────────────────────
            GameObject btnContainer = new GameObject("ButtonContainer");
            btnContainer.transform.SetParent(mainPanel.transform, false);
            RectTransform btnContRect = btnContainer.AddComponent<RectTransform>();
            btnContRect.anchorMin = new Vector2(0.5f, 1f);
            btnContRect.anchorMax = new Vector2(0.5f, 1f);
            btnContRect.pivot = new Vector2(0.5f, 1f);
            btnContRect.anchoredPosition = new Vector2(0, -265);
            btnContRect.sizeDelta = new Vector2(400, 300);

            VerticalLayoutGroup vlg = btnContainer.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 22f;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;

            // ── CREATE BUTTONS ───────────────────────────────────────────────────────
            Button newGameBtn = CreateMenuButton(btnContainer.transform, "NewGameButton",
                "CHƠI MỚI", new Color(0.85f, 0.18f, 0.12f, 1f), new Color(1f, 0.3f, 0.2f, 1f));

            Button continueBtn = CreateMenuButton(btnContainer.transform, "ContinueButton",
                "CHƠI TIẾP", new Color(0.15f, 0.35f, 0.55f, 1f), new Color(0.2f, 0.5f, 0.75f, 1f));

            Button settingsBtn = CreateMenuButton(btnContainer.transform, "SettingsButton",
                "CÀI ĐẶT", new Color(0.18f, 0.18f, 0.22f, 1f), new Color(0.3f, 0.3f, 0.38f, 1f));

            // ── CONFIRM NEW GAME PANEL ────────────────────────────────────────────────
            GameObject confirmPanel = CreateModalPanel(canvasObj.transform, "ConfirmNewGamePanel",
                "Bắt đầu lại từ đầu?\nDữ liệu đã lưu sẽ bị xóa.",
                out Button confirmYes, out Button confirmNo);
            confirmPanel.SetActive(false);

            // ── SETTINGS PANEL ────────────────────────────────────────────────────────
            GameObject settingsPanel = CreateSettingsPanel(canvasObj.transform, out Button settingsClose,
                out Slider musicSlider, out Slider sfxSlider);
            settingsPanel.SetActive(false);

            // ── VERSION TEXT ─────────────────────────────────────────────────────────
            GameObject versionObj = new GameObject("VersionText");
            versionObj.transform.SetParent(canvasObj.transform, false);
            RectTransform verRect = versionObj.AddComponent<RectTransform>();
            verRect.anchorMin = new Vector2(1f, 0f);
            verRect.anchorMax = new Vector2(1f, 0f);
            verRect.pivot = new Vector2(1f, 0f);
            verRect.anchoredPosition = new Vector2(-20, 15);
            verRect.sizeDelta = new Vector2(200, 30);
            TextMeshProUGUI verTMP = versionObj.AddComponent<TextMeshProUGUI>();
            verTMP.text = "v0.1.0 — Demo";
            verTMP.fontSize = 14;
            verTMP.alignment = TextAlignmentOptions.Right;
            verTMP.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);

            // ── WIRE UP MAIN MENU CONTROLLER ──────────────────────────────────────────
            MainMenuController controller = canvasObj.AddComponent<MainMenuController>();
            SetPrivateField(controller, "mainPanel", mainPanel);
            SetPrivateField(controller, "confirmNewGamePanel", confirmPanel);
            SetPrivateField(controller, "newGameButton", newGameBtn);
            SetPrivateField(controller, "continueButton", continueBtn);
            SetPrivateField(controller, "settingsButton", settingsBtn);
            SetPrivateField(controller, "confirmYesButton", confirmYes);
            SetPrivateField(controller, "confirmNoButton", confirmNo);
            SetPrivateField(controller, "settingsPanel", settingsPanel);
            SetPrivateField(controller, "settingsCloseButton", settingsClose);
            SetPrivateField(controller, "musicSlider", musicSlider);
            SetPrivateField(controller, "sfxSlider", sfxSlider);
            SetPrivateField(controller, "fadeCanvasGroup", fadeCG);
            SetPrivateField(controller, "chapter1SceneName", "Chapter1_Room");

            // ── EVENT SYSTEM ──────────────────────────────────────────────────────────
            if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // ── SAVE SCENE ────────────────────────────────────────────────────────────
            if (!Directory.Exists("Assets/Scenes"))
                Directory.CreateDirectory("Assets/Scenes");

            EditorSceneManager.SaveScene(newScene, "Assets/Scenes/MainMenu.unity");
            Debug.Log("Main Menu Scene saved to Assets/Scenes/MainMenu.unity");

            // Add to Build Settings as scene index 0
            AddMainMenuToTopOfBuildSettings("Assets/Scenes/MainMenu.unity");
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HELPER: Create a styled menu button
        // ─────────────────────────────────────────────────────────────────────────────
        private static Button CreateMenuButton(Transform parent, string name,
            string label, Color normalColor, Color hoverColor)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(360, 68);

            // Background image
            Image img = btnObj.AddComponent<Image>();
            img.color = normalColor;

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = normalColor;
            cb.highlightedColor = hoverColor;
            cb.pressedColor = new Color(hoverColor.r * 0.7f, hoverColor.g * 0.7f, hoverColor.b * 0.7f, 1f);
            cb.selectedColor = normalColor;
            cb.fadeDuration = 0.12f;
            btn.colors = cb;
            btn.targetGraphic = img;

            // Left accent bar
            GameObject accent = new GameObject("LeftAccent");
            accent.transform.SetParent(btnObj.transform, false);
            RectTransform accentRect = accent.AddComponent<RectTransform>();
            accentRect.anchorMin = new Vector2(0f, 0f);
            accentRect.anchorMax = new Vector2(0f, 1f);
            accentRect.offsetMin = new Vector2(0, 0);
            accentRect.offsetMax = new Vector2(5, 0);
            Image accentImg = accent.AddComponent<Image>();
            accentImg.color = new Color(1f, 0.85f, 0.7f, 0.6f);

            // Label
            GameObject textObj = new GameObject("Label");
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20, 0);
            textRect.offsetMax = Vector2.zero;
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 26;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = new Color(0.97f, 0.93f, 0.88f, 1f);

            return btn;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HELPER: Create a modal confirmation panel
        // ─────────────────────────────────────────────────────────────────────────────
        private static GameObject CreateModalPanel(Transform parent, string name,
            string message, out Button yesBtn, out Button noBtn)
        {
            // Dim overlay
            GameObject overlay = CreateUIImage(parent, name,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform overlayRect = overlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlay.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.72f);

            // Dialog box
            GameObject dialog = new GameObject("DialogBox");
            dialog.transform.SetParent(overlay.transform, false);
            RectTransform dialogRect = dialog.AddComponent<RectTransform>();
            dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
            dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
            dialogRect.pivot = new Vector2(0.5f, 0.5f);
            dialogRect.anchoredPosition = Vector2.zero;
            dialogRect.sizeDelta = new Vector2(520, 260);
            Image dialogImg = dialog.AddComponent<Image>();
            dialogImg.color = new Color(0.08f, 0.07f, 0.1f, 1f);

            // Border accent
            GameObject border = CreateUIImage(dialog.transform, "Border",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(-2, -2);
            borderRect.offsetMax = new Vector2(2, 2);
            border.GetComponent<Image>().color = new Color(0.75f, 0.18f, 0.12f, 0.8f);
            border.transform.SetAsFirstSibling();

            // Message text
            GameObject msgObj = new GameObject("Message");
            msgObj.transform.SetParent(dialog.transform, false);
            RectTransform msgRect = msgObj.AddComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0.5f, 1f);
            msgRect.anchorMax = new Vector2(0.5f, 1f);
            msgRect.pivot = new Vector2(0.5f, 1f);
            msgRect.anchoredPosition = new Vector2(0, -30);
            msgRect.sizeDelta = new Vector2(460, 130);
            TextMeshProUGUI msgTMP = msgObj.AddComponent<TextMeshProUGUI>();
            msgTMP.text = message;
            msgTMP.fontSize = 22;
            msgTMP.alignment = TextAlignmentOptions.Center;
            msgTMP.color = new Color(0.92f, 0.88f, 0.82f, 1f);

            // Button row
            GameObject btnRow = new GameObject("ButtonRow");
            btnRow.transform.SetParent(dialog.transform, false);
            RectTransform rowRect = btnRow.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0f);
            rowRect.anchorMax = new Vector2(0.5f, 0f);
            rowRect.pivot = new Vector2(0.5f, 0f);
            rowRect.anchoredPosition = new Vector2(0, 28);
            rowRect.sizeDelta = new Vector2(400, 55);
            HorizontalLayoutGroup hlg = btnRow.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20f;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childForceExpandWidth = false;

            yesBtn = CreateSmallButton(btnRow.transform, "YesButton", "CÓ, BẮT ĐẦU LẠI",
                new Color(0.75f, 0.15f, 0.1f, 1f), new Color(0.95f, 0.25f, 0.15f, 1f), 160f);
            noBtn = CreateSmallButton(btnRow.transform, "NoButton", "QUAY LẠI",
                new Color(0.18f, 0.18f, 0.22f, 1f), new Color(0.32f, 0.32f, 0.4f, 1f), 160f);

            return overlay;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HELPER: Create Settings panel
        // ─────────────────────────────────────────────────────────────────────────────
        private static GameObject CreateSettingsPanel(Transform parent, out Button closeBtn,
            out Slider musicSlider, out Slider sfxSlider)
        {
            // Overlay
            GameObject overlay = CreateUIImage(parent, "SettingsPanel",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform overlayRect = overlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlay.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

            // Panel box
            GameObject panel = new GameObject("PanelBox");
            panel.transform.SetParent(overlay.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(560, 380);
            Image panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0.07f, 0.07f, 0.1f, 1f);

            // Title
            GameObject titleObj = new GameObject("SettingsTitle");
            titleObj.transform.SetParent(panel.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -25);
            titleRect.sizeDelta = new Vector2(480, 55);
            TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "CÀI ĐẶT";
            titleTMP.fontSize = 32;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = new Color(0.95f, 0.85f, 0.75f, 1f);

            // Music label
            CreateSettingsLabel(panel.transform, "MusicLabel", "Âm nhạc", new Vector2(0, -115));
            // Music slider
            musicSlider = CreateSlider(panel.transform, "MusicSlider", new Vector2(0, -155));

            // SFX label
            CreateSettingsLabel(panel.transform, "SFXLabel", "Hiệu ứng âm thanh", new Vector2(0, -210));
            // SFX slider
            sfxSlider = CreateSlider(panel.transform, "SFXSlider", new Vector2(0, -250));

            // Close button
            closeBtn = CreateSmallButton(panel.transform, "CloseButton", "ĐÓNG",
                new Color(0.18f, 0.18f, 0.22f, 1f), new Color(0.32f, 0.32f, 0.4f, 1f), 160f);
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(0.5f, 0f);
            closeBtnRect.anchorMax = new Vector2(0.5f, 0f);
            closeBtnRect.pivot = new Vector2(0.5f, 0f);
            closeBtnRect.anchoredPosition = new Vector2(0, 28);
            closeBtnRect.sizeDelta = new Vector2(160, 50);

            return overlay;
        }

        private static void CreateSettingsLabel(Transform parent, string name, string text, Vector2 pos)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(460, 35);
            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 20;
            tmp.color = new Color(0.82f, 0.75f, 0.68f, 1f);
        }

        private static Slider CreateSlider(Transform parent, string name, Vector2 pos)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent, false);
            RectTransform rect = sliderObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(420, 28);
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.8f;

            // Background
            GameObject bg = CreateUIImage(sliderObj.transform, "Background",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.25f);
            bgRect.anchorMax = new Vector2(1f, 0.75f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);

            // Fill area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(5, 0);
            fillAreaRect.offsetMax = new Vector2(-15, 0);

            GameObject fill = CreateUIImage(fillArea.transform, "Fill",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fill.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.15f, 1f);
            slider.fillRect = fill.GetComponent<RectTransform>();

            // Handle slide area
            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObj.transform, false);
            RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(0f, 0f);
            handleAreaRect.anchorMax = new Vector2(1f, 1f);
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);

            GameObject handle = CreateUIImage(handleArea.transform, "Handle",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(24, 24));
            handle.GetComponent<Image>().color = new Color(0.95f, 0.88f, 0.75f, 1f);
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handle.GetComponent<Image>();

            return slider;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HELPER: Create small button
        // ─────────────────────────────────────────────────────────────────────────────
        private static Button CreateSmallButton(Transform parent, string name, string label,
            Color normalColor, Color hoverColor, float width = 140f)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, 50);

            Image img = btnObj.AddComponent<Image>();
            img.color = normalColor;

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = normalColor;
            cb.highlightedColor = hoverColor;
            cb.pressedColor = new Color(hoverColor.r * 0.7f, hoverColor.g * 0.7f, hoverColor.b * 0.7f, 1f);
            cb.fadeDuration = 0.1f;
            btn.colors = cb;
            btn.targetGraphic = img;

            GameObject textObj = new GameObject("Label");
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 17;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.97f, 0.93f, 0.88f, 1f);

            return btn;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HELPER: Create UI Image
        // ─────────────────────────────────────────────────────────────────────────────
        private static GameObject CreateUIImage(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = sizeDelta;
            obj.AddComponent<Image>();
            return obj;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HELPER: Put MainMenu as scene index 0 in Build Settings
        // ─────────────────────────────────────────────────────────────────────────────
        private static void AddMainMenuToTopOfBuildSettings(string mainMenuPath)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            // Remove existing MainMenu entry if present
            scenes.RemoveAll(s => s.path == mainMenuPath);

            // Insert at index 0
            scenes.Insert(0, new EditorBuildSettingsScene(mainMenuPath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"MainMenu scene set as Build Settings index 0.");
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // HELPER: Set private serialized field via reflection
        // ─────────────────────────────────────────────────────────────────────────────
        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
                field.SetValue(target, value);
            else
                Debug.LogWarning($"Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }
}
#endif
