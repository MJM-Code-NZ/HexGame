using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MJM.HG
{
    

    public class WorldState : GameState
    {       
        protected int _worldSize;
        protected int _numberOfPlayers;
        protected PlayerParameters _params;

        protected const string WorldUIGameObjectName = "WorldUI";
        protected WorldUI _worldUI;

        public override void Awake()
        {
            base.Awake();

            _worldSize = _gmInstance.WorldSize;
            _numberOfPlayers = _gmInstance.NumberOfPlayers;
            _params = _gmInstance.PlayerParameters;

            if (_params.OverrideMenuPlayers)
            {
                _numberOfPlayers = _params.PlayerPositionList.Count;
            }
        }
        
        public override void Enter(GameStateName prevGameStateName)
        {
            _stateName = GameStateName.WorldState;
            _sceneName = SceneName.WorldScene;

            bool _loadRequired;

            _loadRequired = !(prevGameStateName == GameStateName.WorldAutoState);

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

            Execute();
        }

        // This is intended to catch common logic that must be performed regardless of whether a scene was loaded or not but
        // that cannot be performed until after the scene has been loaded
        public override void PostAndNoLoadShared()
        {
            _worldUI = GameObject.Find(WorldUIGameObjectName).GetComponent<WorldUI>();
        }

        // This is intended to catch common logic that is performed post scene load by multiple game states.
        public virtual void PostLoadShared()
        {
            _gmInstance.EnergySystemConfigurer.ConfigureEnergySystem();

            World _world = _gmInstance.WorldSystem.Initialize(_worldSize, _numberOfPlayers);

            if (!_params.OverrideMenuPlayers)
            {
                List<int2> _playerPositionList = PlayerSystem.DeterminePlayerLocations(_world, _numberOfPlayers);

                _gmInstance.EntitySystem.Initialize(_world, _playerPositionList);
            }
            else
            {
                _gmInstance.EntitySystem.Initialize(_world, _params.PlayerPositionList);
            }

            CameraManager.Instance.EnableCameraControls(true);

            _gmInstance.EnableGameflowControls(true);

            TimeManager.Instance.StartWorldTime();
        }

        public override void Exit(GameStateName nextGameStateName)
        {
            bool _unloadRequired;

            _unloadRequired = !(nextGameStateName == GameStateName.WorldAutoState);

            if (_unloadRequired)
            {
                PostUnloadShared();
            }
        }

        // This is intended to catch common logic that is performed post scene unload by multiple game states.
        public virtual void PostUnloadShared()
        {
            _gmInstance.ProcessSceneUnload(_sceneName);

            TimeManager.Instance.ResetTimers();

            CameraManager.Instance.EnableCameraControls(false);

            CameraManager.Instance.Reset();

            GameManager.Instance.EnableGameflowControls(false);

            _gmInstance.WorldSystem.Quit();
            _gmInstance.EntitySystem.Quit();
        }

        public override void PauseRequest()
        {
            _worldUI.PauseKeyPress();
        }

        public override void StepRequest()
        {
            _worldUI.StepClick();
        }

        public override void EscapeRequest()
        {
            _worldUI.EscKeyPress();
        }
    }
}
