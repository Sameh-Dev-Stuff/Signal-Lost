using UnityEditor;

namespace ScreenshotTool
{
    public enum ResolutionPreset { ViewSize, ViewSizeX2, ViewSizeX4, HD1080, QHD1440, UHD4K, Custom }

    public static class ScreenshotSettings
    {
        const string Prefix = "ScreenshotTool.";

        // The order of these labels must match the order of the enum above.
        public static readonly string[] PresetLabels =
        {
            "Game View size (x1)", "Game View size (x2)", "Game View size (x4)",
            "1080p (1920 x 1080)", "1440p (2560 x 1440)", "4K (3840 x 2160)", "Custom"
        };

        public static ResolutionPreset Preset
        {
            get => (ResolutionPreset)EditorPrefs.GetInt(Prefix + "Preset", (int)ResolutionPreset.ViewSize);
            set => EditorPrefs.SetInt(Prefix + "Preset", (int)value);
        }

        public static int CustomWidth
        {
            get => EditorPrefs.GetInt(Prefix + "CustomWidth", 1920);
            set => EditorPrefs.SetInt(Prefix + "CustomWidth", value);
        }

        public static int CustomHeight
        {
            get => EditorPrefs.GetInt(Prefix + "CustomHeight", 1080);
            set => EditorPrefs.SetInt(Prefix + "CustomHeight", value);
        }

        // Empty means: use the project name.
        public static string CustomName
        {
            get => EditorPrefs.GetString(Prefix + "CustomName", "");
            set => EditorPrefs.SetString(Prefix + "CustomName", value);
        }
    }
}