using System;
using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(fileName = "New Resource File", menuName = "Tower Defense/Resource Data")]
    public class SResources : ScriptableObject
    {
        [Header("Health")]
        public int startHealth = 1;
        
        [Header("Coins")]
        public int startCoins = 0;
        public int coinsPerRound = 0;

        private void OnValidate()
        {
            if (startHealth <= 0)
            {
                Debug.LogWarning($"Start Health should be higher than 0", this);
                startHealth = 1;
            }
            
            if (startCoins < 0)
            {
                Debug.LogWarning($"Start Coins should be equal to or higher than 0", this);
                startCoins = 0;
            }
            
            if (coinsPerRound < 0)
            {
                Debug.LogWarning($"Coins Per Round should be equal to or higher than 0", this);
                coinsPerRound = 0;
            }
        }
    }
}
