using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using EscapeFromHell.Player;
using EscapeFromHell.Core;

namespace EscapeFromHell.Systems
{
    public class NewspaperUI : MonoBehaviour
    {
        public static NewspaperUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject newspaperPanel;

        [Header("Pages")]
        [SerializeField] private GameObject[] pages; // pages[0] = trang 1, pages[3] = trang 4 bị xé

        [Header("Navigation")]
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI pageIndicator; // "Trang 1/4"

        [Header("Page 4 - Torn")]
        [SerializeField] private Button casinoLinkButton; // link clickable
        [SerializeField] private TextMeshProUGUI casinoLinkText;

        private int currentPage = 0; // 0-indexed
        private const int TOTAL_PAGES = 4;
        private bool isOpen = false;
        private bool hasPlayedTornDialogue = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (newspaperPanel != null) newspaperPanel.SetActive(false);
        }

        private void Start()
        {
            if (prevButton != null) prevButton.onClick.AddListener(PrevPage);
            if (nextButton != null) nextButton.onClick.AddListener(NextPage);
            if (closeButton != null) closeButton.onClick.AddListener(CloseNewspaper);
            if (casinoLinkButton != null) casinoLinkButton.onClick.AddListener(OpenCasinoLink);
        }

        private void Update()
        {
            if (isOpen && Input.GetKeyDown(KeyCode.Escape))
                CloseNewspaper();
        }

        public void OpenNewspaper()
        {
            if (newspaperPanel == null) return;
            isOpen = true;
            currentPage = 0;
            newspaperPanel.SetActive(true);
            ShowPage(currentPage);

            // Disable player control
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var pc = player.GetComponent<PlayerController>();
                if (pc != null) pc.SetControlEnabled(false);
            }
        }

        public void CloseNewspaper()
        {
            if (newspaperPanel == null) return;
            isOpen = false;
            newspaperPanel.SetActive(false);

            // Re-enable player control
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var pc = player.GetComponent<PlayerController>();
                if (pc != null) pc.SetControlEnabled(true);
            }
        }

        private void ShowPage(int index)
        {
            for (int i = 0; i < pages.Length; i++)
                if (pages[i] != null) pages[i].SetActive(i == index);

            if (pageIndicator != null)
                pageIndicator.text = $"Trang {index + 1}/{TOTAL_PAGES}";

            if (prevButton != null) prevButton.interactable = index > 0;
            if (nextButton != null) nextButton.interactable = index < TOTAL_PAGES - 1;

            // Trigger self-dialogue explaining the torn page ONLY when flipping to Page 4 (index == 3)
            if (index == 3 && !hasPlayedTornDialogue)
            {
                hasPlayedTornDialogue = true;
                if (DialogueSystem.Instance != null)
                {
                    DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
                    dialogue.lines = new System.Collections.Generic.List<DialogueLine> {
                        new DialogueLine {
                            speakerName = "Minh",
                            text = "Ơ, trang này bị mình xé mất một phần làm giấy vệ sinh rồi... Cũng tại không có tiền mua giấy."
                        }
                    };
                    DialogueSystem.Instance.StartDialogue(dialogue);
                }
            }
        }

        private void NextPage()
        {
            if (currentPage < TOTAL_PAGES - 1)
            {
                currentPage++;
                ShowPage(currentPage);
            }
        }

        private void PrevPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                ShowPage(currentPage);
            }
        }

        private void OpenCasinoLink()
        {
            // Mở link sòng bạc Hoàng Gia trong trình duyệt hệ thống
            Application.OpenURL("https://hoanggiacacino.com.vn");
        }
    }
}
