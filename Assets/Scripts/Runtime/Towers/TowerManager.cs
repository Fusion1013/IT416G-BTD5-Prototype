using System;
using UnityEngine;

namespace Towers
{
    public class TowerManager : MonoBehaviour
    {
        #region Fields

        public static TowerManager Instance { get; private set; }
        public TowerData[] towers;

        #endregion

        #region Init

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Debug.LogError($"Multiple instances of {this} found in scene", this);
        }

        #endregion
    }
}
