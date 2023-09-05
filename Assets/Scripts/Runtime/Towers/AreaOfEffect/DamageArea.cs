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

        private float _cooldownTimer;
        private float _time;

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
            var colliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
            foreach (var col in colliders)
            {
                var enemy = col.GetComponent<EnemyBrain>();
                if (enemy == null) continue;
                enemy.Kill();
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
