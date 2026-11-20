using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SceneSwitcher
{
    public class SceneSwitcherPopup : PopupWindowContent
    {
        enum Mode { Saved, Search }

        const float WindowWidth = 320f;
        const float Padding = 8f;
        const float Gap = 4f;
        const float RowGap = 2f;
        const float SectionGap = 8f;
        const float HeaderHeight = 24f;
        const float FieldHeight = 20f;
        const float FooterHeight = 24f;
        const float HintHeight = 16f;
        const float SmallButtonWidth = 28f;
        const float ScrollbarWidth = 14f;
        const float SavedRowHeight = 24f;
        const float SearchRowHeight = 36f;
        const float EmptyHeight = 48f;
        const int MaxSavedRows = 8;
        const int MaxSearchRows = 7;
        const string SearchFieldName = "SceneSearchField";

        static readonly Color CurrentColor = new Color(0.4f, 0.7f, 1f);
        static readonly Color StartColor = new Color(0.35f, 0.8f, 0.4f);

        readonly List<string> allPaths;
        readonly List<string> searchResults = new List<string>();
        List<string> savedPaths;
        HashSet<string> savedSet;

        Mode mode = Mode.Saved;
        string searchText = "";
        bool focusSearchField;
        Vector2 savedScroll;
        Vector2 searchScroll;
        Action pendingAction;

        string currentScenePath;
        string startScenePath;

        GUIStyle titleStyle;
        GUIStyle nameButtonStyle;
        GUIStyle rowLabelStyle;
        GUIStyle pathStyle;
        GUIStyle emptyStyle;
        GUIContent searchContent;
        GUIContent backContent;
        GUIContent playContent;
        GUIContent removeContent;

        public SceneSwitcherPopup()
        {
            allPaths = SceneSwitcherService.GetAllScenePaths();
            ReloadSaved();
        }

        // ---------- Size ----------

        public override Vector2 GetWindowSize()
        {
            float listHeight;
            float footerHeight;

            if (mode == Mode.Saved)
            {
                listHeight = GetListHeight(savedPaths.Count, MaxSavedRows, SavedRowHeight);
                footerHeight = FooterHeight;
            }
            else
            {
                // Based on all scenes (not the filtered count), so the window
                // does not jump up and down while typing.
                listHeight = GetListHeight(allPaths.Count, MaxSearchRows, SearchRowHeight);
                footerHeight = HintHeight;
            }

            float height = Padding + HeaderHeight + SectionGap + listHeight + SectionGap + footerHeight + Padding;
            return new Vector2(WindowWidth, height);
        }

        static float GetListHeight(int count, int maxRows, float rowHeight)
        {
            if (count == 0)
                return EmptyHeight;

            return Mathf.Min(count, maxRows) * (rowHeight + RowGap);
        }

        // ---------- Main GUI ----------

        public override void OnGUI(Rect rect)
        {
            CreateStylesIfNeeded();

            float x = Padding;
            float y = Padding;
            float width = rect.width - Padding * 2f;

            if (mode == Mode.Saved)
                DrawSavedMode(x, y, width);
            else
                DrawSearchMode(x, y, width);

            // Changes to the lists happen here, after drawing,
            // so we never edit a list while we are looping over it.
            if (pendingAction != null)
            {
                Action action = pendingAction;
                pendingAction = null;
                action();

                if (editorWindow != null)
                    editorWindow.Repaint();
            }
        }

        // ---------- Saved mode ----------

        void DrawSavedMode(float x, float y, float width)
        {
            currentScenePath = SceneSwitcherService.GetCurrentScenePath();
            startScenePath = SceneSwitcherService.GetStartScenePath();

            var titleRect = new Rect(x, y, width - SmallButtonWidth - Gap, HeaderHeight);
            var searchRect = new Rect(x + width - SmallButtonWidth, y, SmallButtonWidth, HeaderHeight);

            GUI.Label(titleRect, $"Scenes ({savedPaths.Count})", titleStyle);

            if (GUI.Button(searchRect, searchContent))
                pendingAction = EnterSearchMode;

            y += HeaderHeight + SectionGap;

            float listHeight = GetListHeight(savedPaths.Count, MaxSavedRows, SavedRowHeight);
            var listRect = new Rect(x, y, width, listHeight);

            if (savedPaths.Count == 0)
                GUI.Label(listRect, "No scenes yet.\nPress the search button to add some.", emptyStyle);
            else
                DrawScrollList(listRect, savedPaths, SavedRowHeight, ref savedScroll, DrawSavedRow);

            y += listHeight + SectionGap;

            DrawFooter(new Rect(x, y, width, FooterHeight));
        }

        void DrawSavedRow(Rect rowRect, string path)
        {
            float buttonsWidth = (SmallButtonWidth + Gap) * 2f;
            var nameRect = new Rect(rowRect.x, rowRect.y, rowRect.width - buttonsWidth, rowRect.height);
            var startRect = new Rect(nameRect.xMax + Gap, rowRect.y, SmallButtonWidth, rowRect.height);
            var removeRect = new Rect(startRect.xMax + Gap, rowRect.y, SmallButtonWidth, rowRect.height);

            Color oldColor = GUI.backgroundColor;

            GUI.backgroundColor = path == currentScenePath ? CurrentColor : oldColor;

            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                var label = new GUIContent(Path.GetFileNameWithoutExtension(path), path);

                if (GUI.Button(nameRect, label, nameButtonStyle))
                    pendingAction = () => SwitchToScene(path);
            }

            GUI.backgroundColor = path == startScenePath ? StartColor : oldColor;

            if (GUI.Button(startRect, playContent))
                pendingAction = () => ToggleStartScene(path);

            GUI.backgroundColor = oldColor;

            if (GUI.Button(removeRect, removeContent))
                pendingAction = () => RemoveScene(path);
        }

        void DrawFooter(Rect rect)
        {
            float buttonWidth = (rect.width - Gap) / 2f;
            var addAllRect = new Rect(rect.x, rect.y, buttonWidth, rect.height);
            var removeAllRect = new Rect(addAllRect.xMax + Gap, rect.y, buttonWidth, rect.height);

            if (GUI.Button(addAllRect, "Add All Scenes"))
                pendingAction = AddAllScenes;

            using (new EditorGUI.DisabledScope(savedPaths.Count == 0))
            {
                if (GUI.Button(removeAllRect, "Remove All Scenes"))
                    pendingAction = RemoveAllScenes;
            }
        }

        // ---------- Search mode ----------

        void DrawSearchMode(float x, float y, float width)
        {
            var backRect = new Rect(x, y, SmallButtonWidth, HeaderHeight);
            var fieldRect = new Rect(backRect.xMax + Gap, y + (HeaderHeight - FieldHeight) / 2f,
                                     width - SmallButtonWidth - Gap, FieldHeight);

            if (GUI.Button(backRect, backContent))
                pendingAction = ExitSearchMode;

            GUI.SetNextControlName(SearchFieldName);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUI.TextField(fieldRect, searchText);
            if (EditorGUI.EndChangeCheck())
            {
                searchText = newText;
                searchScroll = Vector2.zero;
                RebuildResults();
            }

            if (focusSearchField && Event.current.type == EventType.Repaint)
            {
                EditorGUI.FocusTextInControl(SearchFieldName);
                focusSearchField = false;
            }

            y += HeaderHeight + SectionGap;

            float listHeight = GetListHeight(allPaths.Count, MaxSearchRows, SearchRowHeight);
            var listRect = new Rect(x, y, width, listHeight);

            if (searchResults.Count == 0)
                GUI.Label(listRect, GetEmptySearchMessage(), emptyStyle);
            else
                DrawScrollList(listRect, searchResults, SearchRowHeight, ref searchScroll, DrawSearchRow);

            y += listHeight + SectionGap;

            GUI.Label(new Rect(x, y, width, HintHeight),
                      $"{searchResults.Count} found  -  click a scene to add it",
                      EditorStyles.miniLabel);
        }

        void DrawSearchRow(Rect rowRect, string path)
        {
            // The button is the background. The two labels are drawn on top of it.
            if (GUI.Button(rowRect, new GUIContent("", path)))
                pendingAction = () => AddScene(path);

            var nameRect = new Rect(rowRect.x + 8f, rowRect.y + 3f, rowRect.width - 16f, 16f);
            var pathRect = new Rect(rowRect.x + 8f, rowRect.y + 19f, rowRect.width - 16f, 14f);

            GUI.Label(nameRect, Path.GetFileNameWithoutExtension(path), rowLabelStyle);
            GUI.Label(pathRect, Path.GetDirectoryName(path).Replace('\\', '/'), pathStyle);
        }

        string GetEmptySearchMessage()
        {
            if (allPaths.Count == 0)
                return "No scenes found in the Assets folder.";

            if (string.IsNullOrEmpty(searchText))
                return "All scenes are already in your list.";

            return "No scene matches this search.";
        }

        // ---------- Shared scroll list ----------

        void DrawScrollList(Rect viewRect, List<string> paths, float rowHeight, ref Vector2 scroll, Action<Rect, string> drawRow)
        {
            float step = rowHeight + RowGap;
            float contentHeight = paths.Count * step;
            bool needsScrollbar = contentHeight > viewRect.height;
            float rowWidth = viewRect.width - (needsScrollbar ? ScrollbarWidth : 0f);

            // If the list got shorter, do not stay scrolled past its end.
            scroll.y = Mathf.Clamp(scroll.y, 0f, Mathf.Max(0f, contentHeight - viewRect.height));

            scroll = GUI.BeginScrollView(viewRect, scroll, new Rect(0, 0, rowWidth, contentHeight));

            // Only draw the rows that are visible. This keeps hundreds of scenes fast.
            int firstRow = Mathf.Max(0, Mathf.FloorToInt(scroll.y / step));
            int lastRow = Mathf.Min(paths.Count - 1, Mathf.CeilToInt((scroll.y + viewRect.height) / step));

            for (int i = firstRow; i <= lastRow; i++)
                drawRow(new Rect(0, i * step, rowWidth, rowHeight), paths[i]);

            GUI.EndScrollView();
        }

        // ---------- Actions ----------

        void EnterSearchMode()
        {
            mode = Mode.Search;
            searchText = "";
            searchScroll = Vector2.zero;
            focusSearchField = true;
            RebuildResults();
        }

        void ExitSearchMode()
        {
            mode = Mode.Saved;
            GUI.FocusControl(null);
        }

        void AddScene(string path)
        {
            SavedSceneStore.Add(path);
            ReloadSaved();
            ExitSearchMode();
        }

        void RemoveScene(string path)
        {
            SavedSceneStore.Remove(path);
            ReloadSaved();
        }

        void AddAllScenes()
        {
            SavedSceneStore.AddMany(allPaths);
            ReloadSaved();
        }

        void RemoveAllScenes()
        {
            SavedSceneStore.Clear();
            ReloadSaved();
        }

        void ToggleStartScene(string path)
        {
            SceneSwitcherService.ToggleStartScene(path);
            SceneSwitcherToolbar.Refresh();
        }

        void SwitchToScene(string path)
        {
            if (path == SceneSwitcherService.GetCurrentScenePath())
                return;

            // Open the scene after the popup is closed, so the "save changes?"
            // dialog does not appear in the middle of drawing the popup.
            EditorApplication.delayCall += () => SceneSwitcherService.OpenScene(path);
            editorWindow.Close();
        }

        // ---------- Data ----------

        void ReloadSaved()
        {
            savedPaths = SavedSceneStore.GetPaths();
            savedSet = new HashSet<string>(savedPaths);
            RebuildResults();
        }

        void RebuildResults()
        {
            searchResults.Clear();

            foreach (string path in allPaths)
            {
                if (savedSet.Contains(path))
                    continue;

                string sceneName = Path.GetFileNameWithoutExtension(path);

                if (searchText.Length > 0 && sceneName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                searchResults.Add(path);
            }
        }

        // ---------- Styles and icons ----------

        void CreateStylesIfNeeded()
        {
            if (nameButtonStyle != null)
                return;

            titleStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft };

            nameButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                padding = new RectOffset(8, 4, 2, 2)
            };

            rowLabelStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip
            };

            pathStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip
            };
            pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);

            emptyStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = true };

            searchContent = MakeIcon("Search Icon", "Find", "Search all scenes");
            backContent = new GUIContent("<", "Back to my scenes");
            playContent = MakeIcon("PlayButton", ">", "Play from this scene");
            removeContent = MakeIcon("Toolbar Minus", "-", "Remove from list");
        }

        static GUIContent MakeIcon(string iconName, string fallbackText, string tooltip)
        {
            Texture image = EditorGUIUtility.IconContent(iconName).image;

            return image != null
                ? new GUIContent(image, tooltip)
                : new GUIContent(fallbackText, tooltip);
        }
    }
}