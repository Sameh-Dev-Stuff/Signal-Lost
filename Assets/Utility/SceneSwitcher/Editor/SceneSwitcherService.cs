using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SceneSwitcher
{
    public static class SceneSwitcherService
    {
        public static string GetCurrentSceneName()
        {
            Scene scene = SceneManager.GetActiveScene();
            return string.IsNullOrEmpty(scene.name) ? "Untitled" : scene.name;
        }

        public static string GetCurrentScenePath()
        {
            return SceneManager.GetActiveScene().path;
        }

        public static List<string> GetAllScenePaths()
        {
            var paths = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

            foreach (string guid in guids)
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));

            paths.Sort();
            return paths;
        }

        public static void OpenScene(string path)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }

        public static string GetStartScenePath()
        {
            SceneAsset startScene = EditorSceneManager.playModeStartScene;
            return startScene == null ? "" : AssetDatabase.GetAssetPath(startScene);
        }

        public static void ToggleStartScene(string path)
        {
            if (GetStartScenePath() == path)
                EditorSceneManager.playModeStartScene = null;
            else
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        }
    }
}