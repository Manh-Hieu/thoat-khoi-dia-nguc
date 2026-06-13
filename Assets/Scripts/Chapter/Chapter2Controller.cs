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
        private int betCount = 0;

        // ── TÀI XỈU (DICE) UI & STATE ──────────────────────────────────────
        private GameObject taiXiuPanel;
        private TextMeshProUGUI cashText;
        private TextMeshProUGUI betTypeText;
        private TextMeshProUGUI betAmountText;
        private TextMeshProUGUI dice1Text;
        private TextMeshProUGUI dice2Text;
        private TextMeshProUGUI dice3Text;
        private TextMeshProUGUI resultText;

        private string selectedBetType = ""; // "Tai" or "Xiu"
        private int selectedBetAmount = 0;
        private bool isRolling = false;

        // ── BLACKJACK (XÌ DÁCH) UI & STATE ─────────────────────────────────
        private GameObject blackjackPanel;
        private TextMeshProUGUI bjCashText;
        private TextMeshProUGUI bjBetAmountText;
        private TextMeshProUGUI bjPlayerCardsText;
        private TextMeshProUGUI bjDealerCardsText;
        private TextMeshProUGUI bjResultText;
        private Button bjHitBtn;
        private Button bjStandBtn;
        private Button bjDealBtn;

        private int bjSelectedBet = 0;
        private List<string> bjPlayerHand = new List<string>();
        private List<string> bjDealerHand = new List<string>();
        private bool bjIsGameActive = false;

        // ── ROULETTE UI & STATE ────────────────────────────────────────────
        private GameObject roulettePanel;
        private TextMeshProUGUI rlCashText;
        private TextMeshProUGUI rlBetTypeText;
        private TextMeshProUGUI rlBetAmountText;
        private TextMeshProUGUI rlResultText;

        private string rlSelectedBetType = ""; // "Red", "Black", "Even", "Odd"
        private int rlSelectedBetAmount = 0;
        private bool rlIsSpinning = false;

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
                dice1Text = FindTextRecursive(txT, "Dice1Text");
                dice2Text = FindTextRecursive(txT, "Dice2Text");
                dice3Text = FindTextRecursive(txT, "Dice3Text");
                resultText = FindTextRecursive(txT, "ResultText");

                BindButton(txT, "TaiButton", () => SelectBetType("Tai"));
                BindButton(txT, "XiuButton", () => SelectBetType("Xiu"));
                BindButton(txT, "Bet100kButton", () => SelectBetAmount(100000));
                BindButton(txT, "Bet200kButton", () => SelectBetAmount(200000));
                BindButton(txT, "Bet500kButton", () => SelectBetAmount(500000));
                BindButton(txT, "BetAllInButton", () => SelectBetAmount(-1));
                BindButton(txT, "RollButton", RollDice);
                BindButton(txT, "CloseButton", CloseTaiXiuUI);
            }

            // 2. BLACKJACK PANEL BINDINGS
            Transform bjT = FindTransformRecursive(canvasT, "BlackjackPanel");
            if (bjT != null)
            {
                blackjackPanel = bjT.gameObject;
                bjCashText = FindTextRecursive(bjT, "CashText");
                bjBetAmountText = FindTextRecursive(bjT, "BetAmountText");
                bjPlayerCardsText = FindTextRecursive(bjT, "PlayerCardsText");
                bjDealerCardsText = FindTextRecursive(bjT, "DealerCardsText");
                bjResultText = FindTextRecursive(bjT, "ResultText");

                bjHitBtn = BindButton(bjT, "HitButton", BJHit);
                bjStandBtn = BindButton(bjT, "StandButton", BJStand);
                bjDealBtn = BindButton(bjT, "DealButton", BJStartGame);

                BindButton(bjT, "Bet100kButton", () => BJSelectBetAmount(100000));
                BindButton(bjT, "Bet200kButton", () => BJSelectBetAmount(200000));
                BindButton(bjT, "Bet500kButton", () => BJSelectBetAmount(500000));
                BindButton(bjT, "BetAllInButton", () => BJSelectBetAmount(-1));
                BindButton(bjT, "CloseButton", CloseBlackjackUI);
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

                BindButton(rlT, "RedButton", () => RLSelectBetType("Red"));
                BindButton(rlT, "BlackButton", () => RLSelectBetType("Black"));
                BindButton(rlT, "EvenButton", () => RLSelectBetType("Even"));
                BindButton(rlT, "OddButton", () => RLSelectBetType("Odd"));

                BindButton(rlT, "Bet100kButton", () => RLSelectBetAmount(100000));
                BindButton(rlT, "Bet200kButton", () => RLSelectBetAmount(200000));
                BindButton(rlT, "Bet500kButton", () => RLSelectBetAmount(500000));
                BindButton(rlT, "BetAllInButton", () => RLSelectBetAmount(-1));
                BindButton(rlT, "SpinButton", RLSpin);
                BindButton(rlT, "CloseButton", CloseRouletteUI);
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

            if (taiXiuPanel != null)
            {
                selectedBetType = "";
                selectedBetAmount = 0;
                if (resultText != null) resultText.text = "Chọn Tài/Xỉu & mức cược rồi bấm Xốc!";
                if (dice1Text != null) dice1Text.text = "-";
                if (dice2Text != null) dice2Text.text = "-";
                if (dice3Text != null) dice3Text.text = "-";
                
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

        public void SelectBetAmount(int amount)
        {
            if (isRolling) return;

            if (amount == -1)
            {
                selectedBetAmount = currentCash;
            }
            else
            {
                if (amount > currentCash)
                {
                    if (resultText != null) resultText.text = "<color=#ff4d4d>Bạn không có đủ tiền!</color>";
                    return;
                }
                selectedBetAmount = amount;
            }
            UpdateTaiXiuUI();
        }

        private void UpdateTaiXiuUI()
        {
            if (betTypeText != null) betTypeText.text = string.IsNullOrEmpty(selectedBetType) ? "Chưa chọn" : (selectedBetType == "Tai" ? "Tài" : "Xỉu");
            if (betAmountText != null) betAmountText.text = selectedBetAmount == 0 ? "Chưa đặt" : $"{selectedBetAmount.ToString("N0")}đ";
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

        private IEnumerator RollDiceRoutine()
        {
            isRolling = true;
            if (resultText != null) resultText.text = "Đang xốc xúc xắc...";

            for (int i = 0; i < 10; i++)
            {
                if (dice1Text != null) dice1Text.text = Random.Range(1, 7).ToString();
                if (dice2Text != null) dice2Text.text = Random.Range(1, 7).ToString();
                if (dice3Text != null) dice3Text.text = Random.Range(1, 7).ToString();
                yield return new WaitForSeconds(0.1f);
            }

            // Rigged calculation (Win first 3 bets, then lose)
            bool shouldWin = (betCount < 3) && (Random.value < 0.75f);

            int d1, d2, d3, total;
            bool isTaiResult;
            int attempts = 0;

            do
            {
                d1 = Random.Range(1, 7);
                d2 = Random.Range(1, 7);
                d3 = Random.Range(1, 7);
                total = d1 + d2 + d3;
                isTaiResult = (total >= 11);
                attempts++;

                bool matchesChoice = (selectedBetType == "Tai" && isTaiResult) || (selectedBetType == "Xiu" && !isTaiResult);
                if (shouldWin && matchesChoice) break;
                if (!shouldWin && !matchesChoice) break;

            } while (attempts < 100);

            if (dice1Text != null) dice1Text.text = d1.ToString();
            if (dice2Text != null) dice2Text.text = d2.ToString();
            if (dice3Text != null) dice3Text.text = d3.ToString();

            string resultStr = isTaiResult ? "Tài" : "Xỉu";
            bool playerWon = (selectedBetType == "Tai" && isTaiResult) || (selectedBetType == "Xiu" && !isTaiResult);

            if (playerWon)
            {
                currentCash += selectedBetAmount;
                if (resultText != null) resultText.text = $"<color=#5cb85c>Thắng! {d1}+{d2}+{d3}={total} ({resultStr})\nBạn nhận +{selectedBetAmount.ToString("N0")}đ</color>";
            }
            else
            {
                currentCash -= selectedBetAmount;
                if (resultText != null) resultText.text = $"<color=#ff4d4d>Thua! {d1}+{d2}+{d3}={total} ({resultStr})\nBạn mất -{selectedBetAmount.ToString("N0")}đ</color>";
            }

            betCount++;
            isRolling = false;
            
            UpdateAllCashUI();
            selectedBetAmount = Mathf.Min(selectedBetAmount, currentCash);
            UpdateTaiXiuUI();

            CheckBankruptcy(taiXiuPanel);
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
                bjSelectedBet = 0;
                bjIsGameActive = false;
                bjPlayerHand.Clear();
                bjDealerHand.Clear();
                
                if (bjPlayerCardsText != null) bjPlayerCardsText.text = "-";
                if (bjDealerCardsText != null) bjDealerCardsText.text = "-";
                if (bjResultText != null) bjResultText.text = "Đặt tiền cược rồi bấm Chia Bài!";

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
                if (amount > currentCash)
                {
                    if (bjResultText != null) bjResultText.text = "<color=#ff4d4d>Bạn không có đủ tiền!</color>";
                    return;
                }
                bjSelectedBet = amount;
            }

            if (bjBetAmountText != null) bjBetAmountText.text = $"{bjSelectedBet.ToString("N0")}đ";
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
            bjPlayerHand.Add(DrawCard(out val));
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

            // Rigged logic: If betCount >= 3, player always loses
            bool shouldWin = (betCount < 3) && (Random.value < 0.75f);

            // Dealer hits if score < 17
            while (dScore < 17 && dScore <= pScore && pScore <= 21)
            {
                int val;
                bjDealerHand.Add(DrawCard(out val));
                dScore = CalculateHandScore(bjDealerHand);
                UpdateBJDisplay(true);
                yield return new WaitForSeconds(0.8f);
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

            betCount++;
            bjSelectedBet = Mathf.Min(bjSelectedBet, currentCash);
            if (bjBetAmountText != null) bjBetAmountText.text = $"{bjSelectedBet.ToString("N0")}đ";

            UpdateAllCashUI();
            UpdateBlackjackButtons();
            CheckBankruptcy(blackjackPanel);
        }

        private void UpdateBJDisplay(bool showAllDealerCards)
        {
            int pScore = CalculateHandScore(bjPlayerHand);
            if (bjPlayerCardsText != null) bjPlayerCardsText.text = $"{string.Join(", ", bjPlayerHand)} ({pScore}đ)";

            if (showAllDealerCards)
            {
                int dScore = CalculateHandScore(bjDealerHand);
                if (bjDealerCardsText != null) bjDealerCardsText.text = $"{string.Join(", ", bjDealerHand)} ({dScore}đ)";
            }
            else
            {
                if (bjDealerHand.Count > 0)
                {
                    if (bjDealerCardsText != null) bjDealerCardsText.text = $"{bjDealerHand[0]}, [Ẩn]";
                }
            }
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
                rlSelectedBetAmount = 0;
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
                if (amount > currentCash)
                {
                    if (rlResultText != null) rlResultText.text = "<color=#ff4d4d>Bạn không có đủ tiền!</color>";
                    return;
                }
                rlSelectedBetAmount = amount;
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

            string[] colors = { "Đỏ", "Đen" };
            for (int i = 0; i < 15; i++)
            {
                int num = Random.Range(1, 37);
                string col = colors[Random.Range(0, 2)];
                if (rlResultText != null) rlResultText.text = $"Đang quay: <color=#ffb732>{num} ({col})</color>";
                yield return new WaitForSeconds(0.08f);
            }

            // Rigged Roulette
            bool shouldWin = (betCount < 3) && (Random.value < 0.75f);

            int finalNum = 0;
            string finalColor = "";
            bool isEven = false;
            int attempts = 0;

            int[] redNums = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

            do
            {
                finalNum = Random.Range(1, 37);
                finalColor = System.Array.IndexOf(redNums, finalNum) >= 0 ? "Red" : "Black";
                isEven = (finalNum % 2 == 0);
                attempts++;

                bool matchesChoice = false;
                if (rlSelectedBetType == "Red" && finalColor == "Red") matchesChoice = true;
                else if (rlSelectedBetType == "Black" && finalColor == "Black") matchesChoice = true;
                else if (rlSelectedBetType == "Even" && isEven) matchesChoice = true;
                else if (rlSelectedBetType == "Odd" && !isEven) matchesChoice = true;

                if (shouldWin && matchesChoice) break;
                if (!shouldWin && !matchesChoice) break;

            } while (attempts < 100);

            string colorVN = finalColor == "Red" ? "Đỏ" : "Đen";
            string parityVN = isEven ? "Chẵn" : "Lẻ";
            string colorHex = finalColor == "Red" ? "#ff4d4d" : "#555555";

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
            if (DialogueSystem.Instance == null) return;

            DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
            d.lines = new List<DialogueLine> {
                new DialogueLine { speakerName = "Minh", text = "Bàn chơi Blackjack/Xì Dách... Đây rồi, những lá bài tây quen thuộc." },
                new DialogueLine { speakerName = "Minh", text = "Hãy ngồi xuống xem vận đỏ đen của mình với các lá bài xem sao." }
            };
            DialogueSystem.Instance.OnDialogueEnd += OpenBlackjackGame;
            DialogueSystem.Instance.StartDialogue(d);
        }

        private void OpenBlackjackGame()
        {
            DialogueSystem.Instance.OnDialogueEnd -= OpenBlackjackGame;
            OpenBlackjackUI();
        }

        public void InteractWithRouletteTable()
        {
            if (DialogueSystem.Instance == null) return;

            DialogueData d = ScriptableObject.CreateInstance<DialogueData>();
            d.lines = new List<DialogueLine> {
                new DialogueLine { speakerName = "Minh", text = "Vòng quay Roulette... Trông những ô màu đỏ đen chuyển động thật cuốn hút." },
                new DialogueLine { speakerName = "Minh", text = "Biết đâu một quả bóng nhỏ lăn trúng ô cược sẽ giúp mình đổi đời." }
            };
            DialogueSystem.Instance.OnDialogueEnd += OpenRouletteGame;
            DialogueSystem.Instance.StartDialogue(d);
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
