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

        public WorldState(int worldSize, int numberOfPlayers) : base()
        {
            _worldSize = worldSize;
            _numberOfPlayers = numberOfPlayers;
        }
        
        public override void Execute()
        {
            _gmInstance.ProcessSceneLoad(GameManager.WorldScene);
        }

        public override void LoadSceneComplete()
        {
            _gmInstance.EnergySystemConfigurer.ConfigureEnergySystem();
            World _world = _gmInstance.WorldSystem.Initialize(_worldSize, _numberOfPlayers);

            List<int2> _playerPositionList = PlayerSystem.DeterminePlayerLocations(_world, _numberOfPlayers);

            _gmInstance.MapObjectSystem.Initialize(_world, _playerPositionList);
        }
    }
}
