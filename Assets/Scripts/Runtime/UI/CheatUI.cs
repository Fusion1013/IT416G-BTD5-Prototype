using Game;
using Input;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CheatUI : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject cheatUIObject;
        [SerializeField] private TMP_InputField coinField;
        [SerializeField] private TMP_InputField healthField;

        #endregion

        #region Enable / Disable

        private void OnEnable() => InputManager.OnUIOpenCheats += ToggleUI;
        private void OnDisable() => InputManager.OnUIOpenCheats -= ToggleUI;

        #endregion

        private void ToggleUI() => cheatUIObject.SetActive(!cheatUIObject.activeSelf);
        public void AddCoins() => ResourceManager.Instance.AddCoins(int.Parse(coinField.text));
        public void AddHealth() => ResourceManager.Instance.AddHealth(int.Parse(healthField.text));
    }
}
