using System;
using UnityEngine;

namespace MJM.HG
{
    public static class GameInfo
    {
        public static float TickDuration = 0.2f; // this is timer stuff

        public static GameSettings GameSettings { get; private set; }

        public static void Init()
        {
            // using scriptable object for game parameters is convenient but may not be good longer term
            // as it is making model portion of the solution dependent on untiy editor
            
            GameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
        }
    }
}
