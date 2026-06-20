using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EscapeFromHell.Core;
using EscapeFromHell.Chapter;

namespace EscapeFromHell.Systems
{
    public class PhoneUI : MonoBehaviour
    {
        public static PhoneUI Instance { get; private set; }

        [Header("UI Screens")]
        [SerializeField] private GameObject phonePanel;
        [SerializeField] private GameObject homeScreen;
        [SerializeField] private GameObject messagesListScreen;
        [SerializeField] private GameObject chatViewScreen;
        
        [Header("Mock App Windows")]
        [SerializeField] private GameObject notesWindow;
        [SerializeField] private GameObject photosWindow;
        [SerializeField] private GameObject settingsWindow;
        [SerializeField] private GameObject browserWindow;
        [SerializeField] private GameObject weatherWindow;
        [SerializeField] private GameObject phoneWindow;
        [SerializeField] private GameObject tiktokWindow;
        [SerializeField] private GameObject facebookWindow;
        [SerializeField] private GameObject mapsWindow;
        [SerializeField] private GameObject finCreditWindow;
        [SerializeField] private GameObject bankWindow;
        private GameObject bankTransferBtnObj = null;
        
        [Header("Contacts UI")]
        [SerializeField] private GameObject contactGStar;
        [SerializeField] private GameObject contactVinaTech;
        [SerializeField] private GameObject contactAnhHungScam;

        [Header("Chat View UI")]
        [SerializeField] private TextMeshProUGUI chatTitleText;
        [SerializeField] private TextMeshProUGUI chatBodyText;
        [SerializeField] private GameObject momNotificationDot;
        [SerializeField] private GameObject billsNotificationDot;
        [SerializeField] private GameObject thanhNotificationDot;
        [SerializeField] private GameObject hungNotificationDot;
        [SerializeField] private GameObject finCreditNotificationDot;
        [SerializeField] private GameObject scamNotificationDot;
        [SerializeField] private GameObject messagesAppNotificationDot;

        [Header("Mock App Content Text Fields")]
        [SerializeField] private TextMeshProUGUI notesContentText;
        [SerializeField] private TextMeshProUGUI photosContentText;
        [SerializeField] private TextMeshProUGUI settingsContentText;
        [SerializeField] private TextMeshProUGUI browserContentText;
        [SerializeField] private TextMeshProUGUI weatherContentText;
        [SerializeField] private TextMeshProUGUI phoneContentText;
        [SerializeField] private TextMeshProUGUI tiktokContentText;
        [SerializeField] private TextMeshProUGUI facebookContentText;
        [SerializeField] private TextMeshProUGUI mapsContentText;
        [SerializeField] private TextMeshProUGUI finCreditContentText;
        [SerializeField] private TextMeshProUGUI bankContentText;

        [Header("App Icon Sprites")]
        [SerializeField] private Sprite tiktokIcon;
        [SerializeField] private Sprite facebookIcon;
        [SerializeField] private Sprite mapsIcon;
        [SerializeField] private Sprite bankIcon;
        [SerializeField] private Sprite finCreditIcon;

        [HideInInspector] public bool hasSeenJob1 = false;
        [HideInInspector] public bool hasSeenJob2 = false;
        [HideInInspector] public bool hasSeenScamJob = false;
        [HideInInspector] public bool hasSavedScamLuck = false;

        // Whether the scam chat message has been revealed (climax event)
        private bool scamChatVisible = false;
        private bool scamChatDeleted = false;

        public bool IsScamChatDeleted => scamChatDeleted;
        public bool HasOpenedScamChat => hasOpenedScamChat;

        private bool isRinging = false;
        private GameObject incomingCallScreen = null;

        public bool IsRinging => isRinging;
        public void TriggerRingingCall() { isRinging = true; }

        private bool hasOpenedMomChat = false;
        private bool hasOpenedBillsChat = false;
        private bool hasOpenedThanhChat = false;
        private bool hasOpenedHungChat = false;
        private bool hasOpenedFinCreditChat = false;
        private bool hasOpenedScamChat = false;

        // Delete confirm UI panel (built at runtime)
        private GameObject deleteConfirmPanel = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initially hide phone and all child screens/windows
            CloseAllPhoneScreens();
        }

        private void Start()
        {
            InitializeNewMockApps();
            HookUpPhoneButtons();
            // Scam chat is hidden until ShowScamChat() is called
            HideScamChatRow();
        }

        private void CloseAllPhoneScreens()
        {
            if (phonePanel != null) phonePanel.SetActive(false);
            if (homeScreen != null) homeScreen.SetActive(false);
            if (messagesListScreen != null) messagesListScreen.SetActive(false);
            if (chatViewScreen != null) chatViewScreen.SetActive(false);
            if (notesWindow != null) notesWindow.SetActive(false);
            if (photosWindow != null) photosWindow.SetActive(false);
            if (settingsWindow != null) settingsWindow.SetActive(false);
            if (browserWindow != null) browserWindow.SetActive(false);
            if (weatherWindow != null) weatherWindow.SetActive(false);
            if (phoneWindow != null) phoneWindow.SetActive(false);
            if (tiktokWindow != null) tiktokWindow.SetActive(false);
            if (facebookWindow != null) facebookWindow.SetActive(false);
            if (mapsWindow != null) mapsWindow.SetActive(false);
            if (bankWindow != null) bankWindow.SetActive(false);
            if (finCreditWindow != null) finCreditWindow.SetActive(false);
        }

        public void OpenPhone()
        {
            if (phonePanel != null)
            {
                CloseAllPhoneScreens();
                phonePanel.SetActive(true);

                // Show cursor and unlock it for UI navigation
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeState(GameState.Cutscene);
                }

                if (isRinging)
                {
                    ShowIncomingCallScreen();
                }
                else
                {
                    homeScreen.SetActive(true);
                    UpdateNotificationDots();
                }
            }
        }

        public bool IsPhoneOpen => phonePanel != null && phonePanel.activeSelf;

        public void ClosePhone()
        {
            if (isRinging)
            {
                DeclineMomCall();
                return;
            }

            CloseAllPhoneScreens();

            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
            {
                bool keepCutscene = false;
                if (DialogueSystem.Instance != null && DialogueSystem.Instance.IsDialogueActive)
                {
                    keepCutscene = true;
                }

                if (!keepCutscene)
                {
                    GameManager.Instance.ChangeState(GameState.Playing);
                }
            }

            if (Chapter1Controller.Instance != null)
            {
                Chapter1Controller.Instance.OnUIClosed();
            }
        }

        // --- SCAM CHAT REVEAL ---
        private void HideScamChatRow()
        {
            if (phonePanel == null) return;
            Transform scamRow = FindTransformRecursive(phonePanel.transform, "Chat_ScamJob");
            if (scamRow != null) scamRow.gameObject.SetActive(false);
        }

        /// <summary>Called by Chapter1Controller when climax triggers — reveals the scam SMS in the messages list.</summary>
        public void ShowScamChat()
        {
            if (scamChatVisible || scamChatDeleted) return;
            scamChatVisible = true;
            if (phonePanel == null) return;
            Transform scamRow = FindTransformRecursive(phonePanel.transform, "Chat_ScamJob");
            if (scamRow != null) scamRow.gameObject.SetActive(true);
            UpdateNotificationDots();
        }

        // --- MESSAGES APP ---
        public void OpenMessagesApp()
        {
            homeScreen.SetActive(false);
            messagesListScreen.SetActive(true);
            chatViewScreen.SetActive(false);
        }

        public void CloseMessagesApp()
        {
            homeScreen.SetActive(true);
            messagesListScreen.SetActive(false);
            chatViewScreen.SetActive(false);
        }

        public void OpenChat(int chatIndex)
        {
            messagesListScreen.SetActive(false);
            chatViewScreen.SetActive(true);

            if (chatIndex == 1) // Mom
            {
                chatTitleText.text = "Mẹ";
                chatBodyText.text = "<align=left><color=#aaccff><b>Mẹ</b></color> <size=11><color=#a0a0a5>15:30</color></size></align>\n" +
                                    "<margin-right=15%><align=left><color=#ffffff><size=14>Minh ơi, tháng này dưới quê mất mùa quá, bố con lại đau chân. Con xem có gửi về cho mẹ ít tiền thuốc thang cho bố được không?</size></color></align></margin>";
                
                if (!hasOpenedMomChat)
                {
                    hasOpenedMomChat = true;
                    if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.PhoneDialogue != null)
                    {
                        DialogueSystem.Instance.StartDialogue(Chapter1Controller.Instance.PhoneDialogue);
                    }
                }
            }
            else if (chatIndex == 2) // Landlord / Bills
            {
                chatTitleText.text = "Ban Quản Lý Trọ";
                chatBodyText.text = "<align=left><color=#aaccff><b>BQL Trọ</b></color> <size=11><color=#a0a0a5>08:15</color></size></align>\n" +
                                    "<margin-right=15%><align=left><color=#ffffff><size=14><b>THÔNG BÁO HOÁ ĐƠN THÁNG 6</b>\n• Tiền phòng: 3.000.000đ\n• Điện: 280.000đ\n• Nước: 60.000đ\n• Internet: 160.000đ\n<b>TỔNG CỘNG: 3.500.000đ</b>\nHạn chót đóng: 15/06.</size></color></align></margin>";
                
                if (!hasOpenedBillsChat)
                {
                    hasOpenedBillsChat = true;
                    if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.BillsDialogue != null)
                    {
                        DialogueSystem.Instance.StartDialogue(Chapter1Controller.Instance.BillsDialogue);
                    }
                }
            }
            else if (chatIndex == 3) // Thanh (Friend)
            {
                chatTitleText.text = "Thành";
                chatBodyText.text = 
                    "<align=left><color=#aaccff><b>Thành</b></color> <size=11><color=#a0a0a5>01/06 14:10</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Mày ơi, tháng trước mày vay tao 1 triệu bảo cuối tháng trả mà chưa thấy đâu thế?</size></color></align></margin>\n\n" +
                    "<align=right><color=#ffb732><b>Minh</b></color> <size=11><color=#a0a0a5>01/06 14:15</color></size></align>\n" +
                    "<margin-left=15%><align=right><color=#f0f0f5><size=14>Tao xin lỗi nha, tháng này tao kẹt quá, cho tao khất sang tuần sau nha mày.</size></color></align></margin>\n\n" +
                    "<align=left><color=#aaccff><b>Thành</b></color> <size=11><color=#a0a0a5>08/06 09:30</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Mày ơi tuần này đóng tiền học phí rồi, xoay trả tao được chưa?</size></color></align></margin>\n\n" +
                    "<align=right><color=#ffb732><b>Minh</b></color> <size=11><color=#a0a0a5>08/06 09:42</color></size></align>\n" +
                    "<margin-left=15%><align=right><color=#f0f0f5><size=14>Cho tao xin lỗi nha, công ty nợ lương chưa trả, sang tuần có tao chuyển liền.</size></color></align></margin>\n\n" +
                    "<align=left><color=#aaccff><b>Thành</b></color> <size=11><color=#a0a0a5>Hôm qua 18:00</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Mày ơi, tao gọi điện nhắn tin sao mày không trả lời thế? Có gì trả tao đi, tao cũng hết tiền rồi.</size></color></align></margin>\n\n" +
                    "<align=left><color=#aaccff><b>Thành</b></color> <size=11><color=#a0a0a5>Hôm nay 09:15</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Đọc tin nhắn thì trả lời tao đi chứ Minh. Trốn tránh mãi thế nào được?</size></color></align></margin>\n\n" +
                    "<align=left><color=#aaccff><b>Thành</b></color> <size=11><color=#a0a0a5>Hôm nay 15:30</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Alo, tao đang cần tiền gấp lắm rồi mày ơi. Không trả tao là tao không đóng được tiền học phí đâu đấy.</size></color></align></margin>";
                
                if (!hasOpenedThanhChat)
                {
                    hasOpenedThanhChat = true;
                    if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.ThanhDialogue != null)
                    {
                        DialogueSystem.Instance.StartDialogue(Chapter1Controller.Instance.ThanhDialogue);
                    }
                }
            }
            else if (chatIndex == 4) // Hung (Old Colleague)
            {
                chatTitleText.text = "Hùng (Đồng nghiệp cũ)";
                chatBodyText.text = 
                    "<align=right><color=#ffb732><b>Minh</b></color> <size=11><color=#a0a0a5>10/06 11:20</color></size></align>\n" +
                    "<margin-left=15%><align=right><color=#f0f0f5><size=14>Hùng ơi, bên công ty cũ mày nhận được lương chưa? Mày còn tiền không cho tao mượn tạm 500k mua đồ ăn với đóng tiền điện được không? Cuối tháng tao trả...</size></color></align></margin>\n\n" +
                    "<align=left><color=#aaccff><b>Hùng</b></color> <size=11><color=#a0a0a5>10/06 11:45</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Xin lỗi mày nha Minh, dạo này bên tao cũng bị nợ lương 2 tháng nay rồi, đang phải ăn bám bố mẹ đây... Cứ tưởng mày làm tốt định nhắn mượn mày chứ...</size></color></align></margin>\n\n" +
                    "<align=left><color=#aaccff><b>Hùng</b></color> <size=11><color=#a0a0a5>11/06 09:00</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Mày tìm được chỗ nào chưa Minh? Tao nộp cả chục chỗ rồi toàn bị đánh trượt vì thiếu kinh nghiệm, hoặc là họ ép lương thấp quá.</size></color></align></margin>\n\n" +
                    "<align=right><color=#ffb732><b>Minh</b></color> <size=11><color=#a0a0a5>11/06 09:15</color></size></align>\n" +
                    "<margin-left=15%><align=right><color=#f0f0f5><size=14>Tao cũng thế, vừa bị JoyGame với NovaTech từ chối xong. Nản quá mày ơi, giờ xin làm Fresher/Intern mà họ đòi kinh nghiệm như Senior ấy.</size></color></align></margin>\n\n" +
                    "<align=left><color=#aaccff><b>Hùng</b></color> <size=11><color=#a0a0a5>Hôm nay 10:30</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>Ừ dạo này nản thật. Kiểu này chắc tao phải chạy xe ôm kiếm cơm tạm qua ngày thôi, chứ nằm nhà chờ việc thế này chết đói mất. Còn mày tính sao?</size></color></align></margin>\n\n" +
                    "<align=right><color=#ffb732><b>Minh</b></color> <size=11><color=#a0a0a5>Hôm nay 10:40</color></size></align>\n" +
                    "<margin-left=15%><align=right><color=#f0f0f5><size=14>Tao cũng chưa biết nữa, tiền phòng tháng này còn chưa đóng được, chắc sắp bị đuổi rồi...</size></color></align></margin>";
                
                if (!hasOpenedHungChat)
                {
                    hasOpenedHungChat = true;
                    if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.HungDialogue != null)
                    {
                        DialogueSystem.Instance.StartDialogue(Chapter1Controller.Instance.HungDialogue);
                    }
                }
            }
            else if (chatIndex == 5) // FinCredit (Loan app)
            {
                chatTitleText.text = "FinCredit";
                chatBodyText.text = 
                    "<align=left><color=#ff6b6b><b>FinCredit</b></color> <size=11><color=#a0a0a5>Hôm nay 08:30</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14><b>[NHẮC NỢ LẦN 2]</b>\nKính gửi ông Nguyễn Hoài Minh, khoản vay hợp đồng HD-8827 quá hạn 5 ngày. Yêu cầu thanh toán dư nợ: 1.250.000đ trước 17:00 ngày 13/06.\n<i>Quá thời hạn trên, chúng tôi sẽ tiến hành chuyển hồ sơ sang bộ phận thu hồi nợ thực địa và liên hệ người thân theo quy định.</i></size></color></align></margin>";
                
                if (!hasOpenedFinCreditChat)
                {
                    hasOpenedFinCreditChat = true;
                    if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.FinCreditDialogue != null)
                    {
                        DialogueSystem.Instance.StartDialogue(Chapter1Controller.Instance.FinCreditDialogue);
                    }
                }
            }
            else if (chatIndex == 6) // Scam job offer from unknown number
            {
                chatTitleText.text = "+84 091 XXX XXXX";
                chatBodyText.text =
                    "<align=left><color=#2ecc71><b>Số lạ</b></color> <size=11><color=#a0a0a5>Hôm nay 07:15</color></size></align>\n" +
                    "<margin-right=15%><align=left><color=#ffffff><size=14>" +
                    "Chào bạn! Mình đang tuyển gấp nhân viên văn phòng CSKH làm việc bên Campuchia.\n\n" +
                    "• Lương: 38.000.000đ - 50.000.000đ/tháng (bao ăn ở)\n" +
                    "• Không cần kinh nghiệm, được đào tạo\n" +
                    "• Công ty lo visa, vé máy bay\n" +
                    "• Làm việc tại Bavet, sát biên giới VN\n\n" +
                    "Bạn quan tâm nhắn lại nhé! Cần người gấp tuần này thôi." +
                    "</size></color></align></margin>";

                if (!hasOpenedScamChat)
                {
                    hasOpenedScamChat = true;
                    if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.JobOfferDialogue != null)
                    {
                        DialogueSystem.Instance.StartDialogue(Chapter1Controller.Instance.JobOfferDialogue);
                    }
                }
            }

            UpdateNotificationDots();
            CheckPhoneReadProgress();
        }

        public void CloseChat()
        {
            messagesListScreen.SetActive(true);
            chatViewScreen.SetActive(false);
        }

        /// <summary>Called when player taps delete on a chat row.
        /// chatIndex 6 = scam → actually deletable. Others show a warning.</summary>
        public void TryDeleteChat(int chatIndex)
        {
            if (chatIndex == 6)
            {
                // Scam chat — allow deletion with confirmation
                ShowDeleteConfirm(
                    "Xoá tin nhắn từ số lạ này?",
                    () => {
                        scamChatDeleted = true;
                        scamChatVisible = false;
                        Transform scamRow = FindTransformRecursive(phonePanel.transform, "Chat_ScamJob");
                        if (scamRow != null) scamRow.gameObject.SetActive(false);
                        UpdateNotificationDots();
                        HideDeleteConfirm();
                        ClosePhone();
                    });
            }
            else
            {
                // Other chats — show warning, cannot delete
                HideDeleteConfirm();
                if (DialogueSystem.Instance != null && !DialogueSystem.Instance.IsDialogueActive)
                {
                    DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                    d.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = "Không có lý do gì để xóa tin nhắn này cả." }
                    };
                    DialogueSystem.Instance.StartDialogue(d);
                }
            }
        }

        private void ShowDeleteConfirm(string message, UnityEngine.Events.UnityAction onConfirm)
        {
            if (deleteConfirmPanel == null)
            {
                BuildDeleteConfirmPanel();
            }
            // Update message text
            Transform msgTrans = deleteConfirmPanel.transform.Find("Box/Message");
            if (msgTrans != null)
            {
                TextMeshProUGUI txt = msgTrans.GetComponent<TextMeshProUGUI>();
                if (txt != null) txt.text = message;
            }
            // Wire up confirm button
            Transform confirmBtnTrans = deleteConfirmPanel.transform.Find("Box/ConfirmButton");
            if (confirmBtnTrans != null)
            {
                Button confirmBtn = confirmBtnTrans.GetComponent<Button>();
                if (confirmBtn != null)
                {
                    confirmBtn.onClick.RemoveAllListeners();
                    confirmBtn.onClick.AddListener(onConfirm);
                }
            }
            deleteConfirmPanel.SetActive(true);
        }

        private void HideDeleteConfirm()
        {
            if (deleteConfirmPanel != null) deleteConfirmPanel.SetActive(false);
        }

        private void BuildDeleteConfirmPanel()
        {
            // Create overlay inside the PhoneBody
            Transform parent = phonePanel.transform.Find("PhoneBody");
            if (parent == null) parent = phonePanel.transform;
            deleteConfirmPanel = new GameObject("DeleteConfirmPanel");
            deleteConfirmPanel.transform.SetParent(parent, false);
            RectTransform rt = deleteConfirmPanel.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            // Dim overlay
            Image bg = deleteConfirmPanel.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.75f);

            // Dialog box
            GameObject box = new GameObject("Box");
            box.transform.SetParent(deleteConfirmPanel.transform, false);
            RectTransform boxRt = box.AddComponent<RectTransform>();
            boxRt.anchorMin = new Vector2(0.5f, 0.5f);
            boxRt.anchorMax = new Vector2(0.5f, 0.5f);
            boxRt.sizeDelta = new Vector2(240, 140);
            boxRt.anchoredPosition = Vector2.zero;
            Image boxImg = box.AddComponent<Image>();
            Sprite roundedRectSprite = null;
#if UNITY_EDITOR
            roundedRectSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRect.png");
#endif
            if (roundedRectSprite != null)
            {
                boxImg.sprite = roundedRectSprite;
                boxImg.type = Image.Type.Sliced;
            }
            boxImg.color = new Color(0.15f, 0.15f, 0.18f, 1f);

            // Message text
            GameObject msgObj = new GameObject("Message");
            msgObj.transform.SetParent(box.transform, false);
            RectTransform msgRt = msgObj.AddComponent<RectTransform>();
            msgRt.anchorMin = new Vector2(0f, 0.45f);
            msgRt.anchorMax = new Vector2(1f, 0.9f);
            msgRt.sizeDelta = Vector2.zero;
            msgRt.anchoredPosition = Vector2.zero;
            TextMeshProUGUI msgTxt = msgObj.AddComponent<TextMeshProUGUI>();
            msgTxt.text = "Xoá tin nhắn?";
            msgTxt.fontSize = 13;
            msgTxt.color = Color.white;
            msgTxt.alignment = TextAlignmentOptions.Center;
            msgTxt.verticalAlignment = VerticalAlignmentOptions.Middle;
            msgTxt.raycastTarget = false;

            // Confirm button (red)
            GameObject confirmBtnObj = new GameObject("ConfirmButton");
            confirmBtnObj.transform.SetParent(box.transform, false);
            RectTransform cbRt = confirmBtnObj.AddComponent<RectTransform>();
            cbRt.anchorMin = new Vector2(0f, 0f);
            cbRt.anchorMax = new Vector2(0.5f, 0.4f);
            cbRt.sizeDelta = new Vector2(-12, -12);
            cbRt.anchoredPosition = new Vector2(6, 6);
            Image cbImg = confirmBtnObj.AddComponent<Image>();
            if (roundedRectSprite != null)
            {
                cbImg.sprite = roundedRectSprite;
                cbImg.type = Image.Type.Sliced;
            }
            cbImg.color = new Color(0.8f, 0.15f, 0.15f, 1f);
            Button cbBtn = confirmBtnObj.AddComponent<Button>();
            GameObject cbTextObj = new GameObject("Text");
            cbTextObj.transform.SetParent(confirmBtnObj.transform, false);
            RectTransform cbtRt = cbTextObj.AddComponent<RectTransform>();
            cbtRt.anchorMin = Vector2.zero;
            cbtRt.anchorMax = Vector2.one;
            cbtRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI cbtTxt = cbTextObj.AddComponent<TextMeshProUGUI>();
            cbtTxt.text = "Xoá";
            cbtTxt.fontSize = 12;
            cbtTxt.fontStyle = FontStyles.Bold;
            cbtTxt.color = Color.white;
            cbtTxt.alignment = TextAlignmentOptions.Center;
            cbtTxt.verticalAlignment = VerticalAlignmentOptions.Middle;
            cbtTxt.raycastTarget = false;

            // Cancel button (gray)
            GameObject cancelBtnObj = new GameObject("CancelButton");
            cancelBtnObj.transform.SetParent(box.transform, false);
            RectTransform canRt = cancelBtnObj.AddComponent<RectTransform>();
            canRt.anchorMin = new Vector2(0.5f, 0f);
            canRt.anchorMax = new Vector2(1f, 0.4f);
            canRt.sizeDelta = new Vector2(-12, -12);
            canRt.anchoredPosition = new Vector2(-6, 6);
            Image canImg = cancelBtnObj.AddComponent<Image>();
            if (roundedRectSprite != null)
            {
                canImg.sprite = roundedRectSprite;
                canImg.type = Image.Type.Sliced;
            }
            canImg.color = new Color(0.25f, 0.25f, 0.28f, 1f);
            Button canBtn = cancelBtnObj.AddComponent<Button>();
            canBtn.onClick.AddListener(() => HideDeleteConfirm());
            GameObject canTextObj = new GameObject("Text");
            canTextObj.transform.SetParent(cancelBtnObj.transform, false);
            RectTransform cantRt = canTextObj.AddComponent<RectTransform>();
            cantRt.anchorMin = Vector2.zero;
            cantRt.anchorMax = Vector2.one;
            cantRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI cantTxt = canTextObj.AddComponent<TextMeshProUGUI>();
            cantTxt.text = "Huỷ";
            cantTxt.fontSize = 12;
            cantTxt.color = Color.white;
            cantTxt.alignment = TextAlignmentOptions.Center;
            cantTxt.verticalAlignment = VerticalAlignmentOptions.Middle;
            cantTxt.raycastTarget = false;

            deleteConfirmPanel.SetActive(false);
        }

        // --- MOCK NOTES APP ---
        public void OpenNotes()
        {
            homeScreen.SetActive(false);
            if (notesWindow != null)
            {
                notesWindow.SetActive(true);
                if (notesContentText != null)
                {
                    notesContentText.text = "<b><size=14><color=#ffaa00>• VIỆC CẦN LÀM</color></size></b>\n" +
                                           "-------------------------\n" +
                                           "<b>1.</b> Tiền nhà trước ngày 15/06 (Cần 3.5M - Thiếu 3.0M).\n" +
                                           "<b>2.</b> Tiền gửi về quê cho bố mẹ (Cần ít nhất 2.0M).\n" +
                                           "<b>3.</b> Rải CV tìm việc Fresher Web / Unity Developer.\n" +
                                           "<b>4.</b> Mua mì tôm ăn chống đói (còn 2 gói cuối).";
                }
            }
        }

        public void CloseNotes()
        {
            if (notesWindow != null) notesWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK PHOTOS APP ---
        public void OpenPhotos()
        {
            homeScreen.SetActive(false);
            if (photosWindow != null)
            {
                photosWindow.SetActive(true);
                if (photosContentText != null)
                {
                    photosContentText.text = "<b><size=14><color=#79a6f6>THƯ VIỆN ẢNH</color></size></b>\n" +
                                            "-------------------------\n" +
                                            "<i>(Thư viện ảnh trống)</i>\n\n" +
                                            "<color=#ff4d4d>Cảnh báo hệ thống:</color>\nBộ nhớ điện thoại sắp đầy. Không thể tải trước ảnh thu nhỏ.";
                }
            }
        }

        public void ClosePhotos()
        {
            if (photosWindow != null) photosWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK SETTINGS APP ---
        public void OpenSettings()
        {
            homeScreen.SetActive(false);
            if (settingsWindow != null)
            {
                settingsWindow.SetActive(true);
                if (settingsContentText != null)
                {
                    settingsContentText.text = "<b><size=14><color=#79a6f6>CÀI ĐẶT HỆ THỐNG</color></size></b>\n" +
                                               "-------------------------\n" +
                                               "<b>Tên thiết bị:</b> Joy 3 Lite\n" +
                                               "<b>Hệ điều hành:</b> Android 10\n" +
                                               "<b>Dung lượng:</b> 32 GB (<color=#ff4d4d>Trống 150 MB</color>)\n" +
                                               "<b>Tình trạng pin:</b> Chai 32% (Còn 84%)\n" +
                                               "<b>Số điện thoại:</b> 0907-xxx-123\n" +
                                               "<b>Kết nối mạng:</b> 4G Viettel (<color=#ff4d4d>Sóng yếu</color>)";
                }
            }
        }

        public void CloseSettings()
        {
            if (settingsWindow != null) settingsWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK BROWSER APP ---
        public void OpenBrowser()
        {
            homeScreen.SetActive(false);
            if (browserWindow != null)
            {
                browserWindow.SetActive(true);
                if (browserContentText != null)
                {
                    browserContentText.text = "<b><size=14><color=#8ab4f8>LỊCH SỬ TÌM KIẾM</color></size></b>\n" +
                                               "-------------------------\n" +
                                               "• <i>cách vay tiền online nhanh không thế chấp</i>\n" +
                                               "• <i>fresher it sài gòn 2026 thất nghiệp hàng loạt</i>\n" +
                                               "• <i>tuyển dụng campuchia lương cao bao ăn ở</i>\n" +
                                               "• <i>cách nhịn đói uống nước có hại dạ dày không</i>\n" +
                                               "• <i>nhà trọ giá rẻ dưới 1 triệu quận 7</i>";
                }
            }
        }

        public void CloseBrowser()
        {
            if (browserWindow != null) browserWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK WEATHER APP ---
        public void OpenWeather()
        {
            homeScreen.SetActive(false);
            if (weatherWindow != null)
            {
                weatherWindow.SetActive(true);
                if (weatherContentText != null)
                {
                    weatherContentText.text = "<b><size=14><color=#79a6f6>THỜI TIẾT HÔM NAY</color></size></b>\n" +
                                               "Sài Gòn • 16:02 • Mây U Ám\n" +
                                               "-------------------------\n" +
                                               "<b>Nhiệt độ:</b> 34°C (Cảm giác như 39°C)\n" +
                                               "<b>Độ ẩm:</b> 82%\n" +
                                               "<b>Chất lượng không khí:</b> AQI 165 (<color=#ff4d4d>Độc hại</color>)\n" +
                                               "<b>Dự báo:</b> Mưa dông lớn sấm sét vào cuối chiều.\n\n" +
                                               "<color=#b3b3b3><i>Thời tiết ngột ngạt và ẩm thấp... giống như cuộc đời mình lúc này vậy.</i></color>";
                }
            }
        }

        public void CloseWeather()
        {
            if (weatherWindow != null) weatherWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        private void UpdateNotificationDots()
        {
            if (momNotificationDot != null) momNotificationDot.SetActive(!hasOpenedMomChat);
            if (billsNotificationDot != null) billsNotificationDot.SetActive(!hasOpenedBillsChat);
            if (thanhNotificationDot != null) thanhNotificationDot.SetActive(!hasOpenedThanhChat);
            if (hungNotificationDot != null) hungNotificationDot.SetActive(!hasOpenedHungChat);
            if (finCreditNotificationDot != null) finCreditNotificationDot.SetActive(!hasOpenedFinCreditChat);
            if (scamNotificationDot != null) scamNotificationDot.SetActive(!hasOpenedScamChat && scamChatVisible && !scamChatDeleted);
            if (messagesAppNotificationDot != null)
            {
                // Scam chat only contributes an unread dot if it's actually visible and unread
                bool scamUnread = scamChatVisible && !scamChatDeleted && !hasOpenedScamChat;
                messagesAppNotificationDot.SetActive(
                    !hasOpenedMomChat ||
                    !hasOpenedBillsChat ||
                    !hasOpenedThanhChat ||
                    !hasOpenedHungChat ||
                    !hasOpenedFinCreditChat ||
                    scamUnread
                );
            }
        }

        private void CheckPhoneReadProgress()
        {
            if (Chapter1Controller.Instance != null)
            {
                if (hasOpenedMomChat)
                {
                    Chapter1Controller.Instance.ReadPhoneFromPhoneUI();
                }
                if (hasOpenedBillsChat)
                {
                    Chapter1Controller.Instance.ReadBillsFromPhoneUI();
                }
                if (hasOpenedThanhChat)
                {
                    Chapter1Controller.Instance.ReadFriendFromPhoneUI();
                }
                if (hasOpenedHungChat)
                {
                    Chapter1Controller.Instance.ReadHungFromPhoneUI();
                }
                if (hasOpenedFinCreditChat)
                {
                    Chapter1Controller.Instance.ReadFinCreditFromPhoneUI();
                }
            }
        }

        private void Update()
        {
            if (phonePanel != null && phonePanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ClosePhone();
                }
            }
        }

        public void ClickPhoneApp()
        {
            OpenPhoneApp();
        }

        // --- MOCK PHONE APP (CONTACTS) ---
        public void OpenPhoneApp()
        {
            homeScreen.SetActive(false);
            if (phoneWindow != null)
            {
                phoneWindow.SetActive(true);

                // Show/hide contact cards based on job search progress
                if (contactGStar != null) contactGStar.SetActive(hasSeenJob1);
                if (contactVinaTech != null) contactVinaTech.SetActive(hasSeenJob2);
                if (contactAnhHungScam != null) contactAnhHungScam.SetActive(hasSeenScamJob);
            }
        }

        public void CallContact(string contactName)
        {
            if (DialogueSystem.Instance == null) return;

            DialogueData callDialogue = ScriptableObject.CreateInstance<DialogueData>();
            string thinkingText = "";

            if (contactName == "Mom")
            {
                thinkingText = "Giờ mà gọi cho Mẹ chắc mẹ lại lo lắng phát khóc mất... Mình không dám đối diện với mẹ lúc này, phải tự giải quyết đống nợ này trước đã.";
            }
            else if (contactName == "Dad")
            {
                thinkingText = "Mình không dám gọi cho Bố... Nhìn cuộc gọi nhỡ của bố cách đây 3 ngày mà lòng mình thắt lại. Bố đang đau chân ở quê, nếu biết mình đang nợ nần thế này chắc bố gục ngã mất...";
            }
            else if (contactName == "Thang")
            {
                thinkingText = "Giờ gọi cho Thắng cũng chẳng giải quyết được gì, nó cũng đang chật vật tìm việc ở quê. Tự mình phải lo lấy thân thôi...";
            }
            else if (contactName == "Hung")
            {
                thinkingText = "Gọi cho Hùng lúc này chỉ làm phiền nó thêm, ai cũng có nỗi lo riêng... Mình không muốn làm phiền ai nữa.";
            }
            else if (contactName == "GStar")
            {
                if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.canCallJobCompanies)
                {
                    if (Chapter1Controller.Instance.hasCalledCompany1)
                    {
                        thinkingText = "Gọi mấy lần kết quả vẫn là từ chối thôi... Mình thật sự thất bại mà...";
                    }
                    else
                    {
                        callDialogue.lines = new System.Collections.Generic.List<DialogueLine>
                        {
                            new DialogueLine { speakerName = "Minh", text = "*(Đang kết nối cuộc gọi đến JoyGame Studio...)*" },
                            new DialogueLine { speakerName = "Đại diện JoyGame", text = "Alo, JoyGame Studio xin nghe. Cậu hỏi về vị trí thực tập sinh Unity đúng không?" },
                            new DialogueLine { speakerName = "Minh", text = "Dạ vâng ạ, em muốn hỏi kết quả CV..." },
                            new DialogueLine { speakerName = "Đại diện JoyGame", text = "À, vị trí thực tập sinh hiện tại chúng tôi tuyển đủ người rồi nhé. Hẹn cậu khi khác." },
                            new DialogueLine { speakerName = "Minh", text = "Họ từ chối rồi... Vị trí thực tập sinh không lương cũng không còn cơ hội cho mình nữa..." }
                        };
                        Chapter1Controller.Instance.hasCalledCompany1 = true;
                        DialogueSystem.Instance.StartDialogue(callDialogue);
                        Chapter1Controller.Instance.CheckProgress();
                        return;
                    }
                }
                else
                {
                    thinkingText = "Mình vẫn còn do dự quá... Vị trí thực tập hỗ trợ 1 triệu/tháng cũng không đủ trả tiền phòng trọ lúc này, để tìm hiểu thêm xem sao đã...";
                }
            }
            else if (contactName == "VinaTech")
            {
                if (Chapter1Controller.Instance != null && Chapter1Controller.Instance.canCallJobCompanies)
                {
                    if (Chapter1Controller.Instance.hasCalledCompany2)
                    {
                        thinkingText = "Gọi mấy lần kết quả vẫn là từ chối thôi... Mình thật sự thất bại mà...";
                    }
                    else
                    {
                        callDialogue.lines = new System.Collections.Generic.List<DialogueLine>
                        {
                            new DialogueLine { speakerName = "Minh", text = "*(Đang kết nối cuộc gọi đến NovaTech Group...)*" },
                            new DialogueLine { speakerName = "Đại diện NovaTech", text = "Alo, bộ phận tuyển dụng NovaTech xin nghe." },
                            new DialogueLine { speakerName = "Minh", text = "Dạ chào anh, em muốn ứng tuyển vị trí Fresher Web..." },
                            new DialogueLine { speakerName = "Đại diện NovaTech", text = "Vị trí đó yêu cầu tối thiểu 1 năm kinh nghiệm làm việc thực tế tại doanh nghiệp. Cậu đã đi làm ở đâu chưa?" },
                            new DialogueLine { speakerName = "Minh", text = "Dạ chưa, em mới tự học và làm các dự án cá nhân thôi ạ..." },
                            new DialogueLine { speakerName = "Đại diện NovaTech", text = "Thế thì không được rồi. Chúng tôi cần người vào làm được ngay để chạy dự án. Cảm ơn cậu nhé." },
                            new DialogueLine { speakerName = "Minh", text = "Lại bị từ chối... Đúng là thời buổi kinh tế khó khăn, người không có kinh nghiệm như mình không có cửa..." }
                        };
                        Chapter1Controller.Instance.hasCalledCompany2 = true;
                        DialogueSystem.Instance.StartDialogue(callDialogue);
                        Chapter1Controller.Instance.CheckProgress();
                        return;
                    }
                }
                else
                {
                    thinkingText = "Họ đòi tận 1 năm kinh nghiệm thực tế tại doanh nghiệp, mình mới tự học, gọi lúc này chắc chắn sẽ bị từ chối ngay...";
                }
            }
            else if (contactName == "AnhHungScam")
            {
                thinkingText = "Trong tin tuyển dụng ghi rõ là liên hệ Zalo hoặc cứ nhắn tin đồng ý là họ cho xe đón... Mình không cần phải gọi điện trực tiếp làm gì.";
            }
            else
            {
                thinkingText = "Không nên thực hiện cuộc gọi lúc này.";
            }

            callDialogue.lines = new System.Collections.Generic.List<DialogueLine>
            {
                new DialogueLine { speakerName = "Minh", text = thinkingText }
            };
            DialogueSystem.Instance.StartDialogue(callDialogue);
        }

        public void ClosePhoneApp()
        {
            if (phoneWindow != null) phoneWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        private void HookUpPhoneButtons()
        {
            if (phonePanel == null) return;

            // Helper to find button and add listener
            System.Action<string, UnityEngine.Events.UnityAction> bind = (name, action) =>
            {
                Button btn = FindButtonRecursive(phonePanel.transform, name);
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(action);
                }
            };

            // Bind Home Screen Apps
            bind("NotesAppButton", () => OpenNotes());
            bind("PhotosAppButton", () => OpenPhotos());
            bind("WeatherAppButton", () => OpenWeather());
            bind("SettingsAppButton", () => OpenSettings());
            bind("TikTokAppButton", () => OpenTikTok());
            bind("FacebookAppButton", () => OpenFacebook());
            bind("MapsAppButton", () => OpenMaps());
            bind("BankAppButton", () => OpenBankApp());
            bind("FinCreditAppButton", () => OpenFinCreditApp());

            // Bind Dock Apps
            bind("PhoneAppButton", () => OpenPhoneApp());
            bind("MessagesAppButton", () => OpenMessagesApp());
            bind("BrowserAppButton", () => OpenBrowser());

            // Bind Navigation/Close Buttons
            bind("HomeBar", () => ClosePhone());
            bind("PhoneCloseButton", () => ClosePhone());
            bind("ClickOutsideArea", () => ClosePhone());

            // Bind specific Back Buttons using paths relative to phonePanel
            System.Action<string, UnityEngine.Events.UnityAction> bindPath = (path, action) =>
            {
                Transform t = phonePanel.transform.Find(path);
                if (t != null)
                {
                    Button btn = t.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(action);
                    }
                }
            };

            bindPath("PhoneBody/MessagesListScreen/BackButton", () => CloseMessagesApp());
            bindPath("PhoneBody/ChatViewScreen/TitleBar/BackButton", () => CloseChat());
            bindPath("PhoneBody/NotesWindow/TitleBar/BackButton", () => CloseNotes());
            bindPath("PhoneBody/PhotosWindow/TitleBar/BackButton", () => ClosePhotos());
            bindPath("PhoneBody/SettingsWindow/TitleBar/BackButton", () => CloseSettings());
            bindPath("PhoneBody/BrowserWindow/TitleBar/BackButton", () => CloseBrowser());
            bindPath("PhoneBody/WeatherWindow/TitleBar/BackButton", () => CloseWeather());
            bindPath("PhoneBody/PhoneWindow/TitleBar/BackButton", () => ClosePhoneApp());
            bindPath("PhoneBody/TikTokWindow/TitleBar/BackButton", () => CloseTikTok());
            bindPath("PhoneBody/FacebookWindow/TitleBar/BackButton", () => CloseFacebook());
            bindPath("PhoneBody/MapsWindow/TitleBar/BackButton", () => CloseMaps());
            bindPath("PhoneBody/BankWindow/TitleBar/BackButton", () => CloseBankApp());
            bindPath("PhoneBody/FinCreditWindow/TitleBar/BackButton", () => CloseFinCreditApp());

            // Bind Chats
            bind("Chat_Mom", () => OpenChat(1));
            bind("Chat_Bills", () => OpenChat(2));
            bind("Chat_Thanh", () => OpenChat(3));
            bind("Chat_Hung", () => OpenChat(4));
            bind("Chat_FinCredit", () => OpenChat(5));
            bind("Chat_ScamJob", () => OpenChat(6));

            // Bind Delete Buttons for each chat row
            bindPath("PhoneBody/MessagesListScreen/ChatsContainer/Chat_Mom/DeleteButton", () => TryDeleteChat(1));
            bindPath("PhoneBody/MessagesListScreen/ChatsContainer/Chat_Bills/DeleteButton", () => TryDeleteChat(2));
            bindPath("PhoneBody/MessagesListScreen/ChatsContainer/Chat_Thanh/DeleteButton", () => TryDeleteChat(3));
            bindPath("PhoneBody/MessagesListScreen/ChatsContainer/Chat_Hung/DeleteButton", () => TryDeleteChat(4));
            bindPath("PhoneBody/MessagesListScreen/ChatsContainer/Chat_FinCredit/DeleteButton", () => TryDeleteChat(5));
            bindPath("PhoneBody/MessagesListScreen/ChatsContainer/Chat_ScamJob/DeleteButton", () => TryDeleteChat(6));

            // Bind Contacts
            bind("Contact_Mom", () => CallContact("Mom"));
            bind("Contact_Dad", () => CallContact("Dad"));
            bind("Contact_Thang", () => CallContact("Thang"));
            bind("Contact_Hung", () => CallContact("Hung"));
            bind("Contact_GStar", () => CallContact("GStar"));
            bind("Contact_VinaTech", () => CallContact("VinaTech"));
            bind("Contact_AnhHungScam", () => CallContact("AnhHungScam"));
        }

        private Button FindButtonRecursive(Transform parent, string name)
        {
            if (parent.name == name)
            {
                Button btn = parent.GetComponent<Button>();
                if (btn != null) return btn;
            }
            for (int i = 0; i < parent.childCount; i++)
            {
                Button btn = FindButtonRecursive(parent.GetChild(i), name);
                if (btn != null) return btn;
            }
            return null;
        }

        private Transform FindTransformRecursive(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform t = FindTransformRecursive(parent.GetChild(i), name);
                if (t != null) return t;
            }
            return null;
        }

        // --- NEW MOCK APPS LOGIC ---
        private void InitializeNewMockApps()
        {
            Transform appsGrid = homeScreen != null ? homeScreen.transform.Find("AppsGrid") : null;
            if (appsGrid == null) return;

            Transform row1 = appsGrid.Find("Row1");
            Transform row2 = appsGrid.Find("Row2");

            Transform templateBtn = (row1 != null ? row1.Find("NotesAppButton") : null) ?? 
                                    (row1 != null ? row1.Find("PhotosAppButton") : null) ??
                                    (appsGrid.childCount > 0 ? appsGrid.GetChild(0) : null);
            if (templateBtn == null) return;

            // TikTok, Facebook, Maps, and FinCredit on Row 2
            if (row2 != null)
            {
                CreateDynamicAppButton(row2, templateBtn, "TikTokAppButton", "TikTok", new Color(0.05f, 0.05f, 0.05f, 1f), tiktokIcon, () => OpenTikTok());
                CreateDynamicAppButton(row2, templateBtn, "FacebookAppButton", "Facebook", new Color(0.09f, 0.4f, 0.73f, 1f), facebookIcon, () => OpenFacebook());
                CreateDynamicAppButton(row2, templateBtn, "MapsAppButton", "Ban Do", new Color(0.2f, 0.6f, 0.3f, 1f), mapsIcon, () => OpenMaps());
                CreateDynamicAppButton(row2, templateBtn, "FinCreditAppButton", "FinCredit", new Color(0.8f, 0.2f, 0.2f, 1f), finCreditIcon, () => OpenFinCreditApp());
            }



            if (tiktokWindow == null)
            {
                tiktokWindow = CreateRuntimeAppWindow("TikTokWindow", "TokTok", out tiktokContentText, () => CloseTikTok());
            }
            if (facebookWindow == null)
            {
                facebookWindow = CreateRuntimeAppWindow("FacebookWindow", "FaceNet", out facebookContentText, () => CloseFacebook());
            }
            if (mapsWindow == null)
            {
                mapsWindow = CreateRuntimeAppWindow("MapsWindow", "Bản đồ GoMaps", out mapsContentText, () => CloseMaps());
            }
            if (bankWindow == null)
            {
                bankWindow = CreateRuntimeAppWindow("BankWindow", "MB Bank", out bankContentText, () => CloseBankApp());

                // Create transfer button inside bankWindow
                bankTransferBtnObj = new GameObject("TransferButton");
                bankTransferBtnObj.transform.SetParent(bankWindow.transform, false);
                RectTransform tfRt = bankTransferBtnObj.AddComponent<RectTransform>();
                // Position at bottom center of the bank app
                tfRt.anchorMin = new Vector2(0.1f, 0.05f);
                tfRt.anchorMax = new Vector2(0.9f, 0.13f);
                tfRt.anchoredPosition = Vector2.zero;
                tfRt.sizeDelta = Vector2.zero;

                Image tfImg = bankTransferBtnObj.AddComponent<Image>();
                Sprite roundedRectSprite = null;
#if UNITY_EDITOR
                roundedRectSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRect.png");
#endif
                if (roundedRectSprite != null)
                {
                    tfImg.sprite = roundedRectSprite;
                    tfImg.type = Image.Type.Sliced;
                }
                tfImg.color = new Color(0.1f, 0.45f, 0.8f, 1f); // Blue button

                Button tfBtn = bankTransferBtnObj.AddComponent<Button>();
                tfBtn.onClick.AddListener(() => ClickBankTransfer());

                GameObject tfTextObj = new GameObject("Text");
                tfTextObj.transform.SetParent(bankTransferBtnObj.transform, false);
                RectTransform tftRt = tfTextObj.AddComponent<RectTransform>();
                tftRt.anchorMin = Vector2.zero;
                tftRt.anchorMax = Vector2.one;
                tftRt.sizeDelta = Vector2.zero;

                TextMeshProUGUI tftTxt = tfTextObj.AddComponent<TextMeshProUGUI>();
                tftTxt.text = "Chuyển khoản 100.000đ";
                tftTxt.fontSize = 12;
                tftTxt.fontStyle = FontStyles.Bold;
                tftTxt.color = Color.white;
                tftTxt.alignment = TextAlignmentOptions.Center;
                tftTxt.verticalAlignment = VerticalAlignmentOptions.Middle;
                tftTxt.raycastTarget = false;

                bankTransferBtnObj.SetActive(false); // Initially hidden
            }
            if (finCreditWindow == null)
            {
                finCreditWindow = CreateRuntimeAppWindow("FinCreditWindow", "FinCredit - Ví vay nợ", out finCreditContentText, () => CloseFinCreditApp());
            }
        }

        private void CreateDynamicAppButton(Transform grid, Transform template, string name, string label, Color bgColor, Sprite appIcon, UnityEngine.Events.UnityAction onClick)
        {
            Transform existing = grid.Find(name);
            GameObject btnObj;
            if (existing != null)
            {
                btnObj = existing.gameObject;
            }
            else
            {
                btnObj = Instantiate(template.gameObject, grid);
                btnObj.name = name;
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(onClick);
            }

            Transform bgTrans = btnObj.transform.Find("IconBackground");
            if (bgTrans != null)
            {
                Image bgImg = bgTrans.GetComponent<Image>();
                if (bgImg != null)
                {
                    // If the original has a sprite, keep it, otherwise use color
                    if (bgImg.sprite == null)
                    {
                        bgImg.color = bgColor;
                    }
                }

                Transform glyphTrans = bgTrans.Find("IconGlyph");
                if (glyphTrans != null)
                {
                    Image glyphImg = glyphTrans.GetComponent<Image>();
                    if (glyphImg != null)
                    {
                        if (appIcon != null)
                        {
                            glyphImg.enabled = true;
                            glyphImg.sprite = appIcon;
                        }
                        else
                        {
                            glyphImg.enabled = false;
                        }
                    }

                    Transform emojiTrans = glyphTrans.Find("EmojiLabel");
                    if (emojiTrans != null)
                    {
                        if (appIcon != null)
                        {
                            Destroy(emojiTrans.gameObject);
                        }
                        else
                        {
                            TextMeshProUGUI glyphText = emojiTrans.GetComponent<TextMeshProUGUI>();
                            if (glyphText != null)
                            {
                                glyphText.enabled = true;
                            }
                        }
                    }
                }
            }

            Transform labelTrans = btnObj.transform.Find("Label");
            if (labelTrans != null)
            {
                TextMeshProUGUI lblText = labelTrans.GetComponent<TextMeshProUGUI>();
                if (lblText != null)
                {
                    lblText.text = label;
                }
            }
        }

        private GameObject CreateRuntimeAppWindow(string name, string title, out TextMeshProUGUI contentText, UnityEngine.Events.UnityAction onClose)
        {
            Transform parent = homeScreen != null ? homeScreen.transform.parent : transform;
            GameObject win = new GameObject(name);
            win.transform.SetParent(parent, false);
            RectTransform winRect = win.AddComponent<RectTransform>();
            winRect.anchorMin = Vector2.zero;
            winRect.anchorMax = Vector2.one;
            winRect.sizeDelta = new Vector2(0, -30);
            winRect.anchoredPosition = new Vector2(0, -15);

            Image img = win.AddComponent<Image>();
            img.color = new Color(0.08f, 0.08f, 0.09f, 1f);

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
            titleTxt.fontSize = 14;
            titleTxt.fontStyle = FontStyles.Bold;
            titleTxt.color = Color.white;
            titleTxt.alignment = TextAlignmentOptions.Center;
            titleTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            GameObject backBtnObj = new GameObject("BackButton");
            backBtnObj.transform.SetParent(titleBar.transform, false);
            RectTransform backRect = backBtnObj.AddComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 0.5f);
            backRect.anchorMax = new Vector2(0f, 0.5f);
            backRect.pivot = new Vector2(0f, 0.5f);
            backRect.sizeDelta = new Vector2(60, 30);
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
            btTxt.text = "< Thoát";
            btTxt.fontSize = 13;
            btTxt.color = new Color(0f, 0.478f, 1f);
            btTxt.alignment = TextAlignmentOptions.Left;
            btTxt.verticalAlignment = VerticalAlignmentOptions.Middle;

            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(win.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = new Vector2(-24, -60);
            contentRect.anchoredPosition = new Vector2(0, -25);

            contentText = contentObj.AddComponent<TextMeshProUGUI>();
            contentText.text = "Nội dung...";
            contentText.fontSize = 12;
            contentText.color = Color.white;
            contentText.verticalAlignment = VerticalAlignmentOptions.Top;

            win.SetActive(false);
            return win;
        }

        // --- MOCK TIKTOK APP ---
        public void OpenTikTok()
        {
            homeScreen.SetActive(false);
            if (tiktokWindow != null)
            {
                tiktokWindow.SetActive(true);
                if (tiktokContentText != null)
                {
                    tiktokContentText.text = "<b><size=14><color=#ff0050>TIKTOK</color></size></b>\n" +
                                             "-------------------------\n" +
                                             "• <i>@huong_ngoc:</i> \"Thử thách 14 ngày làm giàu ở xứ Cam...\" (1.2K Tim)\n" +
                                             "• <i>@khoinghiep_tre:</i> \"Cần tìm 5 bạn nam nữ phụ kho, bao ăn ở tại Mộc Bài...\" (560 Tim)\n" +
                                             "• <i>@it_life:</i> \"Khi bạn thất nghiệp 6 tháng và nhận được tin tuyển dụng Campuchia...\" (3.4K Tim)\n" +
                                             "• <i>@bavet_daily:</i> \"Cảnh đẹp Casino Bavet về đêm lung linh ánh đèn...\" (820 Tim)";
                }
                // Note: dialogue is triggered on Close to avoid blocking button input
            }
        }

        public void CloseTikTok()
        {
            if (tiktokWindow != null) tiktokWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK FACEBOOK APP ---
        public void OpenFacebook()
        {
            homeScreen.SetActive(false);
            if (facebookWindow != null)
            {
                facebookWindow.SetActive(true);
                if (facebookContentText != null)
                {
                    facebookContentText.text = "<b><size=14><color=#1877f2>FACEBOOK</color></size></b>\n" +
                                               "-------------------------\n" +
                                               "• <b>Hội Tìm Việc IT Sài Gòn</b>:\n" +
                                               "  \"Dạo này thị trường IT đóng băng quá, có ai biết bên Campuchia làm văn phòng ổn không ạ? Thấy lương cao quá...\"\n" +
                                               "• <b>Group Việc Làm Campuchia 24h</b>:\n" +
                                               "  \"Tuyển gấp CSKH/Nhập liệu, lương cứng 25 triệu/tháng. Xe đưa đón tận nơi từ Sài Gòn, không cần bằng cấp...\"\n" +
                                               "• <b>Tin nhắn từ Thành (Bạn Thân)</b>:\n" +
                                               "  \"Mày ơi rảnh thì online tao nhờ tí nhé...\"";
                }
                // Note: dialogue is triggered on Close to avoid blocking button input
            }
        }

        public void CloseFacebook()
        {
            if (facebookWindow != null) facebookWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK MAPS APP ---
        public void OpenMaps()
        {
            homeScreen.SetActive(false);
            if (mapsWindow != null)
            {
                mapsWindow.SetActive(true);
                if (mapsContentText != null)
                {
                    mapsContentText.text = "<b><size=14><color=#34a853>GOOGLE MAPS</color></size></b>\n" +
                                           "-------------------------\n" +
                                           "• <b>Vị trí hiện tại:</b>\n" +
                                           "  Hẻm 48 Đường số 3, Bình Hưng, Bình Chánh, TP.HCM.\n\n" +
                                           "• <b>Tìm kiếm gần đây:</b>\n" +
                                           "  1. <i>Cửa khẩu Mộc Bài, Tây Ninh</i> (Cách 78 km - 2 giờ đi ô tô)\n" +
                                           "  2. <i>Bavet, Campuchia</i> (Ngay sát biên giới)\n" +
                                           "  3. <i>Quận 7, TP.HCM</i> (Cách 12 km)";
                }
                // Note: dialogue is triggered on Close to avoid blocking button input
            }
        }

        public void CloseMaps()
        {
            if (mapsWindow != null) mapsWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK FINCREDIT APP ---
        public void OpenFinCreditApp()
        {
            homeScreen.SetActive(false);
            if (finCreditWindow != null)
            {
                finCreditWindow.SetActive(true);
                if (finCreditContentText != null)
                {
                    finCreditContentText.text = "<b><size=14><color=#e74c3c>FINCREDIT - VÍ VAY NỢ</color></size></b>\n" +
                                                "-------------------------\n" +
                                                "• <b>Mã hợp đồng:</b> HD-8827\n" +
                                                "• <b>Hạn mức vay:</b> 1.000.000đ\n" +
                                                "• <b>Số tiền quá hạn:</b> 1.250.000đ (Đã cộng lãi phạt quá hạn)\n" +
                                                "• <b>Trạng thái:</b> <color=#ff4d4d>QUÁ HẠN 5 NGÀY</color>\n" +
                                                "• <b>Hạn chót thanh toán:</b> 17:00 ngày 13/06/2026.\n\n" +
                                                "<color=#ff4d4d>Cảnh báo:</color> Nếu không thanh toán đúng hạn, hệ thống sẽ tự động gọi điện nhắc nợ đến danh bạ người thân.";
                }
                // Note: dialogue is triggered on Close to avoid blocking button input
            }
        }

        public void CloseFinCreditApp()
        {
            if (finCreditWindow != null) finCreditWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- MOCK BANK APP ---
        public void OpenBankApp()
        {
            homeScreen.SetActive(false);
            if (bankWindow != null)
            {
                bankWindow.SetActive(true);
                
                // Show or hide the transfer button based on scam status
                if (bankTransferBtnObj != null)
                {
                    bool showBtn = hasSavedScamLuck && !(EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.hasTransferredScamLuck);
                    bankTransferBtnObj.SetActive(showBtn);
                }

                UpdateBankContentText();
            }
        }

        public void UpdateBankContentText()
        {
            if (bankContentText != null)
            {
                string luckAccount = "";
                string balanceStr = "500.000đ";
                string noteStr = "<i>Số dư chỉ đủ để trang trải tạm thời cuộc sống, không đủ để thanh toán tiền nhà trọ (3.500.000đ) hay trả hết nợ nần...</i>";
                
                bool isTransferred = (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.hasTransferredScamLuck);

                if (hasSavedScamLuck)
                {
                    if (isTransferred)
                    {
                        balanceStr = "400.000đ";
                        luckAccount = "\n\n<b><color=#005691>TÀI KHOẢN ĐÃ LƯU CHUYỂN TIỀN:</color></b>\n" +
                                      "• <b>Tên thụ hưởng:</b> DV THAY DOI VAN MENH\n" +
                                      "• <b>Số tài khoản:</b> 666688889999\n" +
                                      "• <b>Ngân hàng:</b> VIETCOMBANK (VCB)\n" +
                                      "• <b>Trạng thái:</b> <color=#5cb85c><b>ĐÃ CHUYỂN 100.000đ</b></color>\n" +
                                      "• <b>Vận mệnh:</b> Đã nhận buff may mắn (10 lượt thắng casino)";
                        noteStr = "<i>Số tiền còn lại (400.000đ) quá ít ỏi. Cả nguồn sống và niềm hy vọng của mình giờ đây phụ thuộc vào 10 lượt buff may mắn này...</i>";
                    }
                    else
                    {
                        luckAccount = "\n\n<b><color=#005691>TÀI KHOẢN ĐÃ LƯU CHUYỂN TIỀN:</color></b>\n" +
                                      "• <b>Tên thụ hưởng:</b> DV THAY DOI VAN MENH\n" +
                                      "• <b>Số tài khoản:</b> 666688889999\n" +
                                      "• <b>Ngân hàng:</b> VIETCOMBANK (VCB)\n" +
                                      "• <b>Mức chuyển:</b> 100.000đ (Đổi vận 10 lần/ngày)";
                    }
                }
                
                bankContentText.text = "<b><size=14><color=#005691>NGÂN HÀNG MB BANK</color></size></b>\n" +
                                       "-------------------------\n" +
                                       "• <b>Chủ tài khoản:</b> NGUYEN HOAI MINH\n" +
                                       "• <b>Số tài khoản:</b> 9907123456789\n" +
                                       "• <b>Số dư khả dụng:</b> <color=#5cb85c><b>" + balanceStr + "</b></color>\n" +
                                       "• <b>Liên kết thẻ:</b> Không có hoạt động" + luckAccount + "\n\n" +
                                       noteStr;
            }
        }

        public void ClickBankTransfer()
        {
            if (EscapeFromHell.Core.GameManager.Instance == null) return;
            
            // Deduct money, set transfer flag and set buff count to 10
            EscapeFromHell.Core.GameManager.Instance.hasTransferredScamLuck = true;
            EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining = 10;
            
            // Hide transfer button immediately
            if (bankTransferBtnObj != null) bankTransferBtnObj.SetActive(false);
            
            // Update bank content text
            UpdateBankContentText();
            
            // Trigger Minh's confirmation dialogue
            if (DialogueSystem.Instance != null)
            {
                DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Đã chuyển khoản thành công 100k cho dịch vụ cải vận VCB: 666688889999." },
                    new DialogueLine { speakerName = "Minh", text = "Tài khoản của mình đã bị trừ 100k, chỉ còn lại 400k. Mong là may mắn 10 lần trong ngày sẽ thực sự linh nghiệm..." }
                };
                DialogueSystem.Instance.StartDialogue(textMock);
            }
        }

        public void CloseBankApp()
        {
            if (bankWindow != null) bankWindow.SetActive(false);
            homeScreen.SetActive(true);
        }

        // --- INCOMING CALL FROM MOM ---
        private void ShowIncomingCallScreen()
        {
            if (incomingCallScreen == null)
            {
                BuildIncomingCallScreen();
            }
            incomingCallScreen.SetActive(true);
        }

        private void BuildIncomingCallScreen()
        {
            Transform parent = phonePanel.transform.Find("PhoneBody");
            if (parent == null) parent = phonePanel.transform;
            incomingCallScreen = new GameObject("IncomingCallScreen");
            incomingCallScreen.transform.SetParent(parent, false);
            RectTransform rt = incomingCallScreen.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            // Background (Dark gray/black calling screen style)
            Image bg = incomingCallScreen.AddComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.1f, 1f);

            // Caller name
            GameObject nameObj = new GameObject("CallerName");
            nameObj.transform.SetParent(incomingCallScreen.transform, false);
            RectTransform nameRt = nameObj.AddComponent<RectTransform>();
            nameRt.anchorMin = new Vector2(0f, 0.6f);
            nameRt.anchorMax = new Vector2(1f, 0.8f);
            nameRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI nameTxt = nameObj.AddComponent<TextMeshProUGUI>();
            nameTxt.text = "Mẹ";
            nameTxt.fontSize = 28;
            nameTxt.color = Color.white;
            nameTxt.alignment = TextAlignmentOptions.Center;
            nameTxt.fontStyle = FontStyles.Bold;

            // Status label
            GameObject statusObj = new GameObject("Status");
            statusObj.transform.SetParent(incomingCallScreen.transform, false);
            RectTransform statusRt = statusObj.AddComponent<RectTransform>();
            statusRt.anchorMin = new Vector2(0f, 0.5f);
            statusRt.anchorMax = new Vector2(1f, 0.6f);
            statusRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI statusTxt = statusObj.AddComponent<TextMeshProUGUI>();
            statusTxt.text = "Đang đổ chuông...";
            statusTxt.fontSize = 16;
            statusTxt.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            statusTxt.alignment = TextAlignmentOptions.Center;

            // Green Answer Button
            GameObject answerBtnObj = new GameObject("AnswerButton");
            answerBtnObj.transform.SetParent(incomingCallScreen.transform, false);
            RectTransform abRt = answerBtnObj.AddComponent<RectTransform>();
            abRt.anchorMin = new Vector2(0.15f, 0.15f);
            abRt.anchorMax = new Vector2(0.45f, 0.27f);
            abRt.sizeDelta = Vector2.zero;
            Image abImg = answerBtnObj.AddComponent<Image>();
            
            // Try to load RoundedRect.png for sliced rounded button look
            Sprite roundedRectSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/RoundedRect.png");
            if (roundedRectSprite != null)
            {
                abImg.sprite = roundedRectSprite;
                abImg.type = Image.Type.Sliced;
            }
            abImg.color = new Color(0.18f, 0.77f, 0.31f, 1f); // Green
            Button abBtn = answerBtnObj.AddComponent<Button>();
            abBtn.onClick.AddListener(() => AnswerMomCall());

            GameObject abTextObj = new GameObject("Text");
            abTextObj.transform.SetParent(answerBtnObj.transform, false);
            RectTransform abtRt = abTextObj.AddComponent<RectTransform>();
            abtRt.anchorMin = Vector2.zero;
            abtRt.anchorMax = Vector2.one;
            abtRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI abtTxt = abTextObj.AddComponent<TextMeshProUGUI>();
            abtTxt.text = "Nghe";
            abtTxt.fontSize = 16;
            abtTxt.fontStyle = FontStyles.Bold;
            abtTxt.color = Color.white;
            abtTxt.alignment = TextAlignmentOptions.Center;
            abtTxt.verticalAlignment = VerticalAlignmentOptions.Middle;
            abtTxt.raycastTarget = false;

            // Red Decline Button
            GameObject declineBtnObj = new GameObject("DeclineButton");
            declineBtnObj.transform.SetParent(incomingCallScreen.transform, false);
            RectTransform dbRt = declineBtnObj.AddComponent<RectTransform>();
            dbRt.anchorMin = new Vector2(0.55f, 0.15f);
            dbRt.anchorMax = new Vector2(0.85f, 0.27f);
            dbRt.sizeDelta = Vector2.zero;
            Image dbImg = declineBtnObj.AddComponent<Image>();
            if (roundedRectSprite != null)
            {
                dbImg.sprite = roundedRectSprite;
                dbImg.type = Image.Type.Sliced;
            }
            dbImg.color = new Color(0.9f, 0.18f, 0.18f, 1f); // Red
            Button dbBtn = declineBtnObj.AddComponent<Button>();
            dbBtn.onClick.AddListener(() => DeclineMomCall());

            GameObject dbTextObj = new GameObject("Text");
            dbTextObj.transform.SetParent(declineBtnObj.transform, false);
            RectTransform dbtRt = dbTextObj.AddComponent<RectTransform>();
            dbtRt.anchorMin = Vector2.zero;
            dbtRt.anchorMax = Vector2.one;
            dbtRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI dbtTxt = dbTextObj.AddComponent<TextMeshProUGUI>();
            dbtTxt.text = "Tắt máy";
            dbtTxt.fontSize = 16;
            dbtTxt.fontStyle = FontStyles.Bold;
            dbtTxt.color = Color.white;
            dbtTxt.alignment = TextAlignmentOptions.Center;
            dbtTxt.verticalAlignment = VerticalAlignmentOptions.Middle;
            dbtTxt.raycastTarget = false;
        }

        private void AnswerMomCall()
        {
            isRinging = false;
            if (incomingCallScreen != null)
            {
                incomingCallScreen.SetActive(false);
            }
            ClosePhone();

            if (Chapter1Controller.Instance != null)
            {
                Chapter1Controller.Instance.StartMomEmergencyCall();
            }
        }

        private void DeclineMomCall()
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Không được, nhỡ đâu dưới quê mẹ có việc gì quan trọng..." }
                };
                DialogueSystem.Instance.StartDialogue(d);
            }
        }
    }
}

