using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Towers
{
    [CreateAssetMenu(fileName = "New Tower Data", menuName = "Tower Defense/Towers/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("General")]
        public string towerName;
        public int cost;
        [TextArea(4, 10)] public string description;

        [FormerlySerializedAs("scenePrefab")] [Header("Scene")] 
        public TowerBrain towerPrefab; // TODO: Replace GameObject with some script that handles placement of tower
        
        [Header("Visual")] 
        public Sprite shopIcon;
    }
}