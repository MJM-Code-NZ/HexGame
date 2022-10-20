using System;
using System.Collections;
using System.Collections.Generic;
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
            _gmInstance.EnergySystemConfigurer.Init();
            _gmInstance.WorldSystem.Init(_worldSize, _numberOfPlayers);
            _gmInstance.MapObjectSystem.Init(_gmInstance.NumberOfPlayers);
        }
    }
}
