using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MJM.HG
{
    public struct PlayerRange { int min; int max; }

    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Slider _worldSizeSlider;
        [SerializeField] private Slider _playerSlider;
        [Space]
        [SerializeField] private TextMeshProUGUI _worldSliderValueText;
        [SerializeField] private TextMeshProUGUI _worldSliderMinText;
        [SerializeField] private TextMeshProUGUI _worldSliderMaxText;
        [Space]
        [SerializeField] private TextMeshProUGUI _playerSliderValueText;
        [SerializeField] private TextMeshProUGUI _playerSliderMinText;
        [SerializeField] private TextMeshProUGUI _playerSliderMaxText;

        //private WorldSystem _worldSystem; 
        
        private string _worldSizeLabel = "World Size = ";
        private string _playerLabel = "Players = ";

        private PlayerRange _playerRange;
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

            //_worldSystem = GameManager.Instance.WorldSystem;

            SetInitialSliderTexts();

            _worldSizeSlider.onValueChanged.AddListener(delegate { WorldSliderChanged(); });
            _playerSlider.onValueChanged.AddListener(delegate { PlayerSliderChanged(); });
        }

        private void WorldSliderChanged()
        {
            _worldSizeSlider.value = Mathf.Round(_worldSizeSlider.value);
            _worldSliderValueText.text = _worldSizeLabel + _worldSizeSlider.value;

            _playerSlider.maxValue = PlayerSystem.CalcMaxPlayers((int)_worldSizeSlider.value);
            _playerSliderMaxText.text = _playerSlider.maxValue.ToString();
        }

        private void PlayerSliderChanged()
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
            
            GameManager.Instance.HandleNewGameRequest((int)_worldSizeSlider.value, (int)_playerSlider.value);      
        }

        public void QuitGameClick()
        {
            Debug.Log("Quit game click");

            GameManager.Instance.HandleQuitGameRequest();

        }
    }
}
