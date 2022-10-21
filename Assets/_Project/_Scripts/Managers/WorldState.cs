using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MJM.HG
{
    

    public class WorldState : GameState
    {
        int _worldSize;
        int _numberOfPlayers;
        PlayerParameters _params;

        const string WorldUIGameObjectName = "WorldUI";
        WorldUI _worldUI;

        public WorldState(int worldSize, int numberOfPlayers) : base()
        {
            _worldSize = worldSize;
            _numberOfPlayers = numberOfPlayers;

            _params = GameManager.Instance.PlayerParameters;

            if (_params.OverrideMenuPlayers)
            {
                _numberOfPlayers = _params.PlayerPositionList.Count;
            }
        }
        
        public override void Execute()
        {
            _gmInstance.ProcessSceneLoad(GameManager.WorldScene);
        }

        public override void LoadSceneComplete()
        {
            _gmInstance.EnergySystemConfigurer.ConfigureEnergySystem();

            //PlayerParameters _params = GameManager.Instance.PlayerParameters;
            _worldUI = GameObject.Find(WorldUIGameObjectName).GetComponent<WorldUI>();

            World _world = _gmInstance.WorldSystem.Initialize(_worldSize, _numberOfPlayers);

            if (!_params.OverrideMenuPlayers)
            {
                List<int2> _playerPositionList = PlayerSystem.DeterminePlayerLocations(_world, _numberOfPlayers);

                _gmInstance.MapObjectSystem.Initialize(_world, _playerPositionList);
            }
            else
            {
                _gmInstance.MapObjectSystem.Initialize(_world, _params.PlayerPositionList);
            }

            CameraManager.Instance.EnableCameraControls(true);

            GameManager.Instance.EnableGameflowControls(true);

            TimeManager.Instance.StartWorldTime();
        }

        public override void PauseRequest()
        {
            _worldUI.SetPauseToggle(true);
        }

        public override void StepRequest()
        {
            _worldUI.StepClick();
        }

        public override void EscapeRequest()
        {
            Debug.Log("POP escape menu");
        }

        public override void Exit()
        {
            CameraManager.Instance.EnableCameraControls(false);

            GameManager.Instance.EnableGameflowControls(false);
        }
    }
}
