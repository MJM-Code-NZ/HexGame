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

        public override bool TickCheck(bool forceTick)
        {
            if (base.TickCheck(forceTick))
            {
                OnTick?.Invoke(this, new OnTickArgs { Tick = _tickCount });

                return true;
            }
            else
            {
                return false;
            }

        }
    }   
}
