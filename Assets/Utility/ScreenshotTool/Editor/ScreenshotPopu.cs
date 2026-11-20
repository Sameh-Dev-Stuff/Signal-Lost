using System.IO;
using UnityEditor;
using UnityEngine;

namespace ScreenshotTool
{
    public class ScreenshotPopup : PopupWindowContent
    {
        const float WindowWidth = 300f;
        const float Padding = 10f;
        const float Gap = 6f;
        const float SectionGap = 8f;
        const float TitleHeight = 20f;
        const float RowHeight = 20f;
        const float InfoHeight = 16f;
        const float ButtonHeight = 26f;
        const float LabelWidth = 55f;
        const float SeparatorWidth = 14f;
        const string NameFieldName = "ScreenshotNameField";

        string outputText;
        string fileText;

        GUIStyle labelStyle;
        GUIStyle infoStyle;
        GUIStyle centerStyle;
        GUIStyle hintStyle;

        public ScreenshotPopup()
        {
            RefreshInfo();
        }

        // ---------- Size ----------

        public override Vector2 GetWindowSize()
        {
            // Rows: Size, Name (always) + Custom size (only when "Custom" is selected).
            float height = Padding + TitleHeight + SectionGap
                         + RowHeight + Gap + RowHeight;

            if (ScreenshotSettings.Preset == ResolutionPreset.Custom)
                height += Gap + RowHeight;

            height += SectionGap + InfoHeight * 2f + SectionGap + ButtonHeight + Padding;

            return new Vector2(WindowWidth, height);
        }

        // ---------- GUI ----------

        public override void OnGUI(Rect rect)
        {
            CreateStylesIfNeeded();

            float x = Padding;
            float y = Padding;
            float width = rect.width - Padding * 2f;

            GUI.Label(new Rect(x, y, width, TitleHeight), "Screenshot Settings", EditorStyles.boldLabel);
            y += TitleHeight + SectionGap;

            DrawPresetRow(new Rect(x, y, width, RowHeight));
            y += RowHeight;

            if (ScreenshotSettings.Preset == ResolutionPreset.Custom)
            {
                y += Gap;
                DrawCustomRow(new Rect(x, y, width, RowHeight));
                y += RowHeight;
            }

            y += Gap;
            DrawNameRow(new Rect(x, y, width, RowHeight));
            y += RowHeight + SectionGap;

            GUI.Label(new Rect(x, y, width, InfoHeight), outputText, infoStyle);
            GUI.Label(new Rect(x, y + InfoHeight, width, InfoHeight), fileText, infoStyle);
            y += InfoHeight * 2f + SectionGap;

            DrawButtons(new Rect(x, y, width, ButtonHeight));
        }

        void DrawPresetRow(Rect row)
        {
            SplitRow(row, out Rect labelRect, out Rect fieldRect);
            GUI.Label(labelRect, "Size", labelStyle);

            EditorGUI.BeginChangeCheck();
            int index = EditorGUI.Popup(fieldRect, (int)ScreenshotSettings.Preset, ScreenshotSettings.PresetLabels);
            if (EditorGUI.EndChangeCheck())
            {
                ScreenshotSettings.Preset = (ResolutionPreset)index;
                RefreshInfo();
            }
        }

        void DrawCustomRow(Rect row)
        {
            SplitRow(row, out Rect labelRect, out Rect fieldRect);
            GUI.Label(labelRect, "Custom", labelStyle);

            float fieldWidth = (fieldRect.width - SeparatorWidth) / 2f;
            var widthRect = new Rect(fieldRect.x, fieldRect.y, fieldWidth, fieldRect.height);
            var separatorRect = new Rect(widthRect.xMax, fieldRect.y, SeparatorWidth, fieldRect.height);
            var heightRect = new Rect(separatorRect.xMax, fieldRect.y, fieldWidth, fieldRect.height);

            EditorGUI.BeginChangeCheck();
            int newWidth = EditorGUI.DelayedIntField(widthRect, ScreenshotSettings.CustomWidth);
            GUI.Label(separatorRect, "x", centerStyle);
            int newHeight = EditorGUI.DelayedIntField(heightRect, ScreenshotSettings.CustomHeight);
            if (EditorGUI.EndChangeCheck())
            {
                ScreenshotSettings.CustomWidth = newWidth;
                ScreenshotSettings.CustomHeight = newHeight;
                RefreshInfo();
            }
        }

        void DrawNameRow(Rect row)
        {
            SplitRow(row, out Rect labelRect, out Rect fieldRect);
            GUI.Label(labelRect, "Name", labelStyle);

            GUI.SetNextControlName(NameFieldName);
            EditorGUI.BeginChangeCheck();
            string newName = EditorGUI.TextField(fieldRect, ScreenshotSettings.CustomName);
            if (EditorGUI.EndChangeCheck())
            {
                ScreenshotSettings.CustomName = newName;
                RefreshInfo();
            }

            // Gray hint inside the empty field: it shows what happens if you write nothing.
            bool isFocused = GUI.GetNameOfFocusedControl() == NameFieldName;

            if (newName.Length == 0 && !isFocused)
                GUI.Label(fieldRect, "Project name (default)", hintStyle);
        }

        void DrawButtons(Rect row)
        {
            float spacing = 4f;
            float buttonWidth = (row.width - spacing) / 2f;
            var takeRect = new Rect(row.x, row.y, buttonWidth, row.height);
            var openRect = new Rect(takeRect.xMax + spacing, row.y, buttonWidth, row.height);

            if (GUI.Button(takeRect, "Take Screenshot"))
            {
                // Render after the popup is closed, not in the middle of drawing it.
                EditorApplication.delayCall += ScreenshotCapture.TakeScreenshot;
                editorWindow.Close();
            }

            using (new EditorGUI.DisabledScope(!Directory.Exists(ScreenshotCapture.FolderPath)))
            {
                if (GUI.Button(openRect, "Open Folder"))
                    EditorUtility.RevealInFinder(ScreenshotCapture.FolderPath);
            }
        }

        // ---------- Helpers ----------

        void RefreshInfo()
        {
            Vector2Int size = ScreenshotCapture.GetTargetSize();

            outputText = $"Output: {size.x} x {size.y} px  -  Game View";
            fileText = "Next file: " + ScreenshotCapture.GetNextFileName();
        }

        static void SplitRow(Rect row, out Rect labelRect, out Rect fieldRect)
        {
            labelRect = new Rect(row.x, row.y, LabelWidth, row.height);
            fieldRect = new Rect(row.x + LabelWidth, row.y, row.width - LabelWidth, row.height);
        }

        void CreateStylesIfNeeded()
        {
            if (labelStyle != null)
                return;

            labelStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft };

            infoStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip
            };

            centerStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };

            hintStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(4, 0, 0, 0)
            };
            hintStyle.normal.textColor = new Color(0.55f, 0.55f, 0.55f);
        }
    }
}