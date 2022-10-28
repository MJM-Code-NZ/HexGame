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
        public Slider SpeedSlider { get { return _speedSlider; } }

        [Space]
        [SerializeField] private TextMeshProUGUI _speedSliderValueText;
        [SerializeField] private TextMeshProUGUI _speedSliderMinText;
        [SerializeField] private TextMeshProUGUI _speedSliderMaxText;
        [Space]
        [SerializeField] private Toggle _pauseToggle;
        [Space]
        [SerializeField] private Button _autoButton;
        [Space]
        [SerializeField] private GameObject _bottomRightPanel;
        [SerializeField] private GameObject _bottomRightPanelAlt;
        [SerializeField] private GameObject _topRightPanel;
        [SerializeField] private GameObject _centerMenuPanel;

        private string _speedLabel1 = "Speed = ";
        private string _speedLabel2 = "s";

        private bool _escapeMenuOpen = false;
        private bool _bottomRightHidden = false;
        private bool _pausedBeforeEscapeUsed;
        
        private void Start()
        {
            if (_speedSlider == null)
            {
                Debug.Log($"Speed slider not linked to UI script {this}");
            }

            SetInitialSliderValues();

            //Ensure center menu panel is hidden
            _centerMenuPanel.SetActive(false);
        }

        public void SpeedSliderChanged()
        {
            _speedSlider.value = Mathf.Round((_speedSlider.value) * 5) / 5;
            _speedSliderValueText.text = _speedLabel1 + _speedSlider.value + _speedLabel2;

            TimeManager.Instance.UpdateWorldSpeedRequest(_speedSlider.value);
        }

        // This is necessary to return the game speed to the default speed when a new game is started.
        // If the speed was not reset the speed from the first game carries over to later games.
        private void SetInitialSliderValues()
        {
            _speedSliderMinText.text = _speedSlider.minValue.ToString();
            _speedSliderMaxText.text = _speedSlider.maxValue.ToString();
            _speedSlider.value = 1.0f;

            SpeedSliderChanged();
        }

        public void HideClick()
        {
            //Debug.Log("Hide click");
            _bottomRightHidden = true;
           
            _bottomRightPanel.SetActive(false);
            _bottomRightPanelAlt.SetActive(true);
        }

        public void ShowClick()
        {
            //Debug.Log("Show click");
            _bottomRightHidden = false;
            
            _bottomRightPanel.SetActive(true);
            _bottomRightPanelAlt.SetActive(false);
        }

        public void StepClick()
        {
            //Debug.Log("Step click");
            if (!_escapeMenuOpen)
            {
                TimeManager.Instance.WorldStepRequest();
            }
        }

        // This is the method called when the pause toggle changes
        public void PauseToggle()
        {
            if (_autoButton.IsActive())
            {
                AutoClick();
            }
            
            TimeManager.Instance.PauseWorldRequest(_pauseToggle.isOn);            
        }

        // This is the method for user input key press changing the toggle
        // Unsure if there is 
        public void PauseKeyPress()
        {
            if (!_escapeMenuOpen)
            {
                _pauseToggle.isOn = !_pauseToggle.isOn;
            }
        }

        public void EscClick()
        {
            if (_autoButton.IsActive())
            {
                AutoClick();
            }
            
            // Store pause state before escape
            _pausedBeforeEscapeUsed = _pauseToggle.isOn;
            // If not paused pause now           
            if (!_pauseToggle.isOn)
                _pauseToggle.isOn = true;

            _escapeMenuOpen = true;

            if (_bottomRightHidden)
            {
                _bottomRightPanelAlt.SetActive(false);               
            }
            else
            {
                _bottomRightPanel.SetActive(false);
            }
            
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
            _escapeMenuOpen = false; 
            
            _centerMenuPanel.SetActive(false);

            if (_bottomRightHidden)
            {
                _bottomRightPanelAlt.SetActive(true);
            }
            else
            {
                _bottomRightPanel.SetActive(true);
            }
           
            _topRightPanel.SetActive(true);

            CameraManager.Instance.EnableCameraControls(true);

            // Restore pause state prior to escape menu opening.
            // Needs to be after the set active calls so toggle onchangevalue will not execute
            if (!_pausedBeforeEscapeUsed)
                _pauseToggle.isOn = false;
        }

        public void AutoShow(bool show)
        {
            _autoButton.gameObject.SetActive(show);
        }

        public void AutoClick()
        {
            AutoShow(false);

            GameManager.Instance.HandleWorldAutoPlayOffRequest();
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
