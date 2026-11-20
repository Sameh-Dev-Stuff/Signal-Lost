using UnityEditor;

namespace PlayModeController
{
    public static class PlayModeSettingsService
    {
        public static bool IsDomainReloadOn => IsReloadOn(EnterPlayModeOptions.DisableDomainReload);
        public static bool IsSceneReloadOn => IsReloadOn(EnterPlayModeOptions.DisableSceneReload);

        public static void SetDomainReload(bool on) => SetReload(EnterPlayModeOptions.DisableDomainReload, on);
        public static void SetSceneReload(bool on) => SetReload(EnterPlayModeOptions.DisableSceneReload, on);

        public static void SetBoth(bool on)
        {
            SetDomainReload(on);
            SetSceneReload(on);
        }

        static bool IsReloadOn(EnterPlayModeOptions disableFlag)
        {
            if (!EditorSettings.enterPlayModeOptionsEnabled)
                return true;

            return (EditorSettings.enterPlayModeOptions & disableFlag) == 0;
        }

        static void SetReload(EnterPlayModeOptions disableFlag, bool on)
        {
            var options = EditorSettings.enterPlayModeOptionsEnabled
                ? EditorSettings.enterPlayModeOptions
                : EnterPlayModeOptions.None;

            if (on)
                options &= ~disableFlag;
            else
                options |= disableFlag;

            EditorSettings.enterPlayModeOptions = options;

            if (!on)
                EditorSettings.enterPlayModeOptionsEnabled = true;
        }
    }
}