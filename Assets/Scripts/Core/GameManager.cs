using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EscapeFromHell.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Cutscene,
        GameOver,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        [SerializeField] private int currentChapter = 1;
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private int currentHealth = 3;

        [Header("Luck Buff State")]
        public bool hasTransferredScamLuck = false;
        public int scamLuckWinsRemaining = 0;

        [Header("Scene Names")]
        public string mainMenuScene = "MainMenu";
        public string chapter1Scene = "Chapter1_Room";
        public string chapter2Scene = "Chapter2_Cutscene";
        public string chapter3Scene = "Chapter3_Compound";
        public string chapter4Scene = "Chapter4_Night";
        public string chapter5Scene = "Chapter5_Escape";
        public string epilogueScene = "Epilogue";

        public event Action<GameState> OnStateChanged;
        public event Action<int> OnHealthChanged;
        public event Action<int> OnChapterChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentHealth = maxHealth;
        }

        public GameState CurrentState
        {
            get => currentState;
            private set
            {
                if (currentState != value)
                {
                    currentState = value;
                    OnStateChanged?.Invoke(currentState);
                }
            }
        }

        public int CurrentChapter
        {
            get => currentChapter;
            private set
            {
                currentChapter = value;
                OnChapterChanged?.Invoke(currentChapter);
            }
        }

        public int CurrentHealth
        {
            get => currentHealth;
            private set
            {
                currentHealth = Mathf.Clamp(value, 0, maxHealth);
                OnHealthChanged?.Invoke(currentHealth);
                if (currentHealth <= 0)
                {
                    TriggerGameOver();
                }
            }
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
        }

        public void UpdateHealth(int amount)
        {
            CurrentHealth += amount;
        }

        public void SetChapter(int chapter)
        {
            CurrentChapter = chapter;
        }

        public void ResetGame()
        {
            currentHealth = maxHealth;
            currentChapter = 1;
            ChangeState(GameState.Playing);
        }

        public void TriggerGameOver()
        {
            ChangeState(GameState.GameOver);
            // Load Game Over screen or handle UI
            Debug.Log("Game Over!");
        }

        public void TriggerVictory()
        {
            ChangeState(GameState.Victory);
            Debug.Log("Victory! Escaped from Hell!");
            LoadScene(epilogueScene);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            // Pause game state or change to cutscene during loading
            GameState previousState = CurrentState;
            ChangeState(GameState.Cutscene);

            // Find transition UI if exists
            SceneTransition transition = FindAnyObjectByType<SceneTransition>();
            if (transition != null)
            {
                yield return transition.FadeOut();
            }
            else
            {
                yield return new WaitForSeconds(0.5f); // Simple delay if no transition UI
            }

            // Load the scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Let scene initialize
            yield return new WaitForEndOfFrame();

            // Try to fade back in
            transition = FindAnyObjectByType<SceneTransition>();
            if (transition != null)
            {
                yield return transition.FadeIn();
            }

            // Restore state based on loaded scene name
            if (sceneName == mainMenuScene)
            {
                ChangeState(GameState.MainMenu);
            }
            else
            {
                ChangeState(GameState.Playing);
            }
        }
    }
}
