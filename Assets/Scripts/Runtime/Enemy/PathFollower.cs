using System;
using UnityEngine;

namespace Enemy
{
    public class PathFollower : MonoBehaviour
    {
        #region Fields

        // Movement
        [HideInInspector] public Path path;
        [HideInInspector] public float movementSpeed;

        public float distanceMoved;
        public float DistanceFromWaypoint => 1f - distanceMoved;
        private Vector3 _lastWaypointPos;
        private Vector3 _targetWaypointPos;
        private float _waypointDistance; // The distance between the last and next waypoints
        public int TargetWaypointId { get; set; }
        
        // Rotation
        private Quaternion _startRotation;
        private Quaternion _targetRotation;
        private float _rotateTime = 0;

        // Settings
        [SerializeField] private bool rotateAlongPath = true;
        [SerializeField] private SpriteRenderer spriteToFlip;
        [SerializeField] private bool flipX = false;
        [SerializeField] private bool flipY = false;

        // Events
        public event Action<PathFollower> OnPathFinished;

        #endregion

        #region Init

        private void Start()
        {
            UpdateWaypoints();
            SetRotation();
        }

        #endregion

        #region Update

        private void Update()
        {
            Move();
            if (rotateAlongPath) RotateTowardsTarget();
            Flip();
        }

        #endregion

        #region Movement

        private void Move()
        {
            distanceMoved += Time.deltaTime * movementSpeed / _waypointDistance;
            
            // If not at the waypoint yet, keep moving towards it
            if (transform.position != _targetWaypointPos)
            {
                transform.position = Vector3.Lerp(_lastWaypointPos, _targetWaypointPos, Mathf.Min(distanceMoved, 1));
            }
            else
            {
                // Check if there are more waypoints in the path
                if (TargetWaypointId < path.Points.Length - 1)
                {
                    TargetWaypointId++;
                    distanceMoved = 0;
                    UpdateWaypoints();
                    SetRotation();
                }
                else // If there are not, call the OnPathFinished event
                {
                    OnPathFinished?.Invoke(this);
                }
            }
        }
        
        private void UpdateWaypoints()
        {
            _lastWaypointPos = transform.position;
            _targetWaypointPos = path.Points[TargetWaypointId];
            _waypointDistance = Vector3.Distance(_lastWaypointPos, _targetWaypointPos);
        }

        #endregion

        #region Rotating

        /// <summary>
        /// Slowly rotate towards the target rotation.
        /// </summary>
        private void RotateTowardsTarget()
        {
            _rotateTime += Time.deltaTime * 2f * movementSpeed;
            transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, _rotateTime);
        }

        private void SetRotation()
        {
            // Rotate the sprite to look in the movement direction
            var position = transform.position;
            Vector2 targetPos = new Vector3(_targetWaypointPos.x - position.x,
                _targetWaypointPos.y - position.y);
            var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;

            _startRotation = transform.rotation;
            _targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
            _rotateTime = 0;
        }

        #endregion

        #region Flipping

        private void Flip()
        {
            var pos = transform.position;
            
            if (flipX)
            {
                if (pos.x < _targetWaypointPos.x) spriteToFlip.flipX = false;
                else if (pos.x > _targetWaypointPos.x) spriteToFlip.flipX = true;
            }

            if (flipY)
            {
                if (pos.y > _targetWaypointPos.y) spriteToFlip.flipY = false;
                else if (pos.y < _targetWaypointPos.y) spriteToFlip.flipY = true;
            }
        }

        #endregion
    }
}
