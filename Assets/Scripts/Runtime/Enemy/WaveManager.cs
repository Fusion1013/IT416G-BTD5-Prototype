using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class WaveManager : MonoBehaviour
    {
        #region Fields
        
        public static WaveManager Instance { get; private set; }

        /// <summary>
        /// Called when a wave is done,
        /// either by the player defeating all the enemies,
        /// or all the enemies making it to the goal.
        /// </summary>
        public static event WaveEvent OnWaveDone;
        [SerializeField] private UnityEvent<int, int> onWaveDone;

        public static event WaveEvent OnWaveStart;
        [SerializeField] private UnityEvent<int, int> onWaveStart;

        /// <summary>
        /// <param name="wave">The wave this action is relevant to</param>
        /// <param name="maxWaves">The number of total waves</param>
        /// </summary>
        public delegate void WaveEvent(int wave, int maxWaves);

        public Path enemyPath;

        public SWaveContainer waveContainer;
        private int _currentWave = 0;
        private bool _isSpawningWave = false;
        private bool _waveInProgress = false;

        #endregion

        #region Init

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogError($"Multiple instances of {this} found in scene", this);
        }

        private void OnEnable()
        {
            EnemyManager.OnEnemyDeath += OnEnemyDeath;
        }

        private void OnDisable()
        {
            EnemyManager.OnEnemyDeath -= OnEnemyDeath;
        }

        #endregion

        /// <summary>
        /// Checks if there are any enemies alive and if there are not,
        /// set the wave state to not active.
        /// </summary>
        /// <param name="enemyBrain">The enemy that just died</param>
        /// <param name="enemiesRemaining">The number of enemies that are remaining</param>
        private void OnEnemyDeath(EnemyBrain enemyBrain, int enemiesRemaining)
        {
            if (enemiesRemaining == 0 && !_isSpawningWave)
            {
                _waveInProgress = false;
                OnWaveDone?.Invoke(_currentWave, waveContainer.waves.Length);
                onWaveDone?.Invoke(_currentWave, waveContainer.waves.Length);
                _currentWave++;
            }
        }

        #region Wave Spawning

        public void SpawnNextWave() => TrySpawnNextWave();
        
        /// <summary>
        /// Attempts to spawn a wave. Fails if another wave is currently in progress.
        /// </summary>
        /// <returns>False if another wave is currently in progress</returns>
        public bool TrySpawnNextWave()
        {
            // Check if a new wave can be spawned
            if (_waveInProgress) return false;
            if (waveContainer.waves.Length <= _currentWave) return false;

            SpawnWave();
            OnWaveStart?.Invoke(_currentWave, waveContainer.waves.Length);
            onWaveStart?.Invoke(_currentWave, waveContainer.waves.Length);

            _waveInProgress = true;
            return true;
        }

        private void SpawnWave()
        {
            var wave = waveContainer.waves[_currentWave];
            _isSpawningWave = true;

            // Spawn the wave
            foreach (var group in wave.groups)
            {
                StartCoroutine(SpawnEnemyGroup(group, enemyPath.Points[0]));
            }
        }

        private IEnumerator SpawnEnemyGroup(EnemyGroup group, Vector3 spawnPosition)
        {
            yield return new WaitForSeconds(group.startTime); // Wait before spawning the wave
            
            // Spawn all enemies in the wave with the specified time interval
            for (int j = 0; j < group.enemies.Length; j++)
            {
                var enemy = group.enemies[j];
                for (var i = 0; i < group.amount; i++)
                {
                    EnemyManager.Instance.SpawnEnemy(enemy, enemyPath, spawnPosition);
                    if (!(j == group.enemies.Length - 1 && i == group.amount - 1)) yield return new WaitForSeconds(group.timeGap);
                }
            }

            _isSpawningWave = false;
        }

        #endregion
    }
}
