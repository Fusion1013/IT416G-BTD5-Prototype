using UnityEngine;

namespace Core.Curves
{
    // Algorithms from https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    public class BezierCurve
    {
        public readonly Vector3[] points;

        public BezierCurve() => points = new Vector3[4];
        public BezierCurve(Vector3[] points) => this.points = points;

        public Vector3 StartPosition => points[0];
        public Vector3 EndPosition => points[3];

        private Vector3 GetSegment(float time)
        {
            time = Mathf.Clamp01(time);
            var iTime = 1 - time;
            return (iTime * iTime * iTime * points[0])
                   + (3 * iTime * iTime * time * points[1])
                   + (3 * iTime * time * time * points[2])
                   + (time * time * time * points[3]);
        }

        public Vector3[] GetSegments(int subdivisions)
        {
            var segments = new Vector3[subdivisions];

            for (int i = 0; i < subdivisions; i++)
            {
                var time = (float)i / subdivisions;
                segments[i] = GetSegment(time);
            }

            return segments;
        }
    }
}
