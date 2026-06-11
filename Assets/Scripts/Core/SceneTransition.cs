using UnityEngine;
using System.Collections;

namespace EscapeFromHell.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneTransition : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            // Ensure it starts full black if we just loaded a scene
            canvasGroup.alpha = 1f;
        }

        private void Start()
        {
            // Auto fade-in when scene starts
            StartCoroutine(FadeInRoutine());
        }

        public Coroutine FadeIn()
        {
            return StartCoroutine(FadeInRoutine());
        }

        public Coroutine FadeOut()
        {
            return StartCoroutine(FadeOutRoutine());
        }

        private IEnumerator FadeInRoutine()
        {
            gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private IEnumerator FadeOutRoutine()
        {
            gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }
    }
}
