using System;
using Towers;
using UnityEngine;

namespace UI
{
    public class TowerUI : MonoBehaviour
    {
        [SerializeField] private Transform towerUIHolder;
        [SerializeField] private TowerUIInstance towerUIInstancePrefab;

        private void Start()
        {
            var tm = TowerManager.Instance;

            foreach (var tower in tm.towers)
            {
                var towerUIInstance = Instantiate(towerUIInstancePrefab, towerUIHolder);
                towerUIInstance.data = tower;
            }
        }
    }
}
