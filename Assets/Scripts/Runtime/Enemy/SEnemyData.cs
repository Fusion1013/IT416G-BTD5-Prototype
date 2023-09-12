using System;
using Towers;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Tower Defense/Enemies/Enemy Data")]
    public class SEnemyData : ScriptableObject
    {
        [Header("Stats")]
        
        [Tooltip("The number of hits it takes to kill this enemy (>0)")] 
        public int health = 1;

        [Tooltip("The size of the enemy sprite (>=0)")]
        public float scale = 1;
        
        [Tooltip("The speed with which this enemy moves along its designated path (>=0)")]
        public float moveSpeed;
        
        [Tooltip("The enemies that should spawn when this enemy dies")]
        public EnemyDrop[] dropOnDeath;

        [Header("Visual")] 
        
        [Tooltip("The sprite that should display for this enemy")]
        public Sprite sprite;

        private void OnValidate()
        {
            if (health < 1)
            {
                Debug.LogWarning("Enemy health should be equal to or higher than 1");
                health = 1;
            }

            if (scale < 0)
            {
                Debug.LogWarning("Enemy scale should be equal to or higher than 0");
                scale = 0;
            }

            if (moveSpeed < 0)
            {
                Debug.LogWarning("Enemy movement speed should be equal to or higher than 0");
                moveSpeed = 0;
            }

            for (int i = 0; i < dropOnDeath.Length; i++)
            {
                var drop = dropOnDeath[i];
                if (drop.count < 1)
                {
                    Debug.LogWarning("Enemy drop count should be higher or equal to 1");
                    drop.count = 1;
                    dropOnDeath[i] = drop;
                }
            }
        }
    }

    [Serializable]
    public struct EnemyDrop
    {
        [Tooltip("The data of the enemy to spawn")]
        public SEnemyData data;
        
        [Tooltip("The number of enemies to spawn (>=1)")]
        public int count;

        public EnemyDrop(SEnemyData data)
        {
            this.data = data;
            count = 1;
        }
    }
}
