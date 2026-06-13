#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EscapeFromHell.Editor
{
    [InitializeOnLoad]
    public static class EditorPlayModeSetup
    {
        private const string MENU_KEY = "Escape From Hell/Always Start From Main Menu";
        private static bool isEnabled;

        static EditorPlayModeSetup()
        {
            // Default to true so it works out of the box
            isEnabled = EditorPrefs.GetBool(MENU_KEY, true);
            EditorApplication.delayCall += UpdatePlayModeStartScene;
        }

        [MenuItem(MENU_KEY)]
        private static void ToggleAction()
        {
            isEnabled = !isEnabled;
            EditorPrefs.SetBool(MENU_KEY, isEnabled);
            Menu.SetChecked(MENU_KEY, isEnabled);
            UpdatePlayModeStartScene();
        }

        [MenuItem(MENU_KEY, true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(MENU_KEY, isEnabled);
            return true;
        }

        private static void UpdatePlayModeStartScene()
        {
            if (isEnabled)
            {
                SceneAsset mainmenu = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/MainMenu.unity");
                if (mainmenu != null)
                {
                    EditorSceneManager.playModeStartScene = mainmenu;
                }
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }
    }
}
#endif
