using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ScreenshotTool
{
    public static class ScreenshotCapture
    {
        const string FolderName = "Screenshots";
        const int MinSize = 16;
        const int MaxSize = 8192;
        const int AntiAliasing = 4;

        public static string FolderPath => Path.Combine(Application.dataPath, FolderName);

        // ---------- Public ----------

        public static void TakeScreenshot()
        {
            List<Camera> cameras = GetGameCameras();

            if (cameras.Count == 0)
            {
                Debug.LogWarning("Screenshot: no active Camera found in the scene.");
                return;
            }

            Vector2Int size = GetTargetSize();
            Texture2D texture = RenderCameras(cameras, size);

            Save(texture, size);
            Object.DestroyImmediate(texture);
        }

        public static Vector2Int GetTargetSize()
        {
            Vector2Int viewSize = GetViewSize();

            switch (ScreenshotSettings.Preset)
            {
                case ResolutionPreset.ViewSize: return ClampSize(viewSize);
                case ResolutionPreset.ViewSizeX2: return ClampSize(viewSize * 2);
                case ResolutionPreset.ViewSizeX4: return ClampSize(viewSize * 4);
                case ResolutionPreset.HD1080: return new Vector2Int(1920, 1080);
                case ResolutionPreset.QHD1440: return new Vector2Int(2560, 1440);
                case ResolutionPreset.UHD4K: return new Vector2Int(3840, 2160);
                default:
                    return ClampSize(new Vector2Int(ScreenshotSettings.CustomWidth, ScreenshotSettings.CustomHeight));
            }
        }

        // First screenshot: "Name.png" (no number).
        // If that file already exists, we add a number so nothing is overwritten.
        public static string GetNextFileName()
        {
            string baseName = GetBaseName();
            string fileName = baseName + ".png";
            int number = 1;

            while (File.Exists(Path.Combine(FolderPath, fileName)))
            {
                fileName = $"{baseName}_{number:0000}.png";
                number++;
            }

            return fileName;
        }

        // ---------- Cameras ----------

        static List<Camera> GetGameCameras()
        {
            var cameras = new List<Camera>();

            foreach (Camera camera in Camera.allCameras)
            {
                if (camera.cameraType == CameraType.Game && camera.targetTexture == null)
                    cameras.Add(camera);
            }

            // Lower depth is drawn first, exactly like the Game View does.
            cameras.Sort((a, b) => a.depth.CompareTo(b.depth));
            return cameras;
        }

        static Texture2D RenderCameras(List<Camera> cameras, Vector2Int size)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                size.x, size.y, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, AntiAliasing);

            RenderTexture previousActive = RenderTexture.active;

            foreach (Camera camera in cameras)
            {
                RenderTexture oldTarget = camera.targetTexture;
                camera.targetTexture = renderTexture;
                camera.Render();
                camera.targetTexture = oldTarget;
            }

            RenderTexture.active = renderTexture;

            var texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);

            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture;
        }

        // ---------- Saving ----------

        static void Save(Texture2D texture, Vector2Int size)
        {
            Directory.CreateDirectory(FolderPath);

            string fileName = GetNextFileName();
            File.WriteAllBytes(Path.Combine(FolderPath, fileName), texture.EncodeToPNG());

            string assetPath = "Assets/" + FolderName + "/" + fileName;
            AssetDatabase.ImportAsset(assetPath);

            // Passing the asset as context: clicking the Console message highlights the file.
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            Debug.Log($"Screenshot saved: {assetPath} ({size.x} x {size.y})", asset);
        }

        // ---------- Helpers ----------

        static Vector2Int GetViewSize()
        {
            Vector2 gameSize = Handles.GetMainGameViewSize();
            var size = new Vector2Int(Mathf.RoundToInt(gameSize.x), Mathf.RoundToInt(gameSize.y));

            if (size.x <= 0 || size.y <= 0)
                return new Vector2Int(1920, 1080);

            return size;
        }

        static Vector2Int ClampSize(Vector2Int size)
        {
            return new Vector2Int(
                Mathf.Clamp(size.x, MinSize, MaxSize),
                Mathf.Clamp(size.y, MinSize, MaxSize));
        }

        // Custom name first. If it is empty, we use the project name.
        static string GetBaseName()
        {
            string name = MakeSafe(ScreenshotSettings.CustomName);

            if (name.Length == 0)
                name = MakeSafe(PlayerSettings.productName);

            return name.Length == 0 ? "Screenshot" : name;
        }

        // Removes characters that Windows and macOS do not allow in file names.
        static string MakeSafe(string text)
        {
            text = text.Trim();

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                text = text.Replace(invalidChar, '_');

            return text.Replace(' ', '_');
        }
    }
}