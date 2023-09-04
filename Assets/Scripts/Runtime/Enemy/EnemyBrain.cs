using System;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(PathFollower))]
    public class EnemyBrain : MonoBehaviour
    {
        #region Fields
        
        // Events
        public static event Action<EnemyBrain> OnEnemySpawn;
        public static event Action<EnemyBrain> OnEnemyKilled; // Called when an enemy is killed by a projectile
        public static event Action<EnemyBrain> OnEnemyPathComplete;

        // Enemy Data
        public SEnemyData data;
        private bool _isDead = false;

        // Movement
        private PathFollower _pathFollower;
        public PathFollower PathFollower => _pathFollower;
        
        // -- Visuals
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Init

        private void Awake()
        {
            _pathFollower = GetComponent<PathFollower>();
        }

        private void Start()
        {
            OnEnemySpawn?.Invoke(this);
            UpdateEnemyVisual();
            _pathFollower.movementSpeed = data.moveSpeed;
        }

        private void OnEnable()
        {
            _pathFollower.OnPathFinished += OnPathFinished;
        }

        private void OnDisable()
        {
            _pathFollower.OnPathFinished -= OnPathFinished;
        }

        #endregion

        private void OnPathFinished(PathFollower pathFollower)
        {
            OnEnemyPathComplete?.Invoke(this);
            Destroy();
        }

        public void Kill()
        {
            if (_isDead) return;
            _isDead = true;
            OnEnemyKilled?.Invoke(this);
            Destroy();
        }

        public void Destroy()
        {
            _isDead = true;
            Destroy(gameObject);
        }

        private void UpdateEnemyVisual()
        {
            spriteRenderer.sprite = data.sprite;
            transform.localScale = Vector3.one * data.scale;
        }
    }
}
