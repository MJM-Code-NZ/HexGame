using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class MainMenuState : GameState
    {
        public override void Execute()
        {
            _gmInstance.ProcessSceneLoad(GameManager.MenuScene);
        }
        
        public override void Exit()
        {
            _gmInstance.ProcessSceneUnload(GameManager.MenuScene);
        }
    }
}
