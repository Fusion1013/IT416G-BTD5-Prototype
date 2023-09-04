using TMPro;
using UnityEngine;

namespace UI
{
    public class ValueTextUI : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The text object to change the text of")]
        private TMP_Text text;

        [SerializeField] 
        [Tooltip("The name of the value")]
        private string textName;

        [Header("Modifiers")] 
        [SerializeField] private int intModifier;
        [SerializeField] private int floatModifier;

        public void UpdateText(int value) => text.text = $"{textName}: {value + intModifier}";
        public void UpdateText(float value) => text.text = $"{textName}: {value + floatModifier}";
        public void UpdateText(string value) => text.text = $"{textName}: {value}";
        public void UpdateText(bool value) => text.text = $"{textName}: {value}";
    }
}
