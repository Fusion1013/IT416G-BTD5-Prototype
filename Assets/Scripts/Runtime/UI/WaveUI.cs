using System;
using Enemy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WaveUI : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TMP_Text waveText;
        [SerializeField] private Button nextWaveButton;

        private int _maxWaves;

        #endregion

        #region Init

        private void Start()
        {
            _maxWaves = WaveManager.Instance.waveContainer.waves.Length;
            SetWaveText(-1, _maxWaves);
        }

        private void OnEnable()
        {
            WaveManager.OnWaveStart += WaveStart;
            WaveManager.OnWaveDone += WaveEnd;
        }

        private void OnDisable()
        {
            WaveManager.OnWaveStart -= WaveStart;
            WaveManager.OnWaveDone -= WaveEnd;
        }

        #endregion

        #region Wave Events

        private void WaveStart(int wave, int maxWaves)
        {
            SetWaveText(wave, maxWaves);
            nextWaveButton.interactable = false;
        }

        private void WaveEnd(int wave, int maxWaves)
        {
            if (wave < maxWaves - 1) nextWaveButton.interactable = true;
        }

        private void SetWaveText(int wave, int maxWaves) => waveText.text = $"Wave: {wave+1} / {maxWaves}";

        #endregion
    }
}
