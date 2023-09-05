using System.Collections.Generic;
using Core.Curves;
using UnityEditor;
using UnityEngine;

namespace Enemy
{
    public class Path : MonoBehaviour
    {
        [Header("Points")]
        public List<PathPoint> waypoints;
        [SerializeField] private float smoothingLength = 2f;
        [SerializeField] private int smoothingSections = 10;
        public int SmoothingSections => smoothingSections;
        
        private Vector3[] _smoothPoints;
        public Vector3[] Points => _smoothPoints;

        public BezierCurve[] Curves => _curves;
        private BezierCurve[] _curves;

        [Header("Drawing")] 
        public bool drawBezier;
        public float bezierTangentDrawSize = 0.1f;

#if UNITY_EDITOR
        private void OnValidate()
#else
        private void Awake()
#endif
        {
            EnsureCurvesMatchPointPositions();
            UpdateCurves();
            CreateSmoothPoints();
        }

        private void CreateSmoothPoints()
        {
            if (_curves == null) return;
            var smoothSegments = new List<Vector3>();
            foreach (var curve in _curves)
            {
                var segments = curve.GetSegments(smoothingSections);
                for (var j = 0; j < segments.Length - 1; j++) smoothSegments.Add(segments[j]);
            }

            _smoothPoints = smoothSegments.ToArray();
        }

        private void EnsureCurvesMatchPointPositions()
        {
            if (waypoints is not { Count: > 0 }) return;
            if (_curves != null && _curves.Length == waypoints.Count - 1) return;

            _curves = new BezierCurve[waypoints.Count - 1];
            for (var i = 0; i < _curves.Length; i++) _curves[i] = new BezierCurve();
        }

        public void UpdateCurves()
        {
            for (var i = 0; i < waypoints.Count - 1; i++)
            {
                var w1 = waypoints[i];
                var w2 = waypoints[i + 1];
                var position = w1.transform.position;
                var startTangent = w1.outTangent.normalized * smoothingLength; // TODO: Move tangent calculations to PathPoint class
                var endTangent = w2.inTangent.normalized * smoothingLength;
                var nextPosition = w2.transform.position;

                _curves[i].points[0] = position;
                _curves[i].points[1] = position + startTangent;
                _curves[i].points[2] = nextPosition + endTangent;
                _curves[i].points[3] = nextPosition;
            }
        }
    }
}
