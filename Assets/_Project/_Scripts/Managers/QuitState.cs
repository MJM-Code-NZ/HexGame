using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class QuitState : GameState
    {

        
        public override void Enter(GameStateName prevGameStateName)
        {
            _stateName = GameStateName.QuitState;

            Execute();
        }

        public override void Execute()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
            Application.Quit();
        }
    }
}
