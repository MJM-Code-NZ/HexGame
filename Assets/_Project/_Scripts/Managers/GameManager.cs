using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace MJM.HG
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameStateMachine GameStateMachine { get; private set; }
        public PlayerParameters PlayerParameters { get; private set; }
        public EnergySystemConfigurer EnergySystemConfigurer { get; private set; }
        public WorldSystem WorldSystem { get; private set; }
        public EntitySystem EntitySystem { get; private set; }
        public PlayerInput PlayerInput { get; private set; }

        [Header("World Size")]
        [SerializeField]
        private int _worldSize = 5;
        public int WorldSize { get { return _worldSize; } }

        [Header("Players")]
        [SerializeField]
        private int _numberOfPlayers = 1;
        public int NumberOfPlayers { get { return _numberOfPlayers; } }

        // Constants for all scenes in build to help with game manager readability
        //public const string MainScene = "MainScene";
        //public const string MenuScene = "MenuScene";
        //public const string WorldScene = "WorldScene";

        void Awake()
        {
            EnforceSingleInstance();
        
            PlayerInput = GetComponent<PlayerInput>();
          
            PlayerParameters = GetComponent<PlayerParameters>();

            GameStateMachine = GetComponent<GameStateMachine>();

            EnergySystemConfigurer = GameObject.Find("Setup").GetComponent<EnergySystemConfigurer>();

            if (EnergySystemConfigurer == null)
            {
                EnergySystemConfigurer = new EnergySystemConfigurer();
            }

#if UNITY_EDITOR
            if (PlayerInput == null 
                || PlayerParameters == null
                || GetComponent<TimeManager>() == null
                || GetComponent<CameraManager>() == null)
            {
                Logging.GeneralLogger.LogError("Missing component(s) on Managers gameObject", this); // risk of logger not being loaded yet
            }
#endif

            WorldSystem = new WorldSystem();
            EntitySystem = new EntitySystem();          
        }

        private void EnforceSingleInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
#if UNITY_EDITOR
            // if game was run in editor mode with any scenes other than MainScene open close all other scenes before starting
            for (int i = 1; i < SceneManager.sceneCount; i++)
            {
                Scene _scene = SceneManager.GetSceneAt(i);
                
                if (_scene.isLoaded && _scene.name != SceneName.MainScene.ToString())
                {
                    ProcessSceneUnload((SceneName)Enum.Parse(typeof(SceneName), _scene.name));
                }
            }
#endif
            GameStateMachine.ChangeState(GameStateName.MainMenuState);
        }

        private void OnValidate()
        {
            _worldSize = Math.Max(_worldSize, 1);
            _numberOfPlayers = Math.Max(_numberOfPlayers, 1);
        }       

        public void ProcessSceneLoad(SceneName sceneName, GameStateName prevGameState = GameStateName.None)
        {
            StartCoroutine(LoadScene(sceneName)); //, prevGameState
        }

        public IEnumerator LoadScene(SceneName sceneName) //, GameStateName prevGameState
        {          
            if (SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                Logging.SceneLogger.LogWarning($"Attemptng to load scene that is already loaded: {sceneName}", this);               
                
            yield return SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);

            // These debug messages only reflect whether the relevant scene is now loaded and does not consider
            // whether an instance of the scene was already loaded or not
            if (SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                Logging.SceneLogger.Log($"Scene is now loaded: {sceneName}");
            else
                Logging.SceneLogger.LogWarning($"Scene load failed: {sceneName}", this);

            // Do not refactor the GetSceneByName call above the LoadScene call as LoadScene causes the scene in SceneManager to change
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName.ToString()));

            GameStateMachine.CurrentState.LoadSceneComplete(); //prevGameState
        }
        public void ProcessSceneUnload(SceneName sceneName)
        {
            StartCoroutine(UnloadScene(sceneName));
        }

        public IEnumerator UnloadScene(SceneName sceneName)
        {
            if (!SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                Logging.SceneLogger.LogWarning($"Attemptng to unload scene that is not loaded: {sceneName}", this);

            yield return SceneManager.UnloadSceneAsync(sceneName.ToString());

            // These debug messages only reflect whether the relevant scene is now unloaded and does not consider
            // whether an instance of the scene was already loaded or not
            if (!SceneManager.GetSceneByName(sceneName.ToString()).isLoaded)
                Logging.SceneLogger.Log($"Scene is now unloaded: {sceneName}");
            else
                Logging.SceneLogger.LogWarning($"Scene unload failed: {sceneName}", this);

            GameStateMachine.CurrentState.UnloadSceneComplete();
        }

        public void HandleNewGameRequest(int worldSize, int numberOfPlayers, bool auto = false)
        {
            _worldSize = worldSize;
            _numberOfPlayers = numberOfPlayers;

            if (auto)
            {
                GameStateMachine.ChangeState(GameStateName.WorldAutoState);
            }
            else
            {
                GameStateMachine.ChangeState(GameStateName.WorldState);
            }
        }

        public void HandleQuitGameRequest()
        {
            GameStateMachine.ChangeState(GameStateName.QuitState);
        }

        public void HandleExitToMenuRequest(bool auto = false)
        {
            if (auto)
            {
                GameStateMachine.ChangeState(GameStateName.MainMenuAutoState);
            }
            else
            {
                GameStateMachine.ChangeState(GameStateName.MainMenuState);
            }
        }

        public void HandleAutoPlayRequest(bool autoPlay)
        {
            if (autoPlay)
            {
                if (!(GameStateMachine.CurrentState.StateName == GameStateName.MainMenuAutoState))
                    GameStateMachine.ChangeState(GameStateName.MainMenuAutoState);
            }
            else
            {
                GameStateMachine.ChangeState(GameStateName.MainMenuState);
            }
        }

        public void HandleWorldAutoPlayOffRequest()
        {
            GameStateMachine.ChangeState(GameStateName.WorldState);
        }

        public void EnableGameflowControls(bool enable)
        {
            if (enable)
            {
                PlayerInput.actions.FindActionMap("Gameflow").Enable();
            }
            else
            {
                PlayerInput.actions.FindActionMap("Gameflow").Disable();
            }
        }

        public void EnableCameraControls(bool enable)
        {
            if (enable)
            {
                PlayerInput.actions.FindActionMap("Camera").Enable();
            }
            else
            {
                PlayerInput.actions.FindActionMap("Camera").Disable();
            }
        }

        private void OnPause()
        {
            GameStateMachine.CurrentState.PauseRequest();
        }

        private void OnStep()
        {
            GameStateMachine.CurrentState.StepRequest();
        }

        private void OnEscape()
        {
            GameStateMachine.CurrentState.EscapeRequest();
        }

        void OnDisable()
        {
           
        }
    }   
}
