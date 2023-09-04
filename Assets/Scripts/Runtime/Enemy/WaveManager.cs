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

        /// <summary>
        /// Called when a wave is done,
        /// either by the player defeating all the enemies,
        /// or all the enemies making it to the goal.
        /// </summary>
        public static event WaveEvent OnWaveDone;
        [SerializeField] private UnityEvent<int> onWaveDone;

        public static event WaveEvent OnWaveStart;
        [SerializeField] private UnityEvent<int> onWaveStart;

        /// <summary>
        /// <param name="wave">The wave this action is relevant to</param>
        /// </summary>
        public delegate void WaveEvent(int wave);

        public Path enemyPath;

        public SWaveContainer waveContainer;
        private int _currentWave = 0;
        private bool _waveInProgress = false;

        #endregion

        #region Enable / Disable

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
            if (enemiesRemaining == 0)
            {
                _waveInProgress = false;
                OnWaveDone?.Invoke(_currentWave);
                onWaveDone?.Invoke(_currentWave);
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
            OnWaveStart?.Invoke(_currentWave);
            onWaveStart?.Invoke(_currentWave);

            _waveInProgress = true;
            return true;
        }

        private void SpawnWave()
        {
            var wave = waveContainer.waves[_currentWave];

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
            foreach (var enemy in group.enemies)
            {
                for (var i = 0; i < group.amount; i++)
                {
                    EnemyManager.Instance.SpawnEnemy(enemy, enemyPath, spawnPosition);
                    yield return new WaitForSeconds(group.timeGap);
                }
            }
        }

        #endregion
    }
}
