using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(fileName = "New Tower Data", menuName = "Tower Defense/Towers/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("General")]
        public string towerName;
        public int cost;
        [TextArea(4, 10)] public string description;

        [Header("Scene")] 
        public TowerBrain towerPrefab;
        
        [Header("Visual")] 
        public Sprite shopIcon;
    }
}