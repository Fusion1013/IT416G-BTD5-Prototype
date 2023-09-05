using System;
using UnityEngine;

namespace Enemy
{
    public class PathPoint : MonoBehaviour
    {
        public Vector3 inTangent;
        public Vector3 outTangent;

        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(position, position + inTangent);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, position + outTangent);
        }
    }
}
