using System;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Tower Defense/Enemies/Enemy Data")]
    public class SEnemyData : ScriptableObject
    {
        [Header("Stats")]
        
        [Tooltip("The number of hits it takes to kill this enemy")] 
        public int health = 1;

        [Tooltip("The size of the enemy sprite")]
        public float scale = 1;
        
        [Tooltip("The speed with which this enemy moves along its designated path")]
        public float moveSpeed;
        
        [Tooltip("The enemies that should spawn when this enemy dies")]
        public EnemyDrop[] dropOnDeath;

        [Header("Visual")] 
        
        [Tooltip("The sprite that should display for this enemy")]
        public Sprite sprite;
    }

    [Serializable]
    public struct EnemyDrop
    {
        [Tooltip("The data of the enemy to spawn")]
        public SEnemyData data;
        
        [Tooltip("The number of enemies to spawn")]
        public int count;
    }
}
