using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using EscapeFromHell.Core;

namespace EscapeFromHell.Systems
{
    public class DialogueSystem : MonoBehaviour
    {
        public static DialogueSystem Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private GameObject continueIndicator;

        [Header("Branching Choices UI")]
        [SerializeField] private GameObject choicesPanel;
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Settings")]
        [SerializeField] private float typingSpeed = 30f; // characters per second

        public event Action OnDialogueStart;
        public event Action OnDialogueEnd;

        private Queue<DialogueLine> dialogueLinesQueue = new Queue<DialogueLine>();
        private List<DialogueChoice> currentChoices = new List<DialogueChoice>();
        private bool isTyping = false;
        private string activeText = "";
        private Coroutine typingCoroutine;
        private DialogueData activeDialogueData;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Hide panels initially
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (choicesPanel != null) choicesPanel.SetActive(false);
            if (continueIndicator != null) continueIndicator.SetActive(false);
        }

        private void Update()
        {
            // If dialogue is active and player clicks or presses Space/E, proceed
            if (dialoguePanel != null && dialoguePanel.activeSelf && !choicesPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    if (isTyping)
                    {
                        // Skip typewriter effect and show full line
                        FinishTypingLine();
                    }
                    else
                    {
                        DisplayNextLine();
                    }
                }
            }
        }

        public void StartDialogue(DialogueData dialogue)
        {
            if (dialogue == null || dialogue.lines.Count == 0) return;

            activeDialogueData = dialogue;
            dialogueLinesQueue.Clear();
            currentChoices.Clear();

            foreach (var line in dialogue.lines)
            {
                dialogueLinesQueue.Enqueue(line);
            }

            if (dialogue.choices != null)
            {
                currentChoices.AddRange(dialogue.choices);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.Cutscene);
            }

            if (dialoguePanel != null) dialoguePanel.SetActive(true);
            if (choicesPanel != null) choicesPanel.SetActive(false);

            OnDialogueStart?.Invoke();
            DisplayNextLine();
        }

        private void DisplayNextLine()
        {
            if (dialogueLinesQueue.Count == 0)
            {
                if (currentChoices.Count > 0)
                {
                    ShowChoices();
                }
                else
                {
                    EndDialogue();
                }
                return;
            }

            DialogueLine line = dialogueLinesQueue.Dequeue();
            speakerNameText.text = line.speakerName;
            activeText = line.text;

            if (portraitImage != null)
            {
                if (line.speakerPortrait != null)
                {
                    portraitImage.gameObject.SetActive(true);
                    portraitImage.sprite = line.speakerPortrait;
                }
                else
                {
                    portraitImage.gameObject.SetActive(false);
                }
            }

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeTextRoutine(line.text));
        }

        private IEnumerator TypeTextRoutine(string text)
        {
            dialogueText.text = "";
            isTyping = true;
            if (continueIndicator != null) continueIndicator.SetActive(false);

            float delay = 1f / typingSpeed;
            foreach (char c in text.ToCharArray())
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(delay);
            }

            isTyping = false;
            if (continueIndicator != null) continueIndicator.SetActive(true);
        }

        private void FinishTypingLine()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            dialogueText.text = activeText;
            isTyping = false;
            if (continueIndicator != null) continueIndicator.SetActive(true);
        }

        private void ShowChoices()
        {
            if (choicesPanel == null || choiceButtonPrefab == null)
            {
                EndDialogue();
                return;
            }

            choicesPanel.SetActive(true);
            if (continueIndicator != null) continueIndicator.SetActive(false);

            // Clear old choice buttons
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new choice buttons
            for (int i = 0; i < currentChoices.Count; i++)
            {
                DialogueChoice choice = currentChoices[i];
                GameObject btnObj = Instantiate(choiceButtonPrefab, choicesContainer);
                
                // Set text
                TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = choice.choiceText;

                // Set click action
                Button btn = btnObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnChoiceSelected(choice));
                }
            }
        }

        private void OnChoiceSelected(DialogueChoice choice)
        {
            choicesPanel.SetActive(false);

            if (choice.nextDialogue != null)
            {
                StartDialogue(choice.nextDialogue);
            }
            else
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (choicesPanel != null) choicesPanel.SetActive(false);
            if (continueIndicator != null) continueIndicator.SetActive(false);

            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Cutscene)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }

            OnDialogueEnd?.Invoke();
        }
    }
}
