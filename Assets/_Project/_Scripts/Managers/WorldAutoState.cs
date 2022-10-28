using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MJM.HG
{
    

    public class WorldAutoState : WorldState
    {
        private const int _maxAutoCycle = 5;
        private const int _closeZoom = 4;
        private const float _autoDuration = 10f;

        public override void Awake()
        {
            base.Awake();

            _stateName = GameStateName.WorldAutoState;      
        }

        public override void Enter(GameStateName prevGameState)
        {
            if (!(prevGameState == GameStateName.WorldState))
            {
                _gmInstance.ProcessSceneLoad(GameManager.WorldScene);
            }
            else
            {
                Execute();
            }
        }

        public override void LoadSceneComplete(GameStateName prevGameState)
        {
            base.LoadSceneComplete(prevGameState);

            _worldUI.AutoShow(true);

            Execute();
        }

        // This is the extra "automation" logic for the world screen
        public override void Execute()
        {
            Debug.Log("Starting World Coroutine");
            StartCoroutine(AutoProcess());
        }

        public IEnumerator AutoProcess()
        {
            // find next pan position
            bool atPosition;
            
            Vector3 _worldPosition;
            Vector3 _cameraPosition;

            for (int autoCycle = 1; autoCycle <= _maxAutoCycle; autoCycle++)
            {
                _cameraPosition = CameraManager.Instance.Camera.transform.position;

                
                CameraManager.Instance.SetAutoZoom(_worldSize + 2, _autoDuration);

                yield return new WaitUntil(ZoomComplete);
                
                yield return new WaitForSeconds(5);

                do
                {
                    Tribe _selectedTribe = _gmInstance.WorldSystem.GetRandomTribe();

                    HexCoord _tribePosition = _selectedTribe.MapObjectList[0].Position;

                    _worldPosition = _gmInstance.WorldSystem.WorldRender.GridToWorld(HexCoordConversion.HexCoordToOffset(_tribePosition));

                    atPosition = Math.Round(_cameraPosition.x - _worldPosition.x, 1) == 0
                        && Math.Round(_cameraPosition.y - _worldPosition.y, 1) == 0;
                }
                while (_numberOfPlayers != 1 && atPosition);

                Debug.Log($"Pan Found {_worldPosition}");

                CameraManager.Instance.SetAutoPan(_worldPosition, _autoDuration);

                yield return new WaitUntil(PanComplete);

                Debug.Log($"Pan Complete {CameraManager.Instance.Camera.transform.position}");

                yield return new WaitForSeconds(5);

                CameraManager.Instance.SetAutoZoom(_closeZoom, _autoDuration);

                yield return new WaitUntil(ZoomComplete);

                yield return new WaitForSeconds(5);

                if (_worldUI.SpeedSlider.value > 0.2f)
                {
                    _worldUI.SpeedSlider.value -= 0.2f;
                }
            }

            CameraManager.Instance.SetAutoPan(Vector3.zero, _autoDuration);

            yield return new WaitUntil(PanComplete);

            CameraManager.Instance.SetAutoZoom(_worldSize + 2, _autoDuration);

            yield return new WaitUntil(ZoomComplete);

            yield return new WaitForSeconds(5);

            _worldUI.EscClick();

            yield return new WaitForSeconds(5);

            _gmInstance.HandleExitToMenuRequest(true);
        }

        private bool ZoomComplete() => !CameraManager.Instance.AutoZoomOn;
        private bool PanComplete() => !CameraManager.Instance.AutoPanOn;

        //public override void PauseRequest()
        //{
        //    _worldUI.PauseKeyPress();
        //}

        //public override void StepRequest()
        //{
        //    _worldUI.StepClick();
        //}

        //public override void EscapeRequest()
        //{
        //    _worldUI.EscKeyPress();
        //}

        public override void Exit(GameStateName nextGameStateName)
        {
            StopCoroutine("AutoProcess");

            CameraManager.Instance.DisableAuto();

            if (!(nextGameStateName == GameStateName.WorldState))
            {
                _gmInstance.ProcessSceneUnload(GameManager.WorldScene);

                TimeManager.Instance.ResetTimers();

                CameraManager.Instance.EnableCameraControls(false);

                CameraManager.Instance.Reset();

                GameManager.Instance.EnableGameflowControls(false);

                _gmInstance.WorldSystem.Quit();
                _gmInstance.EntitySystem.Quit();

                
            }            
        }
    }
}
