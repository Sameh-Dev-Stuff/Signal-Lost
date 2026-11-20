using System;
using UnityEditor;
using UnityEngine;

namespace PlayModeController
{
    public class PlayModeControllerPopup : PopupWindowContent
    {
        static readonly Color OnColor = new Color(0.35f, 0.8f, 0.4f);
        static readonly Color OffColor = new Color(0.6f, 0.6f, 0.6f);

        const float WindowWidth = 260f;
        const float Padding = 10f;
        const float Gap = 4f;
        const float SectionGap = 8f;
        const float TitleHeight = 20f;
        const float RowHeight = 22f;
        const float ButtonHeight = 24f;
        const float StatusWidth = 40f;

        const float WindowHeight =
            Padding + TitleHeight + SectionGap +
            RowHeight + Gap + RowHeight + SectionGap +
            ButtonHeight + Padding;

        GUIStyle statusStyle;

        public override Vector2 GetWindowSize() => new Vector2(WindowWidth, WindowHeight);

        public override void OnOpen()
        {
            statusStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleRight
            };
            EditorApplication.update += RepaintWindow;
        }

        public override void OnClose() => EditorApplication.update -= RepaintWindow;

        void RepaintWindow()
        {
            if (editorWindow != null)
                editorWindow.Repaint();
        }

        public override void OnGUI(Rect rect)
        {
            float x = Padding;
            float y = Padding;
            float width = rect.width - Padding * 2f;

            GUI.Label(new Rect(x, y, width, TitleHeight), "Enter Play Mode Options", EditorStyles.boldLabel);
            y += TitleHeight + SectionGap;

            DrawToggle(new Rect(x, y, width, RowHeight), "Reload Domain",
                       PlayModeSettingsService.IsDomainReloadOn, PlayModeSettingsService.SetDomainReload);
            y += RowHeight + Gap;

            DrawToggle(new Rect(x, y, width, RowHeight), "Reload Scene",
                       PlayModeSettingsService.IsSceneReloadOn, PlayModeSettingsService.SetSceneReload);
            y += RowHeight + SectionGap;

            DrawButtons(new Rect(x, y, width, ButtonHeight));
        }

        void DrawToggle(Rect rect, string label, bool current, Action<bool> setter)
        {
            var toggleRect = new Rect(rect.x, rect.y, rect.width - StatusWidth, rect.height);
            var statusRect = new Rect(rect.xMax - StatusWidth, rect.y, StatusWidth, rect.height);

            EditorGUI.BeginChangeCheck();
            bool value = EditorGUI.ToggleLeft(toggleRect, label, current);
            if (EditorGUI.EndChangeCheck())
                setter(value);

            statusStyle.normal.textColor = current ? OnColor : OffColor;
            GUI.Label(statusRect, current ? "ON" : "OFF", statusStyle);
        }

        static void DrawButtons(Rect rect)
        {
            float spacing = 4f;
            float buttonWidth = (rect.width - spacing) / 2f;

            var onRect = new Rect(rect.x, rect.y, buttonWidth, rect.height);
            var offRect = new Rect(rect.x + buttonWidth + spacing, rect.y, buttonWidth, rect.height);

            if (GUI.Button(onRect, "Both ON"))
                PlayModeSettingsService.SetBoth(true);

            if (GUI.Button(offRect, "Both OFF"))
                PlayModeSettingsService.SetBoth(false);
        }
    }
}