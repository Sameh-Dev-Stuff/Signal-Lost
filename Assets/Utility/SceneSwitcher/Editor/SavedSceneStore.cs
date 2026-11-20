using System;
using System.Collections.Generic;
using UnityEditor;

namespace SceneSwitcher
{
    public static class SavedSceneStore
    {
        static readonly string PrefsKey = "SceneSwitcher.SavedScenes." + PlayerSettings.productGUID;

        static List<string> guids;

        static List<string> Guids
        {
            get
            {
                if (guids == null)
                    Load();

                return guids;
            }
        }

        // Scenes that were deleted or moved out of the project are skipped here,
        // but their GUID stays saved, so they come back if the file comes back.
        public static List<string> GetPaths()
        {
            var paths = new List<string>();

            foreach (string guid in Guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (!string.IsNullOrEmpty(path))
                    paths.Add(path);
            }

            return paths;
        }

        public static void Add(string path) => AddMany(new[] { path });

        public static void AddMany(IEnumerable<string> paths)
        {
            bool changed = false;

            foreach (string path in paths)
            {
                string guid = AssetDatabase.AssetPathToGUID(path);

                if (string.IsNullOrEmpty(guid) || Guids.Contains(guid))
                    continue;

                Guids.Add(guid);
                changed = true;
            }

            if (changed)
                Save();
        }

        public static void Remove(string path)
        {
            if (Guids.Remove(AssetDatabase.AssetPathToGUID(path)))
                Save();
        }

        public static void Clear()
        {
            Guids.Clear();
            Save();
        }

        static void Load()
        {
            string text = EditorPrefs.GetString(PrefsKey, "");
            guids = new List<string>(text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        static void Save()
        {
            EditorPrefs.SetString(PrefsKey, string.Join(",", Guids));
        }
    }
}