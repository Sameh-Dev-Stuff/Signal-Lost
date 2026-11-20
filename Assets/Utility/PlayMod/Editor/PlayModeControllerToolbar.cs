using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace PlayModeController
{
    [InitializeOnLoad]
    public static class PlayModeControllerToolbar
    {
        const string ElementPath = "Play Mode Controller";

        static bool lastDomain = PlayModeSettingsService.IsDomainReloadOn;
        static bool lastScene = PlayModeSettingsService.IsSceneReloadOn;

        static PlayModeControllerToolbar()
        {
            EditorApplication.update += CheckForChanges;
        }

        static void CheckForChanges()
        {
            bool domain = PlayModeSettingsService.IsDomainReloadOn;
            bool scene = PlayModeSettingsService.IsSceneReloadOn;

            if (domain == lastDomain && scene == lastScene)
                return;

            lastDomain = domain;
            lastScene = scene;
            MainToolbar.Refresh(ElementPath);
        }

        [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateElement()
        {
            string domain = PlayModeSettingsService.IsDomainReloadOn ? "ON" : "OFF";
            string scene = PlayModeSettingsService.IsSceneReloadOn ? "ON" : "OFF";

            var icon = EditorGUIUtility.IconContent("d_Settings").image as Texture2D;
            var content = new MainToolbarContent(
                $"Domain {domain} | Scene {scene}",
                icon,
                "Enter Play Mode Options");

            return new MainToolbarDropdown(content, ShowPopup);
        }

        static void ShowPopup(Rect buttonRect)
        {
            PopupWindow.Show(buttonRect, new PlayModeControllerPopup());
        }
    }
}