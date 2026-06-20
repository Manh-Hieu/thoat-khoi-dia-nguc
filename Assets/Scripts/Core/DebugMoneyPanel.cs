using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EscapeFromHell.Chapter;

namespace EscapeFromHell.Core
{
    /// <summary>
    /// Debug money panel - only visible in Unity Editor or Development builds.
    /// Hotkey: F12 to toggle panel on/off.
    /// Allows adding/removing cash for testing purposes.
    /// </summary>
    public class DebugMoneyPanel : MonoBehaviour
    {
        private GameObject panel;
        private TMP_InputField inputField;
        private TextMeshProUGUI cashLabel;
        private bool isVisible = false;

        private void Awake()
        {
            // Only run in Editor or Development builds
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            Destroy(gameObject);
            return;
#endif
            BuildUI();
            panel.SetActive(false);
        }

        private void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // Toggle panel with F12
            if (Input.GetKeyDown(KeyCode.F12))
            {
                isVisible = !isVisible;
                panel.SetActive(isVisible);
                RefreshCashLabel();
            }
#endif
        }

        private void RefreshCashLabel()
        {
            if (cashLabel == null) return;
            int cash = Chapter2Controller.Instance != null ? Chapter2Controller.Instance.CurrentCash : 0;
            cashLabel.text = $"Tiền hiện tại: <color=#5cb85c><b>{cash:N0}đ</b></color>";
        }

        private void AddMoney(int amount)
        {
            if (Chapter2Controller.Instance == null) return;
            Chapter2Controller.Instance.CurrentCash += amount;
            RefreshCashLabel();
        }

        private void SetMoney(int amount)
        {
            if (Chapter2Controller.Instance == null) return;
            Chapter2Controller.Instance.CurrentCash = amount;
            RefreshCashLabel();
        }

        private void OnSetCustomAmount()
        {
            if (inputField == null) return;
            if (int.TryParse(inputField.text.Replace(".", "").Replace(",", ""), out int val))
                SetMoney(val);
        }

        // ─── UI Builder ───────────────────────────────────────────────────────
        private void BuildUI()
        {
            // Root panel
            panel = new GameObject("DebugMoneyPanel");
            panel.transform.SetParent(transform, false);

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                // Create own canvas if not inside one
                GameObject canvasObj = new GameObject("DebugCanvas");
                canvasObj.transform.SetParent(transform, false);
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999;
                canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasObj.AddComponent<GraphicRaycaster>();
                panel.transform.SetParent(canvasObj.transform, false);
            }

            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.anchoredPosition = new Vector2(10f, 10f);
            rt.sizeDelta = new Vector2(260f, 296f);

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.88f);

            // Title
            cashLabel = MakeText(panel, new Vector2(10, -10), new Vector2(240, 28), "Tiền: --", 12, Color.white, TextAlignmentOptions.TopLeft);

            // Preset buttons
            float y = -42;
            MakeButton(panel, new Vector2(10, y), new Vector2(115, 30), "+100.000đ",   () => AddMoney(100000));
            MakeButton(panel, new Vector2(133, y), new Vector2(115, 30), "+500.000đ",  () => AddMoney(500000));
            y -= 36;
            MakeButton(panel, new Vector2(10, y), new Vector2(115, 30), "+1.000.000đ", () => AddMoney(1000000));
            MakeButton(panel, new Vector2(133, y), new Vector2(115, 30), "+5.000.000đ",() => AddMoney(5000000));
            y -= 36;
            MakeButton(panel, new Vector2(10, y), new Vector2(115, 30), "+10.000.000đ",() => AddMoney(10000000));
            MakeButton(panel, new Vector2(133, y), new Vector2(115, 30), "-100.000đ",  () => AddMoney(-100000), new Color(0.7f, 0.2f, 0.2f));
            y -= 36;
            MakeButton(panel, new Vector2(10, y), new Vector2(235, 30), "+100.000.000đ", () => AddMoney(100000000), new Color(0.55f, 0.1f, 0.8f));
            y -= 36;
            MakeButton(panel, new Vector2(10, y), new Vector2(235, 30), "Reset về 500.000đ", () => SetMoney(500000), new Color(0.5f, 0.3f, 0f));
            y -= 40;

            // Custom amount input + set button
            inputField = MakeInputField(panel, new Vector2(10, y), new Vector2(160, 30), "Nhập số tiền...");
            MakeButton(panel, new Vector2(178, y), new Vector2(67, 30), "Set", OnSetCustomAmount, new Color(0.1f, 0.45f, 0.8f));
            y -= 38;

            // Debug label
            MakeText(panel, new Vector2(10, y), new Vector2(240, 24),
                "<color=#888>[F12] Ẩn/Hiện panel debug</color>", 9, Color.white, TextAlignmentOptions.TopLeft);
        }

        // ─── Helper methods ────────────────────────────────────────────────────
        private TextMeshProUGUI MakeText(GameObject parent, Vector2 pos, Vector2 size, string text, int fontSize, Color color, TextAlignmentOptions align)
        {
            GameObject go = new GameObject("Txt");
            go.transform.SetParent(parent.transform, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = align;
            tmp.enableWordWrapping = false;
            return tmp;
        }

        private Button MakeButton(GameObject parent, Vector2 pos, Vector2 size, string label, UnityEngine.Events.UnityAction onClick, Color? color = null)
        {
            GameObject go = new GameObject("Btn_" + label);
            go.transform.SetParent(parent.transform, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.AddComponent<Image>();
            img.color = color ?? new Color(0.15f, 0.55f, 0.15f);

            Button btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(onClick);
            btn.onClick.AddListener(RefreshCashLabel);

            GameObject lblObj = new GameObject("Label");
            lblObj.transform.SetParent(go.transform, false);
            RectTransform lrt = lblObj.AddComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = lrt.offsetMax = Vector2.zero;
            TextMeshProUGUI tmp = lblObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 10;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            return btn;
        }

        private TMP_InputField MakeInputField(GameObject parent, Vector2 pos, Vector2 size, string placeholder)
        {
            GameObject go = new GameObject("InputField");
            go.transform.SetParent(parent.transform, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f);

            TMP_InputField input = go.AddComponent<TMP_InputField>();
            input.contentType = TMP_InputField.ContentType.IntegerNumber;

            // Text area
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(go.transform, false);
            RectTransform taRt = textArea.AddComponent<RectTransform>();
            taRt.anchorMin = Vector2.zero; taRt.anchorMax = Vector2.one;
            taRt.offsetMin = new Vector2(5, 0); taRt.offsetMax = new Vector2(-5, 0);
            RectMask2D mask = textArea.AddComponent<RectMask2D>();

            // Input text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(textArea.transform, false);
            RectTransform txtRt = textObj.AddComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
            TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
            txt.fontSize = 11; txt.color = Color.white;
            txt.alignment = TextAlignmentOptions.MidlineLeft;

            // Placeholder
            GameObject phObj = new GameObject("Placeholder");
            phObj.transform.SetParent(textArea.transform, false);
            RectTransform phRt = phObj.AddComponent<RectTransform>();
            phRt.anchorMin = Vector2.zero; phRt.anchorMax = Vector2.one;
            phRt.offsetMin = phRt.offsetMax = Vector2.zero;
            TextMeshProUGUI ph = phObj.AddComponent<TextMeshProUGUI>();
            ph.text = placeholder; ph.fontSize = 10;
            ph.color = new Color(0.5f, 0.5f, 0.5f);
            ph.alignment = TextAlignmentOptions.MidlineLeft;
            ph.fontStyle = FontStyles.Italic;

            input.textViewport = taRt;
            input.textComponent = txt;
            input.placeholder = ph;
            return input;
        }
    }
}
