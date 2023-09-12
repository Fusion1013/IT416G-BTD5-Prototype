using System;
using UnityEngine;

namespace Towers.Projectile
{
    [CreateAssetMenu(fileName = "Boomerang Projectile Path", menuName = "Tower Defense/Projectile/Path/Boomerang Path")]
    public class SProjectilePathBoomerang : SProjectilePathBase
    {
        [SerializeField]
        [Tooltip("The amount of time before the projectile starts moving backwards (s)")]
        private float timeOffset;

        [SerializeField]
        private float timeDampener = 1;
        
        private void OnValidate()
        {
            if (timeOffset < 0)
            {
                Debug.LogWarning("Time offset should be higher than or equal to zero");
                timeOffset = 0;
            }
        }

        public override void MoveProjectile(Projectile projectile, ProjectilePathInfo info)
        {
            float timeOverride = (info.time / timeDampener) - timeOffset;
            float speedOverride = info.movementSpeed * -timeOverride;
            
            var projectileTransform = projectile.transform;
            var moveDelta = info.moveDirection * (speedOverride * Time.deltaTime);
            var movement = info.projectileRotation * moveDelta;
            projectileTransform.position += movement;
        }
    }
}
