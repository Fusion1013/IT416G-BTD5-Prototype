using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Extensions;
using Enemy;
using UnityEngine;
using UnityEngine.Events;

namespace Towers
{
    public class TowerBrain : MonoBehaviour
    {
        #region Fields

        [Header("Tower")]
        [SerializeField] 
        [Tooltip("Weather the tower should rotate to face the target enemy or not")]
        private bool rotate = true;

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
        private float _rangeModified;

        [SerializeField] 
        [Tooltip("How the tower selects which enemy to target")]
        private TargetingPriority targetingPriority;
        
        [SerializeField] private ProjectileShooter[] projectileShooters;
        public ProjectileShooter[] ProjectileShooters => projectileShooters;
        
        [Header("Events")]
        [SerializeField] 
        [Tooltip("Called when any of the Projectile Shooters connected to the tower shoots")]
        private UnityEvent onTowerFire;

        private EnemyBrain _target;
        private float _time;

        #endregion

        #region Init

        private void Start()
        {
            ReloadShooters();
        }

        private void OnDisable()
        {
            UnsubscribeFromShooters();
        }

        private void OnValidate()
        {
            CalculateModifiers();
            LoadShooters();
        }

        private void CalculateModifiers()
        {
            _rangeModified = baseRange;
            _fireRateModified = baseFireRate;
            
            foreach (var path in upgradePaths)
            {
                foreach (var upgrade in path.upgrades)
                {
                    if (!upgrade.isUnlocked) continue;
                    
                    _rangeModified += upgrade.data.additiveRange;
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
                
                gun.Range = _rangeModified;
                gun.FireRate = _fireRateModified;
                gun.targetingPriority = targetingPriority;
                gun.CenterPos = transform.position;
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

        private void Update()
        {
            _target = EnemyManager.Instance.FindEnemy(targetingPriority, transform.position, _rangeModified);
            if (rotate) LookAtTarget();
        }

        private void OnGunShoot() => onTowerFire?.Invoke();

        /// <summary>
        /// Turns the tower to face the enemy it is targeting.
        /// </summary>
        private void LookAtTarget()
        {
            if (_target == null) return;
            transform.rotation = QuaternionExtensions.Facing(_target.transform.position, transform.position, 90);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw range Gizmo
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _rangeModified);
            
            // Draw target line
            if (_target == null) return;
            var targetPos = _target.transform.position;
            Gizmos.DrawLine(transform.position, targetPos);
        }
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