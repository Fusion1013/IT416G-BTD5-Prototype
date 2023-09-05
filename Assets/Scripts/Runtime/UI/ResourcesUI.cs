using System;
using Game;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResourcesUI : MonoBehaviour
    {
        // Coins
        public TMP_Text coinText;
        private int _currentCoin;
        private int _currentCoinDisplay;
        
        // Health
        public TMP_Text healthText;
        private int _currentHealth;
        private int _currentHealthDisplay;

        private void OnEnable()
        {
            ResourceManager.OnCoinsChange += OnCoinsChange;
            ResourceManager.OnHealthChange += OnHealthChange;
        }

        private void OnDisable()
        {
            ResourceManager.OnCoinsChange -= OnCoinsChange;
            ResourceManager.OnHealthChange -= OnHealthChange;
        }

        private void Update()
        {
            // Lerp values towards the actual value
            if (_currentHealth != _currentHealthDisplay)
            {
                _currentHealthDisplay = LerpValue(_currentHealthDisplay, _currentHealth);
                healthText.text = $"Health: {_currentHealthDisplay}";
            }
            
            if (_currentCoin != _currentCoinDisplay)
            {
                _currentCoinDisplay = LerpValue(_currentCoinDisplay, _currentCoin);
                coinText.text = $"Coins: {_currentCoinDisplay}";
            }
        }

        private int LerpValue(int current, int target)
        {
            if (current > target) return current - 1;
            if (current < target) return current + 1;
            return current;
            
            // TODO
        }

        private void OnCoinsChange(int newCoins) => _currentCoin = newCoins;

        private void OnHealthChange(int newHealth) => _currentHealth = newHealth;
    }
}
