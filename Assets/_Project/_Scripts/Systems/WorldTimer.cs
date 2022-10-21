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

        public override bool TickCheck(bool forceTick)
        {
            if (base.TickCheck(forceTick))
            {
                OnWorldTick?.Invoke(this, new OnWorldTickArgs { WorldTick = _tickCount });

                return true;
            }
            else
            {
                return false;
            }
            
        }
    }   
}
