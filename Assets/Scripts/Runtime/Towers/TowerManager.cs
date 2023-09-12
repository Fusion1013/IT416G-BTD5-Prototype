using System;
using System.Collections.Generic;
using Input;
using Resources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Towers
{
    public class TowerManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] 
        [Tooltip("The bounds of the map")]
        private Vector2 bounds;
        public Vector2 Bounds => bounds;
        
        [SerializeField]
        [Tooltip("The towers that are unlocked at the start of the game")]
        private List<TowerData> towers;

        public static TowerManager Instance { get; private set; }

        // -- Events
        public static event Action<TowerBrain> OnTowerStartPlace;
        public static event Action<TowerBrain> OnTowerFinishPlace;
        public UnityEvent onTowerFinishPlace;

        public static event Action<TowerData> OnTowerAdd; 
        public static event Action<TowerData> OnTowerRemove; 
        
        // -- Internal
        private Camera _mainCamera;
        private TowerBrain _hoveredTower;
        private TowerBrain _heldTower; // The tower that the player is currently trying to place
        private TowerData _heldTowerData;

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

        private void Start()
        {
            foreach (var tower in towers)
            {
                OnTowerAdd?.Invoke(tower);
            }
        }

        #endregion

        #region Tower Availability Modifying

        public void AddTower(TowerData data)
        {
            if (towers.Contains(data)) return;
            
            towers.Add(data);
            OnTowerAdd?.Invoke(data);
        }

        public void RemoveTower(TowerData data)
        {
            if (!towers.Contains(data)) return;
            
            towers.Remove(data);
            OnTowerRemove?.Invoke(data);
        }

        #endregion

        #region Tower Placement

        private void Update() => MoveHeldTower();

        private void MoveHeldTower()
        {
            if (_heldTower == null) return;
            
            var mousePos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _heldTower.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }

        public void BuyTower(TowerData data)
        {
            if (ResourceManager.Instance.Coins < data.cost) return;
            if (_heldTower != null) return;
            
            // Instantiate tower placement placeholder
            var brain = Instantiate(data.towerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            brain.isActive = false;
            _heldTower = brain;
            _heldTowerData = data;

            OnTowerStartPlace?.Invoke(_heldTower);
        }

        public static bool CanPlace(Vector3 position, float size)
        {
            var towerColliders = Physics2D.OverlapCircleAll(position, size, LayerMask.GetMask("Tower"));
            if (towerColliders.Length > 1) return false;

            var pathCollider = Physics2D.OverlapCircleAll(position, .01f, LayerMask.GetMask("Path"));
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
            if (!CanPlace(_heldTower.transform.position, _heldTower.baseSize)) return;

            // Try to buy the tower
            if (!ResourceManager.Instance.TrySpendCoins(_heldTowerData.cost))
            {
                Destroy(_heldTower.gameObject);
                _heldTower = null;
                _heldTowerData = null;
                return;
            }
                
            OnTowerFinishPlace?.Invoke(_heldTower);
            onTowerFinishPlace?.Invoke();
            
            _heldTower.isActive = true;
            _heldTower = null;
        }

        #endregion
    }
}
