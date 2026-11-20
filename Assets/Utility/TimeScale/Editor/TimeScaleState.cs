using UnityEngine;

namespace TimeScaleController
{
    public enum TimeScaleState { Paused, SlowMotion, NormalSpeed, FastForward }

    public static class TimeScaleService
    {
        public const float MinScale = 0f;
        public const float MaxScale = 100f;

        static float lastNonZeroScale = 1f;

        public static float Current => Time.timeScale;
        public static bool IsPaused => Mathf.Approximately(Time.timeScale, 0f);

        public static TimeScaleState GetState()
        {
            float scale = Time.timeScale;

            if (Mathf.Approximately(scale, 0f)) return TimeScaleState.Paused;
            if (Mathf.Approximately(scale, 1f)) return TimeScaleState.NormalSpeed;
            return scale < 1f ? TimeScaleState.SlowMotion : TimeScaleState.FastForward;
        }

        public static string GetStateLabel()
        {
            switch (GetState())
            {
                case TimeScaleState.Paused: return "Paused";
                case TimeScaleState.SlowMotion: return "Slow Motion";
                case TimeScaleState.FastForward: return "Fast Forward";
                default: return "Normal Speed";
            }
        }

        public static void Set(float value)
        {
            if (!Application.isPlaying)
                return;

            if (!IsPaused)
                lastNonZeroScale = Time.timeScale;

            Time.timeScale = Mathf.Clamp(value, MinScale, MaxScale);
        }

        public static void Pause() => Set(0f);
        public static void Resume() => Set(lastNonZeroScale);
        public static void ResetToNormal() => Set(1f);
        public static void ResetMemory() => lastNonZeroScale = 1f;
    }
}