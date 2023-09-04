using Core.Hierarchy_Headers;
using UnityEditor;
using UnityEngine;

namespace Hierarchy
{
    /// <summary>
    /// Hierarchy Window Group Header
    /// From http://diegogiacomelli.com.br/unitytips-hierarchy-window-group-header
    /// Modified by William Reutmer
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyWindowGroupHeader
    {
        static HierarchyWindowGroupHeader()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!gameObject) return;

            var headerObject = gameObject.GetComponent<HierarchyHeaderObject>();
            if (!headerObject) return;
            
            EditorGUI.DrawRect(selectionRect, headerObject.color);

            var style = new GUIStyle
            {
                fontSize = headerObject.fontSize,
                fontStyle = headerObject.fontStyle,
                alignment = headerObject.textAlignment,
                normal =
                {
                    textColor = headerObject.fontColor
                }
            };

            if (headerObject.dropShadow) EditorGUI.DropShadowLabel(selectionRect, headerObject.title.ToUpperInvariant(), style);
            else EditorGUI.LabelField(selectionRect, headerObject.title.ToUpperInvariant(), style);
        }
    }
}