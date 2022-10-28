using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MJM.HG
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        //private Camera _camera;
        public Camera Camera { get; private set; }
       
        private InputAction _panAction;
        private InputAction _zoomAction;

        [Header("Camera")]
        [SerializeField] private float PanSpeed = 8f;
        [SerializeField] private float ZoomSpeed = 8f;

        [Space()]
        [SerializeField] private float DefaultZoom = 6f;
        [SerializeField] private Vector3 DefaultPosition = new Vector3(0, 0, -10);
        [Space()]
        [SerializeField] private float MinZoom = 2f;
        [SerializeField] private float MaxZoom = 20f;

        private bool _autoPanOn;
        public bool AutoPanOn { get { return _autoPanOn; } }

        private bool _autoZoomOn;
        public bool AutoZoomOn { get { return _autoZoomOn; } }

        private Vector3 _autoPanTarget;
        private float _autoZoomSize;
        private float _autoDuration;


        void Awake()
        {
            EnforceSingleInstance();

            Camera = Camera.main;
            Reset();     
        }

        private void Start()
        {
            _panAction = GameManager.Instance.PlayerInput.actions.FindAction("Pan");
            _zoomAction = GameManager.Instance.PlayerInput.actions.FindAction("Zoom");
        }

        private void EnforceSingleInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void Reset()
        {
            Camera.transform.position = DefaultPosition;
            Camera.orthographicSize = DefaultZoom;
        }

        public void EnableCameraControls(bool enable)
        {
            if (enable)
            {
                GameManager.Instance.PlayerInput.actions.FindActionMap("Camera").Enable();                
            }
            else
            {
                GameManager.Instance.PlayerInput.actions.FindActionMap("Camera").Disable();               
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePan();
            UpdateZoom();
        }

        private void UpdatePan()
        {           
            Vector2 panValue = _panAction.ReadValue<Vector2>();

            if (panValue != Vector2.zero)
            {
                _autoPanOn = false;

                Vector3 panDisplacement = PanSpeed * panValue;

                Camera.transform.position = Vector3.Lerp(
                    Camera.transform.position,
                    Camera.transform.position + panDisplacement,
                    Time.deltaTime
                    );
            }
            else
            {
                if (_autoPanOn)
                    AutoPan();
            }
        }

        private void UpdateZoom()
        {           
            float zoomValue = _zoomAction.ReadValue<float>();

            if (zoomValue != 0)
            {

                float zoomDisplacement = ZoomSpeed * zoomValue;

                Camera.orthographicSize = Mathf.Lerp(
                    Camera.orthographicSize,
                    Camera.orthographicSize + zoomDisplacement,
                    Time.deltaTime
                    );

                Camera.orthographicSize = Mathf.Clamp(
                    Camera.orthographicSize,
                    MinZoom,
                    MaxZoom
                    );
            }
            else
            {
                if (_autoZoomOn)
                    AutoZoom();
            }
        }

        public void SetAutoPan(Vector3 targetPosition, float duration)
        {
            _autoPanTarget = targetPosition;
            _autoPanTarget.z = Camera.transform.position.z;
            _autoDuration = duration;
            _autoPanOn = true;
        }

        public void SetAutoZoom(float targetSize, float duration)
        {
            _autoZoomSize = targetSize;
            _autoDuration = duration;
            _autoZoomOn = true;
        }

        public void DisableAuto()
        {
            _autoPanOn = false;
            _autoZoomOn = false;
        }

        private void AutoPan()
        {
            float _proportion;

            if (Time.deltaTime < _autoDuration)
            {
                _proportion = Time.deltaTime / _autoDuration;
                _autoDuration -= Time.deltaTime;
            }
            else
            {
                _proportion = 1;
                _autoPanOn = false;
            }

            Camera.transform.position = Vector3.Lerp(
                Camera.transform.position,                      
                _autoPanTarget,
                _proportion
                );
        }

        private void AutoZoom()
        {
            //float zoomDisplacement = ZoomSpeed * zoomValue;

            float _proportion;

            if (Time.deltaTime < _autoDuration)
            {
                _proportion = Time.deltaTime / _autoDuration;
                _autoDuration -= Time.deltaTime;
            }
            else
            {
                _proportion = 1;
                _autoZoomOn = false;
            }

            Camera.orthographicSize = Mathf.Lerp(
                Camera.orthographicSize,
                _autoZoomSize,
                _proportion
                );

            Camera.orthographicSize = Mathf.Clamp(
                Camera.orthographicSize,
                MinZoom,
                MaxZoom
                );
        }

            private void OnValidate()
        {
            PanSpeed = Mathf.Max(PanSpeed, 0f);
            ZoomSpeed = Mathf.Max(ZoomSpeed, 0f);       

            MinZoom = Mathf.Max(MinZoom, 0.1f);
            MaxZoom = Mathf.Max(MaxZoom, 0.1f);

            if (MinZoom > MaxZoom) { MinZoom = MaxZoom; }

            DefaultZoom = Mathf.Clamp(DefaultZoom, MinZoom, MaxZoom);
        }
    }
}
