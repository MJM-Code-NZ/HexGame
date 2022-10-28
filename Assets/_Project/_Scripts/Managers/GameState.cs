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

        protected SceneName _sceneName = SceneName.None;
        public SceneName SceneName { get { return _sceneName; } }
        public virtual void Awake()
        {
            _gmInstance = GameManager.Instance;
        }
        
        public virtual void Enter(GameStateName prevGameStateName)
        {
            // set _stateName and _sceneName
            bool _loadRequired;
            // Add logic as needed
            _loadRequired = false;

            if (_loadRequired)
            {
                _gmInstance.ProcessSceneLoad(_sceneName);
            }
            else
            {
                // Add logic as needed
                PostAndNoLoadShared();
                // Add logic as needed
                Execute();
            }

        }
        
        //This is executed when the game state needs to load a new scene it is called on completion of the load scene coroutine
        public virtual void LoadSceneComplete()  // GameStateName prevGameState
        {
            // Add logic as needed
            PostAndNoLoadShared();
            // Add logic as needed
            Execute();
        }

        // This is intended to catch common logic that must be performed regardless of whether a scene was loaded or not but
        // that cannot be performed until after the scene has been loaded
        public virtual void PostAndNoLoadShared()
        {
            // Add logic as needed
        }

        public virtual void Execute()
        {

        }

        public virtual void Exit(GameStateName nextGameStateName)
        {
            bool _unloadRequired;

            // Add logic as needed
            _unloadRequired = false;

            if (_unloadRequired)
            {
                _gmInstance.ProcessSceneUnload(_sceneName);
            }
            else
            {
                // Add logic as needed
                PostSceneUnloadCommon();
            }
        }

        //This is executed when the game state needs to unload a new scene it is called on completion of the load scene coroutine
        public virtual void UnloadSceneComplete() 
        {
            // Add logic as needed
            PostSceneUnloadCommon();
            // Add logic as needed
        }

        // This is intended to catch common logic that must be performed regardless of whether a scene was unloaded or not but
        // that cannot be performed until after the scene has been unloaded
        public virtual void PostSceneUnloadCommon()
        {
            // Add logic as needed
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
    }
}
