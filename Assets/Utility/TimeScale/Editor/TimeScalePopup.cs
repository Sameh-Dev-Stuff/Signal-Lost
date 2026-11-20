using UnityEditor;
using UnityEngine;

namespace TimeScaleController
{
    public class TimeScalePopup : PopupWindowContent
    {
        const float SliderMax = 10f;
        static readonly float[] Presets = { 0f, 0.25f, 0.5f, 1f, 2f, 5f };

        const float WindowWidth = 280f;
        const float Padding = 10f;
        const float Gap = 6f;
        const float SectionGap = 8f;
        const float BigValueHeight = 30f;
        const float StateHeight = 16f;
        const float SliderHeight = 18f;
        const float FieldHeight = 20f;
        const float ButtonHeight = 24f;
        const float FieldLabelWidth = 45f;

        const float WindowHeight =
            Padding + BigValueHeight + StateHeight + SectionGap +
            SliderHeight + Gap + FieldHeight + SectionGap +
            ButtonHeight + Gap + ButtonHeight + Padding;

        GUIStyle bigValueStyle;

        public override Vector2 GetWindowSize() => new Vector2(WindowWidth, WindowHeight);

        public override void OnOpen()
        {
            bigValueStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 22,
                alignment = TextAnchor.MiddleCenter
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
            float current = TimeScaleService.Current;
            float x = Padding;
            float y = Padding;
            float width = rect.width - Padding * 2f;

            DrawHeader(new Rect(x, y, width, BigValueHeight),
                       new Rect(x, y + BigValueHeight, width, StateHeight),
                       current);
            y += BigValueHeight + StateHeight + SectionGap;

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                DrawSlider(new Rect(x, y, width, SliderHeight), current);
                y += SliderHeight + Gap;

                DrawValueField(new Rect(x, y, width, FieldHeight), current);
                y += FieldHeight + SectionGap;

                DrawPresets(new Rect(x, y, width, ButtonHeight), current);
                y += ButtonHeight + Gap;

                DrawActions(new Rect(x, y, width, ButtonHeight));
            }
        }

        void DrawHeader(Rect valueRect, Rect stateRect, float current)
        {
            var oldColor = GUI.contentColor;
            GUI.contentColor = Application.isPlaying ? GetStateColor() : Color.gray;

            GUI.Label(valueRect, $"{current:0.##}x", bigValueStyle);

            string stateText = Application.isPlaying
                ? TimeScaleService.GetStateLabel()
                : "Available only in Play Mode";
            GUI.Label(stateRect, stateText, EditorStyles.centeredGreyMiniLabel);

            GUI.contentColor = oldColor;
        }

        static void DrawSlider(Rect rect, float current)
        {
            EditorGUI.BeginChangeCheck();
            float sliderValue = GUI.HorizontalSlider(rect, current, 0f, SliderMax);
            if (EditorGUI.EndChangeCheck())
                TimeScaleService.Set(sliderValue);
        }

        static void DrawValueField(Rect rect, float current)
        {
            var labelRect = new Rect(rect.x, rect.y, FieldLabelWidth, rect.height);
            var fieldRect = new Rect(rect.x + FieldLabelWidth, rect.y, rect.width - FieldLabelWidth, rect.height);

            EditorGUI.LabelField(labelRect, "Value");

            EditorGUI.BeginChangeCheck();
            float typedValue = EditorGUI.DelayedFloatField(fieldRect, current);
            if (EditorGUI.EndChangeCheck())
                TimeScaleService.Set(typedValue);
        }

        static void DrawPresets(Rect rect, float current)
        {
            float spacing = 4f;
            float buttonWidth = (rect.width - spacing * (Presets.Length - 1)) / Presets.Length;

            for (int i = 0; i < Presets.Length; i++)
            {
                var buttonRect = new Rect(rect.x + i * (buttonWidth + spacing), rect.y, buttonWidth, rect.height);
                var oldColor = GUI.backgroundColor;

                if (Mathf.Approximately(current, Presets[i]))
                    GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);

                if (GUI.Button(buttonRect, Presets[i].ToString("0.##")))
                    TimeScaleService.Set(Presets[i]);

                GUI.backgroundColor = oldColor;
            }
        }

        static void DrawActions(Rect rect)
        {
            float spacing = 4f;
            float buttonWidth = (rect.width - spacing) / 2f;

            var pauseRect = new Rect(rect.x, rect.y, buttonWidth, rect.height);
            var resetRect = new Rect(rect.x + buttonWidth + spacing, rect.y, buttonWidth, rect.height);

            string pauseLabel = TimeScaleService.IsPaused ? "Resume" : "Pause";

            if (GUI.Button(pauseRect, pauseLabel))
            {
                if (TimeScaleService.IsPaused) TimeScaleService.Resume();
                else TimeScaleService.Pause();
            }

            if (GUI.Button(resetRect, "Reset to 1"))
                TimeScaleService.ResetToNormal();
        }

        static Color GetStateColor()
        {
            switch (TimeScaleService.GetState())
            {
                case TimeScaleState.Paused: return new Color(0.95f, 0.4f, 0.4f);
                case TimeScaleState.SlowMotion: return new Color(0.4f, 0.7f, 1f);
                case TimeScaleState.FastForward: return new Color(1f, 0.7f, 0.3f);
                default: return new Color(0.35f, 0.8f, 0.4f);
            }
        }
    }
}