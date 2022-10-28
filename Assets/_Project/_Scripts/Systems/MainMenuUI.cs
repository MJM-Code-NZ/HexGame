using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MJM.HG
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Slider _worldSizeSlider;
        public Slider WorldSizeSlider {  get { return _worldSizeSlider; } }
        [SerializeField] private Slider _playerSlider;
        public Slider PlayerSlider { get { return _playerSlider; } }
        [Space]
        [SerializeField] private TextMeshProUGUI _worldSliderValueText;
        [SerializeField] private TextMeshProUGUI _worldSliderMinText;
        [SerializeField] private TextMeshProUGUI _worldSliderMaxText;
        [Space]
        [SerializeField] private TextMeshProUGUI _playerSliderValueText;
        [SerializeField] private TextMeshProUGUI _playerSliderMinText;
        [SerializeField] private TextMeshProUGUI _playerSliderMaxText;
        [Space]
        [SerializeField] private Toggle _autoToggle;
        public Toggle AutoToggle { get { return _autoToggle; } }


        private string _worldSizeLabel = "World Size = ";
        private string _playerLabel = "Players = ";

        private void Start()
        {
            if (_worldSizeSlider == null)
            {
                Debug.Log($"World size slider not linked to UI script {this}");
            }

            if (_playerSlider == null)
            {
                Debug.Log($"World size slider not linked to UI script {this}");
            }
           
            SetInitialSliderTexts();           
        }

        public void WorldSliderChanged()
        {
            _worldSizeSlider.value = Mathf.Round((_worldSizeSlider.value -1 ) / 2) * 2 + 1;
            _worldSliderValueText.text = _worldSizeLabel + _worldSizeSlider.value;

            _playerSlider.maxValue = PlayerSystem.CalcMaxPlayers((int)(_worldSizeSlider.value - 1) / 2);
            _playerSliderMaxText.text = _playerSlider.maxValue.ToString();
        }

        public void PlayerSliderChanged()
        {
            _playerSlider.value = Mathf.Round(_playerSlider.value);
            _playerSliderValueText.text = _playerLabel + _playerSlider.value;
        }

        private void SetInitialSliderTexts()
        {
            _worldSliderMinText.text = _worldSizeSlider.minValue.ToString();
            _worldSliderMaxText.text = _worldSizeSlider.maxValue.ToString();

            _worldSliderValueText.text = _worldSizeLabel + _worldSizeSlider.value;

            _playerSliderMinText.text = _playerSlider.minValue.ToString();
            _playerSliderMaxText.text = _playerSlider.maxValue.ToString();

            _playerSliderValueText.text = _playerLabel + _playerSlider.value;
        }

        public void NewGameClick()
        {
            // Debug.Log("New game click"); 
            
            GameManager.Instance.HandleNewGameRequest((int)(_worldSizeSlider.value - 1) / 2, (int)_playerSlider.value);      
        }

        public void QuitGameClick()
        {
            // Debug.Log("Quit game click");

            GameManager.Instance.HandleQuitGameRequest();

        }

        public void AutoToggleChange()
        {
            // Debug.Log($"Auto toggle change {_autoToggle.isOn}");
            GameManager.Instance.HandleAutoPlayRequest(_autoToggle.isOn);
        }
    }
}
