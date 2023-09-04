using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(fileName = "New Tower Upgrade Data", menuName = "Tower Defense/Towers/Tower Upgrade Data")]
    public class TowerUpgradeData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string upgradeName;
        [SerializeField] private int cost;
        [SerializeField] [TextArea(4, 20)] private string description;

        [Header("Visuals")] 
        [SerializeField] private Sprite icon;
        
        // TODO: Add components that modify the tower/projectile
        // TODO: Add editor script that allows upgrades to be created directly from tower data GUI, store upgrades as new SO's under parent

        [Header("Tower")]
        public float additiveRange;
        public float additiveFireRate;

        [Header("Projectile")] 
        public int additivePiercing;
        public float additiveLifetime;
        public float additiveProjectileSpeed;
        public int additiveDamage;
        
        [Tooltip("Overrides the sprite used for the projectile. Higher level upgrades are prioritized, higher paths are prioritized")] 
        public Sprite projectileSpriteOverride;
    }
}