using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class GameStateMachine : MonoBehaviour
    {
        private static string _namespace = "MJM.HG.";
        
        private GameState _currentState;
        public GameState CurrentState { get { return _currentState; } }

        public void ChangeState(GameStateName newStateName)
        {
            GameStateName _oldStateName;

            if (_currentState != null)
            {
                _currentState.Exit(newStateName);
                _oldStateName = _currentState.StateName;
                Destroy(GetComponent(_namespace + _oldStateName.ToString()));
            }
            else
            {
                _oldStateName = GameStateName.None; 
            }

            _currentState = (GameState)gameObject.AddComponent(System.Type.GetType(_namespace + newStateName.ToString()));
            
            _currentState.Enter(_oldStateName);
        }
    }
}
