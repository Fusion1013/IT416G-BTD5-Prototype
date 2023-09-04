using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "New Wave Data", menuName = "Tower Defense/Enemies/Wave Data")]
    public class SWaveData : ScriptableObject
    {
        public List<EnemyGroup> groups;
    }

    [Serializable]
    public struct EnemyGroup
    {
        [Tooltip("How long to wait after the wave starts to start spawning the enemy group (s)")]
        public float startTime;
        
        [Tooltip("The time between enemy spawns (s)")]
        public float timeGap;
        
        [Tooltip("Multiplies the number of enemies that are spawned")]
        public int amount;
        public SEnemyData[] enemies;
    }
}
