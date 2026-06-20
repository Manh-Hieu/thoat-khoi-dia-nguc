using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using EscapeFromHell.Core;
using EscapeFromHell.Systems;

namespace EscapeFromHell.Chapter
{
    public class Chapter2Controller : MonoBehaviour
    {
        public static Chapter2Controller Instance { get; private set; }

        [Header("Dialogue Data")]
        [SerializeField] private DialogueData recruiterDialogue;

        [Header("Shared Game State")]
        private int currentCash = 500000;
        public int CurrentCash
        {
            get => currentCash;
            set
            {
                currentCash = value;
                UpdateAllCashUI();
            }
        }
        private int betCount = 0;
        // Chỉ hiện intro lần đầu vào mỗi trò
        private bool hasSeenTaiXiuIntro = false;
        private bool hasSeenBlackjackIntro = false;
        private bool hasSeenRouletteIntro = false;

        // ── TÀI XỈU (DICE) UI & STATE ──────────────────────────────────────
        private GameObject taiXiuPanel;
        private TextMeshProUGUI cashText;
        private TextMeshProUGUI betTypeText;
        private TextMeshProUGUI betAmountText;
        private Image dice1Image;
        private Image dice2Image;
        private Image dice3Image;
        private TextMeshProUGUI resultText;

        private string selectedBetType = ""; // "Tai" or "Xiu"
        private int selectedBetAmount = 0;
        private bool isRolling = false;
        private TMP_InputField betInputField;
        private Image taiXiuBowlImage;
        private Button openBowlBtn;
        private Button rollDiceBtn;
        private int finalD1, finalD2, finalD3, finalTotal;
        private bool finalIsTaiResult;

        // ── BLACKJACK (XÌ DÁCH) UI & STATE ─────────────────────────────────
        private GameObject blackjackPanel;
        private TextMeshProUGUI bjCashText;
        private TextMeshProUGUI bjBetAmountText;
        private Transform bjPlayerCardsContainer;
        private Transform bjDealerCardsContainer;
        private TextMeshProUGUI bjResultText;
        private Button bjHitBtn;
        private Button bjStandBtn;
        private Button bjDealBtn;

        private int bjSelectedBet = 0;
        private List<string> bjPlayerHand = new List<string>();
        private List<string> bjDealerHand = new List<string>();
        private bool bjIsGameActive = false;
        private TMP_InputField bjBetInputField;

        // ── ROULETTE UI & STATE ────────────────────────────────────────────
        private GameObject roulettePanel;
        private TextMeshProUGUI rlCashText;
        private TextMeshProUGUI rlBetTypeText;
        private TextMeshProUGUI rlBetAmountText;
        private TextMeshProUGUI rlResultText;
        private RectTransform rlWheelRect;

        private string rlSelectedBetType = ""; // "Red", "Black", "Even", "Odd"
        private int rlSelectedBetAmount = 0;
        private bool rlIsSpinning = false;
        private TMP_InputField rlBetInputField;

        [Header("Procedural Assets Assigned by Builder")]
        public List<Sprite> diceSprites = new List<Sprite>();
        public Sprite cardBackgroundSprite;
        public Sprite suitHeartSprite;
        public Sprite suitDiamondSprite;
        public Sprite suitSpadeSprite;
        public Sprite suitClubSprite;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Dynamically bind all UI components from scene Canvas
            BindAllUIElements();

            // Set all panels inactive initially
            if (taiXiuPanel != null) taiXiuPanel.SetActive(false);
            if (blackjackPanel != null) blackjackPanel.SetActive(false);
            if (roulettePanel != null) roulettePanel.SetActive(false);

            if (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.hasTransferredScamLuck)
            {
                currentCash = 400000;
            }
            else
            {
                currentCash = 500000;
            }

            UpdateAllCashUI();

            // Introduce player to the Casino scene
            StartCoroutine(PlayIntroMonologue());
        }

        private IEnumerator PlayIntroMonologue()
        {
            yield return new WaitForSeconds(1.5f);
            if (DialogueSystem.Instance != null)
            {
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Nơi này... thật hào nhoáng và náo nhiệt. Đúng là sòng bạc thượng lưu." },
                    new DialogueLine { speakerName = "Minh", text = "Trong túi mình chỉ còn đúng 500.000đ... Đây là tia hy vọng cuối cùng để gửi tiền về phẫu thuật cho bố." },
                    new DialogueLine { speakerName = "Minh", text = "Có 3 bàn chơi ở đây: Poker/Blackjack, Tài Xỉu và Roulette. Mình nên thử vận may ở đâu đây?" }
                };
                DialogueSystem.Instance.StartDialogue(d);
            }
        }

        private void UpdateAllCashUI()
        {
            if (cashText != null) cashText.text = $"Tiền mặt: <color=#ffb732>{currentCash.ToString("N0")}đ</color>";
            if (bjCashText != null) bjCashText.text = $"Tiền mặt: <color=#ffb732>{currentCash.ToString("N0")}đ</color>";
            if (rlCashText != null) rlCashText.text = $"Tiền mặt: <color=#ffb732>{currentCash.ToString("N0")}đ</color>";
        }

        private void CheckBankruptcy(GameObject currentActivePanel)
        {
            if (currentCash <= 0)
            {
                currentCash = 0;
                UpdateAllCashUI();
                StartCoroutine(TriggerBankruptcySequence(currentActivePanel));
            }
        }

        private IEnumerator TriggerBankruptcySequence(GameObject panelToClose)
        {
            yield return new WaitForSeconds(1.5f);
            if (panelToClose != null) panelToClose.SetActive(false);

            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }

            if (DialogueSystem.Instance != null)
            {
                DialogueData bankruptDial = ScriptableObject.CreateInstance<DialogueData>();
                bankruptDial.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Không thể nào... Thua sạch rồi... 500.000đ cuối cùng của mình..." },
                    new DialogueLine { speakerName = "Minh", text = "Tiền phẫu thuật cho Bố... Giờ mình biết phải làm sao đây... Mình thật sự bất tài vô dụng mà..." }
                };
                DialogueSystem.Instance.StartDialogue(bankruptDial);

                yield return new WaitUntil(() => !DialogueSystem.Instance.IsDialogueActive);
            }

            InteractWithRecruiter();
        }

        // ── DYNAMIC UI BINDINGS ──────────────────────────────────────────
        private void BindAllUIElements()
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null) return;

            Transform canvasT = canvas.transform;

            // 1. TÀI XỈU PANEL BINDINGS
            Transform txT = FindTransformRecursive(canvasT, "TaiXiuPanel");
            if (txT != null)
            {
                taiXiuPanel = txT.gameObject;
                cashText = FindTextRecursive(txT, "CashText");
                betTypeText = FindTextRecursive(txT, "BetTypeText");
                betAmountText = FindTextRecursive(txT, "BetAmountText");
                
                Transform d1 = FindTransformRecursive(txT, "Dice1Image");
                if (d1 != null) dice1Image = d1.GetComponent<Image>();
                Transform d2 = FindTransformRecursive(txT, "Dice2Image");
                if (d2 != null) dice2Image = d2.GetComponent<Image>();
                Transform d3 = FindTransformRecursive(txT, "Dice3Image");
                if (d3 != null) dice3Image = d3.GetComponent<Image>();
                
                resultText = FindTextRecursive(txT, "ResultText");

                BindButton(txT, "TaiButton", () => SelectBetType("Tai"));
                BindButton(txT, "XiuButton", () => SelectBetType("Xiu"));
                BindButton(txT, "BetMinusButton", () => AdjustBetAmount(-100000));
                BindButton(txT, "BetPlusButton", () => AdjustBetAmount(100000));
                BindButton(txT, "BetAllInButton", () => SelectBetAmount(-1));
                rollDiceBtn = BindButton(txT, "RollButton", RollDice);
                BindButton(txT, "CloseButton", CloseTaiXiuUI);

                Transform bowlT = FindTransformRecursive(txT, "TaiXiuBowl");
                if (bowlT != null) taiXiuBowlImage = bowlT.GetComponent<Image>();

                openBowlBtn = BindButton(txT, "OpenBowlButton", OpenBowl);

                Transform inputTx = FindTransformRecursive(txT, "BetInputField");
                if (inputTx != null)
                {
                    betInputField = inputTx.GetComponent<TMP_InputField>();
                    if (betInputField != null)
                    {
                        betInputField.onEndEdit.RemoveAllListeners();
                        betInputField.onEndEdit.AddListener(OnBetInputEndEdit);
                    }
                }
            }

            // 2. BLACKJACK PANEL BINDINGS
            Transform bjT = FindTransformRecursive(canvasT, "BlackjackPanel");
            if (bjT != null)
            {
                blackjackPanel = bjT.gameObject;
                bjCashText = FindTextRecursive(bjT, "CashText");
                bjBetAmountText = FindTextRecursive(bjT, "BetAmountText");
                bjPlayerCardsContainer = FindTransformRecursive(bjT, "PlayerCardsContainer");
                bjDealerCardsContainer = FindTransformRecursive(bjT, "DealerCardsContainer");
                bjResultText = FindTextRecursive(bjT, "ResultText");

                bjHitBtn = BindButton(bjT, "HitButton", BJHit);
                bjStandBtn = BindButton(bjT, "StandButton", BJStand);
                bjDealBtn = BindButton(bjT, "DealButton", BJStartGame);

                BindButton(bjT, "BJBetMinusButton", () => BJAdjustBetAmount(-100000));
                BindButton(bjT, "BJBetPlusButton", () => BJAdjustBetAmount(100000));
                BindButton(bjT, "BetAllInButton", () => BJSelectBetAmount(-1));
                BindButton(bjT, "CloseButton", CloseBlackjackUI);

                Transform inputBj = FindTransformRecursive(bjT, "BJBetInputField");
                if (inputBj != null)
                {
                    bjBetInputField = inputBj.GetComponent<TMP_InputField>();
                    if (bjBetInputField != null)
                    {
                        bjBetInputField.onEndEdit.RemoveAllListeners();
                        bjBetInputField.onEndEdit.AddListener(BJOnBetInputEndEdit);
                    }
                }
            }

            // 3. ROULETTE PANEL BINDINGS
            Transform rlT = FindTransformRecursive(canvasT, "RoulettePanel");
            if (rlT != null)
            {
                roulettePanel = rlT.gameObject;
                rlCashText = FindTextRecursive(rlT, "CashText");
                rlBetTypeText = FindTextRecursive(rlT, "BetTypeText");
                rlBetAmountText = FindTextRecursive(rlT, "BetAmountText");
                rlResultText = FindTextRecursive(rlT, "ResultText");
                
                Transform wh = FindTransformRecursive(rlT, "RouletteWheel");
                if (wh != null) rlWheelRect = wh as RectTransform;

                BindButton(rlT, "RedButton", () => RLSelectBetType("Red"));
                BindButton(rlT, "BlackButton", () => RLSelectBetType("Black"));
                BindButton(rlT, "EvenButton", () => RLSelectBetType("Even"));
                BindButton(rlT, "OddButton", () => RLSelectBetType("Odd"));

                BindButton(rlT, "RLBetMinusButton", () => RLAdjustBetAmount(-100000));
                BindButton(rlT, "RLBetPlusButton", () => RLAdjustBetAmount(100000));
                BindButton(rlT, "BetAllInButton", () => RLSelectBetAmount(-1));
                BindButton(rlT, "SpinButton", RLSpin);
                BindButton(rlT, "CloseButton", CloseRouletteUI);

                Transform inputRl = FindTransformRecursive(rlT, "RLBetInputField");
                if (inputRl != null)
                {
                    rlBetInputField = inputRl.GetComponent<TMP_InputField>();
                    if (rlBetInputField != null)
                    {
                        rlBetInputField.onEndEdit.RemoveAllListeners();
                        rlBetInputField.onEndEdit.AddListener(RLOnBetInputEndEdit);
                    }
                }
            }
        }

        private Button BindButton(Transform parent, string buttonName, UnityEngine.Events.UnityAction action)
        {
            Transform t = FindTransformRecursive(parent, buttonName);
            if (t != null)
            {
                Button btn = t.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(action);
                    return btn;
                }
            }
            return null;
        }

        private TextMeshProUGUI FindTextRecursive(Transform parent, string name)
        {
            Transform t = FindTransformRecursive(parent, name);
            return t != null ? t.GetComponent<TextMeshProUGUI>() : null;
        }

        private Transform FindTransformRecursive(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            foreach (Transform child in parent)
            {
                Transform found = FindTransformRecursive(child, name);
                if (found != null) return found;
            }
            return null;
        }

        // ── 1. TÀI XỈU GAMEPLAY LOGIC ──────────────────────────────────────
        public void OpenTaiXiuUI()
        {
            if (currentCash <= 0)
            {
                InteractWithRecruiter();
                return;
            }

            // Lần đầu vào: hiện intro dialogue
            if (!hasSeenTaiXiuIntro && DialogueSystem.Instance != null)
            {
                hasSeenTaiXiuIntro = true;
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Bàn Tài Xỉu... Xốc xúc xắc đoán tổng Tài hay Xỉu." },
                    new DialogueLine { speakerName = "Minh", text = "Được rồi, thử vận may xem sao!" }
                };
                DialogueSystem.Instance.OnDialogueEnd += OpenTaiXiuGame;
                DialogueSystem.Instance.StartDialogue(d);
                return;
            }

            // Lần sau: vào thẳng
            OpenTaiXiuGame();
        }

        private void OpenTaiXiuGame()
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.OnDialogueEnd -= OpenTaiXiuGame;

            if (currentCash <= 0) return;

            if (taiXiuPanel != null)
            {
                selectedBetType = "";
                selectedBetAmount = ValidateBetAmount(100000);
                if (resultText != null) resultText.text = "Chọn Tài/Xỉu & mức cược rồi bấm Xốc!";
                if (dice1Image != null && diceSprites.Count > 0) dice1Image.sprite = diceSprites[0];
                if (dice2Image != null && diceSprites.Count > 0) dice2Image.sprite = diceSprites[0];
                if (dice3Image != null && diceSprites.Count > 0) dice3Image.sprite = diceSprites[0];
                
                if (taiXiuBowlImage != null)
                {
                    taiXiuBowlImage.gameObject.SetActive(false); // Initially hidden/lifted
                }
                if (openBowlBtn != null)
                {
                    openBowlBtn.gameObject.SetActive(false); // Hide Open Bowl button initially
                }
                if (rollDiceBtn != null)
                {
                    rollDiceBtn.gameObject.SetActive(true); // Show Xốc button initially
                }

                UpdateTaiXiuUI();
                UpdateAllCashUI();
                taiXiuPanel.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.Cutscene);
            }
        }

        public void CloseTaiXiuUI()
        {
            if (taiXiuPanel != null) taiXiuPanel.SetActive(false);
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }
        }

        public void SelectBetType(string type)
        {
            if (isRolling) return;
            selectedBetType = type;
            UpdateTaiXiuUI();
        }

        private int ValidateBetAmount(int amount)
        {
            if (currentCash <= 0) return 0;
            
            if (currentCash < 100000)
            {
                return currentCash;
            }

            if (amount < 100000)
            {
                return 100000;
            }

            int rounded = (amount / 100000) * 100000;
            
            if (rounded > currentCash)
            {
                rounded = (currentCash / 100000) * 100000;
                if (rounded < 100000) rounded = currentCash;
            }

            return rounded;
        }

        public void SelectBetAmount(int amount)
        {
            if (isRolling) return;

            if (amount == -1)
            {
                selectedBetAmount = currentCash;
            }
            else
            {
                selectedBetAmount = ValidateBetAmount(amount);
            }
            UpdateTaiXiuUI();
        }

        public void AdjustBetAmount(int delta)
        {
            if (isRolling) return;
            int target = selectedBetAmount + delta;
            selectedBetAmount = ValidateBetAmount(target);
            UpdateTaiXiuUI();
        }

        private void OnBetInputEndEdit(string text)
        {
            if (int.TryParse(text, out int amount))
            {
                selectedBetAmount = ValidateBetAmount(amount);
            }
            else
            {
                selectedBetAmount = ValidateBetAmount(0);
            }
            UpdateTaiXiuUI();
        }

        private void UpdateTaiXiuUI()
        {
            if (betTypeText != null) betTypeText.text = string.IsNullOrEmpty(selectedBetType) ? "Chưa chọn" : (selectedBetType == "Tai" ? "Tài" : "Xỉu");
            if (betAmountText != null) betAmountText.text = selectedBetAmount == 0 ? "Chưa đặt" : $"{selectedBetAmount.ToString("N0")}đ";
            if (betInputField != null && betInputField.text != selectedBetAmount.ToString())
            {
                betInputField.text = selectedBetAmount.ToString();
            }
        }

        public void RollDice()
        {
            if (isRolling) return;
            if (string.IsNullOrEmpty(selectedBetType))
            {
                if (resultText != null) resultText.text = "<color=#ff4d4d>Hãy chọn đặt Tài hoặc Xỉu trước!</color>";
                return;
            }
            if (selectedBetAmount <= 0 || selectedBetAmount > currentCash)
            {
                if (resultText != null) resultText.text = "<color=#ff4d4d>Chọn mức đặt cược hợp lệ!</color>";
                return;
            }

            StartCoroutine(RollDiceRoutine());
        }

        public void OpenBowl()
        {
            StartCoroutine(OpenBowlRoutine());
        }

        private IEnumerator OpenBowlRoutine()
        {
            if (openBowlBtn != null) openBowlBtn.gameObject.SetActive(false);

            float duration = 0.6f;
            float elapsed = 0f;
            Vector2 startPos = new Vector2(0, -275);
            Vector2 endPos = new Vector2(0, -215);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Lift-and-fade animation
                if (taiXiuBowlImage != null)
                {
                    taiXiuBowlImage.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                    Color col = taiXiuBowlImage.color;
                    col.a = Mathf.Lerp(1f, 0f, t);
                    taiXiuBowlImage.color = col;
                }
                yield return null;
            }

            if (taiXiuBowlImage != null) taiXiuBowlImage.gameObject.SetActive(false);
            if (rollDiceBtn != null) rollDiceBtn.gameObject.SetActive(true);

            ResolveRollResult();
        }

        private void ResolveRollResult()
        {
            string resultStr = finalIsTaiResult ? "Tài" : "Xỉu";
            bool playerWon = (selectedBetType == "Tai" && finalIsTaiResult) || (selectedBetType == "Xiu" && !finalIsTaiResult);

            if (playerWon)
            {
                currentCash += selectedBetAmount;
                if (resultText != null) resultText.text = $"<color=#5cb85c>Thắng! {finalD1}+{finalD2}+{finalD3}={finalTotal} ({resultStr})\nBạn nhận +{selectedBetAmount.ToString("N0")}đ</color>";
            }
            else
            {
                currentCash -= selectedBetAmount;
                if (resultText != null) resultText.text = $"<color=#ff4d4d>Thua! {finalD1}+{finalD2}+{finalD3}={finalTotal} ({resultStr})\nBạn mất -{selectedBetAmount.ToString("N0")}đ</color>";
            }

            if (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining > 0)
            {
                EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining--;
            }

            betCount++;
            isRolling = false;
            
            UpdateAllCashUI();
            selectedBetAmount = Mathf.Min(selectedBetAmount, currentCash);
            UpdateTaiXiuUI();

            CheckBankruptcy(taiXiuPanel);
        }

        private IEnumerator RollDiceRoutine()
        {
            isRolling = true;
            if (resultText != null) resultText.text = "Đang xốc xốc xốc...";

            if (taiXiuBowlImage != null)
            {
                taiXiuBowlImage.gameObject.SetActive(true);
                taiXiuBowlImage.rectTransform.anchoredPosition = new Vector2(0, -275);
                Color col = taiXiuBowlImage.color;
                col.a = 1f;
                taiXiuBowlImage.color = col;
            }

            if (rollDiceBtn != null) rollDiceBtn.gameObject.SetActive(false);

            // Shaking animation
            int shakeFrames = 15;
            for (int i = 0; i < shakeFrames; i++)
            {
                if (taiXiuBowlImage != null)
                {
                    // Vigorously shake bowl!
                    taiXiuBowlImage.rectTransform.anchoredPosition = new Vector2(Random.Range(-15f, 15f), -275f + Random.Range(-10f, 10f));
                }

                // Cycle random dice faces under the bowl (invisible, but good for timing/realism!)
                if (dice1Image != null && diceSprites.Count >= 6) dice1Image.sprite = diceSprites[Random.Range(0, 6)];
                if (dice2Image != null && diceSprites.Count >= 6) dice2Image.sprite = diceSprites[Random.Range(0, 6)];
                if (dice3Image != null && diceSprites.Count >= 6) dice3Image.sprite = diceSprites[Random.Range(0, 6)];

                yield return new WaitForSeconds(0.06f);
            }

            // Restore bowl to center position
            if (taiXiuBowlImage != null)
            {
                taiXiuBowlImage.rectTransform.anchoredPosition = new Vector2(0, -275);
            }

            // Game logic: 10 trận đầu xác suất 50/50, sau 10 trận thua liên tục
            bool isScamLuckActive = (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining > 0);
            bool shouldWin = isScamLuckActive ? true : ((betCount < 10) ? (Random.value < 0.5f) : false);

            int attempts = 0;
            do
            {
                finalD1 = Random.Range(1, 7);
                finalD2 = Random.Range(1, 7);
                finalD3 = Random.Range(1, 7);
                finalTotal = finalD1 + finalD2 + finalD3;
                finalIsTaiResult = (finalTotal >= 11);
                attempts++;

                bool matchesChoice = (selectedBetType == "Tai" && finalIsTaiResult) || (selectedBetType == "Xiu" && !finalIsTaiResult);
                if (shouldWin && matchesChoice) break;
                if (!shouldWin && !matchesChoice) break;

            } while (attempts < 100);

            // Apply final resolved values to dice faces under the cover
            if (dice1Image != null && diceSprites.Count >= finalD1) dice1Image.sprite = diceSprites[finalD1 - 1];
            if (dice2Image != null && diceSprites.Count >= finalD2) dice2Image.sprite = diceSprites[finalD2 - 1];
            if (dice3Image != null && diceSprites.Count >= finalD3) dice3Image.sprite = diceSprites[finalD3 - 1];

            if (resultText != null) resultText.text = "Đã xốc xong! Hãy mở bát.";

            // Show Mở Bát Button
            if (openBowlBtn != null) openBowlBtn.gameObject.SetActive(true);
        }


        // ── 2. BLACKJACK GAMEPLAY LOGIC ────────────────────────────────────
        public void OpenBlackjackUI()
        {
            if (currentCash <= 0)
            {
                InteractWithRecruiter();
                return;
            }

            if (blackjackPanel != null)
            {
                bjSelectedBet = ValidateBetAmount(100000);
                bjIsGameActive = false;
                bjPlayerHand.Clear();
                bjDealerHand.Clear();
                
                ClearContainer(bjPlayerCardsContainer);
                ClearContainer(bjDealerCardsContainer);
                if (bjResultText != null) bjResultText.text = "Đặt tiền cược rồi bấm Chia Bài!";

                UpdateBJUI();
                UpdateBlackjackButtons();
                UpdateAllCashUI();
                blackjackPanel.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.Cutscene);
            }
        }

        public void CloseBlackjackUI()
        {
            if (blackjackPanel != null) blackjackPanel.SetActive(false);
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }
        }

        public void BJSelectBetAmount(int amount)
        {
            if (bjIsGameActive) return;

            if (amount == -1)
            {
                bjSelectedBet = currentCash;
            }
            else
            {
                bjSelectedBet = ValidateBetAmount(amount);
            }
            UpdateBJUI();
        }

        public void BJAdjustBetAmount(int delta)
        {
            if (bjIsGameActive) return;
            int target = bjSelectedBet + delta;
            bjSelectedBet = ValidateBetAmount(target);
            UpdateBJUI();
        }

        private void BJOnBetInputEndEdit(string text)
        {
            if (int.TryParse(text, out int amount))
            {
                bjSelectedBet = ValidateBetAmount(amount);
            }
            else
            {
                bjSelectedBet = ValidateBetAmount(0);
            }
            UpdateBJUI();
        }

        private void UpdateBJUI()
        {
            if (bjBetAmountText != null) bjBetAmountText.text = bjSelectedBet == 0 ? "Chưa đặt" : $"{bjSelectedBet.ToString("N0")}đ";
            if (bjBetInputField != null && bjBetInputField.text != bjSelectedBet.ToString())
            {
                bjBetInputField.text = bjSelectedBet.ToString();
            }
        }

        public void BJStartGame()
        {
            if (bjIsGameActive) return;
            if (bjSelectedBet <= 0 || bjSelectedBet > currentCash)
            {
                if (bjResultText != null) bjResultText.text = "<color=#ff4d4d>Chọn mức cược hợp lệ!</color>";
                return;
            }

            currentCash -= bjSelectedBet;
            UpdateAllCashUI();

            bjIsGameActive = true;
            bjPlayerHand.Clear();
            bjDealerHand.Clear();

            // Initial deal
            int val;
            bjPlayerHand.Add(DrawCard(out val));
            bjPlayerHand.Add(DrawCard(out val));

            bjDealerHand.Add(DrawCard(out val));
            bjDealerHand.Add(DrawCard(out val)); // Dealer's second card is hidden visually initially

            UpdateBJDisplay(false);
            
            int pScore = CalculateHandScore(bjPlayerHand);
            if (pScore == 21)
            {
                // Blackjack! Stand immediately
                BJStand();
            }
            else
            {
                if (bjResultText != null) bjResultText.text = "Rút thêm bài hoặc Dằn bài?";
                UpdateBlackjackButtons();
            }
        }

        public void BJHit()
        {
            if (!bjIsGameActive) return;

            int val;
            string card = DrawCard(out val);
            
            bool isScamLuckActive = (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining > 0);
            if (isScamLuckActive)
            {
                int currentScore = CalculateHandScore(bjPlayerHand);
                int attempts = 0;
                // Keep drawing until the card doesn't bust the player
                while (currentScore + (val == 11 ? 1 : val) > 21 && attempts < 100)
                {
                    card = DrawCard(out val);
                    attempts++;
                }
            }

            bjPlayerHand.Add(card);
            UpdateBJDisplay(false);

            int score = CalculateHandScore(bjPlayerHand);
            if (score > 21)
            {
                // Bust!
                bjIsGameActive = false;
                if (bjResultText != null) bjResultText.text = $"<color=#ff4d4d>Bị quắc! ({score} điểm). Bạn thua -{bjSelectedBet.ToString("N0")}đ</color>";
                betCount++;
                UpdateBlackjackButtons();
                CheckBankruptcy(blackjackPanel);
            }
        }

        public void BJStand()
        {
            if (!bjIsGameActive) return;
            bjIsGameActive = false;

            StartCoroutine(BJDealerPlayRoutine());
        }

        private IEnumerator BJDealerPlayRoutine()
        {
            if (bjResultText != null) bjResultText.text = "Nhà cái đang đi bài...";
            yield return new WaitForSeconds(1.0f);

            int pScore = CalculateHandScore(bjPlayerHand);
            int dScore = CalculateHandScore(bjDealerHand);

            // Game logic: 10 trận đầu xác suất 50/50, sau 10 trận thua liên tục
            bool isScamLuckActive = (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining > 0);
            bool shouldWin = isScamLuckActive ? true : ((betCount < 10) ? (Random.value < 0.5f) : false);

            // Dealer hits if score < 17
            while (dScore < 17 && dScore <= pScore && pScore <= 21)
            {
                int val;
                bjDealerHand.Add(DrawCard(out val));
                dScore = CalculateHandScore(bjDealerHand);
                UpdateBJDisplay(true);
                yield return new WaitForSeconds(0.8f);
            }

            // Force dealer to draw cards until they bust if player should win
            if (shouldWin && pScore <= 21 && dScore >= pScore && dScore <= 21)
            {
                int attempts = 0;
                while (dScore <= 21 && attempts < 10)
                {
                    int val;
                    bjDealerHand.Add(DrawCard(out val));
                    dScore = CalculateHandScore(bjDealerHand);
                    UpdateBJDisplay(true);
                    yield return new WaitForSeconds(0.8f);
                    attempts++;
                }
            }

            // Force cheat if player should lose
            if (!shouldWin && pScore <= 21 && dScore <= pScore)
            {
                // Cheat! Force dealer score to beat player score
                int targetScore = Random.Range(pScore + 1, 22);
                int diff = targetScore - dScore;
                if (diff > 0)
                {
                    bjDealerHand.Add($"{diff}♦ (Cheat)");
                    dScore = targetScore;
                    UpdateBJDisplay(true);
                    yield return new WaitForSeconds(0.5f);
                }
            }

            UpdateBJDisplay(true);

            if (dScore > 21)
            {
                // Dealer bust
                currentCash += bjSelectedBet * 2;
                if (bjResultText != null) bjResultText.text = $"<color=#5cb85c>Nhà cái quắc ({dScore})! Bạn thắng +{bjSelectedBet.ToString("N0")}đ</color>";
            }
            else if (dScore > pScore)
            {
                // Dealer wins
                if (bjResultText != null) bjResultText.text = $"<color=#ff4d4d>Nhà cái thắng ({dScore} vs {pScore})! Bạn mất -{bjSelectedBet.ToString("N0")}đ</color>";
            }
            else if (dScore < pScore)
            {
                // Player wins
                currentCash += bjSelectedBet * 2;
                if (bjResultText != null) bjResultText.text = $"<color=#5cb85c>Bạn thắng ({pScore} vs {dScore})! Bạn nhận +{bjSelectedBet.ToString("N0")}đ</color>";
            }
            else
            {
                // Draw
                currentCash += bjSelectedBet;
                if (bjResultText != null) bjResultText.text = $"Hòa điểm ({pScore})! Hoàn lại tiền cược.";
            }

            if (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining > 0)
            {
                EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining--;
            }

            betCount++;
            bjSelectedBet = Mathf.Min(bjSelectedBet, currentCash);
            UpdateBJUI();

            UpdateAllCashUI();
            UpdateBlackjackButtons();
            CheckBankruptcy(blackjackPanel);
        }

        private void UpdateBJDisplay(bool showAllDealerCards)
        {
            ClearContainer(bjPlayerCardsContainer);
            foreach (var cardStr in bjPlayerHand)
            {
                CreateCardUI(cardStr, bjPlayerCardsContainer, false);
            }

            ClearContainer(bjDealerCardsContainer);
            for (int i = 0; i < bjDealerHand.Count; i++)
            {
                bool isHidden = (i == 1 && !showAllDealerCards);
                CreateCardUI(bjDealerHand[i], bjDealerCardsContainer, isHidden);
            }
            
            int pScore = CalculateHandScore(bjPlayerHand);
            int dScore = CalculateHandScore(bjDealerHand);
            
            string scoreInfo = $"Bạn: {pScore}đ | Nhà cái: {(showAllDealerCards ? dScore.ToString() + "đ" : "?")}";
            if (bjResultText != null && bjIsGameActive)
            {
                bjResultText.text = $"Rút bài hoặc Dằn bài?\n<size=13><color=#aaaaaa>{scoreInfo}</color></size>";
            }
        }

        private void ClearContainer(Transform container)
        {
            if (container == null) return;
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }

        private void CreateCardUI(string cardStr, Transform parent, bool isHidden)
        {
            if (parent == null) return;

            GameObject cardObj = new GameObject("Card");
            cardObj.transform.SetParent(parent, false);
            RectTransform rect = cardObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(50, 75);

            Image bgImg = cardObj.AddComponent<Image>();
            bgImg.sprite = cardBackgroundSprite;
            bgImg.type = Image.Type.Sliced;

            if (isHidden)
            {
                // Base background color: Luxury Dark Navy Blue
                bgImg.color = ColorFromHex("#111E2E");

                // Gold Border: Size 46x71 (leaves a 2px margin around 50x75 card)
                GameObject goldBorderObj = new GameObject("GoldBorder");
                goldBorderObj.transform.SetParent(cardObj.transform, false);
                RectTransform borderRect = goldBorderObj.AddComponent<RectTransform>();
                borderRect.anchorMin = new Vector2(0.5f, 0.5f);
                borderRect.anchorMax = new Vector2(0.5f, 0.5f);
                borderRect.pivot = new Vector2(0.5f, 0.5f);
                borderRect.anchoredPosition = Vector2.zero;
                borderRect.sizeDelta = new Vector2(46, 71);

                Image borderImg = goldBorderObj.AddComponent<Image>();
                borderImg.sprite = cardBackgroundSprite;
                borderImg.type = Image.Type.Sliced;
                borderImg.color = ColorFromHex("#D4AF37"); // Royal Gold

                // Inner Pattern Background: Size 42x67
                GameObject innerBgObj = new GameObject("InnerBg");
                innerBgObj.transform.SetParent(goldBorderObj.transform, false);
                RectTransform innerRect = innerBgObj.AddComponent<RectTransform>();
                innerRect.anchorMin = new Vector2(0.5f, 0.5f);
                innerRect.anchorMax = new Vector2(0.5f, 0.5f);
                innerRect.pivot = new Vector2(0.5f, 0.5f);
                innerRect.anchoredPosition = Vector2.zero;
                innerRect.sizeDelta = new Vector2(42, 67);

                Image innerImg = innerBgObj.AddComponent<Image>();
                innerImg.sprite = cardBackgroundSprite;
                innerImg.type = Image.Type.Sliced;
                innerImg.color = ColorFromHex("#111E2E");

                // Gold Spade Logo at center
                GameObject logoObj = new GameObject("Logo");
                logoObj.transform.SetParent(innerBgObj.transform, false);
                RectTransform logoRect = logoObj.AddComponent<RectTransform>();
                logoRect.anchorMin = new Vector2(0.5f, 0.5f);
                logoRect.anchorMax = new Vector2(0.5f, 0.5f);
                logoRect.pivot = new Vector2(0.5f, 0.5f);
                logoRect.anchoredPosition = Vector2.zero;
                logoRect.sizeDelta = new Vector2(18, 18);

                Image logoImg = logoObj.AddComponent<Image>();
                logoImg.sprite = suitSpadeSprite;
                logoImg.color = ColorFromHex("#D4AF37"); // Gold spade logo
                return;
            }

            // --- Card Front ---
            bgImg.color = Color.white;

            string rank = "";
            string suit = "";
            
            if (cardStr.Contains("Cheat"))
            {
                string clean = cardStr.Split('♦')[0];
                rank = clean.Trim();
                suit = "♦";
            }
            else
            {
                rank = cardStr.Substring(0, cardStr.Length - 1);
                suit = cardStr.Substring(cardStr.Length - 1);
            }

            bool isRed = (suit == "♥" || suit == "♦");
            Color cardColor = isRed ? ColorFromHex("#E74C3C") : ColorFromHex("#2C3E50");

            Sprite suitSprite = suitSpadeSprite;
            if (suit == "♥") suitSprite = suitHeartSprite;
            else if (suit == "♦") suitSprite = suitDiamondSprite;
            else if (suit == "♠") suitSprite = suitSpadeSprite;
            else if (suit == "♣") suitSprite = suitClubSprite;

            // 1. Top-Left Corner Group
            GameObject topLeftGroup = new GameObject("TopLeftGroup");
            topLeftGroup.transform.SetParent(cardObj.transform, false);
            RectTransform tlRect = topLeftGroup.AddComponent<RectTransform>();
            tlRect.anchorMin = new Vector2(0f, 1f);
            tlRect.anchorMax = new Vector2(0f, 1f);
            tlRect.pivot = new Vector2(0.5f, 0.5f);
            tlRect.anchoredPosition = new Vector2(10, -15);
            tlRect.sizeDelta = new Vector2(14, 25);

            // Top-Left Value Text
            GameObject tlTextObj = new GameObject("ValueText");
            tlTextObj.transform.SetParent(topLeftGroup.transform, false);
            RectTransform tlTextRect = tlTextObj.AddComponent<RectTransform>();
            tlTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            tlTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            tlTextRect.pivot = new Vector2(0.5f, 0.5f);
            tlTextRect.anchoredPosition = new Vector2(0, 6);
            tlTextRect.sizeDelta = new Vector2(14, 12);

            TextMeshProUGUI tlTmp = tlTextObj.AddComponent<TextMeshProUGUI>();
            tlTmp.text = rank;
            tlTmp.fontSize = 11;
            tlTmp.fontStyle = FontStyles.Bold;
            tlTmp.color = cardColor;
            tlTmp.alignment = TextAlignmentOptions.Center;

            // Top-Left Mini-Suit
            GameObject tlSuitObj = new GameObject("SuitImage");
            tlSuitObj.transform.SetParent(topLeftGroup.transform, false);
            RectTransform tlSuitRect = tlSuitObj.AddComponent<RectTransform>();
            tlSuitRect.anchorMin = new Vector2(0.5f, 0.5f);
            tlSuitRect.anchorMax = new Vector2(0.5f, 0.5f);
            tlSuitRect.pivot = new Vector2(0.5f, 0.5f);
            tlSuitRect.anchoredPosition = new Vector2(0, -6);
            tlSuitRect.sizeDelta = new Vector2(10, 10);

            Image tlSuitImg = tlSuitObj.AddComponent<Image>();
            tlSuitImg.sprite = suitSprite;
            tlSuitImg.color = cardColor;

            // 2. Bottom-Right Corner Group (Rotated 180 degrees)
            GameObject bottomRightGroup = new GameObject("BottomRightGroup");
            bottomRightGroup.transform.SetParent(cardObj.transform, false);
            RectTransform brRect = bottomRightGroup.AddComponent<RectTransform>();
            brRect.anchorMin = new Vector2(1f, 0f);
            brRect.anchorMax = new Vector2(1f, 0f);
            brRect.pivot = new Vector2(0.5f, 0.5f);
            brRect.anchoredPosition = new Vector2(-10, 15);
            brRect.sizeDelta = new Vector2(14, 25);
            brRect.localEulerAngles = new Vector3(0, 0, 180f); // Rotate 180 degrees

            // Bottom-Right Value Text
            GameObject brTextObj = new GameObject("ValueText");
            brTextObj.transform.SetParent(bottomRightGroup.transform, false);
            RectTransform brTextRect = brTextObj.AddComponent<RectTransform>();
            brTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            brTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            brTextRect.pivot = new Vector2(0.5f, 0.5f);
            brTextRect.anchoredPosition = new Vector2(0, 6);
            brTextRect.sizeDelta = new Vector2(14, 12);

            TextMeshProUGUI brTmp = brTextObj.AddComponent<TextMeshProUGUI>();
            brTmp.text = rank;
            brTmp.fontSize = 11;
            brTmp.fontStyle = FontStyles.Bold;
            brTmp.color = cardColor;
            brTmp.alignment = TextAlignmentOptions.Center;

            // Bottom-Right Mini-Suit
            GameObject brSuitObj = new GameObject("SuitImage");
            brSuitObj.transform.SetParent(bottomRightGroup.transform, false);
            RectTransform brSuitRect = brSuitObj.AddComponent<RectTransform>();
            brSuitRect.anchorMin = new Vector2(0.5f, 0.5f);
            brSuitRect.anchorMax = new Vector2(0.5f, 0.5f);
            brSuitRect.pivot = new Vector2(0.5f, 0.5f);
            brSuitRect.anchoredPosition = new Vector2(0, -6);
            brSuitRect.sizeDelta = new Vector2(10, 10);

            Image brSuitImg = brSuitObj.AddComponent<Image>();
            brSuitImg.sprite = suitSprite;
            brSuitImg.color = cardColor;

            // 3. Center Large Suit Image
            GameObject centerSuitObj = new GameObject("CenterSuitImage");
            centerSuitObj.transform.SetParent(cardObj.transform, false);
            RectTransform centerSuitRect = centerSuitObj.AddComponent<RectTransform>();
            centerSuitRect.anchorMin = new Vector2(0.5f, 0.5f);
            centerSuitRect.anchorMax = new Vector2(0.5f, 0.5f);
            centerSuitRect.pivot = new Vector2(0.5f, 0.5f);
            centerSuitRect.anchoredPosition = Vector2.zero; // Centered
            centerSuitRect.sizeDelta = new Vector2(20, 20);

            Image centerSuitImg = centerSuitObj.AddComponent<Image>();
            centerSuitImg.sprite = suitSprite;
            centerSuitImg.color = cardColor;
        }

        private Color ColorFromHex(string hex)
        {
            Color col;
            if (ColorUtility.TryParseHtmlString(hex, out col))
            {
                return col;
            }
            return Color.white;
        }

        private void UpdateBlackjackButtons()
        {
            if (bjHitBtn != null) bjHitBtn.interactable = bjIsGameActive;
            if (bjStandBtn != null) bjStandBtn.interactable = bjIsGameActive;
            if (bjDealBtn != null) bjDealBtn.interactable = !bjIsGameActive;
        }

        private string DrawCard(out int value)
        {
            string[] suits = { "♠", "♣", "♥", "♦" };
            string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            int rIndex = Random.Range(0, ranks.Length);
            int sIndex = Random.Range(0, suits.Length);
            
            string rank = ranks[rIndex];
            string suit = suits[sIndex];

            if (rank == "A") value = 11;
            else if (rank == "J" || rank == "Q" || rank == "K") value = 10;
            else value = int.Parse(rank);

            return $"{rank}{suit}";
        }

        private int CalculateHandScore(List<string> hand)
        {
            int score = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                // Handle cheat card representation
                if (card.Contains("Cheat"))
                {
                    string cleanVal = card.Split('♦')[0];
                    score += int.Parse(cleanVal);
                    continue;
                }

                string rank = card.Substring(0, card.Length - 1);
                if (rank == "A")
                {
                    score += 11;
                    aceCount++;
                }
                else if (rank == "J" || rank == "Q" || rank == "K")
                {
                    score += 10;
                }
                else
                {
                    score += int.Parse(rank);
                }
            }

            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }

            return score;
        }


        // ── 3. ROULETTE GAMEPLAY LOGIC ─────────────────────────────────────
        public void OpenRouletteUI()
        {
            if (currentCash <= 0)
            {
                InteractWithRecruiter();
                return;
            }

            if (roulettePanel != null)
            {
                rlSelectedBetType = "";
                rlSelectedBetAmount = ValidateBetAmount(100000);
                if (rlResultText != null) rlResultText.text = "Chọn cửa đặt & tiền rồi bấm Quay!";

                UpdateRouletteUI();
                UpdateAllCashUI();
                roulettePanel.SetActive(true);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                if (GameManager.Instance != null) GameManager.Instance.ChangeState(GameState.Cutscene);
            }
        }

        public void CloseRouletteUI()
        {
            if (roulettePanel != null) roulettePanel.SetActive(false);
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }
        }

        public void RLSelectBetType(string type)
        {
            if (rlIsSpinning) return;
            rlSelectedBetType = type;
            UpdateRouletteUI();
        }

        public void RLSelectBetAmount(int amount)
        {
            if (rlIsSpinning) return;

            if (amount == -1)
            {
                rlSelectedBetAmount = currentCash;
            }
            else
            {
                rlSelectedBetAmount = ValidateBetAmount(amount);
            }
            UpdateRouletteUI();
        }

        public void RLAdjustBetAmount(int delta)
        {
            if (rlIsSpinning) return;
            int target = rlSelectedBetAmount + delta;
            rlSelectedBetAmount = ValidateBetAmount(target);
            UpdateRouletteUI();
        }

        private void RLOnBetInputEndEdit(string text)
        {
            if (int.TryParse(text, out int amount))
            {
                rlSelectedBetAmount = ValidateBetAmount(amount);
            }
            else
            {
                rlSelectedBetAmount = ValidateBetAmount(0);
            }
            UpdateRouletteUI();
        }

        private void UpdateRouletteUI()
        {
            string typeVN = "Chưa chọn";
            if (rlSelectedBetType == "Red") typeVN = "Đỏ";
            else if (rlSelectedBetType == "Black") typeVN = "Đen";
            else if (rlSelectedBetType == "Even") typeVN = "Chẵn";
            else if (rlSelectedBetType == "Odd") typeVN = "Lẻ";

            if (rlBetTypeText != null) rlBetTypeText.text = typeVN;
            if (rlBetAmountText != null) rlBetAmountText.text = rlSelectedBetAmount == 0 ? "Chưa đặt" : $"{rlSelectedBetAmount.ToString("N0")}đ";
            if (rlBetInputField != null && rlBetInputField.text != rlSelectedBetAmount.ToString())
            {
                rlBetInputField.text = rlSelectedBetAmount.ToString();
            }
        }

        public void RLSpin()
        {
            if (rlIsSpinning) return;
            if (string.IsNullOrEmpty(rlSelectedBetType))
            {
                if (rlResultText != null) rlResultText.text = "<color=#ff4d4d>Hãy chọn đặt cửa trước!</color>";
                return;
            }
            if (rlSelectedBetAmount <= 0 || rlSelectedBetAmount > currentCash)
            {
                if (rlResultText != null) rlResultText.text = "<color=#ff4d4d>Chọn mức đặt cược hợp lệ!</color>";
                return;
            }

            StartCoroutine(RLSpinRoutine());
        }

        private IEnumerator RLSpinRoutine()
        {
            rlIsSpinning = true;
            if (rlResultText != null) rlResultText.text = "Vòng quay đang quay...";

            // Game logic: 10 trận đầu xác suất 50/50, sau 10 trận thua liên tục
            bool isScamLuckActive = (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining > 0);
            bool shouldWin = isScamLuckActive ? true : ((betCount < 10) ? (Random.value < 0.5f) : false);

            int finalNum = 0;
            string finalColor = "";
            bool isEven = false;
            int attempts = 0;

            int[] wheelSequence = { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };
            int[] redNums = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

            do
            {
                finalNum = Random.Range(0, 37); // Include 0
                if (finalNum == 0)
                {
                    finalColor = "Green";
                    isEven = false;
                }
                else
                {
                    finalColor = System.Array.IndexOf(redNums, finalNum) >= 0 ? "Red" : "Black";
                    isEven = (finalNum % 2 == 0);
                }
                attempts++;

                bool matchesChoice = false;
                if (rlSelectedBetType == "Red" && finalColor == "Red") matchesChoice = true;
                else if (rlSelectedBetType == "Black" && finalColor == "Black") matchesChoice = true;
                else if (rlSelectedBetType == "Even" && isEven) matchesChoice = true;
                else if (rlSelectedBetType == "Odd" && !isEven) matchesChoice = true;

                if (shouldWin && matchesChoice) break;
                if (!shouldWin && !matchesChoice) break;

            } while (attempts < 100);

            // Spin animation
            float spinDuration = 2.2f;
            float elapsed = 0f;
            
            while (elapsed < spinDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / spinDuration;
                float speed = Mathf.Lerp(900f, 60f, t);
                
                if (rlWheelRect != null)
                {
                    rlWheelRect.Rotate(0, 0, -speed * Time.deltaTime);
                }

                int tempNum = Random.Range(0, 37);
                string tempCol = tempNum == 0 ? "Xanh lá" : (System.Array.IndexOf(redNums, tempNum) >= 0 ? "Đỏ" : "Đen");
                if (rlResultText != null) rlResultText.text = $"Đang quay: <color=#ffb732>{tempNum} ({tempCol})</color>";

                yield return null;
            }

            // Target landing angle for finalNum
            int slotIndex = System.Array.IndexOf(wheelSequence, finalNum);
            if (slotIndex < 0) slotIndex = 0;
            float targetAngle = 90f - (slotIndex * 360f / 37f + 180f / 37f);
            
            // Settle smoothly
            if (rlWheelRect != null)
            {
                float currentZ = rlWheelRect.localEulerAngles.z;
                float tSettle = 0f;
                while (tSettle < 1f)
                {
                    tSettle += Time.deltaTime * 2.5f; // 0.4 seconds
                    rlWheelRect.localRotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(currentZ, targetAngle, tSettle));
                    yield return null;
                }
                rlWheelRect.localRotation = Quaternion.Euler(0, 0, targetAngle);
            }

            string colorVN = finalColor == "Red" ? "Đỏ" : (finalColor == "Black" ? "Đen" : "Xanh lá");
            string parityVN = finalNum == 0 ? "Không" : (isEven ? "Chẵn" : "Lẻ");
            string colorHex = finalColor == "Red" ? "#ff4d4d" : (finalColor == "Black" ? "#555555" : "#27ae60");

            bool playerWon = false;
            if (rlSelectedBetType == "Red" && finalColor == "Red") playerWon = true;
            else if (rlSelectedBetType == "Black" && finalColor == "Black") playerWon = true;
            else if (rlSelectedBetType == "Even" && isEven) playerWon = true;
            else if (rlSelectedBetType == "Odd" && !isEven) playerWon = true;

            if (playerWon)
            {
                currentCash += rlSelectedBetAmount;
                if (rlResultText != null) rlResultText.text = $"<color=#5cb85c>Thắng! Số {finalNum} (<color={colorHex}>{colorVN}</color>, {parityVN})\nBạn nhận +{rlSelectedBetAmount.ToString("N0")}đ</color>";
            }
            else
            {
                currentCash -= rlSelectedBetAmount;
                if (rlResultText != null) rlResultText.text = $"<color=#ff4d4d>Thua! Số {finalNum} (<color={colorHex}>{colorVN}</color>, {parityVN})\nBạn mất -{rlSelectedBetAmount.ToString("N0")}đ</color>";
            }

            if (EscapeFromHell.Core.GameManager.Instance != null && EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining > 0)
            {
                EscapeFromHell.Core.GameManager.Instance.scamLuckWinsRemaining--;
            }

            betCount++;
            rlIsSpinning = false;
            
            UpdateAllCashUI();
            rlSelectedBetAmount = Mathf.Min(rlSelectedBetAmount, currentCash);
            UpdateRouletteUI();

            CheckBankruptcy(roulettePanel);
        }


        // ── 4. NPC INTERACTION & DECORATION DIALOGUES ───────────────────────
        public void InteractWithPokerTable()
        {
            if (currentCash <= 0) { InteractWithRecruiter(); return; }

            // Lần đầu vào: hiện intro
            if (!hasSeenBlackjackIntro && DialogueSystem.Instance != null)
            {
                hasSeenBlackjackIntro = true;
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Bàn chơi Blackjack/Xì Dách... Đây rồi, những lá bài tây quen thuộc." },
                    new DialogueLine { speakerName = "Minh", text = "Hãy ngồi xuống xem vận đỏ đen của mình với các lá bài xem sao." }
                };
                DialogueSystem.Instance.OnDialogueEnd += OpenBlackjackGame;
                DialogueSystem.Instance.StartDialogue(d);
                return;
            }

            // Lần sau: vào thẳng
            OpenBlackjackGame();
        }

        private void OpenBlackjackGame()
        {
            DialogueSystem.Instance.OnDialogueEnd -= OpenBlackjackGame;
            OpenBlackjackUI();
        }

        public void InteractWithRouletteTable()
        {
            if (currentCash <= 0) { InteractWithRecruiter(); return; }

            // Lần đầu vào: hiện intro
            if (!hasSeenRouletteIntro && DialogueSystem.Instance != null)
            {
                hasSeenRouletteIntro = true;
                DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Vòng quay Roulette... Trông những ô màu đỏ đen chuyển động thật cuốn hút." },
                    new DialogueLine { speakerName = "Minh", text = "Biết đâu một quả bóng nhỏ lăn trúng ô cược sẽ giúp mình đổi đời." }
                };
                DialogueSystem.Instance.OnDialogueEnd += OpenRouletteGame;
                DialogueSystem.Instance.StartDialogue(d);
                return;
            }

            // Lần sau: vào thẳng
            OpenRouletteGame();
        }

        private void OpenRouletteGame()
        {
            DialogueSystem.Instance.OnDialogueEnd -= OpenRouletteGame;
            OpenRouletteUI();
        }

        public void InteractWithRecruiter()
        {
            if (DialogueSystem.Instance == null) return;

            DialogueData npcDial = ScriptableObject.CreateInstance<DialogueData>();
            if (currentCash > 0)
            {
                npcDial.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Người lạ", text = "Chào em trai! Đang kẹt tiền hả? Cứ vào trong làm vài ván bài xem sao." },
                    new DialogueLine { speakerName = "Người lạ", text = "Biết đâu vận may đổi đời, kiếm vài chục triệu dễ như chơi ấy chứ." }
                };
                DialogueSystem.Instance.StartDialogue(npcDial);
            }
            else
            {
                npcDial.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Người lạ mặt", text = "Sao rồi em trai? Thua sạch túi rồi à? Trông em thảm hại quá." },
                    new DialogueLine { speakerName = "Minh", text = "Tôi... tôi mất hết rồi. Bố tôi đang đợi tiền phẫu thuật ở quê... Tôi bế tắc quá..." },
                    new DialogueLine { speakerName = "Người lạ mặt", text = "Anh thấy em hiền lành nên muốn giúp. Anh có mối làm việc văn phòng bên Campuchia, CSKH game." },
                    new DialogueLine { speakerName = "Người lạ mặt", text = "Lương 40 triệu một tháng, bao ăn ở, visa, vé máy bay anh lo hết. Sang đó chăm chỉ làm 1 tháng là có tiền gửi về cứu bố ngay." },
                    new DialogueLine { speakerName = "Minh", text = "Campuchia... Có phải là lừa đảo không..." },
                    new DialogueLine { speakerName = "Người lạ mặt", text = "Ôi giời, giờ này còn kén chọn. Không phẫu thuật ngay thì bố em què cả đời đấy. Đi hay không để anh xếp xe đón luôn tối nay?" },
                    new DialogueLine { speakerName = "Minh", text = "Tôi... tôi đi! Tôi không còn sự lựa chọn nào khác..." }
                };
                
                DialogueSystem.Instance.OnDialogueEnd += TransitionToChapter3;
                DialogueSystem.Instance.StartDialogue(npcDial);
            }
        }

        public void InteractWithLeftDoorGuard1()
        {
            if (DialogueSystem.Instance == null) return;

            DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
            d.lines = new List<DialogueLine> {
                new DialogueLine { speakerName = "Bảo vệ cổng (Trái 1)", text = "Cửa này là lối thoát hiểm và lối ra bãi xe." },
                new DialogueLine { speakerName = "Bảo vệ cổng (Trái 1)", text = "Muốn ra về thì cứ mở cửa. Nhưng anh khuyên chú em nên ở lại chơi thêm vài ván nữa gỡ gạc." }
            };
            DialogueSystem.Instance.StartDialogue(d);
        }

        public void InteractWithLeftDoorGuard2()
        {
            if (DialogueSystem.Instance == null) return;

            DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
            d.lines = new List<DialogueLine> {
                new DialogueLine { speakerName = "Bảo vệ cổng (Trái 2)", text = "Chúc em trai thượng lộ bình an nếu quyết định ra về." },
                new DialogueLine { speakerName = "Bảo vệ cổng (Trái 2)", text = "Nhớ kiểm tra lại ví tiền xem còn đồng nào trước khi bước ra khỏi sòng bạc nhé!" }
            };
            DialogueSystem.Instance.StartDialogue(d);
        }

        public void InteractWithLeftExitDoor()
        {
            if (DialogueSystem.Instance == null) return;

            DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
            if (currentCash > 0)
            {
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Mình vẫn còn tiền mặt... Nhưng mình phải kiếm đủ tiền viện phí phẫu thuật cho Bố." },
                    new DialogueLine { speakerName = "Minh", text = "Bố đang trông chờ vào mình... Mình không thể bỏ cuộc giữa chừng để đi về tay trắng lúc này được!" }
                };
            }
            else
            {
                d.lines = new List<DialogueLine> {
                    new DialogueLine { speakerName = "Minh", text = "Mình đã thua sạch túi rồi... Không còn một đồng xu dính túi..." },
                    new DialogueLine { speakerName = "Minh", text = "Bố vẫn đang đợi tiền phẫu thuật ở quê... Giờ đi về thì lấy gì ăn nói với Mẹ đây... Mình biết phải làm sao đây..." }
                };
            }
            DialogueSystem.Instance.StartDialogue(d);
        }

        public void InteractWithVipStairsGuard()
        {
            if (DialogueSystem.Instance == null) return;

            DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
            d.lines = new List<DialogueLine> {
                new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Dừng lại! Tầng 2 là khu vực VIP dành riêng cho giới siêu giàu." },
                new DialogueLine { speakerName = "Vệ sĩ cầu thang VIP", text = "Tài khoản của cậu phải có ít nhất 10.000.000đ mới được phép bước lên đây." },
                new DialogueLine { speakerName = "Minh", text = "10.000.000đ sao... Mình chỉ có 500.000đ. Đành chơi ở tầng 1 vậy." }
            };
            DialogueSystem.Instance.StartDialogue(d);
        }

        private void TransitionToChapter3()
        {
            DialogueSystem.Instance.OnDialogueEnd -= TransitionToChapter3;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetChapter(3);
                GameManager.Instance.LoadScene(GameManager.Instance.chapter3Scene);
            }
        }
    }
}
