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

        [SerializeField] private Vector2 bounds;
        public Vector2 Bounds => bounds;

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
            InputManager.OnGameplayCancel += CancelTowerPlacement;
        }

        private void OnDisable()
        {
            InputManager.OnGameplayInteract -= PlaceHeldTower;
            InputManager.OnGameplayCancel -= CancelTowerPlacement;
        }

        #endregion

        #region Tower Placement

        private void Update()
        {
            MoveHeldTower();
            if (_heldTower == null) return;
            if (!CanPlace()) _heldTower.SetRangeIndicatorColor(new Color(1f, 0f, 0f, 0.5f));
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

        private bool CanPlace()
        {
            var towerColliders = Physics2D.OverlapCircleAll(_heldTower.transform.position, _heldTower.baseSize, LayerMask.GetMask("Tower"));
            if (towerColliders.Length > 1) return false;

            var pathCollider = Physics2D.OverlapCircleAll(_heldTower.transform.position, .01f, LayerMask.GetMask("Path"));
            return pathCollider.Length <= 0;
        }

        private void CancelTowerPlacement()
        {
            if (_heldTower == null) return;
            
            Destroy(_heldTower.gameObject);
            _heldTower = null;
            _heldTowerData = null;
        }

        private void PlaceHeldTower()
        {
            // Check if a tower can be placed
            if (_heldTower == null) return;
            if (!CanPlace()) return;

            // Try to buy the tower
            if (!ResourceManager.Instance.TryBuyTower(_heldTowerData))
            {
                Destroy(_heldTower.gameObject);
                _heldTower = null;
                _heldTowerData = null;
                return;
            }
            
            _heldTower.isActive = true;
            _heldTower.ShowRangeIndicator(false);
            _heldTower = null;
        }

        #endregion
    }
}
