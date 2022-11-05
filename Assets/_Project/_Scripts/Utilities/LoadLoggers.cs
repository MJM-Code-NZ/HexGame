using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class LoadLoggers : MonoBehaviour // look into on create
    {
        private void Awake()
        {
            Logging.LoadLoggers();
        }
    }
}


