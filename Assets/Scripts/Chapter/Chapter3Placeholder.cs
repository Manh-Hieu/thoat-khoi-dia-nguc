using UnityEngine;
using EscapeFromHell.Core;

namespace EscapeFromHell.Chapter
{
    public class Chapter3Placeholder : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetChapter(0);
                    GameManager.Instance.LoadScene("MainMenu");
                }
            }
        }
    }
}
