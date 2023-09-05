using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Extensions;
using Enemy;
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

        [Header("Upgrades")] 
        [SerializeField] 
        private UpgradePath[] upgradePaths;

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
        
        private EnemyBrain _target;
        private float _time;
        public Collider2D Footprint { get; private set; }

        #endregion

        #region Init

        private void Awake()
        {
            Footprint = GetComponent<Collider2D>();
            CalculateModifiers();
        }

        private void Start() => ReloadShooters();
        private void OnDisable() => UnsubscribeFromShooters();

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
            
            foreach (var path in upgradePaths)
            {
                foreach (var upgrade in path.upgrades)
                {
                    if (!upgrade.isUnlocked) continue;
                    
                    RangeModified += upgrade.data.additiveRange;
                    _fireRateModified += upgrade.data.additiveFireRate;
                }
            }
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

        public void ShowRangeIndicator(bool show)
        {
            rangeIndicator.transform.localScale = Vector3.one * RangeModified * 2;
            rangeIndicator.SetActive(show);
        }

        public void SetRangeIndicatorColor(Color color) => rangeIndicator.GetComponent<SpriteRenderer>().color = color;
        private void OnMouseEnter() => ShowRangeIndicator(isActive || rangeIndicator.activeSelf);
        private void OnMouseExit() => ShowRangeIndicator(!isActive && rangeIndicator.activeSelf);

        #endregion
    }

    public enum TargetingPriority
    {
        First, Last, Close, Strong
    }

    [Serializable]
    public struct UpgradePath
    {
        public TowerUpgrade[] upgrades;
    }

    [Serializable]
    public struct TowerUpgrade
    {
        public TowerUpgradeData data;
        [ReadOnly] public bool isUnlocked;
    }
}