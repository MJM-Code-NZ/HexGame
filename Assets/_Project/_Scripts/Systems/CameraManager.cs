using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MJM.HG
{
    public class CameraManager : MonoBehaviour
    {
        //private RenderSettings _renderSettings;
        
        private Camera _camera;

        //private float _panSpeed;
        //private float _zoomSpeed;

        private UserInputActions _userInputActions;

        private InputAction _panAction;
        private InputAction _zoomAction;

        [Header("Camera")]
        [SerializeField] private float PanSpeed = 8f;
        [SerializeField] private float ZoomSpeed = 8f;

        [Space()]
        [SerializeField] public float DefaultZoom = 6f;

        [Space()]
        [SerializeField] public float MinZoom = 2f;
        [SerializeField] public float MaxZoom = 20f;

        void Awake()
        {        
            _camera = Camera.main;
            _camera.transform.position = new Vector3(0.0f, 0.0f, -10);
            _camera.orthographicSize = DefaultZoom;

            _userInputActions = new UserInputActions();

            _panAction = _userInputActions.User.Pan;
            _zoomAction = _userInputActions.User.Zoom;
        }
         
        void OnEnable()
        {
            _panAction.Enable();
            _zoomAction.Enable();
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
            Vector3 panDisplacement = PanSpeed * panValue;

            _camera.transform.position = Vector3.Lerp(
                _camera.transform.position,
                _camera.transform.position + panDisplacement,
                Time.deltaTime
                );
        }

        private void UpdateZoom()
        {
            float zoomValue = _zoomAction.ReadValue<float>();
            float zoomDisplacement = ZoomSpeed * zoomValue;

            _camera.orthographicSize = Mathf.Lerp(
                _camera.orthographicSize,
                _camera.orthographicSize + zoomDisplacement,
                Time.deltaTime
                );

            _camera.orthographicSize = Mathf.Clamp(
                _camera.orthographicSize,
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

        void OnDisable()
        {
            _panAction.Disable();
            _zoomAction.Disable();
        }
    }
}
