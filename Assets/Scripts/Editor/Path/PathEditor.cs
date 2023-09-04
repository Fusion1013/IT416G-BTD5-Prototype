using Enemy;
using UnityEditor;
using UnityEngine;

namespace Path
{
    [CustomEditor(typeof(Enemy.Path))]
    public class PathEditor : Editor
    {
        private Enemy.Path _path;
        
        private void OnEnable()
        {
            _path = (Enemy.Path)target;
        }

        private void OnSceneGUI()
        {
            PathPointGUI();
            if (_path.drawBezier) DrawSegments();
        }

        private void PathPointGUI()
        {
            foreach (var pathPoint in _path.waypoints)
            {
                DrawPathPointPositionHandles(pathPoint);
                DrawPathPointTangentPoints(pathPoint);
            }
        }

        private void DrawPathPointPositionHandles(Component pathPoint)
        {
            EditorGUI.BeginChangeCheck();
            pathPoint.transform.position = Handles.PositionHandle(pathPoint.transform.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                _path.UpdateCurves();
                EditorUtility.SetDirty(pathPoint);
                EditorUtility.SetDirty(_path);
                    
                Undo.RecordObject(pathPoint, "Moved Path Point"); // TODO
            }
        }

        private static void DrawPathPointTangentPoints(PathPoint pathPoint)
        {
            var position = pathPoint.transform.position;
            Handles.color = Color.green;
            Handles.DrawLine(position, position + pathPoint.inTangent);
            Handles.color = Color.red;
            Handles.DrawLine(position, position + pathPoint.outTangent);
        }

        private void DrawSegments()
        {
            Handles.color = Color.magenta;
            for (var i = 0; i < _path.Curves.Length; i++)
            {
                // Draw all lines except for the last one
                var segments = _path.Curves[i].GetSegments(_path.SmoothingSections);
                for (var j = 0; j < segments.Length - 1; j++)
                {
                    Handles.DrawLine(segments[j], segments[j + 1]);
                    Handles.Label(segments[j], $"C{i} S{j}");
                }
                
                // Draw the last line
                Handles.Label(segments[^1], $"C{i} S{segments.Length - 1}");
                Handles.DrawLine(segments[^1], _path.Curves[i].EndPosition);
            }
        }
    }
}
