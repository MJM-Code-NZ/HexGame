using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MJM.HG
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameStateMachine StateMachine { get; private set; }
        public PlayerParameters PlayerParameters { get; private set; }
        public EnergySystemConfigurer EnergySystemConfigurer { get; private set; }
        public WorldSystem WorldSystem { get; private set; }
        public MapObjectSystem MapObjectSystem { get; private set; }

        [Header("World Size")]
        [SerializeField]
        private int _worldSize = 5;
        public int WorldSize { get { return _worldSize; } }

        [Header("Players")]
        [SerializeField]
        private int _numberOfPlayers = 1;
        public int NumberOfPlayers { get { return _numberOfPlayers; } }

        // Constants for all scenes in build to help with game manager readability
        public const string MainScene = "MainScene";
        public const string MenuScene = "MenuScene";
        public const string WorldScene = "WorldScene";

        void Awake()
        {
            EnforceSingleInstance();

            PlayerParameters = GetComponent<PlayerParameters>();

            StateMachine = new GameStateMachine();

            EnergySystemConfigurer = GameObject.Find("Setup").GetComponent<EnergySystemConfigurer>();

            if (EnergySystemConfigurer == null)
            {
                EnergySystemConfigurer = new EnergySystemConfigurer();
            }

            WorldSystem = new WorldSystem();
            MapObjectSystem = new MapObjectSystem();
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
            StateMachine.ChangeState(new StartingState());
        }

        private void OnValidate()
        {
            _worldSize = Math.Max(_worldSize, 1);
            _numberOfPlayers = Math.Max(_numberOfPlayers, 1);
        }

        public void ProcessSceneLoad(string sceneName)
        {
            StartCoroutine(LoadScene(sceneName));
        }

        public IEnumerator LoadScene(string sceneName)
        {          
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
                Debug.Log($"Attemptng to load scene that is already loaded: {sceneName}");               
                
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // These debug messages only reflect whether the relevant scene is now loaded and does not consider
            // whether an instance of the scene was already loaded or not
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
                Debug.Log($"Scene is now loaded: {sceneName}");
            else
                Debug.Log($"Scene load failed: {sceneName}");

            // Do not refactor the GetSceneByName call above the LoadScene call as LoadScene causes the scens in SceneManager to change
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            StateMachine.currentState.LoadSceneComplete();
        }
        public void ProcessSceneUnload(string sceneName)
        {
            StartCoroutine(UnloadScene(sceneName));
        }

        public IEnumerator UnloadScene(string sceneName)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                Debug.Log($"Attemptng to unload scene that is not loaded: {sceneName}");

            yield return SceneManager.UnloadSceneAsync(sceneName);

            // These debug messages only reflect whether the relevant scene is now unloaded and does not consider
            // whether an instance of the scene was already loaded or not
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)           
                Debug.Log($"Scene is now unloaded: {sceneName}");
            else           
                Debug.Log($"Scene unload failed: {sceneName}");
        }

        public void HandleNewGameRequest(int worldSize, int numberOfPlayers)
        {
            StateMachine.ChangeState(new WorldState(worldSize, numberOfPlayers));
        }

        public void HandleQuitGameRequest()
        {
            StateMachine.ChangeState(new QuitState());
        }

        void OnDisable()
        {
           //WorldSystem.Quit();
           //MapObjectSystem.Quit();
        }
    }   
}
