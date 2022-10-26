using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    
    public class MainMenuState : GameState
    {        
        public override void Awake() //WorldState(int worldSize, int numberOfPlayers) : base()
        {
            base.Awake();

            _stateName = GameStateName.MainMenuState;           
        }
        
        public override void Enter(GameStateName prevGameState)
        {
            if (!(prevGameState == GameStateName.MainMenuAutoState))
            {
                _gmInstance.ProcessSceneLoad(GameManager.MenuScene);
            }

            Execute();
        }
        
        public override void Exit(GameStateName nextGameState)
        {
            if (!(nextGameState == GameStateName.MainMenuAutoState))
            {
                _gmInstance.ProcessSceneUnload(GameManager.MenuScene);
            }
        }
    }
}
