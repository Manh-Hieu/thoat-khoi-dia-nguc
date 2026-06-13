using UnityEngine;
using EscapeFromHell.Core;
using EscapeFromHell.Systems;
using System.Collections;

namespace EscapeFromHell.Chapter
{
    public class Chapter1Controller : MonoBehaviour
    {
        public static Chapter1Controller Instance { get; private set; }

        [Header("Dialogue Data")]
        [SerializeField] private DialogueData introDialogue;
        [SerializeField] private DialogueData phoneDialogue;
        [SerializeField] private DialogueData billsDialogue;
        [SerializeField] private DialogueData laptopDialogue;
        [SerializeField] private DialogueData jobOfferDialogue;
        [SerializeField] private DialogueData thanhDialogue;
        [SerializeField] private DialogueData hungDialogue;
        [SerializeField] private DialogueData finCreditDialogue;

        public DialogueData PhoneDialogue => phoneDialogue;
        public DialogueData BillsDialogue => billsDialogue;
        public DialogueData JobOfferDialogue => jobOfferDialogue;
        public DialogueData ThanhDialogue => thanhDialogue;
        public DialogueData HungDialogue => hungDialogue;
        public DialogueData FinCreditDialogue => finCreditDialogue;

        [Header("Progress Tracking")]
        public bool hasReadPhone = false;
        public bool hasReadBills = false;
        public bool hasReadLaptop = false;
        public bool hasReadFriend = false;
        public bool hasReadHung = false;
        public bool hasReadFinCredit = false;
        private bool climaxTriggered = false;

        [Header("New Call Companies State")]
        public bool canCallJobCompanies = false;
        public bool hasCalledCompany1 = false;
        public bool hasCalledCompany2 = false;
        private bool suggestionTriggered = false;

        [Header("Audio")]
        [SerializeField] private AudioClip notificationSFX;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private bool readyToLeave = false;
        private bool showDebugOverlay = true;
        private bool happyEndingTriggered = false;
        private bool readyToGoToCasino = false;

        private void Start()
        {
            // Start the game by playing intro dialogue after a brief delay
            StartCoroutine(StartIntroRoutine());
            SpawnDoorRuntime();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                showDebugOverlay = !showDebugOverlay;
            }

            // Cheat key to complete all reading requirements for testing/debugging
            if (Input.GetKeyDown(KeyCode.F10))
            {
                hasReadPhone = true;
                hasReadBills = true;
                hasReadLaptop = true;
                hasReadFriend = true;
                hasReadHung = true;
                hasReadFinCredit = true;
                if (PhoneUI.Instance != null)
                {
                    PhoneUI.Instance.hasSeenJob1 = true;
                    PhoneUI.Instance.hasSeenJob2 = true;
                }
                Debug.Log("[Chapter1Controller] Cheat activated: All conditions set to true!");
                OnUIClosed();
            }
        }

        private void OnGUI()
        {
            if (!showDebugOverlay) return;

            // Draw a nice semi-transparent background box
            GUI.Box(new Rect(10, 10, 320, 250), "TIẾN TRÌNH NHIỆM VỤ (F12 để ẩn/hiện, F10 để tự động hoàn thành)");

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = Color.white;

            System.Action<string, bool, int> drawRow = (labelText, val, yPos) =>
            {
                GUI.Label(new Rect(20, yPos, 220, 20), labelText, labelStyle);
                GUIStyle valStyle = new GUIStyle(GUI.skin.label);
                valStyle.fontSize = 12;
                valStyle.fontStyle = FontStyle.Bold;
                valStyle.normal.textColor = val ? Color.green : Color.red;
                GUI.Label(new Rect(240, yPos, 80, 20), val ? "ĐÃ XONG" : "CHƯA XONG", valStyle);
            };

            drawRow("1. Đọc tin nhắn của Mẹ:", hasReadPhone, 35);
            drawRow("2. Xem hoá đơn tiền điện:", hasReadBills, 55);
            drawRow("3. Mở Mail trên máy tính:", hasReadLaptop, 75);
            drawRow("4. Đọc tin nhắn của Thành:", hasReadFriend, 95);
            drawRow("5. Đọc tin nhắn của Hùng:", hasReadHung, 115);
            drawRow("6. Đọc tin nhắn FinCredit:", hasReadFinCredit, 135);

            bool hasJob1 = PhoneUI.Instance != null && PhoneUI.Instance.hasSeenJob1;
            bool hasJob2 = PhoneUI.Instance != null && PhoneUI.Instance.hasSeenJob2;

            drawRow("7. Lưu số JoyGame (Chrome):", hasJob1, 155);
            drawRow("8. Lưu số NovaTech (Chrome):", hasJob2, 175);

            GUI.Label(new Rect(20, 200, 280, 20), $"Trạng thái gợi ý: {(suggestionTriggered ? "Đã kích hoạt" : "Chưa kích hoạt")}", labelStyle);
            GUI.Label(new Rect(20, 220, 280, 20), $"Gọi xin việc 1: {(hasCalledCompany1 ? "Đã gọi" : "Chưa gọi")}, 2: {(hasCalledCompany2 ? "Đã gọi" : "Chưa gọi")}", labelStyle);
        }

        private IEnumerator StartIntroRoutine()
        {
            yield return new WaitForSeconds(1.0f);
            if (DialogueSystem.Instance != null && introDialogue != null)
            {
                DialogueSystem.Instance.StartDialogue(introDialogue);
            }
        }

        public void ReadPhone()
        {
            if (PhoneUI.Instance != null)
            {
                PhoneUI.Instance.OpenPhone();
            }
            else
            {
                // Fallback to dialogue if PhoneUI is not found
                if (hasReadPhone)
                {
                    DialogueSystem.Instance.StartDialogue(phoneDialogue);
                    return;
                }

                hasReadPhone = true;
                DialogueSystem.Instance.StartDialogue(phoneDialogue);
                CheckProgress();
            }
        }

        public void ReadBills()
        {
            // Instead of popping up the bills data directly, show a short monologue guiding the player to the phone!
            if (DialogueSystem.Instance != null)
            {
                DialogueData billsGuide = ScriptableObject.CreateInstance<DialogueData>();
                billsGuide.lines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine { speakerName = "Minh", text = "Hóa đơn dịch vụ tháng này... Mình có thể xem chi tiết tin nhắn thông báo trên điện thoại." }
                };
                DialogueSystem.Instance.StartDialogue(billsGuide);
            }
        }

        public void ReadPhoneFromPhoneUI()
        {
            if (!hasReadPhone)
            {
                hasReadPhone = true;
                CheckProgress();
            }
        }

        public void ReadBillsFromPhoneUI()
        {
            if (!hasReadBills)
            {
                hasReadBills = true;
                CheckProgress();
            }
        }

        public void ReadLaptop()
        {
            if (ComputerUI.Instance != null)
            {
                ComputerUI.Instance.OpenComputer();
            }
            else
            {
                // Fallback to dialogue if ComputerUI is not found
                if (hasReadLaptop)
                {
                    DialogueSystem.Instance.StartDialogue(laptopDialogue);
                    return;
                }

                hasReadLaptop = true;
                DialogueSystem.Instance.StartDialogue(laptopDialogue);
                CheckProgress();
            }
        }

        public void ReadLaptopFromComputer()
        {
            if (!hasReadLaptop)
            {
                hasReadLaptop = true;
                CheckProgress();
            }
        }

        public void ReadBillsFromComputer()
        {
            if (!hasReadBills)
            {
                hasReadBills = true;
                CheckProgress();
            }
        }

        public void ReadFriendFromPhoneUI()
        {
            if (!hasReadFriend)
            {
                hasReadFriend = true;
                CheckProgress();
            }
        }

        public void ReadHungFromPhoneUI()
        {
            if (!hasReadHung)
            {
                hasReadHung = true;
                CheckProgress();
            }
        }

        public void ReadFinCreditFromPhoneUI()
        {
            if (!hasReadFinCredit)
            {
                hasReadFinCredit = true;
                CheckProgress();
            }
        }

        public void CheckProgress()
        {
            if (hasReadPhone && hasReadBills && hasReadLaptop && hasReadFriend && hasReadHung && hasReadFinCredit)
            {
                if (hasCalledCompany1 && hasCalledCompany2 && !climaxTriggered)
                {
                    climaxTriggered = true;
                    StartCoroutine(TriggerClimaxRoutine());
                }
            }
        }

        public void OnUIClosed()
        {
            Debug.Log($"[Chapter1Controller] OnUIClosed: hasReadPhone={hasReadPhone}, hasReadBills={hasReadBills}, hasReadLaptop={hasReadLaptop}, hasReadFriend={hasReadFriend}, hasReadHung={hasReadHung}, hasReadFinCredit={hasReadFinCredit}");
            if (PhoneUI.Instance != null)
            {
                Debug.Log($"[Chapter1Controller] OnUIClosed: hasSeenJob1={PhoneUI.Instance.hasSeenJob1}, hasSeenJob2={PhoneUI.Instance.hasSeenJob2}");
            }
            else
            {
                Debug.Log($"[Chapter1Controller] OnUIClosed: PhoneUI.Instance is null");
            }

            if (hasReadPhone && hasReadBills && hasReadLaptop && hasReadFriend && hasReadHung && hasReadFinCredit &&
                PhoneUI.Instance != null && PhoneUI.Instance.hasSeenJob1 && PhoneUI.Instance.hasSeenJob2 &&
                !suggestionTriggered)
            {
                suggestionTriggered = true;
                StartCoroutine(TriggerSuggestionRoutine());
            }

            // Climax check when Phone UI is closed
            if (climaxTriggered && PhoneUI.Instance != null && PhoneUI.Instance.HasOpenedScamChat)
            {
                if (PhoneUI.Instance.IsScamChatDeleted)
                {
                    if (!happyEndingTriggered)
                    {
                        happyEndingTriggered = true;
                        StartCoroutine(TriggerPostSMSDeleteRoutine());
                    }
                }
            }
        }

        private IEnumerator TriggerSuggestionRoutine()
        {
            // Wait 5 seconds
            yield return new WaitForSeconds(5.0f);

            // Wait until dialogue system is not active and game state is playing
            yield return new WaitUntil(() => GameManager.Instance.CurrentState == GameState.Playing && (DialogueSystem.Instance == null || !DialogueSystem.Instance.IsDialogueActive));

            if (DialogueSystem.Instance != null)
            {
                DialogueData suggestionDial = ScriptableObject.CreateInstance<DialogueData>();
                suggestionDial.lines = new System.Collections.Generic.List<DialogueLine>
                {
                    new DialogueLine { speakerName = "Minh", text = "Nhiều nợ nần chồng chất thế này... Mình nên thử gọi điện xin việc của 2 công ty (JoyGame Studio và NovaTech Group) vừa tìm thấy trên mạng xem sao. Biết đâu họ châm chước..." }
                };
                DialogueSystem.Instance.StartDialogue(suggestionDial);
            }

            canCallJobCompanies = true;
        }

        private IEnumerator TriggerClimaxRoutine()
        {
            // Wait for active dialogue to end
            yield return new WaitUntil(() => GameManager.Instance.CurrentState == GameState.Playing);
            
            // Brief pause
            yield return new WaitForSeconds(1.5f);

            // Play notification sound
            if (AudioManager.Instance != null && notificationSFX != null)
            {
                AudioManager.Instance.PlaySFX(notificationSFX);
            }
            
            yield return new WaitForSeconds(1.0f);

            // Reveal the scam job SMS in the Messages app (simulates incoming message)
            if (PhoneUI.Instance != null)
            {
                PhoneUI.Instance.ShowScamChat();
            }

            // Trigger job notification dialogue outside
            if (DialogueSystem.Instance != null)
            {
                DialogueData outsideDial = ScriptableObject.CreateInstance<DialogueData>();
                outsideDial.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "*Ting ting*... Điện thoại có tin nhắn mới." },
                    new DialogueLine { speakerName = "Minh", text = "Hy vọng là tin báo đỗ từ một trong hai công ty mình vừa gọi, để mình còn có việc đi làm..." }
                };
                DialogueSystem.Instance.StartDialogue(outsideDial);
            }
        }

        private IEnumerator TriggerPostSMSDeleteRoutine()
        {
            readyToLeave = false;
            yield return new WaitForSeconds(0.5f);

            if (DialogueSystem.Instance != null)
            {
                DialogueData happyDial = ScriptableObject.CreateInstance<DialogueData>();
                happyDial.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Mình đã xóa tin nhắn lừa đảo kia đi. Dù cuộc sống ở đây có khó khăn, nợ nần, mình vẫn sẽ ở lại Việt Nam." },
                    new DialogueLine { speakerName = "Minh", text = "Ngày mai mình sẽ đăng ký chạy xe ôm công nghệ để kiếm sống và trả nợ dần. Chỉ cần chăm chỉ, nhất định mình sẽ vượt qua." }
                };
                DialogueSystem.Instance.OnDialogueEnd += OnPostSMSDeleteDialogueEnd;
                DialogueSystem.Instance.StartDialogue(happyDial);
            }
        }

        private void OnPostSMSDeleteDialogueEnd()
        {
            DialogueSystem.Instance.OnDialogueEnd -= OnPostSMSDeleteDialogueEnd;
            StartCoroutine(PhoneRingingRoutine());
        }

        private IEnumerator PhoneRingingRoutine()
        {
            if (PhoneUI.Instance != null)
            {
                PhoneUI.Instance.TriggerRingingCall();
            }

            GameObject phonePropObj = GameObject.Find("Props/Phone");
            if (phonePropObj != null)
            {
                Chapter1Prop prop = phonePropObj.GetComponent<Chapter1Prop>();
                if (prop != null)
                {
                    prop.PromptMessage = "Điện thoại đang reo!";
                }
            }

            while (PhoneUI.Instance != null && PhoneUI.Instance.IsRinging)
            {
                if (AudioManager.Instance != null && notificationSFX != null)
                {
                    AudioManager.Instance.PlaySFX(notificationSFX);
                }
                yield return new WaitForSeconds(2.0f);
            }
        }

        public void StartMomEmergencyCall()
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueData momCall = ScriptableObject.CreateInstance<DialogueData>();
                momCall.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Mẹ", text = "Minh ơi... (khóc nấc) Bố con... bố con bị ngã rồi con ơi..." },
                    new DialogueLine { speakerName = "Minh", text = "Mẹ ơi bình tĩnh lại, có chuyện gì xảy ra với bố thế ạ?" },
                    new DialogueLine { speakerName = "Mẹ", text = "Hôm nay bố cố leo lên mái nhà sửa lại mái dột... tự dưng đau chân tái phát nên trượt ngã từ trên cao xuống..." },
                    new DialogueLine { speakerName = "Mẹ", text = "Bác sĩ nói bố bị gãy xương đùi rất nặng, nếu không phẫu thuật gấp thì sẽ bị liệt, què cả đời con ơi..." },
                    new DialogueLine { speakerName = "Mẹ", text = "Mẹ lo quá, tiền viện phí và phẫu thuật lớn quá... Con xem ở trên thành phố có tiền gửi về cho bố phẫu thuật không con? Mẹ hết cách rồi..." },
                    new DialogueLine { speakerName = "Minh", text = "Mẹ đừng lo, sức khỏe của bố là quan trọng nhất. Con đang có tiền đây, mẹ lo cho bố nhập viện trước đi. Con chuyển về ngay." },
                    new DialogueLine { speakerName = "Mẹ", text = "Ôi cảm ơn con... Mẹ trông cậy cả vào con..." }
                };
                DialogueSystem.Instance.OnDialogueEnd += OnMomCallDialogueEnd;
                DialogueSystem.Instance.StartDialogue(momCall);
            }
        }

        private void OnMomCallDialogueEnd()
        {
            DialogueSystem.Instance.OnDialogueEnd -= OnMomCallDialogueEnd;
            StartCoroutine(TriggerPostCallMonologueRoutine());
        }

        private IEnumerator TriggerPostCallMonologueRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            if (DialogueSystem.Instance != null)
            {
                DialogueData postCall = ScriptableObject.CreateInstance<DialogueData>();
                postCall.lines = new System.Collections.Generic.List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Giờ mình lấy đâu ra tiền bây giờ? Trong tài khoản chỉ còn đúng 500k..." },
                    new DialogueLine { speakerName = "Minh", text = "Bạn bè, đồng nghiệp cũ đều nợ nần, từ chối hết rồi. Không còn ai để vay mượn nữa..." },
                    new DialogueLine { speakerName = "Minh", text = "Trong cơn hoảng loạn, mình quyết định... Đành phải liều mạng thôi! Cầm 500k này ra sòng bạc (casino) thử vận may xem sao!" }
                };
                DialogueSystem.Instance.OnDialogueEnd += OnPostCallMonologueEnd;
                DialogueSystem.Instance.StartDialogue(postCall);
            }
        }

        private void OnPostCallMonologueEnd()
        {
            DialogueSystem.Instance.OnDialogueEnd -= OnPostCallMonologueEnd;
            readyToGoToCasino = true;

            // Update door prompt
            GameObject doorObj = GameObject.Find("Props/Door");
            if (doorObj != null)
            {
                Chapter1Prop prop = doorObj.GetComponent<Chapter1Prop>();
                if (prop != null)
                {
                    prop.PromptMessage = "Đi ra casino";
                }
            }
        }

        private void SpawnDoorRuntime()
        {
            GameObject doorObj = new GameObject("Door");
            GameObject propsParent = GameObject.Find("Props");
            if (propsParent != null)
            {
                doorObj.transform.parent = propsParent.transform;
            }

            // Sát mép tường dưới, to hơn
            doorObj.transform.position = new Vector3(0.3f, -3.7f, 0);
            doorObj.transform.localScale = new Vector3(2.8f, 2.8f, 1f);

            SpriteRenderer sr = doorObj.AddComponent<SpriteRenderer>();
#if UNITY_EDITOR
            sr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/Door.png");
#endif
            sr.sortingOrder = 5;
            sr.color = Color.white;

            // Fade: mờ khi nhân vật đứng gần cửa (trong bán kính 1.8 đơn vị)
            DoorOcclusionFade fade = doorObj.AddComponent<DoorOcclusionFade>();
            fade.normalAlpha   = 1.0f;
            fade.fadedAlpha    = 0.55f;
            fade.fadeSpeed     = 8f;
            fade.triggerRadius = 1.8f;

            CircleCollider2D cc = doorObj.AddComponent<CircleCollider2D>();
            cc.radius = 0.6f;
            cc.isTrigger = true;

            Chapter1Prop prop = doorObj.AddComponent<Chapter1Prop>();
            prop.Initialize(Chapter1PropType.Door, "Ra ngoài");
        }

        public void InteractWithDoor()
        {
            if (readyToGoToCasino)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetChapter(2);
                    GameManager.Instance.LoadScene(GameManager.Instance.chapter2Scene);
                }
            }
            else if (climaxTriggered && PhoneUI.Instance != null && PhoneUI.Instance.HasOpenedScamChat && !PhoneUI.Instance.IsScamChatDeleted)
            {
                if (DialogueSystem.Instance != null)
                {
                    DialogueData fallback = ScriptableObject.CreateInstance<DialogueData>();
                    fallback.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = "Mình nên xóa tin nhắn tuyển dụng lừa đảo kia đi trước đã..." }
                    };
                    DialogueSystem.Instance.StartDialogue(fallback);
                }
            }
            else if (happyEndingTriggered && PhoneUI.Instance != null && PhoneUI.Instance.IsRinging)
            {
                if (DialogueSystem.Instance != null)
                {
                    DialogueData fallback = ScriptableObject.CreateInstance<DialogueData>();
                    fallback.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = "Điện thoại đang reo, mình phải nghe máy đã!" }
                    };
                    DialogueSystem.Instance.StartDialogue(fallback);
                }
            }
            else
            {
                if (DialogueSystem.Instance != null)
                {
                    DialogueData fallback = ScriptableObject.CreateInstance<DialogueData>();
                    fallback.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine { speakerName = "Minh", text = "Bây giờ mình không có tâm trạng để ra ngoài..." }
                    };
                    DialogueSystem.Instance.StartDialogue(fallback);
                }
            }
        }
    }
}
