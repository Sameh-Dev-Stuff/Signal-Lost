using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtilities
{
    /// <summary>
    /// General-purpose static utility library for Unity projects.
    /// No need to attach this to any GameObject — everything is accessed statically.
    /// Coroutine-based helpers (DelayedCall, RepeatEvery) spin up a hidden
    /// persistent runner GameObject automatically the first time they're used.
    /// </summary>
    public static class Utilities
    {
        #region Timer / Repeat Utilities

        // Each caller gets its own independent timer, keyed by id,
        // so multiple RepeatAction calls never interfere with each other.
        private static readonly Dictionary<string, float> _timers = new Dictionary<string, float>();

        /// <summary>
        /// Call this every frame (e.g. from Update). When `repeatTime` seconds have
        /// passed since the last trigger for this `id`, `action` is invoked and the timer resets.
        /// </summary>
        public static void RepeatAction(string id, float repeatTime, Action action)
        {
            if (action == null || string.IsNullOrEmpty(id)) return;

            if (!_timers.ContainsKey(id))
                _timers[id] = 0f;

            _timers[id] += Time.deltaTime;
            if (_timers[id] >= repeatTime)
            {
                action.Invoke();
                _timers[id] = 0f;
            }
        }

        /// <summary>Resets a timer without invoking its action.</summary>
        public static void ResetTimer(string id)
        {
            if (_timers.ContainsKey(id)) _timers[id] = 0f;
        }

        /// <summary>Removes a timer entirely (use when you're done with it, to free memory).</summary>
        public static void RemoveTimer(string id)
        {
            _timers.Remove(id);
        }

        #endregion

        #region Delayed / Coroutine Utilities

        private static UtilityRunner _runner;
        private static UtilityRunner Runner
        {
            get
            {
                if (_runner == null)
                {
                    var go = new GameObject("[UtilityRunner]");
                    _runner = go.AddComponent<UtilityRunner>();
                    UnityEngine.Object.DontDestroyOnLoad(go);
                }
                return _runner;
            }
        }

        /// <summary>Invokes `action` once, after `delay` seconds. No manual Update calls needed.</summary>
        public static Coroutine DelayedCall(float delay, Action action)
        {
            if (action == null) return null;
            return Runner.StartCoroutine(DelayedCallRoutine(delay, action));
        }

        private static IEnumerator DelayedCallRoutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        /// <summary>
        /// Invokes `action` every `interval` seconds, forever (or until `duration` runs out, if given).
        /// Runs on a coroutine, so unlike RepeatAction it doesn't need to be pumped from Update.
        /// </summary>
        public static Coroutine RepeatEvery(float interval, Action action, float? duration = null)
        {
            if (action == null) return null;
            return Runner.StartCoroutine(RepeatEveryRoutine(interval, action, duration));
        }

        private static IEnumerator RepeatEveryRoutine(float interval, Action action, float? duration)
        {
            float elapsed = 0f;
            var wait = new WaitForSeconds(interval);
            while (duration == null || elapsed < duration.Value)
            {
                yield return wait;
                action.Invoke();
                elapsed += interval;
            }
        }

        /// <summary>Stops every DelayedCall / RepeatEvery currently running.</summary>
        public static void StopAllDelayedCalls()
        {
            if (_runner != null) _runner.StopAllCoroutines();
        }

        #endregion

        #region Math Utilities

        /// <summary>Remaps a value from one range to another (e.g. Remap(hp, 0, 100, 0, 1)).</summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }

        public static Vector3 RandomPointInSphere(Vector3 center, float radius)
        {
            return center + UnityEngine.Random.insideUnitSphere * radius;
        }

        public static Vector2 RandomPointInCircle(Vector2 center, float radius)
        {
            return center + UnityEngine.Random.insideUnitCircle * radius;
        }

        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        #endregion

        #region GameObject / Transform Utilities

        /// <summary>Gets a component, adding it first if it doesn't already exist.</summary>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp == null) comp = go.AddComponent<T>();
            return comp;
        }

        /// <summary>Destroys every direct child of this transform.</summary>
        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>Sets the layer on this GameObject and all of its children, recursively.</summary>
        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        #endregion

        #region Camera Utilities

        /// <summary>Checks whether a renderer's bounds are inside the given camera's view frustum.</summary>
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }

        #endregion

        #region Random Utilities

        /// <summary>In-place Fisher-Yates shuffle.</summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>Picks a random item, weighted by the given weights array (same length as items).</summary>
        public static T WeightedRandom<T>(T[] items, float[] weights)
        {
            if (items == null || weights == null || items.Length != weights.Length || items.Length == 0)
                throw new ArgumentException("items and weights must be the same non-zero length.");

            float total = 0f;
            foreach (var w in weights) total += w;

            float roll = UnityEngine.Random.value * total;
            float cumulative = 0f;
            for (int i = 0; i < items.Length; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative) return items[i];
            }
            return items[items.Length - 1];
        }

        #endregion

        #region Color Utilities

        /// <summary>Converts a hex string ("#RRGGBB" or "RRGGBB") to a Color. Returns white if invalid.</summary>
        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.white;
            hex = hex.TrimStart('#');

            if (ColorUtility.TryParseHtmlString("#" + hex, out var color))
                return color;

            Debug.LogWarning($"Utilities.HexToColor: invalid hex string '{hex}', returning white.");
            return Color.white;
        }

        /// <summary>Returns a copy of this color with a different alpha.</summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        #endregion
    }

    /// <summary>Hidden MonoBehaviour used internally by Utilities to run coroutines.</summary>
    internal class UtilityRunner : MonoBehaviour { }

    /// <summary>
    /// Generic MonoBehaviour singleton base class.
    /// Usage: public class GameManager : Singleton&lt;GameManager&gt; { ... }
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Unity 2023+: consider FindFirstObjectByType<T>() instead, it's faster.
                    _instance = UnityEngine.Object.FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}