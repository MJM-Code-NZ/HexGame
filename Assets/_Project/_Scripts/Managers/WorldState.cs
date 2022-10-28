using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MJM.HG
{
    

    public class WorldState : GameState
    {       
        // Still considering whether storing these 3 variables is a good idea versus looking them up from GameManager when needed
        protected int _worldSize;
        protected int _numberOfPlayers;
        protected PlayerParameters _params;

        protected const string WorldUIGameObjectName = "WorldUI";
        protected WorldUI _worldUI;

        public override void Awake()
        {
            base.Awake();

            _stateName = GameStateName.WorldState;
            _worldSize = _gmInstance.WorldSize;
            _numberOfPlayers = _gmInstance.NumberOfPlayers;
            _params = _gmInstance.PlayerParameters;

            if (_params.OverrideMenuPlayers)
            {
                _numberOfPlayers = _params.PlayerPositionList.Count;
            }
        }
        
        public override void Enter(GameStateName prevGameState)
        {
            if (!(prevGameState == GameStateName.WorldAutoState))
            {
                _gmInstance.ProcessSceneLoad(GameManager.WorldScene);
            }
            else
            {
                _worldUI = GameObject.Find(WorldUIGameObjectName).GetComponent<WorldUI>();

                Execute();
            }
        }

        public override void LoadSceneComplete(GameStateName prevGameState)
        {
            _gmInstance.EnergySystemConfigurer.ConfigureEnergySystem();

            _worldUI = GameObject.Find(WorldUIGameObjectName).GetComponent<WorldUI>();

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

            // This is a ugly work around - because the WorldStateAuto version of LoadSceneComplete calls this method as its base.
            // This is necessary to prevent WorldStateAuto from running execute twice.
            if (_gmInstance.GameStateMachine.CurrentState.StateName == GameStateName.WorldState)
                Execute();
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

        public override void Exit(GameStateName nextGameStateName)
        {
            if (!(nextGameStateName == GameStateName.WorldAutoState))
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
