using UnityEngine;

namespace Towers.Projectile
{
    [CreateAssetMenu(fileName = "New Projectile Data", menuName = "Tower Defense/Towers/Projectile Data")]
    public class SProjectileData : ScriptableObject
    {
        [Header("Movement")] 
        public float movementSpeed;

        [Header("Statistics")] 
        public float lifetime = 2f;
        public int piercing = 1;
        
        [Header("Events")] 
        public GameObject instantiateOnSpawn;
        public GameObject instantiateOnDeath;

        [Header("Visual")]
        public Towers.Projectile.Projectile projectilePrefab;
    }
}
