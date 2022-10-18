using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MJM.HG
{
    public class Timer : MonoBehaviour
    {
        // Register Event in subclass

        // Tick count is serialized to support custom TimerEditor which provides read only view of timer
        [SerializeField] protected int _tickCount;
        public int TickCount { get { return _tickCount; } }
        
        protected float _tickDuration;
        protected float _tickTimer;
        

        void Awake()
        {
            _tickCount = 0;
            _tickTimer = 0f;
        }

        public void SetDuration(float tickDuration)
        {
            _tickDuration = tickDuration;
        }

        public virtual void Update()
        {
            // Call IsBaseTick and if true Invoke Event in subclass
        }

        public bool IsBaseTick()
        {
            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tickDuration)
            {
                _tickCount++;

                _tickTimer -= _tickDuration;

                return true;
            }

            return false;
        }
    }   
}
