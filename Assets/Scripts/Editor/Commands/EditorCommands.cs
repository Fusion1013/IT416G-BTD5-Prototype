using UnityEditor;
using UnityEngine;

namespace Commands
{
    public static class EditorCommands
    {
        private static int _uiLayers = LayerMask.GetMask("UI");

        [MenuItem("Commands/Toggle/UI Layers %q")]
        public static void ToggleUILayers()
        {
            if (Tools.visibleLayers != _uiLayers) Tools.visibleLayers = _uiLayers;
            else Tools.visibleLayers = ~_uiLayers;
        }
    }
}
