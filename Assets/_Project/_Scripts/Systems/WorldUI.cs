using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MJM.HG
{
    public class WorldUI : MonoBehaviour
    {
        [SerializeField] private Slider _speedSlider;
        [Space]
        [SerializeField] private TextMeshProUGUI _speedSliderValueText;
        [SerializeField] private TextMeshProUGUI _speedSliderMinText;
        [SerializeField] private TextMeshProUGUI _speedSliderMaxText;
        [Space]
        [SerializeField] private Toggle _pauseToggle;
       
        private string _speedLabel1 = "Speed = ";
        private string _speedLabel2 = "s";

        private void Start()
        {
            if (_speedSlider == null)
            {
                Debug.Log($"Speed slider not linked to UI script {this}");
            }

            SetInitialSliderTexts();
        }

        public void SpeedSliderChanged()
        {
            Debug.Log("SpeedSliderChanged");
            
            _speedSlider.value = Mathf.Round((_speedSlider.value ) * 5) / 5;
            _speedSliderValueText.text = _speedLabel1 + _speedSlider.value + _speedLabel2;

            TimeManager.Instance.UpdateWorldSpeedRequest(_speedSlider.value);
        }

        private void SetInitialSliderTexts()
        {
            _speedSliderMinText.text = _speedSlider.minValue.ToString();
            _speedSliderMaxText.text = _speedSlider.maxValue.ToString();

            _speedSliderValueText.text = _speedLabel1 + _speedSlider.value + _speedLabel2;
        }

        public void StepClick()
        {
            //Debug.Log("Step click");

            TimeManager.Instance.WorldStepRequest();
        }

        // This is the method called when the pause toggle changes
        public void PauseToggle()
        {           
            TimeManager.Instance.PauseWorldRequest(_pauseToggle.isOn);
        }

        // This is the method for user input key press changing the toggle
        // Unsure if there is 
        public void SetPauseToggle(bool paused)
        {
            _pauseToggle.isOn = !_pauseToggle.isOn;           
        }
    }
}
