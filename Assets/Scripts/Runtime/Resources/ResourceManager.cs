using System;
using Enemy;
using Towers;
using UnityEngine;

namespace Resources
{
    public class ResourceManager : MonoBehaviour
    {
        // -- Instance
        public static ResourceManager Instance { get; private set; }

        [SerializeField] private SResources resourceData;
        
        // -- Health
        public int Health => _currentHealth;
        private int _currentHealth;
        public static event Action<int> OnHealthChange;
        
        // -- Coins
        public int Coins => _currentCoins;
        private int _currentCoins;
        public static event Action<int> OnCoinsChange;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogError($"Multiple instances of {this} found in scene", this);
            
            // Set start values
            _currentHealth = resourceData.startHealth;
            _currentCoins = resourceData.startCoins;
        }

        private void Start()
        {
            OnCoinsChange?.Invoke(_currentCoins);
            OnHealthChange?.Invoke(_currentHealth);
        }

        private void OnEnable()
        {
            EnemyBrain.OnEnemyKilled += OnEnemyKilled;
            EnemyBrain.OnEnemyPathComplete += OnEnemyExit;
            WaveManager.OnWaveDone += OnRoundFinish;
        }

        private void OnDisable()
        {
            EnemyBrain.OnEnemyKilled -= OnEnemyKilled;
            EnemyBrain.OnEnemyPathComplete -= OnEnemyExit;
            WaveManager.OnWaveDone -= OnRoundFinish;
        }

        public bool TrySpendCoins(int amount)
        {
            if (_currentCoins < amount) return false;
            AddCoins(-amount);
            return true;
        }

        #region Events

        private void OnEnemyKilled(EnemyBrain enemyBrain) => AddCoins(1);
        private void OnEnemyExit(EnemyBrain enemyBrain) => AddHealth(-1);
        private void OnRoundFinish(int round, int maxRounds) => AddCoins(resourceData.coinsPerRound);

        #endregion

        public void AddCoins(int amount)
        {
            _currentCoins += amount;
            OnCoinsChange?.Invoke(_currentCoins);
        }

        public void AddHealth(int amount)
        {
            _currentHealth += amount;
            OnHealthChange?.Invoke(_currentHealth);
        }
    }
}
