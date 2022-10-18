using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MJM.HG
{   
    public class RenderTimer : Timer
    {
        public static event EventHandler<OnTickArgs> OnTick;

        public override void Update()
        {
            if (base.IsBaseTick())
            {
                OnTick?.Invoke(this, new OnTickArgs { Tick = _tickCount });
            }
        }
    }   
}
