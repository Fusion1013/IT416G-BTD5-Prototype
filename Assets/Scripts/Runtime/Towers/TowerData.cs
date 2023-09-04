using System;
using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(fileName = "New Tower Data", menuName = "Tower Defense/Towers/Tower Data")]
    public class TowerData : ScriptableObject
    {
        [Header("General")]
        public string towerName;
        public int cost;

        [Header("Scene")] 
        public GameObject scenePrefab; // TODO: Replace GameObject with some script that handles placement of tower
        
        [Header("Visual")] 
        public Sprite shopIcon;
    }
}