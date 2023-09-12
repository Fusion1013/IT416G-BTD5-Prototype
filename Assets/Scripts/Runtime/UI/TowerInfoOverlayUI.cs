using System;
using System.Collections.Generic;
using System.Linq;
using Towers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class TowerInfoOverlayUI : MonoBehaviour
    {
        #region Fields

        [SerializeField] 
        [Tooltip("The distance from the mouse that towers will be detected")]
        private float mouseDetectionRadius = .1f;
        
        private Camera _mainCamera;
        private TowerBrain _hoveredTower;
        private TowerBrain _heldTower;

        #endregion

        #region Init

        private void Awake() => _mainCamera = Camera.main;

        private void OnEnable()
        {
            TowerManager.OnTowerStartPlace += TowerStartPlace;
            TowerManager.OnTowerFinishPlace += TowerFinishPlace;
        }

        private void OnDisable()
        {
            TowerManager.OnTowerStartPlace -= TowerStartPlace;
            TowerManager.OnTowerFinishPlace -= TowerFinishPlace;
        }

        #endregion

        private void Update()
        {
            FindHoveredTowers();
            if (_heldTower == null) return;
            if (!TowerManager.CanPlace(_heldTower.transform.position, _heldTower.baseSize)) _heldTower.SetRangeIndicatorColor(new Color(1f, 0f, 0f, 0.5f));
            else _heldTower.SetRangeIndicatorColor(new Color(1f, 1f, 1f, 0.5f));
        }

        #region Hover

        private void FindHoveredTowers()
        {
            var towerColliders = new Collider2D[1];
            var mousePos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var size = Physics2D.OverlapCircleNonAlloc(mousePos, mouseDetectionRadius, towerColliders, LayerMask.GetMask("Tower"));
            var col = towerColliders[0];

            if (size <= 0) DisableHoveredTowerInfo();
            else if (_hoveredTower == null) SetHoveredTower(col);
            else if (_hoveredTower.GetInstanceID() != col.GetInstanceID()) SetHoveredTower(col);
            else DisableHoveredTowerInfo();
        }

        private void SetHoveredTower(Collider2D tower)
        {
            if (_hoveredTower != null) _hoveredTower.ShowTowerInfo(false);
            _hoveredTower = tower.GetComponent<TowerBrain>();
            _hoveredTower.ShowTowerInfo(true);
        }

        private void DisableHoveredTowerInfo()
        {
            if (_hoveredTower == null) return;
            _hoveredTower.ShowTowerInfo(false);
            _hoveredTower = null;
        }

        #endregion

        #region Placement

        private void TowerStartPlace(TowerBrain tower)
        {
            _heldTower = tower;
            DisplayInfoOverlay(tower, true);
        }

        private void TowerFinishPlace(TowerBrain tower)
        {
            _heldTower = null;
            DisplayInfoOverlay(tower, false);
        }

        private static void DisplayInfoOverlay(TowerBrain tower, bool display) => tower.ShowTowerInfo(display);

        #endregion
    }
}
