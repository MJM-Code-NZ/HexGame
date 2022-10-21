using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class GameStateMachine
    { 
        private GameState _currentState;
        public GameState CurrentState { get { return _currentState; } }

        public void ChangeState(GameState newState)
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = newState;

            _currentState.Enter();
        }
    }
}
