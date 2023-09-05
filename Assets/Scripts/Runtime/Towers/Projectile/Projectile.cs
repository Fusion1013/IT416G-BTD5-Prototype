using System;
using UnityEngine;

namespace Towers.Projectile
{
    public class Projectile : MonoBehaviour
    {
        public static event Action<GameObject> OnEnemyCollision;

        [Tooltip("The data of the projectile")]
        public SProjectileData data;
        
        [SerializeField] 
        [Range(0, 360)]
        [Tooltip("The angle that the projectile moves in")]
        private float moveAngle;
        
        // Internal
        private Quaternion _cachedRotation;
        private Quaternion AngleRotation => Quaternion.Euler(0, 0, moveAngle);
        private Vector3 MoveDirection => AngleRotation * Vector3.up;
        private Vector3 _centerPosition;
        private float _time;
        private int _hits;

        [Header("Wiggle")]
        [SerializeField] private float amplitude;
        [SerializeField] private float frequency;

        private void Awake()
        {
            var projectileTransform = transform;
            _centerPosition = projectileTransform.position;
            _cachedRotation = projectileTransform.rotation;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            
            Move();
            CheckOutOfBounds();
            
            if (_time > data.lifetime) Destroy(gameObject);
        }

        // TODO: Introduce different movement paths (Straight, wiggle, etc.)
        private void Move()
        {
            var projectileTransform = transform;
            var moveDelta = MoveDirection * (data.movementSpeed * Time.deltaTime);
            var movement = _cachedRotation * moveDelta;
            _centerPosition += movement;
            var sinOffset = _cachedRotation * new Vector3(Mathf.Sin(_time * frequency) * amplitude, 0, 0);
            // TODO: Rotation
            // TODO: Wiggle

            // projectileTransform.position = _centerPosition + sinOffset;
            projectileTransform.position = _centerPosition;
        }

        private void CheckOutOfBounds()
        {
            var boundsX = TowerManager.Instance.Bounds.x;
            var boundsY = TowerManager.Instance.Bounds.y;
            
            var pos = transform.position;
            if (pos.x > boundsX || pos.y < -boundsX) Destroy(gameObject);
            else if (pos.y > boundsY || pos.y < -boundsY) Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Enemy") || _hits >= data.piercing) return;

            _hits++;
            OnEnemyCollision?.Invoke(col.gameObject);

            if (_hits >= data.piercing)
            {
                Destroy(gameObject);
                if (data.instantiateOnDeath != null)
                {
                    Instantiate(data.instantiateOnDeath, transform.position, Quaternion.identity);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var projectilePosition = transform.position;
            Gizmos.DrawLine(projectilePosition, projectilePosition + MoveDirection * data.movementSpeed);
        }
    }
}
