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
        private float _coinLerp;
        
        // Health
        public TMP_Text healthText;
        private int _currentHealth;
        private int _currentHealthDisplay;
        private float _healthLerp;

        [SerializeField] private float lerpDuration;
        [SerializeField] private int cutoffValue = 10;

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
                _healthLerp += Time.deltaTime / lerpDuration;
                _currentHealthDisplay = Mathf.Max(0, LerpValue(_healthLerp, _currentHealthDisplay, _currentHealth));
                healthText.text = $"Health: {_currentHealthDisplay}";
            }
            
            if (_currentCoin != _currentCoinDisplay)
            {
                _coinLerp += Time.deltaTime / lerpDuration;
                _currentCoinDisplay = Mathf.Max(0, LerpValue(_coinLerp, _currentCoinDisplay, _currentCoin));
                coinText.text = $"Coins: {_currentCoinDisplay}";
            }
        }

        private int LerpValue(float lerp, int current, int target)
        {
            if (Mathf.Abs(target - current) < cutoffValue) return target;
            return (int)Mathf.Lerp(current, target, lerp);
        }

        private void OnCoinsChange(int newCoins)
        {
            if (_currentCoin == newCoins) return;
            _currentCoin = newCoins;
            _coinLerp = 0;
        }

        private void OnHealthChange(int newHealth)
        {
            if (_currentHealth == newHealth) return;
            _currentHealth = newHealth;
            _healthLerp = 0;
        }
    }
}
