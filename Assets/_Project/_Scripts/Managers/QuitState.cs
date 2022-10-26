using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class QuitState : GameState
    {
        public override void Awake() //WorldState(int worldSize, int numberOfPlayers) : base()
        {
            base.Awake();

            _stateName = GameStateName.QuitState;
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
