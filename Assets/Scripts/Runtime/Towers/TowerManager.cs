using System;
using Game;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Towers
{
    public class TowerManager : MonoBehaviour
    {
        #region Fields

        private Camera _mainCamera;
        
        public static TowerManager Instance { get; private set; }
        public TowerData[] towers;

        private TowerBrain _heldTower; // The tower that the player is currently trying to place
        private TowerData _heldTowerData;

        private TowerBrain _hoveredTower;

        #endregion

        #region Init

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogError($"Multiple instances of {this} found in scene", this);
            
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            InputManager.OnGameplayInteract += PlaceHeldTower;
        }

        private void OnDisable()
        {
            InputManager.OnGameplayInteract -= PlaceHeldTower;
        }

        #endregion

        #region Tower Placement

        private void Update()
        {
            MoveHeldTower();
            if (_heldTower == null) return;
            var transform1 = _heldTower.transform;
            var colliders = Physics2D.OverlapCircleAll(transform1.position, _heldTower.baseSize);
            if (colliders.Length > 1) _heldTower.SetRangeIndicatorColor(new Color(1f, 0f, 0f, 0.5f));
            else _heldTower.SetRangeIndicatorColor(new Color(1f, 1f, 1f, 0.5f));
        }

        private void MoveHeldTower()
        {
            if (_heldTower == null) return;
            
            var mousePos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _heldTower.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }

        public void BuyTower(TowerData data) // TODO: Should only take money when trying to place the tower on the field
        {
            if (ResourceManager.Instance.Coins < data.cost) return;
            if (_heldTower != null) return;
            
            // Instantiate tower placement placeholder
            var brain = Instantiate(data.towerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            brain.isActive = false;
            brain.ShowRangeIndicator(true);
            _heldTower = brain;
            _heldTowerData = data;
        }

        private void PlaceHeldTower()
        {
            if (_heldTower == null) return;
            
            // Check if the tower is overlapping with anything
            var colliders = Physics2D.OverlapCircleAll(_heldTower.transform.position, _heldTower.baseSize);
            if (colliders.Length > 1) return;
            
            // Try to buy the tower
            if (!ResourceManager.Instance.TryBuyTower(_heldTowerData))
            {
                Destroy(_heldTower);
                _heldTower = null;
                _heldTowerData = null;
                return;
            }
            
            Debug.Log("Place tower");
            _heldTower.isActive = true;
            _heldTower.ShowRangeIndicator(false);
            _heldTower = null;
        }

        #endregion
    }
}
