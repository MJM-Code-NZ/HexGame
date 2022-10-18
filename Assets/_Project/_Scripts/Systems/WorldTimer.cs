using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MJM.HG
{
    public class WorldTimer : Timer
    {
        public static event EventHandler<OnWorldTickArgs> OnWorldTick;

        public override void Update()
        {
            if (base.IsBaseTick())
            {
                OnWorldTick?.Invoke(this, new OnWorldTickArgs { WorldTick = _tickCount });
            }
        }
    }   
}
