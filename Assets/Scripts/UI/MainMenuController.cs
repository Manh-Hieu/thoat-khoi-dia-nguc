using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using EscapeFromHell.Core;

namespace EscapeFromHell.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject confirmNewGamePanel;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;

        [Header("Confirm New Game")]
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;

        [Header("Scene Names")]
        [SerializeField] private string chapter1SceneName = "Chapter1_Room";

        [Header("Settings Panel")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button settingsCloseButton;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Animations")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;

        private bool hasSaveData = false;

        private void Start()
        {
            // Check for save data (simple PlayerPrefs check)
            hasSaveData = PlayerPrefs.GetInt("HasSaveData", 0) == 1;

            // Update Continue button interactability
            if (continueButton != null)
            {
                continueButton.interactable = hasSaveData;
                var tmp = continueButton.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null && !hasSaveData)
                {
                    tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0.4f);
                }
            }

            // Wire up buttons
            if (newGameButton != null)
                newGameButton.onClick.AddListener(OnNewGameClicked);
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClicked);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);

            if (confirmYesButton != null)
                confirmYesButton.onClick.AddListener(OnConfirmNewGame);
            if (confirmNoButton != null)
                confirmNoButton.onClick.AddListener(OnCancelNewGame);

            if (settingsCloseButton != null)
                settingsCloseButton.onClick.AddListener(OnCloseSettings);

            // Start hidden panels
            if (confirmNewGamePanel != null)
                confirmNewGamePanel.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            if (mainPanel != null)
                mainPanel.SetActive(true);

            // Fade in
            if (fadeCanvasGroup != null)
                StartCoroutine(FadeIn());
        }

        private void OnNewGameClicked()
        {
            if (hasSaveData)
            {
                // Show confirm dialog if there's existing save
                if (confirmNewGamePanel != null)
                    confirmNewGamePanel.SetActive(true);
            }
            else
            {
                StartNewGame();
            }
        }

        private void OnContinueClicked()
        {
            if (!hasSaveData) return;
            StartCoroutine(LoadGameScene(chapter1SceneName));
        }

        private void OnSettingsClicked()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        private void OnCloseSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void OnConfirmNewGame()
        {
            if (confirmNewGamePanel != null)
                confirmNewGamePanel.SetActive(false);
            StartNewGame();
        }

        private void OnCancelNewGame()
        {
            if (confirmNewGamePanel != null)
                confirmNewGamePanel.SetActive(false);
        }

        private void StartNewGame()
        {
            // Clear save data
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            StartCoroutine(LoadGameScene(chapter1SceneName));
        }

        private IEnumerator LoadGameScene(string sceneName)
        {
            // Fade out
            if (fadeCanvasGroup != null)
            {
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * 2f;
                    fadeCanvasGroup.alpha = t;
                    yield return null;
                }
                fadeCanvasGroup.alpha = 1f;
            }

            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator FadeIn()
        {
            fadeCanvasGroup.alpha = 1f;
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime * 1.5f;
                fadeCanvasGroup.alpha = t;
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
        }
    }
}
