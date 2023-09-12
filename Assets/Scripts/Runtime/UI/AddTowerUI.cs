using Towers;
using UnityEngine;

namespace UI
{
    public class AddTowerUI : MonoBehaviour
    {
        [SerializeField] private TowerData tower;
        
        public void AddTower() => TowerManager.Instance.AddTower(tower);
    }
}
