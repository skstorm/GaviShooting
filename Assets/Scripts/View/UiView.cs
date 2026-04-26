using UnityEngine;
using UnityEngine.UI;
using GaviShooting.Core;

namespace GaviShooting.View
{
    public class UiView : MonoBehaviour
    {
        [SerializeField] private Image _hpBar;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _weaponLevelText;
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _stageClearPanel;
        [SerializeField] private GameObject _pausePanel;

        public void UpdateHp(int current, int max)
        {
            if (_hpBar == null) return;
            float ratio = (float)current / max;
            _hpBar.fillAmount = ratio;
            _hpBar.color = ratio > 0.5f ? Color.green : ratio > 0.25f ? Color.yellow : Color.red;
        }

        public void UpdateScore(int score)
        {
            if (_scoreText != null) _scoreText.text = $"SCORE: {score}";
        }

        public void UpdateWeaponLevel(int level)
        {
            if (_weaponLevelText != null) _weaponLevelText.text = $"WEAPON LV.{level}";
        }

        public void ShowReady()
        {
            Log.Info("UiView.ShowReady");
            setPanel(_readyPanel);
        }

        public void ShowGameOver()
        {
            Log.Info("UiView.ShowGameOver");
            setPanel(_gameOverPanel);
        }

        public void ShowStageClear()
        {
            Log.Info("UiView.ShowStageClear");
            setPanel(_stageClearPanel);
        }

        public void ShowPause()
        {
            Log.Info("UiView.ShowPause");
            setPanel(_pausePanel);
        }

        public void HideAll()
        {
            setPanel(null);
        }

        private void setPanel(GameObject target)
        {
            if (_readyPanel != null) _readyPanel.SetActive(_readyPanel == target);
            if (_gameOverPanel != null) _gameOverPanel.SetActive(_gameOverPanel == target);
            if (_stageClearPanel != null) _stageClearPanel.SetActive(_stageClearPanel == target);
            if (_pausePanel != null) _pausePanel.SetActive(_pausePanel == target);
        }
    }
}
