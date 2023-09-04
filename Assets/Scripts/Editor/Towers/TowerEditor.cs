using UnityEditor;
using UnityEngine;

namespace Towers
{
    [CustomEditor(typeof(TowerBrain))]
    public class TowerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tower = (TowerBrain)target;
            if (tower == null) return;

            EditorGUILayout.LabelField(new GUIContent("Shooter Time Offsets"), EditorStyles.boldLabel);

            var shooters = tower.ProjectileShooters;
            foreach (var shooter in shooters)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    var newFireOffset = EditorGUILayout.Slider(new GUIContent($"{shooter.name} Offset"), shooter.FireOffset, 0f, 1f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(shooter, "Changed Fire Offset");
                        shooter.FireOffset = newFireOffset;
                        EditorUtility.SetDirty(shooter);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
