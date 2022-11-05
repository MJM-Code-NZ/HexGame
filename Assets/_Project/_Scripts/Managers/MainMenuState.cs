using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    
    public class MainMenuState : GameState
    {               
        public override void Enter(GameStateName prevGameStateName)
        {      
            _stateName = GameStateName.MainMenuState;
            _sceneName = SceneName.MenuScene;

            bool _loadRequired;

            _loadRequired = !(prevGameStateName == GameStateName.MainMenuAutoState);

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
        
        public override void Exit(GameStateName nextGameStateName)
        {
            bool _unloadRequired;

            _unloadRequired = !(nextGameStateName == GameStateName.MainMenuAutoState);

            if (_unloadRequired)
            {
                _gmInstance.ProcessSceneUnload(_sceneName);
            }
        }
    }
}
