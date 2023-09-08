using UnityEngine;

namespace Core.Debug
{
    public class Debug : UnityEngine.Debug
    {
        public static void LogFatal(object message) => LogFatal(message, null);
        
        public static void LogFatal(object message, Object context)
        {
            LogError(message, context);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
