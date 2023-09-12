using System;
using Enemy;
using Resources;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text titleText;

        private void OnEnable()
        {
            WaveManager.OnWaveDone += WaveDone;
            ResourceManager.OnHealthChange += HealthChange;
        }

        private void OnDisable()
        {
            WaveManager.OnWaveDone -= WaveDone;
            ResourceManager.OnHealthChange -= HealthChange;
        }

        private void HealthChange(int newHealth)
        {
            if (newHealth <= 0) GameOver(true);
        }

        private void WaveDone(int wave, int maxWaves)
        {
            if (wave == maxWaves - 1) GameOver(false);
        }

        private void GameOver(bool playerDied)
        {
            gameOverPanel.SetActive(true);
            
            if (playerDied)
            {
                titleText.text = "Game Over!";
            }
            else
            {
                titleText.text = "Game Finished!";
            }
        }

        public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
