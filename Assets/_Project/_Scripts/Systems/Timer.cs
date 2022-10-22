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
        
        protected bool _timerActive;
        public bool TimerActive { get { return _timerActive; } }
        

        void Start()
        {
            _tickCount = 0;
            _tickTimer = 0f;
            _timerActive = false;
        }

        public void SetTimer(float tickDuration, bool activeFlag)
        {
            _tickDuration = tickDuration;
            _timerActive = activeFlag;
        }
        public virtual void StartTimer()
        {
            _timerActive = true;
        }

        public virtual void StopTimer()
        {
            _timerActive = false;
        }

        public virtual void Update()
        {
            TickCheck(false);
        }

        public virtual bool TickCheck(bool forceTick)  // Implement Invoke Event in subclasses
        {           
            if (forceTick) // if a forced tick is requested respond with a tick without doing anything with Time or duration
            {
                _tickCount++;
                return true;
            }

            if (!_timerActive) return false;  // do nothing of timer is off

            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tickDuration)
            {
                _tickCount++;
                _tickTimer -= _tickDuration;
                return true;
            }
            return false;
        }

        public virtual void ForceTick()
        {
            TickCheck(true);
        }

        public virtual void ChangeDuration(float duration)
        {
            _tickDuration = duration;
        }

        public virtual void ResetTimer()  
        {
            _tickCount = 0;
            _tickTimer = 0f;
            _timerActive = false;
        }
    }   
}
