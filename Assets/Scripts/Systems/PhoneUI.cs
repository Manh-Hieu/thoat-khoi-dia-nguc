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
        
        [Header("Contacts UI")]
        [SerializeField] private GameObject contactGStar;
        [SerializeField] private GameObject contactVinaTech;
        [SerializeField] private GameObject contactAnhHungScam;

        [Header("Chat View UI")]
        [SerializeField] private TextMeshProUGUI chatTitleText;
        [SerializeField] private TextMeshProUGUI chatBodyText;
        [SerializeField] private GameObject momNotificationDot;
        [SerializeField] private GameObject billsNotificationDot;
        [SerializeField] private GameObject messagesAppNotificationDot;

        [Header("Mock App Content Text Fields")]
        [SerializeField] private TextMeshProUGUI notesContentText;
        [SerializeField] private TextMeshProUGUI photosContentText;
        [SerializeField] private TextMeshProUGUI settingsContentText;
        [SerializeField] private TextMeshProUGUI browserContentText;
        [SerializeField] private TextMeshProUGUI weatherContentText;
        [SerializeField] private TextMeshProUGUI phoneContentText;

        [HideInInspector] public bool hasSeenJob1 = false;
        [HideInInspector] public bool hasSeenJob2 = false;
        [HideInInspector] public bool hasSeenScamJob = false;

        private bool hasOpenedMomChat = false;
        private bool hasOpenedBillsChat = false;

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
            HookUpPhoneButtons();
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
        }

        public void OpenPhone()
        {
            if (phonePanel != null)
            {
                CloseAllPhoneScreens();
                phonePanel.SetActive(true);
                homeScreen.SetActive(true);

                // Show cursor and unlock it for UI navigation
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeState(GameState.Cutscene);
                }

                UpdateNotificationDots();
            }
        }

        public void ClosePhone()
        {
            CloseAllPhoneScreens();

            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }
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
                chatBodyText.text = "<align=left><color=#79a6f6><b>Mẹ</b></color> <size=9><color=#666666>15:30</color></size></align>\n" +
                                    "<margin-right=15%><align=left><color=#e5e5ea><size=12>Minh ơi, tháng này dưới quê mất mùa quá, bố con lại đau chân. Con xem có gửi về cho mẹ ít tiền thuốc thang cho bố được không?</size></color></align></margin>\n\n" +
                                    "<align=right><color=#ffaa00><b>Minh</b></color> <size=9><color=#666666>15:32</color></size></align>\n" +
                                    "<margin-left=15%><align=right><color=#d1d1d6><size=12>Lại là tin nhắn xin tiền... Mình đến tiền nhà còn chưa đóng nổi, lấy đâu ra tiền gửi về quê bây giờ?</size></color></align></margin>";
                hasOpenedMomChat = true;
            }
            else if (chatIndex == 2) // Landlord / Bills
            {
                chatTitleText.text = "Ban Quản Lý Trọ";
                chatBodyText.text = "<align=left><color=#79a6f6><b>BQL Trọ</b></color> <size=9><color=#666666>08:15</color></size></align>\n" +
                                    "<margin-right=15%><align=left><color=#e5e5ea><size=12><b>THÔNG BÁO HOÁ ĐƠN THÁNG 6</b>\n• Tiền phòng: 3.000.000đ\n• Điện: 280.000đ\n• Nước: 60.000đ\n• Internet: 160.000đ\n<b>TỔNG CỘNG: 3.500.000đ</b>\nHạn chót đóng: 15/06.</size></color></align></margin>\n\n" +
                                    "<align=right><color=#ffaa00><b>Minh</b></color> <size=9><color=#666666>08:20</color></size></align>\n" +
                                    "<margin-left=15%><align=right><color=#d1d1d6><size=12>Hôm nay đã ngày 11 rồi... Trong tài khoản còn đúng 500k. Kiểu này chắc bị đuổi ra đường mất...</size></color></align></margin>";
                hasOpenedBillsChat = true;
            }

            UpdateNotificationDots();
            CheckPhoneReadProgress();
        }

        public void CloseChat()
        {
            messagesListScreen.SetActive(true);
            chatViewScreen.SetActive(false);
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
            if (messagesAppNotificationDot != null)
            {
                messagesAppNotificationDot.SetActive(!hasOpenedMomChat || !hasOpenedBillsChat);
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
                thinkingText = "Tài khoản điện thoại đã hết sạch tiền từ tuần trước rồi... Không thể gọi cho Mẹ được. Mình phải tìm cách kiếm tiền đóng tiền nhà và gửi về quê gấp.";
            }
            else if (contactName == "Dad")
            {
                thinkingText = "Tài khoản hết tiền... Không thể gọi cho Bố được. Nhìn cuộc gọi nhỡ của bố cách đây 3 ngày mà lòng mình thắt lại. Bố đang đau chân ở quê, chắc lo lắng cho mình lắm...";
            }
            else if (contactName == "Thang")
            {
                thinkingText = "Hết tiền điện thoại rồi, không gọi cho Thắng được. Mà gọi cho nó lúc này cũng chẳng giải quyết được gì, nó cũng đang chật vật tìm việc ở quê.";
            }
            else if (contactName == "Hung")
            {
                thinkingText = "Điện thoại hết tiền rồi. Gọi cho Hùng lúc này chỉ làm phiền nó thêm, ai cũng có nỗi lo riêng...";
            }
            else if (contactName == "GStar")
            {
                thinkingText = "Tài khoản hết tiền, không thể gọi đến G-Star Studio được. Mà kể cả có gọi, vị trí thực tập hỗ trợ 1 triệu/tháng cũng không đủ trả tiền phòng trọ...";
            }
            else if (contactName == "VinaTech")
            {
                thinkingText = "Điện thoại hết tiền rồi, không gọi cho VinaTech được. Với lại họ đòi tận 1 năm kinh nghiệm thực tế tại doanh nghiệp, mình gọi cũng vô ích thôi.";
            }
            else if (contactName == "AnhHungScam")
            {
                thinkingText = "Tài khoản hết tiền... Không gọi cho anh Hùng Bavet được. Nhưng trong tin tuyển dụng ghi rõ là liên hệ Zalo hoặc cứ nhắn tin đồng ý là họ cho xe đón... Có lẽ đây là lối thoát duy nhất của mình.";
            }
            else
            {
                thinkingText = "Tài khoản hết tiền. Không thể thực hiện cuộc gọi lúc này.";
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

            // Bind Chats
            bind("Chat_Mom", () => OpenChat(1));
            bind("Chat_Bills", () => OpenChat(2));

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
    }
}
