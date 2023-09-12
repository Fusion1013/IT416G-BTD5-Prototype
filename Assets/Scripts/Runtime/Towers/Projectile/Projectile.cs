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
        
        private Vector3 _startPosition;
        private float _time;
        private int _hits;

        public TowerBrain parent;
        
        private void Awake()
        {
            var projectileTransform = transform;
            _startPosition = projectileTransform.position;
            _cachedRotation = projectileTransform.rotation;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            
            Move();
            CheckOutOfBounds();
            
            if (_time > data.lifetime) Destroy(gameObject);
        }

        private void Move()
        {
            data.projectilePath.MoveProjectile(this, new ProjectilePathInfo(
                _startPosition, MoveDirection, _cachedRotation, data.movementSpeed, _time
            ));
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

            parent.enemiesKilled++;

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
