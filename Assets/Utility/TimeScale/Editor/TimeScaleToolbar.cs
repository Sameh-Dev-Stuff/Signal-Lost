using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace TimeScaleController
{
    [InitializeOnLoad]
    public static class TimeScaleToolbar
    {
        const string ElementPath = "Time Scale Controller";
        const string OriginalScaleKey = "TimeScaleController.OriginalScale";

        static float lastShownScale = -1f;

        static TimeScaleToolbar()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            if (EditorApplication.isPlaying)
                StartWatching();
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    SessionState.SetFloat(OriginalScaleKey, Time.timeScale);
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    TimeScaleService.ResetMemory();
                    StartWatching();
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    EditorApplication.update -= CheckForChanges;
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    RestoreOriginalScale();
                    MainToolbar.Refresh(ElementPath);
                    break;
            }
        }

        static void StartWatching()
        {
            EditorApplication.update -= CheckForChanges;
            EditorApplication.update += CheckForChanges;
            lastShownScale = -1f;
        }

        static void CheckForChanges()
        {
            float scale = Time.timeScale;

            if (Mathf.Approximately(scale, lastShownScale))
                return;

            lastShownScale = scale;
            MainToolbar.Refresh(ElementPath);
        }

        static void RestoreOriginalScale()
        {
            float original = SessionState.GetFloat(OriginalScaleKey, 1f);

            if (!Mathf.Approximately(Time.timeScale, original))
                Time.timeScale = original;
        }

        [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateElement()
        {
            string text = Application.isPlaying
                ? $"{TimeScaleService.Current:0.##}x  {TimeScaleService.GetStateLabel()}"
                : "Time Scale";

            var icon = EditorGUIUtility.IconContent("d_UnityEditor.AnimationWindow").image as Texture2D;
            var content = new MainToolbarContent(text, icon, "Time.timeScale (Play Mode only)");

            return new MainToolbarDropdown(content, ShowPopup);
        }

        static void ShowPopup(Rect buttonRect)
        {
            PopupWindow.Show(buttonRect, new TimeScalePopup());
        }
    }
}