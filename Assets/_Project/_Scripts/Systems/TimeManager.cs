using System;
using UnityEngine;

namespace MJM.HG
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        // World tick timer is intended to primarily support gameplay functions
        // Tick timer is intended to primarily support rendering however currently all
        // rendering updates are event based so it actually does nothing

        [SerializeField]
        private float _tickDuration = 0.2f;
        public float TickDuration { get { return _tickDuration; } }

        [SerializeField]
        private float _worldTickDuration = 1f;
        public float WorldTickDuration { get { return _worldTickDuration; } }

        private float _minimumTickDuration = 0.02f; // will be used to prevent tick duration being configured less than Update() frequency

        private int _tick;
        private float _tickTime;
       
        private int _worldTick;
        private float _worldTickTime;

        Timer _renderTimer;
        Timer _worldTimer;

        void Awake()
        {
            EnforceSingleInstance();

            _renderTimer = this.gameObject.AddComponent<RenderTimer>();
            _worldTimer = this.gameObject.AddComponent<WorldTimer>();            
        }

        void Start()
        {
            _renderTimer.SetDuration(_tickDuration);
            _worldTimer.SetDuration(_worldTickDuration);
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
       
        void Update()
        {
            
        }

        private void OnValidate()
        {
            _tickDuration = Mathf.Max(_tickDuration, _minimumTickDuration);
            _worldTickDuration = Mathf.Max(_worldTickDuration, _tickDuration);
        }

        void OnDisable()
        {
            
        }
    }
}
