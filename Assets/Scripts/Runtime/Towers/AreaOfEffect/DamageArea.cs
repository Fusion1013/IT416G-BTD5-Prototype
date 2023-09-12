using System;
using Enemy;
using UnityEngine;

namespace Towers.AreaOfEffect
{
    public class DamageArea : MonoBehaviour
    {
        #region Fields

        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float lifetime;
        [SerializeField] private float cooldown;
        [SerializeField] private float range;
        
        [SerializeField]
        [Tooltip("The number of enemies the damage area can kill each tick")]
        private int piercing = 10;

        [SerializeField]
        [Tooltip("The number of enemies the damage area can kill in total")]
        private int enemyLimit = 100;

        private float _cooldownTimer;
        private float _time;
        private int _enemiesKilled;

        #endregion

        #region Update

        private void Update()
        {
            _cooldownTimer += Time.deltaTime;
            _time += Time.deltaTime;

            if (_cooldownTimer > cooldown)
            {
                DamageInArea();
                _cooldownTimer = 0;
            }
            
            if (_time > lifetime) Destroy(gameObject);
        }

        private void DamageInArea()
        {
            int enemiesKilledThisTick = 0;
            
            var colliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
            foreach (var col in colliders)
            {
                var enemy = col.GetComponent<EnemyBrain>();
                if (enemy == null) continue;
                enemy.Kill();
                
                _enemiesKilled++;
                enemiesKilledThisTick++;

                if (_enemiesKilled > enemyLimit) Destroy(gameObject);
                if (enemiesKilledThisTick > piercing) break;
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, range);
        }

        #endregion
    }
}
