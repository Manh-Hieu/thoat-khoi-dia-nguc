using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EscapeFromHell.Core;
using EscapeFromHell.Chapter;

namespace EscapeFromHell.Systems
{
    public class ComputerUI : MonoBehaviour
    {
        public static ComputerUI Instance { get; private set; }

        [Header("UI Windows")]
        [SerializeField] private GameObject computerPanel;
        [SerializeField] private GameObject mailWindow;
        [SerializeField] private GameObject billsWindow;
        [SerializeField] private GameObject thisPCWindow;
        [SerializeField] private GameObject recycleBinWindow;
        [SerializeField] private GameObject chromeWindow;

        [Header("Mail Content UI")]
        [SerializeField] private TextMeshProUGUI emailBodyText;
        [SerializeField] private Button email1Button;
        [SerializeField] private Button email2Button;
        [SerializeField] private Button email3Button;
        [SerializeField] private Button email4Button;

        private bool hasOpenedMail = false;
        private bool hasOpenedBills = false;
        private int currentViewingJobIndex = 0; // 0: none, 1: Job 1, 2: Job 2, 3: Scam Job, 4: Scam Luck
        private GameObject startMenuPanel;
        private bool hasViewedJob1 = false;
        private bool hasViewedJob2 = false;
        private bool hasViewedScamLuck = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initially hide computer and child windows
            if (computerPanel != null) computerPanel.SetActive(false);
            CloseAllSubWindows();
        }

        private void Start()
        {
            // Set up email buttons click listeners
            if (email1Button != null)
            {
                email1Button.onClick.AddListener(() => ShowEmail(1));
            }
            if (email2Button != null)
            {
                email2Button.onClick.AddListener(() => ShowEmail(2));
            }
            if (email3Button != null)
            {
                email3Button.onClick.AddListener(() => ShowEmail(3));
            }
            if (email4Button != null)
            {
                email4Button.onClick.AddListener(() => ShowEmail(4));
            }

            // Initialize the Start Menu panel dynamically
            InitializeStartMenu();

            // Hook up all desktop and window buttons at runtime
            HookUpComputerButtons();
        }

        public void OpenComputer()
        {
            if (computerPanel != null)
            {
                computerPanel.SetActive(true);
                
                // Show cursor and unlock it for UI navigation
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.ChangeState(GameState.Cutscene);
                }

                // Hide subwindows initially
                CloseAllSubWindows();

                // Show default email text
                if (emailBodyText != null)
                {
                    emailBodyText.text = "Chọn một email từ danh sách bên trái để đọc...";
                }
            }
        }

        public bool IsComputerOpen => computerPanel != null && computerPanel.activeSelf;

        public void CloseComputer()
        {
            if (computerPanel != null)
            {
                computerPanel.SetActive(false);

                // Re-hide cursor if needed (depending on game state, standard top-down top view might keep cursor visible but here we return control)
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
            }

            if (Chapter1Controller.Instance != null)
            {
                Chapter1Controller.Instance.OnUIClosed();
            }
        }

        private void CloseAllSubWindows()
        {
            if (mailWindow != null) mailWindow.SetActive(false);
            if (billsWindow != null) billsWindow.SetActive(false);
            if (thisPCWindow != null) thisPCWindow.SetActive(false);
            if (recycleBinWindow != null) recycleBinWindow.SetActive(false);
            if (chromeWindow != null) chromeWindow.SetActive(false);
            if (startMenuPanel != null) startMenuPanel.SetActive(false);
        }

        public void OpenMail()
        {
            if (mailWindow != null)
            {
                CloseAllSubWindows();
                mailWindow.SetActive(true);

                hasOpenedMail = true;
                CheckComputerReadProgress();
            }
        }

        public void CloseMail()
        {
            if (mailWindow != null)
            {
                mailWindow.SetActive(false);
            }
        }

        public void OpenBills()
        {
            if (billsWindow != null)
            {
                CloseAllSubWindows();
                billsWindow.SetActive(true);

                hasOpenedBills = true;
                CheckComputerReadProgress();
            }
        }

        public void CloseBills()
        {
            if (billsWindow != null)
            {
                billsWindow.SetActive(false);
            }
        }

        public void OpenThisPC()
        {
            if (thisPCWindow != null)
            {
                CloseAllSubWindows();
                thisPCWindow.SetActive(true);
            }
        }

        public void CloseThisPC()
        {
            if (thisPCWindow != null)
            {
                thisPCWindow.SetActive(false);
            }
        }

        public void OpenRecycleBin()
        {
            if (recycleBinWindow != null)
            {
                CloseAllSubWindows();
                recycleBinWindow.SetActive(true);
            }
        }

        public void CloseRecycleBin()
        {
            if (recycleBinWindow != null)
            {
                recycleBinWindow.SetActive(false);
            }
        }

        public void OpenChrome()
        {
            if (chromeWindow != null)
            {
                CloseAllSubWindows();
                chromeWindow.SetActive(true);
                ShowChromeSearchResults();
            }
        }

        public void CloseChrome()
        {
            if (chromeWindow != null)
            {
                chromeWindow.SetActive(false);
            }
        }

        public void ClickRecycleBinFile(string fileName)
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                string descriptiveText = "";
                if (fileName == "graduation")
                {
                    descriptiveText = "Ảnh lễ tốt nghiệp đại học của mình... Mình đã vứt nó vào đây. Cầm tấm bằng tốt nghiệp loại khá trên tay mà đến giờ vẫn thất nghiệp, nhìn nó chỉ thêm cay đắng.";
                }
                else if (fileName == "cv")
                {
                    descriptiveText = "Bản CV cũ nháp: 'Minh_CV_Fresher_v12.pdf'. Đã gửi rải khắp các công ty tuyển dụng từ đầu năm đến nay nhưng đều bặt vô âm tín.";
                }
                else
                {
                    descriptiveText = "Tệp tin đã bị xóa. Không thể mở trực tiếp từ Thùng rác.";
                }
                textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = descriptiveText }
                };
                DialogueSystem.Instance.StartDialogue(textMock);
            }
        }

        public void ClickThisPCDrive()
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Ổ đĩa C: chỉ còn trống 1.2 GB trên tổng số 120 GB. Laptop rẻ tiền mua từ hồi năm nhất đại học giờ đã quá rệu rã..." }
                };
                DialogueSystem.Instance.StartDialogue(textMock);
            }
        }

        public void ShowChromeSearchResults()
        {
            if (chromeWindow == null) return;

            SetAddressText("<color=#5cb85c>• Secure</color> | https://topcv.vn/tim-kiem?q=unity+fresher");

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=11><color=#9aa0a6>Khoảng 124.000 kết quả (0,32 giây)</color></size><br><br>" +
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
                                 "• Mức lương: 1.000.000đ - 1.500.000đ/ngày (Bao ăn ở toàn bộ, xe đưa đón).<br><br>" +
                                 "<color=#ff3366><size=13><b>4. [ĐỔI VẬN] Bạn Đang Gặp Xui Xẻo? Không Tìm Được Việc Làm? Click Ngay!</b></size></color><br>" +
                                 "<color=#9aa0a6><size=10>https://thaydoivanmenh.com.vn/dich-vu-giai-han</size></color><br>" +
                                 "• Bạn cảm thấy bế tắc vì thất nghiệp, nợ nần chồng chất? Chúng tôi có giải pháp thay đổi vận mệnh của bạn ngay lập tức.<br>" +
                                 "• Đăng ký dịch vụ giải hạn tâm linh đặc biệt giúp tăng cơ hội trúng tuyển lên 100%.<br><br>" +
                                 "<color=#ffcc00><i>Cảnh báo thị trường:</i></color> <i>Thị trường tuyển dụng IT năm 2026 đang có mức độ cạnh tranh rất cao. Cảnh giác với các tin tuyển dụng việc nhẹ lương cao ở nước ngoài, nạp tiền cọc làm nhiệm vụ.</i>";
                }
            }

            // Set all Job buttons active, ChromeBackButton inactive
            Transform job1Btn = chromeWindow.transform.Find("Content/Job1Button");
            if (job1Btn != null) job1Btn.gameObject.SetActive(true);

            Transform job2Btn = chromeWindow.transform.Find("Content/Job2Button");
            if (job2Btn != null) job2Btn.gameObject.SetActive(true);

            Transform scamBtn = chromeWindow.transform.Find("Content/ScamJobButton");
            if (scamBtn != null) scamBtn.gameObject.SetActive(true);

            Transform scamLuckBtn = chromeWindow.transform.Find("Content/ScamLuckButton");
            if (scamLuckBtn != null) scamLuckBtn.gameObject.SetActive(true);

            Transform casinoSearchBtn = chromeWindow.transform.Find("Content/CasinoSearchButton");
            if (casinoSearchBtn != null) casinoSearchBtn.gameObject.SetActive(false);

            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(false);

            currentViewingJobIndex = 0;
            Transform saveContactBtn = chromeWindow.transform.Find("Content/SaveContactButton");
            if (saveContactBtn != null) saveContactBtn.gameObject.SetActive(false);
        }

        private void HideAllChromeJobButtons()
        {
            if (chromeWindow == null) return;
            Transform job1Btn = chromeWindow.transform.Find("Content/Job1Button");
            if (job1Btn != null) job1Btn.gameObject.SetActive(false);

            Transform job2Btn = chromeWindow.transform.Find("Content/Job2Button");
            if (job2Btn != null) job2Btn.gameObject.SetActive(false);

            Transform scamBtn = chromeWindow.transform.Find("Content/ScamJobButton");
            if (scamBtn != null) scamBtn.gameObject.SetActive(false);

            Transform scamLuckBtn = chromeWindow.transform.Find("Content/ScamLuckButton");
            if (scamLuckBtn != null) scamLuckBtn.gameObject.SetActive(false);

            Transform casinoSearchBtn = chromeWindow.transform.Find("Content/CasinoSearchButton");
            if (casinoSearchBtn != null) casinoSearchBtn.gameObject.SetActive(false);
        }

        public void ClickJob1()
        {
            currentViewingJobIndex = 1;
            if (chromeWindow == null) return;

            SetAddressText("<color=#5cb85c>• Secure</color> | https://joygamestudio.vn/tuyen-dung/intern-unity");

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=13><b>CHI TIẾT TUYỂN DỤNG: THỰC TẬP SINH UNITY DEVELOPER</b></size><br>" +
                                 "<b>Công ty:</b> JoyGame Game Studio<br>" +
                                 "<b>Địa điểm:</b> Quận 7, TP. Hồ Chí Minh<br>" +
                                 "<b>Mức lương hỗ trợ:</b> <color=#33cc66>1.000.000đ/tháng</color> (Thử việc không lương 2 tháng)<br>" +
                                 "------------------------------------------------------------------<br>" +
                                 "<b>Mô tả công việc:</b><br>" +
                                 "• Hỗ trợ lập trình game Unity (2D/3D) dưới sự hướng dẫn của Senior.<br>" +
                                 "• Viết mã nguồn game cơ bản, sửa lỗi gameplay logic đơn giản.<br>" +
                                 "• Tham gia kiểm thử game và tối ưu hóa hiệu năng sản phẩm.<br><br>" +
                                 "<b>Yêu cầu ứng viên:</b><br>" +
                                 "• Có sản phẩm game tự làm (Demo/GitHub link).<br>" +
                                 "• Hiểu rõ ngôn ngữ C# và cấu trúc dữ liệu, giải thuật cơ bản.<br>" +
                                 "• Chăm chỉ, có khả năng làm việc nhóm tốt.<br><br>" +
                                 "<b>Thông tin liên hệ tuyển dụng:</b><br>" +
                                 "• Zalo/Hotline: <color=#ff9900><b>028.3333.5555</b></color> (Phòng Nhân Sự - JoyGame Studio)<br>" +
                                 "• Email: hr@joygamestudio.vn";
                }
            }

            HideAllChromeJobButtons();

            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);

            UpdateSaveContactButtonState();

            if (!hasViewedJob1)
            {
                hasViewedJob1 = true;
                if (DialogueSystem.Instance != null)
                {
                    DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                    textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = "Thực tập sinh Unity ở JoyGame Studio... Lương hỗ trợ có 1 triệu một tháng lại còn bắt thử việc không lương 2 tháng." },
                        new DialogueLine { speakerName = "Minh", text = "Đúng là bóc lột mà, làm sao mình đủ trả tiền phòng trọ đây... Có số điện thoại liên lạc ở đây: 028.3333.5555." }
                    };
                    DialogueSystem.Instance.StartDialogue(textMock);
                }
            }
        }

        public void ClickJob2()
        {
            currentViewingJobIndex = 2;
            if (chromeWindow == null) return;

            SetAddressText("<color=#5cb85c>• Secure</color> | https://novatech.com/careers/web-fresher");

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=13><b>CHI TIẾT TUYỂN DỤNG: LẬP TRÌNH VIÊN WEB (FRESHER)</b></size><br>" +
                                 "<b>Công ty:</b> NovaTech Group<br>" +
                                 "<b>Địa điểm:</b> Quận Cầu Giấy, Hà Nội<br>" +
                                 "<b>Mức lương:</b> <color=#33cc66>6.000.000đ - 8.000.000đ/tháng</color> (Phỏng vấn kỹ thuật 3 vòng)<br>" +
                                 "------------------------------------------------------------------<br>" +
                                 "<b>Mô tả công việc:</b><br>" +
                                 "• Tham gia lập trình front-end (React/HTML/CSS) hoặc back-end (Node.js).<br>" +
                                 "• Thiết kế giao diện responsive tương thích với nhiều thiết bị.<br>" +
                                 "• Phối hợp với đội ngũ Designer và BA để hoàn thiện website dự án.<br><br>" +
                                 "<b>Yêu cầu ứng viên:</b><br>" +
                                 "• Có tối thiểu 1 năm kinh nghiệm làm việc thực tế tại doanh nghiệp.<br>" +
                                 "• Sử dụng tốt Git, HTML, CSS, Javascript (hoặc Typescript).<br>" +
                                 "• Ưu tiên ứng viên có kiến thức về React, Angular hoặc Node.js.<br><br>" +
                                 "<b>Thông tin liên hệ tuyển dụng:</b><br>" +
                                 "• Zalo/Hotline: <color=#ff9900><b>024.9999.8888</b></color> (Phòng Tuyển Dụng - NovaTech Group)<br>" +
                                 "• Email: careers@novatech.com";
                }
            }

            HideAllChromeJobButtons();

            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);

            UpdateSaveContactButtonState();

            if (!hasViewedJob2)
            {
                hasViewedJob2 = true;
                if (DialogueSystem.Instance != null)
                {
                    DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                    textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = "Tuyển Fresher Web ở NovaTech lương 6 đến 8 triệu, nhưng lại đòi hỏi tận 1 năm kinh nghiệm làm việc thực tế tại doanh nghiệp..." },
                        new DialogueLine { speakerName = "Minh", text = "Đã ghi là Fresher mà lại đòi 1 năm kinh nghiệm thì sao mình ứng tuyển nổi đây... Số điện thoại phòng nhân sự của họ: 024.9999.8888." }
                    };
                    DialogueSystem.Instance.StartDialogue(textMock);
                }
            }
        }

        public void ClickScamJob()
        {
            if (chromeWindow == null) return;

            HideAllChromeJobButtons();

            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.OnDialogueEnd += OnScamJobDialogueEnd;

                DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Nhìn đã biết là công việc lừa đảo, mình nên tìm những công việc khác." }
                };
                DialogueSystem.Instance.StartDialogue(textMock);
            }
        }

        private void OnScamJobDialogueEnd()
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.OnDialogueEnd -= OnScamJobDialogueEnd;
            }
            ShowChromeSearchResults();
        }

        public void ClickScamLuck()
        {
            currentViewingJobIndex = 4;
            if (chromeWindow == null) return;

            SetAddressText("<color=#ff9900>• Secure</color> | https://thaydoivanmenh.com.vn/dich-vu-giai-han");

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=13><b>PHONG THỦY CẢI VẬN - CHỈ 100K CÓ NGAY MAY MẮN</b></size><br>" +
                                 "<b>Dịch vụ:</b> Giải Hạn Phong Thủy Cải Vận Cấp Tốc<br>" +
                                 "<b>Cam kết:</b> Đổi đời ngay lập tức, may mắn gõ cửa 10 lần/ngày<br>" +
                                 "<b>Chi phí dịch vụ:</b> <color=#33cc66>100.000đ</color><br>" +
                                 "------------------------------------------------------------------<br>" +
                                 "<b>Mô tả dịch vụ:</b><br>" +
                                 "• Bạn đang gặp xui xẻo? Nợ nần chồng chất? Tìm việc mãi không thành công?<br>" +
                                 "• Chỉ cần chuyển 100k vào số tài khoản cải vận dưới đây, bạn sẽ được khai quang vận mệnh, loại bỏ toàn bộ năng lượng tiêu cực.<br>" +
                                 "• Tiền tài, vận may và công việc như ý sẽ tự động tìm đến bạn ngay trong ngày!<br><br>" +
                                 "<b>Thông tin chuyển khoản thụ hưởng:</b><br>" +
                                 "• Số tài khoản: <color=#ff9900><b>666688889999</b></color><br>" +
                                 "• Ngân hàng: VIETCOMBANK (VCB)<br>" +
                                 "• Tên thụ hưởng: DV THAY DOI VAN MENH";
                }
            }

            HideAllChromeJobButtons();

            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);

            UpdateSaveContactButtonState();

            if (!hasViewedScamLuck)
            {
                hasViewedScamLuck = true;
                if (DialogueSystem.Instance != null)
                {
                    DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                    textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = "Hóa giải vận xui bằng cách chuyển khoản 100k? Nghe có vẻ hoang đường quá..." },
                        new DialogueLine { speakerName = "Minh", text = "Nhưng dạo này mình xui xẻo thật sự. Nợ nần ngập đầu, thất nghiệp. Hay là thử xem sao... Số tài khoản VCB: 666688889999." }
                    };
                    DialogueSystem.Instance.StartDialogue(textMock);
                }
            }
        }

        private void ShowEmail(int emailIndex)
        {
            if (emailBodyText == null) return;

            if (emailIndex == 1)
            {
                emailBodyText.text = "<b>Từ:</b> HR Công ty Công nghệ VinaTech<br>" +
                                    "<b>Tiêu đề:</b> Thư từ chối tuyển dụng - Fresher Web<br>" +
                                    "------------------------------------------<br><br>" +
                                    "Chào Minh,<br>" +
                                    "Cảm ơn bạn đã quan tâm và ứng tuyển vào vị trí Lập trình viên Web mới ra trường.<br><br>" +
                                    "Qua đánh giá sơ bộ hồ sơ, chúng tôi rất tiếc phải thông báo bạn chưa phù hợp với các tiêu chí tuyển dụng hiện tại (yêu cầu tối thiểu 2 năm kinh nghiệm làm việc thực tế).<br><br>" +
                                    "Hồ sơ của bạn sẽ được lưu giữ trong cơ sở dữ liệu của chúng tôi cho các đợt tuyển dụng sau.<br><br>" +
                                    "Chúc bạn nhiều sức khỏe và may mắn trên con đường sự nghiệp của mình.<br><br>" +
                                    "Trân trọng,<br>" +
                                    "Bộ phận Tuyển dụng VinaTech.";
            }
            else if (emailIndex == 2)
            {
                emailBodyText.text = "<b>Từ:</b> HR Studio Game G-Star<br>" +
                                    "<b>Tiêu đề:</b> Thông báo kết quả ứng tuyển - Unity Junior<br>" +
                                    "------------------------------------------<br><br>" +
                                    "Chào bạn,<br>" +
                                    "Rất tiếc phải thông báo rằng vị trí ứng tuyển Unity Junior Developer của bạn tại G-Star đã đủ số lượng.<br><br>" +
                                    "Do thị trường IT hiện tại đang biến động mạnh, số lượng hồ sơ nộp về rất lớn và chúng tôi bắt buộc phải ưu tiên các ứng viên có kinh nghiệm thực chiến từ trước.<br><br>" +
                                    "Chúc bạn sớm tìm được công việc ưng ý.<br><br>" +
                                    "Thân ái,<br>" +
                                    "G-Star HR Team.";
            }
            else if (emailIndex == 3)
            {
                emailBodyText.text = "<b>Từ:</b> HR FPT Software<br>" +
                                    "<b>Tiêu đề:</b> Kết quả bài kiểm tra thuật toán - Thực tập sinh C++<br>" +
                                    "------------------------------------------<br><br>" +
                                    "Chào bạn,<br>" +
                                    "Cảm ơn bạn đã tham gia bài kiểm tra thuật toán đầu vào tại FPT Software.<br><br>" +
                                    "Chúng tôi rất tiếc phải báo điểm số của bạn chưa đạt chuẩn tuyển dụng của kỳ thực tập này (Yêu cầu: 80/100, Điểm của bạn: 55/100).<br><br>" +
                                    "Bạn có thể đăng ký thi lại bài kiểm tra sau thời gian tự học 6 tháng.<br><br>" +
                                    "Chúc bạn nhiều sức khỏe và học tập tốt.<br><br>" +
                                    "Trân trọng,<br>" +
                                    "Ban Tuyển dụng FPT Software.";
            }
            else if (emailIndex == 4)
            {
                emailBodyText.text = "<b>Từ:</b> HR VNG Corporation<br>" +
                                    "<b>Tiêu đề:</b> Thư từ chối phỏng vấn - Web Game Developer (Fresher)<br>" +
                                    "------------------------------------------<br><br>" +
                                    "Chào bạn,<br>" +
                                    "Cảm ơn bạn đã dành thời gian tham gia vòng phỏng vấn kỹ thuật tại VNG.<br><br>" +
                                    "Qua xem xét đánh giá từ hội đồng kỹ thuật, kỹ năng lập trình web của bạn tốt nhưng chúng tôi đang cần ứng viên có kinh nghiệm thực chiến với Node.js, Socket.io và cơ sở dữ liệu NoSQL lớn để vận hành dự án game ngay lập tức.<br><br>" +
                                    "Chúng tôi rất tiếc chưa thể hợp tác cùng bạn trong đợt tuyển dụng này.<br><br>" +
                                    "Chúc bạn sớm tìm được cơ hội phù hợp.<br><br>" +
                                    "Thân ái,<br>" +
                                    "VNG Recruitment Team.";
            }
        }

        private void CheckComputerReadProgress()
        {
            if (Chapter1Controller.Instance != null)
            {
                if (hasOpenedMail)
                {
                    Chapter1Controller.Instance.ReadLaptopFromComputer();
                }
                if (hasOpenedBills)
                {
                    Chapter1Controller.Instance.ReadBillsFromComputer();
                }
            }
        }

        private void Update()
        {
            if (computerPanel != null && computerPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseComputer();
                }
            }
        }

        private void HookUpComputerButtons()
        {
            if (computerPanel == null) return;

            // Helper to find button and add listener
            System.Action<string, UnityEngine.Events.UnityAction> bind = (path, action) =>
            {
                Transform t = computerPanel.transform.Find(path);
                if (t != null)
                {
                    Button btn = t.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(action);
                    }
                }
                else
                {
                    // Fallback recursive search by short name (last part of path)
                    string[] parts = path.Split('/');
                    string shortName = parts[parts.Length - 1];
                    Button btn = FindButtonRecursive(computerPanel.transform, shortName);
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(action);
                    }
                    else
                    {
                        Debug.LogError("Failed to bind button: " + path);
                    }
                }
            };

            // Bind Desktop Icons
            bind("Workspace/IconsContainer/ThisPCIcon", () => OpenThisPC());
            bind("Workspace/IconsContainer/RecycleBinIcon", () => OpenRecycleBin());
            bind("Workspace/IconsContainer/MailIcon", () => OpenMail());
            bind("Workspace/IconsContainer/BillsIcon", () => OpenBills());
            bind("Workspace/IconsContainer/ChromeIcon", () => OpenChrome());

            // Bind Taskbar Shortcuts
            bind("Workspace/Taskbar/CenteredContainer/StartButton", () => ToggleStartMenu());
            bind("Workspace/Taskbar/CenteredContainer/ExplorerShortcut", () => OpenThisPC());
            bind("Workspace/Taskbar/CenteredContainer/ChromeShortcut", () => OpenChrome());
            bind("Workspace/Taskbar/CenteredContainer/MailShortcut", () => OpenMail());
            bind("Workspace/Taskbar/ShutdownButton", () => CloseComputer());

            // Bind Window Headers Close Buttons
            bind("Workspace/ThisPCWindow/HeaderBar/CloseButton", () => CloseThisPC());
            bind("Workspace/RecycleBinWindow/HeaderBar/CloseButton", () => CloseRecycleBin());
            bind("Workspace/ChromeWindow/HeaderBar/CloseButton", () => CloseChrome());
            bind("Workspace/MailWindow/HeaderBar/CloseButton", () => CloseMail());
            bind("Workspace/BillsWindow/HeaderBar/CloseButton", () => CloseBills());

            // Bind Inner Window Interaction Buttons
            bind("Workspace/ThisPCWindow/Content/DriveCButton", () => ClickThisPCDrive());
            bind("Workspace/RecycleBinWindow/Content/BinFile_CV", () => ClickRecycleBinFile("cv"));
            bind("Workspace/RecycleBinWindow/Content/BinFile_Grad", () => ClickRecycleBinFile("graduation"));
            bind("Workspace/ChromeWindow/Content/Job1Button", () => ClickJob1());
            bind("Workspace/ChromeWindow/Content/Job2Button", () => ClickJob2());
            bind("Workspace/ChromeWindow/Content/ScamJobButton", () => ClickScamJob());
            bind("Workspace/ChromeWindow/Content/ScamLuckButton", () => ClickScamLuck());
            bind("Workspace/ChromeWindow/Content/ChromeBackButton", () => OnChromeBackButtonClicked());
            bind("Workspace/ChromeWindow/Content/CasinoSearchButton", () => OpenCasinoDetailsPage());
            bind("Workspace/ChromeWindow/Content/SaveContactButton", () => SaveCurrentJobContact());

            // Bind Chrome Address Box input field
            Transform addrBoxTrans = computerPanel.transform.Find("Workspace/ChromeWindow/UrlBar/AddressBox");
            if (addrBoxTrans != null)
            {
                TMP_InputField inputField = addrBoxTrans.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    inputField.onSubmit.RemoveAllListeners();
                    inputField.onSubmit.AddListener(OnAddressBarSubmit);

                    inputField.onEndEdit.RemoveAllListeners();
                    inputField.onEndEdit.AddListener((val) => {
                        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
                        {
                            OnAddressBarSubmit(val);
                        }
                    });

                    inputField.onValueChanged.RemoveAllListeners();
                    inputField.onValueChanged.AddListener((val) => {
                        if (string.IsNullOrEmpty(val.Trim()))
                        {
                            ShowChromeSearchResults();
                        }
                    });

                    inputField.onSelect.RemoveAllListeners();
                    inputField.onSelect.AddListener((val) => {
                        // When clicked/selected, strip out rich text and prefixes to make it easy to edit
                        string cleanText = inputField.text;
                        if (cleanText.Contains("|"))
                        {
                            int idx = cleanText.IndexOf('|');
                            cleanText = cleanText.Substring(idx + 1).Trim();
                        }
                        cleanText = System.Text.RegularExpressions.Regex.Replace(cleanText, "<.*?>", "");
                        inputField.text = cleanText;

                        // Select all
                        inputField.Select();
                    });
                }
            }

            // Bind email buttons by name recursively
            bind("Thư từ chối 1", () => ShowEmail(1));
            bind("Thư từ chối 2", () => ShowEmail(2));
            bind("Thư từ chối 3", () => ShowEmail(3));
            bind("Thư từ chối 4", () => ShowEmail(4));
        }

        private void UpdateSaveContactButtonState()
        {
            if (chromeWindow == null) return;
            Transform saveBtnTrans = chromeWindow.transform.Find("Content/SaveContactButton");
            if (saveBtnTrans == null) return;

            saveBtnTrans.gameObject.SetActive(true);
            Button btn = saveBtnTrans.GetComponent<Button>();
            Transform txtTrans = saveBtnTrans.Find("Text");
            TextMeshProUGUI txt = txtTrans != null ? txtTrans.GetComponent<TextMeshProUGUI>() : null;
            Image img = saveBtnTrans.GetComponent<Image>();

            if (btn == null) return;

            bool alreadySaved = false;
            if (PhoneUI.Instance != null)
            {
                if (currentViewingJobIndex == 1) alreadySaved = PhoneUI.Instance.hasSeenJob1;
                else if (currentViewingJobIndex == 2) alreadySaved = PhoneUI.Instance.hasSeenJob2;
                else if (currentViewingJobIndex == 3) alreadySaved = PhoneUI.Instance.hasSeenScamJob;
                else if (currentViewingJobIndex == 4) alreadySaved = PhoneUI.Instance.hasSavedScamLuck;
            }

            if (alreadySaved)
            {
                btn.interactable = false;
                if (txt != null) txt.text = currentViewingJobIndex == 4 ? "✓ Đã lưu tài khoản" : "✓ Đã lưu số";
                if (img != null) img.color = new Color(0.25f, 0.25f, 0.28f, 1f); // Grey
            }
            else
            {
                btn.interactable = true;
                if (txt != null) txt.text = currentViewingJobIndex == 4 ? "Lưu vào ngân hàng điện thoại" : "Lưu số điện thoại";
                if (img != null) img.color = new Color(0.1f, 0.45f, 0.8f, 1f); // Blue
            }
        }

        public void SaveCurrentJobContact()
        {
            if (PhoneUI.Instance == null) return;

            string contactName = "";
            bool isLuckScam = (currentViewingJobIndex == 4);

            if (isLuckScam)
            {
                bool ready = (Chapter1Controller.Instance != null && Chapter1Controller.Instance.IsReadyToGoToCasino);
                if (!ready)
                {
                    if (DialogueSystem.Instance != null)
                    {
                        DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                        textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                            new DialogueLine { speakerName = "Minh", text = "Mấy cái trò phong thủy giải hạn online này toàn là mê tín dị đoan lừa đảo thôi, mình không nên lưu làm gì..." }
                        };
                        DialogueSystem.Instance.StartDialogue(textMock);
                    }
                    return;
                }
            }

            if (currentViewingJobIndex == 1)
            {
                PhoneUI.Instance.hasSeenJob1 = true;
                contactName = "JoyGame Studio (Tuyển dụng)";
            }
            else if (currentViewingJobIndex == 2)
            {
                PhoneUI.Instance.hasSeenJob2 = true;
                contactName = "NovaTech Group (Tuyển dụng)";
            }
            else if (currentViewingJobIndex == 3)
            {
                PhoneUI.Instance.hasSeenScamJob = true;
                contactName = "Anh Hùng (Bavet Campuchia)";
            }
            else if (currentViewingJobIndex == 4)
            {
                PhoneUI.Instance.hasSavedScamLuck = true;
                contactName = "DV THAY DOI VAN MENH";
            }

            if (!string.IsNullOrEmpty(contactName))
            {
                // Update button visual state immediately
                UpdateSaveContactButtonState();

                // Trigger confirmation monologue
                if (DialogueSystem.Instance != null)
                {
                    DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                    string confirmText = isLuckScam
                        ? "Đã lưu số tài khoản của dịch vụ cải vận VCB: 666688889999 vào danh sách thụ hưởng lưu sẵn của ứng dụng MB Bank."
                        : $"Đã lưu số điện thoại của {contactName} vào danh bạ trên di động.";
                    textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = confirmText }
                    };
                    DialogueSystem.Instance.StartDialogue(textMock);
                }
            }
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

        private void InitializeStartMenu()
        {
            if (computerPanel == null || startMenuPanel != null) return;

            // Find Workspace transform
            Transform workspaceTrans = computerPanel.transform.Find("Workspace");
            if (workspaceTrans == null) return;

            // Hide the old taskbar shutdown button
            Transform oldShutdown = workspaceTrans.Find("Taskbar/ShutdownButton");
            if (oldShutdown != null)
            {
                oldShutdown.gameObject.SetActive(false);
            }

            // Create StartMenuPanel
            startMenuPanel = new GameObject("StartMenuPanel");
            startMenuPanel.transform.SetParent(workspaceTrans, false);
            
            RectTransform smRect = startMenuPanel.AddComponent<RectTransform>();
            smRect.anchorMin = new Vector2(0f, 0f);
            smRect.anchorMax = new Vector2(0f, 0f);
            smRect.pivot = new Vector2(0f, 0f);
            smRect.sizeDelta = new Vector2(280, 320);
            // Position above taskbar (taskbar is 40px high, let's place it at x=10, y=45)
            smRect.anchoredPosition = new Vector2(10, 45);

            // Add background image
            Image bg = startMenuPanel.AddComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.1f, 0.96f);
            
            // Add rounded outline/border
            Outline outline = startMenuPanel.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.12f);
            outline.effectDistance = new Vector2(1.5f, 1.5f);

            // --- Top Header: User Profile ---
            GameObject userProfileObj = new GameObject("UserProfile");
            userProfileObj.transform.SetParent(startMenuPanel.transform, false);
            RectTransform upRect = userProfileObj.AddComponent<RectTransform>();
            upRect.anchorMin = new Vector2(0f, 1f);
            upRect.anchorMax = new Vector2(1f, 1f);
            upRect.pivot = new Vector2(0.5f, 1f);
            upRect.sizeDelta = new Vector2(0, 60);
            upRect.anchoredPosition = new Vector2(0, 0);

            // Subtle dark separator at bottom of profile
            Image upImg = userProfileObj.AddComponent<Image>();
            upImg.color = new Color(1f, 1f, 1f, 0.04f);

            // Avatar Circle/Box
            GameObject avatarObj = new GameObject("Avatar");
            avatarObj.transform.SetParent(userProfileObj.transform, false);
            RectTransform avRect = avatarObj.AddComponent<RectTransform>();
            avRect.anchorMin = new Vector2(0f, 0.5f);
            avRect.anchorMax = new Vector2(0f, 0.5f);
            avRect.pivot = new Vector2(0f, 0.5f);
            avRect.sizeDelta = new Vector2(36, 36);
            avRect.anchoredPosition = new Vector2(15, 0);
            Image avImg = avatarObj.AddComponent<Image>();
            avImg.color = new Color(0.18f, 0.48f, 0.76f, 1f); // Blue avatar background
            Outline avOutline = avatarObj.AddComponent<Outline>();
            avOutline.effectColor = new Color(1f, 1f, 1f, 0.2f);

            // Avatar text (Minh's initials: "M")
            GameObject avTextObj = new GameObject("Text");
            avTextObj.transform.SetParent(avatarObj.transform, false);
            RectTransform avtRect = avTextObj.AddComponent<RectTransform>();
            avtRect.anchorMin = Vector2.zero;
            avtRect.anchorMax = Vector2.one;
            avtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI avtText = avTextObj.AddComponent<TextMeshProUGUI>();
            avtText.text = "M";
            avtText.fontSize = 18;
            avtText.fontStyle = FontStyles.Bold;
            avtText.color = Color.white;
            avtText.alignment = TextAlignmentOptions.Center;
            avtText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // Username Label
            GameObject nameObj = new GameObject("Username");
            nameObj.transform.SetParent(userProfileObj.transform, false);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0f, 0.5f);
            nameRect.anchorMax = new Vector2(1f, 0.5f);
            nameRect.pivot = new Vector2(0f, 0.5f);
            nameRect.sizeDelta = new Vector2(-80, 30);
            nameRect.anchoredPosition = new Vector2(65, 0);
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Nguyễn Hoài Minh";
            nameText.fontSize = 13;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = Color.white;
            nameText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // --- Middle Body: Quick Links ---
            string[] items = { "Tài liệu (Documents)", "Ảnh (Pictures)", "Cài đặt (Settings)", "Trợ giúp & Hỗ trợ (Help)" };
            for (int i = 0; i < items.Length; i++)
            {
                int index = i;
                GameObject itemObj = new GameObject("Link_" + i);
                itemObj.transform.SetParent(startMenuPanel.transform, false);
                RectTransform itRect = itemObj.AddComponent<RectTransform>();
                itRect.anchorMin = new Vector2(0f, 1f);
                itRect.anchorMax = new Vector2(1f, 1f);
                itRect.pivot = new Vector2(0.5f, 1f);
                itRect.sizeDelta = new Vector2(-20, 32);
                itRect.anchoredPosition = new Vector2(0, -75 - (i * 38));

                Image itImg = itemObj.AddComponent<Image>();
                itImg.color = Color.clear;
                Button itBtn = itemObj.AddComponent<Button>();
                itBtn.targetGraphic = itImg;
                
                ColorBlock cb = itBtn.colors;
                cb.normalColor = Color.clear;
                cb.highlightedColor = new Color(1f, 1f, 1f, 0.05f);
                cb.pressedColor = new Color(1f, 1f, 1f, 0.1f);
                cb.selectedColor = Color.clear;
                itBtn.colors = cb;

                GameObject itTextObj = new GameObject("Text");
                itTextObj.transform.SetParent(itemObj.transform, false);
                RectTransform ittRect = itTextObj.AddComponent<RectTransform>();
                ittRect.anchorMin = Vector2.zero;
                ittRect.anchorMax = Vector2.one;
                ittRect.offsetMin = new Vector2(10, 0);
                ittRect.offsetMax = Vector2.zero;
                TextMeshProUGUI ittText = itTextObj.AddComponent<TextMeshProUGUI>();
                ittText.text = items[i];
                ittText.fontSize = 11;
                ittText.color = new Color(0.85f, 0.85f, 0.85f, 1f);
                ittText.verticalAlignment = VerticalAlignmentOptions.Middle;
                
                // Clicking mock links plays a tiny monologue
                itBtn.onClick.AddListener(() => {
                    if (DialogueSystem.Instance != null)
                    {
                        DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                        d.lines = new System.Collections.Generic.List<DialogueLine> {
                            new DialogueLine { speakerName = "Minh", text = $"Không có gì hữu ích trong thư mục này cả... Mình nên tập trung tìm việc." }
                        };
                        DialogueSystem.Instance.StartDialogue(d);
                    }
                });
            }

            // --- Bottom Footer: Shutdown Panel ---
            GameObject footerObj = new GameObject("Footer");
            footerObj.transform.SetParent(startMenuPanel.transform, false);
            RectTransform ftRect = footerObj.AddComponent<RectTransform>();
            ftRect.anchorMin = new Vector2(0f, 0f);
            ftRect.anchorMax = new Vector2(1f, 0f);
            ftRect.pivot = new Vector2(0.5f, 0f);
            ftRect.sizeDelta = new Vector2(0, 50);
            ftRect.anchoredPosition = Vector2.zero;

            Image ftImg = footerObj.AddComponent<Image>();
            ftImg.color = new Color(1f, 1f, 1f, 0.02f);

            // Shutdown Button in Start Menu
            GameObject shutdownBtnObj = new GameObject("ShutdownButtonMenu");
            shutdownBtnObj.transform.SetParent(footerObj.transform, false);
            RectTransform sdbRect = shutdownBtnObj.AddComponent<RectTransform>();
            sdbRect.anchorMin = new Vector2(1f, 0.5f);
            sdbRect.anchorMax = new Vector2(1f, 0.5f);
            sdbRect.pivot = new Vector2(1f, 0.5f);
            sdbRect.sizeDelta = new Vector2(110, 28);
            sdbRect.anchoredPosition = new Vector2(-15, 0);

            Image sdbImg = shutdownBtnObj.AddComponent<Image>();
            sdbImg.color = new Color(0.9f, 0.2f, 0.2f, 0.2f);
            Outline sdbOutline = shutdownBtnObj.AddComponent<Outline>();
            sdbOutline.effectColor = new Color(0.9f, 0.2f, 0.2f, 0.5f);

            Button sdbBtn = shutdownBtnObj.AddComponent<Button>();
            sdbBtn.targetGraphic = sdbImg;
            ColorBlock sdbCb = sdbBtn.colors;
            sdbCb.normalColor = new Color(0.9f, 0.2f, 0.2f, 0.2f);
            sdbCb.highlightedColor = new Color(0.9f, 0.2f, 0.2f, 0.4f);
            sdbCb.pressedColor = new Color(0.9f, 0.2f, 0.2f, 0.7f);
            sdbCb.selectedColor = new Color(0.9f, 0.2f, 0.2f, 0.2f);
            sdbBtn.colors = sdbCb;

            GameObject sdbTextObj = new GameObject("Text");
            sdbTextObj.transform.SetParent(shutdownBtnObj.transform, false);
            RectTransform sdbtRect = sdbTextObj.AddComponent<RectTransform>();
            sdbtRect.anchorMin = Vector2.zero;
            sdbtRect.anchorMax = Vector2.one;
            sdbtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI sdbText = sdbTextObj.AddComponent<TextMeshProUGUI>();
            sdbText.text = "Shut Down ⏻";
            sdbText.fontSize = 11;
            sdbText.fontStyle = FontStyles.Bold;
            sdbText.color = Color.white;
            sdbText.alignment = TextAlignmentOptions.Center;
            sdbText.verticalAlignment = VerticalAlignmentOptions.Middle;

            sdbBtn.onClick.AddListener(() => {
                ToggleStartMenu();
                CloseComputer();
            });

            startMenuPanel.SetActive(false);
        }

        public void ToggleStartMenu()
        {
            if (startMenuPanel != null)
            {
                bool nextState = !startMenuPanel.activeSelf;
                startMenuPanel.SetActive(nextState);
                if (nextState)
                {
                    startMenuPanel.transform.SetAsLastSibling();
                }
            }
        }

        private void OnAddressBarSubmit(string text)
        {
            string cleanText = text.ToLower().Trim();
            if (cleanText.Contains("hoanggiacacino") || 
                cleanText.Contains("hoanggiacasino") || 
                cleanText.Contains("hoanggia") || 
                cleanText.Contains("hgcasino") || 
                cleanText.Contains("sòng bạc hoàng gia") || 
                cleanText.Contains("song bac hoang gia"))
            {
                ShowCasinoSearchResultPage();
            }
            else
            {
                ShowGenericSearchError(text);
            }
        }

        public void ShowCasinoSearchResultPage()
        {
            if (chromeWindow == null) return;

            SetAddressText("hoanggia.com");

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=11><color=#9aa0a6>Khoảng 1 kết quả (0,15 giây)</color></size><br><br>" +
                                 "<color=#8ab4f8><size=13><b>[ĐIỀU TRA ĐỘC QUYỀN] VẠCH TRẦN SỰ THẬT VỀ SÒNG BẠC HOÀNG GIA</b></size></color><br>" +
                                 "<color=#9aa0a6><size=10>https://hoanggia.com/dieu-tra-doc-quyen</size></color><br>" +
                                 "• Hoạt động dưới danh nghĩa giải trí hợp pháp, nhưng thực chất là một bẫy nợ khổng lồ được tổ chức tinh vi.<br>" +
                                 "• Cảnh báo người chơi về các trò gian lận tinh vi và hình thức cho vay nóng lãi suất cắt cổ ngay tại sòng bạc.";
                }
            }

            // Hide all other job buttons first
            HideAllChromeJobButtons();

            // Set Casino Search Button active
            Transform casinoSearchBtn = chromeWindow.transform.Find("Content/CasinoSearchButton");
            if (casinoSearchBtn != null) casinoSearchBtn.gameObject.SetActive(true);

            // Hide save contact button
            Transform saveContactBtn = chromeWindow.transform.Find("Content/SaveContactButton");
            if (saveContactBtn != null) saveContactBtn.gameObject.SetActive(false);

            // Show Back button
            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);
        }

        public void OnChromeBackButtonClicked()
        {
            if (chromeWindow == null) return;

            // Get current address text
            Transform addrBoxTrans = chromeWindow.transform.Find("UrlBar/AddressBox");
            string currentUrl = "";
            if (addrBoxTrans != null)
            {
                TMP_InputField inputField = addrBoxTrans.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    currentUrl = inputField.text;
                }
            }

            if (currentUrl.Contains("hoanggia.com/dieu-tra-doc-quyen"))
            {
                ShowCasinoSearchResultPage();
            }
            else
            {
                ShowChromeSearchResults();
            }
        }

        public void OpenCasinoDetailsPage()
        {
            if (chromeWindow == null) return;
            HideAllChromeJobButtons();
            
            // Set Address Bar Text
            SetAddressText("https://hoanggia.com/dieu-tra-doc-quyen");
            
            // Set content text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<color=#ff3333><size=14><b>[ĐIỀU TRA ĐỘC QUYỀN] VẠCH TRẦN SỰ THẬT VỀ SÒNG BẠC HOÀNG GIA</b></size></color><br>" +
                                 "<color=#9aa0a6><size=10>Thứ Năm, 18/06/2026 - Nhóm phóng viên điều tra</size></color><br><br>" +
                                 "Sòng bạc Hoàng Gia hoạt động dưới danh nghĩa giải trí hợp pháp, nhưng thực chất là một <b>bẫy nợ khổng lồ</b> được tổ chức tinh vi. " +
                                 "Theo tài liệu và lời kể của hàng chục nạn nhân, sòng bạc này sử dụng các thiết bị gian lận điện tử và thủ thuật của dealer trong mọi trò chơi (Tài Xỉu, Blackjack, Roulette) để kiểm soát kết quả, khiến người chơi chắc chắn thua sạch tiền.<br><br>" +
                                 "Nguy hiểm hơn, tại sòng bạc luôn túc trực các đối tượng <b>'tay trong' chuyên cho vay nóng</b> với lãi suất cắt cổ. " +
                                 "Khi người chơi thua hết tiền và hoảng luận, những kẻ này sẽ dụ dỗ ký giấy nợ để lấy tiền chơi tiếp. Kết quả là người chơi càng lún sâu, nợ chồng nợ dẫn đến khánh kiệt, mất nhà cửa và bị ép buộc làm việc phi pháp để trả nợ.<br><br>" +
                                 "<color=#ffcc00><b>Khuyến cáo:</b> Tuyệt đối không tham gia, không tin lời các đối tượng dụ dỗ nạp tiền chơi thử.</color>";
                }
            }
            
            // Show Back button
            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);
        }

        private void ShowGenericSearchError(string query)
        {
            if (chromeWindow == null) return;
            HideAllChromeJobButtons();
            
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = $"<color=#ff3333>Không tìm thấy trang web hoặc kết quả tìm kiếm cho: <b>{query}</b></color><br><br>" +
                                 "Gợi ý: Nhập chính xác địa chỉ website hoặc từ khóa từ tờ báo giấy để xem chi tiết điều tra sòng bạc:<br>" +
                                 "• <b>hoanggia.com</b><br>" +
                                 "• <b>sòng bạc hoàng gia</b>";
                }
            }
            
            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);
        }

        private void SetAddressText(string rawText)
        {
            if (chromeWindow == null) return;
            Transform addrBoxTrans = chromeWindow.transform.Find("UrlBar/AddressBox");
            if (addrBoxTrans != null)
            {
                TMP_InputField inputField = addrBoxTrans.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    inputField.text = rawText;
                    return;
                }
            }

            Transform addrTextTransform = chromeWindow.transform.Find("UrlBar/AddressBox/Text");
            if (addrTextTransform != null)
            {
                TextMeshProUGUI addrt = addrTextTransform.GetComponent<TextMeshProUGUI>();
                if (addrt != null)
                {
                    addrt.text = rawText;
                }
            }
        }
    }
}
