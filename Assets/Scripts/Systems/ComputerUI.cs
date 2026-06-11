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
        private int currentViewingJobIndex = 0; // 0: none, 1: Job 1, 2: Job 2, 3: Scam Job

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

        public void CloseComputer()
        {
            if (computerPanel != null)
            {
                computerPanel.SetActive(false);

                // Re-hide cursor if needed (depending on game state, standard top-down top view might keep cursor visible but here we return control)
                if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
                {
                    GameManager.Instance.ChangeState(GameState.Playing);
                }
            }
        }

        private void CloseAllSubWindows()
        {
            if (mailWindow != null) mailWindow.SetActive(false);
            if (billsWindow != null) billsWindow.SetActive(false);
            if (thisPCWindow != null) thisPCWindow.SetActive(false);
            if (recycleBinWindow != null) recycleBinWindow.SetActive(false);
            if (chromeWindow != null) chromeWindow.SetActive(false);
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

            // Find address text
            Transform addrTextTransform = chromeWindow.transform.Find("HeaderBar/AddressBox/Text");
            if (addrTextTransform != null)
            {
                TextMeshProUGUI addrt = addrTextTransform.GetComponent<TextMeshProUGUI>();
                if (addrt != null)
                {
                    addrt.text = "<color=#5cb85c>• Secure</color> | https://topcv.vn/tim-kiem?q=unity+fresher";
                }
            }

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=11><color=#9aa0a6>Khoảng 124.000 kết quả (0,32 giây)</color></size><br><br>" +
                                 "<color=#8ab4f8><size=13><b>1. Thực Tập Sinh Lập Trình Unity - G-Star Studio</b></size></color><br>" +
                                 "<color=#9aa0a6><size=10>https://gstarstudio.vn/tuyen-dung/intern-unity</size></color><br>" +
                                 "• Yêu cầu: Có sản phẩm game tự làm, hiểu rõ C# và cấu trúc dữ liệu.<br>" +
                                 "• Lương hỗ trợ: 1.000.000đ/tháng. Thử việc không lương 2 tháng.<br><br>" +
                                 "<color=#8ab4f8><size=13><b>2. Lập Trình Viên Web (Fresher) - VinaTech Group</b></size></color><br>" +
                                 "<color=#9aa0a6><size=10>https://vinatech.com/careers/web-fresher</size></color><br>" +
                                 "• Yêu cầu: Có tối thiểu 1 năm kinh nghiệm làm việc thực tế tại doanh nghiệp.<br>" +
                                 "• Mức lương: 6.000.000đ - 8.000.000đ. Phỏng vấn kỹ thuật 3 vòng.<br><br>" +
                                 "<color=#33cc66><size=13><b>3. [HOT] Việc Làm Nước Ngoài Lương Cao - Tuyển Kỹ Thuật Viên Máy Tính (Campuchia)</b></size></color><br>" +
                                 "<color=#9aa0a6><size=10>https://vieclamnuocngoai.com/tuyen-dung/cambodia-tech</size></color><br>" +
                                 "• Mức lương: 1.000.000đ - 1.500.000đ/ngày (Bao ăn ở toàn bộ, hỗ trợ xe đưa đón, không phí trung gian).<br>" +
                                 "• Yêu cầu: Biết sử dụng máy tính cơ bản, gõ phím nhanh. Không yêu cầu bằng cấp hay kinh nghiệm.<br><br>" +
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
        }

        public void ClickJob1()
        {
            currentViewingJobIndex = 1;
            if (chromeWindow == null) return;

            // Find address text
            Transform addrTextTransform = chromeWindow.transform.Find("HeaderBar/AddressBox/Text");
            if (addrTextTransform != null)
            {
                TextMeshProUGUI addrt = addrTextTransform.GetComponent<TextMeshProUGUI>();
                if (addrt != null)
                {
                    addrt.text = "<color=#5cb85c>• Secure</color> | https://gstarstudio.vn/tuyen-dung/intern-unity";
                }
            }

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=13><b>CHI TIẾT TUYỂN DỤNG: THỰC TẬP SINH UNITY DEVELOPER</b></size><br>" +
                                 "<b>Công ty:</b> G-Star Game Studio<br>" +
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
                                 "• Zalo/Hotline: <color=#ff9900><b>028.3333.5555</b></color> (Phòng Nhân Sự - G-Star Studio)<br>" +
                                 "• Email: hr@gstarstudio.vn";
                }
            }

            HideAllChromeJobButtons();

            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);

            UpdateSaveContactButtonState();

            if (DialogueSystem.Instance != null)
            {
                DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Thực tập sinh Unity ở G-Star Studio... Lương hỗ trợ có 1 triệu một tháng lại còn bắt thử việc không lương 2 tháng." },
                    new DialogueLine { speakerName = "Minh", text = "Đúng là bóc lột mà, làm sao mình đủ trả tiền phòng trọ đây... Có số điện thoại liên lạc ở đây: 028.3333.5555." }
                };
                DialogueSystem.Instance.StartDialogue(textMock);
            }
        }

        public void ClickJob2()
        {
            currentViewingJobIndex = 2;
            if (chromeWindow == null) return;

            // Find address text
            Transform addrTextTransform = chromeWindow.transform.Find("HeaderBar/AddressBox/Text");
            if (addrTextTransform != null)
            {
                TextMeshProUGUI addrt = addrTextTransform.GetComponent<TextMeshProUGUI>();
                if (addrt != null)
                {
                    addrt.text = "<color=#5cb85c>• Secure</color> | https://vinatech.com/careers/web-fresher";
                }
            }

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=13><b>CHI TIẾT TUYỂN DỤNG: LẬP TRÌNH VIÊN WEB (FRESHER)</b></size><br>" +
                                 "<b>Công ty:</b> VinaTech Group<br>" +
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
                                 "• Zalo/Hotline: <color=#ff9900><b>024.9999.8888</b></color> (Phòng Tuyển Dụng - VinaTech Group)<br>" +
                                 "• Email: careers@vinatech.com";
                }
            }

            HideAllChromeJobButtons();

            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);

            UpdateSaveContactButtonState();

            if (DialogueSystem.Instance != null)
            {
                DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Tuyển Fresher Web ở VinaTech lương 6 đến 8 triệu, nhưng lại đòi hỏi tận 1 năm kinh nghiệm làm việc thực tế tại doanh nghiệp..." },
                    new DialogueLine { speakerName = "Minh", text = "Đã ghi là Fresher mà lại đòi 1 năm kinh nghiệm thì sao mình ứng tuyển nổi đây... Số điện thoại phòng nhân sự của họ: 024.9999.8888." }
                };
                DialogueSystem.Instance.StartDialogue(textMock);
            }
        }

        public void ClickScamJob()
        {
            currentViewingJobIndex = 3;
            if (chromeWindow == null) return;

            // Find address text
            Transform addrTextTransform = chromeWindow.transform.Find("HeaderBar/AddressBox/Text");
            if (addrTextTransform != null)
            {
                TextMeshProUGUI addrt = addrTextTransform.GetComponent<TextMeshProUGUI>();
                if (addrt != null)
                {
                    addrt.text = "<color=#5cb85c>• Secure</color> | https://vieclamnuocngoai.com/tuyen-dung/cambodia-tech";
                }
            }

            // Find page text
            Transform pageTextTransform = chromeWindow.transform.Find("Content/PageText");
            if (pageTextTransform != null)
            {
                TextMeshProUGUI paget = pageTextTransform.GetComponent<TextMeshProUGUI>();
                if (paget != null)
                {
                    paget.text = "<size=13><b>CHI TIẾT TUYỂN DỤNG: KỸ THUẬT VIÊN MÁY TÍNH</b></size><br>" +
                                 "<b>Địa điểm làm việc:</b> Bavet, Campuchia (sát cửa khẩu Mộc Bài, Tây Ninh)<br>" +
                                 "<b>Mức lương:</b> <color=#33cc66>1.000.000đ - 1.500.000đ/ngày</color> (Nhận trực tiếp theo ngày)<br>" +
                                 "------------------------------------------------------------------<br>" +
                                 "<b>Mô tả công việc:</b><br>" +
                                 "• Sử dụng máy tính và ứng dụng chat để chăm sóc khách hàng trực tuyến.<br>" +
                                 "• Tư vấn và hướng dẫn người chơi tham gia trò chơi điện tử trực tuyến.<br>" +
                                 "• Thực hiện các công việc nhập liệu cơ bản theo hướng dẫn của quản lý.<br><br>" +
                                 "<b>Quyền lợi:</b><br>" +
                                 "• Bao ăn ở 100%, có ký túc xá máy lạnh đầy đủ tiện nghi, wifi 24/7.<br>" +
                                 "• Hỗ trợ toàn bộ chi phí làm hộ chiếu và xe đưa đón sang Campuchia.<br>" +
                                 "• Cam kết công việc văn phòng nhẹ nhàng, không áp doanh số.<br><br>" +
                                 "<b>Liên hệ nộp hồ sơ & phỏng vấn trực tiếp:</b><br>" +
                                 "• Zalo/Hotline: <color=#ff9900><b>098.765.4321</b></color> (Gặp Anh Hùng - Trưởng phòng nhân sự)<br>" +
                                 "• Email: tuyendung.bavetholding@gmail.com";
                }
            }

            HideAllChromeJobButtons();

            Transform backBtn = chromeWindow.transform.Find("Content/ChromeBackButton");
            if (backBtn != null) backBtn.gameObject.SetActive(true);

            UpdateSaveContactButtonState();

            if (DialogueSystem.Instance != null)
            {
                DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Chi tiết ghi rõ là lương 1 đến 1.5 triệu mỗi ngày, phát trực tiếp... Bao ăn ở từ A đến Z, lại còn hỗ trợ chi phí đi lại nữa." },
                    new DialogueLine { speakerName = "Minh", text = "Có cả số điện thoại Zalo của người tên Hùng để liên lạc: 098.765.4321. Mình nên lưu số này lại đề phòng cần đến..." }
                };
                DialogueSystem.Instance.StartDialogue(textMock);
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
            bind("Workspace/ChromeWindow/Content/ChromeBackButton", () => ShowChromeSearchResults());
            bind("Workspace/ChromeWindow/Content/SaveContactButton", () => SaveCurrentJobContact());

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
            }

            if (alreadySaved)
            {
                btn.interactable = false;
                if (txt != null) txt.text = "✓ Đã lưu số";
                if (img != null) img.color = new Color(0.25f, 0.25f, 0.28f, 1f); // Grey
            }
            else
            {
                btn.interactable = true;
                if (txt != null) txt.text = "Lưu số điện thoại";
                if (img != null) img.color = new Color(0.1f, 0.45f, 0.8f, 1f); // Blue
            }
        }

        public void SaveCurrentJobContact()
        {
            if (PhoneUI.Instance == null) return;

            string contactName = "";
            if (currentViewingJobIndex == 1)
            {
                PhoneUI.Instance.hasSeenJob1 = true;
                contactName = "G-Star Studio (Tuyển dụng)";
            }
            else if (currentViewingJobIndex == 2)
            {
                PhoneUI.Instance.hasSeenJob2 = true;
                contactName = "VinaTech Group (Tuyển dụng)";
            }
            else if (currentViewingJobIndex == 3)
            {
                PhoneUI.Instance.hasSeenScamJob = true;
                contactName = "Anh Hùng (Bavet Campuchia)";
            }

            if (!string.IsNullOrEmpty(contactName))
            {
                // Update button visual state immediately
                UpdateSaveContactButtonState();

                // Trigger confirmation monologue
                if (DialogueSystem.Instance != null)
                {
                    DialogueData textMock = ScriptableObject.CreateInstance<DialogueData>();
                    textMock.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = $"Đã lưu số điện thoại của {contactName} vào danh bạ trên di động." }
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
    }
}
