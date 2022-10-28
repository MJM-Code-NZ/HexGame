using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class GameState : MonoBehaviour
    {
        protected GameManager _gmInstance;

        protected GameStateName _stateName = GameStateName.None;
        public GameStateName StateName { get { return _stateName; } }

        //protected GameState()
        //{
        //    _gmInstance = GameManager.Instance;
        //}

        public virtual void Awake()
        {
            _gmInstance = GameManager.Instance;
        }
        
        public virtual void Enter(GameStateName prevGameStateName)
        {
            Execute();
        }

        public virtual void Execute()
        {

        }

        public virtual void LoadSceneComplete(GameStateName prevGameState)
        {
            Execute();
        }

        public virtual void PauseRequest()
        {

        }

        public virtual void StepRequest()
        {

        }

        public virtual void EscapeRequest()
        {

        }

        public virtual void Exit(GameStateName nextGameStateName)
        {

        }
    }
}
