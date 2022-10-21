using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    public class PlayerParameters : MonoBehaviour
    {
        [SerializeField]
        private int _spacePerPlayer = 5;
        public int SpacePerPlayer { get { return _spacePerPlayer; } }
        [SerializeField]
        private int _spaceBetweenPlayers = 1;
        public int SpaceBetweenPlayers { get { return _spaceBetweenPlayers; } }

        [Space()]
        [SerializeField]
        private bool _overrideMenuPlayers;
        public bool OverrideMenuPlayers { get { return _overrideMenuPlayers; } }

        //[SerializeField]
        //private int _player1XPosiiton = 0;
        //public int Player1XPosiiton { get { return _player1XPosiiton; } }
        //[SerializeField]
        //private int _player1YPosiiton = 0;
        //public int Player1YPosiiton { get { return _player1YPosiiton; } }

        [SerializeField]
        private List<int2> _playerPositionList;
        public List<int2> PlayerPositionList { get { return _playerPositionList; } }

    }
}
