using System;
using UnityEngine;

namespace MJM.HG
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

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
 
        [Space()]
        [SerializeField]
        private int _player1XPosiiton = 0;
        public int Player1XPosiiton { get { return _player1XPosiiton; } }
        [SerializeField]
        private int _player1YPosiiton = 0;
        public int Player1YPosiiton { get { return _player1YPosiiton; } }

        // Awake is called on object creation
        void Awake()
        {
            EnforceSingleInstance();

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
            // GameInfo.Init();

            EnergySystemConfigurer.Init();
            WorldSystem.Init(_worldSize, _numberOfPlayers);
            MapObjectSystem.Init(_numberOfPlayers);  // more parameters to be added
        }
            
        void Update() 
        {
            
        }

        private void OnValidate()
        {
            _worldSize = Math.Max(_worldSize, 1);

            _numberOfPlayers = Math.Max(_numberOfPlayers, 1);           
        }

        void OnDisable()
        {
            WorldSystem.Quit();
            MapObjectSystem.Quit();
        }
    }
}
