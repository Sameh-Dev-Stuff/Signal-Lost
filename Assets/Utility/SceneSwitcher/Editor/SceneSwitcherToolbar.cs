using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace SceneSwitcher
{
    [InitializeOnLoad]
    public static class SceneSwitcherToolbar
    {
        const string ElementPath = "Scene Switcher";
        const int MaxNameLength = 20;

        static SceneSwitcherToolbar()
        {
            EditorSceneManager.sceneOpened += (scene, mode) => Refresh();
            EditorSceneManager.sceneSaved += scene => Refresh();
            EditorSceneManager.newSceneCreated += (scene, setup, mode) => Refresh();
            EditorSceneManager.activeSceneChangedInEditMode += (oldScene, newScene) => Refresh();
        }

        public static void Refresh()
        {
            MainToolbar.Refresh(ElementPath);
        }

        [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateElement()
        {
            string text = Truncate(SceneSwitcherService.GetCurrentSceneName(), MaxNameLength);
            string startPath = SceneSwitcherService.GetStartScenePath();

            if (!string.IsNullOrEmpty(startPath))
                text += "  |  Play from: " + Truncate(Path.GetFileNameWithoutExtension(startPath), MaxNameLength);

            string tooltip = "Switch scene\n" + SceneSwitcherService.GetCurrentScenePath();

            var icon = EditorGUIUtility.IconContent("SceneAsset Icon").image as Texture2D;
            var content = new MainToolbarContent(text, icon, tooltip);

            return new MainToolbarDropdown(content, ShowPopup);
        }

        static string Truncate(string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }

        static void ShowPopup(Rect buttonRect)
        {
            PopupWindow.Show(buttonRect, new SceneSwitcherPopup());
        }
    }
}