using System;
using System.Collections.Generic;
using System.Linq;
using Towers;
using Towers.Projectile;
using UnityEngine;
using UnityEngine.Events;
using Debug = Core.Debug.Debug;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        #region Fields

        public static EnemyManager Instance { get; private set; }

        public static event Action<EnemyBrain, int> OnEnemyDeath;
        public UnityEvent<EnemyBrain, int> onEnemyDeath;

        public EnemyBrain enemyBrainPrefab;
        private readonly Dictionary<int, EnemyBrain> _enemies = new();

        #endregion

        #region Init

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogFatal($"Multiple instances of {this} are present in the scene", this);
        }

        #endregion

        #region Enable / Disable

        private void OnEnable()
        {
            EnemyBrain.OnEnemyKilled += EnemyDeath;
            EnemyBrain.OnEnemyPathComplete += EnemyDeath;
            Projectile.OnEnemyCollision += OnEnemyProjectileCollision;
        }

        private void OnDisable()
        {
            EnemyBrain.OnEnemyKilled -= EnemyDeath;
            EnemyBrain.OnEnemyPathComplete -= EnemyDeath;
            Projectile.OnEnemyCollision -= OnEnemyProjectileCollision;
        }

        #endregion

        #region Enemy Spawning

        public EnemyBrain SpawnEnemy(SEnemyData data, Path path, Vector3 spawnPosition) =>
            SpawnEnemy(data, path, spawnPosition, Quaternion.identity);
        
        public EnemyBrain SpawnEnemy(SEnemyData data, Path path, Vector3 spawnPosition, Quaternion rotation)
        {
            EnemyBrain enemyBrain = Instantiate(enemyBrainPrefab, spawnPosition, rotation, transform);
            enemyBrain.data = data;
            enemyBrain.PathFollower.path = path;
            _enemies.Add(enemyBrain.GetInstanceID(), enemyBrain);
            return enemyBrain;
        }

        private void SpawnEnemyDrops(EnemyBrain enemyBrain)
        {
            if (enemyBrain.data.dropOnDeath == null) return;

            foreach (var drops in enemyBrain.data.dropOnDeath)
            {
                if (drops.data == null) continue;

                for (int i = 0; i < drops.count; i++)
                {
                    var enemyTransform = enemyBrain.transform;
                    var newEnemy = SpawnEnemy(drops.data, enemyBrain.PathFollower.path, enemyTransform.position, enemyTransform.rotation);
                    newEnemy.PathFollower.TargetWaypointId = enemyBrain.PathFollower.TargetWaypointId;
                }
            }
        }

        #endregion

        #region Enemy Finding

        public bool IsEnemyInRange(Vector3 position, float maxRange) => _enemies.Values.Any(enemy => Vector3.Distance(position, enemy.transform.position) <= maxRange);

        public EnemyBrain FindEnemy(TargetingPriority targetingPriority, Vector3 position, float maxRange)
        {
            return targetingPriority switch
            {
                TargetingPriority.First => FindFirstEnemy(position, maxRange),
                TargetingPriority.Last => FindLastEnemy(position, maxRange),
                TargetingPriority.Close => FindCloseEnemy(position, maxRange),
                TargetingPriority.Strong => FindStrongEnemy(position, maxRange),
                _ => null
            };
        }
        
        /// <summary>
        /// Finds the enemy that is the closest to the exit.
        /// </summary>
        public EnemyBrain FindFirstEnemy(Vector3 position, float maxRange = 0) // TODO: Check enemy for properties
        {
            EnemyBrain firstEnemyBrain = null;
            
            foreach (var enemy in _enemies.Values.Where(enemy => !(Vector3.Distance(position, enemy.transform.position) > maxRange)))
            {
                if (firstEnemyBrain == null) firstEnemyBrain = enemy;
                else if (enemy.PathFollower.TargetWaypointId > firstEnemyBrain.PathFollower.TargetWaypointId) firstEnemyBrain = enemy;
                else if (enemy.PathFollower.TargetWaypointId == firstEnemyBrain.PathFollower.TargetWaypointId &&
                         enemy.PathFollower.DistanceFromWaypoint < firstEnemyBrain.PathFollower.DistanceFromWaypoint) firstEnemyBrain = enemy;
            }

            return firstEnemyBrain;
        }

        public EnemyBrain FindLastEnemy(Vector3 position, float maxRange = 0)
        {
            EnemyBrain lastEnemyBrain = null;
            
            foreach (var enemy in _enemies.Values.Where(enemy => !(Vector3.Distance(position, enemy.transform.position) > maxRange)))
            {
                if (lastEnemyBrain == null) lastEnemyBrain = enemy;
                else if (enemy.PathFollower.TargetWaypointId < lastEnemyBrain.PathFollower.TargetWaypointId) lastEnemyBrain = enemy;
                else if (enemy.PathFollower.TargetWaypointId == lastEnemyBrain.PathFollower.TargetWaypointId &&
                         enemy.PathFollower.DistanceFromWaypoint > lastEnemyBrain.PathFollower.DistanceFromWaypoint) lastEnemyBrain = enemy;
            }

            return lastEnemyBrain;
        }

        public EnemyBrain FindCloseEnemy(Vector3 position, float maxRange = 0)
        {
            EnemyBrain closeEnemyBrain = null;
            var closeDist = float.MaxValue;

            foreach (var enemy in _enemies.Values.Where(enemy => !(Vector3.Distance(position, enemy.transform.position) > maxRange)))
            {
                var dist = Vector3.Distance(enemy.transform.position, position);
                
                if (!(dist < closeDist)) continue;
                closeEnemyBrain = enemy;
                closeDist = dist;
            }

            return closeEnemyBrain;
        }

        public EnemyBrain FindStrongEnemy(Vector3 position, float maxRange = 0)
        {
            // TODO
            return null;
        }

        #endregion

        #region Enemy Collision

        private void OnEnemyProjectileCollision(GameObject enemyObject)
        {
            var enemy = enemyObject.GetComponent<EnemyBrain>();
            if (enemy == null) return;
            enemy.Kill();
        }

        #endregion
        
        private void EnemyDeath(EnemyBrain enemyBrain)
        {
            if (!_enemies.Remove(enemyBrain.GetInstanceID())) return; // If it could not be removed, it is already dead
            
            SpawnEnemyDrops(enemyBrain);
            OnEnemyDeath?.Invoke(enemyBrain, _enemies.Count);
            onEnemyDeath?.Invoke(enemyBrain, _enemies.Count);
        }
    }
}
