using Game;
using TMPro;
using Towers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TowerUIInstance : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image icon;

        public TowerData data;

        #endregion

        private void Start()
        {
            titleText.text = data.towerName;
            costText.text = $"${data.cost}";
            icon.sprite = data.shopIcon;
            
            UpdateCostColor(ResourceManager.Instance.Coins);
        }

        #region Enable / Disable

        private void OnEnable()
        {
            ResourceManager.OnCoinsChange += UpdateCostColor;
        }

        private void OnDisable()
        {
            ResourceManager.OnCoinsChange -= UpdateCostColor;
        }

        #endregion

        private void UpdateCostColor(int coins) => costText.color = coins < data.cost ? Color.red : Color.green;
    }
}
