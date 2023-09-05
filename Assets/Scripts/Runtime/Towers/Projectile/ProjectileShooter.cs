using System;
using UnityEngine;

namespace Towers.Projectile
{
    public class ProjectileShooter : MonoBehaviour
    {
        [SerializeField] [Tooltip("The data of the projectile it shoots")] private Towers.Projectile.Projectile projectilePrefab;
        [SerializeField] [Tooltip("When in the firing cycle to fire the gun")] [Range(0f, 1f)] private float fireOffset;
        [SerializeField] [Tooltip("How the tower selects which enemy to target")] public TargetingPriority targetingPriority;
        // TODO: Projectile modifiers

        public float FireOffset
        {
            get => fireOffset;
            set
            {
                fireOffset = value;
                _time = FireRate * fireOffset;
            }
        }
        public float FireRate
        {
            get => _fireRate;
            set
            {
                _fireRate = value;
                _time = _fireRate * fireOffset;
            }
        }
        private float _fireRate;
        private float _time;

        // Animations
        private Animator _animator;
        private static readonly int FireAnimId = Animator.StringToHash("Fire");
        
        // Events
        public event Action OnShoot;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _time = FireRate * fireOffset;
        }

        /// <summary>
        /// Attempts to shoot a projectile.
        /// </summary>
        public void TryShoot()
        {
            _time += Time.deltaTime;
            if (!(_time >= _fireRate)) return;
            Shoot();
            OnShoot?.Invoke();
            _time = 0;
        }

        private void Shoot()
        {
            var gunTransform = transform;
            
            // Spawn the projectile and set the data of it
            var projectile = Instantiate(projectilePrefab, gunTransform.position, gunTransform.rotation);
            // TODO: Apply modifiers

            if (_animator) _animator.SetTrigger(FireAnimId);
        }

        private void OnDrawGizmos()
        {
            var gunTransform = transform;
            var gunPosition = gunTransform.position;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(gunPosition, gunPosition + gunTransform.rotation * Vector3.up);
            Gizmos.DrawSphere(gunPosition, .05f);
        }
    }
}
