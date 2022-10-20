using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class MainMenuState : GameState
    {
        public override void Exit()
        {
            _gmInstance.ProcessSceneUnload(GameManager.MenuScene);
        }
    }
}
