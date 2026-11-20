using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace ScreenshotTool
{
    public static class ScreenshotToolbar
    {
        const string ButtonPath = "Screenshot Button";
        const string SettingsPath = "Screenshot Settings";

        [MainToolbarElement(ButtonPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateButton()
        {
            var icon = EditorGUIUtility.IconContent("Camera Icon").image as Texture2D;
            var content = new MainToolbarContent("Screenshot", icon, "Take a screenshot with the current settings");

            return new MainToolbarButton(content, ScreenshotCapture.TakeScreenshot);
        }

        [MainToolbarElement(SettingsPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateSettings()
        {
            var icon = EditorGUIUtility.IconContent("d_Settings").image as Texture2D;
            var content = new MainToolbarContent("", icon, "Screenshot settings");

            return new MainToolbarDropdown(content, ShowPopup);
        }

        static void ShowPopup(Rect buttonRect)
        {
            PopupWindow.Show(buttonRect, new ScreenshotPopup());
        }
    }
}