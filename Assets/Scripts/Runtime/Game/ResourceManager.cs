using System;
using UnityEngine;

namespace Game
{
    public class ResourceManager : MonoBehaviour
    {
        // -- Instance
        public static ResourceManager Instance { get; private set; }
        
        // -- Health
        public int maxHealth = 200;
        public int Health => _currentHealth;
        private int _currentHealth;
        public static event Action<int> OnHealthChange;
        
        // -- Coins
        public int startCoins = 200;
        public int Coins => _currentCoins;
        private int _currentCoins;
        public static event Action<int> OnCoinsChange;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogError($"Multiple instances of {this} found in scene", this);
            
            // Set start values
            _currentHealth = maxHealth;
            _currentCoins = startCoins;
        }

        private void Start()
        {
            OnCoinsChange?.Invoke(_currentCoins);
            OnHealthChange?.Invoke(_currentHealth);
        }

        private void OnEnable()
        {
            Enemy.EnemyBrain.OnEnemyKilled += OnEnemyKilled;
            Enemy.EnemyBrain.OnEnemyPathComplete += OnEnemyExit;
        }

        private void OnDisable()
        {
            Enemy.EnemyBrain.OnEnemyKilled -= OnEnemyKilled;
            Enemy.EnemyBrain.OnEnemyPathComplete -= OnEnemyExit;
        }

        private void OnEnemyKilled(Enemy.EnemyBrain enemyBrain)
        {
            _currentCoins++;
            OnCoinsChange?.Invoke(_currentCoins);
        }

        private void OnEnemyExit(Enemy.EnemyBrain enemyBrain)
        {
            _currentHealth--;
            OnHealthChange?.Invoke(_currentHealth);
        }
    }
}
