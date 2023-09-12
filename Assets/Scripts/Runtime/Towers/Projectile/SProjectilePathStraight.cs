using UnityEngine;

namespace Towers.Projectile
{
    [CreateAssetMenu(fileName = "Straight Projectile Path", menuName = "Tower Defense/Projectile/Path/Straight Path")]
    public class SProjectilePathStraight : SProjectilePathBase
    {
        public override void MoveProjectile(Projectile projectile, ProjectilePathInfo info)
        {
            var projectileTransform = projectile.transform;
            var moveDelta = info.moveDirection * (info.movementSpeed * Time.deltaTime);
            var movement = info.projectileRotation * moveDelta;
            projectileTransform.position += movement;
        }
    }
}
