using UnityEngine;

namespace Towers.Projectile
{
    public abstract class SProjectilePathBase : ScriptableObject
    {
        public abstract void MoveProjectile(Projectile projectile, ProjectilePathInfo info);
    }

    public struct ProjectilePathInfo
    {
        public Vector3 startPosition;
        public Vector3 moveDirection;
        public Quaternion projectileRotation;
        public float movementSpeed;
        public float time;

        public ProjectilePathInfo(Vector3 startPosition, Vector3 moveDirection, Quaternion projectileRotation, float movementSpeed, float time)
        {
            this.startPosition = startPosition;
            this.moveDirection = moveDirection;
            this.projectileRotation = projectileRotation;
            this.movementSpeed = movementSpeed;
            this.time = time;
        }
    }
}
