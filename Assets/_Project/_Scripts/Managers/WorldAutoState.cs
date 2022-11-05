using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MJM.HG
{
    

    public class WorldAutoState : WorldState
    {
        private const int _maxAutoCycle = 4;
        private const int _closeZoom = 4;
        private const float _autoDuration = 3f;

        private bool _autoStopBlock = false;
        public bool AutoStopBlock { get { return _autoStopBlock; } }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Enter(GameStateName prevGameStateName)
        {
            _stateName = GameStateName.WorldAutoState;
            _sceneName = SceneName.WorldScene;

            bool _loadRequired;

            _loadRequired = !(prevGameStateName == GameStateName.WorldState);

            if (_loadRequired)
            {
                _gmInstance.ProcessSceneLoad(_sceneName);
            }
            else
            {
                PostAndNoLoadShared();

                Execute();
            }
        }

        public override void LoadSceneComplete()
        {
            PostLoadShared();

            PostAndNoLoadShared(); 

            _worldUI.AutoShow(true);

            Execute();
        }

        // This is the extra "automation" logic for the world screen
        public override void Execute()
        {
            Logging.GeneralLogger.Log("Starting World Coroutine");
            StartCoroutine(AutoProcess());
        }

        public IEnumerator AutoProcess()
        {
            // find next pan position
            bool atPosition;
            
            Vector3 _worldPosition;
            Vector3 _cameraPosition;

            int _farZoom;

            _farZoom = _worldSize + 2;

            if (_farZoom < 6) _farZoom += 2;

            for (int autoCycle = 1; autoCycle <= _maxAutoCycle; autoCycle++)
            {
                _cameraPosition = CameraManager.Instance.Camera.transform.position;

                
                CameraManager.Instance.SetAutoZoom(_farZoom, _autoDuration);

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

                CameraManager.Instance.SetAutoPan(_worldPosition, _autoDuration);

                yield return new WaitUntil(PanComplete);

                yield return new WaitForSeconds(5);

                CameraManager.Instance.SetAutoZoom(_closeZoom, _autoDuration);

                yield return new WaitUntil(ZoomComplete);

                yield return new WaitForSeconds(5);

                if (_worldUI.SpeedSlider.value > 0.2f)
                {
                    _autoStopBlock = true; 
                    
                    _worldUI.SpeedSlider.value -= 0.2f;

                    _autoStopBlock = false;
                }
            }

            CameraManager.Instance.SetAutoPan(Vector3.zero, _autoDuration);

            yield return new WaitUntil(PanComplete);

            CameraManager.Instance.SetAutoZoom(_farZoom, _autoDuration);

            yield return new WaitUntil(ZoomComplete);

            yield return new WaitForSeconds(5);

            _autoStopBlock = true;
            
            _worldUI.EscClick();

            _autoStopBlock = false;

            yield return new WaitForSeconds(5);

            _gmInstance.HandleExitToMenuRequest(true);
        }

        private bool ZoomComplete() => !CameraManager.Instance.AutoZoomOn;
        private bool PanComplete() => !CameraManager.Instance.AutoPanOn;       

        public override void Exit(GameStateName nextGameStateName)
        {
            bool _unloadRequired;
            
            StopCoroutine("AutoProcess");

            CameraManager.Instance.DisableAuto();
            
            _unloadRequired = !(nextGameStateName == GameStateName.WorldState);

            if (_unloadRequired)
            {
                PostUnloadShared();
            }
        }
    }
}
