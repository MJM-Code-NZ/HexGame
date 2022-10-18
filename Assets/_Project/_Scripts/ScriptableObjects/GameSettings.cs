using System;
using UnityEngine;

namespace MJM.HG
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("World Size")]
        public int WorldSize = 5;  // will be removed

        [Header("Initial Players")]
        public int NumberOfPlayers = 1; // will be removed

        [Space()]
        public int Player1XPosiiton = 0;
        public int Player1YPosiiton = 0;
    }
}

