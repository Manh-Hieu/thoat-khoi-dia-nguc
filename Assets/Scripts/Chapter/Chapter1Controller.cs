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

        [Header("Progress Tracking")]
        public bool hasReadPhone = false;
        public bool hasReadBills = false;
        public bool hasReadLaptop = false;
        private bool climaxTriggered = false;

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

        private void Start()
        {
            // Start the game by playing intro dialogue after a brief delay
            StartCoroutine(StartIntroRoutine());
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

        private void CheckProgress()
        {
            if (hasReadPhone && hasReadBills && hasReadLaptop && !climaxTriggered)
            {
                climaxTriggered = true;
                StartCoroutine(TriggerClimaxRoutine());
            }
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

            // Trigger job offer dialogue
            if (DialogueSystem.Instance != null && jobOfferDialogue != null)
            {
                DialogueSystem.Instance.StartDialogue(jobOfferDialogue);
                DialogueSystem.Instance.OnDialogueEnd += OnClimaxDialogueEnd;
            }
        }

        private void OnClimaxDialogueEnd()
        {
            DialogueSystem.Instance.OnDialogueEnd -= OnClimaxDialogueEnd;
            // Load Chapter 3 Compound (skip Chapter 2 visual cutscene and move straight to gameplay, or load a transition)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetChapter(3);
                GameManager.Instance.LoadScene(GameManager.Instance.chapter3Scene);
            }
        }
    }
}
