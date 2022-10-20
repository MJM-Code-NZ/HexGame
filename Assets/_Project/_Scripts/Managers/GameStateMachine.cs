using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class GameStateMachine
    { 
        public GameState currentState;

        public void ChangeState(GameState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;

            currentState.Enter();
        }
    }
}
