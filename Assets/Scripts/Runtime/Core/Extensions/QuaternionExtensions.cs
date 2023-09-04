using UnityEngine;

namespace Core.Extensions
{
    public static class QuaternionExtensions
    {
        public static Quaternion Facing(Vector3 position, Vector3 target, float angleOffset = 0)
        {
            Vector2 targetPos = new Vector3(
                target.x - position.x,
                target.y - position.y
            );
            var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(new Vector3(0, 0, angle + angleOffset));
        }
    }
}
