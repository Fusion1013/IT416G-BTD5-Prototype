using System;
using Enemy;
using Towers;
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

        [SerializeField] private int coinsPerRound = 150;

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

        public bool TryBuyTower(TowerData data)
        {
            if (_currentCoins < data.cost) return false;
            AddCoins(-data.cost);
            return true;
        }

        private void OnEnemyKilled(EnemyBrain enemyBrain)
        {
            AddCoins(1);
        }

        private void OnEnemyExit(EnemyBrain enemyBrain)
        {
            AddHealth(-1);
        }

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

        private void OnRoundFinish(int round, int maxRounds) => AddCoins(coinsPerRound);
    }
}
