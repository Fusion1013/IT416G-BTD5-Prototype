using System;
using Core.Extensions;
using Enemy;
using TMPro;
using Towers.Projectile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Towers
{
    public class TowerBrain : MonoBehaviour
    {
        #region Fields

        [Header("Tower")]
        [SerializeField] 
        [Tooltip("Weather the tower should rotate to face the target enemy or not")]
        private bool rotate = true;

        [SerializeField]
        private GameObject pivotPoint;

        [SerializeField] private GameObject rangeIndicator;
        public float baseSize;

        [Header("Shooting")]
        [SerializeField] 
        [Tooltip("The time between activations of the shooters attached to this tower")]
        private float baseFireRate;
        private float _fireRateModified;
        
        [SerializeField] 
        [Tooltip("The range at which this tower can target enemies")]
        private float baseRange;
        public float RangeModified { get; private set; }

        [SerializeField] 
        [Tooltip("How the tower selects which enemy to target")]
        private TargetingPriority targetingPriority;
        
        [SerializeField] private ProjectileShooter[] projectileShooters;
        public ProjectileShooter[] ProjectileShooters => projectileShooters;
        
        [FormerlySerializedAs("canShoot")] [HideInInspector] public bool isActive;
        
        [Header("Events")]
        [SerializeField] 
        [Tooltip("Called when any of the Projectile Shooters connected to the tower shoots")]
        private UnityEvent onTowerFire;

        [Header("Tower Info")] 
        [SerializeField] private GameObject canvas;
        [SerializeField] private TMP_Text killsAmountText;

        private EnemyBrain _target;
        private float _time;
        
        [HideInInspector] public int enemiesKilled;
        public Collider2D Footprint { get; private set; }

        // -- Animations
        private Animator _animator;
        private static readonly int TowerPlace = Animator.StringToHash("TowerPlace");

        #endregion

        #region Init

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            Footprint = GetComponent<Collider2D>();
            CalculateModifiers();
            UpdateEnemiesKilledText(null);
        }

        private void Start() => ReloadShooters();

        private void OnEnable()
        {
            TowerManager.OnTowerFinishPlace += TowerFinishPlace;
            Projectile.Projectile.OnEnemyCollision += UpdateEnemiesKilledText;
        }

        private void OnDisable() 
        {
            UnsubscribeFromShooters();
            TowerManager.OnTowerFinishPlace -= TowerFinishPlace;
            Projectile.Projectile.OnEnemyCollision -= UpdateEnemiesKilledText;
        }

        private void OnValidate()
        {
            ValidateFields();
            CalculateModifiers();
            LoadShooters();
        }

        private void ValidateFields()
        {
            if (baseRange < 0)
            {
                Debug.LogWarning($"Base range should be at least 0 (Current: {baseRange})", this);
                baseRange = 0;
            }

            if (baseFireRate < 0)
            {
                Debug.LogWarning($"Base fire rate should be at least 0 (Current: {baseFireRate})", this);
                baseFireRate = 0;
            }
        }

        private void CalculateModifiers()
        {
            RangeModified = baseRange;
            _fireRateModified = baseFireRate;
        }

        private void ReloadShooters()
        {
            // Unsubscribe from shooter events
            UnsubscribeFromShooters();

            LoadShooters();
            
            // Subscribe to shooter events
            SubscribeToShooters();
        }

        private void LoadShooters()
        {
            // Find all shooters connected to tower
            projectileShooters = GetComponentsInChildren<ProjectileShooter>();
            
            // Set the values for all shooters connected to the tower
            foreach (var gun in projectileShooters)
            {
                if (gun == null) continue;
                
                gun.FireRate = _fireRateModified;
                gun.targetingPriority = targetingPriority;
                gun.parent = this;
            }
        }

        private void SubscribeToShooters()
        {
            foreach (var gun in projectileShooters)
            {
                gun.OnShoot += OnGunShoot;
            }
        }

        private void UnsubscribeFromShooters()
        {
            foreach (var gun in projectileShooters)
            {
                gun.OnShoot -= OnGunShoot;
            }
        }

        #endregion

        #region Animations

        private void TowerFinishPlace(TowerBrain tower)
        {
            if (tower == this) _animator.SetTrigger(TowerPlace);
        }

        #endregion

        #region Targeting

        private void Update()
        {
            if (!isActive) return;
            
            _target = EnemyManager.Instance.FindEnemy(targetingPriority, transform.position, RangeModified);
            if (rotate) LookAtTarget();
            FireAllShooters();
        }

        private void FireAllShooters()
        {
            foreach (var shooter in projectileShooters)
            {
                var enemy = EnemyManager.Instance.FindEnemy(shooter.targetingPriority, transform.position, RangeModified);
                if (enemy) shooter.TryShoot();
            }
        }

        private void OnGunShoot() => onTowerFire?.Invoke();

        /// <summary>
        /// Turns the tower to face the enemy it is targeting.
        /// </summary>
        private void LookAtTarget()
        {
            if (_target == null) return;
            pivotPoint.transform.rotation = QuaternionExtensions.Facing(_target.transform.position, transform.position, 90);
        }

        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            
            // Draw range Gizmo
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, RangeModified);

            // Draw base size
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(position, baseSize);
            
            // Draw target line
            if (_target == null) return;
            var targetPos = _target.transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPos);
        }

        #endregion

        #region Information

        private void UpdateEnemiesKilledText(GameObject obj) => killsAmountText.text = $"Kills: {enemiesKilled}";

        public void ShowTowerInfo(bool show)
        {
            rangeIndicator.transform.localScale = Vector3.one * RangeModified * 2;
            rangeIndicator.SetActive(show);
            killsAmountText.gameObject.SetActive(show);
            canvas.SetActive(show);
        }

        public void SetRangeIndicatorColor(Color color) => rangeIndicator.GetComponent<SpriteRenderer>().color = color;

        #endregion
    }

    public enum TargetingPriority
    {
        First, Last, Close, Strong
    }
}