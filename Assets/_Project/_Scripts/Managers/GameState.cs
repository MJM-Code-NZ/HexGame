using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class GameState
    {
        protected GameManager _gmInstance;
        
        protected GameState()
        {
            _gmInstance = GameManager.Instance;
        }
        
        public virtual void Enter()
        {
            Execute();
        }

        public virtual void Execute()
        {

        }

        public virtual void LoadSceneComplete()
        {

        }

        public virtual void Exit()
        {

        }
    }
}
