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
        [Space]
        [SerializeField] private GameObject _bottomRightPanel;
        [SerializeField] private GameObject _topRightPanel;
        [SerializeField] private GameObject _centerMenuPanel;

        private string _speedLabel1 = "Speed = ";
        private string _speedLabel2 = "s";

        private bool _pausedBeforeEscapeUsed;

        private void Start()
        {
            if (_speedSlider == null)
            {
                Debug.Log($"Speed slider not linked to UI script {this}");
            }

            SetInitialSliderTexts();

            //Ensure center menu panel is hidden
            _centerMenuPanel.SetActive(false);
        }

        public void SpeedSliderChanged()
        {            
            _speedSlider.value = Mathf.Round((_speedSlider.value) * 5) / 5;
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
            if (_bottomRightPanel.activeSelf) //required to skip key press if UI is inactive
            {
                TimeManager.Instance.WorldStepRequest();
            }
        }

        // This is the method called when the pause toggle changes
        public void PauseToggle()
        {
            if (_bottomRightPanel.activeSelf) //required to skip key press if UI is inactive
            {
                TimeManager.Instance.PauseWorldRequest(_pauseToggle.isOn);
            }
        }

        // This is the method for user input key press changing the toggle
        // Unsure if there is 
        public void SetPauseToggle(bool paused)
        {
            _pauseToggle.isOn = !_pauseToggle.isOn;
        }

        public void EscClick()
        {           
            // Store pause state before escape
            _pausedBeforeEscapeUsed = _pauseToggle.isOn;
            // If not paused pause now
            if (!_pauseToggle.isOn)
                _pauseToggle.isOn = true;

            _bottomRightPanel.SetActive(false);
            _topRightPanel.SetActive(false);

            _centerMenuPanel.SetActive(true);

            CameraManager.Instance.EnableCameraControls(false);
        }

        // Required because escape key interacts with 2 different buttons depending on whether escapre menu is open
        public void EscKeyPress()
        {
            if (_centerMenuPanel.activeSelf)
                ResumeClick();
            else
                EscClick();
        }

        public void ResumeClick()
        {
            _centerMenuPanel.SetActive(false);

            _bottomRightPanel.SetActive(true);
            _topRightPanel.SetActive(true);

            CameraManager.Instance.EnableCameraControls(true);

            // Restore pause state prior to escape menu opening.
            // Needs to be after the set active calls uo toggle onchangevalue will not execute
            if (!_pausedBeforeEscapeUsed)
                _pauseToggle.isOn = false;
        }

        public void ExitToMenuClick()
        {
            GameManager.Instance.HandleExitToMenuRequest();
        }

        public void ExitGameClick()
        {
            GameManager.Instance.HandleQuitGameRequest();
        }
    }
}
