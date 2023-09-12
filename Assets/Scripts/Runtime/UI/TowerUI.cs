using System;
using System.Collections.Generic;
using Towers;
using UnityEngine;

namespace UI
{
    public class TowerUI : MonoBehaviour
    {
        [SerializeField] private RectTransform towerUIHolder;
        [SerializeField] private float towerUISpacing;
        [SerializeField] private TowerUIInstance towerUIInstancePrefab;

        private List<TowerData> _towerDatas = new();
        private List<TowerUIInstance> _uiInstances = new();

        #region Enable / Disable

        private void OnEnable()
        {
            TowerManager.OnTowerAdd += AddTower;
            TowerManager.OnTowerRemove += RemoveTower;
        }

        private void OnDisable()
        {
            TowerManager.OnTowerAdd -= AddTower;
            TowerManager.OnTowerRemove -= RemoveTower;
        }

        #endregion

        private void AddTower(TowerData data)
        {
            _towerDatas.Add(data);
            ClearMenu();
            PopulateMenu();
        }

        private void RemoveTower(TowerData data)
        {
            _towerDatas.Remove(data);
            ClearMenu();
            PopulateMenu();
        }

        private void ClearMenu()
        {
            foreach (var instance in _uiInstances)
            {
                Destroy(instance.gameObject);
            }
            _uiInstances.Clear();
        }

        private void PopulateMenu()
        {
            var towerCount = _towerDatas.Count;
            
            // Instantiate all tower shop UI's
            foreach (var tower in _towerDatas)
            {
                var towerUIInstance = Instantiate(towerUIInstancePrefab, towerUIHolder);
                towerUIInstance.data = tower;
                _uiInstances.Add(towerUIInstance);
            }
            
            // Scale the background
            var instanceHeight = towerUIInstancePrefab.GetComponent<RectTransform>().rect.height;
            towerUIHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, towerUISpacing * (towerCount + 1) + instanceHeight * towerCount);
        }
    }
}
