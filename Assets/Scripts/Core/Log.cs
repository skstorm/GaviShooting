using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace GaviShooting.Core
{
    public static class Log
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Info(string message)
        {
            Debug.Log($"[Gavi] {message}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Info(string format, params object[] args)
        {
            Debug.Log($"[Gavi] {string.Format(format, args)}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Warn(string message)
        {
            Debug.LogWarning($"[Gavi] {message}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Warn(string format, params object[] args)
        {
            Debug.LogWarning($"[Gavi] {string.Format(format, args)}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Error(string message)
        {
            Debug.LogError($"[Gavi] {message}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Error(string format, params object[] args)
        {
            Debug.LogError($"[Gavi] {string.Format(format, args)}");
        }
    }
}
